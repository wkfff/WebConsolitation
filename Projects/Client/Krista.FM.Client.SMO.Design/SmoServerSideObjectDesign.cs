using System.ComponentModel;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SmoServerSideObjectDesign<T> : ServerManagedObject<T>, IServerSideObject where T : IServerSideObject
    {
        public SmoServerSideObjectDesign(T serverControl)
            : base(serverControl)
        {
            identifier = GetNewIdentifier();
        }


        #region IServerSideObject Members

        public IServerSideObject Lock()
        {
            return serverControl.Lock();
        }

        public void Unlock()
        {
            serverControl.Unlock();
        }

        [Category("Access control lock")]
        [DisplayName(@"Объект заблокирован")]
        [Description("Если объект заблокирован, то принимает знаение true, иначе false")]
        [Browsable(false)]
        public bool IsLocked
        {
            get { return serverControl.IsLocked; }
        }

        [Category("Access control lock")]
        [DisplayName(@"ID пользователя")]
        [Description("ID пользователя, заблокировавшего объект")]
        [Browsable(false)]
        public int LockedByUserID
        {
            get { return serverControl.LockedByUserID; }
        }

        [Category("Access control lock")]
        [DisplayName(@"Имя пользователя")]
        [Description("Имя пользователя, заблокировавшего объект")]
        public string LockedByUserName
        {
            get { return serverControl.LockedByUserName; }
        }

        [Browsable(false)]
        public bool IsEndPoint
        {
            get { return serverControl.IsEndPoint; }
        }

        [Browsable(false)]
        public ServerSideObjectStates State
        {
            get { return serverControl.State; }
        }

        public object Clone()
        {
            return serverControl.Clone();
        }

        [Browsable(false)]
        public IServerSideObject OwnerObject
        {
            get { return serverControl.OwnerObject; }
        }

        [Category("Access control lock")]
        [DisplayName(@"Идентификатор объекта (Identifier)")]
        [Description("Идентификатор объекта выведен для тестовых целей")]
        [Browsable(false)]
        public int Identifier
        {
            get { return serverControl.Identifier; }
        }

        [Category("Access control lock")]
        [DisplayName(@"Клон (IsClone)")]
        [Description("Определяет является ли объект клоном")]
        [Browsable(false)]
        public bool IsClone
        {
            get { return serverControl.IsClone; }
        }

        [Browsable(false)]
        public IServerSideObject ICloneObject
        {
            get { return serverControl.ICloneObject; }
        }

        [Category("Access control lock")]
        [DisplayName(@"Клонированый объект (ICloneObjectWrap)")]
        [Description("Копия объекта, с которым работает пользователь")]
        [Browsable(false)]
        public SmoServerSideObjectDesign<T> ICloneObjectWrap
        {
            get
            {
                if (serverControl.ICloneObject != null)
                {
                    int i = serverControl.ICloneObject.Identifier;
                    SmoServerSideObjectDesign<T> sso = new SmoServerSideObjectDesign<T>((T)serverControl.ICloneObject);
                    return sso;
                }

                return null;
            }
        }

        #endregion
    }
}
