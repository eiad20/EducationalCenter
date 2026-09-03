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

        // Map the custom exceptions to HTTP status codes
        context.Response.StatusCode = exception switch
        {
            NotFoundException => (int)HttpStatusCode.NotFound,        // 404
            BadRequestException => (int)HttpStatusCode.BadRequest,    // 400
            ConflictException => (int)HttpStatusCode.Conflict,        // 409
            _ => (int)HttpStatusCode.InternalServerError              // 500
        };

        // Create the standardized JSON layout
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Error = exception.GetType().Name,
            Message = exception.Message
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(json);
    }
}