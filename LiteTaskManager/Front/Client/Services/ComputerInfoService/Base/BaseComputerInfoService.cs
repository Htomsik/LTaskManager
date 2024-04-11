namespace Client.Services.ComputerInfoService.Base;

/// <summary>
///   <seealso cref="IComputerInfoService"/>
/// </summary>
internal abstract class BaseComputerInfoService : IComputerInfoService
{
   #region Properties

   public double TotalPhysicalMemoryBytes { get; set; }

   #endregion
   
   #region Methods

   protected abstract void InitData();
   
   #endregion
}