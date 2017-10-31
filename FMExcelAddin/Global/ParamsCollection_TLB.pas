unit ParamsCollection_TLB;

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
// File generated on 25.12.2007 20:30:50 from Type Library described below.

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
// Type Lib: X:\System\ParamsCollection\ParamsCollection.tlb (1)
// IID\LCID: {5845FB18-BDC6-4DAB-AC0A-B558B557556F}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (D:\WINDOWS\system32\stdole2.tlb)
//   (2) v3.0 MSXML2, (D:\WINDOWS\system32\msxml3.dll)
//   (3) v4.0 StdVCL, (D:\WINDOWS\system32\STDVCL40.DLL)
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
  ParamsCollectionMajorVersion = 1;
  ParamsCollectionMinorVersion = 0;

  LIBID_ParamsCollection: TGUID = '{5845FB18-BDC6-4DAB-AC0A-B558B557556F}';

  IID_IParam: TGUID = '{7B7CE343-BAA9-4C6A-9619-A23C2C466450}';
  IID_IParamsCollection: TGUID = '{5DD72270-1790-48B8-90EB-3B97BD148C82}';
  CLASS_ParamsCollection_: TGUID = '{F711A33C-7B7A-4B55-ABBC-CC33F1BECF33}';
  CLASS_Param: TGUID = '{9A64493C-1274-4325-8917-352B65CB3C82}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum TValueType
type
  TValueType = TOleEnum;
const
  vtInteger = $00000000;
  vtDouble = $00000001;
  vtBoolean = $00000002;
  vtString = $00000003;
  vtDate = $00000004;

// Constants for enum TParamType
type
  TParamType = TOleEnum;
const
  ptSingle = $00000000;
  ptList = $00000001;
  ptDimensionList = $00000002;
  ptCheckBox = $00000003;
  ptGroupBox = $00000004;
  ptRange = $00000005;

// Constants for enum TOwnerType
type
  TOwnerType = TOleEnum;
const
  otFM = $00000000;
  otBudget = $00000001;
  otBoth = $00000002;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IParam = interface;
  IParamDisp = dispinterface;
  IParamsCollection = interface;
  IParamsCollectionDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ParamsCollection_ = IParamsCollection;
  Param = IParam;


// *********************************************************************//
// Interface: IParam
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {7B7CE343-BAA9-4C6A-9619-A23C2C466450}
// *********************************************************************//
  IParam = interface(IDispatch)
    ['{7B7CE343-BAA9-4C6A-9619-A23C2C466450}']
    function  Get_ParamName: WideString; safecall;
    procedure Set_ParamName(const Value: WideString); safecall;
    function  Get_ParamCaption: WideString; safecall;
    procedure Set_ParamCaption(const Value: WideString); safecall;
    function  Get_ValueType: TValueType; safecall;
    procedure Set_ValueType(Value: TValueType); safecall;
    function  Get_ParamType: TParamType; safecall;
    procedure Set_ParamType(Value: TParamType); safecall;
    function  Get_Value: OleVariant; safecall;
    procedure Set_Value(Value: OleVariant); safecall;
    function  Get_ParamScriptName: WideString; safecall;
    procedure Set_ParamScriptName(const Value: WideString); safecall;
    function  Get_Active: WordBool; safecall;
    procedure Set_Active(Value: WordBool); safecall;
    function  Get_ParentCollection: IParamsCollection; safecall;
    procedure Set_ParentCollection(const Value: IParamsCollection); safecall;
    function  Clone(const ipParentCollection: IParamsCollection): IParam; safecall;
    function  Get_ParamID: Integer; safecall;
    procedure Set_ParamID(Value: Integer); safecall;
    function  Get_OwnerType: TOwnerType; safecall;
    procedure Set_OwnerType(Value: TOwnerType); safecall;
    procedure LoadData(const Node: IXMLDOMNode); safecall;
    function  LoadPredefined(const Node: IXMLDOMNode): WordBool; safecall;
    function  Get_ElementsCount: Integer; safecall;
    procedure Element(Index: Integer; out Name: OleVariant; out ID: OleVariant; 
                      out Level: OleVariant; out Key: OleVariant; out LevelName: OleVariant); safecall;
    function  Get_MDXValue: WideString; safecall;
    function  FormattedValue(WithKey: WordBool): WideString; safecall;
    function  MDXKey: WideString; safecall;
    function  SetActiveElements(const Elements: IXMLDOMNodeList): WordBool; safecall;
    function  FillEnumeration(EnumerationCollection: OleVariant; const LvlName: WideString): WordBool; safecall;
    function  Get_LowLevel: WideString; safecall;
    procedure Set_LowLevel(const Value: WideString); safecall;
    function  SetActiveElement(ElementID: Integer): WordBool; safecall;
    function  Get_IsActiveElement(ID: Integer): WordBool; safecall;
    procedure ResetActiveElements; safecall;
    function  Get_Key: OleVariant; safecall;
    procedure Set_Key(Value: OleVariant); safecall;
    function  SetActiveElementByName(const ElementName: WideString): WordBool; safecall;
    function  Get_DimensionName: WideString; safecall;
    procedure Set_DimensionName(const Value: WideString); safecall;
    function  Get_ParamInnerName: WideString; safecall;
    procedure Set_ParamInnerName(const Value: WideString); safecall;
    procedure MakeSingleSelect; safecall;
    function  Get_PropByName(const PropName: WideString): WideString; safecall;
    function  Get_PropsCount: Integer; safecall;
    function  Get_PropNameByInd(PropInd: Integer): WideString; safecall;
    function  Get_PropValByInd(PropInd: Integer): WideString; safecall;
    function  Get_HierarchyName: WideString; safecall;
    procedure Set_HierarchyName(const Value: WideString); safecall;
    function  Get_Multiple: WordBool; safecall;
    procedure Set_Multiple(Value: WordBool); safecall;
    function  ElementMDXValue(Index: Integer): WideString; safecall;
    property ParamName: WideString read Get_ParamName write Set_ParamName;
    property ParamCaption: WideString read Get_ParamCaption write Set_ParamCaption;
    property ValueType: TValueType read Get_ValueType write Set_ValueType;
    property ParamType: TParamType read Get_ParamType write Set_ParamType;
    property Value: OleVariant read Get_Value write Set_Value;
    property ParamScriptName: WideString read Get_ParamScriptName write Set_ParamScriptName;
    property Active: WordBool read Get_Active write Set_Active;
    property ParentCollection: IParamsCollection read Get_ParentCollection write Set_ParentCollection;
    property ParamID: Integer read Get_ParamID write Set_ParamID;
    property OwnerType: TOwnerType read Get_OwnerType write Set_OwnerType;
    property ElementsCount: Integer read Get_ElementsCount;
    property MDXValue: WideString read Get_MDXValue;
    property LowLevel: WideString read Get_LowLevel write Set_LowLevel;
    property IsActiveElement[ID: Integer]: WordBool read Get_IsActiveElement;
    property Key: OleVariant read Get_Key write Set_Key;
    property DimensionName: WideString read Get_DimensionName write Set_DimensionName;
    property ParamInnerName: WideString read Get_ParamInnerName write Set_ParamInnerName;
    property PropByName[const PropName: WideString]: WideString read Get_PropByName;
    property PropsCount: Integer read Get_PropsCount;
    property PropNameByInd[PropInd: Integer]: WideString read Get_PropNameByInd;
    property PropValByInd[PropInd: Integer]: WideString read Get_PropValByInd;
    property HierarchyName: WideString read Get_HierarchyName write Set_HierarchyName;
    property Multiple: WordBool read Get_Multiple write Set_Multiple;
  end;

