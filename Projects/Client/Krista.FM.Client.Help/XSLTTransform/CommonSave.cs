using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Krista.FM.Common.Exceptions;
using System.IO;
using System.Text.RegularExpressions;

namespace Krista.FM.Client.Help
{
    public abstract class CommonSave
    {
        protected HelpManager manager;

        public CommonSave(HelpManager manager)
        {
            this.manager = manager;
        }

        protected XslCompiledTransform xslt;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeBase"></param>
        public abstract void SaveToHtml(XmlNode nodeBase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeBase"></param>
        /// <returns></returns>
        public virtual XmlDocument GetXMLDocument(XmlNode nodeBase)
        {
            return new XmlDocument();
        }

        protected virtual void CreateTransform()
        {
            if(xslt == null)
                xslt = new XslCompiledTransform();
        }

        /// <summary>
        /// Имя схемы, которой будем трансформировать, зависит от режима генерации справки
        /// </summary>
        /// <returns></returns>
        protected virtual string SchemeName(HelpMode mode)
        {
            return String.Empty;
        }

        /// <summary>
        /// Операция трансформации
        /// </summary>
        /// <param name="argumentList"></param>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        /// <exception cref="Krista.FM.Common.Exceptions.HelpException"></exception>
        protected void Transform(XsltArgumentList argumentList, XmlReader reader, XmlWriter writer)
        {
            try
            {
                if (xslt != null)
                {
                    xslt.Transform(reader, argumentList, writer);
                }
            }
            catch (System.Net.WebException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XmlException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (UriFormatException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (ArgumentNullException ex)
            {
                throw new HelpException(ex.ToString());
            }
            catch (XsltException ex)
            {
                throw new HelpException(ex.ToString());
            }
        }

       
        protected void Transform(XmlReader reader, XmlWriter writer)
        {
            Transform(null, reader, writer);
        }

        /// <summary>
        /// Конвертация из rtf формата в html
        /// </summary>
        /// <param name="doc"></param>
        protected virtual void ConvertCDATASection(XmlDocument doc)
        {
        }

        protected void ConvertCDATANode(XmlNode node, ConvertCDATASection converter)
        {
            string convertString = node.InnerText;
            node.InnerText = string.Empty;
            Krista.FM.Common.Xml.XmlHelper.AppendCDataSection(node, converter.Execute(convertString));
        }

        static public void ReplaceInFile(string filePath, string[] searchText, string[] replaceText)
        {
            if (searchText.Length != replaceText.Length)
                return;

            StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding(1251));
            string content = reader.ReadToEnd();
            reader.Close();

            for (int i = 0; i < searchText.Length; i++)
            {
                content = Regex.Replace(content, searchText[i], replaceText[i]);
            }

            StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding(1251));
            writer.Write(content);
            writer.Close();
        }
    }
}
