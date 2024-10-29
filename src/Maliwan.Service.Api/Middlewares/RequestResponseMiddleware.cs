using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace Maliwan.Service.Api.Middlewares;

public class RequestResponseMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseMiddleware> _logger;

    public RequestResponseMiddleware(RequestDelegate next, ILogger<RequestResponseMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.EnableBuffering();

        //var log = httpContext.RequestServices.GetService<ILog>();

        object request = null;
        object response = null;

        #region LogAsync Requests

        using (var reader = new StreamReader(
            httpContext.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            leaveOpen: true))
        {
            try
            {
                var body = await reader.ReadToEndAsync();

                if (body.ToLower().Contains("password"))
                {
                    var json = JsonConvert.DeserializeObject<JObject>(body);
                    json.Property("password")?.Remove();
                    json.Property("Password")?.Remove();
                    body = JsonConvert.SerializeObject(json);
                }

                request = new
                {
                    httpContext.Request.Method,
                    httpContext.Request.Scheme,
                    httpContext.Request.Host,
                    httpContext.Request.Path,
                    httpContext.Request.QueryString,
                    httpContext.Request.ContentType,
                    Body = body
                };
            }
            catch (Exception e)
            {
                _logger.LogError(1, e, e.Message);
            }
            finally
            {
                // Reset the request body stream position so the next middleware can read it
                httpContext.Request.Body.Position = 0;
            }
        }

        #endregion

        #region LogAsync Responses

        Stream originalBody = httpContext.Response.Body;

        try
        {
            using (var memStream = new MemoryStream())
            {
                httpContext.Response.Body = memStream;

                await _next(httpContext);

                memStream.Position = 0;
                string responseBody = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);

                response = new
                {
                    httpContext.Response.StatusCode,
                    httpContext.Response.ContentType,
                    Body = responseBody
                };
            }
        }
        catch (Exception e)
        {
            _logger.LogError(1, e, e.Message);
        }
        finally
        {
            // Reset the request body stream position so the next middleware can read it
            httpContext.Response.Body = originalBody;
        }

        #endregion

        try
        {
            var logData = new
            {
                User = new
                {
                    Id = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UserName = httpContext.User.FindFirstValue(ClaimTypes.Name)
                },
                Request = request,
                Response = response
            };

            _logger.LogInformation(JsonConvert.SerializeObject(logData));

            //var logLevel = httpContext.Response.StatusCode >= 200 && httpContext.Response.StatusCode <= 299
            //    ? LogLevelEnum.Information
            //    : LogLevelEnum.Error;

            //await log.LogAsync(logLevel, logData, httpContext.TraceIdentifier, "mvno-request-response-{0:yyyy.MM}");
        }
        catch (Exception e)
        {
            _logger.LogError(1, e, e.Message);
        }
    }
}

public static class RequestResponseMiddlewareExtension
{
    public static IApplicationBuilder UseRequestResponseMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestResponseMiddleware>();
    }
}