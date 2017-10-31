using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;

using Krista.FM.Client.MobileReports.Common;
using Krista.FM.ServerLibrary.TemplatesService;
using Chilkat;

namespace Krista.FM.Client.MobileReports.Bootloader
{
    public class WebBootloader
    {
        #region ��������
        public delegate void DownloadCompletedHandler(object sender, string errorText, string infoText);
        public delegate void StartDownloadingResourcesHandler(object sender, int resourcesCount);
        public delegate void StartResourceHandler(object sender, int numberResource);
        public delegate void EndResourceHandler(object sender, int numberResource, bool success);
        public delegate void AuthorizationCompleteHandler(object sender, bool success);
        #endregion
        
        #region �������
        private event DownloadCompletedHandler _downloadCompleted;
        private event StartDownloadingResourcesHandler _startDownloadingResources;
        private event StartResourceHandler _startResource;
        private event EndResourceHandler _endResource;
        private event AuthorizationCompleteHandler _authorizationComplete;
        /// <summary>
        /// ��������� ��������
        /// </summary>
        public event DownloadCompletedHandler DownloadCompleted
        {
            add { _downloadCompleted += value; }
            remove { _downloadCompleted -= value; }
        }

        public event StartDownloadingResourcesHandler StartDownloadingResources
        {
            add { _startDownloadingResources += value; }
            remove { _startDownloadingResources -= value; }
        }

        public event StartResourceHandler StartResource
        {
            add { _startResource += value; }
            remove { _startResource -= value; }
        }

        public event EndResourceHandler EndResource
        {
            add { _endResource += value; }
            remove { _endResource -= value; }
        }
        public event AuthorizationCompleteHandler AuthorizationComplete
        {
            add { _authorizationComplete += value; }
            remove { _authorizationComplete -= value; }
        }
        #endregion

        #region ����
        //proxy
        private WebProxy _proxy;
        private MatchCollection resourcesList;
        private List<ResourceInfo> resourcesInfo;
        private string savePath;
        private string reportName;
        private string reportID;
        private string resourcesFolder;
        private string resourcesPath;
        private string _customScriptsPath;
        private string _sourceHtmlBody;
        private string currentHtmlBody;
        private ScriptsDownloadType _scriptsDownloadMode;
        private bool _isOptimazeHTML;
        private bool isNewSnapshotMode;
        private StringBuilder _errorText;
        private StringBuilder _infoText;
        private Uri absoluteUrl;
        private Uri _htmlBodyAbsoluteUrl;
        private int currentResourcesNumber;
        private CookieContainer _cookieContainer;
        private MobileTemplateTypes _mobileTemplate;
        private Zip zipArchiver;
        #endregion

        #region ��������
        /// <summary>
        /// Proxy
        /// </summary>
        public WebProxy Proxy
        {
            get { return _proxy; }
            set { _proxy = value; }
        }

        public StringBuilder ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        public StringBuilder InfoText
        {
            get { return _infoText; }
            set { _infoText = value; }
        }

        /// <summary>
        /// ���� � ������� � ����������������� ���������
        /// </summary>
        public string CustomScriptsPath
        {
            get { return _customScriptsPath; }
            set { _customScriptsPath = value; }
        }

        /// <summary>
        /// ����� �������� ��������
        /// </summary>
        public ScriptsDownloadType ScriptsDownloadMode
        {
            get { return _scriptsDownloadMode; }
            set { _scriptsDownloadMode = value; }
        }

        /// <summary>
        /// �������������� HTML (������� ������ �������, �������� �����, � �.�.)
        /// </summary>
        public bool IsOptimazeHTML
        {
            get { return _isOptimazeHTML; }
            set { _isOptimazeHTML = value; }
        }

        /// <summary>
        /// ��������� �����
        /// </summary>
        public CookieContainer CookieContainer
        {
            get { return _cookieContainer; }
            set { _cookieContainer = value; }
        }

