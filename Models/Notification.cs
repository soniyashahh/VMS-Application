namespace VMSApplication.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }  // Recipient User ID
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
