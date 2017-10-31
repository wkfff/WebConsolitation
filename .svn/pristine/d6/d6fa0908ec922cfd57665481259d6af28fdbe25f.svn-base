using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Infragistics.Win;
using Infragistics.Win.UltraWinTree;
using Krista.FM.Client.MDXExpert.Common;
using Microsoft.AnalysisServices.AdomdClient;

namespace Krista.FM.Client.MDXExpert.Data
{
    public partial class TotalExpressionForm : Form
    {
        #region поля

        private static string cubeName;
        private PivotData pivotData;
        private PivotTotal pivotTotal;
        private string expression;
        private string measureSource;
        private MeasureFormulaType formulaType;
        private XmlNode filters;
        private string lookupCubeName;
        private bool isLookupMeasure;
        private MDXQueryBuilder mdxQueryBuilder;
        private bool updating;
        private const string tableCaption = "Таблица";
        //признак того, что редактируем существующую вычислимую меру
        private bool isEditTotal = false;

        #endregion

        #region свойства

        public string Expression
        {
            get
            {
                return this.expression;
            }
        }

        public string TotalCaption
        {
            get { return this.teTotalCaption.Text; }
        }

        public PivotData PivotData
        {
            get { return pivotData; }
            set { pivotData = value; }
        }

        public string MeasureSource
        {
            get { return measureSource; }
        }

        public MeasureFormulaType FormulaType
        {
            get { return formulaType; }
        }

        public XmlNode Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public string LookupCubeName
        {
            get { return lookupCubeName; }
            set { lookupCubeName = value; }
        }

        public bool IsLookupMeasure
        {
            get { return isLookupMeasure; }
        }

        #endregion

        public TotalExpressionForm()
        {
            InitializeComponent();
            this.isEditTotal = false;
            pCaptionCont.Visible = true;
        }

        public TotalExpressionForm(PivotTotal pivotTotal)
        {
            InitializeComponent();
            this.isEditTotal = true;
            pCaptionCont.Visible = true;
            this.teTotalCaption.Text = pivotTotal.Caption;
            this.pivotTotal = pivotTotal;
            if (pivotTotal.FormulaType == MeasureFormulaType.Custom)
            {
                //this.expression = pivotTotal.Expression;
                this.teCalcTotalExpression.Text = pivotTotal.Expression;
            }
            this.PivotData = pivotTotal.ParentPivotData;

            this.lookupCubeName = pivotTotal.LookupCubeName;
            this.measureSource = pivotTotal.MeasureSource;
            this.filters = new XmlDocument().CreateNode(XmlNodeType.Element, "filters", null);
            if (pivotTotal.Filters != null)
                this.filters.InnerXml = pivotTotal.Filters.InnerXml;

            this.isLookupMeasure = pivotTotal.IsLookupMeasure;

            Init();
        }

        public TotalExpressionForm(string caption, string expression, PivotData pivotData)
        {
            InitializeComponent();
            this.isEditTotal = false;

            pCaptionCont.Visible = true;
            this.teTotalCaption.Text = caption;
            this.expression = expression;

            this.teCalcTotalExpression.Text = expression;
            this.PivotData = pivotData;
            this.filters = new XmlDocument().CreateNode(XmlNodeType.Element, "filters", null);
            this.isLookupMeasure = false;

            PivotTotal total = new PivotTotal(pivotData, caption, this.expression);

            total.IsLookupMeasure = this.IsLookupMeasure;
            total.LookupCubeName = this.LookupCubeName;

            total.Filters = this.filters;

            total.MeasureSource = this.MeasureSource;
            //total.FormulaType = this.FormulaType;

            this.pivotTotal = total;
            
            Init();
        }

