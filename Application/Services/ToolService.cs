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
		private readonly IUnitOfWork _unitOfWork;
		public ToolService(IToolRepository repo, IMapper mapper, IUnitOfWork unitOfWork)
		{
			_repo = repo;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public Task AddAsync(ToolDto toolDto, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public async Task UpdateAsync(UpdateToolDto toolDto, Guid toolId, CancellationToken ct = default)
		{
			var existingTool = await _repo.GetByIdAsync(toolId, ct);
			if (existingTool == null)
			{
				throw new KeyNotFoundException($"Tool with ID {toolId} not found.");
			}
			var categoryExists = await _unitOfWork.CategoryExistsAsync(toolDto.CategoryId, ct);

			if (!categoryExists)
			{
				throw new KeyNotFoundException($"Category with ID {toolDto.CategoryId} does not exist.");
			}


			// Map updates from DTO to the tracked entity
			_mapper.Map(toolDto, existingTool);

			// Persist changes via UnitOfWork
			await _unitOfWork.SaveChangesAsync(ct);
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

		public async Task<ApiResponse<IEnumerable<ToolDto>>> GetAvailableAsync(CancellationToken ct = default)
		{
			var avalilableTools = await _repo.GetAvailableAsync(ct);
			if (avalilableTools.Count() == 0)
			{
				throw new KeyNotFoundException("No available tools found.");
			}
			var toolDtos = _mapper.Map<IEnumerable<ToolDto>>(avalilableTools);
			return new ApiResponse<IEnumerable<ToolDto>>
			{
				IsError = false,
				Data = toolDtos,
				Message = "Available tools retrieved successfully"
			};

		}

		public Task<ApiResponse<IEnumerable<ToolDto>>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

		public async Task<ApiResponse<ToolDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			var tool =  await _repo.GetByIdAsync(id, ct);
			if (tool == null)
			{
				throw new KeyNotFoundException($"Tool with ID {id} not found.");
			}
			else
			{
				var toolDto = _mapper.Map<ToolDto>(tool);
				return new ApiResponse<ToolDto?>
				{
					IsError = false,
					Data = toolDto,
					Message = "Tool retrieved successfully"
				};
			}
		}

		public Task<ApiResponse<IEnumerable<ToolDto>>> SearchByNameAsync(string name, CancellationToken ct = default)
		{
			throw new NotImplementedException();
		}

	}
}
