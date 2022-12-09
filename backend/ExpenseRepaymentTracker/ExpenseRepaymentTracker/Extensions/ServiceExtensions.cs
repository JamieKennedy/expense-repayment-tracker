using Contracts;
using LoggerService;

namespace ExpenseRepaymentTracker.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();

        }
    }
}