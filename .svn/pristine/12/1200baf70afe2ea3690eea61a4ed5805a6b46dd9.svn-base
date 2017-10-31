using System;
using System.Data;
using System.Linq;


namespace Krista.FM.Server.Dashboards.MinSportSupport
{
    public static class ProcessingQueryResult
    {
        /// <summary>
        /// Удаление строк из таблицы
        /// </summary>
        /// <param name="table"></param>
        public static void DeleteEmptyValues(DataTable table)
        {   
            DataRow[] emptyRows = table.Select("Значение is null");
            foreach (var element in emptyRows)
            {
                table.Rows.Remove(element);
            }
        }

        /// <summary>
        /// Преобразование кода территории ЕМИСС_ОКАТО
        /// </summary>
        /// <param name="table">таблица с данными</param>
        public static void UpdateTerritoryValue(DataTable table)
        {
            foreach (DataRow rows in table.Rows)
            {
                rows["Territory"] = rows["Territory"].ToString().Replace("_", "");
            }
            DataRow[] emptyRows = table.Select("Territory=''");
            foreach (var element in emptyRows)
            {
                table.Rows.Remove(element);
            }
        }

        /// <summary>
        /// Создание темповой таблицы для хранения временных данных
        /// </summary>
        /// <returns>темповая таблица</returns>
        public static DataTable CreateTempDataTable()
        {
            var tempTable = new DataTable();
            tempTable.Columns.Add("dummy");
            tempTable.Columns.Add("Год");
            tempTable.Columns.Add("Отчетный период");
            tempTable.Columns.Add("Значение");
            tempTable.Columns.Add("Единица измерения");
            tempTable.Columns.Add("Справочник_Территория");
            tempTable.Columns.Add("Territory");
            for (var i = 1; i <= XmlWorker.GetFactorUseHandBooks().Count; i++)
            {
                tempTable.Columns.Add(String.Format("Справочник_{0}", i));
                tempTable.Columns.Add(String.Format("ЭлементСправочника_{0}", i));
            }
            return tempTable;
        }

        /// <summary>
        /// Обработка одиночного справочника
        /// </summary>
        public static void ProcessingUseHandBookData(DataTable table, DataTable tempTable, string handBookCode, Factor factor)
        {
            foreach (DataRow row in table.Rows)
            {
                tempTable.Rows.Add();
                tempTable.Rows[tempTable.Rows.Count - 1].BeginEdit();
                FillCommonColumns(tempTable, row, factor);
                FillHandBookElements(tempTable, handBookCode, row);
                FillParentHandBookElemens(tempTable, handBookCode, row);
                tempTable.Rows[tempTable.Rows.Count - 1].EndEdit();
            }
        }

        /// <summary>
        /// Обработка пересекающихся справочников
        /// </summary>
        public static void ProcessingCrossHandBookData(DataTable table, DataTable tempTable, CrossHandBooks crossHandBook, Factor factor)
        {
            foreach (DataRow row in table.Rows)
            {
                tempTable.Rows.Add();
                tempTable.Rows[tempTable.Rows.Count - 1].BeginEdit();
                FillCommonColumns(tempTable, row, factor);
                FillHandBookElements(tempTable, crossHandBook.HandBookOnRows, row);
                FillHandBookElements(tempTable, crossHandBook.HandBookOnColumns, row);
                FillParentHandBookElemens(tempTable, crossHandBook.HandBookOnRows, row);
                FillParentHandBookElemens(tempTable, crossHandBook.HandBookOnColumns, row);
                tempTable.Rows[tempTable.Rows.Count - 1].EndEdit();
            }
        }

        /// <summary>
        /// Обработка элементов "всего"
        /// </summary>
        public static void ProcessingTotalElements(DataTable table, DataTable tempTable, Factor factor)
        {
            foreach (DataRow row in table.Rows)
            {
                tempTable.Rows.Add();
                FillCommonColumns(tempTable, row, factor); 
            }
        }

