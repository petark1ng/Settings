using System.Net;
using System.Text.Json;
using Settings.Contracts.Exceptions;

namespace Settings.Api.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            HttpResponse httpResponse = context.Response;
            httpResponse.ContentType = "application/json";

            httpResponse.StatusCode = ex switch
            {
                AlreadyExistsException _ => (int)HttpStatusCode.Conflict,
                NotAllowedException _ => (int)HttpStatusCode.MethodNotAllowed,
                NotFoundException _ => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError,
            };
            string result = JsonSerializer.Serialize(new { message = ex?.Message });

            await httpResponse.WriteAsync(result);
        }
    }
}
