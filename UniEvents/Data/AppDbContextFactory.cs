using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

//  !!! تأكد من أن مساحة الاسم (namespace) هذه تطابق مساحة الاسم في مشروعك !!!
namespace UniEvents.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbcontext>
    {
        public AppDbcontext CreateDbContext(string[] args)
        {
            // هذا الكود يبني إعدادات بنفسه ويقرأ الملف يدوياً
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbcontext>();

            // يقرأ سلسلة الاتصال من الملف باستخدام المفتاح "constr"
            var connectionString = configuration.GetConnectionString("constr");

            builder.UseSqlServer(connectionString);

            return new AppDbcontext(builder.Options);
        }
    }
}