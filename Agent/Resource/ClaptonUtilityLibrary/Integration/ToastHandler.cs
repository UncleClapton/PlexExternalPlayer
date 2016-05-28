using System;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Clapton.Integration
{
    public class ToastHandler
    {
        private string APP_ID = "";
        public static ToastHandler Current { get; private set; }

        public static void load(string appID)
        {
            Current = new ToastHandler();
            Current.APP_ID = appID;
        }

        public void SendSimpleToast(string imagePath, string toastLine1, string toastLine2, string toastLine3)
        {

            // Get a toast XML template
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            
            stringElements[0].AppendChild(toastXml.CreateTextNode(toastLine1));
            stringElements[1].AppendChild(toastXml.CreateTextNode(toastLine2));
            stringElements[2].AppendChild(toastXml.CreateTextNode(toastLine3));

            // Specify the absolute path to an image
            imagePath = "file:///" + Path.GetFullPath(imagePath);
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create the toast and attach event listeners
            ToastNotification toast = new ToastNotification(toastXml);

            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }
    }
}
