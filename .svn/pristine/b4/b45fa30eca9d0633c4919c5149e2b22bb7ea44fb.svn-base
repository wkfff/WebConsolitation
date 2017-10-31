using System;
using System.Drawing;
using System.Windows.Forms;
using Krista.FM.Common.OfficeHelpers;
using Krista.FM.ServerLibrary;
using Krista.FM.Common;

using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;

namespace Krista.FM.Client.Common.Forms
{
    public partial class FormAbout : Form
    {
        /// <summary>
        /// Информация об активной схеме
        /// </summary>
        IScheme scheme;

        public FormAbout()
        {
            InitializeComponent();
        }

        public FormAbout(IScheme scheme)
            : this()
        {
            this.scheme = scheme;
        }

        private void CheckMainVersions()
        {
            if (DBVersion != null)
            {
                if (mainVersion != AppVersionControl.GetAssemblyBaseVersion(DBVersion))
                {
                    this.laVersion.ForeColor = Color.Red;
                    this.laDatabaseVersion.ForeColor = Color.Red;
                }
                else
                {
                    this.laVersion.ForeColor = Control.DefaultForeColor;
                    this.laDatabaseVersion.ForeColor = Control.DefaultForeColor;
                }
            }
        }


        private string aboutProductName;

        public string AboutProductName
        {
            get { return aboutProductName; }
            set
            {
                aboutProductName = value;
                laProgramName.Text = aboutProductName;//String.Format("АИС {0}", aboutProductName);
            }
        }


        private string supportServiceInfo;

        public string SupportServiceInfo
        {
            get { return supportServiceInfo; }
            set
            {
                supportServiceInfo = value;
                laFmSupportHttpAddr.Text = supportServiceInfo;
                string[] splitedInformation = supportServiceInfo.Split(' ');
                string mail = string.Empty;
                foreach (string str in splitedInformation)
                {
                    if (str.Contains("@"))
                    {
                        mail = str;
                        break;
                    }
                }
                laFmSupportHttpAddr.Links.Add(supportServiceInfo.IndexOf(mail), mail.Length, mail);
            }
        }


        private string mainVersion;

        public string MainVersion
        {
            get { return mainVersion; }
            set
            {
                mainVersion = value;
                this.laVersion.Text = String.Format("Базовая версия сервера: {0}", mainVersion);
                CheckMainVersions();
            }
        }

        private string mainModulesVersion;
        public string MainModulesVersion
        {
            get { return mainModulesVersion; }
            set
            {
                mainModulesVersion = value;
                this.MainVersion = AppVersionControl.GetAssemblyBaseVersion(mainModulesVersion);
            }
        }

        private string dbVersion;

        public string DBVersion
        {
            get { return dbVersion; }
            set
            {
                dbVersion = value;
                this.laDatabaseVersion.Text = String.Format("Версия базы данных: {0}", dbVersion);
                CheckMainVersions();
            }
        }

