using System.Windows.Forms;

using Krista.FM.Client.Common;


namespace Krista.FM.Client.ViewObjects.BaseViewObject
{
	/// <summary>
	/// ������������ ����� ��� �������� ��������� (����������� ������������)
	/// </summary>
    public abstract class BaseViewObj : BaseViewContentPane
	{
		// ��� ���������� �������� - ������ ���������������� ��� ������ ��������� � ���� ������������
		private bool _Initialized = false;

        /// <summary>
        /// �������� ������, ������������ �� ������� ����������
        /// </summary>
        public string Caption;

        // ��������� Workplace, ���������������� ��� ��������
		public IWorkplace Workplace;

		// ���������� ����� ���������� � �������� ������ (��������� �������)
		public int Index = 1000;

		// �������� ��� ���������� 16�16
		public virtual System.Drawing.Image TypeImage16
		{
			get { return null; }
		}

		// �������� ��� ���������� 32�32
		public virtual System.Drawing.Image TypeImage24
		{
			get { return null; }
		}

		// ������ ���������
		protected BaseView fViewCtrl;
		// � ���� ������ ������� ������ ������� �������� �������� ���������
		protected abstract void SetViewCtrl();
		/// <summary>
		/// ������ ���������, ���������� � ����� ����� ����������
		/// </summary>
		public BaseView ViewCtrl
		{
			get
			{
				return fViewCtrl;
			}
		}

        public override Control Control
        {
            get { return fViewCtrl; }
        }

		/// <summary>
		/// ����������� ������
		/// </summary>
		public BaseViewObj()
		{
		}

		// ��������, ������������� ������������� �������
		public bool Initialized
		{
			get { return _Initialized; }
		}
		
        /// <summary>
		/// ���������� ��� ��������� ������� �������������. ��� �������� ��� ��������
		/// </summary>
		public virtual void Initialize()
		{
            // .. � ������� ���������
            SetViewCtrl();

			_Initialized = true;
			fViewCtrl.Customize();
		}

		/// <summary>
		/// ���������� ��� �������� ������� (�������� ��� �������� Workplace)
		/// </summary>
		public virtual void InternalFinalize()
		{
			_Initialized = false;
			fViewCtrl.Dispose();
		}

		/// <summary>
		/// ����� ��� ������������ (����������) ������
		/// </summary>
		public virtual void ReloadData()
		{
		}

		/// <summary>
		/// ����� ��� ���������� ������
		/// </summary>
		public virtual void SaveChanges()
		{
		}

		/// <summary>
		/// ����� ��� ������ ��������� ������������� ���������
		/// </summary>
		public virtual void CancelChanges()
		{
		}

		/// <summary>
		/// �����, ��������������� �� ���������/����������� ����������
		/// </summary>
		/// <param name="Activated"></param>
		public virtual void Activate(bool Activated) 
		{
		}
    
        /// <summary>
        /// �������� ���������� true, ���� ������ ������������ offline ������ ������ ��� ����������� � �����
        /// </summary>
        public virtual bool SupportOfflineMode
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// ����� �������� �� ��������� �� ������� ����������
        /// </summary>
        /// <param name="moduleParams"></param>
        /// <returns></returns>
        public virtual void SetActive(params object[] moduleParams)
        {
        }

        protected bool _canDeactivate = true;
        /// <summary>
        /// �����, �����������, ����� �� ������� � �������� ���������� �� ������
        /// </summary>
        /// <returns></returns>
        public virtual bool CanDeactivate
        {
            get
            {
                return _canDeactivate;
            }
            set
            {
                _canDeactivate = value;
            }
        }

        /// <summary>
        /// ����� �� ��������� ������� ���������
        /// </summary>
        public virtual bool CanUnload
        {
            get
            {
                return true;
            }
        }

	    /// <summary>
	    /// �������� �������, ������������ �� ������� ����������
	    /// </summary>
	    public virtual string FullCaption
	    {
	        get
	        {
	            return Caption;
	        }
	    }
    }
}
