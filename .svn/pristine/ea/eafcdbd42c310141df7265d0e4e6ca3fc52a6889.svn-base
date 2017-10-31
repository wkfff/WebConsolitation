using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Infragistics.Win.UltraWinDock;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;

namespace Krista.FM.Client.MDXExpert
{
    // Класс для сохранения параметров DockManager
    [Serializable]
    [XmlInclude(typeof(CustomDockableControlPane))]
    [XmlInclude(typeof(CustomDockableGroupPane))]
    public class DockSettings
    {
        #region Поля

        private Random random = new Random();

        // версия отчета
        public ExpertVersion version;

        // Коллекция групповых докабельных окон
        public List<CustomDockAreaPane> customDockAreaPaneCollection;

        // Коллекция хэш-кодов
        public List<int> hashCodeCollection;

        // Словарь соответствия уникальных ключей и хэш-кодов
        [XmlIgnore]
        private Dictionary<int, Guid> idDictionary;

        #endregion

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public DockSettings()
        {
            customDockAreaPaneCollection = new List<CustomDockAreaPane>();
            hashCodeCollection = new List<int>();
            idDictionary = new Dictionary<int, Guid>();
            version = new ExpertVersion();
        }

        /// <summary>
        /// Сброс параметров сохранения
        /// </summary>
        public void ResetSettings()
        {
            customDockAreaPaneCollection.Clear();
            hashCodeCollection.Clear();
            idDictionary.Clear();
        }

        #region Сохранение

        /// <summary>
        /// Заполнение полей класса параметрами DockManager
        /// </summary>
        /// <param name="udManager">сохраняемый DockManager</param>
        public bool Save(UltraDockManager udManager, string fileName)
        {
            hashCodeCollection.Clear();

            version.Number = Consts.applicationVersion;
            version.Type = Consts.versionType;
            version.Format = Consts.formatVersion;

            customDockAreaPaneCollection.Clear();
            foreach (DockAreaPane daPane in udManager.DockAreas)
            {
                AddAreaPaneToCollection(daPane);
            }

            return SaveToXML(fileName);
        }

