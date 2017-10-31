using System;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

namespace Krista.FM.ServerLibrary
{
    public enum AuthenticationType : int
    {
        atUndefined = 0,
        atWindows = 1,
        adPwdSHA512 = 2
    };

    public sealed class FixedUsers
    {
        public enum FixedUsersIds : int
        {
            UserNotDefined = 0, // пользователь не определен
            InstallAdmin = 1,   // фиксированный администратор
            System = 2,         // System 
            DataPump = 3        // для закачек
        }

        public static bool UserIsFixed(int userID)
        {
            return (userID < MaxFixedUserID);
        }

        public const int MaxFixedUserID = 100;

        public static string InstallAdminFilter = String.Format("(ID = {0}) or (ID >= {1})", (int)FixedUsersIds.InstallAdmin, MaxFixedUserID);
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class AssociateEnumTypeAttribute : Attribute
    {
        // Fields
        public static readonly AssociateEnumTypeAttribute Default = new AssociateEnumTypeAttribute(typeof(AdministrationOperations));
        private Type type;

        public AssociateEnumTypeAttribute(Type type)
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            AssociateEnumTypeAttribute attribute = obj as AssociateEnumTypeAttribute;
            if (attribute != null)
            {
                return (attribute.Type == this.Type);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return this.Equals(Default);
        }

        // Properties
        public virtual Type Type
        {
            get { return TypeValue; }
        }

        protected Type TypeValue
        {
            get { return type; }
            set { type = value; }
        }

        public static Type GetEnumItemAssociateType(Type type, object value)
        {
            FieldInfo fi = type.GetField(Enum.GetName(type, value));
            AssociateEnumTypeAttribute da = (AssociateEnumTypeAttribute)Attribute.GetCustomAttribute(
                fi, typeof(AssociateEnumTypeAttribute));
            return da.Type;
        }
    }



    /// <summary>
    /// Типы объектов.
    /// </summary>
    public enum SysObjectsTypes : int
    {
        /// <summary>
        /// Неизвестный тип
        /// </summary>
        [Description("Неизвестный тип объекта")]
        Unknown = 0,

        /// <summary>
        /// Администрирование
        /// </summary>
        [Description("Администрирование")]
        [AssociateEnumType(typeof(AdministrationOperations))]
        Administration = 1000,

        /// <summary>
        /// Все блоки интерфейса системы
        /// </summary>
        [Description("Все блоки интерфейса системы")]
        [AssociateEnumType(typeof(AllUIModulesOperations))]
        AllUIModules = 2000,

        /// <summary>
        /// Блок интерфейса системы
        /// </summary>
        [Description("Блок интерфейса системы")]
        [AssociateEnumType(typeof(UIModuleOperations))]
        UIModule = 3000,

        /// <summary>
        /// Блок интерфейса системы "Классификаторы и таблицы"
        /// </summary>
        [Description("Блок интерфейса системы")]
        [AssociateEnumType(typeof(EntityNavigationListUI))]
        EntityNavigationListUI = 3500,

        /// <summary>
        /// Все классификаторы данных
        /// </summary>
        [Description("Все классификаторы данных")]
        [AssociateEnumType(typeof(AllDataClassifiersOperations))]
        AllDataClassifiers = 4000,

        /// <summary>
        /// Классификатор данных
        /// </summary>
        [Description("Классификатор данных")]
        [AssociateEnumType(typeof(DataClassifiesOperations))]
        DataClassifier = 5000,

        /// <summary>
        /// Все сопоставимые классификаторы
        /// </summary>
        [Description("Все сопоставимые классификаторы")]
        [AssociateEnumType(typeof(AllAssociatedClassifiersOperations))]
        AllAssociatedClassifiers = 6000,

        /// <summary>
        /// Сопоставимый классификатор
        /// </summary>
        [Description("Сопоставимый классификатор")]
        [AssociateEnumType(typeof(AssociatedClassifierOperations))]
        AssociatedClassifier = 7000,

        /// <summary>
        /// Все таблицы фактов
        /// </summary>
        [Description("Все таблицы фактов")]
        [AssociateEnumType(typeof(AllFactTablesOperations))]
        AllFactTables = 8000,

        /// <summary>
        /// Таблица фактов
        /// </summary>
        [Description("Таблица фактов")]
        [AssociateEnumType(typeof(FactTableOperations))]
        FactTable = 9000,

        /// <summary>
        /// Сопоставление всех классификаторов
        /// </summary>
        [Description("Сопоставление всех классификаторов")]
        [AssociateEnumType(typeof(AssociateForAllClassifiersOperations))]
        AssociateForAllClassifiers = 10000,

        /// <summary>
        /// Сопоставление
        /// </summary>
        [Description("Сопоставление")]
        [AssociateEnumType(typeof(AssociateOperations))]
        Associate = 11000,

        /// <summary>
        /// Источники данных
        /// </summary>
        [Description("Источники данных")]
        [AssociateEnumType(typeof(AllDataSourcesOperation))]
        AllDataSources = 12000,

        /// <summary>
        /// Все закачки данных
        /// </summary>
        [Description("Все закачки данных")]
        [AssociateEnumType(typeof(AllDataPumpsOperations))]
        AllDataPumps = 13000,

