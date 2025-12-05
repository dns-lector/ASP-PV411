using Microsoft.AspNetCore.Mvc;

namespace ASP_PV411.Models.Admin
{
    public class AdminGroupFormModel
    {
        [FromForm(Name = "admin-group-name")]
        public String Name { get; set; } = null!;

        [FromForm(Name = "admin-group-description")]
        public String Description { get; set; } = null!;

        [FromForm(Name = "admin-group-image")]
        public IFormFile Image { get; set; } = null!;
    }
}
