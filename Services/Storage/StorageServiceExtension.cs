namespace ASP_PV411.Services.Storage
{
    public static class StorageServiceExtension
    {
        public static void AddStorage(this IServiceCollection services)
        {
            services.AddSingleton<IStorageService, LocalStorageService>();
        }
    }
}
