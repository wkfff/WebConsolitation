using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml;
using Krista.FM.Client.MobileReports.Common;
using Krista.FM.Common;
using Krista.FM.Server.Dashboards.Common;
using Krista.FM.Server.Dashboards.Components;
using Krista.FM.Server.Dashboards.Core;
using Krista.FM.ServerLibrary.TemplatesService;

namespace Krista.FM.Server.Dashboards.reports.iPhone
{
    public partial class Index : CustomReportPage
    {
        private static ISnapshotService snapshotService;
        private static BootloaderState bootloaderState;
        private static string fatherSessionId;

        private bool haveGeneratePermission
        {
            get 
            {
                return Session["IsWebAdministrator"] != null &&
                       (bool)Session["IsWebAdministrator"];
            }
        }

        protected override void Page_PreInit(object sender, EventArgs e)
        {
            base.Page_PreInit(sender, e);
            if (ConfigurationManager.AppSettings["SiteName"] != null)
            {
                Title = ConfigurationManager.AppSettings["SiteName"];
            }
        }

        protected override void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (Request.Form.ToString() == "StateCheck")
            {
                ResponseStatusCheck();
                Response.End();
            }
            if (!Page.IsPostBack)
            {
                InitializeComboRegoins();
            }

            GenerateIndexControls();
            InitializeUserParams();

            if (Request.Params["reportID"] != null)
            {
                AddReportPrewiev();
            }
            
            SetupBootloadControls();
        }

        /// <summary>
        /// Выставляет видимость контролов генерации отчетов
        /// </summary>
        private void SetupBootloadControls()
        {
            if (haveGeneratePermission)
            {   
                UpdateReportControls.Visible = true;
                Page.ClientScript.RegisterStartupScript(GetType(), "bootloadScript", bootloaderScriptBlock);
                if (bootloaderState.UpdateState != UpdateState.ReadyForUpdate &&
                fatherSessionId != String.Empty &&
                Session.SessionID != fatherSessionId)
                {
                    btnBurn.Enabled = false;
                }
            }
            else
            {
                UpdateReportControls.Visible = false;
            }
        }

        /// <summary>
        /// Добавляет на страницу превьюшку отчета
        /// </summary>
        private void AddReportPrewiev()
        {
            HtmlGenericControl previewContainer = new HtmlGenericControl("div");
            previewContainer.InnerHtml = GetSnippetText();
            IPhoneReportPlaceHolder.Controls.Add(previewContainer);
        }

        private void InitializeUserParams()
        {
            UserParams.Region.Value = RegionsCombo.SelectedNodeParent;
            UserParams.StateArea.Value = RegionsCombo.SelectedValue;

            UserParams.ShortRegion.Value = RegionsNamingHelper.ShortName(RegionsCombo.SelectedNodeParent);
            UserParams.ShortStateArea.Value = RegionsNamingHelper.ShortName(RegionsCombo.SelectedValue);
        }

        private void InitializeComboRegoins()
        {
            RegionsCombo.Width = 350;
            RegionsCombo.Title = "Субъект РФ";
            RegionsCombo.FillDictionaryValues(CustomMultiComboDataHelper.FillRegions(RegionsNamingHelper.FoNames, RegionsNamingHelper.RegionsFoDictionary));
            RegionsCombo.ParentSelect = false;

            if (string.IsNullOrEmpty(UserParams.Region.Value) ||
                string.IsNullOrEmpty(UserParams.StateArea.Value))
            {
                if (RegionSettings.Instance != null && RegionSettings.Instance.Name != String.Empty)
                {
                    RegionsCombo.SetСheckedState(RegionSettings.Instance.Name, true);
                }
            }
            else
            {
                RegionsCombo.SetСheckedState(UserParams.StateArea.Value, true);
            }
        }

