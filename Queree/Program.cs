using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Queree.Seed;

namespace Queree
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Program>(); });

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddControllers();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerGen(options => { options.SwaggerDoc("api", new OpenApiInfo()); });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                db.Database.EnsureCreated();
                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }

                db.SeedUsers();
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/api/swagger.json", "api");
                options.RoutePrefix = "";
            });
            app.UseEndpoints(builder => builder.MapControllers());
        }
    }
}