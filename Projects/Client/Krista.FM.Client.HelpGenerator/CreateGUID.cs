using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.HelpGenerator
{
    /// <summary>
    /// Класс временный, создан для генерации GUID в многомерной базе
    /// </summary>
    public class CreateGUID
    {
        #region Fields

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
        public CreateGUID()
        {
            Operation operation = new Operation();
            try
            {
                operation.Text = "Начало операции создание GUID для объектов многомерной базы";
                operation.StartOperation();
                utils = new Providers.VSS.VSSFacade();
                // инициализация датасет
                // инициализация XML
                operation.Text = "Инициализация XML-описания";
                InitializeXML();
                // сравнение описаний
                operation.Text = "Создание GUID для объектов многомерной базы";
                Create();
                // сохранение
                operation.Text = "Сохранение результатов";
                Save();
                // применение в VSS
                operation.Text = "Сохранение в VSS";
                CheckIn();

                operation.StopOperation();

                MessageBox.Show("Операция по созданию GUID для объектов многомерной базы завершена", "Перенос описаний", MessageBoxButtons.OK,
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
                utils.Checkin(GetLocalName(), String.Format("Создание GUID для объектов многомерной базы в FMMD_All ({0}:{1}; схема:{2}; пользователь: {3})", SchemeEditor.SchemeEditor.Instance.Scheme.Server.Machine, SchemeEditor.SchemeEditor.Instance.Scheme.Server.GetConfigurationParameter("ServerPort"), SchemeEditor.SchemeEditor.Instance.Scheme.Name, ClientAuthentication.UserName));
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
        /// Создание гвидов
        /// </summary>
        private void Create()
        {
            Create(SchemeEditor.SchemeEditor.Instance.Scheme.RootPackage);
        }

        /// <summary>
        /// Сравнение и внесение изменений в описания объектов FMMD_All.xml
        /// </summary>
        /// <param name="iPackage"></param>
        private void Create(IPackage iPackage)
        {
            foreach (IPackage package in iPackage.Packages.Values)
                Create(package);

            foreach (IEntity entity in iPackage.Classes.Values)
            {
                // Описание к кубам
                XmlNodeList cubes =
                    fmmd_all.SelectNodes(string.Format("XMLDSOConverter/Databases/Database/Cubes/Cube[CustomProperties/Property[@name='FullName']='{0}']", entity.FullName));
                foreach (XmlNode cube in cubes)
                {
                    InsertObjectKey(entity, cube);
                    XmlNodeList partitions = cube.SelectNodes("Partitions/Partition");
                    foreach(XmlNode partition in partitions)
                    {
                        InsertObjectKey(entity, partition);
                    }
                }

                // Описание к измерениям
                XmlNodeList dimentions =
                    fmmd_all.SelectNodes(string.Format("XMLDSOConverter/Databases/Database/DatabaseDimensions/DatabaseDimension[CustomProperties/Property[@name='FullName']='{0}']", entity.FullName));

                foreach (XmlNode dimention in dimentions)
                {
                    InsertObjectKey(entity, dimention);
                }
            }
        }

        private void InsertObjectKey(IEntity entity, XmlNode cube)
        {
            XmlNode customProperties = cube.SelectSingleNode("CustomProperties");

            if (customProperties != null)
            {
                if (!Existtest(customProperties))
                {
                    XmlElement objectKey = fmmd_all.CreateElement("Property");

                    XmlHelper.SetAttribute(objectKey, "name", "ObjectKey");
                    XmlHelper.SetAttribute(objectKey, "datatype", "8");

                    XmlHelper.AppendCDataSection(objectKey, entity.ObjectKey);

                    customProperties.AppendChild(objectKey);
                }
            }
        }

        private static bool Existtest(XmlNode properties)
        {
            foreach (XmlNode childNode in properties.ChildNodes)
            {
                if (childNode.Attributes["name"].Value == "ObjectKey")
                    return true;
            }

            return false;
        }
    }
}
