using System.Diagnostics;
using ASP_PV411.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Intro()
        {
            return View();
        }

        public IActionResult Razor()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            // при пошуку представлення розглядаються два варіанти:
            // 1. /Views/[controller]/[action] -- /Views/Home/Privacy
            // 2. /Views/Shared/[action]
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
/* Д.З. Створити новий проєкт ASP.NET Core MVC
 * Додати до нього нові сторінки:
 * - Intro з описом ASP 
 * - Razor поки що порожню
 * - History з основними етапами історії ASP
 * Включити посилання на ці сторінки до заголовкової частити шаблона, 
 * зробити скріншоти роботи сторінок.
 * Опублікувати репозиторій з проєктом, додати до нього директорію 
 * з скріншотами.
 * Звіт з ДЗ - посилання на репозиторій
 */