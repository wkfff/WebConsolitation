unit Krista_FM_Update_Framework_TLB;

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
// File generated on 04.03.2011 17:04:58 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\UserTools\Planing\Global\Krista.FM.Update.Framework.tlb (1)
// IID\LCID: {563CA010-168F-46C0-A12D-219E3F48DC41}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 mscorlib, (C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb)
//   (2) v2.0 System, (C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.tlb)
//   (3) v2.0 System_Windows_Forms, (C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Windows.Forms.tlb)
//   (4) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (5) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  mscorlib_TLB, System_TLB, System_Windows_Forms_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  Krista_FM_Update_FrameworkMajorVersion = 2;
  Krista_FM_Update_FrameworkMinorVersion = 8;

  LIBID_Krista_FM_Update_Framework: TGUID = '{563CA010-168F-46C0-A12D-219E3F48DC41}';

  IID__XmlFeedReader: TGUID = '{6DD10783-8098-3798-BA84-2619989059D4}';
  IID__FileDownloader: TGUID = '{37598913-842D-3412-91B7-BB6293F24434}';
  IID__FileChecksum: TGUID = '{0C89467E-7492-3734-B535-08DDFF055F33}';
  IID__UpdateTask: TGUID = '{6A89BB0C-629D-381D-9CA9-294D93214A06}';
  IID__NotifyIconForm: TGUID = '{6BA34ECF-DB8E-37C4-B4C7-8411DE06D007}';
  IID__UpdateCondition: TGUID = '{4DD23863-82D1-334D-85D3-D4FB7A14F64D}';
  IID__SchemeDependedCondition: TGUID = '{14120E91-D367-3008-A440-7F09DA26BE09}';
  IID__OKTMOCondition: TGUID = '{3370B864-F49B-353F-AF18-EAAD0D64C5E4}';
  IID__OSCondition: TGUID = '{157B589D-515B-3BD9-AC61-ECF7EEA2906E}';
  IID__MemorySource: TGUID = '{50E2288F-1335-3F48-869D-6C3DD9FEF05B}';
  IID__FileExistsCondition: TGUID = '{304AC7B9-3A59-3F4D-9A45-FB99369A861D}';
  IID__ServerModuleVersionCondition: TGUID = '{DE988C51-2B51-3EDC-AFE9-9E9BF83356E1}';
  IID__FileUpdateTask: TGUID = '{D431EFDA-8659-3739-9C97-B8467BAA6D2B}';
  IID__NotifyIconControl: TGUID = '{82ABC496-83D3-333F-89A6-B9D8B622556A}';
  IID__FileSizeCondition: TGUID = '{5FD65C6F-26B0-363B-9B08-C4CD2E0C429F}';
  IID__FileChecksumCondition: TGUID = '{AB49B252-8D79-347B-AE43-B84167BFD224}';
  IID__NAppUpdateException: TGUID = '{33907E60-213F-3B72-815B-64C17DBE6880}';
  IID__UpdateProcessFailedException: TGUID = '{E0C6F8DF-72C1-36AF-8E40-BF345ED280F6}';
  IID__ReadFeedException: TGUID = '{C656AE45-71BA-3FBD-98B4-1F3A285CC4CE}';
  IID__PreparetaskException: TGUID = '{D16833AF-EDF0-3ED2-AFB3-06ACFD230A36}';
  IID__FileDownloaderException: TGUID = '{5BFFD766-F18B-32F5-84A7-3EDFB03525CB}';
  IID__RollbackException: TGUID = '{ED439704-D754-3DEA-8FC8-780CC2F740D0}';
  IID__FrameworkRemotingException: TGUID = '{6DE0FAA2-AC1D-3B13-9526-1CA0C233A409}';
  IID__PackageExistsCondition: TGUID = '{C8A16FBF-B3D8-3471-808D-DFAF229D8909}';
  IID__PermissionsCheck: TGUID = '{A32C4C36-3CB4-3802-B1F4-806D1C74E54F}';
  IID__PatchListControl: TGUID = '{1CE416D9-141B-3082-8A63-36840DC9D942}';
  IID__VersionUpdateTask: TGUID = '{760D5AF7-66D0-34F3-A4A5-DB99A85F84E3}';
  IID_IConditionItem: TGUID = '{4C6E1212-FCDE-3AD7-A707-63A7579E5AC1}';
  IID__UpdateManagerFactory: TGUID = '{EA27FA8B-AF9B-33F3-A601-CACC98199DFE}';
  IID__UpdatePatch: TGUID = '{640763C0-528E-3D20-A733-1817CD1D63CE}';
  IID__SimpleWebSource: TGUID = '{B865DBB7-0FED-3CC3-914E-CF4CAE078A84}';
  IID__PatchListForm: TGUID = '{0053639E-2552-3C4E-8CB9-888B05B58F11}';
  IID__FileVersionCondition: TGUID = '{B9F9FAB4-5889-3D72-80B0-B2460E764F4F}';
  IID__UpdateItemView: TGUID = '{2B13A581-1307-3AD9-9231-484A2A0B81B9}';
  IID__UpdateFeed: TGUID = '{91FC9AF7-56CF-3893-A07B-B5B5B7B82F36}';
  IID__BooleanCondition: TGUID = '{7F7B23C6-D509-3030-891D-0699D55E365E}';
  IID__FileDateCondition: TGUID = '{4535207E-DC9E-30B0-89E8-D0CB597925C9}';
  IID__ConditionHelper: TGUID = '{3CF0CD67-5B7C-3022-9183-F67FB01C76E2}';
  IID__EntityExistsCondition: TGUID = '{E6DB8FC0-1172-333F-8FA9-13C5B8DA5EC2}';
  IID_IUpdateManagerWrapper: TGUID = '{4932ADA3-4D9D-4EEE-9AE9-286E3269F418}';
  IID__UpdateManagerWrapper: TGUID = '{01E8DF3B-363D-3400-8259-AA2E0093F9FF}';
  CLASS_XmlFeedReader: TGUID = '{F34996BB-8BA5-3A25-8972-5A29773D82B7}';
  CLASS_FileDownloader: TGUID = '{ACB61420-BF4E-3781-B189-3ECAC28D74C5}';
  CLASS_FileChecksum: TGUID = '{AFE1AEF3-AADA-3ED8-9339-8D97941A41F8}';
  CLASS_UpdateTask: TGUID = '{46037C91-100D-3B82-83AD-FE61A1E80A73}';
  CLASS_NotifyIconForm: TGUID = '{000AB0EF-CCA6-3726-8217-BBBB942885C0}';
  CLASS_UpdateCondition: TGUID = '{F9A68019-1EA6-3430-863F-E7525CB44E50}';
  CLASS_SchemeDependedCondition: TGUID = '{4E7A798A-506B-3985-B88E-157686D4F571}';
  CLASS_OKTMOCondition: TGUID = '{60BC6D07-6705-3D02-887B-417946B03DDE}';
  CLASS_OSCondition: TGUID = '{D3AC6CC1-EE0E-3192-8476-52E0716D9A26}';
  CLASS_MemorySource: TGUID = '{DFF72D08-3EAD-3C3C-8824-26798F52882B}';
  CLASS_FileExistsCondition: TGUID = '{63086400-B590-3C4D-B7B7-3998B5FD262E}';
  CLASS_ServerModuleVersionCondition: TGUID = '{958C912F-66CB-3C46-88D5-A5C50744B0FE}';
  CLASS_FileUpdateTask: TGUID = '{DA926D25-921D-3F9A-B47E-09267C594228}';
  CLASS_NotifyIconControl: TGUID = '{A24C8B9B-3EE7-3914-91FE-666A0C14CE83}';
  CLASS_FileSizeCondition: TGUID = '{93D282EC-B9FC-3B15-84F1-2D7C455461E8}';
  CLASS_FileChecksumCondition: TGUID = '{551E1BE0-1FF6-3676-BF2E-8A034C531E10}';
  CLASS_NAppUpdateException: TGUID = '{369EB796-5893-3FA5-915F-BF7567CCA62D}';
  CLASS_UpdateProcessFailedException: TGUID = '{3B226AD3-8990-3E33-A4CA-777B006ECD6A}';
  CLASS_ReadFeedException: TGUID = '{ED8FD6CF-3C63-399E-A914-AE91E720C5D7}';
  CLASS_PreparetaskException: TGUID = '{F32FD7A7-B66E-32A3-9B44-1F4095164B3D}';
  CLASS_FileDownloaderException: TGUID = '{9AB3808D-EB74-36E3-A0F1-9E0A90608087}';
  CLASS_RollbackException: TGUID = '{D6CCC13E-01FD-319D-B414-73C48D9EFA6D}';
  CLASS_FrameworkRemotingException: TGUID = '{D117352E-C103-3C75-876E-75EDB84CCB2D}';
  CLASS_PackageExistsCondition: TGUID = '{E57C5D45-F448-32CE-99ED-4DFA7AFCDA43}';
  CLASS_PermissionsCheck: TGUID = '{A7D8E88D-5D85-3486-B14A-778655FE25DB}';
  CLASS_PatchListControl: TGUID = '{E57BB0B7-C5D8-3F1C-A303-016A1BB2012B}';
  CLASS_VersionUpdateTask: TGUID = '{20B08AD7-680D-30C2-8FEA-AF1B3040AEF0}';
  CLASS_UpdateManagerFactory: TGUID = '{39D55D4E-9BDF-372D-9D58-BCCCE260D14B}';
  CLASS_UpdatePatch: TGUID = '{2C6F73C2-1B1F-32DD-8077-36814C57C86C}';
  CLASS_SimpleWebSource: TGUID = '{55A8A633-F3BB-31F7-A2E7-4BE8BC47F4EB}';
  CLASS_PatchListForm: TGUID = '{95F6E439-FF4D-34FC-87DA-FF6AFBEF3750}';
  CLASS_FileVersionCondition: TGUID = '{0A7A10FE-E82C-37EF-A6BB-264A2652C514}';
  CLASS_UpdateItemView: TGUID = '{F4C647FA-F768-3A68-B9D6-5EFA1EAACF9F}';
  CLASS_UpdateFeed: TGUID = '{A2242841-EDB0-3E89-92E2-F7E4B64E7921}';
  CLASS_BooleanCondition: TGUID = '{04BAE2DA-DF5E-3F85-AF1D-A00D05401F8A}';
  CLASS_FileDateCondition: TGUID = '{DAC3A711-C5BB-3C4F-BE9F-4B3F3832E21B}';
  CLASS_ConditionHelper: TGUID = '{C2259EFB-8982-390A-9E7D-8D67AAA521EF}';
  CLASS_EntityExistsCondition: TGUID = '{129FFE13-A3A5-39CB-8665-BB875E8EDE2A}';
  CLASS_UpdateManagerWrapper: TGUID = '{BE47FCD5-34A7-4B6E-BDAC-8E9A6C73649B}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum UpdateProcessState
