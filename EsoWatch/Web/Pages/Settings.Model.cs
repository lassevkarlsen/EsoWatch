using EsoWatch.Data.Entities;
using EsoWatch.Web.Dialogs;

namespace EsoWatch.Web.Pages;

public partial class Settings
{
    private class Model
    {
        public required UserSettings Settings { get; set; }
        public required bool IsNew { get; set; }

        public bool IsPushoverkeyValid()
        {
            if (!Settings.UsePushover)
            {
                return false;
            }

            return Settings.PushoverUserKey?.Length != 32;
        }
    }
}