namespace AWM.Service.Domain.Common;

/// <summary>
/// Base domain exception for business rule violations.
/// Mapped to HTTP 422 Unprocessable Entity in the WebAPI layer.
/// </summary>
public class DomainException : Exception
{
    public string ErrorCode { get; }

    public DomainException(string errorCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string errorCode, string message, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
