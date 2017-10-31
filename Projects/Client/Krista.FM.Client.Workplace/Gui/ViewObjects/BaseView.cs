using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Infragistics.Win.UltraWinGrid;

using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.Workplace;

namespace Krista.FM.Client.ViewObjects.BaseViewObject
{
	/// <summary>
	/// Базовый класс для объекта просмотра (внедряется в правую часть воркплэйса)
	/// </summary>
	public class BaseView : System.Windows.Forms.UserControl, IPersistenceSupport
	{
		private System.ComponentModel.Container components = null;
	    private BaseViewContentPane viewContent;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		public BaseView()
		{
			InitializeComponent();
			
		}

        public virtual string Caption
        {
            get { return "Область просмотра"; }
        }

	    public BaseViewContentPane ViewContent
	    {
	        get { return viewContent; }
	        set { viewContent = value; }
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
					SavePersistence();
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
			// BaseView
			// 
			this.Name = "BaseView";
			this.Size = new System.Drawing.Size(588, 536);
			this.ResumeLayout(false);

		}
		#endregion

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


		#region IPersistenceSupport Members

		public virtual void SavePersistence()
		{
			Trace.TraceVerbose("Созранение настроек для объекта {0}", ViewContent == null ? "???" : ViewContent.Key);
			if (SaveState != null)
			{
				SaveState(this, new EventArgs());
			}
		}

		public virtual void LoadPersistence()
		{
			Trace.TraceVerbose("Загрузка настроек для объекта {0}", ViewContent == null ? "???" : ViewContent.Key);
			if (LoadState != null)
			{
				LoadState(this, new EventArgs());
			}
		}

		public event EventHandler LoadState;

		public event EventHandler SaveState;

		#endregion
	}
}
