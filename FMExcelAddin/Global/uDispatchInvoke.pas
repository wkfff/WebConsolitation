unit uDispatchInvoke;

interface

uses Windows, ComObj, ActiveX, Sysutils;

const

  e_Com: string = 'lalala';

{ Maximum number of dispatch arguments }

  MaxDispArgs = 64; {!!!}

{ Special variant type codes }

  varStrArg = $0048;

{ Parameter type masks }

  atVarMask  = $3F;
  atTypeMask = $7F;
  atByRef    = $80;

type
  { Dispatch call descriptor }

  PCallDesc = ^TCallDesc;
  TCallDesc = packed record
    CallType: Byte;
    ArgCount: Byte;
    NamedArgCount: Byte;
    ArgTypes: array[0..255] of Byte;
  end;

type
  PNamesArray = ^TNamesArray;
  TNamesArray = array[0..0] of PWideChar;

  procedure DispatchInvoke(const Dispatch: IDispatch; CallDesc: PCallDesc;
  		DispIDs: PDispIDList; Params: Pointer; Result: PVariant);
  procedure DispatchInvokeError(Status: Integer; const ExcepInfo: TExcepInfo);
  procedure GetIDsOfNames(const Dispatch: IDispatch; Names: PChar;
		NameCount: Integer; DispIDs: PDispIDList);

  function GetDispatchProperty(const Dispatch: IDispatch; const name: string): OleVariant;
  procedure PutDispatchProperty(const Dispatch: IDispatch; const name: string; PropertyValue: OleVariant);
  function InvokeMetod(const Dispatch: IDispatch; const MethodName: string; ParamNames, ParamValues: OleVariant): OleVariant;
  function OleVarArrayOf(const Values: array of OleVariant): OleVariant;
  procedure SetDispatchProperty(const Dispatch: IDispatch; const name: string; PropertyValue: OleVariant);

implementation

uses AxCtrls;

function OleVarArrayOf(const Values: array of OleVariant): OleVariant;
var I: Integer;
begin
  Result := VarArrayCreate([0, High(Values)], varVariant);
  for I := 0 to High(Values) do Result[I] := Values[I];
end;

function InvokeMetod(const Dispatch: IDispatch; const MethodName: string; ParamNames, ParamValues: OleVariant): OleVariant;
var
  DispIDs: array[0..MaxDispArgs - 1] of Integer;
  CallDesc: TCallDesc;
  Params: pDispParams;
begin
  CallDesc.CallType := DISPATCH_METHOD;
  CallDesc.ArgCount := 0;
  CallDesc.NamedArgCount := 0;
  GetIDsOfNames(Dispatch, PChar(MethodName), 1, @DispIDs);
  DispatchInvoke(Dispatch, @CallDesc, @DispIDs, @Params, nil);
end;

function GetDispatchProperty(const Dispatch: IDispatch; const name: string): OleVariant;
var
  DispIDs: array[0..MaxDispArgs - 1] of Integer;
  Results: Variant;
  CallDesc: TCallDesc;
  Params: pDispParams;
begin
  CallDesc.CallType := DISPATCH_PROPERTYGET;
  CallDesc.ArgCount := 0;
  CallDesc.NamedArgCount := 0;

  GetIDsOfNames(Dispatch, PChar(name), 1, @DispIDs);
  if DispIDs[0] < 0 then
    raise Exception.CreateFmt('GetDispatchProperty: Свойство %s не найдено', [Name]);
  DispatchInvoke(Dispatch, @CallDesc, @DispIDs, @Params, @Results);
  Result := Results
end;

procedure SetDispatchProperty(const Dispatch: IDispatch; const name: string; PropertyValue: OleVariant);
var
  DispIDs: array[0..MaxDispArgs - 1] of Integer;
  Results: Variant;
  CallDesc: TCallDesc;
  Param: Variant;
begin
  Param := PropertyValue;

  CallDesc.CallType := DISPATCH_PROPERTYPUT;
  CallDesc.ArgCount := 1;
  CallDesc.NamedArgCount := 1;
  CallDesc.ArgTypes[0] := TVarData(Param).VType;

  GetIDsOfNames(Dispatch, PChar(name), 1, @DispIDs);
  DispIDs[1] := DISPID_PROPERTYPUT;
  DispatchInvoke(Dispatch, @CallDesc, @DispIDs, @PropertyValue, @Results);
end;

procedure PutDispatchProperty(const Dispatch: IDispatch; const name: string; PropertyValue: OleVariant);
var
  DispIDs: array[0..MaxDispArgs - 1] of Integer;
  Status: Integer;
  DispParams: TDispParams;
  mydispid: integer;
  ExcepInfo: TExcepInfo;
