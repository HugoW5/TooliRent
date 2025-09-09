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
	}
}
