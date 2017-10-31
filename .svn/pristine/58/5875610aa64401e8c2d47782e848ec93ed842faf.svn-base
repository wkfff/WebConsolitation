unit Krista_FM_PlaningProviderCOMWrapper_TLB;

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
// File generated on 23.05.2011 15:42:01 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\UserTools\Planing\Global\Krista.FM.PlaningProviderCOMWrapper.tlb (1)
// IID\LCID: {04DB24FD-686F-4854-A597-E065E1EAA812}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v2.0 mscorlib, (C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb)
//   (3) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  mscorlib_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  Krista_FM_PlaningProviderCOMWrapperMajorVersion = 3;
  Krista_FM_PlaningProviderCOMWrapperMinorVersion = 1;

  LIBID_Krista_FM_PlaningProviderCOMWrapper: TGUID = '{04DB24FD-686F-4854-A597-E065E1EAA812}';

  IID_IPlaningProviderCOMWrapper: TGUID = '{8E723ADA-D71E-39C6-A370-7EF57EF1F29F}';
  CLASS_PlaningProviderComWrapper: TGUID = '{0D590FFF-4333-4781-8896-1B023F05F5CE}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IPlaningProviderCOMWrapper = interface;
  IPlaningProviderCOMWrapperDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  PlaningProviderComWrapper = IPlaningProviderCOMWrapper;


// *********************************************************************//
// Interface: IPlaningProviderCOMWrapper
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8E723ADA-D71E-39C6-A370-7EF57EF1F29F}
// *********************************************************************//
  IPlaningProviderCOMWrapper = interface(IDispatch)
    ['{8E723ADA-D71E-39C6-A370-7EF57EF1F29F}']
    function  Connect(const serverNameNPort: WideString; const userName: WideString; 
                      const password: WideString; authType: Integer; withinTaskContext: WordBool; 
                      var errStr: WideString): WordBool; safecall;
    procedure Disconnect; safecall;
    function  Get_Connected: WordBool; safecall;
    function  GetSchemeName(const providerId: WideString): WideString; safecall;
    function  Writeback(const data: WideString): WideString; safecall;
    function  GetObjectRecordset(const objectName: WideString; const filter: WideString): OleVariant; safecall;
    function  GetMetadataDate: WideString; safecall;
    function  GetMetaData: WideString; safecall;
    function  GetMembers(const providerId: WideString; const cubeName: WideString; 
                         const dimensionName: WideString; const hierarchyName: WideString; 
                         const levelNames: WideString; const memberPropertiesNames: WideString): WideString; safecall;
    function  GetRecordsetData(const providerId: WideString; const queryText: WideString): WideString; safecall;
    function  GetCellsetData(const providerId: WideString; const queryText: WideString): WideString; safecall;
    procedure RefreshDimension(const providerId: WideString; names: PSafeArray); safecall;
    procedure RefreshCube(const providerId: WideString; names: PSafeArray); safecall;
    function  RefreshMetaData: WideString; safecall;
    function  UpdateTaskParams(taskId: Integer; const paramsText: WideString; 
                               const sectionDivider: WideString; const valuesDivider: WideString): WideString; safecall;
    function  UpdateTaskConsts(taskId: Integer; const constsText: WideString; 
                               const sectionDivider: WideString; const valuesDivider: WideString): WideString; safecall;
    function  GetTaskContext(taskId: Integer): WideString; safecall;
    procedure Dispose; safecall;
    property Connected: WordBool read Get_Connected;
  end;

// *********************************************************************//
// DispIntf:  IPlaningProviderCOMWrapperDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8E723ADA-D71E-39C6-A370-7EF57EF1F29F}
// *********************************************************************//
  IPlaningProviderCOMWrapperDisp = dispinterface
    ['{8E723ADA-D71E-39C6-A370-7EF57EF1F29F}']
    function  Connect(const serverNameNPort: WideString; const userName: WideString; 
                      const password: WideString; authType: Integer; withinTaskContext: WordBool; 
                      var errStr: WideString): WordBool; dispid 1610743808;
    procedure Disconnect; dispid 1610743809;
    property Connected: WordBool readonly dispid 1610743810;
    function  GetSchemeName(const providerId: WideString): WideString; dispid 1610743811;
    function  Writeback(const data: WideString): WideString; dispid 1610743812;
    function  GetObjectRecordset(const objectName: WideString; const filter: WideString): OleVariant; dispid 1610743813;
    function  GetMetadataDate: WideString; dispid 1610743814;
    function  GetMetaData: WideString; dispid 1610743815;
    function  GetMembers(const providerId: WideString; const cubeName: WideString; 
                         const dimensionName: WideString; const hierarchyName: WideString; 
                         const levelNames: WideString; const memberPropertiesNames: WideString): WideString; dispid 1610743816;
    function  GetRecordsetData(const providerId: WideString; const queryText: WideString): WideString; dispid 1610743817;
    function  GetCellsetData(const providerId: WideString; const queryText: WideString): WideString; dispid 1610743818;
    procedure RefreshDimension(const providerId: WideString; names: {??PSafeArray} OleVariant); dispid 1610743819;
    procedure RefreshCube(const providerId: WideString; names: {??PSafeArray} OleVariant); dispid 1610743820;
    function  RefreshMetaData: WideString; dispid 1610743821;
    function  UpdateTaskParams(taskId: Integer; const paramsText: WideString; 
                               const sectionDivider: WideString; const valuesDivider: WideString): WideString; dispid 1610743822;
    function  UpdateTaskConsts(taskId: Integer; const constsText: WideString; 
                               const sectionDivider: WideString; const valuesDivider: WideString): WideString; dispid 1610743823;
    function  GetTaskContext(taskId: Integer): WideString; dispid 1610743824;
    procedure Dispose; dispid 1610743825;
  end;

// *********************************************************************//
// The Class CoPlaningProviderComWrapper provides a Create and CreateRemote method to          
// create instances of the default interface IPlaningProviderCOMWrapper exposed by              
// the CoClass PlaningProviderComWrapper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPlaningProviderComWrapper = class
    class function Create: IPlaningProviderCOMWrapper;
    class function CreateRemote(const MachineName: string): IPlaningProviderCOMWrapper;
  end;

implementation

uses ComObj;

class function CoPlaningProviderComWrapper.Create: IPlaningProviderCOMWrapper;
begin
  Result := CreateComObject(CLASS_PlaningProviderComWrapper) as IPlaningProviderCOMWrapper;
end;

class function CoPlaningProviderComWrapper.CreateRemote(const MachineName: string): IPlaningProviderCOMWrapper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PlaningProviderComWrapper) as IPlaningProviderCOMWrapper;
end;

end.
