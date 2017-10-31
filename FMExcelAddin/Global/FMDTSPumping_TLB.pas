unit FMDTSPumping_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : $Revision:   1.88.1.0.1.0  $
// File generated on 25.05.2005 13:07:23 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\SYSTEM\AdminTools\FMDTSPumping\FMDTSPumping.tlb (1)
// IID\LCID: {F6CFA0C7-0AFF-4738-A226-5B075FB18B6E}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINNT\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  FMDTSPumpingMajorVersion = 1;
  FMDTSPumpingMinorVersion = 0;

  LIBID_FMDTSPumping: TGUID = '{F6CFA0C7-0AFF-4738-A226-5B075FB18B6E}';

  IID_ICustomPump: TGUID = '{34A2A0B2-764B-4A2B-BD44-4205C0EA707C}';
  CLASS_FintechMonthXMLPump: TGUID = '{8D347DA2-8757-4DD6-88E7-A26079730B00}';
  CLASS_FintechYearXMLPump: TGUID = '{2CD0E4A3-4B45-4FA0-BCC4-ACEE700DE804}';
  CLASS_FintechMonthDBFPump2: TGUID = '{1B39CB26-4BE8-4DA7-8BF8-8DF7A3404EE8}';
  CLASS_UFKExcelPump: TGUID = '{C3975B4F-AE17-4E04-8CD9-9123B6F71B37}';
  CLASS_FMDTSPump: TGUID = '{E9A96EC5-0C7C-4886-B6ED-F112BEB24E7E}';
  IID_IDBFHelper: TGUID = '{9E1F3247-5313-46DA-A21E-5C918CBF8941}';
  CLASS_DBFHelper: TGUID = '{5C3EEA8B-E924-469F-A37C-4FC812ED53CB}';
  IID_IGMNHelper: TGUID = '{C78C3CC6-0E10-40EF-A97F-A2B3C854DC91}';
  CLASS_GMNHelper: TGUID = '{40917463-B0ED-40B7-908C-6F3B585F445B}';
  IID_IPumpLogger: TGUID = '{F732CF3C-F523-4CBB-B1AB-A9B053EAD215}';
  CLASS_PumpLogger: TGUID = '{DA9A825F-AED1-432C-AC89-C57D19104F6C}';
  IID_IDTSPropsChanger: TGUID = '{C874921F-D31E-46A8-BCF8-054F796BEC65}';
  CLASS_DTSPropsChanger: TGUID = '{A7C5CCA7-56B7-440E-939E-42459C76F5A6}';
  IID_IDTSPumpProgress: TGUID = '{C41EEBD6-FEE6-41F3-A69E-EC1B54AED086}';
  CLASS_DTSPumpProgress: TGUID = '{97865E2E-7CDF-42C1-B936-8768EE2767C9}';
  IID_IPumpParams: TGUID = '{00F86EB3-63DF-4819-9972-B29E05549F87}';
  IID_IPumpParamsGetter: TGUID = '{6764723F-8BBA-4CCA-BE35-745FA348F2DF}';
  IID_IFileManager: TGUID = '{EAB91607-6E93-4826-9A56-BCAE86A492C3}';
  CLASS_FileManager: TGUID = '{AF17D393-BB83-45A6-A8F2-24E4C8EFF340}';
  IID_IDBHelper: TGUID = '{0D0AD24D-6F80-4A5C-B923-35E61A5ABCA6}';
  CLASS_DBHelper: TGUID = '{E849818A-FED9-422C-B577-2F2CD60EDCA1}';
  IID_IExcelHelper: TGUID = '{F963370E-05F1-4482-A09D-F2E7F6288B96}';
  CLASS_ExcelHelper: TGUID = '{9C6C4F87-D076-459B-90F0-E0986D9F424F}';
  CLASS_PumpParams: TGUID = '{1446AB8E-54E7-4C16-897B-8A055C3CD30C}';
  IID_ITXTSorcerer: TGUID = '{5B592E89-9787-44E8-9302-4451474617A7}';
  CLASS_TXTSorcerer: TGUID = '{CFAB7DB1-FB87-4B69-B89C-FED1D8B2EA78}';
  CLASS_TXTReportsPumping: TGUID = '{8EF1D8AB-125B-4BDD-ACE8-81334858D181}';
  CLASS_UFKReportsDBF: TGUID = '{2298798B-2504-413D-A630-D9507ACA2EE9}';
  CLASS_UFKReportsXLS: TGUID = '{74DD9E48-9213-434A-9531-C79532704633}';
  CLASS_UMNSReports_28N: TGUID = '{7837A6FE-D317-46D9-BB4F-CC2B26CFC634}';
  CLASS_UMNSReports_28N_Disintegrator: TGUID = '{127A5674-543B-4E66-A13D-20F35A1441D9}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum TPumpEventsTypes
