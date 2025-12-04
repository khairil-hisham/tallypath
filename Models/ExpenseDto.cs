namespace Tallypath.Data
{
    public class CreateExpenseDto
    {
        public Guid GroupId { get; set; } = Guid.Empty;
        public string Content { get; set; } = string.Empty;

    }
}