
namespace UniEvents.Models
{
    public class User
    {
        public int UserId { get; set; } // Primary key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int PhoneNumber { get; set; }
    }

    public enum User_Type
    {
        Admin, Attendee
    }
}
