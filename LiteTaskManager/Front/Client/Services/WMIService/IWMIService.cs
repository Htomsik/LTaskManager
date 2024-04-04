namespace Client.Services.WMIService;

/// <summary>
///     Сервис получения данных из WMI
/// </summary>
public interface IWmiService
{
    /// <summary>
    ///     Получение данных из WMI
    /// </summary>
    /// <param name="win32Class">   Наименование класса win32   </param>
    /// <param name="classItemField">   Параметр класса который нужно получить  </param>
    /// <returns></returns>
    public string GetHardwareInfo(string win32Class, string classItemField);
}