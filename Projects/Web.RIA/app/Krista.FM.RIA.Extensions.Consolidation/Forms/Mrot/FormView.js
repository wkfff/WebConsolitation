function isDirty() {
    return store.isDirty() || dsJobTitle.isDirty();
}

function saveReport() {
    store.save();
    dsJobTitle.save();
}

function changeState(newStatus) {
    if (isDirty()) {
        Ext.Msg.alert('Предупреждение', 'Необходимо сохранить внесенные в отчет изменения перед изменением состояния задачи.');
        return;
    }
    
    if (!validateForm(store)) {
        return;
    }

    Ext.net.DirectMethod.request({
        url: '/ConsTask/Save',
        cleanRequest: true,
        params: {
            taskId: taskId.getValue(),
            refStatusId: newStatus
        },
        success: successSaveHandler,
        failure: failureSaveHandler
    });
};

function successSaveHandler(response, result) {
    if (result.extraParams != undefined && result.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: result.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });
    } else if (response.extraParams != undefined && response.extraParams.msg != undefined) {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: response.extraParams.msg,
            title: 'Уведомление',
            hideDelay: 2500
        });

    } else {
        Ext.net.Notification.show({
            iconCls: 'icon-information',
            html: 'Ок',
            title: 'Уведомление',
            hideDelay: 2500
        });
    }
};

function failureSaveHandler(response, result) {
    if (result.extraParams != undefined && result.extraParams.responseText != undefined) {
        Ext.Msg.alert('Ошибка', result.extraParams.responseText);
    } else {
        var responseParams = Ext.decode(result.responseText);
        if (responseParams != undefined && responseParams.extraParams != undefined && responseParams.extraParams.responseText != undefined) {
            Ext.Msg.alert('Ошибка', responseParams.extraParams.responseText);
        } else {
            Ext.Msg.alert('Ошибка', 'Server failed');
        }
    }
};

function onTabCloseEventHandler(currentTab) {
    if (currentTab.forceClose != undefined && currentTab.forceClose) {
        return true;
    }
    var tabBody = currentTab.getBody();
    if (tabBody.isDirty() || tabBody.isDirty()) {
        Ext.Msg.show({
            title: 'Внимание',
            msg: 'При закрытии все несохраненные изменения будут потеряны. Закрыть?',
            buttons: Ext.Msg.YESNO,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING,
            fn: function (buttonId) {
                if (buttonId == "yes") {
                    currentTab.forceClose = true;
                    currentTab.ownerCt.closeTab(currentTab);
                }
            }
        });

        return false;
    }
    else {
        return true;
    }
};

function exceptionHandler(store, type, action, o, response, e) {
    if (!response.success) {
        Ext.Msg.show({
            title: 'Предупреждение',
            msg: response.raw.message,
            buttons: Ext.Msg.OK,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING
        });
    }
};

function beforeEditCell(e) {
    return e.record.get('IsEditable')
        && ((e.record.get('Code') < 50 && (e.field == 'OrgType2' || e.field == 'OrgType3'))
            || (e.record.get('Code') > 50 && e.field == 'OrgType1'));
};

function columnOrgType1Render() {
    var fd = Ext.util.Format.numberRenderer('0.000' + '/i');
    var ff = Ext.util.Format.numberRenderer('0.000,0' + '/i');
    return function (v, p, r) {
        var value = v;
        var rowCode = r.get('Code');
        if (r.get('IsEditable') && rowCode < 50) {
            value = r.get('OrgType2') + r.get('OrgType3');
        }
        if (value != null) {
            if (rowCode % 10 == 1 || rowCode > 50) {
                return fd(value);
            } else {
                return ff(value);
            }
        }
        return value;
    }
};

function columnOrgType2Render() {
    var fd = Ext.util.Format.numberRenderer('0.000' + '/i');
    var ff = Ext.util.Format.numberRenderer('0.000,0' + '/i');
    return function(v, p, r) {
        var value = v;
        if (value != null) {
            if (r.get('Code') % 10 == 1) {
                return fd(value);
            } else {
                return ff(value);
            }
        }
        return value;
    };
};

function columnNameRender() {
    return function(v, p, r) {
        if (r.get('IsEditable')) {
            return '<span class="sub-cell">' + v + '</span>';
        } else {
            return '<span class="parent-cell">' + v + '</span>';
        }
    };
};

/* --- Валидация данных --- */

function validateForm(store, params) {
    var valid = validateStore(store, {});
    if (!valid) {
        Ext.Msg.show({
            title: 'Предупреждение',
            msg: 'Невыполнены контрольные соотношения.',
            buttons: Ext.Msg.OK,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING
        });
    }

    if (dsJobTitle.getCount() == 0) {
        Ext.Msg.show({
            title: 'Предупреждение',
            msg: 'Должен быть заполнен хотя бы один исполнитель.',
            buttons: Ext.Msg.OK,
            animEl: 'viewportMain',
            icon: Ext.MessageBox.WARNING
        });

        return false;
    }
    
    return valid;
};

function validateStore(store, params) {
    var isValid = true;
    for (i = 0; i < CR.length - 1; i++) {
        var row = store.indexOfId(CR[i].row);
        var col = grid.getColumnModel().getIndexById(CR[i].col);
        var cell = grid.getView().getCell(row, col);

        if (CR[i].fn(store)) {
            Ext.fly(cell).addClass('exclamation-cell');
            isValid = false;
        } else {
            Ext.fly(cell).removeClass('exclamation-cell');
        }
    }
    return isValid;
}

