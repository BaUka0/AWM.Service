namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.CreateSupervisorReview;

using FluentValidation;
using AWM.Service.Domain.Common;

public sealed class CreateSupervisorReviewCommandValidator : AbstractValidator<CreateSupervisorReviewCommand>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"];
    public CreateSupervisorReviewCommandValidator(Microsoft.Extensions.Options.IOptions<StorageSettings> storageOptions)
    {
        var settings = storageOptions.Value;
        var maxFileSizeBytes = settings.MaxReviewSizeMb * 1024L * 1024L;

        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.ReviewText)
            .NotEmpty().WithMessage("Review text is required.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File!.Length)
                .GreaterThan(0).WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxReviewSizeMb} MB.");

            RuleFor(x => x.File!.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .Must(HasAllowedExtension)
                .WithMessage($"Allowed file extensions: {string.Join(", ", AllowedExtensions)}.");
        });
    }

    private static bool HasAllowedExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
