using System.ComponentModel.DataAnnotations;
using VMSApplication.Models;

public class VisitorRegistrationViewModel
{
    // Shared fields (Company Details, Host Details)
    [Required(ErrorMessage = "Company is required.")]
    public int? CompanyId { get; set; }

    public string CompanyName { get; set; }
    public string CompanyLocation { get; set; }
    public string Department { get; set; }
    public string Designation { get; set; }
    public string HostContactNumber { get; set; }
    public string HostEmail { get; set; }

    // List of visitors
    public List<VisitorRegistration> Visitors { get; set; } = new List<VisitorRegistration>();

    // Other details
    public string ItemsDetails { get; set; }
    public DateTime FromTime { get; set; }
    public DateTime ToTime { get; set; }

}
