<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SaveRejectButton.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.FindButton" %>
<table>
    <tr>
        <td>
            <div id="saveButton" style="width: 84px; height: 27px; border-style: none" class="SaveButtonDisabled"
                onmousedown="SaveButtonClick(this)" onmouseup="SaveMouseUp(this)" onmouseout="SaveMouseUp(this)"
                onclick="DoSubmit(this)">
            </div>
        </td>
        <td>
            <div id="rejectButton" style="width: 84px; height: 27px; border-style: none" class="RejectButtonDisabled"
                onmousedown="RejectButtonClick(this)" onmouseup="RejectMouseUp(this)" onmouseout="RejectMouseUp(this)"
                onclick="DoSubmit(this)">
            </div>
        </td>
    </tr>
</table>
<script id="Infragistics" type="text/javascript">
<!--
    function EnableSubmitButton() {
        var submitButton = document.getElementById('saveButton');
        if (submitButton != null) {
            submitButton.className = "SaveButton";
        }
        var rejectButton = document.getElementById('rejectButton');
        if (rejectButton != null) {
            rejectButton.className = "RejectButton";
        }
    }

    function SaveButtonClick(submitButton) {
        if (submitButton != null) {
            if (submitButton.className == "SaveButton") {
                submitButton.className = "SaveButtonPressed"
            }
            else {
                submitButton.className = "SaveButtonDisabledPressed"
            }
        }
    }

    function RejectButtonClick(submitButton) {
        if (submitButton != null) {
            if (submitButton.className == "RejectButton") {
                submitButton.className = "RejectButtonPressed"
            }
            else {
                submitButton.className = "RejectButtonDisabledPressed"
            }
        }
    }

    function SaveMouseUp(submitButton) {
        if (submitButton != null) {
            if (submitButton.className == "SaveButtonPressed") {
                submitButton.className = "SaveButton"
            }
            else if (submitButton.className == "SaveButtonDisabledPressed") {
                {
                    submitButton.className = "SaveButtonDisabled"
                }
            }
        }
    }

    function RejectMouseUp(submitButton) {
        if (submitButton != null) {
            if (submitButton.className == "RejectButtonPressed") {
                submitButton.className = "RejectButton"
            }
            else if (submitButton.className == "RejectButtonDisabledPressed") {
                {
                    submitButton.className = "RejectButtonDisabled"
                }
            }
        }
    }

    function DoSubmit(submitButton) {
        if (submitButton != null) {
            try {
                __doPostBack(submitButton.id, submitButton.id);
            }
            catch (ex) { }
        }
    }
// -->
</script>
