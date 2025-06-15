using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace UniEvents.Models
{
    public class AgreementInfo
    {
        public int Id { get; set; }

        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Department { get; set; }
        public string StudentEmail { get; set; }
        public string Phone { get; set; }
        public string Reason { get; set; }
        public bool Agree { get; set; }

        // الربط مع جدول الأحداث
        public int EventId { get; set; }
        public Event Event { get; set; }
    }

}
