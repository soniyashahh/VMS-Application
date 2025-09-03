using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMSApplication.Models
{
    public class VisitorRegistration:UserActivity
    {
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Display(Name = "Gender")]
        public string Gender { get; set; }


        [Display(Name = "E-mail Address")]
        public string EmailId { get; set; }


        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }


        [Display(Name = "Visitor Image")]
        public string VisitorImg { get; set; } = null!;

        [Display(Name = "From-Time")]
        public DateTime FromTime { get; set; }

        [Display(Name = "To-Time")]
        public DateTime ToTime { get; set; }
        [Required(ErrorMessage = "Visitor Type is required.")]
        [Display(Name = "Visitor Type")]
        public int VisitortypeId { get; set; }

        [Display(Name = "Visitor Type")]
        public VisitorType VisitorType { get; set; }
        [Required(ErrorMessage = "Visitor Purpose is required.")]
        [Display(Name = "Visitor Purpose")]
        public int visitorpurposeId { get; set; }

        [Display(Name = "Visitor Purpose")]
        public VisitorPurpose VisitorPurpose { get; set; }

        [Display(Name = " Aadhar Image")]
        public string UploadId { get; set; } = null!;

        [NotMapped]
        [Display(Name = " Aadhar Image")]
        public IFormFile Aadharupload { get; set; }

        [NotMapped]
        [Display(Name = "Visitor Image")]
        public IFormFile VisitorImgFile { get; set; }

        [Required(ErrorMessage = "Aadhar Number is required.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Aadhar Number must be exactly 12 digits.")]
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Aadhar Number must be exactly 12 digits.")]
        [Display(Name = "Aadhar Number")]
        public string IDNumber { get; set; }

        [Display(Name = "Items Details")]
        public string ItemsDetails { get; set; }

        [Display(Name = "Visitor Location")]
        public string VisitorLocation { get; set; }

        [Display(Name = "Vehicle Registration No")]
        public string VehicleRegistrationNo { get; set; }

        [Display(Name = "Driving Licence No")]
        public string DrivingLicenceNo { get; set; }
        [Required(ErrorMessage = "Host Details is required.")]
        public string userId { get; set; }
        public ApplicationUser user { get; set; }

        public int StatusId { get; set; }
        public SystemCode Status { get; set; }

        [NotMapped]
        public string HostEmail { get; set; }  

        [DisplayName("Company Name")]
        [Required(ErrorMessage = "Company is required.")]
        public int? CompanyId { get; set; }
        public Company company { get; set; }

        public string? CHALicense { get; set; }
        public string? CHACompany { get; set; }

        [NotMapped] // Prevents it from being saved in the database
        public IFormFile? CHADocument { get; set; }

        public string? CHAFilePath { get; set; }

    }

    public class VisitorRegistrationGroupViewModel
    {
        public List<VisitorRegistration> Visitors { get; set; } = new List<VisitorRegistration>();
    }

}
