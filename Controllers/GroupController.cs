

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
            Members = new List<GroupMember>()
        };

        // 4. Add group members
        foreach (var userId in request.MemberIds)
        {
            group.Members.Add(new GroupMember
            {
                UserId = userId
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
            Members = group.Members.Select(m => m.UserId).ToList()
        });
    }
}
