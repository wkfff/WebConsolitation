<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.reports.FK_0004_0002.Default" Title="Untitled Page" %>

<%@ Register Src="../../Components/PopupInformer.ascx" TagName="PopupInformer" TagPrefix="uc4" %>
<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc1" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebNavigator.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebNavigator" TagPrefix="ignav" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc3" %>
<%@ Register Assembly="Infragistics35.WebUI.Misc.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.Misc" TagPrefix="igmisc" %>
<%@ Register Assembly="Infragistics35.WebUI.WebDataInput.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebDataInput" TagPrefix="igtxt" %>
<%@ Register Src="../../Components/CustomCalendar.ascx" TagName="CustomCalendar"
    TagPrefix="uc9" %>
<%@ Register Assembly="Infragistics35.Web.v11.2, Version=11.2.20112.1019, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.Web.UI.GridControls" TagPrefix="ig" %>
<%@ Register Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI" TagPrefix="asp" %>
<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script src="../../Scripts/jquery.min.js" type="text/javascript"></script>
    <script src="../../Scripts/highcharts.js" type="text/javascript"></script>
    <script src="../../Scripts/themes/grid.js" type="text/javascript"></script>
    <script type="text/javascript">    
var chart1; // globally available
var chart2;
    $(document).ready(function () {
        chart1 = new Highcharts.Chart({        
            chart: {
                renderTo: 'container',
                type: 'bar',
                marginRight: 20                 
            },
            colors: ['#0867a9', '#e6c221', 'Green', '#f58837','Violet' ],
            title: {
                text: ''
            },
            tooltip: {
        valueDecimals: 2,        
        valueSuffix: '<%=valueSuffix %>'
    },
   plotOptions: {
         series: {
            borderColor: 'Black'            
        }
            },
            xAxis: {
                categories: [<%=xAxisCategories %>]
            },
            yAxis: {
                title: {
                    text: '<%=titleText %>'
                },
                labels: {
            formatter: function() {
                return <%=formatter %>;
            }
        }
            },
            series: [<%=series %>]
        });

         chart2 = new Highcharts.Chart({        
            chart: {
                renderTo: 'container2',
                type: 'column',
                marginRight: 20                 
            },
            colors: [ '#8bc9f0', '#0867a9'],
            title: {
                text: ''
            },
            tooltip: {
        valueDecimals: 2,        
        valueSuffix: '<%=valueSuffix %>'
    },
   plotOptions: {
         series: {
            borderColor: 'Black'            
        }
            },
            xAxis: {
                categories: [<%=xAxisCategories2 %>]
            },
            yAxis: {
                title: {
                    text: '<%=titleText %>'
                },
                labels: {
            formatter: function() {
                return <%=formatter %>;
            }
        }
            },
            series: [<%=dynamicSeries %>]
        });
    });
    
    </script>
    <table width="100%">
        <tr>
            <td valign="top" style="width: 100%; vertical-align: top;">
                <uc4:PopupInformer ID="PopupInformer1" runat="server" HelpPageUrl="Default.html" />
                &nbsp;&nbsp;
                <asp:Label ID="Label1" runat="server" CssClass="PageTitle"></asp:Label>&nbsp;
                <asp:Label ID="Label2" runat="server" CssClass="PageSubTitle"></asp:Label>
            </td>
            <td align="right" style="width: 100%;">
                <asp:HyperLink ID="CrossLink1" runat="server" SkinID="HyperLink"></asp:HyperLink>
                <br />
                <asp:HyperLink ID="CrossLink2" runat="server" SkinID="HyperLink"></asp:HyperLink>
            </td>
        </tr>
    </table>
    <table style="vertical-align: top;">
        <tr>
            <td valign="top">
                <uc9:CustomCalendar ID="CustomCalendar1" runat="server">
                </uc9:CustomCalendar>
            </td>
            <td valign="top">
                <uc1:RefreshButton ID="RefreshButton1" runat="server" />
            </td>
            <td valign="top" align="left" class="InformationText">
                <asp:RadioButtonList ID="RubMiltiplierButtonList" runat="server" AutoPostBack="True"
                    RepeatDirection="horizontal" Width="170px">
                    <asp:ListItem>млн.руб.</asp:ListItem>
                    <asp:ListItem Selected="True">млрд.руб.</asp:ListItem>
                </asp:RadioButtonList>
            </td>
            <td valign="top" align="right" style="padding-top: 4px">
                <asp:CheckBox ID="vvp" runat="server" Text="% к ВВП" AutoPostBack="true" Checked="false"
                    CssClass="InformationText" />
            </td>
        </tr>
    </table>
    <table style="width: 100%">
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <asp:ScriptManager ID="ScriptManager1" runat="server">
                            </asp:ScriptManager>
                            <ig:WebDataGrid ID="WebDataGrid1" runat="server" Height="350px" StyleSetName="Cfo"
                                DefaultColumnWidth="100px" AutoGenerateColumns="False" />
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%; margin-top: 10px;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="headerleft">
                        </td>
                        <td class="headerReport" style="overflow: visible;">
                            <asp:Label ID="DynamicChartCaption" runat="server" CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <div id="container" style="width: 100%; height: 600px">
                            </div>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table style="margin-top: 10px;">
                    <tr>
                        <td valign="top">
                            <uc3:CustomMultiCombo ID="ComboKind" runat="server">
                            </uc3:CustomMultiCombo>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <table style="border-collapse: collapse; background-color: White; width: 100%;">
                    <tr>
                        <td class="topleft">
                        </td>
                        <td class="top">
                        </td>
                        <td class="topright">
                        </td>
                    </tr>
                    <tr>
                        <td class="headerleft">
                        </td>
                        <td class="headerReport" style="overflow: visible;">
                            <a name="chart"></a>
                            <asp:Label ID="Label3" runat="server" CssClass="ElementTitle"></asp:Label>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <div id="container2" style="width: 100%; height: 600px">
                            </div>
                        </td>
                        <td class="right">
                        </td>
                    </tr>
                    <tr>
                        <td class="bottomleft">
                        </td>
                        <td class="bottom">
                        </td>
                        <td class="bottomright">
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>
