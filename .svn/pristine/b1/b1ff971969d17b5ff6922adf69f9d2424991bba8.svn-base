using System;
using System.Reflection;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Users
{
    public struct TypesHelper
    {
        public static string GetCaptionForObjectType(SysObjectsTypes tp)
        {
        	return EnumHelper.GetEnumItemDescription(typeof(SysObjectsTypes), tp);
        }

		private static Type GetOperationsEnumForType(SysObjectsTypes tp)
		{
			return AssociateEnumTypeAttribute.GetEnumItemAssociateType(typeof(SysObjectsTypes), tp);
		}

		public static void GetOperationInfo(int operation, ref string operationCaption, ref Type operationType, 
            ref int? parentOperation)
        {
            operationCaption = "Неизвестная операция";
            operationType = null;
            parentOperation = null;

            switch (operation)
            {
                // операции по администрированию
                case (int)AdministrationOperations.PermissionsManagement:
                    operationType = typeof(AdministrationOperations);
                    operationCaption = "Управление правами";
                    break;

                // операции для всех пользовательских интерфейсов
                case (int)AllUIModulesOperations.DisplayAll:
                    operationType = typeof(AllUIModulesOperations);
                    operationCaption = "Отображение всех блоков в интерфейсе";
                    break;

                // операции для пользовательского интерфейса
                case (int)UIModuleOperations.Display:
                    operationType = typeof(UIModuleOperations);
                    parentOperation = (int)AllUIModulesOperations.DisplayAll;
                    operationCaption = "Отображение блока в интерфейсе";
                    break;
                // операции для интерфейса "Классификаторы и таблицы"
                case (int)EntityNavigationListUI.Display:
                    operationType = typeof(EntityNavigationListUI);
                    parentOperation = (int)AllUIModulesOperations.DisplayAll;
                    operationCaption = "Отображение блока в интерфейсе";
                    break;
                // операции для подблоков интерфейса "Классификаторы и таблицы"
                case (int)UIClassifiersSubmoduleOperation.Display:
                    operationType = typeof(UIClassifiersSubmoduleOperation);
                    parentOperation = (int)EntityNavigationListUI.Display;
                    operationCaption = "Отображение блока в интерфейсе";
                    break;

                // операции для всех классификаторов данных
                case (int)AllDataClassifiersOperations.ViewClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Просмотр классификатора";
                    break;
                case (int)AllDataClassifiersOperations.AddRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Добавление записи в классификатор";
                    break;
                case (int)AllDataClassifiersOperations.DelRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Удаление записи классификатора";
                    break;
                case (int)AllDataClassifiersOperations.EditRecord:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Редактирование записи классификатора";
                    break;
                case (int)AllDataClassifiersOperations.AddClassifierForNewDataSource:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Добавление классификатора по новому источнику";
                    break;
                case (int)AllDataClassifiersOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Изменение иерархии в классификаторе";
                    break;
                case (int)AllDataClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Установка иерархии в классификаторе и расщепление кода";
                    break;
                case (int)AllDataClassifiersOperations.ClearClassifierData:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Очистка классификатора";
                    break;
                case (int)AllDataClassifiersOperations.ImportClassifier:
                    operationType = typeof(AllDataClassifiersOperations);
                    operationCaption = "Импорт классификатора";
                    break;

                // операции для классификатора данных
                case (int)DataClassifiesOperations.ViewClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Просмотр классификатора";
                    parentOperation = (int)AllDataClassifiersOperations.ViewClassifier;
                    break;
                case (int)DataClassifiesOperations.AddRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Добавление записи в классификатор";
                    parentOperation = (int)AllDataClassifiersOperations.AddRecord;
                    break;
                case (int)DataClassifiesOperations.DelRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Удаление записи классификатора";
                    parentOperation = (int)AllDataClassifiersOperations.DelRecord;
                    break;
                case (int)DataClassifiesOperations.EditRecord:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Редактирование записи классификатора";
                    parentOperation = (int)AllDataClassifiersOperations.EditRecord;
                    break;
                case (int)DataClassifiesOperations.AddClassifierForNewDataSource:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Добавление классификатора по новому источнику";
                    parentOperation = (int)AllDataClassifiersOperations.AddClassifierForNewDataSource;
                    break;
                case (int)DataClassifiesOperations.ChangeClassifierHierarchy:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Изменение иерархии в классификаторе";
                    parentOperation = (int)AllDataClassifiersOperations.ChangeClassifierHierarchy;
                    break;
                case (int)DataClassifiesOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Установка иерархии в классификаторе и расщепление кода";
                    parentOperation = (int)AllDataClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier;
                    break;
                case (int)DataClassifiesOperations.ClearClassifierData:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Очистка классификатора";
                    parentOperation = (int)AllDataClassifiersOperations.ClearClassifierData;
                    break;
                case (int)DataClassifiesOperations.ImportClassifier:
                    operationType = typeof(DataClassifiesOperations);
                    operationCaption = "Импорт классификатора";
                    parentOperation = (int)AllDataClassifiersOperations.ImportClassifier;
                    break;

                // операции для всех сопоставимых классификаторов
                case (int)AllAssociatedClassifiersOperations.ViewClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Просмотр классификатора";
                    break;
                case (int)AllAssociatedClassifiersOperations.AddRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Добавление записи в классификатор";
                    break;
                case (int)AllAssociatedClassifiersOperations.DelRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Удаление записи классификатора";
                    break;
                case (int)AllAssociatedClassifiersOperations.EditRecord:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Редактирование записи классификатора";
                    break;
                case (int)AllAssociatedClassifiersOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Изменение иерархии в классификаторе";
                    break;
                case (int)AllAssociatedClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Установка иерархии в классификаторе и расщепление кода";
                    break;
                case (int)AllAssociatedClassifiersOperations.ClearClassifierData:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Очистка классификатора";
                    break;
                case (int)AllAssociatedClassifiersOperations.ImportClassifier:
                    operationType = typeof(AllAssociatedClassifiersOperations);
                    operationCaption = "Импорт классификатора";
                    break;

                // операции для сопоставимого классификатора
                case (int)AssociatedClassifierOperations.ViewClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Просмотр классификатора";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ViewClassifier;
                    break;
                case (int)AssociatedClassifierOperations.AddRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Добавление записи в классификатор";
                    parentOperation = (int)AllAssociatedClassifiersOperations.AddRecord;
                    break;
                case (int)AssociatedClassifierOperations.DelRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Удаление записи классификатора";
                    parentOperation = (int)AllAssociatedClassifiersOperations.DelRecord;
                    break;
                case (int)AssociatedClassifierOperations.EditRecord:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Редактирование записи классификатора";
                    parentOperation = (int)AllAssociatedClassifiersOperations.EditRecord;
                    break;
                case (int)AssociatedClassifierOperations.ChangeClassifierHierarchy:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Изменение иерархии в классификаторе";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ChangeClassifierHierarchy;
                    break;
                case (int)AssociatedClassifierOperations.SetHierarchyAndCodeDisintegrationForClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Установка иерархии в классификаторе и расщепление кода";
                    parentOperation = (int)AllAssociatedClassifiersOperations.SetHierarchyAndCodeDisintegrationForClassifier;
                    break;
                case (int)AssociatedClassifierOperations.ClearClassifierData:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Очистка классификатора";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ClearClassifierData;
                    break;
                case (int)AssociatedClassifierOperations.ImportClassifier:
                    operationType = typeof(AssociatedClassifierOperations);
                    operationCaption = "Импорт классификатора";
                    parentOperation = (int)AllAssociatedClassifiersOperations.ImportClassifier;
                    break;

                // операции для всех таблиц фактов
                case (int)AllFactTablesOperations.ViewClassifier:
                    operationType = typeof(AllFactTablesOperations);
                    operationCaption = "Просмотр данных";
                    break;
                case (int)AllFactTablesOperations.EditRecord:
                    operationType = typeof(AllFactTablesOperations);
                    operationCaption = "Редактирование данных";
                    break;

                // операции для таблицы фактов
                case (int)FactTableOperations.ViewClassifier:
                    operationType = typeof(FactTableOperations);
                    operationCaption = "Просмотр данных";
                    parentOperation = (int)AllFactTablesOperations.ViewClassifier;
                    break;
                case (int)FactTableOperations.EditRecord:
                    operationType = typeof(FactTableOperations);
                    operationCaption = "Редактирование данных";
                    parentOperation = (int)AllFactTablesOperations.EditRecord;
                    break;

                // операции для сопоставления всех клакссифкаторов
                case (int)AssociateForAllClassifiersOperations.Associate:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "Сопоставление";
                    break;
                case (int)AssociateForAllClassifiersOperations.ClearAssociate:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "Очистка сопоставления";
                    break;
                case (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "Добавление записи в таблицу перекодировки";
                    break;
                case (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable:
                    operationType = typeof(AssociateForAllClassifiersOperations);
                    operationCaption = "Удаление записи из таблицы перекодировки";
                    break;

                // операции для сопоставления
                case (int)AssociateOperations.Associate:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "Сопоставление";
                    parentOperation = (int)AssociateForAllClassifiersOperations.Associate;
                    break;
                case (int)AssociateOperations.ClearAssociate:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "Очистка сопоставления";
                    parentOperation = (int)AssociateForAllClassifiersOperations.ClearAssociate;
                    break;
                case (int)AssociateOperations.AddRecordIntoBridgeTable:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "Добавление записи в таблицу перекодировки";
                    parentOperation = (int)AssociateForAllClassifiersOperations.AddRecordIntoBridgeTable;
                    break;
                case (int)AssociateOperations.DelRecordFromBridgeTable:
                    operationType = typeof(AssociateOperations);
                    operationCaption = "Удаление записи из таблицы перекодировки";
                    parentOperation = (int)AssociateForAllClassifiersOperations.DelRecordFromBridgeTable;
                    break;

                // операции для всех источников данных
                case (int)AllDataSourcesOperation.AddDataSource:
                    operationType = typeof(AllDataSourcesOperation);
                    operationCaption = "Добавление источника данных";
                    break;
                case (int)AllDataSourcesOperation.DelDataSource:
                    operationType = typeof (AllDataSourcesOperation);
                    operationCaption = "Удаление источника данных";
                    break;

                // операции для всех закачек данных
                case (int)AllDataPumpsOperations.StartPump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "Запуск закачки данных";
                    break;
                case (int)AllDataPumpsOperations.StopPump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "Остановка закачки данных";
                    break;
                case (int)AllDataPumpsOperations.DeletePump:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "Удаление закачки данных";
                    break;
                case (int)AllDataPumpsOperations.PreviewPumpData:
                    operationType = typeof(AllDataPumpsOperations);
                    operationCaption = "Просмотр данных закачки";
                    break;

                // операции для закачки данных
                case (int)DataPumpOperations.StartPump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "Запуск закачки данных";
                    parentOperation = (int)AllDataPumpsOperations.StartPump;
                    break;
                case (int)DataPumpOperations.StopPump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "Остановка закачки данных";
                    parentOperation = (int)AllDataPumpsOperations.StopPump;
                    break;
                case (int)DataPumpOperations.DeletePump:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "Удаление закачки данных";
                    parentOperation = (int)AllDataPumpsOperations.DeletePump;
                    break;
                case (int)DataPumpOperations.PreviewPumpData:
                    operationType = typeof(DataPumpOperations);
                    operationCaption = "Просмотр данных закачки";
                    break;

                // операции для листа планирования
                case (int)PlanningSheetOperations.Construction:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "Конструирование";
                    break;
                case (int)PlanningSheetOperations.ChangeFilters:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "Изменение фильтров";
                    break;
                case (int)PlanningSheetOperations.UpdateData:
                    operationType = typeof(PlanningSheetOperations);
                    operationCaption = "Обновление данных";
                    break;

                // операции для всех задач
                case (int)AllTasksOperations.CreateTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Создание задач";
                    break;
                case (int)AllTasksOperations.ChangeTaskHierarchyOrder:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Перемещение задач по иерархии";
                    break;
                case (int)AllTasksOperations.DelTaskAction:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Действие: удалить";
                    break;
                case (int)AllTasksOperations.EditTaskAction:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Действие: редактировать";
                    break;
                case (int)AllTasksOperations.ViewAllUsersTasks:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Просмотр задач всех пользователей";
                    break;
                case (int)AllTasksOperations.AssignTaskViewPermission:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Распределение прав на просмотр задач";
                    break;
                case (int)AllTasksOperations.CanCancelEdit:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Снять блокировку";
                    break;
                case (int)AllTasksOperations.ExportTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Экспорт задач";
                    break;
                case (int)AllTasksOperations.ImportTask:
                    operationType = typeof(AllTasksOperations);
                    operationCaption = "Импорт задач";
                    break;

                // операции для типа задачи
                case (int)TaskTypeOperations.CreateTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Создание задачи";
                    parentOperation = (int)AllTasksOperations.CreateTask;
                    break;
                case (int)TaskTypeOperations.ChangeTaskHierarchyOrder:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Перемещение задач по иерархии";
                    parentOperation = (int)AllTasksOperations.ChangeTaskHierarchyOrder;
                    break;
                case (int)TaskTypeOperations.DelTaskAction:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Действие: удалить";
                    parentOperation = (int)AllTasksOperations.DelTaskAction;
                    break;
                case (int)TaskTypeOperations.EditTaskAction:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Действие: редактировать";
                    parentOperation = (int)AllTasksOperations.EditTaskAction;
                    break;
                case (int)TaskTypeOperations.ViewAllUsersTasks:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Просмотр задач всех пользователей";
                    parentOperation = (int)AllTasksOperations.ViewAllUsersTasks;
                    break;
                case (int)TaskTypeOperations.AssignTaskViewPermission:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Распределение прав на задачи";
                    parentOperation = (int)AllTasksOperations.AssignTaskViewPermission;
                    break;
                case (int)TaskTypeOperations.CanCancelEdit:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Снять блокировку";
                    break;
                    /*
                case (int)TaskTypeOperations.ExportTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Экспорт задачи";
                    parentOperation = (int)AllTasksOperations.ExportTask;
                    break;
                case (int)TaskTypeOperations.ImportTask:
                    operationType = typeof(TaskTypeOperations);
                    operationCaption = "Импорт задачи";
                    parentOperation = (int)AllTasksOperations.ImportTask;
                    break;
                    */

                // операции для документа задачи
                case (int)TaskDocumentOperations.View:
                    operationType = typeof(TaskDocumentOperations);
                    operationCaption = "Просмотр";
                    break;

                // операции для типа задачи
                case (int)TaskOperations.View:
                    operationType = typeof(TaskOperations);
                    operationCaption = "Просмотр задачи";
                    break;

                case (int)TaskOperations.Perform:
                    operationType = typeof(TaskOperations);
                    operationCaption = "Выполнение задачи";
                    break;

                // операции для произвольных отчетов
                case (int)WebReportsOperations.View:
                    operationType = typeof(WebReportsOperations);
                    operationCaption = "Просмотр отчетов";
                    break;

				// операции для всех шаблонов
				case (int)AllTemplatesOperations.CreateTemplate:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Создание шаблонов";
					break;
				case (int)AllTemplatesOperations.ChangeTemplateHierarchyOrder:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Перемещение шаблонов по иерархии";
					break;
				case (int)AllTemplatesOperations.EditTemplateAction:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Действие: редактировать";
					break;
				case (int)AllTemplatesOperations.ViewAllUsersTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Просмотр шаблонов всех пользователей";
					break;
				case (int)AllTemplatesOperations.AssignTemplateViewPermission:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Распределение прав на просмотр шаблонов";
					break;
				case (int)AllTemplatesOperations.ExportTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Экспорт шаблонов";
					break;
				case (int)AllTemplatesOperations.ImportTemplates:
					operationType = typeof(AllTemplatesOperations);
					operationCaption = "Импорт шаблонов";
					break;

				// операции для вида шаблона
				case (int)TemplateTypeOperations.CreateTemplate:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "Создание шаблона";
					parentOperation = (int)AllTemplatesOperations.CreateTemplate;
					break;
				case (int)TemplateTypeOperations.ChangeTemplateHierarchyOrder:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "Перемещение шаблона по иерархии";
					parentOperation = (int)AllTemplatesOperations.ChangeTemplateHierarchyOrder;
					break;
				case (int)TemplateTypeOperations.EditTemplateAction:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "Действие: редактировать";
					parentOperation = (int)AllTemplatesOperations.EditTemplateAction;
					break;
				case (int)TemplateTypeOperations.ViewAllUsersTemplates:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "Просмотр шаблонов всех пользователей";
					parentOperation = (int)AllTemplatesOperations.ViewAllUsersTemplates;
					break;
				case (int)TemplateTypeOperations.AssignTemplateViewPermission:
					operationType = typeof(TemplateTypeOperations);
					operationCaption = "Распределение прав на просмотр шаблона";
					parentOperation = (int)AllTemplatesOperations.AssignTemplateViewPermission;
					break;

				// операции для шаблона
				case (int)TemplateOperations.ViewTemplate:
					operationType = typeof(TemplateOperations);
					operationCaption = "Просмотр шаблона";
					break;
				case (int)TemplateOperations.EditTemplateAction:
					operationType = typeof(TemplateOperations);
					operationCaption = "Редактирование шаблона";
					break;

				//опреации для прогноза
				case (Int32)ForecastOperations.ViewData:
					operationType = typeof(ForecastOperations);
					operationCaption = "Просмотр данных";
					break;
				case (Int32)ForecastOperations.AssignParam:
					operationType = typeof(ForecastOperations);
					operationCaption = "Распределение параметров эксператам";
					break;
				case (Int32)ForecastOperations.Calculate:
					operationType = typeof(ForecastOperations);
					operationCaption = "Расчет";
					break;
				case (Int32)ForecastOperations.CreateNew:
					operationType = typeof(ForecastOperations);
					operationCaption = "Создание новых расчетов";
					break;
				case (Int32)ForecastOperations.AllowEdit:
					operationType = typeof(ForecastOperations);
					operationCaption = "Правка сценариев";
					break;

				case (Int32)ScenForecastOperations.AssignParam: //для сценариев и вариантов
					operationType = typeof(ScenForecastOperations);
					operationCaption = "Распределение параметров эксператам";
					break;
				case (Int32)ScenForecastOperations.Calculate:  //для сценариев и вариантов
					operationType = typeof(ScenForecastOperations);
					operationCaption = "Расчет";
					break;
				case (Int32)ScenForecastOperations.CreateNew:  //для сценариев и вариантов
					operationType = typeof(ScenForecastOperations);
					operationCaption = "Создание новых расчетов";
					break;

				case (Int32)Form2pForecastOperations.Calculate:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "Расчет";
					break;
				case (Int32)Form2pForecastOperations.CreateNew:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "Создание новых расчетов";
					break;
				case (Int32)Form2pForecastOperations.ViewData:
					operationType = typeof(Form2pForecastOperations);
					operationCaption = "Просмотр данных";
					break;

                // права для источников финансирования
                case (Int32)FinSourcePlaningOperations.View:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Отображение раздела источников финансирования";
                    break;
                case (Int32)FinSourcePlaningOperations.AddRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Добавление записей";
                    break;
                case (Int32)FinSourcePlaningOperations.DelRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Удаление записей";
                    break;
                case (Int32)FinSourcePlaningOperations.EditRecord:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Редактирование записей";
                    break;
                case (Int32)FinSourcePlaningOperations.ImportData:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Импорт данных";
                    break;
                case (Int32)FinSourcePlaningOperations.ClearData:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Очистка данных";
                    break;
                case (Int32)FinSourcePlaningOperations.Calculate:
                    operationType = typeof(FinSourcePlaningOperations);
                    operationCaption = "Расчет данных";
                    break;

                // права для источников финансирования на просмотр отдельных частей
                case (Int32)FinSourcePlaningUIModuleOperations.View:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Отображение раздела источников финансирования";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.AddRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Добавление записей";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.DelRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Удаление записей";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.EditRecord:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Редактирование записей";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.ImportData:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Импорт данных";
                    break;
                case (Int32)FinSourcePlaningUIModuleOperations.ClearData:
                    operationType = typeof(FinSourcePlaningUIModuleOperations);
                    operationCaption = "Очистка данных";
                    break;

                // права для источников финансирования для расчетов
                case (Int32)FinSourcePlaningCalculateUIModuleOperations.View:
                    operationType = typeof(FinSourcePlaningCalculateUIModuleOperations);
                    operationCaption = "Отображение данных";
                    break;
                case (Int32)FinSourcePlaningCalculateUIModuleOperations.Calculate:
                    operationType = typeof(FinSourcePlaningCalculateUIModuleOperations);
                    operationCaption = "Расчет данных";
                    break;

                 // сообщения системы
                case (int)AllMessageOperations.Subscribe:
                    operationType = typeof (AllMessageOperations);
                    operationCaption = "Подписаться на сообщения от системы";
                    break;
                // сообщения подсистемысистемы
                case (int)MessageOperations.Subscribe:
                    operationType = typeof(MessageOperations);
                    operationCaption = "Подписаться на сообщения от системы";
                    break;
                // права планирования доходов
                case (int) IncomesPlaningOperations.ViewPlaningOperations:
                    operationType = typeof(IncomesPlaningOperations);
                    operationCaption = "Отображение всех разделов интерфейса планирования доходов";
                    break;
                case (int) IncomesPlaningModuleOperations.ViewPlaningOperationsModule:
                    operationType = typeof(IncomesPlaningModuleOperations);
                    operationCaption = "Отображение раздела интерфейса планирования доходов";
                    break;
			}
        }

        public static string[] GetOperationsForType(SysObjectsTypes tp)
        {
            Type operationsEnumType = GetOperationsEnumForType(tp);
            string[] result = null;
            if (operationsEnumType != null)
            {
                FieldInfo[] fiArr = operationsEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                result = new string[fiArr.Length];
                for (int i = 0; i < fiArr.Length; i ++ )
                {
                    FieldInfo fi = fiArr[i];
                    int intVal = Convert.ToInt32(fi.GetValue(null));
                    result[i] = Convert.ToString(intVal);
                }
            }
            return result;
        }

        public static int[] GetOperationsForTypeInt(SysObjectsTypes tp)
        {
            Type operationsEnumType = GetOperationsEnumForType(tp);
            int[] result = null;
            if (operationsEnumType != null)
            {
                FieldInfo[] fiArr = operationsEnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
                result = new int[fiArr.Length];
                for (int i = 0; i < fiArr.Length; i++)
                {
                    result[i] = Convert.ToInt32(fiArr[i].GetValue(null));
                }
            }
            return result;
        }

        public static int? GetParentTypeInt(int tp)
        {
            switch ((SysObjectsTypes)tp)
            {
                case SysObjectsTypes.Unknown:
                    return null;
                case SysObjectsTypes.Administration:
                    return null;
                case SysObjectsTypes.AllUIModules:
                    return null;
                case SysObjectsTypes.UIModule:
                    return (int)SysObjectsTypes.AllUIModules;
                case SysObjectsTypes.EntityNavigationListUI:
                    return (int)SysObjectsTypes.AllUIModules;
                case SysObjectsTypes.UIClassifiersSubmodule:
                    return (Int32)SysObjectsTypes.EntityNavigationListUI;
                case SysObjectsTypes.AllDataClassifiers:
                    return null;
                case SysObjectsTypes.DataClassifier:
                    return (int)SysObjectsTypes.AllDataClassifiers;
                case SysObjectsTypes.AllAssociatedClassifiers:
                    return null;
                case SysObjectsTypes.AssociatedClassifier:
                    return (int)SysObjectsTypes.AllAssociatedClassifiers;
                case SysObjectsTypes.AllFactTables:
                    return null;
                case SysObjectsTypes.FactTable:
                    return (int)SysObjectsTypes.AllFactTables;
                case SysObjectsTypes.AssociateForAllClassifiers:
                    return null;
                case SysObjectsTypes.Associate:
                    return (int)SysObjectsTypes.AssociateForAllClassifiers;
                case SysObjectsTypes.AllDataSources:
                    return null;
                case SysObjectsTypes.AllDataPumps:
                    return null;
                case SysObjectsTypes.DataPump:
                    return (int)SysObjectsTypes.AllDataPumps;
                case SysObjectsTypes.PlanningSheet:
                    return null;
                case SysObjectsTypes.AllTasks:
                    return null;
                case SysObjectsTypes.TaskDocument:
                    return (int)SysObjectsTypes.AllTasks;
                case SysObjectsTypes.Task:
                    return (int)SysObjectsTypes.TaskType;
                case SysObjectsTypes.TaskType:
                    return (int)SysObjectsTypes.AllTasks;
				case SysObjectsTypes.AllTemplates:
					return null;
				case SysObjectsTypes.TemplateType:
					return (int)SysObjectsTypes.AllTemplates;
				case SysObjectsTypes.Template:
					return (int)SysObjectsTypes.TemplateType;

				///прогноз развития региона
				case SysObjectsTypes.AllForecast:
					return null;
				case SysObjectsTypes.ScenarioForecast:
					return (Int32)SysObjectsTypes.AllForecast;
				case SysObjectsTypes.Form2pForecast:
					return null;
				// источники финансирования
				case SysObjectsTypes.FinSourcePlaningUIModule:
                    return (Int32)SysObjectsTypes.FinSourcePlaning;
                case SysObjectsTypes.FinSourcePlaningCalculateUIModule:
                    return (Int32)SysObjectsTypes.FinSourcePlaning;
                // message
                case SysObjectsTypes.Message:
                    return (int) SysObjectsTypes.AllMessages;
                // планирование доходов
                case SysObjectsTypes.IncomesPlaningModule:
                    return (int)SysObjectsTypes.IncomesPlaning;
				default:
                    return null;

            }
        }
    }
}