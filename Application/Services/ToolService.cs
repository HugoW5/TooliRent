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

		/// <summary>
		/// Adds a new tool to the repository after validating the category exists.
		/// </summary>
		/// <param name="addToolDto">The new tool</param>
		/// <param name="ct"></param>
		/// <returns>The id of the new tool</returns>
		/// <exception cref="KeyNotFoundException">The Category does not exist</exception>
		/// <exception cref="Exception">Internal server error</exception>
		public async Task<Guid?> AddAsync(AddToolDto addToolDto, CancellationToken ct = default)
		{
			var categoryExists = await _unitOfWork.CategoryExistsAsync(addToolDto.CategoryId, ct);
			if (!categoryExists)
			{
				throw new KeyNotFoundException($"Category with ID {addToolDto.CategoryId} does not exist.");
			}
			var tool = _mapper.Map<Tool>(addToolDto);
			var addedToolId = await _repo.AddAsync(tool, ct);
			if (addedToolId == null)
			{
				throw new Exception("Failed to add the tool.");
			}
			await _unitOfWork.SaveChangesAsync(ct);
			return addedToolId;
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
			await _unitOfWork.SaveChangesAsync(ct);
		}

		public async Task<ApiResponse<IEnumerable<ToolDto>>> GetAllAsync(CancellationToken ct = default)
		{
			var tools = await _repo.GetAllAsync(ct);

			if (tools.Count() == 0)
			{
				throw new KeyNotFoundException("No tools found.");

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

		public async Task<ApiResponse<IEnumerable<ToolDto>>> GetByCategoryAsync(Guid categoryId, CancellationToken ct = default)
		{
			var tools = await _repo.GetByCategoryAsync(categoryId, ct);
			if (tools.Count() == 0)
			{
				throw new KeyNotFoundException($"No tools found in category with ID {categoryId}.");
			}
			var toolDtos = _mapper.Map<IEnumerable<ToolDto>>(tools);
			return new ApiResponse<IEnumerable<ToolDto>>
			{
				IsError = false,
				Data = toolDtos,
				Message = "Tools retrieved successfully"
			};

		}

		public async Task<ApiResponse<ToolDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
		{
			var tool = await _repo.GetByIdAsync(id, ct);
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

		public async Task<ApiResponse<IEnumerable<ToolDto>>> SearchByNameAsync(string name, CancellationToken ct = default)
		{
			var tools = await _repo.SearchByNameAsync(name, ct);
			if (tools.Count() == 0)
			{
				throw new KeyNotFoundException($"No tools found matching the name '{name}'.");
			}
			var toolDtos = _mapper.Map<IEnumerable<ToolDto>>(tools);
			return new ApiResponse<IEnumerable<ToolDto>>
			{
				IsError = false,
				Data = toolDtos,
				Message = "Tools retrieved successfully"
			};

		}

	}
}
