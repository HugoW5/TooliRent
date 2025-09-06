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
		}
	}
}
