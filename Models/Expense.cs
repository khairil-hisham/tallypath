namespace Tallypath.Models
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreatorId { get; set; } = default;
        public Guid GroupId { get; set; } = default!;

        public Group Group { get; set; } = default!;
        public User Creator { get; set; } = default!;

        public string Title { get; set; } = string.Empty;
        public long Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}