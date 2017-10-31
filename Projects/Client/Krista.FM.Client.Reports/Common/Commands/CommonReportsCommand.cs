using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Common.Gui;
using Krista.FM.Client.Reports.Common.CommonParamForm;
using Krista.FM.Common.ReportHelpers;
using Krista.FM.Common.Templates;
using Krista.FM.ServerLibrary;
using Krista.FM.ServerLibrary.TemplatesService;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;

namespace Krista.FM.Client.Reports.Common.Commands
{
    public class CommonReportsCommand : AbstractCommand
    {
        public delegate string CheckParams(Dictionary<string, string> reportParams);
        public ReportDataServer reportServer;
        public ParamContainer paramBuilder;
        private bool isFirstRun = true;
        public IScheme scheme;
        public IWin32Window window;
        public Operation operationObj;
        public CheckParams paramChecker;

        protected Dictionary<string, object> reportOuterParams = new Dictionary<string, object>();

        public void SetReportParamValue(string paramKey, object value)
        {
            if (reportOuterParams.ContainsKey(paramKey))
            {
                reportOuterParams[paramKey] = value;
            }
            else
            {
                reportOuterParams.Add(paramKey, value);
            }
        }

        protected virtual bool UseNPOI()
        {
            return false;
        }

        protected virtual OfficeReportsHelper CreateOfficeObject()
        {
            return new ExcelReportHelper(scheme);
        }

        private DataRow GetCurrentReportRow(string value, string fieldName)
        {
            TemplatesStore.scheme = scheme;
            var dtTemplates = TemplatesStore.dtTemplates;

            if (dtTemplates.Columns.Contains(fieldName))
            {
                var filterStr = "{0} = '{1}'";

                if (dtTemplates.Columns[fieldName].DataType != typeof(String))
                {
                    filterStr = "{0} = {1}";
                }

                var dtSelect = dtTemplates.Select(String.Format(filterStr, fieldName, value));

                if (CheckRowExists(dtSelect))
                {
                    return dtSelect[0];
                }
            }

            return null;
        }

        private static bool CheckRowExists(ICollection<DataRow> rows)
        {
            return rows != null && rows.Count > 0;
        }

        public virtual bool CheckReportTemplate()
        {
            var templateRow = GetCurrentReportRow(key, TemplateFields.Code);

            if (templateRow != null)
            {
                caption = Convert.ToString(templateRow[TemplateFields.Name]);
            }

            return templateRow != null;
        }

        public virtual int GetReportSortIndex()
        {
            var templateRow = GetCurrentReportRow(key, TemplateFields.Code);

            if (templateRow != null && templateRow[TemplateFields.SortIndex] != DBNull.Value)
            {
                return Convert.ToInt32(templateRow[TemplateFields.SortIndex]);
            }

            return 0;
        }

        public virtual Dictionary<string, string> GetParentPath()
        {
            var result = new Dictionary<string, string>();
            var keys = new Collection<string>();
            var vals = new Collection<string>();
            // инверсивная структура для хранения пути
            var templateRow = GetCurrentReportRow(key, TemplateFields.Code);

            if (templateRow != null)
            {
                while (templateRow[TemplateFields.ParentID] != DBNull.Value)
                {
                    templateRow = GetCurrentReportRow(
                        Convert.ToString(templateRow[TemplateFields.ParentID]),
                        TemplateFields.ID);

                    if (templateRow == null)
                    {
                        continue;
                    }

                    keys.Insert(0, String.Format("{0}={1}={2}",
                                                 templateRow[TemplateFields.ID],
                                                 templateRow[TemplateFields.ParentID],
                                                 templateRow[TemplateFields.SortIndex]));
                    vals.Insert(0, Convert.ToString(templateRow[TemplateFields.Name]));
                }
            }

            for (var i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], vals[i]);
            }

