using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FactTables;
using Krista.FM.ServerLibrary;
using Krista.FM.Client.Common;

using Krista.FM.Client.ViewObjects.AssociatedCLSUI.AssociatedCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.FixedCls;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{

	/// <summary>
	/// кэш вызываемых модальных классификаторов
	/// хранит только контролы, данные не хранит (обновляются при обращении)
	/// </summary>
    public class ModalClsManager : IModalClsManager
	{
		// Интервал времени, по истечении которого классификатор должен быть обновлен (ms)
		public const int ClsReloadInterval = 6000;
		
		// Коллекция уже загруженных классификаторов
		//private Dictionary<string, frmModalTemplate> LoadedCls;

        private Dictionary<string, IInplaceClsView> loadedClsObjects;

        // ссылка на родительский Workplace
		private IWorkplace _Workplace;

        /// <summary>
		/// конструктор класса 
        /// </summary>
        /// <param name="Workplace">Родительский Workplace</param> 
		public ModalClsManager(IWorkplace Workplace)
        {
            _Workplace = Workplace;
        }

        /// <summary>
		/// Очистка ресурсов (выгрузка форм классификаторов)
		/// </summary>
 		public void Clear()
		{
			// Если есть загруженные классификаторы - для каждого освобождаем ресурсы
			/*if (LoadedCls != null)
			{
				// Для каждого загруженного 
				foreach (KeyValuePair<string, frmModalTemplate> kvp in LoadedCls)
				{
					frmModalTemplate tmpForm = (frmModalTemplate)kvp.Value;
					tmpForm.AttachedCls = null;
					tmpForm.Dispose();
				}
				// очищаем список загруженных классификаторов
				LoadedCls.Clear();
			}*/
            if (loadedClsObjects != null)
            {
                foreach (IInplaceClsView viewClsObject in loadedClsObjects.Values)
                {
                    viewClsObject.DetachViewObject();
                }
                loadedClsObjects.Clear();
            }
			// освобождаем ссылку на Workplace
            _Workplace = null;
		}

        // singleId - true - возвращается id текущей записи (int); false - возвращается список выбранных записей (List<int>)
        public bool ShowClsModal(string clsKey, int oldClsID, int sourceID, int sourceYear, ref object clsID, bool singleId)
        {
            if ((_Workplace == null) || (_Workplace.ActiveScheme == null))
            {
                throw new InvalidOperationException("Нет активного подключения");
            }
            // если коллекция загруженных классификаторов еще не создана - создаем
            if (loadedClsObjects == null)
                loadedClsObjects = new Dictionary<string, IInplaceClsView>();

            IInplaceClsView currentViewObject = null;
            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            string modalClassifierCaption = string.Empty;
            // пытаемся найти запрашиваемый классификатор в коллекции
            if (loadedClsObjects.ContainsKey(clsKey))
            {
                // если нашли и прошел некоторый интервал времени с момента последнего его показа - обновляем данные
                currentViewObject = loadedClsObjects[clsKey];
                modalClsForm.AttachCls(currentViewObject);
                if (DataSourceContext.CurrentDataSourceYear != sourceYear)
                {
                    if (sourceID < 0)
                        modalClsForm.AttachedCls.RefreshAttachedData();
                    else
                    {
                        modalClsForm.AttachedCls.RefreshAttachedData(sourceID);
                        currentViewObject.TrySetDataSource(sourceID);
                    }
                }
            }
            else
            {
                // получаем нужный классификатор
                IEntity cls = _Workplace.ActiveScheme.RootPackage.FindEntityByName(clsKey);
                // создаем объект просмотра классификаторов нужного типа
                BaseClsUI clsUI = null;
                switch (cls.ClassType)
                {
                    case ClassTypes.clsFixedClassifier:
                        clsUI = new FixedClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Фиксированный классификатор", cls.OlapName);
                        break;
                    case ClassTypes.clsBridgeClassifier:
                        clsUI = new AssociatedClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Сопоставимый классификатор", cls.OlapName);
                        break;
                    case ClassTypes.clsDataClassifier:
                        clsUI = new DataClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Классификатор данных", cls.OlapName);
                        break;
                    case ClassTypes.clsFactData:
                        clsUI = new FactTablesUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Таблица фактов", cls.OlapName);
                        break;

                }
                modalClsForm.FormCaption = modalClassifierCaption;
                // инициализируем его
                clsUI.Workplace = _Workplace;
                clsUI.RestoreDataSet = false;
                clsUI.InInplaceMode = true;
                clsUI.CurrentDataSourceYear = sourceYear;
                clsUI.Initialize();
                clsUI.CurrentDataSourceID = sourceID;
                currentViewObject = (IInplaceClsView)clsUI;
                currentViewObject.InitModalCls(oldClsID);
                modalClsForm.SuspendLayout();
                // и присоединяем к форме
                try
                {
                    modalClsForm.AttachCls(currentViewObject);
                    ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
                }
                finally
                {
                    modalClsForm.ResumeLayout();
                }
                // ...загружаем данные
                currentViewObject.RefreshAttachedData();
                // ...помещаем в кэш
                loadedClsObjects.Add(clsKey, currentViewObject);
            }
            // показываем классификатор
            modalClsForm.LastShowTickCount = System.Environment.TickCount;
            
            if (modalClsForm.ShowDialog((Form)_Workplace) != DialogResult.OK)
                return false;

            if (singleId)
            {
                clsID = modalClsForm.AttachedCls.GetSelectedID();
                // если ничего не выбрали - считаем что функция завершилась неудачно
                if (Convert.ToInt32(clsID) == -10)
                    return false;
                else
                    return true;
            }
            else
            {
                clsID = modalClsForm.AttachedCls.GetSelectedIDs();
                return true;
            }
        }

        public bool ShowClsModal(string clsKey, int oldClsID, int sourceID, int sourceYear, ref object clsID)
        {
            return ShowClsModal(clsKey, oldClsID, sourceID, sourceYear, ref clsID, true);
        }

		/// <summary>
		/// Показывает модальный справочник, возвращает выбранное ИД
		/// </summary>
		/// <returns></returns>
		public bool ShowClsModal(string clsKey, int oldClsID, int sourceID, ref object clsID)
		{
            return ShowClsModal(clsKey, oldClsID, sourceID, 0, ref clsID);
		}

        public bool ShowClsModal(string clsKey, int oldClsID, int sourceID, int sourceYear, ref object clsID, ref DataTable selectedData)
        {
            if ((_Workplace == null) || (_Workplace.ActiveScheme == null))
            {
                throw new InvalidOperationException("Нет активного подключения");
            }
            // если коллекция загруженных классификаторов еще не создана - создаем
            if (loadedClsObjects == null)
                loadedClsObjects = new Dictionary<string, IInplaceClsView>();

            IInplaceClsView currentViewObject = null;
            // создаем форму
            frmModalTemplate modalClsForm = new frmModalTemplate();
            string modalClassifierCaption = string.Empty;
            // пытаемся найти запрашиваемый классификатор в коллекции
            if (loadedClsObjects.ContainsKey(clsKey))
            {
                // если нашли и прошел некоторый интервал времени с момента последнего его показа - обновляем данные
                currentViewObject = loadedClsObjects[clsKey];
                modalClsForm.AttachCls(currentViewObject);
                if (DataSourceContext.CurrentDataSourceYear != sourceYear)
                {
                    if (sourceID < 0)
                        modalClsForm.AttachedCls.RefreshAttachedData();
                    else
                    {
                        modalClsForm.AttachedCls.RefreshAttachedData(sourceID);
                        currentViewObject.TrySetDataSource(sourceID);
                    }
                }
            }
            else
            {
                // получаем нужный классификатор
                IClassifier cls = _Workplace.ActiveScheme.Classifiers[clsKey];
                // создаем объект просмотра классификаторов нужного типа
                BaseClsUI clsUI = null;
                switch (cls.ClassType)
                {
                    case ClassTypes.clsFixedClassifier:
                        clsUI = new FixedClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Фиксированный классификатор", cls.OlapName);
                        break;
                    case ClassTypes.clsBridgeClassifier:
                        clsUI = new AssociatedClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Сопоставимый классификатор", cls.OlapName);
                        break;
                    case ClassTypes.clsDataClassifier:
                        clsUI = new DataClsUI(cls);
                        modalClassifierCaption = string.Format("{0}: {1}", "Классификатор данных", cls.OlapName);
                        break;
                }
                modalClsForm.FormCaption = modalClassifierCaption;
                // инициализируем его
                clsUI.Workplace = _Workplace;
                clsUI.RestoreDataSet = false;
                clsUI.InInplaceMode = true;
                clsUI.CurrentDataSourceYear = sourceYear;
                clsUI.Initialize();
                clsUI.CurrentDataSourceID = sourceID;
                currentViewObject = (IInplaceClsView) clsUI;
                currentViewObject.InitModalCls(oldClsID);
                modalClsForm.SuspendLayout();
                // и присоединяем к форме
                try
                {
                    modalClsForm.AttachCls(currentViewObject);
                    ComponentCustomizer.CustomizeInfragisticsControls(modalClsForm);
                }
                finally
                {
                    modalClsForm.ResumeLayout();
                }
                // ...загружаем данные
                currentViewObject.RefreshAttachedData();
                // ...помещаем в кэш
                loadedClsObjects.Add(clsKey, currentViewObject);
            }
            modalClsForm.LastShowTickCount = System.Environment.TickCount;
            if (modalClsForm.ShowDialog((Form) _Workplace) != DialogResult.OK)
                return false;
            
            clsID = modalClsForm.AttachedCls.GetSelectedID();
            // если ничего не выбрали - считаем что функция завершилась неудачно
            if (Convert.ToInt32(clsID) == -10)
                return false;
            selectedData = currentViewObject.GetClsDataSet().Tables[0].Clone();
            DataRow activeRow = currentViewObject.GetClsDataSet().Tables[0].Select(string.Format("ID = {0}", clsID))[0];
            selectedData.Rows.Add(activeRow.ItemArray);
            selectedData.AcceptChanges();
            return true;
        }
	}
}
