namespace AWM.Service.Application.Features.Thesis.Attachments.Services;

/// <summary>
/// Application-layer abstraction for physical file storage operations.
/// Implementations may target S3, local disk, Azure Blob, etc.
/// </summary>
public interface IAttachmentService
{
    /// <summary>
    /// Saves a file stream to the file storage backend and returns the stored path.
    /// </summary>
    /// <param name="fileName">Original file name (used to derive extension and display name).</param>
    /// <param name="fileStream">The content stream to upload.</param>
    /// <param name="contentType">MIME content type of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The stored path / key that can later be used to retrieve the file.</returns>
    Task<string> SaveAsync(
        string fileName,
        Stream fileStream,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a previously stored file by its storage path / key.
    /// </summary>
    /// <param name="fileStoragePath">The path / key returned by <see cref="SaveAsync"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(string fileStoragePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Opens a read stream for the given stored file.
    /// </summary>
    /// <param name="fileStoragePath">The path / key returned by <see cref="SaveAsync"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Stream> GetAsync(string fileStoragePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Computes a SHA-256 hex hash of the given stream (does not rewind the stream beforehand).
    /// </summary>
    Task<string> ComputeHashAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
