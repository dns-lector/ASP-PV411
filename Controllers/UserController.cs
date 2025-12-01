using ASP_PV411.Data;
using ASP_PV411.Data.Entities;
using ASP_PV411.Middleware;
using ASP_PV411.Models.User;
using ASP_PV411.Services.Kdf;
using ASP_PV411.Services.Salt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ASP_PV411.Controllers
{
    public class UserController(
        DataContext dataContext,
        ISaltService saltService,
        IKdfService kdfService) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Register(UserRegisterFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                Dictionary<String, String> errors = [];
                foreach (var kv in ModelState)
                {
                    String errMessages = String.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if(!String.IsNullOrEmpty(errMessages))
                    {
                        errors[kv.Key] = errMessages;
                    }                    
                }
                return Json(new { 
                    Status = "Error",
                    Errors = errors
                });
            }
            if (dataContext.Users.Any(u => u.Login == formModel.UserLogin))
            {

                return Json(new
                {
                    Status = "Error",
                    Errors = new Dictionary<String, String>() {
                        { "user-login", "Login in use" }
                    }
                });
            }
            String salt = saltService.GetSalt();
            dataContext.Users.Add(new()
            {
                Id = Guid.NewGuid(),
                Name = formModel.UserName,
                Email = formModel.UserEmail,
                Phone = formModel.UserPhone,
                Login = formModel.UserLogin,
                Salt = salt,
                Dk = kdfService.Dk(formModel.UserPassword, salt),
                Birthdate = formModel.UserBirthdate,
                RegisterAt = DateTime.Now,
                RoleId = "User"
            });
            dataContext.SaveChanges();
            return Json(new { Status = "Ok" });
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UserUpdateFormModel formModel)
        {
            // Перевіряємо наявність та правильність автентифікації
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return Unauthorized("No user in context");
            }
            String userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
            User? user = dataContext
                .Users
                .First(u => u.Id == Guid.Parse(userId));
            if (user == null)
            {
                return Unauthorized("Invalid user restoring from identity");
            }

            // Зберігаємо тип сутності (User)
            Type userType = user.GetType();

            // Перевіряємо, що хоча б одне з полів наявне (не всі null)            
            bool isAllNulls = true;
            foreach(var prop in formModel.GetType().GetProperties())
            {
                Object? val = prop.GetValue(formModel);
                if(val != null)
                {
                    isAllNulls = false;
                    if (prop.Name == "Password")
                    {
                        // Оновлення паролю:
                        String salt = saltService.GetSalt();
                        user.Salt = salt;
                        user.Dk = kdfService.Dk(formModel.Password!, salt);
                    }
                    else
                    {
                        var userProp = userType.GetProperty(prop.Name);
                        if (userProp != null)
                        {
                            userProp.SetValue(user, val);  // а також переносимо усі ненульові значення до сутності (user)
                        }
                        else
                        {
                            return BadRequest($"Form Property '{prop.Name}' not found in entity '{userType.Name}'");
                        }
                    }                    
                }
            }
            if(isAllNulls)
            {
                return BadRequest("No data for update");
            }
            var saveTask = dataContext.SaveChangesAsync();

            // Оновлюємо збережені дані у сесії 
            AuthSessionMiddleware.SaveAuth(HttpContext, user);

            await saveTask;

            return Accepted();
        }

        public IActionResult Profile()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (isAuthenticated)
            {
                String userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
                var user = dataContext
                    .Users
                    .Include(u => u.Role)
                    .First(u => u.Id == Guid.Parse(userId) && u.DeleteAt == null)!;

                return View(new UserProfileViewModel
                {
                    User = user,
                    IsPersonal = true,
                });
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpDelete]
        public async Task<JsonResult> EraseAsync()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (isAuthenticated)
            {
                String userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value;
                var user = dataContext
                    .Users
                    .First(u => u.Id == Guid.Parse(userId))!;
                // dataContext.Remove(user); -- не рекомендується
                // foreach(var prop in user.GetType().GetProperties())
                // {
                //     if(prop.GetCustomAttribute<PersonalDataAttribute>() != null)
                //     {
                //         if (prop.GetType().IsAbstract)
                //         {
                // 
                //         }
                //     }
                // }
                user.Name = user.Email = user.Phone = "";
                user.Birthdate = null;
                user.DeleteAt = DateTime.Now;

                // запускаємо задачу збереження асинхронно
                var saveTask = dataContext.SaveChangesAsync();

                // Видаляємо збережені дані у сесії 
                AuthSessionMiddleware.Logout(HttpContext);

                // очікуємо завершення задачі збереження
                await saveTask;

                return Json(new { Status = "Ok" });
            }
            else
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { 
                    Status = "Error",
                    Errors = new Dictionary<String, String>() {
                        { "auth", "User not authorized" }
                    }
                });
            }
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
            var user = dataContext
                .Users
                .FirstOrDefault(u => u.Login == login && u.DeleteAt == null);

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
/* Д.З. Реалізувати можливість зміни дати народження:
 * - якщо у БД немає відповідних даних, то у таблиці вони не відображаються,
 *    але при переході в режим редагування відповідний рядок має з'являтись
 * - якщо є, то звичайний перехід до редагування.
 * Переконатись у правильній роботі оновлення даних.
 * 
 */
