using System;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO41
{
    public class FO41Extension : IFO41Extension
    {
        public const int GroupTaxpayer = 4103;

        public const int GroupOGV = 4104;

        public const int GroupDF = 4105;

        public const int GroupOther = 4106;

        public const string OKTMOYar = /*"78 000 000"*/ "52 000 000";

        public const string OKTMOHMAO = "71 800 000";

        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearRepository;
        private readonly ILinqRepository<DataSources> sourceRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIV> oivRepository;
        private readonly ILinqRepository<D_Org_Privilege> orgRepository;
        private D_Org_Privilege responsOrg;

        public FO41Extension(
            IScheme scheme,
            IRepository<Users> userRepository,
            IRepository<FX_Date_YearDayUNV> yearRepository,
            ILinqRepository<DataSources> sourceRepository,
            ILinqRepository<D_OMSU_ResponsOIV> oivRepository,
            ILinqRepository<D_Org_Privilege> orgRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.yearRepository = yearRepository;
            this.sourceRepository = sourceRepository;
            this.oivRepository = oivRepository;
            this.orgRepository = orgRepository;

            CurrentYear = DateTime.Today.Year;
        }

        public int UserGroup { get; private set; }

        public int CurrentYear { get; private set; }

        public string OKTMO { get; private set; }

        public FX_Date_YearDayUNV CurrentYearUNV { get; private set; }

        public Users User { get; private set; }

        public D_OMSU_ResponsOIV ResponsOIV { get; private set; }

        public D_Org_Privilege ResponsOrg
        {
            get
            {
                return responsOrg == null ? null : orgRepository.FindOne(responsOrg.ID);
            }
        }

        public string GetEstReqTabId(int categoryId, int periodId, int estReqId)
        {
            return "'estReq_{0}'".FormatWith(estReqId > 0 ? estReqId.ToString() : "new_{0}_{1}".FormatWith(categoryId, periodId));
        }

        public string GetReqTabId(int categoryId, int periodId, int reqId)
        {
            return "'req_{0}'".FormatWith(reqId > 0 ? reqId.ToString() : "new_{0}_{1}".FormatWith(categoryId, periodId));
        }

        public int GetCurPeriod()
        {
            var today = DateTime.Today;
            if (OKTMO == OKTMOHMAO)
            {
                return ((today.Year * 10000) + 9990) + ((today.Month + 2) / 3);
            }

            return (today.Year * 10000) + 1;
        }

        public int GetPrevPeriod()
        {
            var today = DateTime.Today;
            if (OKTMO == OKTMOHMAO)
            {
                var curPeriod = GetCurPeriod();
                return curPeriod % 10 > 1 ? curPeriod - 1 : (curPeriod - 10000) + 3;
            }

            return ((today.Year - 1) * 10000) + 1;
        }

        public int GetPrevPeriod(int curPeriod)
        {
            if (OKTMO == OKTMOHMAO)
            {
                return curPeriod % 10 > 1 ? curPeriod - 1 : (curPeriod - 10000) + 3;
            }

            return curPeriod - 10000;
        }

        public bool IsReqLastPeriod(int periodId)
        {
            return periodId == GetPrevPeriod();
        }

        public string GetTextForPeriod(int periodId)
        {
            var period = periodId % 10000;
            var year = periodId / 10000;

            if (period == 1)
            {
                return "{0} год".FormatWith(year);
            }

            return period / 10 == 999
                       ? "{0} квартал {1} года".FormatWith(period % 10, year)
                       : "{0}.{1} {2} года".FormatWith(period % 100, period / 100, year);
        }
        
        public DataSources DataSource(int year)
        {
            return sourceRepository.FindAll()
                .FirstOrDefault(ds => ds.SupplierCode == "ФО" && ds.DataCode == 6 && ds.Year == year.ToString());
        }

        public bool Initialize()
        {
            try
            {
                var currentUserId = scheme.UsersManager.GetCurrentUserID();

                User = userRepository.Get(currentUserId);

                CurrentYearUNV = yearRepository.Get((CurrentYear * 10000) + 1);

                var list = oivRepository.FindAll().Where(x => x.UserID == currentUserId).ToList();

                ResponsOIV = list.FirstOrDefault();

                OKTMO = Convert.ToString(scheme.GlobalConstsManager.Consts["OKTMO"].Value);

                // if (OKTMO.Equals(OKTMOYar))
                if (!OKTMO.Equals(OKTMOHMAO))
                {
                    OKTMO = "52 000 000";

                    responsOrg = orgRepository.FindAll().FirstOrDefault(x => x.UserID != null && x.UserID == currentUserId);
                    if (responsOrg != null)
                    {
                        UserGroup = GroupTaxpayer;
                        ResponsOIV = null;
                    }
                    else
                    if (ResponsOIV == null)
                    {
                        UserGroup = GroupOther;
                    }
                    else
                    {
                        if (ResponsOIV.Role.Equals("ДФ"))
                        {
                            UserGroup = GroupDF;
                        }
                        else
                        {
                            UserGroup = ResponsOIV.Role.Equals("ОГВ") ? GroupOGV : GroupOther;
                        }
                    }
                }
                else if (OKTMO.Equals(OKTMOHMAO))
                {
                    responsOrg = orgRepository.FindAll().FirstOrDefault(x => x.UserID != null && x.UserID == currentUserId);
                    if (responsOrg != null)
                    {
                        UserGroup = GroupTaxpayer;
                        ResponsOIV = null;
                    }
                    else
                    {
                        ResponsOIV = oivRepository.FindAll().FirstOrDefault(x => x.UserID == currentUserId);
                        if (ResponsOIV != null)
                        {
                            UserGroup = GroupOGV;
                            responsOrg = null;
                        }
                        else
                        {
                            ResponsOIV = null;
                            responsOrg = null;
                            UserGroup = GroupOther;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка инициализации модуля: {0}", e.Message);

                return false;
            }
        }
    }
}