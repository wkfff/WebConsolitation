Ext.ns("EO15AIP.View.CObjectCard");

EO15AIP.View.CObjectCard.Data = {
    updateButtonsClient: function (state) {
        toEdit.hide();
        toAccept.hide();
        if (state == 1) {
            toReview.show();
        }
        else {
            toReview.hide();
        }
    },

    updateButtonsCoord: function (state) {
        if (state == 2) {
            toEdit.show();
            toReview.hide();
            toAccept.show();
        }
        else if (state == 3) {
            toEdit.show();
            toReview.hide();
            toAccept.hide();
        }
        else {
            toEdit.hide();
            toReview.hide();
            toAccept.hide();
        }
    },

    toEdit: function (isCoord, isClient) {
        if ((StatusDId.value  == "2" || StatusDId.value == "3") && isCoord) {
            StatusDId.setValue("1");
            StatusDName.setValue("На редактировании");
        }

        if (isCoord == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsCoord(1);
        }

        if (isClient == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsClient(1);
        }
        if (isClient) 
            objCardFields.setDisabled(false);
        else
            objCardFields.setDisabled(true);
    },

    toReview: function (isCoord, isClient) {
        if (StatusDId.value == "1" && isClient) {
            StatusDId.setValue("2");
            StatusDName.setValue("На рассмотрении");
        }

        if (isCoord == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsCoord(2);
        }

        if (isClient == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsClient(2);
        }
        if (isCoord) 
            objCardFields.setDisabled(false);
        else
            objCardFields.setDisabled(true);

    },

    toAccept: function (isCoord, isClient) {
        if (StatusDId.value == "2" && isCoord) {
            StatusDId.setValue("3");
            StatusDName.setValue("Утверждено");
        }

        if (isCoord == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsCoord(3);
        }

        if (isClient == true) {
            EO15AIP.View.CObjectCard.Data.updateButtonsClient(3);
        }
        if (isCoord) 
            objCardFields.setDisabled(false);
        else
            objCardFields.setDisabled(true);

    },
};