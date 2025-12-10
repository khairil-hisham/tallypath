namespace Tallypath.Data
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; } = Guid.Empty;
        public string Title { get; set; } = string.Empty;
        public long Amount { get; set; }

    }
}