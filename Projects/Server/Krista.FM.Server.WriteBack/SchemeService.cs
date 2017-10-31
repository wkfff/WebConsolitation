using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.ProcessorLibrary;

namespace Krista.FM.Server.WriteBack
{
    public class SchemeService : ISchemeService
    {
        private readonly IScheme scheme;

        public SchemeService(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public ICollection<IFactTable> GetFactTables()
        {
            return scheme.FactTables.Values;
        }

        public IEntityAssociationCollection GetAllAssociations()
        {
            return scheme.AllAssociations;
        }

        /// <summary>
        /// Возвращает классификатор по полному английскому имени.
        /// Если классификатор не найден, то возвращает null.
        /// </summary>
        public IClassifier GetSchemeClassifierByFullName(string clsFullName)
        {
            IClassifier classifier = null;
            foreach (IClassifier classifierItem in scheme.Classifiers.Values)
            {
                if (classifierItem.FullName == clsFullName)
                {
                    classifier = classifierItem;
                    break;
                }
            }
            return classifier;
        }

        /// <summary>
        /// Проверяет наличие ссылки из таблицы фактов на классификатор (Сопоставимые не учитываются).
        /// </summary>
        /// <param name="factTable">Таблица фактов</param>
        /// <param name="classifierName">Полное наименование классификатора</param>
        public bool ContainClassifierInFact(IEntity factTable, string classifierName)
        {
            if (scheme.Classifiers[classifierName].ClassType == ClassTypes.clsBridgeClassifier)
                return true;

            foreach (IAssociation item in factTable.Associations.Values)
            {
                if (item.RoleBridge.ObjectKey == classifierName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Проверка существования задачи и условий, 
        /// позволяющих запись в контексте этой задачи.
        /// </summary>
        public void CheckTask(int taskID)
        {
            ITask task;
            try
            {
                task = scheme.TaskManager.Tasks[taskID];
            }
            catch (PermissionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(
                    "Лист планирования прикреплён к несуществующей задаче (TaskID = {0}). " +
                    "Возможно задача находится в состоянии создания и еще не сохранена " +
                    "или указан неверный адрес веб-сервиса или имя схемы.",
                    taskID),
                    e);
            }

            TaskActions ta = scheme.TaskManager.Tasks.FindActionsFromCaption(task.CashedAction);
            if (scheme.TaskManager.Tasks.FindStateFromCaption(task.State) != TaskStates.tsExecuted && !(
                ta == TaskActions.taExecute ||
                ta == TaskActions.taEdit ||
                ta == TaskActions.taContinueExecute ||
                ta == TaskActions.taOnCheck))
            {
                throw new Exception(String.Format(
                    "Задача не заблокирована Вами и находится в состоянии \"{0}\", запись данных невозможна. " +
                    "Для записи данных в хранилище необходимо задачу (TaskID = {1}), " +
                    "к которой прикреплен лист, перевести в состояние выполнения и заблокировать.",
                    task.State, taskID));
            }

            if (task.Doer != Authentication.UserID &&
                !scheme.UsersManager.CheckPermissionForTask(task.ID, task.RefTasksTypes, (int)TaskOperations.Perform, false))
            {
                throw new Exception(String.Format(
                    "Задача (TaskID = {0}) назначена другому исполнителю ({1}), запись данных невозможна.",
                    taskID, scheme.UsersManager.GetUserNameByID(task.Doer)));
            }
        }

        public bool IsOracle()
        {
            return scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient ||
                   scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
                   scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess;
        }

        public bool IsMsSql()
        {
            return scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.SqlClient;
        }

        public IDatabase GetDb()
        {
            return scheme.SchemeDWH.DB;
        }

        public IProcessor GetOlapProcessor()
        {
            return scheme.Processor;
        }

        public string GetRopositoryDirectory()
        {
            return scheme.BaseDirectory;
        }
    }
}
