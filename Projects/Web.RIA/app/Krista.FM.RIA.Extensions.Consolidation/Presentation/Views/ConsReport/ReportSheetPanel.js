/// <reference path="/Content/js/ext-base-debug.js" />
/// <reference path="/Content/js/ext-all-debug.js" />
/// <reference path="/Content/js/App.js" />
Ext.ns('App.Cons');

App.Cons.ReportSheetPanel = Ext.extend(Ext.Panel, {
    /**
    * @cfg {Number} reportId Id отчета.
    */

    /**
    * @cfg {Object} form Структура формы.
    */

    /**
    * @cfg {Object} sheetMapping Маппинг структуры и разметки листа.
    */

    /**
    * @cfg {Object} markup Разметка листа.
    */

    // public
    needRefresh: false,

    // private
    stores: [],

    constructor: function (config) {
        this.autoScroll = true;
        App.Cons.ReportSheetPanel.superclass.constructor.call(this, config);

        this.stores = [];
        this.needRefresh = false;
    },

    setNeedRefresh: function () {
        this.needRefresh = true;
    },

    refresh: function () {
        this.items.each(function (p) {
            p.refresh();
        });
    },
    
    save: function () {
        this.items.each(function (p) {
            p.save();
        });
    },

    createSheetLayout: function () {
        // Реквизиты
        if (this.sheetMapping.HeaderRequisites) {
            var headReq = [];
            Ext.each(this.form.Requisites, function (r) { if (r.IsHeader) headReq.push(r); });
            var requsitesPanel = new App.Cons.ReportRequisitesGrid({
                title: 'Заголовочные реквизиты формы',
                kind: 'header',
                formId: 'HR' + this.form.InternalName,
                formSectionCode: '',
                requsites: headReq,
                markup: this.markup.HeaderReq,
                reportId: this.reportId
            });
            this.add(requsitesPanel);
        }

        // Разделы
        for (var i = 0; i < this.sheetMapping.Sections.length; i++) {
            var sectionMap = this.sheetMapping.Sections[i], formSection;
            Ext.each(this.form.Parts, function (p) {
                if (p.InternalName == sectionMap.Code) {
                    formSection = p;
                    return false;
                }
                return true;
            });

            // Реквизиты
            if (sectionMap.HeaderRequisites) {
                var headSecReq = [];
                Ext.each(formSection.Requisites, function (r) { if (r.IsHeader) headSecReq.push(r); });
                var requsitesSecPanel = new App.Cons.ReportRequisitesGrid({
                    title: 'Заголовочные реквизиты раздела',
                    type: 'section',
                    kind: 'header',
                    formId: 'HR' + this.form.InternalName + formSection.InternalName,
                    formSectionCode: formSection.Code,
                    requsites: headSecReq,
                    markup: this.markup.Sections[i].HeaderReq,
                    reportId: this.reportId
                });
                this.add(requsitesSecPanel);
            }

            // Таблица
            var sectionTable = new App.Cons.ReportSectionGrid(this, {
                title: 'Раздел ' + formSection.Name,
                formSection: formSection,
                mapping: sectionMap.Table,
                markup: this.markup.Sections[i].Section,
                reportId: this.reportId
            });
            this.add(sectionTable);

            // Реквизиты
            if (sectionMap.FooterRequisites) {
                var footSecReq = [];
                Ext.each(formSection.Requisites, function (r) { if (!r.IsHeader) footSecReq.push(r); });
                var footRequsitesSecPanel = new App.Cons.ReportRequisitesGrid({
                    title: 'Заголовочные реквизиты раздела',
                    type: 'section',
                    kind: 'footer',
                    formId: 'FR' + this.form.InternalName + formSection.InternalName,
                    formSectionCode: formSection.Code,
                    requsites: footSecReq,
                    markup: this.markup.Sections[i].FooterReq,
                    reportId: this.reportId
                });
                this.add(footRequsitesSecPanel);
            }
        }

        // Реквизиты
        if (this.sheetMapping.FooterRequisites) {
            var footReq = [];
            Ext.each(this.form.Requisites, function (r) { if (!r.IsHeader) footReq.push(r); });
            var footRequsitesPanel = new App.Cons.ReportRequisitesGrid({
                title: 'Заключительные реквизиты формы',
                kind: 'footer',
                formId: 'FR' + this.form.InternalName,
                formSectionCode: '',
                requsites: footReq,
                markup: this.markup.FooterReq,
                reportId: this.reportId
            });
            this.add(footRequsitesPanel);
        }
    },

    // private
    createSectionTable: function (formSection, markup) {
        var fields = [{ name: "ID" }, { name: "RefFormRow" }, { name: "MetaRowOrd"}];
        var columns = [];
        for (var i = 0; i < formSection.Columns.length; i++) {
            var c = formSection.Columns[i];
            fields.push({ name: c.InternalName });
            columns.push({
                id: c.InternalName,
                dataIndex: c.InternalName,
                header: c.Name
            });
        }

        var store = new Ext.net.Store({
            proxyId: 'store' + formSection.InternalName,
            proxy: new Ext.data.HttpProxy({ method: "GET", url: "/ConsReport/GetSectionData" }),
            updateProxy: new Ext.net.HttpWriteProxy({ url: "/ConsReport/SaveSectionData" }),
            reader: new Ext.data.JsonReader({
                fields: fields,
                idProperty: "ID",
                root: "data"
            }),
            sortInfo: { field: "MetaRowOrd", direction: "asc" },
            _serverParams: { params: { reportId: this.reportId, sectionCode: formSection.Code} }
        });

        this.stores.push(store);

        var grid = new Ext.grid.GridPanel({
            title: 'Раздел ' + formSection.Name,
            border: false,
            hideHeaders: true,
            collapsible: true,
            colModel: new Ext.grid.ColumnModel({ columns: columns }),
            store: store,
            view: new Ext.ux.ExcelGridView({ layoutMarkup: markup })
        });

        this.add(grid);
    }
});
