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
        public int MaxItemsInFrequentList
        {
            get { return (int)this["MaxItemsInFrequentList"]; }
            set { this["MaxItemsInFrequentList"] = value; }
        }


        [UserScopedSetting]
        [Category("ClipBoard")]
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

    }
}
