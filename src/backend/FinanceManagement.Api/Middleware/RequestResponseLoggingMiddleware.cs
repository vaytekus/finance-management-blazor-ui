using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Serilog.Context;

namespace FinanceManagement.Api.Middleware;

public class RequestResponseLoggingMiddleware
{
    private const int _maxBodyBytes = 32 * 1024;
    private const int _streamReaderBufferSize = 1024;
    private const string _truncatedPlaceholder = "<truncated>";
    private const string _anonymousUser = "anonymous";

    private static readonly string[] _skipPathPrefixes =
    {
        "/swagger", "/favicon", "/SwaggerDark.css"
    };

    private static readonly Regex _sensitiveFieldsRegex = new(
        @"""(password|passwordHash|refreshToken|token|accessToken|email)""\s*:\s*""[^""]*""",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? _anonymousUser;

        using (LogContext.PushProperty("UserId", userId))
        {
            var stopWatch = Stopwatch.StartNew();
            var requestBody = await ReadRequestBodyAsync(context.Request);

            _logger.LogDebug(
                "HTTP {Method} {Path} request body: {RequestBody}",
                context.Request.Method,
                context.Request.Path,
                requestBody);

            var originalResponseBody = context.Response.Body;
            await using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                await _next(context);

                var responseBody = await ReadResponseBodyAsync(context.Response, buffer);

                _logger.LogDebug(
                    "HTTP {Method} {Path} response body: {ResponseBody}",
                    context.Request.Method,
                    context.Request.Path,
                    responseBody);

                buffer.Position = 0;
                await buffer.CopyToAsync(originalResponseBody);
            }
            finally
            {
                context.Response.Body = originalResponseBody;
            }

            stopWatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopWatch.ElapsedMilliseconds);
        }
    }

    private static bool ShouldSkip(PathString path)
    {
        return _skipPathPrefixes.Any(prefix => path.StartsWithSegments(prefix));
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (request.ContentLength is null or 0 || !IsTextContent(request.ContentType))
        {
            return string.Empty;
        }

        if (request.ContentLength > _maxBodyBytes)
        {
            return $"{_truncatedPlaceholder} ({request.ContentLength} bytes)";
        }

        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(
            request.Body,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: _streamReaderBufferSize,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return MaskSensitive(body);
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponse response, MemoryStream buffer)
    {
        if (buffer.Length == 0 || !IsTextContent(response.ContentType))
        {
            return string.Empty;
        }

        if (buffer.Length > _maxBodyBytes)
        {
            return $"{_truncatedPlaceholder} ({buffer.Length} bytes)";
        }

        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();

        return MaskSensitive(body);
    }

    private static bool IsTextContent(string? contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("text", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("xml", StringComparison.OrdinalIgnoreCase)
            || contentType.Contains("x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase);
    }

    private static string MaskSensitive(string body) =>
        _sensitiveFieldsRegex.Replace(body, m => $"\"{m.Groups[1].Value}\":\"***\"");
}
