using System;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.HelpGenerator
{
    public partial class CutFMMD_AllForm : Form
    {
        /// <summary>
        /// Класс для работы с VSS
        /// </summary>
        private IVSSFacade utils;
        /// <summary>
        /// Итоговое описание многомерки
        /// </summary>
        private XmlDocument fmmd_all = null;
        /// <summary>
        /// Относительный путь к FMMD_All
        /// </summary>
        string fmmd_all_path = "OLAP\\FMMD_All.xml";

        public XmlDocument Fmmd_all
        {
            get { return fmmd_all; }
        }

        public RichTextBox RichTextBoxCut
        {
            get { return richTextBox1; }
        }

        public CutFMMD_AllForm()
        {
            InitializeComponent();
            utils = new Providers.VSS.VSSFacade();


            rbFromVSS.Enabled = (SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeIniFile") ==
                                         null)
                                            ? false
                                            : true;
        }

        private void rbFromFile_CheckedChanged(object sender, EventArgs e)
        {
            if(rbFromFile.Checked)
            {
                InitializeOpenDialog();
            }
        }

        private void InitializeOpenDialog()
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Instance();
                fmmd_all.Load(openFileDialog.FileName);
                fmmd_all_path = openFileDialog.FileName;
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void Instance()
        {
            if (fmmd_all == null)
                fmmd_all = new XmlDocument();
        }

        private void rbFromVSS_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFromVSS.Checked)
            {
               utils.Open(SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeIniFile"),
               SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeUser"),
               SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafePassword"),
               SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeWorkingProject"),
               SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory);

                string local = GetLocalName();

                utils.Get(local, SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory + "\\" + fmmd_all_path);
                Instance();
                fmmd_all.Load(SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory + "\\" + fmmd_all_path);
            }
        }
        private string GetLocalName()
        {
            string local = fmmd_all_path;
            return local.Replace('\\', '/');
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            if (fmmd_all == null)
            {
                MessageBox.Show("Не задан выриант выбора FMMD_All", "Ошибка при выборе FMMD_All",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                return;
            }
            Operation operation = new Operation();
            try
            {
                operation.Text = "Создание урезанной FMMD_All";
                operation.StartOperation();
                richTextBox1.Text += "Начало операции создания урезанной FMMD_All...";

                CutFMMD_All.CutDatabase(ref fmmd_all, this);
                
                richTextBox1.Text += "\nЗавершение операции создания урезанной FMMD_All";
                operation.StopOperation();

                MessageBox.Show("Операция создания урезанной FMMD_All завершена", "Создание урезанной FMMD_All",
                                MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                btSave.Enabled = true;

            }
            catch(Exception ex)
            {
                throw new Exception("При создании урезанной FMMD_All возникла ошибка: " + ex);
            }
            finally
            {
                operation.ReleaseThread();
            }
        }


        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Не указан путь сохранения", "Ошибка при сохранении", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return;
            }
            richTextBox1.SaveFile(textBox3.Text);
        }

        private void btSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fmmd_all.Save(saveFileDialog1.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InitializeOpenDialog();
        }

        private void btBrouse_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = openFileDialog.Filter = "DOC files (*.doc)|*.doc|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog1.FileName);
            }
        }

        private void btBuffer_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
            richTextBox1.Copy();
        }
    }
}