using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO
{
    public class SmoServerSideObject<T> : ServerManagedObject<T>, IServerSideObject where T : IServerSideObject
    {
        public SmoServerSideObject(T serverObject)
            : base(serverObject)
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

        public bool IsLocked
        {
            get { return serverControl.IsLocked; }
        }

        public int LockedByUserID
        {
            get { return serverControl.LockedByUserID; }
        }

        public string LockedByUserName
        {
            get { return serverControl.LockedByUserName; }
        }

        public bool IsEndPoint
        {
            get { return serverControl.IsEndPoint; }
        }

        public ServerSideObjectStates State
        {
            get { return serverControl.State; }
        }

        public object Clone()
        {
            return serverControl.Clone();
        }

        public IServerSideObject OwnerObject
        {
            get { return serverControl.OwnerObject; }
        }

        public int Identifier
        {
            get { return serverControl.Identifier; }
        }

        public bool IsClone
        {
            get { return serverControl.IsClone; }
        }

        public IServerSideObject ICloneObject
        {
            get { return serverControl.ICloneObject; }
        }

        public SmoServerSideObject<T> ICloneObjectWrap
        {
            get 
            {
                if (serverControl.ICloneObject != null)
                {
                    int i = serverControl.ICloneObject.Identifier;
                    SmoServerSideObject<T> sso = new SmoServerSideObject<T>((T)serverControl.ICloneObject);
                    return sso;
                }
                else
                    return null;
            }
        }  
        
        #endregion
    }
}
