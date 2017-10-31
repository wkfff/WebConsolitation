unit FMExcelAddin_TLB;

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
// File generated on 14.09.2009 14:29:08 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\UserTools\Planing\ExcelAddIn\FMExcelAddin.tlb (1)
// IID\LCID: {BD0B1F95-72C2-4C95-9FAB-0DD9FDA46FAE}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v1.0 Planing, (X:\System\UserTools\Planing\global\Planing.tlb)
//   (3) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  Planing_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  FMExcelAddinMajorVersion = 1;
  FMExcelAddinMinorVersion = 0;

  LIBID_FMExcelAddin: TGUID = '{BD0B1F95-72C2-4C95-9FAB-0DD9FDA46FAE}';

  IID_IDTExtensibility2: TGUID = '{B65AD801-ABAF-11D0-BB8B-00A0C90F2744}';
  CLASS_DTExtensibility2: TGUID = '{7C76582A-8ACE-4F4B-8950-9F34C04A802E}';
  IID_ISOAPDimChooser: TGUID = '{C73630B5-97EF-4227-9C08-B8C646FA23AE}';
  CLASS_SOAPDimChooser: TGUID = '{E0C8DFBC-B49B-4167-A000-2BE924710FD2}';
  IID_ISOAPDimEditor: TGUID = '{0D3B3075-8467-4255-81A5-CA7FB3B9A600}';
  CLASS_SOAPDimEditor: TGUID = '{775772AE-84D8-4EE8-8907-E1A9B6E47303}';
  CLASS_FrictianalCoClass: TGUID = '{D066BD19-476A-4773-9641-B49495F5FC47}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IDTExtensibility2 = interface;
  IDTExtensibility2Disp = dispinterface;
  ISOAPDimChooser = interface;
  ISOAPDimChooserDisp = dispinterface;
  ISOAPDimEditor = interface;
  ISOAPDimEditorDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  DTExtensibility2 = IDTExtensibility2;
  SOAPDimChooser = ISOAPDimChooser;
  SOAPDimEditor = ISOAPDimEditor;
  FrictianalCoClass = IFMPlanningExtension;


// *********************************************************************//
// Declaration of structures, unions and aliases.                         
// *********************************************************************//
  PPSafeArray1 = ^PSafeArray; 
  PPSafeArray2 = ^PSafeArray; 
  PPSafeArray3 = ^PSafeArray; {*}


// *********************************************************************//
// Interface: IDTExtensibility2
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {B65AD801-ABAF-11D0-BB8B-00A0C90F2744}
// *********************************************************************//
  IDTExtensibility2 = interface(IDispatch)
    ['{B65AD801-ABAF-11D0-BB8B-00A0C90F2744}']
    procedure OnConnection(const HostApp: IDispatch; ext_ConnectMode: Integer; 
                           const AddInInst: IDispatch; var custom: PSafeArray); safecall;
    procedure OnDisconnection(ext_DisconnectMode: Integer; var custom: PSafeArray); safecall;
    procedure OnAddInsUpdate(var custom: PSafeArray); safecall;
    procedure OnStartupComplete(var custom: PSafeArray); safecall;
    procedure BeginShutdown(var custom: PSafeArray); safecall;
  end;

// *********************************************************************//
// DispIntf:  IDTExtensibility2Disp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {B65AD801-ABAF-11D0-BB8B-00A0C90F2744}
// *********************************************************************//
  IDTExtensibility2Disp = dispinterface
    ['{B65AD801-ABAF-11D0-BB8B-00A0C90F2744}']
    procedure OnConnection(const HostApp: IDispatch; ext_ConnectMode: Integer; 
                           const AddInInst: IDispatch; var custom: {??PSafeArray} OleVariant); dispid 1;
    procedure OnDisconnection(ext_DisconnectMode: Integer; var custom: {??PSafeArray} OleVariant); dispid 2;
    procedure OnAddInsUpdate(var custom: {??PSafeArray} OleVariant); dispid 3;
    procedure OnStartupComplete(var custom: {??PSafeArray} OleVariant); dispid 4;
    procedure BeginShutdown(var custom: {??PSafeArray} OleVariant); dispid 5;
  end;

// *********************************************************************//
// Interface: ISOAPDimChooser
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C73630B5-97EF-4227-9C08-B8C646FA23AE}
// *********************************************************************//
  ISOAPDimChooser = interface(IDispatch)
    ['{C73630B5-97EF-4227-9C08-B8C646FA23AE}']
    function  SelectDimension(parentWnd: SYSINT; const URL: WideString; 
                              const SchemeName: WideString; var DimensionName: WideString): WordBool; safecall;
    function  Get_RefreshOnShow: WordBool; safecall;
    procedure Set_RefreshOnShow(Value: WordBool); safecall;
    function  Get_LastError: WideString; safecall;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); safecall;
    property RefreshOnShow: WordBool read Get_RefreshOnShow write Set_RefreshOnShow;
    property LastError: WideString read Get_LastError;
  end;