type
  TPumpEventsTypes = TOleEnum;
const
  ekStartPumping = $00000000;
  ekSuccEndPumping = $00000001;
  ekErrEndPumping = $00000002;
  ekPumpMessage = $00000003;
  ekPumpError = $00000004;
  ekPumpAlert = $00000005;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  ICustomPump = interface;
  ICustomPumpDisp = dispinterface;
  IDBFHelper = interface;
  IDBFHelperDisp = dispinterface;
  IGMNHelper = interface;
  IGMNHelperDisp = dispinterface;
  IPumpLogger = interface;
  IPumpLoggerDisp = dispinterface;
  IDTSPropsChanger = interface;
  IDTSPropsChangerDisp = dispinterface;
  IDTSPumpProgress = interface;
  IDTSPumpProgressDisp = dispinterface;
  IPumpParams = interface;
  IPumpParamsDisp = dispinterface;
  IPumpParamsGetter = interface;
  IPumpParamsGetterDisp = dispinterface;
  IFileManager = interface;
  IFileManagerDisp = dispinterface;
  IDBHelper = interface;
  IDBHelperDisp = dispinterface;
  IExcelHelper = interface;
  IExcelHelperDisp = dispinterface;
  ITXTSorcerer = interface;
  ITXTSorcererDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  FintechMonthXMLPump = ICustomPump;
  FintechYearXMLPump = ICustomPump;
  FintechMonthDBFPump2 = ICustomPump;
  UFKExcelPump = ICustomPump;
  FMDTSPump = ICustomPump;
  DBFHelper = IDBFHelper;
  GMNHelper = IGMNHelper;
  PumpLogger = IPumpLogger;
  DTSPropsChanger = IDTSPropsChanger;
  DTSPumpProgress = IDTSPumpProgress;
  FileManager = IFileManager;
  DBHelper = IDBHelper;
  ExcelHelper = IExcelHelper;
  PumpParams = IPumpParams;
  TXTSorcerer = ITXTSorcerer;
  TXTReportsPumping = ICustomPump;
  UFKReportsDBF = ICustomPump;
  UFKReportsXLS = ICustomPump;
  UMNSReports_28N = ICustomPump;
  UMNSReports_28N_Disintegrator = ICustomPump;


// *********************************************************************//
// Interface: ICustomPump
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {34A2A0B2-764B-4A2B-BD44-4205C0EA707C}
// *********************************************************************//
  ICustomPump = interface(IDispatch)
    ['{34A2A0B2-764B-4A2B-BD44-4205C0EA707C}']
    procedure PumpReports(PumpID: OleVariant; Progress: OleVariant; Logger: OleVariant; 
                          PumpParams: OleVariant; Source: OleVariant; Destination: OleVariant); safecall;
    procedure DeleteData(PumpID: OleVariant; Progress: OleVariant; Logger: OleVariant; 
                         PumpParams: OleVariant; Source: OleVariant; Destination: OleVariant); safecall;
  end;

// *********************************************************************//
// DispIntf:  ICustomPumpDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {34A2A0B2-764B-4A2B-BD44-4205C0EA707C}
// *********************************************************************//
  ICustomPumpDisp = dispinterface
    ['{34A2A0B2-764B-4A2B-BD44-4205C0EA707C}']
    procedure PumpReports(PumpID: OleVariant; Progress: OleVariant; Logger: OleVariant; 
                          PumpParams: OleVariant; Source: OleVariant; Destination: OleVariant); dispid 1;
    procedure DeleteData(PumpID: OleVariant; Progress: OleVariant; Logger: OleVariant; 
                         PumpParams: OleVariant; Source: OleVariant; Destination: OleVariant); dispid 2;
  end;

// *********************************************************************//
// Interface: IDBFHelper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {9E1F3247-5313-46DA-A21E-5C918CBF8941}
// *********************************************************************//
  IDBFHelper = interface(IDispatch)
    ['{9E1F3247-5313-46DA-A21E-5C918CBF8941}']
    function  CreateSYSDSN(const InputDir: WideString; out ConnectionStr: WideString; 
                           out ErrStr: WideString): WordBool; safecall;
    procedure ClearTempSYSDSNs; safecall;
  end;

