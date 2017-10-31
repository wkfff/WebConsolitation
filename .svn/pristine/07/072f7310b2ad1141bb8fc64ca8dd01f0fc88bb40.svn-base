unit Variables_TLB;

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
// File generated on 7/23/2003 2:17:18 PM from Type Library described below.

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
// Type Lib: X:\System\Variables\Variables.tlb (1)
// IID\LCID: {11AAE62B-9CDF-4BE4-BE99-06166E680498}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\System32\STDOLE2.TLB)
//   (2) v4.0 StdVCL, (C:\WINNT\System32\STDVCL40.DLL)
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
  VariablesMajorVersion = 1;
  VariablesMinorVersion = 0;

  LIBID_Variables: TGUID = '{11AAE62B-9CDF-4BE4-BE99-06166E680498}';

  IID_IValuesFM: TGUID = '{EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}';
  IID_IVariablesFM: TGUID = '{0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}';
  CLASS_VariablesFM: TGUID = '{F541F4E6-CAA5-4ABC-9A38-9FF4075BA44F}';
  CLASS_ValuesFM: TGUID = '{78563D5D-6256-4025-A53C-98340F37F28D}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum TValueTypeX
type
  TValueTypeX = TOleEnum;
const
  vtxInteger = $00000000;
  vtxString = $00000001;
  vtxDouble = $00000002;
  vtxData = $00000003;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IValuesFM = interface;
  IValuesFMDisp = dispinterface;
  IVariablesFM = interface;
  IVariablesFMDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  VariablesFM = IVariablesFM;
  ValuesFM = IValuesFM;


// *********************************************************************//
// Interface: IValuesFM
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}
// *********************************************************************//
  IValuesFM = interface(IDispatch)
    ['{EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}']
    function  Get_Values(const Name: WideString): OleVariant; safecall;
    procedure Set_Values(const Name: WideString; Value: OleVariant); safecall;
    function  CheckParam(const Name: WideString): WordBool; safecall;
    procedure SaveToXMLFile(const FileName: WideString); safecall;
    procedure LoadFromXMLFile(const FileName: WideString); safecall;
    procedure SaveToXML(const XMLDOMNode: IDispatch); safecall;
    procedure LoadFromXML(const XMLDOMNode: IDispatch); safecall;
    function  SaveToVariant: OleVariant; safecall;
    procedure LoadFromVariant(Mem: OleVariant); safecall;      
    property Values[const Name: WideString]: OleVariant read Get_Values write Set_Values;
  end;

// *********************************************************************//
// DispIntf:  IValuesFMDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}
// *********************************************************************//
  IValuesFMDisp = dispinterface
    ['{EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}']
    property Values[const Name: WideString]: OleVariant dispid 1;
    function  CheckParam(const Name: WideString): WordBool; dispid 2;
    procedure SaveToXMLFile(const FileName: WideString); dispid 19;
    procedure LoadFromXMLFile(const FileName: WideString); dispid 20;
    procedure SaveToXML(const XMLDOMNode: IDispatch); dispid 8;
    procedure LoadFromXML(const XMLDOMNode: IDispatch); dispid 9;
    function  SaveToVariant: OleVariant; dispid 10;
    procedure LoadFromVariant(Mem: OleVariant); dispid 11;
  end;

// *********************************************************************//
// Interface: IVariablesFM
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}
// *********************************************************************//
  IVariablesFM = interface(IValuesFM)
    ['{0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}']
    procedure Clear; safecall;
    function  Get_Count: Integer; safecall;
    function  Get_Names(Index: Integer): WideString; safecall;
    function  Get_IndexValues(Index: Integer): OleVariant; safecall;
    procedure Set_IndexValues(Index: Integer; Value: OleVariant); safecall;
    procedure PumpData(const Variables: IVariablesFM); safecall;
    function  Delete(const Name: WideString): WordBool; safecall;
    function  SetCapacity(Count: Integer): WordBool; safecall;
    function  Get_ObjectByName(const Name: WideString): IUnknown; safecall;
    procedure Set_ObjectByName(const Name: WideString; const Value: IUnknown); safecall;
    function  Get_Objects(Index: Integer): IUnknown; safecall;
    procedure Set_Objects(Index: Integer; const Value: IUnknown); safecall;
    procedure AddItems(const Items: WideString); safecall;
    procedure AssignData(const Vars: IVariablesFM); safecall;
    function  SaveToVariantEx(Skip: OleVariant; out Mem: OleVariant): WordBool; safecall;
    property Count: Integer read Get_Count;
    property Names[Index: Integer]: WideString read Get_Names;
    property IndexValues[Index: Integer]: OleVariant read Get_IndexValues write Set_IndexValues;
    property ObjectByName[const Name: WideString]: IUnknown read Get_ObjectByName write Set_ObjectByName;
    property Objects[Index: Integer]: IUnknown read Get_Objects write Set_Objects;
  end;

