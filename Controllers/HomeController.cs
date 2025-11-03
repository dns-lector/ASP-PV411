using System.Diagnostics;
using ASP_PV411.Models;
using ASP_PV411.Models.Home;
using ASP_PV411.Services.Hash;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Random;
using ASP_PV411.Services.Salt;
using ASP_PV411.Services.Signature;
using ASP_PV411.Services.Timestamp;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRandomService _randomService;
        private readonly ITimestampService _timestampService;
        private readonly IHashService _hashService;
        private readonly ISaltService _saltService;
        private readonly IKdfService _kdfService;
        private readonly ISignatureService _signatureService;

        public HomeController(ILogger<HomeController> logger, IRandomService randomService, ITimestampService timestampService, IHashService hashService, ISaltService saltService, IKdfService kdfService, ISignatureService signatureService)
        {
            _logger = logger;
            _randomService = randomService;
            _timestampService = timestampService;
            _hashService = hashService;
            _saltService = saltService;
            _kdfService = kdfService;
            _signatureService = signatureService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IoC()
        {
            ViewData["sign"] = _signatureService.Sign("123", "456");
            ViewData["rnd"] = _randomService.RandomInt();
            ViewData["ref"] = _randomService.GetHashCode();
            ViewData["ctrl"] = _timestampService.Timestamp();
            ViewData["hash"] = _hashService.Digest("123");
            ViewData["salt"] = _saltService.GetSalt() + " " + _saltService.GetSalt(8);
            ViewData["dk"] = _kdfService.Dk("123", "456");
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
/* Д.З. Додати до проєкту сервіси:
 * - ОТР (One Time Password) - випадковий рядок з цифрами, кількість цифр
 *     передавати параметром, передбачити значення за замовчанням
 * - Генератор випадкових імен файлів заданої довжини (із замовчанням)
 *     випадковий рядок, що складається з літер одного реєстру, цифр та
 *     символів, дозволених в іменах файлів: "-", "_", тощо
 */