// *********************************************************************//
// DispIntf:  IDBFHelperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {9E1F3247-5313-46DA-A21E-5C918CBF8941}
// *********************************************************************//
  IDBFHelperDisp = dispinterface
    ['{9E1F3247-5313-46DA-A21E-5C918CBF8941}']
    function  CreateSYSDSN(const InputDir: WideString; out ConnectionStr: WideString; 
                           out ErrStr: WideString): WordBool; dispid 1;
    procedure ClearTempSYSDSNs; dispid 2;
  end;

// *********************************************************************//
// Interface: IGMNHelper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C78C3CC6-0E10-40EF-A97F-A2B3C854DC91}
// *********************************************************************//
  IGMNHelper = interface(IDispatch)
    ['{C78C3CC6-0E10-40EF-A97F-A2B3C854DC91}']
  end;

// *********************************************************************//
// DispIntf:  IGMNHelperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C78C3CC6-0E10-40EF-A97F-A2B3C854DC91}
// *********************************************************************//
  IGMNHelperDisp = dispinterface
    ['{C78C3CC6-0E10-40EF-A97F-A2B3C854DC91}']
  end;

// *********************************************************************//
// Interface: IPumpLogger
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {F732CF3C-F523-4CBB-B1AB-A9B053EAD215}
// *********************************************************************//
  IPumpLogger = interface(IDispatch)
    ['{F732CF3C-F523-4CBB-B1AB-A9B053EAD215}']
    procedure WriteEventToPumpReport(const ConnectionStr: WideString; EventType: TPumpEventsTypes; 
                                     PumpID: Integer; const Text: WideString); safecall;
    procedure WriteCommentToPumpInfo(const ConnectionStr: WideString; PumpID: Integer; 
                                     const Comment: WideString); safecall;
  end;

// *********************************************************************//
// DispIntf:  IPumpLoggerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {F732CF3C-F523-4CBB-B1AB-A9B053EAD215}
// *********************************************************************//
  IPumpLoggerDisp = dispinterface
    ['{F732CF3C-F523-4CBB-B1AB-A9B053EAD215}']
    procedure WriteEventToPumpReport(const ConnectionStr: WideString; EventType: TPumpEventsTypes; 
                                     PumpID: Integer; const Text: WideString); dispid 1;
    procedure WriteCommentToPumpInfo(const ConnectionStr: WideString; PumpID: Integer; 
                                     const Comment: WideString); dispid 2;
  end;

// *********************************************************************//
// Interface: IDTSPropsChanger
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C874921F-D31E-46A8-BCF8-054F796BEC65}
// *********************************************************************//
  IDTSPropsChanger = interface(IDispatch)
    ['{C874921F-D31E-46A8-BCF8-054F796BEC65}']
  end;

// *********************************************************************//
// DispIntf:  IDTSPropsChangerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C874921F-D31E-46A8-BCF8-054F796BEC65}
// *********************************************************************//
  IDTSPropsChangerDisp = dispinterface
    ['{C874921F-D31E-46A8-BCF8-054F796BEC65}']
  end;

// *********************************************************************//
// Interface: IDTSPumpProgress
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C41EEBD6-FEE6-41F3-A69E-EC1B54AED086}
// *********************************************************************//
  IDTSPumpProgress = interface(IDispatch)
    ['{C41EEBD6-FEE6-41F3-A69E-EC1B54AED086}']
  end;

// *********************************************************************//
// DispIntf:  IDTSPumpProgressDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C41EEBD6-FEE6-41F3-A69E-EC1B54AED086}
// *********************************************************************//
  IDTSPumpProgressDisp = dispinterface
    ['{C41EEBD6-FEE6-41F3-A69E-EC1B54AED086}']
  end;

// *********************************************************************//
// Interface: IPumpParams
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {00F86EB3-63DF-4819-9972-B29E05549F87}
// *********************************************************************//
  IPumpParams = interface(IDispatch)
    ['{00F86EB3-63DF-4819-9972-B29E05549F87}']
    procedure ShowParamsForm(ParentWND: Integer; var PumpParams: OleVariant; var Success: WordBool); safecall;
  end;

