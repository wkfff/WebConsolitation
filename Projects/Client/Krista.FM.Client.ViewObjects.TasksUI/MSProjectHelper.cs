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
    /// ����� ��� �������� ����� � MSProject
    /// �������������� ������ 2003 ������
    /// </summary>
    public sealed class MSProjectReportHelper
    {
        // �������������� ������ MSProject
        private static string MSProjectAllowedVersion = "MSProject.Application.11";

        /// <summary>
        /// ����� �������� ����������� MSProject ������ ������ (2003)
        /// </summary>
        /// <returns>��������� ��������</returns>
        public static bool MSProjectInstalled()
        {
            // ��������� ����������� � ������� � ������� ����������
            bool res = Utils.CheckLibByProgID("MSProject.Application", false);
            // ��������� ������
            if (res)
            {
                RegistryKey key = Registry.ClassesRoot.OpenSubKey("MSProject.Application\\CurVer", false);
                string curVer = (string)key.GetValue(String.Empty);
                key.Close();
                res = MSProjectAllowedVersion.ToUpper() == curVer.ToUpper();
            }
            return res;
        }

        // ������ ��� �������
        private static string textTemplate =
            "ID: {0}" + Environment.NewLine +
            "��������: {1}" + Environment.NewLine +
            "�������: {2}" + Environment.NewLine +
            "�������: {3}";

        /// <summary>
        /// ����� �������� ���������� ����� � MSProject2003
        /// </summary>
        /// <param name="workplace">��������� workplace</param>
        /// <param name="ug">UltraGrid c ����������� ��������</param>
        public static void CreateTasksReport(IWorkplace workplace, UltraGrid ug)
        {
            try
            {
                // ����-�� ���������� ������?
                if ((ug.Selected.Rows == null) || (ug.Selected.Rows.Count == 0))
                {
                    MessageBox.Show("���������� �������� ������ ������", "��� ����� ��� ��������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Application.DoEvents();

                // ��������� ������ ���������� ID
                List<int> selectedID;
                UltraGridHelper.GetSelectedIDs(ug, out selectedID);

                // ���������� ��������
                workplace.OperationObj.Text = "�������� ������ MS Project";
                workplace.OperationObj.StartOperation();
                string errStr = String.Empty;
                // �������� XML �������� � ��������������� �������
                XmlDocument xmlData = TasksExportHelper.GetTasksListXml(workplace, selectedID, TaskExportType.teIncludeChild, ref errStr);
                // ���� ��� ������ ��� ������ - �������� ������������ ������
                if ((errStr == null) || (errStr != String.Empty))
                {
                    MessageBox.Show("��������� ������: " + errStr, "������� ���������� ��������", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // ������� ����������
                    object app = OfficeHelper.CreateMsProjectApplication();
                    // ��������� ��������������
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", false);
                    // ������� �����
                    BuildMSProjectReport(workplace, xmlData, app);
                    workplace.OperationObj.StopOperation();
                    // �������� ��������������
                    ReflectionHelper.SetProperty(app, "ScreenUpdating", true);
                    // ���������� ����������
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
        /// ������� ������ �������� MS Word � ��������� ��� �� ���������� ������
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
        /// ������� ����� MSProject
        /// </summary>
        /// <param name="workplace">��������� workplace</param>
        /// <param name="tasks">XmlDocument c ��������</param>
        /// <param name="app">���������� MSProject</param>
        private static void BuildMSProjectReport(IWorkplace workplace, XmlDocument tasks, object app)
        {
            // ������� ����� ������
            ReflectionHelper.CallMethod(app, "FileNew", Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            object project = ReflectionHelper.GetProperty(app, "ActiveProject");

            // �������� ����������� ����
            XmlNodeList allTasks = tasks.SelectNodes(String.Format("//{0}", TasksExportXmlConsts.TaskTagName));
            DateTime minDate = DateTime.Now;
            foreach (XmlNode task in allTasks)
            {
                DateTime curDate = Convert.ToDateTime(XmlHelper.GetStringAttrValue(task, TasksExportXmlConsts.TaskFromDateTagName, String.Empty));
                if (curDate < minDate)
                    minDate = curDate;
            }
            // ������������� �� � �������� ��������� ��� �������
            ReflectionHelper.SetProperty(project, "ProjectStart", minDate);
            // �������� ������� �������������
            DataTable users = workplace.ActiveScheme.UsersManager.GetUsers();
            // �������� ������ ����� �������� ������
            XmlNodeList rootTasks = tasks.SelectNodes(String.Format("{0}/{1}/{2}", TasksExportXmlConsts.RootNodeTagName, TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            for (int i = 0; i < rootTasks.Count; i++)
            {
                // ���������� ������������ ������
                ProcessTask(rootTasks[i], project, app, null, i, 1, users);
            }
            ReflectionHelper.CallMethod(app, "CalculateAll");
        }

        /// <summary>
        /// ����������� ��������� �������� ������ � ���� �� ��������
        /// </summary>
        /// <param name="taskNode">XmlNode c ����������� ������</param>
        /// <param name="project">������ MSProject</param>
        /// <param name="app">���������� MSProject</param>
        /// <param name="parentTask">������������ ������ (��� �������� ������ == null)</param>
        /// <param name="childIndex">���������� ����� �������� ������</param>
        /// <param name="level">������� ������� ��������</param>
        /// <param name="users">������� �������������</param>
        private static void ProcessTask(XmlNode taskNode, object project, object app,
            object parentTask, int childIndex, int level, DataTable users)
        {
            // �������� ��������� ������
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
            // �������� ������ ����������� �����
            XmlNodeList childTasks = taskNode.SelectNodes(String.Format("{0}/{1}", TasksExportXmlConsts.TasksTagName, TasksExportXmlConsts.TaskTagName));
            // ���� ���� ����������� - ����� �������� ��������� ������ �������� ������
            if ((childTasks != null) && (childTasks.Count > 0))
            {
                newTask = ReflectionHelper.CallMethod(tasks, "Add", Missing.Value, Missing.Value);
                ReflectionHelper.SetProperty(newTask, "OutlineLevel", (short)level);
                ReflectionHelper.SetProperty(newTask, "Name", headLine.Trim());
                ReflectionHelper.SetProperty(newTask, "Start", fromDate);
                ReflectionHelper.SetProperty(newTask, "Finish", toDate);
            }
            // ������ ��������� ������� ������
            newTask = ReflectionHelper.CallMethod(tasks, "Add", Missing.Value, Missing.Value);
            if ((childTasks != null) && (childTasks.Count > 0))
                // ���� ���� ����������� - ������ ���� �� ���� ������� � ����
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
               
            // ������������ ����� ��� ��������� �����
            int ID = Convert.ToInt32(ReflectionHelper.GetProperty(newTask, "ID"));

            ReflectionHelper.CallMethod(app, "GanttBarFormat", 
                ID, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, 5, Missing.Value, Missing.Value, 
                Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            // ���������� ������������ ��� �����������
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
        /// �������� � ����������� ������� ��� �����������
        /// </summary>
        /// <param name="project">������</param>
        /// <param name="task">P�����</param>
        /// <param name="userName">�����������</param>
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