﻿using System.IO;
using AppInfrastructure.Services.FileService;
using AppInfrastructure.Services.ParserService;
using AppInfrastructure.Stores.DefaultStore;
using Client.Models;
using Splat;

namespace Client.Services.FileServices;

public class AppSettingsStoreFileService : BaseStoreFileService<IStore<AppSettings>, AppSettings>, IEnableLogger
{
    public AppSettingsStoreFileService(IStore<AppSettings> store, IParserService parserService) : base(store, parserService, $"{nameof(AppSettings)}.js", Path.Combine(Directory.GetCurrentDirectory(), "Data"))
    {
    }
}