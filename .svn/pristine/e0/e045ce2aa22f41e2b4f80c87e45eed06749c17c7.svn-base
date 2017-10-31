using System;
using System.Diagnostics;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.OrgGKH
{
    public class OrgGKHExtension : IOrgGkhExtension
    {
        private readonly IScheme scheme;
        private readonly ILinqRepository<Users> userRepository;
        private readonly ILinqRepository<FX_Date_YearDayUNV> yearRepository;
        private readonly ILinqRepository<DataSources> sourceRepository;
        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;
        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;

        public OrgGKHExtension(
            IScheme scheme,
            ILinqRepository<Users> userRepository,
            ILinqRepository<FX_Date_YearDayUNV> yearRepository,
            ILinqRepository<DataSources> sourceRepository,
            ILinqRepository<D_Regions_Analysis> regionRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository)
        {
            this.scheme = scheme;
            this.userRepository = userRepository;
            this.yearRepository = yearRepository;
            this.sourceRepository = sourceRepository;
            this.regionRepository = regionRepository;
            this.orgRepository = orgRepository;
            CurrentYear = DateTime.Today.Year;
        }

        public static int GroupMo
        {
            get { return 3; }
        }

        public static int GroupAudit
        {
            get { return 4; }
        }

        public static int GroupOrg
        {
            get { return 5; }
        }

        public static int GroupIOGV
        {
            get { return 6; }
        }

        public int CurrentYear { get; private set; }

        public FX_Date_YearDayUNV CurrentYearUNV { get; private set; }

        public Users User { get; private set; }

        public D_Regions_Analysis Region { get; private set; }

        public int UserGroup { get; private set; }

        public DataSources DataSource { get; private set; }

        public DataSources RegionSource { get; private set; }

        public bool Initialize()
        {
            try
            {
                var currentUserId = scheme.UsersManager.GetCurrentUserID();
                User = userRepository.FindOne(currentUserId);
                CurrentYearUNV = yearRepository.FindOne((CurrentYear * 10000) + 1);
                Region = User.RefRegion != null ? regionRepository.FindOne((int)User.RefRegion) : null;

                // Источник: ОРГАНИЗАЦИИ\0012 Сбор по ЖКХ
                DataSource = sourceRepository.FindAll()
                    .FirstOrDefault(ds => ds.SupplierCode == "ОРГАНИЗАЦИИ" && ds.DataCode == 12);

                RegionSource = sourceRepository.FindAll()
                    .FirstOrDefault(ds => ds.SupplierCode == "ФО" && ds.DataCode == 6 && ds.Year == "2011");

                UserGroup = -1;
                var groups = scheme.UsersManager.GetGroupsForUser(currentUserId);

                foreach (System.Data.DataRow row in groups.Rows)
                {
                    if (row.ItemArray[2].Equals(true))
                    {
                        var groupName = row.ItemArray[1].ToString();
                        if (groupName.Equals(OrgGKHConsts.GroupMOName))
                        {
                            UserGroup = GroupMo;
                        }
                        else
                        {
                            if (groupName.Equals(OrgGKHConsts.GroupAuditName))
                            {
                                UserGroup = GroupAudit;
                            }
                            else
                            {
                                if (groupName.Equals(OrgGKHConsts.GroupIOGVName))
                                {
                                    UserGroup = GroupIOGV;
                                }
                                else
                                {
                                    if (groupName.Equals(OrgGKHConsts.GroupOrgName) &&
                                        orgRepository.FindAll()
                                            .Count(x => x.Login != null && x.Login.Equals(User.Name)) > 0)
                                    {
                                        UserGroup = GroupOrg;
                                    }
                                }
                            }
                        }
                    }
                }

                // проверка - организация ли
                if (UserGroup == -1)
                {
                    if (orgRepository.FindAll()
                        .Count(x => x.Login != null && x.Login.Equals(User.Name)) > 0)
                    {
                        UserGroup = GroupOrg;
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