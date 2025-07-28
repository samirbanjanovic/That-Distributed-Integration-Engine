using System;
using System.Diagnostics;
using System.Linq;
using TDIE.NodeApi.Models;

namespace TDIE.NodeApi.Extensions
{
    public static class ProcessDetailsModelExtensions
    {
        public static PackageInstanceDetailsModel ToPackageInstanceDetails(this ProcessDetailsModel processDetailsModel)
        {
            Process[] proccesses = Process.GetProcesses();
            if (proccesses.Any(x => x.Id == processDetailsModel.SystemProcessId))
            {
                var process = Process.GetProcessById(processDetailsModel.SystemProcessId);

                return new PackageInstanceDetailsModel
                {

                    PackageName = processDetailsModel.PackageName,
                    Command = processDetailsModel.Command,
                    Arguments = processDetailsModel.Arguments,
                    ProcessUri = processDetailsModel.ProcessUri,
                    NodeProcessId = processDetailsModel.NodeProcessId,
                    StartDateTime = processDetailsModel.StartDateTime,
                    SystemProcessId = process.Id,
                    SystemProcessName = process.ProcessName,
                    WorkingSet64 = process.WorkingSet64,
                    MinWorkingSet = process.MinWorkingSet.ToInt64(),
                    MaxWorkingSet = process.MaxWorkingSet.ToInt64(),
                    ProcessorTimeInSeconds = process.TotalProcessorTime.TotalSeconds,
                    ThreadCount = process.Threads.Count,
                    ProcessorAffinity = process.ProcessorAffinity.ToInt64(),
                    ModuleName = process.MainModule.ModuleName,
                    FileName = process.MainModule.FileName,
                    ModuleMemorySize = process.MainModule.ModuleMemorySize,
                };
            }


            return default;
        }
    }
}
