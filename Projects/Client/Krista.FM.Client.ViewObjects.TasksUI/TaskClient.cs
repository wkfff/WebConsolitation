using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using System.IO;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Common.Tasks;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.TasksUI
{
    public delegate void ReloadDelegate();

    //[ComVisible(true)]
    public sealed class TaskStub : TaskContext, ITask
    {
        #region ������������/�����������

        private TaskStubManager _parentManager = null;
        internal DataRow _taskRow = null;
        private int _taskID;

        internal TaskStub(TaskStubManager parentManager, ITask task, DataRow taskRow)
            : base(task)
        {
            _parentManager = parentManager;
            _taskRow = taskRow;
            _taskID = Convert.ToInt32(_taskRow[TasksNavigationTableColumns.ID_COLUMN]);
        }

        internal TaskStub(TaskStubManager parentManager, DataRow taskRow)
            : base(parentManager.ParentScheme.TaskManager.Tasks[Convert.ToInt32(taskRow[TasksNavigationTableColumns.ID_COLUMN])])
        {
            _parentManager = parentManager;
            _taskRow = taskRow;
            _taskID = Convert.ToInt32(_taskRow[TasksNavigationTableColumns.ID_COLUMN]);
        }

        #endregion

        #region IUpdatedDBObject
        public bool IsNew 
        {
            get { return TaskProxy.IsNew; }
        }

        public bool InEdit 
        {
            get { return TaskProxy.InEdit; }
        }

        public void BeginUpdate(string action)
        {
            TaskProxy.BeginUpdate(action);
        }

        public void EndUpdate()
        {
            TaskProxy.EndUpdate();
        }

        public void CancelUpdate()
        {
            TaskProxy.CancelUpdate();
        }
        #endregion

        #region ITask
        public int ID 
        {
            get { return _taskID; }
        }

        // ��������� ������
        public string State 
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.STATE_COLUMN]); }
            set { TaskProxy.State = value; }
        }

        public string CashedAction 
        {
            get { return TaskProxy.CashedAction; } 
        }

        public DateTime FromDate 
        {
            get { return Convert.ToDateTime(_taskRow[TasksNavigationTableColumns.FROMDATE_COLUMN]); }
            set { TaskProxy.FromDate = value; } 
        }

        public DateTime ToDate 
        {
            get { return Convert.ToDateTime(_taskRow[TasksNavigationTableColumns.TODATE_COLUMN]); }
            set { TaskProxy.ToDate = value; } 
        }

        public int Doer 
        {
            get { return Convert.ToInt32(_taskRow[TasksNavigationTableColumns.DOER_COLUMN]); }
            set { TaskProxy.Doer = value; } 
        }

        public int Owner 
        {
            get { return Convert.ToInt32(_taskRow[TasksNavigationTableColumns.OWNER_COLUMN]); }
            set { TaskProxy.Owner = value; } 
        }

        public int Curator 
        {
            get { return Convert.ToInt32(_taskRow[TasksNavigationTableColumns.CURATOR_COLUMN]); }
            set { TaskProxy.Curator = value; } 
        }

        public string Headline 
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.HEADLINE_COLUMN]); }
            set 
            { 
                if (!string.IsNullOrEmpty(value))
                    TaskProxy.Headline = value; 
            }
        }

        public string Job 
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.JOB_COLUMN]); }
            set 
            {
                if (!string.IsNullOrEmpty(value))
                    TaskProxy.Job = value; 
            }
        }

        public string Description 
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.DESCRIPRION_COLUMN]); }
            set { TaskProxy.Description = value; } 
        }

        public int RefTasks 
        {
            get 
            {
                if (DBNull.Value == _taskRow[TasksNavigationTableColumns.REFTASKS_COLUMN])
                    return -1;
                else
                    return Convert.ToInt32(_taskRow[TasksNavigationTableColumns.REFTASKS_COLUMN]); 
            }

            //set { TaskProxy.RefTasks = value; }
        }

        public int LockByUser 
        {
            get 
            {
                return TaskProxy.LockByUser;
                /*
                object val = _taskRow[TasksNavigationTableColumns.LOCKBYUSER_COLUMN];
                if (val == DBNull.Value)
                    return -1;
                else
                    return Convert.ToInt32(val); */
            }
            set { TaskProxy.LockByUser = value; } 
        }

        public int RefTasksTypes 
        { 
            get { return Convert.ToInt32(_taskRow[TasksNavigationTableColumns.REFTASKSTYPES_COLUMN]); }
            set { TaskProxy.RefTasksTypes = value; }
        }

        public DataTable GetTaskHistory()
        {
            return TaskProxy.GetTaskHistory();
        }

        public void SaveStateIntoDatabase()
        {
            TaskProxy.SaveStateIntoDatabase();
        }

        public void SetParentTask(int? parentId)
        {
            TaskProxy.SetParentTask(parentId);
        }

        public bool PlacedInCacheOnly 
        {
            get { return TaskProxy.PlacedInCacheOnly; }
        }

        public bool LockByCurrentUser()
        {
            return TaskProxy.LockByCurrentUser();
        }

        public int GetNewDocumentID()
        {
            return TaskProxy.GetNewDocumentID();
        }

        public IDataUpdater GetTaskDocumentsAdapter()
        {
            return TaskProxy.GetTaskDocumentsAdapter();
        }

        public ulong GetDocumentCRC32(int documentID)
        {
            return TaskProxy.GetDocumentCRC32(documentID);
        }

        public byte[] GetDocumentData(int documentID)
        {
            return TaskProxy.GetDocumentData(documentID);
        }

        public void SetDocumentData(int documentID, Stream documentData)
        {
            throw new Exception("����� ����� �� ��������������");
        }

        public void SetDocumentData(int documentID, byte[] documentData)
        {
            TaskProxy.SetDocumentData(documentID, documentData);
        }

        public bool CheckPermission(int operation, bool raiseException)
        {
            return TaskProxy.CheckPermission(operation, raiseException);
        }

        public string[] GetActionsForState(string stateCaption)
        {
            return TaskProxy.GetActionsForState(stateCaption);
        }
        #endregion

        #region �����������
        public string DoerName
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.DOERNAME_COLUMN]); }
        }

        public string OwnerName
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.OWNERNAME_COLUMN]); }
        }

        public string CuratorName
        {
            get { return Convert.ToString(_taskRow[TasksNavigationTableColumns.CURATORNAME_COLUMN]); }
        }
        #endregion

        public void RefreshNavigationRow()
        {
            //DataRow row = FindNavigationRowByID(this.ID);
            IUsersManager um = _parentManager.ParentScheme.UsersManager;
            if (_taskRow == null)
                throw new Exception("�� ������ ������ � ������� ��� ������");

            if (_taskRow.RowState == DataRowState.Detached)
                return;
            _taskRow.BeginEdit();
            try
            {
                _taskRow[TasksNavigationTableColumns.ID_COLUMN] = TaskProxy.ID;
                _taskRow[TasksNavigationTableColumns.STATE_COLUMN] = TaskProxy.State;
                _taskRow[TasksNavigationTableColumns.HEADLINE_COLUMN] = TaskProxy.Headline;
                _taskRow[TasksNavigationTableColumns.TODATE_COLUMN] = TaskProxy.ToDate;
                _taskRow[TasksNavigationTableColumns.FROMDATE_COLUMN] = TaskProxy.FromDate;
                _taskRow[TasksNavigationTableColumns.OWNER_COLUMN] = TaskProxy.Owner;
                _taskRow[TasksNavigationTableColumns.DOER_COLUMN] = TaskProxy.Doer;
                _taskRow[TasksNavigationTableColumns.CURATOR_COLUMN] = TaskProxy.Curator;
                _taskRow[TasksNavigationTableColumns.REFTASKSTYPES_COLUMN] = TaskProxy.RefTasksTypes;
                _taskRow[TasksNavigationTableColumns.LOCKBYUSER_COLUMN] = TaskProxy.LockByUser;
                _taskRow[TasksNavigationTableColumns.JOB_COLUMN] = TaskProxy.Job;
                _taskRow[TasksNavigationTableColumns.DESCRIPRION_COLUMN] = TaskProxy.Description;
                // new
                _taskRow[TasksNavigationTableColumns.DOERNAME_COLUMN] = um.GetUserNameByID(TaskProxy.Doer);
                _taskRow[TasksNavigationTableColumns.OWNERNAME_COLUMN] = um.GetUserNameByID(TaskProxy.Owner);
                _taskRow[TasksNavigationTableColumns.CURATORNAME_COLUMN] = um.GetUserNameByID(TaskProxy.Curator);
                _taskRow[TasksNavigationTableColumns.LOCKEDUSERNAME_COLUMN] = um.GetUserNameByID(TaskProxy.LockByUser);
                _taskRow[TasksNavigationTableColumns.STATECODE_COLUMN] =
                    _parentManager.ParentScheme.TaskManager.Tasks.FindStateFromCaption(TaskProxy.State);
            }
            finally
            {
                _taskRow.EndEdit();
                _taskRow.AcceptChanges();
            }

            UltraGridRow gridrow = _parentManager.GetNavigationRowByID(ID);
            if (gridrow != null)
            {
                // ���� ������ ������������, ������� ��������������� ������ (� ��������� ������� ������ �� ������ �� �������������� ������)
                gridrow.Cells[TasksNavigationTableColumns.LOCKBYUSER_COLUMN].Value = TaskProxy.LockByUser;
                gridrow.Update();
                gridrow.Refresh(RefreshRow.FireInitializeRow);
            }
        }
    }

    public sealed class TaskStubManager
    {
        private Dictionary<int, TaskStub> returnedTasks = new Dictionary<int, TaskStub>();

        private IScheme _parentScheme = null;
        public IScheme ParentScheme
        {
            get { return _parentScheme; }
            set { _parentScheme = value; }
        }

        private DataTable _tasksTable = null;
        public DataTable TasksTable
        {
            get { return _tasksTable; }
            set 
            {
                //Clear();
                _tasksTable = value; 
            }
        }

        private UltraGrid _tasksGrid = null;
        public UltraGrid TasksGrid
        {
            get { return _tasksGrid; }
            set 
            {
                _cashedLastRow = null;
                _tasksGrid = value; 
            }
        }

        public TaskStub AddNew(int? parentID)
        {
            // ��������� ������ �� �������
            ITask taskProxy;
            if (parentID == null)
                taskProxy = ParentScheme.TaskManager.Tasks.AddNew();
            else
                taskProxy = ParentScheme.TaskManager.Tasks.AddNew((int)parentID);
            
            if (taskProxy == null)
                return null;

            // ������� ������ ��� ������
            DataRow taskRow = TasksTable.NewRow();
            taskRow[TasksNavigationTableColumns.ID_COLUMN] = taskProxy.ID;
            if (parentID != null)
                taskRow[TasksNavigationTableColumns.REFTASKS_COLUMN] = parentID;
            TasksGrid.EventManager.SetEnabled(EventGroups.AllEvents, false);
            TasksTable.Rows.Add(taskRow);
            TasksGrid.EventManager.SetEnabled(EventGroups.AllEvents, true);

            // ������� ������� 
            TaskStub taskClient = new TaskStub(this, taskProxy, taskRow);
            returnedTasks.Add(taskClient.ID, taskClient);
            // ��������� ����
            taskClient.RefreshNavigationRow();
            // ��������� ���� �������� ��������� �������
            taskRow[TasksNavigationTableColumns.VISIBLE_COLUMN] = (int)TaskVisibleInNavigation.tvInvsible;
            taskRow[TasksNavigationTableColumns.STATECODE_COLUMN] = (int)TaskStates.tsCreated;
            taskRow.AcceptChanges();
            //TasksTable.AcceptChanges();
            return taskClient;
        }

        public bool DeleteTask(TaskStub task)
        {
            // ������� � �������
            if (!task.PlacedInCacheOnly && task.InEdit)
                task.CancelUpdate();
            try
            {
                int deletedId = task.ID;
                returnedTasks.Remove(deletedId);
                // ������� ������ �� ������, ������� ��������� � �������� ������� ��� �����
                if (!task.PlacedInCacheOnly)
                    ParentScheme.TaskManager.Tasks.DeleteTask(task.ID);
                //TasksTable.Rows.Remove(task._taskRow);
                //task.Dispose();
                return true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace, "������ �������� ������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(e.Message/* + Environment.NewLine + e.StackTrace*/, "������ �������� ������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                task.RefreshNavigationRow();
                return false;
            }
        }

        public bool Contains(int taskID)
        {
            return returnedTasks.ContainsKey(taskID);
        }

        #region ���������������� ��������� ������ �����
        private UltraGridRow _cashedLastRow = null;

        private static void GetCompareParams(UltraGridBand band, object value, out IComparable cmp, out UltraGridColumn idClmn)
        {
            idClmn = band.Columns["ID"];
            value = Convert.ChangeType(value, idClmn.DataType);
            cmp = value as IComparable;
        }

        private bool CheckCashedRow(object id)
        {
            // ���� �������������� ������ ��������� - ���������� false
            if ((_cashedLastRow == null) ||
                (_cashedLastRow.IsDeleted))
                return false;
            // ����� - ��������� ID
            IComparable cmp;
            UltraGridColumn clmn;
            GetCompareParams(_cashedLastRow.Band, id, out cmp, out clmn);
            return (cmp.CompareTo(_cashedLastRow.Cells[clmn].Value) == 0);
        }

        private void BuildTaskHierarchyList(List<TaskStub> hierarchyList, int childID)
        {
            // ��������� � ������ ������� ������
            TaskStub curTask = this[childID];
            hierarchyList.Add(curTask);
            // ���� ��� ������ �������� ������ - ������ ����� �� ������
            int parentID = curTask.RefTasks;
            if (parentID == -1)
                return;
            // ����� - ���������� ���������� ����������� �����
            BuildTaskHierarchyList(hierarchyList, parentID);
        }

        private UltraGridRow GetTopLevelNavigationRow(object id)
        {
            UltraGridRow result = null;
            // �������� ��������� ��� ���������
            IComparable cmp;
            UltraGridColumn clmn;
            GetCompareParams(TasksGrid.DisplayLayout.Bands[0], id, out cmp, out clmn);
            // ����� �� ���� �������
            foreach (UltraGridRow row in TasksGrid.DisplayLayout.Rows.All)
            {
                // ���� ������ ��������� ��� ������������� - �� ���������
                if ((!row.IsDataRow) || (row.IsFilteredOut))
                    continue;
                // ��������� ID
                if (cmp.CompareTo(row.Cells[clmn].Value) == 0)
                {
                    result = row;
                    break;
                }
            }
            return result;
        }

        private UltraGridRow findedRow = null;
        private object findedID = null;

        private bool CheckChildRow(UltraGridRow row)
        {
            IComparable cmp;
            UltraGridColumn clmn;
            GetCompareParams(row.Band, findedID, out cmp, out clmn);
            if (cmp.CompareTo(row.Cells[clmn].Value) == 0)
            {
                findedRow = row;
                return true;
            }
            else
                return false;
        }

        private UltraGridRow GetChildNavigationRow(List<TaskStub> taskHierarchy, UltraGridRow curParentRow)
        {
            findedID = taskHierarchy[0].ID;
            UltraGridHelper.EnumGridRows((UltraGrid)curParentRow.Band.Layout.Grid, curParentRow, true, new UltraGridHelper.CheckRowDelegate(CheckChildRow));
            findedID = null;
            return findedRow;
        }

        
        public UltraGridRow GetNavigationRowByID(object id)
        {
            // ������� �������� �������������� ������ 
            if (CheckCashedRow(id))
                return _cashedLastRow;

            // ������ �������� ����� �� ������������ �������� ������
            List<TaskStub> taskHierarchy = new List<TaskStub>();
            BuildTaskHierarchyList(taskHierarchy, Convert.ToInt32(id));
            if (taskHierarchy.Count == 0)
                throw new Exception("���������� ������: �� ������� ��������� �������� ��� ������");
            // �������� ������ ����� �������� ������
            UltraGridRow topLevelRow = GetTopLevelNavigationRow(taskHierarchy[taskHierarchy.Count - 1].ID);
            if (topLevelRow == null && !UltraGridHelper.GridInGroupByMode(TasksGrid))
                throw new Exception("���������� ������: �� ������� ����� ������������ ������ ������� ������");
            // ���� ������� ������ � ��� ��������� �� ������� ������ - ������ ����� �� ������
            if (taskHierarchy.Count == 1)
                return topLevelRow;

            // ����� - ���������� ���� �� �������� ����������, ���� ���� ������
            _cashedLastRow = GetChildNavigationRow(taskHierarchy, topLevelRow);
            taskHierarchy.Clear();
            findedRow = null;
            return _cashedLastRow;
        }
        #endregion

        public void Clear()
        {
            foreach (TaskStub stub in returnedTasks.Values)
                stub.Dispose();
            returnedTasks.Clear();
        }

        public TaskStub this[int taskID]
        {
            get 
            { 
                // ���� ���� ��� ������ - ���������� ���
                if (returnedTasks.ContainsKey(taskID))
                    return returnedTasks[taskID];

                // ���� ��� - �������
                DataRow taskRow = TasksTable.Rows.Find(taskID);
                if (taskRow == null)
                    throw new Exception(String.Format("�� ������� ������ � ��������� ��� ������ (ID={0})", taskID));
                TaskStub taskStub = new TaskStub(this, taskRow);
                returnedTasks.Add(taskStub.ID, taskStub);
                return taskStub; 
            }
        }
    }
}