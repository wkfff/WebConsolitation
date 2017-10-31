using System;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.Navigation;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

using Infragistics.Win.UltraWinGrid;

namespace Krista.FM.Client.ViewObjects.AssociatedCLSUI.Association
{
    public partial class AssociationUI : BaseViewObj
    {
        /// <summary>
        /// переход с одной страницы на другую
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ultraTabControl1_ActiveTabChanged(object sender, Infragistics.Win.UltraWinTabControl.ActiveTabChangedEventArgs e)
        {
            TranslationTablesPageLoad(e.Tab);
        }

        /// <summary>
        /// загружаем данные по перекодировкам при переходе на страницу перекодировок в сопоставлении классификаторов
        /// </summary>
        /// <param name="tab"></param>
        private void TranslationTablesPageLoad(Infragistics.Win.UltraWinTabControl.UltraTab tab)
        {
            if (tab == null) return;
            if (tab.Index == 1)
            {
                // загрузка данных
                
                InitAssociationPage(curentAssociation);

                if (this.associateView.ultraGrid1.Rows.Count <= 0)
                    return;
                // необходимые событи€ грида
                this.associateView.ultraGrid1.InitializeRow += new Infragistics.Win.UltraWinGrid.InitializeRowEventHandler(ultraGrid1_InitializeRow);
                this.associateView.ultraGrid1.ClickCellButton += new Infragistics.Win.UltraWinGrid.CellEventHandler(ultraGrid1_ClickCellButton);
                this.associateView.ultraGrid1.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(ultraGrid1_InitializeLayout);
                // русские названи€ колонок в гриде
                
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[0].Hidden = true;
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[1].Header.Caption = " атегори€ сопоставл€емого";
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[1].Hidden = true;
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[2].Header.Caption = "—опоставл€емый классификатор";
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[2].Hidden = true;
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[3].Header.Caption = " атегори€ сопоставимого";
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[3].Hidden = true;
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[4].Header.Caption = "—опоставимый классификатор";
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[4].Hidden = true;
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns[5].Header.Caption = "ѕравило перекодировки";
                associateView.ultraGrid1.DisplayLayout.Bands[0].Columns["ObjectKey"].Hidden = true;
            }
        }

        /// <summary>
        /// получение данных по таблицам перекодировок
        /// </summary>
        /// <param name="Association"></param>
        private void InitAssociationPage(IAssociation Association)
        {
            DataTable dt = GetConversionTables(Association);
            this.associateView.ultraGrid1.DataSource = dt;
            ultraGrid1_InitializeLayout(this.associateView.ultraGrid1, new InitializeLayoutEventArgs(this.associateView.ultraGrid1.DisplayLayout));
        }

        void ultraGrid1_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            if (!e.Layout.Bands[0].Columns.Exists("GoToConversionTables"))
            {
                UltraGridColumn clmn = e.Layout.Bands[0].Columns.Add("GoToConversionTables");
                clmn.Header.VisiblePosition = 0;
                UltraGridHelper.SetLikelyButtonColumnsStyle(clmn, -1);
                clmn.CellButtonAppearance.Image = this.associateView.ilTools.Images[10];
            }
        }

        private void ultraGrid1_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            string fullTableName = e.Cell.Row.Cells[0].Value.ToString() + "." + e.Cell.Row.Cells["ObjectKey"].Value.ToString();;
			Workplace.SwitchTo(" лассификаторы и таблицы", typeof(TranslationsNavigationListUI).FullName, fullTableName);
        }

        void ultraGrid1_InitializeRow(object sender, Infragistics.Win.UltraWinGrid.InitializeRowEventArgs e)
        {
            e.Row.Cells["GoToConversionTables"].ToolTipText = "ѕерейти на интерфейс таблиц перекодировок";
        }

        private DataTable GetConversionTables(IAssociation association)
        {
            if (association == null)
                return new DataTable();
            string associationKey = association.ObjectKey;
            IConversionTableCollection collection = this.Workplace.ActiveScheme.ConversionTables;
            DataTable conversionTables = collection.GetDataTable();

            DataRow[] rows = conversionTables.Select(String.Format("NAME like '{0}'", associationKey));
            DataTable table = conversionTables.Clone();
            foreach (DataRow selectRow in rows)
            {
                DataRow row = table.Rows.Add(null, null);
                row.ItemArray = selectRow.ItemArray;
            }
            return table;
        }
    }
}
