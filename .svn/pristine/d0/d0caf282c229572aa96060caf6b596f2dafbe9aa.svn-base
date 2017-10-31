function MarkLoadedDataAsDirty(store) {
    var f = function (r) {
        r.newRecord = true;
        if (store.modified.indexOf(r) == -1) {
            store.modified.push(r);
        }
        r.markDirty();
    };
    store.each(f);
    store.groupBy(store.groupField, true); //это только для того, чтобы отрисовались красные треугольнички у ячеек 
};

function saveReport(store) {
    store.save();
};

function addOrganization(store) {
    if (store.isDirty()) {
        Ext.Msg.alert('Предупреждение', 'Необходимо сначала сохранить изменения');
        return;
    }
    wOrganization.show();
};

function deleteOrganization(grid) {
    if (grid.store.isDirty()) {
        Ext.Msg.alert('Предупреждение', 'Необходимо сначала сохранить изменения');
        return;
    }

    var selection = grid.getSelectionModel().getSelections();
    if (selection == undefined || selection.length == 0) {
        Ext.Msg.alert('Предупреждение', 'Необходимо сначала выбрать ячейку у нужной организации');
        return;
    }

    var org = selection[0];

    Ext.Msg.show({
        title: 'Удалить',
        msg: 'Удалить организацию "' + org.get('NameOrg') + '" со всеми данными по ней?',
        buttons: Ext.Msg.YESNO,
        icon: Ext.MessageBox.WARNING,
        fn: function (button) {
            if (button != 'yes') {
                return;
            }
            Ext.net.DirectMethod.request({
                url: '/Org3PriceAndTariffs/ExcludeOrganization',
                cleanRequest: true,
                params: { taskId         : taskId.getValue(),
                          organizationId : org.get('RefRegistrOrg')
                },
                failure: failureSaveHandler,
                success: function () {
                    grid.store.reload();
                }
            }); 
        }
    });
    Ext.MessageBox.getDialog().defaultButton = 2;
    Ext.MessageBox.getDialog().focus();
};


//для wOrganization
function initializeOrgWindow() {
    orgNameCombo.setValue(null);
    orgIsMarketGrid.setValue(false);
    btnChooseOrganization.disable();
};

function onSelectOrganization(comboItem) {
    var selectedRow = dsOrganization.getById(comboItem.value);
    orgIsMarketGrid.setValue(selectedRow.get('IsMarketGrid'));
    btnChooseOrganization.enable();
};

function deselectOrganization() {
    btnChooseOrganization.disable();
};

// Создание новой организации
function createAndAddOrganization(store, goodType) {
   if (!orgNameCombo.getText()) {
       Ext.Msg.alert('Ошибка', 'Наименование организации должно быть заполнено!');
       return;
   }
   Ext.net.Mask.show({ msg: 'Создание...' });
   Ext.net.DirectMethod.request({
       url: '/Org3PriceAndTariffs/CreateAndIncludeOrganization',
       cleanRequest: true,
       params: { taskId: taskId.getValue(),
           orgName: orgNameCombo.getText(),
           orgIsMarketGrid: orgIsMarketGrid.getValue(),
           goodType: goodType
       },
       failure: failureSaveHandler,
       success: function () {
           Ext.net.Mask.hide();
           wOrganization.hide();
           store.reload();
       }
   }); 
};

// Добавление новой организации в список
function addSelectedOrganization(store) {
    var orgId = orgNameCombo.getValue();
    if (!orgId) {
        Ext.Msg.alert('Ошибка', 'Организация не выбрана!');
        return;
    }
    Ext.net.Mask.show({ msg: 'Создание...' });
    Ext.net.DirectMethod.request({
        url: '/Org3PriceAndTariffs/IncludeOrganization',
        cleanRequest: true,
        params: { taskId: taskId.getValue(),
                  organizationId: orgId
        },
        failure: failureSaveHandler,
        success: function () {
            Ext.net.Mask.hide();
            wOrganization.hide();
            store.reload();
        }
    });
};

function failureSaveHandler(response, result) {
    Ext.net.Mask.hide();
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};

function copyFromOtherReport() {
    wCopyReport.show();
}

// для wCopyReport
function initializeCopyWindow() {
    gpCopyReport.getSelectionModel().clearSelections();
    btnCopyFromReport.disable();
};

function copyFromReport(store) {
    if (!gpCopyReport.hasSelection()) {
        Ext.Msg.alert('Ошибка', 'Ничего не выбрано.');
        return;
    }

    var selectedRow = gpCopyReport.getSelectionModel().getSelections()[0];
    Ext.Msg.show({
        title: 'Копирование',
        msg: 'Существующие данные будут удалены, и затем скопированы с отчета за ' + selectedRow.get('ReportDate') + '. Продолжить?',
        buttons: Ext.Msg.YESNO,
        icon: Ext.MessageBox.WARNING,
        fn: function (button) {
                if (button != 'yes') {
                    return;
                }
                Ext.net.Mask.show({ msg: 'Выполняется копирование...' });
                Ext.net.DirectMethod.request({
                    url: '/Org3PriceAndTariffs/CopyFromReport',
                    cleanRequest: true,
                    params: { taskId         : taskId.getValue(),
                              sourceTaskId : selectedRow.get('ID')
                    },
                    failure: failureSaveHandler,
                    success: function () {
                        Ext.net.Mask.hide();
                        wCopyReport.hide();
                        store.commitChanges();
                        store.reload();
                    }
            }); 
        }
    });
    Ext.MessageBox.getDialog().defaultButton = 2;
    Ext.MessageBox.getDialog().focus();
};