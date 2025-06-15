namespace UniEvents.Models
{
    public class Invitation
    {
        public int? InvitationId { get; set; }

        public string? Name { get; set; }

        public string? Position { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Reason { get; set; }

        // ربطه مع الايفينت إذا بدك
        public int EventId { get; set; }
        public Event Event { get; set; }

    }
}
