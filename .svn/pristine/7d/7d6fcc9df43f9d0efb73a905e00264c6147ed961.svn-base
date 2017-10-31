function showCellTip() {
    var rowIndex = gpMarks.view.findRowIndex(this.triggerElement),
        cellIndex = gpMarks.view.findCellIndex(this.triggerElement),
        record = dsMarks.getAt(rowIndex),
        fieldName = gpMarks.getColumnModel().getDataIndex(cellIndex),
        data = record.get(fieldName);

    if (cellIndex == 2) {
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
    else if (cellIndex >= 6 && cellIndex <= 9 && record.get('OKEI') == 'ДА/НЕТ') {
        this.body.dom.innerHTML = '0-Нет, 1-Да';
    }
    else {
        this.hide();
        return false;
    }
};

function beforeEditCell(e) {
    if (e.record.get('RefStatusData') != 1) {
        return false;
    } else if (e.field == 'Fact') {
        return (e.record.get('Formula') == null || e.record.get('Formula') == '');
    } else if (e.field == 'Forecast' || e.field == 'Forecast2' || e.field == 'Forecast3') {
        return (e.record.get('Formula') == null || e.record.get('Formula') == '');
    }
    return true;
};

function getFormatter(scale, okei) {
    if (okei == 'ДА/НЕТ') {
        return function(val) {
            if (val == null || val === '') return val;
            return val == 0 ? 'Нет' : 'Да';
        };
    } else {
        var format = '0.000,';
        var capacity = 0;
        if (scale) {
            capacity = scale;
        }
        format = '0.000,' + String.leftPad('', capacity, '0');
        if (okei == 'ПРОЦ') {
            format = format + '%';
        }
        return Ext.util.Format.numberRenderer(format + '/i');
    }
};

function columnRenderMO() {
    return function(v, p, r) {
        var localf = getFormatter(r.get('Capacity'), r.get('OKEI'));

        if (r.get('Formula') == '' || r.get('Formula') == null) {
            return localf(v);
        } else {
            p.css = 'disable-cell';
            if (v != null) {
                return '<span class="disable-cell">' + localf(v) + '</span>';
            }
            return v;
        }
    };
};

function columnRenderPrognoz() {
    return function(v, p, r) {
        var localf = getFormatter(r.get('Capacity'), r.get('OKEI'));

        if (r.get('Formula') == '' || r.get('Formula') == null) {
            if (v == null || v === '') return v;
            return localf(v);
        } else {
            p.css = 'disable-cell';
            if (v != null) {
                return '<span class="disable-cell">' + localf(v) + '</span>';
            }
            return v;
        }
    };
};

function columnRenderStatus(v, p, r) {
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
