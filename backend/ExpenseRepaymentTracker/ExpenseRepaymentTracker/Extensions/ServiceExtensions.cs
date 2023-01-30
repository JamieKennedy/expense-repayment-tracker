using System.Data.SqlClient;
using Contracts;
using LoggerService;
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
                options.UseSqlServer(connectionString);
            });
        }
    }
}