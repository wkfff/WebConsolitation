<%@ Page Language="C#" MasterPageFile="~/Reports.Master" AutoEventWireup="true" Codebehind="Default.aspx.cs"
    Inherits="Krista.FM.Server.Dashboards.Mobile" Title="Аналитическое приложение для смартфонов и iPad" %>

<asp:Content ID="content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="PresentationText" runat="server" Text="Label" CssClass="PageSubTitle"></asp:Label><br />
    <table style="border-collapse: collapse">
        <tr>
            <td style="width: 270px" valign="top">
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
                        <td class="header">
                            <div style="vertical-align: top; float: left;">
                                iМониторинг в регионах РФ</div>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible;">
                            <table>
                                <tr>
                                    <td>
                                        <a href="http://yar.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Ярославской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Ярославской области" src="../App_Themes/Yaroslavl/Images/emblem.gif" /></a>
                                    </td>
                                    <td style="vertical-align: middle" class="ReportDescription">
                                        <a href="http://yar.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Ярославской области">
                                            Департамент финансов Ярославской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://ryazan.ifinmon.ru/" title="Сводный доклад о результатах мониторинга 
эффективности деятельности органов местного самоуправления городских округов и муниципальных районов Рязанской области за 2008 год">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Сводный доклад о результатах мониторинга 
эффективности деятельности органов местного самоуправления городских округов и муниципальных районов Рязанской области за 2008 год" src="../App_Themes/Ryazan/Images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://ryazan.ifinmon.ru/" title="Сводный доклад о результатах мониторинга эффективности деятельности органов 
местного самоуправления городских округов и муниципальных районов Рязанской области за 2008 год">Правительство Рязанской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.fn.ufnso.ru:8080/CustomReports" title="Мониторинг и анализ показателей 
финансовой сферы Новосибирской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Новосибирской области" src="../App_Themes/Novosib/Images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.fn.ufnso.ru:8080/CustomReports" title="Мониторинг и анализ показателей финансовой сферы Новосибирской 
области">Министерство финансов и налоговой политики Новосибирской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.gubadm.ru:83/site" title="WEB-портал муниципальной 
информационно-аналитической системы администрации муниципального образования г. Губкинский (Ямало-Ненецкий АО)">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="WEB-портал муниципальной 
информационно-аналитической системы администрации муниципального образования г. Губкинский (Ямало-Ненецкий АО)" src="../App_Themes/Gubkinski/Images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.gubadm.ru:83/site" title="WEB-портал муниципальной информационно-аналитической системы администрации 
муниципального образования г. Губкинский (Ямало-Ненецкий АО)">Администрация г.Губкинский (ЯНАО)</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://kursk.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Курской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Курской области" src="../App_Themes/Kursk/Images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://kursk.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Курской области">
                                            Комитет финансов Курской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://penza.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Пензенской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Пензенской области" src="../App_Themes/Penza/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://penza.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Пензенской области">
                                            Министерство финансов Пензенской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://rso-a.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
республики Северная Осетия-Алания">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Республики Северная Осетия-Алания" src="../App_Themes/Alaniya/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://rso-a.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Республики Северная 
Осетия-Алания">Министерство финансов РСО-Алания</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://kostroma.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Костромской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Костромской области" src="../App_Themes/Kostroma/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://kostroma.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Костромской области">
                                            Аналитический центр Костромской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://stavropol.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Ставропольского края">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Ставропольского края" src="../App_Themes/stavropol/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://stavropol.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Ставропольского края">
                                            Министерство финансов Ставропольского края</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://85.192.154.204/" title="Портал муниципальной информационной аналитической 
