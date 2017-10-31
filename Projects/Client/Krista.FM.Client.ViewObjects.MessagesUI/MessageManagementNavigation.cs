#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.ServerLibrary;
using Resources = Krista.FM.Client.ViewObjects.MessagesUI.Properties.Resources;
using ToolTip = System.Windows.Forms.ToolTip;

#endregion


namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    /// <summary>
    /// Пользовательские сообщения
    /// </summary>
    public partial class MessageManagementNavigation : BaseNavigationCtrl
    {
        #region fields

        private static MessageManagementNavigation instance;
        private static ImageList imList;

        #region Const

        private const int ImportanceWidth = 13;
        private const int UnReadWidth = 15;
        private const int AttachmentWidth = 21;
        private const int TransferWidth = 15;
        private const int SelectColumnWidth = 20;
        private const int GroupingWidth = 36;

        private const int AdditionalColumnWidth = ImportanceWidth
                                                  + UnReadWidth
                                                  + AttachmentWidth
                                                  + TransferWidth
                                                  + SelectColumnWidth
                                                  + GroupingWidth;

        private const int MinWidthCalc = 400;

        #endregion

        private readonly object obj = new object();
        private readonly Font cellsFont = new Font("Arial", 8);
        private readonly Font cellsFontBold = new Font("Arial", 8, FontStyle.Bold);
        private readonly Font groupingFont = new Font("Arial", 9, FontStyle.Italic);

        private TimedMessageManager timedMessageManager;
        private bool viewReceivedDateColumn;
        private UltraGridRow selectedRow;

        // Коллекция ID выделенных сообщений
        private readonly List<int> selectedMessages = new List<int>();
        List<string> excludeColumns = new List<string>();

        #endregion

        #region Constructor

        public MessageManagementNavigation()
        {
            InitializeComponent();
            InitializeImageList();
            InitializeToolTip();

            instance = this;
            Caption = "Сообщения";

            InitializeUltraGrid();

            Load += MessageManagementNavigation_Load;

            InitializeOtherMethods();
        }

        #endregion

        #region delegates

        private delegate void StringParameterDelegate(string value);

        private delegate void TableParameterDelegate(DataTable value);

        #endregion

        #region properties

        public override Image TypeImage16
        {
            get { return Resources.email_16; }
        }

        public override Image TypeImage24
        {
            get { return Resources.email_24; }
        }

        internal static MessageManagementNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageManagementNavigation();
                }

                return instance;
            }
        }

        #endregion

        #region initialize methods

        public override void Initialize()
        {
            timedMessageManager = (TimedMessageManager)Workplace.TimedMessageManager;

            timedMessageManager.OnReciveMessages += TimedMessageManager_OnReciveMessages;
            timedMessageManager.OnStartReciveMessages += timedMessageManager_OnStartReciveMessages;
            if (timedMessageManager.MessagesTable != null)
            {
                TimedMessageManager_OnReciveMessages(null, null);
            }

            buttonEdit.Visible = UserInAdminGroup();
        }

        private void InitializeToolTip()
        {
            var toolTip = new ToolTip { AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true };

            toolTip.SetToolTip(deleteMessagesButton, "Удалить выделенные сообщения");
            toolTip.SetToolTip(buttonEdit, "Создать новое сообщение");
            toolTip.SetToolTip(pictureBoxRefresh, "Последнее обновление");
            toolTip.SetToolTip(labelNewMessage, "Количество непрочитанных сообщений");
        }

        private static void InitializeImageList()
        {
            imList = new ImageList();
            imList.TransparentColor = Color.Magenta;
            imList.Images.Add("arrow", Resources.Arrow2);
            imList.Images.Add("openmail", Resources.openmail);
            imList.Images.Add("unopenmail", Resources.unopenmail);
            imList.Images.Add("highimp", Resources.highimp);
            imList.Images.Add("lowinp", Resources.lowinp);
        }

        private void InitializeOtherMethods()
        {
            Bitmap bitmap = new Bitmap(Resources.AddSource);
            bitmap.MakeTransparent(Color.Magenta);
            buttonEdit.Appearance.Image = bitmap;
            buttonEdit.Appearance.ImageHAlign = HAlign.Center;
            buttonEdit.Appearance.ImageVAlign = VAlign.Middle;

            bitmap = new Bitmap(Resources.reload_refresh);
            bitmap.MakeTransparent(Color.Magenta);
            pictureBoxRefresh.Image = bitmap;

            bitmap = new Bitmap(Resources.deleteRows);
            bitmap.MakeTransparent(Color.Magenta);
            deleteMessagesButton.Appearance.Image = bitmap;

            deleteMessagesButton.Visible = false;

            excludeColumns.Add("MessageStatus");
            excludeColumns.Add("SelectBoxColumn");
        }

        private void InitializeUltraGrid()
        {
            ultraGridExMessage.InitializeLayout += UltraGridExMessage_OnGridInitializeLayout;
            ultraGridExMessage.InitializeRow += UltraGridExMessage_InitializeRow;
            ultraGridExMessage.InitializeGroupByRow += UltraGridExMessage_InitializeGroupByRow;
            ultraGridExMessage.Resize += UltraGridExMessage_Resize;
            ultraGridExMessage.DoubleClickCell += UltraGridExMessage_DoubleClickCell;
            ultraGridExMessage.AfterRowActivate += UltraGridExMessage_AfterRowActivate;
            ultraGridExMessage.KeyDown += UltraGridExMessage_KeyDown;
            ultraGridExMessage.ClickCellButton += UltraGridExMessage_ClickCellButton;
            ultraGridExMessage.CellChange += ultraGridExMessage_CellChange;
            ultraGridExMessage.MouseDown += ultraGridExMessage_MouseDown;
            ultraGridExMessage.AfterSelectChange += UltraGridExMessage_AfterSelectChange;
            ultraGridExMessage.AfterHeaderCheckStateChanged +=ultraGridExMessage_AfterHeaderCheckStateChanged;
            // image list
            ultraGridExMessage.ImageList = imList;
            ultraGridExMessage.UpdateMode = UpdateMode.OnCellChange;
        }

        #endregion

        #region methods
        
        private bool UserInAdminGroup()
        {
            DataTable dtGrops =
                Workplace.ActiveScheme.UsersManager.GetGroupsForUser(
                    Workplace.ActiveScheme.UsersManager.GetCurrentUserID());
            DataRow[] adminGroup = dtGrops.Select("ID = 1");
            return Convert.ToBoolean(adminGroup[0][2]);
        }

        private void timedMessageManager_OnStartReciveMessages(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(Resources.reload_refresh1);
            bitmap.MakeTransparent(Color.Magenta);
            pictureBoxRefresh.Image = bitmap;
        }

        private void TimedMessageManager_OnReciveMessages(object sender, EventArgs e)
        {
            try
            {
                lock (obj)
                {
                    DataTable table = timedMessageManager.MessagesTable;
                    UpdateDataSource(table);
                    UpdateLabelText(CalculateDate(timedMessageManager.LastUpdate));
                    UpdateLabelMessagesCount(timedMessageManager.NewMessagesCount.ToString());

                    Bitmap bitmap = new Bitmap(Resources.reload_refresh);
                    bitmap.MakeTransparent(Color.Magenta);
                    pictureBoxRefresh.Image = bitmap;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SetSubjectCalcAndMessageStatus(UltraGridRow row, MessageStatus status)
        {
            string colorForInvisibleSymbols =
                (row.Selected)
                ? ColorTranslator.ToHtml(Color.FromArgb(
                    row.Band.Override.SelectedRowAppearance.BackColor.R,
                    row.Band.Override.SelectedRowAppearance.BackColor.G,
                    row.Band.Override.SelectedRowAppearance.BackColor.B))
                : "#ffffff";

            string calcdate = CalculateDate((DateTime) row.Cells["ReceivedDate"].Value).Trim();

            string userName = row.Cells["RefUserSender"].Value.ToString().Trim();

            int dateSize = MessageUIUtils.GetStringSize(
                calcdate,
                (status == MessageStatus.New) ? cellsFontBold : cellsFont,
                CreateGraphics());

            int userNameSize = MessageUIUtils.GetStringSize(
                userName,
                (status == MessageStatus.New) ? cellsFontBold : cellsFont, CreateGraphics());

            if (status == MessageStatus.New)
            {
                row.Cells["UnRead"].Value = imList.Images["unopenmail"];
                row.Cells["SubjectCalc"].Value = String.Format(
                    "{0}{1}<br/>{2}",
                        "<b>" + ((userName == "Администратор" || userName == "Система")
                            ? "<font color=\"#660000\">"
                            : "<font color=\"#000000\">") + userName.Replace(" ", ReplaceSpace(colorForInvisibleSymbols)) + "</font>" + "</b>",
                        (!viewReceivedDateColumn)
                            ? "<b>" + "<font color=\"#000000\">" +
                                MessageUIUtils.Indent(CreateGraphics(), row.Cells["SubjectCalc"].Width - dateSize - userNameSize, cellsFontBold).Replace
                                (" ", ReplaceSpace(colorForInvisibleSymbols)) +
                                calcdate.Replace(" ", ReplaceSpace(colorForInvisibleSymbols)) + "</font>" + "</b>"
                            : String.Empty,
                        "<font color=\"#666666\">" + row.Cells["Subject"].Value +"</font>");

                row.Cells["ReceivedDateCalc"].Appearance.FontData.Bold = DefaultableBoolean.True;
                row.Cells["RefUserSender"].Appearance.FontData.Bold = DefaultableBoolean.True;
            }
            else
            {
                row.Cells["UnRead"].Value = imList.Images["openmail"];
                row.Cells["SubjectCalc"].Value = String.Format(
                    "{0}{1}<br/>{2}{3}{4}",
                    ((userName == "Администратор" || userName == "Система")
                         ? "<font color=\"#660000\">"
                         : "<font color=\"#000000\">") + userName.Replace(" ", ReplaceSpace(colorForInvisibleSymbols)) + "</font>",
                    (!viewReceivedDateColumn)
                        ? MessageUIUtils.Indent(CreateGraphics(), row.Cells["SubjectCalc"].Width - dateSize - userNameSize, cellsFont).Replace
                              (" ", ReplaceSpace(colorForInvisibleSymbols)) +
                          "<font color=\"#000000\">" + calcdate.Replace(" ", ReplaceSpace(colorForInvisibleSymbols)) + "</font>"
                        : String.Empty,
                    "<font color=\"#666666\">",
                    row.Cells["Subject"].Value,
                    "</font>");

                row.Cells["ReceivedDateCalc"].Appearance.FontData.Bold = DefaultableBoolean.False;
                row.Cells["RefUserSender"].Appearance.FontData.Bold = DefaultableBoolean.False;
            }
        }

        private void UpdateDataSource(DataTable table)
        {
            if (ultraGridExMessage.InvokeRequired)
            {
                ultraGridExMessage.BeginInvoke(new TableParameterDelegate(UpdateDataSource), new object[] {table});
                return;
            }

            if (ultraGridExMessage.DataSource == null)
            {
                ultraGridExMessage.DataSource = table;
                return;
            }
            
            DataTable diff;
            try
            {
                diff = MessageUIUtils.GetDifferenceTable(
                    table,
                    (DataTable) ultraGridExMessage.DataSource,
                    excludeColumns);
            }
            catch (GetDifferenceTableException e)
            {
                ultraGridExMessage.DataSource = table;
                return;
            }

            ((DataTable) ultraGridExMessage.DataSource).BeginInit();
            foreach (DataRow row in diff.Rows)
            {
                if (((DataTable) ultraGridExMessage.DataSource).Rows.Contains(row["ID"]))
                {
                    var rows = ((DataTable)ultraGridExMessage.DataSource).Select(String.Format("ID={0}", Convert.ToInt32(row["id"])));

                    foreach (var r in rows)
                    {
                        r.Delete();
                    }
                }
                else
                {
                    ((DataTable) ultraGridExMessage.DataSource).ImportRow(row);
                }
            }

            ((DataTable)ultraGridExMessage.DataSource).AcceptChanges();
            ((DataTable)ultraGridExMessage.DataSource).EndInit();

            foreach (UltraGridBand ultraGridBand in ultraGridExMessage.DisplayLayout.Bands)
            {
                ultraGridBand.SortedColumns.RefreshSort(false);
            }
        }

        private void UpdateLabelText(string text)
        {
            if (labelLastUpdate.InvokeRequired)
            {
                labelLastUpdate.BeginInvoke(new StringParameterDelegate(UpdateLabelText), new object[] {text});
                return;
            }

            labelLastUpdate.Text = text;
        }

        private void UpdateLabelMessagesCount(string count)
        {
            if (labelNewMessage.InvokeRequired)
            {
                labelNewMessage.BeginInvoke(new StringParameterDelegate(UpdateLabelMessagesCount), new object[] {count});
                return;
            }
            if (Convert.ToInt32(count) > 99)
            {
                labelNewMessage.Font = new Font("Arial", 6.5F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
                labelNewMessage.Location = new Point(1, 4);
            }
            else if (Convert.ToInt32(count) > 9 && Convert.ToInt32(count) < 99)
            {
                labelNewMessage.Font = new Font("Arial", 8F, FontStyle.Bold, GraphicsUnit.Point, ((byte) (204)));
                labelNewMessage.Location = new Point(1, 2);
            }
            else
            {
                labelNewMessage.Font = new Font("Arial", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte) (204)));
                labelNewMessage.Location = new Point(4, 2);
            }

            labelNewMessage.Text = count;
        }

        private static void OpenAttachment(MessageAttachmentDTO messageAttachmentDto)
        {
            try
            {
                string path = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()),
                                           messageAttachmentDto.DocumentFileName);
                File.WriteAllBytes(path, messageAttachmentDto.Document);
                Process.Start(path);
            }
            catch (ArgumentException e)
            {
                throw new OpenAttachmentException(e.Message);
            }
            catch (IOException e)
            {
                throw new OpenAttachmentException(e.Message);
            }
            catch (NotSupportedException e)
            {
                throw new OpenAttachmentException(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new OpenAttachmentException(e.Message);
            }
            catch (SecurityException e)
            {
                throw new OpenAttachmentException(e.Message);
            }
        }

        private void MessageManagementNavigation_Load(object sender, EventArgs e)
        {
            labelNewMessage.BackColor = Color.Transparent;
            labelNewMessage.Parent = pictureBoxNew;
        }

        private void DeleteMessagesButton_Click(object sender, EventArgs e)
        {
            // удаляем
            lock (obj)
            {
                if (MessageBox.Show("Вы действительно хотите удалить отмеченные сообщения?",
                                    "Удаление сообщения",
                                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question) ==
                    DialogResult.OK)
                {
                    List<UltraGridRow> deletedRows = new List<UltraGridRow>();
                    foreach (var row in ultraGridExMessage.Rows)
                    {
                        if ((row is UltraGridGroupByRow))
                        {
                            foreach (var r in ((UltraGridGroupByRow)row).Rows)
                            {
                                if (r.Cells["SelectBoxColumn"].GetText(MaskMode.Raw) == "True")
                                {
                                    deletedRows.Add(r);
                                }
                            }
                        }
                    }

                    foreach (var ultraGridRow in deletedRows)
                    {
                        DeleteRow(ultraGridRow);
                    }

                    UnselectAllRows();
                }
            }
        }

         
        private void ultraToolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "CreateRestartService":
                    {
                        CreateMessageForm createMessageForm = new CreateMessageForm
                                (Workplace.ActiveScheme,
                                true,
                                String.Format("Через 10 минут перезапускаю службу {0}:{1}", Workplace.ActiveScheme.Server.Machine, Workplace.ActiveScheme.Server.GetConfigurationParameter("ServerPort")),
                                MessageActualUnit.Minutes,
                                "10",
                                MessageImportance.Importance);
                        if (createMessageForm.ShowDialog() == DialogResult.OK)
                        {
                            createMessageForm.Close();
                        }
                        break;
                    }
                case "ButtonToolMakeRead":
                    {
                        List<UltraGridRow> readRows = new List<UltraGridRow>();
                        foreach (var row in ultraGridExMessage.Rows)
                        {
                            if ((row is UltraGridGroupByRow))
                            {
                                foreach (var r in ((UltraGridGroupByRow)row).Rows)
                                {
                                    if (r.Cells["SelectBoxColumn"].GetText(MaskMode.Raw) == "True" &&
                                        Convert.ToInt32(r.Cells["MessageStatus"].Value) == (int)MessageStatus.New)
                                    {
                                        readRows.Add(r);
                                    }
                                }
                            }
                        }

                        foreach (var ultraGridRow in readRows)
                        {
                            UpdateStatus(ultraGridRow, MessageStatus.Read);
                            RemoveAndCheckSelectedRows(Convert.ToInt32(ultraGridRow.Cells["ID"].Value));
                        }

                        UnselectAllRows();

                        break;
                    }
                case "ButtonToolMakeUnRead":
                    {
                        List<UltraGridRow> readRows = new List<UltraGridRow>();
                        foreach (var row in ultraGridExMessage.Rows)
                        {
                            if ((row is UltraGridGroupByRow))
                            {
                                foreach (var r in ((UltraGridGroupByRow)row).Rows)
                                {
                                    if (r.Cells["SelectBoxColumn"].GetText(MaskMode.Raw) == "True" &&
                                        Convert.ToInt32(r.Cells["MessageStatus"].Value) == (int)MessageStatus.Read)
                                    {
                                        readRows.Add(r);
                                    }
                                }
                            }
                        }

                        foreach (var ultraGridRow in readRows)
                        {
                            UpdateStatus(ultraGridRow, MessageStatus.New);
                            RemoveAndCheckSelectedRows(Convert.ToInt32(ultraGridRow.Cells["ID"].Value));
                        }

                        UnselectAllRows();

                        break;
                    }
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            CreateMessageForm createMessageForm = new CreateMessageForm(Workplace.ActiveScheme);
            createMessageForm.ShowDialog();
        }

        #endregion

        #region Helper methods

        private string ReplaceSpace(string color)
        {
            return String.Format("<font color=\"{0}\">_</font>", color);
        }

        private string CalculateDate(DateTime dateTime)
        {
            DateTime groupByDate = dateTime;

            if (groupByDate.Date == DateTime.Today)
            {
                return groupByDate.ToString("HH:mm");
            }

            if (groupByDate.Date > DateTime.Today.AddDays(-7f))
            {
                return String.Format(
                    "{0} {1}",
                    GetDayOfWeekName(groupByDate),
                    groupByDate.ToString("HH:mm"));
            }

            return groupByDate.ToString("dd.MM.yyyy");
        }

        private string GetDayOfWeekName(DateTime groupByDate)
        {
            switch (groupByDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Вс";
                case DayOfWeek.Monday:
                    return "Пн";
                case DayOfWeek.Tuesday:
                    return "Вт";
                case DayOfWeek.Wednesday:
                    return "Ср";
                case DayOfWeek.Thursday:
                    return "Чт";
                case DayOfWeek.Friday:
                    return "Пт";
                case DayOfWeek.Saturday:
                    return "Сб";
            }

            return string.Empty;
        }

        private void UnselectAllRows()
        {
            // unselect all
            foreach (var row in ultraGridExMessage.Rows)
            {
                if ((row is UltraGridGroupByRow))
                {
                    foreach (var r in ((UltraGridGroupByRow)row).Rows)
                    {
                        if (r.Cells["SelectBoxColumn"].GetText(MaskMode.Raw) == "True")
                        {
                            r.Cells["SelectBoxColumn"].SetValue(false, false);
                        }
                    }
                }
            }

            ultraGridExMessage.DisplayLayout.Bands[0].Columns["SelectBoxColumn"]
                .SetHeaderCheckedState(ultraGridExMessage.Rows, CheckState.Unchecked);
            selectedMessages.Clear();
            deleteMessagesButton.Visible = false;
        }

        #endregion

    }
}
