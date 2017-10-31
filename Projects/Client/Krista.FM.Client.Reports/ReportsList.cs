using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Planning;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Capital;
using Krista.FM.Client.Reports.Common.Commands;
using Krista.FM.Client.Reports.Common.CommonParamForm.Books.Variants;

namespace Krista.FM.Client.Reports
{
    [Description("ReportChargesDebtInformation")]
    public class ReportChargesDebtInformationCommand : CommonReportsCommand
    {
        public ReportChargesDebtInformationCommand()
        {
            key = "ReportChargesDebtInformation";
            caption = "������� �� ������������ �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetChargesDebtDataOmsk(reportParams);
        }
    }

    [Description("ReportCertificatePercentDebtObligations")]
    public class ReportCertificatePercentDebtObligationsCommand : CommonReportsCommand
    {
        public ReportCertificatePercentDebtObligationsCommand()
        {
            key = "ReportCertificatePercentDebtObligations";
            caption = "������� � ���������� � ��������� ��������� � ������ ������������ �� �������� �������������� ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtObligationsData(reportParams);
        }
    }

    [Description("ReportCertificateDebtObligations")]
    public class ReportCertificateDebtObligationsCommand : CommonReportsCommand
    {
        public ReportCertificateDebtObligationsCommand()
        {
            key = "ReportCertificateDebtObligations";
            caption = "������� � ����������� � ��������� �������� ������������ ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtObligationsData(reportParams);
        }
    }

    [Description("CreditInformationOmsk")]
    public class CreditInformationOmskCommand : CommonReportsCommand
    {
        public CreditInformationOmskCommand()
        {
            key = "CreditInformationOmsk";
            caption = "���������� �� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddEnumParam(ReportConsts.ParamVariantType, typeof(ContractTypeEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditOmskReportData(reportParams);
        }
    }

    [Description("ReportDebtorBookYaroslavl")]
    public class ReportDebtorBookYaroslavlCommand : CommonReportsCommand
    {
        public ReportDebtorBookYaroslavlCommand()
        {
            key = "ReportDebtorBookYaroslavl";
            caption = "�������� ����� ���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookDataYaroslavl(reportParams);
        }
    }

    [Description("OrganizationCreditsIncoming")]
    public class OrganizationDebtCommand : CommonReportsCommand
    {
        public OrganizationDebtCommand()
        {
            key = "OrganizationCreditsIncoming";
            caption = "����������� ������������� �� �������� �� ��������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetOrganizationReportData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]));
        }
    }

    [Description("BudgetCreditsIncoming")]
    public class BudgetDebtCommand : CommonReportsCommand
    {
        public BudgetDebtCommand()
        {
            key = "BudgetCreditsIncoming";
            caption = "����������� ������������� �� �������� �� ������ ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetReportData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]));
        }
    }

