using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.OrgGKH.Params;
using Krista.FM.RIA.Extensions.OrgGKH.Presentation.Views;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.OrgGKH.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с организациями
    /// </summary>
    public class OrganizationController : SchemeBoundController
    {
        /// <summary>
        /// Репозиторий глобальных параметров
        /// </summary>
        private readonly IOrgGkhExtension extension;

        private readonly ILinqRepository<D_OK_OKOPF> okopfRepository;

        private readonly ILinqRepository<D_OK_OKFS> okfsRepository;

        private readonly ILinqRepository<D_Org_TypeOrg> typeOrgRepository;

        private readonly ILinqRepository<D_Org_RegistrOrg> orgRepository;

        private readonly ILinqRepository<D_Regions_Analysis> regionsRepository;

        private readonly IScheme scheme;

        public OrganizationController(
            IScheme scheme,
            IOrgGkhExtension extension,
            ILinqRepository<D_OK_OKOPF> okopfRepository,
            ILinqRepository<D_OK_OKFS> okfsRepository,
            ILinqRepository<D_Org_TypeOrg> typeOrgRepository,
            ILinqRepository<D_Org_RegistrOrg> orgRepository,
            ILinqRepository<D_Regions_Analysis> regionsRepository)
        {
            this.scheme = scheme;
            this.extension = extension;
            this.okopfRepository = okopfRepository;
            this.okfsRepository = okfsRepository;
            this.typeOrgRepository = typeOrgRepository;
            this.orgRepository = orgRepository;
            this.regionsRepository = regionsRepository;
        }

        /// <summary>
        /// Обработчик на открытие формы для добавления новой организации
        /// </summary>
        /// <returns>Представление с формой добавления новой  организации</returns>
        public ActionResult Book(int? orgId, int? regionId)
        {
            var curRegionId = (extension.Region == null) ? regionId : extension.Region.ID;
            
            var regionsKv = new List<KeyValuePair<int, string>>();

            if (curRegionId != null)
            {
                var regions = regionsRepository.FindAll().Where(x => x.ParentID == curRegionId);
                regionsKv = regions.Select(region => new KeyValuePair<int, string>(region.ID, region.Name)).ToList();
                var newValuPair = new KeyValuePair<int, string>(
                    (int)curRegionId,
                    regionsRepository.FindOne((int)curRegionId).Name);
                regionsKv.Add(newValuPair);
            }

            var orgForm = new OrgFormView(extension, orgRepository, curRegionId ?? -1)
                              {
                                  OrgID = orgId, 
                                  Regions = regionsKv
                              };

            return View("~/App_Resource/Krista.FM.RIA.Core.Controllers.dll/Krista.FM.RIA.Core.Controllers/Views/View.aspx", orgForm);
        }

        /// <summary>
        /// Сохранение организации
        /// </summary>
        /// <param name="orgId">Идентификатор организации</param>
        /// <param name="values">Реквизиты организации</param>
        /// <returns>Результат сохранения организации</returns>
        [HttpPost]
        [Transaction]
        public AjaxStoreResult Save(int? orgId, FormCollection values)
        {
            var result = new AjaxStoreResult();
            try
            {
                var inn = values["INN"].ToUpper();

                var regionId = Convert.ToInt32(values["Region_Value"]);

                // проверка, есть ли в базе организация с таким ИНН и МО
                var orgWithEnteredInn = orgRepository.FindAll().FirstOrDefault(x => 
                    x.INN != null && 
                    x.INN.ToUpper().Equals(inn) && 
                    x.RefRegionAn.ID == regionId);

                var curOrg = orgId == null ? null : orgRepository.FindOne((int)orgId);

                var oldLogin = (curOrg == null)
                                   ? string.Empty
                                   : curOrg.Login;

                if (orgWithEnteredInn != null)
                {
                    if ((orgId == null) || (orgWithEnteredInn.ID != orgId))
                    {
                        result.ResponseFormat = StoreResponseFormat.Save;
                        result.SaveResponse.Success = false;
                        result.SaveResponse.Message = 
                            "Организация с такими значениями полей ИНН и МО уже зарегистрирована";

                        return result;
                    }
                }

                var users = scheme.UsersManager.GetUsers();

                var newLogin = inn;
                
                if (User.IsInRole(OrgGKHConsts.GroupAuditName) && 
                    !(values["Login"] == null || values["Login"].IsEmpty()))
                {
                    newLogin = values["Login"];
                }

                // проверка, есть ли пользователь с логином Name в базе
                if (oldLogin != newLogin && users.Rows.Cast<DataRow>().Any(existsUser => existsUser["Name"].ToString().Equals(newLogin)))
                {
                    if (User.IsInRole(OrgGKHConsts.GroupMOName)) 
                    {
                        if (
                            users.Rows.Cast<DataRow>().Any(
                                existsUser => existsUser["Name"].ToString().Equals(inn + extension.User.Name)))
                        {
                            result.ResponseFormat = StoreResponseFormat.Save;
                            result.SaveResponse.Success = false;
                            result.SaveResponse.Message =
                                "Организация с логином {0} и {1} уже зарегистрирована.".
                                FormatWith(inn, inn + extension.User.Name);

                            return result;
                        }
                    }
                    else
                    {
                        if (User.IsInRole(OrgGKHConsts.GroupAuditName))
                        {
                            result.ResponseFormat = StoreResponseFormat.Save;
                            result.SaveResponse.Success = false;
                            result.SaveResponse.Message = 
                                "Организация с логином '{0}' уже зарегистрирована. Скорректируйте поле «Логин»"
                                .FormatWith(newLogin);

                            return result;
                        }
                    }

                    newLogin = inn + extension.User.Name;
                }

                // сохранение организации
                var org = SaveOrganization(values, inn, orgId, curOrg, newLogin);

                // добавление пользователя для организации
                if (orgId == null)
                {
                    CreateUser(org.Login, org.NameOrg, org.RefRegionAn.ID);
                }
                else
                {
                    if (org != null)
                    {
                        EditUser(oldLogin, org.Login, org.NameOrg, org.RefRegionAn.ID);
                    }
                }

                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = true;
                result.SaveResponse.Message = org == null ? "null" : org.ID.ToString();
                result.Data = values;

                return result;
            }
            catch (Exception e)
            {
                result.ResponseFormat = StoreResponseFormat.Save;
                result.SaveResponse.Success = false;
                result.SaveResponse.Message = e.Message;

                return result;
            }
        }

        public ActionResult LookupRegions(int? regionId)
        {
            try
            {
                var curRegionId = (extension.Region == null) ? regionId : extension.Region.ID;
                if (curRegionId != null)
                {
                    var regionsAll = regionsRepository.FindAll();
                    var regions = regionsAll.Where(x => x.ParentID == curRegionId).OrderBy(x => x.Name).ToList();
                    regions.Add(regionsRepository.FindOne((int)curRegionId));

                    var data = from f in regions
                               select new
                                          {
                                              f.ID,
                                              f.Name
                                          };

                    data = data.ToList();
                    return new AjaxStoreResult(data, data.Count());
                }

                return new AjaxStoreResult(new List<int>(), 0);
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        /// <summary>
        /// Удаление пользователя с заданным логином
        /// </summary>
        /// <param name="login">Логин организации</param>
        /// <returns>Результат удаления пользователя</returns>
        public AjaxStoreResult DeleteUser(string login)
        {
            var result = new AjaxStoreResult { ResponseFormat = StoreResponseFormat.Save };
         
            if (login == null || login.Equals(string.Empty))
            {
                return result;
            }

            var users = scheme.UsersManager.GetUsers();
            foreach (DataRow existsUser in users.Rows)
            {
                if (existsUser["Name"].ToString().Equals(login))
                {
                    existsUser.Delete();
                }
            }

            scheme.UsersManager.ApplayUsersChanges(users.GetChanges());
            
            return result;
        }

        private D_Org_RegistrOrg SaveOrganization(
            FormCollection values, 
            string inn, 
            int? orgId, 
            D_Org_RegistrOrg curOrg, 
            string newLogin)
        {
            D_Org_RegistrOrg org = null;

            if (orgId == null)
            {
                var regionId = Convert.ToInt32(values["Region_Value"]);
                var login = newLogin;

                if (values["Login"] != null && values["Login"].IsNotEmpty())
                {
                    login = values["Login"];
                }

                var lastOrgByCode = orgRepository.FindAll().OrderByDescending(x => x.Code).FirstOrDefault();
                var code = (lastOrgByCode == null) ? 10 : lastOrgByCode.Code + 1;

                org = new D_Org_RegistrOrg
                {
                    NameOrg = values["NameOrg"],
                    ShName = values["ShName"],
                    LegalAddress = values["LegalAddress"],
                    FactAddress = values["FactAddress"],
                    Phone = values["Phone"],
                    RefRegionAn = regionsRepository.FindOne(regionId),
                    RowType = 0,
                    Code = code,
                    RefOK = okopfRepository.FindOne(-1),
                    RefOKOKFS = okfsRepository.FindOne(-1),
                    RefOrg = typeOrgRepository.FindOne(-1),
                    DatRegistr = DateTime.Today,
                    INN = inn,
                    Login = login,
                    KPP = values["KPP"]
                };

                orgRepository.Save(org);
            }
            else
            {
                if (curOrg != null)
                {
                    curOrg.NameOrg = values["NameOrg"];
                    curOrg.ShName = values["ShName"];
                    curOrg.LegalAddress = values["LegalAddress"];
                    curOrg.FactAddress = values["FactAddress"];
                    curOrg.Phone = values["Phone"];
                    curOrg.Login = newLogin;
                    curOrg.RefRegionAn = regionsRepository.FindOne(Convert.ToInt32(values["Region_Value"]));
                    curOrg.KPP = values["KPP"];
                    curOrg.INN = values["INN"];
                    if (curOrg.DatRegistr == null)
                    {
                        curOrg.DatRegistr = DateTime.Today;
                    }

                    orgRepository.Save(curOrg);
                    org = curOrg;
                }
            }

            return org;
        }

        private void EditUser(string oldName, string newName, string description, int region)
        {
            var users = scheme.UsersManager.GetUsers();
            
            foreach (DataRow user in users.Rows)
            {
                if (user["Name"].Equals(oldName))
                {
                    user["DNSName"] = newName;
                    user["Name"] = newName;
                    user["Description"] = description;
                    user["RefRegion"] = region;
                }
            }

            scheme.UsersManager.ApplayUsersChanges(users.GetChanges());
        }

        private void CreateUser(string name, string description, int region)
        {
            var users = scheme.UsersManager.GetUsers();

            // добавление пользователя
            var userId = scheme.SchemeDWH.DB.GetGenerator("g_Users");

            var row = users.NewRow();
            row.UpdateRow(new Users
                              {
                                  DNSName = name,
                                  Name = name,
                                  AllowDomainAuth = true,
                                  AllowPwdAuth = true,
                                  UserType = 0,
                                  Blocked = false,
                                  Description = description,
                                  RefRegion = region,
                                  FirstName = string.Empty,
                                  LastName = string.Empty,
                                  LastLogin = DateTime.Today,
                                  JobTitle = string.Empty,
                                  Patronymic = string.Empty,
                                  RefDepartments = null,
                                  RefOrganizations = null
                              });

            row["ID"] = userId;
            users.Rows.Add(row);

            scheme.UsersManager.ApplayUsersChanges(users.GetChanges());

            // изменение параметров пользователя, которые не мб заданы при его создании
            users = scheme.UsersManager.GetUsers();
            users.PrimaryKey = new[] { users.Columns["ID"] };

            var user = users.Rows.Find(userId);
            user["AllowDomainAuth"] = true;
            user["AllowPwdAuth"] = true;
            user["RefRegion"] = region;

            scheme.UsersManager.ApplayUsersChanges(users.GetChanges());

            // Привязка пользователя к группе
            var groups = scheme.UsersManager.GetGroups();
            groups.PrimaryKey = new[] { groups.Columns["Name"] };
            var group = groups.Rows.Find("Организации сбор ЖКХ");
            if (group != null)
            {
                var groupId = Convert.ToInt32(group["ID"]);
                var memberShips = scheme.UsersManager.GetUsersForGroup(groupId);
                var memberShipToEdit =
                    memberShips.Select().Where(x => name.Equals(x.ItemArray[1])).FirstOrDefault();
                if (memberShipToEdit != null)
                {
                    if (!Convert.ToBoolean(memberShipToEdit.ItemArray[2]))
                    {
                        memberShipToEdit[2] = true;
                    }

                    scheme.UsersManager.ApplayMembershipChanges(groupId, memberShips.GetChanges(), false);
                }
            }
        }
    }
}
