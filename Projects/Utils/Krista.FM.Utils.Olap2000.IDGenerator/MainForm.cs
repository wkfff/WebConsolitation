using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.IO;

namespace Krista.FM.Utils.Olap2000.IDGenerator
{
    public partial class MainForm : Form
    {
        private XmlDocument doc;
        private XPathNavigator navigator;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                StringWriter sw = new StringWriter();
                TextWriterTraceListener textWriterListener = new TextWriterTraceListener(sw);
                Trace.Listeners.Add(textWriterListener);
                try
                {
                    Trace.WriteLine(string.Format("����� ��������� ����� \"{0}\"...", openFileDialog1.FileName));
                    Trace.WriteLine(string.Empty);
                    FileInfo fmmdAllInfo = new FileInfo(openFileDialog1.FileName);
                    bool readOnly = (fmmdAllInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
                    try
                    {                        
                        //C������ ������� "ReadOnly".
                        if (readOnly)
                        {                            
                            Trace.WriteLine("������ �������� \"ReadOnly\"...");
                            fmmdAllInfo.Attributes = fmmdAllInfo.Attributes & ~FileAttributes.ReadOnly;
                        }

                        doc = new XmlDocument();
                        doc.Load(openFileDialog1.FileName);
                        navigator = doc.CreateNavigator();

                        //��������� ������������� �������� "ID" � ���������
                        ProcessObjects("DatabaseDimension", "ID");
                        //��������� ������������� �������� "HierarchyID" � ���������
                        ProcessObjects("DatabaseDimension", "HierarchyID");
                        //��������� ������������� �������� "ID" � �����
                        ProcessObjects("Cube", "ID");
                        //��������� ������������� �������� "MeasureGroupID" � �����
                        ProcessObjects("Cube", "MeasureGroupID");
                        //��������� ������������� �������� "ID" � �������� �����
                        ProcessObjects("Partition", "ID");

                        doc.Save(openFileDialog1.FileName);

                        //������������� ������� "ReadOnly".
                        if (readOnly)
                        {
                            Trace.WriteLine(string.Empty);
                            Trace.WriteLine("������������ �������� \"ReadOnly\"...");
                            fmmdAllInfo.Attributes = fmmdAllInfo.Attributes ^ FileAttributes.ReadOnly;
                        }
                        Trace.WriteLine(string.Empty);
                        Trace.WriteLine(string.Format("��������� ����� \"{0}\" ��������!", openFileDialog1.FileName));
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(this, string.Format(
                            "������ ��� ��������� ����� \"{0}\": {1}", openFileDialog1.FileName, exception.Message));
                    }
                }
                finally
                {
                    Trace.Flush();
                    textBoxLog.Text = sw.ToString();
                    Trace.Listeners.Remove(textWriterListener);
                    textWriterListener.Dispose();
                    sw.Dispose();
                }
            }
        }
        
        /// <summary>
        /// ��������� ����� ��������� � "CustomProperties", ��� ������������� �������� ��� ��� "CustomProperties".
        /// </summary>
        /// <param name="parent">������������ ������</param>        
        private void InsertCustomProperty(XPathNavigator parent, string attributeName)
        {
            XmlWriter property;
            XPathNavigator customPropertiesNavigator = parent.SelectSingleNode("./CustomProperties");
            if (null == customPropertiesNavigator)
            {
                property = parent.AppendChild();
                property.WriteStartElement("CustomProperties");                
            }
            else
            {
                property = customPropertiesNavigator.AppendChild();
            }
            property.WriteStartElement("Property");
            property.WriteAttributeString("name", attributeName);
            property.WriteAttributeString("datatype", "8");
            Guid guid = Guid.NewGuid();
            /*if (attributeName == "AggregateFunction")
            {
                property.WriteCData("Sum");
            }
            else
            {
                property.WriteCData(guid.ToString());
            }*/
            property.WriteCData(guid.ToString());
            property.WriteEndElement();
            if (null == customPropertiesNavigator)
            {
                property.WriteEndElement();
            }
            property.Close();            
            Trace.WriteLine(string.Format("                             {0}", guid.ToString()));            
        }

        /// <summary>
        /// �������� ������ �������� "parentObject", � ������� � "CustomProperties" ����������� ��� "attributeName".
        /// </summary>
        /// <param name="parentObject">��� �������������� �������</param>
        /// <param name="attributeName">��� ������������ ��������</param>
        private void ProcessObjects(string parentObject, string attributeName)
        {
            if (null != navigator)
            {   
                Trace.WriteLine(string.Empty);
                Trace.WriteLine(string.Format(
                    "����� ������������ ��� \"{0}\", ������� \"{1}\"", parentObject, attributeName));
                Trace.Indent();
                
                //�������� ������� � ������������� ����� attributeName                
                XPathNodeIterator objectList = navigator.Select(
                    string.Format("//{0}[not (./CustomProperties/Property/@name = '{1}')]", parentObject, attributeName));
                Trace.WriteLine(string.Format("����� ������� ��������: {0}", objectList.Count));
                
                while (objectList.MoveNext())
                {
                    Trace.Write(string.Format(
                        "����������� \"{0}\"", objectList.Current.GetAttribute("name", string.Empty)));
                    InsertCustomProperty(objectList.Current, attributeName); 
                }
                Trace.Unindent();
                Trace.WriteLine(string.Format(
                    "�������� ������������ ��� \"{0}\", ������� \"{1}\"", parentObject, attributeName));
            }
        }
    }
}