using CSharpWars.Common.DependencyInjection;
using CSharpWars.Web.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CSharpWars.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigurationHelper(c =>
            {
                c.ConnectionString = _configuration.GetValue<string>("CONNECTION_STRING");
                c.ArenaSize = _configuration.GetValue<int>("ARENA_SIZE");
                if (string.IsNullOrEmpty(_configuration.GetValue<string>("TYE")))
                {
                    c.ValidationHost = _configuration.GetValue<string>("VALIDATION_HOST");
                }
                else
                {
                    var uri = _configuration.GetServiceUri("cshwarpwars-validator");
                    c.ValidationHost = $"{uri}";
                }
                c.PointsLimit = _configuration.GetValue<int>("POINTS_LIMIT");
                c.BotDeploymentLimit = _configuration.GetValue<int>("BOT_DEPLOYMENT_LIMIT");
            });

            services.ConfigureWeb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UsePathBase("/web");
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}