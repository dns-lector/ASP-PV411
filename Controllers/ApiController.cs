using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Controllers
{
    [Route("/API/Action")]  // в АРІ контролері один маршрут, назва методів ролі не грає
    [ApiController]
    public class ApiController : ControllerBase
    {
        [HttpGet]  // роль грає НТТР-метод запиту, саме за ним відбувається розгалуження
        public String GetAction()
        {
            return "ApiController::GetAction";
        }

        [HttpPost]   // якщо не зазначати атрибут, то метод буде працювати для всіх запитів,
                     // а поява другого методу призведе до помилки неоднозначності
        public Object PostAction()
        {
            return new {
                controller = "ApiController",
                action = "PostAction"
            };
        }

        [HttpGet("sub")]   // створення внутрішньої адреси API/Action/sub
        public String GetSubAction()
        {
            return "ApiController::GetSubAction";
        }

        [HttpPatch("sub")]   // створення внутрішньої адреси API/Action/sub для метода PATCH
        public String PatchSubAction()
        {
            return "ApiController::PatchSubAction";
        }

        [HttpDelete("user")]   // створення внутрішньої адреси API/Action/user для метода DELETE
        public String DeleteUserAction()
        {
            return "ApiController::DeleteUserAction";
        }
    }
}
/* CRUD (Create, Read, Update, Delete)
 * а) життєвий цикл інформації (даних)
 * б) повний набір операцій з даними
 * 
 * GET      Read
 * POST     Create
 * PUT      Replace (Full Update)
 * PATCH    Update  (Partial Update)
 * DELETE   Delete
 */