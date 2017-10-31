Ext.ux.CurrencyField = Ext.extend(Ext.util.Observable, {
    constructor: function(config){
        Ext.apply(this, config);
        this.events = {};
        Ext.ux.CurrencyField.superclass.constructor.call(this, config);
    },

    init : function(f){
        f.setValue = this.setValue;
        f.parseValue = this.parseValue;
        f.getErrors = this.getErrors;
        f.initEvents = this.initEvents;
    },

	initEvents : function() {
        var allowed = this.baseChars + '';
        if (this.allowDecimals) {
            allowed += this.decimalSeparator;
        }
        if (this.allowNegative) {
            allowed += '-';
        }
		allowed += ' ';
        allowed = Ext.escapeRe(allowed);
        this.maskRe = new RegExp('[' + allowed + ']');
        if (this.autoStripChars) {
            this.stripCharsRe = new RegExp('[^' + allowed + ']', 'gi');
        }
        
        Ext.form.NumberField.superclass.initEvents.call(this);
    },

	getErrors: function(value) {
        var errors = Ext.form.NumberField.superclass.getErrors.apply(this, arguments);
        
        value = Ext.isDefined(value) ? value : this.processValue(this.getRawValue());
        
        if (value.length < 1) { // if it's blank and textfield didn't flag it then it's valid
             return errors;
        }
        
        value = String(value).replace(this.decimalSeparator, ".");
        value = value.replace(' ', '').replace(' ', '').replace(' ', '').replace(' ', '');
        
        if(isNaN(value)){
            errors.push(String.format(this.nanText, value));
        }
        
        var num = this.parseValue(value);
        
        if (num < this.minValue) {
            errors.push(String.format(this.minText, this.minValue));
        }
        
        if (num > this.maxValue) {
            errors.push(String.format(this.maxText, this.maxValue));
        }
        
        return errors;
    },
	
	setValue : function(v) {
	    var capacity = 0;
	    if (this.decimalPrecision) {
	        capacity = this.decimalPrecision - 1;
	    }
	    format = '0,0.' + String.leftPad('', capacity, '0');

	    v = this.fixPrecision(v);
        v = Ext.isNumber(v) ? v : parseFloat(String(v).replace(this.decimalSeparator, "."));
        v = isNaN(v) ? '' : String(v).replace(".", this.decimalSeparator);
		v = v.replace(this.decimalSeparator, '.');
		v = Ext.util.Format.number(v, format + '/i');
        return Ext.form.NumberField.superclass.setValue.call(this, v);
    },
	parseValue : function(value) {
        value = value.replace(' ', '').replace(' ', '').replace(' ', '').replace(' ', '');
		value = parseFloat(String(value).replace(this.decimalSeparator, "."));
        return isNaN(value) ? '' : value;
    }
});
Ext.preg('currencyfield', Ext.ux.CurrencyField);