//using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VMSApplication.Models;
//using System.ComponentModel.DataAnnotations;

namespace VMSApplication.UserViewModel
{
	public class UserModel
	{
        public string Id { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string? MiddleName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [DisplayName("User Password")]
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [Display(Name = "Password")]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [DisplayName("Nationality")]
        public string? NationalId { get; set; }
        public string? FullName => $"{FirstName}{MiddleName}{LastName}";
        [DisplayName("Role")]
        public string? RoleId { get; set; }

      
        [DisplayName("CompanyName")]
        public int ? CompanyId { get; set; }

        [DisplayName("Department")]
        public int? DepartmentId { get; set; }

        [DisplayName("Designation")]
        public int? DesignationId { get; set; }


    }
}

