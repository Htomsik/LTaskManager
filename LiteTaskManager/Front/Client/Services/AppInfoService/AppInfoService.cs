using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security.Principal;
using Client.Infrastructure.Logging;
using Splat;

namespace Client.Services.AppInfoService;

internal sealed class AppInfoService : IAppInfoService, IEnableLogger
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

    public AppInfoService()
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

    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    private bool IsAdminCheck()
    {
        var isAdmin = false;

        if (!OperatingSystem.IsWindows()) 
            return isAdmin;
           
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Exception e)
        {
            this.Log().StructLogError("Can't check admin role for Windowns", e.Message);
            isAdmin = false;
        }

        return isAdmin;
    }

    #endregion

}