namespace ASP_PV411.Models.Home
{
    public class HomeRazorViewModel
    {
        public ICollection<Product> Products { get; set; } = [];
    }

    public class Product
    {
        public string Name { get; set; } = null!;
        public double Price { get; set; }
    }
}
