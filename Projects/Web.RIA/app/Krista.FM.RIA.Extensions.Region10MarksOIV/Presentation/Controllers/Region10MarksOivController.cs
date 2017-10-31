using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.Region10MarksOIV.Services;

namespace Krista.FM.RIA.Extensions.Region10MarksOIV.Presentation.Controllers
{
    public class Region10MarksOivController : SchemeBoundController
    {
        private readonly IRegion10MarksOivExtension extension;
        private readonly IFactRepository factRepository;
        private readonly IMarksRepository marksRepository;
        private readonly IRepository<FX_OIV_StatusData> statusRepository;
        private readonly IMarksDataInitializer dataInitializer;
        private readonly IMarksCalculator marksCalculator;
        private readonly IOivService oivService;

        public Region10MarksOivController(
            IRegion10MarksOivExtension extension,
            IFactRepository factRepository,
            IMarksRepository marksRepository,
            IRepository<FX_OIV_StatusData> statusRepository,
            IMarksDataInitializer dataInitializer,
            IMarksCalculator marksCalculator,
            IOivService oivService)
        {
            this.extension = extension;
            this.factRepository = factRepository;
            this.marksRepository = marksRepository;
            this.statusRepository = statusRepository;
            this.dataInitializer = dataInitializer;
            this.marksCalculator = marksCalculator;
            this.oivService = oivService;
        }

        public ActionResult Load(int? markId, bool[] filter)
        {
            dataInitializer.CreateMarksForTerritory(extension.CurrentYearVal, extension.RootTerritoryRf, false);

            List<int> filters = new List<int>();
            for (int index = 0; index < 4; index++)
            {
                if (!filter[index])
                {
                    filters.Add(index + 1);
                }
            }

            var data = User.IsInRole(OivRoles.OivApprove) ? factRepository.GetMarksForIMA() 
                                                          : factRepository.GetMarksForOiv();

            var view = from f in data
                       where !filters.Contains(f.RefStatusData.ID)
                       select new
                       {
                           f.ID,
                           RefStatusData = f.RefStatusData.ID,
                           Status = f.RefStatusData.Name,
                           CompRep = f.RefOIV.RefOIV.Name,
                           MarkName = f.RefOIV.Name,
                           OKEI = f.RefOIV.RefUnits.Symbol,
                           f.RefOIV.CodeRep,
                           f.RefOIV.Formula,
                           f.RefOIV.MO,
                           f.Fact,
                           f.Forecast,
                           f.Forecast2,
                           f.Forecast3,
                           f.Note,
                           RefOIV = f.RefOIV.ID,
                           RefRegions = f.RefTerritory.ID,
                           RefYear = f.RefYear.ID,
                           f.RefOIV.Capacity,
                           Readonly = !String.IsNullOrEmpty(f.RefOIV.Formula)
                       };

            return new AjaxStoreResult(view, view.Count());
        }

        [Transaction]
        public ActionResult Save(object data)
        {
            var ss = JavaScriptDomainConverter<F_OIV_REG10Qual>
                .Deserialize(Convert.ToString(((string[])data)[0]));

            foreach (F_OIV_REG10Qual oivReg10 in ss.Updated)
            {
                // TODO: check mark ovner
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

        [Transaction]
        public ActionResult AcceptReport()
        {
            if (!User.IsInRole(OivRoles.OivApprove))
            {
                return new AjaxResult { ErrorMessage = "Недостаточно привилегий" };
            }

            try
            {
                oivService.Accept();
            }
            catch (Exception e)
            {
                return new AjaxResult { ErrorMessage = e.Message };
            }

            return new AjaxResult();
        }

        [Transaction]
        public ActionResult RejectReport()
        {
            if (!User.IsInRole(OivRoles.OivApprove))
            {
                return new AjaxResult { ErrorMessage = "Недостаточно привилегий" };
            }

            oivService.Reject();

            return new AjaxResult();
        }

        public ActionResult SetCurrentYear(int year)
        {
            try
            {
                extension.CurrentYearVal = year;

                var message = Ext.Net.Notification.Show(new Ext.Net.NotificationConfig
                {
                    Title = "Установка источника.",
                    Html = "Текущий год изменен.",
                    HideDelay = 1000
                });

                return new AjaxResult { Script = message.ToScript() };
            }
            catch (Exception e)
            {
                var errorWnd = new ErrorWindow
                {
                    Title = "Установка источника.",
                    Text = e.Message
                };
                return new AjaxResult { Script = errorWnd.Build(new ViewPage())[0].ToScript() };  
            }
        }

        public ActionResult Expand(int id, int markId)
        {
            var result = new AjaxResult();

            var mark = marksRepository.FindOne(markId);

            string responceOiv = null;
            if (mark.RefResponsOIV.ID == -1 && mark.RefResponsOIV1.ID == -1)
            {
                responceOiv = "ОМСУ";
            }
            else
            {
                if (mark.RefResponsOIV.ID != -1)
                {
                    responceOiv = mark.RefResponsOIV.Name;
                }

                if (mark.RefResponsOIV1.ID != -1)
                {
                    responceOiv = String.Format("{0}{1}{2}", responceOiv, responceOiv.IsNullOrEmpty() ? null : ", ", mark.RefResponsOIV1.Name);
                }
            }

            Label label = new Label();
            label.Html = String.Format(
                @"<div class=""x-window-mc"">
<p><b>Обозначение:</b> {0} </p>
<p><b>Формула:</b> {1} </p>
<p><b>Ответственный (в том числе по согласованию):</b> {2} </p>
</div>",
                mark.Symbol,
                mark.Formula,
                responceOiv);

            result.Script = label.ToScript(RenderMode.RenderTo, "row-" + id);

            return result;
        }
    }
}