// *********************************************************************//
// DispIntf:  IVariablesFMDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}
// *********************************************************************//
  IVariablesFMDisp = dispinterface
    ['{0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}']
    procedure Clear; dispid 6;
    property Count: Integer readonly dispid 3;
    property Names[Index: Integer]: WideString readonly dispid 4;
    property IndexValues[Index: Integer]: OleVariant dispid 5;
    procedure PumpData(const Variables: IVariablesFM); dispid 7;
    function  Delete(const Name: WideString): WordBool; dispid 12;
    function  SetCapacity(Count: Integer): WordBool; dispid 13;
    property ObjectByName[const Name: WideString]: IUnknown dispid 14;
    property Objects[Index: Integer]: IUnknown dispid 15;
    procedure AddItems(const Items: WideString); dispid 16;
    procedure AssignData(const Vars: IVariablesFM); dispid 17;
    function  SaveToVariantEx(Skip: OleVariant; out Mem: OleVariant): WordBool; dispid 18;
    property Values[const Name: WideString]: OleVariant dispid 1;
    function  CheckParam(const Name: WideString): WordBool; dispid 2;
    procedure SaveToXMLFile(const FileName: WideString); dispid 19;
    procedure LoadFromXMLFile(const FileName: WideString); dispid 20;
    procedure SaveToXML(const XMLDOMNode: IDispatch); dispid 8;
    procedure LoadFromXML(const XMLDOMNode: IDispatch); dispid 9;
    function  SaveToVariant: OleVariant; dispid 10;
    procedure LoadFromVariant(Mem: OleVariant); dispid 11;
  end;

// *********************************************************************//
// The Class CoVariablesFM provides a Create and CreateRemote method to          
// create instances of the default interface IVariablesFM exposed by              
// the CoClass VariablesFM. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoVariablesFM = class
    class function Create: IVariablesFM;
    class function CreateRemote(const MachineName: string): IVariablesFM;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TVariablesFM
// Help String      : 
// Default Interface: IVariablesFM
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TVariablesFMProperties= class;
{$ENDIF}
  TVariablesFM = class(TOleServer)
  private
    FIntf:        IVariablesFM;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TVariablesFMProperties;
    function      GetServerProperties: TVariablesFMProperties;
{$ENDIF}
    function      GetDefaultInterface: IVariablesFM;
  protected
    procedure InitServerData; override;
    function  Get_Values(const Name: WideString): OleVariant;
    procedure Set_Values(const Name: WideString; Value: OleVariant);
    function  Get_Count: Integer;
    function  Get_Names(Index: Integer): WideString;
    function  Get_IndexValues(Index: Integer): OleVariant;
    procedure Set_IndexValues(Index: Integer; Value: OleVariant);
    function  Get_ObjectByName(const Name: WideString): IUnknown;
    procedure Set_ObjectByName(const Name: WideString; const Value: IUnknown);
    function  Get_Objects(Index: Integer): IUnknown;
    procedure Set_Objects(Index: Integer; const Value: IUnknown);
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IVariablesFM);
    procedure Disconnect; override;
    function  CheckParam(const Name: WideString): WordBool;
    procedure SaveToXMLFile(const FileName: WideString);
    procedure LoadFromXMLFile(const FileName: WideString);
    procedure SaveToXML(const XMLDOMNode: IDispatch);
    procedure LoadFromXML(const XMLDOMNode: IDispatch);
    function  SaveToVariant: OleVariant;
    procedure LoadFromVariant(Mem: OleVariant);
    procedure Clear;
    procedure PumpData(const Variables: IVariablesFM);
    function  Delete(const Name: WideString): WordBool;
    function  SetCapacity(Count: Integer): WordBool;
    procedure AddItems(const Items: WideString);
    procedure AssignData(const Vars: IVariablesFM);
    function  SaveToVariantEx(Skip: OleVariant; out Mem: OleVariant): WordBool;
    property  DefaultInterface: IVariablesFM read GetDefaultInterface;
    property Values[const Name: WideString]: OleVariant read Get_Values write Set_Values;
    property Count: Integer read Get_Count;
    property Names[Index: Integer]: WideString read Get_Names;
    property IndexValues[Index: Integer]: OleVariant read Get_IndexValues write Set_IndexValues;
    property ObjectByName[const Name: WideString]: IUnknown read Get_ObjectByName write Set_ObjectByName;
    property Objects[Index: Integer]: IUnknown read Get_Objects write Set_Objects;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TVariablesFMProperties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TVariablesFM
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TVariablesFMProperties = class(TPersistent)
  private
    FServer:    TVariablesFM;
    function    GetDefaultInterface: IVariablesFM;
    constructor Create(AServer: TVariablesFM);
  protected
    function  Get_Values(const Name: WideString): OleVariant;
    procedure Set_Values(const Name: WideString; Value: OleVariant);
    function  Get_Count: Integer;
    function  Get_Names(Index: Integer): WideString;
    function  Get_IndexValues(Index: Integer): OleVariant;
    procedure Set_IndexValues(Index: Integer; Value: OleVariant);
    function  Get_ObjectByName(const Name: WideString): IUnknown;
    procedure Set_ObjectByName(const Name: WideString; const Value: IUnknown);
    function  Get_Objects(Index: Integer): IUnknown;
    procedure Set_Objects(Index: Integer; const Value: IUnknown);
  public
    property DefaultInterface: IVariablesFM read GetDefaultInterface;
  published
  end;
{$ENDIF}