        /// <summary>
        /// ��� ������ ���� ���������� ������� ������
        /// </summary>
        public MobileTemplateTypes MobileTemplate
        {
            get { return _mobileTemplate; }
            set { _mobileTemplate = value; }
        }

        /// <summary>
        /// ����������� url � html ��������
        /// </summary>
        public Uri HtmlBodyAbsoluteUrl
        {
            get { return _htmlBodyAbsoluteUrl; }
            set { _htmlBodyAbsoluteUrl = value; }
        }

        /// <summary>
        /// ��� �������� � �������� ����
        /// </summary>
        public string SourceHtmlBody
        {
            get { return _sourceHtmlBody; }
            set { _sourceHtmlBody = value; }
        }
        #endregion
        
        /// <summary>
        /// ���������� ������
        /// </summary>
        /// <param name="webBrowser">����� ���� null, ����� ������� ����� ���������</param>
        public WebBootloader()
        {
            this.CookieContainer = new CookieContainer();
            this.ErrorText = new StringBuilder();
            this.InfoText = new StringBuilder();

            //�������������� ���������
            this.zipArchiver = new Zip();
            if (!this.zipArchiver.UnlockComponent("ZIPT34MB34N_6E1CD374p762"))
                throw new Exception(this.zipArchiver.LastErrorText);
        }

        private void ClearResourcesInfo()
        {
            if (this.resourcesInfo == null)
                this.resourcesInfo = new List<ResourceInfo>();
            else
                this.resourcesInfo.Clear();
        }

        public bool Authorization(string reportsHostAddress, string login, string password)
        {
            bool result = false;
            reportsHostAddress = string.Format("{0}?login={1}&password={2}&isAuthorizationRequest=true",
                reportsHostAddress, login, password);
            string response = this.DownloadString(reportsHostAddress);

            result = response.Contains("Authorization Complete");
            this.OnAuthorizationComplete(result);
            return result;
        }

        public void SaveToHTML(Uri htmlBodyUrl, string htmlBody, string savePath, string reportID,
            MobileTemplateTypes templateType, bool isNewSnapshotMode)
        {
            if ((htmlBodyUrl == null) || string.IsNullOrEmpty(savePath))
                return;

            this.isNewSnapshotMode = isNewSnapshotMode;
            this.absoluteUrl = htmlBodyUrl;
            this.savePath = savePath;
            this.MobileTemplate = templateType;
            this.ClearResourcesInfo();

            //��� ������
            this.reportName = Path.GetFileNameWithoutExtension(savePath);
            //���������� ��� ������ ��� �������� �������������� ����������������
            this.reportID = reportID;
            //��� ����� � ������� ����������� �������
            this.resourcesFolder = this.reportName + Consts.resourcesFolderExtension;
            //������ ���� � ����� � ���������
            this.resourcesPath = Path.Combine(Path.GetDirectoryName(savePath), this.resourcesFolder);

            //�������, ��� �������� �����
            this.InitResourcesFolder(this.resourcesPath);
            this.currentHtmlBody = htmlBody;

            //��������� ���� ������
            if (this.IsValidHTML(this.currentHtmlBody))
            {
                this.HtmlBodyProcessing();
                this.DetectResources(this.currentHtmlBody);
                this.SaveHTMLBody();

                if (isNewSnapshotMode)
                {
                    this.CSSProcessing();
                    this.ArchiveReport(savePath);
                }
            }
            else
            {
                this.ErrorText.AppendLine("�����: " + this.reportName);
                this.ErrorText.AppendLine("������: " + this.currentHtmlBody);
            }
            this.OnDownloadCompleted(this.ErrorText, this.InfoText);
        }

        public void SaveToHTML(string address, string savePath, string reportID,
            MobileTemplateTypes templateType, bool isNewSnapshotMode)
        {
            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(savePath))
                return;

            this.ErrorText.Remove(0, this.ErrorText.Length);
            this.InfoText.Remove(0, this.InfoText.Length);
           
