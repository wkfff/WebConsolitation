//функция формирования маски кода
var buildMask = function (v, mask, left) {
    if (String(mask).length != 0) {
        var i = 0;
        var mskval = "";
        var count = 0;
        while (i < String(mask).length) {
            if (String(mask).charAt(i) == "#") count++;
            i++;
        };
        
        for (i = 0; i < count - String(v).length; i++)
            mskval += "0";

        // определяем добавлять нехватающие 0ли с начала или с конца
        if (left == undefined) {
            left = true;
        }

        if (left != true) {
             mskval = String(v) + mskval;
        }
         else {
            mskval += String(v);
        }
        
        i = 0;
        while (i < String(mask).length) {
            if (String(mask).charAt(i) == ".") {
                mskval = mskval.slice(0, i) + "." + mskval.slice(i);
            }
            i++;
        };
        return mskval;
    }
    else return v;
};                