        /// <summary>
        /// Заполнение общих столбцов показателя
        /// </summary>
        private static void FillCommonColumns(DataTable tempTable, DataRow row, Factor factor)
        {
            tempTable.Rows[tempTable.Rows.Count - 1].BeginEdit();
            tempTable.Rows[tempTable.Rows.Count - 1]["dummy"] = row["dummy"];
            tempTable.Rows[tempTable.Rows.Count - 1]["Territory"] = row["Territory"];
            tempTable.Rows[tempTable.Rows.Count - 1]["Значение"] = row["Значение"];
            tempTable.Rows[tempTable.Rows.Count - 1]["Год"] = factor.WorkYear;
            tempTable.Rows[tempTable.Rows.Count - 1]["Отчетный период"] = factor.TimeProvision;
            tempTable.Rows[tempTable.Rows.Count - 1]["Единица измерения"] = factor.Unit;
            tempTable.Rows[tempTable.Rows.Count - 1]["Справочник_Территория"] = "mОКАТО";
            tempTable.Rows[tempTable.Rows.Count - 1].EndEdit();  
        }

        /// <summary>
        /// Заполнение столбцов-разрезов показателя
        /// </summary>
        private static void FillHandBookElements(DataTable tempTable, string handBookCode, DataRow row)
        {
            tempTable.Rows[tempTable.Rows.Count - 1][UseHandBooksManager.GetColumnNameHandBook(handBookCode)] = handBookCode;
            tempTable.Rows[tempTable.Rows.Count - 1][UseHandBooksManager.GetColumnNameElement(handBookCode)] =
                XmlWorker.GetHandBookECode(handBookCode,
                                           (UseHandBooksManager.GetLayout(handBookCode) == 0)
                                               ? row["rowElement"].ToString()
                                               : row["colElement"].ToString());
        }

        /// <summary>
        /// Заполнение столбцов-разрезов показателя, у которых есть родительские разрезы
        /// </summary>
        private static void FillParentHandBookElemens(DataTable tempTable, string handBookCode, DataRow row)
        {
            if (UseHandBooksManager.HasHandBookParent(handBookCode) != null)
            {
                var handBookParent = UseHandBooksManager.HasHandBookParent(handBookCode);
                tempTable.Rows[tempTable.Rows.Count - 1][UseHandBooksManager.GetColumnNameHandBook(handBookParent)] = handBookParent;
                tempTable.Rows[tempTable.Rows.Count - 1][UseHandBooksManager.GetColumnNameElement(handBookParent)] =
                    XmlWorker.GetHandBookParentECode(handBookCode,
                                               (UseHandBooksManager.GetLayout(handBookParent) == 0)
                                                   ? row["rowElement"].ToString()
                                                   : row["colElement"].ToString());
            }
        }

        /// <summary>
        /// Заполнение пустых ячеек, оставшихся после обработки, кодами всего
        /// </summary>
        /// <param name="tempTable">Таблица с данными по показателю</param>
        public static void FillTotalElements(DataTable tempTable)
        {
            foreach (DataRow row in tempTable.Rows)
            { 
                for (var i = 6; i <= tempTable.Columns.Count - 1; i++)
                {
                    if (row[tempTable.Columns[i].ColumnName].ToString() == "")
                    {
                        row[tempTable.Columns[i].ColumnName] = UseHandBooksManager.GetHandBookCodeForColumnName(tempTable.Columns[i].ColumnName);  
                    }
                }
            }
        }

        #region Расчетные показатели
        /// <summary>
        /// Рассчет обеспеченности спортивными сооружениями (показатель 2.1.61.11)
        /// </summary>
        public static void ProvisionSportConstruction(DataTable tempTable, DataTable populationTable)
        {
            foreach (DataRow row in tempTable.Rows)
            {
                var area = row["Значение"];
                var okato = row["Territory"];
                double normative = 0;
                switch (row["ЭлементСправочника_1"].ToString())
                {
                    case "47.05.02":
                        normative = 19.5;
                        break;
                    case "47.05.03":
                        normative = 3.5;
                        break;
                    case "47.05.10":
                        normative = 0.75;
                        break;
                }
                if (!DBValueIsEmpty(okato))
                {
                    var population = GetPopulation(populationTable, okato.ToString());
                    if (!DBValueIsEmpty(population) && !DBValueIsEmpty(area) && !DBValueIsEmpty(normative))
                    {
                        row["Значение"] = Math.Round((1000 * Convert.ToDouble(area)) / (normative * Convert.ToInt32(population)), 2);
                    }
                    else
                    {
                        row["Значение"] = 0;
                    }
                }
                
            }
        }

