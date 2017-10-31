Ext.ns('E86n.View.ResultsOfActivityView');

E86n.View.ResultsOfActivityView.Common =
    {
        reloadDetail: function () {
            var activeTab = window.DetailTabPanel.getActiveTab();

            if (activeTab.id == 'MembersOfStaff') {
                window.MembersOfStaffStore.reload();
            }

            if (activeTab.id == 'FinNFinAssets') {
                window.FinNFinAssetsStore.reload();
            }

            if (activeTab.id == 'Cash') {
                window.CashReceiptsStore.reload();
                window.CashPaymentsStore.reload();
            }

            if (activeTab.id == 'ServicesWorks') {
                window.ServicesWorksStore.reload();
            }

            if (activeTab.id == 'ServicesWorks2016') {
                window.ServicesWorks2016Store.reload();
            }

            if (activeTab.id == 'PropertyUse') {
                window.PropertyUseStore.reload();
            }
        },
        
        SetReadOnlyResultsOfActivitySpecial: function () {
            var grid = Ext.getCmp('ServicesWorks');
            if (grid) {
                window.ServicesWorksNewRecordBtn.setVisible(false);
                window.ServicesWorksRemoveRecordBtn.setVisible(false);
            }
            
            grid = Ext.getCmp('ServicesWorks2016');
            if (grid) {
                window.ServicesWorks2016NewRecordBtn.setVisible(false);
                window.ServicesWorks2016RemoveRecordBtn.setVisible(false);
            }
        },

        SetReadOnlyResultsOfActivity: function (readOnly, recId) {
            window.btnMembersOfStaffSave.setVisible(!readOnly);
            window.btnFinNFinAssetsSave.setVisible(!readOnly);
            window.btnUpLoadFileDownloadBtn.setDisabled(readOnly);
            window.btnCashReceiptsSave.setVisible(!readOnly);
            window.btnPropertyUseSave.setVisible(!readOnly);

            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('MembersOfStaff', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('FinNFinAssets', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('CashReceipts', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('PropertyUse', readOnly);

            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ServicesWorks');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ServicesWorks2016');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'CashPayments');
            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        }
    };

E86n.View.ResultsOfActivityView.MembersOfStaff =
    {
        StoreDataChanged: function (store) {
            var record = store.getAt(0) || {};
            window.MembersOfStaff.getForm().loadRecord(record);
        },

        ClientValidation: function (valid) {
            window.btnMembersOfStaffSave.setDisabled(!valid);
            window.btnMembersOfStaffSave.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
        },

        Refresh: function () {
            window.MembersOfStaffStore.reload();
        },

        Save: function () {
            window.MembersOfStaff.form.submit({
                waitMsg: 'Сохранение...',
                success: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.net.Notification.show({
                            iconCls: 'icon-information',
                            html: action.result.extraParams.msg,
                            title: 'Сохранение',
                            hideDelay: 2000
                        });
                },
                failure: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.Msg.show({
                            title: 'Ошибка сохранения',
                            msg: action.result.extraParams.msg,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR,
                            maxWidth: 1000
                        });
                    window.MembersOfStaffStore.reload();
                },
            });
        }
    };

E86n.View.ResultsOfActivityView.FinNFinAssets =
    {
        SetRefTypeIxmValue: function (combo) {
            if (combo.getText() == "Без изменений") {
                Ext.getCmp(combo.container.dom.lastChild.id).setValue("");
                Ext.getCmp(combo.container.dom.lastChild.id).disable();
            } else {
                Ext.getCmp(combo.container.dom.lastChild.id).enable();
            }
        },

        StoreDataChanged: function (store) {
            var record = store.getAt(0) || {};
            window.FinNFinAssets.getForm().loadRecord(record);
        },

        ClientValidation: function (valid) {
            window.btnFinNFinAssetsSave.setDisabled(!valid);
            window.btnFinNFinAssetsSave.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
        },

        StoreLoad: function () {
            this.SetRefTypeIxmValue(window.InfAboutCarryingValueTotalRefTypeIxm);
            this.SetRefTypeIxmValue(window.ImmovablePropertyRefTypeIxm);
            this.SetRefTypeIxmValue(window.ParticularlyValuablePropertyRefTypeIxm);
            this.SetRefTypeIxmValue(window.ChangingArrearsRefTypeIxm);
            this.SetRefTypeIxmValue(window.IncomeRefTypeIxm);
            this.SetRefTypeIxmValue(window.ExpenditureRefTypeIxm);
            this.SetRefTypeIxmValue(window.IncreaseInAccountsPayableTotalRefTypeIxm);
            this.SetRefTypeIxmValue(window.OverduePayablesRefTypeIxm);
        },

        Refresh: function () {
            window.FinNFinAssetsStore.reload();
        },

        Save: function () {
            window.FinNFinAssets.form.submit({
                waitMsg: 'Сохранение...',
                success: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.net.Notification.show({
                            iconCls: 'icon-information',
                            html: action.result.extraParams.msg,
                            title: 'Сохранение',
                            hideDelay: 2000
                        });
                },
                failure: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.Msg.show({
                            title: 'Ошибка сохранения',
                            msg: action.result.extraParams.msg,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR,
                            maxWidth: 1000
                        });
                    window.FinNFinAssetsStore.reload();
                }
            });
        }
    };

