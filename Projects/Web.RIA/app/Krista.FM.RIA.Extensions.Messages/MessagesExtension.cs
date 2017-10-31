using System;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.Messages.Services;

namespace Krista.FM.RIA.Extensions.Messages
{
    public class MessagesExtension : IMessageExtension
    {
        private readonly IMessageService messageService;

        public MessagesExtension(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public bool Initialize()
        {
            try
            {
                GetNewMessagesCount();
            }
            catch (Exception)
            {
                // TODO обработчик ошибок
                return false;
            }

            return true;
        }

        public int GetNewMessagesCount()
        {
            if (System.Web.HttpContext.Current == null)
            {
                return -1;
            }

            return messageService.GetNewMessagesCount(((BasePrincipal)System.Web.HttpContext.Current.User).DbUser.ID);
        }
    }
}