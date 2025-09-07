using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using System.Text.Json;
using TooliRent.Exceptions;
using Responses;
using FluentValidation;

namespace TooliRent.Middleware
{
	//https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-from-middleware-to-modern-handlers
	//https://www.milanjovanovic.tech/blog/global-error-handling-in-aspnetcore-from-middleware-to-modern-handlers#chaining-exception-handlers
	//TODO add FluentValidation validation-exception-handler
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
			//Add problem details to this response
			var apiResponse = new ApiResponse
			{
				IsError = true
			};

			//Map exception to status code and title
			var (statusCode, title) = MapException(exception);

			//special handling for IdentityException
			if (exception is IdentityException identityEx)
			{
				apiResponse.Error = new ProblemDetails
				{
					Type = "about:blank",
					Title = title,
					Status = statusCode,
					Detail = string.Join("; ", identityEx.Errors), // Get the errors from the IdentityException
					Instance = context.Request.Path
				};
			}
			else if (exception is ValidationException validationEx)
			{
				// Combine all validation failures into a single string
				var errors = validationEx.Errors
					.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
					.ToArray();

				apiResponse.Error = new ProblemDetails
				{
					Type = "about:blank",
					Title = title,
					Status = statusCode,
					Detail = string.Join("; ", errors),
					Instance = context.Request.Path
				};
			}
			else
			{
				apiResponse.Error = new ProblemDetails
				{
					Type = "about:blank",
					Title = title,
					Status = statusCode,
					Detail = exception.Message,
					Instance = context.Request.Path
				};
			}

			context.Response.ContentType = "application/problem+json";
			context.Response.StatusCode = apiResponse.Error.Status ?? 500;
			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, options));
		}

		private static (int statusCode, string title) MapException(Exception ex) =>
			ex switch
			{
				IdentityException => (StatusCodes.Status400BadRequest, "Identity operation failed"),
				ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument"),
				ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
				UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
				KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
				_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
			};
	}
}