E86n.View.ResultsOfActivityView.CashReceipts =
    {
        StoreDataChanged: function (store) {
            var record = store.getAt(0) || {};
            window.CashReceipts.getForm().loadRecord(record);
        },

        ClientValidation: function (valid) {
            window.btnCashReceiptsSave.setDisabled(!valid);
            window.btnCashReceiptsSave.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
        },

        Refresh: function () {
            window.CashReceiptsStore.reload();
        },

        Save: function () {
            window.CashReceipts.form.submit({
                waitMsg: 'Сохранение...',
                success: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.net.Notification.show({
                            iconCls: 'icon-information',
                            html: action.result.extraParams.msg,
                            title: 'Сохранение',
                            hideDelay: 2000
                        });
                },
                failure: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.Msg.show({
                            title: 'Ошибка сохранения',
                            msg: action.result.extraParams.msg,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR,
                            maxWidth: 1000
                        });
                    window.CashReceiptsStore.reload();
                }
            });
        }
    };

E86n.View.ResultsOfActivityView.PropertyUse =
    {
        StoreDataChanged: function (store) {
            var record = store.getAt(0) || {};
            window.PropertyUse.getForm().loadRecord(record);
        },

        ClientValidation: function (valid) {
            window.btnPropertyUseSave.setDisabled(!valid);
            window.btnPropertyUseSave.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
        },

        Refresh: function () {
            window.PropertyUseStore.reload();
        },

        Save: function () {
            window.PropertyUse.form.submit({
                waitMsg: 'Сохранение...',
                success: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.net.Notification.show({
                            iconCls: 'icon-information',
                            html: action.result.extraParams.msg,
                            title: 'Сохранение',
                            hideDelay: 2000
                        });
                },
                failure: function (form, action) {
                    if (action.result && action.result.extraParams && action.result.extraParams.msg)
                        window.Ext.Msg.show({
                            title: 'Ошибка сохранения',
                            msg: action.result.extraParams.msg,
                            buttons: window.Ext.Msg.OK,
                            icon: window.Ext.MessageBox.ERROR,
                            maxWidth: 1000
                        });
                    window.PropertyUseStore.reload();
                }
            });
        }
    };

E86n.View.ResultsOfActivityView.CashPayments =
    {
        //обработчик ввыбора КОСГУ, подставляем форматированный код
        SelectRowGrid: function (record, lookupVal, grid) {
            var maskval = buildMask(lookupVal.data.Code, "#\.#\.#") + ";" + lookupVal.data.Name;
            record.setValue(maskval);
            Ext.getCmp(grid).getSelectionModel().getSelected().set('RefKosgyName', maskval);
        },
        
        Update: function (){
            if (window.CashPayments.getSelectionModel().hasSelection()) {
                window.CashPaymentsRemoveRecordBtn.setDisabled(false);
            } else {
                window.CashPaymentsRemoveRecordBtn.setDisabled(true);
            }
        },

        getVidRashFilter: function (docId) {
            //стор параметров документа
            var storeId = 'dsParamDoc' + docId;
            //год документа
            var yearDoc = eval(storeId).getAt(0).data.RefYearFormID;
            //текущий год
            var year = new Date().getFullYear();

            if (yearDoc == year) {
                return '((EFFECTIVEFROM IS NULL AND EFFECTIVEBEFORE IS NULL) OR (EFFECTIVEFROM IS NULL AND EFFECTIVEBEFORE >= GETDATE()) OR ' +
                  '(EFFECTIVEFROM <= GETDATE() AND EFFECTIVEBEFORE IS NULL) OR (GETDATE() BETWEEN EFFECTIVEFROM AND EFFECTIVEBEFORE))';
            } else {
                //если текущий год не равен году документа то даты проверяем относительно года формирования документа 
                //и соответственно смотрим только года а не даты целиком
                return String.format('((EFFECTIVEFROM IS NULL AND EFFECTIVEBEFORE IS NULL) OR (EFFECTIVEFROM IS NULL AND YEAR(EFFECTIVEBEFORE) >= {0}) OR ' +
                  '(YEAR(EFFECTIVEFROM) <= {0} AND EFFECTIVEBEFORE IS NULL) OR ({0} BETWEEN YEAR(EFFECTIVEFROM) AND YEAR(EFFECTIVEBEFORE)))', yearDoc);
            }
        },
    };

E86n.View.ResultsOfActivityView.ServicesWorks =
    {
        Update: function (){
            if (window.ServicesWorks.getSelectionModel().hasSelection()) {
                window.ServicesWorksRemoveRecordBtn.setDisabled(false);
            } else {
                window.ServicesWorksRemoveRecordBtn.setDisabled(true);
            }
        }
    };

E86n.View.ResultsOfActivityView.ServicesWorks2016 =
    {
        Update: function (){
            if (window.ServicesWorks2016.getSelectionModel().hasSelection()) {
                window.ServicesWorks2016RemoveRecordBtn.setDisabled(false);
            } else {
                window.ServicesWorks2016RemoveRecordBtn.setDisabled(true);
            }
        }
    };