        private void InitCalcTotalExpression()
        {
            if (this.updating)
                return;

            if (this.rbTypicalFormula.Checked)
            {
                bool isDeferDataUpdating = this.PivotData.IsDeferDataUpdating;
                try
                {
                    this.PivotData.IsDeferDataUpdating = true;

                    this.pivotTotal.MeasureSource = (string)cbSourceMeasure.Value;
                    this.pivotTotal.FormulaType = (MeasureFormulaType)cbFormulaType.Value;
                    this.pivotTotal.Filters.InnerXml = (this.Filters != null) ? this.Filters.InnerXml : null;
                    if (ceCubeList.Value != null)
                        this.pivotTotal.LookupCubeName = ((CubeDef)ceCubeList.Value).Name;
                    this.pivotTotal.IsLookupMeasure = IsLookupCube(ceCubeList.Value);

                    this.teCalcTotalExpression.Text = this.mdxQueryBuilder.GetCalcMemberExpression(this.PivotData,
                                                                                                   this.pivotTotal);
                }
                finally
                {
                    this.PivotData.IsDeferDataUpdating = isDeferDataUpdating;
                }

            }

            lDescription.Text = GetFormulaDescription((MeasureFormulaType) cbFormulaType.Value);

            lDescription.Visible = !String.IsNullOrEmpty(lDescription.Text);
        }

