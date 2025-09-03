using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VMSApplication.Models
{
    public class RRequest
    {
        public int id { get; set; }
        [Required(ErrorMessage = "From-Date is required.")]
        public DateTime Fromdate { get; set; }
        [Required(ErrorMessage = "To-Date is required.")]
        public DateTime Todate { get; set; }
        [Required]
        public int visitorId { get; set; }
        public VisitorRegistration visitor { get; set; }
        [Required(ErrorMessage = "Vehicle Number/NA is required.")]
        public string VehicleNo { get; set; }
        [Required(ErrorMessage = "Items/NA is required.")]
        public string? ItemsDetails { get; set; }
        [Required(ErrorMessage = "Host Details is required.")]
        public string userId { get; set; }
        public ApplicationUser user { get; set; }
        public int StatusId { get; set; }
        public SystemCode Status { get; set; }
        [NotMapped]
        public string HostEmail { get; set; }
        [NotMapped]
        public string VisitorEmail { get; set; }

        [Display(Name = "Visitor Purpose")]
        [Required(ErrorMessage = "Purpose is required.")]
        public int visitorpurposeId { get; set; }

        [Display(Name = "Visitor Purpose")]
        public VisitorPurpose VisitorPurpose { get; set; }
        [Required(ErrorMessage = "Company is required.")]
        public int CompanyId { get; set; }
        public Company company { get; set; }

    }
}
