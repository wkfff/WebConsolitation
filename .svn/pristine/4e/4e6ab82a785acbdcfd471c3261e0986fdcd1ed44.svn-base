unit ScriptManager_TLB;

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
// File generated on 11.06.2003 15:14:24 from Type Library described below.

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
// Type Lib: X:\System\ScriptManager\ScriptManager.tlb (1)
// IID\LCID: {EB5AFAB4-C4C9-4317-BBD0-E75FD241164E}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\System32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINNT\System32\stdvcl40.dll)
//   (3) v3.0 MSXML2, (C:\WINNT\System32\msxml3.dll)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  MSXML2_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  ScriptManagerMajorVersion = 1;
  ScriptManagerMinorVersion = 0;

  LIBID_ScriptManager: TGUID = '{EB5AFAB4-C4C9-4317-BBD0-E75FD241164E}';

  IID_IScriptManager: TGUID = '{2D40C725-F81B-494F-87DE-9C05919E608B}';
  IID_IScript: TGUID = '{FACE5FBA-8581-4603-8AF3-E8DB2A8DC3E9}';
  CLASS_ScriptManager_: TGUID = '{443FC574-39CE-401D-9FA4-C606CDB25090}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum TScriptType
type
  TScriptType = TOleEnum;
const
  stXML = $00000000;
  stHTML = $00000001;
  stJava = $00000002;
  stUndefined = $00000003;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IScriptManager = interface;
  IScriptManagerDisp = dispinterface;
  IScript = interface;
  IScriptDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ScriptManager_ = IScriptManager;


// *********************************************************************//
// Interface: IScriptManager
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {2D40C725-F81B-494F-87DE-9C05919E608B}
// *********************************************************************//
  IScriptManager = interface(IDispatch)
    ['{2D40C725-F81B-494F-87DE-9C05919E608B}']
    procedure AddScript(ScriptType: TScriptType; out Script: IScript); safecall;
    procedure DelScriptByName(const ScriptName: WideString; var ScriptType: TScriptType); safecall;
    function  Get_ParsedAll: WordBool; safecall;
    function  Get_ScriptCount(ScriptType: TScriptType): Integer; safecall;
    procedure Clear; safecall;
    procedure LoadFromXML(const FileName: WideString); safecall;
    property ParsedAll: WordBool read Get_ParsedAll;
    property ScriptCount[ScriptType: TScriptType]: Integer read Get_ScriptCount;
  end;

// *********************************************************************//
// DispIntf:  IScriptManagerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {2D40C725-F81B-494F-87DE-9C05919E608B}
// *********************************************************************//
  IScriptManagerDisp = dispinterface
    ['{2D40C725-F81B-494F-87DE-9C05919E608B}']
    procedure AddScript(ScriptType: TScriptType; out Script: IScript); dispid 1;
    procedure DelScriptByName(const ScriptName: WideString; var ScriptType: TScriptType); dispid 2;
    property ParsedAll: WordBool readonly dispid 4;
    property ScriptCount[ScriptType: TScriptType]: Integer readonly dispid 6;
    procedure Clear; dispid 8;
    procedure LoadFromXML(const FileName: WideString); dispid 3;
  end;

// *********************************************************************//
// Interface: IScript
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {FACE5FBA-8581-4603-8AF3-E8DB2A8DC3E9}
// *********************************************************************//
  IScript = interface(IDispatch)
    ['{FACE5FBA-8581-4603-8AF3-E8DB2A8DC3E9}']
    function  Get_ScriptName: WideString; safecall;
    procedure Set_ScriptName(const Value: WideString); safecall;
    function  Get_ScriptType: TScriptType; safecall;
    function  Get_Parsed: WordBool; safecall;
    function  Get_Script: WideString; safecall;
    procedure Set_Script(const Value: WideString); safecall;
    function  Get_ID: Integer; safecall;
    property ScriptName: WideString read Get_ScriptName write Set_ScriptName;
    property ScriptType: TScriptType read Get_ScriptType;
    property Parsed: WordBool read Get_Parsed;
    property Script: WideString read Get_Script write Set_Script;
    property ID: Integer read Get_ID;
  end;

// *********************************************************************//
// DispIntf:  IScriptDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {FACE5FBA-8581-4603-8AF3-E8DB2A8DC3E9}
// *********************************************************************//
  IScriptDisp = dispinterface
    ['{FACE5FBA-8581-4603-8AF3-E8DB2A8DC3E9}']
    property ScriptName: WideString dispid 1;
    property ScriptType: TScriptType readonly dispid 3;
    property Parsed: WordBool readonly dispid 5;
    property Script: WideString dispid 7;
    property ID: Integer readonly dispid 2;
  end;

