using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class Department : UserActivity
    {
        public int DepartmentID {  get; set; }
        [Display(Name = "Department Name")]
        public string DepqartmentName { get; set; }
    }
}
