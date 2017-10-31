using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;
using Krista.FM.Common.Xml;


namespace Krista.FM.Client.Help
{
    public class PackageSave : CommonSave
    {
        public PackageSave(HelpManager manager)
            : base(manager)
        {
            CreateTransform();
        }

        /// <summary>
        /// Трансформация пакета
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public override void SaveToHtml(XmlNode nodeBase)
        {
            if (nodeBase != null)
            {
                try
                {
                    XmlDocument document = GetXMLDocument(nodeBase);
                    
                    XmlReader reader = XmlReader.Create(new StringReader(document.InnerXml));

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    XmlWriter writer =
                        XmlWriter.Create(
                            String.Format(@"{0}\{1}\packages\{2}.html",
                            AppDomain.CurrentDomain.BaseDirectory, HelpManager.result,
                            nodeBase.Attributes["name"].InnerText), settings);

                    // параметры трансформации
                    XsltArgumentList argumentList = new XsltArgumentList();

                    argumentList.AddParam("mode", "", manager.Mode.ToString());

                    Transform(argumentList, reader, writer);

                    writer.Close();
                    reader.Close();

                    ReplaceInFile(String.Format(
                                     @"{0}\{1}\packages\{2}.html", AppDomain.CurrentDomain.BaseDirectory,
                                     HelpManager.result,
                                     nodeBase.Attributes["name"].InnerText),
                                 new string[] { "&lt;", "&gt;", "&amp;nbsp;", "&amp;lt;", "&amp;gt;" }, new string[] { "<", ">", " ", " ", " " });
                }
                catch (InvalidOperationException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch (ArgumentNullException ex)
                {
                    throw new HelpException(ex.ToString());
                }
                catch (ArgumentException ex)
                {
                    throw new HelpException(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Получает xml-документ для трансформации
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <returns></returns>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private XmlDocument GetXMLDocument(XmlNode nodeBase)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                document.InnerXml =
                    String.Format("<?xml version = \"1.0\" encoding = \"windows-1251\"?><ServerConfiguration/>");

                XmlNode package = document.CreateElement("Package");
                Krista.FM.Common.Xml.XmlHelper.SetAttribute(package, "name", nodeBase.Attributes["name"].InnerText);

                if (nodeBase.Attributes["privatePath"] != null)
                    Krista.FM.Common.Xml.XmlHelper.SetAttribute(package, "privatePath",
                                                                nodeBase.Attributes["privatePath"].InnerText);

                if (nodeBase.Attributes["description"] != null)
                    Krista.FM.Common.Xml.XmlHelper.SetAttribute(package, "description",
                                                                nodeBase.Attributes["description"].InnerText);

                if (nodeBase.SelectSingleNode("./DeveloperDescription") != null)
                {
                    XmlNode devDescr = XmlHelper.AddChildNode(package, "DeveloperDescription", String.Empty, null);
                    XmlHelper.AppendCDataSection(devDescr, nodeBase.SelectSingleNode("./DeveloperDescription").InnerText);
                }

                XmlNode packages = nodeBase.SelectSingleNode("./Packages");
                if (packages != null)
                {
                    XmlNode packagesNode = document.CreateElement("Packages");
                    XmlNodeList packagesList = packages.SelectNodes("./Package");
                    foreach (XmlNode o in packagesList)
                    {
                        XmlNode p = document.CreateElement("Package");
                        Krista.FM.Common.Xml.XmlHelper.SetAttribute(p, "name", o.Attributes["name"].InnerText);
                        if (o.Attributes["privatePath"] != null)
                            Krista.FM.Common.Xml.XmlHelper.SetAttribute(p, "privatePath",
                                                                        o.Attributes["privatePath"].InnerText);

                        packagesNode.AppendChild(p);
                    }
                    package.AppendChild(packagesNode);
                }

                XmlNode classes = nodeBase.SelectSingleNode("./Classes");
                if (classes != null)
                {
                    XmlNode classesNode = document.CreateElement("Classes");

                    // Сохраняем классы в отдельные файлы
                    SaveClasses(classes, nodeBase.Attributes["name"].InnerText);

                    classesNode.InnerXml = classes.InnerXml;
                    package.AppendChild(classesNode);
                }

                XmlNode associations = nodeBase.SelectSingleNode("./Associations");
                if (associations != null)
                {
                    XmlNode associationsNodes = document.CreateElement("Associations");
                    associationsNodes.InnerXml = associations.InnerXml;
                    package.AppendChild(associationsNodes);
                }

                XmlNode documents = nodeBase.SelectSingleNode("./Documents");
                if (documents != null)
                {
                    XmlNode documentsNodes = document.CreateElement("Documents");

                    SaveDocuments(documents);

                    documentsNodes.InnerXml = documents.InnerXml;
                    package.AppendChild(documentsNodes);
                }

                document.DocumentElement.AppendChild(package);

                ConvertCDATASection(document);

                return document;
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch(ArgumentException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XPathException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

        protected override void ConvertCDATASection(XmlDocument doc)
        {
            ConvertCDATASection converter = new ConvertCDATASection();

            XmlNode node = doc.SelectSingleNode("/ServerConfiguration/Package/DeveloperDescription");

            if (node != null)
            {
                ConvertCDATANode(node, converter);
            }
        }

        private void SaveDocuments(XmlNode documents)
        {
            DocumentSave dSave = new DocumentSave(manager);
            XmlNodeList docs = documents.SelectNodes("./Document");
            foreach (XmlNode o in docs)
                dSave.SaveToHtml(o);
        }

        /// <summary>
        /// Сохранение классов
        /// </summary>
        /// <param name="classes"></param>
        private void SaveClasses(XmlNode classes, string packageName)
        {
            // Сохраняем классы в отдельные HTML
            ClassSave clSave = new ClassSave(manager, packageName);

            XmlNodeList fixedClassesList = classes.SelectNodes("./FixedClsDoc");
            foreach (XmlNode o in fixedClassesList)
                clSave.SaveToHtml(o);

            XmlNodeList bridgeClassesList = classes.SelectNodes("./BridgeClsDoc");
            foreach (XmlNode o in bridgeClassesList)
                clSave.SaveToHtml(o);

            XmlNodeList dataList = classes.SelectNodes("./DataClsDoc");
            foreach (XmlNode o in dataList)
                clSave.SaveToHtml(o);

            XmlNodeList datatableList = classes.SelectNodes("./DataTableDoc");
            foreach (XmlNode o in datatableList)
                clSave.SaveToHtml(o);

            XmlNodeList tableList = classes.SelectNodes("./TableDoc");
            foreach (XmlNode o in tableList)
                clSave.SaveToHtml(o);

            XmlNodeList documentEntityList = classes.SelectNodes("./DocumentEntityDoc");
            foreach (XmlNode o in documentEntityList)
                clSave.SaveToHtml(o);
        }

        protected override void CreateTransform()
        {
            base.CreateTransform();

            xslt.Load(String.Format(@"{0}\Package.xsl", HelpManager.profile));
        }
    }
}
