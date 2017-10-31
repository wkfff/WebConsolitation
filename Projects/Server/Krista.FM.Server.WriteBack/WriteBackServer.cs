using System;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.ServerLibrary;
using Krista.FM.WriteBackLibrary;

namespace Krista.FM.Server.WriteBack
{
    public sealed class WriteBackServerClass : DisposableObject, IWriteBackServer
    {
        private static IScheme _scheme;

        public static IScheme Scheme
        {
            get { return _scheme; }
        }

        public static string ConcatenateChar;

        public WriteBackServerClass(IScheme scheme)
        {
            _scheme = scheme;

            if (scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleClient || 
				scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.OracleDataAccess ||
				scheme.SchemeDWH.FactoryName == ProviderFactoryConstants.MSOracleDataAccess)
                ConcatenateChar = "||";
            else
                ConcatenateChar = "+";

            WriteBackServerProcess.Run();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WriteBackServerProcess.Stop();
            }
            base.Dispose(disposing);
        }

        public override void Close()
        {
            Dispose(true);
        }

        public void Activate()
        {
            Trace.TraceVerbose("Объект сервера активирован");
            return;
        }

        /// <summary>
        /// инициализация XML документа
        /// </summary>
        internal static XmlDocument CreateXMLDocument()
        {
            XmlDocument xmlDocument = new XmlDocument();
            string captionValue = "version='1.0' encoding='windows-1251'";
            string captionElement = "xml";
            XmlNode ProcessingNode = xmlDocument.CreateProcessingInstruction(captionElement, captionValue);
            xmlDocument.AppendChild(ProcessingNode);
            //XmlNode xmlNode = xmlDocument.CreateElement("Response");
            //xmlDocument.AppendChild(xmlNode);

            return xmlDocument;
        }


        #region IWriteBackServer Members

        public string ProcessQuery(string queryString)
        {
            // создаем результирующий документ
            XmlDocument resultXmlDocument = CreateXMLDocument();

            queryString = queryString.Replace("&lt;", "<");
            queryString = queryString.Replace("&gt;", ">");
            queryString = queryString.Replace("&amp;", "&");
            queryString = queryString.Replace("&apos;", "'");
            queryString = queryString.Replace("&quot;", "\"");

            Trace.TraceInformation("{0} Обратная запись. Пользователь {1}, сессия {2}", 
                DateTime.Now, Authentication.UserName, SessionContext.SessionId);
            try
            {
                RequestData request = new RequestData();
                request.Data = Validator.LoadDocument(queryString);
                XmlDocument xmlResult = WriteBackServerProcess.Request(request);
                return xmlResult == null ? "" : xmlResult.InnerXml;
            }
            catch (Exception e)
            {
                Trace.TraceError("Ошибка обработки запроса обратной записи: {0}", e.Message);

                resultXmlDocument.InnerXml = String.Format("<Exception><Message><![CDATA[{0}]]></Message><Source><![CDATA[{1}]]></Source><StackTrace><![CDATA[{2}]]></StackTrace></Exception>", Krista.FM.Common.Exceptions.FriendlyExceptionService.GetFriendlyMessage(e).Message, e.Source, e.StackTrace);
                return resultXmlDocument.InnerXml;
            }
        }

        #endregion
    }
}
