using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

using Krista.FM.Common;
using Krista.FM.Common.RegistryUtils;
using Krista.FM.Client.Common;

namespace Krista.FM.Client.Common
{
    [Guid("8C18DBC0-E552-4B2D-9E41-AD525D6BB46A")]
    public enum Const
    {
        otQuery,
        otProcess,
        otSave,
        otUpdate,
        otView,
        otMap
    }
    [Guid("DAEA92E5-BCFB-4E0A-ADA3-96E4937200A7")]
    public enum EventType
    {
        etEdit,
        etRefresh,
        etWriteback,
        etPropertyEdit,
        etUnknown,
        etVersionUpdate,
        etSheetCopy,
        etParamsEdit,
        etOnTaskConnection,
        etTaskConnectionOff,
        etConstEdit
    }

    [ComImport, TypeLibType(0x1040), Guid("A76B33A9-C507-4ADD-8707-6E2CF2ECD710")]
    public interface IOperation
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        void StartOperation([In] int ParentWnd);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        void StopOperation();
        [DispId(4)]
        string Caption { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)] set; }
    }

   /*
    [ComImport, TypeLibType((short)0x1040), Guid("57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9")]
    public interface IProcessForm
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        void OpenOperation([In, MarshalAs(UnmanagedType.BStr)] string Name, [In] bool IsCritical, [In] bool IsNoteTime, [In] int OperationType);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        void CloseOperation();
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        void PostInfo([In, MarshalAs(UnmanagedType.BStr)] string AText);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void PostWarning([In, MarshalAs(UnmanagedType.BStr)] string AText);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void PostError([In, MarshalAs(UnmanagedType.BStr)] string AText);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(6)]
        void SetPBarPosition([In] int CurrentPosition, [In] int MaxPosition);
        [DispId(8)]
        int ParentWnd { [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)] set; }
        [DispId(7)]
        string ProcessTitle { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(7)] set; }
        [DispId(9)]
        string SuccessMessage { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)] set; }
        [DispId(10)]
        string ErrorMessage { [param: In, MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(10)] set; }
        [DispId(11)]
        bool Showing { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)] get; }
        [DispId(12)]
        string LastError { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)] get; }
        [DispId(13)]
        string ErrorList { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)] get; }
        [DispId(14)]
        bool NewProcessClear { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)] set; }
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)]
        void ClearErrors();
    }
    */

    #region Лист 2.2.2
    
    [ComImport, TypeLibType(0x1040), Guid("57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9")]   
    public interface IProcessForm
    {
        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OpenOperation([In, MarshalAs(UnmanagedType.BStr)] string Name, [In] bool IsCritical, 
            [In] bool IsNoteTime, [In] int OperationType);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CloseOperation();

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void PostInfo([In, MarshalAs(UnmanagedType.BStr)] string AText);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void PostWarning([In, MarshalAs(UnmanagedType.BStr)] string AText);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void PostError([In, MarshalAs(UnmanagedType.BStr)] string AText);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPBarPosition([In] int CurrentPosition, [In] int MaxPosition);

        [DispId(0xC)]
        string LastError { [return: MarshalAs(UnmanagedType.BStr)] 
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
            DispId(0xC)] get; }

        [DispId(0xD)]
        string ErrorList
        {
            [return: MarshalAs(UnmanagedType.BStr)]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
            DispId(0xD)]
            get;
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
            DispId(0xD)]
            set;
        }

        [DispId(0xE)]
        bool NewProcessClear { [MethodImpl(MethodImplOptions.InternalCall, 
            MethodCodeType = MethodCodeType.Runtime), DispId(14)] get; [param: In] 
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), 
            DispId(0xE)] set; }

        [DispId(0xF)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearErrors();

        [DispId(0x12)]
        bool Showing { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), 
            DispId(0x12)] get; }

        [DispId(0x14)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OpenProcess([In] int ParentWnd, [In, MarshalAs(UnmanagedType.BStr)] string ProcessTitle, 
            [In, MarshalAs(UnmanagedType.BStr)] string SuccessMessage, 
            [In, MarshalAs(UnmanagedType.BStr)] string ErrorMessage);

        [DispId(0x15)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CloseProcess();

        [DispId(0x7)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnableLogging([In, MarshalAs(UnmanagedType.BStr)] string FileName);

        [DispId(0x8)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void DisableLogging();

        [DispId(0x9)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void DirectWriteLogString([In, MarshalAs(UnmanagedType.BStr)] string Msg);
    }
    #endregion

    [ComImport, Guid("8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D"), TypeLibType((short)0x1040)]
    public interface IFMPlanningExtension
    {
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        string GetPropValueByName([In, MarshalAs(UnmanagedType.BStr)] string PropName);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)]
        void SetPropValueByName([In, MarshalAs(UnmanagedType.BStr)] string PropName, 
            [In, MarshalAs(UnmanagedType.BStr)] string PropValue);

        [return: MarshalAs(UnmanagedType.Error)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        int SetConnectionStr([In, MarshalAs(UnmanagedType.BStr)] string URL, 
            [In, MarshalAs(UnmanagedType.BStr)] string Scheme);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(4)]
        void SetTaskContext([In, MarshalAs(UnmanagedType.IDispatch)] object taskContext);

        [DispId(10)]
        bool IsSilentMode { [MethodImpl(MethodImplOptions.InternalCall, 
            MethodCodeType = MethodCodeType.Runtime), DispId(10)] get; 
            [param: In] [MethodImpl(MethodImplOptions.InternalCall, 
            MethodCodeType = MethodCodeType.Runtime), DispId(10)] set; }

        [DispId(6)]
        IProcessForm ProcessForm { [return: MarshalAs(UnmanagedType.Interface)] 
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), 
            DispId(6)] get; [param: In, MarshalAs(UnmanagedType.Interface)] 
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), 
            DispId(6)] set; }

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(5)]
        void OnTaskConnection([In] bool IsConnected);

        [DispId(7)]
        bool IsLoadingFromTask { [MethodImpl(MethodImplOptions.InternalCall, 
            MethodCodeType = MethodCodeType.Runtime), DispId(7)] get; 
            [param: In] [MethodImpl(MethodImplOptions.InternalCall, 
            MethodCodeType = MethodCodeType.Runtime), DispId(7)] set; }

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(8)]
        bool RefreshSheet([In, MarshalAs(UnmanagedType.Struct)] object Index, out bool IsAccessVioletion);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(9)]
        bool RefreshActiveSheet();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(11)]
        bool WritebackActiveSheet();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(12)]
        bool RefreshActiveBook();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(13)]
        bool WritebackActiveBook();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(14)]
        void SetAuthenticationInfo([In, MarshalAs(UnmanagedType.I4)] int AuthType, 
            [In, MarshalAs(UnmanagedType.BStr)] string Login, 
            [In, MarshalAs(UnmanagedType.BStr)] string PwdHash);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(15)]
        bool WritebackActiveSheetEx([In] bool EraseEmptyCells, [In] bool ProcessCube);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(16)]
        bool WritebackActiveBookEx([In] bool EraseEmptyCells, [In] bool ProcessCube);

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(17)]
        void AddSheetHistoryEvent([In, MarshalAs(UnmanagedType.IDispatch)] object Doc,
                                  [In] EventType EType, [In] string Comment, [In] bool Success);
    }

    [ComImport, Guid("C73630B5-97EF-4227-9C08-B8C646FA23AE"), TypeLibType((short)0x1040)]
    public interface ISOAPDimChooser
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        bool SelectDimension([In] int parentWnd, [In, MarshalAs(UnmanagedType.BStr)] string URL, [In, MarshalAs(UnmanagedType.BStr)] string SchemeName, [In, Out, MarshalAs(UnmanagedType.BStr)] ref string DimensionName);
        [DispId(2)]
        bool RefreshOnShow { [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; [param: In] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] set; }
        [DispId(3)]
        string LastError { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)] get; }
        [DispId(4)]
        void SetAuthenticationInfo([In, MarshalAs(UnmanagedType.I4)] int AuthType, [In, MarshalAs(UnmanagedType.BStr)] string Login, [In, MarshalAs(UnmanagedType.BStr)] string PwdHash);
    }

    [ComImport, Guid("0D3B3075-8467-4255-81A5-CA7FB3B9A600"), TypeLibType((short)0x1040)]
    public interface ISOAPDimEditor
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(1)]
        bool EditMemberTree([In] int parentWnd, [In, MarshalAs(UnmanagedType.BStr)] string URL, [In, MarshalAs(UnmanagedType.BStr)] string SchemeName, [In, MarshalAs(UnmanagedType.BStr)] string DimensionName, [In, Out, MarshalAs(UnmanagedType.BStr)] ref string Value);
        [DispId(2)]
        string LastError { [return: MarshalAs(UnmanagedType.BStr)] [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(2)] get; }
        [return: MarshalAs(UnmanagedType.BStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), DispId(3)]
        string GetTextMemberList([In, MarshalAs(UnmanagedType.BStr)] string XMLValue);
        [DispId(4)]
        void SetAuthenticationInfo([In, MarshalAs(UnmanagedType.I4)] int AuthType, [In, MarshalAs(UnmanagedType.BStr)] string Login, [In, MarshalAs(UnmanagedType.BStr)] string PwdHash);
    }


    /// <summary>
    /// Константы для взаимодействия с плагином
    /// </summary>
    public struct FMOfficeAddinsConsts
    {
        public const string ProcessForm_ClassID = "5B07B2DB-ADEB-497B-AD64-73B38756DF45";
        public const string ProcessForm_ProgID = "PlaningTools.ProcessForm";

        // ProgID нашего плагина (для получения свойств)
        public const string excelAddinProgID = "FMExcelAddIn.DTExtensibility2";
        public const string excelAddinRegPath = @"Software\Microsoft\Office\Excel\AddIns\FMExcelAddIn.DTExtensibility2";

        public const string wordAddinProgID = "FMWordAddIn.FMWordAddIn";
        public const string wordAddinRegPath = @"Software\Microsoft\Office\Word\AddIns\FMWordAddIn.FMWordAddIn";

        public const string SOAPDimChooser_ClassID = "E0C8DFBC-B49B-4167-A000-2BE924710FD2";
        public const string SOAPDimChooser_ProgID = "FMExcelAddIn.SOAPDimChooser";
        public const string SOAPDimEditor_ClassID = "775772AE-84D8-4EE8-8907-E1A9B6E47303";
        public const string SOAPDimEditor_ProgID = "FMExcelAddIn.SOAPDimEditor";

        // Тип документа
        public const string pspSheetType = "PlanningSheetType";
        // Название задачи
        public const string pspTaskName = "PlanningSheetTaskName";
        // ID задачи
        public const string pspTaskId = "PlanningSheetTaskId";
        // Название документа
        public const string pspDocumentName = "PlanningSheetDocumentName";
        // ID документа
        public const string pspDocumentId = "PlanningSheetDocumentId";
        // Владелец документа (исполнитель)
        public const string pspOwner = "PlanningSheetOwner";
        // Дата создания
        public const string pspCreationDate = "PlanningSheetCreationDate";
        // Оригинальный путь к документу задачи
        public const string pspDocPath = "DocPath";

    }

    public class SOAPDimEditorHelper: DisposableObject
    {
        private ISOAPDimEditor soapDimEditorItf = null;

        public SOAPDimEditorHelper()
        {
            if (Utils.CheckLibByProgID(FMOfficeAddinsConsts.SOAPDimEditor_ProgID, true))
            {
                Type objectType = Type.GetTypeFromProgID(FMOfficeAddinsConsts.SOAPDimEditor_ProgID);
                if (objectType == null)
                    throw new Exception("Невозможно создать объект " + FMOfficeAddinsConsts.SOAPDimEditor_ProgID);
                soapDimEditorItf = Activator.CreateInstance(objectType) as ISOAPDimEditor;
            }
        }

        #region Обертки над методами ISOAPDimEditor
        public bool EditMemberTree(int parentWnd, string URL, string SchemeName, string DimensionName, ref string Value)
        {
            if (soapDimEditorItf == null)
                return false;
            return soapDimEditorItf.EditMemberTree(parentWnd, URL, SchemeName, DimensionName, ref Value);
        }

        public string LastError 
        { 
            get 
            {
                if (soapDimEditorItf != null)
                    return soapDimEditorItf.LastError;
                else
                    return String.Empty;
            }
        }


        public string GetTextMemberList(string XMLValue)
        {
            if (soapDimEditorItf == null)
                return String.Empty;
            return soapDimEditorItf.GetTextMemberList(XMLValue);
        }

        public void SetAuthenticationInfo(int AuthType, string Login, string PwdHash)
        {
            if (soapDimEditorItf != null)
                soapDimEditorItf.SetAuthenticationInfo(AuthType, Login, PwdHash);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if ((disposing) && (soapDimEditorItf != null))
            {
                try
                {
                    Marshal.ReleaseComObject(soapDimEditorItf);
                    soapDimEditorItf = null;
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }
    }

    public class SOAPDimChooserHelper : DisposableObject
    {
        private ISOAPDimChooser soapDimChooserItf = null;

        public SOAPDimChooserHelper()
        {
            if (Utils.CheckLibByProgID(FMOfficeAddinsConsts.SOAPDimChooser_ProgID, true))
            {
                Type objectType = Type.GetTypeFromProgID(FMOfficeAddinsConsts.SOAPDimChooser_ProgID);
                if (objectType == null)
                    throw new Exception("Невозможно создать объект " + FMOfficeAddinsConsts.SOAPDimChooser_ProgID);
                soapDimChooserItf = Activator.CreateInstance(objectType) as ISOAPDimChooser;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if ((disposing) && (soapDimChooserItf != null))
            {
                try
                {
                    Marshal.ReleaseComObject(soapDimChooserItf);
                    soapDimChooserItf = null;
                }
                catch
                {
                }
            }
            base.Dispose(disposing);
        }

        #region Обертки над методами ISOAPDimChooser
        public bool SelectDimension(int parentWnd, string URL, string SchemeName, ref string DimensionName)
        {
            if (soapDimChooserItf == null)
                return false;
            return soapDimChooserItf.SelectDimension(parentWnd, URL, SchemeName, ref DimensionName);
        }

        public bool RefreshOnShow 
        { 
            set 
            {
                if (soapDimChooserItf == null)
                    return;
                soapDimChooserItf.RefreshOnShow = value;
            } 
        }

        public string LastError 
        {
            get
            {
                if (soapDimChooserItf == null)
                    return String.Empty;
                return soapDimChooserItf.LastError;
            }
        }

        public void SetAuthenticationInfo(int AuthType, string Login, string PwdHash)
        {
            if (soapDimChooserItf != null)
                try
                {
                    soapDimChooserItf.SetAuthenticationInfo(AuthType, Login, PwdHash);
                }
                catch (Exception e) { throw (e); }
        }
        #endregion

    }



}