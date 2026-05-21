namespace AWM.Service.Application.Features.Thesis.Attachments.Commands.UploadAttachment;

using FluentValidation;
using AWM.Service.Domain.Common;

public sealed class UploadAttachmentCommandValidator : AbstractValidator<UploadAttachmentCommand>
{
    private static readonly string[] AllowedExtensions =
    [
        ".pdf", ".doc", ".docx",
        ".ppt", ".pptx",
        ".zip", ".rar", ".7z",
        ".png", ".jpg", ".jpeg"
    ];

    public UploadAttachmentCommandValidator(Microsoft.Extensions.Options.IOptions<StorageSettings> storageOptions)
    {
        var settings = storageOptions.Value;
        var maxFileSizeBytes = settings.MaxAttachmentSizeMb * 1024L * 1024L;

        RuleFor(x => x.WorkId)
            .GreaterThan(0).WithMessage("Work ID must be greater than 0.");

        RuleFor(x => x.AttachmentTypeId)
            .InclusiveBetween(1, 6).WithMessage("Invalid attachment type (1 = Draft, 2 = Final, 3 = Presentation, 4 = Software, 5 = Demo, 6 = Handout).");

        RuleFor(x => x.File)
            .NotNull().WithMessage("A file must be provided.");

        When(x => x.File is not null, () =>
        {
            RuleFor(x => x.File.Length)
                .GreaterThan(0).WithMessage("The uploaded file is empty.")
                .LessThanOrEqualTo(maxFileSizeBytes)
                .WithMessage($"File size must not exceed {settings.MaxAttachmentSizeMb} MB.");

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
