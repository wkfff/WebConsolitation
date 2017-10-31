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
using Krista.FM.Common.Xml;
using Krista.FM.Server.Common;
using Krista.FM.Server.DataPumps.Common;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.DataPumps
{
    // Модуль с функциями установки иерархии классификаторов

    /// <summary>
    /// Базовый класс для закачек, включающих индивидуальную установку иерархии классификаторов и 
    /// коррекцию сумм по иерархии классификаторов на этапе обработки данных.
    /// </summary>
    public abstract partial class CorrectedPumpModuleBase : DataPumpModuleBase
    {
        #region Структуры, перечисления

        /// <summary>
        /// Вид иерархии классификатора
        /// </summary>
        protected enum ClsHierarchyMode
        {
            /// <summary>
            /// Стандартная иерархия классификатора
            /// </summary>
            Standard,

            /// <summary>
            /// Индивидуальная иерархия классификатора
            /// </summary>
            Special,

            /// <summary>
            /// Плоский классификатор
            /// </summary>
            NoHierarchy,

            /// <summary>
            /// КД 2004
            /// </summary>
            KD2004,

            /// <summary>
            /// Показатели.Расходы
            /// </summary>
            MarksOutcomes,

            /// <summary>
            /// Подчиненный код в иерархии классификатора начинается с родительского кода
            /// </summary>
            StartCodeHierarchy,

            // фкр (понадобилось для свода бюджета)
            FKR
        }

        #endregion Структуры, перечисления


        #region Константы

        /// <summary>
        /// Имя каталога с файлами данных об иерархии классификаторов
        /// </summary>
        private const string constClsHierarchyDirName = "ClsHierarchy";

        // закачка 1 нм - фнс 03,  классификатор кд
        public const string const_d_KD_FNS3_HierarchyFile2004 = "d_KD_FNS3_2004.xml";
        public const string const_d_KD_FNS3_HierarchyFile2005 = "d_KD_FNS3_2005.xml";
        public const string const_d_KD_FNS3_HierarchyFile2006 = "d_KD_FNS3_2006.xml";
        public const string const_d_KD_FNS3_HierarchyFile2007 = "d_KD_FNS3_2007.xml";
        public const string const_d_KD_FNS3_HierarchyFile2008 = "d_KD_FNS3_2008.xml";
        public const string const_d_KD_FNS3_HierarchyFile2009 = "d_KD_FNS3_2009.xml";
        public const string const_d_KD_FNS3_HierarchyFile2010 = "d_KD_FNS3_2010.xml";
        public const string const_d_KD_FNS3_HierarchyFile201007 = "d_KD_FNS3_201007.xml";
        public const string const_d_KD_FNS3_HierarchyFile2011 = "d_KD_FNS3_2011.xml";
        public const string const_d_KD_FNS3_HierarchyFile2012 = "d_KD_FNS3_2012.xml";

        // закачка 4 нм - фнс 06, классификатор задолженности
        public const string const_d_Arrears_FNS6_HierarchyFile2005 = "d_Arrears_FNS6_2005.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile2007 = "d_Arrears_FNS6_2007.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile2009 = "d_Arrears_FNS6_2009.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile2010 = "d_Arrears_FNS6_2010.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile2011 = "d_Arrears_FNS6_2011.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile201108 = "d_Arrears_FNS6_201108.xml";
        public const string const_d_Arrears_FNS6_HierarchyFile2012 = "d_Arrears_FNS6_2012.xml";

        // закачка 4 нм - фнс рф 01, классификатор задолженности
        public const string const_d_Arrears_FNSRF1_HierarchyFile2005 = "d_Arrears_FNSRF1_2005.xml";
        public const string const_d_Arrears_FNSRF1_HierarchyFile2006 = "d_Arrears_FNSRF1_2006.xml";
        public const string const_d_Arrears_FNSRF1_HierarchyFile2009 = "d_Arrears_FNSRF1_2009.xml";
        public const string const_d_Arrears_FNSRF1_HierarchyFile2010 = "d_Arrears_FNSRF1_2010.xml";
        public const string const_d_Arrears_FNSRF1_HierarchyFile2011 = "d_Arrears_FNSRF1_2011.xml";

        // закачка 5 нио - фнс 4, классификатор Показатели.ФНС 5 НИО 
        public const string const_d_Marks_FNS4_HierarchyFile2007 = "d_MARKS_FNS4_2007.xml";
        public const string const_d_Marks_FNS4_HierarchyFile2010 = "d_MARKS_FNS4_2010.xml";

        // закачка 5 тн - фнс 7, классификатор Показатели.ФНС 5 ТН 
        public const string const_d_Marks_FNS7_HierarchyFile2007 = "d_MARKS_FNS7_2007.xml";
        public const string const_d_Marks_FNS7_HierarchyFile2009 = "d_MARKS_FNS7_2009.xml";
        public const string const_d_Marks_FNS7_HierarchyFile2010 = "d_MARKS_FNS7_2010.xml";
        
        // закачка 5 ндфл - фнс 10, классификатор Показатели.ФНС 5 НДФЛ 
        public const string const_d_MARKS_FNS10_HierarchyFile2006 = "d_MARKS_FNS10_2006.xml";
        public const string const_d_MARKS_FNS10_HierarchyFile2008 = "d_MARKS_FNS10_2008.xml";
        public const string const_d_MARKS_FNS10_HierarchyFile2009 = "d_MARKS_FNS10_2009.xml";
        public const string const_d_MARKS_FNS10_HierarchyFile2010 = "d_MARKS_FNS10_2010.xml";

        // закачка 5 усн - фнс 13, классификатор Показатели.ФНС 5 УСН 
        public const string const_d_Marks_FNS13_HierarchyFile2007 = "d_MARKS_FNS13_2007.xml";
        
        // закачка 5 ндпи - фнс 14, классификатор показателей
        public const string const_d_Marks_FNS14_HierarchyFile2007 = "d_MARKS_FNS14_2007.xml";
        public const string const_d_Marks_FNS14_HierarchyFile2009 = "d_MARKS_FNS14_2009.xml";

        // закачка 1 ном - фнс 15, классификатор ОКВЭД
        public const string const_d_OKVED_FNS15_HierarchyFile2005 = "d_OKVED_FNS15_2005.xml";
        public const string const_d_OKVED_FNS15_HierarchyFile2006 = "d_OKVED_FNS15_2006.xml";
        public const string const_d_OKVED_FNS15_HierarchyFile2007 = "d_OKVED_FNS15_2007.xml";

        // закачка 1 ном - фнс рф 03, классификатор оквэд
        public const string const_d_OKVED_FNSRF3_HierarchyFile2005 = "d_OKVED_FNSRF3_2005.xml";
        public const string const_d_OKVED_FNSRF3_HierarchyFile2006 = "d_OKVED_FNSRF3_2006.xml";
        public const string const_d_OKVED_FNSRF3_HierarchyFile2007 = "d_OKVED_FNSRF3_2007.xml";

        // закачка 5 жм - фнс 17, классификатор Показатели.ФНС 5 ЖМ
        public const string const_d_Marks_FNS17_HierarchyFile2007 = "d_MARKS_FNS17_2007.xml";

        // закачка 5 пв - фнс 18, классификатор Показатели.ФНС 5 ПВ
        public const string const_d_Marks_FNS18_HierarchyFile2008 = "d_MARKS_FNS18_2008.xml";
        
        // закачка 5 мн - фнс 22, классификатор показателей
        public const string const_d_Marks_FNS22_HierarchyFile2006 = "d_MARKS_FNS22_2006.xml";
        public const string const_d_Marks_FNS22_HierarchyFile2007 = "d_MARKS_FNS22_2007.xml";
        public const string const_d_Marks_FNS22_HierarchyFile2010 = "d_MARKS_FNS22_2010.xml";

        // закачка 4 ном - фнс 23, классификатор ОКВЭД
        public const string const_d_OKVED_FNS23_HierarchyFile2005 = "d_OKVED_FNS23_2005.xml";
        public const string const_d_OKVED_FNS23_HierarchyFile2007 = "d_OKVED_FNS23_2007.xml";
        public const string const_d_OKVED_FNS23_HierarchyFile2009 = "d_OKVED_FNS23_2009.xml";

        // закачка 5 ал - фнс 27, классификатор показателей
        public const string const_d_Marks_FNS27_HierarchyFile2007 = "d_MARKS_FNS27_2007.xml";
        public const string const_d_Marks_FNS27_HierarchyFile2008 = "d_MARKS_FNS27_2008.xml";
        public const string const_d_Marks_FNS27_HierarchyFile2009 = "d_MARKS_FNS27_2009.xml";

        // закачка 5 фл - фнс 28, классификатор показателей
        public const string const_d_Marks_FNS28_HierarchyFile2009 = "d_MARKS_FNS28_2009.xml";
        public const string const_d_Marks_FNS28_HierarchyFile2010 = "d_MARKS_FNS28_2010.xml";

        // закачка 1 патент - фнс 29, классификатор показателей
        public const string const_d_Marks_FNS29_HierarchyFile2010 = "d_MARKS_FNS29_2010.xml";

        // закачка фо 25, классификатор Расходы.ФО_Форма14 (d.R.FOF14)
        public const string const_d_Outcomes_FO25_HierarchyFile2007 = "d_Outcomes_FO25_2007.xml";
        // закачка фо 25, классификатор Должности.ФО_Форма14 (d.Post.FOF14)
        public const string const_d_Post_FO25_HierarchyFile2007 = "d_Post_FO25_2007.xml";

        // Названия элементов хмл-файла иерархии
        private const string constNodeClsHierarchy = "ClsHierarchy";
        private const string constNodeParentChildPair = "//ParentChildPair";
        private const string constAttrCode = "Code";
        private const string constAttrParentCode = "ParentCode";

        #endregion Константы

        protected bool toSetHierarchy = true;

        #region Функции для работы с файлами данных об иерархии

        /// <summary>
        /// Загружает данные об иерархии классификатора из указанного файла в коллекцию 
        /// (Ключ - ИД элемента, Значение - ИД родительского элемента, -1 если такого нет)
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Коллекция с данными иерархии</returns>
        private Dictionary<int, int> LoadClsHierarchyFromFile(string fileName)
        {
            // Получаем путь к каталогу с файлами настроек
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + constClsHierarchyDirName);
            if (!dir.Exists)
                throw new FileNotFoundException(string.Format("Каталог {0} не найден", dir.FullName));

            // Находим нужный файл
            FileInfo[] files = dir.GetFiles(fileName, SearchOption.AllDirectories);
            if (files.GetLength(0) == 0)
                throw new FileNotFoundException(string.Format("Файл {0} не найден", fileName));

            XmlDocument xd = new XmlDocument();
            xd.Load(files[0].FullName);

            Dictionary<int, int> result = new Dictionary<int, int>(200);

            XmlNodeList xnl = xd.SelectNodes(constNodeParentChildPair);
            for (int i = 0; i < xnl.Count; i++)
            {
                XmlNode xn = xnl[i];
                int code = XmlHelper.GetIntAttrValue(xn, constAttrCode, -1);
                int parentCode = XmlHelper.GetIntAttrValue(xn, constAttrParentCode, -1);
                if (!result.ContainsKey(code))
                    result.Add(code, parentCode);
            }

            return result;
        }

        #endregion Функции для работы с файлами данных об иерархии


        #region Фунции установки иерархии классификаторов

        /// <summary>
        /// Формирует иерархию классификатора.
        /// Принцип работы: выбирается запись по parentFilter (если их несколько, то берется первая!) и
        /// устанавливается родительской для всех записей по childFilter.
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="parentFilter">Фильтр для поиска родительской записи, пустая строка - зануллить иерархию</param>
        /// <param name="childFilter">Фильтр для поиска подчиненных записей</param>
        protected void FormClsGroupHierarchy(DataTable dt, string parentFilter, string childFilter)
        {
            try
            {
                object parentId = DBNull.Value;
                if (parentFilter != string.Empty)
                {
                    int id = Convert.ToInt32(FindRowFieldValue(dt, parentFilter, "ID", -1));
                    if (id == -1)
                        return;
                    parentId = id;
                }
                DataRow[] rows = dt.Select(childFilter);
                int count = rows.GetLength(0);
                for (int i = 0; i < count; i++)
                    rows[i]["PARENTID"] = parentId;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Выполняет мелкие операции по установке индивидуальной иерархии классификатора
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="clsHierarchyMode">Вид иерархии классификатора</param>
        private void FormClsSpecialHierarchy(DataTable dt, IEntity obj, ClsHierarchyMode clsHierarchyMode)
        {
            switch (clsHierarchyMode)
            {
                case ClsHierarchyMode.KD2004:
                    string codeField = GetClsCodeField((IClassifier)obj);

                    // коды 1020211-1020219 должны подчиняться коду 1020200
                    FormClsGroupHierarchy(dt, string.Format("{0} = '1020200'", codeField),
                        string.Format("{0} >= '1020211' and {0} <= '1020219'", codeField));

                    // коды 1020311-1020319 должны подчиняться коду 1020300
                    FormClsGroupHierarchy(dt, string.Format("{0} = '1020300'", codeField),
                        string.Format("{0} >= '1020311' and {0} <= '1020319'", codeField));

                    // коды 1400311-1400319 должны подчиняться коду 1400300
                    FormClsGroupHierarchy(dt, string.Format("{0} = '1400300'", codeField),
                        string.Format("{0} >= '1400311' and {0} <= '1400319'", codeField));

                    // коды 2010211-2010219 должны подчиняться коду 2010200
                    FormClsGroupHierarchy(dt, string.Format("{0} = '2010200'", codeField),
                        string.Format("{0} >= '2010211' and {0} <= '2010219'", codeField));

                    // коды 2010611-2010619 должны подчиняться коду 2010600
                    FormClsGroupHierarchy(dt, string.Format("{0} = '2010600'", codeField),
                        string.Format("{0} >= '2010611' and {0} <= '2010619'", codeField));

                    // коды 3020240-3020259 должны подчиняться коду 3020230
                    FormClsGroupHierarchy(dt, string.Format("{0} = '3020230'", codeField),
                        string.Format("{0} >= '3020240' and {0} <= '3020259'", codeField));

                    // понадобилось в своде бюджета
                    DataRow[] rows = dt.Select(string.Format("{0} = '3029000'", codeField));
                    if (rows.GetLength(0) != 0)
                        rows[0]["PARENTID"] = DBNull.Value; 
                    FormClsGroupHierarchy(dt, string.Format("{0} = '3029000'", codeField),
                        string.Format("{0} = '3020105' or {0} = '3020205' or {0} = '3020305' or {0} = '3020303' or {0} = '3020405' or {0} = '3020505'", codeField));

                    // свод бюджета с 2007 года (и мес отч)
                    if (this.DataSource.Year >= 2007)
                    {
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020200000000000000'", codeField),
                            string.Format("{0} = '00020205000020000151' or {0} = '00020205000030000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020201000000000151'", codeField),
                            string.Format("{0} = '00020201030020000151' or {0} = '00020201060020000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020205000000000151'", codeField),
                            string.Format("{0} = '00020205010020000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020205300070000151'", codeField),
                            string.Format("{0} = '00020205307070000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020202000000000151'", codeField),
                            string.Format("{0} = '00020202132020000151' or {0} = '00020202037040000151' or {0} = '00020202038000000151' or {0} = '00020202039000000151'" +
                                          "or {0} = '00020202012000000151' or {0} = '00020202013000000151' or {0} = '00020202014000000151' or {0} = '00020202015000000151'" +
                                          "or {0} = '00020202016000000151' or {0} = '00020202017000000151' or {0} = '00020202018000000151' or {0} = '00020202019000000151'" +
                                          "or {0} = '00020202021000000151' or {0} = '00020202022000000151' or {0} = '00020202023000000151' or {0} = '00020202024020000151'" +
                                          "or {0} = '00020202025000000151' or {0} = '00020202026000000151' or {0} = '00020202027020000151' or {0} = '00020202028000000151'" +
                                          "or {0} = '00020202029020000151' or {0} = '00020202032020000151' or {0} = '00020202034020000151' or {0} = '00020202035020000151'" +
                                          "or {0} = '00020202042000000151' or {0} = '00020202043000000151' or {0} = '00020202044000000151' or {0} = '00020202045020000151'" +
                                          "or {0} = '00020202046000000151' or {0} = '00020202047000000151' or {0} = '00020202048000000151' or {0} = '00020202049050000151'" +
                                          "or {0} = '00020202051000000151' or {0} = '00020202052000000151' or {0} = '00020202053000000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020204000000000151'", codeField),
                            string.Format("{0} = '00020204011020000151' or {0} = '00020204052000000151' or {0} = '00020204023000000151' or {0} = '00020204025000000151'" +
                                          "or {0} = '00020204016020000151' or {0} = '00020204017020000151' or {0} = '00020204028000000151' or {0} = '00020204022020000151'" +
                                          "or {0} = '00020204024020000151' or {0} = '00020204031000000151' or {0} = '00020204033000000151' or {0} = '00020204034000000151'" +
                                          "or {0} = '00020204035000000151' or {0} = '00020204036000000151' or {0} = '00020204038000000151' or {0} = '00020204039000000151'" +
                                          "or {0} = '00020204041000000151' or {0} = '00020204042000000151' or {0} = '00020204044000000151' or {0} = '00020204046000000151'" +
                                          "or {0} = '00020204048000000151' or {0} = '00020204049000000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020205100060000151'", codeField),
                            string.Format("{0} = '00020205110060000151' or {0} = '00020205111060000151' or {0} = '00020205112060000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020202000000000151'", codeField),
                            string.Format("{0} = '00020202041020000151' or {0} = '00020202033010000152'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020204010000000151'", codeField),
                            string.Format("{0} = '00020204012020000151' or {0} = '00020204013020000151' or {0} = '00020204014020000151' or {0} = '00020204015020000151'" +
                                          "or {0} = '00020204018020000151' or {0} = '00020204011020000151' or {0} = '00020204016020000151'" + 
                                          "or {0} = '00020204017020000151'", codeField));
                        FormClsGroupHierarchy(dt, string.Format("{0} = '00020204000000000151'", codeField),
                            string.Format("{0} = '00020204033020000151' or {0} = '00020204032020000151' or {0} = '00020204037020000151' or {0} = '00020204043020000151'" +
                                          "or {0} = '00020204045020000151' or {0} = '00020204047020000151'", codeField));
                    }

                    break;
                case ClsHierarchyMode.MarksOutcomes:
                    if (this.DataSource.Year >= 2005)
                    {
                        // Показатели.МесОтч_СправРасходы для 2005 года сделать иерархию, где KL=69 и 70. 
                        // Все показатели KL=69 должны подчиняться показателю KL=69 и KST=1. 
                        FormClsGroupHierarchy(dt, "KL = 69 and KST = 1", "KL = 69 and KST <> 1");

                        // Все показатели KL=70 должны подчиняться показателю KL=70 и KST=1.
                        FormClsGroupHierarchy(dt, "KL = 70 and KST = 1", "KL = 70 and KST <> 1");
                    }

                    break;
            }
        }

        /// <summary>
        /// Устанавливает иерархию классификатора
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="hierarchy">Коллекция, содержащая описание иерархии классификатора</param>
        /// <param name="hierarchyField">Поле, по которому строится иерархия</param>
        /// <param name="clsHierarchyMode">Вид иерархии классификатора</param>
        private void FormClsHierarchy(DataTable dt, Dictionary<int, int> hierarchy, 
            string hierarchyField, IClassifier cls, ClsHierarchyMode clsHierarchyMode)
        {
            if (dt == null)
                return;

            string semantic = cls.FullCaption;

            SetProgress(-1, -1, string.Format("Установка иерархии {0}...", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0}...", semantic), TraceMessageKind.Information);

            int valueForHierarchy = -1;

            //string ss = CommonRoutines.ShowDataTable(dt, false, -1);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int id = Convert.ToInt32(dt.Rows[i]["ID"]);
                int value = Convert.ToInt32(dt.Rows[i][hierarchyField]);

                if (!hierarchy.ContainsKey(value))
                {
                    valueForHierarchy = -1;
                }
                else
                {
                    valueForHierarchy = value;
                }

                // а зачем? может некоторая иерархия была установлена вручную...
                //dt.Rows[i]["PARENTID"] = DBNull.Value;
                if (hierarchy.ContainsKey(valueForHierarchy) && hierarchy[valueForHierarchy] != value)
                {
                    if (hierarchy[valueForHierarchy] < 0)
                    {
                        dt.Rows[i]["ParentID"] = DBNull.Value;
                    }
                    else
                    {
                        int parentID = FindRowID(dt,
                            string.Format("{0} = {1}", hierarchyField, hierarchy[valueForHierarchy]), -1);
                        if (parentID > 0)
                            dt.Rows[i]["PARENTID"] = parentID;
                        else
                        {
                            dt.Rows[i]["ParentID"] = DBNull.Value;
                        }
                    }
                }
            }

            FormClsSpecialHierarchy(dt, cls, clsHierarchyMode);

            SetProgress(-1, -1, string.Format("Установка иерархии {0} закончена.", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0} закончена.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Устанавливает иерархию классификатора по умолчанию
        /// </summary>
        /// <param name="clsTable">Таблица классификатора</param>
        /// <param name="cls">IClassifier</param>
        /// <param name="clsHierarchyMode">Вид иерархии классификатора</param>
        protected void FormStandardHierarchy(ref DataSet clsDataSet, IClassifier cls, 
            ClsHierarchyMode clsHierarchyMode)
        {
            string semantic = cls.FullCaption;
            SetProgress(-1, -1, string.Format("Установка иерархии {0}...", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0}...", semantic), TraceMessageKind.Information);
            cls.DivideAndFormHierarchy(this.SourceID, this.PumpID, ref clsDataSet);
            if (clsHierarchyMode == ClsHierarchyMode.KD2004)
                if ((this.DataSource.Year <= 2004) || (this.DataSource.Year >= 2007))
                    FormClsSpecialHierarchy(clsDataSet.Tables[0], cls, ClsHierarchyMode.KD2004);
            SetProgress(-1, -1, string.Format("Установка иерархии {0} закончена.", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0} закончена.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Возвращает родительский код для указанного кода классификатора.
        /// Применяется при установке иерархии, когда подчиненный код начинается с родительского кода.
        /// </summary>
        /// <param name="code">Код</param>
        /// <returns>Родительский код</returns>
        private string GetParentCode(string code)
        {
            // Kоды 010-012 должны быть подчинены коду 10
            if (code == "010" || code == "011" || code == "012")
            {
                return "10";
            }

            // Коды 210-213 должны быть подчинены коду 20
            if (code == "210" || code == "211" || code == "212" || code == "213")
            {
                return "20";
            }

            return code.Substring(0, code.Length - 1);
        }

        /// <summary>
        /// Устанавливает иерархию классификатора.
        /// Подчиненный код в иерархии классификатора начинается с родительского кода.
        /// </summary>
        /// <param name="dt">Таблица классификатора</param>
        /// <param name="hierarchyField">Поле, по которому строится иерархия</param>
        private void FormClsHierarchyByStartCode(DataTable dt, string hierarchyField, IClassifier cls)
        {
            if (dt == null)
                return;

            string semantic = cls.FullCaption; 

            SetProgress(-1, -1, string.Format("Установка иерархии {0}...", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0}...", semantic), TraceMessageKind.Information);

            // Кэш классификатора.
            // Ключ - код, значение - строка
            Dictionary<string, int> cache = null;// new Dictionary<string, int>(2000);
            FillRowsCache(ref cache, dt, hierarchyField);

            // Устанавливаем иерархию
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string code = Convert.ToString(dt.Rows[i][hierarchyField]);

                // Если длина кода 2 знака, то пропускаем - это родительский код
                if (code.Length == 2)
                {
                    continue;
                }
                // Если длина кода больше 2 знаков, то ищем родительский код по всем символам кода, кроме последнего
                else
                {
                    string parentCode = GetParentCode(code);
                    if (cache.ContainsKey(parentCode))
                    {
                        dt.Rows[i]["PARENTID"] = cache[parentCode];
                    }
                }
            }

            SetProgress(-1, -1, string.Format("Установка иерархии {0} закончена.", semantic), string.Empty, true);
            WriteToTrace(string.Format("Установка иерархии {0} закончена.", semantic), TraceMessageKind.Information);
        }

        /// <summary>
        /// Устанавливает иерархию указанного классификатора
        /// </summary>
        /// <param name="clsDataSet">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="hierarchy">Коллекция, содержащая описание иерархии классификатора (по годам)</param>
        /// <param name="hierarchyField">Поле, по которому строится иерархия</param>
        /// <param name="clsHierarchyMode">Вид иерархии классификатора</param>
        protected void SetClsHierarchy(ref DataSet clsDataSet, IClassifier cls, Dictionary<int, int> hierarchy, 
            string hierarchyField, ClsHierarchyMode clsHierarchyMode)
        {
            // Определяем индекс года текущего источника в массиве всех лет.
            // Нужно для того, чтобы определить, что брать из других массивов.
            if (clsDataSet.Tables[0].Rows.Count == 0)
            {
                return;
            }
            // Установка иерархии классификаторов
            switch (clsHierarchyMode)
            {
                case ClsHierarchyMode.MarksOutcomes:
                case ClsHierarchyMode.Special:
                    FormClsHierarchy(clsDataSet.Tables[0], hierarchy, hierarchyField, cls, clsHierarchyMode);
                    break;
                case ClsHierarchyMode.StartCodeHierarchy:
                    FormClsHierarchyByStartCode(clsDataSet.Tables[0], hierarchyField, cls);
                    break;
                case ClsHierarchyMode.FKR:
                    FormStandardHierarchy(ref clsDataSet, cls, clsHierarchyMode);
                    FormClsGroupHierarchy(clsDataSet.Tables[0], string.Format("{0} = '3290 '", "CODE"),
                        string.Format("{0} = '3203' or {0} = '3213' or {0} = '3223' or {0} = '3225' or {0} = '3233' or {0} = '3235' or {0} = '3243'", "CODE"));
                    break;
                default:
                    FormStandardHierarchy(ref clsDataSet, cls, clsHierarchyMode);
                    break;
            }
        }

        /// <summary>
        /// Устанавливает иерархию указанного классификатора
        /// </summary>
        /// <param name="clsDataSet">Таблицы классификаторов</param>
        /// <param name="cls">Объекты классификаторов, соответствующие clsTable</param>
        /// <param name="hierarchy">Коллекция, содержащая описание иерархии классификатора (по годам)</param>
        /// <param name="hierarchyField">Поле, по которому строится иерархия</param>
        /// <param name="hierarchyFileName">Файл с данными иерархии</param>
        /// <param name="clsHierarchyMode">Вид иерархии классификатора</param>
        protected void SetClsHierarchy(IClassifier cls, ref DataSet clsDataSet,  
            string hierarchyField, string hierarchyFileName, ClsHierarchyMode clsHierarchyMode)
        {
            SetClsHierarchy(ref clsDataSet, cls, LoadClsHierarchyFromFile(hierarchyFileName), hierarchyField, 
               clsHierarchyMode);
        }

        protected void ClearHierarchy(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
                row["ParentId"] = DBNull.Value;
        }

        #endregion Фунции установки иерархии классификаторов


        #region Установка иерархии

        /// <summary>
        /// Установка иерархии классификаторов после закачки данных. 
        /// </summary>
        protected override void DirectClsHierarchySetting()
        {
            if (!toSetHierarchy)
            {
                WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                    "Иерархия установлена в процессе закачки.");
                return;
            }
            switch (this.PumpProgramID)
            {
                case PumpProgramID.SKIFMonthRepPump:
                case PumpProgramID.SKIFYearRepPump:
                case PumpProgramID.Form1NMPump:
                case PumpProgramID.BudgetVaultPump:
                case PumpProgramID.FNS23Pump:
                    WriteEventIntoDataPumpProtocol(DataPumpEventKind.dpeInformation,
                        "Иерархия установлена в процессе закачки.");
                    break;
                default:
                    base.DirectClsHierarchySetting();
                    break;
            }
        }

        #endregion Установка иерархии
    }
}