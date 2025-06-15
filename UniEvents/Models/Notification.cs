namespace UniEvents.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; }
    }
}