type
  UpdateProcessState = TOleEnum;
const
  UpdateProcessState_NotChecked = $00000000;
  UpdateProcessState_Checked = $00000001;
  UpdateProcessState_Prepared = $00000002;
  UpdateProcessState_AppliedSuccessfully = $00000003;
  UpdateProcessState_RollbackRequired = $00000004;
  UpdateProcessState_LastVersion = $00000005;
  UpdateProcessState_Error = $00000006;

// Constants for enum KristaApp
type
  KristaApp = TOleEnum;
const
  KristaApp_SchemeDesigner = $00000000;
  KristaApp_Workplace = $00000001;
  KristaApp_OlapAdmin = $00000002;
  KristaApp_MDXExpert = $00000003;
  KristaApp_OfficeAddIn = $00000004;
  KristaApp_Updater = $00000005;

// Constants for enum Use
type
  Use = TOleEnum;
const
  Use_Required = $00000000;
  Use_Optional = $00000001;
  Use_Prohibited = $00000002;

// Constants for enum ConditionType
type
  ConditionType = TOleEnum;
const
  ConditionType_AND = $00000001;
  ConditionType_OR = $00000002;
  ConditionType_NOT = $00000004;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  _XmlFeedReader = interface;
  _XmlFeedReaderDisp = dispinterface;
  _FileDownloader = interface;
  _FileDownloaderDisp = dispinterface;
  _FileChecksum = interface;
  _FileChecksumDisp = dispinterface;
  _UpdateTask = interface;
  _UpdateTaskDisp = dispinterface;
  _NotifyIconForm = interface;
  _NotifyIconFormDisp = dispinterface;
  _UpdateCondition = interface;
  _UpdateConditionDisp = dispinterface;
  _SchemeDependedCondition = interface;
  _SchemeDependedConditionDisp = dispinterface;
  _OKTMOCondition = interface;
  _OKTMOConditionDisp = dispinterface;
  _OSCondition = interface;
  _OSConditionDisp = dispinterface;
  _MemorySource = interface;
  _MemorySourceDisp = dispinterface;
  _FileExistsCondition = interface;
  _FileExistsConditionDisp = dispinterface;
  _ServerModuleVersionCondition = interface;
  _ServerModuleVersionConditionDisp = dispinterface;
  _FileUpdateTask = interface;
  _FileUpdateTaskDisp = dispinterface;
  _NotifyIconControl = interface;
  _NotifyIconControlDisp = dispinterface;
  _FileSizeCondition = interface;
  _FileSizeConditionDisp = dispinterface;
  _FileChecksumCondition = interface;
  _FileChecksumConditionDisp = dispinterface;
  _NAppUpdateException = interface;
  _NAppUpdateExceptionDisp = dispinterface;
  _UpdateProcessFailedException = interface;
  _UpdateProcessFailedExceptionDisp = dispinterface;
  _ReadFeedException = interface;
  _ReadFeedExceptionDisp = dispinterface;
  _PreparetaskException = interface;
  _PreparetaskExceptionDisp = dispinterface;
  _FileDownloaderException = interface;
  _FileDownloaderExceptionDisp = dispinterface;
  _RollbackException = interface;
  _RollbackExceptionDisp = dispinterface;
  _FrameworkRemotingException = interface;
  _FrameworkRemotingExceptionDisp = dispinterface;
  _PackageExistsCondition = interface;
  _PackageExistsConditionDisp = dispinterface;
  _PermissionsCheck = interface;
  _PermissionsCheckDisp = dispinterface;
  _PatchListControl = interface;
  _PatchListControlDisp = dispinterface;
  _VersionUpdateTask = interface;
  _VersionUpdateTaskDisp = dispinterface;
  IConditionItem = interface;
  IConditionItemDisp = dispinterface;
  _UpdateManagerFactory = interface;
  _UpdateManagerFactoryDisp = dispinterface;
  _UpdatePatch = interface;
  _UpdatePatchDisp = dispinterface;
  _SimpleWebSource = interface;
  _SimpleWebSourceDisp = dispinterface;
  _PatchListForm = interface;
  _PatchListFormDisp = dispinterface;
  _FileVersionCondition = interface;
  _FileVersionConditionDisp = dispinterface;
  _UpdateItemView = interface;
  _UpdateItemViewDisp = dispinterface;
  _UpdateFeed = interface;
  _UpdateFeedDisp = dispinterface;
  _BooleanCondition = interface;
  _BooleanConditionDisp = dispinterface;
  _FileDateCondition = interface;
  _FileDateConditionDisp = dispinterface;
  _ConditionHelper = interface;
  _ConditionHelperDisp = dispinterface;
  _EntityExistsCondition = interface;
  _EntityExistsConditionDisp = dispinterface;
  IUpdateManagerWrapper = interface;
  IUpdateManagerWrapperDisp = dispinterface;
  _UpdateManagerWrapper = interface;
  _UpdateManagerWrapperDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  XmlFeedReader = _XmlFeedReader;
  FileDownloader = _FileDownloader;
  FileChecksum = _FileChecksum;
  UpdateTask = _UpdateTask;
  NotifyIconForm = _NotifyIconForm;
  UpdateCondition = _UpdateCondition;
  SchemeDependedCondition = _SchemeDependedCondition;
  OKTMOCondition = _OKTMOCondition;
  OSCondition = _OSCondition;
  MemorySource = _MemorySource;
  FileExistsCondition = _FileExistsCondition;
  ServerModuleVersionCondition = _ServerModuleVersionCondition;
  FileUpdateTask = _FileUpdateTask;
  NotifyIconControl = _NotifyIconControl;
  FileSizeCondition = _FileSizeCondition;
  FileChecksumCondition = _FileChecksumCondition;
  NAppUpdateException = _NAppUpdateException;
  UpdateProcessFailedException = _UpdateProcessFailedException;
  ReadFeedException = _ReadFeedException;
  PreparetaskException = _PreparetaskException;
  FileDownloaderException = _FileDownloaderException;
  RollbackException = _RollbackException;
  FrameworkRemotingException = _FrameworkRemotingException;
  PackageExistsCondition = _PackageExistsCondition;
  PermissionsCheck = _PermissionsCheck;
  PatchListControl = _PatchListControl;
  VersionUpdateTask = _VersionUpdateTask;
  UpdateManagerFactory = _UpdateManagerFactory;
  UpdatePatch = _UpdatePatch;
  SimpleWebSource = _SimpleWebSource;
  PatchListForm = _PatchListForm;
  FileVersionCondition = _FileVersionCondition;
  UpdateItemView = _UpdateItemView;
  UpdateFeed = _UpdateFeed;
  BooleanCondition = _BooleanCondition;
  FileDateCondition = _FileDateCondition;
  ConditionHelper = _ConditionHelper;
  EntityExistsCondition = _EntityExistsCondition;
  UpdateManagerWrapper = _UpdateManagerWrapper;


