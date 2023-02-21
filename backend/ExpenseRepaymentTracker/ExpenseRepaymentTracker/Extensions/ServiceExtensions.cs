using System.Data.SqlClient;
using System.Text;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository.Contracts;
using Service;
using Service.Contracts;

namespace ExpenseRepaymentTracker.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();

        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("SqlConnection"));
            connectionStringBuilder.UserID = configuration["ConnectionStrings:DbUsername"];
            connectionStringBuilder.Password = configuration["ConnectionStrings:DbPassword"];

            var connectionString = connectionStringBuilder.ConnectionString;

            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly("ExpenseRepaymentTracker"));
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 10;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<RepositoryContext>()
                .AddDefaultTokenProviders();

        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];

            if(string.IsNullOrEmpty(secret))
            {
                // Unable to get secret from config
                throw new ArgumentException("Invalid JWT Secret");
            }

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["ValidIssuer"],
                        ValidAudience = jwtSettings["ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                    };
                });
        }
    }
}