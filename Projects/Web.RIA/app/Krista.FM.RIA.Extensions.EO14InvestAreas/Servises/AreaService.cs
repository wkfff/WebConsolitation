using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Helpers;
using Krista.FM.RIA.Extensions.EO14InvestAreas.Models;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Servises
{
    public class AreaService : IAreaService
    {
        private readonly ILinqRepository<D_InvArea_Reestr> areaRepository;
        private readonly ILinqRepository<D_Territory_RF> territoryRfRepository;
        private readonly ILinqRepository<FX_InvArea_Status> invAreaStatusRepository;
        private readonly ILinqRepository<D_InvArea_Visual> filesRepository;

        public AreaService(
                           ILinqRepository<D_InvArea_Reestr> areaRepository,
                           ILinqRepository<D_Territory_RF> territoryRfRepository,
                           ILinqRepository<FX_InvArea_Status> invAreaStatusRepository,
                           ILinqRepository<D_InvArea_Visual> filesRepository)
        {
            this.areaRepository = areaRepository;
            this.territoryRfRepository = territoryRfRepository;
            this.invAreaStatusRepository = invAreaStatusRepository;
            this.filesRepository = filesRepository;
        }

        public IList GetAreasTable(bool filterEdit, bool filterReview, bool filterAccepted, IUserCredentials userCredentials)
        {
            if (!userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator))
            {
                return new List<object>();
            }
            
            var data = from p in this.areaRepository.FindAll()
                       select new
                       {
                           ID = p.ID,
                           Status = p.RefStatus.ID,
                           RegNumber = p.RegNumber,
                           TerritoryName = p.RefTerritory.Name,
                           Location = p.Location,
                           CadNumber = p.CadNumber,
                           CreatedDate = p.CreatedDate,
                           AdoptionDate = p.AdoptionDate,
                           CreateUser = p.CreateUser
                       };

            if (userCredentials.IsInRole(InvAreaRoles.Coordinator))
            {
                data = data.Where(f => (f.CreateUser == userCredentials.User.Name && f.Status == (int)InvAreaStatus.Edit) 
                                      || f.Status == (int)InvAreaStatus.Review 
                                      || f.Status == (int)InvAreaStatus.Accepted);
            }
            else if (userCredentials.IsInRole(InvAreaRoles.Creator))
            {
                data = data.Where(f => f.CreateUser == userCredentials.User.Name);
            }
            else
            {
                throw new Exception("Неизвестная роль пользователя");
            }

            if (!filterEdit || !filterReview || !filterAccepted)
            {
                data = data.Where(f => (filterEdit && (f.Status == (int)InvAreaStatus.Edit))
                                      || (filterReview && (f.Status == (int)InvAreaStatus.Review))
                                      || (filterAccepted && (f.Status == (int)InvAreaStatus.Accepted)));
            }

            var t = data.ToList();
            return t;
        }

        public AreaDetailViewModel GetInitialAreaModel()
        {
            var data = new AreaDetailViewModel
            {
                RefStatusId = (int)InvAreaStatus.Edit
            };
            return data;
        }

        public AreaDetailViewModel GetAreaModel(int areaId)
        {
            var data = new List<AreaDetailViewModel>(
                from p in areaRepository.FindAll()
                where p.ID == areaId
                select new AreaDetailViewModel
                           {
                               ID = p.ID,
                               RegNumber = p.RegNumber,
                               Location = p.Location,
                               CadNumber = p.CadNumber,
                               Area = p.Area,
                               Category = p.Category,
                               Owner = p.Owner,
                               Head = p.Head,
                               Contact = p.Contact,
                               Phone = p.Phone,
                               Email = p.Email,
                               PermittedUse = p.PermittedUse,
                               ActualUse = p.ActualUse,
                               Documentation = p.Documentation,
                               Limitation = p.Limitation,
                               PermConstr = p.PermConstr,
                               Relief = p.Relief,
                               Road = p.Road,
                               Station = p.Station,
                               Pier = p.Pier,
                               Airport = p.Airport,
                               Plumbing = p.Plumbing,
                               Sewage = p.Sewage,
                               Gas = p.Gas,
                               Electricity = p.Electricity,
                               Heating = p.Heating,
                               Landfill = p.Landfill,
                               Telephone = p.Telephone,
                               Connectivity = p.Connectivity,
                               Fee = p.Fee,
                               DistanceZones = p.DistanceZones,
                               Buildings = p.Buildings,
                               Resources = p.Resources,
                               Settlement = p.Settlement,
                               ObjectEducation = p.ObjectEducation,
                               ObjectHealth = p.ObjectHealth,
                               ObjectSocSphere = p.ObjectSocSphere,
                               ObjectServices = p.ObjectServices,
                               Hotels = p.Hotels,
                               CoordinatesLat = p.CoordinatesLat,
                               CoordinatesLng = p.CoordinatesLng,
                               Note = p.Note,
                               CreatedDate = p.CreatedDate,
                               CreateUser = p.CreateUser,
                               AdoptionDate = p.AdoptionDate,
                               RefTerritoryId = p.RefTerritory.ID,
                               RefTerritoryName = p.RefTerritory.Name,
                               RefStatusId = p.RefStatus.ID
                           });

             if (data.Count > 0)
            {
                return data.First();
            }
            else
            {
                throw new KeyNotFoundException("Карточка не найдена.");
            }
        }

        public D_InvArea_Reestr GetProject(int id)
        {
            var entity = areaRepository.FindOne(id);
            if (entity == null)
            {
                throw new KeyNotFoundException("Проект не найден.");
            }

            return entity;
        }

        public void SaveProject(D_InvArea_Reestr entityNew, D_InvArea_Reestr entityOld, IUserCredentials userCredentials)
        {
            if (!userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator))
            {
                throw new SecurityException("Недостаточно привилегий");
            }

            CopyNotChangableParameters(entityOld, entityNew);

            Validate(entityNew);

            CheckParametersOnChange(entityOld, entityNew);

            CheckStatusChange(userCredentials, entityOld, entityNew);
            
            areaRepository.Save(entityNew);
            try
            {
                areaRepository.DbContext.CommitChanges();
            }
            catch (Exception)
            {
                throw new Exception("Ошибка сохранения данных");
            }
        }

        public D_Territory_RF GetRefTerritory(int id)
        {
            var result = territoryRfRepository.FindOne(id);
            return result;
        }

        public FX_InvArea_Status GetRefStatus(int id)
        {
            var result = invAreaStatusRepository.FindOne(id);
            return result;
        }

        public void DeleteProject(int id, IUserCredentials userCredentials)
        {
            var project = areaRepository.FindOne(id);
            if (project == null)
            {
                throw new KeyNotFoundException("Запись не найдена.");
            }

            var permission = new PermissionSettings(userCredentials, project);
            if (!permission.CanDeleteProject)
            {
                throw new SecurityException("Недостаточно привилегий!");
            }

            var files = filesRepository.FindAll().Where(f => f.RefReestr == project);
            foreach (var row in files)
            {
                filesRepository.Delete(row);
            }

            filesRepository.DbContext.CommitChanges();

            areaRepository.Delete(project);
            areaRepository.DbContext.CommitChanges();
        }

        /// <summary>
        /// Проверка корректности заполнения атрибутов
        /// </summary>
        internal void Validate(D_InvArea_Reestr entity)
        {
            if (entity.RefStatus.ID == (int)InvAreaStatus.Accepted && String.IsNullOrEmpty(entity.RegNumber))
            {
                throw new Exception("Не заполнено поле Регистрационный номер");
            }

            if ((entity.RefStatus.ID == (int)InvAreaStatus.Review || entity.RefStatus.ID == (int)InvAreaStatus.Accepted) && entity.CreatedDate == null)
            {
                throw new Exception("Не заполнена дата создания");
            }

            if (entity.RefStatus.ID == (int)InvAreaStatus.Accepted && entity.AdoptionDate == null)
            {
                throw new Exception("Не заполнена дата принятия");
            }

            return;
        }

        /// <summary>
        /// Проверка возможности изменения статуса в соответствии с ролью и автором проекта
        /// </summary>
        internal void CheckStatusChange(IUserCredentials userCredentials, D_InvArea_Reestr entityOld, D_InvArea_Reestr entityNew)
        {
            if (entityOld == null)
            {
                if (userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator) 
                     && (entityNew.RefStatus.ID == (int)InvAreaStatus.Edit || entityNew.RefStatus.ID == (int)InvAreaStatus.Review))
                {
                    return;
                }
            }
            else
            {
                if (userCredentials.IsInAnyRole(InvAreaRoles.Creator, InvAreaRoles.Coordinator)
                    && userCredentials.User.Name == entityOld.CreateUser && userCredentials.User.Name == entityNew.CreateUser
                    && ((entityOld.RefStatus.ID == (int)InvAreaStatus.Edit && entityNew.RefStatus.ID == (int)InvAreaStatus.Edit) 
                        || (entityOld.RefStatus.ID == (int)InvAreaStatus.Edit && entityNew.RefStatus.ID == (int)InvAreaStatus.Review)))
                {
                    return;
                }

                if (userCredentials.IsInRole(InvAreaRoles.Coordinator)
                    && ((entityOld.RefStatus.ID == (int)InvAreaStatus.Review && entityNew.RefStatus.ID == (int)InvAreaStatus.Edit) 
                        || (entityOld.RefStatus.ID == (int)InvAreaStatus.Review && entityNew.RefStatus.ID == (int)InvAreaStatus.Accepted)
                        || (entityOld.RefStatus.ID == (int)InvAreaStatus.Accepted && entityNew.RefStatus.ID == (int)InvAreaStatus.Edit)))
                {
                    return;
                }
            }

            throw new Exception("Изменение статуса запрещено");
        }

        /// <summary>
        /// Проверка параметров, которые должны заполняться при изменении статусов
        /// </summary>
        internal void CheckParametersOnChange(D_InvArea_Reestr entityOld, D_InvArea_Reestr entityNew)
        {
            if (entityOld != null && entityNew != null && entityNew.RefStatus.ID == (int)InvAreaStatus.Edit &&
                (entityOld.RefStatus.ID == (int)InvAreaStatus.Review ||
                 entityOld.RefStatus.ID == (int)InvAreaStatus.Accepted) && String.IsNullOrEmpty(entityNew.Note))
            {
                throw new Exception("Не заполнена причина возврата на доработку");
            }
        }

        /// <summary>
        /// Копирование параметров, которые не должны изменяться при некоторых условиях
        /// </summary>
        internal void CopyNotChangableParameters(D_InvArea_Reestr entityOld, D_InvArea_Reestr entityNew)
        {
            if (entityOld == null || entityOld.RefStatus.ID == (int)InvAreaStatus.Edit)
            {
                return;
            }

            // Нихрена нельзя менять при утверждении, кроме пары атрибутов
            if (entityOld.RefStatus.ID == (int)InvAreaStatus.Review 
                 && entityNew.RefStatus.ID == (int)InvAreaStatus.Accepted)
            {
                // TODO: копировани с old на new кроме статуса, номера и даты
            }
        }
    }
}
