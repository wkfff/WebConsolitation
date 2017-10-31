// Замена функции Ext.util.Format.number на функцию, которая в качестве разделителей разрядов использует пробел
Ext.util.Format.number = function (v, formatString) {
    if (!formatString || !v) {
		return v;
	}
	var thousandSeparator = ' ', 
		decimalSeparator = ',';
	var comma = '.',
		dec = decimalSeparator,
		i18n = false,
		neg = v < 0,
		hasComma,
		psplit,
		formatCleanRe = /[^\d\.]/g,
		I18NFormatCleanRe;

	val = Math.abs(v);

	if (formatString.substr(formatString.length - 2) == '/i') {
		if (!I18NFormatCleanRe) {
			I18NFormatCleanRe = new RegExp('[^\\d\\' + decimalSeparator + ']', 'g');
		}
		formatString = formatString.substr(0, formatString.length - 2);
		i18n = true;
		hasComma = formatString.indexOf(comma) != -1;
		psplit = formatString.replace(I18NFormatCleanRe, '').split(dec);
	} else {
		hasComma = formatString.indexOf(',') != -1;
		psplit = formatString.replace(formatCleanRe, '').split('.');
	}

	if (psplit.length > 2) {
	    Ext.Error.raise({
	        sourceClass: "Ext.util.Format",
	        sourceMethod: "number",
	        value: v,
	        formatString: formatString,
	        msg: "Invalid number format, should have no more than 1 decimal separator"
	    });
	}
    
    /* (!) Осторожно с toFixed:
    (0.355).toFixed(2) = 0.36
    (0.455).toFixed(2) = 0.45(!)
    (0.555).toFixed(2) = 0.56
    Поэтому округление сделаем "вручную" до вызова toFixed. */
	var decimals = psplit[1] ? psplit[1].length : 0;
	var multiplier = Math.pow(10, decimals),
	val = Math.round(val * multiplier) / multiplier;
	val = val.toFixed(decimals);

	var fnum = val.toString();

	psplit = fnum.split('.');

	if (hasComma) {
		var cnum = psplit[0],
			parr = [],
			j = cnum.length,
			m = Math.floor(j / 3),
			n = cnum.length % 3 || 3,
			i;

		for (i = 0; i < j; i += n) {
			if (i !== 0) {
				n = 3;
			}

			parr[parr.length] = cnum.substr(i, n);
			m -= 1;
		}
		fnum = parr.join(thousandSeparator);
		if (psplit[1]) {
			fnum += dec + psplit[1];
		}
	} else {
		if (psplit[1]) {
			fnum = psplit[0] + dec + psplit[1];
		}
	}

	return (neg ? '-' : '') + formatString.replace(/[\d,?\.?]+/, fnum);
};