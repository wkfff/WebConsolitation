Ext.ns('App.Utils');

/*
* Функция для создании копии объекта
*/
App.Utils.clone = function (o) {
    if (!o || 'object' !== typeof o) {
        return o;
    }

    if ('function' === typeof o.clone) {
        return o.clone();
    }

    var c = '[object Array]' === Object.prototype.toString.call(o) ? [] : {};
    var p, v;
    for (p in o) {
        if (o.hasOwnProperty(p)) {
            v = o[p];
            if (v && 'object' === typeof v) {
                c[p] = App.Utils.clone(v);
            }
            else {
                c[p] = v;
            }
        }
    }
    return c;
};
