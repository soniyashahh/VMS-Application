namespace VMSApplication.Models
{
    public class Emailconfig
    {
        public string Email { get; set; }
        public string Emailbody { get; set; }
        public string Emailsender{get;set; }
        public int visitorId{get;set; }
        public VisitorRegistration VisitorRegistration { get; set; }
    }
}