системы МО «Новоорский район»">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Портал муниципальной информационной 
аналитической системы МО «Новоорский район»" src="../App_Themes/novoorsk/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://85.192.154.204/" title="Портал муниципальной информационной аналитической системы МО «Новоорский район»">
                                            Администрация МО «Новоорский район»</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://Krasnodar.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Краснодарского края">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Краснодарского края" src="../App_Themes/Krasnodar/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://Krasnodar.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Краснодарского края">
                                            Департамент по финансам, бюджету и контролю Краснодарского края</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.karelia.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Республики Карелия">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Республики Карелия" src="../App_Themes/Karelia/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.karelia.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Республики Карелия">
                                            Министерство финансов Республики Карелия</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.moris.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы 
Республики Мордовия">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой 
сферы Республики Мордовия" src="../App_Themes/Moris/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.moris.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Республики Мордовия">
                                            Министерство финансов Республики Мордовия</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.astrakhan.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Астраханской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой сферы Астраханской области"
                                                src="../App_Themes/Astrakhan/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.astrakhan.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Астраханской области">
                                            Министерство финансов Астраханской области</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a class="ReportDescription" href="http://www.omsk.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Омской области">
                                            <img border="0" style="float: left; padding: 2px; padding-right: 10px" alt="Мониторинг и анализ показателей финансовой сферы Омской области"
                                                src="../App_Themes/Omsk/images/emblem.gif" /></a>
                                    </td>
                                    <td class="ReportDescription" style="vertical-align: middle">
                                        <a href="http://www.omsk.ifinmon.ru/" title="Мониторинг и анализ показателей финансовой сферы Омской области">
                                            Министерство финансов Омской области</a>
                                    </td>
                                </tr>
                            </table>
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
                <asp:PlaceHolder ID="ContactInformationPlaceHolder" runat="server"></asp:PlaceHolder>
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
                        <td class="header">
                            <div id="ctl00_ContentPlaceHolder1_ctl00_HeaderDiv" style="vertical-align: top; float: left;">
                                Аналитические отчеты&nbsp;</div>
                            <img src="../images/Reports.png" id="ctl00_ContentPlaceHolder1_ctl00_HeaderImage" style="margin-top: -13px;
                                position: absolute; z-index: 100" />
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                     <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible; text-align: justify;" class="ReportDescription">
                        <a href="http://iminfin.ru">Открытый информационный ресурс сравнительного анализа бюджетов субъектов для персональных компьютеров «iМониторинг».</a>
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
            <td valign="top">
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
                        <td class="header">
                            <div style="vertical-align: top; float: left;">
                                Приложение «iМониторинг» для смартфонов и iPad</div>
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible; padding-left: 10px; padding-right: 10px; text-align: justify;" class="ReportDescription">
                            <img src="icon_sait.png" style="float: left; margin-right: 10px" /><span style="padding-left: 20px">Для
                                обеспечения повсеместного оперативного доступа к информации с мобильных устройств и создания открытой
                                среды информационного взаимодействия создано аналитическое приложение «iМониторинг» для смартфонов и
                                планшетных компьютеров iPad. Приложение отражает актуальную официальную информацию Министерства финансов
                                Российской Федерации и Федерального казначейства (Казначейства России) об исполнении бюджетов всех субъектов
                                Российской Федерации. Состав отчетов включает сравнительный анализ исполнения бюджетов субъектов РФ
                                по доходам и расходам, анализ динамики и темпа роста доходов, межбюджетные трансферты из Федеральных
                                фондов в бюджеты субъектов РФ, государственный и муниципальный долг и иные аналитические отчеты.</span><br />
                            <span style="padding-left: 20px">С целью сопоставления показателей исполнения бюджета с социально-экономическим
                                состоянием субъектов РФ в приложении также отражаются индексы цен, среднедушевые доходы и стоимость
                                фиксированного набора товаров и услуг по сведениям Росстата.</span><br />
                            <span style="padding-left: 20px">Для обеспечения возможности анализа информации до уровня городских округов,
                                муниципальных районов и поселений в качестве примера отражены финансовые паспорта муниципальных образований
                                Новосибирской области.</span><br />
                            <br />
                            <span style="padding-left: 20px">Число пользователей мобильной версии «iМониторинг» на сегодняшний день
                                составляет более 15 тысяч человек.</span><br />
                            <br />
                            <span style="padding-left: 20px">Приложение «iМониторинг» для iPhone и iPod Touch размещено в App Store
                                в разделе «Finance» и его можно скачать и установить бесплатно через iTunes <a href="http://itunes.apple.com/ru/app/id301163175?mt=8">
                                    http://itunes.apple.com/ru/app/id301163175?mt=8</a>.</span><br />
                            <br />
                            <span style="padding-left: 20px">Приложение «iМониторинг» для планшетного компьютера iPad размещено в App
                                Store в разделе «Finance» и его можно скачать и установить бесплатно через iTunes <a href="http://itunes.apple.com/ru/app/id368502051?mt=8">
                                    http://itunes.apple.com/ru/app/id368502051?mt=8</a>.</span><br />
                            <br />
                            <span style="padding-left: 20px">Приложение «iМониторинг» для смартфонов на базе Windows Mobile Вы можете
                                установить с дистрибутива, размещенного на <a href="http://www.krista.ru/products/FM/WM">отдельной странице</a>.</span><br />
                            <br />
                            <span style="padding-left: 20px">Субъекты Российской Федерации выбираются в списке отчетов (для iPad) или
                                в настройках приложения (для смартфонов). Доступна информация по всем субъектам РФ. При нажатии на герб
                                субъекта производится переход на официальный сайт органов власти субъекта РФ.</span>
                            <br />
                            <br />
                            <span style="padding-left: 20px">Открытый информационный ресурс сравнительного анализа бюджетов субъектов
                                для персональных компьютеров <a href="http://www.iminfin.ru">«iМониторинг»</a>.</span><br />
                            <br />
                            Горячая линия консультаций: <a href="mailto:fm@krista.ru">fm@krista.ru</a><br />
                            форум <a href="http://www.forum.iminfin.ru">http://www.forum.iminfin.ru</a><br />
                            тел. 8-800-200-20-72<br />
                            <br />
                            Примеры аналитических отчетов для смартфонов:
                            <table style="margin-left: -3px">
                                <tr>
                                    <td>
                                        <img src="iphone1.png" />
                                    </td>
                                    <td>
                                        <img src="iphone2.png" />
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            Пример аналитического отчета для планшетного компьютера iPad:
                            <table style="margin-left: -3px">
                                <tr>
                                    <td>
                                        <img src="ipad.png" />
                                    </td>
                                </tr>
                            </table>
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
            <td valign="top" style="width: 320px">
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
                        <td class="header">
                            <div style="vertical-align: top; float: left;">
                                Горячая линия&nbsp;</div>
                            <img src="../images/HotLine.png" style="margin-top: -16px; position: absolute; z-index: 100" />
                        </td>
                        <td class="headerright">
                        </td>
                    </tr>
                    <tr>
                        <td class="left">
                        </td>
                        <td style="overflow: visible; padding-left: 5px" class="ReportDescription">
                            Горячая линия консультаций по вопросам федерального закона от 8 мая 2010г. N 83-ФЗ «О внесении изменений
                            в отдельные законодательные акты Российской Федерации в связи с совершенствованием правового положения
                            государственных (муниципальных) учреждений»<br />
                            <br />
                            Телефон 8-800-200-20-72<br />
                            E-mail <a class="ReportDescription" href="mailto:iminfin@krista.ru">iminfin@krista.ru</a><br />
                            <a target="_top" href="http://www.forum.iminfin.ru/">Форум вопросов и ответов</a><br />
                            <br />
                            <b><a href="../app_themes/minfin/images/AppealLavrovAM.jpg">Обращение Директора Департамента бюджетной политики
                                и методологии Минфина России А.М. Лаврова к участникам Горячей линии консультаций </a></b>
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
                <asp:PlaceHolder ID="WebNewsPlaceHolder" runat="server"></asp:PlaceHolder>
            </td>
        </tr>
    </table>
</asp:Content>