// *********************************************************************//
// DispIntf:  IParamDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {7B7CE343-BAA9-4C6A-9619-A23C2C466450}
// *********************************************************************//
  IParamDisp = dispinterface
    ['{7B7CE343-BAA9-4C6A-9619-A23C2C466450}']
    property ParamName: WideString dispid 28;
    property ParamCaption: WideString dispid 29;
    property ValueType: TValueType dispid 30;
    property ParamType: TParamType dispid 31;
    property Value: OleVariant dispid 33;
    property ParamScriptName: WideString dispid 34;
    property Active: WordBool dispid 35;
    property ParentCollection: IParamsCollection dispid 37;
    function  Clone(const ipParentCollection: IParamsCollection): IParam; dispid 38;
    property ParamID: Integer dispid 40;
    property OwnerType: TOwnerType dispid 41;
    procedure LoadData(const Node: IXMLDOMNode); dispid 1;
    function  LoadPredefined(const Node: IXMLDOMNode): WordBool; dispid 3;
    property ElementsCount: Integer readonly dispid 2;
    procedure Element(Index: Integer; out Name: OleVariant; out ID: OleVariant; 
                      out Level: OleVariant; out Key: OleVariant; out LevelName: OleVariant); dispid 4;
    property MDXValue: WideString readonly dispid 5;
    function  FormattedValue(WithKey: WordBool): WideString; dispid 14;
    function  MDXKey: WideString; dispid 15;
    function  SetActiveElements(const Elements: IXMLDOMNodeList): WordBool; dispid 6;
    function  FillEnumeration(EnumerationCollection: OleVariant; const LvlName: WideString): WordBool; dispid 7;
    property LowLevel: WideString dispid 8;
    function  SetActiveElement(ElementID: Integer): WordBool; dispid 9;
    property IsActiveElement[ID: Integer]: WordBool readonly dispid 10;
    procedure ResetActiveElements; dispid 11;
    property Key: OleVariant dispid 12;
    function  SetActiveElementByName(const ElementName: WideString): WordBool; dispid 13;
    property DimensionName: WideString dispid 16;
    property ParamInnerName: WideString dispid 17;
    procedure MakeSingleSelect; dispid 18;
    property PropByName[const PropName: WideString]: WideString readonly dispid 19;
    property PropsCount: Integer readonly dispid 20;
    property PropNameByInd[PropInd: Integer]: WideString readonly dispid 21;
    property PropValByInd[PropInd: Integer]: WideString readonly dispid 23;
    property HierarchyName: WideString dispid 22;
    property Multiple: WordBool dispid 24;
    function  ElementMDXValue(Index: Integer): WideString; dispid 25;
  end;

// *********************************************************************//
// Interface: IParamsCollection
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5DD72270-1790-48B8-90EB-3B97BD148C82}
// *********************************************************************//
  IParamsCollection = interface(IDispatch)
    ['{5DD72270-1790-48B8-90EB-3B97BD148C82}']
    procedure OnStartPage(const AScriptingContext: IUnknown); safecall;
    procedure OnEndPage; safecall;
    function  Get_ParamsCount: Integer; safecall;
    function  Get_Params(Index: Integer): IParam; safecall;
    procedure LoadFromFile(const FileName: WideString; out Success: OleVariant); safecall;
    procedure Clear; safecall;
    procedure ClearParams; safecall;
    procedure Clone(var ParamsCollection: IParamsCollection; out Success: OleVariant); safecall;
    function  ParamByID(ParamID: Integer): IParam; safecall;
    function  ParamByName(const ParamName: WideString): IParam; safecall;
    function  ParamByCaption(const ParamCaption: WideString): IParam; safecall;
    procedure Set_ScriptManager(const Param1: IDispatch); safecall;
    function  Get_GetData: IDispatch; safecall;
    procedure Set_GetData(const Value: IDispatch); safecall;
    function  Get_ParentCollection: IParamsCollection; safecall;
    procedure Set_ParentCollection(const Value: IParamsCollection); safecall;
    procedure Reloaded(out Success: OleVariant); safecall;
    procedure AddParam(const Param: IParam); safecall;
    procedure DeleteChildCollection(const ChildCollection: IUnknown); safecall;
    function  Get_LastError: WideString; safecall;
    function  Get_GetDataItf: IDispatch; safecall;
    property ParamsCount: Integer read Get_ParamsCount;
    property Params[Index: Integer]: IParam read Get_Params;
    property ScriptManager: IDispatch write Set_ScriptManager;
    property GetData: IDispatch read Get_GetData write Set_GetData;
    property ParentCollection: IParamsCollection read Get_ParentCollection write Set_ParentCollection;
    property LastError: WideString read Get_LastError;
    property GetDataItf: IDispatch read Get_GetDataItf;
  end;

