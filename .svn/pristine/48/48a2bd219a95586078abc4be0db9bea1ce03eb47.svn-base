Ext.ns("EO15AIP.View.Register");

var beforeCloseTabWithOneGrid = function (currentTab, grid) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }

    currentTab.ownerCt.setActiveTab(currentTab);
    if (grid.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Все несохраненные изменения будут потеряны. Сохранить данные?',
            buttons: { yes: 'Сохранить', no: 'Закрыть', cancel: 'Отмена' },
            animEl: 'MdiTab',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    grid.save();
                }
                else if (buttonId == 'no') {
                    EO15AIP.View.Register.Grid.closeTab(currentTab);
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};

var beforeCloseFinance = function (currentTab) {
    return beforeCloseTabWithOneGrid(currentTab, FinanceGrid);
};

var beforeCloseInfo = function (currentTab) {
    return beforeCloseTabWithOneGrid(currentTab, InfoGrid);
};

var beforeCloseCard = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }

    currentTab.ownerCt.setActiveTab(currentTab);
    var cardIsDirty = CObjectCardForm != undefined && CObjectCardForm.isDirty();
    var expertiseIsDirty = DetailExpertiseGrid != undefined && DetailExpertiseGrid.isDirty();
    var contractIsDirty = DetailContractGrid != undefined && DetailContractGrid.isDirty();
    var limitIsDirty = DetailLimitGrid != undefined && DetailLimitGrid.isDirty();
    var planIsDirty = DetailPlanGrid != undefined && DetailPlanGrid.isDirty();
    var reviewIsDirty = DetailReviewGrid != undefined && DetailReviewGrid.isDirty();
    var additInfoIsDirty = DetailAdditObjectInfoGrid != undefined && DetailAdditObjectInfoGrid.isDirty();
    var filesIsDirty = gpTaskFiles != undefined && gpTaskFiles.isDirty();
    if (cardIsDirty || expertiseIsDirty || contractIsDirty || limitIsDirty || planIsDirty || reviewIsDirty || additInfoIsDirty || filesIsDirty) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Все несохраненные изменения будут потеряны. Сохранить данные?',
            buttons: { yes: 'Сохранить', no: 'Закрыть', cancel: 'Отмена' },
            animEl: 'MdiTab',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    if (expertiseIsDirty) {
                        DetailExpertiseGrid.save();
                    }
                    if (contractIsDirty) {
                        DetailContractGrid.save();
                    }
                    if (limitIsDirty) {
                        DetailLimitGrid.save();
                    }
                    if (planIsDirty) {
                        DetailPlanGrid.save();
                    }
                    if (reviewIsDirty) {
                        DetailReviewGrid.save();
                    }
                    if (additInfoIsDirty) {
                        DetailAdditObjectInfoGrid.save();
                    }
                    if (cardIsDirty) {
                        EO15AIP.View.Register.Grid.cardSaveAndClose(currentTab, false);
                    }
                    if (filesIsDirty) {
                        gpTaskFiles.store.save();
                    }
                }
                else if (buttonId == 'no') {
                    EO15AIP.View.Register.Grid.closeTab(currentTab);
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};

var beforeCloseCardNew = function (currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }

    currentTab.ownerCt.setActiveTab(currentTab);
    if (CObjectCardForm.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'Все несохраненные изменения будут потеряны. Сохранить данные?',
            buttons: { yes: 'Сохранить', no: 'Закрыть', cancel: 'Отмена' },
            animEl: 'MdiTab',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == 'yes') {
                    EO15AIP.View.Register.Grid.cardSaveAndClose(currentTab, true);
                }
                else if (buttonId == 'no') {
                    EO15AIP.View.Register.Grid.closeTab(currentTab);
                }
            }
        });
        return false;
    }
    else {
        return true;
    }
};

