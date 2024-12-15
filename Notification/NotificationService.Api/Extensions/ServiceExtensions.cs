using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace NotificationService.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");

                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    if (exception != null)
                    {
                        logger.LogError($"An unhandled exception occurred: {exception.Message}");

                        context.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                        context.Response.ContentType = "application/json";

                        var errorResponse = new
                        {
                            message = "An unexpected error occurred. Please try again later.",
                            details = exception.Message
                        };

                        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                        var errorJson = JsonSerializer.Serialize(errorResponse, options);

                        await context.Response.WriteAsJsonAsync(errorResponse);
                        logger.LogDebug("Error response sent: {ErrorResponse}", errorJson);
                    }
                    else
                    {
                        logger.LogWarning("Exception handler triggered but no exception was found.");
                    }
                });
            });

            return app;
        }
    }

}
