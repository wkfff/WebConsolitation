<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Krista.FM.Server.Dashboards.Start" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Финансовый анализ</title>
</head>
<script type="text/javascript" language="JavaScript">
<!-- 
       
	function getSize(){
    var x;
    var y;
	if(document.compatMode=='CSS1Compat')
	{	
	     x = document.documentElement.clientWidth;
	     y = document.documentElement.clientHeight;	     
	}	
	else
	{
	     x = document.body.clientWidth;
	     y = document.body.clientHeight;	     
	}
	try
    {
    	document.cookie = "width_size=" + x + "; path=/;";
    	document.cookie = "height_size=" + y + "; path=/;";	
	}
	catch (exception)
	{;}
	document.forms[0].screen_width.value = x;
	document.forms[0].screen_height.value = y;
    document.forms[0].submit();
}        
-->
</script>

<body bgcolor="white" OnLoad="javascript:getSize()">
    <form id="form1" runat="server">   
    <div visible="false" style="font-size: 12pt; color: gray">
        <a href="Index.aspx"><span style="color: #cccccc"></span></a>&nbsp;<br />
        <br />
        <input id="screen_height" type="hidden" runat="server" />
        <input id="screen_width" type="hidden" runat="server" />        
        &nbsp;</div>
        
    </form>
</body>
</html>
