unit Krista_FM_Server_Providers_Planing_TLB;

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
// File generated on 12.10.2009 16:15:41 from Type Library described below.

// ************************************************************************ //
// Type Lib: Y:\debug\Krista.FM.Server\Krista.FM.Server.Providers.Planing.tlb (1)
// IID\LCID: {9BE57B8E-AD8C-404D-B596-FFC2C3776291}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 mscorlib, (c:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\mscorlib.tlb)
//   (2) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
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
  Krista_FM_Server_Providers_PlaningMajorVersion = 2;
  Krista_FM_Server_Providers_PlaningMinorVersion = 6;

  LIBID_Krista_FM_Server_Providers_Planing: TGUID = '{9BE57B8E-AD8C-404D-B596-FFC2C3776291}';

  IID__PlaningProviderCOMWrapper: TGUID = '{0FD9C70E-FE69-3B62-8BA1-7B34A9EF149F}';
  CLASS_PlaningProviderCOMWrapper: TGUID = '{69E0192A-A8CB-36BE-9A7C-8B5A0ED4309B}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  _PlaningProviderCOMWrapper = interface;
  _PlaningProviderCOMWrapperDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  PlaningProviderCOMWrapper = _PlaningProviderCOMWrapper;


// *********************************************************************//
// Interface: _PlaningProviderCOMWrapper
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {0FD9C70E-FE69-3B62-8BA1-7B34A9EF149F}
// *********************************************************************//
  _PlaningProviderCOMWrapper = interface(IDispatch)
    ['{0FD9C70E-FE69-3B62-8BA1-7B34A9EF149F}']
    function  Get_ToString: WideString; safecall;
    function  Equals(obj: OleVariant): WordBool; safecall;
    function  GetHashCode: Integer; safecall;
    function  GetType: _Type; safecall;
    function  Connect(const serverNameNPort: WideString; const userName: WideString; 
                      const password: WideString; authType: Integer; withinTaskContext: WordBool; 
                      var errStr: WideString): WordBool; safecall;
    procedure Disconnect; safecall;
    function  Writeback(const data: WideString): WideString; safecall;
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
    procedure Dispose; safecall;
    function  Get_Connected: WordBool; safecall;
    property ToString: WideString read Get_ToString;
    property Connected: WordBool read Get_Connected;
  end;

// *********************************************************************//
// DispIntf:  _PlaningProviderCOMWrapperDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {0FD9C70E-FE69-3B62-8BA1-7B34A9EF149F}
// *********************************************************************//
  _PlaningProviderCOMWrapperDisp = dispinterface
    ['{0FD9C70E-FE69-3B62-8BA1-7B34A9EF149F}']
    property ToString: WideString readonly dispid 0;
    function  Equals(obj: OleVariant): WordBool; dispid 1610743809;
    function  GetHashCode: Integer; dispid 1610743810;
    function  GetType: _Type; dispid 1610743811;
    function  Connect(const serverNameNPort: WideString; const userName: WideString; 
                      const password: WideString; authType: Integer; withinTaskContext: WordBool; 
                      var errStr: WideString): WordBool; dispid 1610743812;
    procedure Disconnect; dispid 1610743813;
    function  Writeback(const data: WideString): WideString; dispid 1610743814;
    function  GetMetadataDate: WideString; dispid 1610743815;
    function  GetMetaData: WideString; dispid 1610743816;
    function  GetMembers(const providerId: WideString; const cubeName: WideString; 
                         const dimensionName: WideString; const hierarchyName: WideString; 
                         const levelNames: WideString; const memberPropertiesNames: WideString): WideString; dispid 1610743817;
    function  GetRecordsetData(const providerId: WideString; const queryText: WideString): WideString; dispid 1610743818;
    function  GetCellsetData(const providerId: WideString; const queryText: WideString): WideString; dispid 1610743819;
    procedure RefreshDimension(const providerId: WideString; names: {??PSafeArray} OleVariant); dispid 1610743820;
    procedure RefreshCube(const providerId: WideString; names: {??PSafeArray} OleVariant); dispid 1610743821;
    function  RefreshMetaData: WideString; dispid 1610743822;
    procedure Dispose; dispid 1610743823;
    property Connected: WordBool readonly dispid 1610743824;
  end;

// *********************************************************************//
// The Class CoPlaningProviderCOMWrapper provides a Create and CreateRemote method to          
// create instances of the default interface _PlaningProviderCOMWrapper exposed by              
// the CoClass PlaningProviderCOMWrapper. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPlaningProviderCOMWrapper = class
    class function Create: _PlaningProviderCOMWrapper;
    class function CreateRemote(const MachineName: string): _PlaningProviderCOMWrapper;
  end;

implementation

uses ComObj;

class function CoPlaningProviderCOMWrapper.Create: _PlaningProviderCOMWrapper;
begin
  Result := CreateComObject(CLASS_PlaningProviderCOMWrapper) as _PlaningProviderCOMWrapper;
end;

class function CoPlaningProviderCOMWrapper.CreateRemote(const MachineName: string): _PlaningProviderCOMWrapper;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PlaningProviderCOMWrapper) as _PlaningProviderCOMWrapper;
end;

end.