        /// <summary>
        /// Закачка данных (это для конкретных видов закачек)
        /// </summary>
        [Description("Закачка данных")]
        [AssociateEnumType(typeof(DataPumpOperations))]
        DataPump = 14000,

        /// <summary>
        /// Листы планирования
        /// </summary>
        [Description("Листы планирования")]
        [AssociateEnumType(typeof(PlanningSheetOperations))]
        PlanningSheet = 15000,

        /// <summary>
        /// Все задачи
        /// </summary>
        [Description("Все задачи")]
        [AssociateEnumType(typeof(AllTasksOperations))]
        AllTasks = 16000,

        /// <summary>
        /// Документ задачи
        /// </summary>
        [Description("Документ задачи")]
        [AssociateEnumType(typeof(TaskDocumentOperations))]
        TaskDocument = 17000,

        /// <summary>
        /// Обычная задача (это наша теперешняя задача, просто это пока один тип задач из тех которые будут вообще)
        /// </summary>
        [Description("Задача")]
        [AssociateEnumType(typeof(TaskOperations))]
        Task = 18000,

        /// <summary>
        /// Тип задачи
        /// </summary>
        [Description("Вид задачи")]
        [AssociateEnumType(typeof(TaskTypeOperations))]
        TaskType = 19000,

        /// <summary>
        /// Произвольные отчеты
        /// </summary>
        [Description("Web-интерфейс")]
        [AssociateEnumType(typeof(WebReportsOperations))]
        WebReports = 20000,

        /// <summary>
        /// Все шаблоны репозитория отчетов.
        /// </summary>
        [Description("Все шаблоны репозитория отчетов")]
        [AssociateEnumType(typeof(AllTemplatesOperations))]
        AllTemplates = 21000,

        /// <summary>
        /// Вид шаблона репозитория отчетов.
        /// </summary>
        [Description("Вид шаблона репозитория отчетов")]
        [AssociateEnumType(typeof(TemplateTypeOperations))]
        TemplateType = 22000,

        /// <summary>
        /// Шаблон репозитория отчетов.
        /// </summary>
        [Description("Шаблон репозитория отчетов")]
        [AssociateEnumType(typeof(TemplateOperations))]
        Template = 23000,

        /// <summary>
        /// Прогноз развития региона. Все задачи
        /// </summary>
        [Description("Прогноз развития региона")]
        [AssociateEnumType(typeof(ForecastOperations))]
        AllForecast = 24000,

        /// <summary>
        /// Прогноз развития региона. Сценарий. Вариант расчета
        /// </summary>
        [Description("Сценарий. Вариант.")]
        [AssociateEnumType(typeof(ScenForecastOperations))]
        ScenarioForecast = 25000,

        /// <summary>
        /// Прогноз развития региона. Форма 2п
        /// </summary>
        [Description("Форма 2п")]
        [AssociateEnumType(typeof(Form2pForecastOperations))]
        Form2pForecast = 26000,

        /// <summary>
        /// Источники финансирования
        /// </summary>
        [Description("Источники финансирования")]
        [AssociateEnumType(typeof(FinSourcePlaningOperations))]
        FinSourcePlaning = 27000,

        /// <summary>
        /// Блок интерфейса системы
        /// </summary>
        [Description("Источники финансирования. Разделы интерфейса ")]
        [AssociateEnumType(typeof(FinSourcePlaningUIModuleOperations))]
        FinSourcePlaningUIModule = 28000,

        /// <summary>
        /// Блок интерфейса системы
        /// </summary>
        [Description("Источники финансирования. Расчеты")]
        [AssociateEnumType(typeof(FinSourcePlaningCalculateUIModuleOperations))]
        FinSourcePlaningCalculateUIModule = 29000,

        /// <summary>
        /// Блок интерфейса системы
        /// </summary>
        [Description("Подблок блока интерфейса системы 'Классификаторы и таблицы'")]
        [AssociateEnumType(typeof(UIClassifiersSubmoduleOperation))]
        UIClassifiersSubmodule = 30000,

        /// <summary>
        /// Блок интерфейса системы
        /// </summary>
        [Description("Интерфейс системы 'Прогноз консолидированного бюджета'")]
        [AssociateEnumType(typeof(UIClassifiersSubmoduleOperation))]
        UIConsBudgetForecast = 31000,

        /// <summary>
        /// Подписка на все уведомления
        /// </summary>
        [Description("Подписка на все сообщения")]
        [AssociateEnumType(typeof(AllMessageOperations))]
        AllMessages = 32000,

        [Description("Сообщение от подсистемы")]
        [AssociateEnumType(typeof(MessageOperations))]
        Message = 33000,

        [Description("Интерфейс планирование доходов")]
        [AssociateEnumType(typeof(IncomesPlaningOperations))]
        IncomesPlaning = 34000,

        [Description("Интерфейс планирования доходов. Разделы интерфейса")]
        [AssociateEnumType(typeof(IncomesPlaningModuleOperations))]
        IncomesPlaningModule = 35000,

        [Description("Все кубы")]
        [AssociateEnumType(typeof(AllCubesOperations))]
        AllCubes = 36000,

        [Description("Куб")]
        [AssociateEnumType(typeof(CubeOperations))]
        Cube = 37000
    }

