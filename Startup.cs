using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using shitchan.Repositories;

namespace shitchan
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
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                          builder =>
                          {
                              builder.WithOrigins("https://lazpeng.github.io", "https://localhost");
                              builder.AllowAnyMethod();
                              builder.AllowAnyHeader();
                          });
            });
            services.AddControllers();

            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") ?? Configuration.GetConnectionString("Local");
            services.AddSingleton(new ConnectionStringProvider(connectionString));

            switch (Configuration.GetValue<string>("Database").ToUpper())
            {
                case "POSTGRESQL":
                    services.AddScoped<IBoardRepository, Repositories.PostgreSQL.BoardRepository>();
                    services.AddScoped<IThreadRepository, Repositories.PostgreSQL.ThreadRepository>();
                    services.AddScoped<IAdminRepository, Repositories.PostgreSQL.AdminRepository>();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
