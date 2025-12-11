namespace Tallypath.Data
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public long Amount { get; set; }

    }
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}