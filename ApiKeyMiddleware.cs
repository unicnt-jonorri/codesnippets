public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKey = configuration["ApiKey"] ?? throw new ArgumentNullException();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var providedKey = context.Request.Headers["X-API-Key"].FÞirstOrDefault();

        if (providedKey != _apiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}