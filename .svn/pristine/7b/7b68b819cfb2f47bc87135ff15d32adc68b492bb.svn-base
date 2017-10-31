<%@ Control Language="C#" AutoEventWireup="true" Codebehind="RefreshButton.ascx.cs"
    Inherits="Krista.FM.Server.Dashboards.Components.RefreshButton" %>
    
<div id="RefreshButton" style="width:84px; height: 27px; border-style:none" class="ButtonDisabled" onmousedown="SubmitClick(this)" onmouseup="SubmitMouseUp(this)" onmouseout="SubmitMouseUp(this)" onclick="DoSubmit(this)"></div>

<script id="Infragistics" type="text/javascript">
<!--
function EnableSubmitButton()
{
    var submitButton = document.getElementById('RefreshButton');   
    if (submitButton != null)
    {
        submitButton.className = "Button";        
    }  		
}

function SubmitClick(submitButton)
{   
    if (submitButton != null)
    {
        if (submitButton.className == "Button")
        {   
            submitButton.className = "ButtonPressed"
        }
        else
        {
            submitButton.className = "ButtonDisabledPressed"
        }        
    }  	   
}

function SubmitMouseUp(submitButton)
{   
    if (submitButton != null)
    {
        if (submitButton.className == "ButtonPressed")
        {   
            submitButton.className = "Button"
        }
        else if (submitButton.className == "ButtonDisabledPressed")
        {            
            {
                submitButton.className = "ButtonDisabled"
            }
        }        
    }  		
}

function DoSubmit(submitButton)
{   
    if (submitButton != null)
    {
        try {
            __doPostBack(submitButton.id, submitButton.id);
        }
        catch(ex){}   
    }  		
}
// -->
</script>