// *********************************************************************//
// DispIntf:  IPumpParamsDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {00F86EB3-63DF-4819-9972-B29E05549F87}
// *********************************************************************//
  IPumpParamsDisp = dispinterface
    ['{00F86EB3-63DF-4819-9972-B29E05549F87}']
    procedure ShowParamsForm(ParentWND: Integer; var PumpParams: OleVariant; var Success: WordBool); dispid 1;
  end;

// *********************************************************************//
// Interface: IPumpParamsGetter
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {6764723F-8BBA-4CCA-BE35-745FA348F2DF}
// *********************************************************************//
  IPumpParamsGetter = interface(IDispatch)
    ['{6764723F-8BBA-4CCA-BE35-745FA348F2DF}']
    function  GetParamValueByName(PumpParams: OleVariant; const ParamName: WideString; 
                                  out ErrMsg: WideString): OleVariant; safecall;
    procedure SetParamValueByName(var PumpParams: OleVariant; const ParamName: WideString; 
                                  ParamValue: OleVariant; out ErrMsg: WideString); safecall;
    procedure SetParams(PumpParams: OleVariant; out ErrMsg: WideString); safecall;
    procedure BuildXMLForHTMLView(PumpParams: OleVariant; XMLDoc: OleVariant); safecall;
  end;

// *********************************************************************//
// DispIntf:  IPumpParamsGetterDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {6764723F-8BBA-4CCA-BE35-745FA348F2DF}
// *********************************************************************//
  IPumpParamsGetterDisp = dispinterface
    ['{6764723F-8BBA-4CCA-BE35-745FA348F2DF}']
    function  GetParamValueByName(PumpParams: OleVariant; const ParamName: WideString; 
                                  out ErrMsg: WideString): OleVariant; dispid 1;
    procedure SetParamValueByName(var PumpParams: OleVariant; const ParamName: WideString; 
                                  ParamValue: OleVariant; out ErrMsg: WideString); dispid 3;
    procedure SetParams(PumpParams: OleVariant; out ErrMsg: WideString); dispid 2;
    procedure BuildXMLForHTMLView(PumpParams: OleVariant; XMLDoc: OleVariant); dispid 4;
  end;

// *********************************************************************//
// Interface: IFileManager
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EAB91607-6E93-4826-9A56-BCAE86A492C3}
// *********************************************************************//
  IFileManager = interface(IDispatch)
    ['{EAB91607-6E93-4826-9A56-BCAE86A492C3}']
    procedure FindFiles(Dir: OleVariant; FilesExtensions: OleVariant; 
                        out FindFilesCount: OleVariant; out FindFiles: OleVariant; 
                        out ErrMsg: OleVariant); safecall;
    procedure MoveFile(FileName: OleVariant; ToProcessed: OleVariant; out ErrMsg: OleVariant); safecall;
  end;

// *********************************************************************//
// DispIntf:  IFileManagerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EAB91607-6E93-4826-9A56-BCAE86A492C3}
// *********************************************************************//
  IFileManagerDisp = dispinterface
    ['{EAB91607-6E93-4826-9A56-BCAE86A492C3}']
    procedure FindFiles(Dir: OleVariant; FilesExtensions: OleVariant; 
                        out FindFilesCount: OleVariant; out FindFiles: OleVariant; 
                        out ErrMsg: OleVariant); dispid 1;
    procedure MoveFile(FileName: OleVariant; ToProcessed: OleVariant; out ErrMsg: OleVariant); dispid 2;
  end;

// *********************************************************************//
// Interface: IDBHelper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0D0AD24D-6F80-4A5C-B923-35E61A5ABCA6}
// *********************************************************************//
  IDBHelper = interface(IDispatch)
    ['{0D0AD24D-6F80-4A5C-B923-35E61A5ABCA6}']
    procedure GetNextGenValue(Con: OleVariant; GenName: OleVariant; out GenValue: OleVariant; 
                              out ErrMsg: OleVariant); safecall;
  end;

// *********************************************************************//
// DispIntf:  IDBHelperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0D0AD24D-6F80-4A5C-B923-35E61A5ABCA6}
// *********************************************************************//
  IDBHelperDisp = dispinterface
    ['{0D0AD24D-6F80-4A5C-B923-35E61A5ABCA6}']
    procedure GetNextGenValue(Con: OleVariant; GenName: OleVariant; out GenValue: OleVariant; 
                              out ErrMsg: OleVariant); dispid 1;
  end;

