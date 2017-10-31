using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Krista.FM.Server.WebServices
{
    [XmlTypeAttribute]
    public class QueryByMonth
    {
        [XmlAttribute()]
        private int yearField;
        [XmlAttribute()]
        private int monthField;
        [XmlAttribute()]
        private int budgetLevelField;
        [XmlAttribute()]
        private int classifierType;
        [XmlAttribute()]
        private string municipalCodeField;

        [XmlArrayItem]
        private string[] classifierCodesField;

        public int Year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        public int Month
        {
            get
            {
                return this.monthField;
            }
            set
            {
                this.monthField = value;
            }
        }


        public int BudgetLevel
        {
            get
            {
                return this.budgetLevelField;
            }
            set
            {
                this.budgetLevelField = value;
            }
        }

        public int ClassifierType
        {
            get
            {
                return classifierType;
            }
            set
            {
                classifierType = value;
            }
        }

        public string MunicipalCode
        {
            get
            {
                return this.municipalCodeField;
            }
            set
            {
                this.municipalCodeField = value;
            }
        }

        public string[] ClassifierCodes
        {
            get
            {
                return this.classifierCodesField;
            }
            set
            {
                this.classifierCodesField = value;
            }
        }
    }

    [XmlTypeAttribute("ReportByMonth")]
    public class ReportByMonth
    {
        [XmlAttribute("ClassifierCodeField")]
        private string classifierCodeField;

        [XmlAttribute("ClassifierTypeField")]
        private int classifierTypeField;

        [XmlAttribute("PlanForYearField")]
        private double planForYearField;

        [XmlAttribute("FactForMonthField")]
        private double factForMonthField;

        [XmlAttribute("FactForMonthPrevYearField")]
        private double factForMonthPrevYearField;

        [XmlAttribute("ImplementPercentField")]
        private double implementPercentField;

        [XmlAttribute("IncreaseRateField")]
        private double increaseRateField;

        /// <remarks/>
        public string classifierCode
        {
            get
            {
                return this.classifierCodeField;
            }
            set
            {
                this.classifierCodeField = value;
            }
        }

        /// <remarks/>
        public int classifierType
        {
            get
            {
                return this.classifierTypeField;
            }
            set
            {
                this.classifierTypeField = value;
            }
        }

        /// <remarks/>
        public double planForYear
        {
            get
            {
                return this.planForYearField;
            }
            set
            {
                this.planForYearField = value;
            }
        }

        /// <remarks/>
        public double factForMonth
        {
            get
            {
                return this.factForMonthField;
            }
            set
            {
                this.factForMonthField = value;
            }
        }

        /// <remarks/>
        public double factForMonthPrevYear
        {
            get
            {
                return this.factForMonthPrevYearField;
            }
            set
            {
                this.factForMonthPrevYearField = value;
            }
        }

        /// <remarks/>
        public double implementPercent
        {
            get
            {
                return this.implementPercentField;
            }
            set
            {
                this.implementPercentField = value;
            }
        }

        /// <remarks/>
        public double increaseRate
        {
            get
            {
                return this.increaseRateField;
            }
            set
            {
                this.increaseRateField = value;
            }
        }
    }

    [XmlTypeAttribute("BudgetSummary")]
    public partial class BudgetSummary
    {
        [XmlAttribute("PlanValueField")]
        private double planValueField;

        [XmlAttribute("FactValueField")]
        private double factValueField;

        public double planValue
        {
            get
            {
                return this.planValueField;
            }
            set
            {
                this.planValueField = value;
            }
        }

        public double factValue
        {
            get
            {
                return this.factValueField;
            }
            set
            {
                this.factValueField = value;
            }
        }
    }

    [XmlTypeAttribute("BudgetIndicator")]
    public partial class BudgetIndicator
    {
        [XmlAttribute("ClassifierCodeField")]
        private string classifierCodeField;
        [XmlAttribute("ClassifierTypeField")]
        private int classifierTypeField;
        [XmlAttribute("PlanValueField")]
        private double planValueField;
        [XmlAttribute("FactValueField")]
        private double factValueField;

        public string classifierCode
        {
            get
            {
                return this.classifierCodeField;
            }
            set
            {
                this.classifierCodeField = value;
            }
        }

        public int classifierType
        {
            get
            {
                return this.classifierTypeField;
            }
            set
            {
                this.classifierTypeField = value;
            }
        }

        public double planValue
        {
            get
            {
                return this.planValueField;
            }
            set
            {
                this.planValueField = value;
            }
        }

        public double factValue
        {
            get
            {
                return this.factValueField;
            }
            set
            {
                this.factValueField = value;
            }
        }
    }


    [XmlTypeAttribute]
    public partial class ReportByYear
    {
        [XmlArrayItem]
        private BudgetIndicator[] budgetStats;

        [XmlAttribute]
        private BudgetSummary budgetTotalField;

        public BudgetIndicator[] BudgetStats
        {
            get
            {
                return budgetStats;
            }
            set
            {
                budgetStats = value;
            }
        }

        public BudgetSummary budgetTotal
        {
            get
            {
                return budgetTotalField;
            }
            set
            {
                budgetTotalField = value;
            }
        }
    }

    [XmlTypeAttribute()]
    public partial class QueryByYear
    {
        [XmlAttribute()]
        private int yearField;

        [XmlAttribute()]
        private int budgetLevelField;

        [XmlAttribute()]
        private string municipalCodeField;

        [XmlAttribute()]
        private int classifierTypeField;

        [XmlArrayItem]
        private string[] classifierCodesField;

        public int Year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        public int BudgetLevel
        {
            get
            {
                return this.budgetLevelField;
            }
            set
            {
                this.budgetLevelField = value;
            }
        }

        public string MunicipalCode
        {
            get
            {
                return this.municipalCodeField;
            }
            set
            {
                this.municipalCodeField = value;
            }
        }

        public int ClassifierType
        {
            get
            {
                return classifierTypeField;
            }
            set
            {
                classifierTypeField = value;
            }
        }

        public string[] ClassifierCodes
        {
            get
            {
                return this.classifierCodesField;
            }
            set
            {
                this.classifierCodesField = value;
            }
        }
    }
}
