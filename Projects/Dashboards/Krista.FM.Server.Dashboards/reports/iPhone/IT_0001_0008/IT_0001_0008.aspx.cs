using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Dundas.Maps.WebControl;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.Server.Dashboards.Core.DataProviders;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class IT_0001_0008 : CustomReportPage
    {
        private DateTime reportDate;

        private bool IsSmallResolution
        {
            get { return true; }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            lbCNwesRatingDescription.Text = "Стратегические технологии, которые будут определять развитие ИТ-индустрии в ближайшие несколько лет (по мнению аналитической компании Gartner).<br/>Технологии виртуализации, возглавлявшие аналогичный список Gartner на 2009 год, сместились на 9-е место. Впервые в Top-10 попали мобильные приложения: согласно прогнозу, к концу 2010 года более 1,2 млрд человек будут пользоваться мобильными аппаратами, способными обеспечить доступ к онлайновым приложениям и системам мобильной коммерции.";
            BindTagCloud();
            //UltraChart1.Width = 1;
            //UltraChart1.Height = 1;

            //HyperLink1.NavigateUrl = "http://www.cnews.ru";
            //HyperLink1.Text = "CNews Analytics";
            //HyperLink1.ForeColor = Color.White;

            //HyperLink2.NavigateUrl = "http://www.cnews.ru";
            //HyperLink2.Text = "CNews Analytics";
            //HyperLink2.ForeColor = Color.White;
        }

        private void BindTagCloud()
        {
            Dictionary<string, Tag> tags = new Dictionary<string, Tag>();

            FillTagsDirectOrder(tags);

            TagCloud1.ForeColor = Color.FromArgb(192, 192, 192);
            TagCloud1.startFontSize = 14;
            TagCloud1.fontStep = 2;
            TagCloud1.groupCount = 10;
            TagCloud1.floatStyle = FloatStyle.None;
            TagCloud1.Render(tags);
        }

        private static void FillTagsDirectOrder(Dictionary<string, Tag> tags)
        {
            Tag tag = new Tag();
            tag.key = "<img src='../../../images/ItIcons/CloudComputing.png'>&nbsp;Cloud Computing (Облачные вычисления)";
            tag.weight = 901;
            tag.toolTip = "<b>Cloud Computing.</b>&nbsp;Cloud computing<br/>is a style of computing<br/>that characterizes a model in which<br/>providers deliver a variety of IT-enabled<br/>capabilities to consumers. Cloud-based<br/>services can be exploited in<br/>a variety of ways to develop an application<br/>or a solution. Using cloud<br/>resources does not eliminate the costs<br/>of IT solutions, but does re-arrange<br/>some and reduce others. In addition,<br/>consuming cloud services enterprises<br/>will increasingly act as cloud providers<br/>and deliver application, information<br/>or business process services to customers<br/>and business partners.";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 801;
            tag.toolTip = "<b>Advanced Analytics.</b>&nbsp;Optimization<br/>and simulation is using analytical<br/>tools and models to maximize<br/>business process and decision<br/>effectiveness by examining alternative<br/>outcomes and scenarios, before,<br/>during and after process implementation<br/>and execution. This can be<br/>viewed as a third step in supporting<br/>operational business decisions. Fixed<br/>rules and prepared policies gave<br/>way to more informed decisions powered<br/>by the right information delivered<br/>at the right time, whether<br/>through customer relationship management<br/>(CRM) or enterprise resource<br/>planning (ERP) or other applications.<br/>The new step is to provide<br/>simulation, prediction, optimization<br/>and other analytics, not simply information,<br/>to empower even more decision<br/>flexibility at the time and place<br/>of every business process action.<br/>The new step looks into the future,<br/>predicting what can or will happen.";
            tag.key = "<img src='../../../images/ItIcons/AdvancedAnalytics.png'>&nbsp;Advanced Analytics (Продвинутая аналитика)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 701;
            tag.toolTip = "<b>Client Computing.</b>&nbsp;Virtualization<br/>is bringing new ways of packaging<br/>client computing applications<br/>and capabilities. As a result,<br/>the choice of a particular PC<br/>hardware platform, and eventually<br/>the OS platform, becomes less critical.<br/>Enterprises should proactively<br/>build a five to eight year strategic<br/>client computing roadmap outlining<br/>an approach to device standards, ownership<br/>and support; operating system<br/>and application selection, deployment<br/>and update; and management<br/>and security plans to manage diversity.";
            tag.key = "<img src='../../../images/ItIcons/Clientcomputing.png'>&nbsp;Client computing (Клиентские вычисления)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 601;
            tag.toolTip = "<b>IT for Green.</b>&nbsp;IT can enable<br/>many green initiatives. The use<br/>of IT, particularly among the white<br/>collar staff, can greatly enhance<br/>an enterprise’s green credentials.<br/>Common green initiatives include the<br/>use of e-documents, reducing travel<br/>and teleworking. IT can also provide<br/>the analytic tools that others<br/>in the enterprise may use to reduce<br/>energy consumption in the transportation<br/>of goods or other carbon management<br/>activities.";
            tag.key = "<img src='../../../images/ItIcons/ITforGreen.png'>&nbsp;IT for Green (ИТ для «зелёных»)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 501;
            tag.toolTip = "<b>Reshaping the Data Center.</b><br/>In the past, design principles for data<br/>centers were simple: Figure out<br/>what you have, estimate growth<br/>for 15 to 20 years, then build to suit.<br/>Newly-built data centers often opened<br/>with huge areas of white floor space,<br/>fully powered and backed by<br/>a uninterruptible power supply (UPS),<br/>water-and air-cooled and mostly empty.<br/>However, costs are actually lower<br/>if enterprises adopt a pod-based approach<br/>to data center construction and expansion.<br/>If 9,000 square feet is expected<br/>to be needed during the life of<br/>a data center, then design the site to<br/>support it, but only build what’s<br/>needed for five to seven years.<br/>Cutting operating expenses, which<br/>are a nontrivial part of the overall<br/>IT spend for most clients, frees up money<br/>to apply to other projects or<br/>investments either in IT or in the<br/>business itself.";
            tag.key = "<img src='../../../images/ItIcons/ReshapingforDataCenter.png'>&nbsp;Reshaping for Data Center (ЦОД)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 401;
            tag.toolTip = "<b>Social Computing.</b>&nbsp;Workers<br/>do not want two distinct environments<br/>to support their work – one for<br/>their own work products (whether<br/>personal or group) and another for<br/>accessing “external” information. Enterprises<br/>must focus both on use of social<br/>software and social media<br/>in the enterprise and participation<br/>and integration with externally<br/>facing enterprise-sponsored and public<br/>communities. Do not ignore the<br/>role of the social profile to bring<br/>communities together.";
            tag.key = "<img src='../../../images/ItIcons/Socialcomputing.png'>&nbsp;Social computing (Социальные вычисления)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 301;
            tag.toolTip = "<b>Security – Activity Monitoring.</b><br/>Traditionally, security has focused<br/>on putting up a perimeter fence<br/>to keep others out, but it has<br/>evolved to monitoring activities<br/>and identifying patterns that would have<br/>been missed before. Information<br/>security professionals face the<br/>challenge of detecting malicious activity<br/>in a constant stream of discrete events<br/>that are usually associated with an authorized<br/>user and are generated from<br/>multiple network, system and<br/>application sources. At the same time,<br/>security departments are facing<br/>increasing demands for ever-greater log<br/>analysis and reporting to support<br/>audit requirements. A variety<br/>of complimentary (and sometimes<br/>overlapping) monitoring and analysis<br/>tools help enterprises better detect and<br/>investigate suspicious activity – often<br/>with real-time alerting or<br/>transaction intervention. By<br/>understanding the strengths and weaknesses<br/>of these tools, enterprises can better<br/>understand how to use them to defend<br/>the enterprise and meet audit<br/>requirements.";
            tag.key = "<img src='../../../images/ItIcons/Security.png'>&nbsp;Security – Activity Monitoring (Безопасность)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 201;
            tag.toolTip = "<b>Flash Memory.</b>&nbsp;Flash memory<br/>is not new, but it is moving up<br/>to a new tier in the storage echelon.<br/>Flash memory is a semiconductor<br/>memory device, familiar from<br/>its use in USB memory sticks and digital<br/>camera cards. It is much faster<br/>than rotating disk, but considerably<br/>more expensive, however this<br/>differential is shrinking. At the<br/>rate of price declines, the technology will<br/>enjoy more than a 100 percent<br/>compound annual growth rate during<br/>the new few years and become strategic in<br/>many IT areas including consumer<br/>devices, entertainment equipment<br/>and other embedded IT systems.<br/>In addition, it offers a new layer<br/>of the storage hierarchy in servers and client<br/>computers that has key advantages<br/>including space, heat,<br/>performance and ruggedness.";
            tag.key = "<img src='../../../images/ItIcons/FlashMemory.png'>&nbsp;Flash Memory (Флэш-память)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 101;
            tag.toolTip = "<b>Virtualization for Availability.</b><br/>Virtualization has been on the list<br/>of top strategic technologies in<br/>previous years. It is on the<br/>list this year because Gartner emphases<br/>new elements such as live migration for<br/>availability that have longer term<br/>implications. Live migration is the<br/>movement of a running virtual<br/>machine (VM), while its operating<br/>system and other software continue to execute<br/>as if they remained on the<br/>original physical server. This<br/>takes place by replicating the state<br/>of physical memory between the<br/>source and destination VMs, then, at some<br/>instant in time, one instruction finishes<br/>execution on the source<br/>machine and the next instruction<br/>begins on the destination machine.<br/>However, if replication of memory continues<br/>indefinitely, but execution of<br/>instructions remains on the source<br/>VM, and then the source VM fails<br/>the next instruction would now<br/>place on the destination machine. If the<br/>destination VM were to fail, just<br/>pick a new destination to start<br/>the indefinite migration, thus<br/>making very high availability possible.<br/>The key value proposition is to displace<br/>a variety of separate mechanisms<br/>with a single “dial” that can<br/>be set to any level of availability<br/>from baseline to fault tolerance,<br/>all using a common mechanism and permitting<br/>the settings to be changed rapidly<br/>as needed. Expensive high-reliability<br/>hardware, with fail-over<br/>cluster software and perhaps<br/>even fault-tolerant hardware could be dispensed<br/>with, but still meet availability<br/>needs. This is key to cutting costs,<br/>lowering complexity,<br/>as well as increasing agility as<br/>needs shift.";
            tag.key = "<img src='../../../images/ItIcons/VirtualizationforAvailability.png'>&nbsp;Virtualization for Availability (Виртуализация)";
            tags.Add(tag.key, tag);

            tag = new Tag();
            tag.weight = 1;
            tag.toolTip = "<b>Mobile Applications.</b>&nbsp;By year-end 2010,<br/>1.2 billion people will carry handsets<br/>capable of rich, mobile commerce<br/>providing a rich environment for<br/>the convergence of mobility and<br/>the Web. There are already many<br/>thousands of applications for platforms such<br/>as the Apple iPhone, in spite<br/>of the limited market and need<br/>for unique coding. It may take<br/>a newer version that is designed to flexibly<br/>operate on both full PC and miniature systems,<br/>but if the operating system<br/>interface and processor architecture<br/>were identical, that enabling<br/>factor would create a huge turn<br/>upwards in mobile application availability.<br/>“This list should be used as a starting point<br/>and companies should adjust<br/>their list based on their industry,<br/>unique business needs and technology<br/>adoption mode,” said Carl Claunch,<br/>vice president and distinguished analyst<br/>at Gartner. “When determining what<br/>may be right for each company,<br/>the decision may not have anything<br/>to do with a particular technology.<br/>In other cases, it will be to continue<br/>investing in the technology at<br/>the current rate. In still other cases,<br/>the decision may be to test/pilot<br/>or more aggressively adopt/deploy the technology.”";
            tag.key = "<img src='../../../images/ItIcons/MobileApplications.png'>&nbsp;Mobile Application (Мобильные приложения)";
            tags.Add(tag.key, tag);
        }

        private static void FillTagsCloudOrder(Dictionary<string, int> tags)
        {
            tags.Add(String.Format("НКК&nbsp;<img src='../../../images/starYellow.png'>&nbsp;54 млрд.руб.)"), 54432533);
            tags.Add(String.Format("RRC (8 765 млрд.руб.)"), 8765014);
            tags.Add(String.Format("Ниеншанц (10 274 млрд.руб.)"), 10274184);
            tags.Add(String.Format("Verysell (11 439 млрд.руб.)"), 11439245);
            tags.Add(String.Format("Ай-Теко (12 278 млрд.руб.)"), 12278865);
            tags.Add(String.Format("Энвижн Груп (12 885 млрд.руб.)"), 12885000);
            tags.Add(String.Format("Крок (20 011 млрд.руб.)"), 20011467);
            tags.Add(String.Format("Техносерв (27 770 млрд.руб.)"), 27770157);
            tags.Add(String.Format("ЛАНИТ (34 000 млрд.руб.)"), 34000000);
            tags.Add(String.Format("НКК (54 432 млрд.руб.)"), 54432533);
            tags.Add(String.Format("Merlion (52 407 млрд.руб.)"), 52407277);
            tags.Add(String.Format("Ситроникс (33 000 млрд.руб.)"), 33000000);
            tags.Add(String.Format("R-Style (20 405 млрд.руб.)"), 20405623);
            tags.Add(String.Format("IBS (17 061 млрд.руб.)"), 17061501);
            tags.Add(String.Format("Компьюлинк (12 858 млрд.руб.)"), 12858472);
            tags.Add(String.Format("Лаборатория Касперского (11 575 млрд.руб.)"), 11575362);
            tags.Add(String.Format("1C (11 050 млрд.руб.)"), 11050000);
            tags.Add(String.Format("Оптима (10 158 млрд.руб.)"), 10158139);
            tags.Add(String.Format("Softline (8 518 млрд.руб.)"), 8518978);
            tags.Add(String.Format("ITG (Inline TechnologiesGroup) (8 085 млрд.руб.)"), 8085000);
        }
    }
}
