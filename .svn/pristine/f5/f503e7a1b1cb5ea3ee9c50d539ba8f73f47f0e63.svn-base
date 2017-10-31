using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Services
{
    public interface IAuthService
    {
        /// <summary>
        ///   Текущий пользователь.
        /// </summary>
        Users User { get; }

        /// <summary>
        ///   Профиль пользователя
        /// </summary>
        D_Org_UserProfile Profile { get; }

        /// <summary>
        /// Профиль огранизации
        /// </summary>
        D_Org_Structure ProfileOrg { get; }

        /// <summary>
        /// Профиль грбс (не реализанно)
        /// </summary>
        D_Org_GRBS ProfileOrgGrbs { get; }

        /// <summary>
        /// Профиль ппо (не реализанно)
        /// </summary>
        D_Org_PPO ProfileOrgPpo { get; }

        /// <summary>
        ///   Пользователь с правами администратора
        /// </summary>
        bool IsAdmin();

        /// <summary>
        ///   Состоит в группе ГРБС
        /// </summary>
        bool IsGrbsUser();

        /// <summary>
        ///   Состоит в группе ФО
        /// </summary>
        bool IsPpoUser();

        /// <summary>
        ///   Состоит в группе E86N_Consumer(учреждление)
        /// </summary>
        bool IsInstitution();

        /// <summary>
        ///   Зарегестрированный Пользователь
        /// </summary>
        bool IsRegisteredUser();

        /// <summary>
        ///   Имеет право подписи
        /// </summary>
        bool IsAuthority2Sign();

        /// <summary>
        ///   Состоит в группе krista.ru
        /// </summary>
        bool IsKristaRu();

        /// <summary>
        ///   Состоит в группе E86N_Spectator
        /// </summary>
        bool IsSpectator();
    }
}
