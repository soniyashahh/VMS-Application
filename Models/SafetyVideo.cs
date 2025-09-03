using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VMSApplication.Models
{
    public class SafetyVideo : UserActivity
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string FilePath { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        // These are boolean fields to track answers or presence of questions
        public string Q1 { get; set; }
        public string Q2 { get; set; }
        public string Q3 { get; set; }
        public string Q4 { get; set; }
        public string Q5 { get; set; }

        // Corresponding Answers
        public bool Q1Answer { get; set; }
        public bool Q2Answer { get; set; }
        public bool Q3Answer { get; set; }
        public bool Q4Answer { get; set; }
        public bool Q5Answer { get; set; }

        [NotMapped]
        [Required]
        public IFormFile VideoFile { get; set; } // Not mapped to DB, only used for upload
    }
}
