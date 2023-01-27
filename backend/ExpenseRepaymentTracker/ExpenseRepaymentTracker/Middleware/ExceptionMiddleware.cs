using Common.DataTransferObjects;
using Common.Exceptions;
using Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace ExpenseRepaymentTracker.Middleware
{
    public static class ExceptionMiddleware
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        // Exception has been thrown
                        Exception error = contextFeature.Error;

                        int statusCode = GetStatusCodeFromException(error);
                        string message = GetErrorMessageFromException(error);

                        ErrorDto errorDto = new ErrorDto(StatusCode: statusCode, message);

                        logger.LogError(error.ToString());

                        context.Response.StatusCode = statusCode;

                        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDto, Formatting.Indented));

                    }
                });
            });
        }

        // Only returns the exceptions message, but can be expanded if needed
        private static string GetErrorMessageFromException(Exception error)
        {
            return error.Message;
        }

        private static int GetStatusCodeFromException(Exception error)
        {
            return error switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}