using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_PV411.Models.Home
{
    public class HomeDemoFormModel
    {
        [FromForm(Name = "user-name")]
        [Required(ErrorMessage = "Необхідно зазначити ім'я")]
        public String UserName { get; set; } = null!;


        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null!;


        [FromForm(Name = "user-password")]
        [Required(ErrorMessage = "Необхідно зазначити пароль")]
        public String UserPassword { get; set; } = null!;


        [FromForm(Name = "user-birthdate")]
        [Required(ErrorMessage = "Необхідно зазначити дату народження")]
        public DateOnly? UserBirthdate { get; set; }


        [FromForm(Name = "agreement")]
        [Required(ErrorMessage = "Необхідно погодитись з правилами сайту")]
        public bool? IsAgree { get; set; }
    }
}
/*
exe-file
        (..x......ref.)     
run     |             |
[        ..10.....adr.         Object        ] x = 10;  ref = new Object()
                   \__________/

[        ..20.....adr.         Object Object ] x = 20;  ref = new Object()
                   \_xxxxxx___/         /
                    \__________________/

Nullable types
int? - розширення: все з int + спец.значення null
 */