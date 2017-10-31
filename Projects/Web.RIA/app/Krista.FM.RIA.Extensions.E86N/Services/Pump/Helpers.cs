using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using bus.gov.ru.types.Item1;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.E86N.Services.Pump
{
    public static class Helpers
    {
        public static D_Org_Structure GetOrgStructureByRegnum(string targetOrgRenum)
        {
            D_Org_NsiOGS nsiOgs = Resolver.Get<ILinqRepository<D_Org_NsiOGS>>().FindAll()
                .Single(ogs => ogs.regNum.Equals(targetOrgRenum));
            return Resolver.Get<ILinqRepository<D_Org_Structure>>().FindAll()
                .Single(structure => structure.INN.Equals(nsiOgs.inn) && structure.KPP.Equals(nsiOgs.kpp));
        }

        public static void ProcessDocumentsHeader(
            F_F_ParameterDoc header,
            headerType dataDocument,
            IEnumerable<documentType> pumpDataDocument,
            Func<documentType, D_Doc_TypeDoc> getTypeDocFunc)
        {
            const int LengthFDocDocumFieldName = 128;
            foreach (documentType documentType in pumpDataDocument)
            {
                string fileName = String.Format("{0}.{1}", dataDocument.id, documentType.name);
                var document =
                    new F_Doc_Docum
                        {
                            Name =
                                documentType.name.Substring(
                                    0, Math.Min(documentType.name.Length, LengthFDocDocumFieldName)),
                            DocDate = new DateTime(Math.Max(documentType.date.Ticks, SqlDateTime.MinValue.Value.Ticks)),
                            RefParametr = header,
                            RefTypeDoc = getTypeDocFunc(documentType),
                            UrlExternal = documentType.url,
                            Url = documentType.name,
                        };
                if (documentType.content != null)
                {
                    using (var writer = new BinaryWriter(
                        new FileStream(
                            Path.Combine(
                                ConfigurationManager.AppSettings["DocFilesSavePath"],
                                fileName),
                            FileMode.Create)))
                    {
                        writer.Write(documentType.content);
                    }
                }

                header.Documents.Add(document);
            }
        }

        public static F_F_ParameterDoc Header(refNsiConsRegExtendedStrongType targetOrg, int docTypeID, DateTime dateTime)
        {
            return new F_F_ParameterDoc
                       {
                           PlanThreeYear = false,
                           RefPartDoc = Resolver.Get<ILinqRepository<FX_FX_PartDoc>>().Load(docTypeID),
                           RefSost =
                               Resolver.Get<ILinqRepository<FX_Org_SostD>>().Load(FX_Org_SostD.ExportedStateID),
                           RefUchr = GetOrgStructureByRegnum(targetOrg.regNum),
                           RefYearForm = Resolver.Get<ILinqRepository<FX_Fin_YearForm>>().Load(dateTime.Year),
                           CloseDate = DateTime.Now,
                       };
        }
    }
}
