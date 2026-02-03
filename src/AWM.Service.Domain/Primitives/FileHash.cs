namespace AWM.Service.Domain.Primitives;

using AWM.Service.Domain.Common;
using System.Text.RegularExpressions;

/// <summary>
/// Value object representing a SHA-256 file hash for integrity verification.
/// </summary>
public sealed partial class FileHash : ValueObject
{
    private const int Sha256Length = 64;
    
    public string Value { get; }

    private FileHash(string value)
    {
        Value = value.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a new file hash from a SHA-256 hex string.
    /// </summary>
    public static FileHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Hash cannot be empty.", nameof(hash));

        hash = hash.Trim();
        
        if (hash.Length != Sha256Length)
            throw new ArgumentException($"Hash must be {Sha256Length} characters (SHA-256).", nameof(hash));

        if (!HexPattern().IsMatch(hash))
            throw new ArgumentException("Hash must contain only hexadecimal characters.", nameof(hash));

        return new FileHash(hash);
    }

    /// <summary>
    /// Attempts to create a file hash, returning null if validation fails.
    /// </summary>
    public static FileHash? TryCreate(string? hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return null;

        hash = hash.Trim();
        
        if (hash.Length != Sha256Length || !HexPattern().IsMatch(hash))
            return null;

        return new FileHash(hash);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[A-Fa-f0-9]+$")]
    private static partial Regex HexPattern();
}
