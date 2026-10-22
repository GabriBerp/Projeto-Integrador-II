using System.Globalization;

public static class NumberFormatter
{
    private static readonly string[] Suffixes =
        { "", "K", "M", "B", "T", "Qa", "Qi" };

    public static string Format(double value)
    {
        if (value < 1000)
            return value.ToString("0.##", CultureInfo.InvariantCulture);

        int index = 0;
        while (value >= 1000 && index < Suffixes.Length - 1)
        {
            value /= 1000;
            index++;
        }

        return value.ToString("0.##", CultureInfo.InvariantCulture) + Suffixes[index];
    }
}