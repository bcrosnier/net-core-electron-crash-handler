using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElectronCrashReport;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CrashReportApp
{
    public class CrashHandlerService
    {
        private readonly SmtpConfiguration _smtpConfig;
        private readonly CrashHandlerConfig _crashHandlerConfig;

        public CrashHandlerService( IOptions<SmtpConfiguration> smtpConfigAccessor, IOptions<CrashHandlerConfig> crashHandlerConfigAccessor )
        {
            _smtpConfig = smtpConfigAccessor.Value;
            _crashHandlerConfig = crashHandlerConfigAccessor.Value;
        }

        public async Task HandleAsync( IElectronCrashReport crashReport, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            string messageText = CreateMessage(crashReport);
            var textPart = new TextPart( "plain" ) { Text = messageText };

            MimeMessage message = new MimeMessage();

            message.From.Add( new MailboxAddress( _crashHandlerConfig.From ) );
            message.To.AddRange( ParseEmailAddresses( _crashHandlerConfig.To ) );
            if( !string.IsNullOrEmpty( _crashHandlerConfig.ReplyTo ) )
            {
                message.ReplyTo.AddRange( ParseEmailAddresses( _crashHandlerConfig.To ) );
            }

            message.Subject = $"Electron crash report: {crashReport.ProductName} {crashReport.Version} - {crashReport.CrashReportDateUtc.ToString( "s" )}";

            if( crashReport.HasMinidumpFile )
            {
                string baseFilename = EnsureValidFilename($"electron-crash.{crashReport.ProductName}.{crashReport.Version}.{crashReport.CrashReportDateUtc.ToString( "s" )}");
                string dmpFilename = $"{baseFilename}.dmp";
                string zipFilename = $"{baseFilename}.zip";

                var zipStream = await CreateMinidumpZipStreamAsync( crashReport, dmpFilename, cancellationToken );

                var attachment = new MimePart("attachment", "zip")
                {
                    ContentObject = new ContentObject(zipStream, ContentEncoding.Default),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    FileName = zipFilename
                };

                var multipart = new Multipart( "mixed" )
                {
                    textPart,
                    attachment
                };
                message.Body = multipart;
            }
            else
            {
                message.Body = textPart;
            }
            using( var smtpClient = await _smtpConfig.PrepareMailkitSmtpClient() )
            {
                await smtpClient.SendAsync( message, cancellationToken );
            }
        }

        private string CreateMessage( IElectronCrashReport crashReport )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine( $"{crashReport.ProductName} {crashReport.Version} has crashed and submitted this crash report." );
            sb.AppendLine();
            sb.AppendLine( "Details:" );
            foreach( var kvp in crashReport.Payload )
            {
                sb.AppendLine( $"- {kvp.Key}: {kvp.Value}" );
            }
            sb.AppendLine();
            sb.AppendLine( $"This crash was submitted by {crashReport.IpAddress} on {crashReport.CrashReportDateUtc.ToString( "s" )}." );
            sb.AppendLine();
            if( crashReport.HasMinidumpFile )
            {
                sb.AppendLine( "A minidump file is attached to this message for analysis." );
            }
            else
            {
                sb.AppendLine( "No minidump file was submitted with this crash." );
            }

            return sb.ToString();
        }

        private static IEnumerable<MailboxAddress> ParseEmailAddresses( string addressListString )
        {
            return addressListString
                .Split( ';' )
                .Select( s => s.Trim() )
                .Where( s => !string.IsNullOrEmpty( s ) )
                .Select( s => new MailboxAddress( s ) );
        }

        private static string EnsureValidFilename( string s )
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach( char c in invalidChars )
            {
                s = s.Replace( c.ToString(), "" );
            }

            return s;
        }

        private async Task<Stream> CreateMinidumpZipStreamAsync( IElectronCrashReport crashReport, string entryName, CancellationToken cancellationToken )
        {
            MemoryStream ms = new MemoryStream();
            using( ZipArchive za = new ZipArchive( ms, ZipArchiveMode.Create, true ) )
            {
                var entry = za.CreateEntry( entryName );
                using( var entryStream = entry.Open() )
                using( var dmpStream = crashReport.OpenMinidumpFileReader() )
                {
                    await dmpStream.CopyToAsync( entryStream, 81920, cancellationToken );
                }
            }
            ms.Seek( 0, SeekOrigin.Begin );
            return ms;
        }
    }
}