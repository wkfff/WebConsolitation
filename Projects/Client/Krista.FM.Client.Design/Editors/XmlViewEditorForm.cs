using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.Devices;
using System.Windows.Forms;
using System.Xml;

namespace Krista.FM.Client.Design.Editors
{
    public partial class XmlViewEditorForm : Form
    {
        private string xmlString;

        public XmlViewEditorForm()
        {
            InitializeComponent();
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }

        public XmlViewEditorForm(string xmlString)
        {
            this.xmlString = xmlString;
            InitializeComponent();
            Krista.FM.Client.Common.DefaultFormState.Load(this);
        }
       
        private void XmlViewEditorForm_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.xmlString))
            {
                Computer computer = new Computer();
                string com = computer.FileSystem.SpecialDirectories.Temp;
                Guid g = new Guid();
                FileInfo fi = new FileInfo(com + "//" + g + ".xml");
                //fs можно использовать при редактировании
                FileStream fs = fi.Create();
                fs.Close();


                XmlDocument doc = new XmlDocument(); doc.LoadXml(this.xmlString);

                Krista.FM.Common.Xml.XmlHelper.Save(doc, fi.FullName);
                xmlBrowser.Navigate(new Uri(fi.FullName));
            }
            
        }

        private void xmlBrowser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void XmlViewEditorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Krista.FM.Client.Common.DefaultFormState.Save(this);
        }
    }
}