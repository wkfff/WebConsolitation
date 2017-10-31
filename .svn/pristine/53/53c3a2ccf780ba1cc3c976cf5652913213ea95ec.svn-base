using System.Globalization;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.Documents
{
    public sealed class DocumentsService : NewRestService, IDocuments
    {
        #region IDocuments Members

        /// <summary>
        ///   Возвращает массив состояний
        /// </summary>
        public string[] GetStates()
        {
            return GetItems<FX_Org_SostD>().Where(x => (x.ID != 0) && (x.ID != 1) && (x.ID != 6))
                .Select(x => x.Name).ToArray();
        }

        /// <summary>
        ///   Возвращает массив типов документов
        /// </summary>
        public string[] GetDocTypes()
        {
            return GetDocTypesList().Select(x => x.Name).ToArray();
        }

        /// <summary>
        ///   Возвращает типы документов
        /// </summary>
        public IQueryable<FX_FX_PartDoc> GetDocTypesList()
        {
            return GetItems<FX_FX_PartDoc>().Where(x => (x.ID != FX_FX_PartDoc.NoValueDocTypeID));
        }

        /// <summary>
        ///   Возвращает массив годов формирования документов
        /// </summary>
        public string[] GetYears()
        {
            var list = GetItems<F_F_ParameterDoc>().Select(x => x.RefYearForm.ID).Distinct().ToList();
            list.Sort();

            return list.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        #endregion
    }
}