begin
  GetIDsOfNames(Dispatch, PChar(name), 1, @DispIDs);

  DispParams.rgvarg := @PropertyValue;
  mydispid := DISPID_PROPERTYPUT;
  DispParams.rgdispidNamedArgs := @mydispid;
  DispParams.cArgs := 1;
  DispParams.cNamedArgs := 1;
  Status := Dispatch.Invoke(DispIDs[0], GUID_NULL, LOCALE_USER_DEFAULT, DISPATCH_PROPERTYPUT,
    DispParams, nil, @ExcepInfo, nil);
  if Status <> 0 then
    raise Exception.CreateFmt('PutDispatchProperty("%s"): %s: %s', [name, ExcepInfo.bstrSource ,ExcepInfo.bstrDescription])
end;


procedure DispatchInvoke(const Dispatch: IDispatch; CallDesc: PCallDesc;
  DispIDs: PDispIDList; Params: Pointer; Result: PVariant);
type
  PVarArg = ^TVarArg;
  TVarArg = array[0..3] of DWORD;
  TStringDesc = record
    BStr: PWideChar;
    PStr: PString;
  end;
var
  I, J, K, ArgType, ArgCount, StrCount, DispID, InvKind, Status: Integer;
  VarFlag: Byte;
  ParamPtr: ^Integer;
  ArgPtr, VarPtr: PVarArg;
  DispParams: TDispParams;
  ExcepInfo: TExcepInfo;
  Strings: array[0..MaxDispArgs - 1] of TStringDesc;
  Args: array[0..MaxDispArgs - 1] of TVarArg;
begin
  StrCount := 0;
  try
    ArgCount := CallDesc^.ArgCount;
    if ArgCount <> 0 then
    begin
      ParamPtr := Params;
      ArgPtr := @Args[ArgCount];
      I := 0;
      repeat
        Dec(Integer(ArgPtr), SizeOf(TVarData));
        ArgType := CallDesc^.ArgTypes[I] and atTypeMask;
        VarFlag := CallDesc^.ArgTypes[I] and atByRef;
        if ArgType = varError then
        begin
          ArgPtr^[0] := varError;
          ArgPtr^[2] := DWORD(DISP_E_PARAMNOTFOUND);
        end else
        begin
          if ArgType = varStrArg then
          begin
            with Strings[StrCount] do
              if VarFlag <> 0 then
              begin
                BStr := StringToOleStr(PString(ParamPtr^)^);
                PStr := PString(ParamPtr^);
                ArgPtr^[0] := varOleStr or varByRef;
                ArgPtr^[2] := Integer(@BStr);
              end else
              begin
                BStr := StringToOleStr(PString(ParamPtr)^);
                PStr := nil;
                ArgPtr^[0] := varOleStr;
                ArgPtr^[2] := Integer(BStr);
              end;
            Inc(StrCount);
          end else
          if VarFlag <> 0 then
          begin
            if (ArgType = varVariant) and
              (PVarData(ParamPtr^)^.VType = varString) then
              VarCast(PVariant(ParamPtr^)^, PVariant(ParamPtr^)^, varOleStr);
            ArgPtr^[0] := ArgType or varByRef;
            ArgPtr^[2] := ParamPtr^;
          end else
          if ArgType = varVariant then
          begin
            if PVarData(ParamPtr)^.VType = varString then
            begin
              with Strings[StrCount] do
              begin
                BStr := StringToOleStr(string(PVarData(ParamPtr^)^.VString));
                PStr := nil;
                ArgPtr^[0] := varOleStr;
                ArgPtr^[2] := Integer(BStr);
              end;
              Inc(StrCount);
            end else
            begin
              VarPtr := PVarArg(ParamPtr);
              ArgPtr^[0] := VarPtr^[0];
              ArgPtr^[1] := VarPtr^[1];
              ArgPtr^[2] := VarPtr^[2];
              ArgPtr^[3] := VarPtr^[3];
              Inc(Integer(ParamPtr), 12);
            end;
          end else
          begin
            ArgPtr^[0] := ArgType;
            ArgPtr^[2] := ParamPtr^;
            if (ArgType >= varDouble) and (ArgType <= varDate) then
            begin
              Inc(Integer(ParamPtr), 4);
              ArgPtr^[3] := ParamPtr^;
            end;
          end;
          Inc(Integer(ParamPtr), 4);
        end;
        Inc(I);
      until I = ArgCount;
    end;
    DispParams.rgvarg := @Args;
    DispParams.rgdispidNamedArgs := @DispIDs[1];
    DispParams.cArgs := ArgCount;
    DispParams.cNamedArgs := CallDesc^.NamedArgCount;
    DispID := DispIDs[0];
    InvKind := CallDesc^.CallType;
    if InvKind = DISPATCH_PROPERTYPUT then
    begin
      if Args[0][0] and varTypeMask = varDispatch then
        InvKind := DISPATCH_PROPERTYPUTREF;
      DispIDs[0] := DISPID_PROPERTYPUT;
      Dec(Integer(DispParams.rgdispidNamedArgs), SizeOf(Integer));
      Inc(DispParams.cNamedArgs);
    end else
      if (InvKind = DISPATCH_METHOD) and (ArgCount = 0) and (Result <> nil) then
        InvKind := DISPATCH_METHOD or DISPATCH_PROPERTYGET;
    Status := Dispatch.Invoke(DispID, GUID_NULL, 0, InvKind, DispParams,
      Result, @ExcepInfo, nil);
    if Status <> 0 then
    	DispatchInvokeError(Status, ExcepInfo);
    J := StrCount;
    while J <> 0 do
    begin
      Dec(J);
      with Strings[J] do
        if PStr <> nil then OleStrToStrVar(BStr, PStr^);
    end;
  finally
    K := StrCount;
    while K <> 0 do
    begin
      Dec(K);
      SysFreeString(Strings[K].BStr);
    end;
  end;
