using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StorageService.Application.Exceptions;

namespace StorageService.API.Middlewares;

public class GlobalExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var statusCode = ex switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                AlreadyExistsException => StatusCodes.Status400BadRequest,
                ValidationFailedException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = ex switch
            {
                ValidationFailedException validationEx => new HttpValidationProblemDetails
                {
                    Title = "Error",
                    Type = ex.GetType().Name,
                    Errors = validationEx.Failures
                        .GroupBy(f => f.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.ErrorMessage).ToArray()
                        ),
                    Status = statusCode,
                    Instance = context.Request.Path
                },

                _ => new ProblemDetails
                {
                    Title = statusCode == StatusCodes.Status500InternalServerError ? "Internal Server Error" : "Error",
                    Type = ex.GetType().Name,
                    Status = statusCode,
                    Detail = ex.Message,
                    Instance = context.Request.Path
                }
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problemDetails);

            await context.Response.WriteAsync(json);
        }
    }
}
