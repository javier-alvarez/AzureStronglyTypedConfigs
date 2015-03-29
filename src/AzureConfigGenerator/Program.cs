using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureConfigGenerator
{
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.XPath;

    using AzureConfiguration;
    using System.Collections;


    public class Program
    {
        public const string ServiceConfigurationSchema = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";
        public const string ServiceDefinitionSchema = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition";

        private static string PathToSolution;
        private static string PathToAssembly;

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Missing path to assembly containing service configs and environments and/or path to solution");
            }

            PathToAssembly = args[0];
            PathToSolution = args[1];

            var assembly = Assembly.LoadFrom(PathToAssembly);


            var iEnvironmnetType = typeof(IAzureCloudServiceEnvironment);

            var environments = assembly.GetTypes().Where(type => iEnvironmnetType.IsAssignableFrom(type));

            // Fix cscfg
            foreach (var environment in environments)
            {
                StringBuilder sb = new StringBuilder();
                var instaceEnvironment = Activator.CreateInstance(environment);

                string pathToConfigs = (string)environment.GetProperty("CloudServiceConfigsFolder").GetValue(instaceEnvironment);
                sb.Append("<ConfigurationSettings>\r\n");
                var interfaceMapping = typeof(IAzureCloudServiceEnvironment).GetProperties();
                foreach (var property in environment.GetProperties().Where(x => !interfaceMapping.Select(y => y.Name).Contains(x.Name)))
                {
                    string name = AzureConfigSetting.GetNameFromPropertyInfo(property);
                    if (property.PropertyType != typeof(string) && property.PropertyType.IsIEnumerable())
                    {
                        var ienumerable = property.GetValue(instaceEnvironment) as IEnumerable;
                        int i = 0;
                        foreach (var item in ienumerable)
                        {
                            sb.AppendFormat("\t<Setting name=\"{0}_item{1}\" value=\"{2}\"/>\r\n", name, i++, item);
                        }
                        // Add extra capacity
                        int extraCapacity = AzureConfigSetting.GetExtraCapacityPropertyInfo(property);
                        for (int j = 0; j < extraCapacity; j++)
                        {
                            sb.AppendFormat("\t<Setting name=\"{0}_item{1}\" value=\"\"/>\r\n", name, j+i);
                        }
                    }
                    else
                    {
                        string value = property.GetValue(instaceEnvironment).ToString();
                        sb.AppendFormat("\t<Setting name=\"{0}\" value=\"{1}\"/>\r\n", name, value);
                    }
                }

                sb.Append("</ConfigurationSettings>\r\n");
                var pathToFile = string.Format(@"{0}\{1}\ServiceConfiguration.{2}.cscfg", PathToSolution, pathToConfigs, environment.GetProperty("EnvironmentName").GetValue(instaceEnvironment).ToString());
                ReplaceConfigurationSettings(pathToFile, new[] { "ServiceConfiguration", "Role", "ConfigurationSettings" }, ServiceConfigurationSchema, sb.ToString());

                // Change number of instances
                int numberOfInstances = (int)environment.GetProperty("NumberOfInstances").GetValue(instaceEnvironment);
                string xmlToReplace = string.Format("<Instances count=\"{0}\"/>", numberOfInstances);
                ReplaceConfigurationSettings(pathToFile, new[] { "ServiceConfiguration", "Role", "Instances" }, ServiceConfigurationSchema, xmlToReplace);

            }

            // Fix the csdef
            UpdateCSDEF(environments);

        }




        private static void UpdateCSDEF(IEnumerable<Type> environments)
        {
            var firstEnv = environments.First();
            StringBuilder sb = new StringBuilder();
            var instaceEnvironment = Activator.CreateInstance(firstEnv);

            string pathToConfigs = (string)firstEnv.GetProperty("CloudServiceConfigsFolder").GetValue(instaceEnvironment);
            sb.Append("<ConfigurationSettings>\r\n");
            var interfaceMapping = typeof(IAzureCloudServiceEnvironment).GetProperties();
            foreach (var property in firstEnv.GetProperties().Where(x => !interfaceMapping.Select(y => y.Name).Contains(x.Name)))
            {
                if (AzureConfigSetting.GetIncludeInCSDEFFromPropertyInfo(property))
                {
                    string name = AzureConfigSetting.GetNameFromPropertyInfo(property);
                    if (property.PropertyType != typeof(string) && property.PropertyType.IsIEnumerable())
                    {
                        var ienumerable = property.GetValue(instaceEnvironment) as IEnumerable;
                        int i = 0;
                        foreach (var item in ienumerable)
                        {
                            sb.AppendFormat("\t<Setting name=\"{0}_item{1}\"/>\r\n", name, i++);
                        }
                        int extraCapacity = AzureConfigSetting.GetExtraCapacityPropertyInfo(property);
                        for (int j = 0; j < extraCapacity; j++)
                        {
                            sb.AppendFormat("\t<Setting name=\"{0}_item{1}\"/>\r\n", name, j + i);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("\t<Setting name=\"{0}\"/>\r\n", name);
                    }
                }
            }
            sb.Append("</ConfigurationSettings>\r\n");

            var pathToFile = string.Format(@"{0}\{1}\ServiceDefinition.csdef", PathToSolution, pathToConfigs);
            ReplaceConfigurationSettings(pathToFile, new[] { "ServiceDefinition", "WorkerRole", "ConfigurationSettings" }, ServiceDefinitionSchema, sb.ToString());
        }



        private static void ReplaceConfigurationSettings(string pathToFile, IEnumerable<string> pathToConfigSettings, string xmlnamespace, string xmlToReplace)
        {
            XmlDocument document = new XmlDocument();
            document.Load(pathToFile);
            XPathNavigator navigator = document.CreateNavigator();
            foreach (var element in pathToConfigSettings)
            {
                navigator.MoveToChild(element, xmlnamespace);
            }

            navigator.ReplaceSelf(xmlToReplace);
            document.Save(pathToFile);
        }
    }
}
