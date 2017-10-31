using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Components;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.BaseCls;
using Krista.FM.Client.ViewObjects.AssociatedCLSUI.DataCls;
using Krista.FM.Client.ViewObjects.BaseViewObject;
using Krista.FM.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.FinSourcePlanning;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI
{
    internal class BKKUIndicatorsUI : BaseViewObj
    {
        /*
        private static BKKUIndicatorsUI instance;

        public static BKKUIndicatorsUI Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BKKUIndicatorsUI();
                }
                return instance;
            }
        }*/

        private IIndicatorsService BKKUIndicatorsService;

        internal BKKUIndicatorsUI()
            : base(typeof(BKKUIndicatorsUI).FullName)
        {
            Caption = EnumHelper.GetEnumItemDescription(typeof(FinSourcePlanningServiceTypes), FinSourcePlanningServiceTypes.BKKUIndicators);
            fViewCtrl = new BKKUIndicatorsView();
            SetupUgeIndicators();
            BKKUIndicatorsService = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.FinSourcePlanningFace.BKKUIndicators;
		}

        public override Control Control
        {
            get { return fViewCtrl; }
        }

		public override string FullCaption
		{
			get
			{
				return EnumHelper.GetEnumItemDescription(typeof(FinSourcePlanningServiceTypes), FinSourcePlanningServiceTypes.BKKUIndicators);
			}
		}

        protected override void SetViewCtrl()
        {

            //throw new NotSupportedException("Для класса BKKUIndicatorsUI вызов метода SetViewCtrl запрещен.");
        }

        private void SetupUgeIndicators()
        {
            BKKUIndicatorsView vo = ((BKKUIndicatorsView) fViewCtrl);
            vo.ugeIndicators.IsReadOnly = true;
            vo.ugeIndicators.AllowClearTable = true;
            vo.ugeIndicators.AllowDeleteRows = false;
            vo.ugeIndicators.AllowImportFromXML = false;
            vo.ugeIndicators.ExportImportToolbarVisible = false;
            vo.ugeIndicators.SaveMenuVisible = true;
            vo.ugeIndicators.LoadMenuVisible = false;
            vo.ugeIndicators.AllowAddNewRecords = false;
            vo.ugeIndicators._utmMain.Tools["Refresh"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["ShowGroupBy"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["SaveChange"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["CancelChange"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["ShowHierarchy"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["ColumnsVisible"].SharedProps.Visible = false;
            vo.ugeIndicators._utmMain.Tools["ClearCurrentTable"].SharedProps.Visible = true;
            vo.ugeIndicators._utmMain.Tools["DeleteSelectedRows"].SharedProps.Visible = false;

            vo.ugeIndicators.ugData.DisplayLayout.GroupByBox.Hidden = true;

            UltraToolbar tb = vo.ugeIndicators.utmMain.Toolbars["utbColumns"];
            ImageList il = new ImageList();
            il.TransparentColor = Color.Magenta;
            il.Images.Add(Resources.ru.calculator);
            if (!vo.ugeIndicators.utmMain.Tools.Exists("btnCalculate"))
            {
                ButtonTool btnAddDuplicate = new ButtonTool("btnCalculate");
                btnAddDuplicate.SharedProps.ToolTipText = "Рассчитать идикаторы";
                btnAddDuplicate.SharedProps.AppearancesSmall.Appearance.Image =
                   il.Images[0];//.CalculateIndicators;
                vo.ugeIndicators.utmMain.Tools.Add(btnAddDuplicate);
                tb.Tools.AddTool("btnCalculate");

                bool calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("FinSourcePlaning",
                    (int)FinSourcePlaningOperations.Calculate, false);
                if (!calcVisible)
                    calcVisible = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.UsersManager.CheckPermissionForSystemObject("BBK",
                    (int)FinSourcePlaningCalculateUIModuleOperations.Calculate, false);
                btnAddDuplicate.SharedProps.Visible = calcVisible;
            }

            vo.ugeIndicators.ToolClick += new ToolBarToolsClick(ugeIndicators_ToolClick);
            vo.ugeIndicators.OnGridInitializeLayout += new GridInitializeLayout(ugeIndicators_OnGridInitializeLayout);
            vo.ugeIndicators.OnInitializeRow += new InitializeRow(ugeIndicators_OnInitializeRow);
            vo.ugeIndicators.ugData.BeforeRowActivate += new RowEventHandler(ugData_BeforeRowActivate);


            vo.uteVariantIf.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(uteVariantIf_EditorButtonClick);
            vo.uteVariantIncome.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(uteVariantIncome_EditorButtonClick);
            vo.uteVariantOutcome.EditorButtonClick += new Infragistics.Win.UltraWinEditors.EditorButtonEventHandler(uteVariantOutcome_EditorButtonClick);
        }

        void uteVariantOutcome_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            ((BKKUIndicatorsView)fViewCtrl).uteVariantOutcome.Text = ChooseVariant(SchemeObjectsKeys.variantCharge_Key);
        }

        void uteVariantIncome_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            ((BKKUIndicatorsView)fViewCtrl).uteVariantIncome.Text = ChooseVariant(SchemeObjectsKeys.variantIncomes_Key, true);
        }

        void uteVariantIf_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            ((BKKUIndicatorsView)fViewCtrl).uteVariantIf.Text = ChooseVariant(SchemeObjectsKeys.d_Variant_Borrow_Key);
        }
        
        private void AddInformationString(string informationString)
        {
            ((BKKUIndicatorsView)fViewCtrl).tbIndicatorInfo.Text += string.Format("{0}{1}",
               informationString, Environment.NewLine);
        }

        void ugData_BeforeRowActivate(object sender, RowEventArgs e)
        {
            ((BKKUIndicatorsView) fViewCtrl).tbIndicatorInfo.Text = string.Empty;
            if ((AssessionType)e.Row.Cells["AssessionType"].Value != AssessionType.NonAssessed)
            {
                // Выводим результаты в текстбокс
                AddInformationString(string.Format("Формула: {0}", e.Row.Cells["Formula"].Value));
                for (int year = 0; year < BKKUIndicatorsService.YearsCount; year++)
                {
                    AddInformationString(e.Row.Cells[string.Format("FormulaValue{0}", year)].Value.ToString());
                }
                AddInformationString(string.Format("Критерий оценки: {0}", e.Row.Cells["AssessCrit"].Value));
                AddInformationString(string.Format("Комментарий: {0}", e.Row.Cells["Comment"].Value));
            }
            else
            {
                List<string> nonAssessReason = (List<string>) e.Row.Cells["NonAssessReason"].Value;
                AddInformationString("Ошибки при вычислении индикатора:");
                foreach (string reason in nonAssessReason)
                {
                    AddInformationString(reason);   
                }
            }
        }

        void ugeIndicators_OnInitializeRow(object sender, InitializeRowEventArgs e)
        {
            switch((AssessionType)e.Row.Cells["AssessionType"].Value)
            {
                case AssessionType.Logical:
                    {
                        for (int year = 0; year < BKKUIndicatorsService.YearsCount; year++ )
                        {
                            double assess = Convert.ToDouble(e.Row.Cells[string.Format("Assess{0}", year)].Value);
                            e.Row.Cells[string.Format("Value{0}", year)].Appearance.BackColor = GetLogicalAssessColor(assess);
                        }
                        break;
                    }
                case AssessionType.Interval:
                    {
                        for (int year = 0; year < BKKUIndicatorsService.YearsCount; year++)
                        {
                            double assess = Convert.ToDouble(e.Row.Cells[string.Format("Assess{0}", year)].Value);
                            e.Row.Cells[string.Format("Value{0}", year)].Appearance.BackColor = GetIntervalAssessColor(assess);
                        }
                        break;
                    }
                case AssessionType.NonAssessed:
                    {
                        for (int year = 0; year < BKKUIndicatorsService.YearsCount; year++)
                        {
                            e.Row.Cells[string.Format("Value{0}", year)].Appearance.BackColor = Color.Red;
                        }
                        break;
                    }
            }
        }

        private Color GetLogicalAssessColor(double assess)
        {
            if (assess == 0)
            {
                return Color.FromArgb(0, 172, 77);
            }
            else
            {
                return Color.Red;
            }
        }

        private Color GetIntervalAssessColor(double assess)
        {
            if (assess == 0)
                return Color.FromArgb(0, 172, 77);
            if (assess == 1)
                return  Color.Red;
            return Color.Yellow;
        }

        void ugeIndicators_OnGridInitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = e.Layout.Bands[0];

            int visiblePosition = 0;
 
            UltraGridColumn clmn = band.Columns["IndicatorName"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "Индикатор";
            clmn.Width = 100;

            clmn = band.Columns["Caption"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = "Наименование индикатора";
            clmn.Width = 200;

            clmn = band.Columns["Formula"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;

            clmn = band.Columns["Comment"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;

            clmn = band.Columns["AssessCrit"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;

            clmn = band.Columns["AssessionType"];
            clmn.Header.VisiblePosition = visiblePosition++;
            clmn.Header.Caption = string.Empty;
            clmn.Hidden = true;

            int baseYear = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.FinSourcePlanningFace.BaseYear;

            for (int year = 0; year < BKKUIndicatorsService.YearsCount; year++)
            {
                clmn = band.Columns[string.Format("Value{0}", year)];
                clmn.Header.VisiblePosition = visiblePosition++;
                clmn.Header.Caption = string.Format("{0}", baseYear++);
                clmn.Width = 100;

                clmn = band.Columns[string.Format("Assess{0}", year)];
                clmn.Header.VisiblePosition = visiblePosition++;
                clmn.Header.Caption = string.Empty;
                clmn.Hidden = true;

                clmn = band.Columns[string.Format("FormulaValue{0}", year)];
                clmn.Header.VisiblePosition = visiblePosition++;
                clmn.Header.Caption = string.Empty;
                clmn.Hidden = true;
            }

            band = e.Layout.Bands[1];
            band.Hidden = true;
        }

        void ugeIndicators_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch(e.Tool.Key)
            {
                case "btnCalculate":
                    {
                        BKKUIndicatorsView vo = (BKKUIndicatorsView) fViewCtrl;
                        FinSourcePlanningNavigation.Instance.Workplace.OperationObj.Text = "Расчет индикаторов...";
                        FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StartOperation();
                        try
                        {
                            vo.ugeIndicators.DataSource = BKKUIndicatorsService.CalculateAndAssessIndicators(
                                vo.uteVariantIf.Text, vo.uteVariantIncome.Text, FinSourcePlanningNavigation.BaseYear.ToString(), vo.uteVariantOutcome.Text, FinSourcePlanningNavigation.Instance.CurrentVariantID);
                        }
                        finally
                        {
                            FinSourcePlanningNavigation.Instance.Workplace.OperationObj.StopOperation();
                        }
                        break;
                    }
            }
        }

        private string ChooseVariant(string clsKey)
        {
            return ChooseVariant(clsKey, false);
        }

        private string ChooseVariant(string clsKey, bool IsVariantIncome)
        {
            frmModalTemplate tmpClsForm = new frmModalTemplate();
             
            IClassifier cls = FinSourcePlanningNavigation.Instance.Workplace.ActiveScheme.Classifiers[clsKey];
            string variant = cls.OlapName;
            BaseClsUI clsUI = new DataClsUI(cls);
            clsUI.Workplace = FinSourcePlanningNavigation.Instance.Workplace;
            clsUI.Initialize();
            clsUI.RefreshAttachedData();
            tmpClsForm.SuspendLayout();
            try
            {
                tmpClsForm.AttachCls(clsUI);
                ComponentCustomizer.CustomizeInfragisticsControls(tmpClsForm);
            }
            finally
            {
                tmpClsForm.ResumeLayout();
            }
            if (tmpClsForm.ShowDialog(clsUI.Workplace.WindowHandle) == DialogResult.OK)
            {
                variant += string.Format(".{0}", clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["NAME"].Value);
                if (IsVariantIncome)
                {
                    //variantIncomeYear = clsUI.UltraGridExComponent.ugData.ActiveRow.Cells["RefYear"].Value.ToString();
                }
                return variant;
            }
            return string.Empty;
        }
    }
}
