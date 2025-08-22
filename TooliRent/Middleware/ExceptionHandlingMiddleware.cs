using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using TooliRent.Responses;

namespace TooliRent.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
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
                await HandleExceptionAsync(context, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {

            var apiResponse = new ApiResponse
            {
                IsError = true,
				Error = new ProblemDetails
				{
					Type = "https://tools.ietf.org/html/rfc7807#section-3",
					Title = "An unexpected error occurred.",
					Status = (int)HttpStatusCode.InternalServerError,
					Detail = exception.Message,
					Instance = context.Request.Path
				}
			};

			context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = apiResponse.Error.Status ?? 500;
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, options));
        }
    }
}
