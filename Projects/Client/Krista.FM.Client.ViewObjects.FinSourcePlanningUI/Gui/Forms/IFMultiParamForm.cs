using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Krista.FM.Client.Common;
using Krista.FM.ServerLibrary;
using System.Globalization;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using Krista.FM.Client.Workplace.Gui;
using System.IO;
using Krista.FM.Client.Common.Forms;

namespace Krista.FM.Client.ViewObjects.FinSourcePlanningUI.Gui.Forms
{
    public enum ConstructorType
    {   
        [Browsable(false)]
        ctNone,
        [Description("Кредиты организаций")]
        ctCreditOrg,
        [Description("Кредиты бюджетов")]
        ctCreditBud,
        [Description("Гарантии")]
        [Browsable(false)]
        ctGarant
    }

    public partial class IFMultiParamForm : MultiParamForm
    {
        private static FilterablePropertyBase pgdcConstructor;

        private static ConstructorType formType;
 
        public IFMultiParamForm()
        {
            InitializeComponent();
        }

        public static bool ShowFormEx(IWin32Window parent, Dictionary<string, string> paramList,
            Dictionary<string, string> paramCaption, ConstructorType constructorType)
        {
            formType = constructorType;
            CommonBookEditor.clsManager = FinSourcePlanningNavigation.Instance.Workplace.ClsManager;
            CommonIDConverter.scheme = WorkplaceSingleton.Workplace.ActiveScheme;
            if (formType != ConstructorType.ctNone) 
            {
                return ShowConstructorForm(parent, paramList);
            }
            else
            {
                return ShowForm(parent, paramList, paramCaption);
            }
        }

        public static bool ShowForm(IWin32Window parent, Dictionary<string, string> paramList,
            Dictionary<string, string> paramCaption)
        {
            IFMultiParamForm form = new IFMultiParamForm();

            formParams = paramList;

            pgdc = new IFPropertyGridParamsClass();
            FillParamVisibilityFlags(pgdc, paramList, paramCaption);
            form.pg.SelectedObject = pgdc;
            SetComponentSize(form);

            form.btnSave.Visible = false;
            form.btnLoad.Visible = false;
            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                FillParamValues(pgdc, paramList);
                return true;
            }
            return false;
        }

        public static bool ShowConstructorForm(IWin32Window parent, Dictionary<string, string> paramList)
        {
            IFMultiParamForm form = new IFMultiParamForm();

            if (formType == ConstructorType.ctCreditOrg) 
            {
               pgdcConstructor = new PropertyGridConstructorClass();
            }
            if (formType == ConstructorType.ctGarant) 
            {
                pgdcConstructor = new PropertyGridGarantConstructorClass();
            }

            form.pg.SelectedObject = pgdcConstructor;
            form.pg.PropertySort = PropertySort.Categorized;

            form.Width = 800;
            form.Height = 590;

            SetComponentSizeEx(form);

            if (form.ShowDialog(parent) == DialogResult.OK)
            {
                FillVisibleParamValues(pgdcConstructor, paramList);
                return true;
            }
            return false;
        }

        private static void SetComponentSizeEx(IFMultiParamForm form)
        {
            form.pg.Height = form.Height - 75;
            form.pg.Width = form.Width - 25;
            form.btnOK.Top = form.pg.Height + 15;
            form.btnCancel.Top = form.btnOK.Top;
            form.labelError.Top = form.btnOK.Top - 5;

            form.btnCancel.Left = form.Width - 100;
            form.btnOK.Left = form.btnCancel.Left - 80;

            form.btnSave.Top = form.btnOK.Top;
            form.btnLoad.Top = form.btnOK.Top;
            form.btnLoad.Left = form.btnOK.Left - 80;
            form.btnSave.Left = form.btnLoad.Left - 80;
        }

        protected override bool ValidateParams()
        {
            if (formType != ConstructorType.ctNone) return true;
            return base.ValidateParams();
        }

