namespace ASP_PV411.Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public String RoleId { get; set; } = null!;
        public String Name { get; set; } = null!;
        public String Email { get; set; } = null!;
        public DateOnly? Birthdate { get; set; } = null!;
        public String Login { get; set; } = null!;
        public String Salt { get; set; } = null!;
        public String Dk { get; set; } = null!;
        public DateTime RegisterAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public UserRole Role { get; set; } = null!;
    }
}
