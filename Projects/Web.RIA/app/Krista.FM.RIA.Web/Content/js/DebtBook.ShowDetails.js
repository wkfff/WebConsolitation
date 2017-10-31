Extension.DebtBookShowDetailsView = {
    activeRefField: {},
    activeLookupField: {},
    selectedBookRecord: {},
    onBookRowSelect: function (record) {
        btnOk.enable();
        Extension.DebtBookShowDetailsView.selectedBookRecord = record;
    }
};

function unlock() {
    window.parent.DetailsWindow.autoLoad.url = window.parent.DetailsWindow.autoLoad.url + '&ForceUnlock=true';
    window.parent.DetailsWindow.reload();
};

function triggerClick(field, bookObjectKey) {
    Extension.DebtBookShowDetailsView.activeRefField = Ext.getCmp(field.id.substring(3, field.id.length));
    Extension.DebtBookShowDetailsView.activeLookupField = field;

    var sourceId = -1;
    var filter = 'ID>0';
    if (bookObjectKey == '383f887a-3ebb-4dba-8abb-560b5777436f') {
        // Классификатор "Районы.Анализ"
        sourceId = Extension.View.getWorkbench().extensions.DebtBook.currentAnalysisSourceId;
        var userRegionId = Extension.View.getWorkbench().extensions.DebtBook.userRegionId;
        var userRegionType = Extension.View.getWorkbench().extensions.DebtBook.userRegionType;
        var year = Extension.View.getWorkbench().extensions.DebtBook.userYear;
        var regionType = Extension.DebtBookShowDetailsView.regionType;

        // Субъект
        if (userRegionType == 0) {
            // Вкладка «Субъект»
            if (regionType == 0) {
                // Видим только себя
                filter = '(ID = ' + userRegionId + ')';
            }
            // Вкладка «Районы»
            if (regionType == 2) {
                // Видим только МР(4) и ГО(7)
                filter = '(RefTerr = 4 or RefTerr = 7)';
            }
            // Вкладка «Поселениея»
            if (regionType == 3) {
                // Видим только записи лежащие ниже на 4 уровня
                filter = '(ParentID IN (select id from d_Regions_Analysis where ParentID IN (select id from d_Regions_Analysis where ParentID IN (select id from d_Regions_Analysis where ParentID = ' + userRegionId + '))))'
            }
        }

        // Район и город
        if (userRegionType == 2 || userRegionType == 1) {
            // Вкладка «Районы»
            if (regionType == 2) {
                // Видим только себя
                filter = '(ID = ' + userRegionId + ')';
            }
            // Вкладка «Поселения»
            if (regionType == 3) {
                // Видим только записи лежащие ниже на 2 уровня
                filter = '(ParentID IN (select id from d_Regions_Analysis where ParentID = ' + userRegionId + '))'
            }
        }
        // Поселения
        if (userRegionType == 3) {
            // Видим только себя
            filter = ' (ID = ' + userRegionId + ') ';
        }
    } else {
        sourceId = Extension.View.getWorkbench().extensions.DebtBook.currentSourceId
    }

    BookWindow.autoLoad.params.id = Extension.DebtBookShowDetailsView.activeRefField.getValue();
    if (sourceId != -1) {
        BookWindow.autoLoad.params.sourceId = sourceId;
    }
    BookWindow.autoLoad.params.filter = filter;
    BookWindow.autoLoad.params.showMode = "WithoutHierarchy";

    BookWindow.autoLoad.url = "/Entity/Book?objectKey=" + bookObjectKey;
    BookWindow.setTitle(field.fieldLabel);
    btnOk.disable();
    BookWindow.show();
}

function bookClose(btn, e) {
    if (Extension.DebtBookShowDetailsView.selectedBookRecord) {
        Extension.DebtBookShowDetailsView.activeRefField.setValue(BookWindow.getBody().Extension.entityBook.selectedRecord.id);
        Extension.DebtBookShowDetailsView.activeLookupField.setValue(BookWindow.getBody().Extension.entityBook.getLookupValue());
        BookWindow.hide();
    }
}

var failureHandler = function (form, action) {
    var msg = '';
    if (action.failureType == "client" || (action.result && action.result.errors && action.result.errors.length > 0)) {
        msg = "Проверьте правильность заполнения полей";
    } else if (action.result && action.result.extraParams.msg) {
        msg = action.result.extraParams.msg;
    } else if (action.response) {
        msg = action.response.responseText;
    }

    Ext.Msg.show({
        title: 'Ошибка сохранения',
        msg: msg,
        buttons: Ext.Msg.OK,
        icon: Ext.Msg.ERROR
    });
}

var successHandler = function (form, action) {
    if (action.result && action.result.extraParams && action.result.extraParams.newID) {
        dsEntity.getAt(0).id = action.result.extraParams.newID;
        if (dsEntity.getAt(0).newRecord) {
            delete dsEntity.getAt(0).newRecord;
        }
    }
    else {
        Ext.MessageBox.alert('Результат', 'Запись удачно сохранена!');
    }

    entityChanged = true;

    if (action.options.params.setNew) {
        DetailsForm.form.reset();
        dsEntity.removeAll();

        var rec = new dsEntity.recordType();
        rec.newRecord = true;
        dsEntity.add(rec);
        initUI(true);
    }
    else {
        initUI(false);
    }

    txtEntities.lastQuery = null;

    ViewPersistence.isChanged = true;
}

