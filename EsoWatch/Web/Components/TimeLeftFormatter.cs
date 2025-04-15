using System.Text;

namespace EsoWatch.Web.Components;

public static class TimeLeftFormatter
{
    public static string Format(TimeSpan timeLeft)
    {
        if (timeLeft <= TimeSpan.Zero)
        {
            return "now";
        }

        var sb = new StringBuilder();

        if (timeLeft.Days > 0)
        {
            sb.Append($"{timeLeft.Days}D");
        }

        if (timeLeft.Hours > 0)
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            if (timeLeft.Hours > 1 && timeLeft.Minutes >= 15)
            {
                sb.Append($"{timeLeft.Hours + 1}H");
            }
            else
            {
                sb.Append($"{timeLeft.Hours}H");
            }
        }

        if (timeLeft is { Days: 0, Hours: < 2 })
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }
            sb.Append($"{timeLeft.Minutes}M");
        }

        if (timeLeft is { Days: 0, Hours: 0, Minutes: < 10 })
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }
            sb.Append($"{timeLeft.Seconds}S");
        }

        return sb.ToString();
    }
}