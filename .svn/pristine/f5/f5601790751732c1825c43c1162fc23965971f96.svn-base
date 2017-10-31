using Krista.FM.Domain;

namespace Krista.FM.RIA.Extensions.E86N.Services.ChangeLog
{
    public interface IChangeLogService
    {
        /// <summary>
        /// Записывает в лог указанное действие
        /// </summary>
        /// <param name="doc">Документ, над которым производится действие</param>
        /// <param name="logActionType">Id действие. см FX_FX_ChangeLogActionType</param>
        /// <param name="note">Дополнительнвая информация</param>
        void WriteChange(F_F_ParameterDoc doc, int logActionType, string note = null);
        
        // todo зачем пробрасывать объект-документ, переделать на идентификатор

        /// <summary>
        /// Записывает в лог действие "Изменение сдержимого"
        /// </summary>
        /// <param name="doc">Документ, над которым производится действие</param>
        void WriteChangeDocDetail(F_F_ParameterDoc doc);

        /// <summary>
        /// Записывает в лог действие "Удаление сдержимого"
        /// </summary>
        /// <param name="doc">Документ, над которым производится действие</param>
        void WriteDeleteDocDetail(F_F_ParameterDoc doc);

        /// <summary>
        /// Записывает в лог действие "Удаление сдержимого не удалось"
        /// </summary>
        /// <param name="doc">Документ, над которым производится действие</param>
        void WriteDeleteDocDetailAbort(F_F_ParameterDoc doc);

        /// <summary>
        /// Записывает в лог действие "Прикрепление нового файла"
        /// </summary>
        /// <param name="doc">Файл, над которым производится действие</param>
        void WriteCreateFile(F_Doc_Docum doc);
        
        /// <summary>
        /// Записывает в лог действие "Удаление прикрепленного файла"
        /// </summary>
        /// <param name="doc">Файл, над которым производится действие</param>
        void WriteDeleteFile(F_Doc_Docum doc);
    }
}
