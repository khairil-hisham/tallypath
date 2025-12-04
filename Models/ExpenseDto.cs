namespace Tallypath.Data
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; } = default!;
        public string Content { get; set; } = string.Empty;

    }
}