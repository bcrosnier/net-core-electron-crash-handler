using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace CrashReportApp
{
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public bool UseExplicitSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Create, connect and authenticate a <see cref="SmtpClient"/> using this configuration.
        /// </summary>
        /// <param name="cancellationToken">Optional. Cancellation token to use.</param>
        /// <returns>Disposable SmtpClient, already connected and authenticated.</returns>
        public async Task<SmtpClient> PrepareMailkitSmtpClient( CancellationToken cancellationToken = default( CancellationToken ) )
        {
            if( string.IsNullOrEmpty( Host ) ) { throw new InvalidOperationException( $"Required property {nameof( Host )} must be set" ); }

            SmtpClient c = new SmtpClient();
            await c.ConnectAsync( Host, Port, UseExplicitSsl, cancellationToken );
            if( !string.IsNullOrEmpty( Username ) && !string.IsNullOrEmpty( Password ) )
            {
                await c.AuthenticateAsync( Username, Password, cancellationToken );
            }

            return c;
        }
    }
}
