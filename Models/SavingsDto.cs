namespace Tallypath.Data
{
    public class CreatePlanRequest
    {
        public string Title { get; set; } = default!;
        public long Target { get; set; } = default!;
        public DateTime? Due { get; set; }
        public bool HasReminder { get; set; } = false;
        public string Reminder { get; set; } = default!;
    }

    public class CreateContributionRequest
    {
        public string Note { get; set; } = default!;
        public long Amount { get; set; } = default!;
    }

}