// *********************************************************************//
// DispIntf:  IParamsCollectionDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {5DD72270-1790-48B8-90EB-3B97BD148C82}
// *********************************************************************//
  IParamsCollectionDisp = dispinterface
    ['{5DD72270-1790-48B8-90EB-3B97BD148C82}']
    procedure OnStartPage(const AScriptingContext: IUnknown); dispid 1;
    procedure OnEndPage; dispid 2;
    property ParamsCount: Integer readonly dispid 667;
    property Params[Index: Integer]: IParam readonly dispid 7;
    procedure LoadFromFile(const FileName: WideString; out Success: OleVariant); dispid 11;
    procedure Clear; dispid 12;
    procedure ClearParams; dispid 10;
    procedure Clone(var ParamsCollection: IParamsCollection; out Success: OleVariant); dispid 13;
    function  ParamByID(ParamID: Integer): IParam; dispid 15;
    function  ParamByName(const ParamName: WideString): IParam; dispid 14;
    function  ParamByCaption(const ParamCaption: WideString): IParam; dispid 17;
    property ScriptManager: IDispatch writeonly dispid 16;
    property GetData: IDispatch dispid 3;
    property ParentCollection: IParamsCollection dispid 4;
    procedure Reloaded(out Success: OleVariant); dispid 6;
    procedure AddParam(const Param: IParam); dispid 8;
    procedure DeleteChildCollection(const ChildCollection: IUnknown); dispid 9;
    property LastError: WideString readonly dispid 5;
    property GetDataItf: IDispatch readonly dispid 18;
  end;

// *********************************************************************//
// The Class CoParamsCollection_ provides a Create and CreateRemote method to          
// create instances of the default interface IParamsCollection exposed by              
// the CoClass ParamsCollection_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoParamsCollection_ = class
    class function Create: IParamsCollection;
    class function CreateRemote(const MachineName: string): IParamsCollection;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TParamsCollection_
// Help String      : ParamsCollection Object
// Default Interface: IParamsCollection
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TParamsCollection_Properties= class;
{$ENDIF}
  TParamsCollection_ = class(TOleServer)
  private
    FIntf:        IParamsCollection;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TParamsCollection_Properties;
    function      GetServerProperties: TParamsCollection_Properties;
{$ENDIF}
    function      GetDefaultInterface: IParamsCollection;
  protected
    procedure InitServerData; override;
    function  Get_ParamsCount: Integer;
    function  Get_Params(Index: Integer): IParam;
    procedure Set_ScriptManager(const Param1: IDispatch);
    function  Get_GetData: IDispatch;
    procedure Set_GetData(const Value: IDispatch);
    function  Get_ParentCollection: IParamsCollection;
    procedure Set_ParentCollection(const Value: IParamsCollection);
    function  Get_LastError: WideString;
    function  Get_GetDataItf: IDispatch;
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IParamsCollection);
    procedure Disconnect; override;
    procedure OnStartPage(const AScriptingContext: IUnknown);
    procedure OnEndPage;
    procedure LoadFromFile(const FileName: WideString; out Success: OleVariant);
    procedure Clear;
    procedure ClearParams;
    procedure Clone(var ParamsCollection: IParamsCollection; out Success: OleVariant);
    function  ParamByID(ParamID: Integer): IParam;
    function  ParamByName(const ParamName: WideString): IParam;
    function  ParamByCaption(const ParamCaption: WideString): IParam;
    procedure Reloaded(out Success: OleVariant);
    procedure AddParam(const Param: IParam);
    procedure DeleteChildCollection(const ChildCollection: IUnknown);
    property  DefaultInterface: IParamsCollection read GetDefaultInterface;
    property ParamsCount: Integer read Get_ParamsCount;
    property Params[Index: Integer]: IParam read Get_Params;
    property ScriptManager: IDispatch write Set_ScriptManager;
    property GetData: IDispatch read Get_GetData write Set_GetData;
    property ParentCollection: IParamsCollection read Get_ParentCollection write Set_ParentCollection;
    property LastError: WideString read Get_LastError;
    property GetDataItf: IDispatch read Get_GetDataItf;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TParamsCollection_Properties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TParamsCollection_
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TParamsCollection_Properties = class(TPersistent)
  private
    FServer:    TParamsCollection_;
    function    GetDefaultInterface: IParamsCollection;
    constructor Create(AServer: TParamsCollection_);
  protected
    function  Get_ParamsCount: Integer;
    function  Get_Params(Index: Integer): IParam;
    procedure Set_ScriptManager(const Param1: IDispatch);
    function  Get_GetData: IDispatch;
    procedure Set_GetData(const Value: IDispatch);
    function  Get_ParentCollection: IParamsCollection;
    procedure Set_ParentCollection(const Value: IParamsCollection);
    function  Get_LastError: WideString;
    function  Get_GetDataItf: IDispatch;
  public
    property DefaultInterface: IParamsCollection read GetDefaultInterface;
  published
  end;
{$ENDIF}


// *********************************************************************//
// The Class CoParam provides a Create and CreateRemote method to          
// create instances of the default interface IParam exposed by              
// the CoClass Param. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoParam = class
    class function Create: IParam;
    class function CreateRemote(const MachineName: string): IParam;
  end;


