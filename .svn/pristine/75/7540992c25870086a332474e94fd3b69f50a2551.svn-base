using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Documents
{
    public interface IDocuments : INewRestService
    {
        /// <summary>
        /// ¬озвращает массив состо€ний
        /// </summary>
        string[] GetStates();
        
        /// <summary>
        /// ¬озвращает массив типов документов
        /// </summary>
        string[] GetDocTypes();

        /// <summary>
        /// ¬озвращает список типов документов
        /// </summary>
        IQueryable<FX_FX_PartDoc> GetDocTypesList();

        /// <summary>
        /// ¬озвращает массив годов формировани€ документов
        /// </summary>
        string[] GetYears();
    }
}