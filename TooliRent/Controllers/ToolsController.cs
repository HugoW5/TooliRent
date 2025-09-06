using Application.Dto.ToolDtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Responses;

namespace TooliRent.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ToolsController : Controller
	{
		private readonly IToolService _toolService;
		public ToolsController(IToolService toolService)
		{
			_toolService = toolService;
		}

		[HttpGet("all")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetAllTools(CancellationToken ct)
		{
			var tools = await _toolService.GetAllAsync(ct);
			if (tools.IsError)
			{
				return BadRequest(tools);
			}
			else
			{
				return Ok(tools);
			}

		}

	}
}
