using System.Drawing;

namespace DayzServerTools.Application.Services;

public static class RandomColorPicker
{
    public static Color PickColor(string seed)
    {
        var rngSeed = seed.GetHashCode();
        var rng = new Random(rngSeed);

        var minValue = 128;
        var maxValue = 256;

        var red = rng.Next(minValue, maxValue);
        var green = rng.Next(minValue, maxValue);
        var blue = rng.Next(minValue, maxValue);

        return Color.FromArgb(red, green, blue);
    }
}
