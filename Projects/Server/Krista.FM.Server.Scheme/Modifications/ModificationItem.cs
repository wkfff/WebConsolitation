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
    /// �������� �������� �� ����������� �������.
    /// </summary>
    internal abstract class ModificationItem : DisposableObject, IModificationItem
    {
        /// <summary>
        /// ��� �����������.
        /// </summary>
        private readonly ModificationTypes type;
        
        /// <summary>
        /// ��������� ���������.
        /// </summary>
        private ModificationStates state = ModificationStates.NotApplied;

        /// <summary>
        /// ������������ ��������.
        /// </summary>
        private readonly string name;

        /// <summary>
        /// ������������ ��������.
        /// </summary>
        private ModificationItem parent;

        /// <summary>
        /// ������ ������� ���������� ��������������.
        /// </summary>
        private readonly object fromObject;

        /// <summary>
        /// ������� ������ ������� ���������� ������� 
        /// ��� ������ �� ������ �������� ����� ���������������� �������� ������.
        /// </summary>
        private readonly object toObject;

        /// <summary>
        /// �������� ��������.
        /// </summary>
        readonly Dictionary<string, IModificationItem> items;

        /// <summary>
        /// ���������� ��������� ��� ���������� ��������.
        /// </summary>
        private Exception exception;


        /// <summary>
        /// ������� ������� �����������.
        /// </summary>
        /// <param name="type">��� �����������.</param>
        /// <param name="name"></param>
        /// <param name="fromObject">��������� ���������(��������).</param>
        /// <param name="toObject">�������� ���������(��������).</param>
        /// <param name="parent">������������ ��������.</param>
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
        /// ��� �����������.
        /// </summary>
        public ModificationTypes Type
        {
            [DebuggerStepThrough]
            get { return type; }
        }

        /// <summary>
        /// ��������� ���������.
        /// </summary>
        public ModificationStates State
        {
            [DebuggerStepThrough]
            get { return state; }
            [DebuggerStepThrough]
            set { state = value; }
        }

        /// <summary>
        /// ������������ ��������.
        /// </summary>
        public string Name
        {
            [DebuggerStepThrough]
            get { return name; }
        }

        /// <summary>
        /// ������������ ��������.
        /// </summary>
        public ModificationItem Parent
        {
            [DebuggerStepThrough]
            get { return parent; }
            [DebuggerStepThrough]
            set
            {
                if (parent != null)
                    throw new Exception("�������� �������� Parent ��� �����������.");
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
        /// ���������� ������������ ��������.
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
        /// �������� ������ ������� ���������� �������������� ��� �������.
        /// </summary>
        public object FromObject
        {
            [DebuggerStepThrough]
            get { return fromObject; }
        }

        /// <summary>
        /// ������� ������ ������� ���������� ������� 
        /// ��� ������ �� ������ �������� ����� ���������������� �������� ������.
        /// </summary>
        public object ToObject
        {
            [DebuggerStepThrough]
            get { return toObject; }
        }

        /// <summary>
        /// ���������� ��������� ��� ���������� ��������.
        /// </summary>
        public Exception Exception
        {
            get { return exception; }
        }

        /// <summary>
        /// �������� ��������.
        /// </summary>
        public Dictionary<string, IModificationItem> Items
        {
            [DebuggerStepThrough]
            get { return items; }
        }

        ///// <summary>
        ///// ���������� ����� ����������� �������� ��������.
        ///// </summary>
        //protected virtual void OnBeforeChildApplay(ModificationContext context)
        //{
        //}

        /// <summary>
        /// ���������� ����� ����������� �������� ��������.
        /// </summary>
        protected virtual void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            isAppliedPartially = false;
        }

        /// <summary>
        /// ���������� ����� ����������� �������� ��������.
        /// </summary>
        protected virtual void OnAfterChildApplay(ModificationContext context)
        {
        }

        /// <summary>
        /// ���������� ���������.
        /// </summary>
        /// <param name="context">�������� � ������� ����������� ��������� ��������� �����.</param>
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
                    /*if (item.Name == "��_0005_������" || item.Name == "��_0002_������" ||
                        item.Name == "��_0005_������_������������" || item.Name == "��_0002_������_������������")
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
        /// ������� �� ������ ���������.
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
