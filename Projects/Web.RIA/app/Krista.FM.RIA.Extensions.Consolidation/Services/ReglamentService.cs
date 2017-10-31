using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory.NHibernate;

namespace Krista.FM.RIA.Extensions.Consolidation
{
    public class ReglamentService : NHibernateLinqRepository<D_CD_Reglaments>, IReglamentService
    {
        public IQueryable GetQueryAll()
        {
            return from r in FindAll()
                   select new
                   {
                       r.ID,
                       Role = r.RefRole.Name,
                       Level = r.RefLevel.Name,
                       RepKind = r.RefRepKind.Name,
                       Template = r.RefTemplate.Name,
                       TemplateGroup = r.RefTemplate.NameCD,
                       r.Laboriousness,
                       r.Workdays,
                       r.Note,
                       RefRole = r.RefRole.ID,
                       RefLevel = r.RefLevel.ID,
                       RefRepKind = r.RefRepKind.ID,
                       RefTemplate = r.RefTemplate.ID
                   };
        }

        public object GetQueryOne(int id)
        {
            var r = FindOne(id);
            return new
            {
                r.ID,
                Role = r.RefRole.Name,
                Level = r.RefLevel.Name,
                RepKind = r.RefRepKind.Name,
                Template = r.RefTemplate.Name,
                TemplateGroup = r.RefTemplate.NameCD,
                r.Laboriousness,
                r.Workdays,
                r.Note,
                RefRole = r.RefRole.ID,
                RefLevel = r.RefLevel.ID,
                RefRepKind = r.RefRepKind.ID,
                RefTemplate = r.RefTemplate.ID
            };
        }
    }
}
