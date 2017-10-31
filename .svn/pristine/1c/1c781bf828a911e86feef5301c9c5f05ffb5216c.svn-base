using Krista.FM.Common;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.TemplatesService
{
    public class TemplatesService : DisposableObject, ITemplatesService
    {
        private readonly ITemplatesRepository repository;

		/// <summary>
		/// Конструктор для создания единственного экземпляра класса.
		/// </summary>
        public TemplatesService(ITemplatesRepository repository)
		{
		    this.repository = repository;
		}

        public virtual ITemplatesRepository Repository
        {
            get { return repository; }
        }

        protected override void Dispose(bool disposing)
        {
#if DEBUG
            Trace.TraceVerbose("~" + this);
#endif

            if (disposing)
            {
                repository.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
