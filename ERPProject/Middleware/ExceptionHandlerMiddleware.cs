using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Exceptions;
using System.Text.Json;

namespace ERPProject.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        ApiResponseDto<object> response;

        if (exception is AppException appEx)
        {
            context.Response.StatusCode = appEx.StatusCode;
            response = new ApiResponseDto<object>
            {
                Status = appEx.StatusCode,
                Message = appEx.Message,
                Errors = appEx.Errors
            };
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            response = new ApiResponseDto<object>
            {
                Status = 500,
                Message = "Internal Server Error",
                Errors = new List<string> { exception.Message }
            };
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return context.Response.WriteAsync(jsonResponse);
    }

}
