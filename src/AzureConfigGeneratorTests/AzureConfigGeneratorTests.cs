namespace AzureConfigGeneratorTests
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Xml;

    using AzureConfigGenerator;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Xml.Linq;
    using WorkerRole1;

    [TestClass]
    public class AzureConfigGeneratorTests
    {
        [TestMethod]
        public void TestCsdef()
        {
            var generator = new AzureConfigGenerator();
            string output = generator.GenerateCsdef(typeof(MyServiceProdEnvironment));
            Console.WriteLine(output);
            var expected = File.ReadAllText("CsdefExpectedOutput.xml");
            Assert.IsTrue(XDocument.DeepEquals(XDocument.Parse(expected), XDocument.Parse(output)));
        }

        [TestMethod]
        public void TestCscfg()
        {
            var generator = new AzureConfigGenerator();
            string output = generator.GenerateCscfg(typeof(MyServiceProdEnvironment));
            Console.WriteLine(output);
            var expected = File.ReadAllText("CscfgExpectedOutput.xml");
            Assert.IsTrue(XDocument.DeepEquals(XDocument.Parse(expected), XDocument.Parse(output)));
        }
    }
}