        private void ResponseStatusCheck()
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            Response.AddHeader("Cache-Control", "no-cache");
            // Если еще ничего не пытались генерировать
            if (snapshotService == null)
            {
                // Делать тут больше нечего.
                return;
            }
            try
            {
                bootloaderState = snapshotService.GetState();
            }
            catch (Exception e)
            {
                bootloaderState.LastError = e.Message + Environment.NewLine + e.StackTrace;
            }
            if (!String.IsNullOrEmpty(bootloaderState.LastError))
            {
                Response.Write(String.Format("error|Ошибка при генерации отчетов: {0}", bootloaderState.LastError));
                fatherSessionId = String.Empty;
                bootloaderState.UpdateState = UpdateState.ReadyForUpdate;
            }
            else 
            {
                Response.Write(String.Format("{0}|{1}", (int)bootloaderState.UpdateState, bootloaderState.PercentDone));
            }

            if (bootloaderState.UpdateState == UpdateState.FinishUpdateData)
            {
                fatherSessionId = String.Empty;
            }
        }

        private void GenerateIndexControls()
        {
            if (AllowedReportsIPhone == null)
            {
                return;
            }
            Control container = Page.LoadControl("~/Components/ContainerPanel.ascx");
            ((ContainerPanel)container).AddContent(IndexPageHelper.GetReportTable(AllowedReportsIPhone, TemplateTypes.IPhone));
            ((ContainerPanel)container).AddHeader("Аналитические отчеты");
            ((ContainerPanel)container).AddHeaderImage("../images/Reports.png");
            ReportsPlaceHolder.Controls.Add(container);
        }

        private string GetSnippetText()
        {
            int iframeWidth = 768;
            int iframeHeight = 950;
            string scroll = "no";
            string[] reportID = Request.Params["reportID"].ToString().Split('_');
            if (reportID[reportID.Length - 1].ToLower() == "h")
            {
                iframeWidth = 495;
                iframeHeight = 315;
                scroll = "yes";
            }
            else if (reportID[reportID.Length - 1].ToLower() == "v")
            {
                iframeWidth = 808;
                iframeHeight = 950;
                scroll = "yes";
            }
            return String.Format("<iframe frameborder=\"1\" scrolling=\"{3}\" style=\"width:{0}px; height: {1}px; \" src='default.aspx?reportId={2}'></iframe>",
                    iframeWidth, iframeHeight, Request.Params["reportID"], scroll);
        }

        private void DoSnapshot()
        {
            string bootloadServiceUri = String.Format("tcp://{0}/ReportsBootloaderService/Server.rem",
                    ConfigurationManager.AppSettings[CustomReportConst.BootloadServiceNameKeyName]);
            snapshotService = (ISnapshotService)Activator.GetObject(typeof(ISnapshotService), bootloadServiceUri);
            try
            {
                snapshotService.DoSnapshot(GenerateStartParams());
            }
            catch (System.Net.Sockets.SocketException e)
            {
                bootloaderState.LastError = e.Message;
            }
        }

        /// <summary>
        /// Генерирует XML с отчетами для генерации.
        /// </summary>
        private SnapshotStartParams GenerateStartParams()
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
            writer.WriteStartDocument();
            writer.WriteStartElement("startParams");
            writer.WriteStartElement("deployingReports");
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.Contains("checkBoxGenerate_"))
                {
                    string reportId = key.Replace("checkBoxGenerate_", String.Empty);
                    writer.WriteRaw(String.Format("<report id=\"{0}\"/>", reportId));
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            StreamReader s = new StreamReader(stream);
            stream.Position = 0;
            string reports = s.ReadToEnd();
            CRHelper.SaveToErrorLog(reports);
            LogicalCallContextData cnt = Session[ConnectionHelper.LOGICAL_CALL_CONTEXT_DATA_KEY_NAME] as LogicalCallContextData;
            if (cnt != null)
                LogicalCallContextData.SetContext(cnt);
            string serverURL =
                String.Format(CustomReportConst.ServerURL, ConfigurationManager.AppSettings["SchemeServerName"]);
            string reportsHostUrl = Request.Url.AbsoluteUri.ToLower().Replace("index.aspx", "default.aspx");
            reportsHostUrl = reportsHostUrl.Split('?')[0];

            return new SnapshotStartParams(serverURL, reports, reportsHostUrl);
        }

        protected void btnBurn_Click(object sender, EventArgs e)
        {
            if (!haveGeneratePermission)
            {
                return;
            }
            // Если готовы обновлять
            if (bootloaderState.UpdateState == UpdateState.ReadyForUpdate ||
                bootloaderState.UpdateState == UpdateState.FinishUpdateData)
            {
                // Вызываем сервис
                DoSnapshot();
                // Если запустилось нормально
                if (String.IsNullOrEmpty(bootloaderState.LastError))
                {
                    fatherSessionId = HttpContext.Current.Session.SessionID;
                    btnBurn.Text = "Остановить генерацию отчетов";
                }
            }
            else
            {
                // Остановить можем только если родительская сессия.
                if (Session.SessionID == fatherSessionId)
                {
                    snapshotService.Close();
                    fatherSessionId = String.Empty;
                    bootloaderState.UpdateState = UpdateState.ReadyForUpdate;
                    btnBurn.Text = "Начать генерацию отчетов";
                }
            }
        }

        #region bootloaderScript
        private const string bootloaderScriptBlock =
            @"<script type='text/javascript' language='javascript'>                                
            function AJAXInteraction(url, callback) 
            {
                var req = init();
                req.onreadystatechange = processRequest;

                function init() 
                {
                    if (window.XMLHttpRequest) 
                    {
                        return new XMLHttpRequest();
                    } 
                    else if (window.ActiveXObject) 
                    {
                        return new ActiveXObject('Microsoft.XMLHTTP');
                    }
                }

                function processRequest() 
                {
                    if (req.readyState == 4) 
                    {
                        if (req.status == 200) 
                        {                            
                            if (callback) 
                            {
                                callback(req);
                            }
                        }
                    }
                }

                this.doGet = function() 
                {
                    req.open('GET', url, true);
                    req.send(null);
                }

                this.doPost = function(body) 
                {
                    req.open('POST', url, true);
                    req.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
                    req.send(body);
                }
            }

            function makeRequest() 
            {                              
                var ai = new AJAXInteraction('Index.aspx', processResponse);
                ai.doPost('StateCheck');
            }                
                            
	        var timerId = window.setInterval('makeRequest();', 1000); 
	        
	        var processResponse = function (request)
	        {	
	            var progressbar = document.getElementById('progressbar');                
                if (request.responseText != '')
                {
	                var splitString = request.responseText.split('|');                
                    if (splitString[0] != 'error')
                    {
                        var statusCell;
	                    var statusCellId;                    
	                    for (var i=0;i<splitString[0];i++)
	                    {
	                        statusCellId = 'statusCell_' + i;
	                        statusCell = document.getElementById(statusCellId);
	                        statusCell.style.backgroundImage = 'url(../../images/ballGreenBB.png)';
	                    }
                        var j = splitString[0];
                        j++;                    
                        for (var i=j;i<10;i++)
	                    {
	                        statusCellId = 'statusCell_' + i;
	                        statusCell = document.getElementById(statusCellId);
	                        statusCell.style.backgroundImage = 'url(../../images/ballYellowBB.png)';
	                    }                                            
	                    statusCellId = 'statusCell_' + splitString[0];
                        statusCell = document.getElementById(statusCellId);
                        statusCell.style.backgroundImage = 'url(../../images/ballOrangeBB.png)';
                        
                        progressbar.style.width = splitString[1] + '%';
                        if (splitString[0] == 9)
                        {                            
                            var btnBurn = document.getElementById('ctl00_ContentPlaceHolder1_btnBurn');
                            btnBurn.value = 'Начать генерацию отчетов';
                        }
                    }
                    else
                    {                    
                        var btnBurn = document.getElementById('ctl00_ContentPlaceHolder1_btnBurn');
                        btnBurn.value = 'Начать генерацию отчетов';
                        var errorTextSpan = document.getElementById('lastErrorMessage');
                        errorTextSpan.style.display='';
                        errorTextSpan.innerText = splitString[1];
                    }
                }
	        }
        </script>";
        #endregion
    }
}
