using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinDataSource;

using Krista.FM.Client.Common;
using Krista.FM.Client.Components;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.ProcessorLibrary;


namespace Krista.FM.Client.ViewObjects.MDObjectsManagementUI
{
    public partial class OthersViewer : UserControl
    {
        private IWorkplace workplace;
        private static string msgMask = "{0,-30}: {1}" + Environment.NewLine;        

        public OthersViewer()
        {
            InitializeComponent();

            btnSetCubesHierarchy.Click += new System.EventHandler(this.btnSetCubesHierarchy_Click);
        }

        public void Connect(IWorkplace _workplace)
        {
            workplace = _workplace;

            this.Dock = DockStyle.Fill;
            this.Visible = false;

            btnCreateMDBase.Click += new System.EventHandler(this.btnCreateMDBase_Click);
            btnCreateCustomDimensions.Click += new EventHandler(this.btnCreateCustomDimensions_Click);
            btnRefreshPlanningsheetMetadata.Click += new System.EventHandler(this.btnRefreshPlanningsheetMetadata_Click);

            InfragisticComponentsCustomize.CustomizeUltraTabControl(utcMain);
        }

        private void btnSetCubesHierarchy_Click(object sender, EventArgs e)
        {
            tbInfo.Text = String.Empty;
            ArrayList dataClsList = new ArrayList();
            StringBuilder errorList = new StringBuilder();
            errorList.AppendLine("Установка иерархии: ");
            errorList.AppendLine();
            try
            {
                
                workplace.ProgressObj.Caption = "Установка иерархии";
                workplace.ProgressObj.Text = "Построение списка классификаторов";
                workplace.ProgressObj.Position = 0;
                workplace.ProgressObj.StartProgress();
                foreach (IClassifier item in workplace.ActiveScheme.Classifiers.Values)
                {
                    if (item.ClassType == ClassTypes.clsDataClassifier)
                        dataClsList.Add(item);
                }
                if (dataClsList.Count > 0)
                {
                    workplace.ProgressObj.MaxProgress = dataClsList.Count - 1;
                    for (int i = 0; i < dataClsList.Count; i++)
                    {
                        IClassifier tmpCls = (IClassifier)dataClsList[i];
                        workplace.ProgressObj.Position = i;
                        workplace.ProgressObj.Text = "Обрабатывается классификатор '" + tmpCls.Caption + "'";
                        try
                        {
                            int cnt = tmpCls.FormCubesHierarchy();
                            errorList.AppendFormat(msgMask, tmpCls.FullName, String.Format("Обработано {0} записей", cnt));
                        }
                        catch (Exception exception)
                        {
                            errorList.AppendFormat(msgMask, tmpCls.FullName, Krista.FM.Common.Exceptions.FriendlyExceptionService.GetFriendlyMessage(exception).Message);
                        }
                    }
                }
            }
            finally
            {
                workplace.ProgressObj.StopProgress();
                dataClsList.Clear();
                tbInfo.Text = errorList.ToString();
            }
        }

        private void GenerateStandartDimensions()
        {
            tbInfo.Text = String.Empty;
            StringBuilder errorList = new StringBuilder();
            errorList.AppendLine("Генерация многомерной базы: ");
            errorList.AppendLine();
            try
            {
                workplace.ProgressObj.MaxProgress = workplace.ActiveScheme.Classifiers.Values.Count - 1;
                workplace.ProgressObj.Caption = "Генерация многомерной базы";
                workplace.ProgressObj.StartProgress();
                workplace.ProgressObj.Position = 0;
                int i = 0;
                foreach (IClassifier tmpCls in workplace.ActiveScheme.Classifiers.Values)
                {
                    workplace.ProgressObj.Position = i;
                    workplace.ProgressObj.Text = "Обрабатывается классификатор '" + tmpCls.Caption + "'";
                    i++;
                    try
                    {
                        tmpCls.CreateOlapObject();
                        errorList.AppendFormat(msgMask, tmpCls.FullName, "OK");
                    }
                    catch (Exception exception)
                    {
                        errorList.AppendFormat(msgMask, tmpCls.FullName, exception.Message);
                    }
                }
            }
            finally
            {
                workplace.ProgressObj.StopProgress();
                tbInfo.Text = errorList.ToString();
            }
        }

        private void GenerateCustomDimensions()
        {
            tbInfo.Text = String.Empty;
            StringBuilder log = new StringBuilder();
            
            string serverName = workplace.ActiveScheme.SchemeMDStore.ServerName;
            string databaseName = "Специальные измерения";
            string dataSourceName = "Develop";
            string logString;

            IOlapDatabaseGenerator generator = workplace.ActiveScheme.Processor.OlapDatabaseGenerator;            
            FormDatabaseConnection frmDatabaseConnection = new FormDatabaseConnection();
            try
            {
                if (frmDatabaseConnection.ShowOptions(
                this, ref serverName, ref databaseName, ref dataSourceName) == DialogResult.OK)
                {
                    log.AppendLine("Генерация специальных измерений... ");
                    log.AppendLine();
                    int totalCount, errorCount;
                    generator.GenerateMRRDimensions(serverName, databaseName, dataSourceName,
                        out logString, out totalCount, out errorCount);
                    log.AppendLine(logString);
                    log.AppendLine(string.Format(
                        "Создано {0} измерений, {1} измерений не удалось создать.", totalCount - errorCount, errorCount));
                    tbInfo.Text = log.ToString();
                }            
            }
            finally
            {
                frmDatabaseConnection.Dispose();
                ((IDisposable)generator).Dispose();                
            }            
        }
        
        private void btnCreateMDBase_Click(object sender, EventArgs e)
        {
            GenerateStandartDimensions();            
        }

        private void btnCreateCustomDimensions_Click(object sender, EventArgs e)
        {            
            GenerateCustomDimensions();
        }

        private void btnRefreshPlanningsheetMetadata_Click(object sender, EventArgs e)
        {
            tbInfo.Text = String.Empty;
            tbInfo.Text = "Старт обновления метаданных для надстройки MS Office";
            tbInfo.Refresh();
            workplace.OperationObj.Text = "Обновление метаданных для надстройки MS Office";
            workplace.OperationObj.StartOperation();
            try
            {
                workplace.ActiveScheme.PlaningProvider.RefreshMetaData();
                tbInfo.Text = tbInfo.Text + Environment.NewLine + Environment.NewLine
                    + "Обновление метаданных для надстройки MS Office закончилось успешно";
            }
            catch (Exception exc)
            {
                tbInfo.Text = tbInfo.Text + Environment.NewLine + Environment.NewLine
                    + "Обновление метаданных для надстройки MS Office закончилось с ошибкой:" + Environment.NewLine + Environment.NewLine +
                    exc.Message + Environment.NewLine +
                    exc.StackTrace;
            }
            finally
            {
                workplace.OperationObj.StopOperation();
            }
        }
       
        public bool Connected
        {
            get { return workplace != null; }
        }        
    }
}
