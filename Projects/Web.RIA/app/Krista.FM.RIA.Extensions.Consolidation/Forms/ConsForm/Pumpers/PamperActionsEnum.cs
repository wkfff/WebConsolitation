using System;

namespace Krista.FM.RIA.Extensions.Consolidation.Forms.ConsForm.Pumpers
{
    [Flags]
    public enum PamperActionsEnum
    {
        /// <summary>
        /// Операция не задана.
        /// </summary>
        None = 0,

        /// <summary>
        /// Загрузить данные в таблицы фактов в режиме обновления.
        /// </summary>
        Pump = 1,

        /// <summary>
        /// Удалить загруженные данные из таблицы фактов.
        /// </summary>
        Clear = 2,

        /// <summary>
        /// Удалить загруженные данные и загрузить повторно.
        /// </summary>
        ClearPamp = Clear | Pump
    }
}