        private void LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void FormAbout_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Escape))
            {
                btnOK.PerformClick();
            }
        }

        private void ugAssemblyes_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            string version = Convert.ToString(e.Row.Cells["assemblyVersion"].Value);
            if (version != MainModulesVersion)
            {
                e.Row.Appearance.ForeColor = System.Drawing.Color.Red;
                e.Row.ToolTipText = "Версия сборки отличается от основной версии системы";
            }
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            string fileName = "О программе";
            ExportImportHelper.GetFileName(fileName, ExportImportHelper.fileExtensions.xls, ref fileName);
            CreateAboutReport(fileName);
        }

        /// <summary>
        /// создание отчета по версиям программным модулям в Excell
        /// </summary>
        /// <param name="fileName"></param>
        private void CreateAboutReport(string fileName)
        {
            Infragistics.Excel.Workbook wb = new Infragistics.Excel.Workbook();
            UltraGridExcelExporter excelExpoter = new UltraGridExcelExporter();

            Infragistics.Excel.Worksheet wsAll = wb.Worksheets.Add("All");
            Infragistics.Excel.Worksheet wsSuspect = wb.Worksheets.Add("Suspect");
            Infragistics.Excel.Worksheet wsPatch = wb.Worksheets.Add("Patches");

            wsAll.Rows[1].Cells[0].Value = String.Format("Версия базы данных: {0}", DBVersion);
            wsAll.Rows[0].Cells[0].Value = String.Format("Базовая версия сервера: {0}", MainVersion);

            excelExpoter.Export(controlSystemInfo1.UltraGridSuspect, wsSuspect, 0, 0);
            excelExpoter.Export(controlSystemInfo1.UltraGridAll, wsAll, 3, 0);
            excelExpoter.Export(controlSystemInfo1.UltraGridPatch, wsPatch, 0, 0);
            //wb.Save(fileName);
            Infragistics.Excel. BIFF8Writer.WriteWorkbookToFile(wb, fileName);

            excelExpoter.Dispose();

            using (ExcelApplication excel = OfficeHelper.CreateExcelApplication())
            {
                excel.OpenFile(fileName, true, true);
            }
        }

        public void InformationTread()
        {
            ControlInfo.InitializeGrid(scheme);
        }

        public ControlSystemInfo ControlInfo
        {
            get { return controlSystemInfo1; }
        }

        private void laKristaHttpAddr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Text);
        }

        private void laFmSupportHttpAddr_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string email = e.Link.LinkData.ToString();
            if (!email.ToUpper().Contains("MAILTO:"))
                email = "mailto:" + email;
            System.Diagnostics.Process.Start(email);
        }
    }

    public struct About
    {  
        public static void ShowAbout(IScheme scheme, IWin32Window parentWindow)
        {
            if (scheme == null)
                return;
            
            //Dictionary<string, string> clientAssemblyes = AppVersionControl.GetAssemblyesVersions(AppVersionControl.ClientAssemblyesSearchMaskDll);
            //Dictionary<string, string> serverAssemblyes = scheme.UsersManager.GetServerAssemblyesInfo();

            FormAbout about = new FormAbout(scheme);
            // версия программных модулей
            about.MainModulesVersion = scheme.UsersManager.ServerLibraryVersion();
            // версия базы данных
            about.DBVersion = scheme.SchemeDWH.DatabaseVersion;
            about.AboutProductName = scheme.Server.GetConfigurationParameter("ProductName");
            about.SupportServiceInfo = scheme.Server.GetConfigurationParameter("SupportServiceInfo");
            about.ControlInfo.MainForm = (Form) parentWindow;
            about.InformationTread();
            about.ShowDialog(parentWindow);
            /*
            about.udsAssemblies.EventManager.SetEnabled(DataSourceEventGroups.AllEvents, false);
            about.ugAssemblyes.BeginUpdate();
            try
            {
                foreach (string assName in clientAssemblyes.Keys)
                {
                    UltraDataRow row = about.udsAssemblies.Rows.Add(false);
                    row["assemblyName"] = assName;
                    row["assemblyVersion"] = clientAssemblyes[assName];
                    row["placement"] = "Клиент";
                }
                foreach (string assName in serverAssemblyes.Keys)
                {
                    UltraDataRow row = about.udsAssemblies.Rows.Add(false);
                    row["assemblyName"] = assName;
                    row["assemblyVersion"] = serverAssemblyes[assName];
                    row["placement"] = "Сервер";
                }
                about.ugAssemblyes.DisplayLayout.Bands[0].SortedColumns.Add("placement", false, true);
                about.ugAssemblyes.DisplayLayout.Rows.ExpandAll(true);
                about.ugAssemblyes.Selected.Rows.Clear();
            }
            finally
            {
                about.udsAssemblies.EventManager.SetEnabled(DataSourceEventGroups.AllEvents, true);
                about.ugAssemblyes.EndUpdate();
            }
             * */
            //Application.DoEvents();
            //IWin32Window tmp = (IWin32Window)(Control)workplace;            
        }
       
    }
}