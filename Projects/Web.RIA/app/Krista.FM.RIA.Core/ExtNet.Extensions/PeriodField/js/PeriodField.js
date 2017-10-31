Ext.ns("ExtExtensions.PeriodField");

ExtExtensions.PeriodField = {
    months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
    rendererFn: function (v, p, r, rowIndex, colIndex, ds) {
        // пустое значение
        if (v == undefined || v <= 0) {
            return '';
        }
        var day = v % 100;
        var month = ((v - day) % 10000) / 100;
        var year = (v - month * 100 - day) / 10000;
        // выбран год
        if (month == 0 && day == 1) {
            return '' + year + ' год';
        }
        // выбран квартал
        if (month == 99) {
            return '' + (day % 10) + ' квартал ' + year + ' года';
        }
        // выбран месяц
        if (month >= 1 && month <= 12 && day == 0) {
            return '' + ExtExtensions.PeriodField.months[month - 1] + ' ' + year + ' года';
        }
        // выбрана точная дата
        return (day < 10 ? '0' : '') + day + (month < 10 ? '.0' : '.') + month + '.' + year;
    },
    rendererMonthsInQuartersFn: function (v) {
        // пустое значение
        if (v == undefined || v <= 0) {
            return '';
        }
        var day = v % 100;
        var month = ((v - day) % 10000) / 100;
        var year = (v - month * 100 - day) / 10000;
        // выбран год
        if (month == 0 && day == 1) {
            return '' + year + ' год';
        }
        // выбран квартал
        if (month == 99) {
            month = day % 10 * 3;
            var text = '';
            switch (month) {
                case 3:
                    text = ' месяца ';
                    break;
                case 6:
                case 9:
                case 12:
                    text = ' месяцев ';
                    break;
            }
            return '' + month + text + year + ' года';
        }
        // выбран месяц
        if (month >= 1 && month <= 12 && day == 0) {
            return '' + ExtExtensions.PeriodField.months[month - 1] + ' ' + year + ' года';
        }
        // выбрана точная дата
        return (day < 10 ? '0' : '') + day + (month < 10 ? '.0' : '.') + month + '.' + year;
    },
    // при выборе даты в календаре (DatePicker)
    dateSelected: function (item, date) {
        var dateId = date.format("Ymd");
        var day = dateId % 100;
        var month = ((dateId - day) / 100) % 100;
        var year = (dateId - month * 100 - day) / 10000;
        var dateText;
        if (day < 10) {
            dateText = '0' + day;
        }
        else {
            dateText = day;
        }

        if (month < 10) {
            dateText += '.0' + month;
        }
        else {
            dateText += '.' + month;
        }

        dateText += '.' + year;

        item.ownerCt.dropDownField.setValue(dateId, dateText, true);
    },
    // при клике в списке периодов
    periodClicked: function (node, PeriodDatePicker) {
        var day = node.id % 100;
        var month = ((node.id - day) % 10000) / 100;
        var year = (node.id - month * 100 - day) / 10000;
        var date = PeriodDatePicker.getValue();
        if (month == 0 && day == 1) {
            date.setDate(1);
            date.setMonth(0);
            date.setYear(year);
        }
        else if (month == 99) {
            date.setDate(1);
            var quarter = day % 10;
            var month = ((quarter - 1) * 3) + 1;
            date.setMonth(month - 1);
            date.setYear(year);
        }
        else if (month >= 1 && month <= 12 && day == 0) {
            date.setDate(1);
            date.setMonth(month - 1);
            date.setYear(year);
        }
        PeriodDatePicker.update(date, true);

    },
    // при двойном клике в списке периодов
    periodChecked: function (field, node, selYear, selQuarter, selMonth) {
        var day = node.id % 100;
        var month = ((node.id - day) % 10000) / 100;
        var year = (node.id - month * 100 - day) / 10000;
        // выбран год, квартал или месяц
        if ((month == 0 && day == 1 && selYear == true) ||
                (month == 99 && selQuarter) ||
                (month >= 1 && month <= 12 && day == 0 && selMonth)) {
            field.ownerCt.dropDownField.setValue(node.id, node.text, true);
            return true;
        }

        return false;
    }
};

Ext.reg('ExtExtensions.PeriodField', ExtExtensions.PeriodField);