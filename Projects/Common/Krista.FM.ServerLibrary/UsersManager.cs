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
            UserNotDefined = 0, // ������������ �� ���������
            InstallAdmin = 1,   // ������������� �������������
            System = 2,         // System 
            DataPump = 3        // ��� �������
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
    /// ���� ��������.
    /// </summary>
    public enum SysObjectsTypes : int
    {
        /// <summary>
        /// ����������� ���
        /// </summary>
        [Description("����������� ��� �������")]
        Unknown = 0,

        /// <summary>
        /// �����������������
        /// </summary>
        [Description("�����������������")]
        [AssociateEnumType(typeof(AdministrationOperations))]
        Administration = 1000,

        /// <summary>
        /// ��� ����� ���������� �������
        /// </summary>
        [Description("��� ����� ���������� �������")]
        [AssociateEnumType(typeof(AllUIModulesOperations))]
        AllUIModules = 2000,

        /// <summary>
        /// ���� ���������� �������
        /// </summary>
        [Description("���� ���������� �������")]
        [AssociateEnumType(typeof(UIModuleOperations))]
        UIModule = 3000,

        /// <summary>
        /// ���� ���������� ������� "�������������� � �������"
        /// </summary>
        [Description("���� ���������� �������")]
        [AssociateEnumType(typeof(EntityNavigationListUI))]
        EntityNavigationListUI = 3500,

        /// <summary>
        /// ��� �������������� ������
        /// </summary>
        [Description("��� �������������� ������")]
        [AssociateEnumType(typeof(AllDataClassifiersOperations))]
        AllDataClassifiers = 4000,

        /// <summary>
        /// ������������� ������
        /// </summary>
        [Description("������������� ������")]
        [AssociateEnumType(typeof(DataClassifiesOperations))]
        DataClassifier = 5000,

        /// <summary>
        /// ��� ������������ ��������������
        /// </summary>
        [Description("��� ������������ ��������������")]
        [AssociateEnumType(typeof(AllAssociatedClassifiersOperations))]
        AllAssociatedClassifiers = 6000,

        /// <summary>
        /// ������������ �������������
        /// </summary>
        [Description("������������ �������������")]
        [AssociateEnumType(typeof(AssociatedClassifierOperations))]
        AssociatedClassifier = 7000,

        /// <summary>
        /// ��� ������� ������
        /// </summary>
        [Description("��� ������� ������")]
        [AssociateEnumType(typeof(AllFactTablesOperations))]
        AllFactTables = 8000,

        /// <summary>
        /// ������� ������
        /// </summary>
        [Description("������� ������")]
        [AssociateEnumType(typeof(FactTableOperations))]
        FactTable = 9000,

        /// <summary>
        /// ������������� ���� ���������������
        /// </summary>
        [Description("������������� ���� ���������������")]
        [AssociateEnumType(typeof(AssociateForAllClassifiersOperations))]
        AssociateForAllClassifiers = 10000,

        /// <summary>
        /// �������������
        /// </summary>
        [Description("�������������")]
        [AssociateEnumType(typeof(AssociateOperations))]
        Associate = 11000,

        /// <summary>
        /// ��������� ������
        /// </summary>
        [Description("��������� ������")]
        [AssociateEnumType(typeof(AllDataSourcesOperation))]
        AllDataSources = 12000,

        /// <summary>
        /// ��� ������� ������
        /// </summary>
        [Description("��� ������� ������")]
        [AssociateEnumType(typeof(AllDataPumpsOperations))]
        AllDataPumps = 13000,

        /// <summary>
        /// ������� ������ (��� ��� ���������� ����� �������)
        /// </summary>
        [Description("������� ������")]
        [AssociateEnumType(typeof(DataPumpOperations))]
        DataPump = 14000,

        /// <summary>
        /// ����� ������������
        /// </summary>
        [Description("����� ������������")]
        [AssociateEnumType(typeof(PlanningSheetOperations))]
        PlanningSheet = 15000,

        /// <summary>
        /// ��� ������
        /// </summary>
        [Description("��� ������")]
        [AssociateEnumType(typeof(AllTasksOperations))]
        AllTasks = 16000,

        /// <summary>
        /// �������� ������
        /// </summary>
        [Description("�������� ������")]
        [AssociateEnumType(typeof(TaskDocumentOperations))]
        TaskDocument = 17000,

        /// <summary>
        /// ������� ������ (��� ���� ���������� ������, ������ ��� ���� ���� ��� ����� �� ��� ������� ����� ������)
        /// </summary>
        [Description("������")]
        [AssociateEnumType(typeof(TaskOperations))]
        Task = 18000,

        /// <summary>
        /// ��� ������
        /// </summary>
        [Description("��� ������")]
        [AssociateEnumType(typeof(TaskTypeOperations))]
        TaskType = 19000,

        /// <summary>
        /// ������������ ������
        /// </summary>
        [Description("Web-���������")]
        [AssociateEnumType(typeof(WebReportsOperations))]
        WebReports = 20000,

        /// <summary>
        /// ��� ������� ����������� �������.
        /// </summary>
        [Description("��� ������� ����������� �������")]
        [AssociateEnumType(typeof(AllTemplatesOperations))]
        AllTemplates = 21000,

        /// <summary>
        /// ��� ������� ����������� �������.
        /// </summary>
        [Description("��� ������� ����������� �������")]
        [AssociateEnumType(typeof(TemplateTypeOperations))]
        TemplateType = 22000,

        /// <summary>
        /// ������ ����������� �������.
        /// </summary>
        [Description("������ ����������� �������")]
        [AssociateEnumType(typeof(TemplateOperations))]
        Template = 23000,

        /// <summary>
        /// ������� �������� �������. ��� ������
        /// </summary>
        [Description("������� �������� �������")]
        [AssociateEnumType(typeof(ForecastOperations))]
        AllForecast = 24000,

        /// <summary>
        /// ������� �������� �������. ��������. ������� �������
        /// </summary>
        [Description("��������. �������.")]
        [AssociateEnumType(typeof(ScenForecastOperations))]
        ScenarioForecast = 25000,

        /// <summary>
        /// ������� �������� �������. ����� 2�
        /// </summary>
        [Description("����� 2�")]
        [AssociateEnumType(typeof(Form2pForecastOperations))]
        Form2pForecast = 26000,

        /// <summary>
        /// ��������� ��������������
        /// </summary>
        [Description("��������� ��������������")]
        [AssociateEnumType(typeof(FinSourcePlaningOperations))]
        FinSourcePlaning = 27000,

        /// <summary>
        /// ���� ���������� �������
        /// </summary>
        [Description("��������� ��������������. ������� ���������� ")]
        [AssociateEnumType(typeof(FinSourcePlaningUIModuleOperations))]
        FinSourcePlaningUIModule = 28000,

        /// <summary>
        /// ���� ���������� �������
        /// </summary>
        [Description("��������� ��������������. �������")]
        [AssociateEnumType(typeof(FinSourcePlaningCalculateUIModuleOperations))]
        FinSourcePlaningCalculateUIModule = 29000,

        /// <summary>
        /// ���� ���������� �������
        /// </summary>
        [Description("������� ����� ���������� ������� '�������������� � �������'")]
        [AssociateEnumType(typeof(UIClassifiersSubmoduleOperation))]
        UIClassifiersSubmodule = 30000,

        /// <summary>
        /// ���� ���������� �������
        /// </summary>
        [Description("��������� ������� '������� ������������������ �������'")]
        [AssociateEnumType(typeof(UIClassifiersSubmoduleOperation))]
        UIConsBudgetForecast = 31000,

        /// <summary>
        /// �������� �� ��� �����������
        /// </summary>
        [Description("�������� �� ��� ���������")]
        [AssociateEnumType(typeof(AllMessageOperations))]
        AllMessages = 32000,

        [Description("��������� �� ����������")]
        [AssociateEnumType(typeof(MessageOperations))]
        Message = 33000,

        [Description("��������� ������������ �������")]
        [AssociateEnumType(typeof(IncomesPlaningOperations))]
        IncomesPlaning = 34000,

        [Description("��������� ������������ �������. ������� ����������")]
        [AssociateEnumType(typeof(IncomesPlaningModuleOperations))]
        IncomesPlaningModule = 35000,

        [Description("��� ����")]
        [AssociateEnumType(typeof(AllCubesOperations))]
        AllCubes = 36000,

        [Description("���")]
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

    // �������� �� �����������������
    public enum AdministrationOperations : int
    {
        // ���������� �������
        PermissionsManagement = OperationsBase.Administration + 1
    }

    // �������� ��� ���� ���������������� �����������
    public enum AllUIModulesOperations : int
    {
        //����������� ���� ������ � ����������
        DisplayAll = OperationsBase.AllUIModules + 1
    }

    // �������� ��� ����������������� ����������
    public enum UIModuleOperations : int
    {
        //����������� ����� � ����������
        Display = OperationsBase.UIModule + 1
    }

    // �������� ��� ����������������� ����������
    public enum EntityNavigationListUI : int
    {
        //����������� ����� � ����������
        Display = OperationsBase.EntityNavigationListUI + 1
    }

    public enum UIClassifiersSubmoduleOperation : int
    {
        Display = OperationsBase.UIClassifiersSubmodule + 1
    }

    // �������� ��� ���� ���������������
    public enum AllClassifiersOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // ���������� �������������� �� ������ ���������
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // ������� ��������������
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // ������ ��������������
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // �������� ��� ������ ��������������
    public enum ClassifierOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // ���������� �������������� �� ������ ���������
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // ������� ��������������
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // ������ ��������������
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // �������� ��� ���� ��������������� ������
    public enum AllDataClassifiersOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.AllDataClassifiers + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.AllDataClassifiers + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.AllDataClassifiers + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.AllDataClassifiers + 4,
        // ���������� �������������� �� ������ ���������
        AddClassifierForNewDataSource = OperationsBase.AllDataClassifiers + 5,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.AllDataClassifiers + 6,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllDataClassifiers + 7,
        // ������� ��������������
        ClearClassifierData = OperationsBase.AllDataClassifiers + 8,
        // ������ ��������������
        ImportClassifier = OperationsBase.AllDataClassifiers + 9
    }

    // �������� ��� �������������� ������
    public enum DataClassifiesOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.DataClassifier + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.DataClassifier + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.DataClassifier + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.DataClassifier + 4,
        // ���������� �������������� �� ������ ���������
        AddClassifierForNewDataSource = OperationsBase.DataClassifier + 5,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.DataClassifier + 6,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.DataClassifier + 7,
        // ������� ��������������
        ClearClassifierData = OperationsBase.DataClassifier + 8,
        // ������ ��������������
        ImportClassifier = OperationsBase.DataClassifier + 9
    }

    // �������� ��� ���� ������������ ���������������
    public enum AllAssociatedClassifiersOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.AllAssociatedClassifiers + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.AllAssociatedClassifiers + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.AllAssociatedClassifiers + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.AllAssociatedClassifiers + 4,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.AllAssociatedClassifiers + 5,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AllAssociatedClassifiers + 6,
        // ������� ��������������
        ClearClassifierData = OperationsBase.AllAssociatedClassifiers + 7,
        // ������ ��������������
        ImportClassifier = OperationsBase.AllAssociatedClassifiers + 8
    }

    // �������� ��� ������������� ��������������
    public enum AssociatedClassifierOperations : int
    {
        // �������� ��������������
        ViewClassifier = OperationsBase.AssociatedClassifier + 1,
        // ���������� ������ � �������������
        AddRecord = OperationsBase.AssociatedClassifier + 2,
        // �������� ������ ��������������
        DelRecord = OperationsBase.AssociatedClassifier + 3,
        // �������������� ������ ��������������
        EditRecord = OperationsBase.AssociatedClassifier + 4,
        // ��������� �������� � ��������������
        ChangeClassifierHierarchy = OperationsBase.AssociatedClassifier + 5,
        // ��������� �������� � �������������� � ����������� ����
        SetHierarchyAndCodeDisintegrationForClassifier = OperationsBase.AssociatedClassifier + 6,
        // ������� ��������������
        ClearClassifierData = OperationsBase.AssociatedClassifier + 7,
        // ������ ��������������
        ImportClassifier = OperationsBase.AssociatedClassifier + 8
    }

    // �������� ��� ���� ������ ������
    public enum AllFactTablesOperations : int
    {
        // �������� ������
        ViewClassifier = OperationsBase.AllFactTables + 1,
        // �������������� ������
        EditRecord = OperationsBase.AllFactTables + 2
    }

    // �������� ��� ������� ������
    public enum FactTableOperations : int
    {
        // �������� ������
        ViewClassifier = OperationsBase.FactTable + 1,
        // �������������� ������
        EditRecord = OperationsBase.FactTable + 2
    }

    // �������� ��� ������������� ���� ���������������
    public enum AssociateForAllClassifiersOperations : int
    {
        // �������������
        Associate = OperationsBase.AssociateForAllClassifiers + 1,
        // ������� �������������
        ClearAssociate = OperationsBase.AssociateForAllClassifiers + 2,
        // ���������� ������ � ������� �������������
        AddRecordIntoBridgeTable = OperationsBase.AssociateForAllClassifiers + 3,
        // �������� ������ �� ������� �������������
        DelRecordFromBridgeTable = OperationsBase.AssociateForAllClassifiers + 4
    }

    // �������� ��� �������������
    public enum AssociateOperations : int
    {
        // �������������
        Associate = OperationsBase.Associate + 1,
        // ������� �������������
        ClearAssociate = OperationsBase.Associate + 2,
        // ���������� ������ � ������� �������������
        AddRecordIntoBridgeTable = OperationsBase.Associate + 3,
        // �������� ������ �� ������� �������������
        DelRecordFromBridgeTable = OperationsBase.Associate + 4
    }

    // �������� ��� ���� ���������� ������
    public enum AllDataSourcesOperation : int
    {
        // ���������� ��������� ������
        AddDataSource = OperationsBase.AllDataSources + 1,
        // �������� ��������� ������
        DelDataSource = OperationsBase.AllDataSources + 2
    }

    // �������� ��� ���� ������� ������
    public enum AllDataPumpsOperations : int
    {
        // ������ ������� ������
        StartPump = OperationsBase.AllDataPumps + 1,
        // ��������� ������� ������
        StopPump = OperationsBase.AllDataPumps + 2,
        // �������� ������� ������
        DeletePump = OperationsBase.AllDataPumps + 3,
        // ��������������� �������� ������ �������
        PreviewPumpData = OperationsBase.AllDataPumps + 4
    }

    // �������� ��� ������� ������
    public enum DataPumpOperations : int
    {
        // ������ ������� ������
        StartPump = OperationsBase.DataPump + 1,
        // ��������� ������� ������
        StopPump = OperationsBase.DataPump + 2,
        // �������� ������� ������
        DeletePump = OperationsBase.DataPump + 3,
        // ��������������� �������� ������ �������
        PreviewPumpData = OperationsBase.DataPump + 4
    }

    // �������� ��� ����� ������������
    public enum PlanningSheetOperations : int
    {
        // ���������������
        Construction = OperationsBase.PlanningSheet + 1,
        // ��������� ��������
        ChangeFilters = OperationsBase.PlanningSheet + 2,
        // ���������� ������
        UpdateData = OperationsBase.PlanningSheet + 3
    }

    // �������� ��� ���� �����
    public enum AllTasksOperations : int
    {
        // �������� ������
        CreateTask = OperationsBase.AllTasks + 1,
        // ����������� ����� �� ��������
        ChangeTaskHierarchyOrder = OperationsBase.AllTasks + 2,
        // ��������: �������
        DelTaskAction = OperationsBase.AllTasks + 3,
        // ��������: �������������
        EditTaskAction = OperationsBase.AllTasks + 4,

        // ���������� ������ ������� ������
        //AddRootTask = OperationsBase.AllTasks + 1,
        // ���������� ����������� ������
        //AddChildTask = OperationsBase.AllTasks + 2,
        // �������� ����� ���� �������������
        ViewAllUsersTasks = OperationsBase.AllTasks + 5,
        // ������������� ���� �� �������� �����
        AssignTaskViewPermission = OperationsBase.AllTasks + 6,
        // ������ �������������� 
        CanCancelEdit = OperationsBase.AllTasks + 7,
        // ���������� ���������� � ������
        //AddDocumentsIntoTasks = OperationsBase.AllTasks + 4
        // ��������: ������ �����
        ImportTask = OperationsBase.AllTasks + 8,
        // ��������: ������� �����
        ExportTask = OperationsBase.AllTasks + 9
    }

    public enum TaskTypeOperations : int
    {
        // �������� ������
        CreateTask = OperationsBase.TaskType + 1,
        // ����������� ����� �� ��������
        ChangeTaskHierarchyOrder = OperationsBase.TaskType + 2,
        // ��������: �������
        DelTaskAction = OperationsBase.TaskType + 3,
        // ��������: �������������
        EditTaskAction = OperationsBase.TaskType + 4,
        // �������� ����� ���� �������������
        ViewAllUsersTasks = OperationsBase.TaskType + 5,
        // ������������� ���� �� �������� �����
        AssignTaskViewPermission = OperationsBase.TaskType + 6,
        // ������ �������������� 
        CanCancelEdit = OperationsBase.TaskType + 7,
        /*
        // ��������: ������ �����
        ImportTask = OperationsBase.TaskType + 8,
        // ��������: ������� �����
        ExportTask = OperationsBase.TaskType + 9
         * */
    }

    // �������� ��� ������
    public enum TaskOperations : int
    {
        // ��������
        View = OperationsBase.Task + 1,
        // �������� ������
        Perform = OperationsBase.Task + 2
    }

    // �������� ��� ��������� ������
    public enum TaskDocumentOperations : int
    {
        // ��������
        View = OperationsBase.TaskDocument + 1
    }

    /// <summary>
    /// �������� ��� ���-������� �����.
    /// </summary>
    public enum WebReportsOperations : int
    {
        /// <summary>
        /// ��������.
        /// </summary>
        View = SysObjectsTypes.WebReports + 1
    }

    // �������� ��� ���� �����
    public enum AllTemplatesOperations : int
    {
        /// <summary>
        /// �������� ��������
        /// </summary>
        CreateTemplate = SysObjectsTypes.AllTemplates + 1,

        /// <summary>
        /// ����������� �������� �� ��������
        /// </summary>
        ChangeTemplateHierarchyOrder = SysObjectsTypes.AllTemplates + 2,

        /// <summary>
        /// ��������: �������������
        /// </summary>
        EditTemplateAction = SysObjectsTypes.AllTemplates + 4,

        /// <summary>
        /// �������� �������� ���� �������������
        /// </summary>
        ViewAllUsersTemplates = SysObjectsTypes.AllTemplates + 5,

        /// <summary>
        /// ������������� ���� �� �������� ��������
        /// </summary>
        AssignTemplateViewPermission = SysObjectsTypes.AllTemplates + 6,

        /// <summary>
        /// ��������: ������ ��������
        /// </summary>
        ImportTemplates = SysObjectsTypes.AllTemplates + 8,

        /// <summary>
        /// ��������: ������� ��������
        /// </summary>
        ExportTemplates = SysObjectsTypes.AllTemplates + 9
    }

    /// <summary>
    /// �������� ��� ���� ������� ����������� �������.
    /// </summary>
    public enum TemplateTypeOperations : int
    {
        /// <summary>
        /// �������� �������.
        /// </summary>
        CreateTemplate = SysObjectsTypes.TemplateType + 1,

        /// <summary>
        /// ����������� �������� �� ��������.
        /// </summary>
        ChangeTemplateHierarchyOrder = SysObjectsTypes.TemplateType + 2,

        /// <summary>
        /// ��������: �������������.
        /// </summary>
        EditTemplateAction = SysObjectsTypes.TemplateType + 4,

        /// <summary>
        /// �������� �������� ���� �������������.
        /// </summary>
        ViewAllUsersTemplates = SysObjectsTypes.TemplateType + 5,

        /// <summary>
        /// ������������� ���� �� �������� �������.
        /// </summary>
        AssignTemplateViewPermission = SysObjectsTypes.TemplateType + 6,
    }

    /// <summary>
    /// �������� ��� �����������
    /// </summary>
    public enum AllMessageOperations
    {
        /// <summary>
        /// ����������� �� ��� ����������� �� �������
        /// </summary>
        Subscribe = SysObjectsTypes.AllMessages + 1,
        /// <summary>
        /// ����������� �� ��� ����������� �� ���������� ������� �� ����� ����������� �����
        /// </summary>
        EmailSubscribe = SysObjectsTypes.AllMessages + 2
    }

    /// <summary>
    /// �������� ��� �����������
    /// </summary>
    public enum MessageOperations
    {
        /// <summary>
        /// ����������� �� ����������� �� ���������� �������
        /// </summary>
        Subscribe = SysObjectsTypes.Message + 1,

        /// <summary>
        /// ����������� �� ����������� �� ���������� ������� �� ����� ����������� �����
        /// </summary>
        EmailSubscribe = SysObjectsTypes.Message + 2
    }

    /// <summary>
    /// �������� ��� ������� ����������� �������.
    /// </summary>
    public enum TemplateOperations : int
    {
        /// <summary>
        /// �������� �������.
        /// </summary>
        ViewTemplate = SysObjectsTypes.Template + 1,

        /// <summary>
        /// �������������� �������.
        /// </summary>
        EditTemplateAction = SysObjectsTypes.Template + 4,
    }

    public enum IncomesPlaningOperations
    {
        /// <summary>
        /// ����������� ���� ����������� ������������ �������
        /// </summary>
        ViewPlaningOperations = OperationsBase.IncomesPlaning + 1
    }

    public enum IncomesPlaningModuleOperations
    {
        /// <summary>
        /// ����������� ���������� ������������ �������
        /// </summary>
        ViewPlaningOperationsModule = OperationsBase.IncomesPlaningModule + 1
    }

    #region ��������� ��������������

    /// <summary>
    /// ��������� ���������
    /// </summary>
    public enum FinSourcePlaningOperations : int
    {
        // ��������
        View = OperationsBase.FinSourcePlaning + 1,
        // ���������� ������
        AddRecord = OperationsBase.FinSourcePlaning + 2,
        // �������� ������
        DelRecord = OperationsBase.FinSourcePlaning + 3,
        // �������������� ������ 
        EditRecord = OperationsBase.FinSourcePlaning + 4,
        // �������
        ClearData = OperationsBase.FinSourcePlaning + 5,
        // ������
        ImportData = OperationsBase.FinSourcePlaning + 6,
        // ������
        Calculate = OperationsBase.FinSourcePlaning + 7
    }

    /// <summary>
    /// ��������� ���������
    /// </summary>
    public enum FinSourcePlaningUIModuleOperations : int
    {
        // ��������
        View = OperationsBase.FinSourcePlaningUIModule + 1,
        // ���������� ������
        AddRecord = OperationsBase.FinSourcePlaningUIModule + 2,
        // �������� ������
        DelRecord = OperationsBase.FinSourcePlaningUIModule + 3,
        // �������������� ������
        EditRecord = OperationsBase.FinSourcePlaningUIModule + 4,
        // �������
        ClearData = OperationsBase.FinSourcePlaningUIModule + 5,
        // ������
        ImportData = OperationsBase.FinSourcePlaningUIModule + 6
    }

    public enum FinSourcePlaningCalculateUIModuleOperations : int
    {
        // �����������
        View = OperationsBase.FinSourcePlaningCalculateUIModule + 1,
        // ������
        Calculate = OperationsBase.FinSourcePlaningCalculateUIModule + 2
    }

    #endregion

    #region ������� �������� �������
    /// <summary>
    /// �������� ��� �������� �������� �������
    /// </summary>
    public enum ForecastOperations : int
    {
        /// <summary>
        /// �������� ����� ��������
        /// </summary>
        CreateNew = SysObjectsTypes.AllForecast + 1,

        /// <summary>
        /// ������������� ���������� ����������
        /// </summary>
        AssignParam = SysObjectsTypes.AllForecast + 2,

        /// <summary>
        /// ������
        /// </summary>
        Calculate = SysObjectsTypes.AllForecast + 3,

        /// <summary>
        /// �������� ������.
        /// </summary>
        ViewData = SysObjectsTypes.AllForecast + 4,

        /// <summary>
        /// ������ ���������
        /// </summary>
        AllowEdit = SysObjectsTypes.AllForecast + 5
    }

    public enum ScenForecastOperations : int
    {
        /// <summary>
        /// �������� ����� ��������
        /// </summary>
        CreateNew = SysObjectsTypes.ScenarioForecast + 1,

        /// <summary>
        /// ������������� ���������� ����������
        /// </summary>
        AssignParam = SysObjectsTypes.ScenarioForecast + 2,

        /// <summary>
        /// ������
        /// </summary>
        Calculate = SysObjectsTypes.ScenarioForecast + 3,
    }

    public enum Form2pForecastOperations : int
    {
        /// <summary>
        /// �������� ����� ��������
        /// </summary>
        CreateNew = SysObjectsTypes.Form2pForecast + 1,

        /// <summary>
        /// ������
        /// </summary>
        Calculate = SysObjectsTypes.Form2pForecast + 2,

        /// <summary>
        /// �������� ������
        /// </summary>
        ViewData = SysObjectsTypes.Form2pForecast + 3
    }
    #endregion

    #region ����

    // �������� ��� ����������������� ����������
    public enum AllCubesOperations : int
    {
        //����������� ����� � ����������
        Display = OperationsBase.AllCubes + 1
    }

    // �������� ��� ����������������� ����������
    public enum CubeOperations : int
    {
        //����������� ����� � ����������
        Display = OperationsBase.Cube + 1
    }


    #endregion

    // ���� ������������
    public enum DirectoryKind : int
    {
        dkOrganizations = 0,    // �����������
        dkDepartments = 1,      // �����
        dkTasksTypes = 2,       // ���� �����
        dkUsers = 3,            // ������������
        dkGroups = 4            // ������
    }

    // ��������� ����� � ������ ���������
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
        #region ������ ��� ����������������� ����������

        /// <summary>
        /// ���������� ������� �������������
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetUsers();

        /// <summary>
        /// ���������� ������� �����
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetGroups();

        /// <summary>
        /// ���������� ������� �������� �������
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetObjects();

        /// <summary>
        /// ����������� ������ �������� �������. ��������� ��� ���������� ��������� ����� ��������� ����������
        /// � ���� ������� ��������������� ������� �������
        /// </summary>
        void LoadObjects();

        /// <summary>
        /// ���������� ������� �� ������������ �����������
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetOrganizations();

        /// <summary>
        /// ���������� ������� �� ������������ �������
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetDepartments();

        /// <summary>
        /// ��������� ��������� � ������� ��������.
        /// ���������� ������ ��������� ���� "��������" � ������������ ��������
        /// </summary>
        /// <param name="changes">DataTable � �����������</param>
        void ApplayObjectsChanges(DataTable changes);

        /// <summary>
        /// ��������� ��������� � ������� �������������
        /// </summary>
        /// <param name="changes">DataTable � �����������</param>
        void ApplayUsersChanges(DataTable changes);
        /// <summary>
        /// ��������� ��������� � ������� �����
        /// </summary>
        /// <param name="changes">DataTable � �����������</param>
        void ApplayGroupsChanges(DataTable changes);
        /// <summary>
        /// ��������� ��������� � ����������� �������
        /// </summary>
        /// <param name="changes">DataTable � �����������</param>
        void ApplayDepartmentsChanges(DataTable changes);
        /// <summary>
        /// ��������� ��������� � ����������� �����������
        /// </summary>
        /// <param name="changes">DataTable c �����������</param>
        void ApplayOrganizationsChanges(DataTable changes);

        /// <summary>
        /// �������� ������ �������� ������������� � ������
        /// </summary>
        /// <param name="groupID">ID ������</param>
        /// <returns>DataTable �� ������� ��������</returns>
        DataTable GetUsersForGroup(int groupID);

        /// <summary>
        /// �������� ������ ��������� ������������ � ������
        /// </summary>
        /// <param name="userID">ID ������������</param>
        /// <returns>DataTable �� ������� ���������</returns>
        DataTable GetGroupsForUser(int userID);

        /// <summary>
        /// ��������� ��������� � ������� ���������
        /// </summary>
        /// <param name="mainID">ID ������������/������</param>
        /// <param name="changes">DataTable c �����������</param>
        /// <param name="isUsers">�������� �� ������� ������� ��������� ������������ � ������ ��� ������� �������� ������������� � ������</param>
        void ApplayMembershipChanges(int mainID, DataTable changes, bool isUsers);

        /// <summary>
        /// �������� ������ ����������� ����������
        /// </summary>
        /// <param name="mainID">ID ������������/������</param>
        /// <param name="isUser">�������� �� ������ ������������� ��� ��� ������</param>
        /// <returns>DataTable co ������� ����������</returns>
        DataTable GetAssignedPermissions(int mainID, bool isUser);

        /// <summary>
        /// �������� ������� ����������� �������� �� ������ ��� ������������
        /// </summary>
        /// <param name="objectID">ID �������</param>
        /// <param name="objectType">��� �������</param>
        /// <returns>DataTable c ����������</returns>
        DataTable GetUsersPermissionsForObject(int objectID, int objectType);

        /// <summary>
        /// �������� ������� ����������� �������� �� ������ ��� ������������
        /// </summary>
        /// <param name="objectID">ID �������</param>
        /// <param name="objectType">��� �������</param>
        /// <returns>DataTable c ����������</returns>
        DataTable GetGroupsPermissionsForObject(int objectID, int objectType);

        /// <summary>
        /// ��������� ��������� � ����������� ������������� ������ �� ������
        /// </summary>
        /// <param name="objectID">ID �������</param>
        /// <param name="objectType">��� �������</param>
        /// <param name="changes">DataTable c ����������� � ������</param>
        void ApplayUsersPermissionsChanges(int objectID, int objectType, DataTable changes);

        /// <summary>
        /// ��������� ��������� � ����������� ������ ������ �� ������
        /// </summary>
        /// <param name="objectID">ID �������</param>
        /// <param name="objectType">��� �������</param>
        /// <param name="changes">DataTable c �����������</param>
        void ApplayGroupsPermissionsChanges(int objectID, int objectType, DataTable changes);

        /// <summary>
        /// �������� ������� �������� �������� �� �� ����
        /// </summary>
        /// <param name="operation">��� ��������</param>
        /// <returns>������� ��������</returns>
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
        /// ����� ����� �������� ���� ��� ���� ��������� �������� ����� �����
        /// </summary>
        /// <param name="objectName">�������� �������</param>
        /// <param name="operation">��������</param>
        /// <param name="raiseException">� ������ ���������� ���������� ������������ ����������</param>
        /// <returns></returns>
        bool CheckPermissionForSystemObject(string objectName, int operation, bool raiseException);

        /// <summary>
        /// ����� �������� ���� �� ���������.
        /// </summary>
        /// <param name="operation">��������.</param>
        /// <returns></returns>
        bool CheckPermissionForDataSources(int operation);

        /// <summary>
        /// ���������� ������ �������� ������������� �������� 
        /// ��������� ���������� ������������.
        /// </summary>
        string[] GetViewObjectsNamesAllowedForCurrentUser();

        Dictionary<string, string> GetServerAssemblyesInfo(string filter);

        string ServerLibraryVersion();

        /// <summary>
        /// �������� ID ���������� ������� ���������������� �������/������.
        /// </summary>
        /// <param name="objectID">ID ������� (�� ������� ��������/�����).</param>
        /// <returns>-1 ���� ������ �� ������, ID ������� � ��������� ������</returns>
        /// <param name="sysObjectType">��� �������.</param>
        int GetSystemObjectID(int objectID, SysObjectsTypes sysObjectType);

        int RegisterSystemObject(string name, string caption, SysObjectsTypes objectType);

        #endregion

        #region ������

        /// <summary>
        /// ���������� ������� �� ������������ ����� �����
        /// </summary>
        /// <returns>DataTable</returns>
        DataTable GetTasksTypes();

        /// <summary>
        /// ��������� ��������� � ����������� ����� �����
        /// </summary>
        /// <param name="changes">DataTable c �����������</param>
        void ApplayTasksTypesChanges(DataTable changes);
        void ApplayTasksTypesChanges(IDatabase db, DataTable changes);

        /// <summary>
        /// ����� �������� ���� ��� �����
        /// </summary>
        /// <param name="taskID">ID ������</param>
        /// <param name="taskType">��� ������</param>
        /// <param name="operation">��������</param>
        /// <param name="raiseException">� ������ ���������� ���������� ������������ ����������</param>
        /// <returns></returns>
        bool CheckPermissionForTask(int taskID, int taskType, int operation, bool raiseException);

        /// <summary>
        /// ����� �������� ���� ��� ���� �����
        /// </summary>
        /// <param name="userID">ID ������������</param>
        /// <param name="operation">��������</param>
        /// <returns></returns>
        bool CheckAllTasksPermissionForTask(int userID, AllTasksOperations operation);

        ArrayList GetUserCreatableTaskTypes(int userID);

        bool CurrentUserCanCreateTasks();

        #endregion ������

        #region ������� �������

        /// <summary>
        /// ����� �������� ���� ��� ���� ��������.
        /// </summary>
        /// <param name="userID">ID ������������.</param>
        /// <param name="operation">��������.</param>
        /// <returns></returns>
        bool CheckAllTemplatesPermissionForTemplate(int userID, AllTemplatesOperations operation);

        /// <summary>
        /// ����� �������� ���� ��� ����� ��������.
        /// </summary>
        /// <param name="userID">ID ������������.</param>
        /// <param name="taskType">��� �������.</param>
        /// <param name="operation">��������.</param>
        /// <returns></returns>
        bool CheckTemplateTypePermissionForTemplate(int userID, int taskType, TemplateTypeOperations operation);

        /// <summary>
        /// ���������� ���������� �������� � ��������� ��� ���������� ������������.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        ArrayList GetTemplateTypesIdsWithAllowedOperation(TemplateTypeOperations op, int userID);

        /// <summary>
        /// ����� �������� ���� ��� �������.
        /// </summary>
        /// <param name="templateID">ID �������.</param>
        /// <param name="templateType">��� �������.</param>
        /// <param name="operation">��������.</param>
        /// <param name="raiseException">� ������ ���������� ���������� ������������ ����������.</param>
        /// <returns></returns>
        bool CheckPermissionForTemplate(int templateID, int templateType, int operation, bool raiseException);

        #endregion ������� �������

        #region ������� �������� �������
        /// <summary>
        /// ��������� ����� ������������ ��� ������ � ������� �� ������ �� ���������� ��������
        /// ��� ������������ �������� ������������ ForecastOperations
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Boolean CheckAllForecastPermission(Int32 userID, ForecastOperations operation);

        /// <summary>
        /// ��������� ����� ������������ ��� ������ � ������� �� ������ �� ���������� ��������
        /// ��� �������� �������� ������������ ScenForecastOperations (��� ��������� � ��������� )
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="taskType"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        Boolean CheckScenForecastPermission(Int32 userID, String taskType, ScenForecastOperations operation);

        /// <summary>
        /// ��������� ����� ������������ ��� ������ � ������� �� ������ �� ���������� ��������
        /// ��� ������������ �������� ������������ Form2pForecastOperations
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