// *********************************************************************//
// Interface: _XmlFeedReader
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6DD10783-8098-3798-BA84-2619989059D4}
// *********************************************************************//
  _XmlFeedReader = interface(IDispatch)
    ['{6DD10783-8098-3798-BA84-2619989059D4}']
  end;

// *********************************************************************//
// DispIntf:  _XmlFeedReaderDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6DD10783-8098-3798-BA84-2619989059D4}
// *********************************************************************//
  _XmlFeedReaderDisp = dispinterface
    ['{6DD10783-8098-3798-BA84-2619989059D4}']
  end;

// *********************************************************************//
// Interface: _FileDownloader
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {37598913-842D-3412-91B7-BB6293F24434}
// *********************************************************************//
  _FileDownloader = interface(IDispatch)
    ['{37598913-842D-3412-91B7-BB6293F24434}']
  end;

// *********************************************************************//
// DispIntf:  _FileDownloaderDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {37598913-842D-3412-91B7-BB6293F24434}
// *********************************************************************//
  _FileDownloaderDisp = dispinterface
    ['{37598913-842D-3412-91B7-BB6293F24434}']
  end;

// *********************************************************************//
// Interface: _FileChecksum
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {0C89467E-7492-3734-B535-08DDFF055F33}
// *********************************************************************//
  _FileChecksum = interface(IDispatch)
    ['{0C89467E-7492-3734-B535-08DDFF055F33}']
  end;

// *********************************************************************//
// DispIntf:  _FileChecksumDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {0C89467E-7492-3734-B535-08DDFF055F33}
// *********************************************************************//
  _FileChecksumDisp = dispinterface
    ['{0C89467E-7492-3734-B535-08DDFF055F33}']
  end;

// *********************************************************************//
// Interface: _UpdateTask
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6A89BB0C-629D-381D-9CA9-294D93214A06}
// *********************************************************************//
  _UpdateTask = interface(IDispatch)
    ['{6A89BB0C-629D-381D-9CA9-294D93214A06}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateTaskDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6A89BB0C-629D-381D-9CA9-294D93214A06}
// *********************************************************************//
  _UpdateTaskDisp = dispinterface
    ['{6A89BB0C-629D-381D-9CA9-294D93214A06}']
  end;

// *********************************************************************//
// Interface: _NotifyIconForm
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6BA34ECF-DB8E-37C4-B4C7-8411DE06D007}
// *********************************************************************//
  _NotifyIconForm = interface(IDispatch)
    ['{6BA34ECF-DB8E-37C4-B4C7-8411DE06D007}']
  end;

// *********************************************************************//
// DispIntf:  _NotifyIconFormDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6BA34ECF-DB8E-37C4-B4C7-8411DE06D007}
// *********************************************************************//
  _NotifyIconFormDisp = dispinterface
    ['{6BA34ECF-DB8E-37C4-B4C7-8411DE06D007}']
  end;

