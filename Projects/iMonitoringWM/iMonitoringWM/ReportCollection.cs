using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Krista.FM.Client.iMonitoringWM.Downloader;
using Krista.FM.Client.iMonitoringWM.Common;
using System.Windows.Forms;

namespace iMonotoringWM
{
    public class ReportCollection: Control
    {
        private List<Report> _items;
        private WebDownloader downloader;
        //очередь загрузки
        private List<ReportView> downloadingQueoe;

        public List<Report> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ReportCollection()
            : this(new WebDownloader())
        {
        }

        public ReportCollection(WebDownloader downloader)
        {
            this.Items = new List<Report>();
            this.downloader = downloader;
            if (this.downloader != null)
                this.InitEvents();
            this.downloadingQueoe = new List<ReportView>();
        }

        public ReportCollection(XmlNodeList reportsNode)
        {
            this.Items = new List<Report>();
            if (reportsNode != null)
            {
                foreach (XmlNode reportNode in reportsNode)
                {
                    this.Add(new Report(reportNode));
                }
            }
        }

        public void Add(Report item)
        {
            item.Collection = this;
            this.Items.Add(item);
        }

        /// <summary>
        /// Выбор вида отчета
        /// </summary>
        /// <param name="report">отчет</param>
        /// <param viewMode="report">вид отчета</param>
        /// <param name="isPrechange">true если предвыбор следующего 
        /// отчета из ленты</param>
        public void ReportChanged(Report report, ReportViewMode viewMode, bool isPrechange)
        {
            if (report == null)
                return;


            report.Changing(viewMode);
            ReportView reportView = report.GetReportView(viewMode);

            if (this.downloadingQueoe.Contains(reportView))
            {
                //если отчет уже содержится в очереди загрузки, и стоит не на первом месте 
                //(тоесть пока не загружается), передвиним его в начало очереди, на предпервое 
                //место, чтобы следующим загружался иммено он
                if (!isPrechange && !this.IsFirstInDownloadingQueoe(reportView))
                {
                    this.downloadingQueoe.Remove(reportView);
                    this.downloadingQueoe.Insert((this.downloadingQueoe.Count > 0) ? 1 : 0, reportView);
                }
            }
            else
            {
                switch (reportView.ViewState)
                {
                    case ReportViewState.NeedUpdate:
                    case ReportViewState.LoadedOfCache:
                        {
                            if (MainForm.Instance.WorkMode == WorkMode.Online)
                            {
                                //Если есть соединение с сервером, поставим в очередь загрузки
                                if (MainForm.Instance.IsConnetctedToHost)
                                {
                                    this.downloadingQueoe.Add(reportView);
                                    if (this.downloadingQueoe.Count == 1)
                                        this.StartDownloadReport();
                                }
                                //Если соединения нет, выставим признак ошибки загрузки
                                else
                                {
                                    reportView.DownloadComplete("Отстутствует подключение к серверу", "", false);
                                }
                            }
                            break;
                        }
                }
            }
            report.InitIndications();
        }

        /// <summary>
        /// Стоит ли вид отчета в очереди загрузки
        /// </summary>
        /// <param name="reportView"></param>
        /// <returns></returns>
        public bool IsExistsInDownloadingQueoe(ReportView reportView)
        {
            return this.downloadingQueoe.Contains(reportView);
        }

        private void StartDownloadReport()
        {
            if ((this.downloadingQueoe.Count > 0) && !this.downloader.IsDownloading)
            {
                ReportView reportView = this.downloadingQueoe[0];
                this.downloader.AsynchSaveToHTML(reportView.Uri, reportView.CachePath, 
                    reportView.Id, reportView.Report.Caption);
            }
        }

        /// <summary>
        /// Является ли указанный элемент первым в очереди загрузки
        /// </summary>
        /// <param name="reportView"></param>
        /// <returns></returns>
        private bool IsFirstInDownloadingQueoe(ReportView reportView)
        {
            return (this.downloadingQueoe.IndexOf(reportView) == 0);
        }

