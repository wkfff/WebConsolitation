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
    labelCodeRep.update('<b>Номер в докладе</b>:');
    labelDescription.update('<b>Что показывает</b>:');
    labelCalcMark.update('<b>Расчет</b>:');
    labelInfoSource.update('<b>Информационная обеспеченность</b>:');
    labelSymbol.update('<b>Обозначение</b>:');
    labelFormula.update('<b>Формула</b>:');
};

function toggleFilter(button, state) {
    if (state) {
        gpMarks.getColumnModel().setHidden(3, false);
        dsMarks.load();
    } else {
        gpMarks.getColumnModel().setHidden(3, true);
    }
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

function acceptButtonClick() {
    var s = gpMarks.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        if (s[i].get('RefStatusData') == 2) {
            s[i].set('RefStatusData', 3);
        }
    }

    saveData();
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

function rejectToTestButtonClick() {
    var s = gpMarks.getSelectionModel().getSelections();
    for (i = 0; i < s.length; i++) {
        if (s[i].get('RefStatusData') == 3) {
            s[i].set('RefStatusData', 2);
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