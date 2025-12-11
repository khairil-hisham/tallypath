namespace Tallypath.Data
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = "";
        public List<Guid> MemberIds { get; set; } = new();
    }

    public class GroupWithMembershipDto
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = default!;
        public bool Personal { get; set; } = false;

        public MembershipDto Membership { get; set; } = default!;
    }

    public class MembershipDto
    {
        public Guid MemberId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }

}