            return result;
        }

        public virtual int GetImageIndex()
        {
            return 1;
        }

        public virtual void CreateReportParams()
        {
        }

        public virtual string GetCorrectedCaption()
        {
            var correctedCaption = caption;
            var serviceSubstrLen1 = correctedCaption.IndexOf(')');
            var serviceSubstrLen2 = correctedCaption.IndexOf('(');

            if (serviceSubstrLen1 > 0 && serviceSubstrLen2 >= 0)
            {
                if (serviceSubstrLen2 > 0)
                {
                    var startSubstr = correctedCaption.Substring(0, serviceSubstrLen2);
                    var onlyDigits = true;

                    for (var i = 0; i < startSubstr.Length; i++)
                    {
                        onlyDigits = onlyDigits && (
                            Char.IsDigit(startSubstr[i]) || 
                            startSubstr[i] == '-' || 
                            startSubstr[i] == ' ' ||
                            startSubstr[i] == '_');
                    }

                    if (!onlyDigits)
                    {
                        return caption;
                    }
                }

                correctedCaption = correctedCaption.Remove(0, serviceSubstrLen1 + 1);
            }

            return correctedCaption.Length == 0 ? caption : correctedCaption;
        }

        private Dictionary<string, string> GetFullParamList()
        {
            var totalParamList = paramBuilder.GetParams().ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var pair in reportOuterParams)
            {
                var paramValue = Convert.ToString(pair.Value);

                if (!totalParamList.ContainsKey(pair.Key))
                {
                    totalParamList.Add(pair.Key, paramValue);
                }
                else
                {
                    totalParamList[pair.Key] = paramValue;
                }
            }
            return totalParamList;
        }

        public virtual DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return null;
        }

        public override void Run()
        {
            ConvertorSchemeLink.scheme = scheme;
            var hasSelection = true;
            ParamStore.container = paramBuilder;

            if (isFirstRun)
            {
                paramBuilder = new ParamContainer();
                ParamStore.container = paramBuilder;
                CreateReportParams();
            }

            if (paramBuilder.lstParams.Count > 0)
            {
                var paramSaver = new ReportParamSaver(GetType().Name, paramBuilder);
                paramSaver.LoadParams();
                var formParams = new ReportParamForm();
                hasSelection = formParams.ShowForm(window, paramBuilder);

                if (hasSelection)
                {
                    paramSaver.SaveParams();
                }
            }

            isFirstRun = false;

            if (hasSelection && paramChecker != null)
            {
                var messageStr = paramChecker(paramBuilder.GetParams());

                if (messageStr.Length > 0)
                {
                    MessageBox.Show(messageStr, "Внимание");
                    hasSelection = false;
                }
            }

            if (hasSelection)
            {
                operationObj.Text = "Получение и обработка данных";
                operationObj.StartOperation();
                try
                {
                    var templateRow = GetCurrentReportRow(key, TemplateFields.Code);

                    if (templateRow != null)
                    {
                        reportServer = new ReportDataServer
                                           {
                                               scheme = scheme 
                                           };

                        ParamStore.container = paramBuilder;

                        var dtReportData = GetReportData(GetFullParamList());

                        if (dtReportData == null || dtReportData.Length == 0)
                        {
                            return;
                        }

                        var documentsHelper = new TemplatesDocumentsHelper(scheme.TemplatesService.Repository);

                        if (UseNPOI())
                        {
                            var templateDocumentName = documentsHelper.SaveDocument(
                                Convert.ToInt32(templateRow[TemplateFields.ID]),
                                Convert.ToString(templateRow[TemplateFields.Name]),
                                Convert.ToString(templateRow[TemplateFields.DocumentFileName]));

                            var wb = NPOICreateWorkBook(templateDocumentName);
                            NPOIFillReport(wb, dtReportData);
                            var reportFileName = NPOICloseWorkBook(wb, templateDocumentName);
                            Process.Start(reportFileName);
                        }
                        else
                        {
                            var templateDocumentName = documentsHelper.SaveDocument(
                                Convert.ToInt32(templateRow[TemplateFields.ID]),
                                GetCorrectedCaption(),
                                Convert.ToString(templateRow[TemplateFields.DocumentFileName]));

                            using (var helper = CreateOfficeObject())
                            {
                                helper.CreateReport(templateDocumentName, dtReportData);
                                helper.ShowReport();
                            }
                        }
                    }
                }
                finally
                {
                    operationObj.StopOperation();
                }
            }
        }

        protected HSSFWorkbook NPOICreateWorkBook(string templateName)
        {
            HSSFWorkbook wb;

            using (var fs = new FileStream(templateName, FileMode.Open, FileAccess.Read))
            {
                wb = new HSSFWorkbook(fs, true);
            }

            return wb;
        }

        protected virtual string NPOIFileName(string templateName)
        {
            var filePrefix = 1;
            var strBuilder = new StringBuilder();
            strBuilder.Append(Path.GetDirectoryName(templateName));
            strBuilder.Append("\\");
            strBuilder.Append(Path.GetFileNameWithoutExtension(templateName));
            strBuilder.Append("{0}");
            strBuilder.Append(Path.GetExtension(templateName));
            var pathTemplate = strBuilder.ToString();

            while (File.Exists(String.Format(pathTemplate, filePrefix)))
            {
                filePrefix++;
            }

            return String.Format(pathTemplate, filePrefix);
        }

        protected virtual void NPOIFillReport(HSSFWorkbook wb, DataTable[] tableList)
        {
        }

        protected virtual string NPOICloseWorkBook(HSSFWorkbook wb, string templateName)
        {
            var dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI";
            wb.DocumentSummaryInformation = dsi;
            var si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI";
            wb.SummaryInformation = si;
            var fileName = NPOIFileName(templateName);

            using (var file = new FileStream(fileName, FileMode.Create))
            {
                wb.Write(file);
                file.Close();
            }

            return fileName;
        }
    }
}
