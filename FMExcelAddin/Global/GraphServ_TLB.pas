unit GraphServ_TLB;

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
// File generated on 7.09.2004 6:44:39 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\Visualization\GraphServ\GraphServ.tlb (1)
// IID\LCID: {AF9327BC-9992-46B4-A90F-B4F20BA5D488}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v2.7 ADODB, (C:\Program Files\Common Files\System\ado\msado27.tlb)
//   (3) v3.0 MSXML2, (C:\WINNT\system32\msxml3.dll)
//   (4) v2.8 ADOMD, (C:\Program Files\Common Files\System\ado\msadomd.dll)
//   (5) v4.0 StdVCL, (C:\WINNT\system32\stdvcl40.dll)
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
  GraphServMajorVersion = 1;
  GraphServMinorVersion = 0;

  LIBID_GraphServ: TGUID = '{AF9327BC-9992-46B4-A90F-B4F20BA5D488}';

  IID_IGraphServer: TGUID = '{EBCD009C-8587-4977-843F-DFB437009010}';
  CLASS_GraphServer: TGUID = '{D5700A3D-C413-43BC-A5C6-F75BFAB0665C}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IGraphServer = interface;
  IGraphServerDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  GraphServer = IGraphServer;


// *********************************************************************//
// Interface: IGraphServer
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EBCD009C-8587-4977-843F-DFB437009010}
// *********************************************************************//
  IGraphServer = interface(IDispatch)
    ['{EBCD009C-8587-4977-843F-DFB437009010}']
    function  GetPictureFromRecordset(const FileName: WideString; const InRecordset: Recordset; 
                                      Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                      const ParamsTemplateXML: IXMLDOMDocument2): WordBool; safecall;
    function  GetPictureFromCellset(const FileName: WideString; const InCellset: Cellset; 
                                    Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                    const ParamsTemplateXML: IXMLDOMDocument2): WordBool; safecall;
    function  Get_LastError: WideString; safecall;
    function  GetStreamFromRecordset(const OutStream: _Stream; const InRecordset: Recordset; 
                                     Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                     const ParamsTemplateXML: IXMLDOMDocument2): WordBool; safecall;
    function  GetStreamFromCellset(const OutStream: _Stream; const InCellset: Cellset; 
                                   Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                   const ParamsTemplateXML: IXMLDOMDocument2): WordBool; safecall;
    property LastError: WideString read Get_LastError;
  end;

// *********************************************************************//
// DispIntf:  IGraphServerDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {EBCD009C-8587-4977-843F-DFB437009010}
// *********************************************************************//
  IGraphServerDisp = dispinterface
    ['{EBCD009C-8587-4977-843F-DFB437009010}']
    function  GetPictureFromRecordset(const FileName: WideString; const InRecordset: Recordset; 
                                      Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                      const ParamsTemplateXML: IXMLDOMDocument2): WordBool; dispid 1;
    function  GetPictureFromCellset(const FileName: WideString; const InCellset: Cellset; 
                                    Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                    const ParamsTemplateXML: IXMLDOMDocument2): WordBool; dispid 2;
    property LastError: WideString readonly dispid 3;
    function  GetStreamFromRecordset(const OutStream: _Stream; const InRecordset: Recordset; 
                                     Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                     const ParamsTemplateXML: IXMLDOMDocument2): WordBool; dispid 4;
    function  GetStreamFromCellset(const OutStream: _Stream; const InCellset: Cellset; 
                                   Width: Integer; Height: Integer; const Patch: IXMLDOMNode; 
                                   const ParamsTemplateXML: IXMLDOMDocument2): WordBool; dispid 5;
  end;

// *********************************************************************//
// The Class CoGraphServer provides a Create and CreateRemote method to          
// create instances of the default interface IGraphServer exposed by              
// the CoClass GraphServer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoGraphServer = class
    class function Create: IGraphServer;
    class function CreateRemote(const MachineName: string): IGraphServer;
  end;

implementation

uses ComObj;

class function CoGraphServer.Create: IGraphServer;
begin
  Result := CreateComObject(CLASS_GraphServer) as IGraphServer;
end;

class function CoGraphServer.CreateRemote(const MachineName: string): IGraphServer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_GraphServer) as IGraphServer;
end;

end.
