using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class VisitorPurpose:UserActivity
    {
        public int Id { get; set; }
        [Display(Name ="Purpose Name")]
        [Required]
        public string? PurposeName { get; set; }
        
        public string? Description { get; set; }
    }
}
