using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.Reports.EGRUL.Commands
{
    [Description("ReportUFKMO0009")]
    public class ReportEGRUL0001Command : ExcelMacrosCommand
    {
        public ReportEGRUL0001Command()
        {
            key = "ReportUFKMO0009";
            caption = "Выписка по организации";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddListParam(ReportConsts.ParamReportList)
                .SetCaption("Разделы информации")
                .SetMultiSelect(true)
                .Values = new List<object>
                               {
                                   "По всем группам",
                                   "Основная информация о ЮЛ",
                                   "Сведения о наименовании юридического лица",
                                   "Сведения о постановке на учет в налоговом органе",
                                   "Сведения о состоянии юридического лица",
                                   "Сведения об адресе (месте нахождения) юридического лица",
                                   "Сведения о видах экономической деятельности",
                                   "Сведения об образовании юридического лица",
                                   "Сведения о прекращении деятельности",
                                   "Регистрирующий орган, в котором находится регистрационное дело",
                                   "Сведения о капитале",
                                   "Сведения об учредителях (физические лица)",
                                   "Сведения об учредителях - Российских ЮЛ",
                                   "Сведения об учредителях - иностранных ЮЛ",
                                   "Сведения о держателе реестра акционеров АО",
                                   "Сведения о юр.лицах - предшественниках при реорганизации",
                                   "Сведения о юр.лицах - преемниках при реорганизации",
                                   "Сведения о физ.лицах, имеющих право действовать без доверенности от имени юр.лица",
                                   "Сведения об управляющей компании",
                                   "Сведения об обособленных подразделениях организации (филиалах, представительствах)",
                                   "Сведения о лицензиях",
                                   "Сведения о счетах",
                                   "Сведения о регистрации в ПФ России",
                                   "Сведения о регистрации в ФСС России",
                                   "Сведения о регистрации в ФОМС России",
                                   "Сведения о записях в ЕГРЮЛ"
                               };
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetEGRUL0001ReportData(reportParams);
        }
    }
}
