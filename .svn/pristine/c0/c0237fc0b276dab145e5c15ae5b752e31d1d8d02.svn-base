unit ScriptDriver_TLB;

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

// PASTLWTR : $Revision:   1.88  $
// File generated on 24.06.2003 14:42:49 from Type Library described below.

// *************************************************************************//
// NOTE:                                                                      
// Items guarded by $IFDEF_LIVE_SERVER_AT_DESIGN_TIME are used by properties  
// which return objects that may need to be explicitly created via a function 
// call prior to any access via the property. These items have been disabled  
// in order to prevent accidental use from within the object inspector. You   
// may enable them by defining LIVE_SERVER_AT_DESIGN_TIME or by selectively   
// removing them from the $IFDEF blocks. However, such items must still be    
// programmatically created via a method of the appropriate CoClass before    
// they can be used.                                                          
// ************************************************************************ //
// Type Lib: X:\System\ScriptDriver\ScriptDriver.tlb (1)
// IID\LCID: {CF487521-3AE8-11D4-9B6D-008048E67FE2}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (D:\WINNTSRV\System32\stdole2.tlb)
//   (2) v2.0 StdType, (D:\WINNTSRV\System32\OLEPRO32.DLL)
// Errors:
//   Hint: TypeInfo 'ScriptDriver' changed to 'ScriptDriver_'
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
  ScriptDriverMajorVersion = 1;
  ScriptDriverMinorVersion = 0;

  LIBID_ScriptDriver: TGUID = '{CF487521-3AE8-11D4-9B6D-008048E67FE2}';

  IID_IScriptDriver: TGUID = '{CF487529-3AE8-11D4-9B6D-008048E67FE2}';
  CLASS_ScriptDriver_: TGUID = '{C7D258D8-36A7-460E-9DF5-9B30FF4B0B9C}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum ScriptType
type
  ScriptType = TOleEnum;
const
  stVBScript = $00000001;
  stJavaScript = $00000002;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IScriptDriver = interface;
  IScriptDriverDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ScriptDriver_ = IScriptDriver;


// *********************************************************************//
// Interface: IScriptDriver
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {CF487529-3AE8-11D4-9B6D-008048E67FE2}
// *********************************************************************//
  IScriptDriver = interface(IDispatch)
    ['{CF487529-3AE8-11D4-9B6D-008048E67FE2}']
    procedure NewSession(SType: ScriptType); safecall;
    function  AddInterface(const IntfName: WideString; const Intf: IDispatch): WordBool; safecall;
    procedure AddTypeLib(const TypeLibGuid: WideString; dwMajor: Integer; dwMinor: Integer); safecall;
    function  ExecScript(const Text: WideString; out Res: OleVariant; out Error: WideString): Integer; safecall;
    function  GetScriptDispatch(const ItemName: WideString; out Disp: IDispatch): Integer; safecall;
    function  AddScriptlet(const Code: WideString; const ItemName: WideString; 
                           const SubItemName: WideString; const EventName: WideString; 
                           out Name: WideString; out Error: WideString): Integer; safecall;
    function  CloseSession: WordBool; safecall;
    function  Execute(const Text: WideString): WordBool; safecall;
    function  Get_ScriptDispatch: IDispatch; safecall;
    property ScriptDispatch: IDispatch read Get_ScriptDispatch;
  end;

// *********************************************************************//
// DispIntf:  IScriptDriverDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {CF487529-3AE8-11D4-9B6D-008048E67FE2}
// *********************************************************************//
  IScriptDriverDisp = dispinterface
    ['{CF487529-3AE8-11D4-9B6D-008048E67FE2}']
    procedure NewSession(SType: ScriptType); dispid 1;
    function  AddInterface(const IntfName: WideString; const Intf: IDispatch): WordBool; dispid 2;
    procedure AddTypeLib(const TypeLibGuid: WideString; dwMajor: Integer; dwMinor: Integer); dispid 3;
    function  ExecScript(const Text: WideString; out Res: OleVariant; out Error: WideString): Integer; dispid 4;
    function  GetScriptDispatch(const ItemName: WideString; out Disp: IDispatch): Integer; dispid 5;
    function  AddScriptlet(const Code: WideString; const ItemName: WideString; 
                           const SubItemName: WideString; const EventName: WideString; 
                           out Name: WideString; out Error: WideString): Integer; dispid 6;
    function  CloseSession: WordBool; dispid 7;
    function  Execute(const Text: WideString): WordBool; dispid 8;
    property ScriptDispatch: IDispatch readonly dispid 9;
  end;

// *********************************************************************//
// The Class CoScriptDriver_ provides a Create and CreateRemote method to          
// create instances of the default interface IScriptDriver exposed by              
// the CoClass ScriptDriver_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoScriptDriver_ = class
    class function Create: IScriptDriver;
    class function CreateRemote(const MachineName: string): IScriptDriver;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TScriptDriver_
