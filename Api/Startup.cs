using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using Plantagoo.Api.Extensions;
using Plantagoo.Api.Middleware;
using Plantagoo.Authentication;
using Plantagoo.AutoMapper;
using Plantagoo.Data;
using Plantagoo.DTOs.Projects;
using Plantagoo.Encryption;
using Plantagoo.Filtering;
using Plantagoo.Interfaces;
using Plantagoo.Services;
using System.Text;

namespace Plantagoo.Api
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
            services.AddControllers();
            services.AddApiVersioning();

            //SwaggerUI
            services.AddSwaggerDocumentation();

            //Encryption & Token
            services.AddSingleton<IPasswordHasher, PBKDF2Hasher>();
            services.AddSingleton<ITokenHelper, JWTHelper>();

            //Paging & Sorting on Web-Request
            services.AddScoped<IFilterHelper<ProjectDetailsDTO>, FilterHelper<ProjectDetailsDTO>>();

            //Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITagService, TagService>();

            //Automapper
            services.AddSingleton(provider => new MapperConfiguration(options =>
            {
                options.AddProfile(new MappingProfile(provider.GetService<IPasswordHasher>()));
            })
            .CreateMapper());

            //Authentication
            var tokenConfiguration = Configuration.GetSection("TokenSettings");
            services.Configure<TokenSettings>(tokenConfiguration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var tokenSettings = tokenConfiguration.Get<TokenSettings>();
                var symmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSettings.Secret));
                var keyExpiration = tokenSettings.AccessExpirationInMinutes;

                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricKey,
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                };
            });

            //Authorization
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            //EF
            var builder = new MySqlConnectionStringBuilder(Configuration.GetConnectionString("DbConnection"))
            {
                Password = Configuration["DB:Password"],
                UserID = Configuration["DB:Username"],
            };

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseMySQL(builder.ConnectionString);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseGlobalExceptionMiddleware();
            }

            app.UseSwaggerDocumentation();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(option => option
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
