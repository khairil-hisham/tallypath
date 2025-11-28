
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tallypath.Data;
using Tallypath.Models;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public GroupsController(AppDbContext context)
    {
        _context = context;
    }

    private string GenerateRandomCode(int length = 6)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    // ADMIN : Generate join code
    [Authorize]
    [HttpPost("{groupId}/generate-code")]
    public async Task<IActionResult> GenerateJoinCode(int groupId)
    {
        var userId = User.GetUserId();

        var membership = await _context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

        if (membership == null)
            return Unauthorized("You are not in this group.");

        if (!membership.IsAdmin)
            return Forbid("Only admins can generate join codes.");

        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            return NotFound();

        group.JoinCode = GenerateRandomCode();
        await _context.SaveChangesAsync();

        return Ok(new { group.Id, group.Name, group.JoinCode});
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        // 1. Validate group name
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Group name is required.");

        // 2. Make sure members exist
        var users = await _context.Users
            .Where(u => request.MemberIds.Contains(u.Id))
            .ToListAsync();

        if (users.Count != request.MemberIds.Count)
            return BadRequest("One or more user IDs are invalid.");

        // 3. Create group
        var group = new Group
        {
            Name = request.Name,
            Members = new List<GroupMember>(),
            JoinCode = GenerateRandomCode()
        };

        // 4. Add other group members
        foreach (var userId in request.MemberIds)
        {
            group.Members.Add(new GroupMember
            {
                UserId = userId,
                IsAdmin = (userId == request.MemberIds[0]) ? true : false //first ID is admin
            });
        }

        // 5. Save to DB
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        // 6. Return new group info
        return Ok(new
        {
            group.Id,
            group.Name,
            Members = group.Members.Select(m => m.UserId).ToList(),
            group.JoinCode,
        });
    }
}
