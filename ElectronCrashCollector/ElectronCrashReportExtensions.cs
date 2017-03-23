using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ElectronCrashReport
{
    public static class ElectronCrashReportExtensions
    {
        public static IApplicationBuilder UseElectronCrashReportHandler( this IApplicationBuilder app, PathString mapPath, Func<IElectronCrashReport, HttpContext, Task> crashReportHandler )
        {
            if( app == null ) { throw new ArgumentNullException( nameof( app ) ); }
            if( crashReportHandler == null ) { throw new ArgumentNullException( nameof( crashReportHandler ) ); }
            if( string.IsNullOrEmpty( mapPath ) ) { throw new ArgumentNullException( nameof( mapPath ) ); }

            ElectronCrashCollectorConfiguration config = new ElectronCrashCollectorConfiguration()
            {
                ElectronCrashReportHandler = crashReportHandler,
                MapPath = mapPath,
            };

            app.UseMiddleware<ElectronCrashCollectorMiddleware>( config );

            return app;
        }
    }


}
