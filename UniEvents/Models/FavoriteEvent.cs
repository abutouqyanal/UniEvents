using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniEvents.Models
{
    public class FavoriteEvent
    {
        // مفتاح خارجي للمستخدم (Attendee)
        // نفترض أن Attendee يرث UserId من User، لذلك النوع هو int
        [Required] // يجب أن يكون هناك مستخدم
        public int UserId { get; set; }
        // خاصية التنقل للمستخدم
        [ForeignKey("UserId")] // نربط الخاصية بالعمود UserId
        public virtual Attendee Attendee { get; set; }

        // مفتاح خارجي للفعالية (Event)
        [Required] // يجب أن تكون هناك فعالية
        public int EventId { get; set; }
        // خاصية التنقل للفعالية
        [ForeignKey("EventId")] // نربط الخاصية بالعمود EventId
        public virtual Event Event { get; set; }

        // (اختياري ولكن مفيد) تاريخ الإضافة للمفضلة
        public DateTime DateFavorited { get; set; } = DateTime.UtcNow;
    }
}