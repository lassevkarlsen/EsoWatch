using EsoWatch.Data;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

using Radzen;

namespace EsoWatch.Web.Dialogs;

public partial class AddCharacterDialog : ComponentBase
{
    [Parameter]
    public Guid UserId { get; set; }

    private readonly DialogService _dialogService;
    private readonly IDbContextFactory<EsoDbContext> _dbContextFactory;

    private readonly Model _model = new();
    private EditContext? _editContext;

    public AddCharacterDialog(DialogService dialogService, IDbContextFactory<EsoDbContext> dbContextFactory)
    {
        _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
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

    private bool NoDuplicatesAllowed()
    {
        using EsoDbContext dbContext = _dbContextFactory.CreateDbContext();
        string name = _model.Name.Trim().ToLower();
        return !dbContext.Characters.Any(c => c.Name.Trim().ToLower() == name && c.UserId == UserId);
    }
}