namespace ASP_PV411.Data.Entities
{
    public class Discount
    {
        public Guid Id { get; set; }
        public String Description { get; set; } = null!;
        public DateTime StartAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public double? Percent { get; set; }
    }
}