// *********************************************************************//
// The Class CoScriptManager_ provides a Create and CreateRemote method to          
// create instances of the default interface IScriptManager exposed by              
// the CoClass ScriptManager_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoScriptManager_ = class
    class function Create: IScriptManager;
    class function CreateRemote(const MachineName: string): IScriptManager;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TScriptManager_
// Help String      : 
// Default Interface: IScriptManager
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TScriptManager_Properties= class;
{$ENDIF}
  TScriptManager_ = class(TOleServer)
  private
    FIntf:        IScriptManager;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TScriptManager_Properties;
    function      GetServerProperties: TScriptManager_Properties;
{$ENDIF}
    function      GetDefaultInterface: IScriptManager;
  protected
    procedure InitServerData; override;
    function  Get_ParsedAll: WordBool;
    function  Get_ScriptCount(ScriptType: TScriptType): Integer;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IScriptManager);
    procedure Disconnect; override;
    procedure AddScript(ScriptType: TScriptType; out Script: IScript);
    procedure DelScriptByName(const ScriptName: WideString; var ScriptType: TScriptType);
    procedure Clear;
    procedure LoadFromXML(const FileName: WideString);
    property  DefaultInterface: IScriptManager read GetDefaultInterface;
    property ParsedAll: WordBool read Get_ParsedAll;
    property ScriptCount[ScriptType: TScriptType]: Integer read Get_ScriptCount;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TScriptManager_Properties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TScriptManager_
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TScriptManager_Properties = class(TPersistent)
  private
    FServer:    TScriptManager_;
    function    GetDefaultInterface: IScriptManager;
    constructor Create(AServer: TScriptManager_);
  protected
    function  Get_ParsedAll: WordBool;
    function  Get_ScriptCount(ScriptType: TScriptType): Integer;
  public
    property DefaultInterface: IScriptManager read GetDefaultInterface;
  published
  end;
{$ENDIF}


procedure Register;

implementation

uses ComObj;

class function CoScriptManager_.Create: IScriptManager;
begin
  Result := CreateComObject(CLASS_ScriptManager_) as IScriptManager;
end;

class function CoScriptManager_.CreateRemote(const MachineName: string): IScriptManager;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ScriptManager_) as IScriptManager;
end;

procedure TScriptManager_.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{443FC574-39CE-401D-9FA4-C606CDB25090}';
    IntfIID:   '{2D40C725-F81B-494F-87DE-9C05919E608B}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TScriptManager_.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IScriptManager;
  end;
end;

procedure TScriptManager_.ConnectTo(svrIntf: IScriptManager);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TScriptManager_.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TScriptManager_.GetDefaultInterface: IScriptManager;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TScriptManager_.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TScriptManager_Properties.Create(Self);
{$ENDIF}
end;

destructor TScriptManager_.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TScriptManager_.GetServerProperties: TScriptManager_Properties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TScriptManager_.Get_ParsedAll: WordBool;
begin
  Result := DefaultInterface.Get_ParsedAll;
end;

function  TScriptManager_.Get_ScriptCount(ScriptType: TScriptType): Integer;
begin
  Result := DefaultInterface.Get_ScriptCount(ScriptType);
end;

procedure TScriptManager_.AddScript(ScriptType: TScriptType; out Script: IScript);
begin
  DefaultInterface.AddScript(ScriptType, Script);
end;

procedure TScriptManager_.DelScriptByName(const ScriptName: WideString; var ScriptType: TScriptType);
begin
  DefaultInterface.DelScriptByName(ScriptName, ScriptType);
end;

procedure TScriptManager_.Clear;
begin
  DefaultInterface.Clear;
end;

procedure TScriptManager_.LoadFromXML(const FileName: WideString);
begin
  DefaultInterface.LoadFromXML(FileName);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TScriptManager_Properties.Create(AServer: TScriptManager_);
begin
  inherited Create;
  FServer := AServer;
end;

function TScriptManager_Properties.GetDefaultInterface: IScriptManager;
begin
  Result := FServer.DefaultInterface;
end;

function  TScriptManager_Properties.Get_ParsedAll: WordBool;
begin
  Result := DefaultInterface.Get_ParsedAll;
end;

function  TScriptManager_Properties.Get_ScriptCount(ScriptType: TScriptType): Integer;
begin
  Result := DefaultInterface.Get_ScriptCount(ScriptType);
end;

{$ENDIF}

procedure Register;
begin
  RegisterComponents('Servers',[TScriptManager_]);
end;

end.
