using System;
using System.Collections.Generic;

using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Models.DocumentsRegisterModel;
using Krista.FM.RIA.Extensions.E86N.Utils;

using NPOI.HSSF.UserModel;

namespace Krista.FM.RIA.Extensions.E86N.Services.Reports
{
    public interface IReportService
    {
        HSSFWorkbook InitializeWorkBookWithoutTemplate(List<string> headers, List<ReportsHelper.Column> columns, List<DocumentsRegisterViewModel> data);

        HSSFWorkbook GetStateTaskForm(int docId);

        HSSFWorkbook GetStateTaskForm2016(int docId);

        HSSFWorkbook GetDocReport(FX_FX_PartDoc partDoc, DateTime reportDate, bool isPPO, string tmpFileName, string docYear);

        HSSFWorkbook GetAnalReport(DateTime reportDate);

        HSSFWorkbook GetNewAnalReport(DateTime reportDate, bool isPPO);

        HSSFWorkbook GetMonitoringPlacementInfoReport(DateTime reportDate, string docYear);
    }
}
