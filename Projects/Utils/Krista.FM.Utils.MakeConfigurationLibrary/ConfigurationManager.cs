using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Krista.FM.Utils.MakeConfigurationLibrary
{
    public class ConfigurationManager
    {
        private readonly EtalonConfiguration mc;

        public ConfigurationManager(string path)
        {
            mc = new EtalonConfiguration(path);
        }

        /// <summary>
        /// Метод используется как для консольного, так и для GUI приложения.
        /// Получает коллекцию пакетов по описанию в папке OLAPPackages
        /// </summary>
        /// <returns></returns>
        public PackageConfig GetRootPackage()
        {
            var rootPackage = new PackageConfig("Корневой пакет");

            mc.CreateEtalonConfiguration(rootPackage);

            return rootPackage;
        }

        public bool MakeConfiguration(string path, PackageConfig rootPackage)
        {
            try
            {
                var configurationDocument = new XDocument();

                InitializeDocument(configurationDocument);
                
                HandlePackage(rootPackage, configurationDocument);
                
                configurationDocument.Save(string.Format("{0}OlapConfiguration.xml", path));

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static void HandlePackage(PackageConfig rootPackage, XNode configurationDocument)
        {
            XNode node = GetNode(configurationDocument);

            foreach (var package in rootPackage.Packages)
            {
                if (package.IsContain)
                {
                    var packageNode = new XElement("Package");

                    XElement rootElement = packageNode;

                    if (package.Packages.Count != 0)
                    {
                        XElement packeges = new XElement("Packages");
                        packageNode.Add(packeges);
                        rootElement = packeges;
                    }

                    HandlePackage(package, rootElement);

                    XElement items = new XElement("Items");

                    foreach (var item in package.Items)
                    {
                        if (item.IsContain)
                        {
                            XElement itemNode = new XElement("Item");
                            itemNode.SetAttributeValue("name", string.Format("{0}", item.Name));
                            items.Add(itemNode);
                        }
                    }

                    packageNode.Add(items);

                    packageNode.SetAttributeValue("name", package.Name);
                    ((XElement)node).Add(packageNode);
                }
            }


        }

        private static XNode GetNode(XNode configurationDocument)
        {
            XNode node = null;

            switch (configurationDocument.NodeType)
            {
                case XmlNodeType.Document:
                    node = ((XDocument)configurationDocument).Descendants("Packages").First();
                    break;
                case XmlNodeType.Element:
                    node = configurationDocument;
                    break;

            }

            return node;
        }

        private static void InitializeDocument(XDocument configurationDocument)
        {
            configurationDocument.Declaration = new XDeclaration("1.0", "windows-1251", null);
            XElement configuration = new XElement("Configuration");
            configuration.Add(new[] { new XElement("Packages") });
            configurationDocument.Add(configuration);
        }
    }
}
