using System.Reactive;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using ReactiveUI;

namespace Client.ViewModels;

internal sealed class AgreementViewModel : ViewModelBase
{

    #region Constructors

    public AgreementViewModel(IStore<AppSettings> appSettingsStore)
    {
        AgreementAcceptance = ReactiveCommand.Create(() =>
        {
            appSettingsStore.CurrentValue.Agreement = true;
        });
    }

    #endregion
    #region Commands

    public ReactiveCommand<Unit, Unit> AgreementAcceptance { get; set; }

    #endregion
}


