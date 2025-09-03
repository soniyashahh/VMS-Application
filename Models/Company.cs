using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class Company : UserActivity
    {

        public int CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string Location { get; set; }
    }
}
