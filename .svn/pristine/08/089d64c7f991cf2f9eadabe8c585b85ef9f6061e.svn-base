using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common;
using Krista.FM.Client.ProcessManager;
using Krista.FM.Client.ViewObjects.MDObjectsManagementUI;
using Krista.FM.Client.ViewObjects.ProtocolsUI;
using Krista.FM.Client.ViewObjects.MessagesUI.Evaluators;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;
using ButtonDisplayStyle = Infragistics.Win.UltraWinGrid.ButtonDisplayStyle;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;
using Resources = Krista.FM.Client.ViewObjects.MessagesUI.Properties.Resources;

namespace Krista.FM.Client.ViewObjects.MessagesUI
{
    public partial class MessageManagementNavigation
    {
        private void UltraGridExMessage_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                e.Layout.ViewStyleBand = ViewStyleBand.OutlookGroupBy;
                e.Layout.Override.RowSizingAutoMaxLines = 3;
                e.Layout.Override.RowSizing = RowSizing.AutoFree;

                // Hide the GroupBy box
                e.Layout.GroupByBox.Hidden = true;

                // Hide tooltip on scrol
                e.Layout.ScrollStyle = ScrollStyle.Immediate;
                e.Layout.Override.TipStyleScroll = TipStyle.Hide;

                // No borders for the rows or cells, and no row selectors
                e.Layout.Override.BorderStyleCell = UIElementBorderStyle.None;
                e.Layout.Override.BorderStyleRow = UIElementBorderStyle.Etched;
                e.Layout.Override.RowSelectors = DefaultableBoolean.False;

                e.Layout.Override.HeaderCheckBoxSynchronization = HeaderCheckBoxSynchronization.None;

                e.Layout.Override.SelectTypeRow = SelectType.Single;

                e.Layout.Override.TipStyleCell = TipStyle.Hide;

                // DIsplay an ellipsis when the cell's text doesn't fit
                e.Layout.Bands[0].Override.CellAppearance.TextTrimming = TextTrimming.EllipsisCharacter;

                e.Layout.Override.AllowDelete = DefaultableBoolean.False;

                // Select a row when any cell is clicked
                e.Layout.Override.CellClickAction = CellClickAction.RowSelect;

                e.Layout.Bands[0].Override.SelectedRowAppearance.BackColor = Color.LightGray;

                e.Layout.Bands[0].Columns["ReceivedDate"].HiddenWhenGroupBy = DefaultableBoolean.True;
                e.Layout.Bands[0].SortedColumns.Add("ReceivedDate", true, true);
                e.Layout.Bands[0].Columns["ReceivedDate"].GroupByEvaluator = new GroupByEvaluator();
                e.Layout.Bands[0].GroupHeadersVisible = false;

                // Format the 'ReceivedDate' column to display the long representation of the date
                e.Layout.Bands[0].Columns["ReceivedDate"].Format =
                    CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;

                // Set the 'ReceivedDate' column's Style to Edit, so no dropdown button is displayed
                e.Layout.Bands[0].Columns["ReceivedDate"].Style = ColumnStyle.Edit;

                e.Layout.Bands[0].Columns["RefUserSender"].Header.Caption = "От";

                e.Layout.Bands[0].Columns["Subject"].CellMultiLine = DefaultableBoolean.True;
                e.Layout.Bands[0].Columns["Subject"].Header.Caption = "Тема";

