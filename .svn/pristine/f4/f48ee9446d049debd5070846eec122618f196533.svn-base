using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    /// <summary>
    /// Базовый класс для закачек, включающих индивидуальную установку иерархии классификаторов и 
    /// коррекцию сумм по иерархии классификаторов на этапе обработки данных.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Структуры, перечисления

        /// <summary>
        /// Модификатор обработки классификатора. Нужен для сообщения функции обработки какой классификатор 
        /// обрабатывается и что с ним делать
        /// </summary>
        protected enum ClsProcessModifier
        {
            /// <summary>
            /// ЭКР.Расходы
            /// </summary>
            EKR,

            /// <summary>
            /// ФКР.Расходы
            /// </summary>
            FKR,

            /// <summary>
            /// ЭКР.СправРасходы
            /// </summary>
            EKRBook,

            /// <summary>
            /// ФКР.СправРасходы
            /// </summary>
            FKRBook,

            /// <summary>
            /// Показатели.Расходы
            /// </summary>
            MarksOutcomes,

            MarksInDebt,
            MarksOutDebt,

            /// <summary>
            /// Все классификаторы
            /// </summary>
            AllClassifiers,

            /// <summary>
            /// Индивидуальная обработка классификатора
            /// </summary>
            Special,

            /// <summary>
            /// Стандартная обработка классификатора
            /// </summary>
            Standard,

            /// <summary>
            /// Источники внутреннего финансирования
            /// </summary>
            SrcInFin,

            /// <summary>
            /// Источники внешнего финансирования
            /// </summary>
            SrcOutFin,

            /// <summary>
            /// задолженности
            /// </summary>
            Arrears,

            // справ остатки
            Excess,
            // конс расходы
            Account, 

            /// <summary>
            /// Кэшируется не все значение атрибута, а только та часть, которая является кодом классификатора
            /// </summary>
            CacheSubCode
        }

        #endregion Структуры, перечисления


        #region Инициализация

        /// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="scheme">Ссылка на интерфейс объекта схемы</param>
        public CorrectedPumpModuleBase()
		{

		}

		/// <summary>
		/// Деструктор
		/// </summary>
        protected override void Dispose(bool disposing)
		{
            if (disposing)
            {
                if (corrFK2FO != null) corrFK2FO.Clear();

                if (corrFO2FK != null) corrFO2FK.Clear();
            }
        }

        #endregion Инициализация


        #region Общие функции

        /// <summary>
        /// Возвращает индекс элемента массива годов классификаторов, соответствующего параметрам источника
        /// </summary>
        /// <param name="clsYears">Массив годов классификаторов</param>
        /// <returns>Индекс</returns>
        protected int GetYearIndexByYear(int[] clsYears)
        {
            if (clsYears == null || clsYears.GetLength(0) == 0) return 0;

            Array.Sort(clsYears);
            for (int i = clsYears.GetLength(0) - 1; i >= 0; i--)
            {
                if (this.DataSource.Year >= clsYears[i]) return i;
            }

            return 0;
        }

        #endregion Общие функции


        #region Обработка данных

        /// <summary>
        /// Запрос данных для обработки
        /// </summary>
        protected override void QueryDataForProcess()
        {
            QueryData();
        }

        /// <summary>
        /// Функция коррекции сумм фактов по данным источника
        /// </summary>
        /// <param name="sourceID">ИД источника</param>
        protected override void ProcessDataSource()
        {

        }

        /// <summary>
        /// Этап обработки данных
        /// </summary>
        protected override void DirectProcessData()
        {
            ProcessDataSourcesTemplate("Коррекция сумм фактов");
        }

        #endregion Обработка данных
    }
}