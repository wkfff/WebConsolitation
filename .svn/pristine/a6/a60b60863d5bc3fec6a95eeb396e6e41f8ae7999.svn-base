using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using Krista.FM.Client.Common.Forms;

namespace Krista.FM.Client.MDXExpert
{
    /// <summary>
    /// Обработчик исключений adomd
    /// </summary>
    public class AdomdExceptionHandler
    {
        const int TimeoutErrorCode = -2147217900;
        public static MainForm mainForm;

        private static bool isRepeatedProcess = false;

        /// <summary>
        /// Признак того что обрабатываемый процесс уже был до етого запущен
        /// </summary>
        public static bool IsRepeatedProcess
        {
            get { return isRepeatedProcess;}
            set { isRepeatedProcess = value; }
        }

        /// <summary>
        /// Обработка исключения
        /// </summary>
        /// <param name="exc">исключение</param>
        /// <returns>true - если обработка прошла успешно</returns>
        public static bool ProcessOK(Exception exc)
        {
            if ((mainForm == null) || (isRepeatedProcess))
            {
                return false;
            }
            

            bool result = false;

            try
            {

                if (exc is AdomdCacheExpiredException)
                {
                    mainForm.AdomdConn.RefreshMetadata();
                    return true;
                }
                else
                {
                    if (((exc is AdomdErrorResponseException) && !IsTimeout(exc)) || 
                        (exc is AdomdUnknownResponseException) || (exc is AdomdConnectionException))
                    {
                        result = mainForm.InitConnection();
                        return result;
                    }
                    else
                    //if (exc is InvalidOperationException)
                    {
                        result = mainForm.InitConnection();
                        if (result)
                        {
                            isRepeatedProcess = true;
                            mainForm.RefreshReportData();
                            isRepeatedProcess = false;

                        }
                        return result;
                    }

                }
            }
            catch
            {
            }
            
            return false;
        }

        /// <summary>
        /// Является ли исключение исключением adomd
        /// </summary>
        /// <param name="exc"></param>
        /// <returns></returns>
        public static bool IsAdomdException(Exception exc)
        {
            return (exc is AdomdException) ||
                   ((exc is InvalidOperationException) && (exc.Source == "Microsoft.AnalysisServices.AdomdClient"));
        }

        public static bool IsTimeout(Exception exc)
        {
            if (exc is AdomdErrorResponseException)
            {
                return (exc as AdomdErrorResponseException).ErrorCode == TimeoutErrorCode;
            }
            return false;
        }

    }
}
