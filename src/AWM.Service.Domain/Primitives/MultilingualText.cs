namespace AWM.Service.Domain.Primitives;

using AWM.Service.Domain.Common;

/// <summary>
/// Value object representing text in multiple languages (Russian, Kazakh, English).
/// Used for titles and descriptions that need to support three languages.
/// </summary>
public sealed class MultilingualText : ValueObject
{
    public string Ru { get; }
    public string? Kz { get; }
    public string? En { get; }

    private MultilingualText(string ru, string? kz = null, string? en = null)
    {
        if (string.IsNullOrWhiteSpace(ru))
            throw new ArgumentException("Russian text is required.", nameof(ru));

        Ru = ru;
        Kz = kz;
        En = en;
    }

    /// <summary>
    /// Creates a new multilingual text with Russian as required language.
    /// </summary>
    public static MultilingualText Create(string ru, string? kz = null, string? en = null)
    {
        return new MultilingualText(ru, kz, en);
    }

    /// <summary>
    /// Gets the text in the specified language, falling back to Russian if not available.
    /// </summary>
    public string GetText(string languageCode)
    {
        return languageCode.ToLowerInvariant() switch
        {
            "kz" or "kk" => Kz ?? Ru,
            "en" => En ?? Ru,
            _ => Ru
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Ru;
        yield return Kz;
        yield return En;
    }

    public override string ToString() => Ru;
}
