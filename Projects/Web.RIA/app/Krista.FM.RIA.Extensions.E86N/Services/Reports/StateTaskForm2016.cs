using System;
using System.Collections.Generic;
using System.Linq;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;
using Krista.FM.RIA.Extensions.E86N.Utils;

using Microsoft.Practices.ObjectBuilder2;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public class StateTaskForm2016
    {
        public const int LastCol = 7;
        
        private readonly F_F_ParameterDoc doc;

        private readonly INewRestService newRestService;

        private int currentRow;

        public StateTaskForm2016(int docId)
        {
            newRestService = Resolver.Get<INewRestService>();
            this.doc = newRestService.GetItem<F_F_ParameterDoc>(docId);
        }

        public HSSFWorkbook GetStateTaskForm()
        {
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

            var data = this.doc.StateTasks2016.ToList();
           
            var workBook = new HSSFWorkbook();
            var sheet = workBook.CreateSheet("лист 1");
            SetDefaultStyle(workBook, sheet);
            
            MainHeader(workBook, sheet);
            Okved(workBook, sheet, passport);
            this.Parts(workBook, sheet, data);

            StateTaskFormHelper.ClearStyles();
            return workBook;
        }
        
        private void Parts(HSSFWorkbook workBook, HSSFSheet sheet, List<F_F_GosZadanie2016> data)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.FontHeightInPoints = 12;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;

            var services = data.Where(x => x.RefService.RefType.ID == FX_FX_ServiceType.IdOfService).ToList();
            var works = data.Where(x => x.RefService.RefType.ID == FX_FX_ServiceType.IdOfWork).ToList();

            NpoiHelper.SetCellValue(sheet, currentRow, 1, "ЧАСТЬ 1.").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Сведения об оказываемых государственных услугах").CellStyle = cellStyle;
            ++currentRow;
            if (services.Any())
            {
                int counter = 1;

                services.Each(
                    x =>
                        {
                            NpoiHelper.SetCellValue(sheet, currentRow, 1, "РАЗДЕЛ").CellStyle = cellStyle;
                            NpoiHelper.SetCellValue(sheet, currentRow, 2, counter++).CellStyle = cellStyle;
                            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 2);
                            currentRow += 2;
                            this.SetHeaderServiceData(workBook, sheet, x, true);
                            ++currentRow;
                            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Показатели качества государственной услуги:").CellStyle = cellStyle;
                            currentRow += 2;
                            this.Indicators(workBook, sheet, x, FX_FX_CharacteristicType.QualityIndex);
                            currentRow += 2;
                            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Показатели объема государственной услуги:").CellStyle = cellStyle;
                            currentRow += 2;
                            this.Indicators(workBook, sheet, x, FX_FX_CharacteristicType.VolumeIndex);
                            currentRow += 2;
                            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Сведения о платных услугах в составе задания:").CellStyle = cellStyle;
                            currentRow += 2;
                            PaidService(workBook, sheet, x);
                            currentRow += 2;
                            NpoiHelper.SetCellValue(
                                sheet,
                                currentRow, 
                                0, 
                                "Порядок оказания государственной услуги (перечень и реквизиты нормативных правовых актов, регулирующих порядок оказания государственной услуги):").CellStyle = cellStyle;
                            currentRow += 2;
                            Npa(workBook, sheet, x);
                            currentRow += 2;
                        });
            }
            else
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 2, "Учреждение услуги не оказывает").CellStyle = cellStyle;
                ++currentRow;
            }
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "ЧАСТЬ 2.").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Сведения о выполняемых работах").CellStyle = cellStyle;
            ++currentRow;
            if (works.Any())
            {
                int counter = 1;

                works.Each(
                    x =>
                    {
                        NpoiHelper.SetCellValue(sheet, currentRow, 1, "РАЗДЕЛ").CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 2, counter++).CellStyle = cellStyle;
                        StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 2);
                        currentRow += 2;
                        this.SetHeaderServiceData(workBook, sheet, x, false);
                        currentRow += 2;
                        NpoiHelper.SetCellValue(sheet, currentRow, 0, " Показатели качества работы:").CellStyle = cellStyle;
                        currentRow += 2;
                        this.Indicators(workBook, sheet, x, FX_FX_CharacteristicType.QualityIndex);
                        currentRow += 2;
                        NpoiHelper.SetCellValue(sheet, currentRow, 0, "  Показатели объема работы:").CellStyle = cellStyle;
                        currentRow += 2;
                        this.Indicators(workBook, sheet, x, FX_FX_CharacteristicType.VolumeIndex);
                        currentRow += 2;
                    });
            }
            else
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 2, "Учреждение работы не выполняет.").CellStyle = cellStyle;
                ++currentRow;
            }

            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "ЧАСТЬ 3.").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Прочие сведения о государственном задании:").CellStyle = cellStyle;
            ++currentRow;
            OtherInfo(workBook, sheet);
        }

        private void OtherInfo(HSSFWorkbook workBook, HSSFSheet sheet)
        {
            SetOtherInfoData(workBook, sheet, "№ п/п", "Наименование", "Требования");
            SetOtherInfoData(
                workBook,
                sheet,
                "1.",
                "Основания для приостановления и/или досрочного прекращениявыполнения государственного задания",
                this.doc.StateTasksTerminations2016.Select(x => x.EarlyTerminat).JoinStrings(";"));
            SetOtherInfoData(
                workBook,
                sheet,
                "2.",
                "Порядок контроля учредителем выполнения государственного задания");
            SetOtherInfoData(
                workBook,
                sheet,
                "3",
                "Требования к отчетности о выполнении государственного задания",
                string.Empty);
            SetOtherInfoData(
                workBook,
                sheet,
                "3.1",
                "Периодичность представления отчетов о выполнении государственного задания",
                this.doc.StateTasksRequestAccounts2016.Select(x => x.PeriodicityTerm).JoinStrings(";"));
            SetOtherInfoData(
                workBook,
                sheet,
                "3.2",
                "Сроки представления отчетов о выполнении государственного задания",
                this.doc.StateTasksRequestAccounts2016.Select(x => x.DeliveryTerm).JoinStrings(";"));
            SetOtherInfoData(
                workBook,
                sheet,
                "3.3",
                "Иные показатели отчетности о выполнении государственного задания",
                this.doc.StateTasksRequestAccounts2016.Select(x => x.OtherIndicators).JoinStrings(";"));
            SetOtherInfoData(
                workBook,
                sheet,
                "3.4",
                "Иные требования к отчетности о выполнении государственного задания",
                this.doc.StateTasksRequestAccounts2016.Select(x => x.OtherRequest).JoinStrings(";"));
            SetOtherInfoData(
                workBook,
                sheet,
                "4.",
                "Иная информация, связанные с выполнением государсвенного задания",
                this.doc.StateTasksOtherInfo.Select(x => x.OtherInfo).JoinStrings(";"));
        }

        private void SetOtherInfoData(HSSFWorkbook workBook, HSSFSheet sheet, string npp, string name, string requirements)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, npp).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, name).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, requirements).CellStyle = cellStyle;
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 1, currentRow, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 3);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 4, currentRow, LastCol);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, LastCol);
            ++currentRow;
        }

        private void SetOtherInfoData(HSSFWorkbook workBook, HSSFSheet sheet, string npp, string name)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, npp).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, name).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "Формы контроля").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "Периодичность").CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 4, currentRow, 5);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 5);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 6, currentRow, LastCol);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, LastCol);

            this.doc.StateTasksSupervisionProcedures2016.Each(
                x =>
                    {
                        ++currentRow;
                        NpoiHelper.SetCellValue(sheet, currentRow, 4, x.Form).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 6, x.Rate).CellStyle = cellStyle;
                        NpoiHelper.SetMergedRegion(sheet, currentRow, 4, currentRow, 5);
                        NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 5);
                        NpoiHelper.SetMergedRegion(sheet, currentRow, 6, currentRow, LastCol);
                        NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, LastCol);
                    });

            var rows = this.doc.StateTasksSupervisionProcedures2016.Count;

            NpoiHelper.SetMergedRegion(sheet, currentRow - rows, 0, currentRow, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow - rows, 0, currentRow, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow - rows, 0, currentRow, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow - rows, 0, currentRow, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow - rows, 1, currentRow, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow - rows, 1, currentRow, 3);

            ++currentRow;
        }

        private void Npa(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service)
        {
            NpaHeader(workBook, sheet);
            NpaValues(workBook, sheet, service);
        }

        private void NpaValues(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);

            service.RenderOrders.Each(
                x =>
                {
                    NpoiHelper.SetCellValue(sheet, currentRow, 0, service.RefService.Regrnumber).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, currentRow, 1, service.RefService.SvcCntsName1Val + ", " + service.RefService.SvcCntsName2Val + ", " + service.RefService.SvcCntsName3Val).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, currentRow, 2, service.RefService.SvcTermsName1Val + ", " + service.RefService.SvcTermsName2Val).CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, currentRow, 3, x.TypeNpa + " " + x.Author + " \"" + x.RenderEnact + "\"").CellStyle = cellStyle;
                    NpoiHelper.SetCellValue(sheet, currentRow, 4, "№{0} от {1}".FormatWith(x.NumberNpa, x.DateNpa.HasValue ? x.DateNpa.Value.ToString("dd.MM.yyyy") : string.Empty)).CellStyle = cellStyle;
                    
                    for (var i = 0; i <= 4; i++)
                    {
                        NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
                    }

                    ++currentRow;
                });
        }

        private void NpaHeader(HSSFWorkbook workBook, HSSFSheet sheet)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Уникальный номер реестровой записи").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Содержание государственной услуги").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Условия (формы) оказания государственной услуги").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                3,
                "Рекцизиты нормативного праового акта, регулирующего порядок оказания государственной услуги").CellStyle = cellStyle;
            sheet.GetRow(this.currentRow).Height = 1000;
            
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 3, currentRow, 4);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 4);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "наименование (вид,  принявший орган, наименование)").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "дата, номер").CellStyle = cellStyle;
            
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 4);
            
            ++currentRow;
            for (int i = 0; i <= 4; i++)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, i, i + 1).CellStyle = cellStyle;
                NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
            }
            ++currentRow;
        }

        private void PaidService(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.FontHeightInPoints = 12;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;

            if (service.RefService.RefPay.ID == FX_FX_ServicePayType2.IdOfFree)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 0, "Услуга бесплатная").CellStyle = cellStyle;
                return;
            }

            PaidServiceHeader(workBook,sheet);
            PaidServiceValues(workBook, sheet, service);
        }

        private void PaidServiceValues(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, service.RefService.Regrnumber).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, service.RefService.SvcCntsName1Val + ", " + service.RefService.SvcCntsName2Val + ", " + service.RefService.SvcCntsName3Val).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, service.RefService.SvcTermsName1Val + ", " + service.RefService.SvcTermsName2Val).CellStyle = cellStyle;
            var row = service.Prices.FirstOrDefault();
            if (row != null)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 3, row.VidNPAGZ + " " + row.OrgUtvDoc + "  \"" + row.Name + "\"").CellStyle = cellStyle;
                NpoiHelper.SetCellValue(
                    sheet,
                    currentRow,
                    4,
                    "№{0} от {1}".FormatWith(row.NumNPA, row.DataNPAGZ.HasValue ? row.DataNPAGZ.Value.ToString("dd.MM.yyyy") : string.Empty)).CellStyle = cellStyle;
            }
            var rowAve = this.newRestService.GetItems<F_F_AveragePrice>().FirstOrDefault(x => x.RefVolumeIndex.RefFactGZ.ID == service.ID);
            if (rowAve != null)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 5, rowAve.NextYearDec).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, currentRow, 6, rowAve.PlanFirstYearDec).CellStyle = cellStyle;
                NpoiHelper.SetCellValue(sheet, currentRow, 7, rowAve.PlanLastYearDec).CellStyle = cellStyle;
            }
            for (int i = 0; i <= LastCol; i++)
            {
                NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
            }
        }

        private void PaidServiceHeader(HSSFWorkbook workBook, HSSFSheet sheet)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Уникальный номер реестровой записи").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "Содержание государственной услуги").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Условия (формы) оказания государственной услуги").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(
                sheet, 
                currentRow, 
                3, 
                "Реквизиты нормативно правового акта, устанавливающего размер платы (цену, тариф) либо порядок её (его) установления").CellStyle = cellStyle;
            sheet.GetRow(this.currentRow).Height = 1000;
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "Среднегодовой размер платы (цена, тариф)").CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 3, currentRow, 4);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 4);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 5, currentRow, LastCol);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, LastCol);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "наименование (вид,  принявший орган, наименование)").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "дата, номер").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "{0} год".FormatWith(this.doc.RefYearForm.ID)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "{0} год".FormatWith(this.doc.RefYearForm.ID + 1)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "{0} год".FormatWith(this.doc.RefYearForm.ID + 2)).CellStyle = cellStyle;

            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 4);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, 5);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 6);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 7);
            ++currentRow;
            for (int i = 0; i <= LastCol; i++)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, i, i + 1).CellStyle = cellStyle;
                NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
            }
            ++currentRow;
        }

        private void Indicators(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service, int indicatorType)
        {
            this.IndicatorsHeader(workBook, sheet, service.RefService.RefType.ID, indicatorType);
            this.IndicatorsValues(workBook, sheet, service, indicatorType);
        }

        private void IndicatorsValues(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service, int indicatorType)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);

            service.Indicators.Where(x => x.RefIndicators.RefType.ID == indicatorType).Each(
                x =>
                    {
                        NpoiHelper.SetCellValue(sheet, currentRow, 0, service.RefService.Regrnumber).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 1, service.RefService.SvcCntsName1Val + ", " + service.RefService.SvcCntsName2Val + ", " + service.RefService.SvcCntsName3Val).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 2, service.RefService.SvcTermsName1Val + ", " + service.RefService.SvcTermsName2Val).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 3, x.RefIndicators.Name).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 4, x.RefIndicators.RefOKEI.Name).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 5, x.ComingYear).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 6, x.FirstPlanYear).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 7, x.SecondPlanYear).CellStyle = cellStyle;

                        for (var i = 0; i <= LastCol; i++)
                        {
                            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
                        }
                        ++currentRow;
                        var indType = indicatorType == FX_FX_CharacteristicType.QualityIndex ? "качества" : "объема";
                        NpoiHelper.SetCellValue(sheet, currentRow, 0, "Допустимые (возможные) отклонения от установленных показателей {0}".FormatWith(indType)).CellStyle = cellStyle;
                        NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, 3);
                        NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 3);

                        NpoiHelper.SetCellValue(sheet, currentRow, 4, x.RefIndicators.RefOKEI.Symbol).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 5, x.Deviation).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 6, x.Deviation).CellStyle = cellStyle;
                        NpoiHelper.SetCellValue(sheet, currentRow, 7, x.Deviation).CellStyle = cellStyle;
                        for (var i = 4; i <= LastCol; i++)
                        {
                            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
                        }
                    });
         }

        private void IndicatorsHeader(HSSFWorkbook workBook, HSSFSheet sheet, int serviceType, int indicatorType)
        {
            var indType = indicatorType == FX_FX_CharacteristicType.QualityIndex ? "качества" : "объема";
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Уникальный номер реестровой записи").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                1,
                serviceType == FX_FX_ServiceType.IdOfService ? "Содержание государственной услуги" : "Содержание работы").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                2,
                serviceType == FX_FX_ServiceType.IdOfService ? "Условия (формы) оказания государственной услуги" : "Условия (формы) оказания работы").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                3,
                serviceType == FX_FX_ServiceType.IdOfService 
                    ? "Показатели {0} государственной услуги".FormatWith(indType) 
                    : "Показатели {0} работы".FormatWith(indType)).CellStyle = cellStyle;
            sheet.GetRow(this.currentRow).Height = 600;
            NpoiHelper.SetCellValue(
                sheet,
                currentRow,
                5,
                serviceType == FX_FX_ServiceType.IdOfService
                    ? "Значение показателя {0} государственной услуги".FormatWith(indType)
                    : "Значение показателя качества работы".FormatWith(indType)).CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow + 1, 0);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 1, currentRow + 1, 1);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow + 1, 2);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 3, currentRow, 4);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 4);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 5, currentRow, LastCol);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, LastCol);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 3, "наименование показателя").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "единица измерения").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 5, "{0} год".FormatWith(this.doc.RefYearForm.ID)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "{0} год".FormatWith(this.doc.RefYearForm.ID + 1)).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 7, "{0} год".FormatWith(this.doc.RefYearForm.ID + 2)).CellStyle = cellStyle;
            
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 3, currentRow, 3);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 4);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 5, currentRow, 5);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 6, currentRow, 6);
            NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, 7, currentRow, 7);
            ++currentRow;
            for (int i = 0; i <= LastCol; i++)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, i, i + 1).CellStyle = cellStyle;
                NpoiHelper.SetBorderBoth(workBook, sheet, currentRow, i, currentRow, i);
            }
            ++currentRow;
        }

        private void SetRowHeaderServiceDate(HSSFWorkbook workBook, HSSFSheet sheet, string col1, string col2)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            NpoiHelper.SetCellValue(sheet, currentRow, 0, col1).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, col2).CellStyle = cellStyle;
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 3);
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, LastCol);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, 3);
            NpoiHelper.SetMergedRegion(sheet, currentRow, 4, currentRow, LastCol);
            ++currentRow;
        }

        private void SetHeaderServiceData(HSSFWorkbook workBook, HSSFSheet sheet, F_F_GosZadanie2016 service, bool serviceType)
        {
            SetRowHeaderServiceDate(workBook, sheet, serviceType ? "Наименование государственной услуги" : "Наименование работы", service.RefService.NameName);
            SetRowHeaderServiceDate(
                workBook,
                sheet,
                serviceType ? "Код услуги по базовому (отраслевому) перечню" : "Код работы по базовому (отраслевому) перечню",
                service.RefService.NameCode.Insert(2, ".").Insert(6, "."));
            SetRowHeaderServiceDate(
                workBook,
                sheet,
                serviceType ? "ОКВЭД услуги по базовому (отраслевому) перечню" : "ОКВЭД работы по базовому (отраслевому) перечню",
                service.RefService.Okveds.Select(x => x.Code + "-" + x.Name).JoinStrings(";\n\r"));
            SetRowHeaderServiceDate(
                workBook,
                sheet,
                serviceType ? "Категории потребителей государственной услуги" : "Категории потребителей работы",
                service.RefService.ConsumersCategoryes.Select(x => x.Name).JoinStrings(";\n\r"));
        }
        
        private void MainHeader(HSSFWorkbook workBook, HSSFSheet sheet)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.FontHeightInPoints = 12;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
            cellStyle.WrapText = true;

            NpoiHelper.SetCellValue(sheet, currentRow, 4, "УТВЕРЖДЕНА");
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "приказом");
            var attach = doc.Documents.FirstOrDefault();
            if (attach != null)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 5, attach.ProoveOrg);
            }
            ++currentRow;
            StateTaskFormHelper.SetBorderTop(workBook, sheet, currentRow, 4, 7);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "от");
            if (attach != null)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 5, attach.DocDate.HasValue ? attach.DocDate.Value.ToString("dd.MM.yyyy") : string.Empty);
            }

            NpoiHelper.SetCellValue(sheet, currentRow, 6, "№");
            if (attach != null)
            {
                NpoiHelper.SetCellValue(sheet, currentRow, 7, attach.NumberNPA);
            }

            ++currentRow;
            StateTaskFormHelper.SetBorderTop(workBook, sheet, currentRow, 4, 7);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "ГОСУДАРСТВЕННОЕ ЗАДАНИЕ").CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, LastCol);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "на оказание государственных услуг (выполнение работ) в отношении государственных учреждений").CellStyle = cellStyle;
            NpoiHelper.SetMergedRegion(sheet, currentRow, 0, currentRow, LastCol);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, "Ярославской области").CellStyle = cellStyle;
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 2, 3);
            NpoiHelper.SetCellValue(sheet, currentRow, 4, "№ 000001").CellStyle = cellStyle;

            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 4, 4);
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 4, currentRow, 4);
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, this.doc.RefUchr.Name).CellStyle = cellStyle;
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 1, 5);
            NpoiHelper.SetCellValue(sheet, currentRow, 6, "на").CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 7, this.doc.RefYearForm.ID).CellStyle = cellStyle;
            ++currentRow;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, "(наименование учреждения)").CellStyle = cellStyle;
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 0, 5);
            StateTaskFormHelper.SetBorderTop(workBook, sheet, currentRow,  0, 5);
            StateTaskFormHelper.SetBorderTop(workBook, sheet, currentRow, 7, 7);
            ++currentRow;
        }

        private void Okved(HSSFWorkbook workBook, HSSFSheet sheet, F_Org_Passport passport)
        {
            var cellStyle = StateTaskFormHelper.GetDefaultStyle(workBook);
            var font = cellStyle.GetFont(workBook);
            font.FontHeightInPoints = 12;
            font.Boldweight = HSSFFont.BOLDWEIGHT_BOLD;
            cellStyle.Alignment = HSSFCellStyle.ALIGN_LEFT;
            
            NpoiHelper.SetCellValue(sheet, currentRow, 0, "Основные виды деятельности государственного учреждения:").CellStyle = cellStyle;
            ++currentRow;

            cellStyle = StateTaskFormHelper.GetDefaultStyleForData(workBook);
            
            SetOkvedData(workBook, sheet, cellStyle, "№  п/п", "Код ОКВЭД", "Наименование вида деятельности");
            ++currentRow;

            var count = 1;
            passport.Activity.Each(
                x =>
                    {
                        SetOkvedData(workBook, sheet, cellStyle, (count++).ToString(), x.RefOKVED.Code, x.RefOKVED.Name);
                        ++currentRow;
                    });

            ++currentRow;
        }

        private void SetOkvedData(HSSFWorkbook workBook, HSSFSheet sheet, HSSFCellStyle cellStyle, string npp, string code, string name)
        {
            NpoiHelper.SetCellValue(sheet, currentRow, 0, npp).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 1, code).CellStyle = cellStyle;
            NpoiHelper.SetCellValue(sheet, currentRow, 2, name).CellStyle = cellStyle;
            NpoiHelper.SetAlignCenterSelection(workBook, sheet, currentRow, 2, 5);
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 0, currentRow, 0);
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 1, currentRow, 1);
            StateTaskFormHelper.SetBorderBoth(workBook, sheet, currentRow, 2, currentRow, 5);
        }

        private void SetDefaultStyle(HSSFWorkbook workBook, HSSFSheet sheet)
        {
            sheet.SetColumnWidth(0, 5000);
            sheet.SetColumnWidth(1, 5000);
            sheet.SetColumnWidth(2, 5000);
            sheet.SetColumnWidth(3, 5000);
            sheet.SetColumnWidth(4, 5000);
            sheet.SetColumnWidth(5, 5000);
            sheet.SetColumnWidth(6, 5000);
            sheet.SetColumnWidth(7, 5000);
            
            var style = StateTaskFormHelper.GetDefaultStyle(workBook);

            for (short i = 0; i <= LastCol; i++)
            {
                sheet.SetDefaultColumnStyle(i, style);
            }
        }
    }
}