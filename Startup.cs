namespace Meal
{
    using System.Text.Json;
    using Meal.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.StaticFiles;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    public class Startup
    {
        private readonly FileExtensionContentTypeProvider manifestProvider = new FileExtensionContentTypeProvider {Mappings = {[".webmanifest"] = "application/manifest+json"}};

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMemoryCache();
            services.AddResponseCaching();
            services.AddDbContextPool<FoodDbContext>(builder => builder.UseNpgsql(Configuration.GetConnectionString("RubyMeal")));
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "ClientApp/dist");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("GOOGLE:ClientId")))
                };
            });
            services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
            app.UseAuthentication();
            app.UseStaticFiles(new StaticFileOptions {ContentTypeProvider = manifestProvider});
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
                app.UseSpaStaticFiles(new StaticFileOptions {ContentTypeProvider = manifestProvider});
                app.UseSpa(spa => spa.Options.SourcePath = "ClientApp");
            }
        }
    }
}