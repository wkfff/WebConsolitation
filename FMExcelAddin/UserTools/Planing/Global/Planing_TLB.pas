unit Planing_TLB;

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
// File generated on 09.11.2010 14:23:49 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\UserTools\Planing\Global\Planing.tlb (1)
// IID\LCID: {910CC719-16D1-403C-9D57-E254AA61EACD}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v1.0 PlaningTools, (y:\Bin\PlaningTools.dll)
//   (3) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  PlaningTools_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  PlaningMajorVersion = 1;
  PlaningMinorVersion = 0;

  LIBID_Planing: TGUID = '{910CC719-16D1-403C-9D57-E254AA61EACD}';

  IID_IFMPlanningExtension: TGUID = '{8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D}';
  IID_IFMPlanningAncillary: TGUID = '{17B449CC-4A4B-4063-B1F9-AFE4AE7EC9D3}';
  IID_IFMPlanningVBProgramming: TGUID = '{EA7169F4-C478-4BD5-9F58-0C0682F50621}';
  IID_IFMPlaningInteraction: TGUID = '{285B6260-CE19-4142-AF42-A40143D0B91D}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum EventType
type
  EventType = TOleEnum;
const
  etEdit = $00000000;
  etRefresh = $00000001;
  etWriteback = $00000002;
  etPropertyEdit = $00000003;
  etUnknown = $00000004;
  etVersionUpdate = $00000005;
  etSheetCopy = $00000006;
  etParamsEdit = $00000007;
  etOnTaskConnection = $00000008;
  etTaskConnectionOff = $00000009;
  etConstEdit = $0000000A;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IFMPlanningExtension = interface;
  IFMPlanningExtensionDisp = dispinterface;
  IFMPlanningAncillary = interface;
  IFMPlanningAncillaryDisp = dispinterface;
  IFMPlanningVBProgramming = interface;
  IFMPlanningVBProgrammingDisp = dispinterface;
  IFMPlaningInteraction = interface;
  IFMPlaningInteractionDisp = dispinterface;

// *********************************************************************//
// Interface: IFMPlanningExtension
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D}
// *********************************************************************//
  IFMPlanningExtension = interface(IDispatch)
    ['{8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D}']
    function  GetPropValueByName(const PropName: WideString): WideString; safecall;
    procedure SetPropValueByName(const PropName: WideString; const PropValue: WideString); safecall;
    function  SetConnectionStr(const URL: WideString; const Scheme: WideString): HResult; safecall;
    procedure SetTaskContext(const taskContext: IDispatch); safecall;
    function  Get_IsSilentMode: WordBool; safecall;
    procedure Set_IsSilentMode(Value: WordBool); safecall;
    function  Get_ProcessForm: IProcessForm; safecall;
    procedure Set_ProcessForm(const Value: IProcessForm); safecall;
    procedure OnTaskConnection(IsConnected: WordBool); safecall;
    function  Get_IsLoadingFromTask: WordBool; safecall;
    procedure Set_IsLoadingFromTask(Value: WordBool); safecall;
    function  RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; safecall;
    function  RefreshActiveSheet: WordBool; safecall;
    function  WritebackActiveSheet: WordBool; safecall;
    function  RefreshActiveBook: WordBool; safecall;
    function  WritebackActiveBook: WordBool; safecall;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); safecall;
    function  WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;
    function  WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; safecall;
    procedure AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType; 
                                   const Comment: WideString; Success: WordBool); safecall;
    property IsSilentMode: WordBool read Get_IsSilentMode write Set_IsSilentMode;
    property ProcessForm: IProcessForm read Get_ProcessForm write Set_ProcessForm;
    property IsLoadingFromTask: WordBool read Get_IsLoadingFromTask write Set_IsLoadingFromTask;
  end;

// *********************************************************************//
// DispIntf:  IFMPlanningExtensionDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D}
// *********************************************************************//
  IFMPlanningExtensionDisp = dispinterface
    ['{8BE1EEBC-9FE3-4CC3-8B78-66AE4C41892D}']
    function  GetPropValueByName(const PropName: WideString): WideString; dispid 1;
    procedure SetPropValueByName(const PropName: WideString; const PropValue: WideString); dispid 2;
    function  SetConnectionStr(const URL: WideString; const Scheme: WideString): HResult; dispid 3;
    procedure SetTaskContext(const taskContext: IDispatch); dispid 4;
    property IsSilentMode: WordBool dispid 10;
    property ProcessForm: IProcessForm dispid 6;
    procedure OnTaskConnection(IsConnected: WordBool); dispid 5;
    property IsLoadingFromTask: WordBool dispid 7;
    function  RefreshSheet(Index: OleVariant; out IsAccessVioletion: WordBool): WordBool; dispid 8;
    function  RefreshActiveSheet: WordBool; dispid 9;
    function  WritebackActiveSheet: WordBool; dispid 11;
    function  RefreshActiveBook: WordBool; dispid 12;
    function  WritebackActiveBook: WordBool; dispid 13;
    procedure SetAuthenticationInfo(AuthType: SYSINT; const Login: WideString; 
                                    const PwdHash: WideString); dispid 14;
    function  WritebackActiveSheetEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; dispid 15;
    function  WritebackActiveBookEx(EraseEmptyCells: WordBool; ProcessCube: WordBool): WordBool; dispid 16;
    procedure AddSheetHistoryEvent(const Doc: IDispatch; EType: EventType; 
                                   const Comment: WideString; Success: WordBool); dispid 17;
  end;

