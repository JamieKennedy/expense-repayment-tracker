using System.Data.SqlClient;
using Contracts;
using Entities.Models;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Repository;
using Microsoft.EntityFrameworkCore;

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
            connectionStringBuilder.UserID = configuration["DbUsername"];
            connectionStringBuilder.Password = configuration["DbPassword"];

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
    }
}