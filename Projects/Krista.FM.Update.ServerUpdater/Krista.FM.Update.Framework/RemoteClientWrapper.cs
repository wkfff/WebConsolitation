using System;

namespace Krista.FM.Update.Framework
{
    public class RemoteClientWrapper : MarshalByRefObject, INotifierClient
    {
        public event ReceiveNewStateDelegate ReceiveNewStateEvent;

        public delegate void ReceiveNewStateDelegate(object sender, UpdateProcessStateArgs e);
        
        public void InvokeReciveNewStateEvent(UpdateProcessStateArgs e)
        {
            ReceiveNewStateDelegate handler = ReceiveNewStateEvent;
            if (handler != null) handler(this, e);
        }

        public object ReceiveNewState(UpdateProcessState updateProcessState)
        {
            try
            {
                InvokeReciveNewStateEvent(new UpdateProcessStateArgs(updateProcessState));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    [Serializable]
    public class UpdateProcessStateArgs : EventArgs
    {
        private readonly UpdateProcessState state;

        public UpdateProcessStateArgs(UpdateProcessState state)
        {
            this.state = state;
        }

        public UpdateProcessState State
        {
            get { return state; }
        }
    }
}
