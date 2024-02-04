using System;
using Microsoft.AspNetCore.Diagnostics;
using RoommateMatcher.Dtos;
using RoommateMatcher.Exceptions;

namespace RoommateMatcher.Middlewares
{
    public static class UseCustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(config =>
            {
                config.Run(async (context) =>
                {
                    context.Response.ContentType = "application/json";

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var statusCode = exceptionFeature.Error switch
                    {
                        ClientSideException => 400,
                        DirectoryNotFoundException => 404,
                        _ => 500
                    };

                    context.Response.StatusCode = statusCode;

                    var response = CustomResponseDto<TokenDto>.Fail(statusCode, new List<string>() { exceptionFeature.Error.Message });

                    await context.Response.WriteAsJsonAsync(response);
                });
            });
        }
    }
}

