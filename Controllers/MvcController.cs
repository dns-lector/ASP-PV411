using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    public class MvcController : Controller
    {
        public IActionResult Action()
        {
            return Content("MvcController::Action");
        }

        [HttpPost]  // на відміну від АРІ, даний атрибут лише обмежує методи доступу до дії
        public IActionResult PostAction()
        {
            return Content("MvcController::PostAction");
        }

        [HttpGet]               // внутрішні адреси MVC - через параметр [FromRoute] String id
        public IActionResult GetAction([FromRoute] String id)
        {
            return Content("MvcController::GetAction / " + id);
        }
    }
}
/* Створити контролери:
 * InfoController (MVC) з методами .Mvc() та .Api() які будуть виводити інформацію
 * про зазначений тип контролерів
 * 
 * контролер для адреси /api/info який на запит GET дає інформацію про 
 * MVC контролери, а на запит POST - про API
 * 
 * Прикласти скріншоти роботи проєкту
 */