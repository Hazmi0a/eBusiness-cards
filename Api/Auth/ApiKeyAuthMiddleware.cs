namespace QRCodePOC.Auth;

public class ApiKeyAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApiKeyAuthMiddleware> _logger;
    
    public ApiKeyAuthMiddleware(RequestDelegate next, ILogger<ApiKeyAuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Check if the request is for Swagger
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/v3/api-docs") ||
                context.Request.Path.StartsWithSegments("/VcardQRcodeRaw"))
            {
                await _next(context);
                return;
            }
            if (!context.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogError("No API key found in request");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            var apiKey = _configuration.GetValue<string>(AuthConstants.ApiKeySectionName);
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("No API key found in configuration");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Internal Server Error");
                return;
            }
            if (!apiKey.Equals(extractedApiKey))
            {
                _logger.LogError("Invalid API key");
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            await _next(context);
        }
        catch (Exception err)
        {
            _logger.LogError("Error checking the Api key: {message}", err.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal Server Error");
            return;
        }
        
    }
}