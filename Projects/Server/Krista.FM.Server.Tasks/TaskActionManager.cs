using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;


namespace Krista.FM.Server.Tasks
{
    #region ���������/��������
    /// <summary>
    /// ����������� ������ ��� ������� ���������, �������� ������ � �������� �������.
    /// </summary>
    public abstract class Entity
    {
        public int Index;
        public string Caption;

        // ������� ����������� �� ��������� (��� ����������)
        internal Entity()
        {
        }

        /// <summary>
        /// ����������� �������
        /// </summary>
        /// <param name="index">������</param>
        /// <param name="caption">��������</param>
        public Entity(int index, string caption)
        {
            Index = index;
            Caption = caption;
        }
    }

    /// <summary>
    /// ����� ����������� ���� ��������� ������
    /// </summary>
    public class TaskState : Entity
    {
        /// <summary>
        /// ���������� ��� ��������� ��������
        /// </summary>
        public TaskActions[] AllowedActions;

        public TaskActions[] AllowedAdministratorActions;

        // �����������
        public TaskState(int index, string caption)
            : base(index, caption)
        {
        }
    }

    /// <summary>
    /// ����� ����������� ���� �������� ��� �������
    /// </summary>
    public class TaskAction : Entity
    {
        // ��������� � ������� ������ ��������� ����� ��������
        public TaskStates TaskStateAfter;

        // �����������
        public TaskAction(int index, string caption)
            : base(index, caption)
        {
        }
    }

    /// <summary>
    /// �������� �������� � ���������
    /// </summary>
    public class TaskActionManager
    {
        /// <summary>
        /// ��������� ��������
        /// </summary>
        public Dictionary<TaskActions, TaskAction> Actions;

        /// <summary>
        /// ��������� ���������
        /// </summary>
        public Dictionary<TaskStates, TaskState> States;