EO15AIP.View.Register.Grid = {
    toggleFilter: function (button, state) {
        RegisterGrid.load();
    },
    getStateFilter: function () {
        var filter = [true, true, true, true, true];
        filter[0] = statusFilter1.pressed;
        filter[1] = statusFilter2.pressed;
        filter[2] = statusFilter3.pressed;
        filter[3] = statusFilter4.pressed;
        filter[4] = statusFilter5.pressed;
        return filter;
    },
    deleteRecordInGrid: function (elemId, gridId) {
        var grid = Ext.getCmp(gridId);
        Ext.Msg.show({
            title: 'Анализ и планирование',
            msg: 'Вы действительно хотите удалить выделенные записи?',
            buttons: Ext.MessageBox.YESNO,
            multiline: false,
            animEl: elemId,
            icon: Ext.MessageBox.WARNING,
            fn: function (btn, gridId) {
                if (btn == 'yes') {
                    {
                        var selection = grid.selModel.selection;
                        if (selection != undefined && selection != null) {
                            {
                                var rec = selection.record;
                                grid.deleteRecord(rec);
                            }
                        }
                    }
                }
            }
        });
    },
    acceptTypeHandler: function (winId, gpId) {
        var win = Ext.getCmp(winId);
        var gp = Ext.getCmp(gpId);
        var rec = win.getBody().Extension.entityBook.selectedRecord;
        var recordToUpdate = gp.selModel.selection.record;
        recordToUpdate.beginEdit();
        recordToUpdate.set('TypeWorkId', rec.data.ID);
        recordToUpdate.set('TypeWorkName', rec.data.NAME);
        recordToUpdate.endEdit();
        win.hide();
    },
    rendererFn: function (v, p, r, rowIndex, colIndex, ds) {
        if (r.data.StateId > 0) {
            var tpl = '<img title=\'{1}\' src=\'{0}\' width=\'16\' height=\'16\' />';
            return String.format(
                        tpl,
                        Ext.getCmp(String.format('UrlIconStatus{0}', r.data.StateId)).getValue(), r.data.StateName);
        }
    },
    getTabByUrl: function (url) {
        var idTab = parent.MdiTab.hashCode(url);
        var tab = parent.MdiTab.getComponent(idTab);
        return tab;
    },
    closeTab: function (tab) {
        tab.forceClose = true;
        tab.ownerCt.closeTab(tab);
    },
    cardSaveAndClose: function (tab, needClose) {
        if (CObjectCardForm.isValid()) {
            CObjectCardForm.form.submit({
                waitMsg: 'Сохранение...',
                success: function (form, action) {
                    CObjectCardForm.form.items.each(function (f) {
                        f.originalValue = f.getValue();
                    });

                    if (needClose) {
                        EO15AIP.View.Register.Grid.closeTab(tab);
                    }
                    else {
                        var newUrl = '/EO15AIPRegister/ShowCObjectCard?objId=' + action.result.message;
                        if (tab.autoLoad.url != newUrl) {
                            debugger;
                            var objName = Ext.getCmp('Name').getValue();
                            var regionName = Ext.getCmp('cbRegion').getText();
                            parent.MdiTab.addTab({
                                title: 'Карточка объекта: ' + objName + ', ' + regionName,
                                url: newUrl
                            });
                            EO15AIP.View.Register.Grid.closeTab(tab);
                        }
                    }
                },
                failure: function (form, action) {
                    var fi = action.response.responseText.indexOf('message:') + 9;
                    var li = action.response.responseText.lastIndexOf('"}')
                    var msg = action.response.responseText.substring(li, fi);
                    Ext.net.Notification.show({
                        iconCls: 'icon-information',
                        html: msg,
                        title: 'Внимание',
                        hideDelay: 2500
                    });
                }
            });
        }
        else {
            Ext.net.Notification.show({
                iconCls: 'icon-information',
                html: 'Сохранение невозможно. Необходимо заполнить все обязательные поля.',
                title: 'Внимание',
                hideDelay: 2500
            });
        }
    },
    cardSaveAndCloseHandler: function (objectId, needClose) {
        var tab = EO15AIP.View.Register.Grid.getTabByUrl('/EO15AIPRegister/ShowCObjectCard?objId=' + objectId);
        EO15AIP.View.Register.Grid.cardSaveAndClose(tab, needClose);
    },
    registerOpenTab: function (command, record) {
        if (command == 'OpenCObject') {
            parent.MdiTab.addTab({
                title: 'Карточка объекта: ' + record.data.Name + ', ' + record.data.RegionName,
                url: '/EO15AIPRegister/ShowCObjectCard?objId=' + record.data.ID,
                icon: 'icon-report'
            });
        }
        else if (command == 'OpenFinance') {
            parent.MdiTab.addTab({
                title: 'Финансирование: ' + record.data.Name + ', ' + record.data.RegionName,
                url: '/EO15AIPRegister/ShowFinance?objId=' + record.data.ID,
                icon: 'icon-report'
            });
        }
        else if (command == 'OpenInfo') {
            parent.MdiTab.addTab({
                title: 'Справка о вводе эксплуатацию: ' + record.data.Name + ', ' + record.data.RegionName,
                url: '/EO15AIPRegister/ShowInfo?objId=' + record.data.ID,
                icon: 'icon-report'
            });
        }
    },
    openNewObjectTab: function () {
        parent.MdiTab.addTab({
            title: 'Новый объект строительства',
            url: '/EO15AIPRegister/ShowCObjectCard?objId=0',
            icon: 'icon-report'
        });
    }
};