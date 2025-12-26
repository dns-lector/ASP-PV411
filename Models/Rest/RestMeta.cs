namespace ASP_PV411.Models.Rest
{
    public class RestMeta
    {
        public String Service { get; set; } = null!;
        public String Resource { get; set; } = null!;
        public String Method { get; set; } = null!;
        public String Path { get; set; } = null!;
        public String DataType { get; set; } = null!;
        public long ServerTime { get; set; }
        public long Cache { get; set; }
        public Dictionary<String, String> Links { get; set; } = [];
    }
}
