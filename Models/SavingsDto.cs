namespace Tallypath.Data
{
    public class CreatePlanRequest
    {
        public string Title { get; set; } = default!;
        public long Target { get; set; } = default!;
        public DateTime? Due { get; set; }
    }
}