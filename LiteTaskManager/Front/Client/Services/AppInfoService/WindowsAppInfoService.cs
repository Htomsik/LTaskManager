using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using Client.Infrastructure.Logging;
using Client.Services.AppInfoService.Base;
using Splat;

namespace Client.Services.AppInfoService;

/// <summary>
///     <seealso cref="IAppInfoService"/> для Windows
/// </summary>
[SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
internal sealed class WindowsAppInfoService : BaseAppInfoService
{
    protected override bool IsAdminCheck()
    {
        bool isAdmin;
        
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
}