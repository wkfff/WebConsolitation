using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Controllers
{
    public class Region10OmsuCompareController : SchemeBoundController
    {
        private readonly IRegion10MarksOivExtension extension;
        private readonly IFactRepository factRepository;
        private readonly IMarksRepository marksRepository;
        private readonly IRepository<FX_OIV_StatusData> statusRepository;
        private readonly IMarksCalculator marksCalculator;

        public Region10OmsuCompareController(
                                             IRegion10MarksOivExtension extension,
                                             IFactRepository factRepository,
                                             IMarksRepository marksRepository,
                                             IRepository<FX_OIV_StatusData> statusRepository,
                                             IMarksCalculator marksCalculator)
        {
            this.extension = extension;
            this.factRepository = factRepository;
            this.marksRepository = marksRepository;
            this.statusRepository = statusRepository;
            this.marksCalculator = marksCalculator;
        }

        public ActionResult Load(int? markId, bool[] filter)
        {
            if (markId == null)
            {
                return new AjaxStoreResult(new object(), 0);
            }

            List<int> filters = new List<int>();
            for (int index = 0; index < 2; index++)
            {
                if (!filter[index])
                {
                    filters.Add(index + 1);
                }
            }

            var data = factRepository.GetTerritories((int)markId);

            var view = (from f in data
                        where !filters.Contains(f.RefStatusData.ID)
                        select new
                        {
                            f.ID,
                            RefStatusData = f.RefStatusData.ID,
                            Territory = f.RefTerritory.Name,
                            f.Fact,
                            f.Forecast,
                            f.Forecast2,
                            f.Forecast3,
                            f.Note,
                            RefTerritory = f.RefTerritory.ID,
                            f.RefOIV.RefUnits.Symbol,
                            f.RefOIV.Capacity,
                            Readonly = !String.IsNullOrEmpty(f.RefOIV.Formula)
                        })
                      .ToList();

            return new AjaxStoreResult(view, view.Count());
        }

        [Transaction]
        public ActionResult Save(object data)
        {
            var ss = JavaScriptDomainConverter<F_OIV_REG10Qual>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            foreach (F_OIV_REG10Qual oivReg10 in ss.Updated)
            {
                // TODO: check status
                oivReg10.SourceID = extension.DataSourceOiv.ID;
                oivReg10.RefOIV = marksRepository.FindOne(oivReg10.RefOIV.ID);
                oivReg10.RefTerritory = extension.RootTerritoryRf;
                oivReg10.RefStatusData = statusRepository.Get(oivReg10.RefStatusData.ID);
                oivReg10.RefYear = extension.CurrentYear;

                factRepository.Save(oivReg10);
            }

            marksCalculator.Calc(ss.Updated.ToList(), extension.RootTerritoryRf.ID, true);

            factRepository.DbContext.CommitChanges();

            return new AjaxStoreResult(StoreResponseFormat.Save);
        }

        public ActionResult LoadMarkDescription(int? markId)
        {
            StringBuilder sb = new StringBuilder();

            if (markId == null)
            {
                sb.AppendLine("clearMarkDescription();");
                sb.AppendLine("viewportMain.doLayout();");

                return new AjaxResult(sb.ToString());
            }

            var mark = marksRepository.FindOne((int)markId);

            sb.AppendLine("labelOKEI.update('<b>Единицы измерения</b>: {0}');".FormatWith(mark.RefUnits.Symbol));
            sb.AppendLine("labelCompRep.update('<b>Наименование раздела</b>: {0}');".FormatWith(mark.RefOIV.Name));
            sb.AppendLine("labelCodeRep.update('<b>Номер в докладе</b>: {0}');".FormatWith(mark.CodeRep));
            sb.AppendLine("labelSymbol.update('<b>Обозначение</b>: {0}');".FormatWith(mark.Symbol));
            sb.AppendLine("labelFormula.update('<b>Формула</b>: {0}');".FormatWith(mark.Formula));
            sb.AppendLine("viewportMain.doLayout();");

            return new AjaxResult(sb.ToString());
        }

        public ActionResult GetMarksListForCompare(string filter)
        {
            var data = marksRepository.FindAll()
                                      .Where(x => x.RefTypeMark.ID == (int)TypeMark.Gather
                                                  && x.MO == true
                                                  && (x.RefResponsOIV == extension.UserResponseOiv
                                                      || x.RefResponsOIV1 == extension.UserResponseOiv))
                                      .OrderBy(x => x.Code)
                                      .Select(x => new { ID = x.ID, Name = String.Format("{0}{1}{2}", x.CodeRep, String.IsNullOrEmpty(x.CodeRep) ? String.Empty : " ", x.Name) })
                                      .ToList();

            if (!String.IsNullOrEmpty(filter))
            {
                data = data.Where(f => f.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            }

            return new AjaxStoreResult(data, data.Count);
        }
    }
}
