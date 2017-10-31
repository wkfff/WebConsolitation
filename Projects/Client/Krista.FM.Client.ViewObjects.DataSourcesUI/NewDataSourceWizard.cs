using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.ViewObjects.DataSourcesUI
{
    public partial class NewDataSourceWizard : Form
    {

        private IDataSourceManager dsManager;

        enum sourceParams { year, budgetName, variant, quarter, month, territory };
        // основные параметры источника, указываются для каждого добавляемого источника
        string sDataCode = string.Empty;
        string sDataName = string.Empty;
        string sSuplierCode = string.Empty;
        ParamKindTypes parametrsType;

        Dictionary<string, ParamKindTypes> nodeTypes;

        /// <summary>
        /// конструктор мастера
        /// </summary>
        /// <param name="DataSourceManager"></param>
        public NewDataSourceWizard(IDataSourceManager DataSourceManager)
        {
            InitializeComponent();

            dsManager = DataSourceManager;

            // Построение дерева
            // Идем по коллекции поставщиков данных
            int nodeKey = 0;

            nodeTypes = new Dictionary<string, ParamKindTypes>();

            tvSources.BeginUpdate();
            foreach (IDataSupplier item in DataSourceManager.DataSuppliers.Values)
            {
                string nodeText = item.Name + " - " + item.Description;
                TreeNode node = new TreeNode(nodeText);
                tvSources.Nodes.Add(node);
                // Идем по коллекции вида поступающей информации
                foreach (IDataKind item1 in item.DataKinds.Values)
                {
                    string rusTakeMethod = string.Empty;
                    switch (item1.TakeMethod)
                    {
                        case TakeMethodTypes.Import:
                            rusTakeMethod = "Импорт";
                            break;
                        case TakeMethodTypes.Input:
                            rusTakeMethod = "Ввод";
                            break;
                        case TakeMethodTypes.Receipt:
                            rusTakeMethod = "Сбор";
                            break;
                    }

                    string rusParamKind = string.Empty;

                    switch (item1.ParamKind)
                    {
                        case ParamKindTypes.Budget:
                            rusParamKind = "Финансовый орган, год";
                            break;
                        case ParamKindTypes.Year:
                            rusParamKind = "Год";
                            break;
                        case ParamKindTypes.YearMonth:
                            rusParamKind = "Год, месяц";
                            break;
                        case ParamKindTypes.YearMonthVariant:
                            rusParamKind = "Год, месяц, вариант";
                            break;
                        case ParamKindTypes.YearQuarter:
                            rusParamKind = "Год, квартал";
                            break;
                        case ParamKindTypes.YearQuarterMonth:
                            rusParamKind = "Год, квартал, месяц";
                            break;
                        case ParamKindTypes.YearVariant:
                            rusParamKind = "Год, вариант";
                            break;
                        case ParamKindTypes.YearTerritory:
                            rusParamKind = "Год, территория";
                            break;
                        case ParamKindTypes.WithoutParams:
                            rusParamKind = "Без параметров";
                            break;
                        case ParamKindTypes.Variant:
                            rusParamKind = "Вариант";
                            break;
                    }
                    string childNodeText = item1.Code + " - " + item1.Name + " - " + rusTakeMethod + " - " + rusParamKind;
                    TreeNode childNode = node.Nodes.Add(nodeKey.ToString(), childNodeText);
                    nodeTypes.Add(childNode.Name, item1.ParamKind);
                    nodeKey++;
                    childNode.ImageIndex = -1;
                }
            }
            tvSources.Sort();
            tvSources.EndUpdate();
            wizardComponentsList = new List<wizardValueComponents>();
            labels = new List<Label>();
        }

        //Dictionary<sourceParams, Control> wizardComponents;
        List<Label> labels;
        List<wizardValueComponents> wizardComponentsList; 

        private struct wizardValueComponents
        {
            private Control _control;
            public Control control
            {
                get { return _control; }
                set { _control = value; }
            }

            private sourceParams _sourceParams;
            public sourceParams sourceParam
            {
                get { return _sourceParams; }
                set { _sourceParams = value; }
            }
        }

        string paramsName;

        private void tvSources_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null)
            {
                ClearComponents();
                ParamKindTypes par;
                nodeTypes.TryGetValue(e.Node.Name, out par);
                // получаем параметры выбранного источника
                string[] strParams = e.Node.Text.Split('-');

                string TakeMetoth = strParams[2].Trim();
                if (TakeMetoth == "Импорт")
                {
                    btnNextPage.Enabled = false;
                    return;
                }
                wizardValueComponents component = new wizardValueComponents();
                paramsName = strParams[3].Trim();
                sDataCode = strParams[0].Trim();
                sDataName = strParams[1].Trim();
                sSuplierCode = ((string[])e.Node.Parent.Text.Split('-'))[0].Trim();

                // создаем компоненты для сбора информации, пмещаем в список для дальнейшего использования
                NumericUpDown nud;
                ComboBox cb;
                TextBox tb;
                Label lb;

                switch (par)
                {
                    case ParamKindTypes.Budget:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Финансовый орган");
                        labels.Add(lb);
                        tb = CreateTextBox(ultraTabPageControl2, 50, 30);
                        component.control = tb;
                        component.sourceParam = sourceParams.budgetName;
                        wizardComponentsList.Add(component);
                        //wizardComponents.Add(sourceParams.budgetName, tb);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 90, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.Budget;
                        break;
                    case ParamKindTypes.Year:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.Year;
                        break;
                    case ParamKindTypes.YearMonth:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Месяц");
                        labels.Add(lb);
                        cb = CreateCalendarComboBox(ultraTabPageControl2, 50, 90);
                        //wizardComponents.Add(sourceParams.month, cb);
                        component.control = cb;
                        component.sourceParam = sourceParams.month;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearMonth;
                        break;
                    case ParamKindTypes.YearMonthVariant:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Месяц");
                        labels.Add(lb);
                        cb = CreateCalendarComboBox(ultraTabPageControl2, 50, 90);
                        //wizardComponents.Add(sourceParams.month, cb);
                        component.control = cb;
                        component.sourceParam = sourceParams.month;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 130, "Вариант");
                        labels.Add(lb);
                        tb = CreateTextBox(ultraTabPageControl2, 50, 150);
                        //wizardComponents.Add(sourceParams.variant, tb);
                        component.control = tb;
                        component.sourceParam = sourceParams.variant;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearMonthVariant;
                        break;
                    case ParamKindTypes.YearQuarter:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Квартал");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 90, 1, 4, 1);
                        //wizardComponents.Add(sourceParams.quarter, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.quarter;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearQuarter;
                        break;
                    case ParamKindTypes.YearTerritory:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Территория");
                        labels.Add(lb);
                        tb = CreateTextBox(ultraTabPageControl2, 50, 90);
                        //wizardComponents.Add(sourceParams.territory, tb);
                        component.control = tb;
                        component.sourceParam = sourceParams.territory;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearTerritory;
                        break;
                    case ParamKindTypes.YearVariant:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        //wizardComponents.Add(sourceParams.year, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Вариант");
                        labels.Add(lb);
                        tb = CreateTextBox(ultraTabPageControl2, 50, 90);
                        //wizardComponents.Add(sourceParams.variant, tb);
                        component.control = tb;
                        component.sourceParam = sourceParams.variant;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearVariant;
                        break;
                    case ParamKindTypes.YearQuarterMonth:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Год");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 30, 1900, 2100, DateTime.Now.Year);
                        component.control = nud;
                        component.sourceParam = sourceParams.year;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 70, "Квартал");
                        labels.Add(lb);
                        nud = CreateNumericUpDown(ultraTabPageControl2, 50, 90, 1, 4, 1);
                        //wizardComponents.Add(sourceParams.quarter, nud);
                        component.control = nud;
                        component.sourceParam = sourceParams.quarter;
                        wizardComponentsList.Add(component);

                        lb = CreateLabel(ultraTabPageControl2, 50, 130, "Месяц");
                        labels.Add(lb);
                        cb = CreateCalendarComboBox(ultraTabPageControl2, 50, 150);
                        //wizardComponents.Add(sourceParams.month, cb);
                        component.control = cb;
                        component.sourceParam = sourceParams.month;
                        wizardComponentsList.Add(component);

                        parametrsType = ParamKindTypes.YearQuarterMonth;
                        break;
                    case ParamKindTypes.WithoutParams:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Источник не имеет параметров");
                        labels.Add(lb);
                        parametrsType = ParamKindTypes.WithoutParams;
                        break;
                    case ParamKindTypes.Variant:
                        lb = CreateLabel(ultraTabPageControl2, 50, 10, "Вариант");
                        labels.Add(lb);
                        tb = CreateTextBox(ultraTabPageControl2, 50, 30);
                        //wizardComponents.Add(sourceParams.variant, tb);
                        component.control = tb;
                        component.sourceParam = sourceParams.variant;
                        wizardComponentsList.Add(component);
                        parametrsType = ParamKindTypes.Variant;
                        break;
                }
                btnNextPage.Enabled = true;
                SetComponentsEvents();
                return;
            }
            btnNextPage.Enabled = false;
        }

        private void SetComponentsEvents()
        {
            foreach (wizardValueComponents ctrl in wizardComponentsList)
            {
                ctrl.control.TextChanged += new EventHandler(ctrl_TextChanged);
                //ctrl.TextChanged += new EventHandler(ctrl_TextChanged);
            }
        }

        void ctrl_TextChanged(object sender, EventArgs e)
        {
            // для источника без параметров
            if (wizardComponentsList.Count == 0)
            {
                btnApply.Enabled = true;
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

            btnApply.Enabled = allowAddSource;
            //if (allowAddSource)
                
        }

        /// <summary>
        /// Очищает от компонентов форму
        /// </summary>
        private void ClearComponents()
        {
            foreach (wizardValueComponents ctrl in wizardComponentsList)
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
        private Label CreateLabel(Control parentControl, int xPos, int yPos, string Text)
        {
            Label lb = new Label();
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
        private TextBox CreateTextBox(Control parentControl, int xPos, int yPos)
        {
            TextBox tb = new TextBox();
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
        private NumericUpDown CreateNumericUpDown(Control parentControl, int xPos, int yPos, int minVal, int maxVal, int Val)
        {
            NumericUpDown nud = new NumericUpDown();
            nud.Parent = parentControl;
            Point pt = new Point(xPos, yPos);
            nud.Location = pt;
            nud.Minimum = minVal;
            nud.Maximum = maxVal;
            nud.Value = Val;
            return nud;
        }

        /// <summary>
        /// создание компонента для выбора месяца
        /// </summary>
        /// <param name="parentControl">Куда помещаем компонент</param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        /// <returns></returns>
        private ComboBox CreateCalendarComboBox(Control parentControl, int xPos, int yPos)
        {
            ComboBox cb = new ComboBox();
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
            cb.SelectedIndex = 0;
            return cb;
        }

        private TreeNode selectedNode;

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            selectedNode = tvSources.SelectedNode;
            utcWizard.SelectedTab = utcWizard.Tabs[1];
            btnPrevPage.Enabled = true;
            btnNextPage.Enabled = false;
            lCaption.Text = GetLabelText();// string.Format("Выбор параметров для  '{0}'", paramsName);
            ctrl_TextChanged(null, null);
            btnNextPage.Focus();
        }

        private string GetLabelText()
        {
            //tvSources.SelectedNode.Text.Split('-')[1];
            //tvSources.SelectedNode.Parent.Text.Split('-')[1];
            return string.Format("{0}{1}{2}", tvSources.SelectedNode.Parent.Text, Environment.NewLine, tvSources.SelectedNode.Text); 
        }

        private void btnPrevPage_Click(object sender, EventArgs e)
        {
            btnPrevPage.Enabled = false;
            btnApply.Enabled = false;
            utcWizard.SelectedTab = utcWizard.Tabs[0];
            tvSources.SelectedNode = selectedNode;
            tvSources.Select();
            //tvSources.
            TreeViewEventArgs re = new TreeViewEventArgs(selectedNode);
            tvSources_AfterSelect(tvSources, re);
            lCaption.Text = "Вы можете добавить новый источник данных. Тип нового источника определяется набором выбранных параметров";
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            IDataSource ds = dsManager.DataSources.CreateElement();
            // добавляем в источник параметры по умолчанию
            ds.BudgetName = string.Empty;
            ds.DataCode = sDataCode;
            ds.DataName = sDataName;
            ds.ParametersType = parametrsType;
            ds.SupplierCode = sSuplierCode;
            ds.Territory = string.Empty;
            ds.Variant = string.Empty;
            // для каждого типа источника пытаемся получить параметры, введенные с формы
            if (parametrsType != ParamKindTypes.WithoutParams)
            {
                string value = string.Empty;
                // год
                value = GetSourceParamValue(sourceParams.year);
                if (value != string.Empty)
                    ds.Year = Convert.ToInt32(value);
                // вариант
                ds.Variant = GetSourceParamValue(sourceParams.variant);
                // квартал
                value = GetSourceParamValue(sourceParams.quarter);
                if (value != string.Empty)
                    ds.Quarter = Convert.ToInt32(value);
                // месяц
                value = GetSourceParamValue(sourceParams.month);
                if (value != string.Empty)
                    ds.Month = Convert.ToInt32(value);
                // территория
                ds.Territory = GetSourceParamValue(sourceParams.territory);

                // финансовый орган
                ds.BudgetName = GetSourceParamValue(sourceParams.budgetName);
            }
            if (!dsManager.DataSources.Contains(ds))
            {
                dsManager.DataSources.Add(ds);
                // Не надо уничтожать серверные объекты!
                //dsManager.Dispose();
                Close();
            }
            else
                MessageBox.Show("Добавляемый источник уже существует", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
           }

        private string GetSourceParamValue(sourceParams par)
        {
            Control ctrl;
            string returnValue = string.Empty;
            foreach (wizardValueComponents component in wizardComponentsList)
            {
                if (component.sourceParam == par)
                {
                    ctrl = component.control;
                    if (ctrl != null)
                        if (par == sourceParams.month)
                        {
                            int month = ((ComboBox)ctrl).SelectedIndex + 1;
                            return month.ToString();
                        }
                        else
                           returnValue = ctrl.Text;
                    else
                        returnValue = string.Empty;

                }
            }
            return returnValue;
        }

    }
}