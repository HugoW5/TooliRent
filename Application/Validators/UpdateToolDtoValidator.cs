using Application.Dto.ToolDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
	public class UpdateToolDtoValidator : AbstractValidator<UpdateToolDto>
	{
		public UpdateToolDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Name is required")
				.MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

			RuleFor(x => x.Description)
				.MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

			RuleFor(x => x.CategoryId)
				.NotEmpty().WithMessage("Category is required");

			RuleFor(x => x.Status)
				.NotNull().WithMessage("Availability status is required");
		}
	}
}
