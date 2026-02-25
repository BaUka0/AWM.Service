namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.UploadAttachment;

using FluentValidation;

public sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    private static readonly string[] AllowedExtensions =
    [
        ".pdf", ".doc", ".docx",
        ".ppt", ".pptx",
        ".zip", ".rar", ".7z",
        ".png", ".jpg", ".jpeg"
    ];

    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

    public UploadAttachmentCommandValidator()
    {
        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.AttachmentType)
            .IsInEnum().WithMessage("Invalid attachment type.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("A file must be provided.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File.Length)
                .GreaterThan(0).WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage($"File size must not exceed {MaxFileSizeBytes / 1024 / 1024} MB.");

            RuleFor(x => x.File.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .Must(HasAllowedExtension)
                .WithMessage($"Allowed file extensions: {string.Join(", ", AllowedExtensions)}.");
        });
    }

    private static bool HasAllowedExtension(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }
}
