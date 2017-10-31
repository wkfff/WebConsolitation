using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.EO10MissivePRF
{
    public interface IEO10Extension
    {
        Users User { get; }

        /// <summary>
        /// Группа, к которой принадлежит пользователь - User (для исполнителей мероприятий) или пусто (для остальных пользователей)
        /// </summary>
        int UserGroup { get; }

        /// <summary>
        /// Исполнитель мероприятия
        /// </summary>
        D_MissivePRF_Execut Executer { get; }
    }
}
