using Application.Dto.ToolDtos;
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
	public class ToolService : IToolService
	{
		private readonly IToolRepository _repo;
		private readonly IMapper _mapper;
		public ToolService(IToolRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public Task AddAsync(ToolDto toolDto, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(ToolDto toolDto, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}
		public async Task DeleteAsync(Guid id, CancellationToken ct = default)
		{
			await _repo.DeleteAsync(new Tool { Id = id }, ct);
		}

		public async Task<ApiResponse<IEnumerable<ToolDto>>> GetAllAsync(CancellationToken ct = default)
		{
			var tools = await _repo.GetAllAsync(ct);

			if (tools.Count() == 0)
			{
				return new ApiResponse<IEnumerable<ToolDto>>
				{
					IsError = true,
					Data = null!,
					Message = "No tools found"
				};
			}
			var toolDtos = _mapper.Map<IEnumerable<ToolDto>>(tools);
			return new ApiResponse<IEnumerable<ToolDto>>
			{
				IsError = false,
				Data = toolDtos,
				Message = "Tools retrieved successfully"
			};
		}

		public Task<ApiResponse<IEnumerable<ToolDto>>> GetAvailableAsync(CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<IEnumerable<ToolDto>>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<ToolDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse<IEnumerable<ToolDto>>> SearchByNameAsync(string name, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

	}
}