// *********************************************************************//
// Interface: IExcelHelper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {F963370E-05F1-4482-A09D-F2E7F6288B96}
// *********************************************************************//
  IExcelHelper = interface(IDispatch)
    ['{F963370E-05F1-4482-A09D-F2E7F6288B96}']
    procedure LoadDoc(FileName: OleVariant; out ErrMsg: OleVariant); safecall;
    procedure UnloadDoc; safecall;
    procedure GetCellValue(SheetNum: OleVariant; CellAddress: OleVariant; 
                           out CellValue: OleVariant; out ErrMsg: OleVariant); safecall;
    procedure SplitMonospaceTable(SplitConditionals: OleVariant; out xlsData: OleVariant; 
                                  out SplittingTablesBounds: OleVariant; 
                                  out SplittingTablesCount: OleVariant; out TableWidth: OleVariant; 
                                  out ErrMsg: OleVariant); safecall;
    function  Get_ExcelApp: OleVariant; safecall;
    function  Get_ExcelWorkBook: OleVariant; safecall;
    property ExcelApp: OleVariant read Get_ExcelApp;
    property ExcelWorkBook: OleVariant read Get_ExcelWorkBook;
  end;

// *********************************************************************//
// DispIntf:  IExcelHelperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {F963370E-05F1-4482-A09D-F2E7F6288B96}
// *********************************************************************//
  IExcelHelperDisp = dispinterface
    ['{F963370E-05F1-4482-A09D-F2E7F6288B96}']
    procedure LoadDoc(FileName: OleVariant; out ErrMsg: OleVariant); dispid 1;
    procedure UnloadDoc; dispid 2;
    procedure GetCellValue(SheetNum: OleVariant; CellAddress: OleVariant; 
                           out CellValue: OleVariant; out ErrMsg: OleVariant); dispid 3;
    procedure SplitMonospaceTable(SplitConditionals: OleVariant; out xlsData: OleVariant; 
                                  out SplittingTablesBounds: OleVariant; 
                                  out SplittingTablesCount: OleVariant; out TableWidth: OleVariant; 
                                  out ErrMsg: OleVariant); dispid 4;
    property ExcelApp: OleVariant readonly dispid 5;
    property ExcelWorkBook: OleVariant readonly dispid 6;
  end;

// *********************************************************************//
// Interface: ITXTSorcerer
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5B592E89-9787-44E8-9302-4451474617A7}
// *********************************************************************//
  ITXTSorcerer = interface(IDispatch)
    ['{5B592E89-9787-44E8-9302-4451474617A7}']
    function  BewitchFile(ParentWND: LongWord; const SourceDir: WideString; PumpID: SYSINT; 
                          const Progress: IUnknown; const XMLSettings: IUnknown; 
                          var RecSet: IDispatch; out ErrMsg: WideString; out Warnings: OleVariant): HResult; safecall;
  end;

// *********************************************************************//
// DispIntf:  ITXTSorcererDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5B592E89-9787-44E8-9302-4451474617A7}
// *********************************************************************//
  ITXTSorcererDisp = dispinterface
    ['{5B592E89-9787-44E8-9302-4451474617A7}']
    function  BewitchFile(ParentWND: LongWord; const SourceDir: WideString; PumpID: SYSINT; 
                          const Progress: IUnknown; const XMLSettings: IUnknown; 
                          var RecSet: IDispatch; out ErrMsg: WideString; out Warnings: OleVariant): HResult; dispid 1;
  end;

// *********************************************************************//
// The Class CoFintechMonthXMLPump provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass FintechMonthXMLPump. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFintechMonthXMLPump = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoFintechYearXMLPump provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass FintechYearXMLPump. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFintechYearXMLPump = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoFintechMonthDBFPump2 provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass FintechMonthDBFPump2. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFintechMonthDBFPump2 = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoUFKExcelPump provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass UFKExcelPump. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUFKExcelPump = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoFMDTSPump provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass FMDTSPump. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFMDTSPump = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoDBFHelper provides a Create and CreateRemote method to          
// create instances of the default interface IDBFHelper exposed by              
// the CoClass DBFHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDBFHelper = class
    class function Create: IDBFHelper;
    class function CreateRemote(const MachineName: string): IDBFHelper;
  end;