// *********************************************************************//
// Interface: _UpdateCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {4DD23863-82D1-334D-85D3-D4FB7A14F64D}
// *********************************************************************//
  _UpdateCondition = interface(IDispatch)
    ['{4DD23863-82D1-334D-85D3-D4FB7A14F64D}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {4DD23863-82D1-334D-85D3-D4FB7A14F64D}
// *********************************************************************//
  _UpdateConditionDisp = dispinterface
    ['{4DD23863-82D1-334D-85D3-D4FB7A14F64D}']
  end;

// *********************************************************************//
// Interface: _SchemeDependedCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {14120E91-D367-3008-A440-7F09DA26BE09}
// *********************************************************************//
  _SchemeDependedCondition = interface(IDispatch)
    ['{14120E91-D367-3008-A440-7F09DA26BE09}']
  end;

// *********************************************************************//
// DispIntf:  _SchemeDependedConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {14120E91-D367-3008-A440-7F09DA26BE09}
// *********************************************************************//
  _SchemeDependedConditionDisp = dispinterface
    ['{14120E91-D367-3008-A440-7F09DA26BE09}']
  end;

// *********************************************************************//
// Interface: _OKTMOCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {3370B864-F49B-353F-AF18-EAAD0D64C5E4}
// *********************************************************************//
  _OKTMOCondition = interface(IDispatch)
    ['{3370B864-F49B-353F-AF18-EAAD0D64C5E4}']
  end;

// *********************************************************************//
// DispIntf:  _OKTMOConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {3370B864-F49B-353F-AF18-EAAD0D64C5E4}
// *********************************************************************//
  _OKTMOConditionDisp = dispinterface
    ['{3370B864-F49B-353F-AF18-EAAD0D64C5E4}']
  end;

// *********************************************************************//
// Interface: _OSCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {157B589D-515B-3BD9-AC61-ECF7EEA2906E}
// *********************************************************************//
  _OSCondition = interface(IDispatch)
    ['{157B589D-515B-3BD9-AC61-ECF7EEA2906E}']
  end;

// *********************************************************************//
// DispIntf:  _OSConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {157B589D-515B-3BD9-AC61-ECF7EEA2906E}
// *********************************************************************//
  _OSConditionDisp = dispinterface
    ['{157B589D-515B-3BD9-AC61-ECF7EEA2906E}']
  end;

// *********************************************************************//
// Interface: _MemorySource
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {50E2288F-1335-3F48-869D-6C3DD9FEF05B}
// *********************************************************************//
  _MemorySource = interface(IDispatch)
    ['{50E2288F-1335-3F48-869D-6C3DD9FEF05B}']
  end;

// *********************************************************************//
// DispIntf:  _MemorySourceDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {50E2288F-1335-3F48-869D-6C3DD9FEF05B}
// *********************************************************************//
  _MemorySourceDisp = dispinterface
    ['{50E2288F-1335-3F48-869D-6C3DD9FEF05B}']
  end;

// *********************************************************************//
// Interface: _FileExistsCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {304AC7B9-3A59-3F4D-9A45-FB99369A861D}
// *********************************************************************//
  _FileExistsCondition = interface(IDispatch)
    ['{304AC7B9-3A59-3F4D-9A45-FB99369A861D}']
  end;

// *********************************************************************//
// DispIntf:  _FileExistsConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {304AC7B9-3A59-3F4D-9A45-FB99369A861D}
// *********************************************************************//
  _FileExistsConditionDisp = dispinterface
    ['{304AC7B9-3A59-3F4D-9A45-FB99369A861D}']
  end;

// *********************************************************************//
// Interface: _ServerModuleVersionCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {DE988C51-2B51-3EDC-AFE9-9E9BF83356E1}
// *********************************************************************//
  _ServerModuleVersionCondition = interface(IDispatch)
    ['{DE988C51-2B51-3EDC-AFE9-9E9BF83356E1}']
  end;

// *********************************************************************//
// DispIntf:  _ServerModuleVersionConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {DE988C51-2B51-3EDC-AFE9-9E9BF83356E1}
// *********************************************************************//
  _ServerModuleVersionConditionDisp = dispinterface
    ['{DE988C51-2B51-3EDC-AFE9-9E9BF83356E1}']
  end;

// *********************************************************************//
// Interface: _FileUpdateTask
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {D431EFDA-8659-3739-9C97-B8467BAA6D2B}
// *********************************************************************//
  _FileUpdateTask = interface(IDispatch)
    ['{D431EFDA-8659-3739-9C97-B8467BAA6D2B}']
  end;

// *********************************************************************//
// DispIntf:  _FileUpdateTaskDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {D431EFDA-8659-3739-9C97-B8467BAA6D2B}
// *********************************************************************//
  _FileUpdateTaskDisp = dispinterface
    ['{D431EFDA-8659-3739-9C97-B8467BAA6D2B}']
  end;

// *********************************************************************//
// Interface: _NotifyIconControl
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {82ABC496-83D3-333F-89A6-B9D8B622556A}
// *********************************************************************//
  _NotifyIconControl = interface(IDispatch)
    ['{82ABC496-83D3-333F-89A6-B9D8B622556A}']
  end;

// *********************************************************************//
// DispIntf:  _NotifyIconControlDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {82ABC496-83D3-333F-89A6-B9D8B622556A}
// *********************************************************************//
  _NotifyIconControlDisp = dispinterface
    ['{82ABC496-83D3-333F-89A6-B9D8B622556A}']
  end;

// *********************************************************************//
// Interface: _FileSizeCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {5FD65C6F-26B0-363B-9B08-C4CD2E0C429F}
// *********************************************************************//
  _FileSizeCondition = interface(IDispatch)
    ['{5FD65C6F-26B0-363B-9B08-C4CD2E0C429F}']
  end;

// *********************************************************************//
// DispIntf:  _FileSizeConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {5FD65C6F-26B0-363B-9B08-C4CD2E0C429F}
// *********************************************************************//
  _FileSizeConditionDisp = dispinterface
    ['{5FD65C6F-26B0-363B-9B08-C4CD2E0C429F}']
  end;

// *********************************************************************//
// Interface: _FileChecksumCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {AB49B252-8D79-347B-AE43-B84167BFD224}
// *********************************************************************//
  _FileChecksumCondition = interface(IDispatch)
    ['{AB49B252-8D79-347B-AE43-B84167BFD224}']
  end;

// *********************************************************************//
// DispIntf:  _FileChecksumConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {AB49B252-8D79-347B-AE43-B84167BFD224}
// *********************************************************************//
  _FileChecksumConditionDisp = dispinterface
    ['{AB49B252-8D79-347B-AE43-B84167BFD224}']
  end;

// *********************************************************************//
// Interface: _NAppUpdateException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {33907E60-213F-3B72-815B-64C17DBE6880}
// *********************************************************************//
  _NAppUpdateException = interface(IDispatch)
    ['{33907E60-213F-3B72-815B-64C17DBE6880}']
  end;

// *********************************************************************//
// DispIntf:  _NAppUpdateExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {33907E60-213F-3B72-815B-64C17DBE6880}
// *********************************************************************//
  _NAppUpdateExceptionDisp = dispinterface
    ['{33907E60-213F-3B72-815B-64C17DBE6880}']
  end;

// *********************************************************************//
// Interface: _UpdateProcessFailedException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {E0C6F8DF-72C1-36AF-8E40-BF345ED280F6}
// *********************************************************************//
  _UpdateProcessFailedException = interface(IDispatch)
    ['{E0C6F8DF-72C1-36AF-8E40-BF345ED280F6}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateProcessFailedExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {E0C6F8DF-72C1-36AF-8E40-BF345ED280F6}
// *********************************************************************//
  _UpdateProcessFailedExceptionDisp = dispinterface
    ['{E0C6F8DF-72C1-36AF-8E40-BF345ED280F6}']
  end;

// *********************************************************************//
// Interface: _ReadFeedException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {C656AE45-71BA-3FBD-98B4-1F3A285CC4CE}
// *********************************************************************//
  _ReadFeedException = interface(IDispatch)
    ['{C656AE45-71BA-3FBD-98B4-1F3A285CC4CE}']
  end;

// *********************************************************************//
// DispIntf:  _ReadFeedExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {C656AE45-71BA-3FBD-98B4-1F3A285CC4CE}
// *********************************************************************//
  _ReadFeedExceptionDisp = dispinterface
    ['{C656AE45-71BA-3FBD-98B4-1F3A285CC4CE}']
  end;

// *********************************************************************//
// Interface: _PreparetaskException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {D16833AF-EDF0-3ED2-AFB3-06ACFD230A36}
// *********************************************************************//
  _PreparetaskException = interface(IDispatch)
    ['{D16833AF-EDF0-3ED2-AFB3-06ACFD230A36}']
  end;

// *********************************************************************//
// DispIntf:  _PreparetaskExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {D16833AF-EDF0-3ED2-AFB3-06ACFD230A36}
// *********************************************************************//
  _PreparetaskExceptionDisp = dispinterface
    ['{D16833AF-EDF0-3ED2-AFB3-06ACFD230A36}']
  end;

// *********************************************************************//
// Interface: _FileDownloaderException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {5BFFD766-F18B-32F5-84A7-3EDFB03525CB}
// *********************************************************************//
  _FileDownloaderException = interface(IDispatch)
    ['{5BFFD766-F18B-32F5-84A7-3EDFB03525CB}']
  end;

// *********************************************************************//
// DispIntf:  _FileDownloaderExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {5BFFD766-F18B-32F5-84A7-3EDFB03525CB}
// *********************************************************************//
  _FileDownloaderExceptionDisp = dispinterface
    ['{5BFFD766-F18B-32F5-84A7-3EDFB03525CB}']
  end;

// *********************************************************************//
// Interface: _RollbackException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {ED439704-D754-3DEA-8FC8-780CC2F740D0}
// *********************************************************************//
  _RollbackException = interface(IDispatch)
    ['{ED439704-D754-3DEA-8FC8-780CC2F740D0}']
  end;

// *********************************************************************//
// DispIntf:  _RollbackExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {ED439704-D754-3DEA-8FC8-780CC2F740D0}
// *********************************************************************//
  _RollbackExceptionDisp = dispinterface
    ['{ED439704-D754-3DEA-8FC8-780CC2F740D0}']
  end;

// *********************************************************************//
// Interface: _FrameworkRemotingException
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6DE0FAA2-AC1D-3B13-9526-1CA0C233A409}
// *********************************************************************//
  _FrameworkRemotingException = interface(IDispatch)
    ['{6DE0FAA2-AC1D-3B13-9526-1CA0C233A409}']
  end;

// *********************************************************************//
// DispIntf:  _FrameworkRemotingExceptionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {6DE0FAA2-AC1D-3B13-9526-1CA0C233A409}
// *********************************************************************//
  _FrameworkRemotingExceptionDisp = dispinterface
    ['{6DE0FAA2-AC1D-3B13-9526-1CA0C233A409}']
  end;

// *********************************************************************//
// Interface: _PackageExistsCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {C8A16FBF-B3D8-3471-808D-DFAF229D8909}
// *********************************************************************//
  _PackageExistsCondition = interface(IDispatch)
    ['{C8A16FBF-B3D8-3471-808D-DFAF229D8909}']
  end;

// *********************************************************************//
// DispIntf:  _PackageExistsConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {C8A16FBF-B3D8-3471-808D-DFAF229D8909}
// *********************************************************************//
  _PackageExistsConditionDisp = dispinterface
    ['{C8A16FBF-B3D8-3471-808D-DFAF229D8909}']
  end;

// *********************************************************************//
// Interface: _PermissionsCheck
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {A32C4C36-3CB4-3802-B1F4-806D1C74E54F}
// *********************************************************************//
  _PermissionsCheck = interface(IDispatch)
    ['{A32C4C36-3CB4-3802-B1F4-806D1C74E54F}']
  end;

// *********************************************************************//
// DispIntf:  _PermissionsCheckDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {A32C4C36-3CB4-3802-B1F4-806D1C74E54F}
// *********************************************************************//
  _PermissionsCheckDisp = dispinterface
    ['{A32C4C36-3CB4-3802-B1F4-806D1C74E54F}']
  end;

// *********************************************************************//
// Interface: _PatchListControl
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {1CE416D9-141B-3082-8A63-36840DC9D942}
// *********************************************************************//
  _PatchListControl = interface(IDispatch)
    ['{1CE416D9-141B-3082-8A63-36840DC9D942}']
  end;

// *********************************************************************//
// DispIntf:  _PatchListControlDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {1CE416D9-141B-3082-8A63-36840DC9D942}
// *********************************************************************//
  _PatchListControlDisp = dispinterface
    ['{1CE416D9-141B-3082-8A63-36840DC9D942}']
  end;

// *********************************************************************//
// Interface: _VersionUpdateTask
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {760D5AF7-66D0-34F3-A4A5-DB99A85F84E3}
// *********************************************************************//
  _VersionUpdateTask = interface(IDispatch)
    ['{760D5AF7-66D0-34F3-A4A5-DB99A85F84E3}']
  end;

// *********************************************************************//
// DispIntf:  _VersionUpdateTaskDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {760D5AF7-66D0-34F3-A4A5-DB99A85F84E3}
// *********************************************************************//
  _VersionUpdateTaskDisp = dispinterface
    ['{760D5AF7-66D0-34F3-A4A5-DB99A85F84E3}']
  end;

// *********************************************************************//
// Interface: IConditionItem
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4C6E1212-FCDE-3AD7-A707-63A7579E5AC1}
// *********************************************************************//
  IConditionItem = interface(IDispatch)
    ['{4C6E1212-FCDE-3AD7-A707-63A7579E5AC1}']
    function  Get__Condition: IUnknown; safecall;
    procedure Set__Condition(const pRetVal: IUnknown); safecall;
    function  Get__ConditionType: Byte; safecall;
    procedure Set__ConditionType(pRetVal: Byte); safecall;
    property _Condition: IUnknown read Get__Condition;
    property _ConditionType: Byte read Get__ConditionType;
  end;

