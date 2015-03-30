namespace AzureStronglyTypedConfigs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;

    public class Program
    {
        public const string ServiceConfigurationSchema = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";
        public const string ServiceDefinitionSchema = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceDefinition";

        private static string PathToSolution;
        private static string PathToAssembly;

        private static readonly AzureConfigGenerator Generator = new AzureConfigGenerator();

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
                var instaceEnvironment = Activator.CreateInstance(environment);
                string pathToConfigs = (string)environment.GetProperty("CloudServiceConfigsFolder").GetValue(instaceEnvironment);
                var pathToFile = string.Format(@"{0}\{1}\ServiceConfiguration.{2}.cscfg", PathToSolution, pathToConfigs, environment.GetProperty("EnvironmentName").GetValue(instaceEnvironment).ToString());

                string newSettings = Generator.GenerateCscfg(environment);

                ReplaceConfigurationSettings(pathToFile, new[] { "ServiceConfiguration", "Role", "ConfigurationSettings" }, ServiceConfigurationSchema, newSettings);

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
            var instaceEnvironment = Activator.CreateInstance(firstEnv);

            string pathToConfigs = (string)firstEnv.GetProperty("CloudServiceConfigsFolder").GetValue(instaceEnvironment);

            string newCsdef = Generator.GenerateCsdef(firstEnv);

            var pathToFile = string.Format(@"{0}\{1}\ServiceDefinition.csdef", PathToSolution, pathToConfigs);
            ReplaceConfigurationSettings(
                pathToFile,
                new[] { "ServiceDefinition", "WorkerRole", "ConfigurationSettings" },
                ServiceDefinitionSchema,
                newCsdef);
        }



        private static void ReplaceConfigurationSettings(string pathToFile, IEnumerable<string> pathToConfigSettings, string xmlnamespace, string xmlToReplace)
        {
            XmlDocument document = new XmlDocument();
            document.Load(pathToFile);
            XPathNavigator navigator = document.CreateNavigator();
            bool lastMoveGood = false;
            foreach (var element in pathToConfigSettings)
            {
                lastMoveGood = navigator.MoveToChild(element, xmlnamespace);
            }
            if (lastMoveGood)
            {
                navigator.ReplaceSelf(xmlToReplace);
            }
            else
            {
                navigator.AppendChild(xmlToReplace);
            }

            document.Save(pathToFile);
        }
    }
}
