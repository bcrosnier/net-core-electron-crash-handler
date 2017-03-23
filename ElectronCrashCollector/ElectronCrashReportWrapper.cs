using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace ElectronCrashReport
{
    internal class ElectronCrashReportWrapper : IElectronCrashReport
    {
        private IFormCollection _formCollection;
        private readonly DateTime _crashReportTimeUtc;
        private readonly string _ipAddress;

        internal ElectronCrashReportWrapper( string ipAddress, IFormCollection formCollection )
        {
            _formCollection = formCollection ?? throw new ArgumentNullException( nameof( formCollection ) );
            _crashReportTimeUtc = DateTime.UtcNow;
            _ipAddress = ipAddress;
        }

        public DateTime CrashReportDateUtc => _crashReportTimeUtc;

        public string ElectronVersion => GetFormValueOrDefault( "ver" );

        public string Platform => GetFormValueOrDefault( "platform" );

        public string ProcessType => GetFormValueOrDefault( "process_type" );

        public string Version => GetFormValueOrDefault( "_version" );

        public string ProductName => GetFormValueOrDefault( "_productName" );

        public string CompanyName => GetFormValueOrDefault( "_companyName" );

        public string IpAddress => _ipAddress;

        public IReadOnlyDictionary<string, string> Payload => _formCollection.ToDictionary( ( kvp ) => kvp.Key, ( kvp ) => kvp.Value.ToString() );

        public bool HasMinidumpFile => _formCollection.Files.Count > 0;

        public Stream OpenMinidumpFileReader()
        {
            if( _formCollection.Files.Count > 0 )
            {
                return _formCollection.Files[0].OpenReadStream();
            }
            return null;
        }

        private string GetFormValueOrDefault( string key )
        {
            if( _formCollection.TryGetValue( key, out var v ) )
            {
                return v.ToString();
            }
            return null;
        }
    }


}
