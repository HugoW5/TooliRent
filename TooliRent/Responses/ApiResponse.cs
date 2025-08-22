using Microsoft.AspNetCore.Mvc;

namespace TooliRent.Responses
{
	public class ApiResponse<T>
	{
		public bool IsError { get; set; } = false;
		public string Message { get; set; } = string.Empty;
		public T Data { get; set; } = default!;
		public ProblemDetails? Error { get; set; } = null!;
	}

	// Use if data is not needed in the response.
	public class ApiResponse : ApiResponse<object>
	{
		public ApiResponse() => Data = null!;
	}
}
