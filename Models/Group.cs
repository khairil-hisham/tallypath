namespace Tallypath.Models
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;

        public string? JoinCode { get; set; }

        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }

    public class GroupMember
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
    }

    public class Expense
    {
        public int Id { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; } = default!;

        public Guid CreatorId { get; set; }
        public User Creator { get; set; } = default!;

        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
