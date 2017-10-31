using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Krista.FM.Utils.Common
{
    /// <summary>
    /// Операция по разбитию FMMD_All
    /// </summary>
    public class SplitOperation : BaseOperation
    {
        private XDocument _configurationDocument;

        private readonly string sourcePackage;
        private readonly string destPackage;

        public SplitOperation(string sourcePackage, string destPackage)
        {
            this.sourcePackage = SplitHelperClass.CheckPath(sourcePackage);
            this.destPackage = SplitHelperClass.CheckPath(destPackage);
        }

        public XDocument OpenDocuent(string uri)
        {
            try
            {
                return XDocument.Load(uri);
            }
            catch (ArgumentNullException e)
            {
                throw new OpenFMMDAllExeption("Не задан путь к FMMD_All.xml : " + e.Message);
            }
            catch(FileNotFoundException e)
            {
                throw new OpenFMMDAllExeption("Не найден FMMD_All.xml : " + e.Message);
            }
        }

        public void CreateFolders()
        {
            try
            {
                Directory.CreateDirectory(string.Format("{0}\\DatabaseDimensions", destPackage));
                Directory.CreateDirectory(string.Format("{0}\\Cubes", destPackage));
            }
            catch (IOException e)
            {
                throw new CreateFoldersException(e.Message);
            }
            catch (ArgumentException e)
            {
                throw new CreateFoldersException(e.Message);
            }
        }

        public void AnalisysNode(XNode node)
        {
           switch (node.NodeType)
            {
                case XmlNodeType.Document:
                    HandleDocumentType(node);
                    break;
                case XmlNodeType.Element:
                    HandleElementType(node);
                    break;
            }
        }

        private void HandleElementType(XNode node)
        {
            try
            {
                var element = (XElement)node;
                if (!IsSaveObject(element.Name.LocalName))
                {
                    foreach (XNode n in element.Nodes())
                    {
                        AnalisysNode(n);
                    }

                    return;
                }

                RemoveRevisionElement(element);

                SplitHelperClass.Validate(element, false);
                Console.WriteLine("SaveElement: {0} - {1}", element.Name, element.Attribute("name").Value);
                SaveObject(element);  
                SetConfigurationElement(element);
            }
            catch (Exception e)
            {
                throw new SplitException(string.Format("{0}: {1}", e.Message, ((XElement)node).Name));
            }
        }

        private static void RemoveRevisionElement(XElement element)
        {
            if (element.Descendants("CustomProperties").First().Descendants("Property")
                    .Where(el => el.Attribute("name").Value == "Revision").Count() != 0)
            {
                element.Descendants("CustomProperties").First().Descendants("Property")
                    .Where(el => el.Attribute("name").Value == "Revision").Remove();  
            }
        }

        private void HandleDocumentType(XNode node)
        {
            try
            {
                // конфигурационный xml-документ
                _configurationDocument = new XDocument();
                var document = (XDocument)node;
                Console.WriteLine("StartDocument");
                XDeclaration declaration = document.Declaration;
                if (declaration != null)
                {
                    _configurationDocument.Declaration = new XDeclaration(declaration.Version, declaration.Encoding,
                                                                          declaration.Standalone);
                    Console.WriteLine("XmlDeclaration: {0} {1} {2}", declaration.Version, declaration.Encoding,
                                      declaration.Standalone);
                }
                   
                var configuration = new XElement("Configuration");
                configuration.Add(new[] { new XElement("DatabaseDimensions"), new XElement("Cubes") });
                _configurationDocument.Add(configuration);

                foreach (XNode n in document.Nodes())
                {
                    AnalisysNode(n);
                }
                Console.WriteLine("EndDocument");
            }
            catch (Exception e)
            {
                throw new SplitException(e.Message);
            }
        }

        /// <summary>
        /// Добавляем сохраняемый элемент в конфигурацию, для последующего восстаовления в FMFM_All
        /// </summary>
        /// <param name="element"></param>
        private void SetConfigurationElement(XElement element)
        {
            var configurationElement = new XElement(element.Name);
            configurationElement.SetAttributeValue("name", element.Attribute("name").Value);

            if (element.Name == "Cube")
                (from el in _configurationDocument.Descendants("Cubes")
                 select el).First().Add(configurationElement);
        }

        /// <summary>
        ///  Сохранение измерений и кубов
        /// </summary>
        /// <param name="element"></param>
        private void SaveObject(XElement element)
        {
            if (element != null)
            {
                string folderName = element.Name == "Cube" ? "Cubes" : "DatabaseDimensions";
                element.Save(string.Format("{0}\\{1}\\{2}.xml", destPackage, folderName, element.Attribute("name").Value));
            }
        }

        /// <summary>
        /// Является ли узел требуемым сохранения в файл
        /// </summary>
        /// <param name="objectTypeName"></param>
        /// <returns></returns>
        private static bool IsSaveObject(string objectTypeName)
        {
            if (string.Equals("Cube", objectTypeName) || string.Equals("DatabaseDimension", objectTypeName))
                return true;

            return false;
        }

        public override bool Execute()
        {
            try
            {
                XDocument document = OpenDocuent(string.Format("{0}\\FMMD_All.xml", sourcePackage));
                CreateFolders();
                AnalisysNode(document);
            }
            catch (OpenFMMDAllExeption e)
            {
                Trace.TraceError(String.Format("Ошибка при открытии FMMD_All.xml - {0}", e.Message));
                return false;
            }
            catch (CreateFoldersException e)
            {
                Trace.TraceError(String.Format("Ошибка создания подпапок - {0}", e.Message));
                return false;
            }
            catch(SplitException e)
            {
                Trace.TraceError(String.Format("При разборе FMMD_All возникла ошибка - {0}", e.Message));
                return false;
            }

            Trace.TraceSuccess("FMMD_All разбит успешно!");
            return true;
        }
    }
}
