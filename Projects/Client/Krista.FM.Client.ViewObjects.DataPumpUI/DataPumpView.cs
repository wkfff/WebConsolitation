using System.ComponentModel;
using System.Windows.Forms;

using Krista.FM.Client.Common;


namespace Krista.FM.Client.ViewObjects.DataPumpUI
{
	public class DataPumpView : BaseViewObject.BaseView
    {
        internal ToolTip ttHint;
        private ImageList imageList1;
        private ImageList imageList2;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcViews;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage5;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPumpControl;
        private SplitContainer splitContainer4;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcPumpControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcSchedule;
        private SplitContainer splitContainer3;
        private Infragistics.Win.Misc.UltraLabel ultraLabel12;
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteScheduleStartDate;
        internal Infragistics.Win.UltraWinEditors.UltraDateTimeEditor udteScheduleStartTime;
        internal Infragistics.Win.UltraWinEditors.UltraComboEditor uceSchedulePeriod;
        private Infragistics.Win.Misc.UltraLabel ultraLabel13;
        private Infragistics.Win.Misc.UltraLabel ultraLabel6;
        internal Infragistics.Win.Misc.UltraLabel ulSchedulerInfo;
        private Infragistics.Win.Misc.UltraLabel ultraLabel5;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcSchedulePeriod;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage2;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcScheduleMonthly;
        internal GroupBox gbScheduleMonthly;
        internal NumericUpDown nudMonthlyByDays;
        internal Infragistics.Win.Misc.UltraButton ubtnScheduleMonths;
        internal Infragistics.Win.UltraWinEditors.UltraComboEditor uceWeekNumber;
        internal Infragistics.Win.UltraWinEditors.UltraComboEditor uceWeekDay;
        internal RadioButton rbMonthlyByWeekDays;
        private Infragistics.Win.Misc.UltraLabel ultraLabel17;
        internal RadioButton rbMonthlyByDayNumbers;
        private Infragistics.Win.Misc.UltraLabel ultraLabel16;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcScheduleOnce;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcScheduleDaily;
        internal GroupBox gbScheduleDaily;
        internal NumericUpDown nudScheduleByDay;
        private Infragistics.Win.Misc.UltraLabel ultraLabel20;
        private Infragistics.Win.Misc.UltraLabel ultraLabel19;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcScheduleHour;
        internal GroupBox gbScheduleHour;
        internal NumericUpDown nudScheduleByHour;
        private Infragistics.Win.Misc.UltraLabel ultraLabel41;
        private Infragistics.Win.Misc.UltraLabel ultraLabel42;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcScheduleWeekly;
        internal GroupBox gbScheduleWeekly;
        internal NumericUpDown nudScheduleByWeek;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceTuesday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceWednesday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceThursday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceSunday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceSaturday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceFriday;
        internal Infragistics.Win.UltraWinEditors.UltraCheckEditor uceMonday;
        private Infragistics.Win.Misc.UltraLabel ultraLabel22;
        private Infragistics.Win.Misc.UltraLabel ultraLabel25;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcFileManager;
        internal SplitContainer scBrowsers;
        internal WebBrowser wbLeftBrowser;
        internal ToolStrip toolStrip1;
        internal ToolStripSplitButton tsbLeftBack;
        internal ToolStripSplitButton tsbLeftForward;
        internal ToolStripButton tsbLeftRefresh;
        internal ToolStripButton tsbLeftHome;
        private ToolStripSeparator toolStripSeparator1;
        internal ToolStripSplitButton tsbLeftDrives;
        internal ToolStripButton tsbLeftUp;
        private ToolStripSeparator toolStripSeparator3;
        internal ToolStripDropDownButton tsbLeftView;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem списокToolStripMenuItem;
        private ToolStripMenuItem таблицаToolStripMenuItem;
        internal ToolStrip tsLeftBrowserCaption;
        internal WebBrowser wbRightBrowser;
        internal ToolStrip toolStrip2;
        internal ToolStripSplitButton tsbRightBack;
        internal ToolStripSplitButton tsbRightForward;
        internal ToolStripButton tsbRightRefresh;
        internal ToolStripButton tsbRightHome;
        private ToolStripSeparator toolStripSeparator2;
        internal ToolStripSplitButton tsbRightDrives;
        internal ToolStripButton tsbRightUp;
        private ToolStripSeparator toolStripSeparator4;
        internal ToolStripDropDownButton tsbRightView;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        internal ToolStrip tsRightBrowserCaption;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPumpRuling;
        private Infragistics.Win.Misc.UltraLabel ultraLabel4;
        internal Infragistics.Win.Misc.UltraLabel ulCheckDataComment;
        internal Infragistics.Win.Misc.UltraLabel ulProcessCubeComment;
        internal Infragistics.Win.Misc.UltraLabel ulAssociateDataComment;
        internal Infragistics.Win.Misc.UltraLabel ulProcessDataComment;
        internal Infragistics.Win.Misc.UltraLabel ulPumpDataComment;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipCheckData;
        internal Infragistics.Win.Misc.UltraButton ubtnStopCheckData;
        internal Infragistics.Win.Misc.UltraButton ubtnPauseCheckData;
        internal Infragistics.Win.Misc.UltraButton ubtnStartCheckData;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipProcessCube;
        internal Infragistics.Win.Misc.UltraButton ubtnStopProcessCube;
        internal Infragistics.Win.Misc.UltraButton ubtnPauseProcessCube;
        internal Infragistics.Win.Misc.UltraButton ubtnStartProcessCube;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipAssociateData;
        internal Infragistics.Win.Misc.UltraButton ubtnStopAssociateData;
        internal Infragistics.Win.Misc.UltraButton ubtnPauseAssociateData;
        internal Infragistics.Win.Misc.UltraButton ubtnStartAssociateData;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipProcessData;
        internal Infragistics.Win.Misc.UltraButton ubtnStopProcessData;
        internal Infragistics.Win.Misc.UltraButton ubtnPauseProcessData;
        internal Infragistics.Win.Misc.UltraButton ubtnStartProcessData;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipPumpData;
        internal Infragistics.Win.Misc.UltraButton ubtnStopPumpData;
        internal Infragistics.Win.Misc.UltraButton ubtnPausePumpData;
        internal Infragistics.Win.Misc.UltraButton ubtnStartPumpData;
        private Infragistics.Win.Misc.UltraLabel ultraLabel24;
        private Infragistics.Win.Misc.UltraLabel ultraLabel23;
        private Infragistics.Win.Misc.UltraLabel ultraLabel21;
        internal Infragistics.Win.Misc.UltraLabel ulPumpDataMessage;
        internal Infragistics.Win.Misc.UltraLabel ulCheckDataMessage;
        internal Infragistics.Win.Misc.UltraLabel ulProcessDataMessage;
        internal Infragistics.Win.Misc.UltraLabel ulAssociateDataMessage;
        internal Infragistics.Win.Misc.UltraLabel ulProcessCubeMessage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel15;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicPumpData;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicProcessData;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicAssociateData;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicProcessCube;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicCheckData;
        internal Infragistics.Win.Misc.UltraLabel ulCheckDataStartTime;
        internal Infragistics.Win.Misc.UltraLabel ulProcessDataStartTime;
        internal Infragistics.Win.Misc.UltraLabel ulAssociateDataStartTime;
        internal Infragistics.Win.Misc.UltraLabel ulProcessCubeStartTime;
        internal Infragistics.Win.Misc.UltraLabel ulPumpDataEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulCheckDataEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulProcessDataEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulAssociateDataEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulProcessCubeEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulPumpDataStartTime;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbProcessCube;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbAssociateData;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbProcessData;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbCheckData;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbPumpData;
        private Infragistics.Win.Misc.UltraLabel ultraLabel7;
        private Infragistics.Win.Misc.UltraLabel ultraLabel8;
        private Infragistics.Win.Misc.UltraLabel ultraLabel9;
        private Infragistics.Win.Misc.UltraLabel ultraLabel10;
        private Infragistics.Win.Misc.UltraLabel ultraLabel11;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPumpParams;
        private SplitContainer splitContainer2;
        internal SplitContainer scGeneralParams;
        private Infragistics.Win.Misc.UltraLabel ultraLabel3;
        internal Panel pGeneralPumpParams;
        internal SplitContainer scIndividualParams;
        private Infragistics.Win.Misc.UltraLabel ultraLabel2;
        private Infragistics.Win.Misc.UltraLabel ultraLabel1;
        internal Panel pIndividualPumpParams;
        private Infragistics.Win.Misc.UltraLabel ultraLabel14;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcExecutedOps;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcLog;
        private Panel panel5;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcLogSwitcher;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage3;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage4;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPumpDataLog;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcCheckDataLog;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcProcessDataLog;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcAssociateDataLog;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcProcessCubeLog;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcDeleteDataLog;
        private Panel panel4;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcUnknownPump;
        private Infragistics.Win.Misc.UltraLabel ultraLabel18;
        internal Infragistics.Win.Misc.UltraLabel ulPumpRuleMsg;
        internal Infragistics.Win.Misc.UltraLabel ulHistoryInfo;
        internal Infragistics.Win.Misc.UltraLabel ulHistoryMsg;
        internal Infragistics.Win.Misc.UltraButton ubtnApplySchedule;
        internal Infragistics.Win.Misc.UltraButton ubtnCancelSchedule;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPreviewData;
        private SplitContainer splitContainer6;
        public System.Data.DataSet dsReportData;
        public Infragistics.Win.UltraWinGrid.UltraGrid ugFixedParameters;
        public System.Data.DataSet dsFixedParameters;
        private System.Data.DataTable FixParams;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private Panel panel1;
        internal Infragistics.Win.Misc.UltraButton ubtnCancelPreview;
        internal Infragistics.Win.Misc.UltraButton ubtnApplyPreview;
        private SplitContainer splitContainer5;
        public Infragistics.Win.UltraWinGrid.UltraGrid ugReportData;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl utcReportParts;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage6;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPart1;
        public RichTextBox rtbPart1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPart2;
        public RichTextBox rtbPart2;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcPart3;
        public RichTextBox rtbPart3;
        internal Infragistics.Win.Misc.UltraLabel ulPreviewMsg;
        internal Infragistics.Win.Misc.UltraLabel ulPreviewDataComment;
        internal Infragistics.Win.Misc.UltraButton ubtnSkipPreviewData;
        internal Infragistics.Win.Misc.UltraButton ubtnStopPreviewData;
        internal Infragistics.Win.Misc.UltraButton ubtnPausePreviewData;
        internal Infragistics.Win.Misc.UltraButton ubtnStartPreviewData;
        internal Infragistics.Win.Misc.UltraLabel ulPreviewDataMessage;
        private Infragistics.Win.Misc.UltraLabel ultraLabel28;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicPreviewData;
        internal Infragistics.Win.Misc.UltraLabel ulPreviewDataEndTime;
        internal Infragistics.Win.Misc.UltraLabel ulPreviewDataStartTime;
        internal Infragistics.Win.UltraWinProgressBar.UltraProgressBar upbPreviewData;
        private Infragistics.Win.Misc.UltraLabel ultraLabel31;
        internal Infragistics.Win.UltraWinTabControl.UltraTabControl ultraTabControl1;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage7;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl1;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ugPumpHistory;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl ultraTabPageControl2;
        internal Infragistics.Win.UltraWinGrid.UltraGrid ugDataSources;
        internal ToolStripTextBox tstbLeftBrowserCaption;
        internal ToolStripTextBox tstbRightBrowserCaption;
        private ImageList imageList3;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicPreviewDataResult;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicPumpDataResult;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicProcessDataResult;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicAssociateDataResult;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicProcessCubeResult;
        internal Infragistics.Win.UltraWinEditors.UltraPictureBox upicCheckDataResult;
        private ToolStrip toolStrip5;
        internal ToolStripButton tsbRefresh;
        internal Infragistics.Win.UltraWinTabControl.UltraTabPageControl utpcClassifiers;
		private IContainer components;

