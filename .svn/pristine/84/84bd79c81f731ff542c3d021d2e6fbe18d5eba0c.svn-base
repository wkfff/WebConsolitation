Ext.ns('E86n.View');
var getMasterID = function(gridName) {
    var grid = window.Ext.getCmp(gridName);
    if (grid.getSelectionModel().hasSelection()) {
        var row = grid.getSelectionModel().getSelected();
        return row.data.ID;
    } else return -1;
};

function reloadDetail() {
    var activeTab = window.DetailTabPanel.getActiveTab();

    if (activeTab.id == 'ConsumerCategory') {
        window.ConsumerCategoryStore.reload();
    }

    if (activeTab.id == 'IndicatorsOfService') {
        window.IndicatorsOfServiceStore.reload();
    }

    if (activeTab.id == 'RequisitesNpa') {
        window.RequisitesNpaStore.reload();
    }

    if (activeTab.id == 'NpaRegulatesService') {
        window.NpaRegulatesServiceStore.reload();
    }

    if (activeTab.id == 'LimitValuesOfPrices') {
        window.LimitValuesOfPricesStore.reload();
    }

    if (activeTab.id == 'InformConsumers') {
        window.InformConsumersStore.reload();
    }
};

E86n.View.StateTask =
    {
        //требуется ли вызывать контроллер
        update: true,

        RowSelect: function (record) {
            if (record.getSelected().dirty) {
                //Запись изменена и не сохранена!
                window.DetailTabPanel.setDisabled(true);
            } else {
                //Запись в порядке, открываем детализацию!
                
                //Динамически разыменовываем колонку
                var col = window.Ext.getCmp('IndicatorsOfService').getColumnModel().getColumnById('Info');
                var col1 = window.Ext.getCmp('IndicatorsOfService').getColumnModel().getColumnById('RefIndicatorsName');
                if (record.getSelected().data.RefVedPchTipID == 1)
                  {
                    col.header = '<i>Формула расчета</i>';
                    col1.header = 'Показатели объема и качества услуг/работ';
                    window.NpaRegulatesService.setDisabled(false);
                    window.InformConsumers.setDisabled(false);
                    if (record.getSelected().data.RefVedPchCostID != 1)
                        window.LimitValuesOfPrices.setDisabled(false);
                    else window.LimitValuesOfPrices.setDisabled(true);
                   }
                else
                   {
                    col.header = '<i>Содержание работы</i>';
                    col1.header = 'Наименование работ';
                    window.NpaRegulatesService.setDisabled(true);
                    window.LimitValuesOfPrices.setDisabled(true);
                    window.InformConsumers.setDisabled(true);
                   }

                if (record.getSelected().data.RefVedPchCostID != 1)
                    window.RequisitesNpa.setDisabled(false);
                else window.RequisitesNpa.setDisabled(true);

                window.DetailTabPanel.setDisabled(false);
                reloadDetail();
            }
            window.StateTaskRemoveRecordBtn.setDisabled(false);
        },

        getSelectedServiceId: function () {
            var grid = window.Ext.getCmp("StateTask");
            if (grid.getSelectionModel().hasSelection()) {
                return grid.getSelectionModel().getSelected().data.RefVedPch;
            } else return -1;
        },

        updateYearHeader: function (recordHeader) {
            var valuesHeader = window.Ext.getCmp('IndicatorsOfService').getColumnModel();
            var year = recordHeader.data.RefYearFormID;
            valuesHeader.getColumnById('ReportingYear').header = '<i>Отчетный год<p/>' + (year - 2) + 'г.</i>';
            valuesHeader.getColumnById('CurrentYear').header = '<i>Текущий год<p/>' + (year - 1) + 'г.</i>';
            valuesHeader.getColumnById('ComingYear').header = '<i>Очередной год<p/>' + year + 'г.';
            valuesHeader.getColumnById('FirstPlanYear').header = '<i>Первый плановый год<p/>' + (year + 1) + 'г.</i>';
            valuesHeader.getColumnById('SecondPlanYear').header = '<i>Второй плановый год<p/>' + (year + 2) + 'г.</i>';
        },

        SetReadOnlyStateTask: function (readOnly, recId) {
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'StateTask');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'IndicatorsOfService');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'RequisitesNpa');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'MonitoringExecution');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ReportingRequirements');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'GroundsForTermination');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'NpaRegulatesService');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'LimitValuesOfPrices');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'InformConsumers');

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        },

        DisableNotBringCheckBox: function (disable) {
            var cmp = window.Ext.getCmp('StateTaskExtAttrCbNotBring');
            if (cmp) {
                if (disable) {
                    cmp.disable();
                } else {
                    cmp.enable();
                }
            }
        },

        CloseStateTaskDoc: function (readOnly, recId) {
            window.E86n.View.StateTask.SetReadOnlyStateTask(readOnly, recId);
            window.E86n.View.StateTask.DisableNotBringCheckBox(readOnly);
        },

        DataChanged: function (checkBox, store) {
            window.E86n.View.StateTask.update = false;
            var cb = Ext.getCmp(checkBox);
            var record = store.getAt(0);
            if (record) {
                cb.setValue(record.get('NotBring'));
            } else {
                cb.setValue(false);
            }
            window.E86n.View.StateTask.update = true;
        },

        Check: function (item, checked, url, docId) {
            if (window.E86n.View.StateTask.update) {
                if (checked) {
                    Ext.Msg.show({
                        title: 'Подтверждение',
                        msg: 'Данное действие удалит все данные документа. Продолжить?',
                        width: 300,
                        buttons: Ext.Msg.YESNO,
                        fn: function (btn) {
                            if (btn == 'yes') {
                                Ext.Ajax.request({
                                    url: url + '&value=' + checked,
                                    success: function (response) {
                                        var jo = Ext.util.JSON.decode(response.responseText);
                                        if (jo.success) {
                                            Ext.net.Notification.show({
                                                iconCls: 'icon-information',
                                                html: 'Все действия выполнены',
                                                title: 'Выполнено',
                                                hideDelay: 2000
                                            });
                                            window.E86n.View.StateTask.update = false;
                                            window.E86n.View.StateTask.SetReadOnlyStateTask(true, docId);
                                            window.E86n.View.StateTask.update = true;
                                        } else {
                                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                                            window.E86n.View.StateTask.update = false;
                                            window.E86n.View.StateTask.SetReadOnlyStateTask(false, docId);
                                            item.setValue(false);
                                            window.E86n.View.StateTask.update = true;
                                        }
                                    },
                                    failure: function (response) {
                                        var jo = Ext.util.JSON.decode(response.responseText);
                                        Ext.Msg.alert('Ошибка выполнения', jo.message);
                                        window.E86n.View.StateTask.update = false;
                                        window.E86n.View.StateTask.SetReadOnlyStateTask(false, docId);
                                        item.setValue(false);
                                        window.E86n.View.StateTask.update = true;
                                    }
                                });
                            }
                            else {
                                window.E86n.View.StateTask.update = false;
                                window.E86n.View.StateTask.SetReadOnlyStateTask(false, docId);
                                item.setValue(false);
                                window.E86n.View.StateTask.update = true;
                            }
                        },
                        modal: true,
                        icon: Ext.Msg.QUESTION,
                        closable: false
                    });

                } else {
                    Ext.Ajax.request({
                        url: url + '&value=' + checked,
                        success: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            if (jo.success) {
                                Ext.net.Notification.show({
                                    iconCls: 'icon-information',
                                    html: 'Все действия выполнены',
                                    title: 'Выполнено',
                                    hideDelay: 2000
                                });
                                window.E86n.View.StateTask.update = false;
                                window.E86n.View.StateTask.SetReadOnlyStateTask(false, docId);
                                window.E86n.View.StateTask.update = true;
                            } else {
                                Ext.Msg.alert('Ошибка выполнения', jo.message);
                                window.E86n.View.StateTask.update = false;
                                window.E86n.View.StateTask.SetReadOnlyStateTask(true, docId);
                                item.setValue(true);
                                window.E86n.View.StateTask.update = true;
                            }
                        },
                        failure: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                            window.E86n.View.StateTask.update = false;
                            window.E86n.View.StateTask.SetReadOnlyStateTask(true, docId);
                            item.setValue(true);
                            window.E86n.View.StateTask.update = true;
                        }
                    });
                }
            }
        }
    };