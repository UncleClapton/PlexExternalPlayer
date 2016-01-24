using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clapton.Integration
{
    public class SysTrayIcon
    {
        private NotifyIcon _Icon;
        private NotifyIcon Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        public SysTrayIcon(Icon trayIcon, MouseEventHandler clickAction, ContextMenu contextMenu = null)
        {
            Icon = new NotifyIcon();
            Icon.MouseClick += clickAction;
            Icon.Icon = trayIcon;
            Icon.Visible = true;

            if (contextMenu != null)
                Icon.ContextMenu = contextMenu;
        }

        public void Hide()
        {
            Icon.Visible = false;
        }

        public void Show()
        {
            Icon.Visible = true;
        }

        public async void Notify(string title, string text, int timeout = 500, EventHandler clickAction = null)
        {
            if (clickAction != null)
            {
                Icon.BalloonTipClicked += clickAction;
            }
            Icon.BalloonTipTitle = title;
            Icon.BalloonTipText = text;
            Icon.ShowBalloonTip(timeout);
            await Task.Delay(timeout);
            Icon.BalloonTipClicked -= clickAction;
        }
    }
}
