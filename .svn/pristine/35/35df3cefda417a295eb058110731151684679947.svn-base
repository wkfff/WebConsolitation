using System;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Extensions;
using Krista.FM.RIA.Extensions.E86N.Services.RestfulService;

namespace Krista.FM.RIA.Extensions.E86N.Services
{
    public class VersioningService : NewRestService, IVersioningService
    {
        /// <summary>
        /// Проверяет закрыт ли документ.
        /// </summary>
        /// <param name="docId"> идентификатор документа</param>
        /// <returns> true если документ закрыт </returns>
        public bool GetCloseState(int docId)
        {
            try
            {
                return GetItem<F_F_ParameterDoc>(docId).CloseDate.HasValue;
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось посмотреть дату закрытия.  " + e.Message);
            }
        }

        public DateTime GetOpenDate(int docId)
        {
            try
            {
                var date = GetItem<F_F_ParameterDoc>(docId).OpeningDate;
                return date.HasValue ? (DateTime)date : new DateTime(GetItem<F_F_ParameterDoc>(docId).RefYearForm.ID, 01, 01);
            }
            catch (Exception e)
            {
                throw new Exception("Не удалось посмотреть дату закрытия.  " + e.Message);
            }
        }

        public F_F_ParameterDoc GetDocumentForCopy(int docId) 
        {
            var formData = GetItem<F_F_ParameterDoc>(docId);
            var docs = GetItems<F_F_ParameterDoc>().Where(
                x => x.RefUchr.ID == formData.RefUchr.ID
                     && x.RefPartDoc.ID == formData.RefPartDoc.ID
                     && x.ID != docId);
            if (docs.Any(p => p.RefYearForm.ID == formData.RefYearForm.ID))
            {
                return docs.OrderByDescending(doc => doc.ID).First(p => p.RefYearForm.ID == formData.RefYearForm.ID);
            }

            var maxYer = docs.Max(doc => doc.RefYearForm.ID);
            return docs.OrderByDescending(doc => doc.ID).First(p => p.RefYearForm.ID == maxYer);
        }

        public int GetDocumentIdForOGS(int recId)
        {
            var docs = GetItems<F_F_ParameterDoc>().Where(x => x.RefUchr.ID == recId && x.RefPartDoc.ID == 1);
            return docs.Count() != 0 ? docs.OrderByDescending(x => x.ID).First().ID : 0;
        }

        public bool CheckCloseDocs(int docId)
        {
            var formData = GetItem<F_F_ParameterDoc>(docId);
            var closeDocs = GetItems<F_F_ParameterDoc>().Where(
                x => x.RefUchr.ID == formData.RefUchr.ID
                     && x.RefPartDoc.ID == formData.RefPartDoc.ID
                     && x.CloseDate.HasValue
                     && x.ID != docId);
            return closeDocs.Count() != 0;
        }

        public bool CheckDocs(int docId)
        {
            var formData = GetItem<F_F_ParameterDoc>(docId);
            var closeDocs = GetItems<F_F_ParameterDoc>().Where(
                x => x.RefUchr.ID == formData.RefUchr.ID
                     && x.RefPartDoc.ID == formData.RefPartDoc.ID
                     && x.ID != docId);
            return closeDocs.Count() != 0;
        }

        public void CloseDocument(int docId)
        {
            var doc = GetItem<F_F_ParameterDoc>(docId);
            doc.CloseDate = DateTime.Now;

            Save(doc);
            CommitChanges();
        }

        public void OpenDocument(int docId)
        {
            var doc = GetItem<F_F_ParameterDoc>(docId);
            doc.CloseDate = null;

            Save(doc);
            CommitChanges();
        }

        public void CloseOgs(int recId, DateTime closeDate, string note)
        {
            var rec = GetItem<D_Org_Structure>(recId);
            rec.CloseDate = closeDate;
            rec.Status = "866";
            var docs = GetItems<F_F_ParameterDoc>().Where(x => x.RefUchr.ID == recId && x.CloseDate == null);
            docs.ToList().ForEach(doc =>
                {
                    doc.CloseDate = closeDate;
                    doc.Note = "Учреждение закрыто {0}. ".FormatWith(string.Format("{0:MM/dd/yyyy}", closeDate)) +
                             (note.IsNotNullOrEmpty()
                                  ? "{0}".FormatWith(note)
                                  : string.Empty);
                    Save(doc);
                });
            CommitChanges();
            Save(rec);
            CommitChanges();
        }

        public void OpenOgs(int recId)
        {
            var rec = GetItem<D_Org_Structure>(recId);
            rec.CloseDate = null;
            rec.Status = "801";

            Save(rec);
            CommitChanges();
        }
    }
}