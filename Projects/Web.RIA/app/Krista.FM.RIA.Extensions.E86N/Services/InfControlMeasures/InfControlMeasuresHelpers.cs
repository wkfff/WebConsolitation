using Ext.Net;

namespace Krista.FM.RIA.Extensions.E86N.Services.InfControlMeasures
{
    /// <summary>
    /// Поля для "Сведения о проведенных контрольных мероприятиях и их результатах"
    /// </summary>
    public enum InfControlMeasures
    {
        /// <summary>
        /// Без ID данные не грузятся в стор
        /// </summary>
        ID,

        /// <summary>
        /// Тема контрольного мероприятия
        /// </summary>
        Topic,

        /// <summary>
        /// Дата начала проведения контрольного мероприятия
        /// </summary>
        EventBegin,

        /// <summary>
        /// Дата окончания проведения контрольного мероприятия
        /// </summary>
        EventEnd,

        /// <summary>
        /// Выявленные нарушения
        /// </summary>
        Violation,

        /// <summary>
        /// Мероприятия проведенные по результатам контрольного мероприятия
        /// </summary>
        ResultActivity,

        /// <summary>
        /// Параметры документа
        /// </summary>
        RefParametr,

        /// <summary>
        /// Орган государственной власти_ОМС_осуществляющий проведение контрольного мероприятия
        /// </summary>
        Supervisor
    }

    static class InfControlMeasuresHelpers
    {

        #region InfControlMeasures

        static public void InfControlMeasuresExportMetadataTo(JsonReader jsonReader)
        {
            jsonReader.Fields.Clear();
            jsonReader.Fields.Add(InfControlMeasures.ID.ToString());
            jsonReader.Fields.Add(InfControlMeasures.RefParametr.ToString());
            jsonReader.Fields.Add(InfControlMeasures.Supervisor.ToString());
            jsonReader.Fields.Add(InfControlMeasures.Topic.ToString());
            jsonReader.Fields.Add(InfControlMeasures.EventBegin.ToString());
            jsonReader.Fields.Add(InfControlMeasures.EventEnd.ToString());
            jsonReader.Fields.Add(InfControlMeasures.Violation.ToString());
            jsonReader.Fields.Add(InfControlMeasures.ResultActivity.ToString());
        }

        static public string InfControlMeasuresNameMapping(InfControlMeasures field)
        {
            switch (field)
            {
                case InfControlMeasures.ID:
                    {
                        return "ID";
                    }

                case InfControlMeasures.EventBegin:
                    {
                        return "Дата начала";
                    }

                case InfControlMeasures.EventEnd:
                    {
                        return "Дата окончания";
                    }

                case InfControlMeasures.ResultActivity:
                    {
                        return "Мероприятия проведенные по результатам контрольного мероприятия";
                    }

                case InfControlMeasures.Supervisor:
                    {
                        return "Орган осуществляющий контроль";
                    }

                case InfControlMeasures.Topic:
                    {
                        return "План (тема) мероприятия";
                    }

                case InfControlMeasures.Violation:
                    {
                        return "Выявленные нарушения";
                    }
            }

            return string.Empty;
        }

#endregion

    }
}
