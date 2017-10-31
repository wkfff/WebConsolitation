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
    #region Состояния/операции
    /// <summary>
    /// Абстрактный предок для событий состояний, содержит индекс и название объекта.
    /// </summary>
    public abstract class Entity
    {
        public int Index;
        public string Caption;

        // скрытый конструктор по умолчанию (без параметров)
        internal Entity()
        {
        }

        /// <summary>
        /// Конструктор объекта
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <param name="caption">Название</param>
        public Entity(int index, string caption)
        {
            Index = index;
            Caption = caption;
        }
    }

    /// <summary>
    /// Класс описывающий одно состояние задачи
    /// </summary>
    public class TaskState : Entity
    {
        /// <summary>
        /// Допустимые для состояния действия
        /// </summary>
        public TaskActions[] AllowedActions;

        public TaskActions[] AllowedAdministratorActions;

        // Конструктор
        public TaskState(int index, string caption)
            : base(index, caption)
        {
        }
    }

    /// <summary>
    /// Класс описывающий одно действие над задачей
    /// </summary>
    public class TaskAction : Entity
    {
        // состояние в которое задача переходит после действия
        public TaskStates TaskStateAfter;

        // конструктор
        public TaskAction(int index, string caption)
            : base(index, caption)
        {
        }
    }

    /// <summary>
    /// Менеджер действий и состояний
    /// </summary>
    public class TaskActionManager
    {
        /// <summary>
        /// Возможные действия
        /// </summary>
        public Dictionary<TaskActions, TaskAction> Actions;

        /// <summary>
        /// Возможные состояния
        /// </summary>
        public Dictionary<TaskStates, TaskState> States;

        /// <summary>
        /// Конструктор класса.
        /// Создание и настройка состояний и действий.
        /// Пока жестко зашито в коде, потом будет браться их XML.
        /// </summary>
        public TaskActionManager()
        {
            #region Настройка состояний
            States = new Dictionary<TaskStates, TaskState>();
            TaskState tmpState;

            tmpState = new TaskState((int)TaskStates.tsCreated, "Создана");
            tmpState.AllowedActions = new TaskActions[3] {
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taAssign
			};
            States.Add(TaskStates.tsCreated, tmpState);

            tmpState = new TaskState((int)TaskStates.tsAssigned, "Назначена");
            tmpState.AllowedActions = new TaskActions[3] {
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taExecute
			};
            States.Add(TaskStates.tsAssigned, tmpState);

            tmpState = new TaskState((int)TaskStates.tsExecuted, "Выполняется");
            tmpState.AllowedActions = new TaskActions[4] {
				TaskActions.taEdit,
				TaskActions.taDelete,
                TaskActions.taContinueExecute,
				TaskActions.taOnCheck
			};
            States.Add(TaskStates.tsExecuted, tmpState);

            tmpState = new TaskState((int)TaskStates.tsOnCheck, "На проверку");
            tmpState.AllowedActions = new TaskActions[3]{
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taCheck,
			};
            States.Add(TaskStates.tsOnCheck, tmpState);

            tmpState = new TaskState((int)TaskStates.tsCheckInProgress, "Проверяется");
            tmpState.AllowedActions = new TaskActions[5]{
				TaskActions.taEdit,
				TaskActions.taDelete,
				TaskActions.taContinueCheck,
                TaskActions.taCheckWithErrors,
                TaskActions.taCheckWithoutErrors
			};
            States.Add(TaskStates.tsCheckInProgress, tmpState);

            tmpState = new TaskState((int)TaskStates.tsFinisned, "Завершена");
            tmpState.AllowedActions = new TaskActions[5] {
				TaskActions.taEdit,
				TaskActions.taDelete,
                TaskActions.taBackToCheck,
                TaskActions.taBackToRework,
                TaskActions.taClose
			};
            States.Add(TaskStates.tsFinisned, tmpState);

            tmpState = new TaskState((int)TaskStates.tsClosed, "Закрыта");
            tmpState.AllowedActions = new TaskActions[1] {
				TaskActions.taDelete
			};
            tmpState.AllowedAdministratorActions = new TaskActions[2] {
				TaskActions.taDelete,
                TaskActions.taBackToRework
			};
            States.Add(TaskStates.tsClosed, tmpState);

            #endregion

            #region Настройка действий
            Actions = new Dictionary<TaskActions, TaskAction>();
            TaskAction tmpAction;

            tmpAction = new TaskAction((int)TaskActions.taCreate, "Создать");
            tmpAction.TaskStateAfter = TaskStates.tsCreated;
            Actions.Add(TaskActions.taCreate, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taEdit, "Редактировать");
            tmpAction.TaskStateAfter = TaskStates.tsUndefined;
            Actions.Add(TaskActions.taEdit, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taDelete, "Удалить");
            tmpAction.TaskStateAfter = TaskStates.tsUndefined;
            Actions.Add(TaskActions.taDelete, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taAssign, "Назначить");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taAssign, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taExecute, "Выполнять");
            tmpAction.TaskStateAfter = TaskStates.tsExecuted;
            Actions.Add(TaskActions.taExecute, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taContinueExecute, "Продолжить выполнение");
            tmpAction.TaskStateAfter = TaskStates.tsExecuted;
            Actions.Add(TaskActions.taContinueExecute, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taOnCheck, "На проверку");
            tmpAction.TaskStateAfter = TaskStates.tsOnCheck;
            Actions.Add(TaskActions.taOnCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheck, "Проверять");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taContinueCheck, "Продолжить проверку");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taContinueCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheckWithErrors, "Вернуть на доработку");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taCheckWithErrors, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taCheckWithoutErrors, "Успешно проверено");
            tmpAction.TaskStateAfter = TaskStates.tsFinisned;
            Actions.Add(TaskActions.taCheckWithoutErrors, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taBackToCheck, "Вернуть на проверку");
            tmpAction.TaskStateAfter = TaskStates.tsCheckInProgress;
            Actions.Add(TaskActions.taBackToCheck, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taBackToRework, "Вернуть на доработку");
            tmpAction.TaskStateAfter = TaskStates.tsAssigned;
            Actions.Add(TaskActions.taBackToRework, tmpAction);

            tmpAction = new TaskAction((int)TaskActions.taClose, "Закрыть");
            tmpAction.TaskStateAfter = TaskStates.tsClosed;
            Actions.Add(TaskActions.taClose, tmpAction);

            #endregion
        }

        /// <summary>
        /// Идентифицировать действие по заголовку
        /// </summary>
        /// <param name="actionCaption">Заголовок действия</param>
        /// <returns>Действие</returns>
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
        /// Идентифицировать состояние по заголовку
        /// </summary>
        /// <param name="stateCaption">Заголовок состояния</param>
        /// <returns>Состояние</returns>
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
        /// Получить список действия доступных для данного состояния
        /// </summary>
        /// <param name="stateCaption">Заголовок состояния</param>
        /// <returns>Список действий</returns>
        public TaskActions[] GetActionsForState(string stateCaption)
        {
            TaskState state = FindStateFromCaption(stateCaption);

            if (state == null)
                throw new Exception("Состояние '" + stateCaption + "' не найдено");

            return state.AllowedActions;
        }

        /// <summary>
        /// Получить список действия доступных для данного состояния для администратора
        /// </summary>
        /// <param name="stateCaption">Заголовок состояния</param>
        /// <param name="isAdminisrtator">Администратор</param>
        /// <returns>Список действий</returns>
        public TaskActions[] GetActionsForState(string stateCaption, bool isAdminisrtator)
        {
            TaskState state = FindStateFromCaption(stateCaption);

            if (state == null)
                throw new Exception("Состояние '" + stateCaption + "' не найдено");
            if (isAdminisrtator && state.AllowedAdministratorActions != null)
                return state.AllowedAdministratorActions;
            return state.AllowedActions;
        }

        /// <summary>
        /// Получить состояние после выполнения действия
        /// </summary>
        /// <param name="actionCaption">Заголовок действия</param>
        /// <returns>Заголовок состояния</returns>
        public string GetStateAfterAction(string actionCaption)
        {
            TaskAction action = FindActionFromCaption(actionCaption);

            if (action == null)
                throw new Exception("Действие '" + actionCaption + "' не найдено");

            if (action.TaskStateAfter == TaskStates.tsUndefined)
                return String.Empty;
            else
                return States[action.TaskStateAfter].Caption;
        }

        /// <summary>
        /// Получить список всех возможных состояний
        /// </summary>
        /// <returns>Заголовки состояний</returns>
        public string[] GetAllStatesCaptions()
        {
            Array allStates = Enum.GetValues(typeof(TaskStates));
            // первый элемент перечисления в коллекцию не входит
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