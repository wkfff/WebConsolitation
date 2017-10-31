using System.Linq;

using Ext.Net;
using Ext.Net.MVC;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Controllers.Binders;
using Krista.FM.RIA.Extensions.E86N.Models.ChangeLog;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

using NHibernate.Linq;

namespace Krista.FM.RIA.Extensions.E86N.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для представления "Действия пользователей"
    /// </summary>
    public class ChangeLogController : SchemeBoundController
    {
        private readonly INewRestService newRestService;
        private readonly ChangeLogModel model = new ChangeLogModel();

        public ChangeLogController(INewRestService newRestService)
        {
            this.newRestService = newRestService;
        }
        
        public AjaxStoreResult Read([FiltersBinder] FilterConditions filters, int limit, int start)
        {
            var data = newRestService.GetItems<F_F_ChangeLog>().Select(
                                                x => new ChangeLogModel
                                                {
                                                    ID = x.ID,
                                                    Date = x.Data.Date,
                                                    DocId = x.DocId,
                                                    Login = x.Login,
                                                    OrgINN = x.OrgINN,
                                                    Time = x.Data.ToString("HH:mm"),
                                                    Year = x.Year,
                                                    RefChangeType = x.RefChangeType.ID,
                                                    RefChangeTypeName = x.RefChangeType.Name,
                                                    RefType = x.RefType.ID,
                                                    RefTypeName = x.RefType.Name,
                                                    Note = x.Note
                                                });

            filters.Conditions
                .ForEach(
                    filter =>
                    {
                        if (filter.Name == model.NameOf(() => model.Date))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.Date.Date == filter.ValueAsDate);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.Date.Date < filter.ValueAsDate);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.Date.Date > filter.ValueAsDate);
                                    break;
                            }
                        }
                        
                        if (filter.Name == model.NameOf(() => model.Login))
                        {
                            data = data.Where(x => x.Login.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.OrgINN))
                        {
                            data = data.Where(x => x.OrgINN.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.Note))
                        {
                            data = data.Where(x => x.Note != null && x.Note.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.RefChangeTypeName))
                        {
                            data = data.Where(x => x.RefChangeTypeName.Contains(filter.Value));
                        }

                        if (filter.Name == model.NameOf(() => model.RefTypeName))
                        {
                            data = data.Where(x => x.RefTypeName.Contains(filter.Value));
                        }
                        
                        if (filter.Name == model.NameOf(() => model.Year))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.Year == filter.ValueAsInt);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.Year < filter.ValueAsInt);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.Year > filter.ValueAsInt);
                                    break;
                            }
                        }
                        
                        if (filter.Name == model.NameOf(() => model.DocId))
                        {
                            switch (filter.Comparison)
                            {
                                case Comparison.Eq:
                                    data = data.Where(x => x.DocId == filter.ValueAsInt);
                                    break;
                                case Comparison.Lt:
                                    data = data.Where(x => x.DocId < filter.ValueAsInt);
                                    break;
                                case Comparison.Gt:
                                    data = data.Where(x => x.DocId > filter.ValueAsInt);
                                    break;
                            }
                        }
                    });

            return new AjaxStoreResult(data.Skip(start).Take(limit), data.Count());
        }
    }
}