// *********************************************************************//
// The Class CoValuesFM provides a Create and CreateRemote method to          
// create instances of the default interface IValuesFM exposed by              
// the CoClass ValuesFM. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoValuesFM = class
    class function Create: IValuesFM;
    class function CreateRemote(const MachineName: string): IValuesFM;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TValuesFM
// Help String      : 
// Default Interface: IValuesFM
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TValuesFMProperties= class;
{$ENDIF}
  TValuesFM = class(TOleServer)
  private
    FIntf:        IValuesFM;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TValuesFMProperties;
    function      GetServerProperties: TValuesFMProperties;
{$ENDIF}
    function      GetDefaultInterface: IValuesFM;
  protected
    procedure InitServerData; override;
    function  Get_Values(const Name: WideString): OleVariant;
    procedure Set_Values(const Name: WideString; Value: OleVariant);
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IValuesFM);
    procedure Disconnect; override;
    function  CheckParam(const Name: WideString): WordBool;
    procedure SaveToXMLFile(const FileName: WideString);
    procedure LoadFromXMLFile(const FileName: WideString);
    procedure SaveToXML(const XMLDOMNode: IDispatch);
    procedure LoadFromXML(const XMLDOMNode: IDispatch);
    function  SaveToVariant: OleVariant;
    procedure LoadFromVariant(Mem: OleVariant);
    property  DefaultInterface: IValuesFM read GetDefaultInterface;
    property Values[const Name: WideString]: OleVariant read Get_Values write Set_Values;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TValuesFMProperties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TValuesFM
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TValuesFMProperties = class(TPersistent)
  private
    FServer:    TValuesFM;
    function    GetDefaultInterface: IValuesFM;
    constructor Create(AServer: TValuesFM);
  protected
    function  Get_Values(const Name: WideString): OleVariant;
    procedure Set_Values(const Name: WideString; Value: OleVariant);
  public
    property DefaultInterface: IValuesFM read GetDefaultInterface;
  published
  end;
{$ENDIF}


procedure Register;

implementation

uses ComObj;

class function CoVariablesFM.Create: IVariablesFM;
begin
  Result := CreateComObject(CLASS_VariablesFM) as IVariablesFM;
end;

class function CoVariablesFM.CreateRemote(const MachineName: string): IVariablesFM;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_VariablesFM) as IVariablesFM;
end;

procedure TVariablesFM.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{F541F4E6-CAA5-4ABC-9A38-9FF4075BA44F}';
    IntfIID:   '{0F11D7CD-8F55-4A3F-87F0-D3DFCF3580BF}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TVariablesFM.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IVariablesFM;
  end;
end;

