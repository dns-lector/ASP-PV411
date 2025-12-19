using ASP_PV411.Data;
using ASP_PV411.Data.Entities;
using ASP_PV411.Models.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASP_PV411.Controllers
{
    public class CartController(DataContext dataContext) : Controller
    {
        public IActionResult Index([FromRoute] String? id)
        {
            String? userId = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Sid)
                ?.Value;

            CartIndexViewModel viewModel = new()
            {
                IsAuthorized = userId != null,
                Cart = userId == null ? null :
                    dataContext
                    .Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId.ToString() == userId && (id == null ? c.CloseAt == null : c.Id.ToString() == id))
            };

            return View(viewModel);
        }

        public JsonResult Buy([FromRoute] String id)
        {
            try
            {
                Cart cart = GetCartById(id);
                cart.CloseAt = DateTime.Now;
                cart.CloseStatus = 1;
                dataContext.SaveChanges();
                return Json(new { Status = "Ok", Message = "Complete" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = ex.Message });
            }
        }

        public JsonResult Drop([FromRoute] String id)
        {
            try
            {
                Cart cart = GetCartById(id);
                cart.CloseAt = DateTime.Now;
                cart.CloseStatus = -1;
                dataContext.SaveChanges();
                return Json(new { Status = "Ok", Message = "Complete" });
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Message = ex.Message });
            }
        }

        public JsonResult Add([FromRoute] String id)
        {
            // Перевіряємо авторизацію, вилучаємо ід користувача
            Guid? userId = GetAuthUserId();
            if (userId == null)
            {
                return Json(new { Status = "Error", Message = "UnAuthorized" });
            }

            // Перевіряємо валідність товару (ід)
            var product = dataContext.Products.FirstOrDefault(p => p.Id.ToString() == id);
            if (product == null)
            {
                return Json(new { Status = "Error", Message = "Product not found" });
            }

            // Перевіряємо чи є в користувача відкритий кошик
            // якщо ні, то створюємо новий, якщо є - працюємо з ним
            Cart? cart = dataContext.Carts
                .Include(c => c.Discount)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Discount)
                .FirstOrDefault(c => c.UserId == userId && c.CloseAt == null);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId.Value,
                };
                dataContext.Carts.Add(cart);
            }

            // Перевіряємо чи є в кошику даний товар, якщо є, то
            // збільшуємо кількість, якщо ні - створюємо нову позицію
            CartItem? cartItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == product.Id && ci.DeleteAt == null);
            
            if (cartItem == null)
            {
                cartItem = new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    CartId = cart.Id,
                    Quantity = 1,
                    Product = product,
                };
                dataContext.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += 1;
            }

            // Перераховуємо ціни з урахуванням акцій
            cart.Price = 0.0;
            foreach (CartItem ci in cart.CartItems)
            {
                if(ci.DiscountId == null)
                {
                    ci.Price = ci.Product.Price * ci.Quantity;
                }
                else
                {
                    ci.Price = ci.Product.Price * ci.Quantity * 
                        (ci.Discount?.Percent ?? 1.0);
                }
                cart.Price += ci.Price;
            }
            if (cart.DiscountId != null)
            {
                // Корегуємо на акцію кошику
            }

            dataContext.SaveChanges();

            // Повертаємо статус обробки
            return Json(new { Status = "Ok", Message = "Added" });
        }

        private Guid? GetAuthUserId()
        {
            // Перевіряємо авторизацію, вилучаємо ід користувача
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            if (!isAuthenticated)
            {
                return null;
            }
            try
            {
                return Guid.Parse(
                    HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Sid).Value
                );
            }
            catch
            {
                return null;
            }
            
        }
    
        private Cart GetCartById(String id)
        {
            Guid? userId = GetAuthUserId() 
                ?? throw new Exception( "UnAuthorized" );

            // перевіряємо валідність кошику
            Cart? cart = dataContext.Carts.FirstOrDefault(c => c.Id.ToString() == id) 
                ?? throw new Exception("Cart Not Found" );

            // перевіряємо, що кошик активний (не закритий), а також його належність користувачеві
            if (cart.UserId != userId)
            {
                throw new Exception("Forbidden");
            }
            if (cart.CloseAt != null)
            {
                throw new Exception("Cart Is Closed");
            }
            return cart;
        }
    
    }
}
/* Д.З. Реалізувати роботу кнопки "До кошику", що знаходиться
 * на сторінці окремого товару. Додати аналіз статусів відповіді
 * на запит додавання, виводити його повідомленням (alert)
 */