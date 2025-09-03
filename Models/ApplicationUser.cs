using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMSApplication.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string? FirstName {  get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? NationId {  get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public string? FullName => $"{FirstName} {MiddleName} { LastName}";
        public DateTime LoginDate { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public string? RoleId { get; set; }
        public IdentityRole Role { get; set; }
        public int? CompanyId {  get; set; }
        public Company company { get; set; }
        public int? DesignationId { get; set; }
        public Designation designation { get; set; }
        public int? DepartmentId { get; set; }
        public Department department { get; set; }

    }
}
