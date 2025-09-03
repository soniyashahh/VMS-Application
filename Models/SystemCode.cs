using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class SystemCode
    {
        public int Id {  get; set; }
        [Display(Name = "Status")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
