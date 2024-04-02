using System;
using System.Collections.Generic;
using System.Management;
using Client.Infrastructure.Logging;
using ReactiveUI;
using Splat;

namespace Client.Services.WMIService;

public class WmiService : ReactiveObject, IWmiService
{
    public string GetHardwareInfo(string win32Class, string classItemField)
    {
        var result = new List<string>();

        var searcher = new ManagementObjectSearcher("SELECT * FROM " + win32Class);

        try 
        {
            foreach (var obj in searcher.Get()) 
            {
                result.Add(obj[classItemField].ToString().Trim());
            }
        }
        catch (Exception e) 
        {
            this.Log().StructLogError($"Can't get {win32Class}:{classItemField}",e.Message);
        }

        return string.Join(", ", result);
    }
}