end;

procedure GetIDsOfNames(const Dispatch: IDispatch; Names: PChar;
  NameCount: Integer; DispIDs: PDispIDList);

  procedure RaiseNameException;
  begin
//    raise EOleError.CreateResFmt(e_Com, Integer( @SNoMethod ), [Names]);
  end;

type
  PNamesArray = ^TNamesArray;
  TNamesArray = array[0..0] of PWideChar;
var
  N, SrcLen, DestLen: Integer;
  Src: PChar;
  Dest: PWideChar;
  NameRefs: PNamesArray;
  StackTop: Pointer;
  Temp: Integer;
begin
  Src := Names;
  N := 0;
  asm
    MOV  StackTop, ESP
    MOV  EAX, NameCount
    INC  EAX
    SHL  EAX, 2  // sizeof pointer = 4
    SUB  ESP, EAX
    LEA  EAX, NameRefs
    MOV  [EAX], ESP
  end;
  repeat
    SrcLen := StrLen(Src);
    DestLen := MultiByteToWideChar(0, 0, Src, SrcLen, nil, 0) + 1;
    asm
      MOV  EAX, DestLen
      ADD  EAX, EAX
      ADD  EAX, 3      // round up to 4 byte boundary
      AND  EAX, not 3
      SUB  ESP, EAX
      LEA  EAX, Dest
      MOV  [EAX], ESP
    end;
    if N = 0 then NameRefs[0] := Dest else NameRefs[NameCount - N] := Dest;
    MultiByteToWideChar(0, 0, Src, SrcLen, Dest, DestLen);
    Dest[DestLen-1] := #0;
    Inc(Src, SrcLen+1);
    Inc(N);
  until N = NameCount;
  Temp := Dispatch.GetIDsOfNames(GUID_NULL, NameRefs, NameCount,
    GetThreadLocale, DispIDs);
  if Temp = Integer(DISP_E_UNKNOWNNAME) then RaiseNameException else OleCheck(Temp);
  asm
    MOV  ESP, StackTop
  end;
end;

{ Raise exception given an OLE return code and TExcepInfo structure }

procedure DispCallError(Status: Integer; var ExcepInfo: TExcepInfo;
  ErrorAddr: Pointer; FinalizeExcepInfo: Boolean);
var
  E: Exception;
begin
  if Status = Integer(DISP_E_EXCEPTION) then
  begin
    with ExcepInfo do
      E := EOleException.Create(ExcepInfo.bstrSource + ': ' + ExcepInfo.bstrDescription, 0, '', '', 0);
    if FinalizeExcepInfo then
      Finalize(ExcepInfo);
  end else
    E := EOleSysError.Create(ExcepInfo.bstrSource + ': ' + ExcepInfo.bstrDescription, 0, 0);
  if ErrorAddr <> nil then
    raise E at ErrorAddr
  else
    raise E;
end;

{ Raise exception given an OLE return code and TExcepInfo structure }

procedure DispatchInvokeError(Status: Integer; const ExcepInfo: TExcepInfo);
begin
  DispCallError(Status, PExcepInfo(@ExcepInfo)^, nil, False);
end;

{procedure ClearExcepInfo(var ExcepInfo: TExcepInfo);
begin
  FillChar(ExcepInfo, SizeOf(ExcepInfo), 0);
end;}


end.
