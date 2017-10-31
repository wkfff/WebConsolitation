namespace Krista.FM.Common.OfficePluginServices.FMOfficeAddin
{
    /// <summary>
    /// ��������� ��� �������������� � ��������
    /// </summary>
    public struct FMOfficeAddinConsts
    {
        public const string ProcessForm_ClassID = "5B07B2DB-ADEB-497B-AD64-73B38756DF45";
        public const string ProcessForm_ProgID = "PlaningTools.ProcessForm";

        // ProgID ������ ������� (��� ��������� �������)
        public const string excelAddinProgID = "FMExcelAddIn.DTExtensibility2";
        public const string excelAddinRegPath = @"Software\Microsoft\Office\Excel\AddIns\FMExcelAddIn.DTExtensibility2";
        public const string excelAddinRegPath64 = @"Software\Wow6432Node\Microsoft\Office\Excel\AddIns\FMExcelAddIn.DTExtensibility2";

        public const string wordAddinProgID = "FMWordAddIn.FMWordAddIn";
        public const string wordAddinRegPath = @"Software\Microsoft\Office\Word\AddIns\FMWordAddIn.FMWordAddIn";

        public const string SOAPDimChooser_ClassID = "E0C8DFBC-B49B-4167-A000-2BE924710FD2";
        public const string SOAPDimChooser_ProgID = "FMExcelAddIn.SOAPDimChooser";
        public const string SOAPDimEditor_ClassID = "775772AE-84D8-4EE8-8907-E1A9B6E47303";
        public const string SOAPDimEditor_ProgID = "FMExcelAddIn.SOAPDimEditor";

        // ��� ���������
        public const string pspSheetType = "fm.DocType";
        // �������� ������
        public const string pspTaskName = "fm.TaskName";
        // ID ������
        public const string pspTaskId = "fm.TaskId";
        // �������� ���������
        public const string pspDocumentName = "fm.DocumentName";
        // ID ���������
        public const string pspDocumentId = "fm.DocumentId";
        // �������� ��������� (�����������)
        public const string pspOwner = "fm.Owner";
        // ���� ��������
        public const string pspCreationDate = "fm.CreationDate";
        // ������������ ���� � ��������� ������
        public const string pspDocPath = "fm.DocPath";
        // ������ ��������� ���������� � ��������
        public const string pspTaskContextData = "fm.tc.Data";
        // ������ ������ ��������� ���������� � ��������
        public const string pspTaskContextSize = "fm.tc.Data.Size";
    }
}