        private void Init()
        {
            this.mdxQueryBuilder = new MDXQueryBuilder();
            this.mdxQueryBuilder.ElementType = ReportElementType.eTable;
            this.mdxQueryBuilder.usePaging = false;

            this.updating = true;

            cbFormulaType.Items.Clear();

            cbFormulaType.Items.Add(MeasureFormulaType.RowTotalPercent,
                                    GetFormulaTypeDescription(MeasureFormulaType.RowTotalPercent));
            cbFormulaType.Items.Add(MeasureFormulaType.ColumnTotalPercent,
                                    GetFormulaTypeDescription(MeasureFormulaType.ColumnTotalPercent));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentRowPercent,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentRowPercent));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentColumnPercent,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentColumnPercent));
            cbFormulaType.Items.Add(MeasureFormulaType.GrandTotalPercent,
                                    GetFormulaTypeDescription(MeasureFormulaType.GrandTotalPercent));
            cbFormulaType.Items.Add(MeasureFormulaType.GrandTotalRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.GrandTotalRank));
            cbFormulaType.Items.Add(MeasureFormulaType.GrandTotalInverseRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.GrandTotalInverseRank));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentRowRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentRowRank));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentRowInverseRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentRowInverseRank));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentColumnRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentColumnRank));
            cbFormulaType.Items.Add(MeasureFormulaType.ParentColumnInverseRank,
                                    GetFormulaTypeDescription(MeasureFormulaType.ParentColumnInverseRank));
            cbFormulaType.Items.Add(MeasureFormulaType.None,
                                    GetFormulaTypeDescription(MeasureFormulaType.None));

            InitCubeList();
            LoadMeasures();
            ceCubes.SelectedIndex = 0;

            this.cbFormulaType.Value = MeasureFormulaType.None;
            btFilters.Enabled = IsLookupCube(ceCubeList.Value);

            if (this.pivotTotal != null)
            {
                if (this.pivotTotal.FormulaType == MeasureFormulaType.Custom)
                {
                    rbCustomFormula.Checked = true;
                }
                else
                {
                    rbTypicalFormula.Checked = true;
                    cbFormulaType.Value = this.pivotTotal.FormulaType;
                    if (this.cbSourceMeasure.IsItemInList(this.pivotTotal.MeasureSource))
                    {
                        this.cbSourceMeasure.Value = this.pivotTotal.MeasureSource;
                    }
                }
            }
            else
            {
                rbCustomFormula.Checked = true;
            }
            this.updating = false;

            InitCalcTotalExpression();
        }

        private void InitCubeList()
        {
            if (!this.PivotData.CheckConnection())
            {
                return;
            }
            try
            {
                ceCubes.Items.Clear();
                ValueListItem item = ceCubes.Items.Add(tableCaption);
                item.Appearance.Image = 25;

                foreach (CubeDef cube in PivotData.AdomdConn.Cubes)
                {
                    item = ceCubes.Items.Add(cube.Name);
                    item.Appearance.Image = 0;

                    item = ceCubeList.Items.Add(cube);

                    if (IsLookupCube(cube))
                    {
                        item.DisplayText = cube.Caption;
                    }
                    else
                    {
                        item.DisplayText = string.Format("{0} (текущий)", cube.Caption);
                        item.Appearance.FontData.Bold = DefaultableBoolean.True;
                    }

                    if (this.isLookupMeasure)
                    {
                        if (cube.Name == this.LookupCubeName)
                        {
                            ceCubeList.Value = cube;
                        }
                    }
                }

                if (!this.isLookupMeasure)
                    ceCubeList.Value = this.PivotData.Cube;
            }
            catch(Exception exc)
            {
                this.PivotData.DoAdomdExceptionReceived(exc);
                this.Close();
            }
        }

        private void LoadMeasures()
        {
            if (!this.PivotData.CheckConnection())
            {
                return;
            }

            cbSourceMeasure.Items.Clear();
            if (ceCubeList.Value == null)
                return;
            try
            {

                foreach (Measure measure in ((CubeDef) ceCubeList.Value).Measures)
                {
                    cbSourceMeasure.Items.Add(measure.UniqueName, measure.Caption);
                    if (this.measureSource == measure.UniqueName)
                    {
                        cbSourceMeasure.Value = measure.UniqueName;
                    }
                }

                //если куб - текущий, то для него отображаем добавленные пользователем меры
                if (!this.IsLookupCube(ceCubeList.Value))
                {
                    foreach (PivotTotal total in this.PivotData.TotalAxis.Totals)
                    {
                        if (total.IsCustomTotal)
                        {
                            cbSourceMeasure.Items.Add(total.UniqueName, total.Caption);
                            if (this.measureSource == total.UniqueName)
                            {
                                cbSourceMeasure.Value = total.UniqueName;
                            }
                            
                        }
                    }
                }


            }
            catch (Exception exc)
            {
                this.PivotData.DoAdomdExceptionReceived(exc);
                this.Close();
            }

            if ((cbSourceMeasure.Items.Count > 0) && (cbSourceMeasure.SelectedIndex == -1))
            {
                cbSourceMeasure.SelectedIndex = 0;
            }

        }



        private string ClearApos(string sourceStr)
        {
            string resultStr = sourceStr;
            if (resultStr.IndexOf('\'') == 0)
            {
                resultStr = resultStr.Remove(0, 1);
            }

            if (resultStr.Length > 0)
            {
                if (resultStr.LastIndexOf('\'') == (resultStr.Length - 1))
                {
                    resultStr = resultStr.Remove(resultStr.Length - 1, 1);
                }
            }

            return resultStr;
        }

        #region инициализация списка полей

        public void LoadCube(CubeDef adomdCube)
        {
            UltraTreeNode cubeNode, dimNode = null;

            tFieldList.Nodes.Clear();

            if (!this.PivotData.CheckConnection())
            {
                return;
            }

            try
            {
                if (adomdCube == null)
                {
                    return;
                }

                DimensionCollection adomdDims = adomdCube.Dimensions;

                cubeNode = tFieldList.Nodes.Add(adomdCube.Name);
                cubeNode.Override.NodeAppearance.Image = 0;

                foreach (Dimension dim in adomdDims)
                {
                    if (dim.Hierarchies.Count > 1)
                    {
                        dimNode = cubeNode.Nodes.Add(dim.Caption);
                        dimNode.Override.NodeAppearance.Image = 1;

                        foreach (Hierarchy hierch in dim.Hierarchies)
                        {
                            AddHierNode(dimNode, hierch);
                        }
                    }
                    else
                    {
                        AddHierNode(cubeNode, dim.Hierarchies[0]);
                    }
                }
                cubeNode.Expanded = true;
            }
            catch (Exception exc)
            {
                //Common.CommonUtils.ProcessException(exc);
                this.PivotData.DoAdomdExceptionReceived(exc);
                this.Close();

            }
        }

        public void LoadTableMeasures()
        {
            UltraTreeNode tableNode, measuresNode = null;

            tFieldList.Nodes.Clear();

            try
            {
                tableNode = tFieldList.Nodes.Add(tableCaption);
                //cubeNode.Override.NodeAppearance.Image = 0;

                measuresNode = tableNode.Nodes.Add("[Measures]", "Меры");
                measuresNode.Override.NodeAppearance.Image = 3;

                foreach (PivotTotal total in this.PivotData.TotalAxis.Totals)
                {
                    UltraTreeNode node = measuresNode.Nodes.Add(total.UniqueName, total.Caption);
                    node.Override.NodeAppearance.Image = 23;
                }
                
                measuresNode.Expanded = true;
                tableNode.Expanded = true;
            }
            catch (Exception exc)
            {
                //Common.CommonUtils.ProcessException(exc);
                this.PivotData.DoAdomdExceptionReceived(exc);
            }
        }


        private UltraTreeNode AddHierNode(UltraTreeNode root, Hierarchy adomdHierch)
        {
            UltraTreeNode node = null;
            try
            {
                if (adomdHierch.Name == "Measures")
                {
                    node = root.Nodes.Add(adomdHierch.UniqueName, "Меры");

                    if (node != node.Parent.Nodes[0])
                    {
                        node.Reposition(node.GetSibling(NodePosition.First), NodePosition.Previous);
                    }
                    try
                    {
                        LoadMembers(node, adomdHierch.Levels[0].GetMembers());
                    }
                    catch (Exception exc)
                    {
                        this.PivotData.DoAdomdExceptionReceived(exc);
                    }
                    node.Override.NodeAppearance.Image = 3;
                    node.Expanded = true;
                }
                else
                {
                    node = root.Nodes.Add(adomdHierch.UniqueName, adomdHierch.Caption);
                    LoadHierarch(node, adomdHierch);
                    node.Override.NodeAppearance.Image = 2;
                }
            }
            catch (Exception exc)
            {
                this.PivotData.DoAdomdExceptionReceived(exc);
            }
            return node;
        }

        private void LoadHierarch(UltraTreeNode root, Hierarchy adomdHierch)
        {
            UltraTreeNode node;

            try
            {
            foreach (Level level in adomdHierch.Levels)
            {
                if (level.Name != "(All)")
                {
                    node = root.Nodes.Add(level.UniqueName, level.Caption);
                    node.Override.NodeAppearance.Image = level.LevelNumber + 5;
                }
            }
            }
            catch (Exception exc)
            {
                this.PivotData.DoAdomdExceptionReceived(exc);
                this.Close();

            }

        }

        private void LoadMembers(UltraTreeNode root, MemberCollection adomdMbrs)
        {
            if (!this.PivotData.CheckConnection())
            {
                return;
            }

            UltraTreeNode node;
            try
            {
                foreach (Member member in adomdMbrs)
                {
                    node = root.Nodes.Add(member.UniqueName, member.Caption);
                    node.Override.NodeAppearance.Image = member.Type == MemberTypeEnum.Formula ? 23 : 4;
                }


                if (ceCubes.Text == tableCaption)
                {
                    foreach (PivotTotal total in this.PivotData.TotalAxis.Totals)
                    {
                        node = root.Nodes.Add(total.UniqueName, total.Caption);
                        node.Override.NodeAppearance.Image = 23;
                    }
                }
                else //если куб - текущий, то для него отображаем добавленные пользователем меры
                    if (!this.IsLookupCube(PivotData.AdomdConn.Cubes[ceCubes.Text]))
                    {
                        foreach (PivotTotal total in this.PivotData.TotalAxis.Totals)
                        {
                            if (total.IsCustomTotal)
                            {
                                node = root.Nodes.Add(total.UniqueName, total.Caption);
                                node.Override.NodeAppearance.Image = 23;
                            }
                        }
                    }

            }
            catch (Exception exc)
            {
                this.PivotData.DoAdomdExceptionReceived(exc);
                this.Close();

            }

            root.Nodes.Override.Sort = Infragistics.Win.UltraWinTree.SortType.Ascending;
        }

        private string GetFormulaTypeDescription(MeasureFormulaType value)
        {
            FieldInfo fi = value.GetType().GetField(Enum.GetName(typeof(MeasureFormulaType), value));
            DescriptionAttribute dna =
              (DescriptionAttribute)Attribute.GetCustomAttribute(
                fi, typeof(DescriptionAttribute));

            if (dna != null)
                return dna.Description;
            else
                return value.ToString();

        }

        #endregion

        /// <summary>
        /// Проверяем является ли куб текущим, используемым в отчете
        /// </summary>
        /// <returns>false - если текущий</returns>
        private bool IsLookupCube(object cube)
        {
            if (cube != null)
            {
                return (cube is CubeDef) ? (this.PivotData.CubeName != ((CubeDef)cube).Name) : false;
            }
            return false;
        }

        /// <summary>
        /// Есть ли в структуре элемента уже такая мера
        /// </summary>
        /// <returns></returns>
        private bool IsTotalNameExists(string measureCaption)
        {
            //если редактируем существующую меру и заголовок у нее не изменился, то дальше не проверяем
            if (this.isEditTotal)
            {
                if (this.pivotTotal.Caption == measureCaption)
                    return false;
            }

            List<string> measureNames = this.PivotData.GetMeasureNames();
            string currentName = String.Format("[Measures].[{0}]", measureCaption).ToUpper();
            return measureNames.Contains(currentName);
        }

        /// <summary>
        /// Получение описания для формулы
        /// </summary>
        /// <param name="formulaType">тип формулы</param>
        /// <returns></returns>
        private string GetFormulaDescription(MeasureFormulaType formulaType)
        {
            switch(formulaType)
            {
                case MeasureFormulaType.ColumnTotalPercent:
                    return
                        "Рассчитывается процент каждого значения в столбце. Общий итог по столбцу принимается за 100%.";
                case MeasureFormulaType.GrandTotalInverseRank:
                    return "Ранжируются значения в таблице по возрастанию.";
                case MeasureFormulaType.GrandTotalPercent:
                    return
                        "Рассчитывается процент, который составляет значение каждого показателя в таблице. Общий итог по показателю в таблице принимается за 100%.";
                case MeasureFormulaType.GrandTotalRank:
                    return "Ранжируются значения в таблице по убыванию.";
                case MeasureFormulaType.Custom:
                case MeasureFormulaType.None:
                    return String.Empty;
                case MeasureFormulaType.ParentColumnInverseRank:
                    return
                        "Ранжируются в обратном порядке значения элементов, являющихся подчиненными для определенного столбца показателя.";
                case MeasureFormulaType.ParentColumnPercent:
                    return
                        "Рассчитывается процент, который составляет значение элемента по показателю для столбца, являющегося итоговым.";
                case MeasureFormulaType.ParentColumnRank:
                    return
                        "Ранжируются значения элементов, являющихся подчиненными для определенного столбца показателя.";
                case MeasureFormulaType.ParentRowInverseRank:
                    return
                        "Ранжируются в обратном порядке значения элементов являющихся подчиненными для определенной строки показателя.";
                case MeasureFormulaType.ParentRowPercent:
                    return
                        "Рассчитывается процент, который составляет значение элемента по показателю для строки, являющейся итоговой.";
                case MeasureFormulaType.ParentRowRank:
                    return "Ранжируются значения элементов являющихся подчиненными для определенной строки показателя.";
                case MeasureFormulaType.RowTotalPercent:
                    return "Рассчитывается процент каждого значения в строке. Общий итог по строке принимается за 100%.";
            }
            return String.Empty;
        }

        #region обработчики

        private void btOK_Click(object sender, EventArgs e)
        {
            this.btOK.DialogResult = DialogResult.None;
            this.AcceptButton = null;
            if (IsTotalNameExists(this.TotalCaption))
            {
                MessageBox.Show(
                    string.Format("Мера с именем \"{0}\" уже существует. Задайте другое имя.", this.TotalCaption),
                    "MDXExpert 3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.AcceptButton = btOK;
            this.btOK.DialogResult = DialogResult.OK;

            if (rbCustomFormula.Checked)
            {
                this.expression = ClearApos(teCalcTotalExpression.Text);
                this.measureSource = "";
                this.formulaType = MeasureFormulaType.Custom;
            }
            else if (rbTypicalFormula.Checked)
            {
                this.expression = "";
                this.measureSource = (string) cbSourceMeasure.Value;
                this.formulaType = (MeasureFormulaType) cbFormulaType.Value;
                this.isLookupMeasure = IsLookupCube(ceCubeList.Value);
                if (ceCubeList.Value != null)
                    this.lookupCubeName = ((CubeDef) ceCubeList.Value).Name;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void teTotalCaption_ValueChanged(object sender, EventArgs e)
        {
            btOK.Enabled = (teTotalCaption.Text != "");
        }

        private void btDropDownFields_DroppingDown(object sender, CancelEventArgs e)
        {
            tFieldList.Width = btDropDownFields.Width;
        }

        private void ceCubes_ValueChanged(object sender, EventArgs e)
        {
            if (ceCubes.Text == tableCaption)
            {
                LoadTableMeasures();
            }
            else
            {
                if (!this.PivotData.CheckConnection())
                {
                    return;
                }
                LoadCube(PivotData.AdomdConn.Cubes[ceCubes.Text]);
            }
            tFieldList.ExpandAll();
        }

        private void tFieldList_AfterSelect(object sender, SelectEventArgs e)
        {
            /* глючит обработчик, срабатывает почему-то по 2 раза, по 2 раза. Щас сделано через MouseDown   
            UltraTreeNode node = null;

            if(e.NewSelections.Count > 0)
            {
                node = e.NewSelections[0];
            }
            if (node == null)
            {
                btDropDownFields.Text = "";
                btDropDownFields.Tag = null;
                return;
            }

            btDropDownFields.Text = node.Text;
            btDropDownFields.Tag = node.Key;
            btDropDownFields.Appearance.Image = node.Override.NodeAppearance.Image;

            btDropDownFields.CloseUp();
            */
        }

        private void btAddLinkToField_Click(object sender, EventArgs e)
        {
            if (btDropDownFields.Tag != null)
            {
                teCalcTotalExpression.SelectedText = (string)btDropDownFields.Tag;
            }
        }

        private void tFieldList_MouseDown(object sender, MouseEventArgs e)
        {
            UltraTreeNode node = tFieldList.GetNodeFromPoint(e.Location);

            if (node == null)
            {
                btDropDownFields.Text = "";
                btDropDownFields.Tag = null;
                return;
            }

            btDropDownFields.Text = node.Text;
            btDropDownFields.Tag = node.Key;
            btDropDownFields.Appearance.Image = node.Override.NodeAppearance.Image;

            btDropDownFields.CloseUp();

        }

        private void rbLookupCube_CheckedChanged(object sender, EventArgs e)
        {
            gbTypicalFormula.Enabled = rbTypicalFormula.Checked;
            gbCustomFormula.Enabled = rbCustomFormula.Checked;

            if (teCalcTotalExpression.Text == "")
            {
                teCalcTotalExpression.Text = "0";
            }
                
            /*
            this.measureSource = "";
            this.lookupCubeName = "";
            this.filters = null;
            */

            lCube.Text = "Куб:";
            lMeasure.Text = "Мера:";

        }

        private void btFilters_Click(object sender, EventArgs e)
        {
            //LookupCubeForm lcForm = new LookupCubeForm(this.PivotData, this.pivotTotal);
            if (ceCubeList.Value == null)
                return;

            LookupCubeForm lcForm = new LookupCubeForm(this.PivotData, (CubeDef)ceCubeList.Value, this.filters);
            if (lcForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.filters.InnerXml = lcForm.Filters.InnerXml;
                InitCalcTotalExpression();
            }
        }

        private void ceCubeList_ValueChanged(object sender, EventArgs e)
        {
            btFilters.Enabled = IsLookupCube(ceCubeList.Value);
            LoadMeasures();

            InitCalcTotalExpression();
        }

        private void btPlus_Click(object sender, EventArgs e)
        {
            teCalcTotalExpression.SelectedText = "+";
        }

        private void btMinus_Click(object sender, EventArgs e)
        {
            teCalcTotalExpression.SelectedText = "-";
        }

        private void btMultiplication_Click(object sender, EventArgs e)
        {
            teCalcTotalExpression.SelectedText = "*";
        }

        private void btDivision_Click(object sender, EventArgs e)
        {
            teCalcTotalExpression.SelectedText = "/";
        }

        private void cbSourceMeasure_ValueChanged(object sender, EventArgs e)
        {
            InitCalcTotalExpression();
        }

        private void cbFormulaType_ValueChanged(object sender, EventArgs e)
        {
            InitCalcTotalExpression();
        }

        #endregion

    }
}