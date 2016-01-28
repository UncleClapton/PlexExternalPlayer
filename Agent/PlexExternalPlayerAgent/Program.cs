using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using uhttpsharp;
using uhttpsharp.Listeners;
using Clapton.Dialog;
using uhttpsharp.RequestProviders;
using Clapton.Exceptions;

namespace PlexExternalPlayerAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings.Load();
            LoggingService.load();
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ExceptionHandling.ReportException(sender, args.ExceptionObject as Exception);

            using (var httpServer = new HttpServer(new HttpRequestProvider()))
            {
                try
                {
                    if (Settings.Current.EnableLogging) LoggingService.Current.Log("Starting Player Agent. Advanced Logging is active, Welcome!");
                    httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 7251)));
                }
                catch (Exception e)
                {
                    Dialogs.ErrorDialog($"Error while initilizing agent server. There may be another agent currently running. Closing agent...");
                    LoggingService.Current.Log($"Error while initilizing agent server\n------ Start Stack Trace ------\n{e.Message}\n{e.StackTrace}\n------ End stack trace ------");
                    Environment.Exit(1);
                }

                httpServer.Use((context, next) =>
                {
                    context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                    var protocol = GetQueryStringProperty(context.Request, "protocol");

                    if (protocol == Properties.Resources.PlexProtocol)
                    {
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log("Recieived plex request!");
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log($"Raw request data:{context.Request.ToString()}");
                        ServePlexProtocolRequest(context);
                    }
                    else if (protocol == Properties.Resources.GenericProtocol && Settings.Current.EnableGenericProtocol)
                    {
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log("Recieved generic request!");
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log($"Raw request data:{context.Request.ToString()}");
                        ServeGenericProtocolRequest(context);
                    }
                    else
                    {
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log("Recieved invalid request!");
                        if (Settings.Current.EnableLogging) LoggingService.Current.Log($"Raw request data:{context.Request.ToString()}");
                        HandleInvalidProtocolCode(protocol);
                    }
                    return Task.Factory.GetCompleted();
                });

                httpServer.Start();

                Application.ApplicationExit += new EventHandler(onExit);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }

        private static void onExit(object sender, EventArgs e)
        {
            Settings.Current.Save();
            LoggingService.Current.SaveLog();
        }

        private static string GetQueryStringProperty(IHttpRequest request, string queryStringName)
        {
            var propertyValue = "";
            request.QueryString.TryGetByName(queryStringName, out propertyValue);
            return WebUtility.UrlDecode(propertyValue);
        }

        private static void ServePlexProtocolRequest(IHttpContext context)
        {
            var streamPath = GetQueryStringProperty(context.Request, "url");
            var id = GetQueryStringProperty(context.Request, "id");
            var title = GetQueryStringProperty(context.Request, "title");
            var grandparentTitle = GetQueryStringProperty(context.Request, "grandparentTitle");
            var rating = GetQueryStringProperty(context.Request, "rating");
            var filePath = GetQueryStringProperty(context.Request, "filePath");

            var playerArguments = Settings.Current.PlayerPlexArguments
                                    .Replace("%url%", streamPath)
                                    .Replace("%fileId%", id)
                                    .Replace("%title%", title)
                                    .Replace("%seriesTitle%", grandparentTitle)
                                    .Replace("%fullTitle%", (string.IsNullOrWhiteSpace(grandparentTitle) ? "" : grandparentTitle + " - ") + title)
                                    .Replace("%contentRating%", rating)
                                    .Replace("%filePath%", filePath);

            if (!string.IsNullOrWhiteSpace(streamPath))
            {
                try
                {
                    var info = new ProcessStartInfo();
                    info.FileName = Settings.Current.PlayerPath;
                    info.Arguments = playerArguments;
                    info.UseShellExecute = Settings.Current.ShowCommandLine;
                    Process.Start(info);
                    if (Settings.Current.EnableLogging) LoggingService.Current.Log($"Running command: {info.FileName} {info.Arguments}");
                }
                catch (Exception e)
                {
                    Dialogs.ErrorDialog($"Error running {streamPath}  due to : {e.Message}");
                    LoggingService.Current.Log($"Error while trying to launch player application\n------ Start Stack Trace ------\n{e.Message}\n{e.StackTrace}\n------ End stack trace ------");
                }
            }
            else
            {
                Dialogs.WarningDialog($"Blank or null url recieived, Unable to open", "Unable to Open file...");
            }
            return;
        }

        private static void ServeGenericProtocolRequest(IHttpContext context)
        {
            var streamPath = GetQueryStringProperty(context.Request, "url");
            var title = GetQueryStringProperty(context.Request, "title");

            var playerArguments = Settings.Current.PlayerGenericArguments
                                    .Replace("%url%", streamPath)
                                    .Replace("%title%", title);

            if (!string.IsNullOrWhiteSpace(streamPath))
            {
                try
                {
                    var info = new ProcessStartInfo();
                    info.FileName = Settings.Current.PlayerPath;
                    info.Arguments = playerArguments;
                    info.UseShellExecute = Settings.Current.ShowCommandLine;
                    Process.Start(info);
                    if (Settings.Current.EnableLogging) LoggingService.Current.Log($"Running command: {info.FileName} {info.Arguments}");
                }
                catch (Exception e)
                {
                    Dialogs.ErrorDialog($"Error running {streamPath}  due to : {e.Message}");
                    LoggingService.Current.Log($"Error while trying to launch player application\n------ Start Stack Trace ------\n{e.Message}\n{e.StackTrace}\n------ End stack trace ------");
                }
            }
            else
            {
                Dialogs.WarningDialog($"Blank or null url recieived, Unable to open", "Unable to Open file...");
            }
            return;
        }

        private static void HandleInvalidProtocolCode(string protocol)
        {
            if (Properties.Settings.Default.UnsupportedProtocols.Contains(protocol))
                Dialogs.ErrorDialog($"Invalid protocol recieived: {protocol}. This protocol is outdated and no longer supported. Please update your script(s).");
            else if (protocol == Properties.Resources.GenericProtocol)
                Dialogs.ErrorDialog($"Invalid protocol recieived: {protocol}. Generic Protcol is disabled.");
            else
                Dialogs.ErrorDialog($"Invalid protocol recieived: {protocol}. The protocol recieived is invalid. Please contact the executing script's developer.");
            return;
        }
    }
}