        /// <summary>
        /// Рассчет удельного веса населения, занимающегося спортом (показатель 47.1.10)
        /// </summary>
        public static void PercentSportPopulation(DataTable tempTable, DataTable populationTable)
        {
            foreach (DataRow row in tempTable.Rows)
            {
                var area = row["Значение"];
                var okato = row["Territory"];
                if (!DBValueIsEmpty(okato))
                {
                    var population = GetPopulation(populationTable, okato.ToString());
                    if (!DBValueIsEmpty(population) && !DBValueIsEmpty(area))
                    {
                        row["Значение"] = Math.Round((100 * Convert.ToDouble(area)) / (Convert.ToInt32(population)), 2);
                    }
                    else
                    {
                        row["Значение"] = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Количество спортсооружений 
        /// </summary>
        /// <param name="tempTable"></param>
        public static DataTable QuantitySportsBuilding(DataTable tempTable)
        {
            var test = from row in tempTable.AsEnumerable()
                       group row by
                           new
                           {
                               territory = row.Field<string>("Territory"),
                               spr1 = row.Field<string>("ЭлементСправочника_1"),
                               spr2 = row.Field<string>("ЭлементСправочника_2")
                           }
                           into grp
                           select new
                           {
                               dummy = grp.Select(r => r.Field<string>("dummy")),
                               territory = grp.Select(r => r.Field<string>("Territory")),
                               value = grp.Sum(r => Convert.ToInt32(r.Field<string>("Значение"))),
                               year = grp.Select(r => r.Field<string>("Год")),
                               timeProvision = grp.Select(r => r.Field<string>("Отчетный период")),
                               unit = grp.Select(r => r.Field<string>("Единица измерения")),
                               handBook1 = grp.Select(r => r.Field<string>("Справочник_1")),
                               handBook2 = grp.Select(r => r.Field<string>("Справочник_2")),
                               handBookCode1 = grp.Select(r => r.Field<string>("ЭлементСправочника_1")),
                               handBookCode2 = grp.Select(r => r.Field<string>("ЭлементСправочника_2"))
                           };
           /*foreach (var grp in test)
            {
                foreach (var value in grp)
                {
                    var valueNew = value.Field<int>("Значение");
                }
            }*/
            var additionalTable = tempTable.Clone();

            foreach (var row in test)
            {
                additionalTable.Rows.Add();
                additionalTable.Rows[additionalTable.Rows.Count - 1].BeginEdit();
                additionalTable.Rows[additionalTable.Rows.Count - 1]["dummy"] = row.dummy.ElementAt(0) + " + " + row.dummy.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Territory"] = row.territory.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Значение"] = row.value;
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Год"] = row.year.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Отчетный период"] = row.timeProvision.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Единица измерения"] = row.unit.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Справочник_Территория"] = "mОКАТО";
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Справочник_1"] = row.handBook1.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["Справочник_2"] = row.handBook2.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["ЭлементСправочника_1"] = row.handBookCode1.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1]["ЭлементСправочника_2"] = row.handBookCode2.ElementAt(0);
                additionalTable.Rows[additionalTable.Rows.Count - 1].EndEdit();
            }

            return additionalTable;
        }

        #endregion

        /// <summary>
        /// Показывает пустое ли значение взято из БД
        /// </summary>
        private static bool DBValueIsEmpty(object value)
        {
            return value == DBNull.Value
                   || value == null
                   || String.IsNullOrEmpty(value.ToString());
        }

        /// <summary>
        /// Расчет числености населения для заданной территории
        /// </summary>
        private static string GetPopulation(DataTable table, string okato)
        {
            string result = "";
            foreach (DataRow row in table.Rows)
            {
                if ((string)row["Territory"] == okato)
                {
                    if (!DBValueIsEmpty(row["Population"]))
                    {
                        result = (string)row["Population"];   
                    }
                }
            }
            return result;
        }
    }
}
    