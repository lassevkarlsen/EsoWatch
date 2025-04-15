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

        if (timeLeft.Hours == 0 && timeLeft.Minutes == 0 && timeLeft.Seconds == 0)
        {
            return sb.ToString();
        }

        if (sb.Length > 0)
        {
            sb.Append(' ');
        }

        if (timeLeft.Hours > 0)
        {
            sb.Append($"{timeLeft.Hours:00}:");
        }

        sb.Append($"{timeLeft.Minutes:00}:{timeLeft.Seconds:00}");

        return sb.ToString();
    }
}