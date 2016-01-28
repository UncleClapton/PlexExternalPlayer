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
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => ExceptionHandling.ReportException(sender, args.ExceptionObject as Exception);

            using (var httpServer = new HttpServer(new HttpRequestProvider()))
            {
                try
                {
                    httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 7251)));
                }
                catch (Exception)
                {
                    MessageBox.Show($"Error while initilizing agent server. There may be another agent currently running. Closing agent...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
                httpServer.Use((context, next) =>
                {
                    var protocol = context.Request.QueryString.GetByName("protocol");

                    if (protocol == Properties.Resources.PlexProtocol)
                    {
                        #region PlexProtocol

                        Console.WriteLine("Recieved Plex Request!");

                        context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                        var streamPath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("url"));
                        var id = WebUtility.UrlDecode(context.Request.QueryString.GetByName("id")) ?? "";
                        var title = WebUtility.UrlDecode(context.Request.QueryString.GetByName("title")) ?? "";
                        var grandparentTitle = WebUtility.UrlDecode(context.Request.QueryString.GetByName("grandparentTitle")) ?? "";
                        var rating = WebUtility.UrlDecode(context.Request.QueryString.GetByName("rating")) ?? "";
                        var filePath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("filePath")) ?? "";

                        var playerArguments = Properties.Settings.Default.PlayerPlexArguments
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
                                info.FileName = Properties.Settings.Default.PlayerPath;
                                info.Arguments = playerArguments;
                                info.UseShellExecute = Properties.Settings.Default.ShowCommandLine;
                                Process.Start(info);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show($"Error running {streamPath}  due to : {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Blank or null url recieived, Unable to open");
                        }


                        return Task.Factory.GetCompleted();
                        #endregion
                    }
                    else if (protocol == Properties.Resources.GenericProtocol && Properties.Settings.Default.EnableGenericProtocol)
                    {
                        #region GenericProtocol

                        Console.WriteLine("Recieved Generic Request!");

                        context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                        var streamPath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("url"));
                        var title = context.Request.QueryString.GetByName("title") ?? "";

                        var playerArguments = Properties.Settings.Default.PlayerGenericArguments
                                                .Replace("%url%", streamPath)
                                                .Replace("%title%", title);

                        if (!string.IsNullOrWhiteSpace(streamPath))
                        {
                            try
                            {
                                var info = new ProcessStartInfo();
                                info.FileName = Properties.Settings.Default.PlayerPath;
                                info.Arguments = playerArguments;
                                info.UseShellExecute = Properties.Settings.Default.ShowCommandLine;
                                Process.Start(info);
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show($"Error running {streamPath}  due to : {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Blank or null url recieived, Unable to open");
                        }


                        return Task.Factory.GetCompleted();
                        #endregion
                    }
                    else
                    {
                        Console.WriteLine("Recieved Invalid Request!");

                        if(Properties.Settings.Default.UnsupportedProtocols.Contains(protocol))
                            MessageBox.Show($"Invalid protocol recieived: {protocol}. This protocol is outdated and no longer supported. Please update your script(s).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else if(protocol == Properties.Resources.GenericProtocol)
                            MessageBox.Show($"Invalid protocol recieived: {protocol}. Generic Protcol is disabled.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else
                            MessageBox.Show($"Invalid protocol recieived: {protocol}. The protocol recieived is invalid. Please contact the executing script's developer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);



                        return Task.Factory.GetCompleted();
                    }
                });

                httpServer.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
