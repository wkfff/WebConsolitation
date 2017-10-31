using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Excel;

using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using CC = Krista.FM.Client.Components;

namespace Krista.FM.Client.ViewObjects.DisintRulesUI
{
    public partial class DisintRulesUI : BaseViewObj
    {
        //private bool CanSaveData = true;

        //*****************************************************************************//
        // модуль, в который вынесены все методы проверки нормативов
        //*****************************************************************************//

        private bool CheckRowsInTable(DataTable dt, ref List<string> listError)
        {
            if (dt.TableName == "DISINTRULES_ALTKD") return true;

            if (!CheckDisinRulesCorrectSumm(dt, ref listError))
            {
                //CanSaveData = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Проверка на уникальность кодов КД при сохранении
        /// </summary>
        /// <param name="listError"></param>
        /// <returns></returns>
        private bool CheckUniqueKD(ref List<string> listError, bool checkAllRows)
        {
            foreach (DataTable tbl in dsDisinRules.Tables)
            {
                switch (tbl.TableName)
                {
                    // проверяем только основные КД и альтернативные
                    case "DISINTRULES_KD":
                    case "DISINTRULES_ALTKD":
                        string dataTableCaption = GetTableCaptionFromName(tbl.TableName);
                        foreach (DataRow row in tbl.Rows)
                        {
                            if (row.RowState != DataRowState.Deleted)
                            {
                                if (row.RowState != DataRowState.Unchanged || checkAllRows)
                                {
                                    int ID = Convert.ToInt32(row["ID"]);
                                    string KD = row["KD"].ToString();
                                    int year = 0;
                                    if (row.Table.Columns.Contains("YEAR"))
                                        year = Convert.ToInt32(row["YEAR"]);
                                    else
                                    {
                                        string selectQuery = string.Format("ID = {0}", row["REFDISINTRULES_KD"]);
                                        DataRow[] rows = dsDisinRules.Tables[0].Select(selectQuery);
                                        if (rows.Length > 0)
                                            year = Convert.ToInt32(rows[0][3]);
                                    }
                                    if (!CheckCurrentKD(ID, KD, year))
                                    {
                                        string msg = string.Format("Запись с ID = {0} таблица '{1}'. Код дохода должен быть уникальным в пределах года.",
                                            ID, dataTableCaption);
                                        listError.Add(msg);
                                        // если проверяем во время сохранения, то удаляем такие записи, дабы не мешали сохранению
                                        if (!checkAllRows)
                                        {
                                            row.AcceptChanges();
                                            row.Delete();
                                        } 
                                    }
                                    CheckCurrentKDProgramm(ID, KD, year, ref listError);
                                }
                            }
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// проверка заполнения некоторых обязательных полей и соответствия годов в основном КД
        /// и исключении к нему по периоду
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool CheckYearInExKD(ref List<string> errors, bool checkAllRecords)
        {
            bool returnValue = true;
            // проходим по всем записям верхнего уровня в гриде
            for (int i = 0; i <= drv.ugeDisinRul.ugData.Rows.Count - 1; i++)
            {
                UltraGridRow row = drv.ugeDisinRul.ugData.Rows[i];
                DataRow[] rows = dsDisinRules.Tables[0].Select(string.Format("ID = {0}", row.Cells["ID"].Value));
                if (rows.Length > 0)
                {
                    if (rows[0].RowState != DataRowState.Unchanged)
                    {
                        if (row.Cells["YEAR"].Value == DBNull.Value)
                        {
                            errors.Add(string.Format("Запись с ID = {0} таблица '{1}'. Поле 'Год' не заполнено",
                                row.Cells["ID"].Value, row.Band.Header.Caption));
                            returnValue = false;
                        }
                    }
                    int year = Convert.ToInt32(row.Cells["YEAR"].Value);
                    // проверяем соответствие года основного КД и исключениям по дате
                    if (row.HasChild())
                        for (int j = 0; j <= row.ChildBands.Count - 1; j++)
                        {
                            switch (row.ChildBands[j].Key)
                            {
                                case "rel3":
                                case "rel2":
                                    for (int k = 0; k <= row.ChildBands[j].Rows.Count - 1; k++)
                                    {
                                        UltraGridRow childRow = row.ChildBands[j].Rows[k];
                                        DataRow[] childRows = dsDisinRules.Tables[childRow.Band.Index - 1].Select(string.Format("ID = {0}", childRow.Cells["ID"].Value));
                                        if (childRows.Length > 0)
                                        {
                                            // проверяем только для измененных записей
                                            if (childRows[0].RowState != DataRowState.Unchanged || checkAllRecords)
                                            {
                                                if (childRow.Cells["INIT_DATE"].Value == DBNull.Value)
                                                {
                                                    errors.Add(string.Format("Запись с ID = {0} таблица '{1}'. Поле 'Действует с' не заполнено.",
                                                        childRow.Cells["ID"].Value, row.ChildBands[j].Band.Header.Caption));
                                                    returnValue = false;
                                                }
                                                int yearFromDate = Convert.ToInt32(childRow.Cells["INIT_DATE"].Value);
                                                if (year != Convert.ToInt32(yearFromDate / 10000))
                                                {
                                                    errors.Add(string.Format("Запись с ID = {0} таблица '{1}'. Дата периода в исключении не соответствует году основного КД записи с ID = {2}",
                                                       childRow.Cells["ID"].Value, childRow.Band.Header.Caption, row.Cells["ID"].Value));
                                                    returnValue = false;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                }
            }
            return returnValue;
        }

        
        private void CheckUniqueExKD(ref List<string> errors, bool checkAllRecords)
        {
            foreach (DataTable table in this.dsDisinRules.Tables)
            {
                string dataTableCaption = GetTableCaptionFromName(table.TableName);
                if (table.TableName != "DISINTRULES_KD" && table.TableName != "DISINTRULES_ALTKD")
                    foreach (DataRow row in table.Rows)
                    {
                        // не проверяем записи на удаление
                        if (row.RowState != DataRowState.Deleted)
                            if (row.RowState != DataRowState.Unchanged || checkAllRecords)
                                if (!CheckUniqueExKDSingleRow(row, table))
                                {
                                    string msg  = string.Format("Запись с ID = {0} таблица '{1}' родительская запись с ID = {2}.",
                                        row["ID"], dataTableCaption, row["REFDISINTRULES_KD"]);

                                    switch (table.TableName.ToUpper())
                                    {
                                        case "DISINTRULES_EX-BOTH":
                                            msg = msg + " Значения 'Действует с' и 'Регион' должены быть уникальными в пределах родительской записи.";
                                            break;
                                        case "DISINTRULES_EX-REG":
                                            msg = msg + " Значение 'Регион' должно быть уникальным в пределах родительской записи.";
                                            break;
                                        case "DISINTRULES_EX-PER":
                                            msg = msg + " Значение 'Действует с' действия должен быть уникальным в пределах родительской записи.";
                                            break;
                                    }
                                    errors.Add(msg);
                                    if (!checkAllRecords)
                                    {
                                        row.AcceptChanges();
                                        row.Delete();
                                    }
                                }
                    }
            }
        }

        bool CheckUniqueExKDSingleRow(DataRow row, DataTable table)
        {
            // сравним уникальность набора полей, а именно "Период действия", "Район" и "ID" родительской записи
            foreach (DataRow checkRows in table.Rows)
            {
                // не проверяем записи на удаление
                if (checkRows.RowState != DataRowState.Deleted)
                    if (checkRows["ID"].ToString() != row["ID"].ToString())
                        if (checkRows["REGION"].ToString() == row["REGION"].ToString() &&
                            checkRows["INIT_DATE"].ToString() == row["INIT_DATE"].ToString() &&
                            checkRows["REFDISINTRULES_KD"].ToString() == row["REFDISINTRULES_KD"].ToString())
                            return false;
            }
            return true;
        }

        /// <summary>
        /// проверяем коды программ
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="KD"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private bool CheckCurrentKDProgramm(int ID, string KD, int year, ref List<string> listError)
        {
            // проверим КД сначала по основным, потом по альтернативным
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                // записи на удаление не проверяем
                if (row.RowState != DataRowState.Deleted)
                    // запись с собой не сравниваем
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // сравним коды и года записей
                        if (Convert.ToInt32(row[3]) == year)
                            if (!CompareProgrammCodes(KD, row[1].ToString(), row, ref listError))
                                return false;
                    }
            }
            DataRow[] rows = null;
            foreach (DataRow row in dsDisinRules.Tables[4].Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // выберем год записи
                        string selectQuery = string.Format("ID = {0}", row[4]);
                        rows = dsDisinRules.Tables[0].Select(selectQuery);
                        // сравним коды и года записей
                        if (Convert.ToInt32(rows[0][3]) == year)
                            if (!CompareProgrammCodes(KD, row[1].ToString(), row, ref listError))
                                return false;
                    }
            }
            return true;
        }

        /// <summary>
        /// сравнивает коды программ у двух кодов доходов
        /// </summary>
        /// <param name="currentCode"></param>
        /// <param name="compareCode"></param>
        /// <param name="row"></param>
        /// <param name="listError"></param>
        /// <returns></returns>
        private bool CompareProgrammCodes(string currentCode, string compareCode, DataRow row, ref List<string> listError)
        {
            // если код дохода меньше 17 знаков, то нахрен выходим, тут нечего сравнивать
            if (currentCode.Length < 17 || compareCode.Length < 17)
                return true;
            // получаем части кода без кода программ
            string currentProgramCode = GetCodePart(currentCode);
            string compareProgramCode = GetCodePart(compareCode);
            string tmpCurrentCode = string.Empty;
            string tmpCompareCode = string.Empty;

            if (currentCode.Length > 17)
                tmpCurrentCode = currentCode.Remove(0, 3);
            else
                tmpCurrentCode = currentCode;
            if (tmpCurrentCode.Length < 17)
                tmpCurrentCode = tmpCurrentCode.PadRight(17, '0');

            if (compareCode.Length > 17)
                tmpCompareCode = compareCode.Remove(0, 3);
            else
                tmpCompareCode = compareCode;
            if (tmpCompareCode.Length < 17)
                tmpCompareCode = tmpCompareCode.PadRight(17, '0');

            string str1 = tmpCurrentCode.Substring(0, 10);
            string str2 = tmpCompareCode.Substring(0, 10);
            string str3 = tmpCurrentCode.Substring(14, 3);
            string str4 = tmpCompareCode.Substring(14, 3);
            // если части равны, то уже проверяем коды программ
            if ((str1 == str2) && (str3 == str4))
            {
                // если подходит под какой либо случай, пишем сообщение в протокол
                if (compareProgramCode == "0000" && currentProgramCode != "0000")
                {
                    if (listError != null)
                    {
                        listError.Add(string.Format("Запись с ID = {0} таблица {1}. Код не корректен, так как введен такой же код дохода с кодом программ 0000",
                            row["ID"], GetTableCaptionFromName(row.Table.TableName)));
                    }
                    else
                    {
                        return false;
                    }
                }
                if (compareProgramCode != "0000" && currentProgramCode == "0000")
                {
                    if (listError != null)
                    {
                        listError.Add(string.Format("Запись с ID = {0} таблица {1}. Код не корректен, так как введен код дохода с кодом программ, отличным от 0000",
                            row["ID"], GetTableCaptionFromName(row.Table.TableName)));
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// удаление записей, которые имеют неправильную ссылку на родителя
        /// </summary>
        private void DeleteUnnormalRows(ref List<string> listError)
        {
            // получим список записей основной таблицы, которые на удаление
            List<int> deleteRowsID = new List<int>();
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    deleteRowsID.Add(Convert.ToInt32(row["ID", DataRowVersion.Original]));
                }
            }
            // во всех кроме главной таблицах проверим, есть ли записи, которые ссылаются вникуда
            foreach (DataTable table in dsDisinRules.Tables)
            {
                if (table.TableName != "DISINTRULES_KD")
                {
                    for (int i = 0; i <= table.Rows.Count - 1; i++)
                    {
                        DataRow row = table.Rows[i];
                        //не проверяем удаленные записи
                        if (row.RowState != DataRowState.Deleted)
                        {
                            int refDisinRules = Convert.ToInt32(row["REFDISINTRULES_KD"]);
                            // получаем запись верхнего уровня, на которую ссылается проверяемая запись
                            if (dsDisinRules.Tables[0].Select(string.Format("ID = {0}", refDisinRules)).Length == 0)
                            {
                                // если не нашли запись, то проверим не ссылается ли на удаленную запись
                                if (!deleteRowsID.Contains(refDisinRules))
                                {
                                    // если все таки нет, то пишем в протокол сообщение и удаляем эту лишнюю запись
                                    string strMessage = string.Format("Запись с ID {0} таблица '{1}'. Запись ссылается на несуществующую родительскую запись. Запись была удалена.",
                                        refDisinRules, GetTableCaptionFromName(table.TableName));
                                    row.AcceptChanges();
                                    row.Delete();
                                    listError.Add(strMessage);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckGOValues(ref List<string> listError, bool checkAllRows)
        {
            foreach (DataTable dt in this.dsDisinRules.Tables)
            {
                if (dt.TableName != "DISINTRULES_ALTKD")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                            if (row.RowState != DataRowState.Unchanged || checkAllRows)
                            {
                                double go_Percent = Convert.ToDouble(row["GO_PERCENT"]);
                                double sum = Convert.ToDouble(row["STAD_PERCENT"]) + Convert.ToDouble(row["MR_PERCENT"]);
                                if ((go_Percent != 0) && (sum != 0) && (sum != go_Percent))
                                {
                                    string errMessage = string.Format("Запись с ID = {0} таблица {1}. Значение поля '% бюжет ГО' должно быть равно значению поля '% конс. бюджет МР' или нулю.",
                                        row["ID"], GetTableCaptionFromName(row.Table.TableName));
                                    listError.Add(errMessage);
                                }
                            }      
                    }
                }
            }
        }

        private bool CheckDisinRulesCorrectSumm(DataTable dt, ref List<string> listError)
        {
            bool valideCheck = true;

            foreach (DataRow row in dt.Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                    if (!CalcDisinRules(row))
                    {
                        UltraGridRow uncorrectRow = UltraGridHelper.FindGridRow(drv.ugeDisinRul.ugData, "ID", row["ID"].ToString());
                        if (uncorrectRow != null)
                        {
                            listError.Add(string.Format("Запись с ID = {0} таблица '{1}'. Неверный норматив доходов. Сумма процентов отчислений в бюджеты разных уровней не должна превышать 100%",
                                row["ID"], uncorrectRow.Band.Header.Caption));
                        }
                        valideCheck = false;
                    }
            }
            return valideCheck;
        }

        /// <summary>
        ///  Проверка одной записи в гриде на заполненность
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool CheckRowNullValues(UltraGridRow row)
        {
            for (int i = 0; i <= row.Cells.Count - 1; i++)
            {
                UltraGridCell cell = row.Cells[i];
                if (!cell.Column.Hidden && cell.Column.Key != "COMMENTS" && cell.Column.Header.Caption != string.Empty)
                    if (cell.Text == string.Empty || cell.Value == null)
                    {
                        MessageBox.Show(Workplace.WindowHandle, string.Format("Запись с ID = {0}  таблицы '{1}'. Поле '{2}' не заполнено", row.Cells["ID"].Value,
                            row.Band.AddButtonCaption, cell.Column.Header.Caption), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        drv.ugeDisinRul.ugData.ActiveRow.Selected = false;
                        row.Activate();
                        return false;
                    }
            }
            return true;
        }

        /// <summary>
        ///  Проверяем правильность заполнения при записи данных в базу или экспорте в XML
        /// </summary>
        /// <returns></returns>
        private bool CheckNullValues()
        {
            DataSet tmpDs = dsDisinRules.GetChanges();
            if (tmpDs == null)
                return true;
            // Пробегаем все записи
            for (int k = 0; k <= drv.ugeDisinRul.ugData.Rows.Count - 1; k++)
            {
                if (drv.ugeDisinRul.ugData.Rows[k].Cells["PIC"].ToolTipText != "Удалено")
                    if (!CheckRowNullValues(drv.ugeDisinRul.ugData.Rows[k]))
                        return false;
                    else
                        // Если запись верхнего уровня заполнена нормально, проверим подчиненные записи
                        for (int i = 0; i <= drv.ugeDisinRul.ugData.Rows[k].ChildBands.Count - 1; i++)
                        {
                            for (int j = 0; j <= drv.ugeDisinRul.ugData.Rows[k].ChildBands[i].Rows.Count - 1; j++)
                            {
                                if (drv.ugeDisinRul.ugData.Rows[k].Cells["PIC"].ToolTipText != "Удалено")
                                    if (!CheckRowNullValues(drv.ugeDisinRul.ugData.Rows[k].ChildBands[i].Rows[j]))
                                        return false;
                            }
                        }
            }
            return true;
        }

        /// <summary>
        /// Проверяет код дохода в записи на уникальность
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="KD"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private bool CheckCurrentKD(int ID, string KD, int year)
        {
            // проверим КД сначала по основным, потом по альтернативным
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                // записи на удаление не проверяем
                if (row.RowState != DataRowState.Deleted)
                    // запись с собой не сравниваем
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // сравним коды и года записей
                        if ((row[1].ToString() == KD) && (Convert.ToInt32(row[3]) == year))
                        {

                            return false;
                        }
                    }
            }
            DataRow[] rows = null;
            foreach (DataRow row in dsDisinRules.Tables[4].Rows)
            {
                if (row.RowState != DataRowState.Deleted)
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // выберем год записи
                        string selectQuery = string.Format("ID = {0}", row[4]);
                        rows = dsDisinRules.Tables[0].Select(selectQuery);
                        // сравним коды и года записей
                        if ((row[1].ToString() == KD) && (Convert.ToInt32(rows[0][3]) == year))
                        {

                            return false;
                        }
                    }
            }
            return true;
        }

        /// <summary>
        /// проверяет записи подчиненного UltraGridBand грида
        /// </summary>
        /// <param name="parentRow"></param>
        /// <param name="childBandName"></param>
        /// <returns></returns>
        private bool CheckChildRows(UltraGridRow parentRow, string childBandName)
        {
            foreach (UltraGridRow childRow in parentRow.ChildBands["rel4"].Rows)
            {
                if (!CalcDisinRules(childRow))
                {
                    return false;
                }
            }
            return true;
        }

        void GetCheckProtocol()
        {
            // ниибацо проверяем и в отчет
            this.Workplace.OperationObj.Text = "Проверка данных";
            this.Workplace.OperationObj.StartOperation();
            List<string> listError = new List<string>();
            try
            {
                CheckYearInExKD(ref listError, true);
                CheckUniqueKD(ref listError, true);
                CheckYearInExKD(ref listError, true);
                CheckGOValues(ref listError, true);

                foreach (DataTable dt in dsDisinRules.Tables)
                {
                    CheckRowsInTable(dt, ref listError);
                }
            }
            finally
            {
                this.Workplace.OperationObj.StopOperation();
            }
            // если нашли ошипки то ваще предлагаем сохранить отчет об ошибках
            if (listError.Count > 0)
            {
                if (MessageBox.Show(Workplace.WindowHandle, "В результате проверки данных были обнаружены ошибки. Сохранить протокол проверки?", "Проверка данных", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                == DialogResult.Yes)
                {
                    string fileName = string.Empty;
                    if (ExportImportHelper.GetFileName("Протокол проверки нормативов", ExportImportHelper.fileExtensions.xls, true, ref fileName))
                    {
                        // сохраняем протокол в файлик
                        var wb = new Workbook();
                        string str = Path.GetExtension(fileName);
                        if (string.Compare(str, ".xlsx", true) == 0)
                        {
                            wb.SetCurrentFormat( WorkbookFormat.Excel2007);
                        }
                        var ws = wb.Worksheets.Add("Protokol");
                        ws.Rows[0].Cells[0].Value = "Протокол проверки нормативов отчислений";
                        int index = 3;
                        foreach (string strErr in listError)
                        {
                            ws.Rows[index].Cells[0].Value = strErr;
                            index++;
                        }
                        wb.Save(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show(Workplace.WindowHandle, "В результате проверки данных ошибок обнаружено не было.", "Проверка данных", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
