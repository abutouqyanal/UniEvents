
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniEvents.Models
{
    public class MediaFile // <<< تم تغيير الاسم
    {
        [Key]
        public int MediaFileId { get; set; } // <<< تم تغيير الاسم

        [Required]
        [StringLength(300)]
        public string FilePath { get; set; }

        [Required]
        public MediaType MediaType { get; set; } // <<< تم تغيير النوع والاسم

        [StringLength(100)] // اختياري لكن مفيد
        public string? ContentType { get; set; } // مثال: "image/png", "video/mp4"

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User Uploader { get; set; } // أو Attendee
    }

    // تم تغيير اسم الـ Enum وإضافة نوع فيديو
    public enum MediaType
    {
        Unknown = 0,
        ProfilePicture = 1,
        EventBanner = 2,
        EventBackground = 3,
        Video = 4,         // <<< إضافة نوع الفيديو
        Document = 5,      // تغيير الترتيب ممكن
        OrganizerLogo = 6,
        Other = 99
    }
}