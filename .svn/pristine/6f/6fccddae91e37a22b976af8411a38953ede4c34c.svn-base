using System;
using System.Xml;
using System.Collections.Generic;

namespace Krista.FM.ServerLibrary
{
    // константы для тэгов XML
    public struct TasksExportXmlConsts
    {
        // общие
        public static string RootNodeTagName = "tasksExportXml";
        public static string VersionTagName = "version";
        public static string IDTagName = "ID";
        public static string CRCTagName = "crc";
        public static string SizeTagName = "size";
        // задачи
        public static string TasksTagName = "tasks";
        public static string TaskTagName = "task";
        public static string TaskStateTagName = "taskState";
        public static string TaskActionTagName = "taskAction";
        public static string TaskHeadlineTagName = "taskHeadline";
        public static string TaskDescriptionTagName = "taskDescription";
        public static string TaskJobTagName = "taskJob";
        public static string TaskFromDateTagName = "taskFromDate";
        public static string TaskToDateTagName = "taskToDate";
        public static string TaskOwnerTagName = "owner";
        public static string TaskDoerTagName = "doer";
        public static string TaskCuratorTagName = "curator";
        public static string TaskLockByUser = "lockByUser";
        public static string TaskLockedUserName = "lockedUserName";
        // константы и параметры
        public static string TaskConstsTagName = "consts";
        public static string TaskConstTagName = "const";
        public static string TaskParamsTagName = "params";
        public static string TaskParamTagName = "param";
        public static string TaskConstsParamsNameTagName = "name";
        public static string TaskConstsParamsDimensionTagName = "dimension";
        public static string TaskConstsParamsAllowMultiSelectTagName = "allowMultiSelect";
        public static string TaskConstsParamsDescriptionTagName = "description";
        public static string TaskConstsParamsValuesTagName = "values";
        // документы
        public static string DocumentsTagName = "documents";
        public static string DocumentTagName = "document";
        public static string DocNameTagName = "documentName";
        public static string DocSourceFileNameTagName = "documentSourceFileName";
        public static string DocTypeTagName = "documentType";
        public static string DocDataTagName = "documentData";
        public static string DocDescriptionTagName = "documentDescription";
        public static string DocOwnershipTagName = "documentOwnership";
        // справочник типов задач
        public static string TaskTypesTagName = "taskTypes";
        public static string TaskTypeTagName = "taskType";
        public static string TaskTypeCodeTagName = "taskTypesCode";
        public static string TaskTypeNameTagName = "taskTypesName";
        public static string TaskTypeDescriptionTagName = "taskTypesDescription";
        public static string TaskTypeTaskTypeTagName = "taskTypesTaskType";
    }

    public enum TaskExportType : int {
        teUndefined = 0,
        teSelectedOnly = 1,
        teIncludeChild = 2
    };

    public enum TaskImportType : int
    {
        tiUndefined = 0,
        tiAsRootTasks = 1,
        tiAsChildForSelected = 2
    };
    /*public interface ITasksExport
    {
        string GetTasksList(List<int> selectedID, TaskExportType exportType, ref string errStr);
        string ExportTasks(List<int> selectedID, TaskExportType exportType, ref string errStr);
    }

    public interface ITasksImport
    {
        bool ImportTasks(XmlDocument importedTasks);
    }*/
}