namespace Tallypath.Models
{
    public class Group
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;
        public long Total { get; set; } = 0;
        public DateTime? CreatedAt { get; set; }

        public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }


    public class GroupMember
    {
        public Guid Id { get; set; } = default!;

        public Guid GroupId { get; set; } = default!;
        public Group Group { get; set; } = default!;

        public Guid UserId { get; set; }
        public User User { get; set; } = default!;
        public string NameInGroup { get; set; } = default!;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public bool IsAdmin { get; set; } = false;
    }

    public class GroupInvite
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid GroupId { get; set; } = default!;
        public Group Group { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
        public int MaxUses { get; set; } = 1;  // optional
        public int Uses { get; set; } = 0;

        public bool IsRevoked { get; set; } = false;
    }

    public class GroupUnread
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public long UnreadCount { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }


}
