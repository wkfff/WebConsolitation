using Infragistics.Win.UltraWinGrid;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Common
{
    public partial class SessionGrid : UserControl
    {
        private IDictionary<string, ISession> sessions;

        private DataTable sessionsTable;

        public SessionGrid()
        {
            InitializeComponent();
            InfragisticsRusification.LocalizeAll();
            #region события грида
            this.ultraGridEx1.OnRefreshData += new RefreshData(ultraGridEx1_OnRefreshData);
            this.ultraGridEx1.OnGetGridColumnsState += new GetGridColumnsState(ultraGridEx1_OnGetGridColumnsState);
            this.ultraGridEx1.OnClickCellButton += new ClickCellButton(ultraGridEx1_OnClickCellButton);
            this.ultraGridEx1.OnGridInitializeLayout += new GridInitializeLayout(ultraGridEx1_OnGridInitializeLayout);
            this.ultraGridEx1.OnInitializeRow += new InitializeRow(ultraGridEx1_OnInitializeRow);
            #endregion
            this.ultraGridEx1.StateRowEnable = false;
            sessionsTable = this.dsSessions.Tables[0].Clone();
            //this.ultraGridEx1.SetComponentToState(Krista.FM.Client.Components.UltraGridEx.ComponentStates.readonlyState);
            this.ultraGridEx1.IsReadOnly = true;

            InfragisticComponentsCustomize.CustomizeUltraGridParams(ultraGridEx1._ugData);
        }

        void ultraGridEx1_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (Convert.ToBoolean(e.Row.Cells["isBlocked"].Value))
            {
                e.Row.Appearance.BackColor = Color.DarkOrange;
                e.Row.Cells["BlockSesson"].ToolTipText = "Разблокировать сессию";
                e.Row.Cells["BlockSesson"].ButtonAppearance.Image = this.il.Images[1];
            }
            else
            {
                e.Row.Appearance.ResetBackColor();
                e.Row.Cells["BlockSesson"].ToolTipText = "Заблокировать сессию";
                e.Row.Cells["BlockSesson"].ButtonAppearance.Image = this.il.Images[0];
            }
        }

        void ultraGridEx1_OnGridInitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            // добавим колонку с кнопкой для блокирования сессии пользователя
            UltraGridColumn blockClmn = e.Layout.Bands[0].Columns.Add("BlockSesson");
            UltraGridHelper.SetLikelyButtonColumnsStyle(blockClmn, -1); 
            blockClmn.Header.VisiblePosition = 0;
            blockClmn.CellButtonAppearance.Image = this.il.Images[0];

            // установим фильтр на колонку типов сессий
            ColumnFilter filter = this.ultraGridEx1.ugData.DisplayLayout.Bands[0].ColumnFilters["SessionType"];
            filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.Server);
            filter.FilterConditions.Add(FilterComparisionOperator.NotEquals, SessionClientType.DataPump);
        }

        /// <summary>
        /// обработчик кнопки блокировки сессии
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ultraGridEx1_OnClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            // блокируем сессию
            if (e.Cell.Column.Key == "BlockSesson")
            {
                ISession blockSession = this.GetSession(e.Cell.Row.Cells["ID"].Value.ToString());
                bool sessionBlock = Convert.ToBoolean(e.Cell.Row.Cells["isBlocked"].Value);

                if (!sessionBlock)
                {
                    if (MessageBox.Show("Заблокировать сессию", "Сессиия пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        blockSession.IsBlocked = true;
                        e.Cell.Row.Appearance.BackColor = Color.DarkOrange;
                        e.Cell.ToolTipText = "Разблокировать сессию";
                        e.Cell.ButtonAppearance.Image = this.il.Images[1];
                    }
                }
                else
                {
                    if (MessageBox.Show("Разблокировать сессию", "Сессиия пользователя", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        blockSession.IsBlocked = false;
                        e.Cell.Row.Appearance.ResetBackColor();
                        e.Cell.ToolTipText = "Заблокировать сессию";
                        e.Cell.ButtonAppearance.Image = this.il.Images[0];
                    }
                }
            }
        }

        /// <summary>
        /// получение сессии по ее ID
        /// </summary>
        /// <param name="sessionID"></param>
        /// <returns></returns>
        private ISession GetSession(string sessionID)
        {
            ISession session = null;
            sessions.TryGetValue(sessionID, out session);
            return session;
        }

        /// <summary>
        /// установка параметров для колонок в гриде
        /// </summary>
        /// <returns></returns>
        GridColumnsStates ultraGridEx1_OnGetGridColumnsState(object sender)
        {
            GridColumnsStates states = new GridColumnsStates();

            GridColumnState state = new GridColumnState();
            state.ColumnName = "ID";
            state.ColumnCaption = "ID";
            state.IsHiden = true;
            state.IsSystem = true;
            state.ColumnWidth = 40;
            states.Add("ID", state);

            state = new GridColumnState();
            state.ColumnName = "Login";
            state.ColumnCaption = "Логин";
            state.ColumnWidth = 300;
            states.Add("Login", state);

            state = new GridColumnState();
            state.ColumnName = "ConnectionTime";
            state.ColumnCaption = "Время подключения";
            state.ColumnWidth = 200;
            states.Add("ConnectionTime", state);

            state = new GridColumnState();
            state.ColumnName = "Host";
            state.ColumnCaption = "Машина";
            state.ColumnWidth = 150;
            states.Add("Host", state);

            state = new GridColumnState();
            state.ColumnName = "Application";
            state.ColumnCaption = "Приложение";
            state.ColumnWidth = 400;
            states.Add("Application", state);

            state = new GridColumnState();
            state.ColumnName = "SourcesCount";
            state.ColumnCaption = "Количество выделеных ресурсов";
            state.IsHiden = true;
            state.ColumnWidth = 70;
            states.Add("SourcesCount", state);

            state = new GridColumnState();
            state.ColumnName = "SessionType";
            state.ColumnCaption = "Тип сессии";
            state.IsHiden = true;
            state.ColumnWidth = 200;
            states.Add("SessionType", state);

            state = new GridColumnState();
            state.ColumnName = "SessionType";
            state.ColumnCaption = "";
            state.IsHiden = true;
            state.ColumnWidth = 30;
            states.Add("isBlocked", state);

            return states;
        }

        bool ultraGridEx1_OnRefreshData(object sender)
        {
            return this.Refresh();
        }

        /// <summary>
        /// получение таблицы данных из коллекции сессий
        /// </summary>
        /// <param name="sessions"></param>
        /// <param name="SessionsTable"></param>
        /// <returns></returns>
        bool GetSessionsDataTable(IDictionary<string, ISession> sessions, ref DataTable SessionsTable)
        {
            bool returnValue = true;
            SessionsTable.BeginLoadData();
            try
            {
                foreach (ISession session in sessions.Values)
                {
                    DataRow row = SessionsTable.Rows.Add(null, null);
                    row.BeginEdit();
                    row["ID"] = session.SessionId;
                    row["Login"] = session.Principal.Identity.Name;
                    row["ConnectionTime"] = session.LogonTime;
                    row["Host"] = session.Host;
                    row["Application"] = session.Application;
                    row["SourcesCount"] = session.ResourcesCount;
                    row["SessionType"] = session.ClientType;
                    row["isBlocked"] = session.IsBlocked;
                    row.EndEdit();
                }
            }
            catch
            {
                returnValue = false;
            }
            finally
            {
                SessionsTable.EndLoadData();
            }
            return returnValue;
        }

        /// <summary>
        /// обновление данных в гриде
        /// </summary>
        /// <returns></returns>
        public new bool Refresh()
        {
            this.ultraGridEx1.DataSource = null;
            sessionsTable.Clear();
            if (GetSessionsDataTable(sessions, ref sessionsTable))
            {
                this.ultraGridEx1.DataSource = sessionsTable;
                return true;
            }
            return false;
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
    }
}