            //��������� ���� ������
            if (this.DownloadHTMLBody(address))
            {
                this.SaveToHTML(this.HtmlBodyAbsoluteUrl, this.SourceHtmlBody, savePath, 
                    reportID, templateType, isNewSnapshotMode);
            }
            else
            {
                this.ErrorText.AppendLine("�����: " + this.reportName);
                this.ErrorText.AppendLine("������: " + this.currentHtmlBody);
                this.OnDownloadCompleted(this.ErrorText, this.InfoText);
            }
        }

        /// <summary>
        /// ���������� �����
        /// </summary>
        /// <param name="savePath">���� ���������� ������</param>
        private void ArchiveReport(string savePath)
        {
            string reportFolder = Path.GetDirectoryName(savePath);
            string zipReportPath = string.Format("{0}.zip", reportFolder);
            this.zipArchiver.NewZip(zipReportPath);
            this.zipArchiver.SetPassword("cQHQrYkvo8I4wo2kp64Wz05KLqq1gWy8vgeN8GPvhWskCGb4Nh2hduiLvLRAlZ0OC9AuNUEF6NxKg3HmOmjQYA==");
            this.zipArchiver.PasswordProtect = true;
            this.zipArchiver.AppendFiles(reportFolder, true);
            this.zipArchiver.WriteZipAndClose();
         
            Directory.Delete(reportFolder, true);
        }

        private void CSSProcessing()
        {
            List<ResourceInfo> cssResources = new List<ResourceInfo>();
            foreach (ResourceInfo resource in this.resourcesInfo)
            {
                if (resource.ResourceType == ResourceType.CSS)
                {
                    cssResources.Add(new ResourceInfo(resource.Url));
                }
            }

            foreach (ResourceInfo cssFile in cssResources)
            {
                this.absoluteUrl = cssFile.Url;
                string cssFilePath = Path.Combine(this.resourcesPath, cssFile.Name);
                string cssText = File.ReadAllText(cssFilePath);
                this.DetectResources(cssText);

                //����� ����������� ������� ������ ���������� ������ �� �������
                string newCSSText = HTMLParser.SetResources(cssText, this.resourcesInfo, false);
                File.WriteAllText(cssFilePath, newCSSText, Encoding.UTF8);
            }
        }

        /// <summary>
        /// ��������� ��������, ����� ��������� �� � �����
        /// </summary>
        private void HtmlBodyProcessing()
        {
            switch (this.ScriptsDownloadMode)
            {
                //���� ���������������� �������, � ������ ������ ������������� �������� �����
                case ScriptsDownloadType.Custom:
                    {
                        this.currentHtmlBody = HTMLParser.SetCustomScripts(this.currentHtmlBody,
                            this.CustomScriptsPath, this.resourcesPath);
                        break;
                    }
                //���� ������� �� ���������, �� ����� �� � �� ���� ������
                case ScriptsDownloadType.NotDownload:
                    {
                        this.currentHtmlBody = HTMLParser.RemoveScripts(this.currentHtmlBody);
                        break;
                    }
            }
            //���� ����� ��������� �����, ��� ��������� �� ����� ���������
            if (!this.isNewSnapshotMode)
            {
                //� php �� ����� ��������� html ���� � ��� ���� xml �������, ������ ������� �� :)
                this.currentHtmlBody = HTMLParser.RemoveXML(this.currentHtmlBody);
                //��������� ����� � php ��������
                this.currentHtmlBody = HTMLParser.AppendPHPHeader(this.currentHtmlBody, this.reportID);
            }

            string befoChangeTdSize = this.currentHtmlBody;
            //���� ����������� �������� � ������ �������, ������� WM, ������ �������������� 
            //������� ������ ������, �� ���� ������� ������ �����������, ����� ��������� ������ ������
            //� �������� ������, ��� �� ��������� ������������ ������� �����������
            switch (this.MobileTemplate)
            {
                case MobileTemplateTypes.WM240x320:
                    {
                        //������������ ����� ���� �������� ��� ��� ���������� 240�320, 
                        //������� ������ ���������� �������� �� 2px
                        this.currentHtmlBody = HTMLParser.AppendImageTdSize(this.currentHtmlBody, 2);
                        break;
                    }
                case MobileTemplateTypes.WM480x640:
                    {
                        //������������ ����� ���� �������� ��� ��� ���������� 480�640, 
                        //������� ������ ���������� �������� �� 4px
                        this.currentHtmlBody = HTMLParser.AppendImageTdSize(this.currentHtmlBody, 4);
                        break;
                    }
            }
            if (befoChangeTdSize != this.currentHtmlBody)
                Trace.TraceWarning("� ������ ��������� ������� �����, ���������� �����������.");
        }

