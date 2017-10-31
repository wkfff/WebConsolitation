Ext.ux.ExcelGridView = Ext.extend(Ext.grid.GridView, {

    whitespaceRowClass: 'ux-excelgrid-row-whitespace',
    fixedRowClass: 'ux-excelgrid-row-fixed',
    userRowClass: 'ux-excelgrid-row-user',

    /**
    * @cfg {String} cellSelector The selector used to find cells internally (defaults to <tt>'td.x-grid3-cell'</tt>)
    */
    cellSelector: 'td.ux-excelgrid-cell',

    /**
    * @cfg {String} rowSelector The selector used to find rows internally (defaults to <tt>'div.x-grid3-row'</tt>)
    */
    rowSelector: 'tr.ux-excelgrid-row',

    /**
    * @cfg {String} Селектор строк не являющихся строками таблицы с данными. 
    */
    whitespaceRowSelector: 'tr.ux-excelgrid-row-whitespace',

    /**
    * @cfg {String} Селектор фиксированных строк таблицы. 
    */
    fixedRowSelector: 'tr.ux-excelgrid-row-fixed',

    /**
    * @cfg {String} Селектор множимых строк таблицы. 
    */
    userRowSelector: 'tr.ux-excelgrid-row-user',

    /**
    * @cfg {RegExp} Регулярное выражение для поиска идентификаторо строки метаданных.
    * @type RegExp
    */
    metaRowClsRe: new RegExp('ux-excelgrid-metarow-([^\\s]+)', ''),

    rowsTemplate: new Ext.XTemplate(
        '<tr class="ux-excelgrid-row ux-excelgrid-metarow-{metaRowId} {cls}">' +
            '<tpl for="cells">' +
                '<td class="ux-excelgrid-cell x-grid3-td-{id} {css}" tabIndex="0" {cellAttr}>' +
                    '<div class="ux-grid-cell" unselectable="on" style="{style}">{value}</div>' +
                '</td>' +
            '</tpl>' +
        '</tr>'),

    constructor: function () {
        Ext.ux.ExcelGridView.superclass.constructor.apply(this, arguments);

        Ext.apply(this, {
            headerRows: '',
            footerRows: '',
            rowsTemplateData: {}
        });

        this.addEvents(
        /**
        * @event Выполнена команда добавления новой строки.
        * @param {Ext.grid.GridView} view
        * @param {Number} rowIndex The index of the row to be removed.
        * @param {Ext.data.Record} record The Record to be removed
        */
            'newrow',

        /**
        * @event Выполнена команда удаления строки.
        * @param {Ext.grid.GridView} view
        * @param {Number} firstRow The index of the first row to be inserted.
        * @param {Number} lastRow The index of the last row to be inserted.
        */
            'deleterow'
        );
    },

    /**
    * @private
    * Finds and stores references to important elements
    */
    initElements: function () {
        Ext.ux.ExcelGridView.superclass.initElements.apply(this, arguments);

        Ext.apply(this, {
            mainBody: new Ext.Element(Ext.Element.fly(this.scroller).child('div.x-table-layout-ct').child('table').child('tbody'))
        });
    },

    /**
    * Returns the grid's <tt>&lt;td></tt> HtmlElement at the specified coordinates.
    * Ext.Net переопределяет поведение метода getCell, так что нам тоже приходится перекрывать этот метод.
    * @param {Number} row The row index in which to find the cell.
    * @param {Number} col The column index of the cell.
    * @return {HtmlElement} The td at the specified coordinates.
    */
    getCell: function (row, col) {
        return Ext.fly(this.getRow(row)).query(this.cellSelector)[col];
    },

    /**
    * Возвращает индекс первой строки таблицы данных.
    * @return {Number}
    */
    getTableRowIndex: function (row) {
        return Ext.isNumber(row) ? row - this.layoutMarkup.FirstRow : row.rowIndex - this.layoutMarkup.FirstRow;
    },

    /**
    * @private Возвращает массив строк относящихся только к таблице данных, 
    * не влючая заголовок таблицы и ее нижнюю часть.
    * @return {Array}
    */
    getRows: function () {
        if (this.hasRows()) {
            var childs = this.mainBody.dom.childNodes,
                rows = [];

            for (var i = 0; i < childs.length; i++) {
                if (i >= this.layoutMarkup.FirstRow) {
                    var match = childs[i].className.match(/ux-excelgrid-row/g);
                    if (match) {
                        rows.push(childs[i]);
                    }
                }
            }

            return rows;
        }

        return [];
    },

    /**
    * @private Возвращает элемент строки из полной таблицы.
    * @param {Number} row Индекс запрашиваемой строки.
    * @return {HtmlElement} The tr element.
    */
    getNativeRow: function (row) {
        var rows = this.hasRows() ? this.mainBody.dom.childNodes : [];
        return rows[row];
    },

    /**
    * Return the index of the grid row which contains the passed HTMLElement.
    * See also {@link #findCellIndex}
    * @param {HTMLElement} el The target HTMLElement
    * @return {Number} The row index, or <b>false</b> if the target element is not within a row of this GridView.
    */
    findRowIndex: function (el) {
        var row = this.findRow(el);
        if (row) {
            var match = row.className.match(/ux-excelgrid-row-whitespace/g);
            if (match) {
                return false;
            }

            return this.getTableRowIndex(row);
        }

        return false;
    },

    /**
    * @private
    * Фозвращает фиксированные строки.
    * @return {Array}
    */
    getFixedRows: function () {
        return this.mainBody.query(this.fixedRowSelector);
    },

    /**
    * @private
    * Фозвращает множимые (пользовательские) строки.
    * @return {Array}
    */
    getUserRows: function () {
        return this.mainBody.query(this.userRowSelector);
    },

    /**
    * Возвращает строки соответствующие идентификатору метаданных metaId. 
    * @param {Number} metaId
    * @return {Array}
    */
    findRowsByMetaId: function (metaId) {
        return this.mainBody.query('tr.ux-excelgrid-metarow-' + metaId);
    },

    /**
    * @private Возвращает id метаданных строки.
    * @param {HtmlElement} row
    * @return {Number} 
    */
    getRowMetaId: function (row) {
        var match = row.className.match(this.metaRowClsRe);
        if (match && match[1]) {
            return parseInt(match[1], 10);
        }

        return null;
    },

    /**
    * @private
    * Наполняет данными строки грида. Добавляет в DOM пользовательские строки, если это необходимо. 
    * Renders all of the rows to a string buffer and returns the string. This is called internally
    * by renderRows and performs the actual string building for the rows - it does not inject HTML into the DOM.
    * @param {Array} columns The column data acquired from getColumnData.
    * @param {Array} records The array of records to render
    * @param {Ext.data.Store} store The store to render the rows from
    * @param {Number} startRow The index of the first row being rendered. Sometimes we only render a subset of
    * the rows so this is used to maintain logic for striping etc
    * @param {Number} colCount The total number of columns in the column model
    * @return {String} Возвращает пустую строку. A string containing the HTML for the rendered rows
    */
    doRender: function (columns, records, store, startRow, colCount) {
        var rowBuffer = [],
            len = records.length,
            record, i, j;

        //build up each row's HTML
        for (j = 0; j < len; j++) {
            record = records[j];
            var rowMetaId = record.get('RefFormRow');
            var templateData = this.cloneRow(this.rowsTemplateData[rowMetaId]);

            if (templateData) {
                for (i = 0; i < colCount; i++) {
                    var cell = templateData.cells[i];
                    if (cell) {
                        if (cell.id != "") {
                            var val = record.get(cell.id);
                            if (val != null) {
                                cell.value = val;
                            }
                        }
                    }
                }

                rowBuffer[rowBuffer.length] = this.rowsTemplate.apply(templateData);
            }
        }

        return rowBuffer.join('');
    },

    processRows: function () {
        Ext.ux.ExcelGridView.superclass.processRows.apply(this, arguments);

        if (!this.ds || this.ds.getCount() < 1) {
            return;
        }

        var rows = this.getUserRows(),
            length = rows.length,
            row, i;

        for (i = 0; i < length; i++) {
            row = rows[i];
            if (row) {
                row.childNodes[0].childNodes[0].className = 'ux-excelgrid-rowaction-new';
                row.childNodes[row.childNodes.length - 1].childNodes[0].className = 'ux-excelgrid-rowaction-delete';
            }
        }
    },

    /**
    * @private
    */
    afterRender: function () {
        if (!this.ds || !this.cm) {
            return;
        }

        //this.replace_html(this.mainBody.dom, this.renderBody() || '&#160;');
        //this.mainBody.dom.innerHTML = this.renderBody() || '&#160;';
        this.processRows(0, true);

        if (this.deferEmptyText !== true) {
            this.applyEmptyText();
        }

        this.grid.fireEvent('viewready', this.grid);
    },

    processEvent: function (name, e) {
        Ext.ux.ExcelGridView.superclass.processEvent.apply(this, arguments);

        var target = e.getTarget();

        if (name == 'mousedown') {
            var record;
            if (target.className == 'ux-excelgrid-rowaction-new') {
                var x = target.getClientRects()[0].left,
                    y = target.getClientRects()[0].top;

                // Событие должно срабатывать только при нажатии на иконку
                if (e.xy[0] < x + 13 && e.xy[1] < y + 13) {
                    var insertIndex = this.getTableRowIndex(target.parentNode.parentNode.rowIndex);
                    record = this.ds.getAt(insertIndex);

                    this.fireEvent('newrow', record, insertIndex + 1);
                }
            }

            if (target.className == 'ux-excelgrid-rowaction-delete') {
                var x = target.getClientRects()[0].left,
                    y = target.getClientRects()[0].top;

                // Событие должно срабатывать только при нажатии на иконку
                if (e.xy[0] < x + 13 && e.xy[1] < y + 13) {
                    var rowIndex = this.getTableRowIndex(target.parentNode.parentNode.rowIndex);
                    record = this.ds.getAt(rowIndex);

                    this.fireEvent('deleterow', record, rowIndex);
                }
            }
        }
    },

    // private
    renderBody: function () {
        return this.headerRows + this.renderRows() + this.footerRows;
    },

    /**
    * @private
    * Обновляет данные в строке.
    */
    refreshRowData: function (record, row) {
        if (!row) {
            var rows = this.findRowsByMetaId(record.get('RefFormRow'));
            if (rows.length == 1) {
                row = rows[0];
            } else {
                var rowIndex = this.ds.indexOf(record);
                row = this.getRows()[rowIndex];
            }
        }

        for (var c = 0; c < row.children.length; c++) {
            var cellEl = row.children[c];

            var match = cellEl.className.match(new RegExp('x-grid3-td-([^\\s]+)', ''));
            if (match && match[1]) {
                var cellValue = record.get(match[1]);
                var innerCellEl = cellEl.children[0];
                if (innerCellEl) {
                    innerCellEl.innerHTML = cellValue;
                }
            }
        }

        return this.getTableRowIndex(row.rowIndex);
    },

    /**
    * @private
    * Refreshes a row by re-rendering it. Fires the rowupdated event when done
    */
    refreshRow: function (record, row) {
        var rowIndex = this.refreshRowData(record, row);
        this.fireEvent('rowupdated', this, rowIndex, record);
    },

    /**
    * Refreshs the grid UI
    * Переопределяем для того, чтобы в IE обойти глюк с innerHTML внутри tbody
    * @param {Boolean} headersToo (optional) True to also refresh the headers
    */
    refresh: function (headersToo) {
        this.fireEvent('beforerefresh', this);
        this.grid.stopEditing(true);

        var result = this.renderBody();

        this.mainBody.update(result).setWidth(this.getTotalWidth());
        //this.replace_html(this.mainBody, result);

        if (headersToo === true) {
            this.updateHeaders();
            this.updateHeaderSortState();
        }
        this.processRows(0, true);
        this.layout();
        this.applyEmptyText();
        this.fireEvent('refresh', this);
    },

    // private
    render: function () {
        if (this.autoFill) {
            var ct = this.grid.ownerCt;

            if (ct && ct.getLayout()) {
                ct.on('afterlayout', function () {
                    this.fitColumns(true, true);
                }, this, { single: true });
            }
        } else if (this.forceFit) {
            this.fitColumns(true, false);
        } else if (this.grid.autoExpandColumn) {
            this.autoExpand(true);
        }

        this.grid.getGridEl().dom.innerHTML = this.renderUI();

        this.renderExcelTableUI();

        this.afterRenderUI();

        this.processMetaRows();

        this.headerRows = this.initHeader();

        this.footerRows = this.initFooter();

        this.initRowsTemplates();
    },

    // private
    onCellSelect: function (row, col) {
        var cell = this.getCell(row, col);
        if (cell) {
            var tdCell = this.fly(cell);
            if (!tdCell.child('div.ux-excelgrig-focus')) {
                tdCell.createChild({
                    tag: 'div',
                    cls: 'ux-excelgrig-focus',
                    style: "width: " + (tdCell.getWidth() - 3) + "px; height: " + (tdCell.getHeight() - 3) + "px; margin-top: -" + (tdCell.getHeight() + 1) + "px; display: block;"
                });
            }
        }
    },

    // private
    onCellDeselect: function (row, col) {
        var cell = this.getCell(row, col);
        if (cell) {
            this.fly(cell).child('div.ux-excelgrig-focus').remove();
        }
    },

    /* ------------------------------ krista --------------------------------- */

    /** 
    * @private
    * Добавляет к строкам мета-информацию.
    */
    processMetaRows: function () {
        var rows = this.layoutMarkup.Rows || [];
        for (var p in rows) {
            var row = new Ext.Element(this.getNativeRow(p)),
                meta = rows[p] || [];
            for (var i = 0; i < meta.length; i++) {
                row.addClass(meta[i]);
            }
        }
    },

    /** 
    * @private
    * Формирует тело таблицы на основе разметки this.layoutMarkup.
    */
    renderExcelTableUI: function () {
        this.buildStyles(this.layoutMarkup.Styles, this.layoutMarkup.StylesInnerCell);

        var items = [];

        for (var i = 0; i < this.layoutMarkup.Cells.length; i++) {
            var cell = this.layoutMarkup.Cells[i];

            var cellCls = 'ts' + cell.style + ' ux-excelgrid-cell' +
                (cell.colId ? ' x-grid3-td-' + cell.colId : '') +
                (cell.width === 0 || cell.height === 0 ? ' hidden-cell' : '') +
                (cell.required ? ' required-cell' : '') +
                (cell.isAutoBlock ? ' autoblock-cell' : '') +
                (cell.readonly ? ' readonly-cell' : '') +
                (cell.editor !== null ? ' editor-cell' : '');

            var item = Ext.apply(this.createItem(cell), {
                code: cell.code,
                required: cell.required,
                screenCode: cell.screenCode,
                colspan: cell.colspan,
                rowspan: cell.rowspan,
                cellCls: cellCls,
                visible: cell.visible
            });

            var element = Ext.ComponentMgr.create(item);

            items.push(element);
        }

        var tableContainer = new Ext.Container({
            baseCls: 'x-grid3-body',
            style: { margin: '10px' },
            height: this.layoutMarkup.Height + 'px',
            layout: {
                type: 'table',
                columns: this.layoutMarkup.TotalColumns,
                tableAttrs: {
                    style: {
                        'border-collapse': 'collapse'
                    }
                },
                trAttrs: { 'class': 'ux-excelgrid-row' }
            },
            items: items
        });

        var el = Ext.get(this.grid.getGridEl().dom.firstChild),
            mainWrap = new Ext.Element(el.child('div.x-grid3-viewport')),
            scroller = new Ext.Element(mainWrap.child('div.x-grid3-scroller'));

        tableContainer.render(scroller, 0);

        if (Ext.isGecko) {
            tableContainer.getEl().dom.tabIndex = 1;
        }
    },

    initHeader: function () {
        var rows = this.mainBody.dom.childNodes || [],
            headerRows = [];

        for (var i = 0; i < rows.length; i++) {
            if (rows[i].className.indexOf(this.whitespaceRowClass) >= 0) {
                headerRows.push(this.htmlElementToString(rows[i]));
            }
            else {
                break;
            }
        }

        return headerRows.join("");
    },

    initFooter: function () {
        var rows = this.mainBody.dom.childNodes || [],
            headerRows = [];

        for (var i = rows.length - 1; i >= 0; i--) {
            if (rows[i].className.indexOf(this.whitespaceRowClass) >= 0) {
                headerRows.push(this.htmlElementToString(rows[i]));
            }
            else {
                break;
            }
        }

        headerRows.reverse();

        return headerRows.join("");
    },

    initRowsTemplates: function () {
        var rows = this.getRows();

        for (var i = 0; i < rows.length; i++) {
            var row = rows[i],
                rowMetaId = this.getRowMetaId(row),
                templateParams = {};

            templateParams.metaRowId = rowMetaId;
            var match = row.className.match(new RegExp('ux-excelgrid-row-(fixed|user)', ''));
            if (match && match[0]) {
                templateParams.cls = match[0];
            }

            templateParams.cells = [];

            for (var j = 0; j < row.childNodes.length; j++) {
                var cell = row.childNodes[j],
                    cellMeta = {};

                match = cell.className.match(new RegExp('x-grid3-td-([^\\s]+)', ''));
                cellMeta.id = match && match[1] ? match[1] : "";

                match = cell.className.match(new RegExp('ts([^\\s]+)', ''));
                if (match && match[0]) {
                    cellMeta.css = match[0];
                }

                cellMeta.cellAttr = String.format('colspan="{0}" rowspan="{1}"', cell.colSpan, cell.rowSpan);
                cellMeta.style = cell.children[0].style.cssText;

                templateParams.cells.push(cellMeta);
            }

            this.rowsTemplateData[rowMetaId] = templateParams;
        }
    },

    buildStyles: function (styles, stylesInnerCell) {
        var jsonCss = "";
        for (var i in styles) {
            jsonCss += String.format(".ts{0} {1}", i, this.json2css(styles[i]));
        }
        for (var j in stylesInnerCell) {
            jsonCss += String.format(".ts{0} div {1}", j, this.json2css(stylesInnerCell[j]));
        }
        Ext.util.CSS.createStyleSheet(jsonCss, 'markupCss');
    },

    json2css: function (data) {
        var parts = [];

        for (var key in data) {
            var value = data[key];
            if (key == 'background-color' && value != '#FFFFFF' && value != 'White') {
                value = value + ' !important';
            }
            parts.push(key + ':' + value);
        }
        parts.push('overflow:hidden');
        var json = parts.join(";");
        return '{' + json + '}';
    },

    createItem: function (cell) {

        var w;
        if (cell.editor && cell.editor.xtype == 'RIA.Form.Editors.Image') {
            w = cell.width;
        }
        else {
            w = cell.width > 3 ? cell.width - 0 : 0; // для отступа по умолчанию
        }

        var h = cell.height;
        var cellStyle = String.format('width: {0}px; height:{1}px; max-width: {0}px; max-height:{1}px;', w, h);

        cell.visible = true;
        cell.editor = null;
        if (cell.visible) {
            cell.text = cell.text !== null ? /*RIA.Utils.*/this.trim(cell.text) : '';
        }

        var cellItem = {
            width: w,
            height: h,
            style: cellStyle,
            xtype: 'component',
            cls: (cell.visible ? 'ux-grid-cell' : '')
        };

        if (cell.visible) {

            if (cell.isAutoBlock) {
                return this.createAutoBlockCell(cellItem, cell);
            } else if (cell.text !== null && cell.text.substr(0, 1) == '@') {
                return this.createDynamicTableButtonCell(cellItem, cell);
            } else if (cell.readOnly) {
                return this.createReadOnlyCell(cellItem, cell);
            } else if (cell.editor !== null) {
                switch (cell.editor.xtype) {
                    case 'RIA.Form.Editors.Button':
                        return this.createButtonCell(cellItem, cell);
                    case 'RIA.Form.Editors.Image':
                        return this.createImageCell(cellItem, cell);
                    default:
                        return this.createEditorCell(cellItem, cell);
                }
            } else {
                return this.createDefaultCell(cellItem, cell);
            }
        }

        return cellItem;
    },

    // Обычная ячейка
    createDefaultCell: function (cellItem, cell) {
        return Ext.apply(cellItem, {
            html: this.formatItem(cell.value, cell.editor)
        });
    },

    // Изображение
    createImageCell: function (cellItem, cell) {
        var result = this.createEditorCell(cellItem, cell);
        result.cls = result.cls + ' image-static-cell';

        return result;
    },

    // Ячейка с редактором
    createEditorCell: function (cellItem, cell) {
        return Ext.apply(cellItem, {
            html: this.formatItem(cell.value, cell.editor) || '',
            freecell: cell.freecell,
            staticTable: this,
            style: cellItem.style,
            editorStyle: (cellItem.editorStyle || '') + 'border:0;',
            listeners: { afterrender: { fn: this.afterRenderHandler, scope: this} },
            editor: Ext.apply(cell.editor, {
                height: cellItem.height,
                width: cellItem.width + 3
            })
        });
    },

    // Ячейка нередактируемая. Белая. Красивая.
    createReadOnlyCell: function (cellItem, cell) {
        // контекстное меню в режиме только чтение
        if (cell.editor) {
            cellItem.listeners = cellItem.listeners || {};

            Ext.apply(cellItem.listeners,
                { 'afterrender': { fn: this.bindContextMenu, scope: this} }
            );
        }

        return Ext.apply(cellItem, {
            html: this.formatItem(cell.value, cell.editor),
            editor: cell.editor
        });
    },

    // Форматировать значение в ячейке
    //  value - новое значение
    //  editor - редактор ячейки
    formatItem: function (value, editor) {
        if (typeof (value) == 'number') {
            return Ext.util.Format.number(value, '0,00/i');
        }
        else if (Ext.isDate(value)) {
            var formattedValue = value;
            if (editor && editor.format) {
                formattedValue = Ext.util.Format.date(value, editor.format);
            }
            return formattedValue;
        }
        else if (editor) {
            return Ext.util.Format.nl2br(value);
        }
        else {
            return Ext.util.Format.nl2br(value);
        }
    },

    htmlElementToString: function (el) {
        var result = '<' + el.tagName;
        for (var i = 0; i < el.attributes.length; i++) {
            var attrib = el.attributes[i];
            result += ' ' + attrib.name + '="' + attrib.value + '"';
        }
        result += '>';

        result += el.innerHTML;

        result += '</' + el.tagName + '>';

        return result;
    },

    // трим для строк
    trim: function (string) {
        if (!string) {
            return null;
        }

        return string.trim();
    },

    // Для замены el.dom.innerHTML = 'some data...' в IE
    replace_html: function (el, html) {
        if (el) {
            var oldEl = (typeof el === "string" ? document.getElementById(el) : el);
            var newEl = document.createElement(oldEl.nodeName);

            // Preserve any properties we care about (id and class in this example)
            newEl.id = oldEl.id;
            newEl.className = oldEl.className;

            //set the new HTML and insert back into the DOM
            newEl.innerHTML = html;
            if (oldEl.parentNode)
                oldEl.parentNode.replaceChild(newEl, oldEl);
            else
                oldEl.innerHTML = html;

            //return a reference to the new element in case we need it
            return newEl;
        }

        return null;
    },

    /**
    * Clone Function. Перенести в общий модуль утилит.
    * @param {Object/Array} o Object or array to clone
    * @return {Object/Array} Deep clone of an object or an array
    * @author Ing. Jozef Sakáloš
    */
    cloneRow: function (o) {
        if (!o || 'object' !== typeof o) {
            return o;
        }
        if ('function' === typeof o.clone) {
            return o.clone();
        }
        var c = '[object Array]' === Object.prototype.toString.call(o) ? [] : {};
        var p, v;
        for (p in o) {
            if (o.hasOwnProperty(p)) {
                v = o[p];
                if (v && 'object' === typeof v) {
                    c[p] = this.cloneRow(v);
                }
                else {
                    c[p] = v;
                }
            }
        }
        return c;
    }
});

Ext.reg('Ext.ux.ExcelGridView', Ext.ux.ExcelGridView);