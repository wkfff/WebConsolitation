using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.Reports.EGRIP.Commands
{
    [Description("ReportUFKMO0011")]
    public class ReportEGRIP0001Command : ExcelMacrosCommand
    {
        public ReportEGRIP0001Command()
        {
            key = "ReportUFKMO0011";
            caption = "Выписка по ИП";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddListParam(ReportConsts.ParamReportList)
                .SetCaption("Разделы информации")
                .SetMultiSelect(true)
                .Values = new List<object>
                               {
                                   "По всем группам",
                                   "Основные сведения",
                                   "Регистрационные данные",
                                   "Сведения о данных физического лица",
                                   "Сведения о месте жительства в Российской Федерации",
                                   "Сведения о документе, удостоверяющего личность ",
                                   "Сведения о гражданстве",
                                   "Документ, подтверждающий право проживания в РФ",
                                   "Документ, подтверждающий приобретение дееспособности несовершеннолетним",
                                   "Сведения о видах экономической деятельности",
                                   "Сведения о постановке на учет в налоговом органе",
                                   "Сведения о регистрации в ПФ России",
                                   "Сведения о регистрации в ФСС России",
                                   "Сведения о регистрации в ФОМС России",
                                   "Сведения о лицензиях",
                                   "Сведения о счетах",
                                   "Сведения о записях в ЕГРИП"
                               };
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetEGRIP0001ReportData(reportParams);
        }
    }
}
