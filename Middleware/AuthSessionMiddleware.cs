using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace ASP_PV411.Middleware
{
    public class AuthSessionMiddleware
    {
        public const String SessionKey = "AuthSessionMiddleware";

        private readonly RequestDelegate _next;

        public AuthSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // перевіряємо, чи є запит на вихід з авторизованого режиму
            if (context.Request.Query.ContainsKey("logout"))
            {
                context.Session.Remove(SessionKey);
                context.Response.Redirect(context.Request.Path);
                return;  // _next - не буде виконуватись, робота перерветься
            }
            // перевіряємо наявність у сесії збережених даних щодо автентифікації
            if(context.Session.Keys.Contains(SessionKey))
            {
                var user = JsonSerializer.Deserialize<Data.Entities.User>(
                    context.Session.GetString(SessionKey)!)!;

                // context.Items[SessionKey] = user;
                /* Використання (поширення) типу даних з сутностей БД має ряд недоліків:
                 * - зчеплення модулів (шарів) - залежність другого модуля від типів  
                 *    даних першого модуля
                 * - складність відокремлення модуля як сервісу, оскільки модуль, що 
                 *    залишається, втрачає зв'язок з потрібним типом даних
                 * - складність використання сторонніх сервісів автентифікації
                 *    (Гугл, ФБ тощо) - вони передають свої дані, можливо, несумісні 
                 *    з нашими типами
                 */
                context.User = new ClaimsPrincipal(                  // Слід використовувати
                    new ClaimsIdentity(                              // уніфікований інтерфейс
                        [                                            // відокремлений від
                            new Claim(ClaimTypes.Name,               // типізації - пари ключ-значення
                                user.Name),                          // Claim(тип, значення)
                            new Claim(ClaimTypes.Email,              // 
                                user.Email),                         // Вони поєднуються до 
                            new Claim(ClaimTypes.DateOfBirth,        // ClaimsIdentity, до якої
                                user.Birthdate?.ToString() ?? ""),   // додається тип автентифікації
                            new Claim(ClaimTypes.NameIdentifier,     // nameof(AuthSessionMiddleware)
                                user.Login),                         // за яким можна дізнатись 
                            new Claim(ClaimTypes.Sid,                // походження Claims
                                user.Id.ToString()),                 // 
                            new Claim(ClaimTypes.Role,
                                user.RoleId),
                        ],                                           // Технічно, користувач може мати
                        nameof(AuthSessionMiddleware)                // декілька Identity різного походження
                    )
                );
                
            }
            // Call the next delegate/middleware in the pipeline.
            await _next(context);
        }

        public static void SaveAuth(HttpContext context, Data.Entities.User user)
        {
            context.Session.SetString(
                AuthSessionMiddleware.SessionKey,
                JsonSerializer.Serialize(user));
        }

        public static void Logout(HttpContext context)
        {
            context.Session.Remove(
                AuthSessionMiddleware.SessionKey);
        }
    }


    public static class AuthSessionMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthSession(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthSessionMiddleware>();
        }
    }
}

/*
Д.З. Реалізувати стилізацію авторизованого віджета (літери у кружечку)
Додати до підказки, що спливає (title) інформацію про E-mail користувача
Передбачити, щоб літера була великою (навіть якщо ім'я у БД починається з 
малої літери)
 */