// *********************************************************************//
// The Class CoGMNHelper provides a Create and CreateRemote method to          
// create instances of the default interface IGMNHelper exposed by              
// the CoClass GMNHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoGMNHelper = class
    class function Create: IGMNHelper;
    class function CreateRemote(const MachineName: string): IGMNHelper;
  end;

// *********************************************************************//
// The Class CoPumpLogger provides a Create and CreateRemote method to          
// create instances of the default interface IPumpLogger exposed by              
// the CoClass PumpLogger. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPumpLogger = class
    class function Create: IPumpLogger;
    class function CreateRemote(const MachineName: string): IPumpLogger;
  end;

// *********************************************************************//
// The Class CoDTSPropsChanger provides a Create and CreateRemote method to          
// create instances of the default interface IDTSPropsChanger exposed by              
// the CoClass DTSPropsChanger. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDTSPropsChanger = class
    class function Create: IDTSPropsChanger;
    class function CreateRemote(const MachineName: string): IDTSPropsChanger;
  end;

// *********************************************************************//
// The Class CoDTSPumpProgress provides a Create and CreateRemote method to          
// create instances of the default interface IDTSPumpProgress exposed by              
// the CoClass DTSPumpProgress. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDTSPumpProgress = class
    class function Create: IDTSPumpProgress;
    class function CreateRemote(const MachineName: string): IDTSPumpProgress;
  end;

// *********************************************************************//
// The Class CoFileManager provides a Create and CreateRemote method to          
// create instances of the default interface IFileManager exposed by              
// the CoClass FileManager. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileManager = class
    class function Create: IFileManager;
    class function CreateRemote(const MachineName: string): IFileManager;
  end;

// *********************************************************************//
// The Class CoDBHelper provides a Create and CreateRemote method to          
// create instances of the default interface IDBHelper exposed by              
// the CoClass DBHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDBHelper = class
    class function Create: IDBHelper;
    class function CreateRemote(const MachineName: string): IDBHelper;
  end;

// *********************************************************************//
// The Class CoExcelHelper provides a Create and CreateRemote method to          
// create instances of the default interface IExcelHelper exposed by              
// the CoClass ExcelHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoExcelHelper = class
    class function Create: IExcelHelper;
    class function CreateRemote(const MachineName: string): IExcelHelper;
  end;

// *********************************************************************//
// The Class CoPumpParams provides a Create and CreateRemote method to          
// create instances of the default interface IPumpParams exposed by              
// the CoClass PumpParams. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPumpParams = class
    class function Create: IPumpParams;
    class function CreateRemote(const MachineName: string): IPumpParams;
  end;

// *********************************************************************//
// The Class CoTXTSorcerer provides a Create and CreateRemote method to          
// create instances of the default interface ITXTSorcerer exposed by              
// the CoClass TXTSorcerer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoTXTSorcerer = class
    class function Create: ITXTSorcerer;
    class function CreateRemote(const MachineName: string): ITXTSorcerer;
  end;

// *********************************************************************//
// The Class CoTXTReportsPumping provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass TXTReportsPumping. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoTXTReportsPumping = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoUFKReportsDBF provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass UFKReportsDBF. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUFKReportsDBF = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoUFKReportsXLS provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass UFKReportsXLS. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUFKReportsXLS = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoUMNSReports_28N provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass UMNSReports_28N. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUMNSReports_28N = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

// *********************************************************************//
// The Class CoUMNSReports_28N_Disintegrator provides a Create and CreateRemote method to          
// create instances of the default interface ICustomPump exposed by              
// the CoClass UMNSReports_28N_Disintegrator. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUMNSReports_28N_Disintegrator = class
    class function Create: ICustomPump;
    class function CreateRemote(const MachineName: string): ICustomPump;
  end;

implementation

uses ComObj;

class function CoFintechMonthXMLPump.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_FintechMonthXMLPump) as ICustomPump;
end;

class function CoFintechMonthXMLPump.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FintechMonthXMLPump) as ICustomPump;
end;

class function CoFintechYearXMLPump.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_FintechYearXMLPump) as ICustomPump;
end;

class function CoFintechYearXMLPump.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FintechYearXMLPump) as ICustomPump;
end;

class function CoFintechMonthDBFPump2.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_FintechMonthDBFPump2) as ICustomPump;
end;

class function CoFintechMonthDBFPump2.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FintechMonthDBFPump2) as ICustomPump;
end;

class function CoUFKExcelPump.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_UFKExcelPump) as ICustomPump;
end;

