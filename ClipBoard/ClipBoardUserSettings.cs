using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipBoard
{
    [SettingsProvider(typeof(PortableSettingsProvider))]
    public class ClipBoardUserSettings : ApplicationSettingsBase
    {
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
            get { return (string)this["ContentFile"]; }
            set { this["ContentFile"] = value; }

        }

    }
}
