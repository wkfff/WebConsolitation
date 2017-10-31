Ext.ns('E86n.View');
// todo вынести в общий функционал
var getMasterID = function (gridName) {
    var grid = window.Ext.getCmp(gridName);
    if (grid.getSelectionModel().hasSelection()) {
        var row = grid.getSelectionModel().getSelected();
        return row.data.ID;
    } else return -1;
};

E86n.View.StateTask2016 =
    {
        RowSelect: function (record) {
            if (record.dirty) {
                //Запись изменена и не сохранена!
                window.DetailTabPanel.setDisabled(true);
            } else {
                //Запись в порядке, открываем детализацию!
                
                var col1 = window.Ext.getCmp('QualityVolumeIndexes').getColumnModel().getColumnById('RefIndicatorsName');
                
                if (record.data.RefServiceTypeCode == 0) {
                    // Услуга
                    col1.header = 'Показатели объема и качества услуг/работ';

                    window.RenderEnactment.setDisabled(false);
                    window.InformingProcedure.setDisabled(false);
                }
                else {
                    // Работа
                    col1.header = 'Наименование работ';

                    window.RenderEnactment.setDisabled(true);
                    window.InformingProcedure.setDisabled(true);
                }

                if (record.data.RefServicePayCode == 2) {
                    // Бесплатная услуга/работа
                    window.PriceEnactment.setDisabled(true);
                    window.AveragePrice.setDisabled(true);
                } else {
                    // Платная услуга/работа
                    window.PriceEnactment.setDisabled(false);
                    window.AveragePrice.setDisabled(false);
                }
                
                window.DetailTabPanel.setDisabled(false);

                window.E86n.View.StateTask2016.reloadDetail(window.DetailTabPanel);
            }
        },

        getSelectedServiceId: function () {
            var grid = window.Ext.getCmp("StateTask2016Grid");
            if (grid.getSelectionModel().hasSelection()) {
                return grid.getSelectionModel().getSelected().data.RefService;
            } else return -1;
        },

        updateYearHeader: function (recordHeader) {
            var valuesHeader = window.Ext.getCmp('QualityVolumeIndexes').getColumnModel();
            var year = recordHeader.data.RefYearFormID;
            valuesHeader.getColumnById('ReportingYear').header = '<i>Отчетный год<p/>' + (year - 2) + 'г.</i>';
            valuesHeader.getColumnById('CurrentYear').header = '<i>Текущий год<p/>' + (year - 1) + 'г.</i>';
            valuesHeader.getColumnById('ComingYear').header = '<i>Очередной год<p/>' + year + 'г.';
            valuesHeader.getColumnById('FirstPlanYear').header = '<i>Первый плановый год<p/>' + (year + 1) + 'г.</i>';
            valuesHeader.getColumnById('SecondPlanYear').header = '<i>Второй плановый год<p/>' + (year + 2) + 'г.</i>';
        },

        SetReadOnlyStateTask: function (readOnly, recId) {
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'StateTask2016Grid');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'QualityVolumeIndexes');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PriceEnactment');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'SupervisionProcedure');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ReportRequirements');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'EarlyTermination');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'OtherInfo');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Reports');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'RenderEnactment');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'AveragePrice');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'InformingProcedure');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ConsumersCategory');

            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('AdditionalInformationForm', readOnly);

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
            
            var cmp = window.Ext.getCmp('OtherSourcesCheckbox');
            if (cmp) {
                if (readOnly) {
                    cmp.disable();
                } else {
                    cmp.enable();
                }

            }
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
            window.E86n.View.StateTask2016.SetReadOnlyStateTask(readOnly, recId);
            window.E86n.View.StateTask2016.DisableNotBringCheckBox(readOnly);
        },

        update: true,

        changeService: function (item, checked, url, docId) {
            if (!window.E86n.View.StateTask2016.update)
                return;
            Ext.Msg.show({
                title: 'Подтверждение',
                msg: 'Данное действие удалит все данные документа. Продолжить?',
                width: 300,
                buttons: Ext.Msg.YESNO,
                fn: function (btn) {
                    if (btn === 'yes') {
                        Ext.Ajax.request({
                            url: url + '&docId=' + docId,
                            success: function (response) {
                                var jo = Ext.util.JSON.decode(response.responseText);
                                if (jo.success) {
                                    Ext.net.Notification.show({
                                        iconCls: 'icon-information',
                                        html: 'Все действия выполнены',
                                        title: 'Выполнено',
                                        hideDelay: 2000
                                    });
                                    var grid = window.Ext.getCmp("StateTask2016Grid");

                                    // удаляем несохраненные строки
                                    grid.getStore().rejectChanges();
                                    grid.getSelectionModel().clearSelections();

                                    grid.reload();
                                } else {
                                    Ext.Msg.alert('Ошибка выполнения', jo.message);
                                    item.setValue(false);
                                }
                            },
                            failure: function (response) {
                                var jo = Ext.util.JSON.decode(response.responseText);
                                Ext.Msg.alert('Ошибка выполнения', jo.message);
                                item.setValue(false);
                            }
                        });
                    }
                    else {
                        item.setValue(false);
                    }
                },
                modal: true,
                icon: Ext.Msg.QUESTION,
                closable: false
            });
        },

        fillOtherSourcesCheckbox: function (records) {
            if (records.data.items.length === 0)
                return;
            window.E86n.View.StateTask2016.update = false;
            var cb = window.Ext.getCmp('OtherSourcesCheckbox');
            if (cb) {
                cb.setValue(records.data.items[0].get('IsOtherSources'));
            }
            window.E86n.View.StateTask2016.update = true;
        },

        DataChanged: function (checkBox, store) {
            window.E86n.View.StateTask2016.update = false;
            var cb = Ext.getCmp(checkBox);
            var record = store.getAt(0);
            if (record) {
                cb.setValue(record.get('NotBring'));
            } else {
                cb.setValue(false);
            }
            window.E86n.View.StateTask2016.update = true;
        },

        Check: function (item, checked, url, docId) {
            if (window.E86n.View.StateTask2016.update) {
                if (checked) {
                    Ext.Msg.show({
                        title: 'Подтверждение',
                        msg: 'Данное действие удалит все данные документа. Продолжить?',
                        width: 300,
                        buttons: Ext.Msg.YESNO,
                        fn: function (btn) {
                            if (btn === 'yes') {
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
                                            window.E86n.View.StateTask2016.update = false;
                                            window.E86n.View.StateTask2016.SetReadOnlyStateTask(true, docId);
                                            window.E86n.View.StateTask2016.update = true;
                                        } else {
                                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                                            window.E86n.View.StateTask2016.update = false;
                                            window.E86n.View.StateTask2016.SetReadOnlyStateTask(false, docId);
                                            item.setValue(false);
                                            window.E86n.View.StateTask2016.update = true;
                                        }
                                    },
                                    failure: function (response) {
                                        var jo = Ext.util.JSON.decode(response.responseText);
                                        Ext.Msg.alert('Ошибка выполнения', jo.message);
                                        window.E86n.View.StateTask2016.update = false;
                                        window.E86n.View.StateTask2016.SetReadOnlyStateTask(false, docId);
                                        item.setValue(false);
                                        window.E86n.View.StateTask2016.update = true;
                                    }
                                });
                            }
                            else {
                                window.E86n.View.StateTask2016.update = false;
                                window.E86n.View.StateTask2016.SetReadOnlyStateTask(false, docId);
                                item.setValue(false);
                                window.E86n.View.StateTask2016.update = true;
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
                                window.E86n.View.StateTask2016.update = false;
                                window.E86n.View.StateTask2016.SetReadOnlyStateTask(false, docId);
                                window.E86n.View.StateTask2016.update = true;
                            } else {
                                Ext.Msg.alert('Ошибка выполнения', jo.message);
                                window.E86n.View.StateTask2016.update = false;
                                window.E86n.View.StateTask2016.SetReadOnlyStateTask(true, docId);
                                item.setValue(true);
                                window.E86n.View.StateTask2016.update = true;
                            }
                        },
                        failure: function (response) {
                            var jo = Ext.util.JSON.decode(response.responseText);
                            Ext.Msg.alert('Ошибка выполнения', jo.message);
                            window.E86n.View.StateTask2016.update = false;
                            window.E86n.View.StateTask2016.SetReadOnlyStateTask(true, docId);
                            item.setValue(true);
                            window.E86n.View.StateTask2016.update = true;
                        }
                    });
                }
            }
        },

        getOtherSourcesCheckboxValue: function () {
            var cb = window.Ext.getCmp('OtherSourcesCheckbox');
            if (cb) {
                return cb.getValue();
            }

            return false;
        },

        // todo загрузка формы по изменению стора может быть вынесено в общий функционал
        StoreDataChanged: function (store, formId) {
            var record = store.getAt(0) || {};
            var cmp = Ext.getCmp(formId);
            if (cmp) {
                cmp.getForm().loadRecord(record);
            }
        },

        // табы по которым ничего не загружаем(исключаемые)
        // todo вынести в общий функционал
        excludeTabsOnReload: [],

        // todo вынести в общий функционал
        reloadDetail: function (item) {
            if (item) {
                var activeTab = item.getActiveTab();
                if (!window.E86n.View.StateTask2016.excludeTabsOnReload.some(x => x === activeTab.id)) {
                    var storeId = activeTab.id + 'Store';
                    eval(storeId).reload();
                }
            }
        }
    };