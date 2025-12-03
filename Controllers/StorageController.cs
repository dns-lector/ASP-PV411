using ASP_PV411.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        public IActionResult Item([FromRoute]String id)
        {
            int dotIndex = id.LastIndexOf('.');
            if (dotIndex == -1)
            {
                return NotFound("File item must have extension");
            }
            String? mediaType = id[dotIndex..] switch
            {
                ".png"  => "image/png",
                ".bmp"  => "image/bmp",
                ".jpg"  => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => null
            };
            if (mediaType == null)
            {
                return new UnsupportedMediaTypeResult();
            }
            try
            {
                return File(storageService.Get(id), mediaType);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
/* Д.З. Забезпечити перевірку розширень (типів) файлів
 * у LocalStorageService перед їх збереженням.
 * Організувати єдиний перелік дозволених типів, який
 * буде перевірятись як при зчитуванні, так і при записі файлів.
 */