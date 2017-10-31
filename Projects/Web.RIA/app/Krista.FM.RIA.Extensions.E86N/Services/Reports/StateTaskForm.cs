using System;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public class StateTaskForm
    {
        private readonly int docId;

        private readonly INewRestService newRestService;

        public StateTaskForm(int docId)
        {
            this.docId = docId;
            newRestService = Resolver.Get<INewRestService>();
        }

        public HSSFWorkbook GetStateTaskForm()
        {
            var doc = newRestService.GetItem<F_F_ParameterDoc>(docId);

            var lastPassportID = doc.RefUchr.Documents.Where(x => x.RefPartDoc.ID == FX_FX_PartDoc.PassportDocTypeID).Select(x => x.ID).Max();
            F_Org_Passport passport;
            try
            {
                passport = newRestService.GetItems<F_Org_Passport>().Single(x => x.RefParametr.ID == lastPassportID);
            }
            catch
            {
                throw new Exception("Паспорт не найден или не заполнен.");
            }

            var data = doc.StateTasks;

            var services = data.Where(x => x.RefVedPch.RefTipY.ID == D_Services_TipY.FX_FX_SERVICE).ToList();
            var works = data.Where(x => x.RefVedPch.RefTipY.ID == D_Services_TipY.FX_FX_WORK).ToList();

            var workBook = new HSSFWorkbook();
            var sheet = workBook.CreateSheet("лист 1");
            StateTaskFormHelper.SetDefaultStyle(workBook, sheet, StateTaskFormHelper.LastCol);
            var sphereOfActivity = data.Select(x => x.RefVedPch.RefSferaD.Name).Distinct().ToList();
            var currentRow = 0;
            int part;

            if (services.Count != 0)
            {
                StateTaskFormHelper.MainHeader(workBook, sheet, ref currentRow, true);

                if (sphereOfActivity.Count == 1)
                {
                    StateTaskFormHelper.SphereOfActivity(workBook, sheet, ref currentRow, sphereOfActivity.First());
                }

                StateTaskFormHelper.Сustomer(workBook, sheet, ref currentRow, doc.RefUchr.RefOrgGRBS.Name);

                StateTaskFormHelper.Performer(workBook, sheet, ref currentRow, "{0}, {1}".FormatWith(doc.RefUchr.Name, passport.Adr));

                part = 1;
                foreach (var service in services)
                {
                    StateTaskFormHelper.Part(workBook, sheet, ref currentRow, part);

                    StateTaskFormHelper.Service(workBook, sheet, ref currentRow, service);

                    var item = service;
                    var consumers = newRestService.GetItems<F_F_PotrYs>().Where(x => x.RefVedPP.ID == item.RefVedPch.ID).ToList();

                    StateTaskFormHelper.Сonsumers(workBook, sheet, ref currentRow, service, consumers);

                    var indicators = service.Indicators.ToList();
                    StateTaskFormHelper.Indicators(workBook, sheet, ref currentRow, service, indicators);

                    StateTaskFormHelper.OrderOfService(workBook, sheet, ref currentRow, service);

                    StateTaskFormHelper.Tariffs(workBook, sheet, ref currentRow, service);

                    StateTaskFormHelper.ReasonsForEarlyTermination(workBook, sheet, ref currentRow, service);

                    StateTaskFormHelper.ControlOrder(workBook, sheet, ref currentRow, service);

                    StateTaskFormHelper.FormOfReport(workBook, sheet, ref currentRow, service);

                    StateTaskFormHelper.DeadlineSubmissionReports(workBook, sheet, ref currentRow, service);

                    part++;
                }
            }

            if (works.Count != 0)
            {
                StateTaskFormHelper.MainHeader(workBook, sheet, ref currentRow, false);

                if (sphereOfActivity.Count == 1)
                {
                    StateTaskFormHelper.SphereOfActivity(workBook, sheet, ref currentRow, sphereOfActivity.First());
                }

                StateTaskFormHelper.Сustomer(workBook, sheet, ref currentRow, doc.RefUchr.RefOrgGRBS.Name);

                StateTaskFormHelper.Performer(workBook, sheet, ref currentRow, "{0}, {1}".FormatWith(doc.RefUchr.Name, passport.Adr));

                part = 1;
                foreach (var work in works)
                {
                    StateTaskFormHelper.Part(workBook, sheet, ref currentRow, part);

                    StateTaskFormHelper.Service(workBook, sheet, ref currentRow, work);

                    StateTaskFormHelper.CharacteristicsWorksReports(workBook, sheet, ref currentRow, work);

                    StateTaskFormHelper.ReasonsForEarlyTermination(workBook, sheet, ref currentRow, work);

                    StateTaskFormHelper.ControlOrder(workBook, sheet, ref currentRow, work);

                    StateTaskFormHelper.FormOfReport(workBook, sheet, ref currentRow, work);

                    StateTaskFormHelper.DeadlineSubmissionReports(workBook, sheet, ref currentRow, work);

                    part++;
                }
            }

            StateTaskFormHelper.ClearStyles();
            return workBook;
        }
    }
}