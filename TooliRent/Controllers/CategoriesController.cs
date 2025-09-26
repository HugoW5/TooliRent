using Application.Dto.CategoryDtos;
using Application.Services.Interfaces;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Responses;

namespace TooliRent.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class CategoriesController : Controller
	{
		private readonly ICategoryService _categoryService;
		public CategoriesController(ICategoryService service)
		{
			_categoryService = service;
		}

		[HttpGet("all")]
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllCategories(CancellationToken ct)
		{
			var categories = await _categoryService.GetAllAsync(ct);
			if (categories.IsError)
			{
				return BadRequest(categories);
			}
			else
			{
				return Ok(categories);
			}
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<CategoryDto?>>> GetCategoryById(Guid id, CancellationToken ct)
		{
			var category = await _categoryService.GetByIdAsync(id, ct);
			if (category.IsError)
			{
				return BadRequest(category);
			}
			else
			{
				return Ok(category);
			}
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto categoryDto, CancellationToken ct)
		{

			await _categoryService.UpdateAsync(categoryDto, id, ct);
			return Ok();
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken ct)
		{
			await _categoryService.DeleteAsync(id, ct);
			return NoContent();
		}

		[HttpPost("add")]
		[ProducesResponseType(typeof(ApiResponse<Guid?>), StatusCodes.Status201Created)]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<ApiResponse<Guid?>>> AddCategory([FromBody] AddCategoryDto addCategoryDto, CancellationToken ct)
		{
			var addedCategoryId = await _categoryService.AddAsync(addCategoryDto, ct);
			var addedCategory = await _categoryService.GetByIdAsync(addedCategoryId.Value, ct);
			addedCategory.Message = "Category added successfully";
			return CreatedAtAction(nameof(GetCategoryById), new {id = addedCategoryId}, addedCategory);
		}

		[HttpGet("search")]
		[ProducesResponseType(typeof(ApiResponse<IEnumerable<CategoryDto>>), StatusCodes.Status200OK)]
		[Authorize(Roles = "Admin, Member")]
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> SearchCategoriesByName([FromQuery] string name, CancellationToken ct)
		{
			var result = await _categoryService.SearchByNameAsync(name, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}
			return Ok(result);
		}

		[HttpGet("{id}/tools")]
		[Authorize(Roles = "Admin, Member")]
		[ProducesResponseType(typeof(ApiResponse<CategoryWithToolsDto>), StatusCodes.Status200OK)]
		public async Task<ActionResult<ApiResponse<CategoryWithToolsDto?>>> GetCategoryWithTools(Guid id, CancellationToken ct)
		{
			var result = await _categoryService.GetWithToolsAsync(id, ct);
			if (result.IsError)
			{
				return NotFound(result);
			}
			return Ok(result);
		}

	}
}
