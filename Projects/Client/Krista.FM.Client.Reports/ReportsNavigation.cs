using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using Krista.FM.Client.Common.Forms;
using Krista.FM.Client.Reports.Common;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.EGRIP.Commands;
using Krista.FM.Client.Reports.EGRUL.Commands;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.Reports
{
    public class ReportMenuParams
    {
        public UltraToolbarsManager tbManager;
        public UltraToolbar tb;
        public ImageList il;
    }

    public class IFReportMenu
    {
        const string mainMenuItem = "pmReports";
        const string creditIssuedMenuItem = "Templates";
        const string egrulMenuItem = "menuReports";

        private readonly ReportMenuParams listParams;

        private bool hasCommonReportPath;
        private Collection<string> commonReportPath;

        private Collection<string> reportFolders;
        private Collection<string> reportFoldersCaption;
        private Collection<int> reportFoldersSortIndex;
        private Collection<int> reportFoldersLevels;

        private Collection<CommonReportsCommand> reportButtons;
        private Collection<string> reportParams;
        private Collection<int> reportSortIndex;

        private string rootMenuItem;

        public IScheme scheme;
        public IWin32Window window;
        public Operation operationObj;

        public IFReportMenu(ReportMenuParams menuListParams)
        {
            listParams = menuListParams;
        }

        private void CreateServiceLists()
        {
            commonReportPath = new Collection<string>();
            hasCommonReportPath = false;

            reportButtons = new Collection<CommonReportsCommand>();
            reportParams = new Collection<string>();
            reportSortIndex = new Collection<int>();

            reportFolders = new Collection<string>();
            reportFoldersCaption = new Collection<string>();
            reportFoldersSortIndex = new Collection<int>();
            reportFoldersLevels = new Collection<int>();
        }

        public void IFMainReportMenuList()
        {
            CreateServiceLists();
            rootMenuItem = mainMenuItem;
            // common
            AddReportStructure(new ReportAutoTesterCommand());
            AddReportStructure(new OrganizationDebtCommand());
            AddReportStructure(new BudgetDebtCommand());
            //AddReportStructure(new ReportCreditConstructorCommand());
            //AddReportStructure(new ReportGarantConstructorCommand());
            //AddReportStructure(new ReportCreditIssConstructorCommand());
            //omsk
            AddReportStructure(new CreditInformationOmskCommand());
            AddReportStructure(new ReportChargesDebtInformationCommand());
            AddReportStructure(new ReportCreditExtinguishingDates());
            AddReportStructure(new ReportChargesServicePeriod());
            AddReportStructure(new ReportExtinguishingMainDebt());
            AddReportStructure(new ReportPercentDebtDifference());
            AddReportStructure(new ReportMainDebtDifference());
            AddReportStructure(new ReportBudgetCreditLinesObligations());
            AddReportStructure(new ReportAdminBudgetSourceDeficitCommand());
            AddReportStructure(new ReportBudgetSourceDeficitCommand());
            AddReportStructure(new ReportBorrowingCommand());
            AddReportStructure(new ReportDebtStructureCommand());
            AddReportStructure(new ReportGarantProgrammCommand());
            AddReportStructure(new ReportBudgetCreditIssuedCommand());
            AddReportStructure(new ReportDebtorBookOmskCommand());
            AddReportStructure(new ReportDebtContractInformationCommand());
            AddReportStructure(new ReportBKIndicatorsCommand());
            AddReportStructure(new ReportCollationIFCommand());
            AddReportStructure(new ReportCollationDRCommand());
            AddReportStructure(new ReportAdminBudgetSourceDeficitOmskCommand());
            AddReportStructure(new ReportBudgetSourceDeficitOmskCommand());
            AddReportStructure(new ReportBudgetSourceDeficitPeriodOmskCommand());
            AddReportStructure(new ReportBorrowingOmskCommand());
            AddReportStructure(new ReportBorrowingPeriodOmskCommand());
            AddReportStructure(new ReportDebtStructureOmskCommand());
            AddReportStructure(new ReportDebtStructurePeriodOmskCommand());
            AddReportStructure(new ReportGarantProgrammOmskCommand());
            AddReportStructure(new ReportGarantProgrammPeriodOmskCommand());
            AddReportStructure(new ReportGarantProgrammWordOmskCommand());
            AddReportStructure(new ReportGarantProgrammPeriodWordOmskCommand());
            AddReportStructure(new ReportBorrowingWordOmskCommand());
            AddReportStructure(new ReportBorrowingPeriodWordOmskCommand());
            AddReportStructure(new CreditInformationOmskCityCommand());
            AddReportStructure(new ReportChargesDebtInformationOmskCityCommand());
            // vologda
            AddReportStructure(new ReportCertificatePercentDebtObligationsCommand());
            AddReportStructure(new ReportCertificateDebtObligationsCommand());
            AddReportStructure(new ReportBudgetSourceDeficitVologdaCommand());
            AddReportStructure(new ReportGarantProgrammVologdaCommand());
            AddReportStructure(new ReportBorrowingProgrammVologdaCommand());
            AddReportStructure(new ReportGarantCardVologdaCommand());
            AddReportStructure(new ReportGarantChangesVologdaCommand());
            AddReportStructure(new ReportCreditCalcPercentVologdaCommand());
            AddReportStructure(new ReportAnalisysFinSupportVologdaCommand());
            //yaroslavl
            AddReportStructure(new ReportDebtorBookYaroslavlCommand());
            AddReportStructure(new ReportMunicipalDebtYarCommand());
            AddReportStructure(new ReportStateDebt1YarCommand());
            AddReportStructure(new ReportStateDebt1ShortYarCommand());
            AddReportStructure(new ReportStateDebt2YarCommand());
            AddReportStructure(new ReportStateDebt3YarCommand());
            AddReportStructure(new ReportStateDebt4YarCommand());
            AddReportStructure(new ReportStateDebt5YarCommand());
            AddReportStructure(new ReportStateDebt6YarCommand());
            AddReportStructure(new ReportGarantProgrammWordYarCommand());
            AddReportStructure(new ReportBorrowingProgrammWordYarCommand());
            AddReportStructure(new ReportBudgetSourceDeficitYarCommand());
            AddReportStructure(new ReportBudgetSourceDeficitPeriodYarCommand());
            AddReportStructure(new ReportGraphDebtYaroslavlCommand());
            AddReportStructure(new ReportDebtorBookYarOblCommand());
            AddReportStructure(new ReportDebtorBookSummaryYarOblCommand());
            AddReportStructure(new ReportDebtorBookSelectYarOblCommand());
            // saratov
            AddReportStructure(new ReportBudgetSourceDeficitSaratovCommand());
            AddReportStructure(new ReportGarantProgrammSaratovCommand());
            AddReportStructure(new ReportBorrowingSaratovCommand());
            AddReportStructure(new ReportPlanDebtSaratovCommand());
            AddReportStructure(new ReportBKSaratovCommand());
            AddReportStructure(new ReportCreditIssuedSaratovCommand());
            AddReportStructure(new ReportExtractDKSaratovCommand());
            AddReportStructure(new ReportReestrBudCreditSaratovCommand());
            AddReportStructure(new ReportSubjectDebtorBookSaratovCommand());
            // samara
            AddReportStructure(new ReportVaultSamaraCommand());
            AddReportStructure(new ReportVaultDKSamaraCommand());
            AddReportStructure(new ReportRateSwitchSamaraCommand());
            AddReportStructure(new ReportGarantProgrammSamaraCommand());
            AddReportStructure(new ReportBorrowingProgrammSamaraCommand());
            AddReportStructure(new ReportBudgetSourceDeficitSamaraCommand());
            AddReportStructure(new ReportBudgetSourceDeficitPeriodSamaraCommand());
            AddReportStructure(new ReportBudgetCreditIssuedSamaraCommand());
            AddReportStructure(new ReportDebtorBookSamaraOblCommand());
            // ДК МФРФ
            AddReportStructure(new ReportMFRFDKCapitalCommand());
            AddReportStructure(new ReportMFRFDKCreditOrgCommand());
            AddReportStructure(new ReportMFRFDKGarantCommand());
            AddReportStructure(new ReportMFRFDKCreditBudCommand());
            // Московская область
            AddReportStructure(new ReportMOSummaryDebtInfoCommand());
            AddReportStructure(new ReportMOExtractDebtBookFullCommand());
            AddReportStructure(new ReportMOExtractDebtBookShortCommand());
            AddReportStructure(new ReportMOCreditInfoCommand());
            AddReportStructure(new ReportMOGarantInfoCommand());
            AddReportStructure(new ReportMOCashPlanCommand());
            AddReportStructure(new ReportMODebtServiceLoadingCommand());
            AddReportStructure(new ReportMOExtractDebtBookBuhCommand());
            AddReportStructure(new ReportMOContractInfoCommand());
            AddReportStructure(new ReportMOContractInfoForm58Command());
            AddReportStructure(new ReportMOGarantDebtHistoryCommand());
            AddReportStructure(new ReportMOCreditDebtHistoryCommand());
            AddReportStructure(new ReportMOCapitalDebtHistoryCommand());
            AddReportStructure(new ReportMOPlanServicesCommand());
            AddReportStructure(new ReportMOCalcServiceCommand());
            AddReportStructure(new ReportMOForecastDebtVolumeChangeCommand());
            AddReportStructure(new ReportMOExchangeRateCommand());
            AddReportStructure(new ReportMODrawingGovFactoryCommand());
            AddReportStructure(new ReportMODrawingGUPBuhCommand());
            AddReportStructure(new ReportMODrawingMunFactoryCommand());
            AddReportStructure(new ReportMODrawingMUPBuhCommand());
            // Калмыкия 
            AddReportStructure(new ReportGraphDebtKalmykiaCommand());
            AddReportStructure(new ReportVaultGraphDebtKalmykiaCommand());

            AddReportFolders();
            AddReportButtons(listParams.tb);
        }

        public void IFCreditIssuedReportMenuList()
        {
            CreateServiceLists();
            rootMenuItem = creditIssuedMenuItem;
            AddReportStructure(new ReportPeniPercentMainDebtVologdaCommand());
            AddReportStructure(new ReportPeniPercentMainDebtCurMonthVologdaCommand());
            AddReportStructure(new ReportPlanServiceVologdaCommand());
            AddReportStructure(new ReportMainDebtWithTestVologdaCommand());
            AddReportStructure(new ReportMainDebtBudgetCreditVologdaCommand());
            AddReportStructure(new ReportCertificateCalcPercentPennySubjectVologdaCommand());

            AddReportFolders();
            AddReportButtons(listParams.tb);
        }

        public void UFKEGRULReportMenuList()
        {
            CreateServiceLists();
            rootMenuItem = egrulMenuItem;
            
            AddReportStructure(new ReportEGRUL0001Command());
            AddReportStructure(new ReportEGRUL0002Command());
            AddReportStructure(new ReportEGRUL0003Command());
            AddReportStructure(new ReportEGRUL0004Command());
            AddReportStructure(new ReportEGRUL0005Command());

            AddReportFolders();
            AddReportButtons(listParams.tb);
        }

        public void UFKEGRIPReportMenuList()
        {
            CreateServiceLists();
            rootMenuItem = egrulMenuItem;
            AddReportStructure(new ReportEGRIP0001Command());
            AddReportStructure(new ReportEGRIP0002Command());
            AddReportFolders();
            AddReportButtons(listParams.tb);
        }

        private static string CreateReportMenuItemName(string key, int index)
        {
            return string.Format("pmReportFolder{0}", GetReportParamStrPart(key, index));
        }

        private static string GetReportParamStrPart(string key, int index)
        {
            return key.Split('=')[index];
        }

        private static int GetFolderOrder(string orderNum)
        {
            return orderNum.Length > 0 ? Convert.ToInt32(orderNum) : 0;
        }

        private void AddFolderStructure(string key, string caption, int folderLvl)
        {
            if (reportFolders.Contains(key))
            {
                return;
            }

            var sortIndex = GetFolderOrder(GetReportParamStrPart(key, 2));
            var insertIndex = 0;

            for (var i = 0; i < reportFoldersSortIndex.Count; i++)
            {
                if (reportFoldersSortIndex[i] <= sortIndex)
                {
                    insertIndex++;
                }
            }

            reportFolders.Insert(insertIndex, key);
            reportFoldersCaption.Insert(insertIndex, caption);
            reportFoldersLevels.Insert(insertIndex, folderLvl);
            reportFoldersSortIndex.Insert(insertIndex, sortIndex);
        }

        private void AddReportStructure(CommonReportsCommand reportCommand)
        {
            reportCommand.scheme = scheme;
            reportCommand.operationObj = operationObj;
            reportCommand.window = window;

            if (!reportCommand.CheckReportTemplate())
            {
                return;
            }

            var parentCollection = reportCommand.GetParentPath();
            var pmFolder = rootMenuItem;
            var folderLvl = 0;

            foreach (var valuePair in parentCollection)
            {
                pmFolder = CreateReportMenuItemName(valuePair.Key, 0);
                AddFolderStructure(valuePair.Key, valuePair.Value, folderLvl);

                if (!hasCommonReportPath)
                {
                    // если общий путь еще не заполнялся, то заполним
                    commonReportPath.Add(valuePair.Key);
                }
                else
                {
                    // пошло различие в общем пути начиная с этого уровня
                    if (commonReportPath.Count > folderLvl && commonReportPath[folderLvl] != valuePair.Key)
                    {
                        var maxIndex = commonReportPath.Count;

                        for (var i = folderLvl; i < maxIndex; i++)
                        {
                            //commonReportPath.RemoveAt(i);
                            commonReportPath.RemoveAt(folderLvl);
                        }
                    }
                }

                folderLvl++;
            }

            hasCommonReportPath = true;

            if (parentCollection.Count == 0)
            {
                commonReportPath.Clear();
            }

            var sortIndex = reportCommand.GetReportSortIndex();
            var insertIndex = 0;

            for (var i = 0; i < reportSortIndex.Count; i++)
            {
                if (reportSortIndex[i] <= sortIndex)
                {
                    insertIndex++;
                }
            }

            reportButtons.Insert(insertIndex, reportCommand);
            reportParams.Insert(insertIndex, pmFolder);
            reportSortIndex.Insert(insertIndex, sortIndex);
        }

        private void AddReportFolders()
        {
            for (var i = 0; i < reportFolders.Count; i++)            
            {
                var pmReports = (PopupMenuTool)listParams.tbManager.Tools[rootMenuItem];

                if (commonReportPath.Contains(reportFolders[i]))
                {
                    continue;
                }

                var pmFolder = new PopupMenuTool(CreateReportMenuItemName(reportFolders[i], 0));
                pmFolder.SharedProps.Caption = reportFoldersCaption[i];
                listParams.tbManager.Tools.Add(pmFolder);
                var index = listParams.tbManager.Tools.IndexOf(CreateReportMenuItemName(reportFolders[i], 1));
                    
                if (index >= 0)
                {
                    pmReports = (PopupMenuTool) listParams.tbManager.Tools[index];
                }

                pmReports.Tools.AddTool(CreateReportMenuItemName(reportFolders[i], 0));
            }
        }

        private void AddReportButtons(UltraToolbar toolBar)
        {
            for (var i = 0; i < reportButtons.Count; i++)
            {
                var parentTool = rootMenuItem;
                
                // а вот не работает поиск по коллекции по строковому ключу почему-то...
                foreach (var tool in listParams.tbManager.Tools)
                {
                    if (tool.Key == reportParams[i])
                    {
                        parentTool = reportParams[i];
                    }
                }

                var btnCreateReport = ReportCommandService.AttachToolbarTool(reportButtons[i], toolBar, parentTool);

                if (listParams.il != null)
                {
                    btnCreateReport.SharedProps.AppearancesSmall.Appearance.Image = listParams.il.Images[reportButtons[i].GetImageIndex()];
                }
            }
        }
    }
}