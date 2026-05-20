namespace AWM.Service.WebAPI.Common.Contracts.Responses;

/// <summary>
/// Standard paginated response wrapper.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
public sealed class PagedResponse<T>
{
    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Items per page.
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;

    /// <summary>
    /// Items for the current page.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}
