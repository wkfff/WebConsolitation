using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;
using VSSFileStatus=Krista.FM.ServerLibrary.VSSFileStatus;

namespace Krista.FM.Client.HelpGenerator
{
    /// <summary>
    /// Класс для сравнение описания объектов в схеме и в FMMD_All
    /// для исправления в дизайнере выводим два варианта описания
    /// </summary>
    public class CompareDescription
    {
        #region Fields

        /// <summary>
        /// Для тестовых целей, будет удален 
        /// </summary>
        private DataSet dataSet;
        /// <summary>
        /// Итоговая XML
        /// </summary>
        private XmlDocument fmmd_all = new XmlDocument();
        /// <summary>
        /// Класс для работы с VSS
        /// </summary>
        private IVSSFacade utils;

        #endregion Fields

        #region Const
        /// <summary>
        /// Относительный путь к FMMD_All
        /// </summary>
        const string fmmd_all_path = "OLAP\\FMMD_All.xml";

        #endregion Const

        /// <summary>
        /// Конструктор
        /// </summary>
        public CompareDescription()
        {
            Operation operation = new Operation();
            try
            {
                operation.Text = "Начало операции переноса описаний";
                operation.StartOperation();
                utils = new Providers.VSS.VSSFacade();
                // инициализация датасет
                Initialize();
                // инициализация XML
                operation.Text = "Инициализация XML-описания";
                InitializeXML();
                // сравнение описаний
                operation.Text = "Перенос описаний";
                Compare();
                // сохранение
                operation.Text = "Сохранение результатов";
                Save();
                // применение в VSS
                operation.Text = "Сохранение в VSS";
                CheckIn();

                operation.StopOperation();

                MessageBox.Show("Операция по переносу описаний завершена", "Перенос описаний", MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
            }
            finally
            {
                operation.ReleaseThread();
                operation = null;
            }
        }


        private void Save()
        {
            XmlHelper.Save(fmmd_all, SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory + "//" + fmmd_all_path);
        }

        private void CheckIn()
        {
            if (utils.IsCheckedOut(GetLocalName()) == VSSFileStatus.VSSFILE_CHECKEDOUT_ME)
                utils.Checkin(GetLocalName(), String.Format("Перенос описаний семантической структуры в FMMD_All ({0}:{1}; схема:{2}; пользователь: {3})", SchemeEditor.SchemeEditor.Instance.Scheme.Server.Machine, SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("ServerPort"), SchemeEditor.SchemeEditor.Instance.Scheme.Name, ClientAuthentication.UserName));
        }

        private void InitializeXML()
        {
            utils.Open(SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeIniFile"),
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeUser"),
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafePassword"),
                SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("SourceSafeWorkingProject"),
                SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory);

            string local = GetLocalName();

            switch(utils.IsCheckedOut(local))
            {
                case VSSFileStatus.VSSFILE_NOTCHECKEDOUT:
                    utils.Checkout(local, String.Empty);
                    fmmd_all.Load(SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory + "\\" + fmmd_all_path);
                    break;
                case VSSFileStatus.VSSFILE_CHECKEDOUT_ME:
                    fmmd_all.Load(SchemeEditor.SchemeEditor.Instance.Scheme.BaseDirectory + "\\" + fmmd_all_path);
                    break;
                case VSSFileStatus.VSSFILE_CHECKEDOUT:
                    throw new Exception(String.Format("Файл {0} заблокирован другим пользователем", local));
            }
        }

       
        private static string GetLocalName()
        {
            string local = fmmd_all_path;
            return local.Replace('\\', '/');
        }

        /// <summary>
        /// Сравнение описаний
        /// </summary>
        private void Compare()
        {
            Compare(SchemeEditor.SchemeEditor.Instance.Scheme.RootPackage);
        }

        /// <summary>
        /// Сравнение и внесение изменений в описания объектов FMMD_All.xml
        /// </summary>
        /// <param name="iPackage"></param>
        private void Compare(IPackage iPackage)
        {
            foreach (IPackage package in iPackage.Packages.Values)
                Compare(package);

            foreach (IEntity entity in iPackage.Classes.Values)
            {
                
                // Описание к кубам
                XmlNode cube =
                    fmmd_all.SelectSingleNode(string.Format("//XMLDSOConverter/Databases/Database/Cubes/Cube[@name='{0}']", entity.Caption));
                if (cube != null)
                {
                    if (String.Compare(entity.Description, cube.Attributes["Description"].Value.ToString()) != 0)
                        cube.Attributes["Description"].Value = entity.Description;

                    // Описание к мерам
                    foreach (IDataAttribute attribute in entity.Attributes.Values)
                    {
                        XmlNode attr = fmmd_all.SelectSingleNode(string.Format("//XMLDSOConverter/Databases/Database/Cubes/Cube[@name='{0}']//CubeMeasures//CubeMeasure[@name='{1}']",entity.Caption, attribute.Caption));
                        if (attr != null)
                        {
                            if (
                                String.Compare(attribute.Description, attr.Attributes["Description"].Value.ToString()) ==
                                0)
                            {
                            }
                            else attr.Attributes["Description"].Value = attribute.Description;
                        }
                    }
                }

                // Описание к измерениям
                XmlNode dimention =
                    fmmd_all.SelectSingleNode(string.Format("//XMLDSOConverter/Databases/Database/DatabaseDimensions/DatabaseDimension[@name = '{0}']", entity.OlapName));

                if (dimention != null)
                    if (String.Compare(entity.Description, dimention.Attributes["Description"].Value.ToString()) != 0)
                        dimention.Attributes["Description"].Value = entity.Description;
            }
        }

        private void Initialize()
        {
            dataSet = new DataSet();

            DataTable table = new DataTable();

            DataColumn package = new DataColumn();
            package.ColumnName = "package";

            DataColumn olapName = new DataColumn();
            olapName.ColumnName = "entityOLAPName";

            DataColumn schemeDescription = new DataColumn();
            schemeDescription.ColumnName = "schemeDescription";

            DataColumn fmmd_allDescription = new DataColumn();
            fmmd_allDescription.ColumnName = "fmmdallDescription";

            table.Columns.AddRange(new DataColumn[]{package, olapName, schemeDescription, fmmd_allDescription});

            dataSet.Tables.Add(table);
        }

        public DataSet CompareDataSet
        {
            get { return dataSet; }
        }

    }
}
