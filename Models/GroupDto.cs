namespace Tallypath.Data
{
    public class CreateGroupRequest
    {
        public string Name { get; set; } = "";
        public List<Guid> MemberIds { get; set; } = new();
    }
    public class JoinGroupRequest
    {
        public string JoinCode { get; set; } = default!;
    }
}