unit DataAccess_TLB;

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
// File generated on 6.02.2004 1:21:29 PM from Type Library described below.

// ************************************************************************ //
// Type Lib: X:\System\DataAccess\DataAccess.tlb (1)
// IID\LCID: {BE23CBF0-0489-44A0-AEFD-5E2A4B1FC30E}\0
// Helpfile: 
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v2.7 ADOMD, (C:\Program Files\Common Files\System\ADO\msadomd.dll)
//   (3) v2.7 ADODB, (C:\Program Files\Common Files\System\ADO\msado15.dll)
//   (4) v4.0 StdVCL, (C:\WINNT\system32\stdvcl40.dll)
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
interface

uses Windows, ActiveX, Classes, Graphics, OleServer, OleCtrls, StdVCL, 
  ADOMD_TLB, ADODB_TLB;

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  DataAccessMajorVersion = 1;
  DataAccessMinorVersion = 0;

  LIBID_DataAccess: TGUID = '{BE23CBF0-0489-44A0-AEFD-5E2A4B1FC30E}';

  IID_IGetData: TGUID = '{C4264BBA-E3F7-407E-A7B3-CEC7937CB4BC}';
  CLASS_GetData: TGUID = '{DC5123D6-0C99-41CC-97ED-FC2F7C3ED537}';
type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  IGetData = interface;
  IGetDataDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  GetData = IGetData;


// *********************************************************************//
// Declaration of structures, unions and aliases.                         
// *********************************************************************//
  PPUserType1 = ^Recordset; {*}
  PWordBool1 = ^WordBool; {*}


// *********************************************************************//
// Interface: IGetData
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C4264BBA-E3F7-407E-A7B3-CEC7937CB4BC}
// *********************************************************************//
  IGetData = interface(IDispatch)
    ['{C4264BBA-E3F7-407E-A7B3-CEC7937CB4BC}']
    function  GetCellset(const QueryString: WideString; out OutCellset: Cellset): WordBool; safecall;
    function  GetCellsetScr(const QueryString: WideString; out OutCellset: OleVariant): OleVariant; safecall;
    function  GetRecordset(const QueryString: WideString; out OutRecordset: Recordset): WordBool; safecall;
    function  GetRecordsetScr(const QueryString: WideString; out OutRecordset: OleVariant): OleVariant; safecall;
    function  InitAllConnections(const DBConnect: WideString; const MDConnect: WideString): OleVariant; safecall;
    function  InitDBConnect(const DBConnect: WideString): OleVariant; safecall;
    function  InitMDConnect(const MDConnect: WideString): OleVariant; safecall;
    procedure ClearAllConnections; safecall;
    procedure ClearDBConnect; safecall;
    procedure ClearMDConnect; safecall;
    function  Get_LastError: WideString; safecall;
    function  Get_DBConnect: _Connection; safecall;
    function  Get_MDConnect: _Connection; safecall;
    function  GetExternalRecordset(const UDLFileName: WideString; const QueryString: WideString; 
                                   var OutRecordset: Recordset): WordBool; safecall;
    function  Get_BudgetDBConnect: _Connection; safecall;
    procedure ClearBudgetDBConnect; safecall;
    function  GetBudgetRecordset(const QueryString: WideString; out OutRecordset: Recordset): WordBool; safecall;
    function  InitBudgetDBConnect(const BudgetDBConnect: WideString): WordBool; safecall;
    property LastError: WideString read Get_LastError;
    property DBConnect: _Connection read Get_DBConnect;
    property MDConnect: _Connection read Get_MDConnect;
    property BudgetDBConnect: _Connection read Get_BudgetDBConnect;
  end;

// *********************************************************************//
// DispIntf:  IGetDataDisp
// Flags:     (4416) Dual OleAutomation Dispatchable
// GUID:      {C4264BBA-E3F7-407E-A7B3-CEC7937CB4BC}
// *********************************************************************//
  IGetDataDisp = dispinterface
    ['{C4264BBA-E3F7-407E-A7B3-CEC7937CB4BC}']
    function  GetCellset(const QueryString: WideString; out OutCellset: Cellset): WordBool; dispid 1;
    function  GetCellsetScr(const QueryString: WideString; out OutCellset: OleVariant): OleVariant; dispid 10;
    function  GetRecordset(const QueryString: WideString; out OutRecordset: Recordset): WordBool; dispid 2;
    function  GetRecordsetScr(const QueryString: WideString; out OutRecordset: OleVariant): OleVariant; dispid 11;
    function  InitAllConnections(const DBConnect: WideString; const MDConnect: WideString): OleVariant; dispid 3;
    function  InitDBConnect(const DBConnect: WideString): OleVariant; dispid 4;
    function  InitMDConnect(const MDConnect: WideString): OleVariant; dispid 6;
    procedure ClearAllConnections; dispid 5;
    procedure ClearDBConnect; dispid 7;
    procedure ClearMDConnect; dispid 8;
    property LastError: WideString readonly dispid 9;
    property DBConnect: _Connection readonly dispid 12;
    property MDConnect: _Connection readonly dispid 13;
    function  GetExternalRecordset(const UDLFileName: WideString; const QueryString: WideString; 
                                   var OutRecordset: Recordset): WordBool; dispid 14;
    property BudgetDBConnect: _Connection readonly dispid 15;
    procedure ClearBudgetDBConnect; dispid 16;
    function  GetBudgetRecordset(const QueryString: WideString; out OutRecordset: Recordset): WordBool; dispid 17;
    function  InitBudgetDBConnect(const BudgetDBConnect: WideString): WordBool; dispid 18;
  end;

// *********************************************************************//
// The Class CoGetData provides a Create and CreateRemote method to          
// create instances of the default interface IGetData exposed by              
// the CoClass GetData. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoGetData = class
    class function Create: IGetData;
    class function CreateRemote(const MachineName: string): IGetData;
  end;

implementation

uses ComObj;

class function CoGetData.Create: IGetData;
begin
  Result := CreateComObject(CLASS_GetData) as IGetData;
end;

class function CoGetData.CreateRemote(const MachineName: string): IGetData;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_GetData) as IGetData;
end;

end.
