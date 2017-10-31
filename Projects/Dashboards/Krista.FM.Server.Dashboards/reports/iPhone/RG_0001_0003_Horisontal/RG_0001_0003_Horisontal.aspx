<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RG_0001_0003_Horisontal.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.RG_0001_0003_Horisontal" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register TagPrefix="uc1" TagName="iPadElementHeader" Src="~/Components/iPadElementHeader.ascx" %>
<%@ Register Src="../../../Components/iPadElementHeader.ascx" TagName="iPadElementHeader"
    TagPrefix="uc3" %>
<%@ Register Src="../../../Components/UltraGridBrick.ascx" TagName="UltraGridBrick"
    TagPrefix="uc5" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
</head>
<body style="background-color: black;">
    <form id="form1" runat="server">   
     <div style="position: absolute; width: 1000px; top: 0px; left: 0px; overflow: hidden; z-index: 2;">
        <table style="width: 765; height: 950; border-collapse: collapse; background-color: Black; top: 0px; left: 0px">
            <tr>
                <td>
                    <asp:Label ID="lbDescription" runat="server" SkinID="InformationText" Text=""></asp:Label>
                </td>
            </tr>
			<tr>
			    <uc3:iPadElementHeader ID="IPadElementHeader1" runat="server" Text="" Width="1000px" />
			</tr>
            <tr>
             <td align = "left">
                 <asp:Label ID="lbInfo" runat="server" SkinID="InformationText" Text=""></asp:Label>
             </td>
            </tr>
            <tr>
                <td align="left">
                   <uc5:UltraGridBrick ID="GridBrick" runat="server"></uc5:UltraGridBrick>
                    <div style="float: left">
                        <asp:Label ID="lbRests" runat="server" SkinID="ImportantText" Text=""></asp:Label></div>
                </td>
            </tr>
        </table>
    </div>  
    <div style="display: none">
      <igchart:UltraChart ID="UltraChart1" runat="server" SkinID="UltraWebColumnChart">           
         <DeploymentScenario FilePath="../../../TemporaryImages" ImageURL="../../../TemporaryImages/Chart_FK0101Gadget_#SEQNUM(100).png" />
      </igchart:UltraChart>
    </div>  
    </form>
</body>
</html>
