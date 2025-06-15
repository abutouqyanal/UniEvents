using UniEvents.Models;

namespace UniEvents.Models
{
    public class AttendeeEvent
    {


        // المفتاح الأساسي
        public int AttendeeId { get; set; }
        public Attendee Attendee { get; set; } // العلاقة مع Attendee

        public int EventId { get; set; }
        public Event Event { get; set; } // العلاقة مع Event


    }
}

