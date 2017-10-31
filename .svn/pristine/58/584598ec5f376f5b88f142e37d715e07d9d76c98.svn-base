using System;
using System.Collections;
using System.Data;

using System.Runtime.Remoting.Lifetime;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Users;

namespace Krista.FM.Server.Tasks
{

    /// <summary>
    /// Абстрактный класс-предок для объектов, хранящих свое состояние в БД и создаваемых по требованию
    /// </summary>
    public abstract class UpdatedDBObject : DisposableObject, IUpdatedDBObject 
    {
        private RealTimeDBObjectsEnumerableCollection _parentCollection;
        // родительская колекция, у нее будем запращивать различные вспомогательные 
        // объекты типа IDatabase
        public RealTimeDBObjectsEnumerableCollection ParentCollection
        {
            get { return _parentCollection; }
        }

        /// <summary>
        /// Скрытый конструктор без параметров
        /// </summary>
        private UpdatedDBObject()
        {
        }

        /// <summary>
        /// Коструктор объекта 
        /// </summary>
        /// <param name="parentCollection">родительская коллекция</param>
        public UpdatedDBObject(RealTimeDBObjectsEnumerableCollection parentCollection)
        {
            if (parentCollection == null)
                throw new Exception("Необходим экземпляр RealTimeDBObjectsEnumerableCollection");
            // запоминаем указатель на родительскую коллекцию
            _parentCollection = parentCollection;
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">вызван пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _parentCollection = null;
//                System.Runtime.Remoting.RemotingServices.Disconnect(this);
            }
            base.Dispose(disposing);
        }

