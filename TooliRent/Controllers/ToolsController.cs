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

		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<ToolDto>>> GetToolById(Guid id, CancellationToken ct)
		{
			var tool = await _toolService.GetByIdAsync(id, ct);
			return Ok(tool);
		}
		[HttpGet("avalible")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetAvalibleTools(CancellationToken ct)
		{
			var tools = await _toolService.GetAvailableAsync(ct);
			return Ok(tools);
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTool(Guid id, [FromBody] UpdateToolDto updateToolDto, CancellationToken ct)
		{
			await _toolService.UpdateAsync(updateToolDto, id, ct);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTool(Guid id, CancellationToken ct)
		{
			await _toolService.DeleteAsync(id, ct);
			return NoContent();
		}

	}
}
