using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EsoWatch.Web.Dialogs;

public partial class AddTimerDialog
{
    public class Model
    {
        private const string _timeLeftPattern = @"^\s*((?<days>\d+)\s*[dD])?\s*((?<hours>\d+)\s*[hH])?\s*((?<minutes>\d+)\s*[mM]\s*)?$";

        [Required(ErrorMessage = "Duration is required")]
        [RegularExpression(_timeLeftPattern)]
        public string TimeLeft { get; set; } = "";

        [Required]
        [MinLength(1)]
        public string Name { get; set; } = "";

        public TimeSpan? GetTimeLeft()
        {
            Match ma = Regex.Match(TimeLeft, _timeLeftPattern);
            if (!ma.Success)
            {
                return null;
            }

            TimeSpan timeLeft = TimeSpan.Zero;
            if (ma.Groups["days"].Success)
            {
                timeLeft += TimeSpan.FromDays(int.Parse(ma.Groups["days"].Value));
            }
            if (ma.Groups["hours"].Success)
            {
                timeLeft += TimeSpan.FromHours(int.Parse(ma.Groups["hours"].Value));
            }
            if (ma.Groups["minutes"].Success)
            {
                timeLeft += TimeSpan.FromMinutes(int.Parse(ma.Groups["minutes"].Value));
            }
            return timeLeft;
        }
    }
}