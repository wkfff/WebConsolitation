function beforeEditCell(e) {
    if (e.record.get('RefStatusData') != 1) {
        return false;
    } else if (e.field == 'PriorValue' || e.field == 'CurrentValue') {
        return !e.record.get('ReadonlyCurrent');
    } else if (e.field == 'Prognoz1' || e.field == 'Prognoz2' || e.field == 'Prognoz3') {
        return !e.record.get('ReadonlyPrognoz');
    }
    return true;
}

function getFormatter(scale, okei) {
    if (okei == 'ДА/НЕТ') {
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
        
        return Ext.util.Format.numberRenderer(format + '/i');
    }
}

function columnRenderCurrent() {
    return function (v, p, r) {
        var localf = getFormatter(r.get('Capacity'), r.get('OKEI'));
        var ReadonlyCurrent = r.get('ReadonlyCurrent');
        if (!ReadonlyCurrent) {
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

function columnRenderPrognoz() {
    return function (v, p, r) {
        var localf = getFormatter(r.get('Capacity'), r.get('OKEI'));
        var readonlyPrognoz = r.get('ReadonlyPrognoz');
        if (!readonlyPrognoz) {
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