function afterEditCell(e) {
    validateStore(store, {});
};

function cell(store, row, col) {
    var v = store.getById(row).get(col);
    return v == null || v == '' ? 0 : v;
}

var CR = [
// -----------------------
    {
    row: 51,
    col: 'OrgType1',
    fn: function (s) { return !(((cell(s, 11, 'OrgType2') + cell(s, 11, 'OrgType3')) != 0 && (cell(s, 51, 'OrgType1') + cell(s, 52, 'OrgType1')) != 0) || ((cell(s, 11, 'OrgType2') + cell(s, 11, 'OrgType3')) == 0 && (cell(s, 51, 'OrgType1') + cell(s, 52, 'OrgType1')) == 0)); },
    msg: 'Не указано количество работодателей внебюджетного сектора.'
}, {
    row: 52,
    col: 'OrgType1',
    fn: function (s) { return !((cell(s, 52, 'OrgType1') >= cell(s, 51, 'OrgType1')) || cell(s, 52, 'OrgType1') == 0); },
    msg: 'Уровень средней заработной платы не может быть меньше уровня минимальной заработной платы.'
},
// -----------------------
    {
    row: 21,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 21, 'OrgType2') > cell(s, 11, 'OrgType2') },
    msg: 'Количество работодателей участников трехстороннего соглашения по крупным и средним организациям, не может быть больше, чем работодателей внебюджетного сектора.'
}, {
    row: 22,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 22, 'OrgType2') > cell(s, 12, 'OrgType2') },
    msg: 'Численность работающих в крупных и средних организациях, участников трехстороннего соглашения, не может быть больше численности работающих во внебюджетном секторе.'
}, {
    row: 31,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 31, 'OrgType2') > cell(s, 21, 'OrgType2') },
    msg: 'Количество работодателей крупных и средних организаций, выполняющих условия трехстороннего соглашения в части установленного минимального размера заработной платы, не может быть больше количества работодателей участников трехстороннего соглашения.'
}, {
    row: 32,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 32, 'OrgType2') > cell(s, 22, 'OrgType2') },
    msg: 'Численность работающих в крупных и средних организациях, выполняющих условия трехстороннего соглашения в части установленного минимального размера заработной платы, не может быть больше общей численности работающих в организациях участников трехстороннего соглашения.'
}, {
    row: 41,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 41, 'OrgType2') > cell(s, 21, 'OrgType2') },
    msg: 'Количество работодателей крупных и средних организаций, выполняющих условия трехстороннего соглашения в части средней заработной платы, не может быть больше общего количества работодателей участников трехстороннего соглашения.'
}, {
    row: 42,
    col: 'OrgType2',
    fn: function (s) { return cell(s, 42, 'OrgType2') > cell(s, 22, 'OrgType2') },
    msg: 'Численность работающих в крупных и средних организациях, выполняющих условия трехстороннего соглашения в части средней заработной платы, не может быть больше общей численности работающих в организациях участников трехстороннего соглашения.'
},
// -----------------------
    {
    row: 21,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 21, 'OrgType3') > cell(s, 11, 'OrgType3') },
    msg: 'Количество работодателей участников трехстороннего соглашения по субъектам малого предпринимательства, не может быть больше, чем работодателей внебюджетного сектора.'
}, {
    row: 22,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 22, 'OrgType3') > cell(s, 12, 'OrgType3') },
    msg: 'Численность работающих у субъектов малого предпринимательства, участников трехстороннего соглашения, не может быть больше численности работающих во внебюджетном секторе.'
}, {
    row: 31,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 31, 'OrgType3') > cell(s, 21, 'OrgType3') },
    msg: 'Количество работодателей субъектов малого предпринимательства, выполняющих условия трехстороннего соглашения в части установленного минимального размера заработной платы, не может быть больше количества работодателей участников трехстороннего соглашения.'
}, {
    row: 32,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 32, 'OrgType3') > cell(s, 22, 'OrgType3') },
    msg: 'Численность работающих у субъектов малого предпринимательства, выполняющих условия трехстороннего соглашения в части установленного минимального размера заработной платы, не может быть больше общей численности работающих в организациях участников трехстороннего соглашения.'
}, {
    row: 41,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 41, 'OrgType3') > cell(s, 21, 'OrgType3') },
    msg: 'Количество работодателей субъектов малого предпринимательства, выполняющих условия трехстороннего соглашения в части средней заработной платы, не может быть больше общего количества работодателей участников трехстороннего соглашения.'
}, {
    row: 42,
    col: 'OrgType3',
    fn: function (s) { return cell(s, 42, 'OrgType3') > cell(s, 22, 'OrgType3') },
    msg: 'Численность работающих у субъектов малого предпринимательства, выполняющих условия трехстороннего соглашения в части средней заработной платы, не может быть больше общей численности работающих в организациях участников трехстороннего соглашения.'
}
];

function showCellErrorTip() {
    var rowIndex = grid.getView().findRowIndex(this.triggerElement),
    cellIndex = grid.getView().findCellIndex(this.triggerElement),
    row = store.getAt(rowIndex).id,
    col = grid.getColumnModel().getDataIndex(cellIndex),
    data = '';

    for (i = 0; i < CR.length - 1; i++) {
        if (CR[i].row == row && CR[i].col == col) {
            data = CR[i].msg;
            break;
        }
    }

    if (data) {
        this.body.dom.innerHTML = data;
    }
    else {
        this.hide();
        return false;
    }
}