using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Ext.Net;
using Krista.FM.Extensions;
using Krista.FM.RIA.Core;
using Krista.FM.RIA.Core.Gui;
using Krista.FM.RIA.Core.ViewModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.RIA.Extensions.DebtBook
{
    public class BebtBookFormChangesGridControl : Control
    {
        private readonly IPresentation presentation;
        private readonly bool isReadonly;
        private readonly Dictionary<string, ColumnState> fields;

        public BebtBookFormChangesGridControl(IPresentation presentation, bool isReadonly, Dictionary<string, ColumnState> fields)
        {
            this.presentation = presentation;
            this.isReadonly = isReadonly;
            this.fields = fields;
        }

        public override List<Component> Build(ViewPage page)
        {
            return new List<Component> { CreateDetailGrid() };
        }

        private GridPanel CreateDetailGrid()
        {
            //--------------------------------------------------------------
            // Грид детализации
            GridPanel gp = new GridPanel();
            gp.ID = "gpChilds";
            gp.StoreID = "dsChilds";
            gp.Border = true;
            gp.AutoScroll = true;
            gp.Height = 230;
            gp.Hidden = true;
            gp.StyleSpec = "margin-top: 5px; margin-bottom: 5px;";

            gp.View.Add(new Ext.Net.GridView { ForceFit = false });
            var groupRow = new HeaderGroupRow();

            foreach (IDataAttribute attribute in presentation.Attributes.Values
                .Where(x => x.GroupTags != null && x.GroupTags.StartsWith("Изменение;"))
                .OrderBy(x => x.GroupTags))
            {
                // Если группа для колонки не создана, то создаем новую группу
                if (!groupRow.Columns.Any(x => x.Header == attribute.GroupTags.Split(new[] { ';' }, 2)[1]))
                {
                    groupRow.Columns.Add(new HeaderGroupColumn
                    {
                        Header = attribute.GroupTags.Split(new[] { ';' }, 2)[1],
                        Align = Alignment.Center,
                        ColSpan = presentation.Attributes.Values
                            .Count(x => x.GroupTags == attribute.GroupTags)
                    });
                }

                var additionalCalcFields = new List<string> { "REMNSBGNMNTHDBT", "REMNSBGNMNTHINTEREST", "REMNSBGNMNTHPENLT" };
                additionalCalcFields.AddRange(fields.Keys);

                // Добавляем столбец в грид
                // Поле CHARGEDATE в форме является необязательным, 
                // а в гриде изменений оно должно быть обязательным для заполнения
                ColumnBase column = attribute.Name.ToUpper() == "CHARGEDATE"
                    ? gp.ColumnModel.AddColumn(attribute.Name, attribute.Caption, attribute.Type, Mandatory.NotNull)
                    : additionalCalcFields.Contains(attribute.Name.ToUpper())
                        ? gp.ColumnModel.AddColumn(attribute.Name, attribute.Caption, DataAttributeTypes.dtString, Mandatory.NotNull)
                        : gp.ColumnModel.AddColumn(attribute);

                column.SetGroup(attribute.GroupTags.Split(new[] { ';' }, 2)[1]).SetWidth(80);

                // Вычисляемые поля выделяем цветом
                string attrName = attribute.Name.ToUpper();
                if (additionalCalcFields.Contains(attrName))
                {
                    column.Css = "disable-cell";
                    column.Renderer = new Renderer(@"
metadata.css = 'disable-cell'; 
var f = Ext.util.Format.numberRenderer('0.000,00/i');
return f(value);");
                }

                if (!isReadonly && !attribute.IsReadOnly)
                {
                    if (fields.ContainsKey(attribute.Name.ToUpper()))
                    {
                        // Вычисляемые поля делаем нередактируемими и выделяем цветом
                        if (fields[attribute.Name.ToUpper()].CalcFormula.IsNullOrEmpty())
                        {
                            column.SetEditable(attribute);
                        }
                    }
                    else
                    {
                        column.SetEditable(attribute);
                    }
                }
            }

            gp.View[0].HeaderGroupRows.Add(groupRow);
            gp.SelectionModel.Add(new RowSelectionModel());

            // Фирмируем список вычисляемых полей
            StringBuilder afterEditHandler = GetGridColumnsCalculations();
            gp.Listeners.AfterEdit.Handler = afterEditHandler.ToString();

            //--------------------------------------------------------------
            // Тулбар для грида
            var tb = new PagingToolbar { PageSize = 15, BeforePageText = "Страница изменений" };

            // Команда добавления изменения
            tb.Items.Add(new Button
            {
                Icon = Icon.Add,
                Handler =
@"function(){{
    var data = dsEntity.getAt(0).copy().data;
    var record = new gpChilds.store.recordType(data);
    #{{{0}}}.stopEditing();
    #{{{0}}}.store.insert(#{{{0}}}.store.getCount(), record);
	Ext.apply(record, {{newRecord: true}});
    record.set('PARENTID', data.ID);
    record.set('ID', '');
    record.set('CHARGEDATE', '');
    record.set('ISBLOCKED', false);
    try {{
        record = Extension.DebtBookShowDetailsView.initNewDetailsRow(record, dsEntity, gpChilds);
    }} catch (e) {{
        console.warn('Ошибка инициализации новой записи: ', e);
    }}
    #{{{0}}}.getView().focusRow(#{{{0}}}.store.getCount() - 1);
    
    // applyPageSize
    var headerHeight = #{{{0}}}.el.child('.x-grid3-header').getHeight(); //header height;
    var rowHeight = Ext.fly(#{{{0}}}.getView().getRow(0)).getHeight(); //row height;
    #{{{0}}}.setHeight(48 + headerHeight + #{{{0}}}.store.data.length * rowHeight);
    
    #{{{0}}}.startEditing(#{{{0}}}.store.getCount() - 1, 0);
}}".FormatWith(gp.ID),
                ToolTip = "Добавить изменение",
                Disabled = isReadonly
            });

            // Команда удаления изменения
            tb.Items.Add(new Button
            {
                ID = "childsDeleteBtn",
                Icon = Icon.Delete,
                Handler =
                    @"function(){{
Ext.Msg.confirm('Предупреждение','Удалить выделенные изменения?', function (btn) {{
    if(btn == 'yes') {{
        var sel = #{{{0}}}.getSelectionModel().getSelections();
        for(var i = 0; i < sel.length; i++){{
            #{{{1}}}.remove(sel[i]);
        }}
        #{{{1}}}.save();
        #{{{0}}}.setHeight(48 + #{{{0}}}.getView().mainHd.getHeight() + #{{{0}}}.store.data.length * 21);
    }}
}});    
}}".FormatWith(gp.ID, gp.StoreID),
                ToolTip = "Удалить выделенные изменения",
                Disabled = isReadonly
            });

            gp.TopBar.Add(tb);

            gp.Listeners.RowClick.AddAfter(@"
var lockDelete = false;
for(var i = 0; i < item.getSelectionModel().getSelections().length; i++){
    lockDelete = item.getSelectionModel().getSelections()[i].get('ISBLOCKED');
}
childsDeleteBtn.setDisabled(lockDelete && !Unlock.value);");
            gp.Listeners.BeforeEdit.AddAfter("return !e.record.get('ISBLOCKED') || Unlock.value;");

            // Обработчик инициализации новой строчки грида
            StringBuilder strbCalcs = GetGridColumnsCalculationsScriptBody("rec", "record");
            ResourceManager.GetInstance(HttpContext.Current).RegisterOnReadyScript(
@"Extension.DebtBookShowDetailsView.initNewDetailsRow = function(record, masterStore, detailsGrid){{
    // Если создаем первую запись
    if (detailsGrid.getStore().getCount() == 1){{
        var data = masterStore.getAt(0).data;
        if (data.BGNYEARDBT == undefined){{record.set('REMNSBGNMNTHDBT', data.BGNYEARDEBT); }} else {{record.set('REMNSBGNMNTHDBT', data.BGNYEARDBT); }}
        record.set('REMNSBGNMNTHINTEREST', data.BGNYEARINTEREST); 
        record.set('REMNSBGNMNTHPENLT', data.BGNYEARPENLT); 
    }}else{{
        var rec;
        // Ищем последнее изменение
        for (var i = 0; i < detailsGrid.getStore().getCount(); i++){{
            if (detailsGrid.getStore().getAt(i).get('CHARGEDATE') !== ''){{
                rec = detailsGrid.getStore().getAt(i);
            }}
        }}
        if (rec !== undefined){{
            record.set('REMNSBGNMNTHDBT', rec.data.REMNSENDMNTHDBT); 
            record.set('REMNSBGNMNTHINTEREST', rec.data.REMNSENDMNTHINTEREST); 
            record.set('REMNSBGNMNTHPENLT', rec.data.REMNSBGNMNTHPENLT); 
        }}
        {0}
    }}
    return record;
}};".FormatWith(strbCalcs.ToString()));

            return gp;
        }

        /// <summary>
        /// Формирует JS-скрипт для вычисляемых полей в гриде.
        /// </summary>
        /// <returns>JS-скрипт .</returns>
        private StringBuilder GetGridColumnsCalculations()
        {
            StringBuilder strbCalcs = GetGridColumnsCalculationsScriptBody("r", "r");

            return new StringBuilder()
                .Append("var r = e.record;")
                .AppendLine()
                .Append(strbCalcs);
        }

        private StringBuilder GetGridColumnsCalculationsScriptBody(string sourceRecord, string destRecord)
        {
            // Получаем список вычисляемых полей
            Dictionary<string, string> calculations = new Dictionary<string, string>();
            foreach (var columnState in fields)
            {
                if (columnState.Value.CalcFormula.IsNotNullOrEmpty())
                {
                    calculations.Add(
                        columnState.Key.ToUpper(),
                        columnState.Value.CalcFormula.ToUpper());
                }
            }

            // Формируем скрипт для вычисляемых полей
            StringBuilder strbCalcs = new StringBuilder();
            foreach (var calculation in calculations)
            {
                // Пропускаем выражения с функцией PREV
                if (calculation.Value.Contains("PREV("))
                {
                    continue;    
                }

                string[] expParts = calculation.Value.Split(
                    new[] { '+', '-', '*', '/', '(', ')', ',' },
                    StringSplitOptions.RemoveEmptyEntries);

                StringBuilder formula = new StringBuilder(calculation.Value);
                foreach (string expPart in expParts)
                {
                    formula.Replace(expPart, "{0}.data.{1}".FormatWith(sourceRecord, expPart));
                }

                strbCalcs.Append(destRecord)
                    .Append(".set('")
                    .Append(calculation.Key)
                    .Append("', ")
                    .Append(formula)
                    .Append(");")
                    .AppendLine();
            }

            return strbCalcs;
        }
    }
}
