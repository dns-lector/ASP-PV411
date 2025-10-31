namespace ASP_PV411.Services.Random
{
    public class RandomService : IRandomService
    {
        private static readonly System.Random _random = new();

        public int RandomInt() => _random.Next();
    }

   
}
