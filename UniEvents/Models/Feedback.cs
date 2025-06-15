
using System;
using System.ComponentModel.DataAnnotations;

namespace UniEvents.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        public int EventId { get; set; }
        public Event Event { get; set; }

        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; }

        [Required(ErrorMessage = "Comment cannot be empty.")] 
        public string Comment { get; set; }

        public DateTime DatePosted { get; set; } 
    }
}