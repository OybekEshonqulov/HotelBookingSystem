using FluentValidation;
using HotelBookingSystem.Application.DTOsNew.ReservationNew;

namespace HotelBookingSystem.Application.ValidatorsNew.ReservationNew;

public class CreateReservationRequestDtoValidator : AbstractValidator<CreateReservationRequestDto>
{
    public CreateReservationRequestDtoValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("PropertyId majburiy.");

        RuleFor(x => x.GuestFirstName)
            .NotEmpty().WithMessage("GuestFirstName majburiy.")
            .MaximumLength(100).WithMessage("GuestFirstName 100 belgidan oshmasligi kerak.");

        RuleFor(x => x.GuestLastName)
            .NotEmpty().WithMessage("GuestLastName majburiy.")
            .MaximumLength(100).WithMessage("GuestLastName 100 belgidan oshmasligi kerak.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("CheckInDate majburiy.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("CheckOutDate majburiy.")
            .GreaterThan(x => x.CheckInDate).WithMessage("CheckOutDate CheckInDate dan katta bo‘lishi kerak.");

        RuleFor(x => x.AdultsCount)
            .GreaterThanOrEqualTo(1).WithMessage("AdultsCount kamida 1 bo‘lishi kerak.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Kamida bitta item bo‘lishi kerak.");
    }
}