// *********************************************************************//
// OLE Server Proxy class declaration
// Server Object    : TParam
// Help String      : 
// Default Interface: IParam
// Def. Intf. DISP? : No
// Event   Interface: 
// TypeFlags        : (2) CanCreate
// *********************************************************************//
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  TParamProperties= class;
{$ENDIF}
  TParam = class(TOleServer)
  private
    FIntf:        IParam;
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    FProps:       TParamProperties;
    function      GetServerProperties: TParamProperties;
{$ENDIF}
    function      GetDefaultInterface: IParam;
  protected
    procedure InitServerData; override;
    function  Get_ParamName: WideString;
    procedure Set_ParamName(const Value: WideString);
    function  Get_ParamCaption: WideString;
    procedure Set_ParamCaption(const Value: WideString);
    function  Get_ValueType: TValueType;
    procedure Set_ValueType(Value: TValueType);
    function  Get_ParamType: TParamType;
    procedure Set_ParamType(Value: TParamType);
    function  Get_Value: OleVariant;
    procedure Set_Value(Value: OleVariant);
    function  Get_ParamScriptName: WideString;
    procedure Set_ParamScriptName(const Value: WideString);
    function  Get_Active: WordBool;
    procedure Set_Active(Value: WordBool);
    function  Get_ParentCollection: IParamsCollection;
    procedure Set_ParentCollection(const Value: IParamsCollection);
    function  Get_ParamID: Integer;
    procedure Set_ParamID(Value: Integer);
    function  Get_OwnerType: TOwnerType;
    procedure Set_OwnerType(Value: TOwnerType);
    function  Get_ElementsCount: Integer;
    function  Get_MDXValue: WideString;
    function  Get_LowLevel: WideString;
    procedure Set_LowLevel(const Value: WideString);
    function  Get_IsActiveElement(ID: Integer): WordBool;
    function  Get_Key: OleVariant;
    procedure Set_Key(Value: OleVariant);
    function  Get_DimensionName: WideString;
    procedure Set_DimensionName(const Value: WideString);
    function  Get_ParamInnerName: WideString;
    procedure Set_ParamInnerName(const Value: WideString);
    function  Get_PropByName(const PropName: WideString): WideString;
    function  Get_PropsCount: Integer;
    function  Get_PropNameByInd(PropInd: Integer): WideString;
    function  Get_PropValByInd(PropInd: Integer): WideString;
    function  Get_HierarchyName: WideString;
    procedure Set_HierarchyName(const Value: WideString);
    function  Get_Multiple: WordBool;
    procedure Set_Multiple(Value: WordBool);
  public
    constructor Create(AOwner: TComponent); override;
    destructor  Destroy; override;
    procedure Connect; override;
    procedure ConnectTo(svrIntf: IParam);
    procedure Disconnect; override;
    function  Clone(const ipParentCollection: IParamsCollection): IParam;
    procedure LoadData(const Node: IXMLDOMNode);
    function  LoadPredefined(const Node: IXMLDOMNode): WordBool;
    procedure Element(Index: Integer; out Name: OleVariant; out ID: OleVariant; 
                      out Level: OleVariant; out Key: OleVariant; out LevelName: OleVariant);
    function  FormattedValue(WithKey: WordBool): WideString;
    function  MDXKey: WideString;
    function  SetActiveElements(const Elements: IXMLDOMNodeList): WordBool;
    function  FillEnumeration(EnumerationCollection: OleVariant; const LvlName: WideString): WordBool;
    function  SetActiveElement(ElementID: Integer): WordBool;
    procedure ResetActiveElements;
    function  SetActiveElementByName(const ElementName: WideString): WordBool;
    procedure MakeSingleSelect;
    function  ElementMDXValue(Index: Integer): WideString;
    property  DefaultInterface: IParam read GetDefaultInterface;
    property Value: OleVariant read Get_Value write Set_Value;
    property ParamID: Integer read Get_ParamID write Set_ParamID;
    property ElementsCount: Integer read Get_ElementsCount;
    property MDXValue: WideString read Get_MDXValue;
    property IsActiveElement[ID: Integer]: WordBool read Get_IsActiveElement;
    property Key: OleVariant read Get_Key write Set_Key;
    property PropByName[const PropName: WideString]: WideString read Get_PropByName;
    property PropsCount: Integer read Get_PropsCount;
    property PropNameByInd[PropInd: Integer]: WideString read Get_PropNameByInd;
    property PropValByInd[PropInd: Integer]: WideString read Get_PropValByInd;
    property ParamName: WideString read Get_ParamName write Set_ParamName;
    property ParamCaption: WideString read Get_ParamCaption write Set_ParamCaption;
    property ValueType: TValueType read Get_ValueType write Set_ValueType;
    property ParamType: TParamType read Get_ParamType write Set_ParamType;
    property ParamScriptName: WideString read Get_ParamScriptName write Set_ParamScriptName;
    property Active: WordBool read Get_Active write Set_Active;
    property ParentCollection: IParamsCollection read Get_ParentCollection write Set_ParentCollection;
    property OwnerType: TOwnerType read Get_OwnerType write Set_OwnerType;
    property LowLevel: WideString read Get_LowLevel write Set_LowLevel;
    property DimensionName: WideString read Get_DimensionName write Set_DimensionName;
    property ParamInnerName: WideString read Get_ParamInnerName write Set_ParamInnerName;
    property HierarchyName: WideString read Get_HierarchyName write Set_HierarchyName;
    property Multiple: WordBool read Get_Multiple write Set_Multiple;
  published
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
    property Server: TParamProperties read GetServerProperties;
{$ENDIF}
  end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
