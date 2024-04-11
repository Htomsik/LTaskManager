using Client.Services.AppInfoService.Base;

namespace Client.Services.AppInfoService;

/// <summary>
///     <seealso cref="IAppInfoService"/> 
/// </summary>
internal sealed class UnixAppInfoService : BaseAppInfoService
{
    protected override bool IsAdminCheck()
    {
        // Todo: сделать для линукс
        return true;
    }
}