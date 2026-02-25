namespace AWM.Service.Application.Features.Thesis.Reviews.Commands.UploadReview;

using FluentValidation;

public sealed class UploadReviewCommandValidator : AbstractValidator<UploadReviewCommand>
{
    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png"];
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public UploadReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0).WithMessage("Review ID must be greater than 0.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.ReviewText) || x.File is not null)
            .WithMessage("Either ReviewText or File must be provided.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File!.Length)
                .GreaterThan(0).WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"File size must not exceed {MaxFileSizeBytes / 1024 / 1024} MB.");

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
