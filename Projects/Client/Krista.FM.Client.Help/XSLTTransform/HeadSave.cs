using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Krista.FM.Common;
using Krista.FM.Common.Exceptions;
using System.Xml.XPath;

namespace Krista.FM.Client.Help
{
    /// <summary>
    /// Создание XML-описания стартовой страницы 
    /// </summary>
    public class HeadSave : CommonSave
    {
        public HeadSave(HelpManager manager)
            :base(manager)
        {
            CreateTransform();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        public override void SaveToHtml(System.Xml.XmlNode nodeBase)
        {
            try
            {
                XmlDocument doc = GetXMLDocument();

                XmlReader reader = XmlReader.Create(new StringReader(doc.InnerXml));

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Auto;
                XmlWriter writer =
                    XmlWriter.Create(
                        String.Format(
                        @"{0}\{1}\index.html", AppDomain.CurrentDomain.BaseDirectory, HelpManager.result), settings);

                Transform(reader, writer);

                writer.Close();
                reader.Close();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        private XmlDocument GetXMLDocument()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.InnerXml =
                    String.Format("<?xml version = \"1.0\" encoding = \"windows-1251\"?><ServerConfiguration/>");
                XmlNode captionNode = doc.CreateElement("Caption");
                captionNode.InnerText = HelpManager.HelpHeaderName(manager.Mode);
                doc.DocumentElement.AppendChild(captionNode);

                XmlNode CompanyNameNode = doc.CreateElement("CompanyName");
                CompanyNameNode.InnerText = "Анализ и планирование";
                doc.DocumentElement.AppendChild(CompanyNameNode);

                XmlNode mainVersionNode = doc.CreateElement("MainVersion");
                mainVersionNode.InnerText =
                    String.Format("Базовая версия сервера: {0}",
                                  AppVersionControl.GetAssemblyBaseVersion(
                                      SchemeEditor.SchemeEditor.Instance.Scheme.UsersManager.ServerLibraryVersion()));
                doc.DocumentElement.AppendChild(mainVersionNode);

                XmlNode dbVersionNode = doc.CreateElement("DBVersion");
                dbVersionNode.InnerText = String.Format("Версия базы данных: {0}", SchemeEditor.SchemeEditor.Instance.Scheme.SchemeDWH.DatabaseVersion);
                doc.DocumentElement.AppendChild(dbVersionNode);

                XmlNode productNameNode = doc.CreateElement("ProductName");
                productNameNode.InnerText = "(с) НПО Криста 2006-2009";
                doc.DocumentElement.AppendChild(productNameNode);

                XmlNode coordinateNode = doc.CreateElement("Coordinate");
                coordinateNode.InnerText = String.Format("{0}", SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SupportServiceInfo"));
                doc.DocumentElement.AppendChild(coordinateNode);
                return doc;
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


        protected override void CreateTransform()
        {
            base.CreateTransform();
            xslt.Load(String.Format(@"{0}\{1}.xsl", HelpManager.profile, "Head"));
        } 
    }
}
