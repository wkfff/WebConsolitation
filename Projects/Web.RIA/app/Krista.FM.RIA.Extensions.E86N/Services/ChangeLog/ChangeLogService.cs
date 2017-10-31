using System;
using Krista.FM.Domain;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Extensions.E86N.Auth.Services;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services.ChangeLog
{
    public class ChangeLogService : IChangeLogService
    {
        private readonly INewRestService service;
        private readonly IAuthService auth;

        public ChangeLogService()
        {
            service = Resolver.Get<INewRestService>();
            auth = Resolver.Get<IAuthService>();
        }

        public void WriteChange(F_F_ParameterDoc doc, int logActionType, string note = null)
        {
            var haveTransaction = service.HaveTransaction;
            if (!haveTransaction)
            {
                service.BeginTransaction();
            }
            
            var type = service.GetItem<FX_FX_ChangeLogActionType>(logActionType);
            service.Save(
                new F_F_ChangeLog
                {
                    ID = 0,
                    Data = DateTime.Now,
                    DocId = doc.ID,
                    OrgINN = doc.RefUchr.INN,
                    RefChangeType = type,
                    RefType = doc.RefPartDoc,
                    Year = doc.RefYearForm.ID,
                    Login = auth.User.Name,
                    Note = note
                });

            if (!haveTransaction)
            {
                service.CommitTransaction();
            }
        }
        
        public void WriteChangeDocDetail(F_F_ParameterDoc doc)
        {
            WriteChange(doc, FX_FX_ChangeLogActionType.DocumentBodyChange);
        }

        public void WriteDeleteDocDetail(F_F_ParameterDoc doc)
        {
            WriteChange(doc, FX_FX_ChangeLogActionType.DocumentBodyDelete);
        }

        public void WriteDeleteDocDetailAbort(F_F_ParameterDoc doc)
        {
            WriteChange(doc, FX_FX_ChangeLogActionType.DocumentBodyDeleteAbort);
        }

        public void WriteCreateFile(F_Doc_Docum doc)
        {
            WriteChange(doc.RefParametr, FX_FX_ChangeLogActionType.FileCreate, string.Concat("Тип файла:", doc.RefTypeDoc.Name));
        }

        public void WriteDeleteFile(F_Doc_Docum doc)
        {
            WriteChange(doc.RefParametr, FX_FX_ChangeLogActionType.FileDelete, string.Concat("Тип файла:", doc.RefTypeDoc.Name));
        }
    }
}