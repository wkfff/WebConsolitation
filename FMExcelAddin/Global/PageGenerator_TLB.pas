unit PageGenerator_TLB;

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
// File generated on 24.11.2003 4:22:37 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\PageGenerator\PageGenerator.tlb (1)
// IID\LCID: {F760BD45-87A4-40B6-92A6-0F774D6A95C3}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v3.0 MSXML2, (C:\WINNT\system32\msxml3.dll)
//   (3) v4.0 StdVCL, (C:\WINNT\System32\STDVCL40.DLL)
// Errors:
//   Hint: TypeInfo 'PageGenerator' changed to 'PageGenerator_'
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
  PageGeneratorMajorVersion = 1;
  PageGeneratorMinorVersion = 0;

  LIBID_PageGenerator: TGUID = '{F760BD45-87A4-40B6-92A6-0F774D6A95C3}';

  IID_IPageGenerator: TGUID = '{E1EE73B5-6B73-4E3C-9260-763C915B3945}';
  CLASS_PageGenerator_: TGUID = '{8B4D5CF5-8157-4E60-8B1B-8F181E4D54C0}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IPageGenerator = interface;
  IPageGeneratorDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  PageGenerator_ = IPageGenerator;


// *********************************************************************//
// Interface: IPageGenerator
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {E1EE73B5-6B73-4E3C-9260-763C915B3945}
// *********************************************************************//
  IPageGenerator = interface(IDispatch)
    ['{E1EE73B5-6B73-4E3C-9260-763C915B3945}']
    function  GetPageXML(const TemplateDOM: IXMLDOMDocument2; const TemplateFileName: WideString): IXMLDOMDocument2; safecall;
    procedure OnStartPage(const AScriptingContext: IUnknown); safecall;
    procedure OnEndPage; safecall;
    function  Get_ErrorDescr: WideString; safecall;
    function  GetParamXML(const TemplateDOM: IXMLDOMDocument2): IXMLDOMDocument2; safecall;
    function  GetBudgetReportXML(const TemplateDOM: IXMLDOMDocument2;
                                 const TemplateFileName: WideString;
                                 out BudgetReportName: OleVariant;
                                 out BudgetReportParams: OleVariant): IXMLDOMDocument2; safecall;
    function  Get_MapTemplatesDirectory: WideString; safecall;
    procedure Set_MapTemplatesDirectory(const Value: WideString); safecall;
    function  Get_DiagramTemplatesDirectory: WideString; safecall;
    procedure Set_DiagramTemplatesDirectory(const Value: WideString); safecall;
    function  Get_TempDirectory: WideString; safecall;
    procedure Set_TempDirectory(const Value: WideString); safecall;
    property ErrorDescr: WideString read Get_ErrorDescr;
    property MapTemplatesDirectory: WideString read Get_MapTemplatesDirectory write Set_MapTemplatesDirectory;
    property DiagramTemplatesDirectory: WideString read Get_DiagramTemplatesDirectory write Set_DiagramTemplatesDirectory;
    property TempDirectory: WideString read Get_TempDirectory write Set_TempDirectory;
  end;

// *********************************************************************//
// DispIntf:  IPageGeneratorDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {E1EE73B5-6B73-4E3C-9260-763C915B3945}
// *********************************************************************//
  IPageGeneratorDisp = dispinterface
    ['{E1EE73B5-6B73-4E3C-9260-763C915B3945}']
    function  GetPageXML(const TemplateDOM: IXMLDOMDocument2; const TemplateFileName: WideString): IXMLDOMDocument2; dispid 1;
    procedure OnStartPage(const AScriptingContext: IUnknown); dispid 3;
    procedure OnEndPage; dispid 4;
    property ErrorDescr: WideString readonly dispid 2;
    function  GetParamXML(const TemplateDOM: IXMLDOMDocument2): IXMLDOMDocument2; dispid 5;
    function  GetBudgetReportXML(const TemplateDOM: IXMLDOMDocument2; 
                                 const TemplateFileName: WideString; 
                                 out BudgetReportName: OleVariant; 
                                 out BudgetReportParams: OleVariant): IXMLDOMDocument2; dispid 6;
    property MapTemplatesDirectory: WideString dispid 7;
    property DiagramTemplatesDirectory: WideString dispid 8;
    property TempDirectory: WideString dispid 9;
  end;

// *********************************************************************//
// The Class CoPageGenerator_ provides a Create and CreateRemote method to          
// create instances of the default interface IPageGenerator exposed by              
// the CoClass PageGenerator_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPageGenerator_ = class
    class function Create: IPageGenerator;
    class function CreateRemote(const MachineName: string): IPageGenerator;
  end;

implementation

uses ComObj;

class function CoPageGenerator_.Create: IPageGenerator;
begin
  Result := CreateComObject(CLASS_PageGenerator_) as IPageGenerator;
end;

class function CoPageGenerator_.CreateRemote(const MachineName: string): IPageGenerator;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PageGenerator_) as IPageGenerator;
end;

end.
