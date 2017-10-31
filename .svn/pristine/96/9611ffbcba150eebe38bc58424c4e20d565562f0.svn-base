using System.Text;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.Utilities;
using Krista.FM.RIA.Core;

namespace Krista.FM.RIA.Extensions.EO15ExcCostsAIP.Presentation.Controls
{
    public static class FilterControl
    {
        /// <summary>Добавляет обработчик к комбобоксу на выбор записи (обновление грида) </summary>
        /// <param name="comboBox">Комбобокс, к которому добавить обработчик на обновление грида при выборе значения</param>
        /// <param name="storeIdToUpdate">Идентификатор Store, который нужно обновить</param>
        /// <param name="paramName">Имя параметра для store, по которому нужно его фильтровать</param>
        public static void AddSelectListener(ComboBox comboBox, string storeIdToUpdate, string paramName)
        {
            comboBox.Listeners.Select.AddAfter(
                @"
                {0}.baseParams.{1} = {2}.value;
                {0}.reload();
                ".FormatWith(storeIdToUpdate, paramName, comboBox.ID));
        }

        /// <summary>
        /// Возвращает выпадающий список с периодами, формируемыми по url
        /// </summary>
        /// <param name="page">Объект Page</param>
        /// <param name="url">Url для загрузки данных с периодами</param>
        /// <param name="paramName">Имя атрибута для Store</param>
        /// <param name="paramValue">Значение атрибута</param>
        public static ComboBox GetFilterPeriod(ViewPage page, string url, string paramName, string paramValue)
        {
            var periodStore = CreateFilterStore("periodsStore", url, paramName, paramValue);
            page.Controls.Add(periodStore);
            return CreateFilter(periodStore.ID, "cbPeriod", "Период", 100);
        }

        /// <summary>
        /// Возвращает выпадающий список с кварталами, формируемыми по url
        /// </summary>
        /// <param name="page">Объект Page</param>
        /// <param name="url">Url для загрузки данных с периодами</param>
        /// <param name="paramName">Имя атрибута для Store</param>
        /// <param name="paramValue">Значение атрибута</param>
        public static ComboBox GetFilterQuarterPeriod(ViewPage page, string url, string paramName, string paramValue)
        {
            var periodStore = CreateFilterStore("quartersStore", url, paramName, paramValue);
            page.Controls.Add(periodStore);
            return CreateFilter(periodStore.ID, "cbQuarter", "Квартал", 100);
        }

        /// <summary>
        /// Возвращает список кварталов.
        /// </summary>
        /// <param name="page">Объект Page.</param>
        /// <param name="yearFrom">Год начала периодов.</param>
        /// <param name="yearTo">Год окончания периодов.</param>
        public static ComboBox GetFilterQuarterPeriod(ViewPage page, int yearFrom, int yearTo)
        {
            return GetFilterQuarterPeriod(page, "/EO15AIPRegister/LookupQuarterPeriods?yearFrom={0}&yearTo={1}".FormatWith(yearFrom, yearTo), null, null);
        }

        /// <summary>
        /// Возвращает список периодов с 2010 года по текущий год
        /// </summary>
        /// <param name="page">Объект Page</param>
        public static ComboBox GetFilterPeriod(ViewPage page)
        {
            return GetFilterPeriod(page, "/EO15AIPRegister/LookupPeriods", null, null);
        }

        public static ComboBox GetFilterClient(ViewPage page)
        {
            var clientStore = CreateFilterStore("clientsStore", "/EO15AIPRegister/LookupClients");
            page.Controls.Add(clientStore);
            return CreateFilter(clientStore.ID, "cbClient", @"Заказчик", 300);
        }

        public static ComboBox GetFilterProgram(ViewPage page)
        {
            var programStore = CreateFilterStore("programsStore", "/EO15AIPRegister/LookupPrograms");
            page.Controls.Add(programStore);
            return CreateFilter(programStore.ID, "cbProgram", @"Целевая программа", 300);
        }

        public static ComboBox GetFilterRegion(ViewPage page)
        {
            var regionStore = CreateFilterStore("regionsStore", "/EO15AIPRegister/LookupRegions");
            regionStore.Listeners.Load.Handler = @"var msg = store.reader.jsonData.extraParams;
                if (msg) { 
                    Ext.Msg.alert('Список территорий пуст', msg);
                }
                ";
            page.Controls.Add(regionStore);
            return CreateFilter(regionStore.ID, "cbRegion", @"МО", 300);
        }

        public static ComboBox GetFilterStatus(ViewPage page)
        {
            var statusStore = CreateFilterStore("statusStore", "/EO15AIPRegister/LookupStatus");
            page.Controls.Add(statusStore);
            return CreateFilter(statusStore.ID, "cbStatus", @"Статус", 300);
        }

        public static ComboBox GetFilterExpertise(ViewPage page)
        {
            var expertiseStore = CreateFilterStore("expertiseStore", "/EO15AIPDetailExpertise/LookupExpertise");
            page.Controls.Add(expertiseStore);
            return CreateFilter(expertiseStore.ID, "cbExpertise", @"Экспертиза", 300);
        }

        /// <summary>
        /// Возвращает выпадающий список с годами, формируемыми по url
        /// </summary>
        /// <param name="page">Объект Page</param>
        /// <param name="url">Url для загрузки данных с периодами</param>
        /// <param name="paramName">Имя атрибута для Store</param>
        /// <param name="paramValue">Значение атрибута</param>
        public static MultiCombo GetFilterYearPeriod(ViewPage page, string url, string paramName, string paramValue)
        {
            var periodStore = CreateFilterStore("yearsStore", url, paramName, paramValue);
            page.Controls.Add(periodStore);
            var comboBox = new MultiCombo
            {
                EmptyText = @"Выберите значение",
                StoreID = periodStore.ID,
                ID = "cbYears",
                Editable = false,
                FieldLabel = @"Период",
                TriggerAction = TriggerAction.All,
                ValueField = "ID",
                Width = 100,
                Disabled = false,
                DisplayField = "Name",
                SelectionMode = MultiSelectMode.All
            };

            return comboBox;
        }

        /// <summary>
        /// Возвращает список кварталов
        /// </summary>
        /// <param name="page">Объект Page</param>
        /// <param name="yearFrom">Год начала периодов</param>
        /// /// <param name="yearTo">Год заверешения периодов</param>
        public static MultiCombo GetFilterYearPeriod(ViewPage page, int yearFrom, int yearTo)
        {
            return GetFilterYearPeriod(page, "/EO15AIPRegister/LookupYearPeriods?yearFrom={0}&yearTo={1}".FormatWith(yearFrom, yearTo), null, null);
        }

        /// <summary>
        /// Создает Store для фильтра
        /// </summary>
        /// <param name="storeId">Идентификатор для store</param>
        /// <param name="url">Url для store</param>
        /// <returns>Store для фильтра</returns>
        public static Store CreateFilterStore(string storeId, string url)
        {
            return CreateFilterStore(storeId, url, null, null);
        }

        public static Store CreateFilterStore(string storeId, string url, string paramName, string paramValue)
        {
            var ds = new Store
            {
                ID = storeId,
                AutoLoad = true,
            };

            ds.SetHttpProxy(url)
                .SetJsonReader()
                .AddField("ID")
                .AddField("Name");

            if (paramName != null)
            {
                ds.BaseParams.Add(new Parameter(paramName, paramValue, ParameterMode.Raw));
            }

            return ds;
        }

        /// <summary>
        /// Создает выпадающий список - Editor для колонки
        /// </summary>
        /// <param name="storeId">Идентификатор Store для выпадающего списка</param>
        /// <param name="comboId">Идентификатор выпадающего списка</param>
        /// <returns>Выпадающий список - Editor</returns>
        public static ComboBox CreateEditorCombo(string storeId, string comboId)
        {
            var comboBox = new ComboBox
                               {
                                   ID = comboId,
                                   EmptyText = @"Выберите значение",
                                   StoreID = storeId,
                                   Editable = false,
                                   TriggerAction = TriggerAction.All,
                                   ValueField = "ID",
                                   DisplayField = "Name",
                                   ItemSelector = "tr.search-item",
                                   Disabled = false
                               };
            var tableTemplate = new StringBuilder(@"
<table width=100%>
    <tpl for=""."">
        <tr class=""search-item"">
            ")
                .Append(@"<td class=""search-item primary"">{{{0}}}</td>".FormatWith("Name"))
                .Append(@"
        </tr>
    </tpl>
</table>");
            comboBox.Template.Html = tableTemplate.ToString();
            return comboBox;
        }

        /// <summary>
        /// Создает выпадающий список - фильтр для таблицы
        /// </summary>
        /// <param name="storeId">Идентификатор Store для выпадающего списка</param>
        /// <param name="comboId">Идентификатор выпадающего списка</param>
        /// <param name="label">Текст label-а</param>
        /// <param name="width">Ширина списка</param>
        /// <returns>Выпадающий список - фильтр</returns>
        private static ComboBox CreateFilter(string storeId, string comboId, string label, int width)
        {
            var comboBox = new ComboBox
            {
                EmptyText = @"Выберите значение",
                StoreID = storeId,
                ID = comboId,
                Editable = false,
                FieldLabel = label,
                TriggerAction = TriggerAction.All,
                ValueField = "ID",
                Width = width,
                Disabled = false,
                DisplayField = "Name"
            };

            return comboBox;
        }
    }
}