class function CoUFKExcelPump.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UFKExcelPump) as ICustomPump;
end;

class function CoFMDTSPump.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_FMDTSPump) as ICustomPump;
end;

class function CoFMDTSPump.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FMDTSPump) as ICustomPump;
end;

class function CoDBFHelper.Create: IDBFHelper;
begin
  Result := CreateComObject(CLASS_DBFHelper) as IDBFHelper;
end;

class function CoDBFHelper.CreateRemote(const MachineName: string): IDBFHelper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DBFHelper) as IDBFHelper;
end;

class function CoGMNHelper.Create: IGMNHelper;
begin
  Result := CreateComObject(CLASS_GMNHelper) as IGMNHelper;
end;

class function CoGMNHelper.CreateRemote(const MachineName: string): IGMNHelper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_GMNHelper) as IGMNHelper;
end;

class function CoPumpLogger.Create: IPumpLogger;
begin
  Result := CreateComObject(CLASS_PumpLogger) as IPumpLogger;
end;

class function CoPumpLogger.CreateRemote(const MachineName: string): IPumpLogger;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PumpLogger) as IPumpLogger;
end;

class function CoDTSPropsChanger.Create: IDTSPropsChanger;
begin
  Result := CreateComObject(CLASS_DTSPropsChanger) as IDTSPropsChanger;
end;

class function CoDTSPropsChanger.CreateRemote(const MachineName: string): IDTSPropsChanger;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DTSPropsChanger) as IDTSPropsChanger;
end;

class function CoDTSPumpProgress.Create: IDTSPumpProgress;
begin
  Result := CreateComObject(CLASS_DTSPumpProgress) as IDTSPumpProgress;
end;

class function CoDTSPumpProgress.CreateRemote(const MachineName: string): IDTSPumpProgress;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DTSPumpProgress) as IDTSPumpProgress;
end;

class function CoFileManager.Create: IFileManager;
begin
  Result := CreateComObject(CLASS_FileManager) as IFileManager;
end;

class function CoFileManager.CreateRemote(const MachineName: string): IFileManager;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileManager) as IFileManager;
end;

class function CoDBHelper.Create: IDBHelper;
begin
  Result := CreateComObject(CLASS_DBHelper) as IDBHelper;
end;

class function CoDBHelper.CreateRemote(const MachineName: string): IDBHelper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DBHelper) as IDBHelper;
end;

class function CoExcelHelper.Create: IExcelHelper;
begin
  Result := CreateComObject(CLASS_ExcelHelper) as IExcelHelper;
end;

class function CoExcelHelper.CreateRemote(const MachineName: string): IExcelHelper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ExcelHelper) as IExcelHelper;
end;

class function CoPumpParams.Create: IPumpParams;
begin
  Result := CreateComObject(CLASS_PumpParams) as IPumpParams;
end;

class function CoPumpParams.CreateRemote(const MachineName: string): IPumpParams;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PumpParams) as IPumpParams;
end;

class function CoTXTSorcerer.Create: ITXTSorcerer;
begin
  Result := CreateComObject(CLASS_TXTSorcerer) as ITXTSorcerer;
end;

class function CoTXTSorcerer.CreateRemote(const MachineName: string): ITXTSorcerer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_TXTSorcerer) as ITXTSorcerer;
end;

class function CoTXTReportsPumping.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_TXTReportsPumping) as ICustomPump;
end;

class function CoTXTReportsPumping.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_TXTReportsPumping) as ICustomPump;
end;

class function CoUFKReportsDBF.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_UFKReportsDBF) as ICustomPump;
end;

class function CoUFKReportsDBF.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UFKReportsDBF) as ICustomPump;
end;

class function CoUFKReportsXLS.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_UFKReportsXLS) as ICustomPump;
end;

class function CoUFKReportsXLS.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UFKReportsXLS) as ICustomPump;
end;

class function CoUMNSReports_28N.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_UMNSReports_28N) as ICustomPump;
end;

class function CoUMNSReports_28N.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UMNSReports_28N) as ICustomPump;
end;

class function CoUMNSReports_28N_Disintegrator.Create: ICustomPump;
begin
  Result := CreateComObject(CLASS_UMNSReports_28N_Disintegrator) as ICustomPump;
end;

class function CoUMNSReports_28N_Disintegrator.CreateRemote(const MachineName: string): ICustomPump;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UMNSReports_28N_Disintegrator) as ICustomPump;
end;

end.
