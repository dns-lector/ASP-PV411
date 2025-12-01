using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ASP_PV411.Models.User
{
    public class UserRegisterFormModel
    {
        [FromForm(Name = "user-name")]
        [Required(ErrorMessage = "Необхідно зазначити ім'я")]
        public String UserName { get; set; } = null!;


        [FromForm(Name = "user-email")]
        public String UserEmail { get; set; } = null!;


        [FromForm(Name = "user-login")]
        [Required(ErrorMessage = "Необхідно зазначити логін")]
        public String UserLogin { get; set; } = null!;


        [FromForm(Name = "user-phone")]
        [Required(ErrorMessage = "Необхідно зазначити телефон")]
        public String UserPhone { get; set; } = null!;


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
