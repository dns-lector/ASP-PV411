namespace ASP_PV411.Services.Random
{
    public static class RandomServiceExtension
    {
        public static void AddRandom(this IServiceCollection services)
        {
            // binding - зв'язування інтерфейсу з класом
            // "будуть питати IRandomService - видати RandomService"
            services.AddSingleton<IRandomService, NewRandomService>();
        }
    }
}
