using System;
using System.Linq;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.MinSport.Presentation.Controllers
{
    public class ComboController : SchemeBoundController
    {
        private readonly ILinqRepository<D_Territory_RF> repTerritory;
        private readonly ILinqRepository<D_MassMedia_Kind> repMassMediKind;
        private readonly ILinqRepository<D_KD_TargetGroup> repTargetGroup;
        private readonly ILinqRepository<FX_FX_TerritorialPartitionType> repTerritorialPartitionType;
        private readonly ILinqRepository<D_OK_LocalityType> repLocalityType;

        public ComboController(
            ILinqRepository<D_Territory_RF> repTerritory,
            ILinqRepository<D_MassMedia_Kind> repMassMediKind,
            ILinqRepository<D_KD_TargetGroup> repTargetGroup,
            ILinqRepository<FX_FX_TerritorialPartitionType> repTerritorialPartitionType,
            ILinqRepository<D_OK_LocalityType> repLocalityType)
        {
            this.repTerritory = repTerritory;
            this.repMassMediKind = repMassMediKind;
            this.repTargetGroup = repTargetGroup;
            this.repTerritorialPartitionType = repTerritorialPartitionType;
            this.repLocalityType = repLocalityType;
        }

        public ActionResult LoadMassMediaKind(string beginCode, string endCode)
        {
            var list = (from f in repMassMediKind.FindAll()
                        where f.Code >= Convert.ToInt32(beginCode) && f.Code < Convert.ToInt32(endCode)
                        orderby f.Code
                        select new
                        {
                            Value = f.ID,
                            Text = f.KindName
                        }).ToList();
            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult LoadTargetGroup()
        {
            var list = (from f in repTargetGroup.FindAll()
                        orderby f.OtherName
                        select new
                        {
                            Value = f.ID,
                            Text = f.OtherName
                        }).ToList();
            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult LoadTerritorialPartitionType()
        {
            var list = (from f in repTerritorialPartitionType.FindAll()
                        where f.ID == 4 || f.ID == 5
                        orderby f.FullName
                        select new
                        {
                            Value = f.ID,
                            Text = f.FullName
                        }).ToList();
            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult LoadTerritorySubject()
        {
            var list = (from f in repTerritory.FindAll()
                        where f.RefTerritorialPartType.ID == 3
                        orderby f.Name
                        select new
                        {
                            Value = f.ID,
                            Text = f.Name
                        }).ToList();
            return new AjaxStoreResult(list, list.Count); 
        }
        
        public ActionResult LoadTerritorySubjectAndSettlement(string parentId)
        {
            var list = (from f in repTerritory.FindAll()
                        where f.ParentID == Convert.ToInt32(parentId)
                        orderby f.Name
                        select new
                        {
                            Value = f.ID,
                            Text = f.Name
                        }).ToList();
            return new AjaxStoreResult(list, list.Count);
        }

        public ActionResult LoadLocalityType()
        {
            var list = (from f in repLocalityType.FindAll()
                        orderby f.LocalityTypeName
                        select new
                        {
                            Value = f.ID,
                            Text = f.LocalityTypeName
                        }).ToList();
            return new AjaxStoreResult(list, list.Count);
        }
    }
}
