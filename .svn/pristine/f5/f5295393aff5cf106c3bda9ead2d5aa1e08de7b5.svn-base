using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Krista.FM.Domain;
using Krista.FM.Domain.Reporitory;

namespace Krista.FM.RIA.Extensions.DebtBook.Services.Note
{
    public class DebtBookNoteService : IDebtBookNoteService
    {
        private readonly ILinqRepository<F_S_SchBNote> noteRepository;
        private readonly ILinqRepository<D_Regions_Analysis> regionRepository;
        private readonly ILinqRepository<D_Variant_Schuldbuch> variantRepository;

        public DebtBookNoteService(
                                    ILinqRepository<F_S_SchBNote> noteRepository,
                                    ILinqRepository<D_Regions_Analysis> regionRepository,
                                    ILinqRepository<D_Variant_Schuldbuch> variantRepository)
        {
            this.noteRepository = noteRepository;
            this.regionRepository = regionRepository;
            this.variantRepository = variantRepository;
        }

        public IList GetChildRegionsNotesList(int userRegionId, int variantId, int userSourceId)
        {
            {
                // Ищем все МО, которые по справочнику территорий стоят на 2 уровня ниже субьекта(userRegionId)
                var regions1 = this.regionRepository.FindAll().Where(r1 => r1.ParentID == userRegionId);

                var regions2 = this.regionRepository.FindAll().Where(p => regions1.Any(x => x.ID == p.ParentID));

                var dataQ = this.noteRepository.FindAll()
                              .Where(t => t.RefVariant.ID == variantId
                                          && t.SourceID == userSourceId
                                          && regions2.Any(x => x.ID == t.RefRegion.ID))
                              .Select(t => new { ID = t.ID, RefTerritoryName = t.RefRegion.Name });
                var data = dataQ.ToList();
                return data;
            }
        }

        public byte[] GetFile(int id)
        {
            var file = this.noteRepository.FindOne(id);
            if (file == null)
            {
                throw new KeyNotFoundException("Файл не найден");
            }

            return file.Note;
        }

        public void UploadFile(int userRegionId, int variantId, int userSourceId, byte[] fileBody)
        {
            F_S_SchBNote note;
            var noteRows = this.noteRepository.FindAll()
                              .Where(n => n.RefVariant.ID == variantId && n.RefRegion.ID == userRegionId && n.SourceID == userSourceId)
                              .ToList();
            if (noteRows.Count() == 1)
            {
                note = noteRows.First();
                note.Note = fileBody;
                noteRepository.Save(note);
            }
            else
            {
                var region = this.regionRepository.FindOne(userRegionId);
                var variant = this.variantRepository.FindOne(variantId);
                note = new F_S_SchBNote
                               {
                                   Note = fileBody,
                                   RefRegion = region,
                                   RefVariant = variant,
                                   SourceID = userSourceId
                               };
                noteRepository.Save(note);
            }

            noteRepository.DbContext.CommitChanges();

            // Проверка на дубликаты
            // Если обнаруживаем - удаляем только что созданную запись.
            noteRows = this.noteRepository.FindAll()
                              .Where(n => n.RefVariant.ID == variantId && n.RefRegion.ID == userRegionId && n.SourceID == userSourceId)
                              .ToList();
            if (noteRows.Count > 1)
            {
                foreach (var row in noteRows)
                {
                    if (row.ID == note.ID)
                    {
                        noteRepository.Delete(row);
                        break;
                    }
                }

                noteRepository.DbContext.CommitChanges();
                throw new DuplicateWaitObjectException("Обнаружены дубликаты. Попробуйте ещё раз.");
            }
        }

        public int? GetNoteId(int userRegionId, int variantId, int userSourceId)
        {
            var entityList = noteRepository.FindAll()
                                       .Where(n => n.RefVariant.ID == variantId && n.RefRegion.ID == userRegionId && n.SourceID == userSourceId)
                                       .ToList();
            if (entityList.Count >= 1)
            {
                return entityList.First().ID;
            }
            else
            {
                return null;
            }
        }

        public void DeleteNote(int noteId)
        {
            var entity = noteRepository.FindOne(noteId);
            if (entity != null)
            {
                noteRepository.Delete(entity);
                noteRepository.DbContext.CommitChanges();
            }
        }
    }
}
