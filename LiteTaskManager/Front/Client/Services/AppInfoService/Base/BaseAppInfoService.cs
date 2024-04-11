using System.Diagnostics;
using System.Reflection;
using Client.Infrastructure.Logging;
using Splat;

namespace Client.Services.AppInfoService.Base;

/// <summary>
///     <seealso cref="IAppInfoService"/>
/// </summary>
internal abstract class BaseAppInfoService : IAppInfoService, IEnableLogger
{
    #region Properties

    public bool IsAdminMode => IsAdminCheck();

    public string AppName { get; } = string.Empty;
        
    public string AppManufacturer { get; } = string.Empty;

    // TODO вынести в файл 
    public string AppGitHub => "https://github.com/Htomsik/LTaskManager";
        
    public string AppVersion { get;} = string.Empty;

    #endregion

    #region Constructors

    public BaseAppInfoService()
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly is null)
        {
            this.Log().StructLogWarn("Can't get assembly");
            return;
        }
        
        var fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        
        if (fileInfo.ProductName != null) 
            AppName = fileInfo.ProductName;
        
        if (fileInfo.CompanyName != null) 
            AppManufacturer = fileInfo.CompanyName;
        
        if (fileInfo.FileVersion != null) 
            AppVersion = fileInfo.FileVersion;
    }

    #endregion
    
    #region Methods

    protected abstract bool IsAdminCheck();
    
    #endregion
}