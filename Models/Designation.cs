using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class Designation 
    {
        public int Id { get; set; }
        [Display(Name = "Designation Name")]
        public string DesignationName { get; set; }
    }
}
