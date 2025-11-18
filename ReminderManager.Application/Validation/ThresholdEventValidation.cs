using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Validation
{
    public class ThresholdEventFilterValidator : AbstractValidator<ThresholdEventFilterRequest>
    {
        public ThresholdEventFilterValidator()
        {
            RuleFor(d => d.DeviceId)
                .Must(x => x > 0)
                .When(d => d.DeviceId != null)
                .WithMessage("DeviceId must be greater than 0");
            RuleFor(d => d.StartDate)
                .LessThan(d => d.EndDate)
                .When(d => d.StartDate != null && d.EndDate != null)
                .WithMessage("Start date must be less than end date");

            RuleFor(d => d.EndDate)
                .GreaterThan(d => d.StartDate)
                .When(d => d.StartDate != null && d.EndDate != null)
                .WithMessage("End date must be greater than start date");

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