        /// <summary>
        /// Из списка достает отчет с указаной позицией
        /// </summary>
        /// <param name="position">позиция отчета</param>
        /// <returns>отчет</returns>
        public Report ReportByPosition(int position)
        {
            foreach (Report item in this.Items)
            {
                if (item.Position == position)
                    return item;
            }
            return null;
        }

        /// <summary>
        /// Получить список отчетов упорядоченных по возрастанию позиции
        /// </summary>
        /// <returns></returns>
        public List<Report> GetReportsOrderedByAscendPosition()
        {
            List<Report> result = new List<Report>();
            for (int i = 0; i < this.Items.Count; i++)
            {
                Report report = this.ReportByPosition(i);
                if (report != null)
                    result.Add(report);
            }
            return result;
        }

        /// <summary>
        /// Сортирует позиции отчетов (новые добавляет в конец списка)
        /// </summary>
        public void SortReportsPosition()
        {
            const int startMinPosition = 10000;
            //предыдущее минимальное значение
            int prepareMinPosition = -1;

            for (int i = 0; i < this.Items.Count; i++)
            {
                //текущая минимальная позиция
                int minPosition = startMinPosition;

                foreach (Report report in this.Items)
                {
                    //искать минимум будем у всех отчетов кроме новых, и тех которые 
                    //уже обработали
                    if ((report.Position != -1) && (report.Position > prepareMinPosition))
                        minPosition = Math.Min(minPosition, report.Position);
                }

                foreach (Report report in this.Items)
                {
                    if (report.Position == minPosition)
                    {
                        report.Position = i;
                        break;
                    }
                }

                //сортируем новые отчеты, в случае если старых вообще нет, или все индексы старым отчетам 
                //уже проставлены
                if (minPosition == startMinPosition)
                {
                    //пробежимся последний раз по отчетам, и если были новые, проставим им 
                    //индексы
                    foreach (Report report in this.Items)
                    {
                        if (report.Position == -1)
                            report.Position = i++;
                    }
                    break;
                }
                else
                    prepareMinPosition = minPosition;
            }
        }

        private void InitEvents()
        {
            this.downloader.DownloadCompleted += new WebDownloader.DownloadCompletedHandler(downloader_DownloadCompleted);
        }

        public void ChangedEntity()
        {
            for (int i = this.downloadingQueoe.Count - 1; i > 0; i--)
            {
                ReportView reportView = this.downloadingQueoe[i];
                if (reportView.Report.IsSubjectDependent)
                    this.downloadingQueoe.Remove(reportView);
            }

            /* Пока не будем прерывать загрузку текущего отчета, т.к. безглючно это сделать 
             * не получилось
            if (this.downloadingQueoe.Count > 0)
            {
                ReportView reportView = this.downloadingQueoe[0];
                if (reportView.Report.IsSubjectDependent)
                {
                    //теперь прервем загрузку первого в очереди отчета
                    this.downloader.AbortAsynchSaveHTML();
                }
            }*/
        }

        private void ProcessDownloadReport(string reportID, string errorText, string infoText, bool isSuccess)
        {
            //Если дан сигнал к закрытию приложения, выходим, не загружая другие отчеты из очереди
            if (MainForm.Instance.IsClosingApplication)
                return;

            if (!isSuccess)
                Messages.ShowError(errorText, "Ошибка");

            if (this.downloadingQueoe.Count > 0)
            {
                //получим загрузившийся отчет, он всегда первый
                ReportView reportView = this.downloadingQueoe[0];
                if (reportView.Id != reportID)
                    return;
                //удалим его из очереди загрузки
                this.downloadingQueoe.RemoveAt(0);
                //сообщим отчету о том что он загружен
                reportView.DownloadComplete(errorText, infoText, isSuccess);

                //начнем загрузку следующего
                this.StartDownloadReport();
            }
        }

        #region Обработчики событий

        void downloader_DownloadCompleted(object sender, string reportID, string errorText, string infoText, 
            bool isSuccess)
        {
            this.Invoke(new EventHandler(delegate { this.ProcessDownloadReport(reportID, errorText, infoText, isSuccess); }));
        }

        #endregion
    }
}
