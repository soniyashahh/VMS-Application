using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class VisitorType
    {
        public int Id {  get; set; }
        [Display(Name = "Visitor Type")]
        public string? Visitortype { get; set; }
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}
