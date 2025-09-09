using Application.Dto.CategoryDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces.Repositories;
using Domain.Models;
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
	}
}
