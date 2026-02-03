namespace AWM.Service.Domain.Primitives;

using AWM.Service.Domain.Common;

/// <summary>
/// Value object representing a date range with start and end dates.
/// Used for academic periods, workflow stages, etc.
/// </summary>
public sealed class DateRange : ValueObject
{
    public DateTime Start { get; }
    public DateTime End { get; }

    private DateRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Creates a new date range with validation.
    /// </summary>
    public static DateRange Create(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be after start date.", nameof(end));

        return new DateRange(start, end);
    }

    /// <summary>
    /// Checks if a given date falls within this range (inclusive).
    /// </summary>
    public bool Contains(DateTime date)
    {
        return date >= Start && date <= End;
    }

    /// <summary>
    /// Checks if the current date is within this range.
    /// </summary>
    public bool IsCurrentlyActive()
    {
        return Contains(DateTime.UtcNow);
    }

    /// <summary>
    /// Checks if the range has ended.
    /// </summary>
    public bool HasEnded()
    {
        return DateTime.UtcNow > End;
    }

    /// <summary>
    /// Gets the total duration of the range.
    /// </summary>
    public TimeSpan Duration => End - Start;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    public override string ToString() => $"{Start:yyyy-MM-dd} - {End:yyyy-MM-dd}";
}
