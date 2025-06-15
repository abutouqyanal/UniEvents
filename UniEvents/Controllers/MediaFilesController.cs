
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniEvents.Data;
using UniEvents.Models;

[Authorize]
public class MediaFilesController : Controller
{
    private readonly AppDbcontext _context;
    private readonly IWebHostEnvironment _environment;
    // --- تعديل حدود الملفات والأنواع ---
    private readonly long _mediaFileSizeLimit = 25 * 1024 * 1024; // مثال: 25 ميجابايت
    private readonly string[] _permittedMediaExtensions = {
         ".jpg", ".jpeg", ".png", ".gif", // صور
         ".mp4", ".mov", ".avi", ".wmv" // فيديو (أضف ما تريد)
          //,".pdf", ".doc", ".docx" // مستندات إذا أردت
          };
    // --- نهاية التعديل ---
    public MediaFilesController(AppDbcontext context, IWebHostEnvironment environment)
    {
        _context = context; // <<< يجب تعيين القيمة هنا
        _environment = environment; // تأكد من تعيين هذا أيضاً
    }
   

 
    [HttpGet]
    public async Task<IActionResult> Upload(int eventId)
    {
        // التحقق من صلاحيات المستخدم ... (مهم!)
        var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(currentUserIdStr, out int currentUserId)) return Unauthorized();
       


        ViewBag.EventId = eventId;
        ViewBag.UserId = currentUserId; // لا تعتمد عليه مباشرة في الفورم

        // جلب كل الوسائط الموجودة للحدث المحدد
        var existingMedia = await _context.MediaFiles // <<< استخدام MediaFiles
                                  .Where(m => m.EventId == eventId) // فلترة حسب الحدث
                                  .OrderBy(m => m.MediaType)        // ترتيب للعرض المنظم
                                  .ToListAsync();                   // جلب القائمة

        ViewBag.ExistingMedia = existingMedia; // تمرير القائمة للفيو

        return View(); // الذهاب لفيو Upload.cshtml
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(List<IFormFile> Media, List<MediaType> MediaTypes, int eventId) // <<< تغيير أسماء المعاملات
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out int userId)) { return Unauthorized(); }
        // ... (تحقق الملكية) ...

        List<string> errors = new List<string>();
        // <<< تغيير أسماء المتغيرات في التحقق من التوافق
        if (Media == null || MediaTypes == null || Media.Count != MediaTypes.Count) { /* ... */ }

        for (int i = 0; i < Media.Count; i++)
        {
            var file = Media[i]; // <<< تغيير
            var type = MediaTypes[i]; // <<< تغيير

            if (file == null || file.Length == 0) continue;

            // <<< استخدام المتغيرات الجديدة للتحقق
            if (file.Length > _mediaFileSizeLimit)
            { errors.Add($"File '{file.FileName}' exceeds size limit ({_mediaFileSizeLimit / 1024 / 1024} MB)."); continue; }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !_permittedMediaExtensions.Contains(ext)) // <<< استخدام المصفوفة الجديدة
            { errors.Add($"File '{file.FileName}' has an invalid extension..."); continue; }

            var existingSpecificTypeMedia = await _context.MediaFiles // <<< تغيير
                                               .FirstOrDefaultAsync(m => m.EventId == eventId && m.MediaType == type);

            string uniqueFileName = Guid.NewGuid().ToString() + ext;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "ImagesFile", "media", eventId.ToString());
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            var dbPath = $"/ImagesFile/media/{eventId}/{uniqueFileName}";


            try
            {
                // ... (كود حذف الملف القديم إذا كان موجودًا) ...

                using (var stream = new FileStream(filePath, FileMode.Create)) { await file.CopyToAsync(stream); }

                if (existingSpecificTypeMedia != null)
                {
                    existingSpecificTypeMedia.FilePath = dbPath;
                    existingSpecificTypeMedia.UserId = userId;
                    existingSpecificTypeMedia.ContentType = file.ContentType; // <<< ⭐ حفظ ContentType ⭐
                }
                else
                {
                    var mediaFile = new MediaFile // <<< تغيير
                    {
                        FilePath = dbPath,
                        MediaType = type,      // <<< تغيير
                        EventId = eventId,
                        UserId = userId,
                        ContentType = file.ContentType // <<< ⭐ حفظ ContentType ⭐
                    };
                    _context.MediaFiles.Add(mediaFile); // <<< تغيير
                }
            }
            catch (Exception ex) { /* ... (معالجة الأخطاء) ... */ }
        } // نهاية for

        // ... (كود حفظ التغييرات والتعامل مع الأخطاء والـ Redirect) ...
        await _context.SaveChangesAsync(); // حفظ كل التغييرات
        return RedirectToAction("InviteInfo", "Eventt", new { eventId = eventId });
    }

}
