using System.Diagnostics;
using ASP_PV411.Models;
using ASP_PV411.Models.Home;
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
            // Обмінні об'єкти, що дозволяють передати дані
            // від контролера до представлення
            // а) у формі dynamic
            ViewBag.arr1 = new String[] { "string 1", "string 2" };
            // б) у формі Dictionary
            ViewData["arr2"] = new String[] { "string 3", "string 4" };
            // ці об'єкти пов'язані між собою, тобто поклавши дані в один з них,
            // вилучити можна з іншого, тобто ViewBag.arr2 або  ViewData["arr1"]
            // (див. у розмітці)

            // Рекомендований варіант - використання моделей
            HomeRazorViewModel model = new()
            {
                Products = [
                    new() { Name = "Asus",   Price = 18900 },
                    new() { Name = "Lenovo", Price = 19800 },
                    new() { Name = "Acer",   Price = 21000 },
                    new() { Name = "Dell",   Price = 25000 },
                    new() { Name = "HP",     Price = 15200 },
                ]
            };
            return View(model);   // !! сформована модель передається до представлення
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