using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;
namespace MinimalAPI.Common;
public static class Middleware
{
    public static void AddMiddleware(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {

            await next();
        });

        app.Use(async (context, next) =>
        {
            await next();
            if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized) // 401
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new APIResponse
                {
                    Message = "Unauthorized"
                }, options: new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));
            }
        });
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                string errormessage = exceptionHandlerPathFeature?.Error?.Message;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new APIResponse
                {
                    Message = errormessage ?? "Somting went wrong on the server side."
                }, options: new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));
            });
        });
    }
}
