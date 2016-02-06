using Clapton.Xml;
using System;
using System.IO;

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

                if (Current.SettingsVersion != CurrentSettingsVersion)
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
        private string _SettingsVersion;
        public string SettingsVersion
        {
            get { return this._SettingsVersion; }
            set { this._SettingsVersion = value; }
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
        private bool _EnableAdvancedLogging;
        public bool EnableAdvancedLogging
        {
            get { return this._EnableAdvancedLogging; }
            set { this._EnableAdvancedLogging = value; }
        }
        #endregion

        #endregion

        #region Functions
        public Settings()
        {
            SettingsVersion = CurrentSettingsVersion;
            PlayerPath = "mpv";
            PlayerPlexArguments = "\"%url%\" --title=\"%fullTitle%\"";
            ShowCommandLine = false;
            PlayerGenericArguments = "\"%url%\" --title=\"%title%\" --start=\"%time%\"";
            EnableGenericProtocol = true;
            EnableAdvancedLogging = false;
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

