<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PeriodParameter.ascx.cs" Inherits="Krista.FM.Server.Dashboards.core.PeriodParameter" %>
<%@ Register Assembly="Infragistics2.WebUI.WebDateChooser.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebSchedule" TagPrefix="igsch" %>
<%@ Register Assembly="Infragistics2.WebUI.WebCombo.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.WebCombo" TagPrefix="igcmbo" %>
<%@ Register Assembly="Infragistics2.WebUI.UltraWebGrid.v9.1, Version=9.1.20091.2013, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<table width="450" height="0" border="0" cellpadding="0" cellspacing="0">
    <tr>
        <td>
            <table width="100%" height="10" border="0" align="left">
                <tr width="1" valign="top">
                    <td width="150" valign="top" style="width: 1px">Год</td>
                    <td width="150" valign="top" style="width: 1px">Полугодие</td>
                    <td width="150" valign="top" style="width: 1px">Квартал</td>
                    <td width="150" valign="top" style="width: 1px">Месяц</td>
                    <td width="150" valign="top" style="width: 1px">День</td>                                                                                                  
                </tr>
                <tr width="1" valign="top">
                    <td width="150" valign="top" style="width: 1px">
                        <asp:DropDownList ID="ddYear" runat="server">
                            <asp:ListItem Selected="True" Value="Все"></asp:ListItem>
                            <asp:ListItem>1998</asp:ListItem>
                            <asp:ListItem>1999</asp:ListItem>
                            <asp:ListItem>2000</asp:ListItem>
                            <asp:ListItem>2001</asp:ListItem>
                            <asp:ListItem>2002</asp:ListItem>
                            <asp:ListItem>2003</asp:ListItem>
                            <asp:ListItem>2004</asp:ListItem>
                            <asp:ListItem>2005</asp:ListItem>
                            <asp:ListItem>2006</asp:ListItem>
                            <asp:ListItem>2007</asp:ListItem>
                            <asp:ListItem>2008</asp:ListItem>
                        </asp:DropDownList></td>
                    <td width="150" valign="top" style="width: 1px">
                        <asp:DropDownList ID="ddHalfYear" runat="server">
                            <asp:ListItem Selected="True" Value="Все"></asp:ListItem>
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                        </asp:DropDownList></td>
                    <td width="150" valign="top" style="width: 1px">
                        <asp:DropDownList ID="ddQuarter" runat="server">
                            <asp:ListItem Selected="True">Все</asp:ListItem>
                            <asp:ListItem Value="1"></asp:ListItem>
                            <asp:ListItem Value="2"></asp:ListItem>
                            <asp:ListItem Value="3"></asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                        </asp:DropDownList></td>
                    <td width="150" valign="top" style="width: 1px">
                        <asp:DropDownList ID="ddMonth" runat="server">
                            <asp:ListItem Selected="True">Все</asp:ListItem>
                            <asp:ListItem>Январь</asp:ListItem>
                            <asp:ListItem>Февраль</asp:ListItem>
                            <asp:ListItem>Март</asp:ListItem>
                            <asp:ListItem>Апрель</asp:ListItem>
                            <asp:ListItem>Май</asp:ListItem>
                            <asp:ListItem>Июнь</asp:ListItem>
                            <asp:ListItem>Июль</asp:ListItem>
                            <asp:ListItem>Август</asp:ListItem>
                            <asp:ListItem>Сентябрь</asp:ListItem>
                            <asp:ListItem>Октябрь</asp:ListItem>
                            <asp:ListItem>Ноябрь</asp:ListItem>
                            <asp:ListItem>Декабрь</asp:ListItem>
                        </asp:DropDownList></td>
                    <td width="150" valign="top" style="width: 1px">
                        <asp:DropDownList ID="ddDay" runat="server">
                            <asp:ListItem Selected="True" Value="Все"></asp:ListItem>
                            <asp:ListItem>1</asp:ListItem>
                            <asp:ListItem>2</asp:ListItem>
                            <asp:ListItem>3</asp:ListItem>
                            <asp:ListItem>4</asp:ListItem>
                            <asp:ListItem>5</asp:ListItem>
                            <asp:ListItem>6</asp:ListItem>
                            <asp:ListItem>7</asp:ListItem>
                            <asp:ListItem>8</asp:ListItem>
                            <asp:ListItem>9</asp:ListItem>
                            <asp:ListItem>10</asp:ListItem>
                            <asp:ListItem>11</asp:ListItem>
                            <asp:ListItem>12</asp:ListItem>
                            <asp:ListItem>13</asp:ListItem>
                            <asp:ListItem>14</asp:ListItem>
                            <asp:ListItem>15</asp:ListItem>
                            <asp:ListItem>16</asp:ListItem>
                            <asp:ListItem>17</asp:ListItem>
                            <asp:ListItem>18</asp:ListItem>
                            <asp:ListItem>19</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>21</asp:ListItem>
                            <asp:ListItem>22</asp:ListItem>
                            <asp:ListItem>23</asp:ListItem>
                            <asp:ListItem>24</asp:ListItem>
                            <asp:ListItem>25</asp:ListItem>
                            <asp:ListItem>26</asp:ListItem>
                            <asp:ListItem>27</asp:ListItem>
                            <asp:ListItem>28</asp:ListItem>
                            <asp:ListItem>29</asp:ListItem>
                            <asp:ListItem>30</asp:ListItem>
                            <asp:ListItem>31</asp:ListItem>
                        </asp:DropDownList></td> 
                </tr>                
            </table>
        </td>
    </tr>
</table>
