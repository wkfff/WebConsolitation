
using System.ComponentModel;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas.Models
{
    /// <summary>
    /// Типы проектов
    /// </summary>
    public enum InvAreaStatus : int
    {
        /// <summary>
        /// Значение не указано
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// На редактировании
        /// </summary>
        Edit = 1,

        /// <summary>
        /// На рассмотрении у координатора
        /// </summary>
        Review = 2,

        /// <summary>
        /// Принят на данный момент
        /// </summary>
        Accepted = 3
    }

    internal static class InvAreaRoles
    {
        public const string Creator = "EO14_Creator";
        public const string Coordinator = "EO14_Coordinator";
    }
}
