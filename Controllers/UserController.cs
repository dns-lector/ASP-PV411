using ASP_PV411.Data;
using ASP_PV411.Middleware;
using ASP_PV411.Services.Kdf;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ASP_PV411.Controllers
{
    public class UserController(
        DataContext dataContext,
        IKdfService kdfService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authenticate()
        {
            // RFC 7617   https://datatracker.ietf.org/doc/html/rfc7617
            // Зворотній шлях - вилучення логіну, паролю та перевірка їх
            // Приклад з стандарту -- Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==

            // Серед заголовків вибираємо Authorization та перевіряємо на наявність (не-порожність)
            String authHeader = Request.Headers.Authorization.ToString();   // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (String.IsNullOrEmpty(authHeader))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content("Missing Authorization header");
            }

            // перевіряємо, що значення заголовку починається з правильної схеми (Basic )
            // ! кінцевий пробіл є частиною схеми
            String scheme = "Basic ";
            if (!authHeader.StartsWith(scheme))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid Authorization scheme: {scheme} only");
            }

            // виділяємо частину, що іде після схеми та перевіряємо на порожність
            String basicCredentials = authHeader[scheme.Length..];   // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
            if (basicCredentials.Length < 3)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid or empty {scheme} Credentials");
            }

            // декодуємо облікові дані з Base64
            String userPass;
            try
            {
                userPass = Encoding.UTF8.GetString(                  // Aladdin:open sesame
                    Convert.FromBase64String(basicCredentials));
            }
            catch(Exception ex)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid {scheme} Credentials format: {ex.Message}");
            }

            // Розділяємо облікові дані за ":", контролюємо, що поділ здійснюється на 2 частини
            String[] parts = userPass.Split(':', 2);   // 2 - макс. к-сть частин, ігноруємо ":" у паролі
            if (parts.Length != 2)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Invalid {scheme} user-pass format: missing ':' separator");
            }
            String login = parts[0];        // Aladdin
            String password = parts[1];     // open sesame

            // Шукаємо у БД користувача за логіном
            var user = dataContext.Users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Credentials rejected");
            }

            // Перевіряємо пароль шляхом розрахунку DK з використанням паролю, що 
            // переданий в облікових даних, та солі, що зберігається в БД у користувача
            // Розрахований результат має збігатись з тим, що наявний у БД
            String dk = kdfService.Dk(password, user.Salt);
            if (dk != user.Dk)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Content($"Credentials rejected.");
            }

            // якщо автентифікація пройшла успішно, то зберігаємо інформацію
            // у сесії.
            AuthSessionMiddleware.SaveAuth(HttpContext, user);

            return NoContent();
        }
    }
}
/* Д.З. Реалізувати виведення повідомлень про помилки автентифікації
 * у складі модального вікна ліворуч від його кнопок.
 * Рекомендується використати компонент типу alert від bootstrap
 * https://getbootstrap.com/docs/5.3/components/alerts/
 * (виводити тільки за наявності помилок, без них - прихований)
 */
