
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace Client.Services.AppInfoService;

internal sealed class AppInfoService : IAppInfoService
{
    public bool IsAdminMode => IsAdminCheck();
    
    public string? AppName { get; set; }
        
    public string? AppManufacturer { get; set; }
        
    public string AppGitHub{ get; set; }
        
    public string? AppVersion { get; set; }
    
    private bool IsAdminCheck()
    {
        if (!OperatingSystem.IsWindows());
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        return false;
    }

    public AppInfoService()
    {
        Assembly appInfoService = Assembly.GetEntryAssembly();
        FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(appInfoService.Location);
        AppName = fileVersionInfo.ProductName;
        AppManufacturer = fileVersionInfo.CompanyName;
        AppGitHub = "https://github.com/Htomsik/LTaskManager";
        AppVersion = fileVersionInfo.FileVersion;
    }

}