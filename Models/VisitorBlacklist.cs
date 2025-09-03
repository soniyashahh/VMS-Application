using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class VisitorBlacklist:UserActivity
    {
        public int Id { get; set; }

        [Display(Name = "Reason")]
        public string Description { get; set; }

        [Display(Name = "Visitor ID")]
        public int visitorId {  get; set; }
        public VisitorRegistration visitor { get; set; }
    }
}
