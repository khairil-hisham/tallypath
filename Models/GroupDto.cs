using Tallypath.Models;

namespace Tallypath.Data
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = "";
        public List<Guid> MemberIds { get; set; } = new();
    }

    public class GroupWithMembersDto
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = default!;
        public long Total { get; set; } = 0;

        public List<GroupMember> Members { get; set; } = [];
    }


}