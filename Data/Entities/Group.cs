namespace ASP_PV411.Data.Entities
{
    public record Group
    {
        public Guid Id { get; set; }
        public String Name { get; set;} = null!;
        public String Description { get; set;} = null!;
        public String ImageUrl { get; set; } = null!;
        public String Slug { get; set; } = "";
        public DateTime? DeleteAt { get; set; }

        public ICollection<Product> Products { get; set; } = [];
    }
}
