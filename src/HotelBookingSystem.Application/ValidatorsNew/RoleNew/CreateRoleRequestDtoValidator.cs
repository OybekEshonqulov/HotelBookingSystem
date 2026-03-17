using FluentValidation;
using HotelBookingSystem.Application.DTOsNew.RoleManagementNew;

namespace HotelBookingSystem.Application.ValidatorsNew.RoleNew;

public class CreateRoleRequestDtoValidator : AbstractValidator<CreateRoleRequestDto>
{
    public CreateRoleRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role nomi majburiy.")
            .MaximumLength(100).WithMessage("Role nomi 100 belgidan oshmasligi kerak.");
    }
}