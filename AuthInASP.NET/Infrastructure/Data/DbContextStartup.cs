﻿using Microsoft.EntityFrameworkCore;
using EFCoreSecondLevelCacheInterceptor;

namespace AuthInASP.NET.Infrastructure.Data
{
    public static class ControllerStartup
    {
        public static void AddDbContextConfig(this IServiceCollection services,
            IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // the default pool size in 1024 
            //Make sure that the maxPoolSize corresponds to your usage scenario;
            //if it is too low, DbContext instances will be constantly created and disposed,degrading performance.
            //Setting it too high may needlessly consume memory as
            //unused DbContext instances are maintained in the pool.
            services.AddDbContext<IdentityContext>((serviceProvider, options) =>
                options.UseSqlServer(connectionString
                //,builder =>
                //{
                //    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                //}
                )
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>())
            ); // the default pool size in 1024 

            services.AddDatabaseDeveloperPageExceptionFilter();

        }
    }
}