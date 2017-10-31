using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;

namespace Krista.FM.Client.Help
{
    public class ClassSave : CommonSave
    {
        /// <summary>
        /// Имя пакета, которому принадлежит объект
        /// </summary>
        private string packageName = string.Empty;

        public ClassSave(HelpManager manager, string packageName)
            : base(manager)
        {
            this.packageName = packageName;
            CreateTransform();
        }

        /// <summary>
        /// Трансформация класса
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public override void SaveToHtml(System.Xml.XmlNode nodeBase)
        {
            if (nodeBase != null)
            {
                try
                {
                    XmlDocument doc = GetXMLDocument(nodeBase);

                    XmlReader reader = XmlReader.Create(new StringReader(doc.InnerXml));

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.ConformanceLevel = ConformanceLevel.Auto;
                    XmlWriter writer =
                        XmlWriter.Create(
                            String.Format(
                                @"{0}\{1}\classes\{2}.html", AppDomain.CurrentDomain.BaseDirectory, 
                                HelpManager.result,
                                nodeBase.Attributes["objectKey"].InnerText), settings);

                    Transform(reader, writer);

                    writer.Close();
                    reader.Close();

                    ReplaceInFile(String.Format(
                                      @"{0}\{1}\classes\{2}.html", AppDomain.CurrentDomain.BaseDirectory,
                                      HelpManager.result,
                                      nodeBase.Attributes["objectKey"].InnerText),
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

                XmlElement element = document.CreateElement("Class");
                foreach (XmlAttribute attr in nodeBase.Attributes)
                    Krista.FM.Common.Xml.XmlHelper.SetAttribute(element, attr.Name, attr.Value);

                // дополнительный атрибут для определения типа класса
                Krista.FM.Common.Xml.XmlHelper.SetAttribute(element, "classType", nodeBase.Name);

                element.InnerXml = nodeBase.InnerXml;

                document.DocumentElement.AppendChild(element);

                Krista.FM.Common.Xml.XmlHelper.SetAttribute(element, "packageName", packageName);

                ConvertCDATASection(document);

                return document;
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentException ex)
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

            XmlNode node = doc.SelectSingleNode("/ServerConfiguration/Class/DeveloperDescription");

            if (node != null)
            {
                ConvertCDATANode(node, converter);
            }

            // проверка атрибутов
            XmlNodeList list = doc.SelectNodes("/ServerConfiguration/Class/Attributes/Attribute");
            ConvertCDATANodes(converter, list);

            // проверка атрибутов-ссылок
            list = doc.SelectNodes("/ServerConfiguration/Class/Attributes/RefAttribute");
            ConvertCDATANodes(converter, list);

            // проверка ассоциаций
            list = doc.SelectNodes("/ServerConfiguration/Class/AssociationsCls/AssociationCls");
            ConvertCDATANodes(converter, list);

            list = doc.SelectNodes("/ServerConfiguration/Class/AssociatedCls/AssociateCls");
            ConvertCDATANodes(converter, list);
        }

        private void ConvertCDATANodes(ConvertCDATASection converter, XmlNodeList list)
        {
            foreach (XmlNode xmlNode in list)
            {
                XmlNode attrDevDesc = xmlNode.SelectSingleNode("DeveloperDescription");
                if (attrDevDesc != null)
                {
                    ConvertCDATANode(attrDevDesc, converter);
                }
            }
        }

        protected override void CreateTransform()
        {
            base.CreateTransform();

            xslt.Load(String.Format(@"{0}\{1}.xsl", HelpManager.profile, SchemeName(manager.Mode)));
        }

        protected override string SchemeName(HelpMode mode)
        {
            switch (mode)
            {
                case HelpMode.developerMode:
                    {
                        return "Class_Dev";
                    }
                case HelpMode.userMode:
                    {
                        return "Class";
                    }
                case HelpMode.liteMode:
                    {
                        return "Class_lite";
                    }
                default:
                    throw new Exception(String.Format("Необработанный режим генерации справки: {0}", mode.ToString()));
            }
        }
    }
}
