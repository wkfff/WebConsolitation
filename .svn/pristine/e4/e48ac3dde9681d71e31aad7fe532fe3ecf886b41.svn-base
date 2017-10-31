using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Common;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Core.Progress;
using Krista.FM.RIA.Extensions.E86N.Auth.Data;
using Krista.FM.RIA.Extensions.E86N.Auth.Model;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Presentation.Controllers
{
    public sealed class AccountsController : SchemeBoundController
    {
        private readonly ILinqRepository<Memberships> memberships;

        private readonly AuthRepositiory<D_Org_Structure> organization;

        private readonly ILinqRepository<Users> users;

        private readonly ILinqRepository<Groups> groups;

        private readonly IProgressManager progressManager;

        private readonly AuthRepositiory<D_Org_UserProfile> repository;

        public AccountsController(
            IAuthService auth, 
            ILinqRepository<D_Org_UserProfile> repository, 
            ILinqRepository<D_Org_Structure> org, 
            ILinqRepository<Users> users, 
            ILinqRepository<Groups> groups, 
            ILinqRepository<Memberships> memberships, 
            IProgressManager progressManager)
        {
            this.repository = new AuthRepositiory<D_Org_UserProfile>(
                repository, 
                auth, 
                ppoIdExpr => ppoIdExpr.RefUchr.RefOrgPPO, 
                grbsIdExpr => grbsIdExpr.RefUchr.RefOrgGRBS.ID, 
                orgIdExpr => orgIdExpr.RefUchr.ID);
            organization = new AuthRepositiory<D_Org_Structure>(
                org, 
                auth, 
                ppoIdExpr => ppoIdExpr.RefOrgPPO, 
                grbsIdExpr => grbsIdExpr.RefOrgGRBS.ID, 
                orgIdExpr => orgIdExpr.ID);
            this.users = users;

            this.memberships = memberships;
            this.progressManager = progressManager;
            this.groups = groups;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public RestResult Read([FiltersBinder] FilterConditions filters)
        {
            IQueryable<D_Org_UserProfile> accounts = repository.FindAll().Where(x => x.RefUchr.CloseDate == null || x.RefUchr.CloseDate > DateTime.Now);

            // Накладываем фильтры грида
            foreach (FilterCondition filter in filters.Conditions)
            {
                string filterValue = filter.Value;
                switch (filter.Name)
                {
                    case "Login":
                        accounts = accounts.Where(v => v.UserLogin.Contains(filterValue));
                        break;
                    case "OrgName":
                        accounts = accounts.Where(v => v.RefUchr.Name.Contains(filterValue));
                        break;
                }
            }

            Groups grbsGroup = groups.FindAll().Single(x => x.Name.Equals(AccountsRole.Provider));
            Groups ppoGroup = groups.FindAll().Single(x => x.Name.Equals(AccountsRole.SuperProvider));

            return new RestResult
                {
                    Success = true, 
                    Data = accounts.Join(
                        users.FindAll(), 
                        account => account.UserLogin, 
                        user => user.Name, 
                        (account, user) => new AccountsViewModel
                            {
                                LastLogin = user.LastLogin, 
                                ID = account.ID, 
                                IsAdmin = account.IsAdmin, 
                                Email = account.EMail, 
                                Login = account.UserLogin, 
                                OrgId = account.RefUchr.ID, 
                                OrgName = account.RefUchr.Name, 
                                IsGrbs =
                                    memberships.FindAll().Any(
                                        grbs =>
                                        grbs.RefGroups == grbsGroup && grbs.RefUsers == user), 
                                IsPpo =
                                    memberships.FindAll().Any(
                                        ppo =>
                                        ppo.RefGroups == ppoGroup && ppo.RefUsers == user), 
                            })
                };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Create(string data)
        {
            return new RestResult { Success = false, Message = "Действие недоступно" };
        }

        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Update(string data)
        {
            try
            {
                AccountsViewModel formData = JavaScriptDomainConverter<AccountsViewModel>.DeserializeSingle(data);
                Users user = users.FindAll().Single(v => v.Name.Equals(formData.Login));
                DataTable groupsForUser = Scheme.UsersManager.GetGroupsForUser(user.ID);
                Groups grbsGroup = groups.FindAll().Single(x => x.Name.Equals(AccountsRole.Provider));
                Groups ppoGroup = groups.FindAll().Single(x => x.Name.Equals(AccountsRole.SuperProvider));

                if (formData.IsGrbs !=
                    memberships.FindAll().Any(grbs => grbs.RefGroups == grbsGroup && grbs.RefUsers == user))
                {
                    if (formData.IsGrbs)
                    {
                        var membership = new Memberships
                            {
                                ID = 0, 
                                RefUsers = user, 
                                RefGroups = grbsGroup, 
                            };
                        memberships.Save(membership);
                    }
                    else
                    {
                        memberships.Delete(memberships.FindAll().Single(grbs => grbs.RefGroups == grbsGroup && grbs.RefUsers == user));
                    }
                }

                if (formData.IsPpo !=
                    memberships.FindAll().Any(ppo => ppo.RefGroups == ppoGroup && ppo.RefUsers == user))
                {
                    if (formData.IsPpo)
                    {
                        var membership = new Memberships
                            {
                                ID = 0, 
                                RefUsers = user, 
                                RefGroups = ppoGroup, 
                            };
                        memberships.Save(membership);
                    }
                    else
                    {
                        memberships.Delete(memberships.FindAll().Single(ppo => ppo.RefGroups == ppoGroup && ppo.RefUsers == user));
                    }
                }

                Scheme.UsersManager.ApplayMembershipChanges(user.ID, groupsForUser.GetChanges(), true);

                return new RestResult
                    {
                        Success = true, 
                        Message = "Запись обновлена"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Delete)]
        [Transaction(RollbackOnModelStateError = true)]
        public RestResult Destroy(int id)
        {
            return new RestResult { Success = false, Message = "Действие недоступно", };
        }

        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction]
        public ActionResult ActivateAccounts()
        {
            try
            {
                DataTable user = Scheme.UsersManager.GetUsers();
                List<Users> usersCache = users.FindAll().Where(users1 => !users1.AllowPwdAuth).ToList();
                var processedCache = new List<int>();
                int processCount = usersCache.Count;

                usersCache.Each(
                    users1 =>
                    {
                        progressManager.SetCompleted(
                            string.Concat("Активация пользователя: ", users1.Name),
                            (double)processedCache.Count / processCount);
                        users1.AllowPwdAuth = true;
                        users.Save(users1);
                        processedCache.Add(users1.ID);
                    });

                progressManager.SetCompleted(
                    string.Concat(
                        @"Применение изменений(", 
                        processCount,
                        @" пользователей обработано)"));
                Scheme.UsersManager.ApplayUsersChanges(user);
                progressManager.SetCompleted(1.0);

                return new RestResult
                    {
                        Success = true,
                        Message = "Пользователи активированы"
                    };
            }
            catch (Exception e)
            {
                return new RestResult { Success = false, Message = e.Message };
            }
        }

        [AcceptVerbs(HttpVerbs.Put)]
        [Transaction]
        public ActionResult ResetAccountsPassord()
        {
            Stopwatch s = Stopwatch.StartNew();
            Trace.WriteLine(string.Concat("Начало обработки ", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            var accounts = repository.FindAll().Join(
                users.FindAll(), 
                account => account.UserLogin, 
                user => user.Name, 
                (account, user) => new
                    {
                        user.ID, 
                        Login = user.Name, 
                        OrgName = account.RefUchr.Name, 
                        GrbsCode = account.RefUchr.RefOrgGRBS.Code, 
                    }).OrderBy(x => x.GrbsCode).ToList();
            Trace.WriteLine(string.Format("Количество отобранных пользователей {0}. {1:c}", accounts.Count(), s.Elapsed));

            using (new ServerContext())
            {
                using (IUsersManager userManager = Scheme.UsersManager)
                {
                    string errStr = string.Empty;
                    accounts.Each(
                        obj =>
                        {
                            string password = Guid.NewGuid().ToString().Substring(0, 8);
                            userManager.ChangeUserPasswordAdm(obj.ID, PwdHelper.GetPasswordHash(password), ref errStr);
                            Trace.WriteLine(
                                string.Format(
                                    "\tName={0}\tGrbsCode={3}\tLogin={1}\tPassword={2}", 
                                    obj.OrgName, 
                                    obj.Login, 
                                    password, 
                                    obj.GrbsCode));
                        });
                }
            }

            Trace.WriteLine(string.Format("Обработка завершена. Общее время выполнения {0:c}", s.Elapsed));

            return new RestResult { Success = true, Message = "пароли пользователей сброшены" };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [Transaction]
        public ActionResult CreateAgents()
        {
            Stopwatch s = Stopwatch.StartNew();
            Trace.WriteLine(string.Concat("Начало обработки ", DateTime.Now.ToString(CultureInfo.InvariantCulture)));
            List<D_Org_Structure> q = organization.FindAll().Where(o => !repository.FindAll().Any(x => x.RefUchr.ID == o.ID)).ToList();
            Trace.WriteLine(string.Format("Количество отобранных организаций {0}. {1:c}", q.Count(), s.Elapsed));

            using (new ServerContext())
            {
                Groups defaultGroup = groups.FindAll().Single(x => x.Name.Equals(AccountsRole.Consumer));
                q.Each(structure => CreateDefaultAccount(structure, defaultGroup));
            }

            Trace.WriteLine(string.Format("Обработка завершена. Общее время выполнения {0:c}", s.Elapsed));

            return new RestResult { Success = true, Message = "пользователи созданы" };
        }

        private void CreateDefaultAccount(D_Org_Structure org, Groups defaultGroup)
        {
            try
            {
                string password = Guid.NewGuid().ToString().Substring(0, 8);
                string login = string.Concat(org.INN, org.KPP);
                D_Org_UserProfile profile =
                    repository.FindAll().SingleOrDefault(v => v.UserLogin.Equals(login)) ??
                    new D_Org_UserProfile
                        {
                            ID = 0, 
                            IsAdmin = true, 
                            IsActive = true, 
                            UserLogin = login, 
                            EMail = String.Empty, 
                        };
                Users user =
                    users.FindAll().SingleOrDefault(v => v.Name.Equals(login)) ??
                    new Users
                        {
                            ID = 0, 
                            Name = login, 
                            LastLogin = SqlDateTime.MinValue.Value, 
                            PwdHashSHA = PwdHelper.GetPasswordHash(password), 
                            AllowPwdAuth = true, 
                        };

                profile.RefUchr = org;
                repository.Save(profile);
                users.Save(user);

                Memberships membership =
                    memberships.FindAll().SingleOrDefault(v => v.RefGroups == defaultGroup && v.RefUsers == user) ??
                    new Memberships
                        {
                            ID = 0, 
                            RefUsers = user, 
                            RefGroups = defaultGroup, 
                        };
                memberships.Save(membership);

                repository.DbContext.CommitChanges();
                Trace.WriteLine(
                    string.Format(
                        "\tName={0}\tGrbsCode={3}\tLogin={1}\tPassword={2}",
                        org.Name,
                        profile.UserLogin,
                        password,
                        org.RefOrgGRBS != null ? org.RefOrgGRBS.ID.ToString(CultureInfo.InvariantCulture) : "Не задан"));
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ExpandException());
            }
        }
    }
}
