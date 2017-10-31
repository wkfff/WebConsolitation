<%@ Page Language="C#" AutoEventWireup="true" Codebehind="IT_0001_0004_v.aspx.cs" Inherits="Krista.FM.Server.Dashboards.reports.iPhone.IT_0001_0004_v" %>

<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebChart" TagPrefix="igchart" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Resources.Appearance" TagPrefix="igchartprop" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebChart.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.UltraChart.Data" TagPrefix="igchartdata" %>
<%@ Register Assembly="Infragistics35.WebUI.UltraWebGrid.v11.1, Version=11.1.20111.2036, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb"
    Namespace="Infragistics.WebUI.UltraWebGrid" TagPrefix="igtbl" %>
<%@ Register Assembly="DundasWebMap" Namespace="Dundas.Maps.WebControl" TagPrefix="DMWC" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Untitled Page</title>
</head>
<body class="iphoneBody">
    <form id="form1" runat="server">
        <table style="position: absolute; width: 760px; height: 900px; top: 0px; left: 0px;
            overflow: hidden">
            <tr>
                <td align="left" valign="top">
                    <table style="border-collapse: collapse; width: 760px; height: 100%;">
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
                            <td class="headerReport">
                                <asp:Label ID="Label1" runat="server" CssClass="ElementTitle" Text="CNews100: ���������� ��-�������� ������ 2009"></asp:Label>
                            </td>
                            <td class="headerright">
                            </td>
                        </tr>                        
                    </table>
                </td>
            </tr>
            <tr>
                <td align="right" class="InformationTextLarge" style="text-align: right;">
                    <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="http://www.cnews.ru/reviews/free/2009/rating/rating1.shtml" Text = "������� CNews100" SkinId="HyperLinkInformationTextLarge"></asp:HyperLink></td>
            </tr>
            <tr>
                <td>
                    <table style="width: 760px; border: 1px solid red" class="HtmlTable">
                        <tr align="center" class="HtmlTableHeader">
                            <td class="HtmlTableHeader">
                                �</td>
                            <td class="HtmlTableHeader">
                                ��������</td>
                            <td class="HtmlTableHeader" style="width: 150px">
                                ����� ������������</td>
                            <td class="HtmlTableHeader">
                                �����</td>
                            <td class="HtmlTableHeader">
                                ���������� ������� � 2009, ���.���.
                            </td>
                            <td class="HtmlTableHeader">
                                ���� 2009/2008, %</td>
                            <td>
                                ������� ����������� ����������� �������� �� 31.12.2009 �.</td>
                        </tr>
                        <tr align="left">
                            <td>
                                1</td>
                            <td>
                                ���</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                54 432 533</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -2,4</td>
                            <td align="right">
                                2 524</td>
                        </tr>
                        <tr align="left">
                            <td>
                                2</td>
                            <td>
                                Merlion</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                52 407 277</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -2,2</td>
                            <td align="right">
                                1 660</td>
                        </tr>
                        <tr align="left">
                            <td>
                                3</td>
                            <td>
                                �����</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                34 000 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,6</td>
                            <td align="right">
                                3 800</td>
                        </tr>
                        <tr align="left">
                            <td>
                                4</td>
                            <td>
                                ���������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                33 000 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -33,5</td>
                            <td align="right">
                                8 700</td>
                        </tr>
                        <tr align="left">
                            <td>
                                5</td>
                            <td>
                                ���������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                27 770 157</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -24,1</td>
                            <td align="right">
                                1 620</td>
                        </tr>
                        <tr align="left">
                            <td>
                                6</td>
                            <td>
                                R-Style</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                20 405 623</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -5,1</td>
                            <td align="right">
                                1 750</td>
                        </tr>
                        <tr align="left">
                            <td>
                                7</td>
                            <td>
                                ����</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                20 011 467</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,6</td>
                            <td align="right">
                                1 511</td>
                        </tr>
                        <tr align="left">
                            <td>
                                8</td>
                            <td>
                                IBS</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                17 061 501</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -11</td>
                            <td align="right">
                                5 000</td>
                        </tr>
                        <tr align="left">
                            <td>
                                9</td>
                            <td>
                                ������ ����</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                12 885 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                2</td>
                            <td align="right">
                                754</td>
                        </tr>
                        <tr align="left">
                            <td>
                                10</td>
                            <td>
                                ����������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                12 858 472</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -55,9</td>
                            <td align="right">
                                940</td>
                        </tr>
                        <tr align="left">
                            <td>
                                11</td>
                            <td>
                                ��-����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                12 278 865</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                3,6</td>
                            <td align="right">
                                1 050</td>
                        </tr>
                        <tr align="left">
                            <td>
                                12</td>
                            <td>
                                ����������� �����������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                11 575 362</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                29,2</td>
                            <td align="right">
                                1 787</td>
                        </tr>
                        <tr align="left">
                            <td>
                                13</td>
                            <td>
                                Verysell
                            </td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                11 439 245</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -20,5</td>
                            <td align="right">
                                458</td>
                        </tr>
                        <tr align="left">
                            <td>
                                14</td>
                            <td>
                                1C</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                11 050 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                9,1</td>
                            <td align="right">
                                1 200</td>
                        </tr>
                        <tr align="left">
                            <td>
                                15</td>
                            <td>
                                ��������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                10 274 184</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                11</td>
                            <td align="right">
                                913</td>
                        </tr>
                        <tr align="left">
                            <td>
                                16</td>
                            <td>
                                ������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                10 158 139</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -11,7</td>
                            <td align="right">
                                2 489</td>
                        </tr>
                        <tr align="left">
                            <td>
                                17</td>
                            <td>
                                RRC</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                8 765 014</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                6,3</td>
                            <td align="right">
                                430</td>
                        </tr>
                        <tr align="left">
                            <td>
                                18</td>
                            <td>
                                Softline
                            </td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                8 518 978</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,5</td>
                            <td align="right">
                                1 500</td>
                        </tr>
                        <tr align="left">
                            <td>
                                19</td>
                            <td>
                                �������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                8 196 973</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                10</td>
                            <td align="right">
                                1 020</td>
                        </tr>
                        <tr align="left">
                            <td>
                                20</td>
                            <td>
                                ITG (Inline Technologies Group)</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                8 085 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -5,5</td>
                            <td align="right">
                                850</td>
                        </tr>
                        <tr align="left">
                            <td>
                                21</td>
                            <td>
                                ���������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                6 940 048</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -27</td>
                            <td align="right">
                                647</td>
                        </tr>
                        <tr align="left">
                            <td>
                                22</td>
                            <td>
                                ����������� ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                6 732 250</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -18,5</td>
                            <td align="right">
                                1 053</td>
                        </tr>
                        <tr align="left">
                            <td>
                                23</td>
                            <td>
                                ���-����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                6 195 732</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                14,6</td>
                            <td align="right">
                                361</td>
                        </tr>
                        <tr align="left">
                            <td>
                                24</td>
                            <td>
                                �������</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                5 935 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                72,8</td>
                            <td align="right">
                                605</td>
                        </tr>
                        <tr align="left">
                            <td>
                                25</td>
                            <td>
                                �����-��</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                5 320 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                4,9</td>
                            <td align="right">
                                632</td>
                        </tr>
                        <tr align="left">
                            <td>
                                26</td>
                            <td>
                                Epam Systems</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                4 780 012</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                4,8</td>
                            <td align="right">
                                4 200</td>
                        </tr>
                        <tr align="left">
                            <td>
                                27</td>
                            <td>
                                ����
                            </td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                4 189 790</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -19</td>
                            <td align="right">
                                1 490</td>
                        </tr>
                        <tr align="left">
                            <td>
                                28</td>
                            <td>
                                ����� ���������� ����������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                4 163 003</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                21,4</td>
                            <td align="right">
                                1 115</td>
                        </tr>
                        <tr align="left">
                            <td>
                                29</td>
                            <td>
                                ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                3 855 060</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                84,7</td>
                            <td align="right">
                                4 006</td>
                        </tr>
                        <tr align="left">
                            <td>
                                30</td>
                            <td>
                                ������</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                3 785 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -33,9</td>
                            <td align="right">
                                800</td>
                        </tr>
                        <tr align="left">
                            <td>
                                31</td>
                            <td>
                                �� �������</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                3 354 573</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,4</td>
                            <td align="right">
                                180</td>
                        </tr>
                        <tr align="left">
                            <td>
                                32</td>
                            <td>
                                ���� ������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                3 157 863</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -16,7</td>
                            <td align="right">
                                612</td>
                        </tr>
                        <tr align="left">
                            <td>
                                33</td>
                            <td>
                                ���</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                3 072 233</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                19</td>
                            <td align="right">
                                1 184</td>
                        </tr>
                        <tr align="left">
                            <td>
                                34</td>
                            <td>
                                ������ ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 949 520</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -22,1</td>
                            <td align="right">
                                360</td>
                        </tr>
                        <tr align="left">
                            <td>
                                35</td>
                            <td>
                                Leta Group</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 920 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                4,2</td>
                            <td align="right">
                                315</td>
                        </tr>
                        <tr align="left">
                            <td>
                                36</td>
                            <td>
                                �������������</td>
                            <td>
                                ������������ ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 864 988</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                22,4</td>
                            <td align="right">
                                503</td>
                        </tr>
                        <tr align="left">
                            <td>
                                37</td>
                            <td>
                                ������ ����</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 800 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -1,4</td>
                            <td align="right">
                                230</td>
                        </tr>
                        <tr align="left">
                            <td>
                                38</td>
                            <td>
                                ���</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 791 444</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -42,5</td>
                            <td align="right">
                                295</td>
                        </tr>
                        <tr align="left">
                            <td>
                                39</td>
                            <td>
                                ����-����� ����������</td>
                            <td>
                                ���������� � ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 351 162</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                27</td>
                            <td align="right">
                                322</td>
                        </tr>
                        <tr align="left">
                            <td>
                                40</td>
                            <td>
                                ����� ����</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 332 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -11,9</td>
                            <td align="right">
                                540</td>
                        </tr>
                        <tr align="left">
                            <td>
                                41</td>
                            <td>
                                ICL - ��� ��</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 277 913</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                8,9</td>
                            <td align="right">
                                1 238</td>
                        </tr>
                        <tr align="left">
                            <td>
                                42</td>
                            <td>
                                ������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ���������</td>
                            <td align="right">
                                2 187 744</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                28</td>
                            <td align="right">
                                762</td>
                        </tr>
                        <tr align="left">
                            <td>
                                43</td>
                            <td>
                                ���</td>
                            <td>
                                ������������ � ����������� ��
                            </td>
                            <td>
                                �������</td>
                            <td align="right">
                                2 180 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                9</td>
                            <td align="right">
                                514</td>
                        </tr>
                        <tr align="left">
                            <td>
                                44</td>
                            <td>
                                ���������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 062 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                49,4</td>
                            <td align="right">
                                ����� 7 000</td>
                        </tr>
                        <tr align="left">
                            <td>
                                45</td>
                            <td>
                                ���� ������������ �����������
                            </td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                2 010 935</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                1,7</td>
                            <td align="right">
                                174</td>
                        </tr>
                        <tr align="left">
                            <td>
                                46</td>
                            <td>
                                X-Com
                            </td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 965 561</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                4,1</td>
                            <td align="right">
                                145</td>
                        </tr>
                        <tr align="left">
                            <td>
                                47</td>
                            <td>
                                �������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 959 363</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                2,8</td>
                            <td align="right">
                                840</td>
                        </tr>
                        <tr align="left">
                            <td>
                                48</td>
                            <td>
                                ��������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 863 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                2,6</td>
                            <td align="right">
                                280</td>
                        </tr>
                        <tr align="left">
                            <td>
                                49</td>
                            <td>
                                ����� �����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 842 237</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -47,6</td>
                            <td align="right">
                                686</td>
                        </tr>
                        <tr align="left">
                            <td>
                                50</td>
                            <td>
                                ����</td>
                            <td>
                                ����������</td>
                            <td>
                                �����������</td>
                            <td align="right">
                                1 801 678</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -47,2</td>
                            <td align="right">
                                575</td>
                        </tr>
                        <tr align="left">
                            <td>
                                51</td>
                            <td>
                                ��� ������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������������</td>
                            <td align="right">
                                1 800 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                39,5</td>
                            <td align="right">
                                1 100</td>
                        </tr>
                        <tr align="left">
                            <td>
                                52</td>
                            <td>
                                ������ �����������</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 780 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                31,9</td>
                            <td align="right">
                                31</td>
                        </tr>
                        <tr align="left">
                            <td>
                                53</td>
                            <td>
                                ���������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 631 429</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -19,5</td>
                            <td align="right">
                                90</td>
                        </tr>
                        <tr align="left">
                            <td>
                                54</td>
                            <td>
                                �������</td>
                            <td>
                                ��������� ��</td>
                            <td>
                                �����</td>
                            <td align="right">
                                1 472 900</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                8,7</td>
                            <td align="right">
                                749</td>
                        </tr>
                        <tr align="left">
                            <td>
                                55</td>
                            <td>
                                ������������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 460 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -3,9</td>
                            <td align="right">
                                �/�</td>
                        </tr>
                        <tr align="left">
                            <td>
                                56</td>
                            <td>
                                ��������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 339 501</td>
                            <td align="right">
                                �/�</td>
                            <td align="right">
                                150</td>
                        </tr>
                        <tr align="left">
                            <td>
                                57</td>
                            <td>
                                �����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 336 639</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -18,3</td>
                            <td align="right">
                                341</td>
                        </tr>
                        <tr align="left">
                            <td>
                                58</td>
                            <td>
                                �������-������
                            </td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 314 149</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                103,9</td>
                            <td align="right">
                                50</td>
                        </tr>
                        <tr align="left">
                            <td>
                                59</td>
                            <td>
                                ������
                            </td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 200 479</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -30,5</td>
                            <td align="right">
                                132</td>
                        </tr>
                        <tr align="left">
                            <td>
                                60</td>
                            <td>
                                ��������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 194 958</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                9,5</td>
                            <td align="right">
                                175</td>
                        </tr>
                        <tr align="left">
                            <td>
                                61</td>
                            <td>
                                ����� ����������</td>
                            <td>
                                ��-������</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                1 180 192</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -27,4</td>
                            <td align="right">
                                340</td>
                        </tr>
                        <tr align="left">
                            <td>
                                62</td>
                            <td>
                                ���������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 033 900</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                0,9</td>
                            <td align="right">
                                695</td>
                        </tr>
                        <tr align="left">
                            <td>
                                63</td>
                            <td>
                                ������������� ���</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                1 027 118</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -64,3</td>
                            <td align="right">
                                172</td>
                        </tr>
                        <tr align="left">
                            <td>
                                64</td>
                            <td>
                                ������ ����������
                            </td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                930 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,1</td>
                            <td align="right">
                                130</td>
                        </tr>
                        <tr align="left">
                            <td>
                                65</td>
                            <td>
                                ���� ������</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ���������� �����
                            </td>
                            <td align="right">
                                786 564</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -24,2</td>
                            <td align="right">
                                281</td>
                        </tr>
                        <tr align="left">
                            <td>
                                66</td>
                            <td>
                                ���</td>
                            <td>
                                ������ ��������</td>
                            <td>
                                �����</td>
                            <td align="right">
                                786 327</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -2,9</td>
                            <td align="right">
                                351</td>
                        </tr>
                        <tr align="left">
                            <td>
                                67</td>
                            <td>
                                ����</td>
                            <td>
                                ����������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                779 503</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -27,2</td>
                            <td align="right">
                                347</td>
                        </tr>
                        <tr align="left">
                            <td>
                                68</td>
                            <td>
                                �����-�������</td>
                            <td>
                                ��-������</td>
                            <td>
                                �����</td>
                            <td align="right">
                                753 016</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -24,4</td>
                            <td align="right">
                                282</td>
                        </tr>
                        <tr align="left">
                            <td>
                                69</td>
                            <td>
                                ������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                745 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -40,1</td>
                            <td align="right">
                                355</td>
                        </tr>
                        <tr align="left">
                            <td>
                                70</td>
                            <td>
                                ������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������ ��������</td>
                            <td align="right">
                                725 653</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -0,6</td>
                            <td align="right">
                                341</td>
                        </tr>
                        <tr align="left">
                            <td>
                                71</td>
                            <td>
                                ������� (����� - ������� ����)</td>
                            <td>
                                ��-������
                            </td>
                            <td>
                                �����������</td>
                            <td align="right">
                                709 702</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -11</td>
                            <td align="right">
                                115</td>
                        </tr>
                        <tr align="left">
                            <td>
                                72</td>
                            <td>
                                �������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                678 400</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                5,9</td>
                            <td align="right">
                                360</td>
                        </tr>
                        <tr align="left">
                            <td>
                                73</td>
                            <td>
                                �-���</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                658 650</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -32</td>
                            <td align="right">
                                226</td>
                        </tr>
                        <tr align="left">
                            <td>
                                74</td>
                            <td>
                                Digital Design
                            </td>
                            <td>
                                ���������� ��</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                627 742</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -16,1</td>
                            <td align="right">
                                307</td>
                        </tr>
                        <tr align="left">
                            <td>
                                75</td>
                            <td>
                                ������� ��������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                627 485</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -0,1</td>
                            <td align="right">
                                238</td>
                        </tr>
                        <tr align="left">
                            <td>
                                76</td>
                            <td>
                                ���� C��� �������
                            </td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                610 706</td>
                            <td align="right">
                                �/�
                            </td>
                            <td align="right">
                                350</td>
                        </tr>
                        <tr align="left">
                            <td>
                                77</td>
                            <td>
                                TerraLink</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                595 172</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                33,1</td>
                            <td align="right">
                                119</td>
                        </tr>
                        <tr align="left">
                            <td>
                                78</td>
                            <td>
                                �������</td>
                            <td>
                                ���������� � ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                582 707</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                3,6</td>
                            <td align="right">
                                86</td>
                        </tr>
                        <tr align="left">
                            <td>
                                79</td>
                            <td>
                                �����</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                544 098</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -28,5</td>
                            <td align="right">
                                550</td>
                        </tr>
                        <tr align="left">
                            <td>
                                80</td>
                            <td>
                                ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                522 143</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -42,4</td>
                            <td align="right">
                                175</td>
                        </tr>
                        <tr align="left">
                            <td>
                                81</td>
                            <td>
                                ��� ������</td>
                            <td>
                                ����������� ��</td>
                            <td>
                                �������</td>
                            <td align="right">
                                472 584</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                6</td>
                            <td align="right">
                                166</td>
                        </tr>
                        <tr align="left">
                            <td>
                                82</td>
                            <td>
                                ����������
                            </td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                466 008</td>
                            <td align="right">
                                312,5</td>
                            <td align="right">
                                527</td>
                        </tr>
                        <tr align="left">
                            <td>
                                83</td>
                            <td>
                                ������-�����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                465 264</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -57,7</td>
                            <td align="right">
                                294</td>
                        </tr>
                        <tr align="left">
                            <td>
                                84</td>
                            <td>
                                ��������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                436 947</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                9</td>
                            <td align="right">
                                155</td>
                        </tr>
                        <tr align="left">
                            <td>
                                85</td>
                            <td>
                                ITV</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                412 528</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -19,6</td>
                            <td align="right">
                                229</td>
                        </tr>
                        <tr align="left">
                            <td>
                                86</td>
                            <td>
                                ��� ����������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                377 440</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -12,9</td>
                            <td align="right">
                                145</td>
                        </tr>
                        <tr align="left">
                            <td>
                                87</td>
                            <td>
                                Positive Technologies
                            </td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                376 000</td>
                            <td align="right">
                                67,1</td>
                            <td align="right">
                                56</td>
                        </tr>
                        <tr align="left">
                            <td>
                                88</td>
                            <td>
                                Computer Business Systems (CBS)
                            </td>
                            <td>
                                ����������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                370 948</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                0,5</td>
                            <td align="right">
                                37</td>
                        </tr>
                        <tr align="left">
                            <td>
                                89</td>
                            <td>
                                �����-����������
                            </td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                347 774</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -12,4</td>
                            <td align="right">
                                118</td>
                        </tr>
                        <tr align="left">
                            <td>
                                90</td>
                            <td>
                                ������� � �������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                333 672</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -21</td>
                            <td align="right">
                                271</td>
                        </tr>
                        <tr align="left">
                            <td>
                                91</td>
                            <td>
                                ������������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                299 540</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                53,1</td>
                            <td align="right">
                                167</td>
                        </tr>
                        <tr align="left">
                            <td>
                                92</td>
                            <td>
                                ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������������</td>
                            <td align="right">
                                272 472</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -22,3</td>
                            <td align="right">
                                71</td>
                        </tr>
                        <tr align="left">
                            <td>
                                93</td>
                            <td>
                                �������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                263 000</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                9,6</td>
                            <td align="right">
                                140</td>
                        </tr>
                        <tr align="left">
                            <td>
                                94</td>
                            <td>
                                �������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������</td>
                            <td align="right">
                                254 144</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -35,7</td>
                            <td align="right">
                                22</td>
                        </tr>
                        <tr align="left">
                            <td>
                                95</td>
                            <td>
                                SH!C (�����-����)</td>
                            <td>
                                ������������ � ����������� ��
                            </td>
                            <td>
                                �������</td>
                            <td align="right">
                                254 071</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -6,3</td>
                            <td align="right">
                                75</td>
                        </tr>
                        <tr align="left">
                            <td>
                                96</td>
                            <td>
                                ����</td>
                            <td>
                                ��-������</td>
                            <td>
                                ����������</td>
                            <td align="right">
                                248 934</td>
                            <td align="right">
                                <img src='../../../images/arrowGreenUpBB.png' style='float: left; padding-left: 20px'>
                                0,5</td>
                            <td align="right">
                                80</td>
                        </tr>
                        <tr align="left">
                            <td>
                                97</td>
                            <td>
                                �� �� �������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                246 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -1,2</td>
                            <td align="right">
                                73</td>
                        </tr>
                        <tr align="left">
                            <td>
                                98</td>
                            <td>
                                ���� ������</td>
                            <td>
                                ����������� �� � ��</td>
                            <td>
                                �����-���������
                            </td>
                            <td align="right">
                                244 189</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -19,4</td>
                            <td align="right">
                                182</td>
                        </tr>
                        <tr align="left">
                            <td>
                                99</td>
                            <td>
                                ����� ���������� ��������</td>
                            <td>
                                ��-������</td>
                            <td>
                                ������-��-����
                            </td>
                            <td align="right">
                                241 000</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -28,6</td>
                            <td align="right">
                                110</td>
                        </tr>
                        <tr align="left">
                            <td>
                                100</td>
                            <td>
                                ������</td>
                            <td>
                                ���������� ��</td>
                            <td>
                                ������</td>
                            <td align="right">
                                221 860</td>
                            <td align="right">
                                <img src='../../../images/arrowRedDownBB.png' style='float: left; padding-left: 20px'>
                                -2,1</td>
                            <td align="right">
                                209</td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
