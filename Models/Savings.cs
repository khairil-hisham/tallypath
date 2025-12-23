namespace Tallypath.Models
{
    public class Savings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public long Target { get; set; } = default!;
        public long Current { get; set; } = 0;
        public DateTime? Due { get; set; }
        public DateTime CreatedAt { get; set; } 
    }

    public class Contribution
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Note { get; set; } = default!;
        public long Amount { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = default!;
    }


}