ViewPersistence.isDirty = function () {
    return false;
}

ViewPersistence.refresh = function () {
    targetMarkStore.reload()
}

ShowDifferenceWarning = function () {
    var warningsFound = false;
    for (i = 0; i < targetFactsStore.getCount(); i++) {
        warningsFound = warningsFound || (targetFactsStore.getAt(i).get('HasWarning') != '');
    }
    if (warningsFound) {
        Ext.Msg.show({
            title: 'Внимание!',
            msg: 'В некоторых МО есть устаревшие значения показателя!',
            buttons: Ext.Msg.OK,
            animEl: 'targetFactsGrid',
            icon: Ext.MessageBox.EXCLAMATION
        });
    }
}

FactsBackToEditHandler = function () {
    var s = targetFactsGrid.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        s[i].set('RefStatusData', 1);
    }
    targetFactsGrid.save();
}

FactsApproveHandler = function () {
    var s = targetFactsGrid.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        if (s[i].get('RefStatusData') < 3) {
            s[i].set('RefStatusData', 3);
        }
    }
    targetFactsStore.save();
}

TooltipShowHandler = function () {
    var rowIndex = targetFactsGrid.view.findRowIndex(this.triggerElement);
    var cellIndex = targetFactsGrid.view.findCellIndex(this.triggerElement);
    var record = targetFactsStore.getAt(rowIndex);

    if (!record) {
        this.hide();
        return false;
    }

    var fieldName = targetFactsGrid.getColumnModel().getDataIndex(cellIndex);
    var data = record.get(fieldName);

    if (cellIndex == 1) {
        if (data == 1) { this.body.dom.innerHTML = 'На редактировании'; }
        else if (data == 2) { this.body.dom.innerHTML = 'На рассмотрении'; }
        else if (data == 3) { this.body.dom.innerHTML = 'Утвержден'; }
        else if (data == 4) { this.body.dom.innerHTML = 'Принят'; }
        else {
            this.body.dom.innerHTML = '';
            this.hide();
            return false; 
        }
    }
    else if ((cellIndex == 2) && (data != '')) {
        this.body.dom.innerHTML = 'Утвержденное и рассчитанное значения не совпадают';
    }
    else {
        this.body.dom.innerHTML = '';
        this.hide();
        return false;
    }
}

RefStatusDataRenderer = function (value, p, record) {    
    if (value != null) {
        var status = record.get('RefStatusData');
        if (status == 1) { return '<img class="x-panel-inline-icon icon-useredit" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">'; }
        else if (status == 2) { return '<img class="x-panel-inline-icon icon-usermagnify" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">'; }
        else if (status == 3) { return '<img class="x-panel-inline-icon icon-usertick" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">'; }
        else if (status == 4) { return '<img class="x-panel-inline-icon icon-accept" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">'; }
    } 
}

HasWarningRenderer = function (value, p, record) {
    if (record.get('HasWarning') != '') {
        return '<img class="x-panel-inline-icon icon-exclamation" src="/extjs/resources/images/default/s-gif/ext.axd" alt="!">';
    }
}

TargetFactPriorRenderer = function (value, p, record) {
    return ValueRenderer(value, targetMarkPrecision.getValue(), record.get('HasWarnPrior') != '');
}

TargetFactCurrentRenderer = function (value, p, record) {
    return ValueRenderer(value, targetMarkPrecision.getValue(), record.get('HasWarnCurrent') != '');
}

ValueRenderer = function (value, precision, redHighlight) {
    var result = FormatUsing(value, precision);
    if (redHighlight) {
        return '<span class="red-text-cell">' + result + '</span>'
    }
    return result;
}

SrcFactsNameRenderer = function (value, p, record) {
    var indent = record.get('Level') * 30;
    return '<div style="display:inline-block; padding-left:' + indent + 'px">' + value + '</span>'
}

SrcFactValueRenderer = function (value, p, record) {
    return ValueRenderer(value, record.get('Precision'), false);
}

TargetMarkStoreLoadHandler = function () {
    targetMarkForm.getForm().loadRecord(targetMarkStore.getAt(0));
    targetMarkDescriptionLabel.setText(targetMarkDescription.getValue(), true);
    targetMarkCalcMarkLabel.setText(targetMarkCalcMark.getValue(), true);
    targetMarkMeasureUnitLabel.setText(targetMarkMeasureUnit.getValue(), true);

    targetFactsStore.reload();
    targetFactsGrid.getColumnModel().getColumnById('PriorCalc').header = (currentYear.getValue() - 1) + ' рассчитано';
    targetFactsGrid.getColumnModel().getColumnById('PriorApproved').header = (currentYear.getValue() - 1) + ' утверждено';
    targetFactsGrid.getColumnModel().getColumnById('CurrentCalc').header = currentYear.getValue() + ' рассчитано';
    targetFactsGrid.getColumnModel().getColumnById('CurrentApproved').header = currentYear.getValue() + ' утверждено';

    sourceFactsGrid.getColumnModel().getColumnById('srcFactsColPrior').header = (currentYear.getValue() - 1);
    sourceFactsGrid.getColumnModel().getColumnById('srcFactsColCurrent').header = currentYear.getValue();
    sourceFactsGrid.setDisabled(true);
}

function FormatUsing(value, precision) {
    var usePrecision = 0;
    if (precision) {
        usePrecision = precision;
    }
    formatStr = '0.000,' + String.leftPad('', usePrecision, '0');
    result = Ext.util.Format.number(value, formatStr + '/i');
    return result;
}