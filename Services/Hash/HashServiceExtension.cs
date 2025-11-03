namespace ASP_PV411.Services.Hash
{
    public static class HashServiceExtension
    {
        public static void AddHash(this IServiceCollection services)
        {
            // services.AddSingleton<IHashService, Md5HashService>();
            services.AddSingleton<IHashService, Sha1HashService>();
        }
    }
}
