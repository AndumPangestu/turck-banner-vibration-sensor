//using FluentValidation;
//using ReminderManager.Domain.DTO;

//namespace ReminderManager.Application.Validation
//{
//    public class RackValidator : AbstractValidator<RackRequest>
//    {
//        public RackValidator()
//        {
//            RuleFor(d => d.Code)
//                .MinimumLength(1)
//                .MaximumLength(100)
//                .NotEmpty();
//        }
//    }


//    public class RackFilterValidator : AbstractValidator<RackFilterRequest>
//    {
//        public RackFilterValidator()
//        {
//            RuleFor(d => d.Keyword)
//                .MaximumLength(100)
//                .When(d => !string.IsNullOrWhiteSpace(d.Keyword));

//            When(d => d.Paginate, () =>
//            {
//                RuleFor(d => d.Page)
//                    .GreaterThanOrEqualTo(1)
//                    .WithMessage("Page must be at least 1");

//                RuleFor(d => d.Limit)
//                    .GreaterThan(0)
//                    .LessThanOrEqualTo(100)
//                    .WithMessage("Limit must be between 1 and 100");
//            });
//        }
//    }

//}
