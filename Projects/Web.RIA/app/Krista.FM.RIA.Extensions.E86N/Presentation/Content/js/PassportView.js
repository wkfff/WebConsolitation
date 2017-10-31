Ext.ns('E86n.View.PassportView');

E86n.View.PassportView =
    {
        RowSelect: function(grid) {
            var btn = Ext.getCmp(grid + 'RemoveRecordBtn');
            if (btn != undefined) {
                btn.enable();
            }
        },

        RowDeselect: function(grid) {
            var btn = Ext.getCmp(grid + 'RemoveRecordBtn');
            if (!Ext.getCmp(grid).hasSelection() && btn != undefined) {
                btn.disable();
            }
        },

        StoreDataChanged: function(store) {
            var record = store.getAt(0) || { };
            window.InstitutionInfo.getForm().loadRecord(record);
        },

        Load: function(recId, records) {
            window.comboRefCatYh.fireEvent('select');
            if (records[0].data.ID != -1) {
                window.BranchesStore.reload();
                window.TypesOfActivitiesStore.reload();
                window.FoundersStore.reload();
                if (records[0].data.RefCatYh == 2)
                    window.Branches.setDisabled(false);
                else
                    window.Branches.setDisabled(true);
                window.TypesOfActivities.setDisabled(false);
                window.Founders.setDisabled(false);
            }
            ;
        },

        SelectInstitutionSorts: function (record, lookupVal) {
            record.setValue(lookupVal.data.Name);
            RefVid.setValue(lookupVal.data.ID);
        },

        SaveForm: function() {
            window.InstitutionInfo.form.submit({
                clientValidation: false,
                waitMsg: 'Сохранение...',
                failure: function(form, action) {

                    Ext.Msg.show({
                        title: 'Ошибка сохранения данных',
                        msg: "Есть некорректно заполненные поля",
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.ERROR,
                        maxWidth: 1000
                    });
                },
                success: function(form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.newID) {
                        form.setValues({ 'ID': action.result.extraParams.newID });
                    }
                    window.InstitutionInfoStore.reload();
                }
            });
        },

        RestrictFields: function(readOnly) {
            window.Ogrn.setReadOnly(readOnly);
            window.Adr.setReadOnly(readOnly);
            window.Fam.setReadOnly(readOnly);
            window.NameRuc.setReadOnly(readOnly);
            window.Otch.setReadOnly(readOnly);
            window.Ordinary.setReadOnly(readOnly);
        },

        SetReadOnlyPassport: function(readOnly, docId) {
            window.btnSave.setDisabled(readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('InstitutionInfo', readOnly);

            if (!readOnly) {
                //поля которые всегда должны быть закрыты для редактирования
                window.RefOrgPpoName.setReadOnly(true);
                window.RefTipYcName.setReadOnly(true);
                window.RefOrgGrbsName.setReadOnly(true);
                window.Name.setReadOnly(true);
                window.ShortName.setReadOnly(true);
                window.Inn.setReadOnly(true);
                window.Kpp.setReadOnly(true);
                window.OpeningDate.setReadOnly(true);
                window.CloseDate.setReadOnly(true);
                window.RefSostName.setReadOnly(true);
            }

            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Branches');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'TypesOfActivities');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Founders');
            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, docId);
        }
    };