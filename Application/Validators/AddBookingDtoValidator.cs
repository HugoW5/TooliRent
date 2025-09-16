using Application.Dto.BookingDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
	public class AddBookingDtoValidator : AbstractValidator<AddBookingDto>
	{
		public AddBookingDtoValidator()
		{
			RuleFor(x => x.ToolIds)
				.NotEmpty().WithMessage("At least one tool must be selected.");

			RuleFor(x => x.StartAt)
				.GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Start date must be in the future.");

			RuleFor(x => x.EndAt)
				.GreaterThan(x => x.StartAt).WithMessage("End date must be after start date.");
		}
	}
}
