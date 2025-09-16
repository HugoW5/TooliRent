using Application.Dto.BookingDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators
{
	public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
	{
		public UpdateBookingDtoValidator()
		{

			RuleFor(x => x.Status)
				.IsInEnum().WithMessage("Invalid status value.");

			RuleFor(x => x.EndAt)
				.GreaterThan(x => x.StartAt).WithMessage("End date must be after start date.");
		}
	}
}
