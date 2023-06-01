﻿using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace AuthInASP.NET.Infrastructure.Localization
{
    public static class LocalizationStartup
    {
        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public static void UseLocalization(this WebApplication app)
        {
            // Configure the HTTP request pipeline.

            IList<CultureInfo> supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-GB"),
                new CultureInfo("de-DE"),
                new CultureInfo("fr"),
                new CultureInfo("fa")
            };

            var requestLocalizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider()
                    {
                        QueryStringKey = "lang",
                        UIQueryStringKey ="ui-lang"

                    },
                    new AcceptLanguageHeaderRequestCultureProvider(),
                    new CookieRequestCultureProvider(),
                }
            };

            /*  If you want use rout culture uncomment code bellow  */
            //var requestProvider = new RouteDataRequestCultureProvider();
            //requestLocalizationOptions.RequestCultureProviders.Insert(0, requestProvider);

            app.UseRequestLocalization(requestLocalizationOptions);

            /*  If you want use rout culture uncomment code bellow  */
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(name: "default", pattern: "{culture=en-US}/{controller=Home}/{action=Index}/{id?}");
            //});

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public static void AddlocalizationConfig(this IServiceCollection services)
        {

            services.AddLocalization(options => options.ResourcesPath = "Resources");

        }
    }

}