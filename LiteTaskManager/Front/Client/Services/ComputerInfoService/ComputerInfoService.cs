using System;
using Client.Extensions;
using Client.Services.WMIService;


namespace Client.Services.ComputerInfoService;

public sealed class ComputerInfoService : IComputerInfoService
{
   #region Properties

   public double TotalPhysicalMemoryBytes { get; set; }

   #endregion

   #region Fields

   private readonly IWmiService _wmiService;

   #endregion

   #region Constructor

   public ComputerInfoService(IWmiService wmiService)
   {
      _wmiService = wmiService;

      if (OperatingSystem.IsWindows())
      {
         InitForWindows();
      }
      else
      {
         InitForUnix();
      }
   }

   #endregion

   #region Methods

   private void InitForWindows()
   {
      string data;

      data = _wmiService.GetHardwareInfo(WmiExtension.TotalPhysicalRAM.Item1, WmiExtension.TotalPhysicalRAM.Item2);

      if (double.TryParse(data, out var parsed))
      {
         // По непонятной причине возвращает кбайты
         TotalPhysicalMemoryBytes = parsed * 1024;
      }
   }

   private void InitForUnix()
   {
      // TODO Подумать нужно ли вообще под Linux писать 
   }

   #endregion
   
}