using System;
using System.Data;
using System.Xml;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    /// <summary>
    /// Класс для экспорта задач в MSProject
    /// поддерживается только 2003 версия
    /// </summary>
    public sealed class MSProjectReportHelper
    {
        // поддерживаемая версия MSProject
        private static string MSProjectAllowedVersion = "MSProject.Application.11";

        /// <summary>
        /// Метод проверки доступности MSProject нужной версии (2003)
        /// </summary>
        /// <returns>Результат проверки</returns>
        public static bool MSProjectInstalled()
        {
            // проверяем регистрацию в реестре и наличие библиотеки
            bool res = Utils.CheckLibByProgID("MSProject.Application", false);
            // проверяем версию
            if (res)
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("MSProject.Application\\CurVer", false);
                string curVer = (string)key.GetValue(String.Empty);
                key.Close();
                res = MSProjectAllowedVersion.ToUpper() == curVer.ToUpper();
            }
            return res;
        }

        // шаблон для заметки
        private static string textTemplate =
            "ID: {0}" + Environment.NewLine +
            "Владелец: {1}" + Environment.NewLine +
            "Куратор: {2}" + Environment.NewLine +
            "Задание: {3}";

        /// <summary>
        /// Метод экспорта выделенных задач в MSProject2003
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="ug">UltraGrid c выделенными задачами</param>
        public static void CreateTasksReport(IWorkplace workplace, UltraGrid ug)
        {
            try
            {
                // есть-ли выделенные задачи?
                if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
                {
                    MessageBox.Show("Необходимо выделить нужные задачи", "Нет задач для экспорта", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Application.DoEvents();

                // формируем список выделенных ID
                List<int> selectedID;
                UltraGridHelper.GetSelectedIDs(ug, out selectedID);

                // показываем прогресс
                workplace.OperationObj.Text = "Создание отчета MS Project";
                workplace.OperationObj.StartOperation();
                string errStr = String.Empty;
                // получаем XML документ с экспортируемыми данными
                XmlDocument xmlData = TasksExportHelper.GetTasksListXml(workplace, selectedID, TaskExportType.teIncludeChild, ref errStr);
                // если все прошло без ошибок - начинаем формирование отчета
                if ((errStr == null) || (errStr != String.Empty))
                {
                    MessageBox.Show("Произошла ошибка: " + errStr, "Экспорт завершился неудачей", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // создаем приложение
                    object app = OfficeHelper.CreateMsProjectApplication();
                    // отключаем автообновление
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", false);
                    // создаем отчет
                    BuildMSProjectReport(workplace, xmlData, app);
                    workplace.OperationObj.StopOperation();
                    // включаем автообновление
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", true);
                    // показываем приложение
                    ReflectionHelper.SetProperty(app, "Visible", true);
                    ReflectionHelper.CallMethod(app, "AppMaximize");
                    object activeWindow = ReflectionHelper.GetProperty(app, "ActiveWindow");
                    ReflectionHelper.CallMethod(activeWindow, "Activate");
                }
            }
            finally
            {
                workplace.OperationObj.StopOperation();
            }
        }

        /// <summary>
        /// Создать пустой документ MS Word и сохранить его по указанному адресу
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="doVisibleObject"></param>
        /// <returns></returns>
        public static object CreateEmptyDocument(string fileName, bool doVisibleObject)
        {
            object app = OfficeHelper.CreateMsProjectApplication();
            ReflectionHelper.SetProperty(app, "ScreenUpdating", false);
            return app;
        }

        /// <summary>
        /// Создать отчет MSProject
        /// </summary>
        /// <param name="workplace">интерфейс workplace</param>
        /// <param name="tasks">XmlDocument c задачами</param>
        /// <param name="app">приложение MSProject</param>
        private static void BuildMSProjectReport(IWorkplace workplace, XmlDocument tasks, object app)
        {
            // создаем новый проект
            ReflectionHelper.CallMethod(app, "FileNew", Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            object project = ReflectionHelper.GetProperty(app, "ActiveProject");

            // получаем минимальную дату
            XmlNodeList allTasks = tasks.SelectNodes(String.Format("//{0}", TasksExportXmlConsts.TaskTagName));
            DateTime minDate = DateTime.Now;
            foreach (XmlNode task in allTasks)
            {
                DateTime curDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(task, TasksExportXmlConsts.TaskFromDateTagName, String.Empty));
                if (curDate < minDate)
                    minDate = curDate;
            }
            // устанавливаем ее в качестве начальной для проекта
            ReflectionHelper.SetProperty(project, "ProjectStart", minDate);
            // получаем таблицу пользователей
            DataTable users = workplace.ActiveScheme.UsersManager.GetUsers();
            // получаем список задач верхнего уровня
            XmlNodeList rootTasks = tasks.SelectNodes(String.Format("{0}/{1}/{2}", TasksExportXmlConsts.RootNodeTagName, TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            for (int i = 0; i < rootTasks.Count; i++)
            {
                // рекурсивно обрабатываем каждую
                ProcessTask(rootTasks[i], project, app, null, i, 1, users);
            }
            ReflectionHelper.CallMethod(app, "CalculateAll");
        }

        /// <summary>
        /// Рекурсивная процедура экспорта задачи и всех ее потомков
        /// </summary>
        /// <param name="taskNode">XmlNode c параметрами задачи</param>
        /// <param name="project">проект MSProject</param>
        /// <param name="app">приложение MSProject</param>
        /// <param name="parentTask">родительская задача (для верхнего уровня == null)</param>
        /// <param name="childIndex">порядковый номер дочерней задачи</param>
        /// <param name="level">текущий уровень иерархии</param>
        /// <param name="users">таблица пользователей</param>
        private static void ProcessTask(XmlNode taskNode, object project, object app,
            object parentTask, int childIndex, int level, DataTable users)
        {
            // получаем параметры задачи
            int taskID = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.IDTagName, 0);
            string headLine = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskHeadlineTagName, String.Empty);
            string job = XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskJobTagName, String.Empty);
            int owner = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskOwnerTagName, 0);
            int doer = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskDoerTagName, 0);
            int curator = XmlHelper.GetIntAttrValue(taskNode, TasksExportXmlConsts.TaskCuratorTagName, 0);
            DateTime fromDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskFromDateTagName, String.Empty));
            DateTime toDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(taskNode, TasksExportXmlConsts.TaskToDateTagName, String.Empty));

            object newTask;
            object tasks = ReflectionHelper.GetProperty(project, "Tasks");
            // получаем список подчиненных задач
            XmlNodeList childTasks = taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            // если есть подчиненные - нужно добавить фиктивную задачу верхнего уровня
            if ((childTasks != null) && (childTasks.Count > 0))
            {
                newTask = ReflectionHelper.CallMethod(tasks, "Add", Missing.Value, Missing.Value);
                ReflectionHelper.SetProperty(newTask, "OutlineLevel", (short)level);
                ReflectionHelper.SetProperty(newTask, "Name", headLine.Trim());
                ReflectionHelper.SetProperty(newTask, "Start", fromDate);
                ReflectionHelper.SetProperty(newTask, "Finish", toDate);
            }
            // теперь добавляем текущую задачу
            newTask = ReflectionHelper.CallMethod(tasks, "Add", Missing.Value, Missing.Value);
            if ((childTasks != null) && (childTasks.Count > 0))
                // если есть подчиненные - задача идет на один уровень с ними
                ReflectionHelper.SetProperty(newTask, "OutlineLevel", (short)(level + 1));
            else
                ReflectionHelper.SetProperty(newTask, "OutlineLevel", (short)level);
            ReflectionHelper.SetProperty(newTask, "Name", headLine.Trim());
            ReflectionHelper.SetProperty(newTask, "Start", fromDate.Date);
            ReflectionHelper.SetProperty(newTask, "Finish", toDate.Date);

            AssignDoerResource(project, newTask, TaskUtils.DefineUserName(users, doer));
            string notes = String.Format(
                textTemplate,
                taskID,
                TaskUtils.DefineUserName(users, owner),
                TaskUtils.DefineUserName(users, curator),
                job
            );

            ReflectionHelper.SetProperty(newTask, "Notes", notes);
               
            // устанавиваем стиль для диаграммы Ганта
            int ID = Convert.ToInt32(ReflectionHelper.GetProperty(newTask, "ID"));

            ReflectionHelper.CallMethod(app, "GanttBarFormat", 
                ID, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, 5, Missing.Value, Missing.Value, 
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            // рекурсивно обрабатываем все подчиненные
            if ((childTasks != null) && (childTasks.Count > 0))
            {
                for (int i = 0; i < childTasks.Count; i++)
                {
                    XmlNode childTask = childTasks[i];
                    ProcessTask(childTask, project, app, newTask, i, level + 1, users);
                }
            }
        }

        /// <summary>
        /// Создание и назначаение ресурса для исполнителя
        /// </summary>
        /// <param name="project">Проект</param>
        /// <param name="task">Pадача</param>
        /// <param name="userName">Исполнитель</param>
        private static void AssignDoerResource(object project, object task, string userName)
        {
            object newRes = null;
            object resources = ReflectionHelper.GetProperty(project, "Resources");
            int resCount = Convert.ToInt32(ReflectionHelper.GetProperty(resources, "Count"));
            for (int i = 1; i <= resCount; i++)
            {
                object curRes = ReflectionHelper.GetProperty(resources, "Item", i);
                string name = ReflectionHelper.GetProperty(curRes, "Name").ToString();
                if (name == userName)
                {
                    newRes = curRes;
                    break;
                }
            }

            if (newRes == null)
                newRes = ReflectionHelper.CallMethod(resources, "Add", userName, Missing.Value);

            object assignments = ReflectionHelper.GetProperty(newRes, "Assignments");

            ReflectionHelper.CallMethod(assignments, "Add", 
                ReflectionHelper.GetProperty(task, "ID"),
                ReflectionHelper.GetProperty(newRes, "ID"),
                Missing.Value);
        }
    }

}