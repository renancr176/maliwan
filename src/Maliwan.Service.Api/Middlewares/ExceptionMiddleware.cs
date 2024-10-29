using Maliwan.Service.Api.Exceptions;
using Maliwan.Service.Api.Models.Responses;
using Maliwan.Service.Api.Models;
using Newtonsoft.Json;
using Serilog;
using System.Net;
using Serilog.Context;

namespace Maliwan.Service.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        try
        {
            //var log = context.RequestServices.GetService<ILog>();


            string errorId = Guid.NewGuid().ToString();
            LogContext.PushProperty("ErrorId", errorId);
            LogContext.PushProperty("StackTrace", exception.StackTrace);
            var errorResult = new ErrorResult
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message.Trim(),
                ErrorId = errorId,
                SupportMessage = $"Provide the Error Id: {errorId} to the support team for further analysis."
            };
            errorResult.Messages.Add(exception.Message);

            if (exception is not CustomException && exception.InnerException != null)
            {
                while (exception.InnerException != null)
                {
                    exception = exception.InnerException;
                }
            }

            switch (exception)
            {
                case CustomException e:
                    errorResult.StatusCode = (int)e.StatusCode;
                    if (e.ErrorMessages is not null)
                    {
                        errorResult.Messages = e.ErrorMessages;
                    }

                    break;

                case KeyNotFoundException:
                    errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            Serilog.Log.Error($"{errorResult.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
            var response = context.Response;
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = errorResult.StatusCode;
                await response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse<ErrorResult>()
                {
                    Errors = new List<BaseResponseError>()
                    {
                        new BaseResponseError()
                        {
                            ErrorCode = $"{(HttpStatusCode)errorResult.StatusCode}",
                            Message = exception.Message
                        }
                    }
                }));
            }
            else
            {
                Serilog.Log.Warning("Can't write error response. Response has already started.");
            }

            //await log.LogAsync(exception, context.TraceIdentifier);
            //await log.LogAsync(
            //    LogLevelEnum.Error,
            //    errorResult,
            //    context.TraceIdentifier);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}