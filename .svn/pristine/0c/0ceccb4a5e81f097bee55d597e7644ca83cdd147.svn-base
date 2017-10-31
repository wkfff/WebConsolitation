unit Renaming_TLB;

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
// File generated on 29.10.2009 18:39:40 from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\system\InnerTools\RenamingTools\Renaming\DLL\Renaming.tlb (1)
// IID\LCID: {B9F383F3-CBD5-46CA-95EA-B0F47AEAB713}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINDOWS\system32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINDOWS\system32\STDVCL40.DLL)
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
  RenamingMajorVersion = 1;
  RenamingMinorVersion = 0;

  LIBID_Renaming: TGUID = '{B9F383F3-CBD5-46CA-95EA-B0F47AEAB713}';

  IID_IRenamingTool: TGUID = '{9D8DB72B-4F36-4A74-B67F-6C88EF968493}';
  CLASS_RenamingTool: TGUID = '{D514C743-8B34-4509-B096-A5BD904491D4}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IRenamingTool = interface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  RenamingTool = IRenamingTool;


// *********************************************************************//
// Interface: IRenamingTool
// Flags:     (256) OleAutomation
// GUID:      {9D8DB72B-4F36-4A74-B67F-6C88EF968493}
// *********************************************************************//
  IRenamingTool = interface(IUnknown)
    ['{9D8DB72B-4F36-4A74-B67F-6C88EF968493}']
    function  LoadSettings(const FileName: WideString): WordBool; stdcall;
    function  Open(const Path: WideString; out Message: WideString): WordBool; stdcall;
    function  RenameTaskParam(var DimensionName: WideString; var MembersXml: WideString; 
                              out Message: WideString): WordBool; stdcall;
  end;

// *********************************************************************//
// The Class CoRenamingTool provides a Create and CreateRemote method to          
// create instances of the default interface IRenamingTool exposed by              
// the CoClass RenamingTool. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoRenamingTool = class
    class function Create: IRenamingTool;
    class function CreateRemote(const MachineName: string): IRenamingTool;
  end;

implementation

uses ComObj;

class function CoRenamingTool.Create: IRenamingTool;
begin
  Result := CreateComObject(CLASS_RenamingTool) as IRenamingTool;
end;

class function CoRenamingTool.CreateRemote(const MachineName: string): IRenamingTool;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_RenamingTool) as IRenamingTool;
end;

end.
