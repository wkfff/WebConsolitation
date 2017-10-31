using System.ComponentModel;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Components;
using Krista.FM.Client.Common;
using System;

namespace Krista.FM.Client.ViewObjects.BaseViewObject
{
	/// <summary>
	/// Родительский класс для навигационного элемента (внедряется на вкладку воркплэйса)
	/// </summary>
    public /*abstract*/ class BaseNavigationCtrl : UserControl//, IServiceProvider
	{
        // для отложенной загрузки - объект инициализируется при первом обращении к нему пользователя
        private bool initialized = false;

        // интерфейс Workplace, инициализируется при загрузке
        private IWorkplace workplace;

        // Порядковый номер интерфейса в пределах сборки (временный вариант)
        public int Index = 1000;

        /// <summary>
        /// Название модуля, отображается на вкладке воркплэйса
        /// </summary>
        public string Caption;

        private System.ComponentModel.Container components = null;

		
        /// <summary>
		/// Конструктор класса
		/// </summary>
		public BaseNavigationCtrl()
		{
            InitializeComponent();
		}

		/// <summary> 
		/// Деструктор класса, очистка ресурсов
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// BaseNavigationCtrl
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Name = "BaseNavigationCtrl";
			this.Size = new System.Drawing.Size(216, 472);
			this.ResumeLayout(false);

		}
		#endregion

        // картинка для воркплэйса 16х16
        public virtual System.Drawing.Image TypeImage16
        {
            get { return null; }
        }

        // картинка для воркплэйса 32х32
        public virtual System.Drawing.Image TypeImage24
        {
            get { return null; }
        }

        // свойство, индицируюещее загруженность объекта
        public bool Initialized
        {
            get { return initialized; }
        }

        public IWorkplace Workplace
        {
            get { return workplace; }
            set { workplace = value; }
        }

        /// <summary>
        /// Инициализация объкта навигации.
        /// </summary>
        public virtual void Initialize()
        {
            initialized = true;
            Customize();
        }

        public delegate void ActiveItemChangedEventHandler(BaseNavigationCtrl sender, BaseViewContentPane viewObject);

        public event ActiveItemChangedEventHandler ActiveItemChanged;

        public void OnActiveItemChanged(BaseNavigationCtrl sender, BaseViewContentPane viewObject)
        {
            if (ActiveItemChanged != null)
            {
                ActiveItemChanged(this, viewObject);
            }
        }

        public void CustomizeEvents(Component cmp)
        {
            if (cmp.GetType() == typeof(Infragistics.Win.UltraWinGrid.UltraGrid))
            {
                ((UltraGrid)cmp).BeforeCustomRowFilterDialog += new BeforeCustomRowFilterDialogEventHandler(CustomizeUltraGridRowFilterDialog);
            }
        }

		public virtual void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsControls(this);
			ComponentCustomizer.CustomizeInfragisticsComponents(this.components);
            // навешиваем общие обработчики событий
            ComponentCustomizer.EnumControls(this, new checkProcDelegate(this.CustomizeEvents));
		}

        // Обработчик кустомизации диалога фильтрации UltraWinGrid - самый простой способ это разместить его здесь
        public void CustomizeUltraGridRowFilterDialog(object sender, BeforeCustomRowFilterDialogEventArgs e)
        {
            if ((e.CustomRowFiltersDialog != null) && (e.CustomRowFiltersDialog.Grid != null))
            {
                UltraGridHelper.CustomizeRowFilerDialog(e.CustomRowFiltersDialog, e.Column.Header.Caption);
            }
        }

        /// <summary>
        /// Свойство возвращает true, если объект поддерживает offline режимм работы без подключения к схеме
        /// </summary>
        public virtual bool SupportOfflineMode
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanUnload
        {
            get { return true; }
        }

        /// <summary>
        /// метод перехода на интерфейс из другого интерфейса
        /// </summary>
        /// <param name="moduleParams"></param>
        /// <returns></returns>
        public virtual void SetActive(params object[] moduleParams)
        {

        }
        /*
        object IServiceProvider.GetService(Type service)
        {
            if (service == typeof(IAdministrationNavigation) && this is IAdministrationNavigation)
                return((IAdministrationNavigation)this);
            if (service == typeof(IProtocolNavigation) && this is IProtocolNavigation)
                return((IProtocolNavigation)this);
            if (service == typeof(IBaseClsNavigation) && this is IBaseClsNavigation)
                return((IBaseClsNavigation)this);
            return null;
        }*/
	}
}