// *********************************************************************//
// DispIntf:  IConditionItemDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4C6E1212-FCDE-3AD7-A707-63A7579E5AC1}
// *********************************************************************//
  IConditionItemDisp = dispinterface
    ['{4C6E1212-FCDE-3AD7-A707-63A7579E5AC1}']
    property _Condition: IUnknown readonly dispid 1610743808;
    property _ConditionType: Byte readonly dispid 1610743810;
  end;

// *********************************************************************//
// Interface: _UpdateManagerFactory
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {EA27FA8B-AF9B-33F3-A601-CACC98199DFE}
// *********************************************************************//
  _UpdateManagerFactory = interface(IDispatch)
    ['{EA27FA8B-AF9B-33F3-A601-CACC98199DFE}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateManagerFactoryDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {EA27FA8B-AF9B-33F3-A601-CACC98199DFE}
// *********************************************************************//
  _UpdateManagerFactoryDisp = dispinterface
    ['{EA27FA8B-AF9B-33F3-A601-CACC98199DFE}']
  end;

// *********************************************************************//
// Interface: _UpdatePatch
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {640763C0-528E-3D20-A733-1817CD1D63CE}
// *********************************************************************//
  _UpdatePatch = interface(IDispatch)
    ['{640763C0-528E-3D20-A733-1817CD1D63CE}']
  end;

// *********************************************************************//
// DispIntf:  _UpdatePatchDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {640763C0-528E-3D20-A733-1817CD1D63CE}
// *********************************************************************//
  _UpdatePatchDisp = dispinterface
    ['{640763C0-528E-3D20-A733-1817CD1D63CE}']
  end;

// *********************************************************************//
// Interface: _SimpleWebSource
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {B865DBB7-0FED-3CC3-914E-CF4CAE078A84}
// *********************************************************************//
  _SimpleWebSource = interface(IDispatch)
    ['{B865DBB7-0FED-3CC3-914E-CF4CAE078A84}']
  end;

// *********************************************************************//
// DispIntf:  _SimpleWebSourceDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {B865DBB7-0FED-3CC3-914E-CF4CAE078A84}
// *********************************************************************//
  _SimpleWebSourceDisp = dispinterface
    ['{B865DBB7-0FED-3CC3-914E-CF4CAE078A84}']
  end;

// *********************************************************************//
// Interface: _PatchListForm
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {0053639E-2552-3C4E-8CB9-888B05B58F11}
// *********************************************************************//
  _PatchListForm = interface(IDispatch)
    ['{0053639E-2552-3C4E-8CB9-888B05B58F11}']
  end;

// *********************************************************************//
// DispIntf:  _PatchListFormDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {0053639E-2552-3C4E-8CB9-888B05B58F11}
// *********************************************************************//
  _PatchListFormDisp = dispinterface
    ['{0053639E-2552-3C4E-8CB9-888B05B58F11}']
  end;

// *********************************************************************//
// Interface: _FileVersionCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {B9F9FAB4-5889-3D72-80B0-B2460E764F4F}
// *********************************************************************//
  _FileVersionCondition = interface(IDispatch)
    ['{B9F9FAB4-5889-3D72-80B0-B2460E764F4F}']
  end;

// *********************************************************************//
// DispIntf:  _FileVersionConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {B9F9FAB4-5889-3D72-80B0-B2460E764F4F}
// *********************************************************************//
  _FileVersionConditionDisp = dispinterface
    ['{B9F9FAB4-5889-3D72-80B0-B2460E764F4F}']
  end;

// *********************************************************************//
// Interface: _UpdateItemView
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {2B13A581-1307-3AD9-9231-484A2A0B81B9}
// *********************************************************************//
  _UpdateItemView = interface(IDispatch)
    ['{2B13A581-1307-3AD9-9231-484A2A0B81B9}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateItemViewDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {2B13A581-1307-3AD9-9231-484A2A0B81B9}
// *********************************************************************//
  _UpdateItemViewDisp = dispinterface
    ['{2B13A581-1307-3AD9-9231-484A2A0B81B9}']
  end;

// *********************************************************************//
// Interface: _UpdateFeed
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {91FC9AF7-56CF-3893-A07B-B5B5B7B82F36}
// *********************************************************************//
  _UpdateFeed = interface(IDispatch)
    ['{91FC9AF7-56CF-3893-A07B-B5B5B7B82F36}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateFeedDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {91FC9AF7-56CF-3893-A07B-B5B5B7B82F36}
// *********************************************************************//
  _UpdateFeedDisp = dispinterface
    ['{91FC9AF7-56CF-3893-A07B-B5B5B7B82F36}']
  end;

// *********************************************************************//
// Interface: _BooleanCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {7F7B23C6-D509-3030-891D-0699D55E365E}
// *********************************************************************//
  _BooleanCondition = interface(IDispatch)
    ['{7F7B23C6-D509-3030-891D-0699D55E365E}']
  end;

// *********************************************************************//
// DispIntf:  _BooleanConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {7F7B23C6-D509-3030-891D-0699D55E365E}
// *********************************************************************//
  _BooleanConditionDisp = dispinterface
    ['{7F7B23C6-D509-3030-891D-0699D55E365E}']
  end;

// *********************************************************************//
// Interface: _FileDateCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {4535207E-DC9E-30B0-89E8-D0CB597925C9}
// *********************************************************************//
  _FileDateCondition = interface(IDispatch)
    ['{4535207E-DC9E-30B0-89E8-D0CB597925C9}']
  end;

// *********************************************************************//
// DispIntf:  _FileDateConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {4535207E-DC9E-30B0-89E8-D0CB597925C9}
// *********************************************************************//
  _FileDateConditionDisp = dispinterface
    ['{4535207E-DC9E-30B0-89E8-D0CB597925C9}']
  end;

// *********************************************************************//
// Interface: _ConditionHelper
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {3CF0CD67-5B7C-3022-9183-F67FB01C76E2}
// *********************************************************************//
  _ConditionHelper = interface(IDispatch)
    ['{3CF0CD67-5B7C-3022-9183-F67FB01C76E2}']
  end;

// *********************************************************************//
// DispIntf:  _ConditionHelperDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {3CF0CD67-5B7C-3022-9183-F67FB01C76E2}
// *********************************************************************//
  _ConditionHelperDisp = dispinterface
    ['{3CF0CD67-5B7C-3022-9183-F67FB01C76E2}']
  end;

// *********************************************************************//
// Interface: _EntityExistsCondition
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {E6DB8FC0-1172-333F-8FA9-13C5B8DA5EC2}
// *********************************************************************//
  _EntityExistsCondition = interface(IDispatch)
    ['{E6DB8FC0-1172-333F-8FA9-13C5B8DA5EC2}']
  end;

// *********************************************************************//
// DispIntf:  _EntityExistsConditionDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {E6DB8FC0-1172-333F-8FA9-13C5B8DA5EC2}
// *********************************************************************//
  _EntityExistsConditionDisp = dispinterface
    ['{E6DB8FC0-1172-333F-8FA9-13C5B8DA5EC2}']
  end;

// *********************************************************************//
// Interface: IUpdateManagerWrapper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4932ADA3-4D9D-4EEE-9AE9-286E3269F418}
// *********************************************************************//
  IUpdateManagerWrapper = interface(IDispatch)
    ['{4932ADA3-4D9D-4EEE-9AE9-286E3269F418}']
    procedure InitializeNotifyIconForm; safecall;
    procedure Set_ServerModulesString(const Param1: WideString); safecall;
    property ServerModulesString: WideString write Set_ServerModulesString;
  end;

// *********************************************************************//
// DispIntf:  IUpdateManagerWrapperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {4932ADA3-4D9D-4EEE-9AE9-286E3269F418}
// *********************************************************************//
  IUpdateManagerWrapperDisp = dispinterface
    ['{4932ADA3-4D9D-4EEE-9AE9-286E3269F418}']
    procedure InitializeNotifyIconForm; dispid 1610743808;
    property ServerModulesString: WideString writeonly dispid 1610743809;
  end;

// *********************************************************************//
// Interface: _UpdateManagerWrapper
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {01E8DF3B-363D-3400-8259-AA2E0093F9FF}
// *********************************************************************//
  _UpdateManagerWrapper = interface(IDispatch)
    ['{01E8DF3B-363D-3400-8259-AA2E0093F9FF}']
  end;

// *********************************************************************//
// DispIntf:  _UpdateManagerWrapperDisp
// Flags:     (4432) Hidden Dual OleAutomation Dispatchable
// GUID:      {01E8DF3B-363D-3400-8259-AA2E0093F9FF}
// *********************************************************************//
  _UpdateManagerWrapperDisp = dispinterface
    ['{01E8DF3B-363D-3400-8259-AA2E0093F9FF}']
  end;

// *********************************************************************//
// The Class CoXmlFeedReader provides a Create and CreateRemote method to          
// create instances of the default interface _XmlFeedReader exposed by              
// the CoClass XmlFeedReader. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoXmlFeedReader = class
    class function Create: _XmlFeedReader;
    class function CreateRemote(const MachineName: string): _XmlFeedReader;
  end;

// *********************************************************************//
// The Class CoFileDownloader provides a Create and CreateRemote method to          
// create instances of the default interface _FileDownloader exposed by              
// the CoClass FileDownloader. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileDownloader = class
    class function Create: _FileDownloader;
    class function CreateRemote(const MachineName: string): _FileDownloader;
  end;

// *********************************************************************//
// The Class CoFileChecksum provides a Create and CreateRemote method to          
// create instances of the default interface _FileChecksum exposed by              
// the CoClass FileChecksum. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileChecksum = class
    class function Create: _FileChecksum;
    class function CreateRemote(const MachineName: string): _FileChecksum;
  end;

// *********************************************************************//
// The Class CoUpdateTask provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateTask exposed by              
// the CoClass UpdateTask. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateTask = class
    class function Create: _UpdateTask;
    class function CreateRemote(const MachineName: string): _UpdateTask;
  end;

// *********************************************************************//
// The Class CoNotifyIconForm provides a Create and CreateRemote method to          
// create instances of the default interface _NotifyIconForm exposed by              
// the CoClass NotifyIconForm. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoNotifyIconForm = class
    class function Create: _NotifyIconForm;
    class function CreateRemote(const MachineName: string): _NotifyIconForm;
  end;

// *********************************************************************//
// The Class CoUpdateCondition provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateCondition exposed by              
// the CoClass UpdateCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateCondition = class
    class function Create: _UpdateCondition;
    class function CreateRemote(const MachineName: string): _UpdateCondition;
  end;

// *********************************************************************//
// The Class CoSchemeDependedCondition provides a Create and CreateRemote method to          
// create instances of the default interface _SchemeDependedCondition exposed by              
// the CoClass SchemeDependedCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoSchemeDependedCondition = class
    class function Create: _SchemeDependedCondition;
    class function CreateRemote(const MachineName: string): _SchemeDependedCondition;
  end;

// *********************************************************************//
// The Class CoOKTMOCondition provides a Create and CreateRemote method to          
// create instances of the default interface _OKTMOCondition exposed by              
// the CoClass OKTMOCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoOKTMOCondition = class
    class function Create: _OKTMOCondition;
    class function CreateRemote(const MachineName: string): _OKTMOCondition;
  end;

// *********************************************************************//
// The Class CoOSCondition provides a Create and CreateRemote method to          
// create instances of the default interface _OSCondition exposed by              
// the CoClass OSCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoOSCondition = class
    class function Create: _OSCondition;
    class function CreateRemote(const MachineName: string): _OSCondition;
  end;

// *********************************************************************//
// The Class CoMemorySource provides a Create and CreateRemote method to          
// create instances of the default interface _MemorySource exposed by              
// the CoClass MemorySource. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMemorySource = class
    class function Create: _MemorySource;
    class function CreateRemote(const MachineName: string): _MemorySource;
  end;

// *********************************************************************//
// The Class CoFileExistsCondition provides a Create and CreateRemote method to          
// create instances of the default interface _FileExistsCondition exposed by              
// the CoClass FileExistsCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileExistsCondition = class
    class function Create: _FileExistsCondition;
    class function CreateRemote(const MachineName: string): _FileExistsCondition;
  end;

// *********************************************************************//
// The Class CoServerModuleVersionCondition provides a Create and CreateRemote method to          
// create instances of the default interface _ServerModuleVersionCondition exposed by              
// the CoClass ServerModuleVersionCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoServerModuleVersionCondition = class
    class function Create: _ServerModuleVersionCondition;
    class function CreateRemote(const MachineName: string): _ServerModuleVersionCondition;
  end;

// *********************************************************************//
// The Class CoFileUpdateTask provides a Create and CreateRemote method to          
// create instances of the default interface _FileUpdateTask exposed by              
// the CoClass FileUpdateTask. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileUpdateTask = class
    class function Create: _FileUpdateTask;
    class function CreateRemote(const MachineName: string): _FileUpdateTask;
  end;

// *********************************************************************//
// The Class CoNotifyIconControl provides a Create and CreateRemote method to          
// create instances of the default interface _NotifyIconControl exposed by              
// the CoClass NotifyIconControl. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoNotifyIconControl = class
    class function Create: _NotifyIconControl;
    class function CreateRemote(const MachineName: string): _NotifyIconControl;
  end;

// *********************************************************************//
// The Class CoFileSizeCondition provides a Create and CreateRemote method to          
// create instances of the default interface _FileSizeCondition exposed by              
// the CoClass FileSizeCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileSizeCondition = class
    class function Create: _FileSizeCondition;
    class function CreateRemote(const MachineName: string): _FileSizeCondition;
  end;

// *********************************************************************//
// The Class CoFileChecksumCondition provides a Create and CreateRemote method to          
// create instances of the default interface _FileChecksumCondition exposed by              
// the CoClass FileChecksumCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileChecksumCondition = class
    class function Create: _FileChecksumCondition;
    class function CreateRemote(const MachineName: string): _FileChecksumCondition;
  end;

// *********************************************************************//
// The Class CoNAppUpdateException provides a Create and CreateRemote method to          
// create instances of the default interface _NAppUpdateException exposed by              
// the CoClass NAppUpdateException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoNAppUpdateException = class
    class function Create: _NAppUpdateException;
    class function CreateRemote(const MachineName: string): _NAppUpdateException;
  end;

// *********************************************************************//
// The Class CoUpdateProcessFailedException provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateProcessFailedException exposed by              
// the CoClass UpdateProcessFailedException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateProcessFailedException = class
    class function Create: _UpdateProcessFailedException;
    class function CreateRemote(const MachineName: string): _UpdateProcessFailedException;
  end;

// *********************************************************************//
// The Class CoReadFeedException provides a Create and CreateRemote method to          
// create instances of the default interface _ReadFeedException exposed by              
// the CoClass ReadFeedException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoReadFeedException = class
    class function Create: _ReadFeedException;
    class function CreateRemote(const MachineName: string): _ReadFeedException;
  end;

// *********************************************************************//
// The Class CoPreparetaskException provides a Create and CreateRemote method to          
// create instances of the default interface _PreparetaskException exposed by              
// the CoClass PreparetaskException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPreparetaskException = class
    class function Create: _PreparetaskException;
    class function CreateRemote(const MachineName: string): _PreparetaskException;
  end;

// *********************************************************************//
// The Class CoFileDownloaderException provides a Create and CreateRemote method to          
// create instances of the default interface _FileDownloaderException exposed by              
// the CoClass FileDownloaderException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileDownloaderException = class
    class function Create: _FileDownloaderException;
    class function CreateRemote(const MachineName: string): _FileDownloaderException;
  end;

// *********************************************************************//
// The Class CoRollbackException provides a Create and CreateRemote method to          
// create instances of the default interface _RollbackException exposed by              
// the CoClass RollbackException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoRollbackException = class
    class function Create: _RollbackException;
    class function CreateRemote(const MachineName: string): _RollbackException;
  end;

// *********************************************************************//
// The Class CoFrameworkRemotingException provides a Create and CreateRemote method to          
// create instances of the default interface _FrameworkRemotingException exposed by              
// the CoClass FrameworkRemotingException. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFrameworkRemotingException = class
    class function Create: _FrameworkRemotingException;
    class function CreateRemote(const MachineName: string): _FrameworkRemotingException;
  end;

// *********************************************************************//
// The Class CoPackageExistsCondition provides a Create and CreateRemote method to          
// create instances of the default interface _PackageExistsCondition exposed by              
// the CoClass PackageExistsCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPackageExistsCondition = class
    class function Create: _PackageExistsCondition;
    class function CreateRemote(const MachineName: string): _PackageExistsCondition;
  end;

// *********************************************************************//
// The Class CoPermissionsCheck provides a Create and CreateRemote method to          
// create instances of the default interface _PermissionsCheck exposed by              
// the CoClass PermissionsCheck. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPermissionsCheck = class
    class function Create: _PermissionsCheck;
    class function CreateRemote(const MachineName: string): _PermissionsCheck;
  end;

// *********************************************************************//
// The Class CoPatchListControl provides a Create and CreateRemote method to          
// create instances of the default interface _PatchListControl exposed by              
// the CoClass PatchListControl. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPatchListControl = class
    class function Create: _PatchListControl;
    class function CreateRemote(const MachineName: string): _PatchListControl;
  end;

// *********************************************************************//
// The Class CoVersionUpdateTask provides a Create and CreateRemote method to          
// create instances of the default interface _VersionUpdateTask exposed by              
// the CoClass VersionUpdateTask. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoVersionUpdateTask = class
    class function Create: _VersionUpdateTask;
    class function CreateRemote(const MachineName: string): _VersionUpdateTask;
  end;

// *********************************************************************//
// The Class CoUpdateManagerFactory provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateManagerFactory exposed by              
// the CoClass UpdateManagerFactory. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateManagerFactory = class
    class function Create: _UpdateManagerFactory;
    class function CreateRemote(const MachineName: string): _UpdateManagerFactory;
  end;

// *********************************************************************//
// The Class CoUpdatePatch provides a Create and CreateRemote method to          
// create instances of the default interface _UpdatePatch exposed by              
// the CoClass UpdatePatch. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdatePatch = class
    class function Create: _UpdatePatch;
    class function CreateRemote(const MachineName: string): _UpdatePatch;
  end;

// *********************************************************************//
// The Class CoSimpleWebSource provides a Create and CreateRemote method to          
// create instances of the default interface _SimpleWebSource exposed by              
// the CoClass SimpleWebSource. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoSimpleWebSource = class
    class function Create: _SimpleWebSource;
    class function CreateRemote(const MachineName: string): _SimpleWebSource;
  end;

// *********************************************************************//
// The Class CoPatchListForm provides a Create and CreateRemote method to          
// create instances of the default interface _PatchListForm exposed by              
// the CoClass PatchListForm. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPatchListForm = class
    class function Create: _PatchListForm;
    class function CreateRemote(const MachineName: string): _PatchListForm;
  end;

// *********************************************************************//
// The Class CoFileVersionCondition provides a Create and CreateRemote method to          
// create instances of the default interface _FileVersionCondition exposed by              
// the CoClass FileVersionCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileVersionCondition = class
    class function Create: _FileVersionCondition;
    class function CreateRemote(const MachineName: string): _FileVersionCondition;
  end;

// *********************************************************************//
// The Class CoUpdateItemView provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateItemView exposed by              
// the CoClass UpdateItemView. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateItemView = class
    class function Create: _UpdateItemView;
    class function CreateRemote(const MachineName: string): _UpdateItemView;
  end;

// *********************************************************************//
// The Class CoUpdateFeed provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateFeed exposed by              
// the CoClass UpdateFeed. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateFeed = class
    class function Create: _UpdateFeed;
    class function CreateRemote(const MachineName: string): _UpdateFeed;
  end;

// *********************************************************************//
// The Class CoBooleanCondition provides a Create and CreateRemote method to          
// create instances of the default interface _BooleanCondition exposed by              
// the CoClass BooleanCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoBooleanCondition = class
    class function Create: _BooleanCondition;
    class function CreateRemote(const MachineName: string): _BooleanCondition;
  end;

// *********************************************************************//
// The Class CoFileDateCondition provides a Create and CreateRemote method to          
// create instances of the default interface _FileDateCondition exposed by              
// the CoClass FileDateCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFileDateCondition = class
    class function Create: _FileDateCondition;
    class function CreateRemote(const MachineName: string): _FileDateCondition;
  end;

// *********************************************************************//
// The Class CoConditionHelper provides a Create and CreateRemote method to          
// create instances of the default interface _ConditionHelper exposed by              
// the CoClass ConditionHelper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoConditionHelper = class
    class function Create: _ConditionHelper;
    class function CreateRemote(const MachineName: string): _ConditionHelper;
  end;

// *********************************************************************//
// The Class CoEntityExistsCondition provides a Create and CreateRemote method to          
// create instances of the default interface _EntityExistsCondition exposed by              
// the CoClass EntityExistsCondition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoEntityExistsCondition = class
    class function Create: _EntityExistsCondition;
    class function CreateRemote(const MachineName: string): _EntityExistsCondition;
  end;

// *********************************************************************//
// The Class CoUpdateManagerWrapper provides a Create and CreateRemote method to          
// create instances of the default interface _UpdateManagerWrapper exposed by              
// the CoClass UpdateManagerWrapper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoUpdateManagerWrapper = class
    class function Create: _UpdateManagerWrapper;
    class function CreateRemote(const MachineName: string): _UpdateManagerWrapper;
  end;

implementation

uses ComObj;

class function CoXmlFeedReader.Create: _XmlFeedReader;
begin
  Result := CreateComObject(CLASS_XmlFeedReader) as _XmlFeedReader;
end;

class function CoXmlFeedReader.CreateRemote(const MachineName: string): _XmlFeedReader;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_XmlFeedReader) as _XmlFeedReader;
end;

class function CoFileDownloader.Create: _FileDownloader;
begin
  Result := CreateComObject(CLASS_FileDownloader) as _FileDownloader;
end;

class function CoFileDownloader.CreateRemote(const MachineName: string): _FileDownloader;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileDownloader) as _FileDownloader;
end;

class function CoFileChecksum.Create: _FileChecksum;
begin
  Result := CreateComObject(CLASS_FileChecksum) as _FileChecksum;
end;

class function CoFileChecksum.CreateRemote(const MachineName: string): _FileChecksum;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileChecksum) as _FileChecksum;
end;

