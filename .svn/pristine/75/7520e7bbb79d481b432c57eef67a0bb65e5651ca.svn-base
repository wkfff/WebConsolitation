Ext.ns("Krista.FM");

Krista.FM.DSign = function () {
    // private 
    var objCreator = function (name) {
        if (Ext.isIE) {
            return new ActiveXObject(name);
        } else {
            var cadesobject = document.getElementById("cadesplugin");
            return cadesobject.CreateObject(name);
        }
    };

    var decimalToHexString = function (number) {
        if (number < 0) {
            number = 0xFFFFFFFF + number + 1;
        }

        return number.toString(16).toUpperCase();
    };

    var getErrorMessage = function (e) {
        var err = e.message;
        if (!err) {
            err = e;
        } else if (e.number) {
            err += " (0x" + decimalToHexString(e.number) + ")";
        }

        return err;
    };

    var CADESCOM_ATTACHED = false;
    var CADESCOM_DETTACHED = true;

    var CADESCOM_STRING_TO_UCS2LE = 0;
    var CADESCOM_BASE64_TO_BINARY = 1;

    // Перечисление CADESCOM_CADES_TYPE
    var CADESCOM_CADES_DEFAULT = 0; // Квалифицированная подпись
    var CADES_BES = 1; // Обычная неквалифицированная подпись
    var CADESCOM_CADES_X_LONG_TYPE_1 = 2;

    // Перечисление CAPICOM_CERTIFICATE_INCLUDE_OPTION 
    var CAPICOM_CERTIFICATE_INCLUDE_CHAIN_EXCEPT_ROOT = 0;
    var CAPICOM_CERTIFICATE_INCLUDE_WHOLE_CHAIN = 1;

    // public
    return {
        getCertificates: function () {
            var oStore = objCreator("CAPICOM.store");
            if (!oStore) {
                Ext.Msg.alert("Ошибка", "Объект хранилища сертификатов не доступен.");
                return null;
            }

            try {
                oStore.Open();
            }
            catch (e) {
                Ext.Msg.alert("Ошибка", "Ошибка при открытии хранилища: " + getErrorMessage(e));
                return null;
            }

            var certCnt = oStore.Certificates.Count;

            var lst = [];
            for (var i = 1; i <= certCnt; i++) {
                var cert;
                try {
                    cert = oStore.Certificates.Item(i);
                }
                catch (ex) {
                    Ext.Msg.alert("Ошибка", "Ошибка при перечислении сертификатов: " + getErrorMessage(ex));
                    return null;
                }

                var oOpt = [];
                try {
                    oOpt[1] = cert.SubjectName;
                }
                catch (e) {
                    Ext.Msg.alert("Ошибка", "Ошибка при получении свойства SubjectName: " + getErrorMessage(e));
                }

                try {
                    oOpt[0] = cert.Thumbprint;
                }
                catch (e) {
                    Ext.Msg.alert("Ошибка", "Ошибка при получении свойства Thumbprint: " + getErrorMessage(e));
                }

                lst.push(oOpt);
            }

            oStore.Close();

            return lst;
        },

        sign: function (dataToSign, certThumbprint) {
            try {
                var oStore = objCreator("CAPICOM.store");
                oStore.Open();
            } catch (err) {
                Ext.Msg.alert("Ошибка", 'Ошибка создания объекта CAPICOM.store: ' + err.number);
                return null;
            }

            var CAPICOM_CERTIFICATE_FIND_SHA1_HASH = 0;
            var oCerts = oStore.Certificates.Find(CAPICOM_CERTIFICATE_FIND_SHA1_HASH, certThumbprint);
            oStore.Close();

            if (oCerts.Count == 0) {
                Ext.Msg.alert("Ошибка", "Сертификат не найден.");
                return null;
            }

            var oCert = oCerts.Item(1);

            try {
                var oSigner = objCreator("CAdESCOM.CPSigner");
            } catch (err) {
                Ext.Msg.alert("Ошибка", 'Ошибка создания объекта CAdESCOM.CPSigner: ' + err.number);
                return null;
            }

            if (oSigner) {
                oSigner.Certificate = oCert;
            }
            else {
                Ext.Msg.alert("Ошибка", "Ошибка инициализации CPSigner");
                return null;
            }

            var oSignedData = objCreator("CAdESCOM.CadesSignedData");

            try {
                // Данные на подпись ввели
                oSignedData.ContentEncoding = CADESCOM_BASE64_TO_BINARY;
                oSignedData.Content = dataToSign;

                //Выбираем тип подписи
                oSigner.Options = 2;
                try {
                    var sSignedData = oSignedData.SignCades(oSigner, CADES_BES, CADESCOM_DETTACHED);
                    return sSignedData;
                }
                catch (e) {
                    Ext.Msg.alert("Ошибка", "Не удалось создать подпись из-за ошибки: " + getErrorMessage(e));
                    return null;
                }

            } catch (e) {
                Ext.Msg.alert("Ошибка", "Ошибка создания подписи: " + e.number);
                return null;
            }
        },

        verify: function (dataToSign, sSignedMessage) {
            var oSignedData = objCreator("CAdESCOM.CadesSignedData");
            try {
                oSignedData.ContentEncoding = CADESCOM_BASE64_TO_BINARY;
                oSignedData.Content = dataToSign;
                oSignedData.VerifyCades(sSignedMessage, CADES_BES, true);
            } catch (err) {
                Ext.Msg.alert("Ошибка", "Ошибка проверки подписи: " + getErrorMessage(err));
                return false;
            }

            return true;
        }
    };
} ();