        private bool _isNew = false;
        /// <summary>
        /// признак того что объект только что создан (еще не существует в базе)
        /// в этом случае при вызове метода EndUpdate будет выполняться команда вставки, а не обновления
        /// </summary>
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }


        private bool _inEdit = false;
        /// <summary>
        /// Признак того, что объект находится в режиме обновления (после BeginEdit но до  EndEdit или CancelEdit)
        /// </summary>
        public virtual bool InEdit
        {
            get { return _inEdit; }
            set { _inEdit = value; }
        }

        /// <summary>
        /// Начать обновление объекта 
        /// </summary>
        public virtual void BeginUpdate(string action)
        {
            // устанавливаем флаг нахождения в режиме редактирования
            InEdit = true;
        }

        /// <summary>
        /// Завершить обновление объекта, внести изменения в базу
        /// </summary>
        public virtual void EndUpdate()
        {
            if (!InEdit) return;

            InEdit = false;
            string commandText = String.Empty;
            IDbDataParameter[] parameters = null;
            // если это только что созданный объект
            if (IsNew)
                // ..запрашиваем параметры для команды вставки
                InitializeInsertCommand(ref commandText, ref parameters);
            else
                // иначе запрашиваем параметры для команды обновления
                InitializeUpdateCommand(ref commandText, ref parameters);
            IDatabase db = _parentCollection.GetDB();
            try
            {
                // пытаемся внести изменения в базу
                db.ExecQuery(commandText, QueryResultTypes.NonQuery, parameters);
                // запись больше не является новой
                IsNew = false;
            }
            catch
            {
                // если не получается - восстанавливаем предыдущее состояние (???)
                if (!IsNew)
                    ReloadData();
                // передаем исключение на дальнейшую обработку
                throw;
            }
            finally
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Отменить измения (восстановить предыдущее состояние)
        /// </summary>
        public virtual void CancelUpdate()
        {
            // если объект не в состоянии редактирования - ничего не делаем
            if (!InEdit) return;
            // сбрасываем признак редактируемости
            InEdit = false;
            // если объект был создан ранее - обновляем его поля
            if (!IsNew)
                ReloadData();
        }

        /// <summary>
        /// Внутренний метод для обновления состояния объекта
        /// </summary>
        public void ReloadData()
        {
            string commandText = String.Empty;
            IDbDataParameter[] parameters = null;
            // инициализируем команду выборки данных
            InitializeSelectCommand(ref commandText, ref parameters);

            DataTable dt = null;
            IDatabase db = _parentCollection.GetDB();
            try
            {
                // получаем данные объекта
                dt = (DataTable)db.ExecQuery(commandText, QueryResultTypes.DataTable, parameters);
            }
            finally
            {
                db.Dispose();
            }
            // если не удалось получить данные - генерируем исключение
            if ((dt == null) || (dt.Rows.Count == 0))
                throw new Exception("Невозможно обновить данные. Объект не существует в базе данных.");

            DataRow dr = dt.Rows[0];
            // вызываем метод обновления полей объекта
            RefreshObjectFields(dr);
        }

        #region Абстрактные методы подлежащие реализации в потомках
        /// <summary>
        /// Метод вызывается при инициализации команды вставки записи в БД
        /// </summary>
        /// <param name="insertCommandText">текст команды INSERT</param>
        /// <param name="insertCommandParameters">параметры команды INSERT</param>
        public abstract void InitializeInsertCommand(ref string insertCommandText, ref IDbDataParameter[] insertCommandParameters);

        /// <summary>
        /// Метод вызывается при инициализации команды выборки данных
        /// </summary>
        /// <param name="selectCommandText">текст команды SELECT</param>
        /// <param name="selectCommandParameters">параметры команды SELECT</param>
        public abstract void InitializeSelectCommand(ref string selectCommandText, ref IDbDataParameter[] selectCommandParameters);

        /// <summary>
        /// Метод вызывается при инициализации  команды вставки данных
        /// </summary>
        /// <param name="updateCommandText">текст команды UPDATE</param>
        /// <param name="updateCommandParameters">параметры команды UPDATE</param>
        public abstract void InitializeUpdateCommand(ref string updateCommandText, ref IDbDataParameter[] updateCommandParameters);

        /// <summary>
        /// Метод вызывается при необходимости обновления полей объекта
        /// </summary>
        /// <param name="dataRow">данные объекта</param>
        public abstract void RefreshObjectFields(DataRow dataRow);
        #endregion
    }

    /// <summary>
    /// Абстрактный класс-предок для коллекции объектов, хранящих свое состояние в БД 
    /// и создаваемых по требованию
    /// </summary>
    public abstract class RealTimeDBObjectsEnumerableCollection : DisposableObject
    {
        // родительский объект-схема
        private IScheme _scheme;
        public IScheme Scheme
        {
            get { return _scheme; }
        }

        /// <summary>
        /// Вспомогательный метод для получения IDatabase у схемы
        /// </summary>
        /// <returns>IDatabase</returns>
        public IDatabase GetDB()
        {
            return _scheme.SchemeDWH.DB;
        }

        /// <summary>
        /// Вспомогательный метод получения UsersManager у схемы
        /// </summary>
        /// <returns>UsersManager</returns>
        public UsersManager GetUsersManager()
        {
            return (UsersManager)_scheme.UsersManager;
        }

        /// <summary>
        /// Скрытый конструктор без параметров
        /// </summary>
        private RealTimeDBObjectsEnumerableCollection()
        {
        }

        /// <summary>
        /// Конструктор класса 
        /// </summary>
        /// <param name="db">Объект IDatabase для внутреннего использования</param>
        public RealTimeDBObjectsEnumerableCollection(IScheme scheme)
        {
            if (scheme == null)
                throw new Exception("Для создании коллекции необходим экземпляр ISchemeDWH");

            _scheme = scheme;
        }

        /// <summary>
        /// Очистка ресурсов
        /// </summary>
        /// <param name="disposing">Признак вызванности пользователем или сборщиком мусора</param>
        protected override void Dispose(bool disposing)
        {
            if ((disposing) && (_scheme != null))
            {
                _scheme = null;
            }
            base.Dispose(disposing);
        }

        #region Реализация интерфейса ICollection
        public virtual int Count
        {
            get { return -1; }
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual object SyncRoot
        {
            get { return null; }
        }

        public virtual void CopyTo(Array array, int index)
        {
            throw new InvalidOperationException("Метод не поддерживается");
        }
        #endregion

        #region Реализация интерфейса IEnumerable
        // IEnumerable
        public virtual IEnumerator GetEnumerator()
        {
            return null;
        }
        #endregion

        #region IDictionary
        public virtual bool IsFixedSize
        {
            get { return false; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual ICollection Values
        {
            get { return null; }
        }

        public virtual object this[object key]
        {
            get { return null; }
            set { }
        }

        public virtual void Add(object key, object value)
        {
        }

        public virtual void Clear()
        {
        }

        public virtual bool Contains(object key)
        {
            return false;
        }

        public void Remove(object key)
        {
        }
        #endregion
        
    }

}