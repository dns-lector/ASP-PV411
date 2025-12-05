using ASP_PV411.Data;
using ASP_PV411.Models.Admin;
using ASP_PV411.Services.Storage;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ASP_PV411.Controllers
{
    public class AdminController(DataContext dataContext, IStorageService storageService) : Controller
    {
        public IActionResult Index()
        {
            bool isAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            String? roleId = isAuthenticated
                ? HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                : null;
            if(roleId != "Admin")
            {
                return RedirectToAction("Index", "Home");
            }
            AdminIndexViewModel model = new()
            {
                Manufacturers = dataContext.Manufacturers.ToList(),
                Groups = [.. dataContext.Groups]
            };
            return View(model);
        }

        public JsonResult AddGroup(AdminGroupFormModel formModel)
        {
            if (ModelState.IsValid)
            {
                String savedName;
                try { savedName = storageService.Save(formModel.Image); }
                catch (Exception ex) {
                    return Json(new
                    {
                        Status = "Error",
                        Errors = new Dictionary<String, String> { 
                            { "admin-group-image", ex.Message } 
                        }
                    });
                }
                Data.Entities.Group grp = new()
                {
                    Id = Guid.NewGuid(),
                    Name = formModel.Name,
                    Description = formModel.Description,
                    ImageUrl = savedName,
                };
                dataContext.Groups.Add(grp);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = grp
                });
            }
            else
            {
                Dictionary<String, String> errors = [];
                foreach (var kv in ModelState)
                {
                    String errMessages = String.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!String.IsNullOrEmpty(errMessages))
                    {
                        errors[kv.Key] = errMessages;
                    }
                }
                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }
               
        }

        public JsonResult AddManufacturer(AdminManufacturerFormModel formModel)
        {
            if (ModelState.IsValid)
            {
                String savedName;
                try { savedName = storageService.Save(formModel.Image); }
                catch (Exception ex) {
                    return Json(new
                    {
                        Status = "Error",
                        Errors = new Dictionary<String, String> { 
                            { "admin-manufacturer-image", ex.Message } 
                        }
                    });
                }
                Data.Entities.Manufacturer item = new()
                {
                    Id = Guid.NewGuid(),
                    Name = formModel.Name,
                    Description = formModel.Description,
                    ImageUrl = savedName,
                };
                dataContext.Manufacturers.Add(item);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = item
                });
            }
            else
            {
                Dictionary<String, String> errors = [];
                foreach (var kv in ModelState)
                {
                    String errMessages = String.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!String.IsNullOrEmpty(errMessages))
                    {
                        errors[kv.Key] = errMessages;
                    }
                }
                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }
               
        }


        public JsonResult AddProduct(AdminProductFormModel formModel)
        {
            if (ModelState.IsValid)
            {
                String? savedName = null;
                if(formModel.Image != null && formModel.Image.Length > 0)
                {
                    try { savedName = storageService.Save(formModel.Image); }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            Status = "Error",
                            Errors = new Dictionary<String, String> {
                                { "admin-product-image", ex.Message }
                            }
                        });
                    }
                }

                Guid groupId;
                try { groupId = Guid.Parse(formModel.GroupId); }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Status = "Error",
                        Errors = new Dictionary<String, String> {
                            { "admin-product-group", ex.Message }
                        }
                    });
                }

                Guid manufacturerId;
                try { manufacturerId = Guid.Parse(formModel.ManufacturerId); }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        Status = "Error",
                        Errors = new Dictionary<String, String> {
                            { "admin-product-manufacturer", ex.Message }
                        }
                    });
                }

                Data.Entities.Product item = new()
                {
                    Id = Guid.NewGuid(),
                    GroupId = groupId,
                    ManufacturerId = manufacturerId,
                    Name = formModel.Name,
                    Description = formModel.Description,
                    ImageUrl = savedName,
                    Price = formModel.Price,
                    Stock = formModel.Stock,
                };
                dataContext.Products.Add(item);
                dataContext.SaveChanges();
                return Json(new
                {
                    status = "Ok",
                    data = item
                });
            }
            else
            {
                Dictionary<String, String> errors = [];
                foreach (var kv in ModelState)
                {
                    String errMessages = String.Join(", ",
                        kv.Value.Errors.Select(e => e.ErrorMessage));

                    if (!String.IsNullOrEmpty(errMessages))
                    {
                        errors[kv.Key] = errMessages;
                    }
                }
                return Json(new
                {
                    Status = "Error",
                    Errors = errors
                });
            }

        }

    }
}
