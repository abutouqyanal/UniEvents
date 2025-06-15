using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace UniEvents.Models
{
    public class Attendee : User
    {
        [Required(ErrorMessage = "University ID is required")]
        public string University_id { get; set; }

        [Required(ErrorMessage = "University Name is required")]
        public string University_name { get; set; }

        [Required(ErrorMessage = "University Major is required")]
        public string University_major { get; set; }

        public ICollection<Event>? Events { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
        public ICollection<AttendeeEvent>? AttendeeEvents { get; set; }
        public virtual ICollection<FavoriteEvent> FavoriteEvents { get; set; } = new List<FavoriteEvent>();
        public virtual ICollection<MediaFile> UploadedMediaFiles { get; set; } = new List<MediaFile>();
    }
}
