Ext.ns('E86n.View.PfhdView');
E86n.View.PfhdView =
    {
        SetReadOnlyDoc: function (readOnly, recId) {
            window.btnPfhdSave.setDisabled(readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('frmPfhd1', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('frmPfhd2', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('frmPfhd3', readOnly);

            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'gpCapFunds');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'gpRealAssetFunds');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'gpOtherGrantFunds');

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        },

        SavePFHD: function () {
            window.frmPfhd1.getForm().updateRecord(window.dsPfhd.getAt(0));
            if (window.dsPfhd.getAt(0).get('Plan3Year') > 0) {
                window.frmPfhd2.getForm().updateRecord(window.dsPfhd.getAt(1));
                window.frmPfhd3.getForm().updateRecord(window.dsPfhd.getAt(2));
            }
            window.dsPfhd.save();
        },
        
        LoadPFHD: function (store) {
            window.frmPfhd1.setVisible(1);
            window.frmPfhd2.setVisible(0);
            window.frmPfhd3.setVisible(0);

            window.frmPfhd1.getForm().loadRecord(store.getAt(0));
            if (store.getAt(0).get('Plan3Year') > 0)
            {
                window.frmPfhd2.getForm().loadRecord(store.getAt(1));
                window.frmPfhd3.getForm().loadRecord(store.getAt(2));
                window.frmPfhd2.setVisible(1);
                window.frmPfhd3.setVisible(1);
            };
            
            window.Label1.setText('Показатели финансового состояния, на 1 янв. ' + store.getAt(0).data.RefYearFormID + ' года');
            window.Label2.setText(store.getAt(0).data.RefYearFormID + 1 + ' года');
            window.Label3.setText(store.getAt(0).data.RefYearFormID + 2 + ' года');
            window.Label4.setText('Показатели поступлений и выплат, ' + store.getAt(0).data.RefYearFormID + ' год');
            window.Label5.setText(store.getAt(0).data.RefYearFormID + 1 + ' год');
            window.Label6.setText(store.getAt(0).data.RefYearFormID + 2 + ' год');
        },
        
        getOtherGrantFilter: function (docId) {
            //стор параметров документа
            var storeId = 'dsParamDoc' + docId;
            //год документа
            var yearDoc = eval(storeId).getAt(0).data.RefYearFormID;
            //текущий год
            var year = new Date().getFullYear();

            if (yearDoc == year) {
                return '((OPENDATE IS NULL AND CLOSEDATE IS NULL) OR (OPENDATE IS NULL AND CLOSEDATE >= GETDATE()) OR ' +
                  '(OPENDATE <= GETDATE() AND CLOSEDATE IS NULL) OR (GETDATE() BETWEEN OPENDATE AND CLOSEDATE))';
            } else {
                //если текущий год не равен году документа то даты проверяем относительно года формирования документа 
                //и соответственно смотрим только года а не даты целиком
                return String.format('((OPENDATE IS NULL AND CLOSEDATE IS NULL) OR (OPENDATE IS NULL AND YEAR(CLOSEDATE) >= {0}) OR ' +
                  '(YEAR(OPENDATE) <= {0} AND CLOSEDATE IS NULL) OR ({0} BETWEEN YEAR(OPENDATE) AND YEAR(CLOSEDATE)))', yearDoc);
            }
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
        }
    };