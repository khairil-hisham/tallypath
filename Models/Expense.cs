namespace Tallypath.Models
{
    public class Expense
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CreatorId { get; set; } = default!;
        public Guid GroupId { get; set; } = default!;

        public Group Group { get; set; } = default!;
        public User Creator { get; set; } = default!;

        public string Title { get; set; } = string.Empty;
        public long Amount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ExpenseSplit> Splits{ get; set; } = new List<ExpenseSplit>();
        public Guid PaidBy { get; set; } = default!;
    }
    public class ExpenseSplit
    {
        public Guid ExpenseId { get; set; }
        public Guid UserId { get; set; }
        public long Share { get; set; }
    }

    public class UserBalance
    {
        public Guid UserId { get; set; }
        public long NetBalance { get; set; }
    }

}