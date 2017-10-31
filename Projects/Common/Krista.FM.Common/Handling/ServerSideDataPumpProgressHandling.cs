using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// ����� ��� ��������������� ������� ������� ������� �������
    /// </summary>
    public class ServerSideDataPumpProgressHandling : DisposableObject
    {
        #region ����

        private IServerSideDataPumpProgress serverSideDataPumpProgress;

        #endregion ����


        #region �������������

        /// <summary>
        /// ����������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.serverSideDataPumpProgress != null)
            {
                serverSideDataPumpProgress.SetState -= new GetPumpStateDelegate(dataPumpProgress_SetState);
                serverSideDataPumpProgress.GetPumpLiveStatus -= new GetBoolDelegate(serverSideDataPumpProgress_GetPumpLiveStatus);
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region ���������� �������

        /// <summary>
        /// ������ �������
        /// </summary>
        public IServerSideDataPumpProgress ServerSideDataPumpProgress
        {
            get
            {
                return serverSideDataPumpProgress;
            }
            set
            {
                serverSideDataPumpProgress = value;
                serverSideDataPumpProgress.SetState += new GetPumpStateDelegate(dataPumpProgress_SetState);
                serverSideDataPumpProgress.GetPumpLiveStatus += new GetBoolDelegate(serverSideDataPumpProgress_GetPumpLiveStatus);
            }
        }

        #endregion ���������� �������


        #region ���������� �������

        /// <summary>
        /// ������� ����� ��������� �������
        /// </summary>
        public event GetPumpStateDelegate SetState;

        /// <summary>
        /// ������� ��������� �������� ����, ��� ������� �� ��� ��� ��������, � �� ���������� �����
        /// </summary>
        public event GetBoolDelegate GetPumpLiveStatus;

        #endregion ���������� �������


        #region ����������� ������� �������

        /// <summary>
        /// ������� ��������� �����
        /// </summary>
        public void dataPumpProgress_SetState(PumpProcessStates state)
        {
            EventsProcessing.OnGetPumpStateDelegateEvent(ref this.SetState, state);
        }

        /// <summary>
        /// ������� ��������� �������� ����, ��� ������� �� ��� ��� ��������, � �� ���������� �����
        /// </summary>
        public bool serverSideDataPumpProgress_GetPumpLiveStatus()
        {
            return EventsProcessing.OnGetBoolDelegateEvent(ref this.GetPumpLiveStatus);
        }

        #endregion ����������� ������� �������
    }
}
