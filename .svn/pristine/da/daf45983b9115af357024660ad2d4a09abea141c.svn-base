unit ParamsEnumeration_TLB;

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
// File generated on 18.08.2003 2:51:22 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\ParamsEnumeration\ParamsEnumeration.tlb (1)
// IID\LCID: {E4A8FA3D-A17E-4F34-A39A-299D890B473A}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\System32\stdole2.tlb)
//   (2) v4.0 StdVCL, (C:\WINNT\System32\STDVCL40.DLL)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  ParamsEnumerationMajorVersion = 1;
  ParamsEnumerationMinorVersion = 0;

  LIBID_ParamsEnumeration: TGUID = '{E4A8FA3D-A17E-4F34-A39A-299D890B473A}';

  IID_IElementsEnumeration: TGUID = '{9E6EE738-B203-476C-BD81-E63948833386}';
  IID_IElementsEnumerationCollection: TGUID = '{8B6D828B-7B5E-4616-86B7-05F10F9AD44C}';
  CLASS_ElementsEnumeration: TGUID = '{ECF49883-C7A3-4C89-BB7F-1B98C7FC710F}';
  CLASS_ElementsEnumerationCollection: TGUID = '{1DCD4B5C-3B1F-4CB6-8934-F390A2B7C2C6}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IElementsEnumeration = interface;
  IElementsEnumerationDisp = dispinterface;
  IElementsEnumerationCollection = interface;
  IElementsEnumerationCollectionDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  ElementsEnumeration = IElementsEnumeration;
  ElementsEnumerationCollection = IElementsEnumerationCollection;


// *********************************************************************//
// Interface: IElementsEnumeration
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {9E6EE738-B203-476C-BD81-E63948833386}
// *********************************************************************//
  IElementsEnumeration = interface(IDispatch)
    ['{9E6EE738-B203-476C-BD81-E63948833386}']
    function  Get_ElementsCount: Integer; safecall;
    procedure Element(Index: Integer; var LvlName: OleVariant; var Value: OleVariant; 
                      var Key: OleVariant); safecall;
    procedure AddElement(LvlName: OleVariant; Value: OleVariant; Key: OleVariant); safecall;
    function  Get_CheckLvlByName(const LvlName: WideString): WordBool; safecall;
    function  Get_ValueByLvlName(const LvlName: WideString): WideString; safecall;
    function  Get_KeyByLvlName(const LvlName: WideString): WideString; safecall;
    property ElementsCount: Integer read Get_ElementsCount;
    property CheckLvlByName[const LvlName: WideString]: WordBool read Get_CheckLvlByName;
    property ValueByLvlName[const LvlName: WideString]: WideString read Get_ValueByLvlName;
    property KeyByLvlName[const LvlName: WideString]: WideString read Get_KeyByLvlName;
  end;

// *********************************************************************//
// DispIntf:  IElementsEnumerationDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {9E6EE738-B203-476C-BD81-E63948833386}
// *********************************************************************//
  IElementsEnumerationDisp = dispinterface
    ['{9E6EE738-B203-476C-BD81-E63948833386}']
    property ElementsCount: Integer readonly dispid 1;
    procedure Element(Index: Integer; var LvlName: OleVariant; var Value: OleVariant; 
                      var Key: OleVariant); dispid 2;
    procedure AddElement(LvlName: OleVariant; Value: OleVariant; Key: OleVariant); dispid 7;
    property CheckLvlByName[const LvlName: WideString]: WordBool readonly dispid 8;
    property ValueByLvlName[const LvlName: WideString]: WideString readonly dispid 9;
    property KeyByLvlName[const LvlName: WideString]: WideString readonly dispid 5;
  end;

// *********************************************************************//
// Interface: IElementsEnumerationCollection
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8B6D828B-7B5E-4616-86B7-05F10F9AD44C}
// *********************************************************************//
  IElementsEnumerationCollection = interface(IDispatch)
    ['{8B6D828B-7B5E-4616-86B7-05F10F9AD44C}']
    function  Get_EnumerationsCount: Integer; safecall;
    function  Get_Enumeration(Index: Integer): IElementsEnumeration; safecall;
    function  Add: IElementsEnumeration; safecall;
    procedure Clear; safecall;
    property EnumerationsCount: Integer read Get_EnumerationsCount;
    property Enumeration[Index: Integer]: IElementsEnumeration read Get_Enumeration;
  end;

// *********************************************************************//
// DispIntf:  IElementsEnumerationCollectionDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {8B6D828B-7B5E-4616-86B7-05F10F9AD44C}
// *********************************************************************//
  IElementsEnumerationCollectionDisp = dispinterface
    ['{8B6D828B-7B5E-4616-86B7-05F10F9AD44C}']
    property EnumerationsCount: Integer readonly dispid 1;
    property Enumeration[Index: Integer]: IElementsEnumeration readonly dispid 2;
    function  Add: IElementsEnumeration; dispid 3;
    procedure Clear; dispid 4;
  end;

// *********************************************************************//
// The Class CoElementsEnumeration provides a Create and CreateRemote method to          
// create instances of the default interface IElementsEnumeration exposed by              
// the CoClass ElementsEnumeration. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoElementsEnumeration = class
    class function Create: IElementsEnumeration;
    class function CreateRemote(const MachineName: string): IElementsEnumeration;
  end;

// *********************************************************************//
// The Class CoElementsEnumerationCollection provides a Create and CreateRemote method to          
// create instances of the default interface IElementsEnumerationCollection exposed by              
// the CoClass ElementsEnumerationCollection. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoElementsEnumerationCollection = class
    class function Create: IElementsEnumerationCollection;
    class function CreateRemote(const MachineName: string): IElementsEnumerationCollection;
  end;

implementation

uses ComObj;

class function CoElementsEnumeration.Create: IElementsEnumeration;
begin
  Result := CreateComObject(CLASS_ElementsEnumeration) as IElementsEnumeration;
end;

class function CoElementsEnumeration.CreateRemote(const MachineName: string): IElementsEnumeration;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ElementsEnumeration) as IElementsEnumeration;
end;

class function CoElementsEnumerationCollection.Create: IElementsEnumerationCollection;
begin
  Result := CreateComObject(CLASS_ElementsEnumerationCollection) as IElementsEnumerationCollection;
end;

class function CoElementsEnumerationCollection.CreateRemote(const MachineName: string): IElementsEnumerationCollection;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ElementsEnumerationCollection) as IElementsEnumerationCollection;
end;

end.