        private bool IsValidHTML(string html)
        {
            return html.Contains(Consts.successHTMLIndicator);
        }

        private bool DownloadHTMLBody(string address)
        {
            this.InfoText.AppendLine("===================================");
            this.InfoText.AppendLine(string.Empty);
            this.InfoText.AppendLine("��� ������: " + this.reportName);
            this.InfoText.AppendLine("����� ����������: " + this.savePath);
            this.InfoText.AppendLine("������ �������� ���� ��������, �����: " + address);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = this.GetUserAgent();
            request.CookieContainer = this.CookieContainer;
            request.Proxy = this.Proxy;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                this.InfoText.AppendLine("������ ��� �������� ���� ��������: " + e.Message);
                this.ErrorText.AppendLine(e.Message);
                request.Abort();
                return false;
            }

            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            this._sourceHtmlBody = streamReader.ReadToEnd();
            this.currentHtmlBody = this._sourceHtmlBody;

            this.HtmlBodyAbsoluteUrl = response.ResponseUri;
            this.absoluteUrl = this.HtmlBodyAbsoluteUrl;

            streamReader.Close();
            response.Close();
            request.Abort();

            this.InfoText.AppendLine("��������� ���� ��������.");
            return true;
        }

        private string DownloadString(string address)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.UserAgent = this.GetUserAgent();
            request.CookieContainer = this.CookieContainer;
            request.Proxy = this.Proxy;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                this.ErrorText.AppendLine(e.Message);
                return result;
            }

            Stream stream = response.GetResponseStream();
            int contentLenght = (int)response.ContentLength;

            if (contentLenght > 0)
            {
                byte[] bytes = new byte[contentLenght];
                int numBytesToRead = contentLenght;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                Encoding enc = new UTF8Encoding();
                result = enc.GetString(bytes);
            }

            stream.Close();
            response.Close();
            request.Abort();
            return result;
        }

        private bool DownloadFile(Uri requestUri, string savePath)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUri);
            request.CookieContainer = this._cookieContainer;
            request.UserAgent = this.GetUserAgent();
            request.Proxy = this.Proxy;

            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                this.InfoText.AppendLine("    ������ ��� ��������: " + e.Message);
                this.ErrorText.AppendLine(string.Format("�����: {0}. ����� �������: {1}. ������: {2}.", 
                    this.reportName, requestUri.ToString(), e.Message));
                this.OnEndResource(this.currentResourcesNumber, false);
                //response.Close();
                //request.Abort();
                return false;
            }

            Stream stream = response.GetResponseStream();
            int contentLenght = (int)response.ContentLength;
            this.InfoText.AppendLine("    ������ ���������� �������: " + contentLenght + "kb");

            if (contentLenght > 0)
            {
                try
                {
                    byte[] bytes = new byte[contentLenght];
                    int numBytesToRead = contentLenght;
                    int numBytesRead = 0;
                    while (numBytesToRead > 0)
                    {
                        int n = stream.Read(bytes, numBytesRead, numBytesToRead);
                        if (n == 0)
                            break;
                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    File.WriteAllBytes(savePath, bytes);
                    this.InfoText.AppendLine("    ������ ������� ��������.");
                    this.OnEndResource(this.currentResourcesNumber, true);
                }
                catch (Exception e)
                {
                    this.ErrorText.AppendLine(string.Format("   �����: {0}. ���� ����������: {1}. ������: {2}.",
                        this.reportName, savePath, e.Message));
                    this.OnEndResource(this.currentResourcesNumber, false);
                }
            }

            stream.Close();
            response.Close();
            request.Abort();
            return true;
        }

        private void InitResourcesFolder(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);
                Directory.CreateDirectory(folder);
            }
            catch (Exception e)
            {
                this.ErrorText.AppendLine("�����: " + this.reportName);
                this.ErrorText.AppendLine("������: " + e.Message);
                this.InfoText.AppendLine("������ ��� ������������� ����� ��������: " + e.Message);
            }
        }

        private void FillResourcesInfo()
        {
            this.resourcesInfo.Clear();
            for (int i = 0; i < this.resourcesList.Count; i++)
            {
                Group group = this.GetResourcesGroup(i);
                if (group.Value != string.Empty)
                    this.resourcesInfo.Add(new ResourceInfo(group, this.absoluteUrl, this.resourcesFolder));
            }
        }

        /// <summary>
        /// ����� ���� ��� ������� ���� ��������, ������ �� �� ������� ��������
        /// </summary>
        private void DetectResources(string sourceText)
        {
            this.resourcesList = HTMLParser.GetResources(sourceText);
            this.InfoText.AppendLine("���������� ��������: " + this.resourcesList.Count);
            this.OnStartResourcesDownloading(this.resourcesList.Count);

            if (this.resourcesList != null)
            {
                this.currentResourcesNumber = 0;
                this.FillResourcesInfo();
                this.DownloadResources(this.currentResourcesNumber);
            }
        }

        private void SaveHTMLBody()
        {
            //����� ����������� ������� ������ ���������� ������ �� �������
            string newBody = HTMLParser.SetResources(this.currentHtmlBody, this.resourcesInfo, true);
            //���� ������ ��������������� �������, ������������ HTML 
            if (this.IsOptimazeHTML)
                newBody = HTMLParser.OptimizeHTML(newBody);
            File.WriteAllText(this.savePath, newBody, Encoding.UTF8);
            this.InfoText.AppendLine("� ���� ������ �������� ������ �������� �� �����, � ��������� ���");
        }

        private void DownloadResources(int resourceNumber)
        {
            if (resourceNumber > this.resourcesInfo.Count - 1)
            {
                //��� ������ ��� ��� ������� �����������
                return;
            }

            //Application.DoEvents();

            ResourceInfo info = this.resourcesInfo[resourceNumber];
            if (info.Url != null)
            {
                this.InfoText.AppendLine(string.Empty);
                this.InfoText.AppendLine("    ����� �������: " + resourceNumber);
                this.InfoText.AppendLine("    ������ �������� �������, �����: " + info.Url.ToString());
                this.OnStartResource(resourceNumber);
                //���� ��� ������ ���� � ������ �������, ����� ������ ���������
                if (this.IsNeedDownload(resourceNumber))
                {
                    info.Name = this.GetValidName(resourceNumber);
                    string path = Path.Combine(this.resourcesPath, info.Name);
                    this.InfoText.AppendLine("    ����� ����� �������: " + info.NewValue);

                    //��������� ������ ����� ���� �� �� ������, ��� ���� ������ � ������ ������� ���������� ��������
                    if ((info.ResourceType != ResourceType.Script) ||
                        ((info.ResourceType == ResourceType.Script) && (this.ScriptsDownloadMode == ScriptsDownloadType.DownloadSource)))
                        info.SuccessLoaded = this.DownloadFile(info.Url, path);

                    //��������� ��������� ������
                    this.DownloadResources(++this.currentResourcesNumber);
                }
                else
                {
                    this.InfoText.AppendLine("    ������ � ������ ������ ��� ������. ��� �������: " + info.Name);
                    this.OnEndResource(this.currentResourcesNumber, true);
                    this.DownloadResources(++this.currentResourcesNumber);
                }
            }
        }

        private string GetValidName(int resourcesNumber)
        {
            ResourceInfo resource = this.resourcesInfo[resourcesNumber];

            string fileName = Path.GetFileNameWithoutExtension(resource.Name);
            string extension = Path.GetExtension(resource.Name);
            bool isNameExist = false;

            //IIS ������������ ���������� ������� � ����������� .axd, � ����� ������ ��� ����������
            //�������, ������� ������ ��� �� .js
            if (extension == ".axd")
                extension = ".js";

            string result = fileName + extension;

            int i = 1;
            do
            {
                isNameExist = this.IsExistName(result, resourcesNumber);
                if (isNameExist)
                {
                    result = string.Format("{0}({1}){2}", fileName, i, extension);
                    i++;
                }
            }
            while (isNameExist);

            return result;
        }

        private bool IsExistName(string name, int resourcesNumber)
        {
            for (int i = 0; i < resourcesNumber; i++)
            {
                if (this.resourcesInfo[i].Name == name)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ������� ��������� �� ����������, ����� ���� ��� ������� �����
        /// </summary>
        /// <param name="resourcesNumber"></param>
        /// <returns></returns>
        private bool IsNeedDownload(int resourcesNumber)
        {
            ResourceInfo resource = this.resourcesInfo[resourcesNumber];

            for (int i = 0; i < resourcesNumber; i++)
            {
                ResourceInfo temp = this.resourcesInfo[i];
                if (temp.SuccessLoaded && (temp.Url.ToString() == resource.Url.ToString()))
                {
                    resource.Name = temp.Name;
                    resource.SuccessLoaded = true;
                    return false;
                }
            }
            return true;
        }

        private Group GetResourcesGroup(int index)
        {
            Group result = null;
            if (index > this.resourcesList.Count - 1)
                return result;

            Match match = this.resourcesList[index];
            result = match.Groups[Consts.srcResources];
            if (this.IsGroupEmpty(result))
            {
                result = match.Groups[Consts.urlResources];
                if (this.IsGroupEmpty(result))
                    result = match.Groups[Consts.hrefResources];
            }
            
            return result;
        }

        private bool IsGroupEmpty(Group group)
        {
            return ((group == null) || ((group != null) && (group.Value == string.Empty)));
        }

        /// <summary>
        /// � ����������� �� ���� ���������� ����� ������� ������ UserAgent-�, ��� 
        /// ��������� ��������� ������� aspx
        /// </summary>
        /// <returns></returns>
        private string GetUserAgent()
        {
            switch (this.MobileTemplate)
            {
                case MobileTemplateTypes.IPad:
                case MobileTemplateTypes.IPhone:
                    {
                        return Consts.safariUserAgent;
                    }
                case MobileTemplateTypes.WM240x320:
                case MobileTemplateTypes.WM480x640:
                    {
                        return Consts.ieUserAgent;
                    }
            }
            return Consts.safariUserAgent;
        }

        private void OnDownloadCompleted(StringBuilder errorText, StringBuilder infoText)
        {
            if (_downloadCompleted != null)
                _downloadCompleted(this, errorText.ToString(), infoText.ToString());
        }

        private void OnStartResourcesDownloading(int resourcesCount)
        {
            if (_startDownloadingResources != null)
                _startDownloadingResources(this, resourcesCount);
        }

        private void OnStartResource(int resourceNumber)
        {
            if (_startResource != null)
                _startResource(this, resourceNumber);
        }

        private void OnEndResource(int resourceNumber, bool success)
        {
            if (_endResource != null)
                _endResource(this, resourceNumber, success);
        }

        public void OnAuthorizationComplete(bool success)
        {
            if (_authorizationComplete != null)
                _authorizationComplete(this, success);
        }

        #region ����������� �������
        #endregion
    }
}