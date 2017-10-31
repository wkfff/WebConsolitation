using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Domain.Reporitory.NHibernate;
using Krista.FM.Domain.Services.FinSourceDebtorBook;
using Krista.FM.ServerLibrary;
using DataSourceService = Krista.FM.Domain.Services.FinSourceDebtorBook.DataSourceService;
using RegionsAccordanceService = Krista.FM.Domain.Services.FinSourceDebtorBook.RegionsAccordanceService;
using VariantDescriptor = Krista.FM.Domain.Services.FinSourceDebtorBook.VariantDescriptor;
using VariantService = Krista.FM.Domain.Services.FinSourceDebtorBook.VariantService;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class DebtBookExtension : IDebtBookExtension
    {
        public const string CapitalUIKey = "Capital";
        public const string OrganizationCreditUIKey = "OrganizationCredit";
        public const string BudgetCreditUIKey = "BudgetCredit";
        public const string OtherCreditUIKey = "OtherCredit";
        public const string GuaranteeUIKey = "Guarantee";

        private readonly IScheme scheme;
        private readonly RegionsAccordanceService regionsAccordance;
        private readonly VariantService variantService;
        private readonly RegionsService regionsService;
        private readonly DataSourceService dataSourceService;
        
        private readonly ILinqRepository<T_S_ProtocolTransfer> protocolRepository;

        public DebtBookExtension(
                                  IScheme scheme, 
                                  RegionsAccordanceService regionsAccordance, 
                                  VariantService variantService, 
                                  RegionsService regionsService,
                                  DataSourceService dataSourceService, 
                                  ILinqRepository<T_S_ProtocolTransfer> protocolRepository)
        {
            this.scheme = scheme;
            this.regionsAccordance = regionsAccordance;
            this.variantService = variantService;
            this.regionsService = regionsService;
            this.dataSourceService = dataSourceService;

            this.protocolRepository = protocolRepository;
        }

        public enum DebtBookStatus : int
        {
            /// <summary>
            /// Вариант не определен
            /// </summary>
            Undefined = 0,

            /// <summary>
            /// Вариант на редактировании
            /// </summary>
            Edit = 1,

            /// <summary>
            /// Вариант на рассмотрении
            /// </summary>
            Review = 2,

            /// <summary>
            /// Вариант утвержден
            /// </summary>
            Approved = 3
        }

        public RegionsAccordanceService RegionsAccordance
        {
            get { return regionsAccordance; }
        }

        public VariantDescriptor Variant { get; set; }

        /// <summary>
        /// год источника, по которому записан текущий пользователь
        /// </summary>
        public int UserYear { get; private set; }

        public int CurrentSourceId { get; private set; }
        
        /// <summary>
        /// Источник данных "ФО_Анализ данных" соответствующий текущему году варианта.
        /// </summary>
        public int CurrentAnalysisSourceId { get; private set; }

        /// <summary>
        /// Регион текущего пользователя.
        /// </summary>
        public int UserRegionId { get; private set; }

        /// <summary>
        /// Наименование текущего региона пользователя.
        /// </summary>
        public string UserRegionName { get; private set; }
        
        /// <summary>
        /// Тип региона текущего пользователя.
        /// </summary>
        public UserRegionType UserRegionType { get; set; }

        /// <summary>
        /// Id субъекта по выбранному источнику данных "ФО_Анализ данных" из классификатора "Районы.Анализ".
        /// </summary>
        public int SubjectRegionId { get; private set; }

        /// <summary>
        /// Статус сбора, для текущего региона
        /// </summary>
        public T_S_ProtocolTransfer StatusSchb { get; set; }

        public bool HighlightColor 
        { 
            get
            {
                // если утвержден, нужно подсветить
                return StatusSchb.RefStatusSchb.ID == 3;
            }
        }

        /// <summary>
        /// Имя статуса сбора, для текущего региона
        /// </summary>
        public string StatusSchbText 
        { 
            get
            {
                if (StatusSchb == null)
                {
                    return "На редактировании МО";
                }

                return StatusSchb.RefStatusSchb.Name;
            }  
        }

        /// <summary>
        /// Инициализация модуля.
        /// </summary>
        public bool Initialize()
        {
            try
            {
                SetCurrentVariant(variantService.GetDefaultVariant());
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации Долговой книги: {0}", Diagnostics.KristaDiagnostics.ExpandException(e));
                return false;
            }
        }

        public void SetCurrentVariant(VariantDescriptor variant)
        {
            Variant = variant;
            CurrentSourceId = dataSourceService.GetDataSource(Variant.ActualYear);
            SetUserRegion();
            UserRegionType = regionsService.GetUserRegionType(UserRegionId);

            CurrentAnalysisSourceId = dataSourceService.GetDataSource(Variant.ActualYear, "ФО", 6, "Анализ данных", true);
            SubjectRegionId = regionsService.GetSubjectRegionId(CurrentAnalysisSourceId);
        }

        public bool CurrentVariantBlocked()
        {
            ////var repository = new NHibernateLinqRepository<T_S_ProtocolTransfer>();
            var status = this.protocolRepository.FindAll()
                .Where(x => x.RefVariant.ID == UserRegionId && x.RefRegion.ID == Variant.Id)
                .OrderByDescending(x => x.ID)
                .FirstOrDefault();

            if (status == null || status.RefStatusSchb.ID == (int)DebtBookStatus.Edit)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// установка региона текущего пользователя
        /// </summary>
        internal void SetUserRegion()
        {
            using (IDatabase db = scheme.SchemeDWH.DB)
            {
                var reporitory = new NHibernateLinqRepository<T_S_ProtocolTransfer>();
                var variantReporitory = new NHibernateLinqRepository<D_Variant_Schuldbuch>();
                var regionReporitory = new NHibernateLinqRepository<D_Regions_Analysis>();
                var statusSchbReporitory = new NHibernateLinqRepository<FX_S_StatusSchb>();

                Users user = new NHibernateRepository<Users>().Get(scheme.UsersManager.GetCurrentUserID());

                UserRegionId = (int)user.RefRegion;

                UserYear = regionsAccordance.GetRegionYear(UserRegionId, db);
            
                regionsAccordance.FillData(UserYear);

                if (Variant.ActualYear != UserYear && Variant.Id != -1)
                {
                    UserRegionId = regionsAccordance.GetRegionsByYear(Variant.ActualYear, UserRegionId, UserYear)[0];
                }
                else
                {    
                    UserRegionId = UserRegionId;
                }

                UserRegionName = regionReporitory.Get(UserRegionId).Name;

                var status = reporitory.FindAll()
                    .Where(x => x.RefVariant.ID == Variant.Id && x.RefRegion.ID == UserRegionId)
                    .OrderByDescending(x => x.ID)
                    .FirstOrDefault();

                if (status == null)
                {
                    StatusSchb = new T_S_ProtocolTransfer
                    {
                        RefStatusSchb = statusSchbReporitory.FindOne(1),
                        RefVariant = variantReporitory.FindOne(Variant.Id),
                        RefRegion = regionReporitory.FindOne(UserRegionId)
                    };
                }
                else
                {
                    StatusSchb = status;
                }
            }
        }

        internal int GetActualRegion(int region, IDatabase db)
        {
            UserYear = regionsAccordance.GetRegionYear(region, db);
            if (Variant.ActualYear != UserYear && Variant.Id != -1)
            {
                return regionsAccordance.GetRegionsByYear(Variant.ActualYear, region, UserYear)[0];
            }

            return -1;
        }
    }
}
