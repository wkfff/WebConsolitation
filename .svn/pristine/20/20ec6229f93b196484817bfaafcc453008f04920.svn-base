<%@ Page Language="C#" AutoEventWireup="true" Codebehind="FO_0003_0007_Horizontal.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.FO_0003_0007_Horizontal" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Src="../../../Components/TagCloud.ascx" TagName="TagCloud" TagPrefix="uc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
 <script  type="text/javascript" src="http://widgets.twimg.com/j/2/widget.js"></script>
					<script type="text/javascript">
					    new TWTR.Widget({
					        version: 2,
					        type: 'list',
					        rpp: 30,
					        interval: 6000,
					        title: '<%=listTitle%>',
					        subject: '<%=listSubject%>',
					        width: 319,
					        height: 300,
					        theme: {
					            shell: {
					                background: '#e0e0e0',
					                color: '#000000'
					            },
					            tweets: {
					                background: '#ffffff',
					                color: '#000000',
					                links: '#1572b4'
					            }
					        },
					        features: {
					            scrollbar: true,
					            loop: false,
					            live: true,
					            hashtags: true,
					            timestamp: true,
					            avatars: true,
					            behavior: 'all'
					        }
					    }).render().setList('imonitoringnews', '<%=listNum%>').start();
					</script>
<head runat="server">
    <title>Untitled Page</title>
</head>
<body link="White" vlink="White" style="background-color: #a8a8a8">
<!--<ireportdescription width="690" height="500"></ireportdescription>-->
    <form id="form1" runat="server">
        
    </form>
</body>
</html>
