using System;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml;
using System.IO;

namespace ClipBoard
{
    /// <summary>
    /// A settings provider: https://github.com/everweb/PortableSettingsProvider
    /// </summary>
    public sealed class PortableSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        //TEST//private static string _machineName = null;
        private const string _rootNodeName = "settings";
        private const string _localSettingsNodeName = "localSettings";
        private const string _globalSettingsNodeName = "globalSettings";
        private const string _className = "PortableSettingsProvider";
        private string _appName = null;
        private XmlDocument _xmlDocument;

        //TEST//public static void OverrideMachineName(string machineName)
        //TEST//{
        //TEST//	_machineName = machineName;
        //TEST//}

        private string _filePath
        {
            get
            {
                //Not available to Console App// Path.GetDirectoryName(Application.ExecutablePath)			
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                   string.Format("{0}.settings", ApplicationName));
            }
        }

        private XmlNode _localSettingsNode
        {
            get
            {
                XmlNode settingsNode = GetSettingsNode(_localSettingsNodeName);

                string xpath = Environment.MachineName.ToLowerInvariant();
                //TEST//if (_machineName != null)
                //TEST//{
                //TEST//	xpath = _machineName;
                //TEST//}
                if (!string.IsNullOrEmpty(xpath))
                {
                    //XPath should not begin with a number or symbol. Prefix with an arbitrary 'm'.
                    //There are a lot more rules in System/Xml/XPath/Internal/XPathScanner.NextLex() but machine names have rules of their own...
                    if (char.IsDigit(xpath, 0) || char.IsSymbol(xpath, 0) || char.IsPunctuation(xpath, 0) || char.IsControl(xpath, 0))
                    {
                        xpath = 'm' + xpath;
                    }
                }
                XmlNode machineNode = settingsNode.SelectSingleNode(xpath);

                if (machineNode == null)
                {
                    machineNode = _rootDocument.CreateElement(xpath);
                    settingsNode.AppendChild(machineNode);
                }

                return machineNode;
            }
        }

        private XmlNode _globalSettingsNode
        {
            get { return GetSettingsNode(_globalSettingsNodeName); }
        }

        private XmlNode _rootNode
        {
            get { return _rootDocument.SelectSingleNode(_rootNodeName); }
        }

        private XmlDocument _rootDocument
        {
            get
            {
                if (_xmlDocument == null)
                {
                    try
                    {
                        _xmlDocument = new XmlDocument();
                        _xmlDocument.Load(_filePath);
                    }
                    catch (Exception)
                    {
                    }

                    if (_xmlDocument.SelectSingleNode(_rootNodeName) != null)
                        return _xmlDocument;

                    _xmlDocument = GetBlankXmlDocument();
                }

                return _xmlDocument;
            }
        }

        public override string ApplicationName
        {
            get
            {
                if (_appName == null)
                {
                    //Not available to Console App// Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                    _appName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().FullName);
                    //Not available to Test Case//_appName = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
                }
                return _appName;
            }
            set
            {
                //Since ApplicationName.set is required, implement it
                _appName = value;
            }
        }

        public override string Name
        {
            get { return _className; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(Name, config);
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            foreach (SettingsPropertyValue propertyValue in collection)
            {
                SetValue(propertyValue);
            }

            try
            {
                _rootDocument.Save(_filePath);
            }
            catch (Exception)
            {
                /* 
                 * If this is a portable application and the device has been 
                 * removed then this will fail, so don't do anything. It's 
                 * probably better for the application to stop saving settings 
                 * rather than just crashing outright. Probably.
                 */
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty property in collection)
            {
                values.Add(new SettingsPropertyValue(property)
                {
                    SerializedValue = GetValue(property)
                });
            }

            return values;
        }

        private void SetValue(SettingsPropertyValue propertyValue)
        {
            XmlNode targetNode = IsGlobal(propertyValue.Property)
               ? _globalSettingsNode
               : _localSettingsNode;

            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", propertyValue.Name));

            if (settingNode != null)
                settingNode.InnerText = propertyValue.SerializedValue.ToString();
            else
            {
                settingNode = _rootDocument.CreateElement("setting");

                XmlAttribute nameAttribute = _rootDocument.CreateAttribute("name");
                nameAttribute.Value = propertyValue.Name;

                settingNode.Attributes.Append(nameAttribute);
                settingNode.InnerText = propertyValue.SerializedValue.ToString();

                targetNode.AppendChild(settingNode);
            }
        }

        private string GetValue(SettingsProperty property)
        {
            XmlNode targetNode = IsGlobal(property) ? _globalSettingsNode : _localSettingsNode;
            XmlNode settingNode = targetNode.SelectSingleNode(string.Format("setting[@name='{0}']", property.Name));

            if (settingNode == null)
                return property.DefaultValue != null ? property.DefaultValue.ToString() : string.Empty;

            return settingNode.InnerText;
        }

        private bool IsGlobal(SettingsProperty property)
        {
            foreach (DictionaryEntry attribute in property.Attributes)
            {
                if ((Attribute)attribute.Value is SettingsManageabilityAttribute)
                    return true;
            }

            return false;
        }

        private XmlNode GetSettingsNode(string name)
        {
            XmlNode settingsNode = _rootNode.SelectSingleNode(name);

            if (settingsNode == null)
            {
                settingsNode = _rootDocument.CreateElement(name);
                _rootNode.AppendChild(settingsNode);
            }

            return settingsNode;
        }

        public XmlDocument GetBlankXmlDocument()
        {
            XmlDocument blankXmlDocument = new XmlDocument();
            blankXmlDocument.AppendChild(blankXmlDocument.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
            blankXmlDocument.AppendChild(blankXmlDocument.CreateElement(_rootNodeName));

            return blankXmlDocument;
        }

        public void Reset(SettingsContext context)
        {
            _localSettingsNode.RemoveAll();
            _globalSettingsNode.RemoveAll();

            _xmlDocument.Save(_filePath);
        }

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            // do nothing
            return new SettingsPropertyValue(property);
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
        }
    }
}