using System.Web.Mvc;
using Krista.FM.Domain.Reporitory.NHibernate;
using NHibernate;

namespace Krista.FM.RIA.Core.NHibernate
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        public bool RollbackOnModelStateError { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            NHibernateSession.Current.BeginTransaction();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ITransaction currentTransaction =
                NHibernateSession.Current.Transaction;

            if (currentTransaction.IsActive)
            {
                if ((filterContext.Exception != null && filterContext.ExceptionHandled) || ShouldRollback(filterContext))
                {
                    currentTransaction.Rollback();
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            ITransaction currentTransaction = NHibernateSession.Current.Transaction;

            base.OnResultExecuted(filterContext);
            try
            {
                if (currentTransaction.IsActive)
                {
                    if (((filterContext.Exception != null) && (!filterContext.ExceptionHandled)) || ShouldRollback(filterContext))
                    {
                        currentTransaction.Rollback();
                    }
                    else
                    {
                        currentTransaction.Commit();
                    }
                }
            }
            finally
            {
                currentTransaction.Dispose();
            }
        }

        private bool ShouldRollback(ControllerContext filterContext)
        {
            return RollbackOnModelStateError && !filterContext.Controller.ViewData.ModelState.IsValid;
        }
    }
}
