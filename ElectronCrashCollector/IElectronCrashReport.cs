using System;
using System.Collections.Generic;
using System.IO;

namespace ElectronCrashReport
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// See also: https://github.com/electron/electron/blob/master/docs/api/crash-reporter.md
    /// </remarks>
    public interface IElectronCrashReport
    {
        /// <summary>
        /// Date at which the crash report was received, in UTC
        /// </summary>
        DateTime CrashReportDateUtc { get; }

        /// <summary>
        /// Version of Electron, as reported by "ver"
        /// </summary>
        string ElectronVersion { get; }

        /// <summary>
        /// Prodct platform, as reported by "platform"
        /// </summary>
        /// <example>win32</example>
        string Platform { get; }

        /// <summary>
        /// Process type, as reported by "process_type"
        /// </summary>
        /// <example>browser</example>
        /// <example>renderer</example>
        string ProcessType { get; }

        /// <summary>
        /// Product version as reported by "_version"
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Product name as reported by "_productName"
        /// </summary>
        string ProductName { get; } // Product


        /// <summary>
        /// Company name as reported by "_companyName"
        /// </summary>
        string CompanyName { get; }


        /// <summary>
        /// IP Address that submitted the report
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// Complete request payload with extras, excluding the minidump binary file
        /// </summary>
        IReadOnlyDictionary<string, string> Payload { get; }

        /// <summary>
        /// Opens a read stream to the uploaded minidump file
        /// </summary>
        /// <returns>null if no minidump file is provided, otherwise a read stream to the minidump file</returns>
        Stream OpenMinidumpFileReader();


        /// <summary>
        /// Whether this crash report has a minidump file that can be read using <see cref="OpenMinidumpFileReader"/>
        /// </summary>
        bool HasMinidumpFile { get; }
    }


}
