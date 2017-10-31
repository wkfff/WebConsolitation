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
using Krista.FM.RIA.Extensions.MarksOMSU.Services;

namespace Krista.FM.RIA.Extensions.MarksOMSU.Presentation.Controllers
{
    public class MarksOivController : SchemeBoundController
    {
        private readonly IMarksOmsuExtension extension;
        private readonly IMarksOmsuRepository marksOmsuRepository;
        private readonly IMarksRepository marksRepository;
        private readonly IRegionsRepository regionRepository;
        private readonly IRepository<FX_OMSU_StatusData> statusRepository;
        private readonly IMarksDataInitializer dataInitializer;
        private readonly IMarksCalculator marksCalculator;

        public MarksOivController(
            IMarksOmsuExtension extension,
            IMarksOmsuRepository marksOmsuRepository,
            IMarksRepository marksRepository,
            IRegionsRepository regionRepository,
            IRepository<FX_OMSU_StatusData> statusRepository,
            IMarksDataInitializer dataInitializer,
            IMarksCalculator marksCalculator)
        {
            this.extension = extension;
            this.marksOmsuRepository = marksOmsuRepository;
            this.marksRepository = marksRepository;
            this.regionRepository = regionRepository;
            this.statusRepository = statusRepository;
            this.dataInitializer = dataInitializer;
            this.marksCalculator = marksCalculator;
        }

        public ActionResult Load(int? markId, bool? calculatePreviosResult)
        {
            if (markId == null)
            {
                return new AjaxStoreResult(new object(), 0);
            }

            dataInitializer.CreateRegionsForMark("mark{0}_{1}".FormatWith(markId, extension.CurrentYear), (int)markId);

            IList<F_OMSU_Reg16> previousFacts = new List<F_OMSU_Reg16>();
            if (calculatePreviosResult == true)
            {
                previousFacts = marksOmsuRepository.GetForOIVPrevious((int)markId);
            }
            
            var facts = marksOmsuRepository.GetForOIV((int)markId);

            var view = (from f in facts
                        select new
                        {
                            f.ID,
                            Region = f.RefRegions.Name,
                            RegionCodeLine = f.RefRegions.CodeLine,
                            PriorYearCurrentValue = calculatePreviosResult == true ? 
                                                    (from p in previousFacts 
                                                     where p.RefMarksOMSU.RefMarksB == f.RefMarksOMSU.RefMarksB 
                                                           && p.RefRegions.RefBridgeRegions == f.RefRegions.RefBridgeRegions
                                                     select p.CurrentValue)
                                                     .FirstOrDefault()
                                                    : null,
                            f.PriorValue,
                            f.CurrentValue,
                            f.Prognoz1,
                            f.Prognoz2,
                            f.Prognoz3,
                            f.Note,
                            RefMarksOMSU = f.RefMarksOMSU.ID,
                            RefRegions = f.RefRegions.ID,
                            RefStatusData = f.RefStatusData.ID,
                            RefYearDayUNV = f.RefYearDayUNV.ID,
                            OKEI = f.RefMarksOMSU.RefOKEI.Symbol,
                            f.RefMarksOMSU.Capacity,
                            Readonly = !String.IsNullOrEmpty(f.RefMarksOMSU.Formula) || f.RefMarksOMSU.Grouping
                        })
                      .ToList();

            return new AjaxStoreResult(view, view.Count());
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

            sb.AppendLine("labelOKEI.update('<b>Единицы измерения</b>: {0}');".FormatWith(mark.RefOKEI.Name));
            sb.AppendLine("labelCodeRep.update('<b>Номер в докладе</b>: {0}');".FormatWith(mark.CodeRepDouble));
            sb.AppendLine("labelDescription.update('<b>Что показывает</b>: {0}');".FormatWith(mark.Description));
            sb.AppendLine("labelCalcMark.update('<b>Расчет</b>: {0}');".FormatWith(mark.CalcMark == null ? null : mark.CalcMark.Replace("\r\n", String.Empty)));
            sb.AppendLine("labelInfoSource.update('<b>Информационная обеспеченность</b>: {0}');".FormatWith(mark.InfoSource == null ? null : mark.InfoSource.Replace("\r\n", String.Empty)));
            sb.AppendLine("labelSymbol.update('<b>Обозначение</b>: {0}');".FormatWith(mark.Symbol));
            sb.AppendLine("labelFormula.update('<b>Формула</b>: {0}');".FormatWith(mark.Formula));
            sb.AppendLine("viewportMain.doLayout();");

            return new AjaxResult(sb.ToString());
        }

