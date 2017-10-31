using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Krista.FM.Client.Common;
using Krista.FM.Client.Reports.Common.Commands;

namespace Krista.FM.Client.ViewObjects.ReportsUI.Common
{
    public class ReportService
    {
        private readonly Dictionary<string, Type> reportsTypes;
        private readonly Dictionary<string, CommonReportsCommand> reportCommands;

        public ReportService()
        {
            reportsTypes = new Dictionary<string, Type>();
            reportCommands = new Dictionary<string, CommonReportsCommand>();
        }

        /// <summary>
        /// поиск типа для создания отчета
        /// </summary>
        /// <param name="reportKey"></param>
        /// <returns></returns>
        private Type GetReportType(string reportKey)
        {
            if (reportsTypes.ContainsKey(reportKey))
            {
                return reportsTypes[reportKey];
            }

            var ass = Assembly.Load("Krista.FM.Client.Reports");
            foreach (var type in ass.GetTypes())
            {
                foreach (DescriptionAttribute attribute in type.GetCustomAttributes(typeof(DescriptionAttribute), false))
                {
                    if (String.Compare(attribute.Description, reportKey, true) == 0)
                    {
                        reportsTypes.Add(reportKey, type);
                        return type;
                    }
                }
            }
            return null;
        }

        public void CreateReport(string key, IWorkplace workplace)
        {
            var type = GetReportType(key);
            
            if (type == null)
            {
                return;
            }

            if (!reportCommands.ContainsKey(key))
            {
                // создание экземпляра типа и вызов его функции создания отчета
                var obj = Activator.CreateInstance(type) as CommonReportsCommand;
                
                if (obj != null)
                {
                    obj.operationObj = workplace.OperationObj;
                    obj.scheme = workplace.ActiveScheme;
                    obj.window = workplace.WindowHandle;
                }

                reportCommands.Add(key, obj);
            }

            var cmd = reportCommands[key];

            if (cmd.CheckReportTemplate())
            {
                cmd.Run();
            }
        }
    }
}
