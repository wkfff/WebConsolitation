using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Itenso.Configuration;
using Krista.FM.Client.Common;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Configuration;
using Krista.FM.Client.Common.Gui;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls
{
	public partial class frmModalTemplate : Form
	{
		// время последнего показа классификатора
		public int LastShowTickCount;

		public IInplaceClsView AttachedCls;

        public string FormCaption = string.Empty;

        /// <summary>
        /// Метод присоединения классификатора
        /// </summary>
        /// <param name="attachedCls">присоединяемый классификатор</param>
        /// <param name="loadSetting">загружать настройки</param>
		public void AttachCls(IInplaceClsView attachedCls, Boolean loadSetting)
		{
			AttachedCls = attachedCls;
            AttachedCls.AttachViewObject(spcContainer.Panel1);
            if (FormCaption == string.Empty)
                Text = "Классификатор " + AttachedCls.GetClsRusName();
            else
                Text = FormCaption;
            UltraGrid grid = AttachedCls.UltraGridExComponent.ugData;
            grid.KeyDown += new KeyEventHandler(grid_KeyDown);

			if (loadSetting)
			{
				FormSettings formSettings = new FormSettings(this,
															 String.Format("{0}.{1}", this.GetType().FullName,
																		   attachedCls.ActiveDataObj.ObjectKey));
				formSettings.Settings.Add(
					 new UltraGridExSettingsPartial(
						 String.Format("{0}.{1}", this.GetType().FullName, attachedCls.ActiveDataObj.ObjectKey),
						 AttachedCls.UltraGridExComponent.ugData));
			}
		}

		/// <summary>
		/// Метод присоединения классификатора. Не загружает и не сохраняет настройки.
		/// </summary>
		/// <param name="attachedCls">присоединяемый классификатор</param>
		public void AttachCls(IInplaceClsView attachedCls)
		{
			AttachCls(attachedCls, false);
		}
		
        void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.Enter)
            {
                btnOk.PerformClick();
            }
        }

		public frmModalTemplate()
		{
			InitializeComponent();
			this.DialogResult = DialogResult.Cancel;
		}

		/// <summary>
		/// Методо скрывает Кнопку ОК на форме
		/// </summary>
		/// <param name="hide">true - скрыть, false - показать</param>
		public void HideBtnOk(Boolean hide)
		{
			btnOk.Visible = !hide;
		}
	}
/*
	/// <summary>
	/// Интерфейс объекта для внедрения протоколов
	/// </summary>
	public interface IInplaceClsView
	{
		/// <summary>
		/// Метод для внедрения классификатора в объект просмотра
		/// </summary>		
		void AttachViewObject(Control parentControl);

		void DetachViewObject();

        void InitModalCls(IEntity cls, int oldID);

		/// <summary>
		///	 Метод для обновления данных во внедренном гриде
		/// </summary>
		void RefreshAttachedData();

        void RefreshAttachedData(int sourceID);

        int GetSelectedID();

		string GetClsRusName();

		DataSet GetClsDataSet();

		Infragistics.Win.UltraWinGrid.UltraGrid GetClsGrid();

		Infragistics.Win.UltraWinToolbars.UltraToolbarsManager GetClsToolBar();
	}
    */
}