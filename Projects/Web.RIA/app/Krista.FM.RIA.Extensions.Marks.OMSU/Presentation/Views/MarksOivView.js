ViewPersistence.isDirty = function () {
    return this.dsMarks.isDirty();
};

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

function toggleFilter(button, state) {
    if (state) {
        gpMarks.getColumnModel().setHidden(3, false);
        dsMarks.load();
    } else {
        gpMarks.getColumnModel().setHidden(3, true);
    }
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
