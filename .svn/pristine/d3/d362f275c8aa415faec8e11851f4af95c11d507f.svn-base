using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.NHibernate;
using Krista.FM.RIA.Extensions.E86N.Models.Abstract;
using Krista.FM.RIA.Extensions.E86N.Services.DocService;
using Krista.FM.RIA.Extensions.E86N.Utils;

namespace Krista.FM.RIA.Extensions.E86N.Models.DocsDetail
{
    public class DocsDetailViewModel : AbstractModelBase<DocsDetailViewModel, F_Doc_Docum>
    {
        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public string Url { get; set; }

        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public string Name { get; set; }

        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public string DocDate { get; set; }
        
        public int RefTypeDoc { get; set; }

        [DataBaseBindingField(typeof(D_Doc_TypeDoc), "Name")]
        public string RefTypeDocName { get; set; }

        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public string UrlExternal { get; set; }

        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public int? FileSize { get; set; }

        public int TotalDocsSize { get; set; }

        [DataBaseBindingTable(typeof(F_Doc_Docum))]
        public string NumberNPA { get; set; }

        public override string ValidateData()
        {
            const string Msg = "Длинна имени файла не должна превышать 128 символов<br>";
            const string Msg1 = "Некорректная дата утверждения <br/>";

            var message = new StringBuilder(string.Empty);

            if (Name.Length > 128)
            {
                message.Append(Msg);
            }

            if (DocDate.IsNotNullOrEmpty() && Convert.ToDateTime(DocDate).Date > DateTime.Today)
            {
                message.Append(Msg1);
            }

            return message.ToString();
        }
        
        public override DocsDetailViewModel GetModelByDomain(F_Doc_Docum domain)
        {
            var totalSize = GetNewRestService().GetItems<F_Doc_Docum>().Where(x => x.RefParametr.ID.Equals(domain.RefParametr.ID)).SumWithNull(x => x.FileSize);
            
            return new DocsDetailViewModel
                        {
                            ID = domain.ID,
                            RefParent = domain.RefParametr.ID,
                            Name = domain.Name,
                            DocDate = domain.DocDate.HasValue ? domain.DocDate.GetValueOrDefault().ToString("dd.MM.yyyy") : string.Empty,
                            Url = domain.Url,
                            UrlExternal = domain.UrlExternal,
                            RefTypeDoc = domain.RefTypeDoc.ID,
                            RefTypeDocName = domain.RefTypeDoc.Name,
                            FileSize = domain.FileSize,
                            TotalDocsSize = totalSize.GetValueOrDefault(),
                            NumberNPA = domain.NumberNPA
            };
        }
        
        public override F_Doc_Docum GetDomainByModel()
        {
            return new F_Doc_Docum
            {
                ID = ID,
                RefParametr = GetNewRestService().GetItem<F_F_ParameterDoc>(RefParent),
                Name = Name,
                Url = Url,
                DocDate = DocDate.IsNotNullOrEmpty() ? Convert.ToDateTime(DocDate).Date : (DateTime?)null,
                FileSize = FileSize,
                RefTypeDoc = GetNewRestService().GetItem<D_Doc_TypeDoc>(RefTypeDoc),
                UrlExternal = UrlExternal,
                NumberNPA = NumberNPA
            };
        }

        [Transaction]
        public override IQueryable<DocsDetailViewModel> GetModelData(NameValueCollection paramsList)
        {
            int docId = Convert.ToInt32(paramsList["docId"]);
            var docService = Resolver.Get<IDocService>();

            return docService.GetItems(docId).Select(GetModelByDomain).AsQueryable();
        }
    }
}
