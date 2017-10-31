/// <reference path="/Content/js/ext-base-debug.js" />
/// <reference path="/Content/js/ext-all-debug.js" />
/// <reference path="/Content/js/App.js" />
/// <reference path="/Content/Consolidation/Views/ConsReport/ReportSheetPanel.js" />
Ext.ns('App.Cons');

App.Cons.ReportView = function (config) {
    // private variables
    var formLayout = {};
    var reportTabPanel = new Ext.TabPanel();
    var viewport;

    // private functions

    // Устанавливает флаг необходимости перегрузки данных для всех неактивных вкладок
    var setNeedRefresh = function () {
        for (var i = 0; i < reportTabPanel.items.length; i++) {
            var p = reportTabPanel.items.items[i];
            p.needRefresh = true;
        }
    };

    var refresh = function () {
        setNeedRefresh();
        var activeTab = reportTabPanel.getActiveTab();
        activeTab.refresh();
        activeTab.needRefresh = false;
    };

    var save = function () {
        reportTabPanel.items.each(function (p) {
            p.save();
        });
    };

    var calc = function () {
        Ext.net.DirectEvent.confirmRequest({
            cleanRequest: true,
            url: "/ConsRelations/CalcReport",
            timeout: 180000,
            formProxyArg: "PageForm",
            extraParams: {
                 "reportId": config.reportId,
                 fxProgressConfig: { message: 'Выполняется расчет...' }
            },
            before: function (el, type, action, extraParams, o) {
                save();
            },
            userSuccess: function (response, result, el, type, action, extraParams, o) {
                refresh();
            },
            control: this
        });
    };

    var report = function () {
        Ext.net.DirectEvent.confirmRequest({
            cleanRequest: true,
            isUpload: true,
            url: "/ConsReport/Report",
            formProxyArg: "PageForm",
            extraParams: { "reportId": config.reportId }
        });
    };

    var onTabChange = function (tp, p) {
        if (p.needRefresh) {
            p.refresh();
            p.needRefresh = false;
        }
    };

    var createReportLayout = function (formConfig) {
        formLayout = formConfig;
        for (var i = 0; i < formLayout.Mapping.Sheets.length; i++) {
            var sheetMap = formLayout.Mapping.Sheets[i];
            var markup = formLayout.FormLayout[i];

            var sheetPanel = new App.Cons.ReportSheetPanel({
                title: sheetMap.Name,
                autoScroll: true,
                border: false,
                layout: 'auto',
                reportId: config.reportId,
                form: formLayout.Form,
                sheetMapping: sheetMap,
                markup: markup
            });
            sheetPanel.createSheetLayout();
            sheetPanel.setNeedRefresh();

            reportTabPanel.add(sheetPanel);
        }

        reportTabPanel.on('tabchange', onTabChange);
        reportTabPanel.activate(reportTabPanel.items.items[0]);
        viewport.getEl().unmask();
    };

    // public space
    return {
        // public properties

        // public methods
        init: function () {
            var refreshAction = new Ext.Action({
                text: 'Обновить',
                handler: function () {
                    refresh();
                },
                iconCls: 'icon-pagerefresh',
                itemId: 'refreshAction'
            });

            var saveAction = new Ext.Action({
                text: 'Сохранить',
                handler: function () {
                    save();
                },
                iconCls: 'icon-pagesave',
                itemId: 'saveAction'
            });

            var сalcAction = new Ext.Action({
                text: 'Вычислить соотношения',
                handler: function () {
                    calc();
                },
                iconCls: 'icon-calculator',
                itemId: 'сalcAction'
            });

            var reportAction = new Ext.Action({
                text: 'Отчет',
                handler: function () {
                    report();
                },
                iconCls: 'icon-report',
                itemId: 'reportAction'
            });

            // Запращиваем метаданные формы отчета
            Ext.net.DirectEvent.confirmRequest({
                cleanRequest: true,
                url: '/ConsReport/GetReportMetadata',
                extraParams: { reportId: config.reportId },
                userSuccess: function (response) {
                    createReportLayout(Ext.decode(response.responseText));
                },
                userFailure: function () {
                    Ext.Msg.alert('Ошибка', 'Не удалость загрузить форму.');
                },
                scope: this
            });

            viewport = new Ext.Viewport({
                layout: 'border',
                hideBorders: true,
                items: [
                    {
                        region: 'north',
                        border: false,
                        autoHeight: true,
                        tbar: [refreshAction, saveAction, сalcAction, reportAction]
                    },
                    {
                        region: 'center',
                        border: false,
                        layout: "fit",
                        items: [
                            new Ext.TabPanel({
                                activeTab: 0,
                                border: false,
                                enableTabScroll: true
                            })
                        ]
                    }
                ]
            });

            viewport.getEl().mask('Загрузка отчета...');

            reportTabPanel = viewport.layout.center.items[0];
        }
    };
};
