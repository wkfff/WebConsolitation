using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;
using SourceSafeTypeLib;

namespace Krista.FM.Client.HelpGenerator
{
    /// <summary>
    /// Генерация справки по кубам
    /// </summary>
    public class HelpCubesManager
    {
        #region Fields

        /// <summary>
        /// Исходное XML-описание, по которому создаем справку
        /// </summary>
        private XmlDocument fmmdAll;

        /// <summary>
        /// Определение путь к источнику с ресурсами, необходимыми для преобразования
        /// </summary>
        private string fileDirectoryProfile;
        /// <summary>
        /// Файл с исходным XML описанием кубов и измерений
        /// </summary>
        private string useFile;

        /// <summary>
        /// Результирующий файл
        /// </summary>
        private string fileResult;

        /// <summary>
        /// Версия аналазиса(по-умолчанию 2000)
        /// </summary>
        private bool isAnalysis2000 = true;
        /// <summary>
        /// Вариант создания справки по кубам(по-умолчанию по ВСС)
        /// </summary>
        public bool variant = false;

        public bool IsAnalysis2000
        {
            get { return isAnalysis2000; }
            set { isAnalysis2000 = value; }
        }

        private string fileName;

        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        #endregion Fields

        #region Properties

        public string FileResult
        {
            get { return fileResult; }
            set { fileResult = value; }
        }

        public string UseFile
        {
            get { return useFile; }
            set { useFile = value; }
        }

        public string FileDirectoryProfile
        {
            get { return fileDirectoryProfile; }
            set { fileDirectoryProfile = value; }
        }

        public XmlDocument FmmdAll
        {
            get { return fmmdAll; }
            set { fmmdAll = value; }
        }

        #endregion

        #region Events
        /// <summary>
        /// Событие при обработке очередного объекта
        /// </summary>
        public event EventHandler onNextObj;

        #endregion 

        #region Constructor

        public HelpCubesManager()
        {
            fmmdAll = new XmlDocument();
        }

        #endregion

        #region Methods

        private void CallEvent(EventArgs args)
        {
            if (onNextObj != null)
                onNextObj(this, args);
        }

        private void NextObj()
        {
            EventArgs args = new EventArgs();
            CallEvent(args);
        }

