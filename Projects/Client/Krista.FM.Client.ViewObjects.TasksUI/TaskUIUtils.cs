using System;
using System.Data;

using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    internal struct TaskUtils
    {
        public static string TaskDocumentTypeToString(TaskDocumentType dt, string fileExt)
        {
            string fileTypeExt = String.Empty;
            switch (fileExt.ToLower())
            {
                case OfficeFileExt.ExcelDocument:
                case OfficeFileExt.ExcelDocumentX:
                    fileTypeExt = "Надстройка MS Excel";
                    break;
                case OfficeFileExt.WordDocument:
                case OfficeFileExt.WordDocumentX:
                    fileTypeExt = "Надстройка MS Word";
                    break;
            }

            switch (dt)
            {
                case TaskDocumentType.dtArbitraryDocument:
                    return "Произвольный документ";
                case TaskDocumentType.dtCalcSheet:
                    return fileTypeExt + " - Расчетный лист";
                case TaskDocumentType.dtDataCaptureList:
                    return fileTypeExt + " - Форма сбора данных";
                case TaskDocumentType.dtInputForm:
                    return fileTypeExt + " - Форма ввода";
                case TaskDocumentType.dtReport:
                    return fileTypeExt + " - Отчет";
                case TaskDocumentType.dtPlanningSheet:
                    return fileTypeExt;
                case TaskDocumentType.dtMDXExpertDocument:
                    return "Документ MDX Эксперт";
                case TaskDocumentType.dtWordDocument:
                    return "Документ MS Word";
                case TaskDocumentType.dtExcelDocument:
                    return "Документ MS Excel";
                case TaskDocumentType.dtDummyValue:
                    return String.Empty;
                default:
                    return "Неизвестный тип документа";
            }
        }

        public static string TaskDocumentOwnershipToString(TaskDocumentOwnership docOwn)
        {
            switch (docOwn)
            {
                case TaskDocumentOwnership.doGeneral:
                    return "Общий документ";
                case TaskDocumentOwnership.doOwner:
                    return "Документ владельца";
                case TaskDocumentOwnership.doDoer:
                    return "Документ исполнителя";
                case TaskDocumentOwnership.doCurator:
                    return "Документ куратора";
                default:
                    return "Неизвестный тип принадлежности документа";
            }
        }

        /// <summary>
        /// Определение ФИО пользователя по ID
        /// </summary>
        /// <param name="users">таблица пользователей</param>
        /// <param name="userID">ID пользователя</param>
        /// <returns>ФИО или login (если ФИО не заполнено)</returns>
        public static string DefineUserName(DataTable users, int userID)
        {
            // ищем пользователя
            DataRow[] user = users.Select(String.Format("ID={0}", userID));
            if (user.Length == 0)
                return "Пользователь не определен";
            // получаем имя, фамилию, отчество
            string firstName = Convert.ToString(user[0]["FirstName"]);
            string lastName = Convert.ToString(user[0]["LastName"]);
            string patronymic = Convert.ToString(user[0]["Patronymic"]);
            // если хотя бы одно из полей заполнено - формируем ФИО
            if ((!String.IsNullOrEmpty(firstName)) ||
                (!String.IsNullOrEmpty(lastName)) ||
                (!String.IsNullOrEmpty(patronymic)))
                return String.Format("{0} {1} {2}", lastName, firstName, patronymic);
            else
                // иначе - возвращаем логин
                return Convert.ToString(user[0]["Name"]);
        }

    }
}