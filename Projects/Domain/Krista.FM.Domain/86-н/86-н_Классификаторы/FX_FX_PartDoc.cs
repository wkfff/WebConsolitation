namespace Krista.FM.Domain
{
    public class FX_FX_PartDoc : ClassifierTable
    {
        /// <summary>
        /// Документ не указан
        /// </summary>
        public const int NoValueDocTypeID = 0;

        /// <summary>
        /// Паспорт учреждения
        /// </summary>
        public const int PassportDocTypeID = 1;

        /// <summary>
        /// Государственное (муниципальное) задание
        /// </summary>
        public const int StateTaskDocTypeID = 2;

        /// <summary>
        /// План ФХД
        /// </summary>
        public const int PfhdDocTypeID = 3;

        /// <summary>
        /// Отчет об исполнении смет доходов и расходов по приносящей доход деятельности (ф. 0503137)
        /// </summary>
        public const int AnnualBalanceF0503137Type = 4;

        /// <summary>
        /// Информация о бюджетной смете
        /// </summary>
        public const int SmetaDocTypeID = 5;

        /// <summary>
        /// Информация о результатах деятельности и об использовании имущества
        /// </summary>
        public const int ResultsOfActivityDocTypeID = 6;

        /// <summary>
        /// Информация о контрольных мероприятиях 
        /// </summary>
        public const int InfAboutControlActionsDocTypeID = 7;

        /// <summary>
        /// Баланс государственного (муниципального) учреждения (ф. 0503730)
        /// </summary>
        public const int AnnualBalanceF0503730Type = 9;

        /// <summary>
        /// Отчет о финансовых результатах деятельности (ф. 0503721)
        /// </summary>
        public const int AnnualBalanceF0503721Type = 10;

        /// <summary>
        /// Отчет об исполнении учреждением плана ФХД (ф. 0503737)
        /// </summary>
        public const int AnnualBalanceF0503737Type = 11;

        /// <summary>
        /// Баланс (ф. 0503130)
        /// </summary>
        public const int AnnualBalanceF0503130Type = 12;

        /// <summary>
        /// Отчет о финансовых результатах деятельности (ф. 0503121)
        /// </summary>
        public const int AnnualBalanceF0503121Type = 13;

        /// <summary>
        /// Отчет об исполнении бюджета (ф. 0503127)
        /// </summary>
        public const int AnnualBalanceF0503127Type = 14;

        /// <summary>
        /// Иная информация об учреждении
        /// </summary>
        public const int DiverseInfo = 15;

        public static readonly string Key = "4d74d48a-0175-42fb-8301-a638d24451d8";

        public virtual int RowType { get; set; }

        public virtual string Name { get; set; }

        public virtual string Url { get; set; }
    }
}
