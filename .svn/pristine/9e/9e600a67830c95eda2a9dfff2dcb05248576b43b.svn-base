using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinMaskedEdit;
using Krista.FM.Client.Common.Wizards;
using Krista.FM.ServerLibrary;
using Infragistics.Win.UltraWinEditors;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI.DataSourceWizard
{
    public partial class DataSourcesParametersPage : WizardPageBase
    {
        private IDataKind selectedDataKind;

        public DataSourcesParametersPage()
        {
            InitializeComponent();

            labels = new List<UltraLabel>();
            wizardComponentsList = new List<WizardValueComponents>();
        }

        public IDataKind SelectedDataKind
        {
            set { selectedDataKind = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) == WizardForm.TWizardsButtons.Next)
                WizardPageParent.WizardButtons -= WizardForm.TWizardsButtons.Next;

            ValidatePage();
        }

        internal enum SourceParams { Year, BudgetName, Variant, Quarter, Month, Territory };

        private struct WizardValueComponents
        {
            private Control _control;
            public Control control
            {
                get { return _control; }
                set { _control = value; }
            }

            private SourceParams _sourceParams;
            public SourceParams sourceParam
            {
                get { return _sourceParams; }
                set { _sourceParams = value; }
            }
        }

        private List<UltraLabel> labels;
        private List<WizardValueComponents> wizardComponentsList;
        
        internal void CreateControls()
        {
            ClearComponents();

            // создаем компоненты для сбора информации, пмещаем в список для дальнейшего использования
            WizardValueComponents component = new WizardValueComponents();

            UltraNumericEditor nud;
            UltraComboEditor cb;
            UltraTextEditor tb;
            UltraLabel lb;
            UltraMaskedEdit me;

            switch (selectedDataKind.ParamKind)
            {
                case ParamKindTypes.Budget:
                    lb = CreateLabel(this, 50, 10, "Финансовый орган");
                    labels.Add(lb);
                    tb = CreateTextBox(this, 50, 30);
                    component.control = tb;
                    component.sourceParam = SourceParams.BudgetName;
                    wizardComponentsList.Add(component);
                    //wizardComponents.Add(sourceParams.budgetName, tb);

                    lb = CreateLabel(this, 50, 70, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 90, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.Year:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearMonth:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Месяц");
                    labels.Add(lb);
                    cb = CreateCalendarComboBox(this, 50, 90);
                    //wizardComponents.Add(sourceParams.month, cb);
                    component.control = cb;
                    component.sourceParam = SourceParams.Month;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearMonthVariant:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Месяц");
                    labels.Add(lb);
                    cb = CreateCalendarComboBox(this, 50, 90);
                    //wizardComponents.Add(sourceParams.month, cb);
                    component.control = cb;
                    component.sourceParam = SourceParams.Month;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 130, "Вариант");
                    labels.Add(lb);
                    tb = CreateTextBox(this, 50, 150);
                    //wizardComponents.Add(sourceParams.variant, tb);
                    component.control = tb;
                    component.sourceParam = SourceParams.Variant;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearQuarter:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Квартал");
                    labels.Add(lb);
                    me = CreateMaskedEditor(this, 50, 90, 4, 1, "9");
                    //nud = CreateNumericUpDown(this, 50, 90, -1, 4, 1);
                    //wizardComponents.Add(sourceParams.quarter, nud);
                    component.control = me;
                    component.sourceParam = SourceParams.Quarter;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearTerritory:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Территория");
                    labels.Add(lb);
                    tb = CreateTextBox(this, 50, 90);
                    //wizardComponents.Add(sourceParams.territory, tb);
                    component.control = tb;
                    component.sourceParam = SourceParams.Territory;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearVariant:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    //wizardComponents.Add(sourceParams.year, nud);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Вариант");
                    labels.Add(lb);
                    tb = CreateTextBox(this, 50, 90);
                    //wizardComponents.Add(sourceParams.variant, tb);
                    component.control = tb;
                    component.sourceParam = SourceParams.Variant;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.YearQuarterMonth:
                    lb = CreateLabel(this, 50, 10, "Год");
                    labels.Add(lb);
                    nud = CreateNumericUpDown(this, 50, 30, 1900, 2100, DateTime.Now.Year);
                    component.control = nud;
                    component.sourceParam = SourceParams.Year;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 70, "Квартал");
                    labels.Add(lb);
                    me = CreateMaskedEditor(this, 50, 90, 4, 1, "9");
                    //wizardComponents.Add(sourceParams.quarter, nud);
                    component.control = me;
                    component.sourceParam = SourceParams.Quarter;
                    wizardComponentsList.Add(component);

                    lb = CreateLabel(this, 50, 130, "Месяц");
                    labels.Add(lb);
                    cb = CreateCalendarComboBox(this, 50, 150);
                    //wizardComponents.Add(sourceParams.month, cb);
                    component.control = cb;
                    component.sourceParam = SourceParams.Month;
                    wizardComponentsList.Add(component);

                    break;
                case ParamKindTypes.WithoutParams:
                    lb = CreateLabel(this, 50, 10, "Источник не имеет параметров");
                    labels.Add(lb);
                    break;
                case ParamKindTypes.Variant:
                    lb = CreateLabel(this, 50, 10, "Вариант");
                    labels.Add(lb);
                    tb = CreateTextBox(this, 50, 30);
                    //wizardComponents.Add(sourceParams.variant, tb);
                    component.control = tb;
                    component.sourceParam = SourceParams.Variant;
                    wizardComponentsList.Add(component);
                    break;
            }
            SetComponentsEvents();
            ValidatePage();
        }

        private void SetComponentsEvents()
        {
            foreach (WizardValueComponents ctrl in wizardComponentsList)
            {
                ctrl.control.TextChanged += new EventHandler(ctrl_TextChanged);
            }
        }

        internal string GetSourceParamValue(SourceParams par)
        {
            Control ctrl;
            string returnValue = string.Empty;
            foreach (WizardValueComponents component in wizardComponentsList)
            {
                if (component.sourceParam == par)
                {
                    ctrl = component.control;
					if (ctrl != null)
					{
						if (par == SourceParams.Month)
						{
							int month = ((UltraComboEditor)ctrl).SelectedIndex + 1;
                            if (month > 12)
                                return string.Empty;
							return month.ToString();
						}
						else if (par == SourceParams.Quarter)
						{
                            returnValue = Convert.ToString(((UltraMaskedEdit)ctrl).Value);
						}
						else
						{
							returnValue = ctrl.Text;
						}
					}
					else
						returnValue = string.Empty;

                }
            }
            return returnValue;
        }

        private void ctrl_TextChanged(object sender, EventArgs e)
        {
            ValidatePage();
        }

        /// <summary>
        /// Проверка на корректность заполнения параметров.
        /// </summary>
        private void ValidatePage()
        {
            // для источника без параметров
            if (wizardComponentsList.Count == 0)
            {
                return;
            }
            bool allowAddSource = wizardComponentsList[0].control.Text != string.Empty;
            bool prevComponentFilled = allowAddSource;
            if (allowAddSource)
                if (wizardComponentsList.Count > 1)
                    for (int i = 1; i <= wizardComponentsList.Count - 1; i++)
                    {
                        if (prevComponentFilled && (wizardComponentsList[i].control.Text != string.Empty))
                            allowAddSource = true;
                        else
                            if (prevComponentFilled && (wizardComponentsList[i].control.Text == string.Empty))
                                allowAddSource = true;
                            else
                                if (!prevComponentFilled && (wizardComponentsList[i].control.Text == string.Empty))
                                    allowAddSource = true;
                                else
                                    if (!prevComponentFilled && (wizardComponentsList[i].control.Text != string.Empty))
                                    {
                                        allowAddSource = false;
                                        break;
                                    }
                        prevComponentFilled = wizardComponentsList[i].control.Text != string.Empty;
                    }

            if (allowAddSource)
            {
                if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) !=
                    WizardForm.TWizardsButtons.Finish)
                    WizardPageParent.WizardButtons |= WizardForm.TWizardsButtons.Next;
            }
            else
            {
                if ((WizardPageParent.WizardButtons & WizardForm.TWizardsButtons.Next) ==
                    WizardForm.TWizardsButtons.Next)
                    WizardPageParent.WizardButtons -= WizardForm.TWizardsButtons.Next;
            }
        }

        /// <summary>
        /// Очищает от компонентов форму
        /// </summary>
        internal void ClearComponents()
        {
            foreach (WizardValueComponents ctrl in wizardComponentsList)
            {
                ctrl.control.Parent = null;
                ctrl.control.Dispose(); ;
            }
            wizardComponentsList.Clear();

            for (int i = 0; i <= labels.Count - 1; i++)
            {
                labels[i].Parent = null;
                labels[i].Dispose();
            }
        }

        /// <summary>
        /// Создание надпись над компонентом
        /// </summary>
        /// <param name="parentControl">Куда помещаем надпись</param>
        /// <param name="xPos">позиция X</param>
        /// <param name="yPos">позиция Y</param>
        /// <param name="Text">Текст надписи</param>
        /// <returns></returns>
        private static UltraLabel CreateLabel(Control parentControl, int xPos, int yPos, string Text)
        {
            UltraLabel lb = new UltraLabel();
            lb.Parent = parentControl;
            lb.AutoSize = true;

            lb.Text = Text;

            Point pt = new Point(xPos, yPos);
            lb.Location = pt;

            Size sz = new Size(100, 20);
            lb.Size = sz;

            return lb;
        }

        /// <summary>
        /// Создание компонента для ввода текста
        /// </summary>
        /// <param name="parentControl">Куда помещаем компонент</param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private static UltraTextEditor CreateTextBox(Control parentControl, int xPos, int yPos)
        {
            UltraTextEditor tb = new UltraTextEditor();
            tb.Parent = parentControl;
            Point pt = new Point(xPos, yPos);
            tb.Location = pt;
            Size sz = new Size(300, 20);
            tb.Size = sz;
            return tb;
        }

        /// <summary>
        /// создание компонента для выбора года или квартала
        /// </summary>
        /// <param name="parentControl">Куда помещаем компонент</param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <param name="minVal">Минимальное значение</param>
        /// <param name="maxVal">Максимальное значение</param>
        /// <param name="Val">Текущее значение</param>
        /// <returns></returns>
        private static UltraNumericEditor CreateNumericUpDown(Control parentControl, int xPos, int yPos, int minVal, int maxVal, int Val)
        {
            UltraNumericEditor nud = new UltraNumericEditor();
            nud.Parent = parentControl;
            Point pt = new Point(xPos, yPos);
            nud.Location = pt;
            nud.NumericType = NumericType.Integer;
            nud.SpinIncrement = 1;
            nud.MinValue = minVal;
            nud.MaxValue = maxVal;
            nud.MaskInput = "9999";
            nud.Value = Val;
            return nud;
        }

        private static UltraMaskedEdit CreateMaskedEditor(Control parentControl, int xPos, int yPos, int maxVal, int Val, string mask)
        {
            UltraMaskedEdit masketEdit = new UltraMaskedEdit();
            masketEdit.Parent = parentControl;
            Point pt = new Point(xPos, yPos);
            masketEdit.Location = pt;
            masketEdit.MaxValue = maxVal;
            masketEdit.Value = Val;
            masketEdit.Appearance.TextHAlign = Infragistics.Win.HAlign.Right;
            masketEdit.InputMask = mask;
            return masketEdit;
        }

        /// <summary>
        /// создание компонента для выбора месяца
        /// </summary>
        /// <param name="parentControl">Куда помещаем компонент</param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private static UltraComboEditor CreateCalendarComboBox(Control parentControl, int xPos, int yPos)
        {
            UltraComboEditor cb = new UltraComboEditor();
			cb.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            cb.Parent = parentControl;
            Point pt = new Point(xPos, yPos);
            cb.Location = pt;
            cb.Items.Add("Январь");
            cb.Items.Add("Февраль");
            cb.Items.Add("Март");
            cb.Items.Add("Апрель");
            cb.Items.Add("Май");
            cb.Items.Add("Июнь");
            cb.Items.Add("Июль");
            cb.Items.Add("Август");
            cb.Items.Add("Сентябрь");
            cb.Items.Add("Октябрь");
            cb.Items.Add("Ноябрь");
            cb.Items.Add("Декабрь");
            cb.Items.Add("");
            cb.SelectedIndex = 0;
            return cb;
        }
    }
}

