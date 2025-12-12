using Microsoft.AspNetCore.Identity;

namespace ASP_PV411.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public String RoleId { get; set; } = null!;

        [PersonalData]
        public String Name { get; set; } = null!;

        [PersonalData]
        public String Email { get; set; } = null!;

        [PersonalData]
        public String? Phone { get; set; } = null!;

        [PersonalData]
        public DateOnly? Birthdate { get; set; } = null!;

        public String Login { get; set; } = null!;
        public String Salt { get; set; } = null!;
        public String Dk { get; set; } = null!;
        public DateTime RegisterAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public UserRole Role { get; set; } = null!;
        public ICollection<Cart> Carts { get; set; } = [];
    }
}
