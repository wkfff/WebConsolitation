using System.Xml;
using System.Xml.XPath;
using System.Windows.Forms;
using System.ComponentModel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Krista.FM.Utils.OLAP.RenameGenerator
{
    partial class frmMain : Form
    {
        // Fields
        private Button btnChangeDestDir;
        private Button btnGenerateLists;
        private Button btnOpenFmmd;
        private IContainer components;
        private FolderBrowserDialog folderBrowserDialogMain;
        private GroupBox groupBoxDestDir;
        private GroupBox groupBoxFmmd;
        private Label lbDestDir;
        private Label lbFmmd;
        private XPathNavigator navigator;
        private OpenFileDialog openFileDialogMain;
        private XmlWriter to2000writer;
        private XmlWriter to2005writer;
        private XmlWriterSettings writerSettings;

        // Methods
        public frmMain()
        {
            this.InitializeComponent();
        }

        private void btnChangeDestDir_Click(object sender, EventArgs e)
        {
            if (this.folderBrowserDialogMain.ShowDialog(this) == DialogResult.OK)
            {
                this.lbDestDir.Text = this.folderBrowserDialogMain.SelectedPath;
            }
        }

        private void btnGenerateLists_Click(object sender, EventArgs e)
        {
            this.GenerateLists();
            MessageBox.Show(this, string.Format("Списки сгенерированы в \"{0}\"", this.lbDestDir.Text), DateTime.Now.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            Process.Start("explorer", string.Format("\"{0}\"", this.lbDestDir.Text));
        }

        private void btnOpenFmmd_Click(object sender, EventArgs e)
        {
            if (this.openFileDialogMain.ShowDialog(this) == DialogResult.OK)
            {
                this.lbFmmd.Text = this.openFileDialogMain.FileName;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void FinalizeWriter(XmlWriter writer)
        {
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private void FinalizeWriters()
        {
            this.FinalizeWriter(this.to2005writer);
            this.FinalizeWriter(this.to2000writer);
        }

        private void GenerateLists()
        {
            this.navigator = new XPathDocument(this.lbFmmd.Text).CreateNavigator();
            this.InitWriters(this.lbDestDir.Text);
            try
            {
                this.RenameDimensions();
            }
            finally
            {
                this.FinalizeWriters();
            }
        }

        private string GenerateNewName(string oldName)
        {
            return string.Format("{0}.{0}", oldName.Replace(".", "__"));
        }

        private void InitializeComponent()
        {
            this.openFileDialogMain = new OpenFileDialog();
            this.groupBoxFmmd = new GroupBox();
            this.btnOpenFmmd = new Button();
            this.lbFmmd = new Label();
            this.groupBoxDestDir = new GroupBox();
            this.lbDestDir = new Label();
            this.btnChangeDestDir = new Button();
            this.folderBrowserDialogMain = new FolderBrowserDialog();
            this.btnGenerateLists = new Button();
            this.groupBoxFmmd.SuspendLayout();
            this.groupBoxDestDir.SuspendLayout();
            base.SuspendLayout();
            this.openFileDialogMain.FileName = "FMMD_All.xml";
            this.openFileDialogMain.Filter = "Файлы xml (*.xml)|*.xml|Все файлы (*.*)|*.*";
            this.openFileDialogMain.InitialDirectory = @"X:\dotNET\Repository\_Подопытная\OLAP";
            this.openFileDialogMain.Title = "Выберите файл \"FMMD_All.xml\"";
            this.groupBoxFmmd.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBoxFmmd.Controls.Add(this.lbFmmd);
            this.groupBoxFmmd.Controls.Add(this.btnOpenFmmd);
            this.groupBoxFmmd.Location = new Point(2, 2);
            this.groupBoxFmmd.Name = "groupBoxFmmd";
            this.groupBoxFmmd.Size = new Size(0x1e8, 0x31);
            this.groupBoxFmmd.TabIndex = 1;
            this.groupBoxFmmd.TabStop = false;
            this.groupBoxFmmd.Text = "Где искать измерения";
            this.btnOpenFmmd.Location = new Point(6, 0x13);
            this.btnOpenFmmd.Name = "btnOpenFmmd";
            this.btnOpenFmmd.Size = new Size(0x4b, 0x17);
            this.btnOpenFmmd.TabIndex = 1;
            this.btnOpenFmmd.Text = "Изменить";
            this.btnOpenFmmd.UseVisualStyleBackColor = true;
            this.btnOpenFmmd.Click += new EventHandler(this.btnOpenFmmd_Click);
            this.lbFmmd.AutoSize = true;
            this.lbFmmd.Location = new Point(0x57, 0x18);
            this.lbFmmd.Name = "lbFmmd";
            this.lbFmmd.Size = new Size(0x125, 13);
            this.lbFmmd.TabIndex = 2;
            this.lbFmmd.Text = @"X:\dotNET\Repository\_Подопытная\OLAP\FMMD_All.xml";
            this.groupBoxDestDir.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBoxDestDir.Controls.Add(this.lbDestDir);
            this.groupBoxDestDir.Controls.Add(this.btnChangeDestDir);
            this.groupBoxDestDir.Location = new Point(2, 0x39);
            this.groupBoxDestDir.Name = "groupBoxDestDir";
            this.groupBoxDestDir.Size = new Size(0x1e8, 0x31);
            this.groupBoxDestDir.TabIndex = 3;
            this.groupBoxDestDir.TabStop = false;
            this.groupBoxDestDir.Text = "Куда записать списки переименований";
            this.lbDestDir.AutoSize = true;
            this.lbDestDir.Location = new Point(0x57, 0x18);
            this.lbDestDir.Name = "lbDestDir";
            this.lbDestDir.Size = new Size(0x160, 13);
            this.lbDestDir.TabIndex = 2;
            this.lbDestDir.Text = @"X:\dotNET\Repository\_Подопытная\OLAP\Списки переименований";
            this.btnChangeDestDir.Location = new Point(6, 0x13);
            this.btnChangeDestDir.Name = "btnChangeDestDir";
            this.btnChangeDestDir.Size = new Size(0x4b, 0x17);
            this.btnChangeDestDir.TabIndex = 1;
            this.btnChangeDestDir.Text = "Изменить";
            this.btnChangeDestDir.UseVisualStyleBackColor = true;
            this.btnChangeDestDir.Click += new EventHandler(this.btnChangeDestDir_Click);
            this.folderBrowserDialogMain.Description = "Выберите каталог для записи списков переименований";
            this.folderBrowserDialogMain.SelectedPath = @"X:\dotNET\Repository\_Подопытная\OLAP";
            this.btnGenerateLists.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.btnGenerateLists.Location = new Point(8, 0x70);
            this.btnGenerateLists.Name = "btnGenerateLists";
            this.btnGenerateLists.Size = new Size(0x1e2, 0x17);
            this.btnGenerateLists.TabIndex = 4;
            this.btnGenerateLists.Text = "Сгенерировать списки";
            this.btnGenerateLists.UseVisualStyleBackColor = true;
            this.btnGenerateLists.Click += new EventHandler(this.btnGenerateLists_Click);
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1ec, 0x8b);
            base.Controls.Add(this.btnGenerateLists);
            base.Controls.Add(this.groupBoxDestDir);
            base.Controls.Add(this.groupBoxFmmd);
            this.MinimumSize = new Size(500, 0xa8);
            base.Name = "frmMain";
            this.Text = "Генератор списков переименований для перехода на SSAS2005";
            this.groupBoxFmmd.ResumeLayout(false);
            this.groupBoxFmmd.PerformLayout();
            this.groupBoxDestDir.ResumeLayout(false);
            this.groupBoxDestDir.PerformLayout();
            base.ResumeLayout(false);
        }

        private void InitWriter(string fileName, out XmlWriter writer)
        {
            writer = XmlWriter.Create(fileName, this.writerSettings);
            writer.WriteStartDocument();
            writer.WriteStartElement("replaces");
        }

        private void InitWriters(string outputDirName)
        {
            Directory.CreateDirectory(outputDirName);
            this.InitWriterSettings();
            this.InitWriter(outputDirName + @"\2000_to_2005.xml", out this.to2005writer);
            this.InitWriter(outputDirName + @"\2005_to_2000.xml", out this.to2000writer);
        }

        private void InitWriterSettings()
        {
            this.writerSettings = new XmlWriterSettings();
            this.writerSettings.Encoding = Encoding.GetEncoding("windows-1251");
            this.writerSettings.Indent = true;
            this.writerSettings.NewLineChars = Environment.NewLine;
            this.writerSettings.NewLineHandling = NewLineHandling.Replace;
        }

        private void RenameDimensions()
        {
            if (this.navigator != null)
            {
                XPathNodeIterator iterator = this.navigator.Select(".//DatabaseDimension");
                while (iterator.MoveNext())
                {
                    string attribute = iterator.Current.GetAttribute("name", string.Empty);
                    string newName = this.GenerateNewName(attribute);
                    this.WriteElementString(this.to2005writer, attribute, newName);
                    this.WriteElementString(this.to2000writer, newName, attribute);
                }
            }
        }

        private void WriteElementString(XmlWriter writer, string oldName, string newName)
        {
            writer.WriteStartElement("replace");
            writer.WriteAttributeString("type", "dimension");
            writer.WriteAttributeString("old", oldName);
            writer.WriteAttributeString("new", newName);
            writer.WriteEndElement();
        }
    }
}
 