// *********************************************************************//
// OLE Server Properties Proxy Class
// Server Object    : TParam
// (This object is used by the IDE's Property Inspector to allow editing
//  of the properties of this server)
// *********************************************************************//
 TParamProperties = class(TPersistent)
  private
    FServer:    TParam;
    function    GetDefaultInterface: IParam;
    constructor Create(AServer: TParam);
  protected
    function  Get_ParamName: WideString;
    procedure Set_ParamName(const Value: WideString);
    function  Get_ParamCaption: WideString;
    procedure Set_ParamCaption(const Value: WideString);
    function  Get_ValueType: TValueType;
    procedure Set_ValueType(Value: TValueType);
    function  Get_ParamType: TParamType;
    procedure Set_ParamType(Value: TParamType);
    function  Get_Value: OleVariant;
    procedure Set_Value(Value: OleVariant);
    function  Get_ParamScriptName: WideString;
    procedure Set_ParamScriptName(const Value: WideString);
    function  Get_Active: WordBool;
    procedure Set_Active(Value: WordBool);
    function  Get_ParentCollection: IParamsCollection;
    procedure Set_ParentCollection(const Value: IParamsCollection);
    function  Get_ParamID: Integer;
    procedure Set_ParamID(Value: Integer);
    function  Get_OwnerType: TOwnerType;
    procedure Set_OwnerType(Value: TOwnerType);
    function  Get_ElementsCount: Integer;
    function  Get_MDXValue: WideString;
    function  Get_LowLevel: WideString;
    procedure Set_LowLevel(const Value: WideString);
    function  Get_IsActiveElement(ID: Integer): WordBool;
    function  Get_Key: OleVariant;
    procedure Set_Key(Value: OleVariant);
    function  Get_DimensionName: WideString;
    procedure Set_DimensionName(const Value: WideString);
    function  Get_ParamInnerName: WideString;
    procedure Set_ParamInnerName(const Value: WideString);
    function  Get_PropByName(const PropName: WideString): WideString;
    function  Get_PropsCount: Integer;
    function  Get_PropNameByInd(PropInd: Integer): WideString;
    function  Get_PropValByInd(PropInd: Integer): WideString;
    function  Get_HierarchyName: WideString;
    procedure Set_HierarchyName(const Value: WideString);
    function  Get_Multiple: WordBool;
    procedure Set_Multiple(Value: WordBool);
  public
    property DefaultInterface: IParam read GetDefaultInterface;
  published
    property ParamName: WideString read Get_ParamName write Set_ParamName;
    property ParamCaption: WideString read Get_ParamCaption write Set_ParamCaption;
    property ValueType: TValueType read Get_ValueType write Set_ValueType;
    property ParamType: TParamType read Get_ParamType write Set_ParamType;
    property ParamScriptName: WideString read Get_ParamScriptName write Set_ParamScriptName;
    property Active: WordBool read Get_Active write Set_Active;
    property ParentCollection: IParamsCollection read Get_ParentCollection write Set_ParentCollection;
    property OwnerType: TOwnerType read Get_OwnerType write Set_OwnerType;
    property LowLevel: WideString read Get_LowLevel write Set_LowLevel;
    property DimensionName: WideString read Get_DimensionName write Set_DimensionName;
    property ParamInnerName: WideString read Get_ParamInnerName write Set_ParamInnerName;
    property HierarchyName: WideString read Get_HierarchyName write Set_HierarchyName;
    property Multiple: WordBool read Get_Multiple write Set_Multiple;
  end;
{$ENDIF}


procedure Register;

implementation

uses ComObj;

class function CoParamsCollection_.Create: IParamsCollection;
begin
  Result := CreateComObject(CLASS_ParamsCollection_) as IParamsCollection;
end;

class function CoParamsCollection_.CreateRemote(const MachineName: string): IParamsCollection;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ParamsCollection_) as IParamsCollection;
end;

procedure TParamsCollection_.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{F711A33C-7B7A-4B55-ABBC-CC33F1BECF33}';
    IntfIID:   '{5DD72270-1790-48B8-90EB-3B97BD148C82}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TParamsCollection_.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IParamsCollection;
  end;
end;

procedure TParamsCollection_.ConnectTo(svrIntf: IParamsCollection);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TParamsCollection_.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TParamsCollection_.GetDefaultInterface: IParamsCollection;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TParamsCollection_.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TParamsCollection_Properties.Create(Self);
{$ENDIF}
end;

destructor TParamsCollection_.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TParamsCollection_.GetServerProperties: TParamsCollection_Properties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TParamsCollection_.Get_ParamsCount: Integer;
begin
  Result := DefaultInterface.Get_ParamsCount;
end;

function  TParamsCollection_.Get_Params(Index: Integer): IParam;
begin
  Result := DefaultInterface.Get_Params(Index);
end;

procedure TParamsCollection_.Set_ScriptManager(const Param1: IDispatch);
begin
  DefaultInterface.Set_ScriptManager(Param1);
end;

function  TParamsCollection_.Get_GetData: IDispatch;
begin
  Result := DefaultInterface.Get_GetData;
end;

procedure TParamsCollection_.Set_GetData(const Value: IDispatch);
begin
  DefaultInterface.Set_GetData(Value);
end;

function  TParamsCollection_.Get_ParentCollection: IParamsCollection;
begin
  Result := DefaultInterface.Get_ParentCollection;
end;

procedure TParamsCollection_.Set_ParentCollection(const Value: IParamsCollection);
begin
  DefaultInterface.Set_ParentCollection(Value);
end;

function  TParamsCollection_.Get_LastError: WideString;
begin
  Result := DefaultInterface.Get_LastError;
end;

function  TParamsCollection_.Get_GetDataItf: IDispatch;
begin
  Result := DefaultInterface.Get_GetDataItf;
end;

procedure TParamsCollection_.OnStartPage(const AScriptingContext: IUnknown);
begin
  DefaultInterface.OnStartPage(AScriptingContext);
end;

procedure TParamsCollection_.OnEndPage;
begin
  DefaultInterface.OnEndPage;
end;

