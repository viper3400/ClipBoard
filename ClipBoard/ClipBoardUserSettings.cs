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

    }
}