    internal struct OperationsBase
    {
        public const int Administration = (int)SysObjectsTypes.Administration;
        public const int AllUIModules = (int)SysObjectsTypes.AllUIModules;
        public const int UIModule = (int)SysObjectsTypes.UIModule;
        public const int EntityNavigationListUI = (int)SysObjectsTypes.EntityNavigationListUI;
        public const int UIClassifiersSubmodule = (int)SysObjectsTypes.UIClassifiersSubmodule;
        public const int AllDataClassifiers = (int)SysObjectsTypes.AllDataClassifiers;
        public const int DataClassifier = (int)SysObjectsTypes.DataClassifier;
        public const int AllAssociatedClassifiers = (int)SysObjectsTypes.AllAssociatedClassifiers;
        public const int AssociatedClassifier = (int)SysObjectsTypes.AssociatedClassifier;
        public const int AllFactTables = (int)SysObjectsTypes.AllFactTables;
        public const int FactTable = (int)SysObjectsTypes.FactTable;
        public const int AssociateForAllClassifiers = (int)SysObjectsTypes.AssociateForAllClassifiers;
        public const int Associate = (int)SysObjectsTypes.Associate;
        public const int AllDataSources = (int)SysObjectsTypes.AllDataSources;
        public const int AllDataPumps = (int)SysObjectsTypes.AllDataPumps;
        public const int DataPump = (int)SysObjectsTypes.DataPump;
        public const int PlanningSheet = (int)SysObjectsTypes.PlanningSheet;
        public const int AllTasks = (int)SysObjectsTypes.AllTasks;
        public const int TaskDocument = (int)SysObjectsTypes.TaskDocument;
        public const int Task = (int)SysObjectsTypes.Task;
        public const int TaskType = (int)SysObjectsTypes.TaskType;
        public const int WebReports = (int)SysObjectsTypes.WebReports;
        public const int FinSourcePlaning = (int)SysObjectsTypes.FinSourcePlaning;
        public const int FinSourcePlaningUIModule = (int)SysObjectsTypes.FinSourcePlaningUIModule;
        public const int FinSourcePlaningCalculateUIModule = (int)SysObjectsTypes.FinSourcePlaningCalculateUIModule;
        public const int AllEvents = (int)SysObjectsTypes.AllMessages;
        public const int IncomesPlaning = (int) SysObjectsTypes.IncomesPlaning;
        public const int IncomesPlaningModule = (int)SysObjectsTypes.IncomesPlaningModule;
        public const int AllCubes = (int) SysObjectsTypes.AllCubes;
        public const int Cube = (int) SysObjectsTypes.Cube;
    }

    // операции по администрированию
    public enum AdministrationOperations : int
    {
        // Управление правами
        PermissionsManagement = OperationsBase.Administration + 1
    }

    // операции для всех пользовательских интерфейсов
    public enum AllUIModulesOperations : int
    {
        //Отображение всех блоков в интерфейсе
        DisplayAll = OperationsBase.AllUIModules + 1
    }

    // операции для пользовательского интерфейса
    public enum UIModuleOperations : int
    {
        //Отображение блока в интерфейсе
        Display = OperationsBase.UIModule + 1
    }

    // операции для пользовательского интерфейса
    public enum EntityNavigationListUI : int
    {
        //Отображение блока в интерфейсе
        Display = OperationsBase.EntityNavigationListUI + 1
    }

    public enum UIClassifiersSubmoduleOperation : int
    {
        Display = OperationsBase.UIClassifiersSubmodule + 1
    }

    // операции для всех классификаторов
    public enum AllClassifiersOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // Добавление классификатора по новому источнику
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // Импорт классификатора
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // операции для одного классификатора
    public enum ClassifierOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // Добавление классификатора по новому источнику
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // Импорт классификатора
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // операции для всех классификаторов данных
    public enum AllDataClassifiersOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // Добавление классификатора по новому источнику
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // Импорт классификатора
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // операции для классификатора данных
    public enum DataClassifiesOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.DataClassifier + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.DataClassifier + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.DataClassifier + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.DataClassifier + 4,
        // Добавление классификатора по новому источнику
        AddClassifierForNewDataSource = OperationsBase.DataClassifier + 5,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.DataClassifier + 6,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.DataClassifier + 7,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.DataClassifier + 8,
        // Импорт классификатора
        ImportClassifier = OperationsBase.DataClassifier + 9
    }