class function CoUpdateTask.Create: _UpdateTask;
begin
  Result := CreateComObject(CLASS_UpdateTask) as _UpdateTask;
end;

class function CoUpdateTask.CreateRemote(const MachineName: string): _UpdateTask;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateTask) as _UpdateTask;
end;

class function CoNotifyIconForm.Create: _NotifyIconForm;
begin
  Result := CreateComObject(CLASS_NotifyIconForm) as _NotifyIconForm;
end;

class function CoNotifyIconForm.CreateRemote(const MachineName: string): _NotifyIconForm;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_NotifyIconForm) as _NotifyIconForm;
end;

class function CoUpdateCondition.Create: _UpdateCondition;
begin
  Result := CreateComObject(CLASS_UpdateCondition) as _UpdateCondition;
end;

class function CoUpdateCondition.CreateRemote(const MachineName: string): _UpdateCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateCondition) as _UpdateCondition;
end;

class function CoSchemeDependedCondition.Create: _SchemeDependedCondition;
begin
  Result := CreateComObject(CLASS_SchemeDependedCondition) as _SchemeDependedCondition;
end;

class function CoSchemeDependedCondition.CreateRemote(const MachineName: string): _SchemeDependedCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_SchemeDependedCondition) as _SchemeDependedCondition;
end;

