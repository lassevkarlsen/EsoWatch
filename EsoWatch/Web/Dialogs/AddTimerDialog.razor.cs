using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using Radzen;

namespace EsoWatch.Web.Dialogs;

public partial class AddTimerDialog : ComponentBase
{
    private readonly DialogService _dialogService;

    private readonly Model _model = new();
    private EditContext? _editContext;

    public AddTimerDialog(DialogService dialogService)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
    }

    protected override void OnInitialized()
    {
        _editContext = new EditContext(_model);
    }

    private void Add()
    {
        _dialogService.Close(_model);
    }

    private void Cancel()
    {
        _dialogService.Close();
    }
}