		public DataPumpView()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem12 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataPumpView));
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance66 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance67 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance68 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance69 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance70 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance71 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance72 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance73 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueListItem valueListItem13 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem14 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem15 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem16 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem17 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance74 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab50 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab5 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab6 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance77 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab7 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab8 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab9 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab10 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab11 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab12 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab13 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance78 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab14 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab15 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab16 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance79 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("FixParams", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Value");
            Infragistics.Win.Appearance appearance80 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance81 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab17 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab18 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab19 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab20 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab21 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab22 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab23 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.Appearance appearance82 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab24 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab25 = new Infragistics.Win.UltraWinTabControl.UltraTab();
            this.utpcScheduleOnce = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcScheduleDaily = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gbScheduleDaily = new System.Windows.Forms.GroupBox();
            this.nudScheduleByDay = new System.Windows.Forms.NumericUpDown();
            this.ultraLabel20 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel19 = new Infragistics.Win.Misc.UltraLabel();
            this.utpcScheduleHour = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gbScheduleHour = new System.Windows.Forms.GroupBox();
            this.nudScheduleByHour = new System.Windows.Forms.NumericUpDown();
            this.ultraLabel41 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel42 = new Infragistics.Win.Misc.UltraLabel();
            this.utpcScheduleWeekly = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gbScheduleWeekly = new System.Windows.Forms.GroupBox();
            this.nudScheduleByWeek = new System.Windows.Forms.NumericUpDown();
            this.uceTuesday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceWednesday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceThursday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceSunday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceSaturday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceFriday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.uceMonday = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            this.ultraLabel22 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel25 = new Infragistics.Win.Misc.UltraLabel();
            this.utpcScheduleMonthly = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.gbScheduleMonthly = new System.Windows.Forms.GroupBox();
            this.nudMonthlyByDays = new System.Windows.Forms.NumericUpDown();
            this.ubtnScheduleMonths = new Infragistics.Win.Misc.UltraButton();
            this.uceWeekNumber = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.uceWeekDay = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.rbMonthlyByWeekDays = new System.Windows.Forms.RadioButton();
            this.ultraLabel17 = new Infragistics.Win.Misc.UltraLabel();
            this.rbMonthlyByDayNumbers = new System.Windows.Forms.RadioButton();
            this.ultraLabel16 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraTabPageControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugPumpHistory = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.imageList3 = new System.Windows.Forms.ImageList(this.components);
            this.ultraTabPageControl2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ugDataSources = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.utpcPumpDataLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcProcessDataLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcAssociateDataLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcProcessCubeLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcCheckDataLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcDeleteDataLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcClassifiers = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.utpcPart1 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.rtbPart1 = new System.Windows.Forms.RichTextBox();
            this.utpcPart2 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.rtbPart2 = new System.Windows.Forms.RichTextBox();
            this.utpcPart3 = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.rtbPart3 = new System.Windows.Forms.RichTextBox();
            this.utpcPumpRuling = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.upicPreviewDataResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicPumpDataResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicProcessDataResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicAssociateDataResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicProcessCubeResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicCheckDataResult = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ulPreviewDataComment = new Infragistics.Win.Misc.UltraLabel();
            this.ubtnSkipPreviewData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopPreviewData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPausePreviewData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartPreviewData = new Infragistics.Win.Misc.UltraButton();
            this.ulPreviewDataMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel28 = new Infragistics.Win.Misc.UltraLabel();
            this.upicPreviewData = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ulPreviewDataEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulPreviewDataStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.upbPreviewData = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.ultraLabel31 = new Infragistics.Win.Misc.UltraLabel();
            this.ulPumpRuleMsg = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel4 = new Infragistics.Win.Misc.UltraLabel();
            this.ulCheckDataComment = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessCubeComment = new Infragistics.Win.Misc.UltraLabel();
            this.ulAssociateDataComment = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessDataComment = new Infragistics.Win.Misc.UltraLabel();
            this.ulPumpDataComment = new Infragistics.Win.Misc.UltraLabel();
            this.ubtnSkipCheckData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopCheckData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPauseCheckData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartCheckData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSkipProcessCube = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopProcessCube = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPauseProcessCube = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartProcessCube = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSkipAssociateData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopAssociateData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPauseAssociateData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartAssociateData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSkipProcessData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopProcessData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPauseProcessData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartProcessData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnSkipPumpData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStopPumpData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnPausePumpData = new Infragistics.Win.Misc.UltraButton();
            this.ubtnStartPumpData = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel24 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel23 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel21 = new Infragistics.Win.Misc.UltraLabel();
            this.ulPumpDataMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ulCheckDataMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessDataMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ulAssociateDataMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessCubeMessage = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel15 = new Infragistics.Win.Misc.UltraLabel();
            this.upicPumpData = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicProcessData = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicAssociateData = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicProcessCube = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.upicCheckData = new Infragistics.Win.UltraWinEditors.UltraPictureBox();
            this.ulCheckDataStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessDataStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulAssociateDataStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessCubeStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulPumpDataEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulCheckDataEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessDataEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulAssociateDataEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulProcessCubeEndTime = new Infragistics.Win.Misc.UltraLabel();
            this.ulPumpDataStartTime = new Infragistics.Win.Misc.UltraLabel();
            this.upbProcessCube = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upbAssociateData = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upbProcessData = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upbCheckData = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.upbPumpData = new Infragistics.Win.UltraWinProgressBar.UltraProgressBar();
            this.ultraLabel7 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel8 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel9 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel10 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel11 = new Infragistics.Win.Misc.UltraLabel();
            this.utpcPumpParams = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.scGeneralParams = new System.Windows.Forms.SplitContainer();
            this.ultraLabel3 = new Infragistics.Win.Misc.UltraLabel();
            this.pGeneralPumpParams = new System.Windows.Forms.Panel();
            this.scIndividualParams = new System.Windows.Forms.SplitContainer();
            this.ultraLabel2 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel1 = new Infragistics.Win.Misc.UltraLabel();
            this.pIndividualPumpParams = new System.Windows.Forms.Panel();
            this.ultraLabel14 = new Infragistics.Win.Misc.UltraLabel();
            this.utpcSchedule = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.ubtnCancelSchedule = new Infragistics.Win.Misc.UltraButton();
            this.ubtnApplySchedule = new Infragistics.Win.Misc.UltraButton();
            this.ultraLabel12 = new Infragistics.Win.Misc.UltraLabel();
            this.udteScheduleStartDate = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.udteScheduleStartTime = new Infragistics.Win.UltraWinEditors.UltraDateTimeEditor();
            this.uceSchedulePeriod = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ultraLabel13 = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel6 = new Infragistics.Win.Misc.UltraLabel();
            this.ulSchedulerInfo = new Infragistics.Win.Misc.UltraLabel();
            this.ultraLabel5 = new Infragistics.Win.Misc.UltraLabel();
            this.utcSchedulePeriod = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage2 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.utpcFileManager = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.scBrowsers = new System.Windows.Forms.SplitContainer();
            this.wbLeftBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbLeftBack = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbLeftForward = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbLeftRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbLeftHome = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLeftDrives = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbLeftUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbLeftView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.списокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.таблицаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsLeftBrowserCaption = new System.Windows.Forms.ToolStrip();
            this.tstbLeftBrowserCaption = new System.Windows.Forms.ToolStripTextBox();
            this.wbRightBrowser = new System.Windows.Forms.WebBrowser();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.tsbRightBack = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbRightForward = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbRightRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbRightHome = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRightDrives = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbRightUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRightView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsRightBrowserCaption = new System.Windows.Forms.ToolStrip();
            this.tstbRightBrowserCaption = new System.Windows.Forms.ToolStripTextBox();
            this.utpcExecutedOps = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraTabControl1 = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage7 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ulHistoryMsg = new Infragistics.Win.Misc.UltraLabel();
            this.utpcLog = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel5 = new System.Windows.Forms.Panel();
            this.utcLogSwitcher = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage3 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ultraTabSharedControlsPage4 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ulHistoryInfo = new Infragistics.Win.Misc.UltraLabel();
            this.utpcPreviewData = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.ugReportData = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsReportData = new System.Data.DataSet();
            this.utcReportParts = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage6 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.ulPreviewMsg = new Infragistics.Win.Misc.UltraLabel();
            this.ugFixedParameters = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.dsFixedParameters = new System.Data.DataSet();
            this.FixParams = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ubtnCancelPreview = new Infragistics.Win.Misc.UltraButton();
            this.ubtnApplyPreview = new Infragistics.Win.Misc.UltraButton();
            this.utpcPumpControl = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.toolStrip5 = new System.Windows.Forms.ToolStrip();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.utcPumpControl = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.utpcUnknownPump = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.ultraLabel18 = new Infragistics.Win.Misc.UltraLabel();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ttHint = new System.Windows.Forms.ToolTip(this.components);
            this.utcViews = new Infragistics.Win.UltraWinTabControl.UltraTabControl();
            this.ultraTabSharedControlsPage5 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.utpcScheduleDaily.SuspendLayout();
            this.gbScheduleDaily.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByDay)).BeginInit();
            this.utpcScheduleHour.SuspendLayout();
            this.gbScheduleHour.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByHour)).BeginInit();
            this.utpcScheduleWeekly.SuspendLayout();
            this.gbScheduleWeekly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByWeek)).BeginInit();
            this.utpcScheduleMonthly.SuspendLayout();
            this.gbScheduleMonthly.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonthlyByDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceWeekNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceWeekDay)).BeginInit();
            this.ultraTabPageControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugPumpHistory)).BeginInit();
            this.ultraTabPageControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugDataSources)).BeginInit();
            this.utpcPart1.SuspendLayout();
            this.utpcPart2.SuspendLayout();
            this.utpcPart3.SuspendLayout();
            this.utpcPumpRuling.SuspendLayout();
            this.utpcPumpParams.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.scGeneralParams.Panel1.SuspendLayout();
            this.scGeneralParams.Panel2.SuspendLayout();
            this.scGeneralParams.SuspendLayout();
            this.scIndividualParams.Panel1.SuspendLayout();
            this.scIndividualParams.Panel2.SuspendLayout();
            this.scIndividualParams.SuspendLayout();
            this.utpcSchedule.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udteScheduleStartDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteScheduleStartTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceSchedulePeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcSchedulePeriod)).BeginInit();
            this.utcSchedulePeriod.SuspendLayout();
            this.utpcFileManager.SuspendLayout();
            this.scBrowsers.Panel1.SuspendLayout();
            this.scBrowsers.Panel2.SuspendLayout();
            this.scBrowsers.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tsLeftBrowserCaption.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tsRightBrowserCaption.SuspendLayout();
            this.utpcExecutedOps.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).BeginInit();
            this.ultraTabControl1.SuspendLayout();
            this.utpcLog.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).BeginInit();
            this.utcLogSwitcher.SuspendLayout();
            this.panel4.SuspendLayout();
            this.utpcPreviewData.SuspendLayout();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugReportData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsReportData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcReportParts)).BeginInit();
            this.utcReportParts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ugFixedParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFixedParameters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FixParams)).BeginInit();
            this.panel1.SuspendLayout();
            this.utpcPumpControl.SuspendLayout();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.toolStrip5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcPumpControl)).BeginInit();
            this.utcPumpControl.SuspendLayout();
            this.utpcUnknownPump.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcViews)).BeginInit();
            this.utcViews.SuspendLayout();
            this.SuspendLayout();
            // 
            // utpcScheduleOnce
            // 
            this.utpcScheduleOnce.AutoScroll = true;
            this.utpcScheduleOnce.Location = new System.Drawing.Point(0, 0);
            this.utpcScheduleOnce.Name = "utpcScheduleOnce";
            this.utpcScheduleOnce.Size = new System.Drawing.Size(778, 472);
            // 
            // utpcScheduleDaily
            // 
            this.utpcScheduleDaily.AutoScroll = true;
            this.utpcScheduleDaily.Controls.Add(this.gbScheduleDaily);
            this.utpcScheduleDaily.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcScheduleDaily.Name = "utpcScheduleDaily";
            this.utpcScheduleDaily.Size = new System.Drawing.Size(778, 472);
            // 
            // gbScheduleDaily
            // 
            this.gbScheduleDaily.Controls.Add(this.nudScheduleByDay);
            this.gbScheduleDaily.Controls.Add(this.ultraLabel20);
            this.gbScheduleDaily.Controls.Add(this.ultraLabel19);
            this.gbScheduleDaily.Location = new System.Drawing.Point(13, 0);
            this.gbScheduleDaily.Name = "gbScheduleDaily";
            this.gbScheduleDaily.Size = new System.Drawing.Size(411, 63);
            this.gbScheduleDaily.TabIndex = 1;
            this.gbScheduleDaily.TabStop = false;
            this.gbScheduleDaily.Text = "Расписание по дням";
            // 
            // nudScheduleByDay
            // 
            this.nudScheduleByDay.Location = new System.Drawing.Point(71, 26);
            this.nudScheduleByDay.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScheduleByDay.Name = "nudScheduleByDay";
            this.nudScheduleByDay.Size = new System.Drawing.Size(102, 20);
            this.nudScheduleByDay.TabIndex = 3;
            this.nudScheduleByDay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudScheduleByDay.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ultraLabel20
            // 
            this.ultraLabel20.Location = new System.Drawing.Point(192, 28);
            this.ultraLabel20.Name = "ultraLabel20";
            this.ultraLabel20.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel20.TabIndex = 2;
            this.ultraLabel20.Text = "день";
            // 
            // ultraLabel19
            // 
            this.ultraLabel19.Location = new System.Drawing.Point(6, 28);
            this.ultraLabel19.Name = "ultraLabel19";
            this.ultraLabel19.Size = new System.Drawing.Size(59, 23);
            this.ultraLabel19.TabIndex = 0;
            this.ultraLabel19.Text = "каждый";
            // 
            // utpcScheduleHour
            // 
            this.utpcScheduleHour.AutoScroll = true;
            this.utpcScheduleHour.Controls.Add(this.gbScheduleHour);
            this.utpcScheduleHour.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcScheduleHour.Name = "utpcScheduleHour";
            this.utpcScheduleHour.Size = new System.Drawing.Size(778, 472);
            // 
            // gbScheduleHour
            // 
            this.gbScheduleHour.Controls.Add(this.nudScheduleByHour);
            this.gbScheduleHour.Controls.Add(this.ultraLabel41);
            this.gbScheduleHour.Controls.Add(this.ultraLabel42);
            this.gbScheduleHour.Location = new System.Drawing.Point(13, 0);
            this.gbScheduleHour.Name = "gbScheduleHour";
            this.gbScheduleHour.Size = new System.Drawing.Size(411, 63);
            this.gbScheduleHour.TabIndex = 1;
            this.gbScheduleHour.TabStop = false;
            this.gbScheduleHour.Text = "Расписание по часам";
            // 
            // nudScheduleByHour
            // 
            this.nudScheduleByHour.Location = new System.Drawing.Point(71, 26);
            this.nudScheduleByHour.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScheduleByHour.Name = "nudScheduleByHour";
            this.nudScheduleByHour.Size = new System.Drawing.Size(102, 20);
            this.nudScheduleByHour.TabIndex = 3;
            this.nudScheduleByHour.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudScheduleByHour.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ultraLabel41
            // 
            this.ultraLabel41.Location = new System.Drawing.Point(192, 28);
            this.ultraLabel41.Name = "ultraLabel41";
            this.ultraLabel41.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel41.TabIndex = 2;
            this.ultraLabel41.Text = "час";
            // 
            // ultraLabel42
            // 
            this.ultraLabel42.Location = new System.Drawing.Point(6, 28);
            this.ultraLabel42.Name = "ultraLabel42";
            this.ultraLabel42.Size = new System.Drawing.Size(59, 23);
            this.ultraLabel42.TabIndex = 0;
            this.ultraLabel42.Text = "каждый";
            // 
            // utpcScheduleWeekly
            // 
            this.utpcScheduleWeekly.AutoScroll = true;
            this.utpcScheduleWeekly.Controls.Add(this.gbScheduleWeekly);
            this.utpcScheduleWeekly.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcScheduleWeekly.Name = "utpcScheduleWeekly";
            this.utpcScheduleWeekly.Size = new System.Drawing.Size(778, 472);
            // 
            // gbScheduleWeekly
            // 
            this.gbScheduleWeekly.Controls.Add(this.nudScheduleByWeek);
            this.gbScheduleWeekly.Controls.Add(this.uceTuesday);
            this.gbScheduleWeekly.Controls.Add(this.uceWednesday);
            this.gbScheduleWeekly.Controls.Add(this.uceThursday);
            this.gbScheduleWeekly.Controls.Add(this.uceSunday);
            this.gbScheduleWeekly.Controls.Add(this.uceSaturday);
            this.gbScheduleWeekly.Controls.Add(this.uceFriday);
            this.gbScheduleWeekly.Controls.Add(this.uceMonday);
            this.gbScheduleWeekly.Controls.Add(this.ultraLabel22);
            this.gbScheduleWeekly.Controls.Add(this.ultraLabel25);
            this.gbScheduleWeekly.Location = new System.Drawing.Point(13, 0);
            this.gbScheduleWeekly.Name = "gbScheduleWeekly";
            this.gbScheduleWeekly.Size = new System.Drawing.Size(411, 175);
            this.gbScheduleWeekly.TabIndex = 2;
            this.gbScheduleWeekly.TabStop = false;
            this.gbScheduleWeekly.Text = "Расписание по неделям";
            // 
            // nudScheduleByWeek
            // 
            this.nudScheduleByWeek.Location = new System.Drawing.Point(71, 26);
            this.nudScheduleByWeek.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudScheduleByWeek.Name = "nudScheduleByWeek";
            this.nudScheduleByWeek.Size = new System.Drawing.Size(102, 20);
            this.nudScheduleByWeek.TabIndex = 10;
            this.nudScheduleByWeek.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudScheduleByWeek.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // uceTuesday
            // 
            this.uceTuesday.Location = new System.Drawing.Point(10, 92);
            this.uceTuesday.Name = "uceTuesday";
            this.uceTuesday.Size = new System.Drawing.Size(120, 20);
            this.uceTuesday.TabIndex = 9;
            this.uceTuesday.Text = "вторникам";
            // 
            // uceWednesday
            // 
            this.uceWednesday.Location = new System.Drawing.Point(10, 118);
            this.uceWednesday.Name = "uceWednesday";
            this.uceWednesday.Size = new System.Drawing.Size(120, 20);
            this.uceWednesday.TabIndex = 8;
            this.uceWednesday.Text = "средам";
            // 
            // uceThursday
            // 
            this.uceThursday.Location = new System.Drawing.Point(10, 144);
            this.uceThursday.Name = "uceThursday";
            this.uceThursday.Size = new System.Drawing.Size(120, 20);
            this.uceThursday.TabIndex = 7;
            this.uceThursday.Text = "четвергам";
            // 
            // uceSunday
            // 
            this.uceSunday.Location = new System.Drawing.Point(158, 118);
            this.uceSunday.Name = "uceSunday";
            this.uceSunday.Size = new System.Drawing.Size(120, 20);
            this.uceSunday.TabIndex = 6;
            this.uceSunday.Text = "воскресеньям";
            // 
            // uceSaturday
            // 
            this.uceSaturday.Location = new System.Drawing.Point(158, 92);
            this.uceSaturday.Name = "uceSaturday";
            this.uceSaturday.Size = new System.Drawing.Size(120, 20);
            this.uceSaturday.TabIndex = 5;
            this.uceSaturday.Text = "субботам";
            // 
            // uceFriday
            // 
            this.uceFriday.Location = new System.Drawing.Point(158, 66);
            this.uceFriday.Name = "uceFriday";
            this.uceFriday.Size = new System.Drawing.Size(120, 20);
            this.uceFriday.TabIndex = 4;
            this.uceFriday.Text = "пятницам";
            // 
            // uceMonday
            // 
            this.uceMonday.Location = new System.Drawing.Point(10, 66);
            this.uceMonday.Name = "uceMonday";
            this.uceMonday.Size = new System.Drawing.Size(120, 20);
            this.uceMonday.TabIndex = 3;
            this.uceMonday.Text = "понедельникам";
            // 
            // ultraLabel22
            // 
            this.ultraLabel22.Location = new System.Drawing.Point(199, 28);
            this.ultraLabel22.Name = "ultraLabel22";
            this.ultraLabel22.Size = new System.Drawing.Size(72, 23);
            this.ultraLabel22.TabIndex = 2;
            this.ultraLabel22.Text = "неделю по:";
            // 
            // ultraLabel25
            // 
            this.ultraLabel25.Location = new System.Drawing.Point(6, 28);
            this.ultraLabel25.Name = "ultraLabel25";
            this.ultraLabel25.Size = new System.Drawing.Size(59, 23);
            this.ultraLabel25.TabIndex = 0;
            this.ultraLabel25.Text = "каждую";
            // 
            // utpcScheduleMonthly
            // 
            this.utpcScheduleMonthly.AutoScroll = true;
            this.utpcScheduleMonthly.Controls.Add(this.gbScheduleMonthly);
            this.utpcScheduleMonthly.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcScheduleMonthly.Name = "utpcScheduleMonthly";
            this.utpcScheduleMonthly.Size = new System.Drawing.Size(778, 472);
            // 
            // gbScheduleMonthly
            // 
            this.gbScheduleMonthly.BackColor = System.Drawing.Color.Transparent;
            this.gbScheduleMonthly.Controls.Add(this.nudMonthlyByDays);
            this.gbScheduleMonthly.Controls.Add(this.ubtnScheduleMonths);
            this.gbScheduleMonthly.Controls.Add(this.uceWeekNumber);
            this.gbScheduleMonthly.Controls.Add(this.uceWeekDay);
            this.gbScheduleMonthly.Controls.Add(this.rbMonthlyByWeekDays);
            this.gbScheduleMonthly.Controls.Add(this.ultraLabel17);
            this.gbScheduleMonthly.Controls.Add(this.rbMonthlyByDayNumbers);
            this.gbScheduleMonthly.Controls.Add(this.ultraLabel16);
            this.gbScheduleMonthly.Location = new System.Drawing.Point(13, 0);
            this.gbScheduleMonthly.Name = "gbScheduleMonthly";
            this.gbScheduleMonthly.Size = new System.Drawing.Size(411, 128);
            this.gbScheduleMonthly.TabIndex = 48;
            this.gbScheduleMonthly.TabStop = false;
            this.gbScheduleMonthly.Text = "Расписание по месяцам";
            // 
            // nudMonthlyByDays
            // 
            this.nudMonthlyByDays.Location = new System.Drawing.Point(61, 29);
            this.nudMonthlyByDays.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudMonthlyByDays.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudMonthlyByDays.Name = "nudMonthlyByDays";
            this.nudMonthlyByDays.Size = new System.Drawing.Size(79, 20);
            this.nudMonthlyByDays.TabIndex = 47;
            this.nudMonthlyByDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudMonthlyByDays.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ubtnScheduleMonths
            // 
            this.ubtnScheduleMonths.Location = new System.Drawing.Point(6, 100);
            this.ubtnScheduleMonths.Name = "ubtnScheduleMonths";
            this.ubtnScheduleMonths.Size = new System.Drawing.Size(104, 23);
            this.ubtnScheduleMonths.TabIndex = 46;
            this.ubtnScheduleMonths.Text = "Выбор месяцев";
            // 
            // uceWeekNumber
            // 
            appearance1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uceWeekNumber.Appearance = appearance1;
            this.uceWeekNumber.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uceWeekNumber.DisplayMember = "";
            this.uceWeekNumber.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = "по первым";
            valueListItem2.DataValue = "по вторым";
            valueListItem3.DataValue = "по третьим";
            valueListItem4.DataValue = "по четвертым";
            valueListItem5.DataValue = "по последним";
            this.uceWeekNumber.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3,
            valueListItem4,
            valueListItem5});
            this.uceWeekNumber.Location = new System.Drawing.Point(61, 64);
            this.uceWeekNumber.Name = "uceWeekNumber";
            this.uceWeekNumber.Size = new System.Drawing.Size(136, 21);
            this.uceWeekNumber.TabIndex = 45;
            this.uceWeekNumber.ValueMember = "";
            // 
            // uceWeekDay
            // 
            appearance2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uceWeekDay.Appearance = appearance2;
            this.uceWeekDay.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.uceWeekDay.DisplayMember = "";
            this.uceWeekDay.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem6.DataValue = "воскресеньям";
            valueListItem7.DataValue = "понедельникам";
            valueListItem7.DisplayText = "";
            valueListItem8.DataValue = "вторникам";
            valueListItem8.DisplayText = "";
            valueListItem9.DataValue = "средам";
            valueListItem9.DisplayText = "";
            valueListItem10.DataValue = "четвергам";
            valueListItem10.DisplayText = "";
            valueListItem11.DataValue = "пятницам";
            valueListItem11.DisplayText = "";
            valueListItem12.DataValue = "субботам";
            this.uceWeekDay.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem6,
            valueListItem7,
            valueListItem8,
            valueListItem9,
            valueListItem10,
            valueListItem11,
            valueListItem12});
            this.uceWeekDay.Location = new System.Drawing.Point(203, 64);
            this.uceWeekDay.Name = "uceWeekDay";
            this.uceWeekDay.Size = new System.Drawing.Size(133, 21);
            this.uceWeekDay.TabIndex = 44;
            this.uceWeekDay.ValueMember = "";
            // 
            // rbMonthlyByWeekDays
            // 
            this.rbMonthlyByWeekDays.AutoSize = true;
            this.rbMonthlyByWeekDays.Location = new System.Drawing.Point(6, 64);
            this.rbMonthlyByWeekDays.Name = "rbMonthlyByWeekDays";
            this.rbMonthlyByWeekDays.Size = new System.Drawing.Size(43, 17);
            this.rbMonthlyByWeekDays.TabIndex = 1;
            this.rbMonthlyByWeekDays.Text = "или";
            this.rbMonthlyByWeekDays.UseVisualStyleBackColor = true;
            // 
            // ultraLabel17
            // 
            appearance3.TextVAlignAsString = "Middle";
            this.ultraLabel17.Appearance = appearance3;
            this.ultraLabel17.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel17.Location = new System.Drawing.Point(342, 64);
            this.ultraLabel17.Name = "ultraLabel17";
            this.ultraLabel17.Size = new System.Drawing.Size(59, 23);
            this.ultraLabel17.TabIndex = 43;
            this.ultraLabel17.Text = "месяцев";
            // 
            // rbMonthlyByDayNumbers
            // 
            this.rbMonthlyByDayNumbers.AutoSize = true;
            this.rbMonthlyByDayNumbers.Checked = true;
            this.rbMonthlyByDayNumbers.Location = new System.Drawing.Point(6, 29);
            this.rbMonthlyByDayNumbers.Name = "rbMonthlyByDayNumbers";
            this.rbMonthlyByDayNumbers.Size = new System.Drawing.Size(37, 17);
            this.rbMonthlyByDayNumbers.TabIndex = 0;
            this.rbMonthlyByDayNumbers.TabStop = true;
            this.rbMonthlyByDayNumbers.Text = "по";
            this.rbMonthlyByDayNumbers.UseVisualStyleBackColor = true;
            // 
            // ultraLabel16
            // 
            appearance4.TextVAlignAsString = "Middle";
            this.ultraLabel16.Appearance = appearance4;
            this.ultraLabel16.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel16.Location = new System.Drawing.Point(157, 27);
            this.ultraLabel16.Name = "ultraLabel16";
            this.ultraLabel16.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel16.TabIndex = 42;
            this.ultraLabel16.Text = "числам месяцев";
            // 
            // ultraTabPageControl1
            // 
            this.ultraTabPageControl1.Controls.Add(this.ugPumpHistory);
            this.ultraTabPageControl1.Location = new System.Drawing.Point(1, 1);
            this.ultraTabPageControl1.Name = "ultraTabPageControl1";
            this.ultraTabPageControl1.Size = new System.Drawing.Size(776, 555);
            // 
            // ugPumpHistory
            // 
            this.ugPumpHistory.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.ugPumpHistory.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugPumpHistory.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ugPumpHistory.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.ugPumpHistory.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugPumpHistory.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ugPumpHistory.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            this.ugPumpHistory.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.ugPumpHistory.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
            this.ugPumpHistory.DisplayLayout.Scrollbars = Infragistics.Win.UltraWinGrid.Scrollbars.Both;
            this.ugPumpHistory.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugPumpHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugPumpHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ugPumpHistory.ImageList = this.imageList3;
            this.ugPumpHistory.Location = new System.Drawing.Point(0, 0);
            this.ugPumpHistory.Name = "ugPumpHistory";
            this.ugPumpHistory.Size = new System.Drawing.Size(776, 555);
            this.ugPumpHistory.TabIndex = 91;
            // 
            // imageList3
            // 
            this.imageList3.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList3.ImageStream")));
            this.imageList3.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList3.Images.SetKeyName(0, "Arrow2.bmp");
            this.imageList3.Images.SetKeyName(1, "Cross2.bmp");
            // 
            // ultraTabPageControl2
            // 
            this.ultraTabPageControl2.Controls.Add(this.ugDataSources);
            this.ultraTabPageControl2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabPageControl2.Name = "ultraTabPageControl2";
            this.ultraTabPageControl2.Size = new System.Drawing.Size(776, 555);
            // 
            // ugDataSources
            // 
            this.ugDataSources.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugDataSources.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ugDataSources.DisplayLayout.Override.AllowColMoving = Infragistics.Win.UltraWinGrid.AllowColMoving.NotAllowed;
            this.ugDataSources.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.ugDataSources.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ugDataSources.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ugDataSources.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.AllRowsInBand;
            this.ugDataSources.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.ugDataSources.DisplayLayout.Override.RowSizing = Infragistics.Win.UltraWinGrid.RowSizing.AutoFixed;
            this.ugDataSources.DisplayLayout.Scrollbars = Infragistics.Win.UltraWinGrid.Scrollbars.Both;
            this.ugDataSources.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ugDataSources.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugDataSources.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ugDataSources.ImageList = this.imageList3;
            this.ugDataSources.Location = new System.Drawing.Point(0, 0);
            this.ugDataSources.Name = "ugDataSources";
            this.ugDataSources.Size = new System.Drawing.Size(776, 555);
            this.ugDataSources.TabIndex = 92;
            // 
            // utpcPumpDataLog
            // 
            this.utpcPumpDataLog.Location = new System.Drawing.Point(1, 1);
            this.utpcPumpDataLog.Name = "utpcPumpDataLog";
            this.utpcPumpDataLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcProcessDataLog
            // 
            this.utpcProcessDataLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcProcessDataLog.Name = "utpcProcessDataLog";
            this.utpcProcessDataLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcAssociateDataLog
            // 
            this.utpcAssociateDataLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcAssociateDataLog.Name = "utpcAssociateDataLog";
            this.utpcAssociateDataLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcProcessCubeLog
            // 
            this.utpcProcessCubeLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcProcessCubeLog.Name = "utpcProcessCubeLog";
            this.utpcProcessCubeLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcCheckDataLog
            // 
            this.utpcCheckDataLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcCheckDataLog.Name = "utpcCheckDataLog";
            this.utpcCheckDataLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcDeleteDataLog
            // 
            this.utpcDeleteDataLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcDeleteDataLog.Name = "utpcDeleteDataLog";
            this.utpcDeleteDataLog.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcClassifiers
            // 
            this.utpcClassifiers.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcClassifiers.Name = "utpcClassifiers";
            this.utpcClassifiers.Size = new System.Drawing.Size(776, 535);
            // 
            // utpcPart1
            // 
            this.utpcPart1.Controls.Add(this.rtbPart1);
            this.utpcPart1.Location = new System.Drawing.Point(1, 20);
            this.utpcPart1.Name = "utpcPart1";
            this.utpcPart1.Size = new System.Drawing.Size(301, 395);
            // 
            // rtbPart1
            // 
            this.rtbPart1.BackColor = System.Drawing.SystemColors.Window;
            this.rtbPart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPart1.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbPart1.Location = new System.Drawing.Point(0, 0);
            this.rtbPart1.Name = "rtbPart1";
            this.rtbPart1.ReadOnly = true;
            this.rtbPart1.Size = new System.Drawing.Size(301, 395);
            this.rtbPart1.TabIndex = 0;
            this.rtbPart1.Text = "";
            this.rtbPart1.WordWrap = false;
            // 
            // utpcPart2
            // 
            this.utpcPart2.Controls.Add(this.rtbPart2);
            this.utpcPart2.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcPart2.Name = "utpcPart2";
            this.utpcPart2.Size = new System.Drawing.Size(301, 395);
            // 
            // rtbPart2
            // 
            this.rtbPart2.BackColor = System.Drawing.SystemColors.Window;
            this.rtbPart2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPart2.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbPart2.Location = new System.Drawing.Point(0, 0);
            this.rtbPart2.Name = "rtbPart2";
            this.rtbPart2.ReadOnly = true;
            this.rtbPart2.Size = new System.Drawing.Size(301, 395);
            this.rtbPart2.TabIndex = 0;
            this.rtbPart2.Text = "";
            this.rtbPart2.WordWrap = false;
            // 
            // utpcPart3
            // 
            this.utpcPart3.Controls.Add(this.rtbPart3);
            this.utpcPart3.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcPart3.Name = "utpcPart3";
            this.utpcPart3.Size = new System.Drawing.Size(301, 395);
            // 
            // rtbPart3
            // 
            this.rtbPart3.BackColor = System.Drawing.SystemColors.Window;
            this.rtbPart3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbPart3.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rtbPart3.Location = new System.Drawing.Point(0, 0);
            this.rtbPart3.Name = "rtbPart3";
            this.rtbPart3.ReadOnly = true;
            this.rtbPart3.Size = new System.Drawing.Size(301, 395);
            this.rtbPart3.TabIndex = 0;
            this.rtbPart3.Text = "";
            this.rtbPart3.WordWrap = false;
            // 
            // utpcPumpRuling
            // 
            this.utpcPumpRuling.AutoScroll = true;
            this.utpcPumpRuling.Controls.Add(this.upicPreviewDataResult);
            this.utpcPumpRuling.Controls.Add(this.upicPumpDataResult);
            this.utpcPumpRuling.Controls.Add(this.upicProcessDataResult);
            this.utpcPumpRuling.Controls.Add(this.upicAssociateDataResult);
            this.utpcPumpRuling.Controls.Add(this.upicProcessCubeResult);
            this.utpcPumpRuling.Controls.Add(this.upicCheckDataResult);
            this.utpcPumpRuling.Controls.Add(this.ulPreviewDataComment);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipPreviewData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopPreviewData);
            this.utpcPumpRuling.Controls.Add(this.ubtnPausePreviewData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartPreviewData);
            this.utpcPumpRuling.Controls.Add(this.ulPreviewDataMessage);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel28);
            this.utpcPumpRuling.Controls.Add(this.upicPreviewData);
            this.utpcPumpRuling.Controls.Add(this.ulPreviewDataEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulPreviewDataStartTime);
            this.utpcPumpRuling.Controls.Add(this.upbPreviewData);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel31);
            this.utpcPumpRuling.Controls.Add(this.ulPumpRuleMsg);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel4);
            this.utpcPumpRuling.Controls.Add(this.ulCheckDataComment);
            this.utpcPumpRuling.Controls.Add(this.ulProcessCubeComment);
            this.utpcPumpRuling.Controls.Add(this.ulAssociateDataComment);
            this.utpcPumpRuling.Controls.Add(this.ulProcessDataComment);
            this.utpcPumpRuling.Controls.Add(this.ulPumpDataComment);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipCheckData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopCheckData);
            this.utpcPumpRuling.Controls.Add(this.ubtnPauseCheckData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartCheckData);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipProcessCube);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopProcessCube);
            this.utpcPumpRuling.Controls.Add(this.ubtnPauseProcessCube);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartProcessCube);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipAssociateData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopAssociateData);
            this.utpcPumpRuling.Controls.Add(this.ubtnPauseAssociateData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartAssociateData);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipProcessData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopProcessData);
            this.utpcPumpRuling.Controls.Add(this.ubtnPauseProcessData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartProcessData);
            this.utpcPumpRuling.Controls.Add(this.ubtnSkipPumpData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStopPumpData);
            this.utpcPumpRuling.Controls.Add(this.ubtnPausePumpData);
            this.utpcPumpRuling.Controls.Add(this.ubtnStartPumpData);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel24);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel23);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel21);
            this.utpcPumpRuling.Controls.Add(this.ulPumpDataMessage);
            this.utpcPumpRuling.Controls.Add(this.ulCheckDataMessage);
            this.utpcPumpRuling.Controls.Add(this.ulProcessDataMessage);
            this.utpcPumpRuling.Controls.Add(this.ulAssociateDataMessage);
            this.utpcPumpRuling.Controls.Add(this.ulProcessCubeMessage);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel15);
            this.utpcPumpRuling.Controls.Add(this.upicPumpData);
            this.utpcPumpRuling.Controls.Add(this.upicProcessData);
            this.utpcPumpRuling.Controls.Add(this.upicAssociateData);
            this.utpcPumpRuling.Controls.Add(this.upicProcessCube);
            this.utpcPumpRuling.Controls.Add(this.upicCheckData);
            this.utpcPumpRuling.Controls.Add(this.ulCheckDataStartTime);
            this.utpcPumpRuling.Controls.Add(this.ulProcessDataStartTime);
            this.utpcPumpRuling.Controls.Add(this.ulAssociateDataStartTime);
            this.utpcPumpRuling.Controls.Add(this.ulProcessCubeStartTime);
            this.utpcPumpRuling.Controls.Add(this.ulPumpDataEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulCheckDataEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulProcessDataEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulAssociateDataEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulProcessCubeEndTime);
            this.utpcPumpRuling.Controls.Add(this.ulPumpDataStartTime);
            this.utpcPumpRuling.Controls.Add(this.upbProcessCube);
            this.utpcPumpRuling.Controls.Add(this.upbAssociateData);
            this.utpcPumpRuling.Controls.Add(this.upbProcessData);
            this.utpcPumpRuling.Controls.Add(this.upbCheckData);
            this.utpcPumpRuling.Controls.Add(this.upbPumpData);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel7);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel8);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel9);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel10);
            this.utpcPumpRuling.Controls.Add(this.ultraLabel11);
            this.utpcPumpRuling.Location = new System.Drawing.Point(1, 20);
            this.utpcPumpRuling.Name = "utpcPumpRuling";
            this.utpcPumpRuling.Size = new System.Drawing.Size(778, 594);
            // 
            // upicPreviewDataResult
            // 
            this.upicPreviewDataResult.BackColor = System.Drawing.Color.Transparent;
            this.upicPreviewDataResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicPreviewDataResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicPreviewDataResult.Location = new System.Drawing.Point(41, 62);
            this.upicPreviewDataResult.Name = "upicPreviewDataResult";
            this.upicPreviewDataResult.Size = new System.Drawing.Size(16, 16);
            this.upicPreviewDataResult.TabIndex = 105;
            this.upicPreviewDataResult.Tag = "21";
            // 
            // upicPumpDataResult
            // 
            this.upicPumpDataResult.BackColor = System.Drawing.Color.Transparent;
            this.upicPumpDataResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicPumpDataResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicPumpDataResult.Location = new System.Drawing.Point(41, 153);
            this.upicPumpDataResult.Name = "upicPumpDataResult";
            this.upicPumpDataResult.Size = new System.Drawing.Size(16, 16);
            this.upicPumpDataResult.TabIndex = 104;
            this.upicPumpDataResult.Tag = "22";
            // 
            // upicProcessDataResult
            // 
            this.upicProcessDataResult.BackColor = System.Drawing.Color.Transparent;
            this.upicProcessDataResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicProcessDataResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicProcessDataResult.Location = new System.Drawing.Point(41, 243);
            this.upicProcessDataResult.Name = "upicProcessDataResult";
            this.upicProcessDataResult.Size = new System.Drawing.Size(16, 16);
            this.upicProcessDataResult.TabIndex = 103;
            this.upicProcessDataResult.Tag = "23";
            // 
            // upicAssociateDataResult
            // 
            this.upicAssociateDataResult.BackColor = System.Drawing.Color.Transparent;
            this.upicAssociateDataResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicAssociateDataResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicAssociateDataResult.Location = new System.Drawing.Point(41, 335);
            this.upicAssociateDataResult.Name = "upicAssociateDataResult";
            this.upicAssociateDataResult.Size = new System.Drawing.Size(16, 16);
            this.upicAssociateDataResult.TabIndex = 102;
            this.upicAssociateDataResult.Tag = "24";
            // 
            // upicProcessCubeResult
            // 
            this.upicProcessCubeResult.BackColor = System.Drawing.Color.Transparent;
            this.upicProcessCubeResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicProcessCubeResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicProcessCubeResult.Location = new System.Drawing.Point(41, 425);
            this.upicProcessCubeResult.Name = "upicProcessCubeResult";
            this.upicProcessCubeResult.Size = new System.Drawing.Size(16, 16);
            this.upicProcessCubeResult.TabIndex = 101;
            this.upicProcessCubeResult.Tag = "25";
            // 
            // upicCheckDataResult
            // 
            this.upicCheckDataResult.BackColor = System.Drawing.Color.Transparent;
            this.upicCheckDataResult.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicCheckDataResult.ImageTransparentColor = System.Drawing.Color.White;
            this.upicCheckDataResult.Location = new System.Drawing.Point(42, 516);
            this.upicCheckDataResult.Name = "upicCheckDataResult";
            this.upicCheckDataResult.Size = new System.Drawing.Size(16, 16);
            this.upicCheckDataResult.TabIndex = 100;
            this.upicCheckDataResult.Tag = "26";
            // 
            // ulPreviewDataComment
            // 
            appearance5.TextHAlignAsString = "Left";
            appearance5.TextVAlignAsString = "Middle";
            this.ulPreviewDataComment.Appearance = appearance5;
            this.ulPreviewDataComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPreviewDataComment.Location = new System.Drawing.Point(6, 83);
            this.ulPreviewDataComment.Name = "ulPreviewDataComment";
            this.ulPreviewDataComment.Size = new System.Drawing.Size(742, 18);
            this.ulPreviewDataComment.TabIndex = 99;
            this.ulPreviewDataComment.Text = "Нет данных";
            // 
            // ubtnSkipPreviewData
            // 
            appearance6.Image = ((object)(resources.GetObject("appearance6.Image")));
            appearance6.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance6.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipPreviewData.Appearance = appearance6;
            this.ubtnSkipPreviewData.Enabled = false;
            this.ubtnSkipPreviewData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipPreviewData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipPreviewData.Location = new System.Drawing.Point(251, 28);
            this.ubtnSkipPreviewData.Name = "ubtnSkipPreviewData";
            this.ubtnSkipPreviewData.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipPreviewData.TabIndex = 98;
            this.ubtnSkipPreviewData.Tag = "13";
            // 
            // ubtnStopPreviewData
            // 
            appearance7.Image = ((object)(resources.GetObject("appearance7.Image")));
            appearance7.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance7.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopPreviewData.Appearance = appearance7;
            this.ubtnStopPreviewData.Enabled = false;
            this.ubtnStopPreviewData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopPreviewData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopPreviewData.Location = new System.Drawing.Point(215, 28);
            this.ubtnStopPreviewData.Name = "ubtnStopPreviewData";
            this.ubtnStopPreviewData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopPreviewData.TabIndex = 97;
            this.ubtnStopPreviewData.Tag = "12";
            // 
            // ubtnPausePreviewData
            // 
            appearance8.Image = ((object)(resources.GetObject("appearance8.Image")));
            appearance8.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance8.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPausePreviewData.Appearance = appearance8;
            this.ubtnPausePreviewData.Enabled = false;
            this.ubtnPausePreviewData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPausePreviewData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPausePreviewData.Location = new System.Drawing.Point(179, 28);
            this.ubtnPausePreviewData.Name = "ubtnPausePreviewData";
            this.ubtnPausePreviewData.Size = new System.Drawing.Size(36, 25);
            this.ubtnPausePreviewData.TabIndex = 96;
            this.ubtnPausePreviewData.Tag = "11";
            // 
            // ubtnStartPreviewData
            // 
            appearance9.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.StartBtn;
            appearance9.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance9.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartPreviewData.Appearance = appearance9;
            this.ubtnStartPreviewData.Enabled = false;
            this.ubtnStartPreviewData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartPreviewData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartPreviewData.Location = new System.Drawing.Point(143, 28);
            this.ubtnStartPreviewData.Name = "ubtnStartPreviewData";
            this.ubtnStartPreviewData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartPreviewData.TabIndex = 95;
            this.ubtnStartPreviewData.Tag = "10";
            // 
            // ulPreviewDataMessage
            // 
            appearance10.TextHAlignAsString = "Left";
            appearance10.TextVAlignAsString = "Middle";
            this.ulPreviewDataMessage.Appearance = appearance10;
            this.ulPreviewDataMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPreviewDataMessage.Location = new System.Drawing.Point(427, 22);
            this.ulPreviewDataMessage.Name = "ulPreviewDataMessage";
            this.ulPreviewDataMessage.Size = new System.Drawing.Size(325, 23);
            this.ulPreviewDataMessage.TabIndex = 94;
            this.ulPreviewDataMessage.Tag = "1";
            this.ulPreviewDataMessage.Text = "Нет данных";
            this.ulPreviewDataMessage.Visible = false;
            // 
            // ultraLabel28
            // 
            this.ultraLabel28.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel28.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel28.Location = new System.Drawing.Point(1, 107);
            this.ultraLabel28.Name = "ultraLabel28";
            this.ultraLabel28.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel28.TabIndex = 93;
            // 
            // upicPreviewData
            // 
            this.upicPreviewData.BackColor = System.Drawing.Color.Transparent;
            this.upicPreviewData.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicPreviewData.ImageTransparentColor = System.Drawing.Color.White;
            this.upicPreviewData.Location = new System.Drawing.Point(6, 62);
            this.upicPreviewData.Name = "upicPreviewData";
            this.upicPreviewData.Size = new System.Drawing.Size(16, 16);
            this.upicPreviewData.TabIndex = 92;
            this.upicPreviewData.Tag = "11";
            // 
            // ulPreviewDataEndTime
            // 
            appearance11.TextHAlignAsString = "Left";
            appearance11.TextVAlignAsString = "Middle";
            this.ulPreviewDataEndTime.Appearance = appearance11;
            this.ulPreviewDataEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPreviewDataEndTime.Location = new System.Drawing.Point(300, 52);
            this.ulPreviewDataEndTime.Name = "ulPreviewDataEndTime";
            this.ulPreviewDataEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulPreviewDataEndTime.TabIndex = 91;
            this.ulPreviewDataEndTime.Tag = "1";
            this.ulPreviewDataEndTime.Text = "Нет данных";
            this.ulPreviewDataEndTime.Visible = false;
            // 
            // ulPreviewDataStartTime
            // 
            appearance12.TextHAlignAsString = "Left";
            appearance12.TextVAlignAsString = "Middle";
            this.ulPreviewDataStartTime.Appearance = appearance12;
            this.ulPreviewDataStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPreviewDataStartTime.Location = new System.Drawing.Point(300, 22);
            this.ulPreviewDataStartTime.Name = "ulPreviewDataStartTime";
            this.ulPreviewDataStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulPreviewDataStartTime.TabIndex = 90;
            this.ulPreviewDataStartTime.Tag = "1";
            this.ulPreviewDataStartTime.Text = "Нет данных";
            this.ulPreviewDataStartTime.Visible = false;
            // 
            // upbPreviewData
            // 
            this.upbPreviewData.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance13.BackColor = System.Drawing.Color.RoyalBlue;
            appearance13.BackColor2 = System.Drawing.Color.White;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbPreviewData.FillAppearance = appearance13;
            this.upbPreviewData.Location = new System.Drawing.Point(427, 52);
            this.upbPreviewData.Name = "upbPreviewData";
            this.upbPreviewData.Size = new System.Drawing.Size(325, 23);
            this.upbPreviewData.TabIndex = 89;
            this.upbPreviewData.Text = "[Formatted]";
            this.upbPreviewData.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbPreviewData.Visible = false;
            // 
            // ultraLabel31
            // 
            appearance14.TextHAlignAsString = "Left";
            appearance14.TextVAlignAsString = "Middle";
            this.ultraLabel31.Appearance = appearance14;
            this.ultraLabel31.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel31.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel31.Location = new System.Drawing.Point(6, 22);
            this.ultraLabel31.Name = "ultraLabel31";
            this.ultraLabel31.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel31.TabIndex = 88;
            this.ultraLabel31.Text = "Предпросмотр";
            // 
            // ulPumpRuleMsg
            // 
            appearance15.TextVAlignAsString = "Middle";
            this.ulPumpRuleMsg.Appearance = appearance15;
            this.ulPumpRuleMsg.BackColorInternal = System.Drawing.Color.Khaki;
            this.ulPumpRuleMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.ulPumpRuleMsg.Location = new System.Drawing.Point(0, 0);
            this.ulPumpRuleMsg.Name = "ulPumpRuleMsg";
            this.ulPumpRuleMsg.Size = new System.Drawing.Size(778, 18);
            this.ulPumpRuleMsg.TabIndex = 87;
            // 
            // ultraLabel4
            // 
            this.ultraLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel4.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel4.Location = new System.Drawing.Point(1, 561);
            this.ultraLabel4.Name = "ultraLabel4";
            this.ultraLabel4.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel4.TabIndex = 86;
            // 
            // ulCheckDataComment
            // 
            appearance16.TextHAlignAsString = "Left";
            appearance16.TextVAlignAsString = "Middle";
            this.ulCheckDataComment.Appearance = appearance16;
            this.ulCheckDataComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulCheckDataComment.Location = new System.Drawing.Point(7, 537);
            this.ulCheckDataComment.Name = "ulCheckDataComment";
            this.ulCheckDataComment.Size = new System.Drawing.Size(742, 18);
            this.ulCheckDataComment.TabIndex = 85;
            this.ulCheckDataComment.Text = "Нет данных";
            // 
            // ulProcessCubeComment
            // 
            appearance17.TextHAlignAsString = "Left";
            appearance17.TextVAlignAsString = "Middle";
            this.ulProcessCubeComment.Appearance = appearance17;
            this.ulProcessCubeComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessCubeComment.Location = new System.Drawing.Point(6, 446);
            this.ulProcessCubeComment.Name = "ulProcessCubeComment";
            this.ulProcessCubeComment.Size = new System.Drawing.Size(742, 18);
            this.ulProcessCubeComment.TabIndex = 84;
            this.ulProcessCubeComment.Text = "Нет данных";
            // 
            // ulAssociateDataComment
            // 
            appearance18.TextHAlignAsString = "Left";
            appearance18.TextVAlignAsString = "Middle";
            this.ulAssociateDataComment.Appearance = appearance18;
            this.ulAssociateDataComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulAssociateDataComment.Location = new System.Drawing.Point(6, 356);
            this.ulAssociateDataComment.Name = "ulAssociateDataComment";
            this.ulAssociateDataComment.Size = new System.Drawing.Size(742, 18);
            this.ulAssociateDataComment.TabIndex = 83;
            this.ulAssociateDataComment.Text = "Нет данных";
            // 
            // ulProcessDataComment
            // 
            appearance19.TextHAlignAsString = "Left";
            appearance19.TextVAlignAsString = "Middle";
            this.ulProcessDataComment.Appearance = appearance19;
            this.ulProcessDataComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessDataComment.Location = new System.Drawing.Point(6, 264);
            this.ulProcessDataComment.Name = "ulProcessDataComment";
            this.ulProcessDataComment.Size = new System.Drawing.Size(742, 18);
            this.ulProcessDataComment.TabIndex = 82;
            this.ulProcessDataComment.Text = "Нет данных";
            // 
            // ulPumpDataComment
            // 
            appearance20.TextHAlignAsString = "Left";
            appearance20.TextVAlignAsString = "Middle";
            this.ulPumpDataComment.Appearance = appearance20;
            this.ulPumpDataComment.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPumpDataComment.Location = new System.Drawing.Point(6, 174);
            this.ulPumpDataComment.Name = "ulPumpDataComment";
            this.ulPumpDataComment.Size = new System.Drawing.Size(742, 18);
            this.ulPumpDataComment.TabIndex = 80;
            this.ulPumpDataComment.Text = "Нет данных";
            // 
            // ubtnSkipCheckData
            // 
            appearance21.Image = ((object)(resources.GetObject("appearance21.Image")));
            appearance21.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance21.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipCheckData.Appearance = appearance21;
            this.ubtnSkipCheckData.Enabled = false;
            this.ubtnSkipCheckData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipCheckData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipCheckData.Location = new System.Drawing.Point(251, 482);
            this.ubtnSkipCheckData.Name = "ubtnSkipCheckData";
            this.ubtnSkipCheckData.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipCheckData.TabIndex = 79;
            this.ubtnSkipCheckData.Tag = "63";
            // 
            // ubtnStopCheckData
            // 
            appearance22.Image = ((object)(resources.GetObject("appearance22.Image")));
            appearance22.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance22.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopCheckData.Appearance = appearance22;
            this.ubtnStopCheckData.Enabled = false;
            this.ubtnStopCheckData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopCheckData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopCheckData.Location = new System.Drawing.Point(215, 482);
            this.ubtnStopCheckData.Name = "ubtnStopCheckData";
            this.ubtnStopCheckData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopCheckData.TabIndex = 74;
            this.ubtnStopCheckData.Tag = "62";
            // 
            // ubtnPauseCheckData
            // 
            appearance23.Image = ((object)(resources.GetObject("appearance23.Image")));
            appearance23.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance23.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPauseCheckData.Appearance = appearance23;
            this.ubtnPauseCheckData.Enabled = false;
            this.ubtnPauseCheckData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPauseCheckData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPauseCheckData.Location = new System.Drawing.Point(179, 482);
            this.ubtnPauseCheckData.Name = "ubtnPauseCheckData";
            this.ubtnPauseCheckData.Size = new System.Drawing.Size(36, 25);
            this.ubtnPauseCheckData.TabIndex = 73;
            this.ubtnPauseCheckData.Tag = "61";
            // 
            // ubtnStartCheckData
            // 
            appearance24.Image = ((object)(resources.GetObject("appearance24.Image")));
            appearance24.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance24.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartCheckData.Appearance = appearance24;
            this.ubtnStartCheckData.Enabled = false;
            this.ubtnStartCheckData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartCheckData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartCheckData.Location = new System.Drawing.Point(143, 482);
            this.ubtnStartCheckData.Name = "ubtnStartCheckData";
            this.ubtnStartCheckData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartCheckData.TabIndex = 72;
            this.ubtnStartCheckData.Tag = "60";
            // 
            // ubtnSkipProcessCube
            // 
            appearance25.Image = ((object)(resources.GetObject("appearance25.Image")));
            appearance25.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance25.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipProcessCube.Appearance = appearance25;
            this.ubtnSkipProcessCube.Enabled = false;
            this.ubtnSkipProcessCube.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipProcessCube.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipProcessCube.Location = new System.Drawing.Point(251, 391);
            this.ubtnSkipProcessCube.Name = "ubtnSkipProcessCube";
            this.ubtnSkipProcessCube.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipProcessCube.TabIndex = 71;
            this.ubtnSkipProcessCube.Tag = "53";
            // 
            // ubtnStopProcessCube
            // 
            appearance26.Image = ((object)(resources.GetObject("appearance26.Image")));
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance26.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopProcessCube.Appearance = appearance26;
            this.ubtnStopProcessCube.Enabled = false;
            this.ubtnStopProcessCube.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopProcessCube.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopProcessCube.Location = new System.Drawing.Point(215, 391);
            this.ubtnStopProcessCube.Name = "ubtnStopProcessCube";
            this.ubtnStopProcessCube.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopProcessCube.TabIndex = 70;
            this.ubtnStopProcessCube.Tag = "52";
            // 
            // ubtnPauseProcessCube
            // 
            appearance27.Image = ((object)(resources.GetObject("appearance27.Image")));
            appearance27.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance27.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPauseProcessCube.Appearance = appearance27;
            this.ubtnPauseProcessCube.Enabled = false;
            this.ubtnPauseProcessCube.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPauseProcessCube.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPauseProcessCube.Location = new System.Drawing.Point(179, 391);
            this.ubtnPauseProcessCube.Name = "ubtnPauseProcessCube";
            this.ubtnPauseProcessCube.Size = new System.Drawing.Size(36, 25);
            this.ubtnPauseProcessCube.TabIndex = 69;
            this.ubtnPauseProcessCube.Tag = "51";
            // 
            // ubtnStartProcessCube
            // 
            appearance28.Image = ((object)(resources.GetObject("appearance28.Image")));
            appearance28.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance28.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartProcessCube.Appearance = appearance28;
            this.ubtnStartProcessCube.Enabled = false;
            this.ubtnStartProcessCube.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartProcessCube.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartProcessCube.Location = new System.Drawing.Point(143, 391);
            this.ubtnStartProcessCube.Name = "ubtnStartProcessCube";
            this.ubtnStartProcessCube.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartProcessCube.TabIndex = 68;
            this.ubtnStartProcessCube.Tag = "50";
            // 
            // ubtnSkipAssociateData
            // 
            appearance29.Image = ((object)(resources.GetObject("appearance29.Image")));
            appearance29.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance29.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipAssociateData.Appearance = appearance29;
            this.ubtnSkipAssociateData.Enabled = false;
            this.ubtnSkipAssociateData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipAssociateData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipAssociateData.Location = new System.Drawing.Point(251, 302);
            this.ubtnSkipAssociateData.Name = "ubtnSkipAssociateData";
            this.ubtnSkipAssociateData.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipAssociateData.TabIndex = 67;
            this.ubtnSkipAssociateData.Tag = "43";
            // 
            // ubtnStopAssociateData
            // 
            appearance30.Image = ((object)(resources.GetObject("appearance30.Image")));
            appearance30.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance30.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopAssociateData.Appearance = appearance30;
            this.ubtnStopAssociateData.Enabled = false;
            this.ubtnStopAssociateData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopAssociateData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopAssociateData.Location = new System.Drawing.Point(215, 302);
            this.ubtnStopAssociateData.Name = "ubtnStopAssociateData";
            this.ubtnStopAssociateData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopAssociateData.TabIndex = 66;
            this.ubtnStopAssociateData.Tag = "42";
            // 
            // ubtnPauseAssociateData
            // 
            appearance31.Image = ((object)(resources.GetObject("appearance31.Image")));
            appearance31.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance31.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPauseAssociateData.Appearance = appearance31;
            this.ubtnPauseAssociateData.Enabled = false;
            this.ubtnPauseAssociateData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPauseAssociateData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPauseAssociateData.Location = new System.Drawing.Point(179, 302);
            this.ubtnPauseAssociateData.Name = "ubtnPauseAssociateData";
            this.ubtnPauseAssociateData.Size = new System.Drawing.Size(36, 25);
            this.ubtnPauseAssociateData.TabIndex = 65;
            this.ubtnPauseAssociateData.Tag = "41";
            // 
            // ubtnStartAssociateData
            // 
            appearance32.Image = ((object)(resources.GetObject("appearance32.Image")));
            appearance32.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance32.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartAssociateData.Appearance = appearance32;
            this.ubtnStartAssociateData.Enabled = false;
            this.ubtnStartAssociateData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartAssociateData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartAssociateData.Location = new System.Drawing.Point(143, 302);
            this.ubtnStartAssociateData.Name = "ubtnStartAssociateData";
            this.ubtnStartAssociateData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartAssociateData.TabIndex = 64;
            this.ubtnStartAssociateData.Tag = "40";
            // 
            // ubtnSkipProcessData
            // 
            appearance33.Image = ((object)(resources.GetObject("appearance33.Image")));
            appearance33.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance33.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipProcessData.Appearance = appearance33;
            this.ubtnSkipProcessData.Enabled = false;
            this.ubtnSkipProcessData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipProcessData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipProcessData.Location = new System.Drawing.Point(251, 209);
            this.ubtnSkipProcessData.Name = "ubtnSkipProcessData";
            this.ubtnSkipProcessData.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipProcessData.TabIndex = 63;
            this.ubtnSkipProcessData.Tag = "33";
            // 
            // ubtnStopProcessData
            // 
            appearance34.Image = ((object)(resources.GetObject("appearance34.Image")));
            appearance34.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance34.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopProcessData.Appearance = appearance34;
            this.ubtnStopProcessData.Enabled = false;
            this.ubtnStopProcessData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopProcessData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopProcessData.Location = new System.Drawing.Point(215, 209);
            this.ubtnStopProcessData.Name = "ubtnStopProcessData";
            this.ubtnStopProcessData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopProcessData.TabIndex = 62;
            this.ubtnStopProcessData.Tag = "32";
            // 
            // ubtnPauseProcessData
            // 
            appearance35.Image = ((object)(resources.GetObject("appearance35.Image")));
            appearance35.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance35.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPauseProcessData.Appearance = appearance35;
            this.ubtnPauseProcessData.Enabled = false;
            this.ubtnPauseProcessData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPauseProcessData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPauseProcessData.Location = new System.Drawing.Point(179, 209);
            this.ubtnPauseProcessData.Name = "ubtnPauseProcessData";
            this.ubtnPauseProcessData.Size = new System.Drawing.Size(36, 25);
            this.ubtnPauseProcessData.TabIndex = 61;
            this.ubtnPauseProcessData.Tag = "31";
            // 
            // ubtnStartProcessData
            // 
            appearance36.Image = ((object)(resources.GetObject("appearance36.Image")));
            appearance36.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance36.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartProcessData.Appearance = appearance36;
            this.ubtnStartProcessData.Enabled = false;
            this.ubtnStartProcessData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartProcessData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartProcessData.Location = new System.Drawing.Point(143, 209);
            this.ubtnStartProcessData.Name = "ubtnStartProcessData";
            this.ubtnStartProcessData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartProcessData.TabIndex = 60;
            this.ubtnStartProcessData.Tag = "30";
            // 
            // ubtnSkipPumpData
            // 
            appearance37.Image = ((object)(resources.GetObject("appearance37.Image")));
            appearance37.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance37.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnSkipPumpData.Appearance = appearance37;
            this.ubtnSkipPumpData.Enabled = false;
            this.ubtnSkipPumpData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnSkipPumpData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnSkipPumpData.Location = new System.Drawing.Point(251, 119);
            this.ubtnSkipPumpData.Name = "ubtnSkipPumpData";
            this.ubtnSkipPumpData.Size = new System.Drawing.Size(36, 25);
            this.ubtnSkipPumpData.TabIndex = 59;
            this.ubtnSkipPumpData.Tag = "23";
            // 
            // ubtnStopPumpData
            // 
            appearance38.Image = ((object)(resources.GetObject("appearance38.Image")));
            appearance38.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance38.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStopPumpData.Appearance = appearance38;
            this.ubtnStopPumpData.Enabled = false;
            this.ubtnStopPumpData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStopPumpData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStopPumpData.Location = new System.Drawing.Point(215, 119);
            this.ubtnStopPumpData.Name = "ubtnStopPumpData";
            this.ubtnStopPumpData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStopPumpData.TabIndex = 58;
            this.ubtnStopPumpData.Tag = "22";
            // 
            // ubtnPausePumpData
            // 
            appearance39.Image = ((object)(resources.GetObject("appearance39.Image")));
            appearance39.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance39.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnPausePumpData.Appearance = appearance39;
            this.ubtnPausePumpData.Enabled = false;
            this.ubtnPausePumpData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnPausePumpData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnPausePumpData.Location = new System.Drawing.Point(179, 119);
            this.ubtnPausePumpData.Name = "ubtnPausePumpData";
            this.ubtnPausePumpData.Size = new System.Drawing.Size(36, 25);
            this.ubtnPausePumpData.TabIndex = 57;
            this.ubtnPausePumpData.Tag = "21";
            // 
            // ubtnStartPumpData
            // 
            appearance40.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.StartBtn;
            appearance40.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance40.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnStartPumpData.Appearance = appearance40;
            this.ubtnStartPumpData.Enabled = false;
            this.ubtnStartPumpData.ImageSize = new System.Drawing.Size(32, 23);
            this.ubtnStartPumpData.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnStartPumpData.Location = new System.Drawing.Point(143, 119);
            this.ubtnStartPumpData.Name = "ubtnStartPumpData";
            this.ubtnStartPumpData.Size = new System.Drawing.Size(36, 25);
            this.ubtnStartPumpData.TabIndex = 45;
            this.ubtnStartPumpData.Tag = "20";
            // 
            // ultraLabel24
            // 
            this.ultraLabel24.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel24.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel24.Location = new System.Drawing.Point(0, 470);
            this.ultraLabel24.Name = "ultraLabel24";
            this.ultraLabel24.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel24.TabIndex = 44;
            // 
            // ultraLabel23
            // 
            this.ultraLabel23.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel23.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel23.Location = new System.Drawing.Point(0, 379);
            this.ultraLabel23.Name = "ultraLabel23";
            this.ultraLabel23.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel23.TabIndex = 43;
            // 
            // ultraLabel21
            // 
            this.ultraLabel21.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel21.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel21.Location = new System.Drawing.Point(0, 288);
            this.ultraLabel21.Name = "ultraLabel21";
            this.ultraLabel21.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel21.TabIndex = 41;
            // 
            // ulPumpDataMessage
            // 
            appearance41.TextHAlignAsString = "Left";
            appearance41.TextVAlignAsString = "Middle";
            this.ulPumpDataMessage.Appearance = appearance41;
            this.ulPumpDataMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPumpDataMessage.Location = new System.Drawing.Point(427, 113);
            this.ulPumpDataMessage.Name = "ulPumpDataMessage";
            this.ulPumpDataMessage.Size = new System.Drawing.Size(325, 23);
            this.ulPumpDataMessage.TabIndex = 40;
            this.ulPumpDataMessage.Tag = "2";
            this.ulPumpDataMessage.Text = "Нет данных";
            this.ulPumpDataMessage.Visible = false;
            // 
            // ulCheckDataMessage
            // 
            appearance42.TextHAlignAsString = "Left";
            appearance42.TextVAlignAsString = "Middle";
            this.ulCheckDataMessage.Appearance = appearance42;
            this.ulCheckDataMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulCheckDataMessage.Location = new System.Drawing.Point(428, 476);
            this.ulCheckDataMessage.Name = "ulCheckDataMessage";
            this.ulCheckDataMessage.Size = new System.Drawing.Size(325, 23);
            this.ulCheckDataMessage.TabIndex = 39;
            this.ulCheckDataMessage.Tag = "6";
            this.ulCheckDataMessage.Text = "Нет данных";
            this.ulCheckDataMessage.Visible = false;
            // 
            // ulProcessDataMessage
            // 
            appearance43.TextHAlignAsString = "Left";
            appearance43.TextVAlignAsString = "Middle";
            this.ulProcessDataMessage.Appearance = appearance43;
            this.ulProcessDataMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessDataMessage.Location = new System.Drawing.Point(427, 204);
            this.ulProcessDataMessage.Name = "ulProcessDataMessage";
            this.ulProcessDataMessage.Size = new System.Drawing.Size(325, 23);
            this.ulProcessDataMessage.TabIndex = 38;
            this.ulProcessDataMessage.Tag = "3";
            this.ulProcessDataMessage.Text = "Нет данных";
            this.ulProcessDataMessage.Visible = false;
            // 
            // ulAssociateDataMessage
            // 
            appearance44.TextHAlignAsString = "Left";
            appearance44.TextVAlignAsString = "Middle";
            this.ulAssociateDataMessage.Appearance = appearance44;
            this.ulAssociateDataMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulAssociateDataMessage.Location = new System.Drawing.Point(430, 296);
            this.ulAssociateDataMessage.Name = "ulAssociateDataMessage";
            this.ulAssociateDataMessage.Size = new System.Drawing.Size(325, 23);
            this.ulAssociateDataMessage.TabIndex = 37;
            this.ulAssociateDataMessage.Tag = "4";
            this.ulAssociateDataMessage.Text = "Нет данных";
            this.ulAssociateDataMessage.Visible = false;
            // 
            // ulProcessCubeMessage
            // 
            appearance45.TextHAlignAsString = "Left";
            appearance45.TextVAlignAsString = "Middle";
            this.ulProcessCubeMessage.Appearance = appearance45;
            this.ulProcessCubeMessage.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessCubeMessage.Location = new System.Drawing.Point(427, 385);
            this.ulProcessCubeMessage.Name = "ulProcessCubeMessage";
            this.ulProcessCubeMessage.Size = new System.Drawing.Size(325, 23);
            this.ulProcessCubeMessage.TabIndex = 36;
            this.ulProcessCubeMessage.Tag = "5";
            this.ulProcessCubeMessage.Text = "Нет данных";
            this.ulProcessCubeMessage.Visible = false;
            // 
            // ultraLabel15
            // 
            this.ultraLabel15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel15.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel15.Location = new System.Drawing.Point(1, 198);
            this.ultraLabel15.Name = "ultraLabel15";
            this.ultraLabel15.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel15.TabIndex = 35;
            // 
            // upicPumpData
            // 
            this.upicPumpData.BackColor = System.Drawing.Color.Transparent;
            this.upicPumpData.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicPumpData.ImageTransparentColor = System.Drawing.Color.White;
            this.upicPumpData.Location = new System.Drawing.Point(6, 153);
            this.upicPumpData.Name = "upicPumpData";
            this.upicPumpData.Size = new System.Drawing.Size(16, 16);
            this.upicPumpData.TabIndex = 34;
            this.upicPumpData.Tag = "12";
            // 
            // upicProcessData
            // 
            this.upicProcessData.BackColor = System.Drawing.Color.Transparent;
            this.upicProcessData.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicProcessData.ImageTransparentColor = System.Drawing.Color.White;
            this.upicProcessData.Location = new System.Drawing.Point(6, 243);
            this.upicProcessData.Name = "upicProcessData";
            this.upicProcessData.Size = new System.Drawing.Size(16, 16);
            this.upicProcessData.TabIndex = 33;
            this.upicProcessData.Tag = "13";
            // 
            // upicAssociateData
            // 
            this.upicAssociateData.BackColor = System.Drawing.Color.Transparent;
            this.upicAssociateData.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicAssociateData.ImageTransparentColor = System.Drawing.Color.White;
            this.upicAssociateData.Location = new System.Drawing.Point(6, 335);
            this.upicAssociateData.Name = "upicAssociateData";
            this.upicAssociateData.Size = new System.Drawing.Size(16, 16);
            this.upicAssociateData.TabIndex = 32;
            this.upicAssociateData.Tag = "14";
            // 
            // upicProcessCube
            // 
            this.upicProcessCube.BackColor = System.Drawing.Color.Transparent;
            this.upicProcessCube.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicProcessCube.ImageTransparentColor = System.Drawing.Color.White;
            this.upicProcessCube.Location = new System.Drawing.Point(6, 425);
            this.upicProcessCube.Name = "upicProcessCube";
            this.upicProcessCube.Size = new System.Drawing.Size(16, 16);
            this.upicProcessCube.TabIndex = 31;
            this.upicProcessCube.Tag = "15";
            // 
            // upicCheckData
            // 
            this.upicCheckData.BackColor = System.Drawing.Color.Transparent;
            this.upicCheckData.BorderShadowColor = System.Drawing.Color.Empty;
            this.upicCheckData.ImageTransparentColor = System.Drawing.Color.White;
            this.upicCheckData.Location = new System.Drawing.Point(7, 516);
            this.upicCheckData.Name = "upicCheckData";
            this.upicCheckData.Size = new System.Drawing.Size(16, 16);
            this.upicCheckData.TabIndex = 30;
            this.upicCheckData.Tag = "16";
            // 
            // ulCheckDataStartTime
            // 
            appearance46.TextHAlignAsString = "Left";
            appearance46.TextVAlignAsString = "Middle";
            this.ulCheckDataStartTime.Appearance = appearance46;
            this.ulCheckDataStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulCheckDataStartTime.Location = new System.Drawing.Point(301, 476);
            this.ulCheckDataStartTime.Name = "ulCheckDataStartTime";
            this.ulCheckDataStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulCheckDataStartTime.TabIndex = 28;
            this.ulCheckDataStartTime.Tag = "6";
            this.ulCheckDataStartTime.Text = "Нет данных";
            this.ulCheckDataStartTime.Visible = false;
            // 
            // ulProcessDataStartTime
            // 
            appearance47.TextHAlignAsString = "Left";
            appearance47.TextVAlignAsString = "Middle";
            this.ulProcessDataStartTime.Appearance = appearance47;
            this.ulProcessDataStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessDataStartTime.Location = new System.Drawing.Point(300, 203);
            this.ulProcessDataStartTime.Name = "ulProcessDataStartTime";
            this.ulProcessDataStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulProcessDataStartTime.TabIndex = 27;
            this.ulProcessDataStartTime.Tag = "3";
            this.ulProcessDataStartTime.Text = "Нет данных";
            this.ulProcessDataStartTime.Visible = false;
            // 
            // ulAssociateDataStartTime
            // 
            appearance48.TextHAlignAsString = "Left";
            appearance48.TextVAlignAsString = "Middle";
            this.ulAssociateDataStartTime.Appearance = appearance48;
            this.ulAssociateDataStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulAssociateDataStartTime.Location = new System.Drawing.Point(300, 294);
            this.ulAssociateDataStartTime.Name = "ulAssociateDataStartTime";
            this.ulAssociateDataStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulAssociateDataStartTime.TabIndex = 26;
            this.ulAssociateDataStartTime.Tag = "4";
            this.ulAssociateDataStartTime.Text = "Нет данных";
            this.ulAssociateDataStartTime.Visible = false;
            // 
            // ulProcessCubeStartTime
            // 
            appearance49.TextHAlignAsString = "Left";
            appearance49.TextVAlignAsString = "Middle";
            this.ulProcessCubeStartTime.Appearance = appearance49;
            this.ulProcessCubeStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessCubeStartTime.Location = new System.Drawing.Point(300, 385);
            this.ulProcessCubeStartTime.Name = "ulProcessCubeStartTime";
            this.ulProcessCubeStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulProcessCubeStartTime.TabIndex = 25;
            this.ulProcessCubeStartTime.Tag = "5";
            this.ulProcessCubeStartTime.Text = "Нет данных";
            this.ulProcessCubeStartTime.Visible = false;
            // 
            // ulPumpDataEndTime
            // 
            appearance50.TextHAlignAsString = "Left";
            appearance50.TextVAlignAsString = "Middle";
            this.ulPumpDataEndTime.Appearance = appearance50;
            this.ulPumpDataEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPumpDataEndTime.Location = new System.Drawing.Point(300, 143);
            this.ulPumpDataEndTime.Name = "ulPumpDataEndTime";
            this.ulPumpDataEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulPumpDataEndTime.TabIndex = 24;
            this.ulPumpDataEndTime.Tag = "2";
            this.ulPumpDataEndTime.Text = "Нет данных";
            this.ulPumpDataEndTime.Visible = false;
            // 
            // ulCheckDataEndTime
            // 
            appearance51.TextHAlignAsString = "Left";
            appearance51.TextVAlignAsString = "Middle";
            this.ulCheckDataEndTime.Appearance = appearance51;
            this.ulCheckDataEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulCheckDataEndTime.Location = new System.Drawing.Point(301, 506);
            this.ulCheckDataEndTime.Name = "ulCheckDataEndTime";
            this.ulCheckDataEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulCheckDataEndTime.TabIndex = 23;
            this.ulCheckDataEndTime.Tag = "6";
            this.ulCheckDataEndTime.Text = "Нет данных";
            this.ulCheckDataEndTime.Visible = false;
            // 
            // ulProcessDataEndTime
            // 
            appearance52.TextHAlignAsString = "Left";
            appearance52.TextVAlignAsString = "Middle";
            this.ulProcessDataEndTime.Appearance = appearance52;
            this.ulProcessDataEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessDataEndTime.Location = new System.Drawing.Point(300, 234);
            this.ulProcessDataEndTime.Name = "ulProcessDataEndTime";
            this.ulProcessDataEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulProcessDataEndTime.TabIndex = 22;
            this.ulProcessDataEndTime.Tag = "3";
            this.ulProcessDataEndTime.Text = "Нет данных";
            this.ulProcessDataEndTime.Visible = false;
            // 
            // ulAssociateDataEndTime
            // 
            appearance53.TextHAlignAsString = "Left";
            appearance53.TextVAlignAsString = "Middle";
            this.ulAssociateDataEndTime.Appearance = appearance53;
            this.ulAssociateDataEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulAssociateDataEndTime.Location = new System.Drawing.Point(300, 326);
            this.ulAssociateDataEndTime.Name = "ulAssociateDataEndTime";
            this.ulAssociateDataEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulAssociateDataEndTime.TabIndex = 21;
            this.ulAssociateDataEndTime.Tag = "4";
            this.ulAssociateDataEndTime.Text = "Нет данных";
            this.ulAssociateDataEndTime.Visible = false;
            // 
            // ulProcessCubeEndTime
            // 
            appearance54.TextHAlignAsString = "Left";
            appearance54.TextVAlignAsString = "Middle";
            this.ulProcessCubeEndTime.Appearance = appearance54;
            this.ulProcessCubeEndTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulProcessCubeEndTime.Location = new System.Drawing.Point(300, 416);
            this.ulProcessCubeEndTime.Name = "ulProcessCubeEndTime";
            this.ulProcessCubeEndTime.Size = new System.Drawing.Size(110, 23);
            this.ulProcessCubeEndTime.TabIndex = 20;
            this.ulProcessCubeEndTime.Tag = "5";
            this.ulProcessCubeEndTime.Text = "Нет данных";
            this.ulProcessCubeEndTime.Visible = false;
            // 
            // ulPumpDataStartTime
            // 
            appearance55.TextHAlignAsString = "Left";
            appearance55.TextVAlignAsString = "Middle";
            this.ulPumpDataStartTime.Appearance = appearance55;
            this.ulPumpDataStartTime.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulPumpDataStartTime.Location = new System.Drawing.Point(300, 113);
            this.ulPumpDataStartTime.Name = "ulPumpDataStartTime";
            this.ulPumpDataStartTime.Size = new System.Drawing.Size(110, 23);
            this.ulPumpDataStartTime.TabIndex = 19;
            this.ulPumpDataStartTime.Tag = "2";
            this.ulPumpDataStartTime.Text = "Нет данных";
            this.ulPumpDataStartTime.Visible = false;
            // 
            // upbProcessCube
            // 
            this.upbProcessCube.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance56.BackColor = System.Drawing.Color.RoyalBlue;
            appearance56.BackColor2 = System.Drawing.Color.White;
            appearance56.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbProcessCube.FillAppearance = appearance56;
            this.upbProcessCube.Location = new System.Drawing.Point(427, 415);
            this.upbProcessCube.Name = "upbProcessCube";
            this.upbProcessCube.Size = new System.Drawing.Size(325, 23);
            this.upbProcessCube.TabIndex = 14;
            this.upbProcessCube.Text = "[Formatted]";
            this.upbProcessCube.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbProcessCube.Visible = false;
            // 
            // upbAssociateData
            // 
            this.upbAssociateData.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance57.BackColor = System.Drawing.Color.RoyalBlue;
            appearance57.BackColor2 = System.Drawing.Color.White;
            appearance57.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbAssociateData.FillAppearance = appearance57;
            this.upbAssociateData.Location = new System.Drawing.Point(430, 327);
            this.upbAssociateData.Name = "upbAssociateData";
            this.upbAssociateData.Size = new System.Drawing.Size(325, 23);
            this.upbAssociateData.TabIndex = 13;
            this.upbAssociateData.Text = "[Formatted]";
            this.upbAssociateData.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbAssociateData.Visible = false;
            // 
            // upbProcessData
            // 
            appearance58.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.upbProcessData.Appearance = appearance58;
            this.upbProcessData.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance59.BackColor = System.Drawing.Color.RoyalBlue;
            appearance59.BackColor2 = System.Drawing.Color.White;
            appearance59.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbProcessData.FillAppearance = appearance59;
            this.upbProcessData.Location = new System.Drawing.Point(427, 234);
            this.upbProcessData.Name = "upbProcessData";
            this.upbProcessData.Size = new System.Drawing.Size(325, 23);
            this.upbProcessData.TabIndex = 12;
            this.upbProcessData.Text = "[Formatted]";
            this.upbProcessData.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbProcessData.Visible = false;
            // 
            // upbCheckData
            // 
            this.upbCheckData.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance60.BackColor = System.Drawing.Color.RoyalBlue;
            appearance60.BackColor2 = System.Drawing.Color.White;
            appearance60.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbCheckData.FillAppearance = appearance60;
            this.upbCheckData.Location = new System.Drawing.Point(428, 505);
            this.upbCheckData.Name = "upbCheckData";
            this.upbCheckData.Size = new System.Drawing.Size(325, 23);
            this.upbCheckData.TabIndex = 11;
            this.upbCheckData.Text = "[Formatted]";
            this.upbCheckData.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbCheckData.Visible = false;
            // 
            // upbPumpData
            // 
            this.upbPumpData.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance61.BackColor = System.Drawing.Color.RoyalBlue;
            appearance61.BackColor2 = System.Drawing.Color.White;
            appearance61.BackGradientStyle = Infragistics.Win.GradientStyle.BackwardDiagonal;
            this.upbPumpData.FillAppearance = appearance61;
            this.upbPumpData.Location = new System.Drawing.Point(427, 143);
            this.upbPumpData.Name = "upbPumpData";
            this.upbPumpData.Size = new System.Drawing.Size(325, 23);
            this.upbPumpData.TabIndex = 10;
            this.upbPumpData.Text = "[Formatted]";
            this.upbPumpData.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            this.upbPumpData.Visible = false;
            // 
            // ultraLabel7
            // 
            appearance62.TextHAlignAsString = "Left";
            appearance62.TextVAlignAsString = "Middle";
            this.ultraLabel7.Appearance = appearance62;
            this.ultraLabel7.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel7.Location = new System.Drawing.Point(6, 385);
            this.ultraLabel7.Name = "ultraLabel7";
            this.ultraLabel7.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel7.TabIndex = 9;
            this.ultraLabel7.Text = "Расчет кубов";
            // 
            // ultraLabel8
            // 
            appearance63.TextHAlignAsString = "Left";
            appearance63.TextVAlignAsString = "Middle";
            this.ultraLabel8.Appearance = appearance63;
            this.ultraLabel8.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel8.Location = new System.Drawing.Point(6, 294);
            this.ultraLabel8.Name = "ultraLabel8";
            this.ultraLabel8.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel8.TabIndex = 8;
            this.ultraLabel8.Text = "Сопоставление";
            // 
            // ultraLabel9
            // 
            appearance64.TextHAlignAsString = "Left";
            appearance64.TextVAlignAsString = "Middle";
            this.ultraLabel9.Appearance = appearance64;
            this.ultraLabel9.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel9.Location = new System.Drawing.Point(6, 203);
            this.ultraLabel9.Name = "ultraLabel9";
            this.ultraLabel9.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel9.TabIndex = 7;
            this.ultraLabel9.Text = "Обработка данных";
            // 
            // ultraLabel10
            // 
            appearance65.TextHAlignAsString = "Left";
            appearance65.TextVAlignAsString = "Middle";
            this.ultraLabel10.Appearance = appearance65;
            this.ultraLabel10.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel10.Location = new System.Drawing.Point(7, 476);
            this.ultraLabel10.Name = "ultraLabel10";
            this.ultraLabel10.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel10.TabIndex = 6;
            this.ultraLabel10.Text = "Проверка данных";
            // 
            // ultraLabel11
            // 
            appearance66.TextHAlignAsString = "Left";
            appearance66.TextVAlignAsString = "Middle";
            this.ultraLabel11.Appearance = appearance66;
            this.ultraLabel11.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel11.Location = new System.Drawing.Point(6, 113);
            this.ultraLabel11.Name = "ultraLabel11";
            this.ultraLabel11.Size = new System.Drawing.Size(130, 35);
            this.ultraLabel11.TabIndex = 5;
            this.ultraLabel11.Text = "Закачка данных";
            // 
            // utpcPumpParams
            // 
            this.utpcPumpParams.Controls.Add(this.splitContainer2);
            this.utpcPumpParams.Controls.Add(this.ultraLabel14);
            this.utpcPumpParams.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcPumpParams.Name = "utpcPumpParams";
            this.utpcPumpParams.Size = new System.Drawing.Size(778, 594);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.scGeneralParams);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.scIndividualParams);
            this.splitContainer2.Size = new System.Drawing.Size(778, 594);
            this.splitContainer2.SplitterDistance = 122;
            this.splitContainer2.TabIndex = 5;
            this.splitContainer2.Text = "splitContainer2";
            // 
            // scGeneralParams
            // 
            this.scGeneralParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scGeneralParams.IsSplitterFixed = true;
            this.scGeneralParams.Location = new System.Drawing.Point(0, 0);
            this.scGeneralParams.Name = "scGeneralParams";
            this.scGeneralParams.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scGeneralParams.Panel1
            // 
            this.scGeneralParams.Panel1.Controls.Add(this.ultraLabel3);
            // 
            // scGeneralParams.Panel2
            // 
            this.scGeneralParams.Panel2.Controls.Add(this.pGeneralPumpParams);
            this.scGeneralParams.Size = new System.Drawing.Size(778, 122);
            this.scGeneralParams.SplitterDistance = 34;
            this.scGeneralParams.TabIndex = 3;
            this.scGeneralParams.Text = "splitContainer4";
            // 
            // ultraLabel3
            // 
            appearance67.TextVAlignAsString = "Middle";
            this.ultraLabel3.Appearance = appearance67;
            this.ultraLabel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel3.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel3.Name = "ultraLabel3";
            this.ultraLabel3.Size = new System.Drawing.Size(778, 34);
            this.ultraLabel3.TabIndex = 44;
            this.ultraLabel3.Text = "Общие параметры закачки";
            // 
            // pGeneralPumpParams
            // 
            this.pGeneralPumpParams.AutoScroll = true;
            this.pGeneralPumpParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pGeneralPumpParams.Location = new System.Drawing.Point(0, 0);
            this.pGeneralPumpParams.Name = "pGeneralPumpParams";
            this.pGeneralPumpParams.Size = new System.Drawing.Size(778, 84);
            this.pGeneralPumpParams.TabIndex = 3;
            // 
            // scIndividualParams
            // 
            this.scIndividualParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scIndividualParams.IsSplitterFixed = true;
            this.scIndividualParams.Location = new System.Drawing.Point(0, 0);
            this.scIndividualParams.Name = "scIndividualParams";
            this.scIndividualParams.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scIndividualParams.Panel1
            // 
            this.scIndividualParams.Panel1.Controls.Add(this.ultraLabel2);
            this.scIndividualParams.Panel1.Controls.Add(this.ultraLabel1);
            // 
            // scIndividualParams.Panel2
            // 
            this.scIndividualParams.Panel2.Controls.Add(this.pIndividualPumpParams);
            this.scIndividualParams.Size = new System.Drawing.Size(778, 468);
            this.scIndividualParams.SplitterDistance = 35;
            this.scIndividualParams.TabIndex = 2;
            this.scIndividualParams.Text = "scIndividualParams";
            // 
            // ultraLabel2
            // 
            appearance68.TextVAlignAsString = "Middle";
            this.ultraLabel2.Appearance = appearance68;
            this.ultraLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ultraLabel2.Location = new System.Drawing.Point(0, 1);
            this.ultraLabel2.Name = "ultraLabel2";
            this.ultraLabel2.Size = new System.Drawing.Size(778, 34);
            this.ultraLabel2.TabIndex = 46;
            this.ultraLabel2.Text = "Индивидуальные параметры закачки";
            // 
            // ultraLabel1
            // 
            this.ultraLabel1.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.ultraLabel1.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel1.Name = "ultraLabel1";
            this.ultraLabel1.Size = new System.Drawing.Size(778, 1);
            this.ultraLabel1.TabIndex = 45;
            // 
            // pIndividualPumpParams
            // 
            this.pIndividualPumpParams.AutoScroll = true;
            this.pIndividualPumpParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pIndividualPumpParams.Location = new System.Drawing.Point(0, 0);
            this.pIndividualPumpParams.Name = "pIndividualPumpParams";
            this.pIndividualPumpParams.Size = new System.Drawing.Size(778, 429);
            this.pIndividualPumpParams.TabIndex = 2;
            // 
            // ultraLabel14
            // 
            this.ultraLabel14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel14.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel14.Location = new System.Drawing.Point(0, 1105);
            this.ultraLabel14.Name = "ultraLabel14";
            this.ultraLabel14.Size = new System.Drawing.Size(777, 1);
            this.ultraLabel14.TabIndex = 2;
            // 
            // utpcSchedule
            // 
            this.utpcSchedule.Controls.Add(this.splitContainer3);
            this.utpcSchedule.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcSchedule.Name = "utpcSchedule";
            this.utpcSchedule.Size = new System.Drawing.Size(778, 594);
            // 
            // splitContainer3
            // 
            this.splitContainer3.BackColor = System.Drawing.Color.Transparent;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.AutoScroll = true;
            this.splitContainer3.Panel1.Controls.Add(this.ubtnCancelSchedule);
            this.splitContainer3.Panel1.Controls.Add(this.ubtnApplySchedule);
            this.splitContainer3.Panel1.Controls.Add(this.ultraLabel12);
            this.splitContainer3.Panel1.Controls.Add(this.udteScheduleStartDate);
            this.splitContainer3.Panel1.Controls.Add(this.udteScheduleStartTime);
            this.splitContainer3.Panel1.Controls.Add(this.uceSchedulePeriod);
            this.splitContainer3.Panel1.Controls.Add(this.ultraLabel13);
            this.splitContainer3.Panel1.Controls.Add(this.ultraLabel6);
            this.splitContainer3.Panel1.Controls.Add(this.ulSchedulerInfo);
            this.splitContainer3.Panel1.Controls.Add(this.ultraLabel5);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.AutoScroll = true;
            this.splitContainer3.Panel2.Controls.Add(this.utcSchedulePeriod);
            this.splitContainer3.Size = new System.Drawing.Size(778, 594);
            this.splitContainer3.SplitterDistance = 118;
            this.splitContainer3.TabIndex = 48;
            // 
            // ubtnCancelSchedule
            // 
            this.ubtnCancelSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance69.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Cross2;
            appearance69.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance69.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnCancelSchedule.Appearance = appearance69;
            this.ubtnCancelSchedule.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnCancelSchedule.Location = new System.Drawing.Point(607, 81);
            this.ubtnCancelSchedule.Name = "ubtnCancelSchedule";
            this.ubtnCancelSchedule.Size = new System.Drawing.Size(159, 23);
            this.ubtnCancelSchedule.TabIndex = 59;
            this.ubtnCancelSchedule.Text = "Отменить расписание";
            // 
            // ubtnApplySchedule
            // 
            this.ubtnApplySchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance70.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Check3;
            appearance70.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance70.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnApplySchedule.Appearance = appearance70;
            this.ubtnApplySchedule.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnApplySchedule.Location = new System.Drawing.Point(607, 52);
            this.ubtnApplySchedule.Name = "ubtnApplySchedule";
            this.ubtnApplySchedule.Size = new System.Drawing.Size(159, 23);
            this.ubtnApplySchedule.TabIndex = 58;
            this.ubtnApplySchedule.Text = "Принять расписание";
            // 
            // ultraLabel12
            // 
            appearance71.TextVAlignAsString = "Middle";
            this.ultraLabel12.Appearance = appearance71;
            this.ultraLabel12.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel12.Location = new System.Drawing.Point(190, 52);
            this.ultraLabel12.Name = "ultraLabel12";
            this.ultraLabel12.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel12.TabIndex = 57;
            this.ultraLabel12.Text = "Дата начала";
            // 
            // udteScheduleStartDate
            // 
            this.udteScheduleStartDate.DateTime = new System.DateTime(2006, 6, 8, 0, 0, 0, 0);
            this.udteScheduleStartDate.Location = new System.Drawing.Point(190, 80);
            this.udteScheduleStartDate.MaskInput = "{LOC}dd/mm/yyyy";
            this.udteScheduleStartDate.Name = "udteScheduleStartDate";
            this.udteScheduleStartDate.Size = new System.Drawing.Size(108, 21);
            this.udteScheduleStartDate.TabIndex = 56;
            this.udteScheduleStartDate.Value = new System.DateTime(2006, 6, 8, 0, 0, 0, 0);
            // 
            // udteScheduleStartTime
            // 
            appearance72.TextHAlignAsString = "Center";
            this.udteScheduleStartTime.Appearance = appearance72;
            this.udteScheduleStartTime.DateTime = new System.DateTime(2006, 6, 8, 0, 0, 0, 0);
            this.udteScheduleStartTime.DropDownButtonDisplayStyle = Infragistics.Win.ButtonDisplayStyle.Never;
            this.udteScheduleStartTime.Location = new System.Drawing.Point(322, 80);
            this.udteScheduleStartTime.MaskInput = "{LOC}hh:mm tt";
            this.udteScheduleStartTime.Name = "udteScheduleStartTime";
            this.udteScheduleStartTime.Size = new System.Drawing.Size(100, 21);
            this.udteScheduleStartTime.TabIndex = 55;
            this.udteScheduleStartTime.Value = new System.DateTime(2006, 6, 8, 0, 0, 0, 0);
            // 
            // uceSchedulePeriod
            // 
            appearance73.Cursor = System.Windows.Forms.Cursors.Default;
            this.uceSchedulePeriod.Appearance = appearance73;
            this.uceSchedulePeriod.Cursor = System.Windows.Forms.Cursors.Default;
            this.uceSchedulePeriod.DisplayMember = "";
            this.uceSchedulePeriod.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem13.DataValue = "Однократно";
            valueListItem14.DataValue = "Ежедневно";
            valueListItem15.DataValue = "Еженедельно";
            valueListItem16.DataValue = "Ежемесячно";
            valueListItem17.DataValue = "Ежечасно";
            this.uceSchedulePeriod.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem13,
            valueListItem14,
            valueListItem15,
            valueListItem16, 
            valueListItem17 });
            this.uceSchedulePeriod.Location = new System.Drawing.Point(14, 81);
            this.uceSchedulePeriod.Name = "uceSchedulePeriod";
            this.uceSchedulePeriod.Size = new System.Drawing.Size(152, 21);
            this.uceSchedulePeriod.TabIndex = 53;
            this.uceSchedulePeriod.ValueMember = "";
            // 
            // ultraLabel13
            // 
            appearance74.TextVAlignAsString = "Middle";
            this.ultraLabel13.Appearance = appearance74;
            this.ultraLabel13.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel13.Location = new System.Drawing.Point(14, 52);
            this.ultraLabel13.Name = "ultraLabel13";
            this.ultraLabel13.Size = new System.Drawing.Size(130, 23);
            this.ultraLabel13.TabIndex = 52;
            this.ultraLabel13.Text = "Назначить задание";
            // 
            // ultraLabel6
            // 
            appearance75.TextVAlignAsString = "Middle";
            this.ultraLabel6.Appearance = appearance75;
            this.ultraLabel6.BackColorInternal = System.Drawing.Color.Transparent;
            this.ultraLabel6.Location = new System.Drawing.Point(322, 52);
            this.ultraLabel6.Name = "ultraLabel6";
            this.ultraLabel6.Size = new System.Drawing.Size(100, 23);
            this.ultraLabel6.TabIndex = 50;
            this.ultraLabel6.Text = "Время начала";
            // 
            // ulSchedulerInfo
            // 
            this.ulSchedulerInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            appearance76.TextVAlignAsString = "Middle";
            this.ulSchedulerInfo.Appearance = appearance76;
            this.ulSchedulerInfo.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulSchedulerInfo.Location = new System.Drawing.Point(14, 11);
            this.ulSchedulerInfo.Name = "ulSchedulerInfo";
            this.ulSchedulerInfo.Size = new System.Drawing.Size(734, 23);
            this.ulSchedulerInfo.TabIndex = 48;
            this.ulSchedulerInfo.Tag = "1";
            this.ulSchedulerInfo.Text = "Расписание не установлено";
            // 
            // ultraLabel5
            // 
            this.ultraLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraLabel5.BackColorInternal = System.Drawing.Color.Gray;
            this.ultraLabel5.Location = new System.Drawing.Point(0, 45);
            this.ultraLabel5.Name = "ultraLabel5";
            this.ultraLabel5.Size = new System.Drawing.Size(775, 1);
            this.ultraLabel5.TabIndex = 47;
            // 
            // utcSchedulePeriod
            // 
            this.utcSchedulePeriod.Controls.Add(this.ultraTabSharedControlsPage2);
            this.utcSchedulePeriod.Controls.Add(this.utpcScheduleMonthly);
            this.utcSchedulePeriod.Controls.Add(this.utpcScheduleOnce);
            this.utcSchedulePeriod.Controls.Add(this.utpcScheduleDaily);
            this.utcSchedulePeriod.Controls.Add(this.utpcScheduleHour);
            this.utcSchedulePeriod.Controls.Add(this.utpcScheduleWeekly);
            this.utcSchedulePeriod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcSchedulePeriod.Location = new System.Drawing.Point(0, 0);
            this.utcSchedulePeriod.Name = "utcSchedulePeriod";
            this.utcSchedulePeriod.SharedControlsPage = this.ultraTabSharedControlsPage2;
            this.utcSchedulePeriod.ShowButtonSeparators = true;
            this.utcSchedulePeriod.ShowTabListButton = Infragistics.Win.DefaultableBoolean.False;
            this.utcSchedulePeriod.Size = new System.Drawing.Size(778, 472);
            this.utcSchedulePeriod.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.utcSchedulePeriod.TabIndex = 0;
            ultraTab1.Key = "Once";
            ultraTab1.TabPage = this.utpcScheduleOnce;
            ultraTab1.Text = "Once";
            ultraTab2.Key = "Daily";
            ultraTab2.TabPage = this.utpcScheduleDaily;
            ultraTab2.Text = "Daily";
            ultraTab3.Key = "Weekly";
            ultraTab3.TabPage = this.utpcScheduleWeekly;
            ultraTab3.Text = "Weekly";
            ultraTab4.Key = "Monthly";
            ultraTab4.TabPage = this.utpcScheduleMonthly;
            ultraTab4.Text = "Monthly";
            ultraTab50.Key = "Hour";
            ultraTab50.TabPage = this.utpcScheduleHour;
            ultraTab50.Text = "Hour";
            this.utcSchedulePeriod.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4,
            ultraTab50});
            // 
            // ultraTabSharedControlsPage2
            // 
            this.ultraTabSharedControlsPage2.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage2.Name = "ultraTabSharedControlsPage2";
            this.ultraTabSharedControlsPage2.Size = new System.Drawing.Size(778, 472);
            // 
            // utpcFileManager
            // 
            this.utpcFileManager.Controls.Add(this.scBrowsers);
            this.utpcFileManager.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcFileManager.Name = "utpcFileManager";
            this.utpcFileManager.Size = new System.Drawing.Size(778, 594);
            // 
            // scBrowsers
            // 
            this.scBrowsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scBrowsers.Location = new System.Drawing.Point(0, 0);
            this.scBrowsers.Name = "scBrowsers";
            // 
            // scBrowsers.Panel1
            // 
            this.scBrowsers.Panel1.BackColor = System.Drawing.Color.Transparent;
            this.scBrowsers.Panel1.Controls.Add(this.wbLeftBrowser);
            this.scBrowsers.Panel1.Controls.Add(this.toolStrip1);
            this.scBrowsers.Panel1.Controls.Add(this.tsLeftBrowserCaption);
            // 
            // scBrowsers.Panel2
            // 
            this.scBrowsers.Panel2.BackColor = System.Drawing.Color.Transparent;
            this.scBrowsers.Panel2.Controls.Add(this.wbRightBrowser);
            this.scBrowsers.Panel2.Controls.Add(this.toolStrip2);
            this.scBrowsers.Panel2.Controls.Add(this.tsRightBrowserCaption);
            this.scBrowsers.Size = new System.Drawing.Size(778, 594);
            this.scBrowsers.SplitterDistance = 389;
            this.scBrowsers.TabIndex = 0;
            this.scBrowsers.Text = "splitContainer1";
            // 
            // wbLeftBrowser
            // 
            this.wbLeftBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbLeftBrowser.Location = new System.Drawing.Point(0, 50);
            this.wbLeftBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbLeftBrowser.Name = "wbLeftBrowser";
            this.wbLeftBrowser.Size = new System.Drawing.Size(389, 544);
            this.wbLeftBrowser.TabIndex = 9;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbLeftBack,
            this.tsbLeftForward,
            this.tsbLeftRefresh,
            this.tsbLeftHome,
            this.toolStripSeparator1,
            this.tsbLeftDrives,
            this.tsbLeftUp,
            this.toolStripSeparator3,
            this.tsbLeftView});
            this.toolStrip1.Location = new System.Drawing.Point(0, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(389, 25);
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbLeftBack
            // 
            this.tsbLeftBack.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Back;
            this.tsbLeftBack.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbLeftBack.Name = "tsbLeftBack";
            this.tsbLeftBack.Size = new System.Drawing.Size(70, 22);
            this.tsbLeftBack.Text = "Назад";
            // 
            // tsbLeftForward
            // 
            this.tsbLeftForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftForward.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Forward;
            this.tsbLeftForward.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbLeftForward.Name = "tsbLeftForward";
            this.tsbLeftForward.Size = new System.Drawing.Size(32, 22);
            this.tsbLeftForward.ToolTipText = "Вперед";
            // 
            // tsbLeftRefresh
            // 
            this.tsbLeftRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbLeftRefresh.Image")));
            this.tsbLeftRefresh.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbLeftRefresh.Name = "tsbLeftRefresh";
            this.tsbLeftRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbLeftRefresh.Text = "toolStripButton3";
            this.tsbLeftRefresh.ToolTipText = "Обновить";
            // 
            // tsbLeftHome
            // 
            this.tsbLeftHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftHome.Image = ((System.Drawing.Image)(resources.GetObject("tsbLeftHome.Image")));
            this.tsbLeftHome.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbLeftHome.Name = "tsbLeftHome";
            this.tsbLeftHome.Size = new System.Drawing.Size(23, 22);
            this.tsbLeftHome.Text = "toolStripButton4";
            this.tsbLeftHome.ToolTipText = "Данные для закачки";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbLeftDrives
            // 
            this.tsbLeftDrives.Image = ((System.Drawing.Image)(resources.GetObject("tsbLeftDrives.Image")));
            this.tsbLeftDrives.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbLeftDrives.Name = "tsbLeftDrives";
            this.tsbLeftDrives.Size = new System.Drawing.Size(32, 22);
            this.tsbLeftDrives.ToolTipText = "Диски";
            // 
            // tsbLeftUp
            // 
            this.tsbLeftUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftUp.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Up;
            this.tsbLeftUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLeftUp.Name = "tsbLeftUp";
            this.tsbLeftUp.Size = new System.Drawing.Size(23, 22);
            this.tsbLeftUp.Text = "toolStripButton1";
            this.tsbLeftUp.ToolTipText = "Вверх";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbLeftView
            // 
            this.tsbLeftView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLeftView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.списокToolStripMenuItem,
            this.таблицаToolStripMenuItem});
            this.tsbLeftView.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.TableView;
            this.tsbLeftView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLeftView.Name = "tsbLeftView";
            this.tsbLeftView.Size = new System.Drawing.Size(29, 22);
            this.tsbLeftView.Text = "toolStripDropDownButton1";
            this.tsbLeftView.ToolTipText = "Вид";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.IconsView;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem1.Text = "Значки";
            // 
            // списокToolStripMenuItem
            // 
            this.списокToolStripMenuItem.Checked = true;
            this.списокToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.списокToolStripMenuItem.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.ListView;
            this.списокToolStripMenuItem.Name = "списокToolStripMenuItem";
            this.списокToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.списокToolStripMenuItem.Text = "Список";
            // 
            // таблицаToolStripMenuItem
            // 
            this.таблицаToolStripMenuItem.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.TableView;
            this.таблицаToolStripMenuItem.Name = "таблицаToolStripMenuItem";
            this.таблицаToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.таблицаToolStripMenuItem.Text = "Таблица";
            // 
            // tsLeftBrowserCaption
            // 
            this.tsLeftBrowserCaption.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tstbLeftBrowserCaption});
            this.tsLeftBrowserCaption.Location = new System.Drawing.Point(0, 0);
            this.tsLeftBrowserCaption.Name = "tsLeftBrowserCaption";
            this.tsLeftBrowserCaption.Size = new System.Drawing.Size(389, 25);
            this.tsLeftBrowserCaption.TabIndex = 1;
            this.tsLeftBrowserCaption.Text = "toolStrip3";
            // 
            // tstbLeftBrowserCaption
            // 
            this.tstbLeftBrowserCaption.AutoSize = false;
            this.tstbLeftBrowserCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tstbLeftBrowserCaption.Name = "tstbLeftBrowserCaption";
            this.tstbLeftBrowserCaption.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tstbLeftBrowserCaption.ReadOnly = true;
            this.tstbLeftBrowserCaption.Size = new System.Drawing.Size(365, 25);
            this.tstbLeftBrowserCaption.Text = "Данные для закачки";
            // 
            // wbRightBrowser
            // 
            this.wbRightBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbRightBrowser.Location = new System.Drawing.Point(0, 50);
            this.wbRightBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbRightBrowser.Name = "wbRightBrowser";
            this.wbRightBrowser.Size = new System.Drawing.Size(385, 544);
            this.wbRightBrowser.TabIndex = 14;
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbRightBack,
            this.tsbRightForward,
            this.tsbRightRefresh,
            this.tsbRightHome,
            this.toolStripSeparator2,
            this.tsbRightDrives,
            this.tsbRightUp,
            this.toolStripSeparator4,
            this.tsbRightView});
            this.toolStrip2.Location = new System.Drawing.Point(0, 25);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(385, 25);
            this.toolStrip2.TabIndex = 13;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // tsbRightBack
            // 
            this.tsbRightBack.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Back;
            this.tsbRightBack.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbRightBack.Name = "tsbRightBack";
            this.tsbRightBack.Size = new System.Drawing.Size(70, 22);
            this.tsbRightBack.Text = "Назад";
            // 
            // tsbRightForward
            // 
            this.tsbRightForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightForward.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Forward;
            this.tsbRightForward.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbRightForward.Name = "tsbRightForward";
            this.tsbRightForward.Size = new System.Drawing.Size(32, 22);
            this.tsbRightForward.ToolTipText = "Вперед";
            // 
            // tsbRightRefresh
            // 
            this.tsbRightRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRightRefresh.Image")));
            this.tsbRightRefresh.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbRightRefresh.Name = "tsbRightRefresh";
            this.tsbRightRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbRightRefresh.ToolTipText = "Обновить";
            // 
            // tsbRightHome
            // 
            this.tsbRightHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightHome.Image = ((System.Drawing.Image)(resources.GetObject("tsbRightHome.Image")));
            this.tsbRightHome.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbRightHome.Name = "tsbRightHome";
            this.tsbRightHome.Size = new System.Drawing.Size(23, 22);
            this.tsbRightHome.ToolTipText = "Данные для закачки";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbRightDrives
            // 
            this.tsbRightDrives.Image = ((System.Drawing.Image)(resources.GetObject("tsbRightDrives.Image")));
            this.tsbRightDrives.ImageTransparentColor = System.Drawing.Color.Black;
            this.tsbRightDrives.Name = "tsbRightDrives";
            this.tsbRightDrives.Size = new System.Drawing.Size(32, 22);
            this.tsbRightDrives.ToolTipText = "Диски";
            // 
            // tsbRightUp
            // 
            this.tsbRightUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightUp.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Up;
            this.tsbRightUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRightUp.Name = "tsbRightUp";
            this.tsbRightUp.Size = new System.Drawing.Size(23, 22);
            this.tsbRightUp.ToolTipText = "Вверх";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbRightView
            // 
            this.tsbRightView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRightView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.tsbRightView.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.TableView;
            this.tsbRightView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRightView.Name = "tsbRightView";
            this.tsbRightView.Size = new System.Drawing.Size(29, 22);
            this.tsbRightView.ToolTipText = "Вид";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.IconsView;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem2.Text = "Значки";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Checked = true;
            this.toolStripMenuItem3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem3.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.ListView;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem3.Text = "Список";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.TableView;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(127, 22);
            this.toolStripMenuItem4.Text = "Таблица";
            // 
            // tsRightBrowserCaption
            // 
            this.tsRightBrowserCaption.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tstbRightBrowserCaption});
            this.tsRightBrowserCaption.Location = new System.Drawing.Point(0, 0);
            this.tsRightBrowserCaption.Name = "tsRightBrowserCaption";
            this.tsRightBrowserCaption.Size = new System.Drawing.Size(385, 25);
            this.tsRightBrowserCaption.TabIndex = 12;
            this.tsRightBrowserCaption.Text = "toolStrip4";
            // 
            // tstbRightBrowserCaption
            // 
            this.tstbRightBrowserCaption.AutoSize = false;
            this.tstbRightBrowserCaption.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tstbRightBrowserCaption.Name = "tstbRightBrowserCaption";
            this.tstbRightBrowserCaption.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tstbRightBrowserCaption.ReadOnly = true;
            this.tstbRightBrowserCaption.Size = new System.Drawing.Size(365, 25);
            this.tstbRightBrowserCaption.Text = "Данные для закачки";
            // 
            // utpcExecutedOps
            // 
            this.utpcExecutedOps.Controls.Add(this.ultraTabControl1);
            this.utpcExecutedOps.Controls.Add(this.ulHistoryMsg);
            this.utpcExecutedOps.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcExecutedOps.Name = "utpcExecutedOps";
            this.utpcExecutedOps.Size = new System.Drawing.Size(778, 594);
            // 
            // ultraTabControl1
            // 
            this.ultraTabControl1.Controls.Add(this.ultraTabSharedControlsPage7);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl1);
            this.ultraTabControl1.Controls.Add(this.ultraTabPageControl2);
            this.ultraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraTabControl1.Location = new System.Drawing.Point(0, 18);
            this.ultraTabControl1.Name = "ultraTabControl1";
            this.ultraTabControl1.SharedControlsPage = this.ultraTabSharedControlsPage7;
            this.ultraTabControl1.Size = new System.Drawing.Size(778, 576);
            this.ultraTabControl1.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.ultraTabControl1.TabIndex = 91;
            this.ultraTabControl1.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.BottomLeft;
            ultraTab5.Key = "PumpHistory";
            ultraTab5.TabPage = this.ultraTabPageControl1;
            ultraTab5.Text = "Закачки-источники";
            ultraTab6.Key = "DataSources";
            ultraTab6.TabPage = this.ultraTabPageControl2;
            ultraTab6.Text = "Источники-закачки";
            this.ultraTabControl1.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab5,
            ultraTab6});
            // 
            // ultraTabSharedControlsPage7
            // 
            this.ultraTabSharedControlsPage7.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage7.Name = "ultraTabSharedControlsPage7";
            this.ultraTabSharedControlsPage7.Size = new System.Drawing.Size(776, 555);
            // 
            // ulHistoryMsg
            // 
            appearance77.TextVAlignAsString = "Middle";
            this.ulHistoryMsg.Appearance = appearance77;
            this.ulHistoryMsg.BackColorInternal = System.Drawing.Color.Khaki;
            this.ulHistoryMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.ulHistoryMsg.Location = new System.Drawing.Point(0, 0);
            this.ulHistoryMsg.Name = "ulHistoryMsg";
            this.ulHistoryMsg.Size = new System.Drawing.Size(778, 18);
            this.ulHistoryMsg.TabIndex = 89;
            // 
            // utpcLog
            // 
            this.utpcLog.Controls.Add(this.panel5);
            this.utpcLog.Controls.Add(this.panel4);
            this.utpcLog.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcLog.Name = "utpcLog";
            this.utpcLog.Size = new System.Drawing.Size(778, 594);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.utcLogSwitcher);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 38);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(778, 556);
            this.panel5.TabIndex = 13;
            // 
            // utcLogSwitcher
            // 
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage3);
            this.utcLogSwitcher.Controls.Add(this.ultraTabSharedControlsPage4);
            this.utcLogSwitcher.Controls.Add(this.utpcPumpDataLog);
            this.utcLogSwitcher.Controls.Add(this.utpcCheckDataLog);
            this.utcLogSwitcher.Controls.Add(this.utpcProcessDataLog);
            this.utcLogSwitcher.Controls.Add(this.utpcAssociateDataLog);
            this.utcLogSwitcher.Controls.Add(this.utpcProcessCubeLog);
            this.utcLogSwitcher.Controls.Add(this.utpcDeleteDataLog);
            this.utcLogSwitcher.Controls.Add(this.utpcClassifiers);
            this.utcLogSwitcher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcLogSwitcher.Location = new System.Drawing.Point(0, 0);
            this.utcLogSwitcher.Name = "utcLogSwitcher";
            this.utcLogSwitcher.SharedControlsPage = this.ultraTabSharedControlsPage3;
            this.utcLogSwitcher.Size = new System.Drawing.Size(778, 556);
            this.utcLogSwitcher.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcLogSwitcher.TabIndex = 11;
            this.utcLogSwitcher.TabOrientation = Infragistics.Win.UltraWinTabs.TabOrientation.BottomLeft;
            ultraTab7.Key = "PumpData";
            ultraTab7.TabPage = this.utpcPumpDataLog;
            ultraTab7.Text = "Закачка данных";
            ultraTab8.Key = "ProcessData";
            ultraTab8.TabPage = this.utpcProcessDataLog;
            ultraTab8.Text = "Обработка данных";
            ultraTab9.Key = "AssociateData";
            ultraTab9.TabPage = this.utpcAssociateDataLog;
            ultraTab9.Text = "Сопоставление данных";
            ultraTab10.Key = "ProcessCube";
            ultraTab10.TabPage = this.utpcProcessCubeLog;
            ultraTab10.Text = "Расчет кубов";
            ultraTab11.Key = "CheckData";
            ultraTab11.TabPage = this.utpcCheckDataLog;
            ultraTab11.Text = "Проверка данных";
            ultraTab12.Key = "DeleteData";
            ultraTab12.TabPage = this.utpcDeleteDataLog;
            ultraTab12.Text = "Удаление данных";
            ultraTab13.Key = "Classifiers";
            ultraTab13.TabPage = this.utpcClassifiers;
            ultraTab13.Text = "Классификаторы и таблицы";
            this.utcLogSwitcher.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab7,
            ultraTab8,
            ultraTab9,
            ultraTab10,
            ultraTab11,
            ultraTab12,
            ultraTab13});
            // 
            // ultraTabSharedControlsPage3
            // 
            this.ultraTabSharedControlsPage3.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage3.Name = "ultraTabSharedControlsPage3";
            this.ultraTabSharedControlsPage3.Size = new System.Drawing.Size(776, 535);
            // 
            // ultraTabSharedControlsPage4
            // 
            this.ultraTabSharedControlsPage4.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage4.Name = "ultraTabSharedControlsPage4";
            this.ultraTabSharedControlsPage4.Size = new System.Drawing.Size(413, 444);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.ulHistoryInfo);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(778, 38);
            this.panel4.TabIndex = 12;
            // 
            // ulHistoryInfo
            // 
            appearance78.TextVAlignAsString = "Middle";
            this.ulHistoryInfo.Appearance = appearance78;
            this.ulHistoryInfo.BackColorInternal = System.Drawing.Color.Transparent;
            this.ulHistoryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ulHistoryInfo.Location = new System.Drawing.Point(0, 0);
            this.ulHistoryInfo.Name = "ulHistoryInfo";
            this.ulHistoryInfo.Padding = new System.Drawing.Size(5, 5);
            this.ulHistoryInfo.Size = new System.Drawing.Size(778, 38);
            this.ulHistoryInfo.TabIndex = 89;
            // 
            // utpcPreviewData
            // 
            this.utpcPreviewData.Controls.Add(this.splitContainer6);
            this.utpcPreviewData.Controls.Add(this.panel1);
            this.utpcPreviewData.Enabled = false;
            this.utpcPreviewData.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcPreviewData.Name = "utpcPreviewData";
            this.utpcPreviewData.Size = new System.Drawing.Size(778, 594);
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(0, 0);
            this.splitContainer6.Name = "splitContainer6";
            this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.splitContainer5);
            this.splitContainer6.Panel1.Controls.Add(this.ulPreviewMsg);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.ugFixedParameters);
            this.splitContainer6.Size = new System.Drawing.Size(778, 562);
            this.splitContainer6.SplitterDistance = 434;
            this.splitContainer6.TabIndex = 4;
            this.splitContainer6.Text = "splitContainer1";
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.Location = new System.Drawing.Point(0, 18);
            this.splitContainer5.Name = "splitContainer5";
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.ugReportData);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.utcReportParts);
            this.splitContainer5.Size = new System.Drawing.Size(778, 416);
            this.splitContainer5.SplitterDistance = 471;
            this.splitContainer5.TabIndex = 91;
            this.splitContainer5.Text = "splitContainer2";
            // 
            // ugReportData
            // 
            this.ugReportData.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugReportData.DataSource = this.dsReportData;
            this.ugReportData.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.ugReportData.DisplayLayout.Override.ColumnAutoSizeMode = Infragistics.Win.UltraWinGrid.ColumnAutoSizeMode.None;
            this.ugReportData.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.True;
            this.ugReportData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugReportData.Location = new System.Drawing.Point(0, 0);
            this.ugReportData.Name = "ugReportData";
            this.ugReportData.Size = new System.Drawing.Size(471, 416);
            this.ugReportData.TabIndex = 0;
            this.ugReportData.Text = "Данные отчетов";
            // 
            // dsReportData
            // 
            this.dsReportData.DataSetName = "ReportData";
            // 
            // utcReportParts
            // 
            this.utcReportParts.Controls.Add(this.ultraTabSharedControlsPage6);
            this.utcReportParts.Controls.Add(this.utpcPart1);
            this.utcReportParts.Controls.Add(this.utpcPart2);
            this.utcReportParts.Controls.Add(this.utpcPart3);
            this.utcReportParts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcReportParts.Location = new System.Drawing.Point(0, 0);
            this.utcReportParts.Name = "utcReportParts";
            this.utcReportParts.SharedControlsPage = this.ultraTabSharedControlsPage6;
            this.utcReportParts.Size = new System.Drawing.Size(303, 416);
            this.utcReportParts.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcReportParts.TabIndex = 0;
            ultraTab14.TabPage = this.utpcPart1;
            ultraTab14.Text = "Часть 1";
            ultraTab15.TabPage = this.utpcPart2;
            ultraTab15.Text = "Часть 2";
            ultraTab16.TabPage = this.utpcPart3;
            ultraTab16.Text = "Часть 3";
            this.utcReportParts.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab14,
            ultraTab15,
            ultraTab16});
            // 
            // ultraTabSharedControlsPage6
            // 
            this.ultraTabSharedControlsPage6.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage6.Name = "ultraTabSharedControlsPage6";
            this.ultraTabSharedControlsPage6.Size = new System.Drawing.Size(301, 395);
            // 
            // ulPreviewMsg
            // 
            appearance79.TextVAlignAsString = "Middle";
            this.ulPreviewMsg.Appearance = appearance79;
            this.ulPreviewMsg.BackColorInternal = System.Drawing.Color.Khaki;
            this.ulPreviewMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.ulPreviewMsg.Location = new System.Drawing.Point(0, 0);
            this.ulPreviewMsg.Name = "ulPreviewMsg";
            this.ulPreviewMsg.Size = new System.Drawing.Size(778, 18);
            this.ulPreviewMsg.TabIndex = 90;
            this.ulPreviewMsg.Text = "Закладка используется только на этапе предварительного просмотра";
            // 
            // ugFixedParameters
            // 
            this.ugFixedParameters.Cursor = System.Windows.Forms.Cursors.Default;
            this.ugFixedParameters.DataSource = this.dsFixedParameters;
            this.ugFixedParameters.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 377;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.None;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Width = 380;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.ugFixedParameters.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ugFixedParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ugFixedParameters.Location = new System.Drawing.Point(0, 0);
            this.ugFixedParameters.Name = "ugFixedParameters";
            this.ugFixedParameters.Size = new System.Drawing.Size(778, 124);
            this.ugFixedParameters.TabIndex = 0;
            this.ugFixedParameters.Text = "Фиксированные параметры";
            // 
            // dsFixedParameters
            // 
            this.dsFixedParameters.DataSetName = "FixedParameters";
            this.dsFixedParameters.Tables.AddRange(new System.Data.DataTable[] {
            this.FixParams});
            // 
            // FixParams
            // 
            this.FixParams.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2});
            this.FixParams.TableName = "FixParams";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Name";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Value";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ubtnCancelPreview);
            this.panel1.Controls.Add(this.ubtnApplyPreview);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 562);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(778, 32);
            this.panel1.TabIndex = 5;
            // 
            // ubtnCancelPreview
            // 
            this.ubtnCancelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance80.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Cross2;
            appearance80.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance80.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnCancelPreview.Appearance = appearance80;
            this.ubtnCancelPreview.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnCancelPreview.Location = new System.Drawing.Point(659, 5);
            this.ubtnCancelPreview.Name = "ubtnCancelPreview";
            this.ubtnCancelPreview.Size = new System.Drawing.Size(116, 23);
            this.ubtnCancelPreview.TabIndex = 60;
            this.ubtnCancelPreview.Text = "Отменить";
            // 
            // ubtnApplyPreview
            // 
            this.ubtnApplyPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            appearance81.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Check3;
            appearance81.ImageHAlign = Infragistics.Win.HAlign.Left;
            appearance81.ImageVAlign = Infragistics.Win.VAlign.Middle;
            this.ubtnApplyPreview.Appearance = appearance81;
            this.ubtnApplyPreview.ImageTransparentColor = System.Drawing.Color.White;
            this.ubtnApplyPreview.Location = new System.Drawing.Point(537, 5);
            this.ubtnApplyPreview.Name = "ubtnApplyPreview";
            this.ubtnApplyPreview.Size = new System.Drawing.Size(116, 23);
            this.ubtnApplyPreview.TabIndex = 59;
            this.ubtnApplyPreview.Text = "Принять";
            // 
            // utpcPumpControl
            // 
            this.utpcPumpControl.Controls.Add(this.splitContainer4);
            this.utpcPumpControl.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcPumpControl.Name = "utpcPumpControl";
            this.utpcPumpControl.Size = new System.Drawing.Size(780, 640);
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.IsSplitterFixed = true;
            this.splitContainer4.Location = new System.Drawing.Point(0, 0);
            this.splitContainer4.Name = "splitContainer4";
            this.splitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.toolStrip5);
            this.splitContainer4.Panel1MinSize = 20;
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.utcPumpControl);
            this.splitContainer4.Size = new System.Drawing.Size(780, 640);
            this.splitContainer4.SplitterDistance = 21;
            this.splitContainer4.TabIndex = 10;
            // 
            // toolStrip5
            // 
            this.toolStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbRefresh});
            this.toolStrip5.Location = new System.Drawing.Point(0, 0);
            this.toolStrip5.Name = "toolStrip5";
            this.toolStrip5.Size = new System.Drawing.Size(780, 25);
            this.toolStrip5.TabIndex = 7;
            this.toolStrip5.Text = "toolStrip5";
            // 
            // tsbRefresh
            // 
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRefresh.Image = global::Krista.FM.Client.ViewObjects.DataPumpUI.Properties.Resources.Refresh;
            this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.White;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(23, 22);
            this.tsbRefresh.Text = "Обновить";
            // 
            // utcPumpControl
            // 
            this.utcPumpControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.utcPumpControl.Controls.Add(this.utpcSchedule);
            this.utcPumpControl.Controls.Add(this.utpcFileManager);
            this.utcPumpControl.Controls.Add(this.utpcPumpRuling);
            this.utcPumpControl.Controls.Add(this.utpcPumpParams);
            this.utcPumpControl.Controls.Add(this.utpcExecutedOps);
            this.utcPumpControl.Controls.Add(this.utpcLog);
            this.utcPumpControl.Controls.Add(this.utpcPreviewData);
            this.utcPumpControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcPumpControl.Location = new System.Drawing.Point(0, 0);
            this.utcPumpControl.Name = "utcPumpControl";
            this.utcPumpControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.utcPumpControl.Size = new System.Drawing.Size(780, 615);
            this.utcPumpControl.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Excel;
            this.utcPumpControl.TabIndex = 6;
            ultraTab17.Key = "Ruling";
            ultraTab17.TabPage = this.utpcPumpRuling;
            ultraTab17.Text = "Управление";
            ultraTab18.Key = "Params";
            ultraTab18.TabPage = this.utpcPumpParams;
            ultraTab18.Text = "Параметры";
            ultraTab19.Key = "Schedule";
            ultraTab19.TabPage = this.utpcSchedule;
            ultraTab19.Text = "Расписание";
            ultraTab20.Key = "Folders";
            ultraTab20.TabPage = this.utpcFileManager;
            ultraTab20.Text = "Данные для закачки";
            ultraTab21.Key = "ExecutedOps";
            ultraTab21.TabPage = this.utpcExecutedOps;
            ultraTab21.Text = "Выполненные операции";
            ultraTab22.Key = "Logs";
            ultraTab22.TabPage = this.utpcLog;
            ultraTab22.Text = "Протоколы";
            ultraTab23.Key = "Preview";
            ultraTab23.TabPage = this.utpcPreviewData;
            ultraTab23.Text = "Предпросмотр";
            this.utcPumpControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab17,
            ultraTab18,
            ultraTab19,
            ultraTab20,
            ultraTab21,
            ultraTab22,
            ultraTab23});
            this.utcPumpControl.UseFlatMode = Infragistics.Win.DefaultableBoolean.True;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(778, 594);
            // 
            // utpcUnknownPump
            // 
            this.utpcUnknownPump.Controls.Add(this.ultraLabel18);
            this.utpcUnknownPump.Location = new System.Drawing.Point(-10000, -10000);
            this.utpcUnknownPump.Name = "utpcUnknownPump";
            this.utpcUnknownPump.Size = new System.Drawing.Size(780, 640);
            // 
            // ultraLabel18
            // 
            appearance82.TextHAlignAsString = "Center";
            appearance82.TextVAlignAsString = "Middle";
            this.ultraLabel18.Appearance = appearance82;
            this.ultraLabel18.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraLabel18.Location = new System.Drawing.Point(0, 0);
            this.ultraLabel18.Name = "ultraLabel18";
            this.ultraLabel18.Size = new System.Drawing.Size(780, 640);
            this.ultraLabel18.TabIndex = 0;
            this.ultraLabel18.Text = "Выберите закачку";
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "");
            this.imageList2.Images.SetKeyName(1, "");
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            this.imageList1.Images.SetKeyName(1, "");
            this.imageList1.Images.SetKeyName(2, "");
            this.imageList1.Images.SetKeyName(3, "");
            this.imageList1.Images.SetKeyName(4, "");
            this.imageList1.Images.SetKeyName(5, "");
            // 
            // utcViews
            // 
            this.utcViews.Controls.Add(this.ultraTabSharedControlsPage5);
            this.utcViews.Controls.Add(this.utpcPumpControl);
            this.utcViews.Controls.Add(this.utpcUnknownPump);
            this.utcViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.utcViews.Location = new System.Drawing.Point(0, 0);
            this.utcViews.Name = "utcViews";
            this.utcViews.SharedControlsPage = this.ultraTabSharedControlsPage5;
            this.utcViews.ShowTabListButton = Infragistics.Win.DefaultableBoolean.False;
            this.utcViews.Size = new System.Drawing.Size(780, 640);
            this.utcViews.Style = Infragistics.Win.UltraWinTabControl.UltraTabControlStyle.Wizard;
            this.utcViews.TabButtonStyle = Infragistics.Win.UIElementButtonStyle.Flat;
            this.utcViews.TabIndex = 10;
            ultraTab24.TabPage = this.utpcPumpControl;
            ultraTab24.Text = "tab1";
            ultraTab25.TabPage = this.utpcUnknownPump;
            ultraTab25.Text = "tab2";
            ultraTab25.Visible = false;
            this.utcViews.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab24,
            ultraTab25});
            // 
            // ultraTabSharedControlsPage5
            // 
            this.ultraTabSharedControlsPage5.Location = new System.Drawing.Point(0, 0);
            this.ultraTabSharedControlsPage5.Name = "ultraTabSharedControlsPage5";
            this.ultraTabSharedControlsPage5.Size = new System.Drawing.Size(780, 640);
            // 
            // DataPumpView
            // 
            this.Controls.Add(this.utcViews);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "DataPumpView";
            this.Size = new System.Drawing.Size(780, 640);
            this.utpcScheduleDaily.ResumeLayout(false);
            this.gbScheduleDaily.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByDay)).EndInit();
            this.utpcScheduleHour.ResumeLayout(false);
            this.gbScheduleHour.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByHour)).EndInit();
            this.utpcScheduleWeekly.ResumeLayout(false);
            this.gbScheduleWeekly.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudScheduleByWeek)).EndInit();
            this.utpcScheduleMonthly.ResumeLayout(false);
            this.gbScheduleMonthly.ResumeLayout(false);
            this.gbScheduleMonthly.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudMonthlyByDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceWeekNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceWeekDay)).EndInit();
            this.ultraTabPageControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugPumpHistory)).EndInit();
            this.ultraTabPageControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugDataSources)).EndInit();
            this.utpcPart1.ResumeLayout(false);
            this.utpcPart2.ResumeLayout(false);
            this.utpcPart3.ResumeLayout(false);
            this.utpcPumpRuling.ResumeLayout(false);
            this.utpcPumpParams.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.scGeneralParams.Panel1.ResumeLayout(false);
            this.scGeneralParams.Panel2.ResumeLayout(false);
            this.scGeneralParams.ResumeLayout(false);
            this.scIndividualParams.Panel1.ResumeLayout(false);
            this.scIndividualParams.Panel2.ResumeLayout(false);
            this.scIndividualParams.ResumeLayout(false);
            this.utpcSchedule.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.udteScheduleStartDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udteScheduleStartTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uceSchedulePeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcSchedulePeriod)).EndInit();
            this.utcSchedulePeriod.ResumeLayout(false);
            this.utpcFileManager.ResumeLayout(false);
            this.scBrowsers.Panel1.ResumeLayout(false);
            this.scBrowsers.Panel1.PerformLayout();
            this.scBrowsers.Panel2.ResumeLayout(false);
            this.scBrowsers.Panel2.PerformLayout();
            this.scBrowsers.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tsLeftBrowserCaption.ResumeLayout(false);
            this.tsLeftBrowserCaption.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tsRightBrowserCaption.ResumeLayout(false);
            this.tsRightBrowserCaption.PerformLayout();
            this.utpcExecutedOps.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraTabControl1)).EndInit();
            this.ultraTabControl1.ResumeLayout(false);
            this.utpcLog.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcLogSwitcher)).EndInit();
            this.utcLogSwitcher.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.utpcPreviewData.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            this.splitContainer6.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugReportData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsReportData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.utcReportParts)).EndInit();
            this.utcReportParts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ugFixedParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFixedParameters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FixParams)).EndInit();
            this.panel1.ResumeLayout(false);
            this.utpcPumpControl.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.ResumeLayout(false);
            this.toolStrip5.ResumeLayout(false);
            this.toolStrip5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.utcPumpControl)).EndInit();
            this.utcPumpControl.ResumeLayout(false);
            this.utpcUnknownPump.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.utcViews)).EndInit();
            this.utcViews.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		public override void Customize()
		{
			ComponentCustomizer.CustomizeInfragisticsComponents(components);
			base.Customize();
		}
	}
}