        /// <summary>
        /// Преобразование из XML в HTML
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Dimension"></param>
        /// <param name="Cube"></param>
        /// <param name="HHC"></param>
        /// <param name="HHK"></param>
        /// <param name="HHP"></param>
        /// <param name="CubeNode"></param>
        /// <param name="DimensionNode"></param>
        private void TransformXslt(string Index, string Dimension, string Cube, string HHC, string HHK, string HHP, XmlNodeList CubeNode, XmlNodeList DimensionNode)
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileDirectoryProfile + "\\" + Index);
            xslt.Transform(UseFile, AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Index.htm");

            // Генерируем HTML сраницы по измерениям
            XslCompiledTransform xsltDim = new XslCompiledTransform();
            for (int i = 1; DimensionNode.Count + 1 > i; i++)
            {
                xsltDim.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, Dimension));
                XsltArgumentList xslArgD = new XsltArgumentList();
                xslArgD.AddParam("index", "", i);
                XmlWriter writer = XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Pages\\" + "Dim" + i +
                                 ".htm");
                xsltDim.Transform(UseFile, xslArgD, writer);
                NextObj();

                writer.Close();
            }

            // Генерируем HTML сраницы по кубам
            XslCompiledTransform xsltCube = new XslCompiledTransform();
            for (int j = 1; CubeNode.Count + 1 > j; j++)
            {
                xsltCube.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, Cube));
                XsltArgumentList xslArg = new XsltArgumentList();
                xslArg.AddParam("index", "", j);
                XmlWriter writer =
                    XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Pages\\" + "Cube" +
                                     j + ".htm");
                xsltCube.Transform(UseFile, xslArg, writer);
                NextObj();

                writer.Close();
            }

            xslt.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, HHK));
            xslt.Transform(UseFile, AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Index.hhk");
            xslt.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, HHC));
            xslt.Transform(UseFile, AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\TOC.hhc");
            xslt.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, HHP));
            xslt.Transform(UseFile, AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Cubes.hhp");
        }

        /// <summary>
        /// Создание рабочих директорий
        /// </summary>
        private void FileDefinition()
        {
            fileDirectoryProfile = "HelpGeneratorProfileOLAP";
            fileResult = "HelpGeneratorResultOLAP";

            // Создаем папку "HelpGeneratorResult" и копируем туда инфу из папки "HelpGeneratorProfile"
            //string XPathDirrectory = FileDirrectory + "HelpGeneratorResult";
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult);

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Resources"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Resources");

            // Создаем папку Pages, в которую будем генерировать страницы
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Pages"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Pages");

            Copy();

        }

        private void Copy()
        {
            // Скопировать возможно и проще, нужно разбираться
            Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileDirectoryProfile + "\\Resources");
            string[] countFileResources = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\" + fileDirectoryProfile + "\\Resources");
            //
            string[] expressionDelimiter = { "\\", };
            string[] splitedTable;
            for (int i = 0; countFileResources.Length > i; i++)
            {
                splitedTable = countFileResources[i].Split(expressionDelimiter, StringSplitOptions.RemoveEmptyEntries);
                int lengthFile;
                for (lengthFile = 0; splitedTable.Length - 1 > lengthFile; lengthFile++)
                {

                }
                string currentFile = splitedTable[lengthFile];
                File.Copy(countFileResources[i], AppDomain.CurrentDomain.BaseDirectory + "\\" + fileResult + "\\Resources\\" + currentFile, true);
            }
        }

        /// <summary>
        /// Вызов преобразования
        /// </summary>
        private  void Transform()
        {
            //создание стартовой странички intro.html
            CreateIntro();
            //// Собственно генерация
            //Генерим для SAS 2000
            if (isAnalysis2000)
            {
                TransformXslt("Index.xsl", "Dimension.xsl", "Cube.xsl", "HHC.xsl", "HHK.xsl", "HHP.xsl",
                    fmmdAll.SelectNodes("//XMLDSOConverter/Databases/Database/Cubes/Cube"), fmmdAll.SelectNodes("//Databases/Database/DatabaseDimensions/DatabaseDimension"));

            }
            else
            {
                TransformXslt("IndexN.xsl", "Dimension_N.xsl", "Cube_N.xsl", "HHC_N.xsl", "HHK_N.xsl", "HHP_N.xsl",
                    fmmdAll.SelectNodes("//databaseheader/controlblock/content/cubes/cube"), fmmdAll.SelectNodes("//databaseheader/controlblock/content/dimensions/dimension"));

            }
        }

        private void CreateIntro()
        {
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(string.Format("{0}\\{1}\\{2}", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile, "Head.xsl"));

            XmlDocument doc = new XmlDocument();
            doc.InnerXml =
                        String.Format("<?xml version = \"1.0\" encoding = \"windows-1251\"?><ServerConfiguration/>");
            XmlNode captionNode = doc.CreateElement("Caption");
            captionNode.InnerText = "Описание многомерной базы";
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
            dbVersionNode.InnerText = String.Format("Версия многомерной базы данных: {0}", SchemeEditor.SchemeEditor.Instance.Scheme.SchemeMDStore.DatabaseVersion);
            doc.DocumentElement.AppendChild(dbVersionNode);

            XmlNode productNameNode = doc.CreateElement("ProductName");
            productNameNode.InnerText = "(с) НПО Криста 2006-2008";
            doc.DocumentElement.AppendChild(productNameNode);

            XmlNode coordinateNode = doc.CreateElement("Coordinate");
            coordinateNode.InnerText = String.Format("{0}", SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SupportServiceInfo"));
            doc.DocumentElement.AppendChild(coordinateNode);

            XmlReader reader = XmlReader.Create(new StringReader(doc.InnerXml));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            XmlWriter writer =
                XmlWriter.Create(
                    String.Format(
                    @"{0}\{1}\intro.htm", AppDomain.CurrentDomain.BaseDirectory, fileResult), settings);

            if (true)
                xslt.Transform(reader, writer);

            writer.Close();
            reader.Close();
        }

        /// <summary>
        /// Загружаем FMMDALL из VSS
        /// </summary>
        private void GetXML()
        {
            // Create a VSSDatabase object.
            IVSSDatabase vssDatabase = new VSSDatabase();

            // Open a VSS database using network name 
            // for automatic user login.
            vssDatabase.Open(
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeIniFile"),
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeUser"),
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafePassword"));

            IVSSItem vssFile =
                vssDatabase.get_VSSItem(
                    "$/dotNET/Repository/_Подопытная/OLAP/FMMD_All.xml", false);
            // Get a file into a specified folder.

            string testFile = null;
            vssFile.Get(ref testFile, 0);

            if (File.Exists("FMMD_All.xml"))
            {
                File.SetAttributes("FMMD_All.xml", FileAttributes.Normal);
                if (File.Exists(String.Format(@"{0}\{1}\FMMD_All.xml", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile)))
                    File.SetAttributes(String.Format(@"{0}\{1}\FMMD_All.xml", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile), FileAttributes.Normal);

                File.Copy("FMMD_All.xml", String.Format(@"{0}\{1}\FMMD_All.xml", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile), true);
                File.Delete("FMMD_All.xml");
            }

            fmmdAll.Load(String.Format(@"{0}\{1}\FMMD_All.xml", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile));

            useFile = String.Format(@"{0}\{1}\FMMD_All.xml", AppDomain.CurrentDomain.BaseDirectory, fileDirectoryProfile);
        }

        /// <summary>
        /// !Точка входа!
        /// </summary>
        public void HelpGenerate()
        {
            Operation operation = new Operation();

            try
            {
                operation.StartOperation();
                operation.Text = "Создание каталогов";
                FileDefinition();

                operation.Text = "Получение XML-описания";
                if(!variant)
                    GetXML();

                operation.Text = "Преобразование из XML в HTML";
                Transform();
                operation.StopOperation();

                MessageBox.Show("Генерация справки успешно завершена", "Генерация справки", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            finally
            {
                operation.ReleaseThread();
                operation = null;
            }
        }

        #endregion Methods
    }
}
