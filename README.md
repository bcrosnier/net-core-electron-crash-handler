# .NET Core Electron crash report handler

Various [electron crash-reporter](https://github.com/electron/electron/blob/master/docs/api/crash-reporter.md) collector utilities, providing a means to handle them and transfer them to configurable e-mail addresses.

**These utilities currently do not store, process symbols and/or translate crash dumps on their own.** For actual symbol servers, see projects like these:
- [mozilla/socorro](https://github.com/mozilla/socorro)
- [electron/mini-breakpad-server](https://github.com/electron/mini-breakpad-server)

----

Includes these projects:

## ElectronCrashCollector

.NET Core middleware to handle [electron crash-reporter](https://github.com/electron/electron/blob/master/docs/api/crash-reporter.md) requests and provide a means to handle them.

### Usage

In your `Startup.Configure()` method, or equivalent:

```csharp
// "/app-crash" is the path you will use in crashReporter's submitURL property
app.UseElectronCrashReportHandler( "/app-crash", async ( crashReport, httpContext ) => {
	// Do whatever you want with crashReport here (eg. store or transfer them)
	//await httpContext.RequestServices.GetService<CrashHandlerService>().HandleAsync( crashReport );
} );
```

## CrashReportApp

Simple ASP.NET Core web app that uses ElectronCrashCollector to handle electron crash-reporter queries on path `/app-crash` and send a mail using configured settings.

### Usage

1. Copy `CrashReportApp/appsettings.default.json` to `CrashReportApp/appsettings.json`
2. Edit `CrashReportApp/appsettings.json` with correct SMTP settings and e-mail addresses
3. Configure [electron crash-reporter](https://github.com/electron/electron/blob/master/docs/api/crash-reporter.md) in your Electron app to target http://your-server/app-crash, in both your main process and renderer process
4. Run your server
5. Crash your Electron app (eg. by calling `process.crash()` in either the main process or the renderer process)


# TODO

- NuGet package