var getEntityID = function () {
    return (dsEntity.getCount() > 0 && !dsEntity.getAt(0).newRecord) ? dsEntity.getAt(0).id : ''
}

var entityLoaded = function (store, records) {
    var isNew = false;
    if (records.length > 0) {
        if (window.dsChilds) {
            dsChilds.load();
        }

        var isBlocked = records[0].get('ISBLOCKED');
        if (isBlocked && !Unlock.value) {
            disableUI();
        }

        if (Unlock.value) {
            Unlock.setDisabled(true);
        }

        DetailsForm.form.loadRecord(records[0]);

        if (records[0].get('ID') == -1) {
            isNew = true;
            createNewEntity(records[0]);
            ViewPersistence.isChanged = true;
        }
    }
    initUI(isNew);
    DetailsMainPanel.el.unmask();
};

var disableUI = function() {
    window.DetailsForm.getForm().items.each(function (item) { item.setReadOnly(true); });

    if (this.addChangesButton != undefined) {
        window.addChangesButton.setDisabled(true);
    }
    else {
        if (!window.dsChilds) {
            window.btnSave.setDisabled(true);
        }
    }
};

var initUI = function(isNew) {
    if (this.addChangesButton != undefined) {
        addChangesButton.setDisabled(isNew);
    }
    ID.setDisabled(!isNew);
};

var createNewEntity = function(record) {
    // Скрываем грид изменений
    setVisibleChangesGrid(false);

    if (window.dsChilds) {
        dsChilds.removeAll();
    }

    record.beginEdit();

    initNewRecord(record);

    record.set('ID', 0);
    record.markDirty();
    record.newRecord = true;
    record.endEdit();

    DetailsForm.form.loadRecord(record);
    DetailsForm.validate();
};

var initNewRecord = function(record) { };

var deleteEntity = function(recordId) {
    Ext.Msg.confirm('Предупреждение', 'Удалить запись?', function(btn) {
        if (btn == 'yes') {
            var indx = dsEntity.findBy(function(record, id) {
                return record.id == recordId;
            });

            dsEntity.removeAt(indx);
            dsEntity.save();
            idFilter.setValue('');
            ViewPersistence.isChanged = true;
        }
    });
};

var setVisibleChangesGrid = function(visible) {
    if (this.gpChilds) {
        gpChilds.setVisible(visible);
        changesPanel.setVisible(!visible);
        addChangesButton.setVisible(!visible);
        gpChilds.setHeight(48 + gpChilds.getView().mainHd.getHeight() + gpChilds.store.data.length * 21);
    }
    ToolbarsPanel.doLayout();
};

ViewPersistence.isValid = function() {
    return DetailsForm.isValid();
};

ViewPersistence.isDirty = function () {
    var dirty = false;
    if (window.dsChilds) {
        dirty = dsChilds.isDirty();
    }
    return dirty || DetailsForm.isDirty();
};

// Сохранение данных формы
ViewPersistence.save = function (reload) {
    if (!DetailsForm.isValid()) {
        Ext.Msg.show({
            title: 'Анализ и планирование',
            msg: 'В карточке не все поля заполнены либо имеют некорректные значения.',
            buttons: Ext.MessageBox.OK,
            multiline: false,
            animEl: DetailsForm,
            icon: Ext.MessageBox.WARNING
        });
        return;
    }

    if (this.gpChilds) {
        gpChilds.stopEditing(false);
        if (!validateChanges(gpChilds.getStore().getModifiedRecords())) {
            Ext.Msg.show({
                title: 'Анализ и планирование',
                msg: 'Поле "Дата изменений" должно быть заполнено.',
                buttons: Ext.MessageBox.OK,
                multiline: false,
                animEl: gpChilds,
                icon: Ext.MessageBox.WARNING
            });
            return;
        }
        dsChilds.on(
            'commitdone',
            function () {
                saveForm(reload);
            },
            dsChilds,
            { single: true }
        );
        dsChilds.save();
    }
    else {
        saveForm(reload);
    }

    ViewPersistence.isChanged = true;
};

function saveForm(reload) {
    dsEntity.data.items[0].beginEdit();
    DetailsForm.form.updateRecord(dsEntity.data.items[0]);
    dsEntity.data.items[0].endEdit();

   // устанавливаем обработчик на завершение сохранения
    dsEntity.addListener(
        'save',
        function (store, batch, data) {
            // id записи (только что созданной или существующей)
            var id = -1;
            if (store.getAt(0).id > 0) {
                id = store.getAt(0).id;
            } else if (data.message) {
                if (data.message.indexOf('newId', 0) == 1) {
                    var result = eval('(' + data.message + ')');
                    id = result.newId;
                }
            }

            var viewName = document.location.pathname.split('/')[2];

            DetailsMainPanel.getEl().mask('Пересчет остатков...');
            Ext.net.DirectEvent.confirmRequest({
                cleanRequest: true,
                url: '/BebtBookData/Recalc',
                extraParams: { documentId: id, viewId: viewName }
            });

            DetailsMainPanel.getEl().mask('Проверка контрольных соотношений...');
            Ext.net.DirectEvent.confirmRequest({
                cleanRequest: true,
                url: '/BebtBookData/CheckControlRelationships',
                extraParams: { documentId: id, viewId: viewName }
            });
            
            if (reload) {
               store.reload();
            }
        },
        dsEntity,
        { single: true }
    );
    
    dsEntity.save();
}

