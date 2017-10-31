Ext.ns('E86n.View.AnnualBalanceView');

E86n.View.AnnualBalanceView =
    {
        Save: function () {
            window.E86n.View.AnnualBalanceView.submitForm('HeadAttribute');
        },

        Refresh: function () {
            window.HeadAttributeStore.reload();
        },

        submitForm: function (formId) {
            Ext.getCmp(formId).form.submit({
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
                }
            });
        },

        StoreDataChanged: function (store) {
            var record = store.getAt(0) || { };
            window.HeadAttribute.getForm().loadRecord(record);
        },

        ClientValidation: function (valid) {
            window.btnHeadAttributeSave.setDisabled(!valid);
            window.btnHeadAttributeSave.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
        },

        reloadDetail: function () {
            var activeTab = window.DetailTabPanel.getActiveTab();
            if (activeTab.id != 'DocDetail') {
                eval(activeTab.id + 'Store').reload();
            }
        },

        SetReadOnlyDoc: function (readOnly, recId, doctype) {
            
            window.consPumpBtn.setDisabled(readOnly);
            window.btnHeadAttributeSave.setVisible(!readOnly);

            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('HeadAttribute', readOnly);
            
            switch (doctype) {
            
            //AnnualBalanceF0503137Type   
            case 4:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Incomes');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Expenses');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'SourcesOfFinancing');
                }
                break;
            //AnnualBalanceF0503730Type
            case 9:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'NonfinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'FinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Liabilities');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'FinancialResult');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Information');
                }
                break;
            //AnnualBalanceF0503721Type   
            case 10:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Incomes');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Expenses');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'NonFinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'FinancialAssetsLiabilities');
                }
                break;
                
            //AnnualBalanceF0503737Type   
            case 11:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Incomes');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Expenses');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'SourcesOfFinancing');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ReturnExpense');
                }
                break;
            //AnnualBalanceF0503130Type
            case 12:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'NonfinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'FinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Liabilities');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'FinancialResult');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Information');
                }
                break;
            //AnnualBalanceF0503121Type
            case 13:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Incomes');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'Expenses');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'OperatingResult');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'OperationNonfinancialAssets');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'OperationFinancialAssets');
                }
                break;
            //AnnualBalanceF0503127Type   
            case 14:
                {
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'BudgetIncomes');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'BudgetExpenses');
                    window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'SourcesOfFinancing');
                }
                break;           
            }

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        },
        
        //обработчик ввыбора КОСГУ, подставляем форматированный код
        SelectRowGrid: function (record, lookupVal) {
            var grid = Ext.getCmp(HB.Grid);
            var fld = Ext.getCmp(HB.Field);
            var srec = grid.getSelectionModel().getSelected();
            if (srec != null) {
                srec.set(HB.Field.substring(0, HB.Field.length - 4), record.data.ID);
                var maskval = buildMask(record.data.CODE, "#\.#\.#") + ";" + lookupVal;
                srec.set(HB.Field, maskval);
                fld.setValue(maskval);
            }
        },
        
        BeforeEdit: function (e) {
            if (["150", "400", "410", "600", "900"].indexOf(e.record.data.LineCode) != -1)
                e.cancel = true;
        },
        
        BeforeSumm: function (grid) {
            var g = Ext.getCmp(grid);
            if (g) {
                if (g.getStore().getModifiedRecords().length == 0)
                {
                    return true;
                } else {
                    Ext.Msg.show({
                        title: 'Внимание',
                        msg: 'Имеются несохраненные изменения.<br> Необходимо сохранить данные перед расчетом сумм.',
                        buttons: Ext.Msg.OK,
                        icon: Ext.MessageBox.WARNING,
                        maxWidth: 1000
                    });
                }
            } else {
                Ext.Msg.show({title: 'Внимание',
                msg: 'Не найден ' + grid,
                buttons: Ext.Msg.OK,
                icon: Ext.MessageBox.WARNING,
                maxWidth: 1000
                });
            }
            
            return false;
        },
        
        // Маска для закачки на документ
        PumpMask: new Ext.LoadMask(Ext.getBody(), { msg: 'Пожалуйста подождите идет закачка...' })
    };