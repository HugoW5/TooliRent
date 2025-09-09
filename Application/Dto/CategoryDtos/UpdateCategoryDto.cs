using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto.CategoryDtos
{
	public class UpdateCategoryDto
	{
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
	}
}
