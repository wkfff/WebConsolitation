using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Design.Editors
{
    /// <summary>
    /// Контрол для просмотра сессий
    /// </summary>
    public partial class SessionGridControl : UserControl
    {
        private IDictionary<string, ISession> sessions;

        public SessionGridControl()
            : this(null)
        {
        }

        public SessionGridControl(IDictionary<string, ISession> sessions)
        {
            this.sessions = sessions;

            InitializeComponent();
            this.dataTable1 = new DataTable();
            this.dataColumn1 = new DataColumn();
            this.dataColumn2 = new DataColumn();
            this.dataColumn3 = new DataColumn();
            this.dataColumn4 = new DataColumn();
            this.dataColumn5 = new DataColumn();

            this.dataColumn1.ColumnName = "SessionID";
            this.dataColumn2.ColumnName = "SessionName";
            this.dataColumn3.ColumnName = "LogonTime";
            this.dataColumn4.ColumnName = "Host";
            this.dataColumn5.ColumnName = "Application";

            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5});

            this.sessionGrid.OnGridInitializeLayout += new Krista.FM.Client.Components.GridInitializeLayout(sessionGrid_OnGridInitializeLayout);

            //инициализация грида
            this.sessionGrid.IsReadOnly = true;
            this.sessionGrid.DataSource = this.dataTable1;
            
            this.sessionGrid.ugData.Text = "";
            this.sessionGrid.ugData.DisplayLayout.GroupByBox.Hidden = false;
            this.sessionGrid.AllowAddNewRecords = true;
            this.sessionGrid.OnRefreshData += new Krista.FM.Client.Components.RefreshData(sessionGrid_OnRefreshData);
            this.sessionGrid.ColumnsToolbarVisible = true;
            Refresh();
        }
        /// <summary>
        /// Обновление грида
        /// </summary>
        public new void Refresh()
        {
            dataTable1.Rows.Clear();
    
            if (this.sessions == null)
                return;

            foreach (KeyValuePair<string, ISession> item in this.sessions)
            {
                dataTable1.Rows.Add(item.Value.SessionId, item.Value.Principal.Identity.Name, item.Value.LogonTime, item.Value.Host, item.Value.Application);
            }
            dataTable1.AcceptChanges();
        }

        /// <summary>
        /// При нажатии Обновить
        /// </summary>
        public bool sessionGrid_OnRefreshData(object sender)
        {
            Refresh();
            return true;
        }

        /// <summary>
        /// Свойство для sessions
        /// </summary>
        public IDictionary<string, ISession> Sessions
        {
            get { return sessions; }
            set
            {
                sessions = value;
                Refresh();
            }
        }
        /// <summary>
        /// Срабатывает при инициализации DataSources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sessionGrid_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            UltraGridColumn clmn = band.Columns["SessionID"];
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowFiltering = DefaultableBoolean.False;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 1;
            clmn.Hidden = true; 
            clmn.Header.Caption = "ID сессии";
            clmn.Width = 200;

            clmn = band.Columns["SessionName"];
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.CellActivation = Activation.ActivateOnly;
            clmn.Header.VisiblePosition = 2;
            clmn.Header.Caption = "Логин";
            clmn.Width = 300;

            clmn = band.Columns["LogonTime"];
            clmn.AllowGroupBy = DefaultableBoolean.False;
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 3;
            clmn.Header.Caption = "Время подключения";
            clmn.Width = 140;
            clmn.SortIndicator = SortIndicator.Ascending; 

            clmn = band.Columns["Host"];
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 4;
            clmn.Header.Caption = "Машина";
            clmn.Width = 80;

            clmn = band.Columns["Application"];
            clmn.AllowRowSummaries = AllowRowSummaries.False;
            clmn.Header.VisiblePosition = 5;
            clmn.Header.Caption = "Приложение";
            clmn.Width = 500;
        }
    }
}
