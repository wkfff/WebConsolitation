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
        // ������, � ������� �������� ��� ������ �������� ����������
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
        /// �������� �� ������������ ����� �� ��� ����������
        /// </summary>
        /// <param name="listError"></param>
        /// <returns></returns>
        private bool CheckUniqueKD(ref List<string> listError, bool checkAllRows)
        {
            foreach (DataTable tbl in dsDisinRules.Tables)
            {
                switch (tbl.TableName)
                {
                    // ��������� ������ �������� �� � ��������������
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
                                        string msg = string.Format("������ � ID = {0} ������� '{1}'. ��� ������ ������ ���� ���������� � �������� ����.",
                                            ID, dataTableCaption);
                                        listError.Add(msg);
                                        // ���� ��������� �� ����� ����������, �� ������� ����� ������, ���� �� ������ ����������
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
        /// �������� ���������� ��������� ������������ ����� � ������������ ����� � �������� ��
        /// � ���������� � ���� �� �������
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool CheckYearInExKD(ref List<string> errors, bool checkAllRecords)
        {
            bool returnValue = true;
            // �������� �� ���� ������� �������� ������ � �����
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
                            errors.Add(string.Format("������ � ID = {0} ������� '{1}'. ���� '���' �� ���������",
                                row.Cells["ID"].Value, row.Band.Header.Caption));
                            returnValue = false;
                        }
                    }
                    int year = Convert.ToInt32(row.Cells["YEAR"].Value);
                    // ��������� ������������ ���� ��������� �� � ����������� �� ����
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
                                            // ��������� ������ ��� ���������� �������
                                            if (childRows[0].RowState != DataRowState.Unchanged || checkAllRecords)
                                            {
                                                if (childRow.Cells["INIT_DATE"].Value == DBNull.Value)
                                                {
                                                    errors.Add(string.Format("������ � ID = {0} ������� '{1}'. ���� '��������� �' �� ���������.",
                                                        childRow.Cells["ID"].Value, row.ChildBands[j].Band.Header.Caption));
                                                    returnValue = false;
                                                }
                                                int yearFromDate = Convert.ToInt32(childRow.Cells["INIT_DATE"].Value);
                                                if (year != Convert.ToInt32(yearFromDate / 10000))
                                                {
                                                    errors.Add(string.Format("������ � ID = {0} ������� '{1}'. ���� ������� � ���������� �� ������������� ���� ��������� �� ������ � ID = {2}",
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
                        // �� ��������� ������ �� ��������
                        if (row.RowState != DataRowState.Deleted)
                            if (row.RowState != DataRowState.Unchanged || checkAllRecords)
                                if (!CheckUniqueExKDSingleRow(row, table))
                                {
                                    string msg  = string.Format("������ � ID = {0} ������� '{1}' ������������ ������ � ID = {2}.",
                                        row["ID"], dataTableCaption, row["REFDISINTRULES_KD"]);

                                    switch (table.TableName.ToUpper())
                                    {
                                        case "DISINTRULES_EX-BOTH":
                                            msg = msg + " �������� '��������� �' � '������' ������� ���� ����������� � �������� ������������ ������.";
                                            break;
                                        case "DISINTRULES_EX-REG":
                                            msg = msg + " �������� '������' ������ ���� ���������� � �������� ������������ ������.";
                                            break;
                                        case "DISINTRULES_EX-PER":
                                            msg = msg + " �������� '��������� �' �������� ������ ���� ���������� � �������� ������������ ������.";
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
            // ������� ������������ ������ �����, � ������ "������ ��������", "�����" � "ID" ������������ ������
            foreach (DataRow checkRows in table.Rows)
            {
                // �� ��������� ������ �� ��������
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
        /// ��������� ���� ��������
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="KD"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private bool CheckCurrentKDProgramm(int ID, string KD, int year, ref List<string> listError)
        {
            // �������� �� ������� �� ��������, ����� �� ��������������
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                // ������ �� �������� �� ���������
                if (row.RowState != DataRowState.Deleted)
                    // ������ � ����� �� ����������
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // ������� ���� � ���� �������
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
                        // ������� ��� ������
                        string selectQuery = string.Format("ID = {0}", row[4]);
                        rows = dsDisinRules.Tables[0].Select(selectQuery);
                        // ������� ���� � ���� �������
                        if (Convert.ToInt32(rows[0][3]) == year)
                            if (!CompareProgrammCodes(KD, row[1].ToString(), row, ref listError))
                                return false;
                    }
            }
            return true;
        }

        /// <summary>
        /// ���������� ���� �������� � ���� ����� �������
        /// </summary>
        /// <param name="currentCode"></param>
        /// <param name="compareCode"></param>
        /// <param name="row"></param>
        /// <param name="listError"></param>
        /// <returns></returns>
        private bool CompareProgrammCodes(string currentCode, string compareCode, DataRow row, ref List<string> listError)
        {
            // ���� ��� ������ ������ 17 ������, �� ������ �������, ��� ������ ����������
            if (currentCode.Length < 17 || compareCode.Length < 17)
                return true;
            // �������� ����� ���� ��� ���� ��������
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
            // ���� ����� �����, �� ��� ��������� ���� ��������
            if ((str1 == str2) && (str3 == str4))
            {
                // ���� �������� ��� ����� ���� ������, ����� ��������� � ��������
                if (compareProgramCode == "0000" && currentProgramCode != "0000")
                {
                    if (listError != null)
                    {
                        listError.Add(string.Format("������ � ID = {0} ������� {1}. ��� �� ���������, ��� ��� ������ ����� �� ��� ������ � ����� �������� 0000",
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
                        listError.Add(string.Format("������ � ID = {0} ������� {1}. ��� �� ���������, ��� ��� ������ ��� ������ � ����� ��������, �������� �� 0000",
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
        /// �������� �������, ������� ����� ������������ ������ �� ��������
        /// </summary>
        private void DeleteUnnormalRows(ref List<string> listError)
        {
            // ������� ������ ������� �������� �������, ������� �� ��������
            List<int> deleteRowsID = new List<int>();
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    deleteRowsID.Add(Convert.ToInt32(row["ID", DataRowVersion.Original]));
                }
            }
            // �� ���� ����� ������� �������� ��������, ���� �� ������, ������� ��������� �������
            foreach (DataTable table in dsDisinRules.Tables)
            {
                if (table.TableName != "DISINTRULES_KD")
                {
                    for (int i = 0; i <= table.Rows.Count - 1; i++)
                    {
                        DataRow row = table.Rows[i];
                        //�� ��������� ��������� ������
                        if (row.RowState != DataRowState.Deleted)
                        {
                            int refDisinRules = Convert.ToInt32(row["REFDISINTRULES_KD"]);
                            // �������� ������ �������� ������, �� ������� ��������� ����������� ������
                            if (dsDisinRules.Tables[0].Select(string.Format("ID = {0}", refDisinRules)).Length == 0)
                            {
                                // ���� �� ����� ������, �� �������� �� ��������� �� �� ��������� ������
                                if (!deleteRowsID.Contains(refDisinRules))
                                {
                                    // ���� ��� ���� ���, �� ����� � �������� ��������� � ������� ��� ������ ������
                                    string strMessage = string.Format("������ � ID {0} ������� '{1}'. ������ ��������� �� �������������� ������������ ������. ������ ���� �������.",
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
                                    string errMessage = string.Format("������ � ID = {0} ������� {1}. �������� ���� '% ����� ��' ������ ���� ����� �������� ���� '% ����. ������ ��' ��� ����.",
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
                            listError.Add(string.Format("������ � ID = {0} ������� '{1}'. �������� �������� �������. ����� ��������� ���������� � ������� ������ ������� �� ������ ��������� 100%",
                                row["ID"], uncorrectRow.Band.Header.Caption));
                        }
                        valideCheck = false;
                    }
            }
            return valideCheck;
        }

        /// <summary>
        ///  �������� ����� ������ � ����� �� �������������
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
                        MessageBox.Show(Workplace.WindowHandle, string.Format("������ � ID = {0}  ������� '{1}'. ���� '{2}' �� ���������", row.Cells["ID"].Value,
                            row.Band.AddButtonCaption, cell.Column.Header.Caption), "��������������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        drv.ugeDisinRul.ugData.ActiveRow.Selected = false;
                        row.Activate();
                        return false;
                    }
            }
            return true;
        }

        /// <summary>
        ///  ��������� ������������ ���������� ��� ������ ������ � ���� ��� �������� � XML
        /// </summary>
        /// <returns></returns>
        private bool CheckNullValues()
        {
            DataSet tmpDs = dsDisinRules.GetChanges();
            if (tmpDs == null)
                return true;
            // ��������� ��� ������
            for (int k = 0; k <= drv.ugeDisinRul.ugData.Rows.Count - 1; k++)
            {
                if (drv.ugeDisinRul.ugData.Rows[k].Cells["PIC"].ToolTipText != "�������")
                    if (!CheckRowNullValues(drv.ugeDisinRul.ugData.Rows[k]))
                        return false;
                    else
                        // ���� ������ �������� ������ ��������� ���������, �������� ����������� ������
                        for (int i = 0; i <= drv.ugeDisinRul.ugData.Rows[k].ChildBands.Count - 1; i++)
                        {
                            for (int j = 0; j <= drv.ugeDisinRul.ugData.Rows[k].ChildBands[i].Rows.Count - 1; j++)
                            {
                                if (drv.ugeDisinRul.ugData.Rows[k].Cells["PIC"].ToolTipText != "�������")
                                    if (!CheckRowNullValues(drv.ugeDisinRul.ugData.Rows[k].ChildBands[i].Rows[j]))
                                        return false;
                            }
                        }
            }
            return true;
        }

        /// <summary>
        /// ��������� ��� ������ � ������ �� ������������
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="KD"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private bool CheckCurrentKD(int ID, string KD, int year)
        {
            // �������� �� ������� �� ��������, ����� �� ��������������
            foreach (DataRow row in dsDisinRules.Tables[0].Rows)
            {
                // ������ �� �������� �� ���������
                if (row.RowState != DataRowState.Deleted)
                    // ������ � ����� �� ����������
                    if (Convert.ToInt32(row[0]) != ID)
                    {
                        // ������� ���� � ���� �������
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
                        // ������� ��� ������
                        string selectQuery = string.Format("ID = {0}", row[4]);
                        rows = dsDisinRules.Tables[0].Select(selectQuery);
                        // ������� ���� � ���� �������
                        if ((row[1].ToString() == KD) && (Convert.ToInt32(rows[0][3]) == year))
                        {

                            return false;
                        }
                    }
            }
            return true;
        }

        /// <summary>
        /// ��������� ������ ������������ UltraGridBand �����
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
            // ������� ��������� � � �����
            this.Workplace.OperationObj.Text = "�������� ������";
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
            // ���� ����� ������ �� ���� ���������� ��������� ����� �� �������
            if (listError.Count > 0)
            {
                if (MessageBox.Show(Workplace.WindowHandle, "� ���������� �������� ������ ���� ���������� ������. ��������� �������� ��������?", "�������� ������", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                == DialogResult.Yes)
                {
                    string fileName = string.Empty;
                    if (ExportImportHelper.GetFileName("�������� �������� ����������", ExportImportHelper.fileExtensions.xls, true, ref fileName))
                    {
                        // ��������� �������� � ������
                        var wb = new Workbook();
                        string str = Path.GetExtension(fileName);
                        if (string.Compare(str, ".xlsx", true) == 0)
                        {
                            wb.SetCurrentFormat( WorkbookFormat.Excel2007);
                        }
                        var ws = wb.Worksheets.Add("Protokol");
                        ws.Rows[0].Cells[0].Value = "�������� �������� ���������� ����������";
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
                MessageBox.Show(Workplace.WindowHandle, "� ���������� �������� ������ ������ ���������� �� ����.", "�������� ������", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
