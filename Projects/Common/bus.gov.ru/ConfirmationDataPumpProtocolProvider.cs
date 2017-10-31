using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using bus.gov.ru.external.Item1;
using bus.gov.ru.types.Item1;
using Krista.FM.Extensions;
using Krista.FM.ServerLibrary;

namespace bus.gov.ru
{
    public class ConfirmationDataPumpProtocolProvider : IDataPumpProtocol, IDataPumpProtocolProvider
    {
        public ConfirmationDataPumpProtocolProvider()
        {
           Confirmation = new confirmation
                {
                    header = new headerType
                       {
                           createDateTime = DateTime.Now,
                           id = Guid.NewGuid().ToString()
                       },
                    body = new packetResultType
                            {
                                result = "success",
                                violation = new List<violationType>()
                            }
                };
        }
        
        // протокол загрузки
        public confirmation Confirmation { get; set; }
        
        public int UserOperationID { get; set; }

        public DateTime MinProtocolsDate { get; set; }

        public void WriteEventIntoDataPumpProtocol(DataPumpEventKind EventKind, int PumpHistoryID, int DataSourceID, string InfoMsg)
        {
            DataPumpEventType eventType;
            switch (EventKind)
            {
                case DataPumpEventKind.dpeError:
                case DataPumpEventKind.dpeCriticalError:
                case DataPumpEventKind.dpeFinishFilePumpWithError:
                    eventType = DataPumpEventType.Error;
                    break;
                case DataPumpEventKind.dpeInformation:
                    eventType = DataPumpEventType.Info;
                    break;
                default:
                    eventType = DataPumpEventType.Warning;
                    break;
            }

            WriteEventIntoDataPumpProtocol(eventType, " ", InfoMsg);
        }

        public void WriteEventIntoDataPumpProtocol(DataPumpEventType eventType, string name, string description = "")
        {
            string level, code;
            switch (eventType)
            {
                case DataPumpEventType.Error:
                    level = "error";
                    code = "error";
                    Confirmation.body.result = "failure";
                    break;
                case DataPumpEventType.Warning:
                    level = "warning";
                    code = "warning";
                    break;
                case DataPumpEventType.Info:
                    level = "warning";
                    code = "info";
                    break;
                default:
                    level = "error";
                    code = "error";
                    Confirmation.body.result = "failure";
                    break;
            }

            Confirmation.body.violation.Add(new violationType
                {
                    code = code,
                    level = level,
                    name = name,
                    description = description
                });
        }

        // протокол в виде текста
        public override string ToString()
        {
            var violation = new StringBuilder();
            Confirmation.body.violation.Each(
                x =>
                    {
                        violation.AppendLine(x.name.Equals(" ") 
                                                ? string.Format("code={0}; description={1}", x.code, x.description) 
                                                : string.Format("code={0}; name={1}; description={2}", x.code, x.name, x.description));
                        violation.AppendLine();
                    });

            return violation.ToString();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData)
        {
            throw new NotImplementedException();
        }

        public void GetProtocolData(ModulesTypes mt, ref DataTable ProtocolData, string Filter, params IDbDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }

        public void GetProtocolsDate(ref DateTime MinDate, ref DateTime MaxDate)
        {
            throw new NotImplementedException();
        }
        
        public int DeleteProtocolData(ModulesTypes mt, int sourceID)
        {
            throw new NotImplementedException();
        }

        public int DeleteProtocolData(ModulesTypes mt, int sourceID, int pumpHistoryID)
        {
            throw new NotImplementedException();
        }

        public int DeleteProtocolData(ModulesTypes mt, string filterStr)
        {
            throw new NotImplementedException();
        }

        public bool DeleteProtocolArchive(string filterStr, params IDbDataParameter[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}