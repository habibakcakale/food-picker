using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Meal.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Meal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDbContextPool<FoodDbContext>(builder => builder.UseSqlite("Data Source=food.db"));
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Events.OnRedirectToLogin += context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return Task.CompletedTask;
                };
            }).AddGoogle(options =>
            {
                options.ClientId = Configuration.GetValue<string>("GOOGLE_ClientId");
                options.ClientSecret = Configuration.GetValue<string>("GOOGLE_ClientSecret");;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
            app.UseAuthentication();
            var provider = new FileExtensionContentTypeProvider {Mappings = {[".webmanifest"] = "application/manifest+json"}};
            app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = provider});
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSpa(spa => spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"));
            }
            else
            {
                app.UseSpaStaticFiles(new StaticFileOptions {ContentTypeProvider = provider});
                app.UseSpa(spa => spa.Options.SourcePath = "ClientApp");
            }
        }
    }
}