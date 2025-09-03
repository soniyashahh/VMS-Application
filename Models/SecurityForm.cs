using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMSApplication.Models
{
    public class SecurityForm : UserActivity
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int VisitorID { get; set; }
        public VisitorRegistration Visitor { get; set; }
        public string status { get; set; }
        [NotMapped]
        public string visitorName { get; set; }
        [NotMapped] 
        public string visitorAddress { get; set; }
        [NotMapped]
        public string visitorPurpose { get; set; }

        public int KPassId { get; set; }

    }
}
