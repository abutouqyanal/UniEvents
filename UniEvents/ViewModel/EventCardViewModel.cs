using UniEvents.Models; // تأكد من أن using صحيح لمجلد الموديلز

namespace UniEvents.ViewModels
{
    public class EventCardViewModel
    {
        public Event Event { get; set; } // كائن الفعالية
        public bool IsFavorite { get; set; } // هل هي في المفضلة أم لا؟
    }
}