// Help String      : 
// Default Interface: IScriptDriver
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TScriptDriver_Properties= class;
{$ENDIF}
  TScriptDriver_ = class(TOleServer)
  private
    FIntf:        IScriptDriver;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TScriptDriver_Properties;
    function      GetServerProperties: TScriptDriver_Properties;
{$ENDIF}
    function      GetDefaultInterface: IScriptDriver;
  protected
    procedure InitServerData; override;
    function  Get_ScriptDispatch: IDispatch;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IScriptDriver);
    procedure Disconnect; override;
    procedure NewSession(SType: ScriptType);
    function  AddInterface(const IntfName: WideString; const Intf: IDispatch): WordBool;
    procedure AddTypeLib(const TypeLibGuid: WideString; dwMajor: Integer; dwMinor: Integer);
    function  ExecScript(const Text: WideString; out Res: OleVariant; out Error: WideString): Integer;
    function  GetScriptDispatch(const ItemName: WideString; out Disp: IDispatch): Integer;
    function  AddScriptlet(const Code: WideString; const ItemName: WideString; 
                           const SubItemName: WideString; const EventName: WideString; 
                           out Name: WideString; out Error: WideString): Integer;
    function  CloseSession: WordBool;
    function  Execute(const Text: WideString): WordBool;
    property  DefaultInterface: IScriptDriver read GetDefaultInterface;
    property ScriptDispatch: IDispatch read Get_ScriptDispatch;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TScriptDriver_Properties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TScriptDriver_
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TScriptDriver_Properties = class(TPersistent)
  private
    FServer:    TScriptDriver_;
    function    GetDefaultInterface: IScriptDriver;
    constructor Create(AServer: TScriptDriver_);
  protected
    function  Get_ScriptDispatch: IDispatch;
  public
    property DefaultInterface: IScriptDriver read GetDefaultInterface;
  published
  end;
{$ENDIF}


procedure Register;

implementation

uses ComObj;

class function CoScriptDriver_.Create: IScriptDriver;
begin
  Result := CreateComObject(CLASS_ScriptDriver_) as IScriptDriver;
end;

class function CoScriptDriver_.CreateRemote(const MachineName: string): IScriptDriver;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ScriptDriver_) as IScriptDriver;
end;

procedure TScriptDriver_.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{C7D258D8-36A7-460E-9DF5-9B30FF4B0B9C}';
    IntfIID:   '{CF487529-3AE8-11D4-9B6D-008048E67FE2}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TScriptDriver_.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IScriptDriver;
  end;
end;

procedure TScriptDriver_.ConnectTo(svrIntf: IScriptDriver);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TScriptDriver_.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TScriptDriver_.GetDefaultInterface: IScriptDriver;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TScriptDriver_.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TScriptDriver_Properties.Create(Self);
{$ENDIF}
end;

destructor TScriptDriver_.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TScriptDriver_.GetServerProperties: TScriptDriver_Properties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TScriptDriver_.Get_ScriptDispatch: IDispatch;
begin
  Result := DefaultInterface.Get_ScriptDispatch;
end;

procedure TScriptDriver_.NewSession(SType: ScriptType);
begin
  DefaultInterface.NewSession(SType);
end;

function  TScriptDriver_.AddInterface(const IntfName: WideString; const Intf: IDispatch): WordBool;
begin
  Result := DefaultInterface.AddInterface(IntfName, Intf);
end;

procedure TScriptDriver_.AddTypeLib(const TypeLibGuid: WideString; dwMajor: Integer; 
                                    dwMinor: Integer);
begin
  DefaultInterface.AddTypeLib(TypeLibGuid, dwMajor, dwMinor);
end;

function  TScriptDriver_.ExecScript(const Text: WideString; out Res: OleVariant; 
                                    out Error: WideString): Integer;
begin
  Result := DefaultInterface.ExecScript(Text, Res, Error);
end;

function  TScriptDriver_.GetScriptDispatch(const ItemName: WideString; out Disp: IDispatch): Integer;
begin
  Result := DefaultInterface.GetScriptDispatch(ItemName, Disp);
end;

function  TScriptDriver_.AddScriptlet(const Code: WideString; const ItemName: WideString; 
                                      const SubItemName: WideString; const EventName: WideString; 
                                      out Name: WideString; out Error: WideString): Integer;
begin
  Result := DefaultInterface.AddScriptlet(Code, ItemName, SubItemName, EventName, Name, Error);
end;

function  TScriptDriver_.CloseSession: WordBool;
begin
  Result := DefaultInterface.CloseSession;
end;

function  TScriptDriver_.Execute(const Text: WideString): WordBool;
begin
  Result := DefaultInterface.Execute(Text);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TScriptDriver_Properties.Create(AServer: TScriptDriver_);
begin
  inherited Create;
  FServer := AServer;
end;

function TScriptDriver_Properties.GetDefaultInterface: IScriptDriver;
begin
  Result := FServer.DefaultInterface;
end;

function  TScriptDriver_Properties.Get_ScriptDispatch: IDispatch;
begin
  Result := DefaultInterface.Get_ScriptDispatch;
end;

{$ENDIF}

procedure Register;
begin
  RegisterComponents('Servers',[TScriptDriver_]);
end;

end.
