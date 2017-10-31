using System;
using System.Collections.Generic;
using System.Diagnostics;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    /// <summary>
    /// Описание операции по модификации объекта.
    /// </summary>
    internal abstract class ModificationItem : DisposableObject, IModificationItem
    {
        /// <summary>
        /// Тип модификации.
        /// </summary>
        private readonly ModificationTypes type;
        
        /// <summary>
        /// Состояние изменения.
        /// </summary>
        private ModificationStates state = ModificationStates.NotApplied;

        /// <summary>
        /// Наименование операции.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// Родительская операция.
        /// </summary>
        private ModificationItem parent;

        /// <summary>
        /// Объект который необходимо модифицировать.
        /// </summary>
        private readonly object fromObject;

        /// <summary>
        /// Целевой объект который необходимо создать 
        /// или объект на основе которого будет модифицироваться исходный объект.
        /// </summary>
        private readonly object toObject;

        /// <summary>
        /// Дочерние операции.
        /// </summary>
        readonly Dictionary<string, IModificationItem> items;

        /// <summary>
        /// Исключение возникшее при выполнении операции.
        /// </summary>
        private Exception exception;


        /// <summary>
        /// Базовый элемент модификации.
        /// </summary>
        /// <param name="type">Тип модификации.</param>
        /// <param name="name"></param>
        /// <param name="fromObject">Начальное состояние(значение).</param>
        /// <param name="toObject">Конечное состояние(значение).</param>
        /// <param name="parent">Родительская операция.</param>
        [DebuggerStepThrough]
        public ModificationItem(ModificationTypes type, string name, object fromObject, object toObject, ModificationItem parent)
        {
            this.type = type;
            this.name = name;
            this.fromObject = fromObject;
            this.toObject = toObject;
            this.parent = parent;
            items = new Dictionary<string, IModificationItem>();
        }

        /// <summary>
        /// Тип модификации.
        /// </summary>
        public ModificationTypes Type
        {
            [DebuggerStepThrough]
            get { return type; }
        }

        /// <summary>
        /// Состояние изменения.
        /// </summary>
        public ModificationStates State
        {
            [DebuggerStepThrough]
            get { return state; }
            [DebuggerStepThrough]
            set { state = value; }
        }

        /// <summary>
        /// Наименование операции.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return name; }
        }

        /// <summary>
        /// Родительская операция.
        /// </summary>
        public ModificationItem Parent
        {
            [DebuggerStepThrough]
            get { return parent; }
            [DebuggerStepThrough]
            set
            {
                if (parent != null)
                    throw new Exception("Значение свойства Parent уже установлено.");
                else
                    parent = value;
            }
        }

        protected object ModificationObject
        {
            [DebuggerStepThrough]
            get
            {
                if (type == ModificationTypes.Create)
                    return toObject;
                else if (type == ModificationTypes.Remove)
                    return fromObject;
                else
                    return fromObject;
            }
        }

        /// <summary>
        /// Уникальное наименование операции.
        /// </summary>
        public string Key
        {
            [DebuggerStepThrough]
            get
            {
                return String.Format("{0} - {1}:{2}", type, name, ModificationObject != null ? ModificationObject.GetType().ToString() : "unknow");
            }
        }

        /// <summary>
        /// ImageIndex
        /// </summary>
        public virtual int ImageIndex
        {
            get { return 0; }
        }

        /// <summary>
        /// Исходный объект который необходимо модифицировать или удалить.
        /// </summary>
        public object FromObject
        {
            [DebuggerStepThrough]
            get { return fromObject; }
        }

        /// <summary>
        /// Целевой объект который необходимо создать 
        /// или объект на основе которого будет модифицироваться исходный объект.
        /// </summary>
        public object ToObject
        {
            [DebuggerStepThrough]
            get { return toObject; }
        }

        /// <summary>
        /// Исключение возникшее при выполнении операции.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// Дочерние операции.
        /// </summary>
        public Dictionary<string, IModificationItem> Items
        {
            [DebuggerStepThrough]
            get { return items; }
        }

        ///// <summary>
        ///// Вызывается перед применением дочерних операций.
        ///// </summary>
        //protected virtual void OnBeforeChildApplay(ModificationContext context)
        //{
        //}

        /// <summary>
        /// Вызывается перед применением дочерних операций.
        /// </summary>
        protected virtual void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            isAppliedPartially = false;
        }

        /// <summary>
        /// Вызывается после применениея дочерних операций.
        /// </summary>
        protected virtual void OnAfterChildApplay(ModificationContext context)
        {
        }

        /// <summary>
        /// Применение изменения.
        /// </summary>
        /// <param name="context">Контекст в котором выполняются изменения структуры схемы.</param>
        public virtual void Applay(IModificationContext context, out bool isAppliedPartially)
        {
            if (state == ModificationStates.Applied)
            {
                isAppliedPartially = false;
                return;
            }

            bool needRestoreUserContext = false;

            LogicalCallContextData userContext = LogicalCallContextData.GetContext();
            try
            {
                if ((int)userContext["UserID"] != (int)FixedUsers.FixedUsersIds.System)
                {
                    needRestoreUserContext = true;
                    SessionContext.SetSystemContext();
                }

                Trace.Indent();

                ((ModificationContext)context).StartOperation(this);
                bool isAppliedPartiallyBeforeChild=false;
                OnBeforeChildApplay((ModificationContext)context, out isAppliedPartiallyBeforeChild);

                bool isAppliedPartiallyAllChild = false;
                foreach (ModificationItem item in Items.Values)
                {
                    /*if (item.Name == "ФО_0005_ГодОтч" || item.Name == "ФО_0002_МесОтч" ||
                        item.Name == "ФО_0005_ГодОтч_Планирование" || item.Name == "ФО_0002_МесОтч_Планирование")
                        continue;*/
                    bool isAppliedPartiallyChild = false;
                    item.Applay(context, out isAppliedPartiallyChild);
                    if (isAppliedPartiallyChild)
                    {
                        isAppliedPartiallyAllChild = true;
                    }
                }

                OnAfterChildApplay((ModificationContext)context);
            
                isAppliedPartially = ( isAppliedPartiallyAllChild || isAppliedPartiallyBeforeChild);
                ((ModificationContext)context).EndOperation(this, isAppliedPartially);
            }
            catch (Exception e)
            {
                exception = e;
                ((ModificationContext)context).EndOperation(this, e);
                throw new ServerException(e.Message, e);
            }
            finally
            {
                Trace.Unindent();
                if (needRestoreUserContext)
                {
                    LogicalCallContextData.SetContext(userContext);
                }
            }
        }

        /// <summary>
        /// Очистка от пустых элементов.
        /// </summary>
        internal void Purge()
        {
            List<string> forPurge = new List<string>();

            foreach (ModificationItem item in items.Values)
            {
                if (item.Type == ModificationTypes.Modify 
                    && !(item is PropertyModificationItem)
                    && !(item is DataRowModificationItem)
                    && !(item is HierarchyModificationItem)
                    && !(item is KeyObjectModificationItem))
                {
                    item.Purge();
                    
                    if (item.Items.Count == 0)
                        forPurge.Add(item.Key);
                }
            }

            foreach (string itemKey in forPurge)
                items.Remove(itemKey);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (toObject is IDisposable)
                {
                    ((IDisposable) toObject).Dispose();
                }

                foreach (var item in Items.Values)
                {
                    item.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
