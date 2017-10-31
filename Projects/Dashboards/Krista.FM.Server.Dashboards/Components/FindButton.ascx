<%@ Control Language="C#" AutoEventWireup="true" Codebehind="FindButton.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.FindButton" %>
    
<div id="RefreshButton" style="width:84px; height: 27px; border-style:none" class="FindButtonDisabled" onmousedown="SubmitClick(this)" onmouseup="SubmitMouseUp(this)" onmouseout="SubmitMouseUp(this)" onclick="DoSubmit(this)"></div>

<script id="Infragistics" type="text/javascript">
<!--
function EnableSubmitButton()
{
    var submitButton = document.getElementById('RefreshButton');   
    if (submitButton != null)
    {
        submitButton.className = "FindButton";        
    }  		
}

function SubmitClick(submitButton)
{   
    if (submitButton != null)
    {
        if (submitButton.className == "FindButton")
        {
            submitButton.className = "FindButtonPressed"
        }
        else
        {
            submitButton.className = "FindButtonDisabledPressed"
        }        
    }  	   
}

function SubmitMouseUp(submitButton)
{   
    if (submitButton != null)
    {
        if (submitButton.className == "FindButtonPressed")
        {
            submitButton.className = "FindButton"
        }
        else if (submitButton.className == "FindButtonDisabledPressed")
        {            
            {
                submitButton.className = "FindButtonDisabled"
            }
        }        
    }  		
}

function DoSubmit(submitButton)
{   
    if (submitButton != null)
    {
        try
        {            
            __doPostBack(this.ID,'');
        }
        catch(ex){}   
    }  		
}
// -->
</script>
