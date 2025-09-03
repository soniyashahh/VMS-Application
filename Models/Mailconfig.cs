using System.Security.Policy;

namespace VMSApplication.Models
{
    public class Mailconfig : UserActivity
    {
        public int Id { get; set; }
        public string mailId { get; set; }
        public int? CompanyId { get; set; }
        public Company company { get; set; }
    }
}
