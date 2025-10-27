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
            // ��� ������ ������������� ������������� ��� �������:
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
/* �.�. �������� ����� ����� ASP.NET Core MVC
 * ������ �� ����� ��� �������:
 * - Intro � ������ ASP 
 * - Razor ���� �� �������
 * - History � ��������� ������� ����� ASP
 * �������� ��������� �� �� ������� �� ����������� ������� �������, 
 * ������� �������� ������ �������.
 * ����������� ���������� � �������, ������ �� ����� ��������� 
 * � ����������.
 * ��� � �� - ��������� �� ����������
 */