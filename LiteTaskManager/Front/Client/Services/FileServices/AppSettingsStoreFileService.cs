using System;
using System.IO;
using System.Threading.Tasks;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Infrastructure.Logging;
using Client.Models;
using Splat;

namespace Client.Services.FileServices;

/// <summary>
///     Отвечает за ввод/вывод настроек приложения из файла конфигурции
/// </summary>
internal sealed class AppSettingsStoreFileService : BaseStoreFileService<IStore<AppSettings>, AppSettings>, IEnableLogger
{
    public AppSettingsStoreFileService(IStore<AppSettings> store, IParserService parserService) : base(store, parserService, $"{nameof(AppSettings)}.js", Path.Combine(Directory.GetCurrentDirectory(), "Data"))
    {
    }
    
    public override Task<bool> GetAsync()
    {
        Task<bool> result = null;
        
        new Action(() => {result = base.GetAsync(); }).TimeLog(this.Log());
        
        return result;
    }

    public override Task<bool> SetAsync()
    {
        Task<bool> result = null;
        
        new Action(() => {result = base.SetAsync(); }).TimeLog(this.Log());
        
        return result;
    }
}