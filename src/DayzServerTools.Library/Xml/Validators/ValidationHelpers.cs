namespace DayzServerTools.Library.Xml.Validators;

internal static class ValidationHelpers
{
    public static bool BeCorrectProbabilityValue(double value)
    {
        return value >= 0 && value <= 1;
    }
}