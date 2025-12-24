namespace Tallypath.Data
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public long Amount { get; set; } = 0;

        public List<ExpenseSplitDto> Splits { get; set; } = [];
        public Guid PaidBy { get; set; } = Guid.Empty;
        public bool IsMessage { get; set; } = false;

    }
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        public decimal Amount { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<ExpenseSplitDto> Splits { get; set; } = [];
        public Guid PaidBy { get; set; }
        public bool IsMessage { get; set; }
    }

    public class ExpenseSplitDto
    {
        public Guid UserId { get; set; }
        public long Share { get; set; }
    }

    public class BalanceDto
    {
        public List<Debt> Debts = [];
    }

    public class Debt
    {
        public Guid Debtor { get; set; }
        public Guid Creditor { get; set; }
        public long Amount { get; set; }
    }
}