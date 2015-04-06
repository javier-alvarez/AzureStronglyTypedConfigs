namespace AzureStronglyTypedConfigs
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    public class AzureConfigGenerator
    {
        private const string OpenConfigurationSettingsTag = "<ConfigurationSettings>";
        private const string CloseConfigurationSettingsTag = "</ConfigurationSettings>";

        private const string SettingTagFormat = "<Setting name=\"{0}\"/>";
        private const string SettingCollectionTagFormat = "<Setting name=\"{0}_item{1}\"/>";

        private const string SettingValueTagFormat = "<Setting name=\"{0}\" value=\"{1}\"/>";
        private const string SettingCollectionValueTagFormat = "<Setting name=\"{0}_item{1}\" value=\"{2}\"/>";

        public string GenerateCsdef(Type type)
        {
            return CreateXML(type, FileType.Csdef);
        }

        public string GenerateCscfg(Type type)
        {
            return CreateXML(type, FileType.Cscfg);
        }

        private static string CreateXML(Type type, FileType fileType)
        {
            StringBuilder sb = new StringBuilder();
            var instaceEnvironment = Activator.CreateInstance(type);

            sb.Append(OpenConfigurationSettingsTag);
            var interfaceMapping = typeof(IAzureCloudServiceEnvironment).GetProperties();
            foreach (var property in type.GetProperties().Where(x => !interfaceMapping.Select(y => y.Name).Contains(x.Name)))
            {
                if (!(fileType == FileType.Csdef && !AzureConfigSetting.GetIncludeInCSDEFFromPropertyInfo(property)))
                {
                    string name = AzureConfigSetting.GetNameFromPropertyInfo(property);
                    if (property.PropertyType != typeof(string) && property.PropertyType.IsIEnumerable())
                    {
                        // Collection type
                        var ienumerable = property.GetValue(instaceEnvironment) as IEnumerable;
                        int capacity = AzureConfigSetting.GetCapacityPropertyInfo(property);
                        int i = 0;
                        foreach (var item in ienumerable)
                        {
                            GenerateCollectionSetting(fileType, sb, name, item, i++);
                        }

                        for (int j = i; j < capacity; j++)
                        {
                            GenerateCollectionSetting(fileType, sb, name, string.Empty, j);
                        }
                    }
                    else
                    {
                        // Primitive type
                        string value = property.GetValue(instaceEnvironment).ToString();
                        if (fileType == FileType.Cscfg)
                        {
                            sb.Append(GenerateSetting(SettingType.Cscfg, name, value));
                        }
                        else if (fileType == FileType.Csdef)
                        {
                            sb.Append(GenerateSetting(SettingType.Csdef, name, value));
                        }
                    }
                }
            }

            sb.Append(CloseConfigurationSettingsTag);
            return sb.ToString();
        }

        private static void GenerateCollectionSetting(FileType fileType, StringBuilder sb, string name, object item, int i)
        {
            if (fileType == FileType.Csdef)
            {
                sb.Append(GenerateSetting(SettingType.CsdefCollection, name, item, i));
            }
            else
            {
                sb.Append(GenerateSetting(SettingType.CscfgCollection, name, item, i));
            }
        }

        private static string GenerateSetting(SettingType settingType, string name, object value, int item = 0)
        {
            if (settingType == SettingType.Cscfg)
            {
                return string.Format(SettingValueTagFormat, name, value);
            }
            else if (settingType == SettingType.Csdef)
            {
                return string.Format(SettingTagFormat, name);
            }
            else if (settingType == SettingType.CscfgCollection)
            {
                return string.Format(SettingCollectionValueTagFormat, name, item, value);
            }
            else if (settingType == SettingType.CsdefCollection)
            {
                return string.Format(SettingCollectionTagFormat, name, item);
            }

            throw new InvalidOperationException();
        }
    }

    public enum SettingType
    {
        Cscfg,
        Csdef,
        CscfgCollection,
        CsdefCollection
    }

    public enum FileType
    {
        Cscfg,
        Csdef
    }

}
