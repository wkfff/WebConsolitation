using bus.gov.ru.external.Item1;

namespace bus.gov.ru
{
    public interface IDataPumpProtocolProvider
    {
        /// <summary>
        /// ѕротокол операции
        /// </summary>
        confirmation Confirmation { get; set; }

        /// <summary>
        /// «апись событи€ в протокол закачки
        /// </summary>
        /// <param name="eventType"> тип событи€ </param>
        /// <param name="name"> наименование событи€ </param>
        /// <param name="description"> описание событи€ </param>
        void WriteEventIntoDataPumpProtocol(DataPumpEventType eventType, string name, string description = "");
    }
}