// *********************************************************************//
// DispIntf:  ISOAPDimChooserDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C73630B5-97EF-4227-9C08-B8C646FA23AE}
// *********************************************************************//
  ISOAPDimChooserDisp = dispinterface
    ['{C73630B5-97EF-4227-9C08-B8C646FA23AE}']
    function  SelectDimension(parentWnd: SYSINT; const URL: WideString; 
                              const SchemeName: WideString; var DimensionName: WideString): WordBool; dispid 1;
    property RefreshOnShow: WordBool dispid 2;
    property LastError: WideString readonly dispid 3;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); dispid 4;
  end;

// *********************************************************************//
// Interface: ISOAPDimEditor
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0D3B3075-8467-4255-81A5-CA7FB3B9A600}
// *********************************************************************//
  ISOAPDimEditor = interface(IDispatch)
    ['{0D3B3075-8467-4255-81A5-CA7FB3B9A600}']
    function  EditMemberTree(parentWnd: SYSINT; const URL: WideString; 
                             const SchemeName: WideString; const DimensionName: WideString; 
                             var Value: WideString): WordBool; safecall;
    function  Get_LastError: WideString; safecall;
    function  GetTextMemberList(const XMLValue: WideString): WideString; safecall;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); safecall;
    property LastError: WideString read Get_LastError;
  end;

// *********************************************************************//
// DispIntf:  ISOAPDimEditorDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0D3B3075-8467-4255-81A5-CA7FB3B9A600}
// *********************************************************************//
  ISOAPDimEditorDisp = dispinterface
    ['{0D3B3075-8467-4255-81A5-CA7FB3B9A600}']
    function  EditMemberTree(parentWnd: SYSINT; const URL: WideString; 
                             const SchemeName: WideString; const DimensionName: WideString; 
                             var Value: WideString): WordBool; dispid 1;
    property LastError: WideString readonly dispid 2;
    function  GetTextMemberList(const XMLValue: WideString): WideString; dispid 3;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); dispid 4;
  end;

// *********************************************************************//
// The Class CoDTExtensibility2 provides a Create and CreateRemote method to          
// create instances of the default interface IDTExtensibility2 exposed by              
// the CoClass DTExtensibility2. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDTExtensibility2 = class
    class function Create: IDTExtensibility2;
    class function CreateRemote(const MachineName: string): IDTExtensibility2;
  end;

// *********************************************************************//
// The Class CoSOAPDimChooser provides a Create and CreateRemote method to          
// create instances of the default interface ISOAPDimChooser exposed by              
// the CoClass SOAPDimChooser. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoSOAPDimChooser = class
    class function Create: ISOAPDimChooser;
    class function CreateRemote(const MachineName: string): ISOAPDimChooser;
  end;

// *********************************************************************//
// The Class CoSOAPDimEditor provides a Create and CreateRemote method to          
// create instances of the default interface ISOAPDimEditor exposed by              
// the CoClass SOAPDimEditor. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoSOAPDimEditor = class
    class function Create: ISOAPDimEditor;
    class function CreateRemote(const MachineName: string): ISOAPDimEditor;
  end;

// *********************************************************************//
// The Class CoFrictianalCoClass provides a Create and CreateRemote method to          
// create instances of the default interface IFMPlanningExtension exposed by              
// the CoClass FrictianalCoClass. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoFrictianalCoClass = class
    class function Create: IFMPlanningExtension;
    class function CreateRemote(const MachineName: string): IFMPlanningExtension;
  end;

implementation

uses ComObj;

class function CoDTExtensibility2.Create: IDTExtensibility2;
begin
  Result := CreateComObject(CLASS_DTExtensibility2) as IDTExtensibility2;
end;

class function CoDTExtensibility2.CreateRemote(const MachineName: string): IDTExtensibility2;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DTExtensibility2) as IDTExtensibility2;
end;

class function CoSOAPDimChooser.Create: ISOAPDimChooser;
begin
  Result := CreateComObject(CLASS_SOAPDimChooser) as ISOAPDimChooser;
end;

class function CoSOAPDimChooser.CreateRemote(const MachineName: string): ISOAPDimChooser;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_SOAPDimChooser) as ISOAPDimChooser;
end;

class function CoSOAPDimEditor.Create: ISOAPDimEditor;
begin
  Result := CreateComObject(CLASS_SOAPDimEditor) as ISOAPDimEditor;
end;

class function CoSOAPDimEditor.CreateRemote(const MachineName: string): ISOAPDimEditor;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_SOAPDimEditor) as ISOAPDimEditor;
end;

class function CoFrictianalCoClass.Create: IFMPlanningExtension;
begin
  Result := CreateComObject(CLASS_FrictianalCoClass) as IFMPlanningExtension;
end;

class function CoFrictianalCoClass.CreateRemote(const MachineName: string): IFMPlanningExtension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_FrictianalCoClass) as IFMPlanningExtension;
end;

end.
