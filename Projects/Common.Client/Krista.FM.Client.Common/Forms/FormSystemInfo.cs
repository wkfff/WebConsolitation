using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infragistics.Win.UltraWinEditors;
using Infragistics.Win;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.Design;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Win.UltraWinGrid.ExcelExport;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    public partial class SystemInfoForm : Form
    {
        public SystemInfoForm(InformationState state)
        {
            InitializeComponent();

            InitializeGrid(state);
        }

        private void InitializeGrid(InformationState state)
        {
            this.ultraGrid1.DataSource = state.GetInformation();

            this.ultraGrid1.DisplayLayout.Bands[0].SortedColumns.Add("category", false, true);
            this.ultraGrid1.DisplayLayout.Rows.ExpandAll(true);
            this.ultraGrid1.Selected.Rows.Clear();

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGrid1);
        }

        private void ultraGrid1_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            //TODO: можно раскрашивать  подозрительные строки и т.п.
        }
    }

    public struct SystemInfoStruct
    {
        public static void Show(InformationState state)
        {
            SystemInfoForm systemInfoForm = new SystemInfoForm(state);

            systemInfoForm.ShowDialog();
        }
    }

    /// <summary>
    /// Базовый класс описания объекта (клиент/сервер)
    /// </summary>
    public class InformationState
    {
        protected IScheme scheme;

        public InformationState(IScheme scheme)
        {
            this.scheme = scheme;
        }

        public virtual DataTable GetInformation()
        {
            return null;
        }
    }

    public class ClientState : InformationState
    {
        public ClientState(IScheme scheme)
            : base(scheme)
        {}

        public override DataTable GetInformation()
        {
            return new ClientSystemInfo(scheme).GetInfo();
        }
    }

    public class ServerState : InformationState
    {
        public ServerState(IScheme scheme)
            : base(scheme)
        {}

        public override DataTable GetInformation()
        {
            //TODO: добавить интерфейс схемы, возвращающий инфо о сервере 
            return scheme.ServerSystemInfo;
        }
    }
}