                // Add unbound columns for the 'Importance' and 'UnRead' fields, if they aren't already there
                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("Importance") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("Importance");
                    e.Layout.Bands[0].Columns["Importance"].DataType = typeof (Bitmap);
                }

                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("UnRead") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("UnRead");
                    e.Layout.Bands[0].Columns["UnRead"].DataType = typeof (Bitmap);
                }

                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("Attachment") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("Attachment");
                    e.Layout.Bands[0].Columns["Attachment"].DataType = typeof (Bitmap);
                }

                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("Transfer") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("Transfer");
                    e.Layout.Bands[0].Columns["Transfer"].CellButtonAppearance.Image = imList.Images[0];
                    e.Layout.Bands[0].Columns["Transfer"].ButtonDisplayStyle = ButtonDisplayStyle.Always;
                }

                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("SubjectCalc") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("SubjectCalc");
                    e.Layout.Bands[0].Columns["SubjectCalc"].DataType = typeof (string);
                    e.Layout.Bands[0].Columns["SubjectCalc"].Header.Caption = "Тема";
                    e.Layout.Bands[0].Columns["SubjectCalc"].Style = ColumnStyle.FormattedText;
                    e.Layout.Bands[0].Columns["SubjectCalc"].CellAppearance.FontData.Name = cellsFont.Name;
                    e.Layout.Bands[0].Columns["SubjectCalc"].CellAppearance.FontData.SizeInPoints =
                        cellsFont.SizeInPoints;
                }

                e.Layout.Bands[0].Columns["SelectBoxColumn"].Style = ColumnStyle.CheckBox;
                e.Layout.Bands[0].Columns["SelectBoxColumn"].Header.CheckBoxVisibility = HeaderCheckBoxVisibility.Always;
                e.Layout.Bands[0].Columns["SelectBoxColumn"].Header.CheckBoxSynchronization = HeaderCheckBoxSynchronization.None;
                e.Layout.Bands[0].Columns["SelectBoxColumn"].Header.Caption = "";
                e.Layout.Bands[0].Columns["SelectBoxColumn"].CellClickAction = CellClickAction.Edit;

                if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Exists("ReceivedDateCalc") == false)
                {
                    ultraGridExMessage.DisplayLayout.Bands[0].Columns.Add("ReceivedDateCalc");
                    e.Layout.Bands[0].Columns["ReceivedDateCalc"].DataType = typeof (string);
                    e.Layout.Bands[0].Columns["ReceivedDateCalc"].Header.Caption = "Получено";
                    e.Layout.Bands[0].Columns["ReceivedDateCalc"].CellAppearance.FontData.Name = cellsFont.Name;
                    e.Layout.Bands[0].Columns["ReceivedDateCalc"].CellAppearance.FontData.SizeInPoints =
                        cellsFont.SizeInPoints;
                }

                // Set the widths of the 'Importance' and 'UnRead' columns
                e.Layout.Bands[0].Columns["Importance"].Width = ImportanceWidth;
                e.Layout.Bands[0].Columns["UnRead"].Width = UnReadWidth;
                e.Layout.Bands[0].Columns["Attachment"].Width = AttachmentWidth;
                e.Layout.Bands[0].Columns["Transfer"].Width = TransferWidth;
                e.Layout.Bands[0].Columns["SelectBoxColumn"].Width = SelectColumnWidth;

                // Set the header captions for the 'Importance' and 'UnRead' columns,
                // and make them the first 2 columns.
                e.Layout.Bands[0].Columns["Importance"].Header.Caption = "!";

                e.Layout.Bands[0].Columns["UnRead"].Header.Caption = ((char) 47).ToString();
                e.Layout.Bands[0].Columns["UnRead"].Header.Appearance.FontData.Name = "Wingdings 2";

                e.Layout.Bands[0].Columns["Attachment"].Header.Caption = ((char) 42).ToString();
                e.Layout.Bands[0].Columns["Attachment"].Header.Appearance.FontData.Name = "Wingdings";

                e.Layout.Bands[0].Columns["Transfer"].Header.Caption = ((char) 104).ToString();
                e.Layout.Bands[0].Columns["Transfer"].Header.Appearance.FontData.Name = "Wingdings 3";

                e.Layout.Bands[0].Columns["SelectBoxColumn"].Hidden = false;
                e.Layout.Bands[0].Columns["MessageStatus"].Hidden = true;
                e.Layout.Bands[0].Columns["MessageImportance"].Hidden = true;
                e.Layout.Bands[0].Columns["MessageType"].Hidden = true;
                e.Layout.Bands[0].Columns["RefMessageAttachment"].Hidden = true;
                e.Layout.Bands[0].Columns["RefUserSender"].Hidden = (ultraGridExMessage.Width - AdditionalColumnWidth) <
                                                                    MinWidthCalc;
                e.Layout.Bands[0].Columns["Subject"].Hidden = (ultraGridExMessage.Width - AdditionalColumnWidth) <
                                                              MinWidthCalc;
                e.Layout.Bands[0].Columns["ReceivedDate"].Hidden = (ultraGridExMessage.Width - AdditionalColumnWidth) <
                                                                   MinWidthCalc;
                e.Layout.Bands[0].Columns["SubjectCalc"].Hidden = (ultraGridExMessage.Width - AdditionalColumnWidth) >=
                                                                  MinWidthCalc;
                e.Layout.Bands[0].Columns["ID"].Hidden = true;
                e.Layout.Bands[0].Columns["TransferLink"].Hidden = true;

                // visible position
                e.Layout.Bands[0].Columns["SelectBoxColumn"].Header.VisiblePosition = 0;
                e.Layout.Bands[0].Columns["SubjectCalc"].Header.VisiblePosition = 1;
                e.Layout.Bands[0].Columns["RefUserSender"].Header.VisiblePosition = 2;
                e.Layout.Bands[0].Columns["Subject"].Header.VisiblePosition = 3;
                e.Layout.Bands[0].Columns["ReceivedDateCalc"].Header.VisiblePosition = 4;
                e.Layout.Bands[0].Columns["Importance"].Header.VisiblePosition = 5;
                e.Layout.Bands[0].Columns["UnRead"].Header.VisiblePosition = 6;
                e.Layout.Bands[0].Columns["Attachment"].Header.VisiblePosition = 7;
                e.Layout.Bands[0].Columns["Transfer"].Header.VisiblePosition = 8;

                // Don't display a caption
                ultraGridExMessage.Text = string.Empty;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["SubjectCalc"].CellMultiLine =
                    DefaultableBoolean.True;

                e.Layout.Override.HeaderPlacement = HeaderPlacement.FixedOnTop;
                e.Layout.AutoFitStyle = AutoFitStyle.None;
            }
            catch (IndexOutOfRangeException exception)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
            catch (NullReferenceException exception)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
            catch (ArgumentNullException exception)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
        }

        private void UltraGridExMessage_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (!e.ReInitialize)
                {
                    HandleSingleRow(e.Row);

                    int id = Convert.ToInt32(e.Row.Cells["ID"].Value);
                    if (selectedMessages.Contains(id))
                    {
                        e.Row.Cells["SelectBoxColumn"].SetValue(true, false);
                        deleteMessagesButton.Visible = selectedMessages.Count > 0;
                    }
                }
            }
            catch (Exception exception)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
        }

        private void HandleSingleRow(UltraGridRow row)
        {
            try
            {
                MessageStatus status =
                    (MessageStatus)
                    (row.Cells["MessageStatus"].Value is int ? (int) row.Cells["MessageStatus"].Value : 1);
                MessageImportance importance =
                    (MessageImportance)
                    (row.Cells["MessageImportance"].Value is int ? (int) row.Cells["MessageImportance"].Value : 3);

                Bitmap importanceBitmap = null;
                switch (importance)
                {
                    case MessageImportance.Unimportant:
                        {
                            importanceBitmap = (Bitmap) imList.Images["lowinp"];
                            break;
                        }

                    case MessageImportance.Importance:
                    case MessageImportance.HighImportance:
                        {
                            importanceBitmap = (Bitmap) imList.Images["highimp"];
                            break;
                        }
                }

                row.Cells["RefUserSender"].Appearance.FontData.Bold = (status == MessageStatus.New)
                                                                          ? DefaultableBoolean.True
                                                                          : DefaultableBoolean.False;
                row.Cells["RefUserSender"].Appearance.ForeColor =
                    (row.Cells["RefUserSender"].Value.ToString() == "Администратор"
                     || row.Cells["RefUserSender"].Value.ToString() == "Система")
                        ? ColorTranslator.FromHtml("#660000")
                        : ColorTranslator.FromHtml("#000000");
                row.Cells["ReceivedDateCalc"].Appearance.FontData.Bold = (status == MessageStatus.New)
                                                                             ? DefaultableBoolean.True
                                                                             : DefaultableBoolean.False;

                row.Cells["Importance"].Value = importanceBitmap;

                SetSubjectCalcAndMessageStatus(row, status);

                row.Cells["ReceivedDateCalc"].Value = CalculateDate((DateTime) row.Cells["ReceivedDate"].Value);

                if (Convert.ToInt32(row.Cells["RefMessageAttachment"].Value) != 0)
                {
                    row.Cells["Attachment"].Value = Resources.clip2;
                }

                if (!String.IsNullOrEmpty(row.Cells["TransferLink"].Value.ToString()))
                {
                    row.Cells["Transfer"].Style = ColumnStyle.EditButton;
                    row.Cells["Transfer"].ButtonAppearance.BackColor = Color.LightGray;
                    row.Cells["Transfer"].ToolTipText = "Перейти к протоколам";
                }

                row.ToolTipText = String.Format("От: {0}\nПолучено: {1}\nСообщение: {2}",
                                                row.Cells["RefUserSender"].Value,
                                                row.Cells["ReceivedDateCalc"].Value,
                                                row.Cells["Subject"].Value);
            }
            catch
            {
            }
        }

        private void UltraGridExMessage_InitializeGroupByRow(object sender, InitializeGroupByRowEventArgs e)
        {
            try
            {
                if (e.Row.Value is DayOfWeek)
                {
                    e.Row.Description = ((string) CultureInfo.CurrentCulture.DateTimeFormat.DayNames.
                                                      GetValue(Convert.ToInt32((DayOfWeek) e.Row.Value))).UppercaseFirst
                        ();
                }
                else
                {
                    if (e.Row.Value is DateTime)
                    {
                        DateTime groupByDate = ((DateTime) e.Row.Value).Date;

                        if (groupByDate == DateTime.Today)
                        {
                            e.Row.Description = "Сегодня";
                        }
                        else if (groupByDate == DateTime.Today.AddDays(-1f))
                        {
                            e.Row.Description = "Вчера";
                        }
                        else if (groupByDate > DateTime.Today.AddDays(-7f))
                        {
                            DayOfWeek dayOfWeek = groupByDate.DayOfWeek;
                            e.Row.Description = ((string) CultureInfo.CurrentCulture.DateTimeFormat.DayNames.
                                                              GetValue((int) dayOfWeek)).UppercaseFirst();
                        }
                        else
                        {
                            e.Row.Description = "Неделю назад или ранее";
                        }
                    }
                    else
                    {
                        e.Row.Description = "Неделю назад или ранее";
                    }
                }

                e.Row.Appearance.FontData.Name = groupingFont.Name;
                e.Row.Appearance.FontData.SizeInPoints = groupingFont.SizeInPoints;
                e.Row.Appearance.FontData.Italic = groupingFont.Italic
                                                       ? DefaultableBoolean.True
                                                       : DefaultableBoolean.False;
                e.Row.ExpandAll();
            }
            catch (Exception exception)
            {
                Trace.TraceError(Diagnostics.KristaDiagnostics.ExpandException(exception));
            }
        }

        private void UltraGridExMessage_Resize(object sender, EventArgs e)
        {
            if (ultraGridExMessage.DisplayLayout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            if (ultraGridExMessage.Width < MinWidthCalc)
            {
                viewReceivedDateColumn = false;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["ReceivedDateCalc"].Hidden = true;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["SubjectCalc"].Hidden = false;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["SubjectCalc"].Width = (ultraGridExMessage.Width -
                                                                                         AdditionalColumnWidth > 30) ? ultraGridExMessage.Width -
                                                                                         AdditionalColumnWidth : 30;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["Subject"].Hidden = true;
                ultraGridExMessage.DisplayLayout.Bands[0].Columns["RefUserSender"].Hidden = true;
                foreach (UltraGridGroupByRow groupRow in ultraGridExMessage.Rows)
                {
                    foreach (UltraGridRow row in groupRow.Rows)
                    {
                        HandleSingleRow(row);
                    }
                }

                return;
            }

            viewReceivedDateColumn = true;
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["SubjectCalc"].Hidden = true;
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["ReceivedDateCalc"].Hidden = false;
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["Subject"].Hidden = false;
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["RefUserSender"].Hidden = false;

            int width = ultraGridExMessage.Width - AdditionalColumnWidth;
            if (width < 0)
            {
                return;
            }

            if (ultraGridExMessage.DisplayLayout.Bands.Count == 0 ||
                ultraGridExMessage.DisplayLayout.Bands[0].Columns.Count == 0)
            {
                return;
            }

            // 55%
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["Subject"].Width = (int) ((width)*0.55);
            // 30%
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["RefUserSender"].Width = (int) ((width)*0.3);
            // 15%
            ultraGridExMessage.DisplayLayout.Bands[0].Columns["ReceivedDateCalc"].Width = (int) ((width)*0.15);

            foreach (UltraGridGroupByRow groupRow in ultraGridExMessage.Rows)
            {
                foreach (UltraGridRow row in groupRow.Rows)
                {
                    HandleSingleRow(row);
                }
            }
        }

        private void UltraGridExMessage_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "Attachment")
                {
                    if (Convert.ToInt32(e.Cell.Row.Cells["RefMessageAttachment"].Value) != 0)
                    {
                        MessageAttachmentDTO messageAttachmentDto =
                            timedMessageManager.GetMessageAttachment(Convert.ToInt32(e.Cell.Row.Cells["ID"].Value));
                        // open attachment
                        if (messageAttachmentDto != null)
                        {
                            OpenAttachment(messageAttachmentDto);
                        }
                    }
                }
            }
            catch (OpenAttachmentException exception)
            {
                Trace.TraceError("При открытии вложения возникло исключение: " + exception.Message);
                Diagnostics.KristaDiagnostics.ExpandException(exception);
            }
        }

        private void UltraGridExMessage_AfterRowActivate(object sender, EventArgs e)
        {
            if (ultraGridExMessage.ActiveRow is UltraGridGroupByRow)
            {
                return;
            }

            try
            {
                if (selectedRow != null)
                {
                    if (Convert.ToInt32(selectedRow.Cells["MessageStatus"].Value) == (int) MessageStatus.New)
                    {
                        UpdateStatus(selectedRow, MessageStatus.Read);
                        selectedRow.Cells["MessageStatus"].Value = (int) MessageStatus.Read;
                    }

                    selectedRow.Selected = false;
                    HandleSingleRow(selectedRow);
                }
            }
            finally
            {
                selectedRow = null;
            }
        }

        private void UltraGridExMessage_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (e.Type != typeof(UltraGridRow))
            {
                return;
            }

            if (ultraGridExMessage.Selected.Rows.Count != 0)
            {
                HandleSingleRow(ultraGridExMessage.Selected.Rows[0]);
            }
        }

        private void UpdateStatus(UltraGridRow row, MessageStatus status)
        {
            {
                lock (obj)
                {
                    row.Cells["MessageStatus"].Value = (int)status;
                    timedMessageManager.UpdateMessage(Convert.ToInt32(row.Cells["ID"].Value),
                                                      status);
                    SetSubjectCalcAndMessageStatus(row, status);
                    MakeMessageRead((status == MessageStatus.Read));
                    row.Update();
                }
            }
        }

        private void MakeMessageRead(bool read)
        {
            timedMessageManager.NewMessagesCount = (read) ? timedMessageManager.NewMessagesCount - 1 : timedMessageManager.NewMessagesCount + 1;
            UpdateLabelMessagesCount(timedMessageManager.NewMessagesCount.ToString());
            Workplace.MainStatusBar.Panels["Messages"].Text = String.Format("Новых сообщений: {0}",
                                                                            timedMessageManager.NewMessagesCount);
            Workplace.MainStatusBar.Panels["Messages"].Visible = timedMessageManager.NewMessagesCount > 0;
        }

        private void UltraGridExMessage_KeyDown(object sender, KeyEventArgs e)
        {
            UltraGrid grid = (UltraGrid) sender;
            if (e.KeyCode == Keys.Delete)
            {
                if (grid.ActiveRow != null)
                {
                    if (
                        MessageBox.Show("Вы действительно хотите удалить выделенное сообщение?", "Удаление сообщения",
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        DeleteRow(grid.ActiveRow);

                        e.Handled = true;
                    }
                }
            }

            if (e.KeyCode == Keys.F9)
            {
                timedMessageManager_OnStartReciveMessages(null, null);
                timedMessageManager.ReceiveMessages();
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (ultraGridExMessage.ActiveRow.Selected && ultraGridExMessage.ActiveRow is UltraGridRow)
                {
                    selectedRow = ultraGridExMessage.ActiveRow;
                }
            }
        }

        private void DeleteRow(UltraGridRow ultraGridRow)
        {
            lock (obj)
            {
                if (Convert.ToInt32(ultraGridRow.Cells["MessageStatus"].Value) == (int)MessageStatus.New)
                {
                    MakeMessageRead(true);
                }

                timedMessageManager.DeleteMessage(Convert.ToInt32(ultraGridRow.Cells["ID"].Value));

                RemoveAndCheckSelectedRows(Convert.ToInt32(ultraGridRow.Cells["ID"].Value));

                ultraGridRow.Delete(false);
            }
        }

        private void UltraGridExMessage_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "Transfer")
            {
                switch ((MessageType)Convert.ToInt32(e.Cell.Row.Cells["MessageType"].Value))
                {
                    case MessageType.CubesManagerMessage:
                    case MessageType.PumpProcessCubesMessage:
                        {
                            // Получаем идентификатор пакета
                            string batchID = e.Cell.Row.Cells["TransferLink"].Value.ToString();
                            ViewProtocols(batchID, "Операции обработки многомерных хранилищ", ModulesTypes.MDProcessingModule, "BatchID");
                            break;
                        }

                    case MessageType.PumpMessage:
                        {
                            string pumpID = e.Cell.Row.Cells["TransferLink"].Value.ToString();
                            ViewProtocols(pumpID, "Закачка данных", ModulesTypes.DataPumpModule, "PumpHistoryID");
                            break;
                        }
                    case MessageType.PumpProcessMessage:
                        {
                            string pumpID = e.Cell.Row.Cells["TransferLink"].Value.ToString();
                            ViewProtocols(pumpID, "Операции обработки данных", ModulesTypes.ProcessDataModule, "PumpHistoryID");
                            break;
                        }
                    case MessageType.PumpCheckDataMessage:
                        {
                            string pumpID = e.Cell.Row.Cells["TransferLink"].Value.ToString();
                            ViewProtocols(pumpID, "Операции проверки данных", ModulesTypes.ReviseDataModule, "PumpHistoryID");
                            break;
                        }
                    case MessageType.PumpAssociateMessage:
                        {
                            string pumpID = e.Cell.Row.Cells["TransferLink"].Value.ToString();
                            ViewProtocols(pumpID, "Операции сопоставления данных", ModulesTypes.BridgeOperationsModule, "PumpHistoryID");
                            break;
                        }
                }
            }
        }

        private void ViewProtocols(string pumpID, string protocolsCaption, ModulesTypes modulesTypes, string filteredColumnName)
        {
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                MessageUI messageUi = new MessageUI(protocolsCaption);
                messageUi.Workplace = Workplace;
                messageUi.Initialize();

                IDbDataParameter[] dbParams = null;

                string constraint = filteredColumnName + "= ?";
                dbParams = new[] { db.CreateParameter(filteredColumnName, pumpID, DbType.AnsiString) };

                IInplaceProtocolView inplaceProtocolView = Workplace.ProtocolsInplacer;
                inplaceProtocolView.AttachViewObject(modulesTypes, messageUi.Control, constraint, dbParams);
                ((Workplace.Workplace) Workplace).ShowView(messageUi);
            }
        }

        private void ultraGridExMessage_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "SelectBoxColumn")
            {
                if (e.Cell.GetText(Infragistics.Win.UltraWinMaskedEdit.MaskMode.Raw) == "True")
                {
                    AddAndCheckSelectedRows(Convert.ToInt32(e.Cell.Row.Cells["ID"].Value));
                }
                else
                {
                    RemoveAndCheckSelectedRows(Convert.ToInt32(e.Cell.Row.Cells["ID"].Value));
                }
            }
        }

        #region working with selected items

        private void AddAndCheckSelectedRows(int id)
        {
            if (!selectedMessages.Contains(id))
            {
                selectedMessages.Add(id);
                deleteMessagesButton.Visible = selectedMessages.Count > 0;
            }
        }

        private void RemoveAndCheckSelectedRows(int id)
        {
            if (selectedMessages.Contains(id))
            {
                selectedMessages.Remove(id);
                deleteMessagesButton.Visible = selectedMessages.Count > 0;
            }
        }

        #endregion

        private void ultraGridExMessage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                UIElement element = ultraGridExMessage.DisplayLayout.UIElement.ElementFromPoint(
                    e.Location
                    );

                UltraGridRow row = element.GetContext(
                    typeof (UltraGridRow)) as UltraGridRow;

                UltraGridCell cell = element.GetContext(
                    typeof (UltraGridCell)) as UltraGridCell;

                if (row != null && row.IsDataRow && cell != null &&
                    cell.Column.Key != "Transfer" &&
                    cell.Column.Key != "SelectBoxColumn")
                {
                    row.Selected = true;

                    if (ultraGridExMessage.ActiveRow.Selected && ultraGridExMessage.ActiveRow is UltraGridRow)
                    {
                        selectedRow = ultraGridExMessage.ActiveRow;
                    }
                }
            }
        }

        private void ultraGridExMessage_AfterHeaderCheckStateChanged(object sender, AfterHeaderCheckStateChangedEventArgs e)
        {
            try
            {
                CheckState state = e.Column.GetHeaderCheckedState(e.Rows);

                foreach (var row in e.Rows)
                {
                    if (row is UltraGridGroupByRow)
                    {
                        foreach (var r in ((UltraGridGroupByRow)row).Rows)
                        {
                            if (state == CheckState.Checked)
                            {
                                AddAndCheckSelectedRows(Convert.ToInt32(r.Cells["ID"].Value));
                                r.Cells["SelectBoxColumn"].Value = true;
                            }
                            else if (state == CheckState.Unchecked)
                            {
                                RemoveAndCheckSelectedRows(Convert.ToInt32(r.Cells["ID"].Value));
                                r.Cells["SelectBoxColumn"].Value = false;
                            }       
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
