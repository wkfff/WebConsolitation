unit PlaningProvider_TLB;

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
// File generated on 24.11.2010 16:06:17 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\UserTools\Planing\DefattedPlaningProvider\PlaningProvider.tlb (1)
// IID\LCID: {5FD3EBE4-F952-4377-90CD-081C708DECF2}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v3.0 MSXML2, (C:\WINDOWS\system32\msxml3.dll)
//   (3) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
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
  PlaningProviderMajorVersion = 1;
  PlaningProviderMinorVersion = 0;

  LIBID_PlaningProvider: TGUID = '{5FD3EBE4-F952-4377-90CD-081C708DECF2}';

  IID_IPlaningProvider: TGUID = '{5E942C57-E865-4237-AEC8-BEA703890F27}';
  CLASS_PlaningProvider_: TGUID = '{0326AF68-B98D-4076-B7ED-565FC78711B8}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IPlaningProvider = interface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  PlaningProvider_ = IPlaningProvider;


// *********************************************************************//
// Interface: IPlaningProvider
// Flags:     (256) OleAutomation
// GUID:      {5E942C57-E865-4237-AEC8-BEA703890F27}
// *********************************************************************//
  IPlaningProvider = interface(IUnknown)
    ['{5E942C57-E865-4237-AEC8-BEA703890F27}']
    function  URL: WideString; stdcall;
    function  Scheme: WideString; stdcall;
    function  LastError: WideString; stdcall;
    function  LastWarning: WideString; stdcall;
    function  Connected: WordBool; stdcall;
    procedure ClearCache; stdcall;
    procedure FreeProvider; stdcall;
    function  Connect(const URL: WideString; const Login: WideString; const Password: WideString; 
                      AuthType: SYSINT; var SchemeName: WideString; WithinTaskContext: WordBool): WordBool; stdcall;
    procedure Disconnect; stdcall;
    function  GetMetadataDate: WideString; stdcall;
    function  GetMetaData(var XmlDomDocument: IXMLDOMDocument2): WordBool; stdcall;
    function  Writeback(const Data: WideString): WideString; stdcall;
    function  ClientSessionIsAlive: WordBool; stdcall;
    function  GetMemberList(const ProviderId: WideString; const CubeName: WideString; 
                            const DimensionName: WideString; const HierarchyName: WideString; 
                            const LevelNames: WideString; const PropertiesNamesList: WideString): IXMLDOMDocument2; stdcall;
    function  GetRecordsetData(const ProviderId: WideString; const QueryText: WideString; 
                               var DataDom: IXMLDOMDocument2): WordBool; stdcall;
    function  GetCellsetData(const ProviderId: WideString; const QueryText: WideString; 
                             var Data: IXMLDOMDocument2; out ErrorMsg: WideString): WordBool; stdcall;
    function  UpdateMemberList(const ProviderId: WideString; const SourceDom: IXMLDOMDocument2; 
                               var DestDom: IXMLDOMDocument2; const CubeName: WideString; 
                               const DimensionName: WideString; const HierarchyName: WideString; 
                               const LevelList: WideString; const PropertiesNamesList: WideString): WordBool; stdcall;
    function  GetTaskContext(TaskId: SYSINT): WideString; stdcall;
    function  UpdateTaskParams(TaskId: SYSINT; const ParamsText: WideString; 
                               const SectionDivider: WideString; const ValuesDivider: WideString): WideString; stdcall;
    function  UpdateTaskConsts(TaskId: SYSINT; const ConstsText: WideString; 
                               const SectionDivider: WideString; const ValuesDivider: WideString): WideString; stdcall;
  end;

// *********************************************************************//
// The Class CoPlaningProvider_ provides a Create and CreateRemote method to          
// create instances of the default interface IPlaningProvider exposed by              
// the CoClass PlaningProvider_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPlaningProvider_ = class
    class function Create: IPlaningProvider;
    class function CreateRemote(const MachineName: string): IPlaningProvider;
  end;

implementation

uses ComObj;

class function CoPlaningProvider_.Create: IPlaningProvider;
begin
  Result := CreateComObject(CLASS_PlaningProvider_) as IPlaningProvider;
end;

class function CoPlaningProvider_.CreateRemote(const MachineName: string): IPlaningProvider;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PlaningProvider_) as IPlaningProvider;
end;

end.
