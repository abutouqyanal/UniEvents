using System;
using System.ComponentModel.DataAnnotations;

namespace UniEvents.Models
{
    public class EventRating
    {
        [Key]
        public int EventRatingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int RatingValue { get; set; } 

        public DateTime DateRated { get; set; }
        public int EventId { get; set; }
        public int AttendeeId { get; set; }

        public Event Event { get; set; }
        public Attendee Attendee { get; set; }
    }
}