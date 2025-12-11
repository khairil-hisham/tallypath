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
        public long Total { get; set; } = 0;

        public MembershipDto Membership { get; set; } = default!;
    }

    public class MembershipDto
    {
        public Guid MemberId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsAdmin { get; set; }
    }

}