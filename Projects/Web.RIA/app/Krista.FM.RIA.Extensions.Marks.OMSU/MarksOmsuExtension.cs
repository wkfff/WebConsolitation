using System;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.MarksOMSU
{
    public class MarksOmsuExtension : IMarksOmsuExtension
    {
        private readonly IScheme scheme;
        private readonly IRepository<Users> userRepository;
        private readonly IRepository<FX_Date_YearDayUNV> yearDayUnvRepository;
        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;
        private readonly ILinqRepository<DataSources> sourceRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository;
        private readonly ILinqRepository<D_OMSU_ResponsOIV> oivRepository;

        private int currentYear;

        public MarksOmsuExtension(
            IScheme scheme,
            IRepository<Users> userRepository,
            IRepository<FX_Date_YearDayUNV> yearDayUnvRepository,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<DataSources> sourceRepository,
            ILinqRepository<D_OMSU_ResponsOIVUser> oivUserRepository,
            ILinqRepository<D_OMSU_ResponsOIV> oivRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.yearDayUnvRepository = yearDayUnvRepository;
            this.regionRepository = regionRepository;
            this.sourceRepository = sourceRepository;
            this.oivUserRepository = oivUserRepository;
            this.oivRepository = oivRepository;
        }

        public int CurrentYear
        {
            get
            {
                return currentYear;
            }

            set
            {
                try
                {
                    if (Years.Contains(value))
                    {
                        this.currentYear = value;
                        this.CurrentYearUNV = GetRefYear(value);
                        this.PreviousYearUNV = GetRefYear(value - 1);
                        this.DataSourceOmsu = GetDatasourceOmsu(value);
                        this.DataSourceOmsuPrevious = GetDatasourceOmsu(value - 1);
                        this.DataSourceRegion = GetDatasourceRegion(value);
                        this.UserRegionCurrent = GetUserRegionCurrentYear(User.RefRegion, value);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("Нет источника с данным значением года");
                    }
                }
                catch (Exception)
                {
                    this.currentYear = 0;
                    this.CurrentYearUNV = null;
                    this.UserRegionCurrent = null;
                    throw;
                }
            }
        }

        public FX_Date_YearDayUNV CurrentYearUNV { get; private set; }

        public FX_Date_YearDayUNV PreviousYearUNV { get; private set; }

        public Users User { get; private set; }

        public DataSources DataSourceOmsu { get; private set; }

        public DataSources DataSourceOmsuPrevious { get; private set; }

        public DataSources DataSourceRegion { get; private set; }

        public D_Regions_Analysis UserRegionCurrent { get; private set; }

        public D_OMSU_ResponsOIV ResponsOIV { get; private set; }

        public List<int> Years { get; private set; }
    
        public bool Initialize()
        {
            try
            {
                int currentUserId = scheme.UsersManager.GetCurrentUserID();

                User = userRepository.Get(currentUserId);

                List<int> years = GetDatasourceOmsuYears();
                if (years.Count == 0)
                {
                    throw new Exception("Не найдено ни одного источника данных ОМСУ");
                }

                years.Sort();
                Years = years;

                CurrentYear = years.Max();

                var responsOivUser = oivUserRepository.FindAll();

                var list = oivRepository.FindAll()
                    .Where(x => responsOivUser.Where(p => p.RefUser == User.ID).Any(r => r.RefResponsOIV.ID == x.ID)).ToList();

                ResponsOIV = list.FirstOrDefault();

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(
                                  "Ошибка инициализации модуля ОМСУ. Пользователь {0}. Ошибка: {1}", 
                                  System.Web.HttpContext.Current.User.Identity.Name,
                                  Diagnostics.KristaDiagnostics.ExpandException(e));
                return false;
            }
        }

        private FX_Date_YearDayUNV GetRefYear(int year)
        {
            int id = (year * 10000) + 1; ////формат id-шников для годовых записей: YYYY0001
            var result = yearDayUnvRepository.Get(id);
            if (result == null)
            {
                throw new KeyNotFoundException(String.Format("Запись с годом {0} не найдена.", year));
            }

            return result;
        }

        private DataSources GetDatasourceOmsu(int year)
        {
            const int DatasourceCode = 16;
            const string DatasourceSupplierCode = "РЕГИОН";

            var result = sourceRepository.FindAll()
                                         .FirstOrDefault(x => x.SupplierCode == DatasourceSupplierCode 
                                                           && x.DataCode == DatasourceCode 
                                                           && x.Year == Convert.ToString(year)
                                                           && x.Deleted == "0");
            return result;
        }

        private List<int> GetDatasourceOmsuYears()
        {
            const int DatasourceCode = 16;
            const string DatasourceSupplierCode = "РЕГИОН";

            return sourceRepository.FindAll()
                                    .Where(x => x.SupplierCode == DatasourceSupplierCode 
                                                && x.DataCode == DatasourceCode
                                                && x.Deleted == "0")
                                    .Select(x => Convert.ToInt32(x.Year))
                                    .ToList();
        }

        private DataSources GetDatasourceRegion(int year)
        {
            const int DatasourceCode = 6;
            const string DatasourceSupplierCode = "ФО";
            var result = sourceRepository.FindAll()
                                         .FirstOrDefault(x => x.SupplierCode == DatasourceSupplierCode 
                                                              && x.DataCode == DatasourceCode 
                                                              && x.Year == Convert.ToString(year)
                                                              && x.Deleted == "0");
            return result;
        }

        /// <summary>
        /// Возвращает значение из Районы.Анализ соответстующее району пользователя в разрезе выбранного года(источника)
        /// </summary>
        private D_Regions_Analysis GetUserRegionCurrentYear(int? refRegion, int year)
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

            var datasourceRegion = GetDatasourceRegion(year);
            if (datasourceRegion == null)
            {
                throw new Exception("Не найден источник для Районы.Анализ год = {0}".FormatWith(year));
            }
            
            // Если пользователь того же года, то сопоставление не нужно
            if (userRegion.SourceID == datasourceRegion.ID)
            {
                return userRegion;
            }

            // Необходимо найти соответстующего пользователя в нужном году по сопоставимому классификатору
            var brigeRegion = userRegion.RefBridgeRegions;
            if (brigeRegion == null || brigeRegion.ID == -1)
            {
                throw new Exception("Район, назначенный пользователю, не имеет сопоставления в сопоставимом классификаторе - User.refRegion = {0}".FormatWith(refRegion));
            }

            var result = regionRepository.FindAll()
                                         .Where(r => r.SourceID == datasourceRegion.ID
                                                     && r.RefBridgeRegions == brigeRegion)
                                         .ToList();
            if (result.Count == 0)
            {
                throw new Exception("Не найден сопоставленный район в искомом году! Год = {0}, Районы.Сопоставимый = {1}".FormatWith(year, brigeRegion.ID));
            }
            else if (result.Count > 1)
            {
                throw new Exception("В искомом году несколько районов, сопоставленных данному! Год = {0}, Районы.Сопоставимый = {1}".FormatWith(year, brigeRegion.ID));
            }

            return result.First();
        }
    }
}
