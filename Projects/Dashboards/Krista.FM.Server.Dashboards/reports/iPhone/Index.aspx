<%@ Page Language="C#" AutoEventWireup="true" Codebehind="Index.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.Index"
    Theme="Default" %>

<%@ Register Src="../../Components/RefreshButton.ascx" TagName="RefreshButton" TagPrefix="uc2" %>
<%@ Register Src="../../Components/CustomMultiCombo.ascx" TagName="CustomMultiCombo"
    TagPrefix="uc1" %>
<asp:content id="content" contentplaceholderid="ContentPlaceHolder1" runat="server">
 <script type="text/javascript">

     var needResize = true;

     function resize(objCell) {
         if (!needResize) {
             needResize = true;
             return;
         }

         var objParent;
         objParent = objCell.parentNode.parentNode;
         for (var j = 1; j < objParent.childNodes.length; j++) {
             if (objParent.childNodes[j].className == "ReportRowFirstState") {
                 objParent.childNodes[j].className = "ReportRowSecondState";
             }
             else {
                 objParent.childNodes[j].className = "ReportRowFirstState";
             }
         }
         if (objCell.className == "GroupReportExpandCellFirstState") {
             objCell.className = "GroupReportExpandCellSecondState";
         }
         else {
             objCell.className = "GroupReportExpandCellFirstState";
         }
     }

     function recheck(obj, checked) {
         needResize = false;
         for (var i = 0; i < obj.childNodes.length; i++) {
             obj.childNodes[i].checked = checked;
             recheck(obj.childNodes[i], checked);
         }
     }   
	
    </script>
    <table>
        <tr>
            <td><uc1:CustomMultiCombo ID="RegionsCombo" runat="server" /></td>
            <td><uc2:RefreshButton ID="RefreshButton1" runat="server" /></td>
        </tr>
    </table>        
    <table>
        <tr>
            <td style="vertical-align:top"><asp:PlaceHolder ID="ReportsPlaceHolder" runat="server"></asp:PlaceHolder></td>
            <td style="vertical-align:top; padding-top: 10px"><asp:PlaceHolder ID="IPhoneReportPlaceHolder" runat="server"></asp:PlaceHolder></td>
        </tr>
    </table>            
    <asp:panel id="UpdateReportControls" runat="server" Visible="false">
    <span id="lastErrorMessage" style="display: none"></span>
        <table class="PageTitle">
            <tr>
                <td id="statusCell_0" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>
                    Готов к обновлению
                </td>
            </tr>
            <tr>
                <td id="statusCell_1" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Загрузка настроек
                </td>
            </tr>
            <tr>
                <td id="statusCell_2" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Запроc параметров отчетов у базы данных
                </td>
            </tr>
            <tr>
                <td id="statusCell_3" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Генерация отчетов
                </td>
            </tr>
            <tr>
                <td id="statusCell_4" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Формирование дампа базы данных для PHP сервиса
                </td>
            </tr>
            <tr>
                <td id="statusCell_5" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Архивация пакета данных
                </td>
            </tr>
            <tr>
                <td id="statusCell_6" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Загрузка пакета на удаленный сервер
                </td>
            </tr>
            <tr>
                <td id="statusCell_7" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Разворачивание пакета на удаленом сервере
                </td>
            </tr>
            <tr>
                <td id="statusCell_8" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Синхронизация даты обновления отчетов в базе данных с отчетами на удаленом хосте
                </td>
            </tr>
            <tr>
                <td id="statusCell_9" style="width: 20px; background-image: url(../../images/ballYellowBB.png); background-repeat:no-repeat">
                </td>
                <td>Данные обновлены
                </td>
            </tr>
        </table>
        <div style="width: 300px; height: 25px; border: solid 1px black">
            <div id="progressbar" style="width: 0%; height: 100%; background-color: blue">
            </div>
        </div>
        <asp:button id="btnBurn" runat="server" text="Начать генерацию отчетов" OnClick="btnBurn_Click"/>        
    </asp:panel>
   
</asp:content>
