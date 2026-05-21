namespace AWM.Service.Domain.Thesis.Constants;

/// <summary>
/// System codes for check types that have specific logic associated with them.
/// Use these instead of hardcoded IDs.
/// Dynamic department checks (like Software Check) will have null codes or their own codes configured by the department.
/// </summary>
public static class CheckTypeCodes
{
    public const string NormControl = "NORMCONTROL";
    public const string AntiPlagiarism = "ANTIPLAGIARISM";
}
