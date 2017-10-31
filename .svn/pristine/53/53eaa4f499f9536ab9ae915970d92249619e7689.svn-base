using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using System.Collections;


namespace Krista.FM.Client.HelpGenerator
{
    
    public partial class HelpGenerator : Form
    {
        /// <summary>
        /// Экземпляр класса менеджера генерации справки по кубам
        /// </summary>
        private HelpCubesManager cubesManager = new HelpCubesManager();

        public HelpCubesManager CubesManager
        {
            get { return cubesManager; }
            set { cubesManager = value; }
        }

        /// <summary>
        /// Код ошибки
        /// </summary>
        private int ErrorMessage;

        public HelpGenerator()
        {
            InitializeComponent();
            btnPush.Enabled = false;

            cubesManager.onNextObj += new EventHandler(cubesManager_onNextObj);

            // Создание справки с вариантом выбора XML-описания кубов
            cubesManager.variant = true;
        }

        /// <summary>
        /// Работа с прогрессбаром
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cubesManager_onNextObj(object sender, EventArgs e)
        {
            progressBar.PerformStep();
        }

        //Открываем файл с многомерной базой
        private void btnOpen_Click(object sender, EventArgs e)
        {
            label.Text = "Выберите XML файл с описанием базы";
            openFileDialog1.Filter = "XML files (*.xml)|*.XML|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // загружаем XML из файла
                cubesManager.FmmdAll.Load(openFileDialog1.FileName);

                cubesManager.UseFile = openFileDialog1.FileName;
                textBox.Text = openFileDialog1.FileName;
            }
            XmlNodeList dataBase;
            dataBase = cubesManager.FmmdAll.SelectNodes("//XMLDSOConverter/Databases/Database");
            if (dataBase.Count > 0)
            {
                radioBtn_SAS2000.Checked = true;
            }
            else
            {
                radioBtn_SAS2005.Checked = true;
            }
            if (cubesManager.FileName != "")
            {
                btnOpen.Enabled = false;
            }
            btnPush.Enabled = true;
        }

        public void btnPush_Click(object sender, EventArgs e)
        {
            Transform();
        }

        private void Transform()
        {
            // Проверяем какая из RadioButton зачекана и делаем проверку на соответствие с базой
            if (radioBtn_SAS2000.Checked)
            {
                XmlNodeList dataBase;
                dataBase = cubesManager.FmmdAll.SelectNodes("//XMLDSOConverter/Databases/Database");
                if (dataBase.Count > 0)
                {
                    ErrorMessage = 0;
                }
                else
                {
                    MessageBox.Show("Выбранный формат базы не соответствует XML файлу.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorMessage = 5;

                }

                cubesManager.IsAnalysis2000 = true;
            }
            if (radioBtn_SAS2005.Checked)
            {
                XmlNodeList dataBase;
                dataBase = cubesManager.FmmdAll.SelectNodes("//databaseheader/controlblock/content");
                if (dataBase.Count > 0)
                {
                    ErrorMessage = 0;
                }
                else
                {
                    MessageBox.Show("Выбранный формат базы не соответствует XML файлу.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ErrorMessage = 5;
                }

                cubesManager.IsAnalysis2000 = false;
            }
            if (ErrorMessage != 5)
            {
                // запускаем генерацию справки
                cubesManager.HelpGenerate();
            }
        }

       
    }
   
}
