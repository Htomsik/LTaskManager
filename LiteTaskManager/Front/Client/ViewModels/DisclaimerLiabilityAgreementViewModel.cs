using System.Reactive;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Microsoft.CodeAnalysis;
using ReactiveUI;
using Splat;

namespace Client.ViewModels;


internal sealed class DisclaimerLiabilityAgreementViewModel : ViewModelBase
{

    #region Constructors
/*
    public DisclaimerLiabilityAgreementViewModel(
        IStoreFileService<IStore<AppSettings>, AppSettings> appSettingsFileService,
        DisclaimerLiabilityAgreementViewModel disclaimerLiabilityAgreementViewModel)
    {
        OpenDisclaimerLiabilityAgreement = ReactiveCommand.Create(() =>
            {
                
            });
    }*/

    #endregion
    #region Commands

    public ReactiveCommand<Unit, Unit> OpenDisclaimerLiabilityAgreement { get; set; }

    #endregion
}


