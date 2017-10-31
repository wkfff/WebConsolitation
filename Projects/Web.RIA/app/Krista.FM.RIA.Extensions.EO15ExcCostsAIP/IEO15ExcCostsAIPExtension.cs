using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP
{
    public interface IEO15ExcCostsAIPExtension
    {
        /// <summary>
        /// Заказчик объектов строительства
        /// </summary>
        D_ExcCosts_Clients Client { get; }

        /// <summary>
        /// Источник данных
        /// </summary>
        DataSources DataSource { get; }

        string OKTMO { get; }

        /// <summary>
        /// Группа пользователя
        /// </summary>
        string UserGroup { get; }
    }
}