        /// <summary>
        /// ����������� ������.
        /// �������� � ��������� ��������� � ��������.
        /// ���� ������ ������ � ����, ����� ����� ������� �� XML.
        /// </summary>
        public TaskActionManager()
        {
            #region ��������� ���������
            States = new Dictionary<TaskStates, TaskState>();
            TaskState tmpState;

            tmpState = new TaskState((int)TaskStates.tsCreated, "�������");
            tmpState.AllowedActions = new TaskActions[3] {
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taAssign
			};
            States.Add(TaskStates.tsCreated, tmpState);

            tmpState = new TaskState((int)TaskStates.tsAssigned, "���������");
            tmpState.AllowedActions = new TaskActions[3] {
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taExecute
			};
            States.Add(TaskStates.tsAssigned, tmpState);

            tmpState = new TaskState((int)TaskStates.tsExecuted, "�����������");
            tmpState.AllowedActions = new TaskActions[4] {
				TaskActions.taEdit,
				TaskActions.taDelete,
                TaskActions.taContinueExecute,
				TaskActions.taOnCheck
			};
            States.Add(TaskStates.tsExecuted, tmpState);

            tmpState = new TaskState((int)TaskStates.tsOnCheck, "�� ��������");
            tmpState.AllowedActions = new TaskActions[3]{
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taCheck,
			};
            States.Add(TaskStates.tsOnCheck, tmpState);

            tmpState = new TaskState((int)TaskStates.tsCheckInProgress, "�����������");
            tmpState.AllowedActions = new TaskActions[5]{
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taContinueCheck,
                TaskActions.taCheckWithErrors,
                TaskActions.taCheckWithoutErrors
			};
            States.Add(TaskStates.tsCheckInProgress, tmpState);

            tmpState = new TaskState((int)TaskStates.tsFinisned, "���������");
            tmpState.AllowedActions = new TaskActions[5] {
				TaskActions.taEdit,
				TaskActions.taDelete,
                TaskActions.taBackToCheck,
                TaskActions.taBackToRework,
                TaskActions.taClose
			};
            States.Add(TaskStates.tsFinisned, tmpState);

            tmpState = new TaskState((int)TaskStates.tsClosed, "�������");
            tmpState.AllowedActions = new TaskActions[1] {
				TaskActions.taDelete
			};
            tmpState.AllowedAdministratorActions = new TaskActions[2] {
				TaskActions.taDelete,
                TaskActions.taBackToRework
			};
            States.Add(TaskStates.tsClosed, tmpState);

            #endregion

            #region ��������� ��������
            Actions = new Dictionary<TaskActions, TaskAction>();
            TaskAction tmpAction;

            tmpAction = new TaskAction((int)TaskActions.taCreate, "�������");
            tmpAction.TaskStateAfter = TaskStates.tsCreated;
            Actions.Add(TaskActions.taCreate, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taEdit, "�������������");
            tmpAction.TaskStateAfter = TaskStates.tsUndefined;
            Actions.Add(TaskActions.taEdit, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taDelete, "�������");
            tmpAction.TaskStateAfter = TaskStates.tsUndefined;
            Actions.Add(TaskActions.taDelete, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taAssign, "���������");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taAssign, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taExecute, "���������");
            tmpAction.TaskStateAfter = TaskStates.tsExecuted;
            Actions.Add(TaskActions.taExecute, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taContinueExecute, "���������� ����������");
            tmpAction.TaskStateAfter = TaskStates.tsExecuted;
            Actions.Add(TaskActions.taContinueExecute, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taOnCheck, "�� ��������");
            tmpAction.TaskStateAfter = TaskStates.tsOnCheck;
            Actions.Add(TaskActions.taOnCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheck, "���������");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taContinueCheck, "���������� ��������");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taContinueCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheckWithErrors, "������� �� ���������");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taCheckWithErrors, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheckWithoutErrors, "������� ���������");
            tmpAction.TaskStateAfter = TaskStates.tsFinisned;
            Actions.Add(TaskActions.taCheckWithoutErrors, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taBackToCheck, "������� �� ��������");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taBackToCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taBackToRework, "������� �� ���������");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taBackToRework, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taClose, "�������");
            tmpAction.TaskStateAfter = TaskStates.tsClosed;
            Actions.Add(TaskActions.taClose, tmpAction);

            #endregion
        }

        /// <summary>
        /// ���������������� �������� �� ���������
        /// </summary>
        /// <param name="actionCaption">��������� ��������</param>
        /// <returns>��������</returns>
        public TaskAction FindActionFromCaption(string actionCaption)
        {
            TaskAction action = null;
            foreach (TaskAction item in Actions.Values)
            {
                if (string.Compare(actionCaption, item.Caption, true) == 0)
                {
                    action = item;
                    break;
                }
            }
            return action;
        }

        /// <summary>
        /// ���������������� ��������� �� ���������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <returns>���������</returns>
        public TaskState FindStateFromCaption(string stateCaption)
        {
            TaskState state = null;
            foreach (TaskState item in States.Values)
            {
                if (string.Compare(stateCaption, item.Caption, true) == 0)
                {
                    state = item;
                    break;
                }
            }
            return state;
        }

        /// <summary>
        /// �������� ������ �������� ��������� ��� ������� ���������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <returns>������ ��������</returns>
        public TaskActions[] GetActionsForState(string stateCaption)
        {
            TaskState state = FindStateFromCaption(stateCaption);

            if (state == null)
                throw new Exception("��������� '" + stateCaption + "' �� �������");

            return state.AllowedActions;
        }

        /// <summary>
        /// �������� ������ �������� ��������� ��� ������� ��������� ��� ��������������
        /// </summary>
        /// <param name="stateCaption">��������� ���������</param>
        /// <param name="isAdminisrtator">�������������</param>
        /// <returns>������ ��������</returns>
        public TaskActions[] GetActionsForState(string stateCaption, bool isAdminisrtator)
        {
            TaskState state = FindStateFromCaption(stateCaption);

            if (state == null)
                throw new Exception("��������� '" + stateCaption + "' �� �������");
            if (isAdminisrtator && state.AllowedAdministratorActions != null)
                return state.AllowedAdministratorActions;
            return state.AllowedActions;
        }

        /// <summary>
        /// �������� ��������� ����� ���������� ��������
        /// </summary>
        /// <param name="actionCaption">��������� ��������</param>
        /// <returns>��������� ���������</returns>
        public string GetStateAfterAction(string actionCaption)
        {
            TaskAction action = FindActionFromCaption(actionCaption);

            if (action == null)
                throw new Exception("�������� '" + actionCaption + "' �� �������");

            if (action.TaskStateAfter == TaskStates.tsUndefined)
                return String.Empty;
            else
                return States[action.TaskStateAfter].Caption;
        }

        /// <summary>
        /// �������� ������ ���� ��������� ���������
        /// </summary>
        /// <returns>��������� ���������</returns>
        public string[] GetAllStatesCaptions()
        {
            Array allStates = Enum.GetValues(typeof(TaskStates));
            // ������ ������� ������������ � ��������� �� ������
            string[] captions = new string[allStates.Length - 1];
            for (int i = 1; i < allStates.Length; i++)
            {
                captions[i - 1] = States[(TaskStates)allStates.GetValue(i)].Caption;
            }
            return captions;
        }

    }
    #endregion

}