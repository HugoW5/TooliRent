using Application.Dto.CategoryDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
	public class CategoryService : ICategoryService
	{
		private readonly ICategoryRepository _repo;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;

		public CategoryService(ICategoryRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
		{
			_repo = repo;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}
		public async Task<Guid?> AddAsync(AddCategoryDto addCategoryDto, CancellationToken ct = default)
		{
			var category = _mapper.Map<Category>(addCategoryDto);
			var addedCategoryId = await _repo.AddAsync(category, ct);

			if (addedCategoryId == null)
			{
				throw new Exception("Failed to add the category.");
			}

			await _unitOfWork.SaveChangesAsync(ct);
			return addedCategoryId;
		}
		public async Task UpdateAsync(UpdateCategoryDto categoryDto, Guid categoryId, CancellationToken ct = default)
		{
			var existingCategory = await _repo.GetByIdAsync(categoryId, ct);
			if (existingCategory == null)
			{
				throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
			}

			// Map changes into tracked entity
			_mapper.Map(categoryDto, existingCategory);

			await _unitOfWork.SaveChangesAsync(ct);
		}

		public async Task DeleteAsync(Guid id, CancellationToken ct = default)
		{
			await _repo.DeleteAsync(new Category { Id = id }, ct);
			await _unitOfWork.SaveChangesAsync(ct);
		}

		public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken ct = default)
		{
			var categories = await _repo.GetAllAsync(ct);

			if (!categories.Any())
			{
				throw new KeyNotFoundException("No categories found.");
			}

			var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
			return new ApiResponse<IEnumerable<CategoryDto>>
			{
				IsError = false,
				Data = categoryDtos,
				Message = "Categories retrieved successfully"
			};
		}

		public async Task<ApiResponse<CategoryDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			var category = await _repo.GetByIdAsync(id, ct);
			if (category == null)
			{
				throw new KeyNotFoundException($"Category with ID {id} not found.");
			}

			var categoryDto = _mapper.Map<CategoryDto>(category);
			return new ApiResponse<CategoryDto?>
			{
				IsError = false,
				Data = categoryDto,
				Message = "Category retrieved successfully"
			};
		}

		public async Task<ApiResponse<IEnumerable<CategoryDto>>> SearchByNameAsync(string name, CancellationToken ct = default)
		{
			var categories = await _repo.SearchByNameAsync(name, ct);

			if (!categories.Any())
			{
				throw new KeyNotFoundException($"No categories found matching the name '{name}'.");
			}

			var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
			return new ApiResponse<IEnumerable<CategoryDto>>
			{
				IsError = false,
				Data = categoryDtos,
				Message = "Categories retrieved successfully"
			};
		}

		public async Task<ApiResponse<CategoryWithToolsDto?>> GetWithToolsAsync(Guid id, CancellationToken ct = default)
		{
			var category = await _repo.GetWithToolsAsync(id, ct);
			if (category == null)
			{
				throw new KeyNotFoundException($"Category with ID {id} not found.");
			}

			var dto = _mapper.Map<CategoryWithToolsDto>(category);
			return new ApiResponse<CategoryWithToolsDto?>
			{
				IsError = false,
				Data = dto,
				Message = "Category with tools retrieved successfully"
			};
		}
	}
}
