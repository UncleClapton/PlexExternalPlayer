using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uhttpsharp;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;

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

            using (var httpServer = new HttpServer(new HttpRequestProvider()))
            {
                httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 7251)));
                httpServer.Use((context, next) =>
                {
                    var protocol = context.Request.QueryString.GetByName("protocol");

                    if (protocol == Properties.Resources.PlexProtocol)
                    {
                        #region PlexProtocol

                        Console.WriteLine("Recieved Plex Request!");

                        context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                        var streamPath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("url"));
                        var id = context.Request.QueryString.GetByName("id") ?? "";
                        var title = context.Request.QueryString.GetByName("title") ?? "";
                        var grandparentTitle = context.Request.QueryString.GetByName("grandfatherTitle") ?? "";
                        var rating = context.Request.QueryString.GetByName("rating") ?? "";
                        var filePath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("filePath")) ?? "";

                        var playerArguments = Properties.Settings.Default.PlayerPlexArguments
                                                .Replace("%url%", streamPath)
                                                .Replace("%fileId%", id)
                                                .Replace("%title%", title)
                                                .Replace("%seriesTitle%", grandparentTitle )
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
                    else if (protocol == Properties.Resources.GenericProtocol)
                    {
                        #region GenericProtocol

                        Console.WriteLine("Recieved Generic Request!");

                        context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                        var streamPath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("url"));
                        var title = context.Request.QueryString.GetByName("title") ?? "";

                        var playerArguments = Properties.Settings.Default.PlayerPlexArguments
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

                        MessageBox.Show($"Invalid protocol recieived: {protocol}. Ensure the sending script is updated and valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