class function CoOKTMOCondition.Create: _OKTMOCondition;
begin
  Result := CreateComObject(CLASS_OKTMOCondition) as _OKTMOCondition;
end;

class function CoOKTMOCondition.CreateRemote(const MachineName: string): _OKTMOCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_OKTMOCondition) as _OKTMOCondition;
end;

class function CoOSCondition.Create: _OSCondition;
begin
  Result := CreateComObject(CLASS_OSCondition) as _OSCondition;
end;

class function CoOSCondition.CreateRemote(const MachineName: string): _OSCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_OSCondition) as _OSCondition;
end;

class function CoMemorySource.Create: _MemorySource;
begin
  Result := CreateComObject(CLASS_MemorySource) as _MemorySource;
end;

class function CoMemorySource.CreateRemote(const MachineName: string): _MemorySource;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MemorySource) as _MemorySource;
end;

class function CoFileExistsCondition.Create: _FileExistsCondition;
begin
  Result := CreateComObject(CLASS_FileExistsCondition) as _FileExistsCondition;
end;

class function CoFileExistsCondition.CreateRemote(const MachineName: string): _FileExistsCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileExistsCondition) as _FileExistsCondition;
end;

class function CoServerModuleVersionCondition.Create: _ServerModuleVersionCondition;
begin
  Result := CreateComObject(CLASS_ServerModuleVersionCondition) as _ServerModuleVersionCondition;
end;

class function CoServerModuleVersionCondition.CreateRemote(const MachineName: string): _ServerModuleVersionCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ServerModuleVersionCondition) as _ServerModuleVersionCondition;
end;

class function CoFileUpdateTask.Create: _FileUpdateTask;
begin
  Result := CreateComObject(CLASS_FileUpdateTask) as _FileUpdateTask;
