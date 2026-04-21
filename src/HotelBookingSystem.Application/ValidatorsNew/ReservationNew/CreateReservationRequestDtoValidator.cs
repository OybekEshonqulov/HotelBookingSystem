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

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Items majburiy.")
            .Must(items => items != null && items.Count > 0)
            .WithMessage("Kamida bitta reservation item bo‘lishi kerak.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateReservationItemRequestDtoValidator());
    }
}

public class CreateReservationItemRequestDtoValidator : AbstractValidator<CreateReservationItemRequestDto>
{
    public CreateReservationItemRequestDtoValidator()
    {
        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("UnitPrice manfiy bo‘lmasligi kerak.");

        RuleFor(x => x)
            .Must(x => (x.RoomId.HasValue && !x.BedId.HasValue) || (!x.RoomId.HasValue && x.BedId.HasValue))
            .WithMessage("Har bir itemda faqat RoomId yoki faqat BedId bo‘lishi kerak.");
    }
}