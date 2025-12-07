using Application.ApplicationException;
using Microsoft.AspNetCore.SignalR;
using SignalR.Model;
using System.Text.Json;

namespace SignalR.Middleware
{
    public class HubGlobalExceptionHandler : IHubFilter
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public HubGlobalExceptionHandler()
        {
        }

        #region Methods
        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext context,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
                return await next(context); // runs your hub & service logic
            }
            catch (Exception exception)
            {
                var response = new ErrorResponse();

                switch (exception)
                {
                    // Domain Layer Exceptions - 400 Bad Request
                    //case :
                    //    response.Type = "Bad Request";
                    //    response.Message = exception.Message;
                    //    break;

                    // Not Found Exceptions - 404 Not Found
                    //case :
                    //    response.Type = "Not Found";
                    //    response.Message = exception.Message;
                    //    break;

                    // Conflict Exceptions - 409 Conflict
                    //case :
                    //    response.Type = "Conflict";
                    //    response.Message = exception.Message;
                    //    break;

                    // Authentication/Authorization Exceptions - 401 Unauthorized
                    //case :
                    //    response.Type = "Unauthorized";
                    //    response.Message = exception.Message;
                    //    break;

                    // Application Layer Exceptions - 500 Internal Server Error
                    case ApplicationExceptionBase:
                        response.Type = "Application Error";
                        response.Message = exception.Message;
                        break;

                    // Default Exception - 500 Internal Server Error
                    default:
                        response.Type = "Internal Server Error";
                        response.Message = "An internal error occurred. Please try again later.";
                        response.Details = exception.ToString();
                        break;
                }

                throw new HubException(JsonSerializer.Serialize(response));
            }
        }
        #endregion
    }
}
