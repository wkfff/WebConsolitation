unit KernelUtils_TLB;

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
// File generated on 14.11.2003 6:02:49 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\KernelUtils\KernelUtils.tlb (1)
// IID\LCID: {37F0943B-CB74-49EE-BDCF-F1D02882DF0A}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
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
  KernelUtilsMajorVersion = 1;
  KernelUtilsMinorVersion = 0;

  LIBID_KernelUtils: TGUID = '{37F0943B-CB74-49EE-BDCF-F1D02882DF0A}';

  IID_ICommonUtils: TGUID = '{FC4092A3-12B0-44C1-88BB-631A45497616}';
  CLASS_CommonUtils: TGUID = '{ED225776-71ED-4E36-B86B-D4D3EE443737}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  ICommonUtils = interface;
  ICommonUtilsDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  CommonUtils = ICommonUtils;


// *********************************************************************//
// Interface: ICommonUtils
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {FC4092A3-12B0-44C1-88BB-631A45497616}
// *********************************************************************//
  ICommonUtils = interface(IDispatch)
    ['{FC4092A3-12B0-44C1-88BB-631A45497616}']
    function  EncodeString(InpStr: OleVariant): OleVariant; safecall;
    function  DecodeString(InpStr: OleVariant): OleVariant; safecall;
  end;

// *********************************************************************//
// DispIntf:  ICommonUtilsDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {FC4092A3-12B0-44C1-88BB-631A45497616}
// *********************************************************************//
  ICommonUtilsDisp = dispinterface
    ['{FC4092A3-12B0-44C1-88BB-631A45497616}']
    function  EncodeString(InpStr: OleVariant): OleVariant; dispid 1;
    function  DecodeString(InpStr: OleVariant): OleVariant; dispid 2;
  end;

// *********************************************************************//
// The Class CoCommonUtils provides a Create and CreateRemote method to          
// create instances of the default interface ICommonUtils exposed by              
// the CoClass CommonUtils. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCommonUtils = class
    class function Create: ICommonUtils;
    class function CreateRemote(const MachineName: string): ICommonUtils;
  end;

implementation

uses ComObj;

class function CoCommonUtils.Create: ICommonUtils;
begin
  Result := CreateComObject(CLASS_CommonUtils) as ICommonUtils;
end;

class function CoCommonUtils.CreateRemote(const MachineName: string): ICommonUtils;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CommonUtils) as ICommonUtils;
end;

end.
