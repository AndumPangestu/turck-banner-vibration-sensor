using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using ReminderManager.Domain.DTO;

namespace ReminderManager.Application.Validation
{
    public class ThresholdValidator : AbstractValidator<ThresholdRequest>
    {
        public ThresholdValidator()
        {
            // DeviceId wajib diisi dan harus > 0
            RuleFor(x => x.DeviceId)
                .GreaterThan(0)
                .WithMessage("DeviceId harus lebih dari 0.");

            // ThresholdVelocityX wajib diisi dan bernilai positif
            RuleFor(x => x.ThresholdVelocityX)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdVelocityX tidak boleh bernilai negatif.");

            // Pesan VelocityX wajib diisi jika ThresholdVelocityX > 0
            When(x => x.ThresholdVelocityX > 0, () =>
            {
                RuleFor(x => x.MessageThresholdVelocityX)
                    .NotEmpty()
                    .WithMessage("MessageThresholdVelocityX wajib diisi jika ThresholdVelocityX > 0.");
            });

            // ThresholdVelocityY wajib diisi dan bernilai positif
            RuleFor(x => x.ThresholdVelocityY)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdVelocityY tidak boleh bernilai negatif.");

            // Pesan VelocityY wajib diisi jika ThresholdVelocityY > 0
            When(x => x.ThresholdVelocityY > 0, () =>
            {
                RuleFor(x => x.MessageThresholdVelocityY)
                    .NotEmpty()
                    .WithMessage("MessageThresholdVelocityY wajib diisi jika ThresholdVelocityY > 0.");
            });

            // ThresholdVelocityZ
            RuleFor(x => x.ThresholdVelocityZ)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdVelocityZ tidak boleh bernilai negatif.");

            When(x => x.ThresholdVelocityZ > 0, () =>
            {
                RuleFor(x => x.MessageThresholdVelocityZ)
                    .NotEmpty()
                    .WithMessage("MessageThresholdVelocityZ wajib diisi jika ThresholdVelocityZ > 0.");
            });

            // ThresholdAccelerationX
            RuleFor(x => x.ThresholdAccelerationX)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdAccelerationX tidak boleh bernilai negatif.");

            When(x => x.ThresholdAccelerationX > 0, () =>
            {
                RuleFor(x => x.MessageThresholdAccelerationX)
                    .NotEmpty()
                    .WithMessage("MessageThresholdAccelerationX wajib diisi jika ThresholdAccelerationX > 0.");
            });

            // ThresholdAccelerationY
            RuleFor(x => x.ThresholdAccelerationY)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdAccelerationY tidak boleh bernilai negatif.");

            When(x => x.ThresholdAccelerationY > 0, () =>
            {
                RuleFor(x => x.MessageThresholdAccelerationY)
                    .NotEmpty()
                    .WithMessage("MessageThresholdAccelerationY wajib diisi jika ThresholdAccelerationY > 0.");
            });

            // ThresholdAccelerationZ
            RuleFor(x => x.ThresholdAccelerationZ)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ThresholdAccelerationZ tidak boleh bernilai negatif.");

            When(x => x.ThresholdAccelerationZ > 0, () =>
            {
                RuleFor(x => x.MessageThresholdAccelerationZ)
                    .NotEmpty()
                    .WithMessage("MessageThresholdAccelerationZ wajib diisi jika ThresholdAccelerationZ > 0.");
            });

            // ThresholdTemperature
            RuleFor(x => x.ThresholdTemperature)
                .InclusiveBetween(-50, 200) // contoh batas suhu
                .WithMessage("ThresholdTemperature harus berada antara -50°C dan 200°C.");

            When(x => x.ThresholdTemperature > 0, () =>
            {
                RuleFor(x => x.MessageThresholdTemperature)
                    .NotEmpty()
                    .WithMessage("MessageThresholdTemperature wajib diisi jika ThresholdTemperature > 0.");
            });
        }
    }


    public class ThresholdFilterValidator : AbstractValidator<ThresholdFilterRequest>
    {
        public ThresholdFilterValidator()
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
