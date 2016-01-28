using System;
using System.IO;
using Clapton.Xml;

namespace PlexExternalPlayerAgent
{
    [Serializable]
    public class Settings
    {
        #region Static Members
        private static readonly string filePath = Path.Combine(
            Environment.CurrentDirectory,
            "UserConfig.xml");

        public static Settings Current { get; private set; }

        private static readonly string CurrentSettingsVersion = "1";

        public static void Load()
        {
            try
            {
                Current = filePath.ReadXml<Settings>();

                if (Current.Settings_FileVersion != CurrentSettingsVersion)
                    Current = new Settings();
            }
            catch (Exception ex)
            {
                Current = new Settings();
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        #endregion

        #region Properties

        #region Settings - Version
        private string _Settings_FileVersion;
        public string Settings_FileVersion
        {
            get { return this._Settings_FileVersion; }
            set { this._Settings_FileVersion = value; }
        }
        #endregion

        #region PlayerPath
        private string _PlayerPath;
        public string PlayerPath
        {
            get { return this._PlayerPath; }
            set { this._PlayerPath = value; }
        }
        #endregion

        #region PlayerPlexArguments
        private string _PlayerPlexArguments;
        public string PlayerPlexArguments
        {
            get { return this._PlayerPlexArguments; }
            set { this._PlayerPlexArguments = value; }
        }
        #endregion

        #region ShowCommandLine
        private bool _ShowCommandLine;
        public bool ShowCommandLine
        {
            get { return this._ShowCommandLine; }
            set { this._ShowCommandLine = value; }
        }
        #endregion

        #region PlayerGenericArguments
        private string _PlayerGenericArguments;
        public string PlayerGenericArguments
        {
            get { return this._PlayerGenericArguments; }
            set { this._PlayerGenericArguments = value; }
        }
        #endregion

        #region EnableGenericProtocol
        private bool _EnableGenericProtocol;
        public bool EnableGenericProtocol
        {
            get { return this._EnableGenericProtocol; }
            set { this._EnableGenericProtocol = value; }
        }
        #endregion

        #region EnableLogging
        private bool _EnableLogging;
        public bool EnableLogging
        {
            get { return this._EnableLogging; }
            set { this._EnableLogging = value; }
        }
        #endregion

        #endregion

        #region Functions
        public Settings()
        {
            PlayerPath = "mpv";
            PlayerPlexArguments = "\"%url%\" --title=\"%fullTitle%\"";
            ShowCommandLine = false;
            PlayerGenericArguments = "\"%url%\" --title=\"%title%\"";
            EnableGenericProtocol = true;
            EnableLogging = false;
        }
        public void Save()
        {
            try
            {
                this.WriteXml(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

        }
        #endregion
    }
}

