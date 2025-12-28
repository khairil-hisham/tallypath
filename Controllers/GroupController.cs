using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tallypath.Data;
using Tallypath.Models;

namespace Tallypath.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("{groupId}/invites")]
        public async Task<IActionResult> CreateInvite(Guid groupId)
        {
            var userId = User.GetUserId();

            var membership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (membership == null || !membership.IsAdmin)
                return Forbid();

            var invite = new GroupInvite
            {
                GroupId = groupId,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                MaxUses = 5
            };

            _context.GroupInvites.Add(invite);
            await _context.SaveChangesAsync();

            var link = $"https://tallypath.my/invite?token={invite.Id}";

            return Ok(new
            {
                inviteId = invite.Id,
                deepLink = link,
                expiresAt = invite.ExpiresAt
            });
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
            };


            // 4. Add other group members
            foreach (var user in users)
            {
                group.Members.Add(new GroupMember
                {
                    UserId = user.Id,
                    IsAdmin = (user.Id == request.MemberIds[0]) ? true : false, //first ID is admin
                    NameInGroup = user.Fullname
                });
            }

            // 5. Save to DB
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            //create an invite code
            var invite = new GroupInvite
            {
                GroupId = group.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                MaxUses = 5
            };

            _context.GroupInvites.Add(invite);
            await _context.SaveChangesAsync();

            var link = $"https://tallypath.my/invite?token={invite.Id}";

            // 6. Return new group info
            return Ok(new
            {
                group.Id,
                group.Name,
                Members = group.Members.Select(m => m.UserId).ToList(),
                inviteCode = invite.Id,
                deepLink = link,
                expiresAt = invite.ExpiresAt,
                NameInGroup = group.Members.Select(m => m.NameInGroup).ToList(),
            });
        }


        [Authorize]
        [HttpPost("join/{inviteId}")]
        public async Task<IActionResult> JoinGroupViaInvite(Guid inviteId)
        {
            var userId = User.GetUserId();

            var invite = await _context.GroupInvites
                .Include(i => i.Group)
                .ThenInclude(g => g.Members)
                .FirstOrDefaultAsync(i => i.Id == inviteId);

            if (invite == null)
                return NotFound("Invite does not exist.");

            if (invite.IsRevoked)
                return BadRequest("This invite has been revoked.");

            if (invite.Uses >= invite.MaxUses)
                return BadRequest("This invite has reached max uses.");

            if (invite.ExpiresAt < DateTime.UtcNow)
                return BadRequest("This invite has expired.");

            if (invite.Group.Members.Any(m => m.UserId == userId))
                return BadRequest("You are already a member of this group.");

            invite.Group.Members.Add(new GroupMember
            {
                GroupId = invite.GroupId,
                UserId = userId,
                NameInGroup = User.GetName()
            });

            invite.Uses += 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Joined group successfully",
                groupId = invite.GroupId,
                groupName = invite.Group.Name,
                nameInGroup = User.GetName()
            });
        }

        [Authorize]
        [HttpPost("{groupId}/invites/{inviteId}/revoke")]
        public async Task<IActionResult> RevokeInvite(Guid groupId, Guid inviteId)
        {
            var userId = User.GetUserId();

            var membership = await _context.GroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (membership == null || !membership.IsAdmin)
                return Forbid();

            var invite = await _context.GroupInvites.FindAsync(inviteId);

            if (invite == null)
                return NotFound();

            invite.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok("Invite revoked.");
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<GroupWithMembersDto>>> GetGroupsForUser()
        {
            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == User.GetUserId())
                .Include(gm => gm.Group)
                .Include(gm => gm.Group.Members)
                .OrderBy(gm => gm.JoinedAt)
                .Select(gm => new GroupWithMembersDto
                {
                    GroupId = gm.Group.Id,
                    Name = gm.Group.Name,
                    Total = gm.Group.Total,
                    Members = gm.Group.Members.ToList(),

                })
                .ToListAsync();

            return Ok(groups);
        }


        [HttpGet("{groupId}/details")]
        public async Task<IActionResult> GetGroupDetails(Guid groupId)
        {
            var details = await _context.Groups
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (details == null)
                return NotFound("Group Id not found");

            var count = await _context.GroupMembers
                .CountAsync(gm => gm.GroupId == groupId);

            return Ok(new
            {
                name = details.Name,
                memberCount = count,
                createdAt = details.CreatedAt
            });

        }

    }


}