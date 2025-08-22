using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TooliRent.Responses;

namespace TooliRent.Middleware
{
	public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
	{

		public async ValueTask<bool> TryHandleAsync(
		  HttpContext httpContext,
		  Exception exception,
		  CancellationToken cancellationToken)
		{
			//Log exception
			logger.LogError(exception, "An unhandled exception occurred.");
			//Error Response without any data
			var apiResponse = new ApiResponse
			{
				IsError = true,
				Message = "An unexpected error occurred. Please try again later.",
				Error = new ProblemDetails
				{
					Type = "https://httpstatuses.org/500",
					Title = "An error occurred",
					Status = StatusCodes.Status400BadRequest,
					Detail = exception.Message
				}
			};

			httpContext.Response.StatusCode = apiResponse.Error.Status.Value;

			await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);

			return true;
		}
	}

}