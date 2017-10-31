Ext.override(Ext.form.DateField, {
    setValue: function (date) {
        var value = this.formatDate(this.parseDate(date));
        if (value == null && date.length == 1 && date[0] >= 0 && date[0] <= 9)
            value = date;
        return Ext.form.DateField.superclass.setValue.call(this, value);
    }
});