        public ActionResult GetMarksListForOiv(string filter)
        {
            if (extension.DataSourceRegion == null)
            {
                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Ошибка инициализации.",
                    Html = "Не найден источник данных у Регионы.Анализ для выбранного года!",
                    HideDelay = 2000
                });
                return new AjaxStoreExtraResult(new object(), 0, message.ToScript());
            }

            var data = marksRepository.FindAll()
                                      .Where(x => x.RefResponsOIV == extension.ResponsOIV && x.RefTypeMark.ID == (int)TypeMark.Gather && !x.Grouping)
                                      .OrderBy(x => x.Code)
                                      .Select(x => new { ID = x.ID, Name = String.Format("{0}{1}{2}", x.CodeRepDouble, String.IsNullOrEmpty(x.CodeRepDouble) ? String.Empty : " ", x.Name) })
                                      .ToList();
            if (!String.IsNullOrEmpty(filter))
            {
                data = data.Where(f => f.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            }

            return new AjaxStoreResult(data, data.Count);
        }

        public ActionResult GetMarksListForCompare(string filter)
        {
            if (extension.DataSourceRegion == null)
            {
                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Ошибка инициализации.",
                    Html = "Не найден источник данных у Регионы.Анализ для выбранного года!",
                    HideDelay = 2000
                });
                return new AjaxStoreExtraResult(new object(), 0, message.ToScript());
            }

            var data = marksRepository.FindAll()
                                      .Where(x => x.RefTypeMark.ID == (int)TypeMark.Gather && !x.Grouping)
                                      .OrderBy(x => x.Code)
                                      .Select(x => new { ID = x.ID, Name = String.Format("{0}{1}{2}", x.CodeRepDouble, String.IsNullOrEmpty(x.CodeRepDouble) ? String.Empty : " ", x.Name) })
                                      .ToList();
            
            if (!String.IsNullOrEmpty(filter))
            {
                data = data.Where(f => f.Name.IndexOf(filter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            }

            return new AjaxStoreResult(data, data.Count);
        }

        [Transaction]
        public ActionResult Save(object data, bool? withPrognoz)
        {
            if (withPrognoz == null)
            {
                withPrognoz = false;
            }

            var ss = JavaScriptDomainConverter<F_OMSU_Reg16>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            foreach (F_OMSU_Reg16 omsuReg16 in ss.Updated)
            {
                omsuReg16.SourceID = extension.DataSourceOmsu.ID;
                omsuReg16.RefMarksOMSU = marksRepository.FindOne(omsuReg16.RefMarksOMSU.ID);
                omsuReg16.RefRegions = regionRepository.FindOne(omsuReg16.RefRegions.ID);
                omsuReg16.RefStatusData = statusRepository.Get(omsuReg16.RefStatusData.ID);
                omsuReg16.RefYearDayUNV = extension.CurrentYearUNV;

                marksOmsuRepository.Save(omsuReg16);

                marksCalculator.Calc(new List<F_OMSU_Reg16> { omsuReg16 }, omsuReg16.RefRegions.ID, (bool)withPrognoz);
            }

            marksOmsuRepository.DbContext.CommitChanges();

            return new AjaxStoreResult(StoreResponseFormat.Save);
        }
    }
}
