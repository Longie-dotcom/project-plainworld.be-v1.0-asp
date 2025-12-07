using API.APIException;
using API.Models;
using Application.ApplicationException;
using Application.Helper;
using Domain.DomainException;
using PlainWorld.Authorization;
using System.Text.Json;

namespace API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public GlobalExceptionMiddleware(
            RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "Application/json";
            var response = new ErrorResponse();

            switch (exception)
            {
                // Domain Layer Exceptions - 400 Bad Request
                case 
                InvalidUserAggregateException or
                InvalidRoleAggregateException or
                InvalidPasswordOVException or 
                InvalidChangePassword:
                    ServiceLogger.Warning(
                        Level.API, $"Bad request: {exception.GetType().Name}, detail: {exception.Message}");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Type = "Bad Request";
                    response.Message = exception.Message;
                    break;

                // Domain Layer Exceptions - 400 Bad Request
                case DomainExceptionBase:
                    ServiceLogger.Warning(
                        Level.API, "Domain validation error occurred");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Type = "Domain Error";
                    response.Message = exception.Message;
                    break;

                // Not Found Exceptions - 404 Not Found
                case 
                UserNotFound or 
                UserEmailNotFound or 
                RoleNotFound or 
                PrivilegeNotFound:
                    ServiceLogger.Warning(
                        Level.API, $"Resource not found: {exception.GetType().Name}, detail: {exception.Message}");
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response.Type = "Not Found";
                    response.Message = exception.Message;
                    break;

                // Conflict Exceptions - 409 Conflict
                case 
                UserAlreadyExists or 
                RoleCodeAlreadyExists or 
                PrivilegeAlreadyExists:
                    ServiceLogger.Warning(
                        Level.API, $"Resource conflict: {exception.GetType().Name}, detail: {exception.Message}");
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.Type = "Conflict";
                    response.Message = exception.Message;
                    break;

                // Authentication/Authorization Exceptions - 401 Unauthorized
                case 
                InvalidTokenException or 
                InvalidResetPassword or 
                InvalidRole or 
                InvalidPassword or 
                AuthorizationFailedException or 
                ClaimNotFound or 
                InvalidOwner:
                    ServiceLogger.Warning(
                        Level.API, "Authentication/Authorization error");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Type = "Unauthorized";
                    response.Message = exception.Message;
                    break;

                // Application Layer Exceptions - 500 Internal Server Error
                case ApplicationExceptionBase:
                    ServiceLogger.Warning(
                        Level.API, $"Application error: {exception.GetType().Name}, detail: {exception.Message}");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Type = "Application Error";
                    response.Message = exception.Message;
                    break;

                // Default Exception - 500 Internal Server Error
                default:
                    ServiceLogger.Error(
                        Level.API, exception.Message);
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response.Type = "Internal Server Error";
                    response.Message = "An internal error occurred. Please try again later.";
                    response.Details = exception.ToString();
                    break;
            }

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
