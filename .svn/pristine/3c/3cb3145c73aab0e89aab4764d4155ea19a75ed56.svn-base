using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Common.Handling
{
    /// <summary>
    /// Класс для перенаправления собылий сервера клиенту.
    /// </summary>
    /// <remarks>
    /// Клиент должен создать екземрляр класса.
    /// Для событий сервера зарегистрировать обработчик OnServerModificationMessage.
    /// Для обработки события на клиенте нужно подписаться на OnClientModificationMessage.
    /// </remarks>
    public class ModificationMessageHandling : DisposableObject
    {
        /// <summary>
        /// Обработчик для события сервера.
        /// </summary>
        /// <param name="sender">.</param>
        /// <param name="e"></param>
        public void OnServerModificationMessage(object sender, ModificationMessageEventArgs args)
        {
            if (OnClientModificationMessage != null)
            {
                OnClientModificationMessage(sender, args);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        /// <summary>
        /// Событие к которому подписывается клиент.
        /// </summary>
        public event ModificationMessageEventHandler OnClientModificationMessage;
    }
}
