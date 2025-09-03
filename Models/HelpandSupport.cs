using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class HelpandSupport : UserActivity
    {
        public int Id { get; set; }
        [Display(Name ="Remarks")]
        public string Facingissues { get; set; }
    }
}
