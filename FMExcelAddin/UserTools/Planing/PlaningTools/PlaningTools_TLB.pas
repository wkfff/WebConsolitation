unit PlaningTools_TLB;

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
// File generated on 21.04.2011 11:19:53 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\UserTools\Planing\PlaningTools\PlaningTools.tlb (1)
// IID\LCID: {D8A7FF1E-C463-4A8F-B234-CA42E03CC361}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
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
  PlaningToolsMajorVersion = 1;
  PlaningToolsMinorVersion = 0;

  LIBID_PlaningTools: TGUID = '{D8A7FF1E-C463-4A8F-B234-CA42E03CC361}';

  IID_IProcessForm: TGUID = '{57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9}';
  DIID_IProcessFormEvents: TGUID = '{8DDEDF81-EE9C-46AE-9A1E-2E515D6454BD}';
  CLASS_ProcessForm: TGUID = '{5B07B2DB-ADEB-497B-AD64-73B38756DF45}';
  IID_IOperation: TGUID = '{A76B33A9-C507-4ADD-8707-6E2CF2ECD710}';
  CLASS_Operation: TGUID = '{DF608F6C-1893-4C1B-A7F1-8A0C3814ED52}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum Const_
type
  Const_ = TOleEnum;
const
  otQuery = $00000000;
  otProcess = $00000001;
  otSave = $00000002;
  otUpdate = $00000003;
  otView = $00000004;
  otMap = $00000005;
  otUser = $00000006;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IProcessForm = interface;
  IProcessFormDisp = dispinterface;
  IProcessFormEvents = dispinterface;
  IOperation = interface;
  IOperationDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ProcessForm = IProcessForm;
  Operation = IOperation;


// *********************************************************************//
// Interface: IProcessForm
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9}
// *********************************************************************//
  IProcessForm = interface(IDispatch)
    ['{57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9}']
    procedure OpenOperation(const Name: WideString; IsCritical: WordBool; IsNoteTime: WordBool; 
                            OperationType: Integer); safecall;
    procedure CloseOperation; safecall;
    procedure PostInfo(const AText: WideString); safecall;
    procedure PostWarning(const AText: WideString); safecall;
    procedure PostError(const AText: WideString); safecall;
    procedure SetPBarPosition(CurrentPosition: Integer; MaxPosition: Integer); safecall;
    function  Get_LastError: WideString; safecall;
    function  Get_ErrorList: WideString; safecall;
    procedure Set_ErrorList(const Value: WideString); safecall;
    function  Get_NewProcessClear: WordBool; safecall;
    procedure Set_NewProcessClear(Value: WordBool); safecall;
    procedure ClearErrors; safecall;
    function  Get_Showing: WordBool; safecall;
    procedure OpenProcess(ParentWnd: Integer; const ProcessTitle: WideString; 
                          const SuccessMessage: WideString; const ErrorMessage: WideString; 
                          CanReturn: WordBool); safecall;
    procedure CloseProcess; safecall;
    procedure EnableLogging(const FileName: WideString); safecall;
    procedure DisableLogging; safecall;
    procedure DirectWriteLogString(const Msg: WideString); safecall;
    property LastError: WideString read Get_LastError;
    property ErrorList: WideString read Get_ErrorList write Set_ErrorList;
    property NewProcessClear: WordBool read Get_NewProcessClear write Set_NewProcessClear;
    property Showing: WordBool read Get_Showing;
  end;

// *********************************************************************//
// DispIntf:  IProcessFormDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9}
// *********************************************************************//
  IProcessFormDisp = dispinterface
    ['{57BCCE39-AD6D-4AD2-A1E8-B090D5E82FE9}']
    procedure OpenOperation(const Name: WideString; IsCritical: WordBool; IsNoteTime: WordBool; 
                            OperationType: Integer); dispid 1;
    procedure CloseOperation; dispid 2;
    procedure PostInfo(const AText: WideString); dispid 3;
    procedure PostWarning(const AText: WideString); dispid 4;
    procedure PostError(const AText: WideString); dispid 5;
    procedure SetPBarPosition(CurrentPosition: Integer; MaxPosition: Integer); dispid 6;
    property LastError: WideString readonly dispid 12;
    property ErrorList: WideString dispid 13;
    property NewProcessClear: WordBool dispid 14;
    procedure ClearErrors; dispid 15;
    property Showing: WordBool readonly dispid 18;
    procedure OpenProcess(ParentWnd: Integer; const ProcessTitle: WideString; 
                          const SuccessMessage: WideString; const ErrorMessage: WideString; 
                          CanReturn: WordBool); dispid 20;
    procedure CloseProcess; dispid 21;
    procedure EnableLogging(const FileName: WideString); dispid 7;
    procedure DisableLogging; dispid 8;
    procedure DirectWriteLogString(const Msg: WideString); dispid 9;
  end;

// *********************************************************************//
// DispIntf:  IProcessFormEvents
// Flags:     (4096) Dispatchable
// GUID:      {8DDEDF81-EE9C-46AE-9A1E-2E515D6454BD}
// *********************************************************************//
  IProcessFormEvents = dispinterface
    ['{8DDEDF81-EE9C-46AE-9A1E-2E515D6454BD}']
    procedure OnReturn; dispid 1;
    procedure OnClose; dispid 2;
    procedure OnShow; dispid 3;
  end;

// *********************************************************************//
// Interface: IOperation
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {A76B33A9-C507-4ADD-8707-6E2CF2ECD710}
// *********************************************************************//
  IOperation = interface(IDispatch)
    ['{A76B33A9-C507-4ADD-8707-6E2CF2ECD710}']
    procedure StartOperation(ParentWnd: Integer); safecall;
    procedure StopOperation; safecall;
    procedure Set_Caption(const Param1: WideString); safecall;
    property Caption: WideString write Set_Caption;
  end;

// *********************************************************************//
// DispIntf:  IOperationDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {A76B33A9-C507-4ADD-8707-6E2CF2ECD710}
// *********************************************************************//
  IOperationDisp = dispinterface
    ['{A76B33A9-C507-4ADD-8707-6E2CF2ECD710}']
    procedure StartOperation(ParentWnd: Integer); dispid 1;
    procedure StopOperation; dispid 2;
    property Caption: WideString writeonly dispid 4;
  end;

// *********************************************************************//
// The Class CoProcessForm provides a Create and CreateRemote method to          
// create instances of the default interface IProcessForm exposed by              
// the CoClass ProcessForm. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoProcessForm = class
    class function Create: IProcessForm;
    class function CreateRemote(const MachineName: string): IProcessForm;
  end;

// *********************************************************************//
// The Class CoOperation provides a Create and CreateRemote method to          
// create instances of the default interface IOperation exposed by              
// the CoClass Operation. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoOperation = class
    class function Create: IOperation;
    class function CreateRemote(const MachineName: string): IOperation;
  end;

implementation

uses ComObj;

class function CoProcessForm.Create: IProcessForm;
begin
  Result := CreateComObject(CLASS_ProcessForm) as IProcessForm;
end;

class function CoProcessForm.CreateRemote(const MachineName: string): IProcessForm;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ProcessForm) as IProcessForm;
end;

class function CoOperation.Create: IOperation;
begin
  Result := CreateComObject(CLASS_Operation) as IOperation;
end;

class function CoOperation.CreateRemote(const MachineName: string): IOperation;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Operation) as IOperation;
end;

end.
