using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO14InvestAreas
{
    public interface IUserCredentials
    {
        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        Users User { get; }

        /// <summary>
        /// Группы, в которые входит пользователь
        /// </summary>
        Groups[] Roles { get; }

        /// <summary>
        /// Проверяет входит ли пользователь в данную группу
        /// </summary>
        bool IsInRole(string role);

        /// <summary>
        /// Проверяет входит ли пользователь хотябы в одну группу
        /// </summary>
        bool IsInAnyRole(params string[] roles);
    }
}