procedure TParamsCollection_.LoadFromFile(const FileName: WideString; out Success: OleVariant);
begin
  DefaultInterface.LoadFromFile(FileName, Success);
end;

procedure TParamsCollection_.Clear;
begin
  DefaultInterface.Clear;
end;

procedure TParamsCollection_.ClearParams;
begin
  DefaultInterface.ClearParams;
end;

procedure TParamsCollection_.Clone(var ParamsCollection: IParamsCollection; out Success: OleVariant);
begin
  DefaultInterface.Clone(ParamsCollection, Success);
end;

function  TParamsCollection_.ParamByID(ParamID: Integer): IParam;
begin
  Result := DefaultInterface.ParamByID(ParamID);
end;

function  TParamsCollection_.ParamByName(const ParamName: WideString): IParam;
begin
  Result := DefaultInterface.ParamByName(ParamName);
end;

function  TParamsCollection_.ParamByCaption(const ParamCaption: WideString): IParam;
begin
  Result := DefaultInterface.ParamByCaption(ParamCaption);
end;

procedure TParamsCollection_.Reloaded(out Success: OleVariant);
begin
  DefaultInterface.Reloaded(Success);
end;

procedure TParamsCollection_.AddParam(const Param: IParam);
begin
  DefaultInterface.AddParam(Param);
end;

procedure TParamsCollection_.DeleteChildCollection(const ChildCollection: IUnknown);
begin
  DefaultInterface.DeleteChildCollection(ChildCollection);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TParamsCollection_Properties.Create(AServer: TParamsCollection_);
begin
  inherited Create;
  FServer := AServer;
end;

function TParamsCollection_Properties.GetDefaultInterface: IParamsCollection;
begin
  Result := FServer.DefaultInterface;
end;

function  TParamsCollection_Properties.Get_ParamsCount: Integer;
begin
  Result := DefaultInterface.Get_ParamsCount;
end;

function  TParamsCollection_Properties.Get_Params(Index: Integer): IParam;
begin
  Result := DefaultInterface.Get_Params(Index);
end;

procedure TParamsCollection_Properties.Set_ScriptManager(const Param1: IDispatch);
begin
  DefaultInterface.Set_ScriptManager(Param1);
end;

function  TParamsCollection_Properties.Get_GetData: IDispatch;
begin
  Result := DefaultInterface.Get_GetData;
end;

procedure TParamsCollection_Properties.Set_GetData(const Value: IDispatch);
begin
  DefaultInterface.Set_GetData(Value);
end;

function  TParamsCollection_Properties.Get_ParentCollection: IParamsCollection;
begin
  Result := DefaultInterface.Get_ParentCollection;
end;

procedure TParamsCollection_Properties.Set_ParentCollection(const Value: IParamsCollection);
begin
  DefaultInterface.Set_ParentCollection(Value);
end;

function  TParamsCollection_Properties.Get_LastError: WideString;
begin
  Result := DefaultInterface.Get_LastError;
end;

function  TParamsCollection_Properties.Get_GetDataItf: IDispatch;
begin
  Result := DefaultInterface.Get_GetDataItf;
end;

{$ENDIF}

class function CoParam.Create: IParam;
begin
  Result := CreateComObject(CLASS_Param) as IParam;
end;

class function CoParam.CreateRemote(const MachineName: string): IParam;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Param) as IParam;
end;

procedure TParam.InitServerData;
const
  CServerData: TServerData = (
    ClassID:   '{9A64493C-1274-4325-8917-352B65CB3C82}';
    IntfIID:   '{7B7CE343-BAA9-4C6A-9619-A23C2C466450}';
    EventIID:  '';
    LicenseKey: nil;
    Version: 500);
begin
  ServerData := @CServerData;
end;

procedure TParam.Connect;
var
  punk: IUnknown;
begin
  if FIntf = nil then
  begin
    punk := GetServer;
    Fintf:= punk as IParam;
  end;
end;

procedure TParam.ConnectTo(svrIntf: IParam);
begin
  Disconnect;
  FIntf := svrIntf;
end;

procedure TParam.DisConnect;
begin
  if Fintf <> nil then
  begin
    FIntf := nil;
  end;
end;

function TParam.GetDefaultInterface: IParam;
begin
  if FIntf = nil then
    Connect;
  Assert(FIntf <> nil, 'DefaultInterface is NULL. Component is not connected to Server. You must call ''Connect'' or ''ConnectTo'' before this operation');
  Result := FIntf;
end;

constructor TParam.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps := TParamProperties.Create(Self);
{$ENDIF}
end;

