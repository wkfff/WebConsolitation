using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// ����� ��� ��������������� ������� �������� ������� ������� �������.
    /// </summary>
    public sealed class PumpSchedulerHandling : DisposableObject
    {
        #region ����

        private IPumpScheduler pumpScheduler;

        #endregion ����


        #region �������������

        /// <summary>
        /// ����������
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.pumpScheduler != null)
            {
                pumpScheduler.ScheduleIsChanged -= new GetStringDelegate(pumpScheduler_ScheduleIsChanged);
            }

            base.Dispose(disposing);
        }

        #endregion �������������


        #region ���������� �������

        /// <summary>
        /// ��� �������
        /// </summary>
        public IPumpScheduler PumpScheduler
        {
            get
            {
                return pumpScheduler;
            }
            set
            {
                pumpScheduler = null;
                pumpScheduler = value;
                pumpScheduler.ScheduleIsChanged += new GetStringDelegate(pumpScheduler_ScheduleIsChanged);
            }
        }

        #endregion ���������� �������


        #region ���������� �������

        /// <summary>
        /// ������� ��������� �������� ����������
        /// </summary>
        public event GetStringDelegate ScheduleIsChanged;

        #endregion ���������� �������


        #region ����������� ������� ���� �������

        /// <summary>
        /// ������� ��������� �������� ����������
        /// </summary>
        /// <param name="str">�� ��������� �������</param>
        public void pumpScheduler_ScheduleIsChanged(string str)
        {
            EventsProcessing.OnGetStringDelegateEvent(ref this.ScheduleIsChanged, str);
        }

        #endregion ����������� ������� ���� �������
    }
}
