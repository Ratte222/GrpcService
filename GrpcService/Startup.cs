using BLL.Interfaces;
using BLL.Services;
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
using GrpcService.AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BLL.Helpers;
using Microsoft.AspNetCore.Identity;
using DAL.Model;
using GrpcService.Services;

namespace GrpcService
{
    //https://medium.com/@jnewmano/grpc-postman-173b62a64341 //does not fit 
    //https://docs.microsoft.com/ru-ru/aspnet/core/grpc/test-tools?view=aspnetcore-5.0
    //https://github.com/fullstorydev/grpcui#installation //its work
    //https://github.com/fullstorydev/grpcurl //its work
    //https://github.com/uw-labs/bloomrpc //does not work
    //https://www.youtube.com/watch?v=DNxdvRQ4qRQ //usefully video about gRPC
    //https://grpc.io/docs/languages/csharp/basics/
    //https://grpc.github.io/grpc/csharp-dotnet/api/Grpc.AspNetCore.Server.Model.html
    //https://developers.google.com/protocol-buffers/docs/proto3#simple
    //https://docs.microsoft.com/ru-ru/aspnet/core/grpc/protobuf?view=aspnetcore-5.0 //datatype proto and c#
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

            var appSettingsSection = Configuration.GetSection("AppSettings");

            services.AddSingleton<AppSettings>(appSettingsSection
                .Get<AppSettings>());

            #region JWT
            services.AddIdentity<User, IdentityRole>()
               .AddEntityFrameworkStores<AppDBContext>()
               .AddDefaultTokenProviders(); ;

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
            });
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = System.Text.Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
          .AddJwtBearer(options =>
          {
              options.RequireHttpsMetadata = true;//if false - do not use SSl
              options.SaveToken = true;
              options.Events = new JwtBearerEvents()
              {
                  OnAuthenticationFailed = (ctx) =>
                  {
                      if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                      {
                          ctx.Response.StatusCode = 401;
                      }

                      return Task.CompletedTask;
                  },
                  OnForbidden = (ctx) =>
                  {
                      if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                      {
                          ctx.Response.StatusCode = 403;
                      }

                      return Task.CompletedTask;
                  }
              };
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  // specifies whether the publisher will be validated when validating the token 
                  ValidateIssuer = true,
                  // a string representing the publisher
                  ValidIssuer = appSettings.Issuer,

                  // whether the consumer of the token will be validated 
                  ValidateAudience = true,
                  // token consumer setting 
                  ValidAudience = appSettings.Audience,
                  // whether the lifetime will be validated 
                  ValidateLifetime = true,

                  // security key installation 
                  //IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                  IssuerSigningKey = new SymmetricSecurityKey(key),
                  // security key validation 
                  ValidateIssuerSigningKey = true,
              };
          });
            //--------- JWT settingd ---------------------
            #endregion

            services.AddAuthorization();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddScoped<IProductService, BLL.Services.ProductService>();
            services.AddScoped<IProductPhotoService, BLL.Services.ProductPhotoService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDBContext applicationContext,
            UserManager<User> userManager)
        {
            applicationContext.Database.Migrate();
            DbInitializer.Initialize(applicationContext, userManager);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<AccountService>();
                endpoints.MapGrpcService<GrpcService.Services.ProductService>();
                endpoints.MapGrpcService<GrpcService.Services.ProductPhotoService>();
                endpoints.MapGrpcService<FileService>();
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
