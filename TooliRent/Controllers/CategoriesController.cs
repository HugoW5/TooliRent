using Application.Dto.CategoryDtos;
using Application.Services.Interfaces;
using Domain.Interfaces.Repositories;
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
		public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllCategories(CancellationToken ct)
		{
			var categories = await _categoryService.GetAllAsync(ct);
			if(categories.IsError)
			{
				return BadRequest(categories);
			}
			else
			{
				return Ok(categories);
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ApiResponse<CategoryDto?>>> GetCategoryById(Guid id, CancellationToken ct)
		{
			var category = await _categoryService.GetByIdAsync(id, ct);
			if(category.IsError)
			{
				return BadRequest(category);
			}
			else
			{
				return Ok(category);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto categoryDto, CancellationToken ct)
		{

				await _categoryService.UpdateAsync(categoryDto, id, ct);
				return Ok();
		}

	}
}