    [Description("GovernmentGuarantee")]
    public class GovernmentGuaranteeReportCommand : CommonReportsCommand
    {
        public GovernmentGuaranteeReportCommand()
        {
            key = "GovernmentGuarantee";
            caption = "����� � ��������������� ���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            var garantServer = new GuaranteeReportServer(scheme);
            return garantServer.GetGovernmentGuaranteeReportData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]),
                Convert.ToInt32(reportParams[ReportConsts.ParamVariantID]));
        }
    }

    [Description("ReportCreditExtinguishingDates")]
    public class ReportCreditExtinguishingDates : CommonReportsCommand
    {
        public ReportCreditExtinguishingDates()
        {
            key = "ReportCreditExtinguishingDates";
            caption = "����� ������� ��������";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditExtinguishingData();
        }
    }

    [Description("ReportChargesServicePeriod")]
    public class ReportChargesServicePeriod : CommonReportsCommand
    {
        public ReportChargesServicePeriod()
        {
            key = "ReportChargesServicePeriod";
            caption = "������� �� ������������ ����� �� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetChargesServicePeriodData(reportParams);
        }
    }

    [Description("ReportExtinguishingMainDebt")]
    public class ReportExtinguishingMainDebt : CommonReportsCommand
    {
        public ReportExtinguishingMainDebt()
        {
            key = "ReportExtinguishingMainDebt";
            caption = "������� ��������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetExtinguishingMainDebtData(reportParams);
        }
    }

    [Description("ReportPercentDebtDifference")]
    public class ReportPercentDebtDifference : CommonReportsCommand
    {
        public ReportPercentDebtDifference()
        {
            key = "ReportPercentDebtDifference";
            caption = "��������� ������������� �� ���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetPercentDebtDifferenceData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]));
        }
    }

    [Description("ReportMainDebtDifference")]
    public class ReportMainDebtDifference : CommonReportsCommand
    {
        public ReportMainDebtDifference()
        {
            key = "ReportMainDebtDifference";
            caption = "��������� ������������� �� ��������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMainDebtDifferenceData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]));
        }
    }

    [Description("ReportBudgetCreditLinesObligations")]
    public class ReportBudgetCreditLinesObligations : CommonReportsCommand
    {
        public ReportBudgetCreditLinesObligations()
        {
            key = "ReportBudgetCreditLinesObligations";
            caption = "������������� ������� � ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetReportBudgetCreditLinesObligationsData(
                Convert.ToDateTime(reportParams[ReportConsts.ParamEndDate]));
        }
    }

    [Description("ReportBudgetSourceDeficit")]
    public class ReportBudgetSourceDeficitCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitCommand()
        {
            key = "ReportBudgetSourceDeficit";
            caption = "��������� �������������� �������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, true);
        }
    }

    [Description("ReportAdminBudgetSourceDeficit")]
    public class ReportAdminBudgetSourceDeficitCommand : CommonReportsCommand
    {
        public ReportAdminBudgetSourceDeficitCommand()
        {
            key = "ReportAdminBudgetSourceDeficit";
            caption = "�������������� ����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetAdminBudgetSourceDeficitData(reportParams);
        }
    }

    [Description("ReportDebtStructure")]
    public class ReportDebtStructureCommand : CommonReportsCommand
    {
        public ReportDebtStructureCommand()
        {
            key = "ReportDebtStructure";
            caption = "��������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtStructureData(reportParams);
        }
    }

    [Description("ReportBorrowing")]
    public class ReportBorrowingCommand : CommonReportsCommand
    {
        public ReportBorrowingCommand()
        {
            key = "ReportBorrowing";
            caption = "�������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingData(reportParams);
        }
    }

    [Description("ReportBudgetCreditIssued")]
    public class ReportBudgetCreditIssuedCommand : CommonReportsCommand
    {
        public ReportBudgetCreditIssuedCommand()
        {
            key = "ReportBudgetCreditIssued";
            caption = "��������� ������� ���������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetCreditIssuedData(reportParams);
        }
    }

    [Description("ReportDebtorBookOmsk")]
    public class ReportDebtorBookOmskCommand : CommonReportsCommand
    {
        public ReportDebtorBookOmskCommand()
        {
            key = "ReportDebtorBookOmsk";
            caption = "�������� ����� ����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookOmskData(reportParams);
        }
    }

    [Description("ReportGarantProgramm")]
    public class ReportGarantProgrammCommand : CommonReportsCommand
    {
        public ReportGarantProgrammCommand()
        {
            key = "ReportGarantProgramm";
            caption = "��������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammData(reportParams);
        }
    }

    [Description("ReportDebtContractInformation")]
    public class ReportDebtContractInformationCommand : CommonReportsCommand
    {
        public ReportDebtContractInformationCommand()
        {
            key = "ReportDebtContractInformation";
            caption = "���������� �� �������� ��������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtContractInformationData(reportParams);
        }
    }

    [Description("ReportMunicipalDebtYar")]
    public class ReportMunicipalDebtYarCommand : CommonReportsCommand
    {
        public ReportMunicipalDebtYarCommand()
        {
            key = "ReportMunicipalDebtYar";
            caption = "������������� ����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMunicipalDebtYarData(reportParams);
        }
    }

    [Description("ReportBKIndicators")]
    public class ReportBKIndicatorsCommand : CommonReportsCommand
    {
        public ReportBKIndicatorsCommand()
        {
            key = "ReportBKIndicators";
            caption = "������������ ����������� ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow())
                .SetValue(ReportConsts.DefBookID);
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncome());
            paramBuilder.AddBookParam(ReportConsts.ParamVariantRID, new ParamVariantOutcome());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBKIndicatorsData(reportParams);
        }
    }

    [Description("ReportCollationIF")]
    public class ReportCollationIFCommand : CommonReportsCommand
    {
        public ReportCollationIFCommand()
        {
            key = "ReportCollationIF";
            caption = "������ �� � ������ �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCollationIFData(reportParams);
        }
    }

    [Description("ReportCollationDR")]
    public class ReportCollationDRCommand : CommonReportsCommand
    {
        public ReportCollationDRCommand()
        {
            key = "ReportCollationDR";
            caption = "������ �������� � ������� � ������ �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCollationDRData(reportParams);
        }
    }

    [Description("ReportBorrowingOmsk")]
    public class ReportBorrowingOmskCommand : CommonReportsCommand
    {
        public ReportBorrowingOmskCommand()
        {
            key = "ReportBorrowingOmsk";
            caption = "�������.�������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingData(reportParams);
        }
    }

    [Description("ReportDebtStructureOmsk")]
    public class ReportDebtStructureOmskCommand : CommonReportsCommand
    {
        public ReportDebtStructureOmskCommand()
        {
            key = "ReportDebtStructureOmsk";
            caption = "��������� �������.�����";          
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtStructureData(reportParams);
        }
    }

    [Description("ReportGarantProgrammOmsk")]
    public class ReportGarantProgrammOmskCommand : CommonReportsCommand
    {
        public ReportGarantProgrammOmskCommand()
        {
            key = "ReportGarantProgrammOmsk";
            caption = "��������� �������.��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammData(reportParams);
        }
    }

    [Description("ReportDebtStructurePeriodOmsk")]
    public class ReportDebtStructurePeriodOmskCommand : CommonReportsCommand
    {
        public ReportDebtStructurePeriodOmskCommand()
        {
            key = "ReportDebtStructurePeriodOmsk";
            caption = "��������� �������.����� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtStructurePeriodData(reportParams);
        }
    }

    [Description("ReportBorrowingPeriodOmsk")]
    public class ReportBorrowingPeriodOmskCommand : CommonReportsCommand
    {
        public ReportBorrowingPeriodOmskCommand()
        {
            key = "ReportBorrowingPeriodOmsk";
            caption = "�������.������������� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingPeriodData(reportParams);
        }
    }

    [Description("ReportGarantProgrammPeriodOmsk")]
    public class ReportGarantProgrammPeriodOmskCommand : CommonReportsCommand
    {
        public ReportGarantProgrammPeriodOmskCommand()
        {
            key = "ReportGarantProgrammPeriodOmsk";
            caption = "��������� �������.�������� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammPeriodData(reportParams);
        }
    }

    [Description("ReportPeniPercentMainDebtVologda")]
    public class ReportPeniPercentMainDebtVologdaCommand : CommonReportsCommand
    {
        public ReportPeniPercentMainDebtVologdaCommand()
        {
            key = "ReportPeniPercentMainDebtVologda";
            caption = "������� �� ����������� ����� �� ������������ �������� � �������� ����� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetPeniPercentMainDebtData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitOmsk")]
    public class ReportBudgetSourceDeficitOmskCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitOmskCommand()
        {
            key = "ReportBudgetSourceDeficitOmsk";
            caption = "��������� �������������� �������� ������� �����.";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, true);
        }
    }

    [Description("ReportAdminBudgetSourceDeficitOmsk")]
    public class ReportAdminBudgetSourceDeficitOmskCommand : CommonReportsCommand
    {
        public ReportAdminBudgetSourceDeficitOmskCommand()
        {
            key = "ReportAdminBudgetSourceDeficitOmsk";
            caption = "�������������� ���������� �����.";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetAdminBudgetSourceDeficitData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitPeriodOmsk")]
    public class ReportBudgetSourceDeficitPeriodOmskCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitPeriodOmskCommand()
        {
            key = "ReportBudgetSourceDeficitPeriodOmsk";
            caption = "��������� �������������� �������� ����� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitPeriodData(reportParams);
        }
    }

    [Description("ReportPeniPercentMainDebtCurMonthVologda")]
    public class ReportPeniPercentMainDebtCurMonthVologdaCommand : CommonReportsCommand
    {
        public ReportPeniPercentMainDebtCurMonthVologdaCommand()
        {
            key = "ReportPeniPercentMainDebtCurMonthVologda";
            caption = "������� �� ����������� ����� � �������� ������ �� ������������ ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetPeniPercentMainDebtCurMonthData(reportParams);
        }
    }

    [Description("ReportPlanServiceYearVologda")]
    public class ReportPlanServiceVologdaCommand : CommonReportsCommand
    {
        public ReportPlanServiceVologdaCommand()
        {
            key = "ReportPlanServiceYearVologda";
            caption = "������� �� ����������� ��������� �� �������� ������� �� ������������� ������������ ������� �� 01 ������ ��������� ����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetPlanServiceYearVologdaData(reportParams);
        }
    }

    [Description("ReportMainDebtWithTestVologda")]
    public class ReportMainDebtWithTestVologdaCommand : CommonReportsCommand
    {
        public ReportMainDebtWithTestVologdaCommand()
        {
            key = "ReportMainDebtWithTestVologda";
            caption = "�������� ����� ������������� � �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMainDebtWithTestData(reportParams);
        }
    }

    [Description("ReportMainDebtBudgetCreditVologda")]
    public class ReportMainDebtBudgetCreditVologdaCommand : CommonReportsCommand
    {
        public ReportMainDebtBudgetCreditVologdaCommand()
        {
            key = "ReportMainDebtBudgetCreditVologda";
            caption = "���������� � �������������� ��������� �������� ������� �������� � �� ���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMainDebtBudgetCreditData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication6Yar")]
    public class ReportStateDebt6YarCommand : CommonReportsCommand
    {
        public ReportStateDebt6YarCommand()
        {
            key = "ReportStateDebtApplication6Yar";
            caption = "���������� 6. ����� � ���������� ������������ �� ��������������� ����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication6YarData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication5Yar")]
    public class ReportStateDebt5YarCommand : CommonReportsCommand
    {
        public ReportStateDebt5YarCommand()
        {
            key = "ReportStateDebtApplication5Yar";
            caption = "���������� 5. ����� � ��������� ���������������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication5YarData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication4Yar")]
    public class ReportStateDebt4YarCommand : CommonReportsCommand
    {
        public ReportStateDebt4YarCommand()
        {
            key = "ReportStateDebtApplication4Yar";
            caption = "���������� 4. ����� � ��������������� ��������������� ��������� � ���������� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication4YarData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication3Yar")]
    public class ReportStateDebt3YarCommand : CommonReportsCommand
    {
        public ReportStateDebt3YarCommand()
        {
            key = "ReportStateDebtApplication3Yar";
            caption = "���������� 3. ����� � ��������� ���������������� ����� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddEnumParam(ReportConsts.ParamSumModifier, typeof(SumDividerEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication3YarData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication2Yar")]
    public class ReportStateDebt2YarCommand : CommonReportsCommand
    {
        public ReportStateDebt2YarCommand()
        {
            key = "ReportStateDebtApplication2Yar";
            caption = "���������� 2. ����� � ��������� ���������������� ����� �� ����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("����");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniEuroRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication2YarData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication1Yar")]
    public class ReportStateDebt1YarCommand : CommonReportsCommand
    {
        public ReportStateDebt1YarCommand()
        {
            key = "ReportStateDebtApplication1Yar";
            caption = "���������� 1. ����� � ��������� ���������������� ����� �� ���� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("����");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());

            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniEuroRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication1YarData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitSaratov")]
    public class ReportBudgetSourceDeficitSaratovCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitSaratovCommand()
        {
            key = "ReportBudgetSourceDeficitSaratov";
            caption = "��������� �������������� �������� ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, false);
        }
    }

    [Description("ReportGarantProgrammSaratov")]
    public class ReportGarantProgrammSaratovCommand : CommonReportsCommand
    {
        public ReportGarantProgrammSaratovCommand()
        {
            key = "ReportGarantProgrammSaratov";
            caption = "��������� ��������������� �������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammSaratovData(reportParams);
        }
    }

    [Description("ReportBorrowingSaratov")]
    public class ReportBorrowingSaratovCommand : CommonReportsCommand
    {
        public ReportBorrowingSaratovCommand()
        {
            key = "ReportBorrowingSaratov";
            caption = "��������� ��������������� ���������� ������������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingSaratovData(reportParams);
        }
    }

    [Description("ReportPlanDebtSaratov")]
    public class ReportPlanDebtSaratovCommand : CommonReportsCommand
    {
        public ReportPlanDebtSaratovCommand()
        {
            key = "ReportPlanDebtSaratov";
            caption = "������������ �������� �� ������������ �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetPlanDebtSaratovData(reportParams);
        }
    }

    [Description("ReportBKSaratov")]
    public class ReportBKSaratovCommand : CommonReportsCommand
    {
        public ReportBKSaratovCommand()
        {
            key = "ReportBKSaratov";
            caption = "���������� ���������� �� ��";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddBookParam(ReportConsts.ParamVariantDID, new ParamVariantIncome());
            paramBuilder.AddBookParam(ReportConsts.ParamVariantRID, new ParamVariantOutcome());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBKSaratovData(reportParams);
        }
    }

    [Description("ReportCreditIssuedSaratov")]
    public class ReportCreditIssuedSaratovCommand : CommonReportsCommand
    {
        public ReportCreditIssuedSaratovCommand()
        {
            key = "ReportCreditIssuedSaratov";
            caption = "����� �� �������������� ��������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditIssuedSaratovData(reportParams);
        }
    }

    [Description("ReportExtractDKSaratov")]
    public class ReportExtractDKSaratovCommand : CommonReportsCommand
    {
        public ReportExtractDKSaratovCommand()
        {
            key = "ReportExtractDKSaratov";
            caption = "������� �� ��������������� �������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamOrgID, new ParamOrganizationPlan());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetExtractDKSaratovData(reportParams);
        }
    }

    [Description("ReportCertificateCalcPercentPennyVologda")]
    public class ReportCertificateCalcPercentPennySubjectVologdaCommand : CommonReportsCommand
    {
        public ReportCertificateCalcPercentPennySubjectVologdaCommand()
        {
            key = "ReportCertificateCalcPercentPennyVologda";
            caption = "�������-������ ����������� ��������� � ����� �� ��������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCertificateCalcPercentPennyVologdaData(reportParams, false);
        }
    }

    [Description("ReportCertificateCalcPercentPennyVologda")]
    public class ReportCertificateCalcPercentPennyMOVologdaCommand : CommonReportsCommand
    {
        public ReportCertificateCalcPercentPennyMOVologdaCommand()
        {
            key = "ReportCertificateCalcPercentPennyVologda";
            caption = "�������-������ ����������� ��������� � ����� �� ��������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCertificateCalcPercentPennyVologdaData(reportParams, true);
        }
    }

    [Description("ReportRateSwitchSamara")]
    public class ReportRateSwitchSamaraCommand : CommonReportsCommand
    {
        public ReportRateSwitchSamaraCommand()
        {
            key = "ReportRateSwitchSamara";
            caption = "�������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetRateSwitchSamaraData(reportParams);
        }
    }

    [Description("ReportVaultSamara")]
    public class ReportVaultSamaraCommand : CommonReportsCommand
    {
        public ReportVaultSamaraCommand()
        {
            key = "ReportVaultSamara";
            caption = "�������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetVaultSamaraData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitSamara")]
    public class ReportBudgetSourceDeficitSamaraCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitSamaraCommand()
        {
            key = "ReportBudgetSourceDeficitSamara";
            caption = "�� ��������� ��� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, false);
        }
    }

    [Description("ReportGarantProgrammSamara")]
    public class ReportGarantProgrammSamaraCommand : CommonReportsCommand
    {
        public ReportGarantProgrammSamaraCommand()
        {
            key = "ReportGarantProgrammSamara";
            caption = "��������� ��������������� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitPeriodSamara")]
    public class ReportBudgetSourceDeficitPeriodSamaraCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitPeriodSamaraCommand()
        {
            key = "ReportBudgetSourceDeficitPeriodSamara";
            caption = "�� �������� ������ ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitPeriodData(reportParams);
        }
    }

    [Description("ReportBorrowingProgrammSamara")]
    public class ReportBorrowingProgrammSamaraCommand : CommonReportsCommand
    {
        public ReportBorrowingProgrammSamaraCommand()
        {
            key = "ReportBorrowingProgrammSamara";
            caption = "��������� ��������������� ���������� ������������� ��������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingProgrammSamaraData(reportParams);
        }
    }

    [Description("ReportBudgetCreditIssuedSamara")]
    public class ReportBudgetCreditIssuedSamaraCommand : CommonReportsCommand
    {
        public ReportBudgetCreditIssuedSamaraCommand()
        {
            key = "ReportBudgetCreditIssuedSamara";
            caption = "��������� ������� ��������������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetCreditIssuedSamaraData(reportParams);
        }
    }

    [Description("ReportGarantProgrammWordOmsk")]
    public class ReportGarantProgrammWordOmskCommand : WordMacrosCommand
    {
        public ReportGarantProgrammWordOmskCommand()
        {
            key = "ReportGarantProgrammWordOmsk";
            caption = "��������� �������.��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammData(reportParams);
        }
    }

    [Description("ReportGarantProgrammPeriodWordOmsk")]
    public class ReportGarantProgrammPeriodWordOmskCommand : WordMacrosCommand
    {
        public ReportGarantProgrammPeriodWordOmskCommand()
        {
            key = "ReportGarantProgrammPeriodWordOmsk";
            caption = "��������� �������.�������� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammPeriodData(reportParams);
        }
    }

    [Description("ReportBorrowingWordOmsk")]
    public class ReportBorrowingWordOmskCommand : WordMacrosCommand
    {
        public ReportBorrowingWordOmskCommand()
        {
            key = "ReportBorrowingWordOmsk";
            caption = "�������.�������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingData(reportParams);
        }
    }

    [Description("ReportBorrowingPeriodWordOmsk")]
    public class ReportBorrowingPeriodWordOmskCommand : WordMacrosCommand
    {
        public ReportBorrowingPeriodWordOmskCommand()
        {
            key = "ReportBorrowingPeriodWordOmsk";
            caption = "�������.������������� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingPeriodData(reportParams);
        }
    }

    [Description("ReportVaultDKSamara")]
    public class ReportVaultDKSamaraCommand : CommonReportsCommand
    {
        public ReportVaultDKSamaraCommand()
        {
            key = "ReportVaultDKSamara";
            caption = "���� ��";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetVaultDKSamaraData(reportParams);
        }
    }

    [Description("ReportGarantProgrammYar")]
    public class ReportGarantProgrammWordYarCommand : WordMacrosCommand
    {
        public ReportGarantProgrammWordYarCommand()
        {
            key = "ReportGarantProgrammYar";
            caption = "��������� ��������������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammYarData(reportParams);
        }
    }

    [Description("ReportBorrowingProgrammYar")]
    public class ReportBorrowingProgrammWordYarCommand : WordMacrosCommand
    {
        public ReportBorrowingProgrammWordYarCommand()
        {
            key = "ReportBorrowingProgrammYar";
            caption = "��������� �������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingProgrammYarData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitYar")]
    public class ReportBudgetSourceDeficitYarCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitYarCommand()
        {
            key = "ReportBudgetSourceDeficitYar";
            caption = "���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, true);
        }
    }

    [Description("ReportBudgetSourceDeficitPeriodYar")]
    public class ReportBudgetSourceDeficitPeriodYarCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitPeriodYarCommand()
        {
            key = "ReportBudgetSourceDeficitPeriodYar";
            caption = "��������� �� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitPeriodData(reportParams);
        }
    }

    [Description("ReportBudgetSourceDeficitVologda")]
    public class ReportBudgetSourceDeficitVologdaCommand : CommonReportsCommand
    {
        public ReportBudgetSourceDeficitVologdaCommand()
        {
            key = "ReportBudgetSourceDeficitVologda";
            caption = "��������� �������������� �������� ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBudgetSourceDeficitData(reportParams, 1, true);
        }
    }

    [Description("ReportBorrowingVologda")]
    public class ReportBorrowingProgrammVologdaCommand : CommonReportsCommand
    {
        public ReportBorrowingProgrammVologdaCommand()
        {
            key = "ReportBorrowingVologda";
            caption = "��������� ��������������� ���������� ������������� ����������� ������� �� �������� ���";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetBorrowingVologdaData(reportParams);
        }
    }

    [Description("ReportGarantProgrammVologda")]
    public class ReportGarantProgrammVologdaCommand : CommonReportsCommand
    {
        public ReportGarantProgrammVologdaCommand()
        {
            key = "ReportGarantProgrammVologda";
            caption = "��������� ��������������� �������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantProgrammVologdaData(reportParams);
        }
    }

    [Description("ReportGarantCardVologda")]
    public class ReportGarantCardVologdaCommand : CommonReportsCommand
    {
        public ReportGarantCardVologdaCommand()
        {
            key = "ReportGarantCardVologda";
            caption = "�������� ����������� ��������� �������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantCardVologdaData(reportParams);
        }
    }

    [Description("ReportGarantChangesVologda")]
    public class ReportGarantChangesVologdaCommand : CommonReportsCommand
    {
        public ReportGarantChangesVologdaCommand()
        {
            key = "ReportGarantChangesVologda";
            caption = "��������� �������� ������������ �� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate);
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantChangesVologdaData(reportParams);
        }
    }

    [Description("ReportGarantCalcPercentVologda")]
    public class ReportCreditCalcPercentVologdaCommand : CommonReportsCommand
    {
        public ReportCreditCalcPercentVologdaCommand()
        {
            key = "ReportGarantCalcPercentVologda";
            caption = "������ ��������� �� ����������� �������� ����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditCalcPercentVologdaData(reportParams);
        }
    }

    [Description("ReportAnalisysFinSupportVologda")]
    public class ReportAnalisysFinSupportVologdaCommand : CommonReportsCommand
    {
        public ReportAnalisysFinSupportVologdaCommand()
        {
            key = "ReportAnalisysFinSupportVologda";
            caption = "������ ���������� ��������� �� ���������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetAnalisysFinSupportVologdaData(reportParams);
        }
    }

    [Description("MFRFCapital")]
    public class ReportMFRFDKCapitalCommand : CommonReportsCommand
    {
        public ReportMFRFDKCapitalCommand()
        {
            key = "MFRFCapital";
            caption = "���� �� ������ ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDKMFRFSubjectCapitalData(reportParams);
        }
    }

    [Description("MFRFCreditOrg")]
    public class ReportMFRFDKCreditOrgCommand : CommonReportsCommand
    {
        public ReportMFRFDKCreditOrgCommand()
        {
            key = "MFRFCreditOrg";
            caption = "���� �� ������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDKMFRFSubjectCreditOrgData(reportParams);
        }
    }

    [Description("MFRFGarant")]
    public class ReportMFRFDKGarantCommand : CommonReportsCommand
    {
        public ReportMFRFDKGarantCommand()
        {
            key = "MFRFGarant";
            caption = "���� �� ��������������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddDateParam(ReportConsts.ParamExchangeDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamExchangeDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDKMFRFSubjectGarantData(reportParams);
        }
    }

    [Description("MFRFCreditBud")]
    public class ReportMFRFDKCreditBudCommand : CommonReportsCommand
    {
        public ReportMFRFDKCreditBudCommand()
        {
            key = "MFRFCreditBud";
            caption = "���� �� ��������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDKMFRFSubjectCreditBudData(reportParams);
        }
    }

    [Description("ReportMOSummaryDebtInfo")]
    public class ReportMOSummaryDebtInfoCommand : CommonReportsCommand
    {
        public ReportMOSummaryDebtInfoCommand()
        {
            key = "ReportMOSummaryDebtInfo";
            caption = "������� ���������� � �������� ��������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate)
                .SetValue(String.Format("01.01.{0}", DateTime.Now.Year)).SetCaption("������ ����");
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("������ ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamStartExchangeRate).SetCaption("���� ��� ������ ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ��� ������ ����");

            paramBuilder.AddParamLink(ReportConsts.ParamStartExchangeRate, ReportConsts.ParamStartDate, new UndercutExchangeUniUsdRate());
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOSummaryDebtInfoData(reportParams);
        }
    }

    [Description("ReportMOExtractDebtBookFull")]
    public class ReportMOExtractDebtBookFullCommand : CommonReportsCommand
    {
        public ReportMOExtractDebtBookFullCommand()
        {
            key = "ReportMOExtractDebtBookFull";
            caption = "������� �� �������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamReportDate);
            paramBuilder.AddDateParam(ReportConsts.ParamExchangeDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor2, new ParamReadingsJob())
                .SetCaption("�������2");

            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamExchangeDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOExtractDebtBookData(reportParams);
        }
    }

    [Description("ReportMOExtractDebtBookShort")]
    public class ReportMOExtractDebtBookShortCommand : CommonReportsCommand
    {
        public ReportMOExtractDebtBookShortCommand()
        {
            key = "ReportMOExtractDebtBookShort";
            caption = "������� �� �������� ����� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamReportDate);
            paramBuilder.AddDateParam(ReportConsts.ParamExchangeDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor2, new ParamReadingsJob())
                .SetCaption("�������2");

            paramBuilder.AddParamLink(
                ReportConsts.ParamExchangeRate, 
                ReportConsts.ParamExchangeDate, 
                new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOExtractDebtBookShortData(reportParams);
        }
    }

    [Description("ReportReestrBudCreditSaratov")]
    public class ReportReestrBudCreditSaratovCommand : CommonReportsCommand
    {
        public ReportReestrBudCreditSaratovCommand()
        {
            key = "ReportReestrBudCreditSaratov";
            caption = "������ ��������������� ��������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetReestrBudCreditSaratovData(reportParams);
        }
    }

    [Description("ReportSubjectDebtorBookSaratov")]
    public class ReportSubjectDebtorBookSaratovCommand : CommonReportsCommand
    {
        public ReportSubjectDebtorBookSaratovCommand()
        {
            key = "ReportSubjectDebtorBookSaratov";
            caption = "��������������� �������� ����� ����������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetSubjectDebtorBookSaratovData(reportParams);
        }
    }

    [Description("ReportMOCreditInfo")]
    public class ReportMOCreditInfoCommand : CommonReportsCommand
    {
        public ReportMOCreditInfoCommand()
        {
            key = "ReportMOCreditInfo";
            caption = "������� � ��������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddEnumParam(ReportConsts.ParamVariantType, typeof(VariantTypeEnum));
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");

            // ����� ����������
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCreditInfoData(reportParams);
        }
    }

    [Description("ReportMOGarantInfo")]
    public class ReportMOGarantInfoCommand : CommonReportsCommand
    {
        public ReportMOGarantInfoCommand()
        {
            key = "ReportMOGarantInfo";
            caption = "��������������� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddEnumParam(ReportConsts.ParamVariantType, typeof(VariantTypeEnum));
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");

            // ����� ����������
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOGarantInfoData(reportParams);
        }
    }

    [Description("ReportMOCashPlan")]
    public class ReportMOCashPlanCommand : CommonReportsCommand
    {
        public ReportMOCashPlanCommand()
        {
            key = "ReportMOCashPlan";
            caption = "������� �������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddEnumParam(ReportConsts.ParamPeriodType, typeof(ReportPeriodEnum));
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamMonth, typeof(MonthEnum))
                .SetValue(MonthEnum.i1)
                .SetCaption("�����")
                .SetFilter(ReportConsts.ParamPeriodType, new List<object> { "�����" });
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("���� ����� �����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCashPlanData(reportParams);
        }
    }

    [Description("ReportMODebtServiceLoading")]
    public class ReportMODebtServiceLoadingCommand : CommonReportsCommand
    {
        public ReportMODebtServiceLoadingCommand()
        {
            key = "ReportMODebtServiceLoading";
            caption = "�������� �������� � ������������� ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("�������� ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMODebtServiceLoadingData(reportParams);
        }
    }

    [Description("ReportMOExtractDebtBookBuh")]
    public class ReportMOExtractDebtBookBuhCommand : CommonReportsCommand
    {
        public ReportMOExtractDebtBookBuhCommand()
        {
            key = "ReportMOExtractDebtBookBuh";
            caption = "������� �� �������� ����� ��� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamReportDate);
            paramBuilder.AddDateParam(ReportConsts.ParamExchangeDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1"); ;
            paramBuilder.AddParamLink(
                ReportConsts.ParamExchangeRate, 
                ReportConsts.ParamExchangeDate, 
                new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOExtractDebtBookData(reportParams);
        }
    }

    [Description("ReportMOContractInfo")]
    public class ReportMOContractInfoCommand : CommonReportsCommand
    {
        public ReportMOContractInfoCommand()
        {
            key = "ReportMOContractInfo";
            caption = "�������� � �������� (�������� �������������)";
            paramChecker = ReportDataServer.CheckParamsMOContractInfoData;
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistRegNum());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOContractInfoData(reportParams);
        }
    }

    [Description("ReportMOContractInfoForm58")]
    public class ReportMOContractInfoForm58Command : CommonReportsCommand
    {
        public ReportMOContractInfoForm58Command()
        {
            key = "ReportMOContractInfoForm58";
            caption = "�������� � �������� (����.58)";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistRegNum());
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOContractInfoForm58Data(reportParams);
        }
    }

    [Description("ReportMOGarantDebtHistory")]
    public class ReportMOGarantDebtHistoryCommand : CommonReportsCommand
    {
        public ReportMOGarantDebtHistoryCommand()
        {
            key = "ReportMOGarantDebtHistory";
            caption = "�������� ����� ����� �� ��������";
            paramChecker = ReportDataServer.CheckMOGarantDateData;
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistGarant());
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor2, new ParamReadingsJob())
                .SetCaption("�������2");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor3, new ParamReadingsJob())
                .SetCaption("�������3");
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGarantDebtHistoryMOData(reportParams);
        }
    }

    [Description("ReportMOCreditDebtHistory")]
    public class ReportMOCreditDebtHistoryCommand : CommonReportsCommand
    {
        public ReportMOCreditDebtHistoryCommand()
        {
            key = "ReportMOCreditDebtHistory";
            caption = "�������� ����� ����� �� �������";
            paramChecker = ReportDataServer.CheckMOCreditDateData;
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistCredit());
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor2, new ParamReadingsJob())
                .SetCaption("�������2");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor3, new ParamReadingsJob())
                .SetCaption("�������3");
            var settings1 = paramBuilder.AddBookParam(ReportConsts.ParamCreditPlanFilter1, new ParamPlanService())
                .SetCaption("���� ������������ 1");
            var settings2 = paramBuilder.AddBookParam(ReportConsts.ParamCreditPlanFilter2, new ParamPlanService())
                .SetCaption("���� ������������ 2");

            var undercuter1 = new UndercutPlanServiceStart { settings = settings1 };
            var undercuter2 = new UndercutPlanServiceEnd { settings = settings2 };

            paramBuilder.AddParamLink(ReportConsts.ParamCreditPlanFilter1, ReportConsts.ParamRegNum, undercuter1);
            paramBuilder.AddParamLink(ReportConsts.ParamCreditPlanFilter2, ReportConsts.ParamRegNum, undercuter2);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditDebtHistoryMOData(reportParams);
        }
    }

    [Description("ReportMOCapitalDebtHistory")]
    public class ReportMOCapitalDebtHistoryCommand : CommonReportsCommand
    {
        public ReportMOCapitalDebtHistoryCommand()
        {
            key = "ReportMOCapitalDebtHistory";
            caption = "�������� ����� ����� �� �����";
            paramChecker = ReportDataServer.CheckMOCapitalDateData;
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistCapital());
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob())
                .SetCaption("�������1");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor2, new ParamReadingsJob())
                .SetCaption("�������2");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor3, new ParamReadingsJob())
                .SetCaption("�������3");
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCapitalDebtHistoryMOData(reportParams);
        }
    }

    [Description("ReportGraphDebtYaroslavl")]
    public class ReportGraphDebtYaroslavlCommand : CommonReportsCommand
    {
        public ReportGraphDebtYaroslavlCommand()
        {
            key = "ReportGraphDebtYaroslavl";
            caption = "������ ��������� �������� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate);
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddNumParam(ReportConsts.ParamDigitNumber).SetValue(2).SetMask("n");
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniEuroRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGraphDebtYaroslavlData(reportParams);
        }
    }

    [Description("ReportDebtorBookYarObl")]
    public class ReportDebtorBookYarOblCommand : CommonReportsCommand
    {
        public ReportDebtorBookYarOblCommand()
        {
            key = "ReportDebtorBookYarObl";
            caption = "�������� ����� ����������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookYarOblData(reportParams);
        }
    }

    [Description("ReportDebtorBookSamaraObl")]
    public class ReportDebtorBookSamaraOblCommand : CommonReportsCommand
    {
        public ReportDebtorBookSamaraOblCommand()
        {
            key = "ReportDebtorBookSamaraObl";
            caption = "�������� ����� ��������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookSamaraOblData(reportParams);
        }
    }

    [Description("ReportMOPlanServices")]
    public class ReportMOPlanServicesCommand : CommonReportsCommand
    {
        public ReportMOPlanServicesCommand()
        {
            key = "ReportMOPlanServices";
            caption = "����� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamRegNum, new ParamExistCredit());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOMOPlanServicesData(reportParams);
        }
    }

    [Description("ReportStateDebtApplication1ShortYar")]
    public class ReportStateDebt1ShortYarCommand : CommonReportsCommand
    {
        public ReportStateDebt1ShortYarCommand()
        {
            key = "ReportStateDebtApplication1ShortYar";
            caption = "������ �������� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("����");
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob()).SetCaption("�������1");
            paramBuilder.AddParamLink(
                ReportConsts.ParamExchangeRate, 
                ReportConsts.ParamEndDate, 
                new UndercutExchangeUniEuroRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetStateDebtApplication1YarData(reportParams);
        }
    }

    [Description("ReportDebtorBookSummaryYarObl")]
    public class ReportDebtorBookSummaryYarOblCommand : CommonReportsCommand
    {
        public ReportDebtorBookSummaryYarOblCommand()
        {
            key = "ReportDebtorBookSummaryYarObl";
            caption = "�������� ����� ����������� �������(�����)";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookYarOblData(reportParams);
        }
    }

    [Description("ReportDebtorBookSelectYarObl")]
    public class ReportDebtorBookSelectYarOblCommand : CommonReportsCommand
    {
        public ReportDebtorBookSelectYarOblCommand()
        {
            key = "ReportDebtorBookSelectYarObl";
            caption = "�������� ����� ����������� �������(��������� �������)";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamExecutor1, new ParamReadingsJob());
            paramBuilder.AddBookParam(ReportConsts.ParamStartDate, new ParamExistContractDate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDebtorBookYarOblData(reportParams);
        }
    }

    [Description("CreditInformationOmskCity")]
    public class CreditInformationOmskCityCommand : CommonReportsCommand
    {
        public CreditInformationOmskCityCommand()
        {
            key = "CreditInformationOmskCity";
            caption = "���������� �� ��������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddEnumParam(ReportConsts.ParamVariantType, typeof(ContractTypeEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetCreditOmskReportData(reportParams);
        }
    }

    [Description("ReportChargesDebtInformationOmskCity")]
    public class ReportChargesDebtInformationOmskCityCommand : CommonReportsCommand
    {
        public ReportChargesDebtInformationOmskCityCommand()
        {
            key = "ReportChargesDebtInformationOmskCity";
            caption = "������� �� ������������ �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetChargesDebtDataOmsk(reportParams);
        }
    }

    [Description("ReportMOCalcService")]
    public class ReportMOCalcServiceCommand : CommonReportsCommand
    {
        public ReportMOCalcServiceCommand()
        {
            key = "ReportMOCalcService";
            caption = "������ ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrowPlan());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCalcServiceReportData(reportParams);
        }
    }

    [Description("ReportMOForecastDebtVolumeChange")]
    public class ReportMOForecastDebtVolumeChangeCommand : CommonReportsCommand
    {
        public ReportMOForecastDebtVolumeChangeCommand()
        {
            key = "ReportMOForecastDebtVolumeChange";
            caption = "������� ��������� ������ �������� ������������ ���������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("����");
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamVariantBorrow());
            paramBuilder.AddStringParam(ReportConsts.ParamSum).SetCaption("�� ���������� ������������");
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOForecastDebtVolumeChangeData(reportParams);
        }
    }

    [Description("ReportMOCapitalBasement")]
    public class ReportMOCapitalBasementCommand : CommonReportsCommand
    {
        public ReportMOCapitalBasementCommand()
        {
            key = "ReportMOCapitalBasement";
            caption = "���������� �����";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCapitalBasementData(reportParams);
        }
    }

    [Description("ReportMOExchangeCapitalCredit")]
    public class ReportMOExchangeCapitalCreditCommand : CommonReportsCommand
    {
        public ReportMOExchangeCapitalCreditCommand()
        {
            key = "ReportMOExchangeCapitalCredit";
            caption = "������ ����� �� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCapitalReplaceIssFoLn());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOExchangeCapitalCreditData(reportParams);
        }
    }

    [Description("ReportMOCapitalRedemption")]
    public class ReportMOCapitalRedemptionCommand : CommonReportsCommand
    {
        public ReportMOCapitalRedemptionCommand()
        {
            key = "ReportMOCapitalRedemption";
            caption = "����� ���������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCapitalBondBuyback());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCapitalRedemptionData(reportParams);
        }
    }

    [Description("ReportMOExchangeRate")]
    public class ReportMOExchangeRateCommand : CommonReportsCommand
    {
        public ReportMOExchangeRateCommand()
        {
            key = "ReportMOExchangeRate";
            caption = "�������� �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamStartDate).SetValue(DateTime.Now.AddMonths(-1));
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
            paramBuilder.AddExchangeParam(ReportConsts.ParamStartExchangeRate).SetCaption("���� ��� ������ ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ��� ������ ����");

            paramBuilder.AddParamLink(
                ReportConsts.ParamStartExchangeRate, 
                ReportConsts.ParamStartDate,
                new UndercutExchangeUniUsdPrevRate());

            paramBuilder.AddParamLink(
                ReportConsts.ParamExchangeRate, 
                ReportConsts.ParamEndDate,
                new UndercutExchangeUniUsdPrevRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOExchangeRateData(reportParams);
        }
    }

    [Description("ReportMOCompareCapitalBasement")]
    public class ReportMOCompareCapitalBasementCommand : CommonReportsCommand
    {
        public ReportMOCompareCapitalBasementCommand()
        {
            key = "ReportMOCompareCapitalBasement";
            caption = "���������� �����";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCalcParam());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCompareCapitalBasementData(reportParams);
        }
    }


    [Description("ReportMOCompareCapitalBasementDetail")]
    public class ReportMOCapitalBasementDetailCommand : CommonReportsCommand
    {
        public ReportMOCapitalBasementDetailCommand()
        {
            key = "ReportMOCompareCapitalBasement";
            caption = "���������� �����";
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMOCompareCapitalBasementData(reportParams);
        }
    }

    [Description("ReportMODrawingGovFactory")]
    public class ReportMODrawingGovFactoryCommand : CommonReportsCommand
    {
        public ReportMODrawingGovFactoryCommand()
        {
            key = "ReportMODrawingGovFactory";
            caption = "������������� ��������������� ��������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDrawingGovFactoryData(reportParams);
        }
    }

    [Description("ReportMODrawingGUPBuh")]
    public class ReportMODrawingGUPBuhCommand : CommonReportsCommand
    {
        public ReportMODrawingGUPBuhCommand()
        {
            key = "ReportMODrawingGUPBuh";
            caption = "������������� ��������������� ��������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDrawingGUPBuhData(reportParams);
        }
    }

    [Description("ReportMODrawingMunFactory")]
    public class ReportMODrawingMunFactoryCommand : CommonReportsCommand
    {
        public ReportMODrawingMunFactoryCommand()
        {
            key = "ReportMODrawingMunFactory";
            caption = "������������� ������������� ��������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDrawingMunFactoryData(reportParams);
        }
    }

    [Description("ReportMODrawingMUPBuh")]
    public class ReportMODrawingMUPBuhCommand : CommonReportsCommand
    {
        public ReportMODrawingMUPBuhCommand()
        {
            key = "ReportMODrawingMUPBuh";
            caption = "������������� ��������������� ��������� �����������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
            paramBuilder.AddEnumParam(ReportConsts.ParamQuarter, typeof(QuarterEnum));
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetDrawingMUPBuhData(reportParams);
        }
    }

    [Description("ReportMODebtServiceLoadingWithCapBasement")]
    public class ReportMODebtServiceLoadingWithCapBasementCommand : CommonReportsCommand
    {
        public ReportMODebtServiceLoadingWithCapBasementCommand()
        {
            key = "ReportMODebtServiceLoadingWithCapBasement";
            caption = "�������� �������� � ����������� ������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("�������� ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCalcParam());
            paramBuilder.AddEnumParam(ReportConsts.ParamOutputMode, typeof(DebtLoadingListTypeEnum));
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMODebtServiceLoadingWithCapBasementData(reportParams);
        }
    }

    [Description("ReportMODebtServiceLoadingWithCapRedemption")]
    public class ReportMODebtServiceLoadingWithCapRedemptionCommand : CommonReportsCommand
    {
        public ReportMODebtServiceLoadingWithCapRedemptionCommand()
        {
            key = "ReportMODebtServiceLoadingWithCapRedemption";
            caption = "�������� �������� � �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("�������� ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCapitalBondBuybackDetail());
            paramBuilder.AddEnumParam(ReportConsts.ParamOutputMode, typeof(DebtLoadingListTypeEnum));
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMODebtServiceLoadingWithRedemptionData(reportParams);
        }
    }

    [Description("ReportMODebtServiceLoadingWithCapReplace")]
    public class ReportMODebtServiceLoadingWithCapReplaceCommand : CommonReportsCommand
    {
        public ReportMODebtServiceLoadingWithCapReplaceCommand()
        {
            key = "ReportMODebtServiceLoadingWithExchangeCapCredit";
            caption = "�������� �������� � �������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate).SetCaption("�������� ����");
            paramBuilder.AddExchangeParam(ReportConsts.ParamExchangeRate).SetCaption("���� ������");
            paramBuilder.AddBookParam(ReportConsts.ParamVariantID, new ParamCapitalReplaceIssFoLnDetail());
            paramBuilder.AddEnumParam(ReportConsts.ParamOutputMode, typeof(DebtLoadingListTypeEnum));
            paramBuilder.AddParamLink(ReportConsts.ParamExchangeRate, ReportConsts.ParamEndDate, new UndercutExchangeUniUsdRate());
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetMODebtServiceLoadingWithReplaceData(reportParams);
        }
    }

    [Description("ReportGraphDebtKalmykia")]
    public class ReportGraphDebtKalmykiaCommand : CommonReportsCommand
    {
        public ReportGraphDebtKalmykiaCommand()
        {
            key = "ReportGraphDebtKalmykia";
            caption = "������ ��������� �������� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddYearParam(ReportConsts.ParamYear);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetGraphDebtKalmykiaData(reportParams);
        }
    }

    [Description("ReportVaultGraphDebtKalmykia")]
    public class ReportVaultGraphDebtKalmykiaCommand : CommonReportsCommand
    {
        public ReportVaultGraphDebtKalmykiaCommand()
        {
            key = "ReportVaultGraphDebtKalmykia";
            caption = "������� ������ ��������� �������� ������������";
        }

        public override void CreateReportParams()
        {
            paramBuilder.AddDateParam(ReportConsts.ParamEndDate);
        }

        public override DataTable[] GetReportData(Dictionary<string, string> reportParams)
        {
            return reportServer.GetVaultGraphDebtKalmykiaData(reportParams);
        }
    }
}