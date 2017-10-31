using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using System.Data;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui
{
    public class FinSourcePlaningVariantUI : BaseClsUI
    {
        public FinSourcePlaningVariantUI(IEntity entity)
            : base(entity, entity.ObjectKey + "_if")
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            vo.ugeCls.ugData.CellChange += ugData_CellChange;
        }

        // удаление варианта
        protected override void BeforeRowsDeleteOperations(object sender, BeforeRowsDeletedEventArgs e)
        {
            if (vo.ugeCls.ugData.ActiveRow == null)
                return;

            if (e.Cancel) return;

            int variantID = UltraGridHelper.GetActiveID(vo.ugeCls.ugData);
            DataRow[] rows = dsObjData.Tables[0].Select(string.Format("ID = {0}", variantID));
            if (rows.Length == 0)
                return;

            if (rows[0].RowState == DataRowState.Added)
            {
                rows[0].Delete();
                return;
            }

            if (Convert.ToBoolean(rows[0]["VariantCompleted"]))
            {
                e.Cancel = true;
                vo.ugeCls.SetRowToStateByID(variantID, vo.ugeCls.ugData.Rows, UltraGridEx.LocalRowState.Unchanged);
                return;
            }

            if (MessageBox.Show(Workplace.WindowHandle, "Удалить вариант вместе с данными?", "Варианты", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string variantName = UltraGridHelper.GetActiveName(vo.ugeCls.ugData);
                RemoteMetod metod = new RemoteMetod();
                VariantListenerDelegate listener = new VariantListenerDelegate(metod.GetStrFromRemote);
                rows[0].Delete();
                rows[0].AcceptChanges();
                ((IVariantDataClassifier)ActiveDataObj).DeleteVariant(variantID, listener);
                ShowDeleteVariantResults(metod.Messages, variantName);
                // если больше нет никаких изменений, то убираем подсветку кнопок
                if (!IsChanges())
                    vo.ugeCls.BurnChangesDataButtons(false);
            }
            else
            {
                vo.ugeCls.SetRowToStateByID(variantID, vo.ugeCls.ugData.Rows, UltraGridEx.LocalRowState.Unchanged);
            }

            e.Cancel = true;
        }

        private void ShowDeleteVariantResults(List<string> messages, string variantName)
        {
            StringBuilder messageBuilder = new StringBuilder();
            foreach (string msg in messages)
            {
                messageBuilder.AppendLine(msg);
            }
            string message = variantName == string.Empty ?
                String.Format("Результаты удаления варианта: {0}{0}{1}", Environment.NewLine, messageBuilder.ToString()) :
                String.Format("Результаты удаления варианта '{0}': {1}{1}{2}", variantName, Environment.NewLine, messageBuilder.ToString());

            MessageBox.Show(Workplace.WindowHandle, message, "Варианты", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override void ugeCls_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            base.ugeCls_OnInitializeRow(sender, e);

            UltraGridCell lockCell = e.Row.Cells["VariantCompleted"];
            UltraGridCell lockCellButton = e.Row.Cells["VariantCompleted_Image"];
            // если запись только что добавлена руками, ничего не делаем
            if (lockCell.Value == DBNull.Value || lockCell.Value == null)
                return;
            int id = UltraGridHelper.GetRowID(e.Row);
            DataRow[] rows = dsObjData.Tables[0].Select(string.Format("ID = {0}", id));
            if (rows.Length == 0)
                return;
            if (rows[0].RowState == DataRowState.Added)
                return;
            bool variantLocked = Convert.ToBoolean(lockCell.Value);
            if (!variantLocked)
            {
                lockCellButton.Appearance.Image = clsImageList.Images[12];
                lockCellButton.ToolTipText = "Вариант открыт для изменений";
            }
            else
            {
                lockCellButton.Appearance.Image = clsImageList.Images[13];
                lockCellButton.ToolTipText = "Вариант закрыт от изменений";
            }
        }

        protected override void ugeCls_OnAfterRowActivate(object sender, EventArgs e)
        {
            UltraGridRow tmpRow = GetActiveMasterGridRow();

            if (tmpRow.Cells["ID"].Value == DBNull.Value || tmpRow.Cells["ID"].Value == null)
                return;

            int variantId = Convert.ToInt32(tmpRow.Cells["ID"].Value);
            if (variantId <= 0)
            {
                vo.ugeCls.ugData.ActiveRow.Activation = Activation.NoEdit;
                return;
            }

            ButtonTool btnCopyVariant = (ButtonTool)vo.ugeCls.utmMain.Tools["btnCopyVariant"];
            ButtonTool btnLockVariant = (ButtonTool)vo.ugeCls.utmMain.Tools["btnLockUnlockVariant"];
            btnLockVariant.SharedProps.Visible = true;
            btnCopyVariant.SharedProps.Visible = true;

            bool variantLocked = false;
            if (!InInplaceMode)
            {
                UltraGridCell lockCell = tmpRow.Cells["VariantCompleted"];
                bool currentRowIsAdded = false;
                if (tmpRow.Cells["ID"].Value == DBNull.Value)
                {
                    currentRowIsAdded = true;
                }
                else
                {
                    int id = UltraGridHelper.GetRowID(tmpRow);
                    if (dsObjData.Tables[0].Select(string.Format("ID = {0}", id))[0].RowState == DataRowState.Added)
                        currentRowIsAdded = true;
                    else
                    {
                        variantLocked = Convert.ToBoolean(lockCell.Value);
                    }
                }
                SetLockButton(btnLockVariant, btnCopyVariant, variantLocked, currentRowIsAdded);
            }

            if (allowDelRecords)
            {
                if (CheckDeveloperRow(vo.ugeCls.ugData.ActiveRow) && !Workplace.IsDeveloperMode)
                {
                    if (vo.ugeCls.AllowEditRows)
                        vo.ugeCls.AllowEditRows = false;
                    if (vo.ugeCls.AllowDeleteRows)
                        vo.ugeCls.AllowDeleteRows = false;
                }
                else
                {
                    if (!variantLocked)
                    {
                        if (!vo.ugeCls.AllowDeleteRows)
                            vo.ugeCls.AllowEditRows = true;
                        if (!vo.ugeCls.AllowDeleteRows)
                            vo.ugeCls.AllowDeleteRows = true;
                    }
                }
            }

            // для фикcированных вариантов установим невозможность редактирования
            if (Convert.ToInt32(tmpRow.Cells["ID"].Value) < 1)
            {
                //SetLockButton(btnLockVariant, btnCopyVariant, true, false);
                vo.ugeCls.AllowDeleteRows = false;
                vo.ugeCls.AllowEditRows = true;
                btnLockVariant.SharedProps.Visible = false;
                btnCopyVariant.SharedProps.Visible = false;
            }
        }

        #region Функции работы с вариантами

        /// <summary>
        /// устанавливает состояния кнопок для вариантов
        /// </summary>
        /// <param name="button"></param>
        /// <param name="button2"></param>
        /// <param name="toLock"></param>
        /// <param name="addedRow"></param>
        private void SetLockButton(ButtonTool button, ButtonTool button2, bool toLock, bool addedRow)
        {
            if (addedRow)
            {
                button.SharedProps.Enabled = false;
                if (button2 != null)
                    button2.SharedProps.Enabled = false;
                return;
            }

            button.SharedProps.Enabled = allowAddRecord;
            if (button2 != null)
                button2.SharedProps.Enabled = true;
            if (toLock)
            {
                button.SharedProps.AppearancesSmall.Appearance.Image = clsImageList.Images[12];
                button.SharedProps.ToolTipText = "Открыть вариант для изменений";
                vo.ugeCls.AllowDeleteRows = false;
                vo.ugeCls.AllowEditRows = false;
            }
            else
            {
                button.SharedProps.AppearancesSmall.Appearance.Image = clsImageList.Images[13];
                button.SharedProps.ToolTipText = "Закрыть вариант от изменений";
                vo.ugeCls.AllowDeleteRows = true;
                vo.ugeCls.AllowEditRows = true;
            }
        }

        private void SetVariantRow(bool blokedVariant, UltraGridRow row, ButtonTool button)
        {
            SetLockButton(button, null, !blokedVariant, false);
            vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, false);
            row.Cells["VariantCompleted"].Value = Convert.ToInt32(!blokedVariant);
            row.Update();
            dsObjData.AcceptChanges();
            vo.ugeCls.ugData.EventManager.SetEnabled(Infragistics.Win.UltraWinGrid.EventGroups.AllEvents, true);
        }

        private void CopyVariant(UltraGridRow row)
        {
            int variantID = 0;
            string variantName = string.Empty;
            if (row == null)
            {
                MessageBox.Show(Workplace.WindowHandle, "Не выбрана запись для копирования", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Вызываем поиск зависимых данных.
            DialogResult dialogResult =
                FrmDependedData.ShowDependedData(ActiveDataObj, UltraGridHelper.GetActiveID(vo.ugeCls.ugData),
                                                 DependedDataSearchType.CopyVariant, GetFileName(), (Form)Workplace);
            // Если не нажали отмену, то копируем.
            if (dialogResult != DialogResult.Cancel)
            {
                variantID = UltraGridHelper.GetActiveID(vo.ugeCls.ugData);
                variantName = UltraGridHelper.GetActiveName(vo.ugeCls.ugData);
                RemoteMetod metod = new RemoteMetod();
                VariantListenerDelegate listener = new VariantListenerDelegate(metod.GetStrFromRemote);
                ((IVariantDataClassifier)ActiveDataObj).CopyVariant(variantID, listener);
                ShowCopyVariantResults(metod.Messages, variantName);

                Refresh();

                UltraGridRow variantRow =
                    UltraGridHelper.FindGridRow(vo.ugeCls.ugData, "ID", variantID.ToString());
                if (variantRow != null)
                    variantRow.Activate();
            }
        }

        private void ShowCopyVariantResults(List<string> messages, string variantName)
        {
            StringBuilder messageBuilder = new StringBuilder();
            foreach (string msg in messages)
            {
                messageBuilder.AppendLine(msg);
            }
            string message = string.Empty;
            if (variantName == string.Empty)
                message = String.Format("Результаты копирования варианта: {0}{0}{1}", Environment.NewLine, messageBuilder.ToString());
            else
                message = String.Format("Результаты копирования варианта '{0}': {1}{1}{2}", variantName, Environment.NewLine, messageBuilder.ToString());

            MessageBox.Show(Workplace.WindowHandle, message, "Варианты", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //private static string d_Variant_PlanIncomes = "1525f07f-8a60-47af-9b80-7200e74956bc";

        public override void UpdateToolbar()
        {
            UltraToolbar tb = vo.ugeCls.utmMain.Toolbars["utbColumns"];
            // кнопка блокировки варианта
            ButtonTool btnLockVariant = null;
            if (!vo.ugeCls.utmMain.Tools.Exists("btnLockUnlockVariant"))
            {
                btnLockVariant = new ButtonTool("btnLockUnlockVariant");
                btnLockVariant.SharedProps.ToolTipText = "Открыть вариант для изменений";
                btnLockVariant.SharedProps.AppearancesSmall.Appearance.Image = clsImageList.Images[12];
                vo.ugeCls.utmMain.Tools.Add(btnLockVariant);
                tb.Tools.AddTool("btnLockUnlockVariant");
            }
            else
                btnLockVariant = (ButtonTool)vo.ugeCls.utmMain.Tools["btnLockUnlockVariant"];
            btnLockVariant.SharedProps.Visible = true;

            // кнопка копирования варианта
            ButtonTool btnCopyVariant = null;
            if (!vo.ugeCls.utmMain.Tools.Exists("btnCopyVariant"))
            {
                btnCopyVariant = new ButtonTool("btnCopyVariant");
                btnCopyVariant.SharedProps.ToolTipText = "Копировать вариант";
                btnCopyVariant.SharedProps.AppearancesSmall.Appearance.Image = clsImageList.Images[14];
                vo.ugeCls.utmMain.Tools.Add(btnCopyVariant);
                tb.Tools.AddTool("btnCopyVariant");
            }
            else
                btnCopyVariant = (ButtonTool)vo.ugeCls.utmMain.Tools["btnCopyVariant"];
            btnCopyVariant.SharedProps.Visible = true;

            base.UpdateToolbar();
        }

        #endregion

        protected override void ugeCls_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (e.Tool == null) return;
            ButtonTool btn = e.Tool as ButtonTool;
            if (btn == null)
                return;

            if ((btn.Key == "btnCopyVariant" ||
                btn.Key == "btnVariantSplit" ||
                btn.Key == "btnLockUnlockVariant") &&
                vo.ugeCls.GridInGroupByMode(true))
                return;

            int variantID = 0;
            string variantName = string.Empty;
            UltraGridRow row = vo.ugeCls.ugData.ActiveRow;

            switch (btn.Key)
            {
                case "btnCopyVariant":
                    CopyVariant(row);
                    break;
                case "btnLockUnlockVariant":
                    if (row == null)
                    {
                        MessageBox.Show(Workplace.WindowHandle, "Для работы с вариантом необходимо выбрать запись", "Варианты", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    variantID = UltraGridHelper.GetActiveID(vo.ugeCls.ugData);
                    variantName = UltraGridHelper.GetActiveName(vo.ugeCls.ugData);
                    string lockColumnName = "VariantCompleted";
                    bool isVariantLock = Convert.ToBoolean(row.Cells[lockColumnName].Value);
                    string message = string.Empty;
                    if (isVariantLock)
                    {
                        if (variantName != string.Empty)
                            message = string.Format("Открыть для изменений вариант '{0}'?", variantName);
                        else
                            message = "Открыть для изменений вариант?";
                        if (MessageBox.Show(Workplace.WindowHandle, message, "Варианты", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ((IVariantDataClassifier)ActiveDataObj).UnlockVariant(variantID);
                            row.Cells["VariantCompleted_Image"].Appearance.Image = clsImageList.Images[12];
                            row.Cells["VariantCompleted_Image"].ToolTipText = "Вариант открыт для изменений";
                            SetVariantRow(isVariantLock, row, btn);
                        }
                    }
                    else
                    {
                        if (variantName != string.Empty)
                            message = string.Format("Закрыть от изменений вариант '{0}'?", variantName);
                        else
                            message = "Закрыть от изменений вариант?";
                        if (MessageBox.Show(Workplace.WindowHandle, message, "Варианты", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            ((IVariantDataClassifier)ActiveDataObj).LockVariant(variantID);
                            row.Cells["VariantCompleted_Image"].Appearance.Image = clsImageList.Images[13];
                            row.Cells["VariantCompleted_Image"].ToolTipText = "Вариант закрыт от изменений";
                            SetVariantRow(isVariantLock, row, btn);
                        }
                    }
                    break;
                    /*
                case "btnVariantSplit":
                    // вызываем расщепление данных по нормативу
                    if (vo.ugeCls.ugData.ActiveRow == null)
                        return;

                    Workplace.OperationObj.Text = "Расщепление данных";
                    Workplace.OperationObj.StartOperation();
                    string splitDataInfo = string.Empty;
                    try
                    {
                        UltraGridRow acttiveRow = UltraGridHelper.GetActiveRowCells(vo.ugeCls.ugData);
                        // получаем данные, необходимые для расщепления
                        variantID = Convert.ToInt32(acttiveRow.Cells["ID"].Value);
                        int variantType = Convert.ToInt32(acttiveRow.Cells["RefVarD"].Value);
                        int year = Convert.ToInt32(acttiveRow.Cells["RefYear"].Value);
                        IDisintRules disintRulesModule = Workplace.ActiveScheme.DisintRules;
                        splitDataInfo = disintRulesModule.SplitData(variantID, variantType, year);
                        Refresh();
                    }
                    finally
                    {
                        Workplace.OperationObj.StopOperation();
                        MessageBox.Show(Workplace.WindowHandle, splitDataInfo, "Расщепление данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    break;*/
            }

            base.ugeCls_ToolClick(sender, e);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);

            vo.ugeCls.utmMain.Tools["DeleteSelectedRows"].SharedProps.ToolTipText = "Удалить вариант";
            vo.ugeCls.utmMain.Tools["DeleteSelectedRows"].SharedProps.Caption = "Удалить вариант";
            vo.ugeCls.AllowClearTable = false;
            vo.ugeCls.ugData.DisplayLayout.Override.SelectTypeRow = SelectType.Single;
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            foreach (UltraGridBand band in e.Layout.Bands)
            {
                UltraGridColumn btnColumn = band.Columns.Add("VariantCompleted_Image");
                btnColumn.Header.Caption = string.Empty;
                btnColumn.Header.VisiblePosition = 1;
                UltraGridHelper.SetLikelyImageColumnsStyle(btnColumn, -1);
            }

            base.ugeCls_OnGridInitializeLayout(sender, e);
        }

        protected override void ugeCls_OnAfterRowsDelete(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (DataRow row in dsObjData.Tables[0].Rows)
            {
                if (row.RowState != DataRowState.Added)
                    row.AcceptChanges();
            }
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            bool isSaveData = base.SaveData(sender, e);
            foreach (UltraGridRow row in vo.ugeCls.ugData.Rows)
            {
                UltraGridCell lockCellButton = row.Cells["VariantCompleted_Image"];
                if (lockCellButton.Appearance.Image == null)
                    row.Refresh(RefreshRow.FireInitializeRow);
            }
            if (vo.ugeCls.ugData.ActiveRow != null)
            {
                UltraGridRow row = vo.ugeCls.ugData.ActiveRow;
                vo.ugeCls.ugData.ActiveRow = null;
                row.Activate();
            }
            return isSaveData;
        }

        public override void CheckClassifierPermissions()
        {
            base.CheckClassifierPermissions();

            allowAddRecord = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.AddRecord, false);
            allowDelRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.DelRecord, false);
            allowEditRecords = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.EditRecord, false);
            allowImportClassifier = um.CheckPermissionForSystemObject("FinSourcePlaning", (int)FinSourcePlaningOperations.ImportData, false);
            if (!allowAddRecord)
                allowAddRecord = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.AddRecord, false);
            if (!allowDelRecords)
                allowDelRecords = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.DelRecord, false);
            if (!allowEditRecords)
                allowEditRecords = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.EditRecord, false);
            if (!allowImportClassifier)
                allowImportClassifier = um.CheckPermissionForSystemObject("ReferenceBooks", (int)FinSourcePlaningUIModuleOperations.ImportData, false);
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            filterStr = dataQuery;
            if (!InInplaceMode)
                dataQuery = string.IsNullOrEmpty(dataQuery) ? "id <> -1" : dataQuery + " and id <> -1";
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        void ugData_CellChange(object sender, CellEventArgs e)
        {
            if (string.Compare(e.Cell.Column.Key, "CurrentVariant", true) == 0)
            {
                bool currentVariant = false;
                try
                {
                    currentVariant = Convert.ToBoolean(e.Cell.Text);
                }
                catch
                {
                    currentVariant = Convert.ToBoolean(Convert.ToInt32(e.Cell.Text));
                }
                
                if (currentVariant)
                {
                    foreach (DataRow row in dsObjData.Tables[0].Rows)
                    {
                        if (Convert.ToBoolean(row["CurrentVariant"]))
                            row["CurrentVariant"] = false;
                    }
                }
            }
        }
    }
}
