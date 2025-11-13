using FluentValidation;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Validation
{
    public class LoginUserValidator : AbstractValidator<LoginUserRequest>
    {
        public LoginUserValidator()
        {
            RuleFor(d => d.Username)
                .MinimumLength(1)
                .MaximumLength(100)
                .NotEmpty();

            RuleFor(d => d.Password)
                .MinimumLength(1)
                .MaximumLength(100)
                .NotEmpty();
        }
    }
}
