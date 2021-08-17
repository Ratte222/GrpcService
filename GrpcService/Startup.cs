using DAL.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService
{
    //https://medium.com/@jnewmano/grpc-postman-173b62a64341 //does not fit 
    //https://docs.microsoft.com/ru-ru/aspnet/core/grpc/test-tools?view=aspnetcore-5.0
    //https://github.com/fullstorydev/grpcui#installation //does not work
    //https://github.com/fullstorydev/grpcurl //its work
    //https://github.com/uw-labs/bloomrpc
    //https://www.youtube.com/watch?v=DNxdvRQ4qRQ
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddGrpcReflection();
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDBContext>(options => options.
                UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 26))), ServiceLifetime.Transient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext applicationContext)
        {
            applicationContext.Database.Migrate();
            DbInitializer.Initialize(applicationContext);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<ProductService>();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
                if (env.IsDevelopment())
                {
                    endpoints.MapGrpcReflectionService();
                }
            });
        }
    }
}
