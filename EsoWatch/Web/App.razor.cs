using Microsoft.AspNetCore.Components;

using Radzen;

namespace EsoWatch.Web;

public partial class App
{
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    [Inject]
    private ThemeService? ThemeService { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (HttpContext == null || ThemeService == null)
        {
            return;
        }

        string? theme = HttpContext.Request.Cookies["EsoWatchTheme"];

        if (!string.IsNullOrEmpty(theme))
        {
            ThemeService.SetTheme(theme, false);
        }
    }
}