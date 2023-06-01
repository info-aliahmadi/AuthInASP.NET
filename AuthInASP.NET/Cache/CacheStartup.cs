namespace AuthInASP.NET.Cache
{
    public static class CacheStartup
    {
        public static void AddCacheProvider(this IServiceCollection services,
            IConfiguration configuration)
        {
            string cacheProvider = configuration.GetSection("CacheProvider").Value;
            if (!string.IsNullOrEmpty(cacheProvider))
            {
                const string providerName = "DefaultAuth";
                services.AddEFCacheProvider(providerName);

                if (cacheProvider == "inmemory")
                {
                    services.AddInMemoryCacheProvider(configuration, providerName);
                }
            }
            else
            {
                services.AddInMemoryEFCacheProvider();
            }
        }

    }
}