namespace Tallypath.Models
{
    public class Savings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title = default!;
        public long Target = default!;
        public long Current = default!;
        public string Deadline = default!;
    }

    public class Contribution
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Note { get; set; } = default!;
        public long Amount { get; set; } = default!;
        public string Date { get; set; } = default!;
    }


}