using System;
using Client.Models;
using Client.Services.WMIService;


namespace Client.Services.ComputerInfoService;

public sealed class ComputerInfoService : IComputerInfoService
{
   private readonly IWmiService _wmiService;

   #region Properties

   public double TotalPhysicalMemoryBytes { get; set; }

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

      data = _wmiService.GetHardwareInfo(WmiElement.TotalPhysicalRAM.Item1, WmiElement.TotalPhysicalRAM.Item2);

      if (double.TryParse(data, out var parsed))
      {
         TotalPhysicalMemoryBytes = parsed;
      }
   }

   private void InitForUnix()
   {
      // TODO Подумать нужно ли вообще под Linux писать 
   }

   #endregion
   
}