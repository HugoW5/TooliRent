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
		[HttpPost]
		public async Task<ActionResult<ApiResponse<ToolDto>>> AddTool([FromBody] AddToolDto addToolDto, CancellationToken ct)
		{
			var toolId = await _toolService.AddAsync(addToolDto, ct);
			var toolDto = await _toolService.GetByIdAsync(toolId.Value, ct);
			toolDto.Message = "Tool created successfully"; // Set the correct message for creation
			return CreatedAtAction(nameof(GetToolById), new { id = toolId }, toolDto);

		}
		[HttpGet("avalible")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetAvalibleTools(CancellationToken ct)
		{
			var tools = await _toolService.GetAvailableAsync(ct);
			return Ok(tools);
		}

		[HttpGet("search")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> SearchTools([FromQuery] string searchQuery, CancellationToken ct)
		{
			var tools = await _toolService.SearchByNameAsync(searchQuery, ct);
			return Ok(tools);
		}
		[HttpGet("category/{categoryId}")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetToolsByCategory(Guid categoryId, CancellationToken ct)
		{
			var tools = await _toolService.GetByCategoryAsync(categoryId, ct);
			return Ok(tools);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTool(Guid id, [FromBody] UpdateToolDto updateToolDto, CancellationToken ct)
		{
			await _toolService.UpdateAsync(updateToolDto, id, ct);
			return Ok();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTool(Guid id, CancellationToken ct)
		{
			await _toolService.DeleteAsync(id, ct);
			return NoContent();
		}

	}
}
