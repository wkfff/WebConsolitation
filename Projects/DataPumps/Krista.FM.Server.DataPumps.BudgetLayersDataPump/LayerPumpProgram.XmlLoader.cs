using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Text;

using Krista.FM.Common.Xml;

namespace Krista.FM.Server.DataPumps.BudgetLayersDataPump
{
    #region ��������������� ����� ��� �������� XML
    /// <summary>
    /// ��������������� ����� ��� �������� XML � ���������� �� �����. 
    /// ����������� �� Krista.FM.Common �������� � ����� ����� � ������ 
    /// ����� �������� ��������
    /// </summary>
    public struct XmlLoader
    {
        // ���������� ������
        private static StringBuilder errors;

        /// <summary>
        /// ����� ��������� ������ ��� ����������� ������ �������� �� �����
        /// </summary>
        /// <param name="sender">����������� ������</param>
        /// <param name="e">��������� �������</param>
        private static void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            if (errors == null)
                return;
            // �������� ���������� � ��������� � �����
            errors.AppendFormat("Line: {0,4}\tPos: {1,4}\t{2}: {3}",
                e.Exception.LineNumber, e.Exception.LinePosition,
                e.Severity.ToString(), e.Message + Environment.NewLine);
        }

        /// <summary>
        /// �������� ������� �����
        /// </summary>
        /// <param name="filePath">������ ��� �����</param>
        /// <param name="error">��������� �� ������</param>
        /// <returns>��������� ������</returns>
        private static bool CheckFile(string filePath, out string error)
        {
            error = String.Empty;
            if (!File.Exists(filePath))
            {
                error = String.Format("���� '{0}' �� ������", filePath);
            }
            return String.IsNullOrEmpty(error);
        }

        /// <summary>
        /// ��������� XML ������� � ��������� ��� �� ������������ �����
        /// </summary>
        /// <param name="schemaPath">���� � �����</param>
        /// <param name="xmlPath">���� � ���������</param>
        /// <param name="xmlData">DomDocument � ������� ��������� (���� �������� ���������� � ������� ��������)</param>
        /// <param name="message">��������� �� ������� (���� �������� ����������� ��������)</param>
        /// <returns>true - �������� ���������� � ������� ��������,  false - �������� ����������� ��� �������������� �����</returns>
        public static bool LoadXmlValidated(string schemaPath, string xmlPath, out XmlDocument xmlData, out string message)
        {
            xmlData = null;
            // ��������� ������� ������
            if ((!CheckFile(schemaPath, out message)) ||
                (!CheckFile(xmlPath, out message)))
                return false;
            // ��������� �����
            XmlSchema sch = null;
            using (FileStream fs = new FileStream(schemaPath, FileMode.Open, FileAccess.Read))
            {
                sch = XmlSchema.Read(fs, null);
            }
            // ������� � ��������� XML ��������
            xmlData = new XmlDocument();
            try
            {
                xmlData.Load(xmlPath);
            }
            catch (XmlException exp)
            {
                message = String.Format("���������� ��������� XML ��������: {0}", exp.Message);
                return false;
            }
            // �� ������ ������ �������������� namespace ����� �� ��� � � �����
            XmlAttribute attr = xmlData.DocumentElement.SetAttributeNode("xmlns", "http://www.w3.org/2000/xmlns/");
            attr.Value = sch.TargetNamespace;
            // ������� ���������� ������
            errors = new StringBuilder();
            // ������� StrinReader ��� ������ XML
            using (StringReader sr = new StringReader(xmlData.InnerXml))
            {
                // ������� XmlReader � �����������, ������������ ��� ���������
                XmlReaderSettings stt = new XmlReaderSettings();
                stt.CheckCharacters = false;
                stt.Schemas.Add(sch);
                stt.ValidationType = ValidationType.Schema;
                stt.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);
                // ������ � ��������� ��� ������ XML
                using (XmlReader rdr = XmlReader.Create(sr, stt))
                {
                    while (rdr.Read()) { }
                }
            }
            // ��������� ��������� �� ������
            message = errors.ToString();
            errors = null;
            // ���� ��� �� ������ - ����������� XML �������� � ���������� false
            if (!String.IsNullOrEmpty(message))
            {
                XmlHelper.ClearDomDocument(ref xmlData);
                return false;
            }
            // ����� - true
            return true;
        }

    }
    #endregion

}