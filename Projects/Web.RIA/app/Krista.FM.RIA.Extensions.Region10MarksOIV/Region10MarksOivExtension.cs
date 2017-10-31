using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV
{
    public class Region10MarksOivExtension : IRegion10MarksOivExtension
    {
        private readonly IScheme scheme;
        private readonly IRepository<FX_Date_Year> yearRepository;
        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;
        private readonly ILinqRepository<D_Territory_RF> territoryRepository;
        private readonly ILinqRepository<DataSources> sourceRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIV> oivRepository;

        private int currentYearVal;

        public Region10MarksOivExtension(
            IScheme scheme,
            IRepository<FX_Date_Year> yearRepository,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_Territory_RF> territoryRepository,
            ILinqRepository<DataSources> sourceRepository,
            ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository,
            ILinqRepository<D_OMSU_ResponsOIV> oivRepository)
        {
            this.scheme = scheme;
            this.yearRepository = yearRepository;
            this.regionRepository = regionRepository;
            this.territoryRepository = territoryRepository;
            this.sourceRepository = sourceRepository;
            this.oivUserRepository = oivUserRepository;
            this.oivRepository = oivRepository;
        }

        public int CurrentYearVal
        {
            get
            {
                return currentYearVal;
            }

            set
            {
                try
                {
                    if (Years.Contains(value))
                    {
                        this.currentYearVal = value;
                        this.CurrentYear = GetRefYear(value);
                        this.DataSourceOiv = GetDatasourceOiv(value);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Нет источника с данным значением года");
                    }
                }
                catch (Exception)
                {
                    this.currentYearVal = 0;
                    this.CurrentYear = null;
                    throw;
                }
            }
        }

        public FX_Date_Year CurrentYear { get; private set; }

        public DataSources DataSourceOiv { get; private set; }

        public D_Territory_RF UserTerritoryRf { get; private set; }

        public D_OMSU_ResponsOIV UserResponseOiv { get; private set; }

        public D_Territory_RF RootTerritoryRf { get; private set; }
        
        public List<int> Years { get; private set; }
    
        public bool Initialize()
        {
            try
            {
                this.UserTerritoryRf = GetUserTerritory(((BasePrincipal)System.Web.HttpContext.Current.User).DbUser.RefRegion);

                this.RootTerritoryRf = GetRootTerritory();

                List<int> years = GetDatasourceOivYears();
                if (years.Count == 0)
                {
                    throw new Exception("Не найдено ни одного источника данных ОИВ");
                }

                years.Sort();
                this.Years = years;

                this.CurrentYearVal = years.Max();

                var responsOivUser = oivUserRepository.FindAll();
                var list = oivRepository.FindAll()
                                        .Where(x => responsOivUser.Where(p => p.RefUser == ((BasePrincipal)System.Web.HttpContext.Current.User).DbUser.ID)
                                                                  .Any(r => r.RefResponsOIV.ID == x.ID))
                                        .ToList();

                this.UserResponseOiv = list.FirstOrDefault();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                                  "Ошибка инициализации модуля ОИВ. Пользователь {0}. Ошибка: {1}", 
                                  System.Web.HttpContext.Current.User.Identity.Name,
                                  Diagnostics.KristaDiagnostics.ExpandException(e));
                return false;
            }
        }

        private FX_Date_Year GetRefYear(int year)
        {
            int id = year; ////формат id-шников для годовых записей: YYYY
            var result = yearRepository.Get(id);
            if (result == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0} не найдена.", year));
            }

            return result;
        }

        private DataSources GetDatasourceOiv(int year)
        {
            const int DatasourceCode = 10;
            const string DatasourceSupplierCode = "РЕГИОН";

            var result = sourceRepository.FindAll()
                                         .FirstOrDefault(x => x.SupplierCode == DatasourceSupplierCode 
                                                           && x.DataCode == DatasourceCode
                                                           && x.KindsOfParams == "1"
                                                           && x.Year == Convert.ToString(year)
                                                           && x.Deleted == "0");
            return result;
        }

        private List<int> GetDatasourceOivYears()
        {
            const int DatasourceCode = 10;
            const string DatasourceSupplierCode = "РЕГИОН";

            return sourceRepository.FindAll()
                                    .Where(x => x.SupplierCode == DatasourceSupplierCode 
                                                && x.DataCode == DatasourceCode
                                                && x.KindsOfParams == "1"
                                                && x.Deleted == "0")
                                    .Select(x => Convert.ToInt32(x.Year))
                                    .ToList();
        }

        /// <summary>
        /// Возвращает значение из Территории.РФ соответстующее району пользователя
        /// </summary>
        private D_Territory_RF GetUserTerritory(int? refRegion)
        {
            if (refRegion == null)
            {
                return null;
            }

            var userRegion = regionRepository.FindOne((int)refRegion);
            if (userRegion == null)
            {
                throw new Exception("Не найден район, назначенный пользователю - User.refRegion = {0}".FormatWith(refRegion));
            }

            if (userRegion.RefBridgeRegions.ID == -1)
            {
                throw new Exception("Не сопоставлен район пользователя! Районы.Анализ = {0}".FormatWith(userRegion.ID));
            }

            var result = territoryRepository.FindAll().Where(x => x.RefRegionsBridge == userRegion.RefBridgeRegions).ToList();

            if (result.Count == 0)
            {
                throw new Exception("Не найдена территория, сопоставленная району пользователя! Районы.Анализ = {0}".FormatWith(userRegion.ID));
            }
            else if (result.Count > 1)
            {
                throw new Exception("Найдено несколько территорий, сопоставленных району пользователя! Районы.Анализ = {0}".FormatWith(userRegion.ID));
            }

            return result.First();
        }

        /// <summary>
        /// Находит запись в Территории.РФ, соответстующую константе ОКТМО сервера
        /// </summary>
        private D_Territory_RF GetRootTerritory()
        {
            string oktmoStr = scheme.GlobalConstsManager.Consts["OKTMO"].Value.ToString();
            int oktmo = Convert.ToInt32(oktmoStr.Replace(" ", String.Empty));

            var territoryList = territoryRepository.FindAll().Where(r => r.Code == oktmo).ToList();

            if (territoryList.Count == 0)
            {
                throw new Exception("Не найдена территория с кодом ОКТМО = {0}".FormatWith(oktmoStr));
            }

            if (territoryList.Count == 1)
            {
                return territoryList.First();
            }

            foreach (var territory in territoryList)
            {
                if (IsRootTerritory(territory, territoryList))
                {
                    return territory;
                }
            }

            throw new Exception("Не найдена головная территория среди нескольких");
        }

        private bool IsRootTerritory(D_Territory_RF territory, List<D_Territory_RF> territoryList)
        {
            // Ищем родителя
            var parent = territoryRepository.FindAll().FirstOrDefault(x => x.ID == territory.ParentID);

            if (parent == null)
            {
                return true;
            }

            // Проверяем не входит ли найденный родитель в число кандитатов
            if (territoryList.Find(x => x.ID == parent.ID) != null)
            {
                return false;
            }

            return IsRootTerritory(parent, territoryList);
        }
    }
}
