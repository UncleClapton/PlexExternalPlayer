using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexExternalPlayerAgent
{
    class LoggingService
    {
        public static LoggingService Current{ get; private set; }

        public static void load()
        {
            Current = new LoggingService();
        }

        private string _SessionLogFilePath;

        private List<string> _LogBuffer;
        public List<string> LogBuffer
        {
            get { return _LogBuffer; }
            set { _LogBuffer = value; }
        }

        #region Constructor
        internal LoggingService()
        {
            _SessionLogFilePath = GetNewSessionLogFilePath();
            LogBuffer = new List<string>();
        }
        #endregion

        #region Functions

        #region Log
        public void Log(string message)
        {
            string formattedMessage = string.Format("{1:HH:mm:ss} {0}", message, DateTime.Now);
            LogBuffer.Add(formattedMessage);
            Clapton.IO.File.TryWriteToFile(_SessionLogFilePath, formattedMessage + "\n", true);
        }
        #endregion

        #region SaveLog
        public void SaveLog()
        {
            string buffer = string.Join("\n", LogBuffer);
            if (buffer != null && buffer != "")
            {
                Clapton.IO.File.TryWriteToFile(_SessionLogFilePath, buffer, false);
            }
        }
        #endregion

        #region ClearLogBuffer
        public void ClearLogBuffer() { ClearLogBuffer(false); }
        public void ClearLogBuffer(bool saveFirst)
        {
            if (saveFirst)
            {
                SaveLog();
            }
            LogBuffer.Clear();
            _SessionLogFilePath = GetNewSessionLogFilePath();
        }
        #endregion

        #region GetNewSessionLogFilePath
        private string GetNewSessionLogFilePath()
        {
            return "Log-" + DateTime.Now.ToString("yyyyMMddTHHmmss") + ".log";
        }
        #endregion

        #endregion

    }
}
