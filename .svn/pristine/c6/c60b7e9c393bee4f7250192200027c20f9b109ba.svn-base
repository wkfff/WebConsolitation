using System;
using System.Diagnostics;
using System.Threading;
using Krista.FM.RIA.Core.Extensions;
using Krista.FM.RIA.Core.Principal;
using Krista.FM.RIA.Extensions.Informator;

namespace Krista.FM.RIA.Extensions.Informator
{
    public class InformatorExtension : IInformatorExtension
    {
        private readonly INewsService newsManager;

        public InformatorExtension(INewsService newsManager)
        {
            this.newsManager = newsManager;
        }

        #region IInformatorExtension Members

        public int NewMessagesCount { get; set; }

        #endregion

        public bool Initialize()
        {
            try
            {
                NewMessagesCount = newsManager.GetNewMessagesCount(((BasePrincipal)System.Web.HttpContext.Current.User).DbUser.ID);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}