procedure TVariablesFM.ConnectTo(svrIntf: IVariablesFM);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TVariablesFM.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TVariablesFM.GetDefaultInterface: IVariablesFM;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TVariablesFM.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TVariablesFMProperties.Create(Self);
{$ENDIF}
end;

destructor TVariablesFM.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TVariablesFM.GetServerProperties: TVariablesFMProperties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TVariablesFM.Get_Values(const Name: WideString): OleVariant;
begin
  Result := DefaultInterface.Get_Values(Name);
end;

procedure TVariablesFM.Set_Values(const Name: WideString; Value: OleVariant);
begin
  DefaultInterface.Set_Values(Name, Value);
end;

function  TVariablesFM.Get_Count: Integer;
begin
  Result := DefaultInterface.Get_Count;
end;

function  TVariablesFM.Get_Names(Index: Integer): WideString;
begin
  Result := DefaultInterface.Get_Names(Index);
end;

function  TVariablesFM.Get_IndexValues(Index: Integer): OleVariant;
begin
  Result := DefaultInterface.Get_IndexValues(Index);
end;

procedure TVariablesFM.Set_IndexValues(Index: Integer; Value: OleVariant);
begin
  DefaultInterface.Set_IndexValues(Index, Value);
end;

function  TVariablesFM.Get_ObjectByName(const Name: WideString): IUnknown;
begin
  Result := DefaultInterface.Get_ObjectByName(Name);
end;

procedure TVariablesFM.Set_ObjectByName(const Name: WideString; const Value: IUnknown);
begin
  DefaultInterface.Set_ObjectByName(Name, Value);
end;

function  TVariablesFM.Get_Objects(Index: Integer): IUnknown;
begin
  Result := DefaultInterface.Get_Objects(Index);
end;

procedure TVariablesFM.Set_Objects(Index: Integer; const Value: IUnknown);
begin
  DefaultInterface.Set_Objects(Index, Value);
end;

function  TVariablesFM.CheckParam(const Name: WideString): WordBool;
begin
  Result := DefaultInterface.CheckParam(Name);
end;

procedure TVariablesFM.SaveToXMLFile(const FileName: WideString);
begin
  DefaultInterface.SaveToXMLFile(FileName);
end;

procedure TVariablesFM.LoadFromXMLFile(const FileName: WideString);
begin
  DefaultInterface.LoadFromXMLFile(FileName);
end;

procedure TVariablesFM.SaveToXML(const XMLDOMNode: IDispatch);
begin
  DefaultInterface.SaveToXML(XMLDOMNode);
end;

procedure TVariablesFM.LoadFromXML(const XMLDOMNode: IDispatch);
begin
  DefaultInterface.LoadFromXML(XMLDOMNode);
end;

function  TVariablesFM.SaveToVariant: OleVariant;
begin
  Result := DefaultInterface.SaveToVariant;
end;

procedure TVariablesFM.LoadFromVariant(Mem: OleVariant);
begin
  DefaultInterface.LoadFromVariant(Mem);
end;

procedure TVariablesFM.Clear;
begin
  DefaultInterface.Clear;
end;

procedure TVariablesFM.PumpData(const Variables: IVariablesFM);
begin
  DefaultInterface.PumpData(Variables);
end;

function  TVariablesFM.Delete(const Name: WideString): WordBool;
begin
  Result := DefaultInterface.Delete(Name);
end;

function  TVariablesFM.SetCapacity(Count: Integer): WordBool;
begin
  Result := DefaultInterface.SetCapacity(Count);
end;

procedure TVariablesFM.AddItems(const Items: WideString);
begin
  DefaultInterface.AddItems(Items);
end;

procedure TVariablesFM.AssignData(const Vars: IVariablesFM);
begin
  DefaultInterface.AssignData(Vars);
end;

function  TVariablesFM.SaveToVariantEx(Skip: OleVariant; out Mem: OleVariant): WordBool;
begin
  Result := DefaultInterface.SaveToVariantEx(Skip, Mem);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TVariablesFMProperties.Create(AServer: TVariablesFM);
begin
  inherited Create;
  FServer := AServer;
end;

function TVariablesFMProperties.GetDefaultInterface: IVariablesFM;
begin
  Result := FServer.DefaultInterface;
end;

function  TVariablesFMProperties.Get_Values(const Name: WideString): OleVariant;
begin
  Result := DefaultInterface.Get_Values(Name);
end;