// *********************************************************************//
// Interface: IFMPlanningAncillary
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {17B449CC-4A4B-4063-B1F9-AFE4AE7EC9D3}
// *********************************************************************//
  IFMPlanningAncillary = interface(IDispatch)
    ['{17B449CC-4A4B-4063-B1F9-AFE4AE7EC9D3}']
    function  QueryRange(const ACaption: WideString; const APrompt: WideString; 
                         AllowMultiArea: WordBool): IDispatch; safecall;
  end;

// *********************************************************************//
// DispIntf:  IFMPlanningAncillaryDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {17B449CC-4A4B-4063-B1F9-AFE4AE7EC9D3}
// *********************************************************************//
  IFMPlanningAncillaryDisp = dispinterface
    ['{17B449CC-4A4B-4063-B1F9-AFE4AE7EC9D3}']
    function  QueryRange(const ACaption: WideString; const APrompt: WideString; 
                         AllowMultiArea: WordBool): IDispatch; dispid 1;
  end;

// *********************************************************************//
// Interface: IFMPlanningVBProgramming
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EA7169F4-C478-4BD5-9F58-0C0682F50621}
// *********************************************************************//
  IFMPlanningVBProgramming = interface(IDispatch)
    ['{EA7169F4-C478-4BD5-9F58-0C0682F50621}']
    function  VBGetPropertyByName(const PropertyName: WideString): WideString; safecall;
    procedure VBSetPropertyByName(const PropertyName: WideString; const PropertyValue: WideString); safecall;
    function  VBGetConstValueByName(const ConstName: WideString): WideString; safecall;
    function  VBSetConstValueByName(const ConstName: WideString; const ConstValue: WideString): WordBool; safecall;
    function  VBRefresh: WordBool; safecall;
    function  VBWriteback: WordBool; safecall;
    function  VBGetCurrentConnection(var URL: WideString; var SchemeName: WideString): WordBool; safecall;
    function  VBEditMembers(const DimensionName: WideString): WordBool; safecall;
    function  VBGetMemberProperty(const DimensionName: WideString; const UniqueName: WideString; 
                                  const MemberPropertyName: WideString): WideString; safecall;
    function  VBGetMembers(const DimensionName: WideString): OleVariant; safecall;
    procedure VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant); safecall;
    function  VBGetParamMembers(const ParamName: WideString): OleVariant; safecall;
    procedure VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant); safecall;
    function  VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString; safecall;
    function  VBGetSingleCellValue(const SingleCellName: WideString): WideString; safecall;
  end;

// *********************************************************************//
// DispIntf:  IFMPlanningVBProgrammingDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EA7169F4-C478-4BD5-9F58-0C0682F50621}
// *********************************************************************//
  IFMPlanningVBProgrammingDisp = dispinterface
    ['{EA7169F4-C478-4BD5-9F58-0C0682F50621}']
    function  VBGetPropertyByName(const PropertyName: WideString): WideString; dispid 1;
    procedure VBSetPropertyByName(const PropertyName: WideString; const PropertyValue: WideString); dispid 2;
    function  VBGetConstValueByName(const ConstName: WideString): WideString; dispid 3;
    function  VBSetConstValueByName(const ConstName: WideString; const ConstValue: WideString): WordBool; dispid 4;
    function  VBRefresh: WordBool; dispid 5;
    function  VBWriteback: WordBool; dispid 6;
    function  VBGetCurrentConnection(var URL: WideString; var SchemeName: WideString): WordBool; dispid 8;
    function  VBEditMembers(const DimensionName: WideString): WordBool; dispid 11;
    function  VBGetMemberProperty(const DimensionName: WideString; const UniqueName: WideString; 
                                  const MemberPropertyName: WideString): WideString; dispid 12;
    function  VBGetMembers(const DimensionName: WideString): OleVariant; dispid 13;
    procedure VBSetMembers(const DimensionName: WideString; UniqueNames: OleVariant); dispid 14;
    function  VBGetParamMembers(const ParamName: WideString): OleVariant; dispid 15;
    procedure VBSetParamMembers(const ParamName: WideString; UniqueNames: OleVariant); dispid 16;
    function  VBGetTotalValue(const TotalName: WideString; Coordinates: OleVariant): WideString; dispid 7;
    function  VBGetSingleCellValue(const SingleCellName: WideString): WideString; dispid 9;
  end;

// *********************************************************************//
// Interface: IFMPlaningInteraction
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {285B6260-CE19-4142-AF42-A40143D0B91D}
// *********************************************************************//
  IFMPlaningInteraction = interface(IDispatch)
    ['{285B6260-CE19-4142-AF42-A40143D0B91D}']
    procedure SetTaskContext(const TaskContextXml: WideString; IsPacked: WordBool); safecall;
  end;

// *********************************************************************//
// DispIntf:  IFMPlaningInteractionDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {285B6260-CE19-4142-AF42-A40143D0B91D}
// *********************************************************************//
  IFMPlaningInteractionDisp = dispinterface
    ['{285B6260-CE19-4142-AF42-A40143D0B91D}']
    procedure SetTaskContext(const TaskContextXml: WideString; IsPacked: WordBool); dispid 1;
  end;

implementation

uses ComObj;

end.
