using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ElectronCrashReport
{
    public class ElectronCrashCollectorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ElectronCrashCollectorConfiguration _config;

        public ElectronCrashCollectorMiddleware( RequestDelegate next, ElectronCrashCollectorConfiguration config )
        {
            _next = next ?? throw new ArgumentNullException( nameof( next ) );
            _config = config ?? throw new ArgumentNullException( nameof( config ) );
        }

        public async Task Invoke( HttpContext httpContext )
        {
            if( httpContext == null ) { throw new ArgumentNullException( nameof( httpContext ) ); }
            if( httpContext.Request.Method == "POST" && httpContext.Request.Path == _config.MapPath )
            {

                if( httpContext.Request.HasFormContentType && httpContext.Request.Form != null )
                {
                    var crashReport = new ElectronCrashReportWrapper( httpContext.Connection.RemoteIpAddress.ToString(), httpContext.Request.Form );
                    httpContext.Response.StatusCode = StatusCodes.Status200OK;

                    if( _config.ElectronCrashReportHandler == null ) { throw new InvalidOperationException( $"Property {nameof( _config.ElectronCrashReportHandler )} is required in configuration" ); }
                    await _config.ElectronCrashReportHandler( crashReport, httpContext );
                }
                else
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                return;
            }
            await _next.Invoke( httpContext );
        }
    }


}
