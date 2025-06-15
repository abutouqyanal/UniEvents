using Microsoft.EntityFrameworkCore;
using UniEvents.Models;

namespace UniEvents.Data
{
    public class AppDbcontext : DbContext
    {
        public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options)
        {
        }

        // --- DbSets ---
        public DbSet<User> users { get; set; } // اسم غير تقليدي (عادةً يكون Users)
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Admin> Admins { get; set; } // افترض وجود موديل Admin
        public DbSet<Event> Events { get; set; }
        public DbSet<Notification> Notifications { get; set; } // افترض وجود موديل Notification
        public DbSet<Category> Categories { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AttendeeEvent> AttendeeEvents { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<AgreementInfo> Agreements { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<FavoriteEvent> FavoriteEvents { get; set; } // DbSet الجديد للمفضلة
                                                                 // In Data/AppDbcontext.cs
        public DbSet<EventRating> EventRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // احتفظ بهذا السطر

            // --- تكوين AttendeeEvent (موجود لديك) ---
            modelBuilder.Entity<AttendeeEvent>()
                .HasKey(ae => new { ae.AttendeeId, ae.EventId });

            modelBuilder.Entity<AttendeeEvent>()
                .HasOne(ae => ae.Attendee)
                .WithMany(a => a.AttendeeEvents)
                .HasForeignKey(ae => ae.AttendeeId)
                .OnDelete(DeleteBehavior.Restrict); // تحديد سلوك الحذف (أو Cascade حسب الحاجة)

            modelBuilder.Entity<AttendeeEvent>()
                .HasOne(ae => ae.Event)
                .WithMany(e => e.AttendeeEvents)
                .HasForeignKey(ae => ae.EventId)
                .OnDelete(DeleteBehavior.Restrict); // تحديد سلوك الحذف

            // --- تكوين علاقة Event مع Organizer (موجود لديك) ---
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany() // User ليس لديه قائمة Events ينظمها مباشرة (صحيح حسب الموديل الحالي)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- ⭐ إضافة جديدة: تكوين FavoriteEvent ⭐ ---
            // 1. تحديد المفتاح المركب لجدول FavoriteEvent
            modelBuilder.Entity<FavoriteEvent>()
                .HasKey(fe => new { fe.UserId, fe.EventId }); // المفتاح هو تركيبة من العمودين

            // 2. تعريف علاقة FavoriteEvent مع Attendee (المستخدم)
            modelBuilder.Entity<FavoriteEvent>()
                .HasOne(fe => fe.Attendee)           // كل FavoriteEvent له Attendee واحد
                .WithMany(a => a.FavoriteEvents)   // وكل Attendee له قائمة FavoriteEvents (تأكد من وجود هذه الخاصية في Attendee.cs)
                .HasForeignKey(fe => fe.UserId)    // المفتاح الخارجي هو UserId في FavoriteEvent
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتتالي (أو Cascade إذا أردت)

            // 3. تعريف علاقة FavoriteEvent مع Event (الفعالية)
            modelBuilder.Entity<FavoriteEvent>()
                .HasOne(fe => fe.Event)             // كل FavoriteEvent له Event واحد
                .WithMany(e => e.FavoritedByUsers) // وكل Event له قائمة FavoritedByUsers (تأكد من وجود هذه الخاصية في Event.cs)
                .HasForeignKey(fe => fe.EventId)   // المفتاح الخارجي هو EventId في FavoriteEvent
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتتالي (أو Cascade)
            // --- نهاية الإضافة الجديدة ---


            // أضف أي تكوينات أخرى ضرورية هنا...
        }
    }
}