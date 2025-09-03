namespace VMSApplication.Models
{
    public class UserActivity
    {
        public string? CreatedId { get;  set; }
        public DateTime? createdOn { get; set; }
        public string? ModifiedId { get; set; }
        public DateOnly? ModifiedOn { get; set; }
    }

    public class ApprovalActivity : UserActivity
    {

        public string? ActivityId { get; set; }
        public DateTime? ApprovedOn { get; set; }
    }
}