end;

class function CoFileUpdateTask.CreateRemote(const MachineName: string): _FileUpdateTask;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileUpdateTask) as _FileUpdateTask;
end;

class function CoNotifyIconControl.Create: _NotifyIconControl;
begin
  Result := CreateComObject(CLASS_NotifyIconControl) as _NotifyIconControl;
end;

class function CoNotifyIconControl.CreateRemote(const MachineName: string): _NotifyIconControl;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_NotifyIconControl) as _NotifyIconControl;
end;

class function CoFileSizeCondition.Create: _FileSizeCondition;
begin
  Result := CreateComObject(CLASS_FileSizeCondition) as _FileSizeCondition;
end;

class function CoFileSizeCondition.CreateRemote(const MachineName: string): _FileSizeCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileSizeCondition) as _FileSizeCondition;
end;

class function CoFileChecksumCondition.Create: _FileChecksumCondition;
begin
  Result := CreateComObject(CLASS_FileChecksumCondition) as _FileChecksumCondition;
end;

class function CoFileChecksumCondition.CreateRemote(const MachineName: string): _FileChecksumCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileChecksumCondition) as _FileChecksumCondition;
end;

class function CoNAppUpdateException.Create: _NAppUpdateException;
begin
  Result := CreateComObject(CLASS_NAppUpdateException) as _NAppUpdateException;
end;

class function CoNAppUpdateException.CreateRemote(const MachineName: string): _NAppUpdateException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_NAppUpdateException) as _NAppUpdateException;
end;

class function CoUpdateProcessFailedException.Create: _UpdateProcessFailedException;
begin
  Result := CreateComObject(CLASS_UpdateProcessFailedException) as _UpdateProcessFailedException;
end;

class function CoUpdateProcessFailedException.CreateRemote(const MachineName: string): _UpdateProcessFailedException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateProcessFailedException) as _UpdateProcessFailedException;
end;

class function CoReadFeedException.Create: _ReadFeedException;
begin
  Result := CreateComObject(CLASS_ReadFeedException) as _ReadFeedException;
end;

class function CoReadFeedException.CreateRemote(const MachineName: string): _ReadFeedException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ReadFeedException) as _ReadFeedException;
end;

class function CoPreparetaskException.Create: _PreparetaskException;
begin
  Result := CreateComObject(CLASS_PreparetaskException) as _PreparetaskException;
end;

class function CoPreparetaskException.CreateRemote(const MachineName: string): _PreparetaskException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PreparetaskException) as _PreparetaskException;
end;

class function CoFileDownloaderException.Create: _FileDownloaderException;
begin
  Result := CreateComObject(CLASS_FileDownloaderException) as _FileDownloaderException;
end;

class function CoFileDownloaderException.CreateRemote(const MachineName: string): _FileDownloaderException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileDownloaderException) as _FileDownloaderException;
end;

class function CoRollbackException.Create: _RollbackException;
begin
  Result := CreateComObject(CLASS_RollbackException) as _RollbackException;
end;

class function CoRollbackException.CreateRemote(const MachineName: string): _RollbackException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_RollbackException) as _RollbackException;
end;

class function CoFrameworkRemotingException.Create: _FrameworkRemotingException;
begin
  Result := CreateComObject(CLASS_FrameworkRemotingException) as _FrameworkRemotingException;
end;

class function CoFrameworkRemotingException.CreateRemote(const MachineName: string): _FrameworkRemotingException;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FrameworkRemotingException) as _FrameworkRemotingException;
end;

class function CoPackageExistsCondition.Create: _PackageExistsCondition;
begin
  Result := CreateComObject(CLASS_PackageExistsCondition) as _PackageExistsCondition;
end;

class function CoPackageExistsCondition.CreateRemote(const MachineName: string): _PackageExistsCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PackageExistsCondition) as _PackageExistsCondition;
end;

class function CoPermissionsCheck.Create: _PermissionsCheck;
begin
  Result := CreateComObject(CLASS_PermissionsCheck) as _PermissionsCheck;
end;

class function CoPermissionsCheck.CreateRemote(const MachineName: string): _PermissionsCheck;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PermissionsCheck) as _PermissionsCheck;
end;

class function CoPatchListControl.Create: _PatchListControl;
begin
  Result := CreateComObject(CLASS_PatchListControl) as _PatchListControl;
end;

class function CoPatchListControl.CreateRemote(const MachineName: string): _PatchListControl;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PatchListControl) as _PatchListControl;
end;

class function CoVersionUpdateTask.Create: _VersionUpdateTask;
begin
  Result := CreateComObject(CLASS_VersionUpdateTask) as _VersionUpdateTask;
end;

class function CoVersionUpdateTask.CreateRemote(const MachineName: string): _VersionUpdateTask;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_VersionUpdateTask) as _VersionUpdateTask;
end;

class function CoUpdateManagerFactory.Create: _UpdateManagerFactory;
begin
  Result := CreateComObject(CLASS_UpdateManagerFactory) as _UpdateManagerFactory;
end;

class function CoUpdateManagerFactory.CreateRemote(const MachineName: string): _UpdateManagerFactory;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateManagerFactory) as _UpdateManagerFactory;
end;

class function CoUpdatePatch.Create: _UpdatePatch;
begin
  Result := CreateComObject(CLASS_UpdatePatch) as _UpdatePatch;
end;

class function CoUpdatePatch.CreateRemote(const MachineName: string): _UpdatePatch;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdatePatch) as _UpdatePatch;
end;

class function CoSimpleWebSource.Create: _SimpleWebSource;
begin
  Result := CreateComObject(CLASS_SimpleWebSource) as _SimpleWebSource;
end;

class function CoSimpleWebSource.CreateRemote(const MachineName: string): _SimpleWebSource;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_SimpleWebSource) as _SimpleWebSource;
end;

class function CoPatchListForm.Create: _PatchListForm;
begin
  Result := CreateComObject(CLASS_PatchListForm) as _PatchListForm;
end;

class function CoPatchListForm.CreateRemote(const MachineName: string): _PatchListForm;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PatchListForm) as _PatchListForm;
end;

class function CoFileVersionCondition.Create: _FileVersionCondition;
begin
  Result := CreateComObject(CLASS_FileVersionCondition) as _FileVersionCondition;
end;

class function CoFileVersionCondition.CreateRemote(const MachineName: string): _FileVersionCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileVersionCondition) as _FileVersionCondition;
end;

class function CoUpdateItemView.Create: _UpdateItemView;
begin
  Result := CreateComObject(CLASS_UpdateItemView) as _UpdateItemView;
end;

class function CoUpdateItemView.CreateRemote(const MachineName: string): _UpdateItemView;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateItemView) as _UpdateItemView;
end;

class function CoUpdateFeed.Create: _UpdateFeed;
begin
  Result := CreateComObject(CLASS_UpdateFeed) as _UpdateFeed;
end;

class function CoUpdateFeed.CreateRemote(const MachineName: string): _UpdateFeed;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateFeed) as _UpdateFeed;
end;

class function CoBooleanCondition.Create: _BooleanCondition;
begin
  Result := CreateComObject(CLASS_BooleanCondition) as _BooleanCondition;
end;

class function CoBooleanCondition.CreateRemote(const MachineName: string): _BooleanCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_BooleanCondition) as _BooleanCondition;
end;

class function CoFileDateCondition.Create: _FileDateCondition;
begin
  Result := CreateComObject(CLASS_FileDateCondition) as _FileDateCondition;
end;

class function CoFileDateCondition.CreateRemote(const MachineName: string): _FileDateCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FileDateCondition) as _FileDateCondition;
end;

class function CoConditionHelper.Create: _ConditionHelper;
begin
  Result := CreateComObject(CLASS_ConditionHelper) as _ConditionHelper;
end;

class function CoConditionHelper.CreateRemote(const MachineName: string): _ConditionHelper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ConditionHelper) as _ConditionHelper;
end;

class function CoEntityExistsCondition.Create: _EntityExistsCondition;
begin
  Result := CreateComObject(CLASS_EntityExistsCondition) as _EntityExistsCondition;
end;

class function CoEntityExistsCondition.CreateRemote(const MachineName: string): _EntityExistsCondition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_EntityExistsCondition) as _EntityExistsCondition;
end;

class function CoUpdateManagerWrapper.Create: _UpdateManagerWrapper;
begin
  Result := CreateComObject(CLASS_UpdateManagerWrapper) as _UpdateManagerWrapper;
end;

class function CoUpdateManagerWrapper.CreateRemote(const MachineName: string): _UpdateManagerWrapper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_UpdateManagerWrapper) as _UpdateManagerWrapper;
end;

end.
