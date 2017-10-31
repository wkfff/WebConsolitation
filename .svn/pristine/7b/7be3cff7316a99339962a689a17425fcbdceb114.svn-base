ViewPersistence.refresh = function () {
    var comboBox = this.marksCombo;
    comboBox.clear();
    comboBox.store.addListener(
        'load',
        function () {
            if (comboBox.store.data.length > 0) {
                comboBox.selectByIndex(0);
            } else {
                comboBox.clear();
            }
            comboBox.fireEvent('select');
        },
        this,
        { single: true }
    );
    comboBox.store.reload();
    comboBox.lastQuery = '';
};

ViewPersistence.isDirty = function () {
    return this.dsMarks.isDirty();
};

function clearMarkDescription() {
    labelOKEI.update('<b>Единицы измерения</b>:');
    labelCompRep.update('<b>Наименование раздела</b>:');
    labelCodeRep.update('<b>Номер в докладе</b>:');
    labelSymbol.update('<b>Обозначение</b>:');
    labelFormula.update('<b>Формула</b>:');
};

function toggleFilter(button, state) {
    dsMarks.load();
};

function getStateFilter() {
    var filter = [true, true];

    filter[0] = filterEdit.pressed;
    filter[1] = filterMagnify.pressed;

    return filter;
};
function beforeEditCell(e) {
    if (e.record.get('RefStatusData') == 2) {
        return !e.record.get('Readonly');
    }
    return false;
};

function saveData() {
    dsMarks.save();
};

function rejectButtonClick() {
    var s = gpMarks.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        if (s[i].get('RefStatusData') == 2 || s[i].get('RefStatusData') == 3) {
            s[i].set('RefStatusData', 1);
        }
    }

    saveData();
};

columnRenderNote = function() {
    return function(v, p, r) {
        if (!r.get('Readonly')) {
            if (v == null || v === '') return v;
            return v;
        } else {
            p.css = 'disable-cell';
            if (v != null) {
                return '<span class="disable-cell">' + v + '</span>';
            }
            return v;
        }
    }
};

function showCellTip() {
    var rowIndex = gpMarks.view.findRowIndex(this.triggerElement),
        cellIndex = gpMarks.view.findCellIndex(this.triggerElement),
        record = dsMarks.getAt(rowIndex),
        fieldName = gpMarks.getColumnModel().getDataIndex(cellIndex),
        data = record.get(fieldName);

    if (cellIndex == 1) {
        if (data == 1) {
            data = 'На редактировании';
        } else if (data == 2) {
            data = 'На рассмотрении';
        } else if (data == 3) {
            data = 'Утвержден';
        } else if (data == 4) {
            data = 'Принят';
        }

        this.body.dom.innerHTML = data;
    }
    else {
        this.hide();
        return false;
    }
};

columnRenderStatus = function (v, p, r) {
    var s;
    if (v != null) {
        var st = r.get('RefStatusData');
        if (st == 1) {
            s = '<img class="x-panel-inline-icon icon-useredit" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">';
        } else if (st == 2) {
            s = '<img class="x-panel-inline-icon icon-usermagnify" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">';
        } else if (st == 3) {
            s = '<img class="x-panel-inline-icon icon-usertick" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">';
        } else if (st == 4) {
            s = '<img class="x-panel-inline-icon icon-accept" src="/extjs/resources/images/default/s-gif/ext.axd" alt="">';
        }
    }
    return s;
};

function getFormatter(scale, symbol) {
    if (symbol == 'ДА/НЕТ') {
        return function (val) {
            if (val == null || val === '') return val;
            return val == 0 ? 'Нет' : 'Да';
        }
    } else {
        var format = '0.000,';
        var capacity = 0;
        if (scale) {
            capacity = scale;
        }
        format = '0.000,' + String.leftPad('', capacity, '0');
        if (symbol == 'ПРОЦ') {
            format = format + '%';
        }
        return Ext.util.Format.numberRenderer(format + '/i');
    }
}

columnRenderValue = function () {
    return function (v, p, r) {
        var localf = getFormatter(r.get('Capacity'), r.get('Symbol'));
        if (!r.get('Readonly')) {
            if (v == null || v === '') return v;
            return localf(v);
        } else {
            p.css = 'disable-cell';
            if (v != null) {
                return '<span class="disable-cell">' + localf(v) + '</span>';
            }
            return v;
        }
    }
};