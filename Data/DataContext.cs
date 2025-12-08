using ASP_PV411.Services.Kdf;
using Microsoft.EntityFrameworkCore;

namespace ASP_PV411.Data
{
    public class DataContext : DbContext
    {
        private readonly IKdfService _kdfService;

        // В ASP прийнято конфігурувати контекст як сервіс (у Program.cs),
        // тому переозначається конструктор, що приймає параметр-конфігуратор
        public DataContext(DbContextOptions options, IKdfService kdfService):base(options) 
        {
            _kdfService = kdfService;
        }

        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.UserRole> UserRoles { get; set; }
        public DbSet<Entities.Token> Tokens { get; set; }
        public DbSet<Entities.Group> Groups { get; set; }
        public DbSet<Entities.Manufacturer> Manufacturers { get; set; }
        public DbSet<Entities.Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Product>()
                .HasOne(p => p.Group)
                .WithMany(g => g.Products);

            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Entities.User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            modelBuilder.Entity<Entities.Token>()
                .HasOne(t => t.User)
                .WithMany();


            ///// Сідування /////
            modelBuilder.Entity<Entities.UserRole>()
                .HasData(
                new Entities.UserRole
                {
                    Id = "Admin",
                    Description = "Full access role",
                    CanCreate = 1,
                    CanDelete = 1,
                    CanRead   = 1,
                    CanUpdate = 1
                },
                new Entities.UserRole
                {
                    Id = "User",
                    Description = "Self registered user",
                    CanCreate = 0,
                    CanDelete = 0,
                    CanRead   = 0,
                    CanUpdate = 0
                }
            );
            String salt = "F97AD1A25F4E";
            String dk = _kdfService.Dk("Admin", salt);
            modelBuilder.Entity<Entities.User>()
                .HasData(
                new Entities.User
                {
                    Id = Guid.Parse("5B81869E-E405-4119-AB96-F97AD1A25F4E"),
                    Name =  "Administrator",
                    Email = "admin@change.me",
                    Login = "Admin",
                    Salt = salt,
                    Dk = dk,
                    RoleId = "Admin",
                    RegisterAt = DateTime.MinValue,
                });
        }
    }
}
/*
Shop:

[Group]             [Product]            [Manufacturer]
 Id-----------\      Id                /---Id         
 Name          \-----GroupId          /    Name       
 Description         ManufacturerId--/     Description
 ImageUrl            Name                  ImageUrl   
 DeleteAt            Description           DeleteAt
                     Price               
                     Stock          
                     ImageUrl
                     DeleteAt   
 
 
 
 
 
 
 
 
 */
