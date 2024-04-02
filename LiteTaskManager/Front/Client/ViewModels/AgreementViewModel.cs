using System;
using System.Reactive;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using ReactiveUI;
using Splat;

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
        
        #region Command logging

        AgreementAcceptance.ThrownExceptions.Subscribe(e =>
            this.Log().StructLogError($"Processing", e.Message, nameof(AgreementAcceptance)));
        
        AgreementAcceptance.Subscribe(_ => this.Log().StructLogInfo($"Processing", nameof(AgreementAcceptance)));

        #endregion
    }

    #endregion
    
    #region Commands

    public ReactiveCommand<Unit, Unit> AgreementAcceptance { get; set; }

    #endregion
}


