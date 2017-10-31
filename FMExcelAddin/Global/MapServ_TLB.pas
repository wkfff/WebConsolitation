unit MapServ_TLB;

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
// File generated on 28.11.2003 11:46:54 AM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\Visualization\MapServ\MapServ.tlb (1)
// IID\LCID: {4DFD89F9-A427-4D4D-A4B4-92A649E0B3A1}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v2.7 ADODB, (C:\Program Files\Common Files\System\ADO\msado15.dll)
//   (3) v3.0 MSXML2, (C:\WINNT\system32\msxml3.dll)
//   (4) v2.7 ADOMD, (C:\Program Files\Common Files\System\ADO\msadomd.dll)
//   (5) v4.0 StdVCL, (C:\WINNT\System32\STDVCL40.DLL)
// Errors:
//   Hint: TypeInfo 'MapServ' changed to 'MapServ_'
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  ADODB_TLB, MSXML2_TLB, ADOMD_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  MapServMajorVersion = 1;
  MapServMinorVersion = 0;

  LIBID_MapServ: TGUID = '{4DFD89F9-A427-4D4D-A4B4-92A649E0B3A1}';

  IID_IMapServ: TGUID = '{938260EA-FF23-4037-A62E-ABCF67C98008}';
  CLASS_MapServ_: TGUID = '{AB853222-5ABC-424E-B86E-BBAAE3D610B4}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IMapServ = interface;
  IMapServDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  MapServ_ = IMapServ;


// *********************************************************************//
// Interface: IMapServ
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {938260EA-FF23-4037-A62E-ABCF67C98008}
// *********************************************************************//
  IMapServ = interface(IDispatch)
    ['{938260EA-FF23-4037-A62E-ABCF67C98008}']
    function  GetPictureFromRecordset(const FileName: WideString; const InRecordset: Recordset; 
                                      Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                      const ParamsTemplateXML: IXMLDOMDocument2;
                                      const MapName: WideString): WordBool; safecall;
    function  GetPictureFromCellset(const FileName: WideString; const InCellset: Cellset;
                                    Width: Integer; Height: Integer; const Patch: IXMLDOMNode;
                                    const ParamsTemplateXML: IXMLDOMDocument2;
                                    const MapName: WideString): WordBool; safecall;
    function  GetStreamFromRecordset(const OutStream: _Stream; const InRecordset: Recordset;
                                     Width: Integer; Height: Integer; const Patch: IXMLDOMNode;
                                     const ParamsTemplateXML: IXMLDOMDocument2;
                                     const MapName: WideString): WordBool; safecall;
    function  GetStreamFromCellset(const OutStream: _Stream; const InCellset: Cellset;
                                   Width: Integer; Height: Integer; const Patch: IXMLDOMNode;
                                   const ParamsTemplateXML: IXMLDOMDocument2;
                                   const MapName: WideString): WordBool; safecall;
    function  Get_LastError: WideString; safecall;
    procedure ReloadAllMaps; safecall;
    procedure Clear; safecall;
    property LastError: WideString read Get_LastError;
  end;

// *********************************************************************//
// DispIntf:  IMapServDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {938260EA-FF23-4037-A62E-ABCF67C98008}
// *********************************************************************//
  IMapServDisp = dispinterface
    ['{938260EA-FF23-4037-A62E-ABCF67C98008}']
    function  GetPictureFromRecordset(const FileName: WideString; const InRecordset: Recordset; 
                                      Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                      const ParamsTemplateXML: IXMLDOMDocument2;
                                      const MapName: WideString): WordBool; dispid 1;
    function  GetPictureFromCellset(const FileName: WideString; const InCellset: Cellset; 
                                    Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                    const ParamsTemplateXML: IXMLDOMDocument2;
                                    const MapName: WideString): WordBool; dispid 2;
    function  GetStreamFromRecordset(const OutStream: _Stream; const InRecordset: Recordset; 
                                     Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                     const ParamsTemplateXML: IXMLDOMDocument2;
                                     const MapName: WideString): WordBool; dispid 3;
    function  GetStreamFromCellset(const OutStream: _Stream; const InCellset: Cellset; 
                                   Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                   const ParamsTemplateXML: IXMLDOMDocument2; 
                                   const MapName: WideString): WordBool; dispid 4;
    property LastError: WideString readonly dispid 5;
    procedure ReloadAllMaps; dispid 6;
    procedure Clear; dispid 8;
  end;

// *********************************************************************//
// The Class CoMapServ_ provides a Create and CreateRemote method to          
// create instances of the default interface IMapServ exposed by              
// the CoClass MapServ_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMapServ_ = class
    class function Create: IMapServ;
    class function CreateRemote(const MachineName: string): IMapServ;
  end;

implementation

uses ComObj;

class function CoMapServ_.Create: IMapServ;
begin
  Result := CreateComObject(CLASS_MapServ_) as IMapServ;
end;

class function CoMapServ_.CreateRemote(const MachineName: string): IMapServ;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MapServ_) as IMapServ;
end;

end.
