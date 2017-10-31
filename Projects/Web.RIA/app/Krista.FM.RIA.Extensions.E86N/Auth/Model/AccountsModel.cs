using System;

namespace Krista.FM.RIA.Extensions.E86N.Auth.Model
{
    public sealed class AccountsViewModel
    {
        #region Характеристики учетной записи

        /// <summary>
        /// идентификатор пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// идентификатор Организации
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// имя огранизации
        /// </summary>
        public string OrgName { get; set; }

        public DateTime? LastLogin { get; set; }

        /// <summary>
        ///   учетная запись помечена как привелигированная в рамках задачи
        ///   сейчас используем для управления учетными записями в рамках организации и подведомственных
        ///   потом решим где этому место
        /// </summary>
        public bool IsAdmin { get; set; }

        public bool IsGrbs { get; set; }

        public bool IsPpo { get; set; }

        #endregion

        #region Характеристики профиля пользователя

        /// <summary>
        /// адрес электронной почты
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// идентификатор профиля пользователя
        /// </summary>
        public int ID { get; set; }

        #endregion
    }
}
