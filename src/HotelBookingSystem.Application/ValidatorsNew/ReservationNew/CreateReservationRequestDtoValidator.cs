using FluentValidation;
using HotelBookingSystem.Application.DTOsNew.ReservationNew;

namespace HotelBookingSystem.Application.ValidatorsNew.ReservationNew;

public class CreateReservationRequestDtoValidator : AbstractValidator<CreateReservationRequestDto>
{
    public CreateReservationRequestDtoValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty().WithMessage("PropertyId majburiy.");

        RuleFor(x => x.GuestId)
            .NotEmpty().WithMessage("GuestId majburiy.");

        RuleFor(x => x.CheckInDate)
            .NotEmpty().WithMessage("CheckInDate majburiy.");

        RuleFor(x => x.CheckOutDate)
            .NotEmpty().WithMessage("CheckOutDate majburiy.")
            .GreaterThan(x => x.CheckInDate)
            .WithMessage("CheckOutDate CheckInDate dan katta bo‘lishi kerak.");

        RuleFor(x => x.AdultsCount)
            .GreaterThanOrEqualTo(1)
            .WithMessage("AdultsCount kamida 1 bo‘lishi kerak.");

        RuleFor(x => x.ChildrenCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ChildrenCount 0 dan kichik bo‘lmasligi kerak.");

        RuleFor(x => x.TotalAmount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalAmount manfiy bo‘lmasligi kerak.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("CurrencyCode majburiy.")
            .MaximumLength(10).WithMessage("CurrencyCode 10 belgidan oshmasligi kerak.");
    }
}