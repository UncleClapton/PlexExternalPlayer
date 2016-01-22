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
                    Console.WriteLine("Got Request!");

                    var protocol = context.Request.QueryString.GetByName("protocol");

                    if (protocol != Properties.Resources.ExpectedProtocol)
                    {
                        MessageBox.Show($"Agent and script version differ.  Agent: {Properties.Resources.ExpectedProtocol}  Script : {protocol}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return Task.Factory.GetCompleted();
                    }

                    context.Response = HttpResponse.CreateWithMessage(HttpResponseCode.Ok, "Received", false);

                    var streamPath = WebUtility.UrlDecode(context.Request.QueryString.GetByName("item"));
                    if (streamPath.EndsWith(".avi") ||
                        streamPath.EndsWith(".mkv") ||
                        streamPath.EndsWith(".mp4") ||
                        streamPath.EndsWith(".mpg") ||
                        streamPath.EndsWith(".ts") ||
                        streamPath.EndsWith(".mpeg"))
                    {
                        try
                        {
                            var info = new ProcessStartInfo();
                            info.FileName = Properties.Settings.Default.PlayerPath;
                            info.Arguments = streamPath + Properties.Settings.Default.PlayerArguments;
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
                        MessageBox.Show($"Tried to run {streamPath} but it wasn't allowed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return Task.Factory.GetCompleted();
                });

                httpServer.Start();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
