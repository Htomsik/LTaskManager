
using System;
using System.Security.Principal;

namespace Client.Services.AppInfoService;

internal sealed class AppInfoService : IAppInfoService
{
    public bool IsAdminMode => IsAdminCheck();
    
    private bool IsAdminCheck()
    {
        if (!OperatingSystem.IsWindows()) ;
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        return false;
    }
}