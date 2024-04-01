using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
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
        // TODO придумать оберкту для измерения времени
        var operationTimer = new Stopwatch();
        operationTimer.Start();
        
        this.Log().Info($"Start processing {nameof(AppSettingsStoreFileService)}:{nameof(GetAsync)}");

        var result = base.GetAsync();
       
        operationTimer.Stop();
        
        this.Log().Info($"Stop processing {nameof(AppSettingsStoreFileService)}:{nameof(GetAsync)}. Elapsed Time {operationTimer.ElapsedMilliseconds} ms");
        
        return result;
    }

    public override Task<bool> SetAsync()
    {
        var operationTimer = new Stopwatch();
        operationTimer.Start();
        
        this.Log().Info($"Start processing {nameof(AppSettingsStoreFileService)}:{nameof(SetAsync)}");

        var result = base.GetAsync();
       
        operationTimer.Stop();
        
        this.Log().Info($"Stop processing {nameof(AppSettingsStoreFileService)}:{nameof(SetAsync)}. Elapsed Time {operationTimer.ElapsedMilliseconds} ms");
        
        return result;
    }
}