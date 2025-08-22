namespace TooliRent.Responses
{
	public class ApiResponse<T>
	{
		public bool IsError { get; set; } = false;
		public string Message { get; set; } = string.Empty;
		public T Data { get; set; } = default!;
		public List<string> Errors { get; set; } = null!;
	}
}
