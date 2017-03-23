using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ElectronCrashReport
{

    public class ElectronCrashCollectorConfiguration
    {
        public PathString MapPath { get; set; }
        public Func<IElectronCrashReport, HttpContext, Task> ElectronCrashReportHandler { get; set; }
    }


}