        private void pg_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            pg.Refresh();
        }

        private static void FillVisibleParamValues(FilterablePropertyBase pgdc, Dictionary<string, string> paramList)
        {
            paramList.Clear();
            foreach (PropertyInfo prop in pgdc.GetType().GetProperties())
            {
                if (prop.GetValue(pgdc, null) != null)
                {
                    paramList.Add(prop.Name, prop.GetValue(pgdc, null).ToString());
                }
                else
                {
                    paramList.Add(prop.Name, string.Empty);
                }
            }
        }

        private void MultiParamForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((formType != ConstructorType.ctNone) && DialogResult == DialogResult.OK && !ValidateParams())
            { 
                e.Cancel = true; 
            }
        }

        private void InitializeComponent()
        {
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(338, 236);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(419, 236);
            // 
            // pg
            // 
            this.pg.Size = new System.Drawing.Size(485, 178);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(177, 236);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 25;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(257, 236);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 26;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Пользовательские отчеты (*.krista)|*.krista";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "*.krista";
            this.saveFileDialog1.Filter = "Пользовательские отчеты (*.krista)|*.krista";
            // 
            // IFMultiParamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(506, 271);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnLoad);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "IFMultiParamForm";
            this.SizeChanged += new System.EventHandler(this.IFMultiParamForm_SizeChanged);
            this.Controls.SetChildIndex(this.btnLoad, 0);
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.pg, 0);
            this.Controls.SetChildIndex(this.labelError, 0);
            this.ResumeLayout(false);

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try                    
                {
                    string line;
                    System.IO.StreamReader file = new System.IO.StreamReader(openFileDialog1.FileName);
                    while ((line = file.ReadLine()) != null)
                    {
                        string propName = line.Split('=')[0];
                        Type propType = pgdcConstructor.GetType().GetProperty(propName).PropertyType;
                        string propValue = string.Empty;
                        propValue = line.Split('=')[1];
                        object value = propValue;
                        if (propType == typeof(Int32))
                            value = Convert.ToInt32(propValue);
                        if (propType == typeof(DateTime))
                            value = Convert.ToDateTime(propValue);
                        if (propType == typeof(Boolean))
                            value = Convert.ToBoolean(propValue);
                        if (propType.IsEnum)
                            value = Enum.Parse(propType, propValue);
                        pgdcConstructor.GetType().GetProperty(propName).SetValue(pgdcConstructor, value, null);
                    }
                    file.Close();
                    pg.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка чтении файла: " + ex.Message);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Создаём или перезаписываем существующий файл
                    StreamWriter sw = File.CreateText(saveFileDialog1.FileName);
                    //Записываем текст в поток файла
                    foreach (PropertyInfo prop in pgdcConstructor.GetType().GetProperties())
                    {
                        if (prop.GetValue(pgdcConstructor, null) != null)
                        {
                            sw.WriteLine(string.Format("{0}={1}", prop.Name, prop.GetValue(pgdcConstructor, null)));
                        }
                        else
                        {
                            sw.WriteLine(string.Format("{0}={1}", prop.Name, string.Empty));
                        }
                    }
                    //Закрываем файл
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка записи в файл: " + ex.Message);
                }
            }
        }

        private void IFMultiParamForm_SizeChanged(object sender, EventArgs e)
        {
            SetComponentSizeEx(sender as IFMultiParamForm);
        }
    }

    #region Списки для параметров

    public enum ContractTypeEnum
    {
        [Description("Действующие договора")]
        i1,
        [Description("Действующие и закрытые договора")]
        i2,
        [Description("Закрытые договора")]
        i3
    }

    public enum VariantTypeEnum
    {
        [Description("Действующие договора")]
        i1,
        [Description("Действующие и Архив")]
        i2,
        [Description("Архив")]
        i3
    }

    public enum DateParamEnum
    {
        [Description("Все договора")]
        i1,
        [Description("Начиная с даты")]
        i2,
        [Description("До даты")]
        i3,
        [Description("Начиная с даты до даты")]
        i4
    }

    #endregion

    #region Класс с общим набором свойств

    public class IFPropertyGridParamsClass : PropertyGridParamsClass
    {
        bool flagContractType = false;
        [Browsable(false)]
        [PropertyOrder(130)]
        public bool visibilityIFContractType
        {
            get { return flagContractType; }
            set { flagContractType = value; }
        }
        ContractTypeEnum contractType;
        [DisplayName("Статус договора")]
        [DynamicPropertyFilter("visibilityIFContractType", "True")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(140)]
        public ContractTypeEnum IFContractType
        {
            get { return contractType; }
            set { contractType = value; }
        }

        bool flagVariantType = false;
        [Browsable(false)]
        [PropertyOrder(150)]
        public bool visibilityIFVariantType
        {
            get { return flagVariantType; }
            set { flagVariantType = value; }
        }
        VariantTypeEnum variantType;
        [DisplayName("Вариант.ИФ")]
        [DynamicPropertyFilter("visibilityIFVariantType", "True")]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(160)]
        public VariantTypeEnum IFVariantType
        {
            get { return variantType; }
            set { variantType = value; }
        }

        bool flagVariantID = false;
        [Browsable(false)]
        [PropertyOrder(170)]
        public bool visibilityIFVariantID
        {
            get { return flagVariantID; }
            set { flagVariantID = value; }
        }
        int variantID;
        [DisplayName("Вариант.ИФ")]
        [Editor(typeof(VariantIDEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("visibilityIFVariantID", "True")]
        [TypeConverter(typeof(VariantIDConverter))]
        [PropertyOrder(180)]
        public int IFVariantID
        {
            get { return variantID; }
            set { variantID = value; }
        }

        bool flagDVariantID = false;
        [Browsable(false)]
        [PropertyOrder(190)]
        public bool visibilityIFDVariantID
        {
            get { return flagDVariantID; }
            set { flagDVariantID = value; }
        }
        int variantDID = 0;
        [DisplayName("Вариант.Проект доходов")]
        [Editor(typeof(DVariantIDEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("visibilityIFDVariantID", "True")]
        [TypeConverter(typeof(DVariantIDConverter))]
        [PropertyOrder(200)]
        public int IFDVariantID
        {
            get { return variantDID; }
            set { variantDID = value; }
        }

        bool flagRVariantID = false;
        [Browsable(false)]
        [PropertyOrder(210)]
        public bool visibilityIFRVariantID
        {
            get { return flagRVariantID; }
            set { flagRVariantID = value; }
        }
        int variantRID = 781;
        [DisplayName("Вариант.Проект расходов")]
        [Editor(typeof(RVariantIDEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("visibilityIFRVariantID", "True")]
        [TypeConverter(typeof(RVariantIDConverter))]
        [PropertyOrder(220)]
        public int IFRVariantID
        {
            get { return variantRID; }
            set { variantRID = value; }
        }

        bool flagOrgID = false;
        [Browsable(false)]
        [PropertyOrder(230)]
        public bool visibilityOrgID
        {
            get { return flagOrgID; }
            set { flagOrgID = value; }
        }
        int orgID = -1;
        [DisplayName("Организация")]
        [TypeConverter(typeof(OrgIDConverter))]
        [Editor(typeof(OrgIDEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("visibilityOrgID", "True")]
        [PropertyOrder(240)]
        public int OrgID
        {
            get { return orgID; }
            set { orgID = value; }
        }

        bool flagPlanStartDate = false;
        [Browsable(false)]
        [PropertyOrder(250)]
        public bool visibilityIFPlanStartDate
        {
            get { return flagPlanStartDate; }
            set { flagPlanStartDate = value; }
        }

        bool flagRegionID = false;
        [Browsable(false)]
        [PropertyOrder(290)]
        public bool visibilityRegionID
        {
            get { return flagRegionID; }
            set { flagRegionID = value; }
        }
        int regionID = -1;
        [DisplayName("Район")]
        [TypeConverter(typeof(RegionIDConverter))]
        [Editor(typeof(RegionIDEditor), typeof(UITypeEditor))]
        [DynamicPropertyFilter("visibilityRegionID", "True")]
        [PropertyOrder(300)]
        public int RegionID
        {
            get { return regionID; }
            set { regionID = value; }
        }

        bool flagPlanExchange = false;
        [Browsable(false)]
        [PropertyOrder(310)]
        public bool visibilityIFPlanExchange
        {
            get { return flagPlanExchange; }
            set { flagPlanExchange = value; }
        }
        string planExchange = "38";
        [DisplayName("Прогнозный курс")]
        [DynamicPropertyFilter("visibilityIFPlanExchange", "True")]
        [PropertyOrder(320)]
        public string IFPlanExchange
        {
            get { return planExchange; }
            set { planExchange = value; }
        }
    }

    #endregion

    public class VariantIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_Variant_Borrow_Key;
        }
    }

    public class DVariantIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_Variant_PlanIncomes_Key;
        }
    }

    public class RVariantIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_Variant_PlanOutcomes_Key;
        }
    }

    public class OrgIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_Organizations_Plan_Key;
        }
    }

    public class RegionIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_Regions_Plan;
        }
    }

    public class OkvIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.d_OKV_Currency_Key;
        }
    }

    public class StatusIDEditor : CommonBookEditor
    {
        public override string ClsFormKey()
        {
            return SchemeObjectsKeys.fx_S_StatusPlan_Key;
        }
    }

    #region Конверторы списков

    class VariantIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_Variant_Borrow_Key;
        }
    }

    class DVariantIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_Variant_PlanIncomes_Key;
        }
    }

    class RVariantIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_Variant_PlanOutcomes_Key;
        }
    }

    class OrgIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_Organizations_Plan_Key;
        }
    }

    class RegionIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_Regions_Plan;
        }
    }

    class OkvIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.d_OKV_Currency_Key;
        }
    }

    class StatusIDConverter : CommonIDConverter
    {
        public override string GetBookEntityName()
        {
            return SchemeObjectsKeys.fx_S_StatusPlan_Key;
        }
    }

    #endregion

    enum FactDebtMainFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("ОД факт привлечения – ОД факт погашения")]
        i1,
        [Description("ОД план привлечения – ОД факт погашения")]
        i2,
        [Description("ОД план погашения – ОД факт погашения")]
        i3
    }

    enum FactDebtServiceFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("% план погашения – % факт погашения")]
        i1
    }

    enum FactDebtOverdatedFormulaEnum
    {
        [Description("Не выводить")]
        i0,
        [Description("ОД факт привл. –ОД факт пог. +%план пог. –%факт пог.")]
        i1,
        [Description("ОД план привл. –ОД факт пог. +%план пог. –%факт пог.")]
        i2,
        [Description("ОД план пог. – ОД факт пог. + %план пог. –%факт пог.")]
        i3
    }

    enum DetailWriteTypeEnum
    {
        [Description("Не выводить")]
        i1,
        [Description("Все")]
        i2,
        [Description("Начиная с даты")]
        i3,
        [Description("Заканчивая датой")]
        i4,
        [Description("Период")]
        i5,
        [Description("Детализация строк")]
        i6,
        [Description("До даты отчета 1")]
        i7,
        [Description("C даты отчета 1")]
        i8,
        [Description("До даты отчета 2")]
        i9,
        [Description("C даты отчета 2")]
        i10,
        [Description("C даты отчета 1 по дату отчета 2")]
        i11
    }

    [TypeConverter(typeof(PropertySorter))]
    class PropertyGridConstructorClass : FilterablePropertyBase
    {
        const string category0 = "0. Общие параметры";
        const string category1 = "1. Фильтры таблицы фактов";
        const string category2 = "2. Выбор столбцов таблицы и фильтр";
        const string category3 = "3. Задолженность";

        const string space = "   ";

        const string startDateCaption = space + "Начиная с даты";
        const string endDateCaption = space + "Заканчивая датой";

        const string startDateFormula = "Начиная с даты";
        const string endDateFormula = "Заканчивая датой";

        const int undefinedBookValue = -999;

        ConstructorType docType = ConstructorType.ctCreditOrg;
        [DisplayName("Тип документа")]
        [Category(category0)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(10)]
        public ConstructorType DocumentType
        {
            get { return docType; }
            set { docType = value; }
        }

        string reportCaption;
        [DisplayName("Название отчета")]
        [Category(category0)]
        [PropertyOrder(20)]
        public string ReportCaption
        {
            get { return reportCaption; }
            set { reportCaption = value; }
        }

        DateTime reportDate1;
        [DisplayName("Дата отчета 1")]
        [Category(category0)]
        [PropertyOrder(21)]
        public DateTime ReportDate1
        {
            get { return reportDate1; }
            set { reportDate1 = value; }
        }

        DateTime reportDate2;
        [DisplayName("Дата отчета 2")]
        [Category(category0)]
        [PropertyOrder(22)]
        public DateTime ReportDate2
        {
            get { return reportDate2; }
            set { reportDate2 = value; }
        }

        // фильтры мастер-таблицы
        int orgID = undefinedBookValue;
        [DisplayName("Наименование кредитора")]
        [TypeConverter(typeof(OrgIDConverter))]
        [Editor(typeof(OrgIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(30)]
        public int FilterRefOrganizations
        {
            get { return orgID; }
            set { orgID = value; }
        }
        
        string contractNum;
        [DisplayName("Номер договора")]
        [Category(category1)]
        [PropertyOrder(40)]
        public string FilterNum
        {
            get { return contractNum; }
            set { contractNum = value; }
        }

        DateParamEnum dateContractParamType = DateParamEnum.i1;
        [DisplayName("Дата заключения")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(50)]
        public DateParamEnum FilterContractDateParamType
        {
            get { return dateContractParamType; }
            set { dateContractParamType = value; }
        }

        DateTime contractDateStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterContractDateParamType", "i2;i4")]
        [PropertyOrder(60)]
        public DateTime FilterContractDateStart
        {
            get { return contractDateStart; }
            set { contractDateStart = value; }
        }

        DateTime contractDateEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterContractDateParamType", "i3;i4")]
        [PropertyOrder(70)]
        public DateTime FilterContractDateEnd
        {
            get { return contractDateEnd; }
            set { contractDateEnd = value; }
        }

        DateParamEnum dateStartParamType = DateParamEnum.i1;
        [DisplayName("Дата возникновения обязательства")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(80)]
        public DateParamEnum FilterStartDateParamType
        {
            get { return dateStartParamType; }
            set { dateStartParamType = value; }
        }

        DateTime startDateStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterStartDateParamType", "i2;i4")]
        [PropertyOrder(90)]
        public DateTime FilterStartDateStart
        {
            get { return startDateStart; }
            set { startDateStart = value; }
        }

        DateTime startDateEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterStartDateParamType", "i3;i4")]
        [PropertyOrder(100)]
        public DateTime FilterStartDateEnd
        {
            get { return startDateEnd; }
            set { startDateEnd = value; }
        }

        DateParamEnum dateEndParamType = DateParamEnum.i1;
        [DisplayName("Дата окончательного погашения")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(110)]
        public DateParamEnum FilterEndDateParamType
        {
            get { return dateEndParamType; }
            set { dateEndParamType = value; }
        }

        DateTime endDateStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterEndDateParamType", "i2;i4")]
        [PropertyOrder(120)]
        public DateTime FilterEndDateStart
        {
            get { return endDateStart; }
            set { endDateStart = value; }
        }

        DateTime endDateEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterEndDateParamType", "i3;i4")]
        [PropertyOrder(130)]
        public DateTime FilterEndDateEnd
        {
            get { return endDateEnd; }
            set { endDateEnd = value; }
        }

        int variantID = undefinedBookValue;
        [DisplayName("Вариант")]
        [Browsable(true)]
        [TypeConverter(typeof(VariantIDConverter))]
        [Editor(typeof(VariantIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(140)]
        public int FilterRefVariant
        {
            get { return variantID; }
            set { variantID = value; }
        }

        int okvID = undefinedBookValue;
        [DisplayName("Валюта")]
        [Browsable(true)]
        [TypeConverter(typeof(OkvIDConverter))]
        [Editor(typeof(OkvIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(150)]
        public int FilterRefOkv
        {
            get { return okvID; }
            set { okvID = value; }
        }

        int statusID = undefinedBookValue;
        [DisplayName("Статус договора")]
        [Browsable(true)]
        [TypeConverter(typeof(StatusIDConverter))]
        [Editor(typeof(StatusIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(160)]
        public int FilterRefSStatusPlan
        {
            get { return statusID; }
            set { statusID = value; }
        }

        // 2 раздел

        bool viewCreditorName = true;
        [DisplayName("Наименование кредитора")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(170)]
        public bool ViewCalcOrgName
        {
            get { return viewCreditorName; }
            set { viewCreditorName = value; }
        }

        bool viewDocInfo = true;
        [DisplayName("Наименование договора, номер и дата заключения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(180)]
        public bool ViewCalcContractNum
        {
            get { return viewDocInfo; }
            set { viewDocInfo = value; }
        }

        bool viewRegNum = true;
        [DisplayName("Регистрационный номер")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(190)]
        public bool ViewRegNum
        {
            get { return viewRegNum; }
            set { viewRegNum = value; }
        }

        bool viewContractDate = true;
        [DisplayName("Дата заключения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(200)]
        public bool ViewContractDate
        {
            get { return viewContractDate; }
            set { viewContractDate = value; }
        }

        bool viewBasement = true;
        [DisplayName("Основание для заключения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(210)]
        public bool ViewOccasion
        {
            get { return viewBasement; }
            set { viewBasement = value; }
        }

        bool viewPurpose = true;
        [DisplayName("Целевое назначение")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(220)]
        public bool ViewPurpose
        {
            get { return viewPurpose; }
            set { viewPurpose = value; }
        }

        bool viewVolume = true;
        [DisplayName("Объем долгового обязательства по договору")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(230)]
        public bool ViewSum
        {
            get { return viewVolume; }
            set { viewVolume = value; }
        }

        bool viewPercent = true;
        [DisplayName("Процентная ставка")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(240)]
        public bool ViewCreditPercent
        {
            get { return viewPercent; }
            set { viewPercent = value; }
        }

        bool viewStartDate = true;
        [DisplayName("Дата возникновения обязательства")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(250)]
        public bool ViewStartDate
        {
            get { return viewStartDate; }
            set { viewStartDate = value; }
        }

        bool viewEndDate = true;
        [DisplayName("Дата окончательного погашения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(260)]
        public bool ViewEndDate
        {
            get { return viewEndDate; }
            set { viewEndDate = value; }
        }

        bool viewRenewalDate = true;
        [DisplayName("Дата пролонгации")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(270)]
        public bool ViewRenewalDate
        {
            get { return viewRenewalDate; }
            set { viewRenewalDate = value; }
        }

        // настройка деталей

        // план привлечения
        DetailWriteTypeEnum detailPlanAttract = DetailWriteTypeEnum.i1;
        [DisplayName("План привлечения основного долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(280)]
        public DetailWriteTypeEnum Detail3PlanAttract
        {
            get { return detailPlanAttract; }
            set { detailPlanAttract = value; }
        }

        DateTime detailPlanAttractStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail3PlanAttract", "i3;i5;i6")]
        [PropertyOrder(290)]
        public DateTime Detail3PlanAttractStart
        {
            get { return detailPlanAttractStart; }
            set { detailPlanAttractStart = value; }
        }

        DateTime detailPlanAttractEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail3PlanAttract", "i4;i5;i6")]
        [PropertyOrder(300)]
        public DateTime Detail3PlanAttractEnd
        {
            get { return detailPlanAttractEnd; }
            set { detailPlanAttractEnd = value; }
        }

        // факт привлечения
        DetailWriteTypeEnum detailFactAttract = DetailWriteTypeEnum.i1;
        [DisplayName("Факт привлечения основного долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(310)]
        public DetailWriteTypeEnum Detail0FactAttract
        {
            get { return detailFactAttract; }
            set { detailFactAttract = value; }
        }

        DateTime detailFactAttractStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail0FactAttract", "i3;i5;i6")]
        [PropertyOrder(320)]
        public DateTime Detail0FactAttractStart
        {
            get { return detailFactAttractStart; }
            set { detailFactAttractStart = value; }
        }

        DateTime detailFactAttractEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail0FactAttract", "i4;i5;i6")]
        [PropertyOrder(330)]
        public DateTime Detail0FactAttractEnd
        {
            get { return detailFactAttractEnd; }
            set { detailFactAttractEnd = value; }
        }

        // план погашения
        DetailWriteTypeEnum detailPlanRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("План погашения основного долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(340)]
        public DetailWriteTypeEnum Detail2PlanRedemption
        {
            get { return detailPlanRedemption; }
            set { detailPlanRedemption = value; }
        }

        DateTime detailPlanRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail2PlanRedemption", "i3;i5;i6")]
        [PropertyOrder(350)]
        public DateTime Detail2PlanRedemptionStart
        {
            get { return detailPlanRedemptionStart; }
            set { detailPlanRedemptionStart = value; }
        }

        DateTime detailPlanRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail2PlanRedemption", "i4;i5;i6")]
        [PropertyOrder(360)]
        public DateTime Detail2PlanRedemptionEnd
        {
            get { return detailPlanRedemptionEnd; }
            set { detailPlanRedemptionEnd = value; }
        }

        // факт погашения
        DetailWriteTypeEnum detailFactRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения основного долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(370)]
        public DetailWriteTypeEnum Detail1FactRedemption
        {
            get { return detailFactRedemption; }
            set { detailFactRedemption = value; }
        }

        DateTime detailFactRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail1FactRedemption", "i3;i5;i6")]
        [PropertyOrder(380)]
        public DateTime Detail1FactRedemptionStart
        {
            get { return detailFactRedemptionStart; }
            set { detailFactRedemptionStart = value; }
        }

        DateTime detailFactRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail1FactRedemption", "i4;i5;i6")]
        [PropertyOrder(390)]
        public DateTime Detail1FactRedemptionEnd
        {
            get { return detailFactRedemptionEnd; }
            set { detailFactRedemptionEnd = value; }
        }

        // План обслуживания долга
        DetailWriteTypeEnum detailPlanService = DetailWriteTypeEnum.i1;
        [DisplayName("План обслуживания долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(400)]
        public DetailWriteTypeEnum Detail5PlanService
        {
            get { return detailPlanService; }
            set { detailPlanService = value; }
        }

        DateTime detailPlanServiceStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail5PlanService", "i3;i5;i6")]
        [PropertyOrder(410)]
        public DateTime Detail5PlanServiceStart
        {
            get { return detailPlanServiceStart; }
            set { detailPlanServiceStart = value; }
        }

        DateTime detailPlanServiceEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail5PlanService", "i4;i5;i6")]
        [PropertyOrder(420)]
        public DateTime Detail5PlanServiceEnd
        {
            get { return detailPlanServiceEnd; }
            set { detailPlanServiceEnd = value; }
        }

        // Факт погашения процентов
        DetailWriteTypeEnum detailFactPercentRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения процентов")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(430)]
        public DetailWriteTypeEnum Detail4FactPercentRedemption
        {
            get { return detailFactPercentRedemption; }
            set { detailFactPercentRedemption = value; }
        }

        DateTime detailFactPercentRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail4FactPercentRedemption", "i3;i5;i6")]
        [PropertyOrder(440)]
        public DateTime Detail4FactPercentRedemptionStart
        {
            get { return detailFactPercentRedemptionStart; }
            set { detailFactPercentRedemptionStart = value; }
        }

        DateTime detailFactPercentRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail4FactPercentRedemption", "i4;i5;i6")]
        [PropertyOrder(450)]
        public DateTime Detail4FactPercentRedemptionEnd
        {
            get { return detailFactPercentRedemptionEnd; }
            set { detailFactPercentRedemptionEnd = value; }
        }

        // Начисление пени по основному долгу
        DetailWriteTypeEnum detailPeniMainDebt = DetailWriteTypeEnum.i1;
        [DisplayName("Начисление пени по основному долгу")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(460)]
        public DetailWriteTypeEnum Detail6PeniMainDebt
        {
            get { return detailPeniMainDebt; }
            set { detailPeniMainDebt = value; }
        }

        DateTime detailPeniMainDebtStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail6PeniMainDebt", "i3;i5;i6")]
        [PropertyOrder(470)]
        public DateTime Detail6PeniMainDebtStart
        {
            get { return detailPeniMainDebtStart; }
            set { detailPeniMainDebtStart = value; }
        }

        DateTime detailPeniMainDebtEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail6PeniMainDebt", "i4;i5;i6")]
        [PropertyOrder(480)]
        public DateTime Detail6PeniMainDebtEnd
        {
            get { return detailPeniMainDebtEnd; }
            set { detailPeniMainDebtEnd = value; }
        }

        // Начисление пени по процентам
        DetailWriteTypeEnum detailPeniPercent = DetailWriteTypeEnum.i1;
        [DisplayName("Начисление пени по процентам")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(490)]
        public DetailWriteTypeEnum Detail7PeniPercent
        {
            get { return detailPeniPercent; }
            set { detailPeniPercent = value; }
        }

        DateTime detailPeniPercentStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail7PeniPercent", "i3;i5;i6")]
        [PropertyOrder(500)]
        public DateTime Detail7PeniPercentStart
        {
            get { return detailPeniPercentStart; }
            set { detailPeniPercentStart = value; }
        }

        DateTime detailPeniPercentEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail7PeniPercent", "i4;i5;i6")]
        [PropertyOrder(510)]
        public DateTime Detail7PeniPercentEnd
        {
            get { return detailPeniPercentEnd; }
            set { detailPeniPercentEnd = value; }
        }

        // Факт погашения пени по основному долгу
        DetailWriteTypeEnum detailFactMainDebtPeniRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения пени по основному долгу")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(520)]
        public DetailWriteTypeEnum Detail8FactMainDebtPeniRedemption
        {
            get { return detailFactMainDebtPeniRedemption; }
            set { detailFactMainDebtPeniRedemption = value; }
        }

        DateTime detailFactMainDebtPeniRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail8FactMainDebtPeniRedemption", "i3;i5;i6")]
        [PropertyOrder(530)]
        public DateTime Detail8FactMainDebtPeniRedemptionStart
        {
            get { return detailFactMainDebtPeniRedemptionStart; }
            set { detailFactMainDebtPeniRedemptionStart = value; }
        }

        DateTime detailFactMainDebtPeniRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail8FactMainDebtPeniRedemption", "i4;i5;i6")]
        [PropertyOrder(540)]
        public DateTime Detail8FactMainDebtPeniRedemptionEnd
        {
            get { return detailFactMainDebtPeniRedemptionEnd; }
            set { detailFactMainDebtPeniRedemptionEnd = value; }
        }

        // Факт погашения пени по процентам
        DetailWriteTypeEnum detailFactPercentPeniRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения пени по процентам")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(550)]
        public DetailWriteTypeEnum Detail9FactPercentPeniRedemption
        {
            get { return detailFactPercentPeniRedemption; }
            set { detailFactPercentPeniRedemption = value; }
        }

        DateTime detailFactPercentPeniRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail9FactPercentPeniRedemption", "i3;i5;i6")]
        [PropertyOrder(560)]
        public DateTime Detail9FactPercentPeniRedemptionStart
        {
            get { return detailFactPercentPeniRedemptionStart; }
            set { detailFactPercentPeniRedemptionStart = value; }
        }

        DateTime detailFactPercentPeniRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail9FactPercentPeniRedemption", "i4;i5;i6")]
        [PropertyOrder(570)]
        public DateTime Detail9FactPercentPeniRedemptionEnd
        {
            get { return detailFactPercentPeniRedemptionEnd; }
            set { detailFactPercentPeniRedemptionEnd = value; }
        }

        // формулы задолженности
        FactDebtMainFormulaEnum mainDebtFormula = FactDebtMainFormulaEnum.i1;
        [DisplayName("Фактическая задолженность по основному долгу")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(580)]
        public FactDebtMainFormulaEnum FormulaMainDebt
        {
            get { return mainDebtFormula; }
            set { mainDebtFormula = value; }
        }

        DateTime mainDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(590)]
        public DateTime FormulaMainDebt1Start
        {
            get { return mainDebtFormula1Start; }
            set { mainDebtFormula1Start = value; }
        }

        DateTime mainDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(600)]
        public DateTime FormulaMainDebt1End
        {
            get { return mainDebtFormula1End; }
            set { mainDebtFormula1End = value; }
        }

        DateTime mainDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(610)]
        public DateTime FormulaMainDebt2Start
        {
            get { return mainDebtFormula2Start; }
            set { mainDebtFormula2Start = value; }
        }

        DateTime mainDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(620)]
        public DateTime FormulaMainDebt2End
        {
            get { return mainDebtFormula2End; }
            set { mainDebtFormula2End = value; }
        }
        //
        FactDebtServiceFormulaEnum serviceDebtFormula = FactDebtServiceFormulaEnum.i1;
        [DisplayName("Фактическая задолженность по обслуживанию")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(630)]
        public FactDebtServiceFormulaEnum FormulaServiceDebt
        {
            get { return serviceDebtFormula; }
            set { serviceDebtFormula = value; }
        }

        DateTime serviceDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(640)]
        public DateTime FormulaServiceDebt1Start
        {
            get { return serviceDebtFormula1Start; }
            set { serviceDebtFormula1Start = value; }
        }

        DateTime serviceDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(650)]
        public DateTime FormulaServiceDebt1End
        {
            get { return serviceDebtFormula1End; }
            set { serviceDebtFormula1End = value; }
        }

        DateTime serviceDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(660)]
        public DateTime FormulaServiceDebt2Start
        {
            get { return serviceDebtFormula2Start; }
            set { serviceDebtFormula2Start = value; }
        }

        DateTime serviceDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(670)]
        public DateTime FormulaServiceDebt2End
        {
            get { return serviceDebtFormula2End; }
            set { serviceDebtFormula2End = value; }
        }
        //
        FactDebtOverdatedFormulaEnum overdatedDebtFormula = FactDebtOverdatedFormulaEnum.i1;
        [DisplayName("Просроченная задолженность")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(680)]
        public FactDebtOverdatedFormulaEnum FormulaOverdatedDebt
        {
            get { return overdatedDebtFormula; }
            set { overdatedDebtFormula = value; }
        }
        
        DateTime overdatedDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(690)]
        public DateTime FormulaOverdatedDebt1Start
        {
            get { return overdatedDebtFormula1Start; }
            set { overdatedDebtFormula1Start = value; }
        }

        DateTime overdatedDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(700)]
        public DateTime FormulaOverdatedDebt1End
        {
            get { return overdatedDebtFormula1End; }
            set { overdatedDebtFormula1End = value; }
        }

        DateTime overdatedDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(710)]
        public DateTime FormulaOverdatedDebt2Start
        {
            get { return overdatedDebtFormula2Start; }
            set { overdatedDebtFormula2Start = value; }
        }

        DateTime overdatedDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(720)]
        public DateTime FormulaOverdatedDebt2End
        {
            get { return overdatedDebtFormula2End; }
            set { overdatedDebtFormula2End = value; }
        }

        DateTime overdatedDebtFormula3Start;
        [DisplayName(space + "3 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(730)]
        public DateTime FormulaOverdatedDebt3Start
        {
            get { return overdatedDebtFormula3Start; }
            set { overdatedDebtFormula3Start = value; }
        }

        DateTime overdatedDebtFormula3End;
        [DisplayName(space + "3 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(740)]
        public DateTime FormulaOverdatedDebt3End
        {
            get { return overdatedDebtFormula3End; }
            set { overdatedDebtFormula3End = value; }
        }

        DateTime overdatedDebtFormula4Start;
        [DisplayName(space + "4 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(750)]
        public DateTime FormulaOverdatedDebt4Start
        {
            get { return overdatedDebtFormula4Start; }
            set { overdatedDebtFormula4Start = value; }
        }

        DateTime overdatedDebtFormula4End;
        [DisplayName(space + "4 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(760)]
        public DateTime FormulaOverdatedDebt4End
        {
            get { return overdatedDebtFormula4End; }
            set { overdatedDebtFormula4End = value; }
        }
    }


    // Конструктор Гарантий

    [TypeConverter(typeof(PropertySorter))]
    class PropertyGridGarantConstructorClass : FilterablePropertyBase
    {
        const string category0 = "0. Общие параметры";
        const string category1 = "1. Фильтры таблицы фактов";
        const string category2 = "2. Выбор столбцов таблицы и фильтр";
        const string category3 = "3. Задолженность";

        const string space = "   ";

        const string startDateCaption = space + "Начиная с даты";
        const string endDateCaption = space + "Заканчивая датой";

        const string startDateFormula = "Начиная с даты";
        const string endDateFormula = "Заканчивая датой";

        const int undefinedBookValue = -999;

        ConstructorType docType = ConstructorType.ctGarant;
        [DisplayName("Тип документа")]
        [Category(category0)]
        [ReadOnly(true)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(10)]
        public ConstructorType DocumentType
        {
            get { return docType; }
            set { docType = value; }
        }

        string reportCaption;
        [DisplayName("Название отчета")]
        [Category(category0)]
        [PropertyOrder(20)]
        public string ReportCaption
        {
            get { return reportCaption; }
            set { reportCaption = value; }
        }

        DateTime reportDate1;
        [DisplayName("Дата отчета 1")]
        [Category(category0)]
        [PropertyOrder(30)]
        public DateTime ReportDate1
        {
            get { return reportDate1; }
            set { reportDate1 = value; }
        }

        DateTime reportDate2;
        [DisplayName("Дата отчета 2")]
        [Category(category0)]
        [PropertyOrder(40)]
        public DateTime ReportDate2
        {
            get { return reportDate2; }
            set { reportDate2 = value; }
        }

        // фильтры мастер-таблицы

        int orgID1 = undefinedBookValue;
        [DisplayName("Наименование бенефициара")]
        [TypeConverter(typeof(OrgIDConverter))]
        [Editor(typeof(OrgIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(50)]
        public int FilterRefOrganizationsPlan3
        {
            get { return orgID1; }
            set { orgID1 = value; }
        }

        int orgID2 = undefinedBookValue;
        [DisplayName("Наименование принципала")]
        [TypeConverter(typeof(OrgIDConverter))]
        [Editor(typeof(OrgIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(60)]
        public int FilterRefOrganizations
        {
            get { return orgID2; }
            set { orgID2 = value; }
        }

        string contractNum;
        [DisplayName("Номер договора")]
        [Category(category1)]
        [PropertyOrder(70)]
        public string FilterNum
        {
            get { return contractNum; }
            set { contractNum = value; }
        }

        DateParamEnum dateStartParamType = DateParamEnum.i1;
        [DisplayName("Дата выдачи")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(80)]
        public DateParamEnum FilterStartDateParamType
        {
            get { return dateStartParamType; }
            set { dateStartParamType = value; }
        }

        DateTime startDateStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterStartDateParamType", "i2;i4")]
        [PropertyOrder(90)]
        public DateTime FilterStartDateStart
        {
            get { return startDateStart; }
            set { startDateStart = value; }
        }

        DateTime startDateEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterStartDateParamType", "i3;i4")]
        [PropertyOrder(100)]
        public DateTime FilterStartDateEnd
        {
            get { return startDateEnd; }
            set { startDateEnd = value; }
        }

        DateParamEnum dateDocParamType = DateParamEnum.i1;
        [DisplayName("Дата возникновения долгового обязательства")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(110)]
        public DateParamEnum FilterDateDocParamType
        {
            get { return dateDocParamType; }
            set { dateDocParamType = value; }
        }

        DateTime dateDocStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterDateDocParamType", "i2;i4")]
        [PropertyOrder(120)]
        public DateTime FilterDateDocStart
        {
            get { return dateDocStart; }
            set { dateDocStart = value; }
        }

        DateTime dateDocEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterDateDocParamType", "i3;i4")]
        [PropertyOrder(130)]
        public DateTime FilterDateDocEnd
        {
            get { return dateDocEnd; }
            set { dateDocEnd = value; }
        }

        DateParamEnum dateEndParamType = DateParamEnum.i1;
        [DisplayName("Срок действия гарантии")]
        [Category(category1)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(140)]
        public DateParamEnum FilterEndDateParamType
        {
            get { return dateEndParamType; }
            set { dateEndParamType = value; }
        }

        DateTime endDateStart;
        [DisplayName(startDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterEndDateParamType", "i2;i4")]
        [PropertyOrder(150)]
        public DateTime FilterEndDateStart
        {
            get { return endDateStart; }
            set { endDateStart = value; }
        }

        DateTime endDateEnd;
        [DisplayName(endDateCaption)]
        [Category(category1)]
        [DynamicPropertyFilter("FilterEndDateParamType", "i3;i4")]
        [PropertyOrder(160)]
        public DateTime FilterEndDateEnd
        {
            get { return endDateEnd; }
            set { endDateEnd = value; }
        }

        int variantID = undefinedBookValue;
        [DisplayName("Вариант заимствования")]
        [Browsable(true)]
        [TypeConverter(typeof(VariantIDConverter))]
        [Editor(typeof(VariantIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(170)]
        public int FilterRefVariant
        {
            get { return variantID; }
            set { variantID = value; }
        }

        int okvID = undefinedBookValue;
        [DisplayName("Валюта")]
        [Browsable(true)]
        [TypeConverter(typeof(OkvIDConverter))]
        [Editor(typeof(OkvIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(180)]
        public int FilterRefOkv
        {
            get { return okvID; }
            set { okvID = value; }
        }

        int statusID = undefinedBookValue;
        [DisplayName("Статус договора")]
        [Browsable(true)]
        [TypeConverter(typeof(StatusIDConverter))]
        [Editor(typeof(StatusIDEditor), typeof(UITypeEditor))]
        [Category(category1)]
        [PropertyOrder(190)]
        public int FilterRefSStatusDog
        {
            get { return statusID; }
            set { statusID = value; }
        }

        // 2 раздел

        bool viewBenefitName = true;
        [DisplayName("Наименование бенефициара")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(200)]
        public bool ViewCalcOrg3Name
        {
            get { return viewBenefitName; }
            set { viewBenefitName = value; }
        }

        bool viewCreditorName = true;
        [DisplayName("Наименование принципала")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(210)]
        public bool ViewCalcOrgName
        {
            get { return viewCreditorName; }
            set { viewCreditorName = value; }
        }

        bool viewDocInfo = true;
        [DisplayName("Наименование договора, номер и дата заключения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(220)]
        public bool ViewCalcContractNum
        {
            get { return viewDocInfo; }
            set { viewDocInfo = value; }
        }

        bool viewRegNum = true;
        [DisplayName("Регистрационный номер")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(230)]
        public bool ViewRegNum
        {
            get { return viewRegNum; }
            set { viewRegNum = value; }
        }

        bool viewStartDate = true;
        [DisplayName("Дата выдачи")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(240)]
        public bool ViewStartDate
        {
            get { return viewStartDate; }
            set { viewStartDate = value; }
        }

        bool viewBasement = true;
        [DisplayName("Основание для заключения")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(250)]
        public bool ViewOccasion
        {
            get { return viewBasement; }
            set { viewBasement = value; }
        }

        bool viewPurpose = true;
        [DisplayName("Целевое назначение")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(260)]
        public bool ViewPurpose
        {
            get { return viewPurpose; }
            set { viewPurpose = value; }
        }

        bool viewSum = true;
        [DisplayName("Объем обязательств по гарантии")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(270)]
        public bool ViewSum
        {
            get { return viewSum; }
            set { viewSum = value; }
        }

        bool viewDebtSum = true;
        [DisplayName("Объем обязательств по основному долгу гарантии")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(280)]
        public bool ViewDebtSum
        {
            get { return viewDebtSum; }
            set { viewDebtSum = value; }
        }

        bool viewCalcPercent = true;
        [DisplayName("Процентная ставка")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(290)]
        public bool ViewCalcPercent
        {
            get { return viewCalcPercent; }
            set { viewCalcPercent = value; }
        }

        bool viewDateDoc = true;
        [DisplayName("Дата возникновения обязательства")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(300)]
        public bool ViewDateDoc
        {
            get { return viewDateDoc; }
            set { viewDateDoc = value; }
        }

        bool viewEndDate = true;
        [DisplayName("Срок действия гарантии")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(310)]
        public bool ViewEndDate
        {
            get { return viewEndDate; }
            set { viewEndDate = value; }
        }

        bool viewRenewalDate = true;
        [DisplayName("Дата пролонгации")]
        [TypeConverter(typeof(YesNoConverter))]
        [Category(category2)]
        [PropertyOrder(320)]
        public bool ViewRenewalDate
        {
            get { return viewRenewalDate; }
            set { viewRenewalDate = value; }
        }

        // настройка деталей

        // план привлечения
        DetailWriteTypeEnum detailPlanAttract = DetailWriteTypeEnum.i1;
        [DisplayName("План привлечения")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(330)]
        public DetailWriteTypeEnum Detail7PlanAttract
        {
            get { return detailPlanAttract; }
            set { detailPlanAttract = value; }
        }

        DateTime detailPlanAttractStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail7PlanAttract", "i3;i5;i6")]
        [PropertyOrder(340)]
        public DateTime Detail7PlanAttractStart
        {
            get { return detailPlanAttractStart; }
            set { detailPlanAttractStart = value; }
        }

        DateTime detailPlanAttractEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail7PlanAttract", "i4;i5;i6")]
        [PropertyOrder(350)]
        public DateTime Detail7PlanAttractEnd
        {
            get { return detailPlanAttractEnd; }
            set { detailPlanAttractEnd = value; }
        }

        // факт привлечения
        DetailWriteTypeEnum detailFactAttract = DetailWriteTypeEnum.i1;
        [DisplayName("Факт привлечения")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(360)]
        public DetailWriteTypeEnum Detail0FactAttract
        {
            get { return detailFactAttract; }
            set { detailFactAttract = value; }
        }

        DateTime detailFactAttractStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail0FactAttract", "i3;i5;i6")]
        [PropertyOrder(370)]
        public DateTime Detail0FactAttractStart
        {
            get { return detailFactAttractStart; }
            set { detailFactAttractStart = value; }
        }

        DateTime detailFactAttractEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail0FactAttract", "i4;i5;i6")]
        [PropertyOrder(380)]
        public DateTime Detail0FactAttractEnd
        {
            get { return detailFactAttractEnd; }
            set { detailFactAttractEnd = value; }
        }

        // план погашения
        DetailWriteTypeEnum detailPlanRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("План погашения")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(390)]
        public DetailWriteTypeEnum Detail3PlanRedemption
        {
            get { return detailPlanRedemption; }
            set { detailPlanRedemption = value; }
        }

        DateTime detailPlanRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail3PlanRedemption", "i3;i5;i6")]
        [PropertyOrder(400)]
        public DateTime Detail3PlanRedemptionStart
        {
            get { return detailPlanRedemptionStart; }
            set { detailPlanRedemptionStart = value; }
        }

        DateTime detailPlanRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail3PlanRedemption", "i4;i5;i6")]
        [PropertyOrder(410)]
        public DateTime Detail3PlanRedemptionEnd
        {
            get { return detailPlanRedemptionEnd; }
            set { detailPlanRedemptionEnd = value; }
        }

        // факт погашения
        DetailWriteTypeEnum detailFactRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(420)]
        public DetailWriteTypeEnum Detail1FactRedemption
        {
            get { return detailFactRedemption; }
            set { detailFactRedemption = value; }
        }

        DateTime detailFactRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail1FactRedemption", "i3;i5;i6")]
        [PropertyOrder(430)]
        public DateTime Detail1FactRedemptionStart
        {
            get { return detailFactRedemptionStart; }
            set { detailFactRedemptionStart = value; }
        }

        DateTime detailFactRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail1FactRedemption", "i4;i5;i6")]
        [PropertyOrder(440)]
        public DateTime Detail1FactRedemptionEnd
        {
            get { return detailFactRedemptionEnd; }
            set { detailFactRedemptionEnd = value; }
        }

        // План обслуживания долга
        DetailWriteTypeEnum detailPlanService = DetailWriteTypeEnum.i1;
        [DisplayName("План обслуживания долга")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(450)]
        public DetailWriteTypeEnum Detail5PlanService
        {
            get { return detailPlanService; }
            set { detailPlanService = value; }
        }

        DateTime detailPlanServiceStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail5PlanService", "i3;i5;i6")]
        [PropertyOrder(460)]
        public DateTime Detail5PlanServiceStart
        {
            get { return detailPlanServiceStart; }
            set { detailPlanServiceStart = value; }
        }

        DateTime detailPlanServiceEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail5PlanService", "i4;i5;i6")]
        [PropertyOrder(470)]
        public DateTime Detail5PlanServiceEnd
        {
            get { return detailPlanServiceEnd; }
            set { detailPlanServiceEnd = value; }
        }

        // Факт погашения процентов
        DetailWriteTypeEnum detailFactPercentRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения процентов")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(480)]
        public DetailWriteTypeEnum Detail4FactPercentRedemption
        {
            get { return detailFactPercentRedemption; }
            set { detailFactPercentRedemption = value; }
        }

        DateTime detailFactPercentRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail4FactPercentRedemption", "i3;i5;i6")]
        [PropertyOrder(490)]
        public DateTime Detail4FactPercentRedemptionStart
        {
            get { return detailFactPercentRedemptionStart; }
            set { detailFactPercentRedemptionStart = value; }
        }

        DateTime detailFactPercentRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail4FactPercentRedemption", "i4;i5;i6")]
        [PropertyOrder(500)]
        public DateTime Detail4FactPercentRedemptionEnd
        {
            get { return detailFactPercentRedemptionEnd; }
            set { detailFactPercentRedemptionEnd = value; }
        }

        // Начисление пени по основному долгу
        DetailWriteTypeEnum detailPeniMainDebt = DetailWriteTypeEnum.i1;
        [DisplayName("Начисление пени по основному долгу")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(510)]
        public DetailWriteTypeEnum Detail8PeniMainDebt
        {
            get { return detailPeniMainDebt; }
            set { detailPeniMainDebt = value; }
        }

        DateTime detailPeniMainDebtStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail8PeniMainDebt", "i3;i5;i6")]
        [PropertyOrder(520)]
        public DateTime Detail8PeniMainDebtStart
        {
            get { return detailPeniMainDebtStart; }
            set { detailPeniMainDebtStart = value; }
        }

        DateTime detailPeniMainDebtEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail8PeniMainDebt", "i4;i5;i6")]
        [PropertyOrder(530)]
        public DateTime Detail8PeniMainDebtEnd
        {
            get { return detailPeniMainDebtEnd; }
            set { detailPeniMainDebtEnd = value; }
        }

        // Начисление пени по процентам
        DetailWriteTypeEnum detailPeniPercent = DetailWriteTypeEnum.i1;
        [DisplayName("Начисление пени по процентам")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(540)]
        public DetailWriteTypeEnum Detail11PeniPercent
        {
            get { return detailPeniPercent; }
            set { detailPeniPercent = value; }
        }

        DateTime detailPeniPercentStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail11PeniPercent", "i3;i5;i6")]
        [PropertyOrder(550)]
        public DateTime Detail11PeniPercentStart
        {
            get { return detailPeniPercentStart; }
            set { detailPeniPercentStart = value; }
        }

        DateTime detailPeniPercentEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail11PeniPercent", "i4;i5;i6")]
        [PropertyOrder(560)]
        public DateTime Detail11PeniPercentEnd
        {
            get { return detailPeniPercentEnd; }
            set { detailPeniPercentEnd = value; }
        }

        // Факт погашения пени по основному долгу
        DetailWriteTypeEnum detailFactMainDebtPeniRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения пени по основному долгу")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(570)]
        public DetailWriteTypeEnum Detail12FactMainDebtPeniRedemption
        {
            get { return detailFactMainDebtPeniRedemption; }
            set { detailFactMainDebtPeniRedemption = value; }
        }

        DateTime detailFactMainDebtPeniRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail12FactMainDebtPeniRedemption", "i3;i5;i6")]
        [PropertyOrder(580)]
        public DateTime Detail12FactMainDebtPeniRedemptionStart
        {
            get { return detailFactMainDebtPeniRedemptionStart; }
            set { detailFactMainDebtPeniRedemptionStart = value; }
        }

        DateTime detailFactMainDebtPeniRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail12FactMainDebtPeniRedemption", "i4;i5;i6")]
        [PropertyOrder(590)]
        public DateTime Detail12FactMainDebtPeniRedemptionEnd
        {
            get { return detailFactMainDebtPeniRedemptionEnd; }
            set { detailFactMainDebtPeniRedemptionEnd = value; }
        }

        // Факт погашения пени по процентам
        DetailWriteTypeEnum detailFactPercentPeniRedemption = DetailWriteTypeEnum.i1;
        [DisplayName("Факт погашения пени по процентам")]
        [Category(category2)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(600)]
        public DetailWriteTypeEnum Detail9FactPercentPeniRedemption
        {
            get { return detailFactPercentPeniRedemption; }
            set { detailFactPercentPeniRedemption = value; }
        }

        DateTime detailFactPercentPeniRedemptionStart;
        [DisplayName(startDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail9FactPercentPeniRedemption", "i3;i5;i6")]
        [PropertyOrder(610)]
        public DateTime Detail9FactPercentPeniRedemptionStart
        {
            get { return detailFactPercentPeniRedemptionStart; }
            set { detailFactPercentPeniRedemptionStart = value; }
        }

        DateTime detailFactPercentPeniRedemptionEnd;
        [DisplayName(endDateCaption)]
        [Category(category2)]
        [DynamicPropertyFilter("Detail9FactPercentPeniRedemption", "i4;i5;i6")]
        [PropertyOrder(620)]
        public DateTime Detail9FactPercentPeniRedemptionEnd
        {
            get { return detailFactPercentPeniRedemptionEnd; }
            set { detailFactPercentPeniRedemptionEnd = value; }
        }

        // формулы задолженности
        FactDebtMainFormulaEnum mainDebtFormula = FactDebtMainFormulaEnum.i1;
        [DisplayName("Фактическая задолженность по основному долгу")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(630)]
        public FactDebtMainFormulaEnum FormulaMainDebt
        {
            get { return mainDebtFormula; }
            set { mainDebtFormula = value; }
        }

        DateTime mainDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(640)]
        public DateTime FormulaMainDebt1Start
        {
            get { return mainDebtFormula1Start; }
            set { mainDebtFormula1Start = value; }
        }

        DateTime mainDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(650)]
        public DateTime FormulaMainDebt1End
        {
            get { return mainDebtFormula1End; }
            set { mainDebtFormula1End = value; }
        }

        DateTime mainDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(660)]
        public DateTime FormulaMainDebt2Start
        {
            get { return mainDebtFormula2Start; }
            set { mainDebtFormula2Start = value; }
        }

        DateTime mainDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaMainDebt", "i1;i2;i3")]
        [PropertyOrder(670)]
        public DateTime FormulaMainDebt2End
        {
            get { return mainDebtFormula2End; }
            set { mainDebtFormula2End = value; }
        }
        //
        FactDebtServiceFormulaEnum serviceDebtFormula = FactDebtServiceFormulaEnum.i1;
        [DisplayName("Фактическая задолженность по обслуживанию")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(680)]
        public FactDebtServiceFormulaEnum FormulaServiceDebt
        {
            get { return serviceDebtFormula; }
            set { serviceDebtFormula = value; }
        }

        DateTime serviceDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(690)]
        public DateTime FormulaServiceDebt1Start
        {
            get { return serviceDebtFormula1Start; }
            set { serviceDebtFormula1Start = value; }
        }

        DateTime serviceDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(700)]
        public DateTime FormulaServiceDebt1End
        {
            get { return serviceDebtFormula1End; }
            set { serviceDebtFormula1End = value; }
        }

        DateTime serviceDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(710)]
        public DateTime FormulaServiceDebt2Start
        {
            get { return serviceDebtFormula2Start; }
            set { serviceDebtFormula2Start = value; }
        }

        DateTime serviceDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaServiceDebt", "i1")]
        [PropertyOrder(720)]
        public DateTime FormulaServiceDebt2End
        {
            get { return serviceDebtFormula2End; }
            set { serviceDebtFormula2End = value; }
        }
        //
        FactDebtOverdatedFormulaEnum overdatedDebtFormula = FactDebtOverdatedFormulaEnum.i1;
        [DisplayName("Просроченная задолженность")]
        [Category(category3)]
        [TypeConverter(typeof(EnumTypeConverter))]
        [PropertyOrder(730)]
        public FactDebtOverdatedFormulaEnum FormulaOverdatedDebt
        {
            get { return overdatedDebtFormula; }
            set { overdatedDebtFormula = value; }
        }

        DateTime overdatedDebtFormula1Start;
        [DisplayName(space + "1 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(740)]
        public DateTime FormulaOverdatedDebt1Start
        {
            get { return overdatedDebtFormula1Start; }
            set { overdatedDebtFormula1Start = value; }
        }

        DateTime overdatedDebtFormula1End;
        [DisplayName(space + "1 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(750)]
        public DateTime FormulaOverdatedDebt1End
        {
            get { return overdatedDebtFormula1End; }
            set { overdatedDebtFormula1End = value; }
        }

        DateTime overdatedDebtFormula2Start;
        [DisplayName(space + "2 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(760)]
        public DateTime FormulaOverdatedDebt2Start
        {
            get { return overdatedDebtFormula2Start; }
            set { overdatedDebtFormula2Start = value; }
        }

        DateTime overdatedDebtFormula2End;
        [DisplayName(space + "2 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(770)]
        public DateTime FormulaOverdatedDebt2End
        {
            get { return overdatedDebtFormula2End; }
            set { overdatedDebtFormula2End = value; }
        }

        DateTime overdatedDebtFormula3Start;
        [DisplayName(space + "3 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(780)]
        public DateTime FormulaOverdatedDebt3Start
        {
            get { return overdatedDebtFormula3Start; }
            set { overdatedDebtFormula3Start = value; }
        }

        DateTime overdatedDebtFormula3End;
        [DisplayName(space + "3 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(790)]
        public DateTime FormulaOverdatedDebt3End
        {
            get { return overdatedDebtFormula3End; }
            set { overdatedDebtFormula3End = value; }
        }

        DateTime overdatedDebtFormula4Start;
        [DisplayName(space + "4 операнд " + startDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(800)]
        public DateTime FormulaOverdatedDebt4Start
        {
            get { return overdatedDebtFormula4Start; }
            set { overdatedDebtFormula4Start = value; }
        }

        DateTime overdatedDebtFormula4End;
        [DisplayName(space + "4 операнд " + endDateFormula)]
        [Category(category3)]
        [DynamicPropertyFilter("FormulaOverdatedDebt", "i1;i2;i3")]
        [PropertyOrder(810)]
        public DateTime FormulaOverdatedDebt4End
        {
            get { return overdatedDebtFormula4End; }
            set { overdatedDebtFormula4End = value; }
        }
    }

}