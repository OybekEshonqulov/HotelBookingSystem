using FluentValidation;
using HotelBookingSystem.Application.DTOsNew.UserManagementNew;

namespace HotelBookingSystem.Application.ValidatorsNew.UserNew;

public class CreateUserRequestDtoValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserRequestDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("FirstName majburiy.")
            .MaximumLength(100).WithMessage("FirstName 100 belgidan oshmasligi kerak.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("LastName majburiy.")
            .MaximumLength(100).WithMessage("LastName 100 belgidan oshmasligi kerak.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email majburiy.")
            .EmailAddress().WithMessage("Email formati noto‘g‘ri.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password majburiy.")
            .MinimumLength(6).WithMessage("Password kamida 6 ta belgidan iborat bo‘lishi kerak.");
    }
}