        /// <summary>
        /// Сохранение объекта класса DockSettings в XML-файл
        /// </summary>
        /// <param name="fileName">имя файла</param>
        private bool SaveToXML(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write,
                    FileShare.None))
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(DockSettings));
                    xmlFormat.Serialize(stream, this);
                }
                return true;
            }
            catch (Exception e)
            {
                FormException.ShowErrorForm(e, ErrorFormButtons.WithoutTerminate);
                return false;
            }
        }

        /// <summary>
        /// Добавление области в коллекцию
        /// </summary>
        /// <param name="daPane">добавляемая область</param>
        private void AddAreaPaneToCollection(DockAreaPane daPane)
        {
            CustomDockAreaPane cdaPane = new CustomDockAreaPane(daPane.GetHashCode(), daPane.DockedLocation, daPane.FloatingLocation, daPane.Size, daPane.ChildPaneStyle, daPane.SelectedTabIndex);
            customDockAreaPaneCollection.Add(cdaPane);
            AddHashCode(daPane.GetHashCode());

            foreach (DockablePaneBase dpBase in daPane.Panes)
            {
                if (dpBase is DockableControlPane)
                {
                    AddControlPaneToCollection(cdaPane, (DockableControlPane)dpBase);
                }
                else if (dpBase is DockableGroupPane)
                {
                    AddGroupPaneToCollection(cdaPane, (DockableGroupPane)dpBase);
                }
            }
        }

        /// <summary>
        /// Добавление группового окна в коллекцию
        /// </summary>
        /// <param name="parent">родитель окна</param>
        /// <param name="dgPane">добавляемое окно</param>
        private void AddGroupPaneToCollection(object parent, DockableGroupPane dgPane)
        {
            CustomDockableGroupPane cdgPane = new CustomDockableGroupPane(dgPane.GetHashCode(),
                dgPane.ParentFloating == null ? 0 : dgPane.ParentFloating.GetHashCode(),
                dgPane.ParentDocked == null ? 0 : dgPane.ParentDocked.GetHashCode(),
                dgPane.DockedState,
                dgPane.ChildPaneStyle,
                dgPane.Size,
                dgPane.SelectedTabIndex,
                dgPane.Minimized);
            AddHashCode(dgPane.GetHashCode());

            if (parent is CustomDockAreaPane)
            {
                ((CustomDockAreaPane)parent).customDockableBaseCollection.Add(cdgPane);
            }
            else if (parent is CustomDockableGroupPane)
            {
                ((CustomDockableGroupPane)parent).customDockableBaseCollection.Add(cdgPane);
            }

            foreach (DockablePaneBase dpBase in dgPane.Panes)
            {
                if (dpBase is DockableControlPane)
                {
                    AddControlPaneToCollection(cdgPane, (DockableControlPane)dpBase);
                }
                else if (dpBase is DockableGroupPane)
                {
                    AddGroupPaneToCollection(cdgPane, (DockableGroupPane)dpBase);
                }
            }
        }

        /// <summary>
        /// Добавление докабельного окна в коллекцию
        /// </summary>
        /// <param name="parent">родитель окна</param>
        /// <param name="dcPane">добавляемое окно</param>
        private void AddControlPaneToCollection(object parent, DockableControlPane dcPane)
        {
            CustomReportElement reportElement = (CustomReportElement)dcPane.Control;

            CustomDockableControlPane cdcPane = new CustomDockableControlPane(dcPane.GetHashCode(),
                dcPane.ParentFloating == null ? 0 : dcPane.ParentFloating.GetHashCode(),
                dcPane.ParentDocked == null ? 0 : dcPane.ParentDocked.GetHashCode(),
                dcPane.DockedState,
                reportElement.Size,
                dcPane.Pinned,                
                dcPane.Minimized,
                reportElement.Save());

            AddHashCode(dcPane.GetHashCode());

            //cdcPane.ReportElementNode = reportElement.Save();

            if (parent is CustomDockAreaPane)
            {
                ((CustomDockAreaPane)parent).customDockableBaseCollection.Add(cdcPane);
            }
            else if (parent is CustomDockableGroupPane)
            {
                ((CustomDockableGroupPane)parent).customDockableBaseCollection.Add(cdcPane);
            }
        }

        #endregion

        #region Восстановление

        private bool CheckReportVersion()
        {
            VersionRelation vr = CommonUtils.GetVersionRelation(version.Number, Consts.applicationVersion);
            switch (vr)
            {
                case VersionRelation.Future:
                    FormException.ShowErrorForm(new Exception(
                                                    "Отчет имеет более новую версию (" + version.Number +
                                                    ") по отношению к текущей версии MDX Эксперт (" +
                                                    Consts.applicationVersion + ")."),
                                                ErrorFormButtons.WithoutTerminate);
                    return false;
                case VersionRelation.Modern:
                    if (version.Format > Consts.formatVersion)
                    {
                        FormException.ShowErrorForm(new Exception(
                                          "Отчет имеет более новую версию формата по отношению к текущей версии MDX Эксперт."),
                                                    ErrorFormButtons.WithoutTerminate);
                        return false;
                    }
                    return true;
            }
            return true;
        }

        /// <summary>
        /// Загрузка параметров DockManager из XML
        /// </summary>
        /// <param name="udManager"></param>
        /// <param name="dockPanelControl"></param>
        public void Load(UltraDockManager udManager, DockPanelControl dockPanelControl, string fileName)
        {
            // Десериализация объекта
            DockSettings tempSettings = (DockSettings)LoadFromXML(fileName);
            if (tempSettings == null)
            {
                //MessageBox.Show("Файл DockSettings.xml пуст.");
                //return;
            }
            if (tempSettings != null)
                CopySettings(tempSettings);

            if (!CheckReportVersion())
            {
                ResetSettings();
                return;
            }

            FillHashCodeCollection();
            hashCodeCollection.Clear();

            foreach (CustomDockAreaPane daPane in customDockAreaPaneCollection)
            {
                RestoreAreasPanes(daPane, dockPanelControl);
            }
        }

        /// <summary>
        /// Загрузка из XML-файла
        /// </summary>
        /// <param name="fileName">имя файла</param>
        /// <returns>десериализованный объект</returns>
        private static object LoadFromXML(string fileName)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                
                object ds = null;
                if (stream.Length > 0)
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof (DockSettings));
                    ds = xmlFormat.Deserialize(stream);
                }
                stream.Close();
                return ds;
            }
            catch(Exception e)
            {
                Common.CommonUtils.ProcessException(e);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return null;
        }

        /// <summary>
        /// Восстановление параметров из десериализованного объекта
        /// </summary>
        /// <param name="tempSettings">десериализованный объект</param>
        private void CopySettings(DockSettings tempSettings)
        {
            customDockAreaPaneCollection.Clear();
            foreach (CustomDockAreaPane areaPane in tempSettings.customDockAreaPaneCollection)
            {
                customDockAreaPaneCollection.Add(areaPane);
            }

            hashCodeCollection.Clear();
            foreach (int code in tempSettings.hashCodeCollection)
            {
                hashCodeCollection.Add(code);
            }

            version = tempSettings.version;
        }

        /// <summary>
        /// Восстановление области
        /// </summary>
        /// <param name="daPane">восстанавливаемая область</param>
        /// <param name="panelControl">владелец DockManager'а</param>
        private void RestoreAreasPanes(CustomDockAreaPane daPane, DockPanelControl panelControl)
        {
            DockAreaPane newAreaPane = panelControl.AddDockAreaPane(idDictionary[daPane.PaneId],
                                                                    daPane.DockLocation,
                                                                    daPane.LocationPoint,
                                                                    daPane.Size,
                                                                    daPane.ChildPaneStyle);

            foreach (CustomDockablePaneBase cdBase in daPane.customDockableBaseCollection)
            {
                if (cdBase is CustomDockableControlPane)
                {
                    RestoreControlPanes((CustomDockableControlPane)cdBase, newAreaPane, panelControl);
                }
                else if (cdBase is CustomDockableGroupPane)
                {
                    RestoreGroupPanes((CustomDockableGroupPane)cdBase, newAreaPane, panelControl);
                }
            }

            if (daPane.ChildPaneStyle == ChildPaneStyle.TabGroup)
            {
                newAreaPane.SelectedTabIndex = daPane.SelectedTabIndex;
            }
        }

        /// <summary>
        /// Восстановление группового окна
        /// </summary>
        /// <param name="dgPane">восстанавливаемое окно </param>
        /// <param name="parent">родитель окна</param>
        /// <param name="panelControl">владелец DockManager'а</param>
        private void RestoreGroupPanes(CustomDockableGroupPane dgPane, object parent, DockPanelControl panelControl)
        {
            DockableGroupPane newGroupPane = panelControl.AddDockableGroupPane(parent,
                                                                               idDictionary[dgPane.PaneId],
                                                                               idDictionary[dgPane.FloatPaneId],
                                                                               idDictionary[dgPane.DockPaneId],
                                                                               dgPane.State,
                                                                               dgPane.Style,
                                                                               dgPane.Size,
                                                                               dgPane.Minimized);

            foreach (CustomDockablePaneBase cdBase in dgPane.customDockableBaseCollection)
            {
                if (cdBase is CustomDockableControlPane)
                {
                    RestoreControlPanes((CustomDockableControlPane) cdBase, newGroupPane, panelControl);
                }
                else if (cdBase is CustomDockableGroupPane)
                {
                    RestoreGroupPanes((CustomDockableGroupPane) cdBase, newGroupPane, panelControl);
                }
            }

            if (dgPane.Style == ChildPaneStyle.TabGroup)
            {
                newGroupPane.SelectedTabIndex = dgPane.SelectedTabIndex;
            }
        }

        /// <summary>
        /// Восстановление докабельного окна
        /// </summary>
        /// <param name="dcPane">восстанавливаемое окно</param>
        /// <param name="parent">родитель окна</param>
        /// <param name="panelControl">владелец DockManager'а</param>
        private void RestoreControlPanes(CustomDockableControlPane dcPane, object parent, DockPanelControl panelControl)
        {
            panelControl.AddDockableControlPane(parent,
                                                this,
                                                idDictionary[dcPane.PaneId],
                                                idDictionary[dcPane.FloatPaneId],
                                                idDictionary[dcPane.DockPaneId],
                                                dcPane.Size,
                                                dcPane.Minimized,
                                                dcPane.Pinned,
                                                dcPane.ReportElementNode,
                                                dcPane.ElementType);
        }

        #endregion

        #region Работа с hash-кодами

        /// <summary>
        /// Добавление хэш-кода в коллекцию
        /// </summary>
        /// <param name="code"></param>
        public void AddHashCode(int code)
        {
            hashCodeCollection.Add(code);
        }

        /// <summary>
        /// Заполнение словаря соответствия уникальных ключей и хэш-кодов
        /// </summary>
        private void FillHashCodeCollection()
        {
            idDictionary.Clear();
            idDictionary.Add(0, new Guid("00000000-0000-0000-0000-000000000000"));
            foreach (int code in hashCodeCollection)
            {
                Guid id = Guid.NewGuid();
                idDictionary.Add(code, id);
            }
        }

        #endregion
    }
}
