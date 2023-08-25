using IEM.Application.Models.Exceptions;
using IEM.Application.Utils;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IEM.Application.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate RequestDelegate;
        private readonly ILogger<ExceptionHandlerMiddleware> Logger;
        private readonly IHostEnvironment Env;

        public ExceptionHandlerMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlerMiddleware> logger, IHostEnvironment env)
        {
            RequestDelegate = requestDelegate;
            Logger = logger;
            Env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await RequestDelegate(context);
            }
            catch (Exception exception)
            {
                int statusCode;
                string message = GetErrorMessage(exception);
                string detail = Env.IsDevelopment() ? GetStackTrace(exception) : string.Empty;
                int errorCode;
                switch (exception)
                {
                    case SecurityTokenExpiredException _:
                        message = "SecurityToken has expired";
                        statusCode = StatusCodes.Status401Unauthorized;
                        errorCode = StatusCodes.Status401Unauthorized;
                        break;
                    case SecurityTokenValidationException _:
                    case UnauthorizedAccessException _:
                        message = "You are not authorized";
                        statusCode = StatusCodes.Status401Unauthorized;
                        errorCode = StatusCodes.Status401Unauthorized;
                        break;

                    case DbUpdateConcurrencyException _:
                        statusCode = StatusCodes.Status409Conflict;
                        errorCode = (int)StatusCodes.Status409Conflict;
                        break;

                    case ArgumentException _:
                    case DbUpdateException _:
                        statusCode = StatusCodes.Status500InternalServerError;
                        errorCode = (int)StatusCodes.Status500InternalServerError;
                        break;

                    case DomainException e:
                        statusCode = StatusCodes.Status400BadRequest;
                        errorCode = e.ErrorCode;
                        break;

                    case HttpException e:
                        statusCode = (int)e.StatusCode;
                        errorCode = (int)e.StatusCode;
                        break;

                    case AntiforgeryValidationException _:
                        statusCode = StatusCodes.Status400BadRequest;
                        errorCode = (int)statusCode;
                        break;

                    default:
                        if (string.IsNullOrEmpty(message))
                        {
                            message = "Some unknown error occurred";
                        }
                        statusCode = StatusCodes.Status500InternalServerError;
                        errorCode = (int)statusCode;
                        break;
                }
                Logger.LogError(exception, exception.Message, exception.StackTrace);
                await context.CreateJsonResponseAsync(statusCode, errorCode, message, detail);
            }
        }

        private string GetErrorMessage(Exception exception)
        {
            var errorMessage = string.Empty;
            if (exception.Message != null)
            {
                errorMessage = exception.Message;
            }
            while (exception.InnerException != null)
            {
                errorMessage = exception.InnerException.Message;
                exception = exception.InnerException;
            }
            return errorMessage.ToString();
        }

        private string GetStackTrace(Exception exception)
        {
            var stackTrace = new StringBuilder();
            if (exception.StackTrace != null)
            {
                stackTrace.Append(exception.StackTrace);
            }
            while (exception.InnerException != null)
            {
                stackTrace.Append(exception.InnerException.StackTrace ?? string.Empty);
                exception = exception.InnerException;
            }
            return stackTrace.ToString();
        }
    }
}
