using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Krista.FM.Common.Xml
{
    /// <summary>
    /// ��������������� ������� ��� ������ � XMLDOM.
    /// </summary>
    public struct XmlHelper
    {
        /// <summary>
        /// ��������� � �������� xmlRootNode �������� �������
        /// </summary>
        /// <param name="xmlRootNode">������� � �������� ����� �������� �������� �������</param>
        /// <param name="tagName">��� ��������� ��������</param>
        /// <param name="value">�������� ����</param>
        /// <param name="attributes">
        /// ������ ���������� ������������ ��������.
        /// ������ ������� ������� �������� �������� �� ���� ��������� ���������.
        /// ������ - ��� ��������, ������ - �������� ��������.
        /// </param>
        public static XmlNode AddChildNode(
            XmlNode xmlRootNode, string tagName, string value, params string[][] attributes)
        {
            XmlNode xmlNode = AddChildNode(xmlRootNode, tagName, attributes);
            xmlNode.InnerText = value;
            return xmlNode;
        }

        /// <summary>
        /// ��������� � �������� xmlRootNode �������� �������
        /// </summary>
        /// <param name="xmlRootNode">������� � �������� ����� �������� �������� �������</param>
        /// <param name="tagName">��� ��������� ��������</param>
        /// <param name="attributes">
        /// ������ ���������� ������������ ��������.
        /// ������ ������� ������� �������� �������� �� ���� ��������� ���������.
        /// ������ - ��� ��������, ������ - �������� ��������.
        /// </param>
        public static XmlNode AddChildNode(XmlNode xmlRootNode, string tagName, params string[][] attributes)
        {
            XmlNode xmlNode = xmlRootNode.OwnerDocument.CreateElement(tagName);
            if (attributes != null)
            {
                foreach (string[] attribute in attributes)
                {
                    XmlAttribute xmlAttribute = xmlRootNode.OwnerDocument.CreateAttribute(attribute[0]);
                    xmlAttribute.Value = attribute[1];
                    xmlNode.Attributes.Append(xmlAttribute);
                }

            }
            xmlRootNode.AppendChild(xmlNode);
            return xmlNode;
        }

        /// <summary>
        /// �������� �������� ���� �� ������� �����
        /// </summary>
        /// <param name="xmlRootNode"></param>
        /// <param name="tagName"></param>
        /// <param name="strList"></param>
        /// <returns></returns>
        public static XmlNode AddStringListNode(XmlNode xmlRootNode, string tagName, List<string> strList)
        {
            XmlNode xmlNode = xmlRootNode.OwnerDocument.CreateElement(tagName);
            foreach (string str in strList)
            {
                XmlNode strElemNode = xmlRootNode.OwnerDocument.CreateElement("string");
                strElemNode.InnerText = str;
                xmlNode.AppendChild(strElemNode);
            }
            xmlRootNode.AppendChild(xmlNode);
            return xmlNode;
        }

        /// <summary>
        /// �������� ������ ����� �� ������ �������� ����� xmlRootNode
        /// </summary>
        /// <param name="xmlRootNode"></param>
        /// <returns></returns>
        public static List<string> GetStringListFromXmlNode(XmlNode xmlRootNode)
        {
            List<string> result = new List<string>();

            if (xmlRootNode == null) return result;

            foreach (XmlNode xmlNode in xmlRootNode.ChildNodes) result.Add(xmlNode.InnerText);
            return result;
        }

        /// <summary>
        /// �������� � ���� ������ CDATA
        /// </summary>
        /// <param name="xmlParentNode">������������ ����</param>
        /// <param name="value">�������� CDATA</param>
        public static void AppendCDataSection(XmlNode xmlParentNode, string value)
        {
            XmlCDataSection cdata = xmlParentNode.OwnerDocument.CreateCDataSection(value);
            xmlParentNode.AppendChild(cdata);
        }

        /// <summary>
        /// ��������� XML �������� �� ����� � XmlDocument
        /// </summary>
        /// <param name="fileName">��� ������������ �����</param>
        /// <returns>���������� ����������� XmlDocument</returns>
        /// <remarks></remarks>
        public static XmlDocument Load(string fileName)
        {
            try
            {
                /*StringBuilder sb = new StringBuilder();
                using (StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default))
                {
                    while (sr.Peek() >= 0)
                    {
                        sb.Append(sr.ReadLine());
                    }
                }
                XmlDocument doc = Validator.LoadDocument(sb.ToString());*/
                XmlDocument doc = Validator.LoadDocument(File.ReadAllText(fileName, Encoding.Default));
                return doc;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("���������� ��������� ���� {0}:{1}", fileName, e.Message));
            }
        }

        /// <summary>
        /// ���������� XML - �������� � ����
        /// </summary>
        /// <param name="doc">Xml - ��������, ������� ���������</param>
        /// <param name="fileName">Xml - ��������, ������� ���������</param>

        public static void Save(XmlDocument doc, string fileName)
        {
            try
            {
                if (doc.DocumentElement.HasAttribute("xmlns"))
                {
                    doc.DocumentElement.RemoveAttribute("xmlns");
                }
                XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.GetEncoding(1251));
                writer.IndentChar = '\t';
                writer.Indentation = 1;
                writer.Formatting = Formatting.Indented;
                doc.Save(writer);
                writer.Close();
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("���������� ��������� ���� {0}:{1}", fileName, e.Message));
            }
        }

        /// <summary>
        /// ���������� ������ ���������� XML ����������
        /// </summary>
        /// <param name="doc">��������</param>
        public static void ClearDomDocument(ref XmlDocument doc)
        {
            if (doc == null) return;
            doc.LoadXml("<xml></xml>");
            doc = null;
        }

        /// <summary>
        /// ����������� ������ � ������ XML.
        /// </summary>
        /// <param name="obj">������������� ������.</param>
        /// <param name="nameSpace">������������ ���� ��� ������������.</param>
        public static string Obj2XmlStr(object obj, string nameSpace)
        {
            if (obj == null) return String.Empty;

            XmlSerializer sr = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter w = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture);
            sr.Serialize(
                w, obj, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName("", nameSpace) }));

            return sb.ToString();
        }

        /// <summary>
        /// ����������� ������ � ������ XML.
        /// </summary>
        public static string Obj2XmlStr(object obj)
        {
            if (obj == null) return String.Empty;

            XmlSerializer sr = new XmlSerializer(obj.GetType());
            StringBuilder sb = new StringBuilder();
            StringWriter w = new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture);
            sr.Serialize(
                w, obj, new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(String.Empty) }));

            return sb.ToString();
        }



        /// <summary>
        /// ������������� ������ XML � ������ ��������� ����.
        /// </summary>
        /// <typeparam name="T">��� ���������������� �������.</typeparam>
        /// <param name="xml">������ � ���� ������ XML.</param>
        public static T XmlStr2Obj<T>(string xml)
        {
            if (xml == null) return default(T);
            if (xml == String.Empty) return (T)Activator.CreateInstance(typeof(T));

            StringReader reader = new StringReader(xml);
            XmlSerializer sr = new XmlSerializer(typeof(T));

            return (T)sr.Deserialize(reader);
        }

        /// <summary>
        /// ���������� �������� �������� � ������� ���������
        /// </summary>
        /// <param name="xn">���-�������</param>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="defaultValue">�������� �������� �� ��������� (���� �� �� ������)</param>
        /// <returns>�������� ��������</returns>
        public static bool GetBoolAttrValue(XmlNode xn, string attrName, bool defaultValue)
        {
            try
            {
                if (xn == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes[attrName].Value.ToUpper() == "TRUE")
                {
                    return true;
                }
                else if (xn.Attributes[attrName].Value.ToUpper() == "FALSE")
                {
                    return false;
                }

                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// ���������� �������� �������� �� ��������� ���������
        /// </summary>
        /// <param name="xn">���-�������</param>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="defaultValue">�������� �������� �� ��������� (���� �� �� ������)</param>
        /// <returns>�������� ��������</returns>
        public static string GetStringAttrValue(XmlNode xn, string attrName, string defaultValue)
        {
            try
            {
                if (xn == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }

                string value = xn.Attributes[attrName].Value;
                if (value != string.Empty)
                {
                    return value;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        public static string GetDecodedStringAttrValue(XmlNode xn, string attrName, string defaultValue)
        {
            string value = GetStringAttrValue(xn, attrName, defaultValue);
            return XmlDecode(value);
        }

        public static string XmlDecode(string source)
        {
            return
                source.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").Replace("&apos;", "'").Replace(
                    "&quot;", "\"");
        }

        /// <summary>
        /// ���������� �������� �������� � �������� ���������
        /// </summary>
        /// <param name="xn">���-�������</param>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="defaultValue">�������� �������� �� ��������� (���� �� �� ������)</param>
        /// <returns>�������� ��������</returns>
        public static int GetIntAttrValue(XmlNode xn, string attrName, int defaultValue)
        {
            try
            {
                if (xn == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }

                string value = xn.Attributes[attrName].Value;
                if (value != string.Empty)
                {
                    return Convert.ToInt32(value);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// ���������� �������� �������� � �������� ���������
        /// </summary>
        /// <param name="xn">���-�������</param>
        /// <param name="attrName">������������ ��������</param>
        /// <param name="defaultValue">�������� �������� �� ��������� (���� �� �� ������)</param>
        /// <returns>�������� ��������</returns>
        public static float GetFloatAttrValue(XmlNode xn, string attrName, float defaultValue)
        {
            try
            {
                if (xn == null)
                {
                    return defaultValue;
                }
                if (xn.Attributes.GetNamedItem(attrName) == null)
                {
                    return defaultValue;
                }

                string value = xn.Attributes[attrName].Value;
                if (value != string.Empty)
                {
                    return (float)Convert.ToDouble(value);
                }
                else
                {
                    return defaultValue;
                }
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// ������������� �������� ��������, ���� �������� ���, �� ������� ���
        /// </summary>
        /// <param name="node">������� � ������� ��������������� �������</param>
        /// <param name="attributeName">��� ��������</param>
        /// <param name="value">��������</param>
        public static XmlAttribute SetAttribute(XmlNode node, string attributeName, string value)
        {
            XmlAttribute attribute = node.Attributes[attributeName];

            if (attribute == null)
            {
                attribute = node.OwnerDocument.CreateAttribute(attributeName);
                node.Attributes.Append(attribute);
            }

            attribute.Value = value;
            return attribute;
        }

        public static XmlAttribute SetAttribute(XmlNode node, string attributeName, int value)
        {
            return SetAttribute(node, attributeName, value.ToString());
        }

        public static XmlAttribute SetAttribute(XmlNode node, string attributeName, float value)
        {
            return SetAttribute(node, attributeName, value.ToString());
        }
        
        public static XmlAttribute SetAttribute(XmlNode node, string attributeName, bool value)
        {
            return SetAttribute(node, attributeName, value.ToString());
        }

        /// <summary>
        /// ������� ������� �� ���������� ��������
        /// </summary>
        /// <param name="xn">�������</param>
        /// <param name="attrName">��� ��������</param>
        public static void RemoveAttribute(XmlNode xn, string attrName)
        {
            if (xn.Attributes[attrName] != null)
            {
                xn.Attributes.Remove(xn.Attributes[attrName]);
            }
        }
    }


	/// <summary>
	/// ������� ��� �������� XML �� ����� XSD.
	/// </summary>
	public struct Validator 
	{
		// ��������� �� �������
		private static String validationMessage = String.Empty;
        private static XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

		// ��������� ��� ������ � �������� ��������
		private static void ValidationFullCallBack(object sender, ValidationEventArgs e) 
		{
			if(validationMessage != String.Empty)
				validationMessage += Environment.NewLine + e.Message;
			else
				validationMessage = e.Message;
		}

		// ��������� ������� �������� �� ������ ������
		private static void ValidationCallBack(object sender, ValidationEventArgs e) 
		{
			throw (new Exception(e.Message));
		}

        /// <summary>
        /// ���������� XML �������� ����������� �� document
        /// </summary>
        /// <param name="document">������</param>
        /// <returns>XML-��������</returns>
        public static XmlDocument LoadDocument(string document)
        {
            string errMsg;

            XmlDocument doc = new XmlDocument();
            // ��������� XML ��������
            doc.LoadXml(document);
            // ��������� ������� ������������ ����. ��� ����������� ���� �������� ����������!
            XmlAttribute attr = doc.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
            attr.Value = "xmluml";
            
            if (!Validator.Validate(doc.InnerXml, "ServerConfiguration.xsd", "xmluml", out errMsg))
            {
                throw new Exception(errMsg);
            }
            
            return doc;
        }

        /// <summary>
		/// ��������� XML �������� � ��������� �� ������������ ����� XSD
		/// </summary>
		/// <param name="url">���� � XML ���������</param>
		/// <param name="urlSchema">���� � XSD �����</param>
		/// <param name="nameSpace">������������ ������������ ���� XML ���������</param>
		/// <param name="errorMessage">��������� �� ������� � XML ���������</param>
		/// <returns>� ������ ������� �������� ���������� ��������� �� XmlDocument, ����� ���������� null</returns>
		public static XmlDocument LoadValidated(string url, string urlSchema, string nameSpace, out string errorMessage)
		{
			errorMessage = String.Empty;
			XmlDocument doc = new XmlDocument();
			try
			{
				// ��������� XML ��������
				doc.Load(url);

				// ��������� ������� ������������ ����. ��� ����������� ���� �������� ����������!
				XmlAttribute attr = doc.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
				attr.Value = nameSpace;

				// ��������� �������� �� ����������
                if (!Validate(doc.InnerXml, urlSchema, nameSpace, out errorMessage, true))
					return null;
			} 
			catch (Exception e)
			{
				errorMessage = e.ToString();
				return null;
			}
			return doc;
		}

		/// <summary>
		/// �������� �� ���������� XML ��������������� � ���� ������
		/// </summary>
		/// <param name="xmlFragment">XML � ���� ������</param>
		/// <param name="urlSchema">XSD �����</param>
        /// <param name="targetNamespace">������������ ����</param>
        /// <param name="errorMessage">��������� �� ������� � XML ���������</param>
		/// <returns>���������� true, ���� XML-�� ���������, ����� ���������� false</returns>
        public static bool Validate(string xmlFragment, string urlSchema, string targetNamespace, out string errorMessage)
		{
			return Validate(xmlFragment, urlSchema, targetNamespace, out errorMessage, false);
		}

		/// <summary>
		/// �������� �� ���������� XML ��������������� � ���� ������
		/// </summary>
		/// <param name="xmlFragment">XML � ���� ������</param>
		/// <param name="urlSchema">XSD �����</param>
        /// <param name="targetNamespace">������������ ����</param>
        /// <param name="errorMessage">��������� �� ������� � XML ���������</param>
		/// <param name="fullMessage">true - ���������� ������ ������ ��������������, false - ���������� ������ ������ �������������� XML-�� �����</param>
		/// <returns>���������� true, ���� XML-�� ���������, ����� ���������� false</returns>
		public static bool Validate(string xmlFragment, string urlSchema, string targetNamespace, out string errorMessage, bool fullMessage)
		{
			XmlParserContext xmlParserContext = new XmlParserContext(null, null, null, XmlSpace.None);
			XmlTextReader xmlTextReader = new XmlTextReader(xmlFragment, System.Xml.XmlNodeType.Document, xmlParserContext);
			return Validate(xmlTextReader, urlSchema, targetNamespace, out errorMessage, fullMessage);
		}

		/// <summary>
		/// �������� �� ���������� XML ���������
		/// </summary>
		/// <param name="xmlTextReader">������ XML ���������</param>
		/// <param name="urlSchema">XSD �����</param>
        /// <param name="targetNamespace">������������ ����</param>
        /// <param name="errorMessage">��������� �� ������� � XML ���������</param>
		/// <returns>���������� true, ���� XML-�� ���������, ����� ���������� false</returns>
		public static bool Validate(XmlTextReader xmlTextReader, string urlSchema, string targetNamespace, out string errorMessage)
		{
			return Validate(xmlTextReader, urlSchema, targetNamespace, out errorMessage, false);
		}

        private static XmlSchemaCollection schemaCollection = new XmlSchemaCollection();

        private static void CheckSchema(string urlSchema, ref string errorMessage)
        {
            // ���� ����� ��� ��������� - ������ �� ������
            if (schemaCollection.Count > 0)
                return;
            // ���� ��� - �������� ���������
            schemaCollection.Add(null, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + urlSchema);
            // ���� �� ���������� - ��������� ��������� �� ������
            if (schemaCollection.Count == 0)
                errorMessage = String.Format("�� ������� ��������� ����� {0}:\n{1}", urlSchema, validationMessage);
        }

        /// <summary>
		/// �������� �� ���������� XML ���������
		/// </summary>
		/// <param name="xmlTextReader">������ XML ���������</param>
		/// <param name="urlSchema">XSD �����</param>
        /// <param name="targetNamespace">������������ ����</param>
        /// <param name="errorMessage">��������� �� ������� � XML ���������</param>
		/// <param name="fullMessage">true - ���������� ������ ������ ��������������, false - ���������� ������ ������ �������������� XML-�� �����</param>
		/// <returns>���������� true, ���� XML-�� ���������, ����� ���������� false</returns>
		public static bool Validate(XmlTextReader xmlTextReader, string urlSchema, string targetNamespace, out string errorMessage, bool fullMessage)
		{
            errorMessage = String.Empty;

            // ��������� ������������� �����
            CheckSchema(urlSchema, ref errorMessage);
            
            if (!String.IsNullOrEmpty(errorMessage))
                return false;

            // ������� ������� ��� ������ ��������� ������
            ValidationEventHandler currentValidationDelegate =
                fullMessage ? new ValidationEventHandler(ValidationFullCallBack) : new ValidationEventHandler(ValidationCallBack);

            // ������� ��� � �����
            schemaCollection.ValidationEventHandler += currentValidationDelegate;

            XmlValidatingReader rdr = new XmlValidatingReader(xmlTextReader);
            rdr.ValidationType = ValidationType.Schema;
            rdr.Schemas.Add(schemaCollection);

            if (fullMessage)
                rdr.ValidationEventHandler += new ValidationEventHandler(ValidationFullCallBack);
            else
                rdr.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            try
            {
                while (rdr.Read()) ;
            }
            catch (Exception e)
            {
                if (validationMessage == String.Empty)
                {
                    errorMessage = e.Message;
                }
                else
                {
                    errorMessage = String.Format("{0}{1}{2}", validationMessage, Environment.NewLine, e.Message);
                }
                return false;
            }
            finally
            {
                rdr.Close();
                // ��������� ������� �� �����
                schemaCollection.ValidationEventHandler -= currentValidationDelegate;
            }

            if (validationMessage == String.Empty)
                return true;
            else
            {
                errorMessage = validationMessage;
                return false;
            }
/*
			errorMessage = String.Empty;

            if (targetNamespace == String.Empty)
                targetNamespace = urlSchema;

            if (!xmlSchemaSet.Contains(targetNamespace))
            {

//                XmlSchema schema = new XmlSchema();
//                schema.TargetNamespace = targetNamespace;
//                schema.SourceUri = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + urlSchema;

                if (fullMessage)
                    xmlSchemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationFullCallBack);
                else
                    xmlSchemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

                xmlSchemaSet.Add(targetNamespace, AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + urlSchema);

                if (xmlSchemaSet.Count < 1)
                {
                    errorMessage = String.Format("�� ������� ��������� ����� {0}:\n{1}", urlSchema, validationMessage);
                    return false;
                }
            }

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.Schemas = xmlSchemaSet;
            if (fullMessage)
                xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(ValidationFullCallBack);
            else
                xmlReaderSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
            xmlReaderSettings.ValidationFlags -= XmlSchemaValidationFlags.AllowXmlAttributes;
            //XmlReader xmlReader = XmlReader.Create(xmlTextReader, xmlReaderSettings);
            XmlReader xmlReader = XmlReader.Create(@"N:\Repository\RepositorySettings.xml", xmlReaderSettings);

            try
            {
                while (xmlReader.Read()) ;
            }
            catch (Exception e)
            {
                if (validationMessage == String.Empty)
                {
                    errorMessage = e.Message;
                }
                else
                {
                    errorMessage = String.Format("{0}{1}{2}", validationMessage, Environment.NewLine, e.Message);
                }
                return false;
            }
            if (validationMessage == String.Empty)
                return true;
            else
            {
                errorMessage = validationMessage;
                return false;
            }
 * */
		}
	}
}