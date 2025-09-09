using Application.Dto.CategoryDtos;
using Application.Dto.ToolDtos;
using AutoMapper;
using Domain.Enums;
using Domain.Models;


namespace Application.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Tool, ToolDto>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
				.ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.Status == ToolStatus.Available));

			CreateMap<UpdateToolDto, Tool>()
				.ForMember(dest => dest.Category, opt => opt.Ignore());
			CreateMap<AddToolDto, Tool>()
				.ForMember(dest => dest.Category, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => ToolStatus.Available));

			CreateMap<AddCategoryDto, Category>();
			CreateMap<UpdateCategoryDto, Category>();

			CreateMap<Category, CategoryDto>();
			CreateMap<Category , CategoryWithToolsDto>();
		}
	}
}
