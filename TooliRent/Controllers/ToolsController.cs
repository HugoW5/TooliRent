using Application.Dto.ToolDtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<ToolDto>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize(Roles = "Admin, Member")]
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
		[ProducesResponseType(typeof(ApiResponse<ToolDto>), StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<ToolDto>>> GetToolById(Guid id, CancellationToken ct)
		{
			var tool = await _toolService.GetByIdAsync(id, ct);
			return Ok(tool);
		}

		[HttpPost]
		[ProducesResponseType(typeof(ApiResponse<ToolDto>), StatusCodes.Status201Created)]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<ToolDto>>> AddTool([FromBody] AddToolDto addToolDto, CancellationToken ct)
		{
			var toolId = await _toolService.AddAsync(addToolDto, ct);
			var toolDto = await _toolService.GetByIdAsync(toolId.Value, ct);
			toolDto.Message = "Tool created successfully"; // Set the correct message for creation
			return CreatedAtAction(nameof(GetToolById), new { id = toolId }, toolDto);

		}
		[HttpGet("avalible")]
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<ToolDto>>), StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetAvalibleTools(CancellationToken ct)
		{
			var tools = await _toolService.GetAvailableAsync(ct);
			return Ok(tools);
		}

		[HttpGet("search")]
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<ToolDto>>), StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> SearchTools([FromQuery] string searchQuery, CancellationToken ct)
		{
			var tools = await _toolService.SearchByNameAsync(searchQuery, ct);
			return Ok(tools);
		}

		[HttpGet("category/{categoryId}")]
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<ToolDto>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<IEnumerable<ToolDto>>>> GetToolsByCategory(Guid categoryId, CancellationToken ct)
		{
			var tools = await _toolService.GetByCategoryAsync(categoryId, ct);
			return Ok(tools);
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateTool(Guid id, [FromBody] UpdateToolDto updateToolDto, CancellationToken ct)
		{
			await _toolService.UpdateAsync(updateToolDto, id, ct);
			return Ok();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteTool(Guid id, CancellationToken ct)
		{
			await _toolService.DeleteAsync(id, ct);
			return NoContent();
		}

	}
}
