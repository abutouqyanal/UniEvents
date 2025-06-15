using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace UniEvents.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        public string EventName { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public int Capicity { get; set; }

        public int OrganizerId { get; set; }
        public virtual User? Organizer { get; set; }

        public virtual ICollection<Attendee>? Attendees { get; set; } = new List<Attendee>();
        public virtual ICollection<AttendeeEvent>? AttendeeEvents { get; set; } = new List<AttendeeEvent>();

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public Status Status { get; set; }

        public string? Sponsor { get; set; }
        public string? College { get; set; }
        public string? EventType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime EndTime { get; set; }

        public string? NewCategory { get; set; }
        public string? ImagePath { get; set; }
        public virtual ICollection<FavoriteEvent> FavoritedByUsers { get; set; } = new List<FavoriteEvent>();
        public virtual ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();

    }

    public enum Status
    {
        Approved,
        Rejected,
        Pending
    }
}
