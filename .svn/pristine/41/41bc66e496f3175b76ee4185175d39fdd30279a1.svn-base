namespace Krista.FM.Domain
{
    public class FX_FX_ChangeLogActionType : ClassifierTable
    {
        /// <summary>
        ///   Добавление документа
        /// </summary>
        public const int AddDocument = 13;

        /// <summary>
        ///   Удалнеие документа
        /// </summary>
        public const int DeleteDocument = 2;

        /// <summary>
        ///   Не удалось удалить документ
        /// </summary>
        public const int DeleteDocumentAbort = 3;

        /// <summary>
        ///   Переведен на рассмотрение
        /// </summary>
        public const int OnUnderConsiderationState = 4;

        /// <summary>
        ///   Отправлен на доработку
        /// </summary>
        public const int OnEditingState = 5;

        /// <summary>
        ///   Документ Экспортирован
        /// </summary>
        public const int OnExportedState = 6;

        /// <summary>
        ///   Документ Завершен
        /// </summary>
        public const int OnFinishedState = 7;

        /// <summary>
        ///   В документ внесена новая информация.
        /// </summary>
        public const int DocumentBodyChange = 8;

        /// <summary>
        ///   Информация удалена из документа
        /// </summary>
        public const int DocumentBodyDelete = 9;

        /// <summary>
        ///   Не удалось удалить информацию из документа
        /// </summary>
        public const int DocumentBodyDeleteAbort = 10;

        /// <summary>
        ///   Прикреплен новый файл
        /// </summary>
        public const int FileCreate = 11;

        /// <summary>
        ///   Прикрепленный файл удален
        /// </summary>
        public const int FileDelete = 12;

        public static readonly string Key = "abf6f24d-0935-474c-be50-74ee332339d7";

        public virtual int RowType { get; set; }

        public virtual string Name { get; set; }
    }
}
