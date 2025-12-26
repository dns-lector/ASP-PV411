using ASP_PV411.Data;
using ASP_PV411.Models.Rest;
using ASP_PV411.Models.Shop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Controllers
{
    public class ShopController(DataContext dataContext, DataAccessor dataAccessor) : Controller
    {
        private String FullImageUrl(String localUrl) =>
            $"{Request.Scheme}://{Request.Host}/Storage/Item/{localUrl}";

        public IActionResult Index()
        {
            ShopIndexViewModel model = new()
            {
                Groups = dataContext.Groups.Where(g => g.DeleteAt == null).ToList(),
            };
            return View(model);
        }
        public JsonResult ApiIndex()
        {
            ShopIndexViewModel model = new()
            {
                Groups = dataContext
                .Groups
                .Where(g => g.DeleteAt == null)
                .AsEnumerable()
                .Select(g => g with {
                    ImageUrl = FullImageUrl(g.ImageUrl)
                })
                .ToList(),
            };
            return Json(model);
        }



        public IActionResult Group([FromRoute]String id)
        {
            ShopGroupViewModel model = new()
            {
                Group = dataAccessor.GetGroupBySlug(id),
            };
            return View(model);
        }
        public JsonResult ApiGroup([FromRoute] String id)
        {
            var group = dataAccessor.GetGroupBySlug(id);
            if(group != null)
            {
                group = group with { 
                    ImageUrl = FullImageUrl(group.ImageUrl),
                    Products = [..group.Products.Select(p => p with
                    {
                        ImageUrl = p.ImageUrl == null ? null : FullImageUrl(p.ImageUrl),
                    })],
                };
            }

            RestResponse restResponse = new()
            {
                Status = group == null ? RestStatus.NotFound : RestStatus.Ok,
                Meta = new()
                {
                    Service = "Крамниця",
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 86400,
                    DataType = group == null ? "null" : "object",
                    Method = Request.Method,
                    Path = Request.Path,
                    Resource = "Product group",
                },
                Data = group,
            };
            return Json(restResponse);
        }



        public IActionResult Product([FromRoute]String id)
        {
            ShopProductViewModel model = new()
            {
                Product = dataContext
                .Products
                .FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id),
            };
            return View(model);
        }
    }
}
/* REST
 * {
 *      "status": {
 *          "isOk": false,
 *          "code": 401,
 *          "phrase": "UnAuthorized"
 *      },
 *      "meta": {
 *          "service": "Shop-The-Best API",
 *          "resource": "User",
 *          "method": "GET",
 *          "path": "/user",
 *          "serverTime": 1283760783,
 *          "cache": 0,
 *          "links": {
 *              "Authentication": "GET /user",
 *              "Registration": "POST /user",
 *              "Profile": "GET /user/{id}",
 *              "Delete": "DELETE /user/{id}"
 *          },
 *          "dataType": "string"
 *      },
 *      "data": "Missing or empty 'Authorization' header"  
 * }
 * 
 */
