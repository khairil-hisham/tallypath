namespace Tallypath.Models
{
    public class Expense
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public Guid GroupId { get; set; }

        public Group Group { get; set; } = default!;
        public User Creator { get; set; } = default!;

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}