destructor TParam.Destroy;
begin
{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
  FProps.Free;
{$ENDIF}
  inherited Destroy;
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
function TParam.GetServerProperties: TParamProperties;
begin
  Result := FProps;
end;
{$ENDIF}

function  TParam.Get_ParamName: WideString;
begin
  Result := DefaultInterface.Get_ParamName;
end;

procedure TParam.Set_ParamName(const Value: WideString);
begin
  DefaultInterface.Set_ParamName(Value);
end;

function  TParam.Get_ParamCaption: WideString;
begin
  Result := DefaultInterface.Get_ParamCaption;
end;

procedure TParam.Set_ParamCaption(const Value: WideString);
begin
  DefaultInterface.Set_ParamCaption(Value);
end;

function  TParam.Get_ValueType: TValueType;
begin
  Result := DefaultInterface.Get_ValueType;
end;

procedure TParam.Set_ValueType(Value: TValueType);
begin
  DefaultInterface.Set_ValueType(Value);
end;

function  TParam.Get_ParamType: TParamType;
begin
  Result := DefaultInterface.Get_ParamType;
end;

procedure TParam.Set_ParamType(Value: TParamType);
begin
  DefaultInterface.Set_ParamType(Value);
end;

function  TParam.Get_Value: OleVariant;
begin
  Result := DefaultInterface.Get_Value;
end;

procedure TParam.Set_Value(Value: OleVariant);
begin
  DefaultInterface.Set_Value(Value);
end;

function  TParam.Get_ParamScriptName: WideString;
begin
  Result := DefaultInterface.Get_ParamScriptName;
end;

procedure TParam.Set_ParamScriptName(const Value: WideString);
begin
  DefaultInterface.Set_ParamScriptName(Value);
end;

function  TParam.Get_Active: WordBool;
begin
  Result := DefaultInterface.Get_Active;
end;

procedure TParam.Set_Active(Value: WordBool);
begin
  DefaultInterface.Set_Active(Value);
end;

function  TParam.Get_ParentCollection: IParamsCollection;
begin
  Result := DefaultInterface.Get_ParentCollection;
end;

procedure TParam.Set_ParentCollection(const Value: IParamsCollection);
begin
  DefaultInterface.Set_ParentCollection(Value);
end;

function  TParam.Get_ParamID: Integer;
begin
  Result := DefaultInterface.Get_ParamID;
end;

procedure TParam.Set_ParamID(Value: Integer);
begin
  DefaultInterface.Set_ParamID(Value);
end;

function  TParam.Get_OwnerType: TOwnerType;
begin
  Result := DefaultInterface.Get_OwnerType;
end;

procedure TParam.Set_OwnerType(Value: TOwnerType);
begin
  DefaultInterface.Set_OwnerType(Value);
end;

function  TParam.Get_ElementsCount: Integer;
begin
  Result := DefaultInterface.Get_ElementsCount;
end;

function  TParam.Get_MDXValue: WideString;
begin
  Result := DefaultInterface.Get_MDXValue;
end;

function  TParam.Get_LowLevel: WideString;
begin
  Result := DefaultInterface.Get_LowLevel;
end;

procedure TParam.Set_LowLevel(const Value: WideString);
begin
  DefaultInterface.Set_LowLevel(Value);
end;

function  TParam.Get_IsActiveElement(ID: Integer): WordBool;
begin
  Result := DefaultInterface.Get_IsActiveElement(ID);
end;

function  TParam.Get_Key: OleVariant;
begin
  Result := DefaultInterface.Get_Key;
end;

procedure TParam.Set_Key(Value: OleVariant);
begin
  DefaultInterface.Set_Key(Value);
end;

function  TParam.Get_DimensionName: WideString;
begin
  Result := DefaultInterface.Get_DimensionName;
end;

procedure TParam.Set_DimensionName(const Value: WideString);
begin
  DefaultInterface.Set_DimensionName(Value);
end;

function  TParam.Get_ParamInnerName: WideString;
begin
  Result := DefaultInterface.Get_ParamInnerName;
end;

procedure TParam.Set_ParamInnerName(const Value: WideString);
begin
  DefaultInterface.Set_ParamInnerName(Value);
end;

function  TParam.Get_PropByName(const PropName: WideString): WideString;
begin
  Result := DefaultInterface.Get_PropByName(PropName);
end;

function  TParam.Get_PropsCount: Integer;
begin
  Result := DefaultInterface.Get_PropsCount;
end;

function  TParam.Get_PropNameByInd(PropInd: Integer): WideString;
begin
  Result := DefaultInterface.Get_PropNameByInd(PropInd);
end;

function  TParam.Get_PropValByInd(PropInd: Integer): WideString;
begin
  Result := DefaultInterface.Get_PropValByInd(PropInd);
end;

function  TParam.Get_HierarchyName: WideString;
begin
  Result := DefaultInterface.Get_HierarchyName;
end;

procedure TParam.Set_HierarchyName(const Value: WideString);
begin
  DefaultInterface.Set_HierarchyName(Value);
end;

function  TParam.Get_Multiple: WordBool;
begin
  Result := DefaultInterface.Get_Multiple;
end;

procedure TParam.Set_Multiple(Value: WordBool);
begin
  DefaultInterface.Set_Multiple(Value);
end;

function  TParam.Clone(const ipParentCollection: IParamsCollection): IParam;
begin
  Result := DefaultInterface.Clone(ipParentCollection);
end;

procedure TParam.LoadData(const Node: IXMLDOMNode);
begin
  DefaultInterface.LoadData(Node);
end;

function  TParam.LoadPredefined(const Node: IXMLDOMNode): WordBool;
begin
  Result := DefaultInterface.LoadPredefined(Node);
end;

procedure TParam.Element(Index: Integer; out Name: OleVariant; out ID: OleVariant; 
                         out Level: OleVariant; out Key: OleVariant; out LevelName: OleVariant);
begin
  DefaultInterface.Element(Index, Name, ID, Level, Key, LevelName);
end;

function  TParam.FormattedValue(WithKey: WordBool): WideString;
begin
  Result := DefaultInterface.FormattedValue(WithKey);
end;

function  TParam.MDXKey: WideString;
begin
  Result := DefaultInterface.MDXKey;
end;

function  TParam.SetActiveElements(const Elements: IXMLDOMNodeList): WordBool;
begin
  Result := DefaultInterface.SetActiveElements(Elements);
end;

function  TParam.FillEnumeration(EnumerationCollection: OleVariant; const LvlName: WideString): WordBool;
begin
  Result := DefaultInterface.FillEnumeration(EnumerationCollection, LvlName);
end;

function  TParam.SetActiveElement(ElementID: Integer): WordBool;
begin
  Result := DefaultInterface.SetActiveElement(ElementID);
end;

procedure TParam.ResetActiveElements;
begin
  DefaultInterface.ResetActiveElements;
end;

function  TParam.SetActiveElementByName(const ElementName: WideString): WordBool;
begin
  Result := DefaultInterface.SetActiveElementByName(ElementName);
end;

procedure TParam.MakeSingleSelect;
begin
  DefaultInterface.MakeSingleSelect;
end;

function  TParam.ElementMDXValue(Index: Integer): WideString;
begin
  Result := DefaultInterface.ElementMDXValue(Index);
end;

{$IFDEF LIVE_SERVER_AT_DESIGN_TIME}
constructor TParamProperties.Create(AServer: TParam);
begin
  inherited Create;
  FServer := AServer;
end;

function TParamProperties.GetDefaultInterface: IParam;
begin
  Result := FServer.DefaultInterface;
end;

function  TParamProperties.Get_ParamName: WideString;
begin
  Result := DefaultInterface.Get_ParamName;
end;

procedure TParamProperties.Set_ParamName(const Value: WideString);
begin
  DefaultInterface.Set_ParamName(Value);
end;

function  TParamProperties.Get_ParamCaption: WideString;
begin
  Result := DefaultInterface.Get_ParamCaption;
end;

procedure TParamProperties.Set_ParamCaption(const Value: WideString);
begin
  DefaultInterface.Set_ParamCaption(Value);
end;

function  TParamProperties.Get_ValueType: TValueType;
begin
  Result := DefaultInterface.Get_ValueType;
end;

procedure TParamProperties.Set_ValueType(Value: TValueType);
begin
  DefaultInterface.Set_ValueType(Value);
end;

function  TParamProperties.Get_ParamType: TParamType;
begin
  Result := DefaultInterface.Get_ParamType;
end;

procedure TParamProperties.Set_ParamType(Value: TParamType);
begin
  DefaultInterface.Set_ParamType(Value);
end;

function  TParamProperties.Get_Value: OleVariant;
begin
  Result := DefaultInterface.Get_Value;
end;

procedure TParamProperties.Set_Value(Value: OleVariant);
begin
  DefaultInterface.Set_Value(Value);
end;

function  TParamProperties.Get_ParamScriptName: WideString;
begin
  Result := DefaultInterface.Get_ParamScriptName;
end;

procedure TParamProperties.Set_ParamScriptName(const Value: WideString);
begin
  DefaultInterface.Set_ParamScriptName(Value);
end;

function  TParamProperties.Get_Active: WordBool;
begin
  Result := DefaultInterface.Get_Active;
end;

procedure TParamProperties.Set_Active(Value: WordBool);
begin
  DefaultInterface.Set_Active(Value);
end;

function  TParamProperties.Get_ParentCollection: IParamsCollection;
begin
  Result := DefaultInterface.Get_ParentCollection;
end;

procedure TParamProperties.Set_ParentCollection(const Value: IParamsCollection);
begin
  DefaultInterface.Set_ParentCollection(Value);
end;

function  TParamProperties.Get_ParamID: Integer;
begin
  Result := DefaultInterface.Get_ParamID;
end;

procedure TParamProperties.Set_ParamID(Value: Integer);
begin
  DefaultInterface.Set_ParamID(Value);
end;

function  TParamProperties.Get_OwnerType: TOwnerType;
begin
  Result := DefaultInterface.Get_OwnerType;
end;

procedure TParamProperties.Set_OwnerType(Value: TOwnerType);
begin
  DefaultInterface.Set_OwnerType(Value);
end;

function  TParamProperties.Get_ElementsCount: Integer;
begin
  Result := DefaultInterface.Get_ElementsCount;
end;

function  TParamProperties.Get_MDXValue: WideString;
begin
  Result := DefaultInterface.Get_MDXValue;
end;

function  TParamProperties.Get_LowLevel: WideString;
begin
  Result := DefaultInterface.Get_LowLevel;
end;

procedure TParamProperties.Set_LowLevel(const Value: WideString);
begin
  DefaultInterface.Set_LowLevel(Value);
end;

function  TParamProperties.Get_IsActiveElement(ID: Integer): WordBool;
begin
  Result := DefaultInterface.Get_IsActiveElement(ID);
end;

function  TParamProperties.Get_Key: OleVariant;
begin
  Result := DefaultInterface.Get_Key;
end;

procedure TParamProperties.Set_Key(Value: OleVariant);
begin
  DefaultInterface.Set_Key(Value);
end;

function  TParamProperties.Get_DimensionName: WideString;
begin
  Result := DefaultInterface.Get_DimensionName;
end;

procedure TParamProperties.Set_DimensionName(const Value: WideString);
begin
  DefaultInterface.Set_DimensionName(Value);
end;

function  TParamProperties.Get_ParamInnerName: WideString;
begin
  Result := DefaultInterface.Get_ParamInnerName;
end;

procedure TParamProperties.Set_ParamInnerName(const Value: WideString);
begin
  DefaultInterface.Set_ParamInnerName(Value);
end;

function  TParamProperties.Get_PropByName(const PropName: WideString): WideString;
begin
  Result := DefaultInterface.Get_PropByName(PropName);
end;

function  TParamProperties.Get_PropsCount: Integer;
begin
  Result := DefaultInterface.Get_PropsCount;
end;

function  TParamProperties.Get_PropNameByInd(PropInd: Integer): WideString;
begin
  Result := DefaultInterface.Get_PropNameByInd(PropInd);
end;

function  TParamProperties.Get_PropValByInd(PropInd: Integer): WideString;
begin
  Result := DefaultInterface.Get_PropValByInd(PropInd);
end;

function  TParamProperties.Get_HierarchyName: WideString;
begin
  Result := DefaultInterface.Get_HierarchyName;
end;

procedure TParamProperties.Set_HierarchyName(const Value: WideString);
begin
  DefaultInterface.Set_HierarchyName(Value);
end;

function  TParamProperties.Get_Multiple: WordBool;
begin
  Result := DefaultInterface.Get_Multiple;
end;

procedure TParamProperties.Set_Multiple(Value: WordBool);
begin
  DefaultInterface.Set_Multiple(Value);
end;

{$ENDIF}

procedure Register;
begin
  RegisterComponents('Servers',[TParamsCollection_, TParam]);
end;

end.
