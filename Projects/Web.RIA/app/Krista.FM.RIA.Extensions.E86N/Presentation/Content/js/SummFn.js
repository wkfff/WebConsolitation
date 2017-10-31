function CalculateSumm(store, field, summfld) {
    eval(summfld).setValue(Ext.util.Format.number(eval(store).sum(field), '0,000.00') + ' руб.');
};