procedure TVariablesFMProperties.Set_Values(const Name: WideString; Value: OleVariant);
begin
  DefaultInterface.Set_Values(Name, Value);
end;

function  TVariablesFMProperties.Get_Count: Integer;
begin
  Result := DefaultInterface.Get_Count;
end;

function  TVariablesFMProperties.Get_Names(Index: Integer): WideString;
begin
  Result := DefaultInterface.Get_Names(Index);
end;

function  TVariablesFMProperties.Get_IndexValues(Index: Integer): OleVariant;
begin
  Result := DefaultInterface.Get_IndexValues(Index);
end;

procedure TVariablesFMProperties.Set_IndexValues(Index: Integer; Value: OleVariant);
begin
  DefaultInterface.Set_IndexValues(Index, Value);
end;

function  TVariablesFMProperties.Get_ObjectByName(const Name: WideString): IUnknown;
begin
  Result := DefaultInterface.Get_ObjectByName(Name);
end;

procedure TVariablesFMProperties.Set_ObjectByName(const Name: WideString; const Value: IUnknown);
begin
  DefaultInterface.Set_ObjectByName(Name, Value);
end;

function  TVariablesFMProperties.Get_Objects(Index: Integer): IUnknown;
begin
  Result := DefaultInterface.Get_Objects(Index);
end;

procedure TVariablesFMProperties.Set_Objects(Index: Integer; const Value: IUnknown);
begin
  DefaultInterface.Set_Objects(Index, Value);
end;

{$ENDIF}

class function CoValuesFM.Create: IValuesFM;
begin
  Result := CreateComObject(CLASS_ValuesFM) as IValuesFM;
end;

class function CoValuesFM.CreateRemote(const MachineName: string): IValuesFM;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ValuesFM) as IValuesFM;
end;

procedure TValuesFM.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{78563D5D-6256-4025-A53C-98340F37F28D}';
    IntfIID:   '{EF34F9D4-86D8-4EAD-9598-214F32F8B8B4}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TValuesFM.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IValuesFM;
  end;
end;

procedure TValuesFM.ConnectTo(svrIntf: IValuesFM);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TValuesFM.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TValuesFM.GetDefaultInterface: IValuesFM;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TValuesFM.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TValuesFMProperties.Create(Self);
{$ENDIF}
end;

destructor TValuesFM.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TValuesFM.GetServerProperties: TValuesFMProperties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TValuesFM.Get_Values(const Name: WideString): OleVariant;
begin
  Result := DefaultInterface.Get_Values(Name);
end;

procedure TValuesFM.Set_Values(const Name: WideString; Value: OleVariant);
begin
  DefaultInterface.Set_Values(Name, Value);
end;

function  TValuesFM.CheckParam(const Name: WideString): WordBool;
begin
  Result := DefaultInterface.CheckParam(Name);
end;

procedure TValuesFM.SaveToXMLFile(const FileName: WideString);
begin
  DefaultInterface.SaveToXMLFile(FileName);
end;

procedure TValuesFM.LoadFromXMLFile(const FileName: WideString);
begin
  DefaultInterface.LoadFromXMLFile(FileName);
end;

procedure TValuesFM.SaveToXML(const XMLDOMNode: IDispatch);
begin
  DefaultInterface.SaveToXML(XMLDOMNode);
end;

procedure TValuesFM.LoadFromXML(const XMLDOMNode: IDispatch);
begin
  DefaultInterface.LoadFromXML(XMLDOMNode);
end;

function  TValuesFM.SaveToVariant: OleVariant;
begin
  Result := DefaultInterface.SaveToVariant;
end;

procedure TValuesFM.LoadFromVariant(Mem: OleVariant);
begin
  DefaultInterface.LoadFromVariant(Mem);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TValuesFMProperties.Create(AServer: TValuesFM);
begin
  inherited Create;
  FServer := AServer;
end;

function TValuesFMProperties.GetDefaultInterface: IValuesFM;
begin
  Result := FServer.DefaultInterface;
end;

function  TValuesFMProperties.Get_Values(const Name: WideString): OleVariant;
begin
  Result := DefaultInterface.Get_Values(Name);
end;

procedure TValuesFMProperties.Set_Values(const Name: WideString; Value: OleVariant);
begin
  DefaultInterface.Set_Values(Name, Value);
end;

{$ENDIF}

procedure Register;
begin
  RegisterComponents('Servers',[TVariablesFM, TValuesFM]);
end;

end.
