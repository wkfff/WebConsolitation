using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Krista.FM.RIA.Extensions.DebtBook.Services.Note
{
    public interface IDebtBookNoteService
    {
        /// <summary>
        /// Поиск МО, подчиненных данному субьекту, с введенным документом - пояснительной_запиской
        /// </summary>
        /// <param name="userRegionId">Код территории субьекта</param>
        /// <param name="variantId">Код варианта</param>
        /// <param name="userSourceId">Код источника</param>
        IList GetChildRegionsNotesList(int userRegionId, int variantId, int userSourceId);

        /// <summary>
        /// Получает файл пояснительной запиской с сервера
        /// </summary>
        /// <param name="id">код записи в таблице F_S_SchBNote</param>
        byte[] GetFile(int id);

        /// <summary>
        /// Загружает файл пояснительной записки на сервер
        /// </summary>
        /// <param name="userRegionId">Код территории субьекта</param>
        /// <param name="variantId">Код варианта</param>
        /// <param name="userSourceId">Код источника</param>
        /// <param name="fileBody">Содержимое файла</param>
        void UploadFile(int userRegionId, int variantId, int userSourceId, byte[] fileBody);

        /// <summary>
        /// Находит запись с пояснительной запиской для указанного варианта/источника/субьекта
        /// </summary>
        /// <param name="userRegionId">Код территории субьекта</param>
        /// <param name="variantId">Код варианта</param>
        /// <param name="userSourceId">Код источника</param>
        int? GetNoteId(int userRegionId, int variantId, int userSourceId);

        /// <summary>
        /// Удаляет запись с пояснительной запиской
        /// </summary>
        /// <param name="noteId">код записи в таблице F_S_SchBNote</param>
        void DeleteNote(int noteId);
    }
}
