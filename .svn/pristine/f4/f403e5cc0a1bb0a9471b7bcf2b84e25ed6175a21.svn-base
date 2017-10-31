using System.Collections.Generic;
using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public interface IUserSessionState
    {
        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        Users User { get; }

        /// <summary>
        /// Субъекты к которым привязан пользователь.
        /// </summary>
        IList<D_CD_Subjects> Subjects { get; }

        /// <summary>
        /// Район к которому относится текущий пользователь.
        /// </summary>
        D_Regions_Analysis UserRegion { get; }
    }
}