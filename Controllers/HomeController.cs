using ASP_PV411.Models;
using ASP_PV411.Models.Home;
using ASP_PV411.Services.Hash;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Random;
using ASP_PV411.Services.Salt;
using ASP_PV411.Services.Signature;
using ASP_PV411.Services.Timestamp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Text.Json;

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

        public IActionResult Forms([FromQuery]String? data, [FromQuery(Name = "data-2")] String? data2)
        {
            /* Binding (model binding) - процес в ASP, який автоматично 
             * зв'язує параметри, що передаються у запиті, із параметрами 
             * методів контролерів. Якщо зв'язування не вдається (виникають
             * помилки), то автоматично формується відповідь-помилка. 
             * Зв'язування відбувається за іменами - назва параметра методу 
             * має збігатись із назвою параметра, що приходить з форми */
            ViewData["data"] = data;

            /* Якщо було надсилання POST-даних, то при переході (Redirect)
             * на даний метод у сесії буде наявний параметр "data".
             * Слід перевірити його існування, за наявності обробити та 
             * видалити з сесії, щоб при повторному заході він вже не оброблявся.
             */
            if (HttpContext.Session.Keys.Contains("data"))
            {
                ViewData["post-data"] = HttpContext.Session.GetString("data");
                HttpContext.Session.Remove("data");
            }
            return View();
        }

        public IActionResult FormPost([FromForm] String data)
        {
            /* Проблема з post-даними виникає, коли користувач намагається
             * оновити сторінку браузера, що побудована за результатами надсилання
             * форми. Браузер видає попередження, що не стилізується та може
             * спантеличити користувача.
             * Застосовується наступна схема:
             * Browser                     Server                           Session
             * POST data=123 ----------------> зберігає дані (data=123) ----> data = 123
             *                 Redirect        і повідомляє браузер            |
             *     <-------------------------  про перенаправлення             |
             * GET                                                             |
             *     --------------------------> Відновлюємо збережені дані      |
             *                                 та формуємо результат          data = 123
             */
            if ( ! String.IsNullOrEmpty(data))
            {
                HttpContext.Session.SetString("data", data);
            }            
            return RedirectToAction(nameof(Forms));
        }

        public IActionResult FormModels(HomeDemoFormModel? formModel)
        {
            if(Request.Method == "POST")
            {
                // Зберігаємо дані (модель) у сесії
                // попередньо серіалізуємо, оскільки сесія не зберігає посилання
                String json = JsonSerializer.Serialize(formModel);
                HttpContext.Session.SetString(nameof(HomeDemoFormModel), json);
                // також зберігаємо результати валідації моделі
                Dictionary<String, String> dict = [];
                foreach(var kv in ModelState)
                {
                    dict[kv.Key] = String.Join(", ", 
                        kv.Value.Errors.Select(e => e.ErrorMessage));
                }
                json = JsonSerializer.Serialize(dict);
                HttpContext.Session.SetString(nameof(ModelState),json);

                return RedirectToAction(nameof(FormModels));
            }
            else
            {
                // готуємо модель представлення
                HomeDemoViewModel viewModel = new()
                {
                    PageTitle = "Форми ІІ. Моделі форм",
                    FormTitle = "Заповніть наступні дані:"
                };

                // перевіряємо чи є збережена у сесії модель форми
                if (HttpContext.Session.Keys.Contains(nameof(HomeDemoFormModel)))
                {
                    // якщо є, то десеріалізуємо та переносимо в модель представлення
                    viewModel.FormModel = JsonSerializer.Deserialize<HomeDemoFormModel>(
                        HttpContext.Session.GetString(nameof(HomeDemoFormModel))!
                    );
                    // також відновлюємо результати валідації моделі
                    viewModel.ModelErrors = JsonSerializer.Deserialize<Dictionary<String,String>>(
                        HttpContext.Session.GetString(nameof(ModelState))!
                    );
                }

                // передаємо модель на представлення
                return View(viewModel);
            }
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
/* Д.З. Додати до демонстраційних форм поля:
 * - Прізвище
 * - Ім'я
 * - Побатькові
 * Реалізувати передачу даних методами get та post,
 * їх відображення біля форм за умови передачі.
 */