    // операции для всех сопоставимых классификаторов
    public enum AllAssociatedClassifiersOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.AllAssociatedClassifiers + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.AllAssociatedClassifiers + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.AllAssociatedClassifiers + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.AllAssociatedClassifiers + 4,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.AllAssociatedClassifiers + 5,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllAssociatedClassifiers + 6,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.AllAssociatedClassifiers + 7,
        // Импорт классификатора
        ImportClassifier = OperationsBase.AllAssociatedClassifiers + 8
    }

    // операции для сопоставимого классификатора
    public enum AssociatedClassifierOperations : int
    {
        // Просмотр классификатора
        ViewClassifier = OperationsBase.AssociatedClassifier + 1,
        // Добавление записи в классификатор
        AddRecord = OperationsBase.AssociatedClassifier + 2,
        // Удаление записи классификатора
        DelRecord = OperationsBase.AssociatedClassifier + 3,
        // Редактирование записи классификатора
        EditRecord = OperationsBase.AssociatedClassifier + 4,
        // Изменение иерархии в классификаторе
        ChangeClassifierHierarchy = OperationsBase.AssociatedClassifier + 5,
        // Установка иерархии в классификаторе и расщепление кода
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AssociatedClassifier + 6,
        // Очистка классификатора
        ClearClassifierData = OperationsBase.AssociatedClassifier + 7,
        // Импорт классификатора
        ImportClassifier = OperationsBase.AssociatedClassifier + 8
    }

    // операции для всех таблиц фактов
    public enum AllFactTablesOperations : int
    {
        // Просмотр данных
        ViewClassifier = OperationsBase.AllFactTables + 1,
        // Редактирование данных
        EditRecord = OperationsBase.AllFactTables + 2
    }

    // операции для таблицы фактов
    public enum FactTableOperations : int
    {
        // Просмотр данных
        ViewClassifier = OperationsBase.FactTable + 1,
        // Редактирование данных
        EditRecord = OperationsBase.FactTable + 2
    }

    // операции для сопоставления всех клакссифкаторов
    public enum AssociateForAllClassifiersOperations : int
    {
        // Сопоставление
        Associate = OperationsBase.AssociateForAllClassifiers + 1,
        // Очистка сопоставления
        ClearAssociate = OperationsBase.AssociateForAllClassifiers + 2,
        // Добавление записи в таблицу перекодировки
        AddRecordIntoBridgeTable = OperationsBase.AssociateForAllClassifiers + 3,
        // Удаление записи из таблицы перекодировки
        DelRecordFromBridgeTable = OperationsBase.AssociateForAllClassifiers + 4
    }

    // операции для сопоставления
    public enum AssociateOperations : int
    {
        // Сопоставление
        Associate = OperationsBase.Associate + 1,
        // Очистка сопоставления
        ClearAssociate = OperationsBase.Associate + 2,
        // Добавление записи в таблицу перекодировки
        AddRecordIntoBridgeTable = OperationsBase.Associate + 3,
        // Удаление записи из таблицы перекодировки
        DelRecordFromBridgeTable = OperationsBase.Associate + 4
    }

    // операции для всех источников данных
    public enum AllDataSourcesOperation : int
    {
        // Добавление источника данных
        AddDataSource = OperationsBase.AllDataSources + 1,
        // Удаление источника данных
        DelDataSource = OperationsBase.AllDataSources + 2
    }

    // операции для всех закачек данных
    public enum AllDataPumpsOperations : int
    {
        // Запуск закачки данных
        StartPump = OperationsBase.AllDataPumps + 1,
        // Остановка закачки данных
        StopPump = OperationsBase.AllDataPumps + 2,
        // Удаление закачки данных
        DeletePump = OperationsBase.AllDataPumps + 3,
        // Предварительный просмотр данных закачки
        PreviewPumpData = OperationsBase.AllDataPumps + 4
    }

    // операции для закачки данных
    public enum DataPumpOperations : int
    {
        // Запуск закачки данных
        StartPump = OperationsBase.DataPump + 1,
        // Остановка закачки данных
        StopPump = OperationsBase.DataPump + 2,
        // Удаление закачки данных
        DeletePump = OperationsBase.DataPump + 3,
        // Предварительный просмотр данных закачки
        PreviewPumpData = OperationsBase.DataPump + 4
    }

    // операции для листа планирования
    public enum PlanningSheetOperations : int
    {
        // Конструирование
        Construction = OperationsBase.PlanningSheet + 1,
        // Изменение фильтров
        ChangeFilters = OperationsBase.PlanningSheet + 2,
        // Обновление данных
        UpdateData = OperationsBase.PlanningSheet + 3
    }

    // операции для всех задач
    public enum AllTasksOperations : int
    {
        // Создание задачи
        CreateTask = OperationsBase.AllTasks + 1,
        // Перемещение задач по иерархии
        ChangeTaskHierarchyOrder = OperationsBase.AllTasks + 2,
        // Действие: удалить
        DelTaskAction = OperationsBase.AllTasks + 3,
        // Действие: редактировать
        EditTaskAction = OperationsBase.AllTasks + 4,

        // Добавление задачи первого уровня
        //AddRootTask = OperationsBase.AllTasks + 1,
        // Добавление подчиненной задачи
        //AddChildTask = OperationsBase.AllTasks + 2,
        // Просмотр задач всех пользователей
        ViewAllUsersTasks = OperationsBase.AllTasks + 5,
        // Распределение прав на просмотр задач
        AssignTaskViewPermission = OperationsBase.AllTasks + 6,
        // Отмена редактирования 
        CanCancelEdit = OperationsBase.AllTasks + 7,
        // Добавление документов в задачи
        //AddDocumentsIntoTasks = OperationsBase.AllTasks + 4
        // Действие: импорт задач
        ImportTask = OperationsBase.AllTasks + 8,
        // Действие: экспорт задач
        ExportTask = OperationsBase.AllTasks + 9
    }

    public enum TaskTypeOperations : int
    {
        // Создание задачи
        CreateTask = OperationsBase.TaskType + 1,
        // Перемещение задач по иерархии
        ChangeTaskHierarchyOrder = OperationsBase.TaskType + 2,
        // Действие: удалить
        DelTaskAction = OperationsBase.TaskType + 3,
        // Действие: редактировать
        EditTaskAction = OperationsBase.TaskType + 4,
        // Просмотр задач всех пользователей
        ViewAllUsersTasks = OperationsBase.TaskType + 5,
        // Распределение прав на просмотр задач
        AssignTaskViewPermission = OperationsBase.TaskType + 6,
        // Отмена редактирования 
        CanCancelEdit = OperationsBase.TaskType + 7,
        /*
        // Действие: импорт задач
        ImportTask = OperationsBase.TaskType + 8,
        // Действие: экспорт задач
        ExportTask = OperationsBase.TaskType + 9
         * */
    }

    // операции для задачи
    public enum TaskOperations : int
    {
        // Просмотр
        View = OperationsBase.Task + 1,
        // обратная запись
        Perform = OperationsBase.Task + 2
    }

    // операции для документа задачи
    public enum TaskDocumentOperations : int
    {
        // Просмотр
        View = OperationsBase.TaskDocument + 1
    }

    /// <summary>
    /// Операции для веб-отчетов сайта.
    /// </summary>
    public enum WebReportsOperations : int
    {
        /// <summary>
        /// Просмотр.
        /// </summary>
        View = SysObjectsTypes.WebReports + 1
    }

    // операции для всех задач
    public enum AllTemplatesOperations : int
    {
        /// <summary>
        /// Создание шаблонов
        /// </summary>
        CreateTemplate = SysObjectsTypes.AllTemplates + 1,

        /// <summary>
        /// Перемещение шаблонов по иерархии
        /// </summary>
        ChangeTemplateHierarchyOrder = SysObjectsTypes.AllTemplates + 2,

        /// <summary>
        /// Действие: редактировать
        /// </summary>
        EditTemplateAction = SysObjectsTypes.AllTemplates + 4,

        /// <summary>
        /// Просмотр шаблонов всех пользователей
        /// </summary>
        ViewAllUsersTemplates = SysObjectsTypes.AllTemplates + 5,

        /// <summary>
        /// Распределение прав на просмотр шаблонов
        /// </summary>
        AssignTemplateViewPermission = SysObjectsTypes.AllTemplates + 6,

        /// <summary>
        /// Действие: импорт шаблонов
        /// </summary>
        ImportTemplates = SysObjectsTypes.AllTemplates + 8,

        /// <summary>
        /// Действие: экспорт шаблонов
        /// </summary>
        ExportTemplates = SysObjectsTypes.AllTemplates + 9
    }

    /// <summary>
    /// Операции для вида шаблона репозитория отчетов.
    /// </summary>
    public enum TemplateTypeOperations : int
    {
        /// <summary>
        /// Создание шаблона.
        /// </summary>
        CreateTemplate = SysObjectsTypes.TemplateType + 1,

        /// <summary>
        /// Перемещение шаблонов по иерархии.
        /// </summary>
        ChangeTemplateHierarchyOrder = SysObjectsTypes.TemplateType + 2,

        /// <summary>
        /// Действие: редактировать.
        /// </summary>
        EditTemplateAction = SysObjectsTypes.TemplateType + 4,

        /// <summary>
        /// Просмотр шаблонов всех пользователей.
        /// </summary>
        ViewAllUsersTemplates = SysObjectsTypes.TemplateType + 5,

        /// <summary>
        /// Распределение прав на просмотр шаблона.
        /// </summary>
        AssignTemplateViewPermission = SysObjectsTypes.TemplateType + 6,
    }

    /// <summary>
    /// Операции для уведомлений
    /// </summary>
    public enum AllMessageOperations
    {
        /// <summary>
        /// Подписаться на все уведомления от системы
        /// </summary>
        Subscribe = SysObjectsTypes.AllMessages + 1,
        /// <summary>
        /// Подписаться на все уведомления от подсистемы системы на адрес электронной почты
        /// </summary>
        EmailSubscribe = SysObjectsTypes.AllMessages + 2
    }

    /// <summary>
    /// Операции для уведомлений
    /// </summary>
    public enum MessageOperations
    {
        /// <summary>
        /// Подписаться на уведомления от подсистемы системы
        /// </summary>
        Subscribe = SysObjectsTypes.Message + 1,

        /// <summary>
        /// Подписаться на уведомления от подсистемы системы на адрес электронной почты
        /// </summary>
        EmailSubscribe = SysObjectsTypes.Message + 2
    }

    /// <summary>
    /// Операции для шаблона репозитория отчетов.
    /// </summary>
    public enum TemplateOperations : int
    {
        /// <summary>
        /// Просмотр шаблона.
        /// </summary>
        ViewTemplate = SysObjectsTypes.Template + 1,

        /// <summary>
        /// Редактирование шаблона.
        /// </summary>
        EditTemplateAction = SysObjectsTypes.Template + 4,
    }

    public enum IncomesPlaningOperations
    {
        /// <summary>
        /// Отображение всех интерфейсов планирования доходов
        /// </summary>
        ViewPlaningOperations = OperationsBase.IncomesPlaning + 1
    }

    public enum IncomesPlaningModuleOperations
    {
        /// <summary>
        /// Отображение интерфейса планирования доходов
        /// </summary>
        ViewPlaningOperationsModule = OperationsBase.IncomesPlaningModule + 1
    }

    #region Источники финансирования

    /// <summary>
    /// видимость отдельных
    /// </summary>
    public enum FinSourcePlaningOperations : int
    {
        // Просмотр
        View = OperationsBase.FinSourcePlaning + 1,
        // Добавление записи
        AddRecord = OperationsBase.FinSourcePlaning + 2,
        // Удаление записи
        DelRecord = OperationsBase.FinSourcePlaning + 3,
        // Редактирование записи 
        EditRecord = OperationsBase.FinSourcePlaning + 4,
        // Очистка
        ClearData = OperationsBase.FinSourcePlaning + 5,
        // Импорт
        ImportData = OperationsBase.FinSourcePlaning + 6,
        // расчет
        Calculate = OperationsBase.FinSourcePlaning + 7
    }

    /// <summary>
    /// видимость отдельных
    /// </summary>
    public enum FinSourcePlaningUIModuleOperations : int
    {
        // Просмотр
        View = OperationsBase.FinSourcePlaningUIModule + 1,
        // Добавление записи
        AddRecord = OperationsBase.FinSourcePlaningUIModule + 2,
        // Удаление записи
        DelRecord = OperationsBase.FinSourcePlaningUIModule + 3,
        // Редактирование записи
        EditRecord = OperationsBase.FinSourcePlaningUIModule + 4,
        // Очистка
        ClearData = OperationsBase.FinSourcePlaningUIModule + 5,
        // Импорт
        ImportData = OperationsBase.FinSourcePlaningUIModule + 6
    }

    public enum FinSourcePlaningCalculateUIModuleOperations : int
    {
        // отображение
        View = OperationsBase.FinSourcePlaningCalculateUIModule + 1,
        // расчет
        Calculate = OperationsBase.FinSourcePlaningCalculateUIModule + 2
    }

    #endregion

    #region Прогноз развития региона
    /// <summary>
    /// Операции для прогноза развития региона
    /// </summary>
    public enum ForecastOperations : int
    {
        /// <summary>
        /// Создание новых расчетов
        /// </summary>
        CreateNew = SysObjectsTypes.AllForecast + 1,

        /// <summary>
        /// Распределение параметров эксператам
        /// </summary>
        AssignParam = SysObjectsTypes.AllForecast + 2,

        /// <summary>
        /// Расчет
        /// </summary>
        Calculate = SysObjectsTypes.AllForecast + 3,

        /// <summary>
        /// Просмотр данных.
        /// </summary>
        ViewData = SysObjectsTypes.AllForecast + 4,

        /// <summary>
        /// Правка сценариев
        /// </summary>
        AllowEdit = SysObjectsTypes.AllForecast + 5
    }

    public enum ScenForecastOperations : int
    {
        /// <summary>
        /// Создание новых расчетов
        /// </summary>
        CreateNew = SysObjectsTypes.ScenarioForecast + 1,

        /// <summary>
        /// Распределение параметров эксператам
        /// </summary>
        AssignParam = SysObjectsTypes.ScenarioForecast + 2,

        /// <summary>
        /// Расчет
        /// </summary>
        Calculate = SysObjectsTypes.ScenarioForecast + 3,
    }

    public enum Form2pForecastOperations : int
    {
        /// <summary>
        /// Создание новых расчетов
        /// </summary>
        CreateNew = SysObjectsTypes.Form2pForecast + 1,

        /// <summary>
        /// Расчет
        /// </summary>
        Calculate = SysObjectsTypes.Form2pForecast + 2,

        /// <summary>
        /// Просмотр данных
        /// </summary>
        ViewData = SysObjectsTypes.Form2pForecast + 3
    }
    #endregion

    #region Кубы

    // операции для пользовательского интерфейса
    public enum AllCubesOperations : int
    {
        //Отображение блока в интерфейсе
        Display = OperationsBase.AllCubes + 1
    }

    // операции для пользовательского интерфейса
    public enum CubeOperations : int
    {
        //Отображение блока в интерфейсе
        Display = OperationsBase.Cube + 1
    }


    #endregion

    // виды справочников
    public enum DirectoryKind : int
    {
        dkOrganizations = 0,    // организации
        dkDepartments = 1,      // отдел
        dkTasksTypes = 2,       // виды задач
        dkUsers = 3,            // пользователи
        dkGroups = 4            // группы
    }

    // видимость задач в дереве навигации
    public enum TaskVisibleInNavigation : int
    {
        tvVisible = 0,
        tvInvsible = 1,
        tvFantom = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IUsersManager : IDisposable
    {
        #region Методы для пользовательского интерфейса

        /// <summary>
        /// Возвращает таблицу пользователей
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetUsers();

        /// <summary>
        /// Возвращает таблицу групп
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetGroups();

        /// <summary>
        /// Возвращает таблицу объектов системы
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetObjects();

        /// <summary>
        /// Перегрузить список объектов системы. Требуется для обновления состояния послу неудачных транзакций
        /// в ходе которых регистрируеются объекты системы
        /// </summary>
        void LoadObjects();

        /// <summary>
        /// Возвращает таблицу со справочником организаций
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetOrganizations();

        /// <summary>
        /// Возвращает таблицу во справочником отделов
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetDepartments();

        /// <summary>
        /// Применить изменения к таблице объектов.
        /// Применятся ТОЛЬКО изменения поля "Описание" у существующих объектов
        /// </summary>
        /// <param name="changes">DataTable с изменениями</param>
        void ApplayObjectsChanges(DataTable changes);

        /// <summary>
        /// Применить изменения к таблице пользователей
        /// </summary>
        /// <param name="changes">DataTable с изменениями</param>
        void ApplayUsersChanges(DataTable changes);
        /// <summary>
        /// Применить изменения к таблице групп
        /// </summary>
        /// <param name="changes">DataTable с изменениями</param>
        void ApplayGroupsChanges(DataTable changes);
        /// <summary>
        /// Применить изменения к справочнику отделов
        /// </summary>
        /// <param name="changes">DataTable с изменениями</param>
        void ApplayDepartmentsChanges(DataTable changes);
        /// <summary>
        /// Применить изменения к справочнику организаций
        /// </summary>
        /// <param name="changes">DataTable c изменениями</param>
        void ApplayOrganizationsChanges(DataTable changes);

        /// <summary>
        /// Получить список членства пользователей в группе
        /// </summary>
        /// <param name="groupID">ID группы</param>
        /// <returns>DataTable со списком членства</returns>
        DataTable GetUsersForGroup(int groupID);

        /// <summary>
        /// Получить список вхождений пользователя в группы
        /// </summary>
        /// <param name="userID">ID пользователя</param>
        /// <returns>DataTable со списком вхождений</returns>
        DataTable GetGroupsForUser(int userID);

        /// <summary>
        /// Применить изменения к таблице вхождений
        /// </summary>
        /// <param name="mainID">ID пользователя/группы</param>
        /// <param name="changes">DataTable c изменениями</param>
        /// <param name="isUsers">является ли таблица списком вхождения пользователя в группы или списком членства пользователей в группе</param>
        void ApplayMembershipChanges(int mainID, DataTable changes, bool isUsers);

        /// <summary>
        /// Получить список назначенных разрешений
        /// </summary>
        /// <param name="mainID">ID пользователя/группы</param>
        /// <param name="isUser">Является ли объект пользователем или это группа</param>
        /// <returns>DataTable co списком разрешений</returns>
        DataTable GetAssignedPermissions(int mainID, bool isUser);

        /// <summary>
        /// Получить таблицу разрешенных операций на объект для пользователя
        /// </summary>
        /// <param name="objectID">ID объекта</param>
        /// <param name="objectType">Тип объекта</param>
        /// <returns>DataTable c операциями</returns>
        DataTable GetUsersPermissionsForObject(int objectID, int objectType);

        /// <summary>
        /// Получить таблицу разрешенных операций на объект для пользователя
        /// </summary>
        /// <param name="objectID">ID объекта</param>
        /// <param name="objectType">Тип объекта</param>
        /// <returns>DataTable c операциями</returns>
        DataTable GetGroupsPermissionsForObject(int objectID, int objectType);

        /// <summary>
        /// Применить изменения в назначенных пользователяю правах на объект
        /// </summary>
        /// <param name="objectID">ID объекта</param>
        /// <param name="objectType">Тип объекта</param>
        /// <param name="changes">DataTable c изменениями в правах</param>
        void ApplayUsersPermissionsChanges(int objectID, int objectType, DataTable changes);

        /// <summary>
        /// Применить изменения в назначенных группе правах на объект
        /// </summary>
        /// <param name="objectID">ID объекта</param>
        /// <param name="objectType">Тип объекта</param>
        /// <param name="changes">DataTable c изменениями</param>
        void ApplayGroupsPermissionsChanges(int objectID, int objectType, DataTable changes);

        /// <summary>
        /// Получить русское название операции по ее коду
        /// </summary>
        /// <param name="operation">Код операции</param>
        /// <returns>Русское название</returns>
        string GetCaptionForOperation(int operation);

        void CheckCurrentUser();
        string GetUserNameByID(int userID);
        int GetCurrentUserID();
        string GetCurrentUserName();

        string GetNameFromDirectoryByID(DirectoryKind dk, int id);

        bool AuthenticateUser(string dnsName, ref string errStr);
        bool AuthenticateUser(string login, string pwdHash, ref string errStr);
        bool ChangeUserPasswordAdm(int userID, string newPwdHash, ref string errStr);
        bool ChangeUserPassword(string login, string pwdHash, string newPwdHash, ref string errStr);

        /// <summary>
        /// Общий метод проверки прав для всех системных объектов кроме задач
        /// </summary>
        /// <param name="objectName">Название объекта</param>
        /// <param name="operation">Операция</param>
        /// <param name="raiseException">В случае отсутствия разрешения генерировать исключение</param>
        /// <returns></returns>
        bool CheckPermissionForSystemObject(string objectName, int operation, bool raiseException);

        /// <summary>
        /// Метод проверки прав на источники.
        /// </summary>
        /// <param name="operation">Операция.</param>
        /// <returns></returns>
        bool CheckPermissionForDataSources(int operation);

        /// <summary>
        /// Возвращает массив названий навигационных объектов 
        /// доступных текукущему пользователю.
        /// </summary>
        string[] GetViewObjectsNamesAllowedForCurrentUser();

        Dictionary<string, string> GetServerAssemblyesInfo(string filter);

        string ServerLibraryVersion();

        /// <summary>
        /// Получить ID системного объекта соответствующего шаблону/задаче.
        /// </summary>
        /// <param name="objectID">ID объекта (из таблицы шаблонов/задач).</param>
        /// <returns>-1 если объект не найден, ID объекта в противном случае</returns>
        /// <param name="sysObjectType">Тип объекта.</param>
        int GetSystemObjectID(int objectID, SysObjectsTypes sysObjectType);

        int RegisterSystemObject(string name, string caption, SysObjectsTypes objectType);

        #endregion

        #region Задачи

        /// <summary>
        /// Возвращает таблицу со справочником типов задач
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetTasksTypes();

        /// <summary>
        /// Применить изменения к справочнику типов задач
        /// </summary>
        /// <param name="changes">DataTable c изменениями</param>
        void ApplayTasksTypesChanges(DataTable changes);
        void ApplayTasksTypesChanges(IDatabase db, DataTable changes);

        /// <summary>
        /// Метод проверки прав для задач
        /// </summary>
        /// <param name="taskID">ID задачи</param>
        /// <param name="taskType">Тип задачи</param>
        /// <param name="operation">Операция</param>
        /// <param name="raiseException">В случае отсутствия разрешения генерировать исключение</param>
        /// <returns></returns>
        bool CheckPermissionForTask(int taskID, int taskType, int operation, bool raiseException);

        /// <summary>
        /// Метод проверки прав для всех задач
        /// </summary>
        /// <param name="userID">ID пользователя</param>
        /// <param name="operation">Операция</param>
        /// <returns></returns>
        bool CheckAllTasksPermissionForTask(int userID, AllTasksOperations operation);

        ArrayList GetUserCreatableTaskTypes(int userID);

        bool CurrentUserCanCreateTasks();

        #endregion Задачи

        #region Шаблоны отчетов

        /// <summary>
        /// Метод проверки прав для всех шаблонов.
        /// </summary>
        /// <param name="userID">ID пользователя.</param>
        /// <param name="operation">Операция.</param>
        /// <returns></returns>
        bool CheckAllTemplatesPermissionForTemplate(int userID, AllTemplatesOperations operation);

        /// <summary>
        /// Метод проверки прав для типов шаблонов.
        /// </summary>
        /// <param name="userID">ID пользователя.</param>
        /// <param name="taskType">Тип шаблона.</param>
        /// <param name="operation">Операция.</param>
        /// <returns></returns>
        bool CheckTemplateTypePermissionForTemplate(int userID, int taskType, TemplateTypeOperations operation);

        /// <summary>
        /// Возвращает допустимые операции с шаблонами для указанного пользователя.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ArrayList GetTemplateTypesIdsWithAllowedOperation(TemplateTypeOperations op, int userID);

        /// <summary>
        /// Метод проверки прав для шаблона.
        /// </summary>
        /// <param name="templateID">ID шаблона.</param>
        /// <param name="templateType">Тип шаблона.</param>
        /// <param name="operation">Операция.</param>
        /// <param name="raiseException">В случае отсутствия разрешения генерировать исключение.</param>
        /// <returns></returns>
        bool CheckPermissionForTemplate(int templateID, int templateType, int operation, bool raiseException);

        #endregion Шаблоны отчетов

        #region Прогноз развития региона
        /// <summary>
        /// Проверяет права пользователя или группы в которую он входит на проведение операции
        /// для родительских операций подмножества ForecastOperations
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Boolean CheckAllForecastPermission(Int32 userID, ForecastOperations operation);

        /// <summary>
        /// Проверяет права пользователя или группы в которую он входит на проведение операции
        /// для дочерних операций подмножества ScenForecastOperations (для сценариев и вариантов )
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="taskType"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Boolean CheckScenForecastPermission(Int32 userID, String taskType, ScenForecastOperations operation);

        /// <summary>
        /// Проверяет права пользователя или группы в которую он входит на проведение операции
        /// для родительских операций подмножества Form2pForecastOperations
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Boolean CheckForm2pForecastPermission(Int32 userID, Form2pForecastOperations operation);

        #endregion
    }

    [Serializable]
    public class PermissionException : RemotingException
    {
        private string _userName;

        public string UserName
        {
            get { return _userName; }
        }

        private string _objectName;

        public string ObjectName
        {
            get { return _objectName; }
        }

        private string _operation;

        public string Operation
        {
            get { return _operation; }
        }

        public PermissionException(string userName, string objectName, string operation, string message)
            : base(message)
        {
            _userName = userName;
            _objectName = objectName;
            _operation = operation;
        }

        protected PermissionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._userName = info.GetString("UserName");
            this._objectName = info.GetString("ObjectName");
            this._operation = info.GetString("Operation");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("UserName", this._userName, typeof(string));
            info.AddValue("ObjectName", this._objectName, typeof(string));
            info.AddValue("Operation", this._operation, typeof(string));
        }

    }

}