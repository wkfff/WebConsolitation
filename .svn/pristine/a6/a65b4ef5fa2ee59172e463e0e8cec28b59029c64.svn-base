using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms;
using Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Server;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui
{
    public class CalculationBaseUI : BaseClsUI
    {
        #region внутренние переменные

        internal FinSourcesRererencesUtils finSourcesRererencesUtils;

        protected string currentComment;

        protected ImageList il;

        #endregion


        public CalculationBaseUI(IEntity entity)
            : base(entity, entity.ObjectKey + "_if")
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            finSourcesRererencesUtils = new FinSourcesRererencesUtils(Workplace.ActiveScheme);

            il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.calculator);
            il.Images.Add(Resources.ru.excelDocument);

            FinSourcePlanningNavigation.Instance.VariantChanged += ChangeVariant;

            foreach (UltraTab tab in vo.utcDataCls.Tabs)
            {
                if (tab.Index == 0)
                    continue;
                tab.Visible = false;
            }

            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);

            clsClassType = ClassTypes.clsFactData;

            vo.ugeCls.ServerFilterEnabled = false;
            vo.ugeCls.AllowImportFromXML = false;
            vo.ugeCls.AllowAddNewRecords = false;
            vo.ugeCls.AllowDeleteRows = false;
            vo.ugeCls.SaveMenuVisible = true;

            vo.ugeCls.utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
            vo.ugeCls.ugData.DisplayLayout.GroupByBox.Hidden = false;
            vo.ugeCls.PerfomAction("ShowGroupBy");
            vo.ugeCls.utmMain.Toolbars[1].Visible = false;
            vo.ugeCls.utmMain.Toolbars[2].Visible = false;
            vo.ugeCls.utmMain.Tools["btnSearchText"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["FirstRow"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["LastRow"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["NextRow"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["PrevRow"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["CopyRow"].SharedProps.Visible = false;
            vo.ugeCls.utmMain.Tools["PasteRow"].SharedProps.Visible = false;
        }

        private void SetCurrentVariant(int refVariant)
        {
            CurrentDataSourceID = FinSourcePlanningNavigation.Instance.CurrentSourceID;
            RefVariant = refVariant;
        }

        /// <summary>
        /// очищаем текущий классификатор
        /// </summary>
        protected override void ugeCls_OnClearCurrentTable(object sender)
        {
            if (MessageBox.Show(Workplace.WindowHandle, "Удалить все данные текущей таблицы?", "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Workplace.OperationObj.Text = "Удаление данных текущей таблицы";
                Workplace.OperationObj.StartOperation();
                string message = string.Empty;
                try
                {
                    string deleteFilter = string.Empty;
                    int deletedRowsCount = 0;

                    deleteFilter = string.IsNullOrEmpty(currentComment) ? "where CalcComment is null" :
                        string.Format("where CalcComment = '{0}'", currentComment);
                    ActiveDataObj.DeleteData(deleteFilter, true, ParametersListToValueArray(GetServerFilterParameters()));

                    dsObjData.Tables[0].BeginLoadData();
                    dsObjData.Tables[0].Clear();
                    dsObjData.Tables[0].AcceptChanges();
                    dsObjData.Tables[0].EndLoadData();
                    vo.ugeCls.BurnChangesDataButtons(false);
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceInformation,
                        ActiveDataObj.OlapName, -1, this.CurrentDataSourceID, (int)this.clsClassType, string.Format("{0} Удалено записей: {1}", message, deletedRowsCount));
                }
                catch (Exception exception)
                {
                    protocol.WriteEventIntoClassifierProtocol(ClassifiersEventKind.ceError,
                        FullCaption, -1, this.CurrentDataSourceID, (int)this.clsClassType, "Очистка данных закончилась с ошибками");

                    if (exception.Message.Contains("ORA-02292") || exception.Message.Contains("Конфликт инструкции DELETE с ограничением"))
                    {
                        MessageBox.Show(Workplace.WindowHandle, "Нарушено ограничение целостности. Обнаружена порожденная запись", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (exception.Message.Contains("ORA-20101"))
                    {
                        MessageBox.Show(Workplace.WindowHandle, "Вариант закрыт от изменений. Запись и изменение данных варианта запрещены", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        throw;
                }
                finally
                {
                    CanDeactivate = true;
                    Workplace.OperationObj.StopOperation();
                }
            }
        }

        private object[] ParametersListToValueArray(List<UltraGridEx.FilterParamInfo> parameters)
        {
            if (parameters == null)
                return null;
            object[] values = new object[parameters.Count];
            int counter = 0;
            foreach (UltraGridEx.FilterParamInfo paramInfo in parameters)
            {
                values[counter] = paramInfo.ParamValue;
                counter++;
            }
            return values;
        }

        public override void ugeCls_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            base.ugeCls_OnGridInitializeLayout(sender, e);
            // для периода делаем видимым значением только год
            e.Layout.Bands[0].Columns["RefYearDayUNV"].Header.VisiblePosition = 0;
            e.Layout.Bands[0].Columns["RefYearDayUNV"].MaskInput = "9999";

            e.Layout.Override.AllowColMoving = AllowColMoving.NotAllowed;
            foreach (UltraGridColumn column in e.Layout.Bands[0].Columns)
            {
                column.AllowRowFiltering = DefaultableBoolean.False;
                column.SortIndicator = SortIndicator.Disabled;
                column.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;

            }
            e.Layout.Bands[0].Columns["RefYearDayUNV"].SortIndicator = SortIndicator.Ascending;
        }

        protected override IDataUpdater GetActiveUpdater(int? parentID, out string filterStr)
        {
            if (!string.IsNullOrEmpty(currentComment))
                dataQuery = String.Concat(
                    GetDataSourcesFilter(),
                    String.Format(" and {0} = '{1}'", "CalcComment", currentComment));
            else
                dataQuery = String.Concat(
                GetDataSourcesFilter(), " and 1 = 2");

            filterStr = dataQuery;
            return ActiveDataObj.GetDataUpdater(dataQuery, null, null);
        }

        protected override void LoadData(object sender, EventArgs e)
        {
            base.LoadData(sender, e);
            vo.ugeCls.ugData.DisplayLayout.Bands[0].CardView = true;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].CardSettings.Width = 150;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].CardSettings.LabelWidth = 350;
            vo.ugeCls.ugData.DisplayLayout.Bands[0].CardSettings.MaxCardAreaRows = 1;
            ((StateButtonTool)vo.ugeCls.utmMain.Tools["ID"]).Checked = false;
        }

        public override void SetTaskId(ref UltraGridRow row)
        {
            base.SetTaskId(ref row);
            row.Cells["RefBrwVariant"].Value = FinSourcePlanningNavigation.Instance.CurrentVariantID;
            row.Cells["TaskID"].Value = -1;
        }

        private void ChangeVariant(object sender, EventArgs e)
        {
            SetCurrentVariant(FinSourcePlanningNavigation.Instance.CurrentVariantID);
            vo.ugeCls.BurnRefreshDataButton(true);
        }

        public override bool SaveData(object sender, EventArgs e)
        {
            DataSet changedRecords = dsObjData.GetChanges();
            if (changedRecords == null)
                return false;
            using (IDatabase db = Workplace.ActiveScheme.SchemeDWH.DB)
            {
                string comment = string.Empty;
                if (!SelectCommentForm.ShowSaveCalcResultsForm(GetCommentsList(db), ref comment))
                    return false;
                // удаляем расчеты, которые были под таким же комментарием и в тот же день, что и сохраняемый
                IDbDataParameter[] queryParams = new IDbDataParameter[1];
                queryParams[0] = new System.Data.OleDb.OleDbParameter("CalcComment", comment);
                db.ExecQuery(string.Format("delete from {0} where CalcComment like ?",
                    ActiveDataObj.FullDBName), QueryResultTypes.NonQuery, queryParams);
                ValueListItem item = ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.FindByDataValue(comment);
                if (item != null)
                    ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Remove(item);

                foreach (DataRow row in dsObjData.Tables[0].Rows)
                {
                    // если при сохранении изменений выбран новый комментарий, старые данные по этому комментарию удаляем,
                    // добавляем новые
                    if (row.RowState != DataRowState.Added)
                    {
                        row.AcceptChanges();
                        row.SetAdded();
                        //row["ID"] = GetNewId();
                    }
                    row["CalcComment"] = comment;
                }
                currentComment = comment;
                using (IDataUpdater upd = GetActiveUpdater(null))
                {
                    // сохраняем измененные ( а так же удаленные и добавленные) записи
                    changedRecords = dsObjData.GetChanges();
                    if (changedRecords != null)
                    {
                        Workplace.OperationObj.Text = "Сохранение изменений";
                        Workplace.OperationObj.StartOperation();
                        // если такие записи есть
                        try
                        {
                            // сохраняем основные данные
                            upd.Update(ref changedRecords);
                            // применение всех изменений в источнике данных
                            dsObjData.Tables[0].AcceptChanges();
                            object newComment = ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).ValueList.ValueListItems.Add(currentComment);
                            ((ComboBoxTool)vo.ugeCls.utmMain.Tools["CalculationResults"]).SelectedItem = newComment;
                            Workplace.OperationObj.StopOperation();
                        }
                        catch (Exception exception)
                        {
                            // в случае исключения даем пользователю изменить некорректные данные
                            Workplace.OperationObj.StopOperation();
                            throw new Exception(exception.Message, exception.InnerException);
                        }
                    }
                }
            }
            return true;
        }

        private List<string> GetCommentsList(IDatabase db)
        {
            DataTable dtComments =
                (DataTable)db.ExecQuery(string.Format("select Distinct CalcComment from {0}",
                ActiveDataObj.FullDBName), QueryResultTypes.DataTable);
            List<string> comments = new List<string>();
            foreach (DataRow row in dtComments.Rows)
                comments.Add(row[0].ToString());
            return comments;
        }

        public virtual void ClearData()
        {
            dsObjData.Tables[0].Clear();
        }

        public DataTable GetData()
        {
            return dsObjData.Tables[0];
        }

        public void AddData(DataRow row)
        {
            switch (row.RowState)
            {
                case DataRowState.Added:
                    dsObjData.Tables[0].Rows.Add(row.ItemArray);
                    if (!row.IsNull("ID"))
                        vo.ugeCls.SetRowToStateByID(Convert.ToInt32(row["ID"]), vo.ugeCls.ugData.Rows, UltraGridEx.LocalRowState.Added);
                    vo.ugeCls.BurnChangesDataButtons(true);
                    break;
                case DataRowState.Modified:
                    DataRow[] rows = dsObjData.Tables[0].Select(string.Format("ID = {0}", row["ID"]));
                    if (rows != null && rows.Length > 0)
                    {
                        rows[0].ItemArray = row.ItemArray;
                        vo.ugeCls.SetRowToStateByID(Convert.ToInt32(row["ID"]), vo.ugeCls.ugData.Rows,
                            UltraGridEx.LocalRowState.Modified);
                        vo.ugeCls.BurnChangesDataButtons(true);
                    }
                    break;
            }
        }
    }
}
