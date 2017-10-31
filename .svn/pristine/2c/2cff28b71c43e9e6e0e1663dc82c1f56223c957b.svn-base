Ext.ns('E86n.View.Pfhd2017View');

function checkVal(value) {
    if (value) {
        return value;
    }
    return 0;
};

E86n.View.Pfhd2017View =
    {
        // табы по которым ничего не загружаем(исключаемые)
        // todo вынести в общий функционал
        excludeTabsOnReload: ['PlanPaymentIndexTabPanel', 'ActionGrantTabPanel'],

        // включение и отключение клиентской валидации форм
        clientValidationEnabled: true,

        // todo вынести в общий функционал
        reloadDetail: function (item) {
            if (item) {
                var activeTab = item.getActiveTab();
                if (!window.E86n.View.Pfhd2017View.excludeTabsOnReload.some(x => x === activeTab.id)) {
                    var storeId = activeTab.id + 'Store';
                    eval(storeId).reload();
                }
            }
        },

        SetReadOnlyDoc: function (readOnly, recId) {
            window.E86n.View.Pfhd2017View.clientValidationEnabled = !readOnly;

            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('FinancialIndexForm', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('TemporaryResourcesForm', readOnly);
            window.E86n.Control.StateToolBar.SetReadOnlyFormPanel('ReferenceForm', readOnly);
           
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'ExpensePaymentGrid');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PlanPaymentIndexGrid0');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PlanPaymentIndexGrid1');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'PlanPaymentIndexGrid2');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'CapitalConstructionFundsGrid');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'RealAssetFundsGrid');
            window.E86n.Control.StateToolBar.SetReadOnlyGrid(readOnly, 'OtherGrantFundsGrid');

            window.E86n.Control.StateToolBar.ReadOnlyDocs(readOnly, recId);
        },

        // todo загрузка формы по изменению стора может быть вынесено в общий функционал
        StoreDataChanged: function (store, formId) {
            var record = store.getAt(0) || {};
            var cmp = Ext.getCmp(formId);
            if (cmp) {
                cmp.getForm().loadRecord(record);
            }
        },

        // клиентская валидация по умолчанию формы
        ClientValidation: function (item, valid) {
            if (window.E86n.View.Pfhd2017View.clientValidationEnabled) {
                var cmp = Ext.getCmp(item.id + 'SaveBtn');
                if (cmp) {
                    cmp.setDisabled(!valid);
                    cmp.setTooltip(valid ? 'Сохранить изменения' : 'В форме не все обязательные поля заполнены. Либо имеют некорректное значение.');
                }
            }
        },

        FinancialIndexFormValidation: function () {
            var msg = '';
            var sum = checkVal(window.RealAssets.getValue())
                        + checkVal(window.HighValuePersonalAssets.getValue());
                                
            if (checkVal(window.NonFinancialAssets.getValue()) < parseFloat(parseFloat(sum).toFixed(2))) {
                msg += '"Нефинансовые активы, всего" должна быть больше или равны сумме' +
                    ' "Недвижимое имущество. Всего"' +
                    ' "Особо ценное движимое имущество. Всего"';
            }

            sum = checkVal(window.MoneyInstitutions.getValue())
                    + checkVal(window.OtherFinancialInstruments.getValue())
                    + checkVal(window.DebitIncome.getValue())
                    + checkVal(window.DebitExpense.getValue());

            if (checkVal(window.FinancialAssets.getValue()) < parseFloat(parseFloat(sum).toFixed(2))) {
                msg += '"Финансовые активы, всего" должна быть больше или равны сумме' +
                    ' "Денежные средства учреждения. Всего"' +
                    ' "Иные финансовые инструменты"' +
                    ' "Дебиторская задолженность по доходам"' +
                    ' "Дебиторская задолженность по расходам"';
            }

            sum = checkVal(window.Debentures.getValue())
                + checkVal(window.AccountsPayable.getValue());

            if (checkVal(window.FinancialCircumstanc.getValue()) < parseFloat(parseFloat(sum).toFixed(2))) {
                msg += '"Обязательства, всего" должна быть больше или равны сумме' +
                       ' "Долговые обязательства" "Кредиторская задолженность"';
            }

            return msg;
        },

        getOtherGrantFilter: function (docId) {
            //стор параметров документа
            var storeId = 'dsParamDoc' + docId;
            //год документа
            var yearDoc = eval(storeId).getAt(0).data.RefYearFormID;
            //текущий год
            var year = new Date().getFullYear();

            if (yearDoc === year) {
                return '((OPENDATE IS NULL AND CLOSEDATE IS NULL) OR (OPENDATE IS NULL AND CLOSEDATE >= GETDATE()) OR ' +
                  '(OPENDATE <= GETDATE() AND CLOSEDATE IS NULL) OR (GETDATE() BETWEEN OPENDATE AND CLOSEDATE))';
            } else {
                //если текущий год не равен году документа то даты проверяем относительно года формирования документа 
                //и соответственно смотрим только года а не даты целиком
                return String.format('((OPENDATE IS NULL AND CLOSEDATE IS NULL) OR (OPENDATE IS NULL AND YEAR(CLOSEDATE) >= {0}) OR ' +
                  '(YEAR(OPENDATE) <= {0} AND CLOSEDATE IS NULL) OR ({0} BETWEEN YEAR(OPENDATE) AND YEAR(CLOSEDATE)))', yearDoc);
            }
        },

        ExpensePaymentGridBeforeStartEdit: function (editor) {
            var lineCode = window.ExpensePaymentGrid.getSelectionModel().getSelected().get('LineCode');
            if (lineCode === '0001') {
                return false;
            }

            var fieldId = window.ExpensePaymentGrid.getColumnModel().columns[editor.col].id;

            var arr = ['1001', '2001'];
            var fields = ['TotalSumNextYear', 'TotalSumFirstPlanYear', 'TotalSumSecondPlanYear'];
            if (arr.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }
            
            if (fieldId === 'Year') {
                arr = ['1001', '1002', '1003', '1004', '1005'];
                return !(arr.some(x => lineCode === x) || lineCode === '');
            }

            return true;
        },

        // todo настройку редактируемости ячеек можно вынести в общий модуль
        PlanPaymentIndexGridBeforeStartEdit: function (editor, gridName) {
            var grid = Ext.getCmp(gridName);
            var lineCode = grid.getSelectionModel().getSelected().get('LineCode');
            var fieldId = grid.getColumnModel().columns[editor.col].id;
            var codes = ['100', '180', '200', '260', '300', '500', '600'];
            var fields = ['Kbk'];
            if (codes.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }

            codes = ['110', '130', '140', '150', '160', '180'];
            fields = ['FinancialProvision', 'HealthInsurance'];
            if (codes.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }

            codes = ['110', '130', '140', '150', '180'];
            fields = ['ServiceGrant'];
            if (codes.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }

            codes = ['110', '120', '130', '140', '160', '180'];
            fields = ['SubsidyOtherPurposes', 'CapitalInvestment'];
            if (codes.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }

            codes = ['150'];
            fields = ['ServiceTotal'];
            if (codes.some(x => lineCode === x) && fields.some(x => fieldId === x)) {
                return false;
            }

            return true;
        },

        // todo проверка на измененные записи в сторе можно вынести в общий модуль
        BeforeSumm: function (grid) {
            var g = Ext.getCmp(grid);
            if (g) {
                if (g.getStore().getModifiedRecords().length === 0) {
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
                Ext.Msg.show({
                    title: 'Внимание',
                    msg: 'Не найден ' + grid,
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.WARNING,
                    maxWidth: 1000
                });
            }

            return false;
        }
    };