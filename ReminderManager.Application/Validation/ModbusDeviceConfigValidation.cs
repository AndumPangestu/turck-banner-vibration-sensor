using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Validation
{
    public class ModbusDeviceConfigFilterValidator : AbstractValidator<ModbusDeviceConfigFilterRequest>
    {
        public ModbusDeviceConfigFilterValidator()
        {
            RuleFor(d => d.Keyword)
                .MinimumLength(1)
                .MaximumLength(100)
                .When(d => !string.IsNullOrWhiteSpace(d.Keyword))
                .WithMessage("Keyword must not be empty");

            When(d => d.Paginate, () =>
            {
                RuleFor(d => d.Page)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Page must be at least 1");

                RuleFor(d => d.Limit)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(100)
                    .WithMessage("Limit must be between 1 and 100");
            });
        }
    }
}
