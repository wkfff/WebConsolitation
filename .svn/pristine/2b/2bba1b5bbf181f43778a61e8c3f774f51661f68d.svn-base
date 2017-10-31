using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Reflection;
using System.Xml;
using ADODB;
using Infragistics.Win.UltraWinGrid;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.MDXExpert.Common;
using Krista.FM.Client.MDXExpert.Data;
using Krista.FM.Common.Xml;
using Microsoft.AnalysisServices.AdomdClient;
using System.Threading;

namespace Krista.FM.Client.MDXExpert.Controls
{
    public partial class DrillThroughForm : Form
    {
        private DataTable dataSource;
        private PivotData _pivotData;
        private Operation operation = null;
        private BackgroundWorker bw;
        private IWin32Window owner;
        private string actionName = String.Empty;

        public Operation Operation
        {
            get { return operation; }
            set { operation = value; }
        }


        public PivotData PivotData
        {
            get { return _pivotData; }
            set { _pivotData = value; }
        }

        public DataTable DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                SetDataSource(value);
            }
        }

        public DrillThroughForm(IWin32Window owner, PivotData pivotData)
        {
            InitializeComponent();
            this.owner = owner;

            this.PivotData = pivotData;

            bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        private void SetDataSource(DataTable ds)
        {
            // добавление в таблицу строки с контрольными суммами
            /*if ((ds != null)&&(ds.Rows.Count > 0))
            {
                ds.Rows.Add(DBNull.Value);
                foreach (DataColumn col in ds.Columns)
                {
                    if (col.DataType == typeof (Double))
                    {
                        double value = 0;
                        for (int i = 0; i < ds.Rows.Count - 1; i++)
                        {
                            if (ds.Rows[i][col] != DBNull.Value)
                                value += (double) ds.Rows[i][col];
                        }
                        ds.Rows[ds.Rows.Count - 1][col] = value;
                    }

                    if (col.DataType == typeof (Decimal))
                    {
                        Decimal value2 = 0;
                        for (int i = 0; i < ds.Rows.Count - 1; i++)
                        {
                            if (ds.Rows[i][col] != DBNull.Value)
                                value2 += (Decimal) ds.Rows[i][col];
                        }
                        ds.Rows[ds.Rows.Count - 1][col] = value2;
                    }
                }
            }*/

            gDrillThroughData.DataSource = ds;
            
            SetGridReadOnly();
            SetColumnNames();
        }

        private void SetGridReadOnly()
        {
            foreach (UltraGridColumn column in gDrillThroughData.DisplayLayout.Bands[0].Columns)
            {
                column.CellActivation = Activation.NoEdit;
            }
            //возможность сортировки данных по столбцам
            if (gDrillThroughData.DisplayLayout.Bands[0].Columns.Count > 0)
            {
                gDrillThroughData.DisplayLayout.Bands[0].Columns[0].SortIndicator = SortIndicator.Ascending;
            }
        }

        /// <summary>
        /// Перегруппировка колонок в таблице источнике в соответствии с представлением в гриде
        /// </summary>
        private void ReOrdinalSourceTable()
        {
            foreach (UltraGridColumn column in gDrillThroughData.DisplayLayout.Bands[0].Columns)
            {
                int visiblePosition = column.RowLayoutColumnInfo.Column.Header.VisiblePosition;

                if (column.Index != visiblePosition)
                    this.dataSource.Columns[column.Key].SetOrdinal(visiblePosition);
            }

        }



        /// <summary>
        /// Установить имена для столбцов
        /// </summary>
        private void SetColumnNames()
        {
            foreach (UltraGridColumn column in gDrillThroughData.DisplayLayout.Bands[0].Columns)
            {
                column.Header.Caption = GetNameFromUniqueName(column.Header.Caption);
            }

            if (this.dataSource != null)
            {
                foreach (DataColumn column in this.dataSource.Columns)
                {
                    column.Caption = GetNameFromUniqueName(column.ColumnName);
                }
            }

        }



        /// <summary>
        /// проверка включен ли drillthrough для текущего куба
        /// </summary>
        /// <returns>true - включен</returns>
        private bool IsDrillTroughEnabled()
        {
            if ((bool)this.PivotData.Cube.Properties["IS_DRILLTHROUGH_ENABLED"].Value == false)
            {
                MessageBox.Show(
                    String.Format("Для куба \"{0}\" не включен режим просмотра детальных данных", this.PivotData.Cube.Name),
                    "MDX Expert",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// проверка является ли мера вычисляемой
        /// </summary>
        /// <param name="measureUN">юникнейм меры</param>
        /// <returns>true - если не является</returns>
        private bool IsNotCalculatedMeasure(string measureUN)
        {
            Member measure =
                this.PivotData.Cube.Dimensions["Measures"].Hierarchies["Measures"].Levels["MeasuresLevel"].GetMembers().
                    Find(GetNameFromUniqueName(measureUN));

            if ((measure == null) || (measure.Type == MemberTypeEnum.Formula))
            {
                MessageBox.Show("Для вычислимых показателей невозможно просмотреть детальные данные",
                    "MDX Expert",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                return false;
            }
            return true;
        }

        /// <summary>
        /// получение детальных данных для ячейки показателя
        /// </summary>
        public void ShowDrillThroughData(string measureUN, string rowCellUN, bool rowCellIsTotal, 
            string columnCellUN, bool columnCellIsTotal, string actionName)
        {
            List<List<string>> filterMembers = new List<List<string>>();
            this.actionName = actionName;
            AddMembersToLists(this.PivotData.RowAxis, rowCellUN, rowCellIsTotal, ref filterMembers);
            AddMembersToLists(this.PivotData.ColumnAxis, columnCellUN, columnCellIsTotal, ref filterMembers);
            AddMembersToLists(this.PivotData.FilterAxis, string.Empty, false, ref filterMembers);

            ShowDrillThroughData(measureUN, filterMembers);
        }


        /// <summary>
        /// получение детальных данных для ячейки показателя
        /// </summary>
        public void ShowDrillThroughData(string measureUN, List<List<string>> filterMembers)
        {
            if (!IsDrillTroughEnabled() || !IsNotCalculatedMeasure(measureUN))
            {
                this.Dispose(false);
                return;
            }

            bw.RunWorkerAsync(new object[2] { measureUN, filterMembers });

            if (bw.IsBusy)
            {
                bw.CancelAsync();
            }
        }


        /// <summary>
        /// получение имен элементов с осей и добавление в общий список
        /// </summary>
        /// <param name="axis">ось, с которой получаем элементы</param>
        /// <param name="cell">уникальное имя ячейки</param>
        /// <param name="isTotal">является ли ячейка итогом</param>
        /// <param name="filterMembers">общий список элементов, куда добавляем</param>
        /// <returns>кол-во добавленных элементов</returns>
        private int AddMembersToLists(PivotAxis axis, string cellUN, bool isTotal, ref List<List<string>> filterMembers)
        {
            int listCount = 1;
            List<string> memberList = new List<string>();
            bool isFilters = (axis.AxisType == Data.AxisType.atFilters);

            if (((cellUN != string.Empty) && isTotal) || isFilters)
            {
                foreach (FieldSet fs in axis.FieldSets)
                {
                    memberList = GetFieldSetMembers(fs);
                    filterMembers.Add(memberList);
                    listCount *= memberList.Count;
                }
            }
            else if (cellUN != string.Empty)
            {
                memberList.Add(cellUN);
                filterMembers.Add(memberList);
            }

            return listCount;
        }


        /// <summary>
        /// Считываем из куба запрос детализации данных, если она там прописана в Actions
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        private string GetActionContent(string coordinate)
        {
            //для as2000 работает старая детализация (без RETURN)
            if (PivotData.AnalysisServicesVersion == AnalysisServicesVersion.v2000)
                return String.Empty;

            if (this.actionName == String.Empty)
                return String.Empty;

            AdomdRestrictionCollection oRest = new AdomdRestrictionCollection();
            oRest.Add("ACTION_NAME", this.actionName);
            oRest.Add("CUBE_NAME", PivotData.CubeName);
            oRest.Add("COORDINATE", coordinate);
            //oRest.Add("ACTION_TYPE", 256);
            oRest.Add("COORDINATE_TYPE", 6);
            DataSet ds = PivotData.AdomdConn.GetSchemaDataSet("MDSCHEMA_ACTIONS", oRest, true);
            
            string content = String.Empty;
            if (ds.Tables[0].Rows.Count > 0)
                content = (string)ds.Tables[0].Rows[0]["CONTENT"];

            //заменяем координату, на нашу, которую задавали изначально, т.к. иногда возникают коллизии 
            //при использовании спецсимволов в именах элементов
            if (!String.IsNullOrEmpty(content))
            {
                int startCoordinateInd = content.IndexOf("(");
                int endCoordinateInd = content.ToUpper().IndexOf(")  ON 0 FROM");
                if ((startCoordinateInd > -1) && (endCoordinateInd > startCoordinateInd))
                {
                    content = content.Remove(startCoordinateInd, endCoordinateInd - startCoordinateInd + 1);
                    content = content.Insert(startCoordinateInd, coordinate);
                }
            }

            return content;
        }


        /// <summary>
        /// получение данных конкретной ячейки куба
        /// </summary>
        /// <param name="dt">таблица, в которую сливаем данные</param>
        /// <param name="con">подключение</param>
        /// <param name="membersTuple">кортеж элементов, по которым фильтруем</param>
        private void GetCellData(string measureUN, ref DataTable dt, Connection con, string membersTuple)
        {
            string mdx = GetActionContent(String.Format("({0}{1})", measureUN, AddHead(", ", membersTuple)));

            //если для куба не создано действий детализации, то вытаскиваем данные по умолчанию
            if (mdx == "")
            {
                mdx = "DRILLTHROUGH \n";
                mdx += " SELECT ({" + measureUN + "}) ON AXIS(0) \n";
                mdx += FromClause() + "\n";

                if (membersTuple != "")
                    membersTuple = "(" + membersTuple + ")";
                membersTuple = AddHead("WHERE ", membersTuple);
                mdx += membersTuple;
            }
            
            
            Recordset rs = new Recordset();
            OleDbDataAdapter odda = new OleDbDataAdapter();
            try
            {
                rs.Open(mdx, con, 0, LockTypeEnum.adLockUnspecified, 0);
                odda.Fill(dt, rs);
                rs.Close();
            }

            catch (Exception e)
            {
                FormException.ShowErrorForm(e);
            }
            finally
            {
                if (rs.State == (int)ObjectStateEnum.adStateOpen)
                    rs.Close();
            }
        }

        /// <summary>
        /// получение всех включенных мемберов из филдсета
        /// </summary>
        /// <param name="fs">филдсет, из которого получаем</param>
        /// <returns>список включенных элементов</returns>
        private List<string> GetFieldSetMembers(FieldSet fs)
        {
            Hierarchy h = fs.AdomdHierarchy;
            
            if (h == null)
            {
                return new List<string>();
            }

            return GetIncludedMembers(fs.MemberNames, h);
        }

        /// <summary>
        /// получение всех включенных элементов из филдсета
        /// </summary>
        /// <param name="parentMember">узел родительского элемента (в филдсете)</param>
        /// <param name="h">иерархия, к которой принадлежит элемент</param>
        /// <returns>список включенных элементов</returns>
        private List<string> GetIncludedMembers(XmlNode parentMember, Hierarchy h)
        {
            List<string> result = new List<string>();

            if (IsChildrenIncluding(parentMember))
            {
                foreach (XmlNode node in parentMember.ChildNodes)
                {
                    if (!node.HasChildNodes)
                    {
                        result.Add(XmlHelper.GetStringAttrValue(node, "uname", string.Empty));
                    }
                    else
                    {
                        result.AddRange(GetIncludedMembers(node, h));
                    }
                }
            }
            else
            {
                Level lev = null;
                MemberCollection mbrs = null;
                int levelNumber = GetNodeLevel(parentMember);
                if (levelNumber == -1)
                {
                    lev = h.Levels[0];
                    mbrs = lev.GetMembers();
                }
                else
                {
                    lev = h.Levels[levelNumber];
                    string parentUName = XmlHelper.GetStringAttrValue(parentMember, "uname", string.Empty);
                    Member member = lev.GetMembers().Find(GetNameFromUniqueName(parentUName));
                    mbrs = member.GetChildren();
                }

                foreach (Member member in mbrs)
                {
                    bool isIncludedMember = true;
                    foreach (XmlNode excMemberNode in parentMember.ChildNodes)
                    {
                        if ((excMemberNode.HasChildNodes) || (member.UniqueName == XmlHelper.GetStringAttrValue(excMemberNode, "uname", string.Empty)))
                        {
                            isIncludedMember = false;
                            break;
                        }
                    }
                    if (isIncludedMember)
                    {
                        result.Add(member.UniqueName);
                    }
                }

                foreach (XmlNode excMemberNode in parentMember.ChildNodes)
                {
                    if (excMemberNode.HasChildNodes)
                        result.AddRange(GetIncludedMembers(excMemberNode, h));
                }

            }
            return result;
        }

        //возвращает уровень вложенности xml узла относительно корня мемберов
        private int GetNodeLevel(XmlNode node)
        {
            int result = -1;

            while (node.Name != "dummy")
            {
                result++;
                node = node.ParentNode;
            }

            return result;
        }

        #region Утилиты сборки MDX

        /// <summary>
        /// Конкатенация при условии непустой второй части
        /// </summary>
        private string AddHead(string head, string tail)
        {
            return (!string.IsNullOrEmpty(tail)) ? head + tail : string.Empty;
        }

        /// <summary>
        /// Конкатенация при условии непустой первой части
        /// </summary>
        private string AddTail(string head, string tail)
        {
            return (!string.IsNullOrEmpty(head)) ? head + tail : string.Empty;
        }

        /// <summary>
        /// Взятие в скобки.       
        /// </summary>
        /// <param name="src">содержимое</param>
        /// <param name="leftBracket">символ левой скобки</param>
        /// <param name="rightBracket">символ правой скобки</param>        
        private string _Brackets(string src, char leftBracket, char rightBracket)
        {
            if (String.IsNullOrEmpty(src))
                return string.Empty;

            if ((src[0] == leftBracket) && (src[src.Length - 1] == rightBracket))
                return src;

            return leftBracket + src + rightBracket;
        }

        /// <summary>
        /// Взятие в скобки мембера
        /// Пользоваться остарожно, из за оптимизации. 
        /// Например, такую структуру [..] .. [..] не закавычит
        /// </summary>
        /// <param name="src">содержимое</param>        
        private string MemberBrackets(string src)
        {
            return _Brackets(src, '[', ']');
        }

        /// <summary>
        /// Включить (или исключать) перечисленные потомки мембера
        /// </summary>
        private bool IsChildrenIncluding(XmlNode memNode)
        {
            if (memNode != null)
            {
                return (XmlHelper.GetStringAttrValue(memNode, "childrentype", "") == "included");
            }
            else
                return true;
        }

        //получение имени объекта по юникнейму
        private string GetNameFromUniqueName(string uniqueName)
        {
            string[] arrStr = uniqueName.Split('[');
            if (arrStr.Length > 0)
            {
                return ClearBrackets(arrStr[arrStr.Length - 1]);
            }

            return uniqueName;
        }

        //очищаем имя от скобок
        private string ClearBrackets(string source)
        {
            source = source.Replace("[", "");
            int lastBracketPos = source.LastIndexOf(']');
            if (lastBracketPos > 0)
            {
                source = source.Remove(lastBracketPos);
            }
            return source;
        }
        #endregion

        #region FROM - Выражение куба
        /// <summary>
        /// Секция куба
        /// </summary>
        private string FromClause()
        {
            return " FROM " + MemberBrackets(this.PivotData.Cube.Name);
        }
        #endregion        

        private void ExportToExcel(string fileName)
        {
            if (this.dataSource != null)
            {
                if (Operation != null)
                {
                    Operation.StartOperation();
                    Operation.Text = "Экспорт детальных данных в MS Excel";
                }
                try
                {
                    ReOrdinalSourceTable();
                    ExcelUtils.ExportDataTable(this.dataSource, fileName);
                }
                finally
                {
                    if (Operation != null)
                        Operation.StopOperation();
                }

            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] args = (object[])e.Argument;
            string measureUN = (string)args[0];
            List<List<string>> filterMembers = (List<List<string>>)args[1];

            DataTable dt = new DataTable();

            int filterTuplesCount = 1;

            foreach (List<string> members in filterMembers)
            {
                filterTuplesCount *= members.Count;
            }
            
            ADODB.Connection adodbConn = new ADODB.Connection();
            adodbConn.Open(Consts.TmpConnStr, "", "", -1);

            if (filterMembers.Count == 0)
                filterMembers.Add(new List<string>());

            if (Operation != null)
                Operation.StartOperation();

            for (int i = 0; i < filterTuplesCount; i++)
            {
                string membersTuple = "";
                for (int k = 0; k < filterMembers.Count; k++)
                {
                    int cnt = 1;
                    for (int m = 0; m < k; m++)
                    {
                        cnt *= filterMembers[m].Count;
                    }

                    List<string> memberList = filterMembers[k];

                    if (memberList.Count == 0)
                        continue;

                    membersTuple += membersTuple == ""
                                           ? memberList[i / cnt % memberList.Count]
                                           : ", " + memberList[i / cnt % memberList.Count];
                }
                if (Operation != null)
                {
                    if (i > 0)
                    {
                        Operation.Text = String.Format("Получение детальных данных: {0}%", i * 100 / filterTuplesCount);
                    }
                    else
                    {
                        Operation.Text = "Получение детальных данных...";
                    }
                }
                GetCellData(measureUN, ref dt, adodbConn, membersTuple);
            }

            if (Operation != null)
                operation.StopOperation();

            e.Result = dt;    // будет передано в RunWorkerComрleted
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((!e.Cancelled) && (e.Error == null))
            {
                this.DataSource = (DataTable)e.Result;
                this.ShowDialog(this.owner);
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DrillThroughForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose(false);
        }

        private void btExcelExport_Click(object sender, EventArgs e)
        {
            string fileName = "";
            ExportToExcel(fileName);
        }

    }
}