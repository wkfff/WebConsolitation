Ext.ns('E86n.View.SmetaView');
E86n.View.SmetaView =
    {
        //формирование динамического фильтра для Бюджета
        getFlt: function() {
            var val = window.gpSmetaDetail.getSelectionModel().getSelected().data.RefBudgetID;
            if (!val) return '(REFTYPEKBK=10)';
            else return '(REFTYPEKBK=10) and (REFBUDGET=' + val + ')';
        },

        docStoreLoad: function(store) {
            var cm = window.gpSmetaDetail.getColumnModel();
            var year = store.getAt(0).data.RefYearFormID;
            var year1 = year + 1;
            var year2 = year + 2;

            var col = cm.getColumnById('Funds');
            col.header = year + 'год, руб';
            window.Summ1Label.setText('Сумма ' + year + 'год');

            col = cm.getColumnById('FundsOneYear');
            col.header = year1 + 'год, руб';
            window.Summ2Label.setText('Сумма ' + year1 + 'год');

            col = cm.getColumnById('FundsTwoYear');
            col.header = year2 + 'год, руб';
            window.Summ3Label.setText('Сумма ' + year2 + 'год');


            var col1Idx = cm.findColumnIndex('Funds');
            var col2Idx = cm.findColumnIndex('FundsOneYear');
            var col3Idx = cm.findColumnIndex('FundsTwoYear');
            cm.setHidden(col1Idx, false);
            cm.setHidden(col2Idx, true);
            cm.setHidden(col3Idx, true);

            window.Summ1.setVisible(true);
            window.Summ2.setVisible(false);
            window.Summ3.setVisible(false);

            if (store.getAt(0).data.PlanThreeYear == 1) {
                cm.setHidden(col2Idx, false);
                cm.setHidden(col3Idx, false);

                window.Summ2.setVisible(true);
                window.Summ3.setVisible(true);
            }
        },

        SetReadOnlySmeta: function (readOnly, recId) {
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'gpSmetaDetail');
            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
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