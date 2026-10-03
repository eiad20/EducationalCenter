using System.Net;
using System.Text.Json;
using EducationalCenter.Shared.Exceptions;

namespace EducationalCenter.web.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Proceed to the controllers
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the raw error for the developers
            _logger.LogError(ex, "An unhandled exception occurred during the request.");
            
            // Send a clean response to the user
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,
            BadRequestException => (int)HttpStatusCode.BadRequest,
            ConflictException => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        var message = context.Response.StatusCode == 500 && !isDev
            ? "An internal server error occurred."
            : exception.Message;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Error = exception.GetType().Name,
            Message = message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}