using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipBoard
{
    [SettingsProvider(typeof(PortableSettingsProvider))]
    public class ClipBoardUserSettings : ApplicationSettingsBase
    {
        public ClipBoardUserSettings(string ConfigurationFileName)
            : base ()
        {
            var provider = base.Providers["PortableSettingsProvider"] as PortableSettingsProvider;
            provider.FilePath = ConfigurationFileName;
        }

        [UserScopedSetting]
        [Category("ClipBoard")]
        [DefaultSettingValue("10")]
        [Description("Number of items which are shown in the frequent list. Set this to 0 to disable this feature.")]
        public int MaxItemsInFrequentList
        {
            get { return (int)this["MaxItemsInFrequentList"]; }
            set { this["MaxItemsInFrequentList"] = value; }
        }


        [UserScopedSetting]
        [Category("ClipBoard")]
        [Description("Path and filename of the file where your ClipBoard content will be saved.")]
        public string ContentFile
        {
            get
            {
                var contentFile = String.IsNullOrWhiteSpace((string)this["ContentFile"]) ? 
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clipboard", "content.csv") :
                    (string)this["ContentFile"];

                return contentFile;
            }
            set { this["ContentFile"] = value; }

        }

        [UserScopedSetting]
        [Category("ClipBoard")]
        [DefaultSettingValue("10000")]
        [Description("Text with more chars than this value won't be handled by ClipBoard.")]
        public int MaxCopyTextLength
        {
            get { return (int)this["MaxCopyTextLength"]; }
            set { this["MaxCopyTextLength"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard HotKey")]
        [DefaultSettingValue("Space")]
        [Description("ModifierKeys + HotKey will open ClipBoard Manager window when minimized. See https://msdn.microsoft.com/de-de/library/system.windows.forms.keys(v=vs.110).aspx for available keys.")]
        public string HotKey
        {
            get { return (string)this["HotKey"]; }
            set { this["HotKey"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard HotKey")]
        [DefaultSettingValue("true")]
        [Description("Modfierkey in addition to HotKey")]
        public bool UseCtrlKey
        {
            get { return (bool)this["UseCtrlKey"]; }
            set { this["UseCtrlKey"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard HotKey")]
        [DefaultSettingValue("false")]
        [Description("Modfierkey in addition to HotKey")]
        public bool UseShiftKey
        {
            get { return (bool)this["UseShiftKey"]; }
            set { this["UseShiftKey"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard HotKey")]
        [DefaultSettingValue("false")]
        [Description("Modfierkey in addition to HotKey")]
        public bool UseAltKey
        {
            get { return (bool)this["UseAltKey"]; }
            set { this["UseAltKey"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard HotKey")]
        [DefaultSettingValue("false")]
        [Description("Modfierkey in addition to HotKey")]
        public bool UseWindowsKey
        {
            get { return (bool)this["UseWindowsKey"]; }
            set { this["UseWindowsKey"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard")]
        [DefaultSettingValue("false")]
        [Description("Set true if Clipboard Manager should run on Windows startup.")]
        public bool RunOnStartup
        {
            get { return (bool)this["RunOnStartup"]; }
            set { this["RunOnStartup"] = value; }
        }

        [UserScopedSetting]
        [Category("ClipBoard")]
        [DefaultSettingValue("false")]
        [Description("Set true if Clipboard Manager should run on minimized on startup.")]
        public bool StartMinimized
        {
            get { return (bool)this["StartMinimized"]; }
            set { this["StartMinimized"] = value; }
        }
    }
}
