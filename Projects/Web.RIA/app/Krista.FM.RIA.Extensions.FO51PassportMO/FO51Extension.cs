using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.FO51PassportMO
{
    public class FO51Extension : IFO51Extension
    {
        public const int GroupMo = 5103;

        public const int GroupOGV = 5104;

        public const int GroupOther = 5105;

        public const int RegionsMR = -4;

        public const int RegionsGO = -7;

        public const int RegionsAll = -74;

        public const int StateEdit = 1;

        public const int StateConsider = 2;

        public const int StateAccept = 3;

        public const int RegionFictID = 278;

        public const string OKTMOHMAO = "71 800 000";

        private readonly IScheme scheme;
        
        private readonly ILinqRepository<Users> userRepository;

        private readonly ILinqRepository<D_OMSU_ResponsOIV> oivRepository;

        private readonly ILinqRepository<D_Marks_PassportMO> marksPassportRepository;

        private readonly ILinqRepository<DataSources> sourceRepository;

        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;

        private IList<DataSources> sourcesForRegion;
        private IList<D_Regions_Analysis> regionsAll;

        public FO51Extension(
            IScheme scheme,
            ILinqRepository<Users> userRepository,
            ILinqRepository<D_OMSU_ResponsOIV> oivRepository,
            ILinqRepository<D_Marks_PassportMO> marksPassportRepository,
            ILinqRepository<DataSources> sourceRepository,
            ILinqRepository<D_Regions_Analysis> regionRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.oivRepository = oivRepository;
            this.marksPassportRepository = marksPassportRepository;
            this.sourceRepository = sourceRepository;
            this.regionRepository = regionRepository;
            ResponsOIV = null;
            MarkOIV = null;
        }

        public Users User { get; private set; }

        public int UserGroup { get; private set; }

        public D_OMSU_ResponsOIV ResponsOIV { get; private set; }

        public D_Marks_PassportMO MarkOIV { get; private set; }

        public List<DataSources> DataSourcesFO51 { get; private set; }

        public string OKTMO { get; private set; }

        public bool Initialize()
        {
            try
            {
                OKTMO = Convert.ToString(scheme.GlobalConstsManager.Consts["OKTMO"].Value);

                var currentUserId = scheme.UsersManager.GetCurrentUserID();
                User = userRepository.FindOne(currentUserId);
                DataSourcesFO51 = sourceRepository.FindAll().Where(x => x.SupplierCode.Equals("ФО") && x.DataCode == 51).ToList();
                sourcesForRegion = sourceRepository.FindAll().Where(x => x.SupplierCode.Equals("ФО") && x.DataCode == 6).ToList();
                regionsAll = regionRepository.FindAll().ToList();

                if (User.RefRegion != null)
                {
                    UserGroup = GroupMo;
                }
                else
                {
                    ResponsOIV = oivRepository.FindAll().FirstOrDefault(x => x.UserID == User.ID);
                    if (ResponsOIV == null)
                    {
                        UserGroup = GroupOther;
                    }
                    else
                    {
                        UserGroup = GroupOGV;
                        MarkOIV =
                            marksPassportRepository.FindAll().FirstOrDefault(x => x.RefOGV.ID == ResponsOIV.ID);
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

        public D_Regions_Analysis GetActualRegion(int periodId, int regionId)
        {
            if (regionId == RegionsMR || regionId == RegionsGO || regionId == RegionsAll)
            {
                return new D_Regions_Analysis { ID = regionId, Name = regionId == RegionsMR ? "Все МР" : (regionId == RegionsGO ? "Все ГО" : "Все ГО и МР") };
            }

            var region = regionsAll.FirstOrDefault(x => x.ID == regionId);
            if (region == null)
            {
                return null;
            }

            var bridgeId = region.RefBridgeRegions.ID;
            var year = (periodId / 10000).ToString();
            var source = sourcesForRegion.FirstOrDefault(x => x.Year.Equals(year));
            return source != null
                ? regionsAll.FirstOrDefault(x => x.RefBridgeRegions.ID == bridgeId && x.SourceID == source.ID) 
                : null;
        }

        public D_Regions_Analysis GetRegionByBridge(int periodId, int regionBridgeId)
        {
            var year = (periodId / 10000).ToString();
            var source = sourcesForRegion.FirstOrDefault(x => x.Year.Equals(year));
            return source != null
                ? regionsAll.FirstOrDefault(x => x.RefBridgeRegions.ID == regionBridgeId && x.SourceID == source.ID)
                : null;
        }

        public List<D_Regions_Analysis> GetRegions(int periodId)
        {
            var year = (periodId / 10000).ToString();
            var sources = sourceRepository.FindAll();
            var sourceRegion = sources.FirstOrDefault(x =>
                                                      x.Year.Equals(year) &&
                                                      x.SupplierCode.Equals("ФО") &&
                                                      x.DataName.Equals("Анализ данных"));
            return (sourceRegion == null)
                       ? null
                       : regionRepository.FindAll()
                             .Where(x => (x.RefTerr.ID == 4 || x.RefTerr.ID == 7) && x.SourceID == sourceRegion.ID)
                             .OrderBy(x => x.Code).ToList();
        }

        /// <summary>
        /// Ссылка на сводный отчет на сайте для ОГВ
        /// </summary>
        public string GetSvodReportUrl()
        {
            return OKTMO == OKTMOHMAO
                ? "http://www.monitoring.admhmao.ru/Site/reports/FO_0002_0056_02/Default.aspx" 
                : null;
        }

        /// <summary>
        /// Ссылка на сводный отчет на сайте для МО
        /// </summary>
        public string GetReportForRegionsUrl()
        {
            return OKTMO == OKTMOHMAO
                ? "http://www.monitoring.admhmao.ru/Site/reports/FO_0002_0056_03/Default.aspx"
                : null;
        }
    }
}