unit CommonTools_TLB;

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
// File generated on 21.08.2004 7:45:35 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\AdminTools\CommonTools\CommonTools.tlb (1)
// IID\LCID: {3DDF9509-21CA-4F64-B649-85EDC23BA681}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINNT\system32\stdvcl40.dll)
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
  CommonToolsMajorVersion = 1;
  CommonToolsMinorVersion = 0;

  LIBID_CommonTools: TGUID = '{3DDF9509-21CA-4F64-B649-85EDC23BA681}';

  IID_IProgress: TGUID = '{A0C1E9F2-27BE-49E1-A337-3387A4AF8DC0}';
  CLASS_Progress: TGUID = '{502DF36C-6717-424B-B6D9-7CA88696A653}';
  IID_IOperation: TGUID = '{45297C0D-8827-4635-9F66-88935F459515}';
  CLASS_Operation: TGUID = '{BC477B50-16AA-4A84-85F8-B34ECAF89DCA}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IProgress = interface;
  IProgressDisp = dispinterface;
  IOperation = interface;
  IOperationDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  Progress = IProgress;
  Operation = IOperation;


// *********************************************************************//
// Interface: IProgress
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {A0C1E9F2-27BE-49E1-A337-3387A4AF8DC0}
// *********************************************************************//
  IProgress = interface(IDispatch)
    ['{A0C1E9F2-27BE-49E1-A337-3387A4AF8DC0}']
    procedure StartProgress(HasStopped: WordBool); safecall;
    procedure StopProgress; safecall;
    function  Get_MaxProgress: Integer; safecall;
    procedure Set_MaxProgress(Value: Integer); safecall;
    function  Get_MinProgress: Integer; safecall;
    procedure Set_MinProgress(Value: Integer); safecall;
    function  Get_Position: Integer; safecall;
    procedure Set_Position(Value: Integer); safecall;
    function  Get_ProgressCaption: WideString; safecall;
    procedure Set_ProgressCaption(const Value: WideString); safecall;
    function  Get_ProgressMsg: WideString; safecall;
    procedure Set_ProgressMsg(const Value: WideString); safecall;
    function  Get_Stopped: WordBool; safecall;
    procedure Set_ParentWnd(Value: Integer); safecall;
    property MaxProgress: Integer read Get_MaxProgress write Set_MaxProgress;
    property MinProgress: Integer read Get_MinProgress write Set_MinProgress;
    property Position: Integer read Get_Position write Set_Position;
    property ProgressCaption: WideString read Get_ProgressCaption write Set_ProgressCaption;
    property ProgressMsg: WideString read Get_ProgressMsg write Set_ProgressMsg;
    property Stopped: WordBool read Get_Stopped;
    property ParentWnd: Integer write Set_ParentWnd;
  end;

// *********************************************************************//
// DispIntf:  IProgressDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {A0C1E9F2-27BE-49E1-A337-3387A4AF8DC0}
// *********************************************************************//
  IProgressDisp = dispinterface
    ['{A0C1E9F2-27BE-49E1-A337-3387A4AF8DC0}']
    procedure StartProgress(HasStopped: WordBool); dispid 1;
    procedure StopProgress; dispid 2;
    property MaxProgress: Integer dispid 3;
    property MinProgress: Integer dispid 4;
    property Position: Integer dispid 5;
    property ProgressCaption: WideString dispid 6;
    property ProgressMsg: WideString dispid 7;
    property Stopped: WordBool readonly dispid 8;
    property ParentWnd: Integer writeonly dispid 9;
  end;

// *********************************************************************//
// Interface: IOperation
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {45297C0D-8827-4635-9F66-88935F459515}
// *********************************************************************//
  IOperation = interface(IDispatch)
    ['{45297C0D-8827-4635-9F66-88935F459515}']
    procedure StartOperation(ParentWnd: Integer); safecall;
    procedure StopOperation; safecall;
    procedure Set_Caption(const Param1: WideString); safecall;
    property Caption: WideString write Set_Caption;
  end;

// *********************************************************************//
// DispIntf:  IOperationDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {45297C0D-8827-4635-9F66-88935F459515}
// *********************************************************************//
  IOperationDisp = dispinterface
    ['{45297C0D-8827-4635-9F66-88935F459515}']
    procedure StartOperation(ParentWnd: Integer); dispid 1;
    procedure StopOperation; dispid 2;
    property Caption: WideString writeonly dispid 3;
  end;

// *********************************************************************//
// The Class CoProgress provides a Create and CreateRemote method to          
// create instances of the default interface IProgress exposed by              
// the CoClass Progress. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoProgress = class
    class function Create: IProgress;
    class function CreateRemote(const MachineName: string): IProgress;
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

class function CoProgress.Create: IProgress;
begin
  Result := CreateComObject(CLASS_Progress) as IProgress;
end;

class function CoProgress.CreateRemote(const MachineName: string): IProgress;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Progress) as IProgress;
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
