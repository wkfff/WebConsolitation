unit uDSOPropEdit;

interface

uses Windows, Activex, SysUtils, Classes, Graphics, Controls,
  uDsoXmlSchema, uDsoXmlConverter, DSO80_TLB;

resourcestring
  srUnknown = '(Unknown)';
  srErrorGetProp = '(Error)';
  srNotAccessableValue = '(Not accessable value)';
  SInvalidPropertyValue = 'Invalid property value';
  SOutOfRange = 'Value must be between %d and %d';

type
{Smallint, WideString, WordBool, TOleEnum, TDateTime, OleVariant, Integer, Double, IDispatch}
  TTypeKind = (
    tkUnknown,
    tkSmallint,
    tkWordBool,
    tkTDateTime,
    tkInteger,
    tkWideString,
    tkTOleEnum,
    tkDouble,
    tkVariant,
    tkInterface,
    tkLWideString
  );

{  TTypeKind = (tkUnknown, tkInteger, tkChar, tkEnumeration, tkFloat,
    tkString, tkSet, tkClass, tkMethod, tkWChar, tkLString, tkWString,
    tkVariant, tkArray, tkRecord, tkInterface, tkInt64, tkDynArray);}
  TTypeKinds = set of TTypeKind;

  PTypeInfo = ^TTypeInfo;
  TTypeInfo = record
    Kind: TTypeKind;
    Name: ShortString;
  end;

  PPropInfo = ^TPropInfo;
  TPropInfo = packed record
    AProp: TProperty;
    PropType: ^PTypeInfo;
    GetProc: Pointer;
    SetProc: Pointer;
    StoredProc: Pointer;
    Index: Integer;
    Default: Longint;
    NameIndex: SmallInt;
    Name: ShortString;
  end;

  TPropertyAttribute = (paValueList, paSubProperties, paDialog, paMultiSelect,
    paAutoUpdate, paSortList, paReadOnly, paRevertable, paFullWidthName);
  TPropertyAttributes = set of TPropertyAttribute;

  TPropertyEditor = class;

  TInstProp = record
//!!    Instance: TPersistent;
    PropInfo: PPropInfo;
    Prop: TProperty;
  end;

  PInstPropList = ^TInstPropList;
  TInstPropList = array[0..1023] of TInstProp;

  TGetPropEditProc = procedure(Prop: TPropertyEditor) of object;
  TModifiedProc = procedure(Prop: TPropertyEditor) of object;

  TPropertyEditor = class
  private
    FDSOObject: ICommon;
    FOnGetMacrosList: TGetMacrosListProc;
    FPropList: PInstPropList;
    FPropCount: Integer;
    FOnModified: TModifiedProc;
    FLastException: string;
//!!    function GetPrivateDirectory: string;
  protected
    function GetFloatValue: Extended;
    function GetFloatValueAt(Index: Integer): Extended;
    function GetInt64Value: Int64;
    function GetInt64ValueAt(Index: Integer): Int64;
//!!    function GetMethodValue: TMethod;
//!!    function GetMethodValueAt(Index: Integer): TMethod;
    function GetOrdValue: Longint;
    function GetOrdValueAt(Index: Integer): Longint;
    function GetStrValue: string;
    function GetStrValueAt(Index: Integer): string;
    function GetVarValue: Variant;
    function GetVarValueAt(Index: Integer): Variant;
    procedure Modified;
    procedure SetFloatValue(Value: Extended);
    procedure SetDateTimeValue(Value: Extended);
//!!    procedure SetMethodValue(const Value: TMethod);
    procedure SetInt64Value(Value: Int64);
    procedure SetOrdValue(Value: Longint);
    procedure SetStrValue(const Value: string);
    procedure SetVarValue(const Value: Variant);
  public
    constructor Create(DSOObject: ICommon; APropCount: Integer); {virtual;}
    procedure SetPropEntry(Index: Integer; AProp: TProperty);
    destructor Destroy; override;
    procedure Activate; virtual;
    function AllEqual: Boolean; virtual;
    function AutoFill: Boolean; virtual;
    procedure Edit; virtual;
    function GetAttributes: TPropertyAttributes; virtual;
    function GetComponent(Index: Integer): ICommon;
    function GetPropInfo: TProperty;
    function GetEditLimit: Integer; virtual;
    function GetName: string; virtual;
    procedure GetProperties(Proc: TGetPropEditProc); virtual;
    function GetPropType: PTypeInfo;
    function GetValue: string; virtual;
    function GetVisualValue: string;
    procedure GetValues(Proc: TGetStrProc); virtual;
    procedure Initialize; virtual;
    procedure Revert;
    procedure SetValue(const Value: string); virtual;
    function ValueAvailable: Boolean;
    procedure ListMeasureWidth(const Value: string; ACanvas: TCanvas;
      var AWidth: Integer); dynamic;
    procedure ListMeasureHeight(const Value: string; ACanvas: TCanvas;
      var AHeight: Integer); dynamic;
    procedure ListDrawValue(const Value: string; ACanvas: TCanvas;
      const ARect: TRect; ASelected: Boolean); dynamic;
    procedure PropDrawName(ACanvas: TCanvas; const ARect: TRect;
      ASelected: Boolean); dynamic;
    procedure PropDrawValue(ACanvas: TCanvas; const ARect: TRect;
      ASelected: Boolean); dynamic;
    function IsReadOnly: boolean;
//!!    property Designer: IFormDesigner read FDesigner;
//!!    property PrivateDirectory: string read GetPrivateDirectory;
    property PropCount: Integer read FPropCount;
    property Value: string read GetValue write SetValue;
    property OnModified: TModifiedProc read FOnModified write FOnModified;
    property DSOObject: ICommon read FDSOObject;
    property OnGetMacrosList: TGetMacrosListProc read FOnGetMacrosList write FOnGetMacrosList;
    property LastException: string read FLastException write FLastException;
  end;

  TPropertyEditorClass = class of TPropertyEditor;

{ TOrdinalProperty
  The base class of all ordinal property editors.  It established that ordinal
  properties are all equal if the GetOrdValue all return the same value. }

  TOrdinalProperty = class(TPropertyEditor)
    function AllEqual: Boolean; override;
    function GetEditLimit: Integer; override;
  end;

{ TIntegerProperty
  Default editor for all Longint properties and all subtypes of the Longint
  type (i.e. Integer, Word, 1..10, etc.).  Restricts the value entered into
  the property to the range of the sub-type. }

  TIntegerProperty = class(TOrdinalProperty)
  public
    function GetValue: string; override;
    procedure SetValue(const Value: string); override;
  end;

{ TEnumProperty
  The default property editor for all enumerated properties (e.g. TShape =
  (sCircle, sTriangle, sSquare), etc.). }

  TEnumProperty = class(TOrdinalProperty)
  public
    function GetAttributes: TPropertyAttributes; override;
    function GetValue: string; override;
    procedure GetValues(Proc: TGetStrProc); override;
    procedure SetValue(const Value: string); override;
  end;

  TBoolProperty = class(TEnumProperty)
    function GetValue: string; override;
    procedure GetValues(Proc: TGetStrProc); override;
    procedure SetValue(const Value: string); override;
  end;

{ TStringProperty
  The default property editor for all strings and sub types (e.g. string,
  string[20], etc.). }

  TStringProperty = class(TPropertyEditor)
  public
    function AllEqual: Boolean; override;
    function GetEditLimit: Integer; override;
    function GetValue: string; override;
    procedure SetValue(const Value: string); override;
  end;

  TMultiLineStr = type string;

  TMultiLineStrProperty = class(TStringProperty)
  public
    procedure Edit; override;
    function GetAttributes: TPropertyAttributes; override;
  end;

{ TDateProperty
  Property editor for date portion of TDateTime type. }

  TDateProperty = class(TPropertyEditor)
    function GetAttributes: TPropertyAttributes; override;
    function GetValue: string; override;
    procedure SetValue(const Value: string); override;
  end;

{ TTimeProperty
  Property editor for time portion of TDateTime type. }

  TTimeProperty = class(TPropertyEditor)
    function GetAttributes: TPropertyAttributes; override;
    function GetValue: string; override;
    procedure SetValue(const Value: string); override;
  end;

{ TDateTimeProperty
  Edits both date and time data... simultaneously!  }

  TDateTimeProperty = class(TPropertyEditor)
    function GetAttributes: TPropertyAttributes; override;
    function GetValue: string; override;
    procedure SetValue(const Value: string); override;
  end;

  TObjectProperty = class(TPropertyEditor)
  public
    function GetAttributes: TPropertyAttributes; override;
    procedure GetProperties(Proc: TGetPropEditProc); override;
    function GetValue: string; override;
  end;

  TDataSourceProperty = class(TObjectProperty)
  public
    procedure Edit; override;
    function GetAttributes: TPropertyAttributes; override;
    procedure GetProperties(Proc: TGetPropEditProc); override;
  end;


  EPropertyError = class(Exception);

var
  BooleanIdents: array [Boolean] of string = ('False', 'True');
  DotSep: string = '.';

implementation

uses
  StdCtrls, ExtCtrls, Forms, Messages, uDispatchInvoke, uDSOPropList;

function GetDSOEnumName(Enum: TDSOEnum; Value: Integer): string;
begin
  Result := TEnumItem(Enum[Value]^).Name;
end;
(*
function GetEnumName(TypeInfo: PTypeInfo; Value: Integer): string;
begin
  if TypeInfo = System.TypeInfo(Boolean) then
  begin
    Result := BooleanIdents[Boolean(Value)];
    if CompareText(HexDisplayPrefix, '0x') = 0 then Result := LowerCase(Result);
    Exit;
  end;
  if TypeInfo^.Kind = tkInteger then
  begin
    Result := IntToStr(Value);
    Exit;
  end;
)*(*!!  T := GetTypeData(GetTypeData(TypeInfo)^.BaseType^);
  if T^.MinValue < 0 then      { must be LongBool/WordBool/ByteBool }
    Value := Ord(Value <> 0);  { map non-zero to true in this case  }
  P := @T^.NameList;
  while Value <> 0 do
  begin
    Inc(Integer(P), Length(P^) + 1);
    Dec(Value);
  end;
  Result := P^;*)
//end;

function GetDSOEnumValue(Enum: TDSOEnum; const Name: string): Integer;
var
  Indx: Integer;
begin
  Indx := Enum.FindItemIDByName(Name);
  Result := TEnumItem(Enum[Indx]^).id
end;

function GetEnumValue(TypeInfo: PTypeInfo; const Name: string): Integer;
begin
  if TypeInfo^.Kind = tkInteger then
    Result := StrToInt(Name)
  else
    Result := 0{!!GetEnumNameValue(TypeInfo, Name)};
end;

function GetStrProp(Obj: ICommon; Prop: TProperty): string;
{!!var
  Disp: IDispatch;}
begin
//  try
//    Result := GetDSOObjectPropMetaValue(Obj, Prop.Name, Prop.Owner,);
{!!    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    if Disp = nil then
      Result := srNotAccessableValue
    else
      Result := VarToStr(GetDispatchProperty(Disp, Prop.Name));}
//  except
//    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
//  end
end;

procedure SetStrProp(Obj: ICommon; Prop: TProperty; value: string);
var
  Disp: IDispatch;
  v: Variant;
begin
//  try
    v := value;
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    PutDispatchProperty(Disp, Prop.Name, v);
//  except
//  end
end;

function GetIntegerProp(Obj: ICommon; Prop: TProperty): Integer;
var
  Disp: IDispatch;
begin
  Result := 0;
//  try
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    if Disp <> nil then
      Result := GetDispatchProperty(Disp, Prop.Name);
//  except
//    exit
//  end
end;

procedure SetIntegerProp(Obj: ICommon; Prop: TProperty; value: integer);
var
  Disp: IDispatch;
  v: Variant;
begin
//  try
    v := value;
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    PutDispatchProperty(Disp, Prop.Name, v);
//  except
//  end
end;

function GetFloatProp(Obj: ICommon; Prop: TProperty): Extended;
var
  Disp: IDispatch;
begin
//  try
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    Result := GetDispatchProperty(Disp, Prop.Name);
//  except
//    on E: Exception do
//    exit
//  end
end;

procedure SetFloatProp(Obj: ICommon; Prop: TProperty; value: Extended);
var
  Disp: IDispatch;
  v: Variant;
begin
//  try
    v := VarAsType(v, varDate	);
    v := value;
    v := VarAsType(v, varDate	);
    v := VarAsType(value, varDate	);
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    PutDispatchProperty(Disp, Prop.Name, v);
//  except
//  end
end;

procedure SetDateTimeProp(Obj: ICommon; Prop: TProperty; value: TDateTime);
var
  Disp: IDispatch;
  v: Variant;
begin
//  try
    v := value;
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    PutDispatchProperty(Disp, Prop.Name, v);
//  except
//  end
end;

function GetVariantProp(Obj: ICommon; Prop: TProperty): string;
var
  Disp: IDispatch;
begin
//  try
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    Result := GetDispatchProperty(Disp, Prop.Name);
//  except
//    Result := srErrorGetProp;
//  end
end;

procedure SetVariantProp(Obj: ICommon; Prop: TProperty; value: Variant);
var
  Disp: IDispatch;
begin
//  try
    IUnknown(Obj).QueryInterface(Prop.Owner.InterfaceGUID, Disp);
    PutDispatchProperty(Disp, Prop.Name, value);
//  except
//  end
end;

{ TPropertyEditor }

constructor TPropertyEditor.Create(DSOObject: ICommon; APropCount: Integer);
begin
  FDSOObject := DSOObject;
  GetMem(FPropList, APropCount * SizeOf(TInstProp));
  FPropCount := APropCount;
end;

destructor TPropertyEditor.Destroy;
var
  i: Integer;
begin
  if FPropList <> nil then begin
    for i := 0 to FPropCount - 1 do
      if FPropList[i].PropInfo <> nil then
        FreeMem(FPropList[i].PropInfo);
    FreeMem(FPropList, FPropCount * SizeOf(TInstProp));
  end
end;

procedure TPropertyEditor.Activate;
begin
end;

function TPropertyEditor.AllEqual: Boolean;
begin
  Result := FPropCount = 1;
end;

procedure TPropertyEditor.Edit;
type
  TGetStrFunc = function(const Value: string): Integer of object;
var
  I: Integer;
  Values: TStringList;
  AddValue: TGetStrFunc;
begin
  if not AutoFill then Exit;
  Values := TStringList.Create;
  Values.Sorted := paSortList in GetAttributes;
  try
    AddValue := Values.Add;
    GetValues(TGetStrProc(AddValue));
    if Values.Count > 0 then
    begin
      I := Values.IndexOf(Value) + 1;
      if I = Values.Count then I := 0;
      Value := Values[I];
    end;
  finally
    Values.Free;
  end;
end;

function TPropertyEditor.AutoFill: Boolean;
begin
  Result := True;
end;

function TPropertyEditor.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paRevertable];
end;

function TPropertyEditor.GetComponent(Index: Integer): ICommon;
begin
//  Result := FPropList^[Index].Instance;
  Result := FDSOObject;
end;

function TPropertyEditor.GetFloatValue: Extended;
begin
  Result := GetFloatValueAt(0);
end;

function TPropertyEditor.GetFloatValueAt(Index: Integer): Extended;
begin
  with FPropList^[Index] do Result := GetFloatProp(FDSOObject, Prop);
end;

{!!function TPropertyEditor.GetMethodValue: TMethod;
begin
  Result := GetMethodValueAt(0);
end;

function TPropertyEditor.GetMethodValueAt(Index: Integer): TMethod;
begin
  with FPropList^[Index] do Result := GetMethodProp(Instance, PropInfo);
end;}

function TPropertyEditor.GetEditLimit: Integer;
begin
  Result := 255;
end;

function TPropertyEditor.GetName: string;
begin
  Result := FPropList^[0].Prop.Name;
end;

function TPropertyEditor.GetOrdValue: Longint;
begin
  Result := GetOrdValueAt(0);
end;

function TPropertyEditor.GetOrdValueAt(Index: Integer): Longint;
begin
  with FPropList^[Index] do Result := GetIntegerProp(FDSOObject, Prop);
end;

{!!function TPropertyEditor.GetPrivateDirectory: string;
begin
  Result := '';
  if Designer <> nil then
    Result := Designer.GetPrivateDirectory;
end;}

procedure TPropertyEditor.GetProperties(Proc: TGetPropEditProc);
begin
end;

function TPropertyEditor.GetPropInfo: TProperty;
begin
//  Result := FPropList^[0].PropInfo;
  Result := FPropList^[0].Prop;
end;

function TPropertyEditor.GetPropType: PTypeInfo;
var
  PropType: PTypeInfo;
begin
  PropType := @FPropList^[0].PropInfo^.PropType;
  case FPropList^[0].Prop.DataType of
  0: PropType^.Kind := tkSmallint;
  1: PropType^.Kind := tkWideString;
  2: PropType^.Kind := tkWordBool;
  3: PropType^.Kind := tkTDateTime;
  4: PropType^.Kind := tkDouble;
  5: PropType^.Kind := tkInterface;
  11: PropType^.Kind := tkLWideString;
  else
    PropType^.Kind := tkUnknown;
  end;
  Result := PropType;
end;

function TPropertyEditor.GetStrValue: string;
begin
  Result := GetStrValueAt(0);
end;

function TPropertyEditor.GetStrValueAt(Index: Integer): string;
begin
  with FPropList^[Index] do
    Result := GetDSOObjectPropMetaValue(FDSOObject, Prop.Name, Prop.Owner, FOnGetMacrosList);
//!!    Result := GetStrProp(FDSOObject, Prop);
end;

function TPropertyEditor.GetVarValue: Variant;
begin
  Result := GetVarValueAt(0);
end;

function TPropertyEditor.GetVarValueAt(Index: Integer): Variant;
begin
  with FPropList^[Index] do Result := GetVariantProp(FDSOObject, Prop);
end;

function TPropertyEditor.GetValue: string;
var
  Disp: IDispatch;
begin
  try
    IUnknown(FDSOObject).QueryInterface(FPropList^[0].Prop.Owner.InterfaceGUID, Disp);
    Result := VarToStr(GetDispatchProperty(Disp, FPropList^[0].Prop.Name));
  except
    Result := srUnknown;
  end
end;

function TPropertyEditor.GetVisualValue: string;
begin
  if AllEqual then
    Result := GetValue
  else
    Result := '';
end;

procedure TPropertyEditor.GetValues(Proc: TGetStrProc);
begin
end;

procedure TPropertyEditor.Initialize;
begin
end;

procedure TPropertyEditor.Modified;
begin
  if Assigned(FOnModified) then
    FOnModified(Self);
//    FDSOObject.SaveObject;
//    FPropList.MarkModified;
end;

procedure TPropertyEditor.SetFloatValue(Value: Extended);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetFloatProp(FDSOObject, Prop, Value);
  Modified;
end;

procedure TPropertyEditor.SetDateTimeValue(Value: Extended);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetDateTimeProp(FDSOObject, Prop, Value);
  Modified;
end;

{!!procedure TPropertyEditor.SetMethodValue(const Value: TMethod);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetMethodProp(Instance, PropInfo, Value);
  Modified;
end;}

procedure TPropertyEditor.SetOrdValue(Value: Longint);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetIntegerProp(FDSOObject, Prop, Value);
  Modified;
end;

procedure TPropertyEditor.SetPropEntry(Index: Integer; AProp: TProperty);
var APropInfo: PPropInfo;
begin
  New(APropInfo);
  with FPropList^[Index] do
  begin
//!!    Instance := AInstance;
    PropInfo := APropInfo;
    Prop := AProp;
  end;
end;

procedure TPropertyEditor.SetStrValue(const Value: string);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do begin
//      SetStrProp(FDSOObject, Prop, Value);
      SetDSOObjectPropMetaValue(FDSOObject, Prop.Name, Prop.Owner, FOnGetMacrosList, Value);
    end;

  Modified;
end;

procedure TPropertyEditor.SetVarValue(const Value: Variant);
var
  I: Integer;
begin
  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetVariantProp(FDSOObject, Prop, Value);
  Modified;
end;

procedure TPropertyEditor.Revert;
begin
{!!  if Designer <> nil then
    for I := 0 to FPropCount - 1 do
      with FPropList^[I] do Designer.Revert(Instance, PropInfo);}
end;

procedure TPropertyEditor.SetValue(const Value: string);
begin
end;

function TPropertyEditor.ValueAvailable: Boolean;
var
  I: Integer;
//!!  S: string;
begin
  Result := True;
  for I := 0 to FPropCount - 1 do
  begin
{!!    if (FPropList^[I].Instance is TComponent) and
      (csCheckPropAvail in TComponent(FPropList^[I].Instance).ComponentStyle) then
    begin
      try
        S := GetValue;
        AllEqual;
      except
        Result := False;
      end;
      Exit;
    end;}
  end;
end;

function TPropertyEditor.GetInt64Value: Int64;
begin
  Result := GetInt64ValueAt(0);
end;

function TPropertyEditor.GetInt64ValueAt(Index: Integer): Int64;
begin
//!!  with FPropList^[Index] do Result := GetInt64Prop(Instance, PropInfo);
  Result := 0;
end;

procedure TPropertyEditor.SetInt64Value(Value: Int64);
begin
{!!  for I := 0 to FPropCount - 1 do
    with FPropList^[I] do SetInt64Prop(Instance, PropInfo, Value);}
  Modified;
end;

{ these three procedures implement the default render behavior of the
  object/property inspector's drop down list editor.  you don't need to
  override the two measure procedures if the default width or height don't
  need to be changed. }
procedure TPropertyEditor.ListMeasureHeight(const Value: string; ACanvas: TCanvas;
  var AHeight: Integer);
begin
end;

procedure TPropertyEditor.ListMeasureWidth(const Value: string; ACanvas: TCanvas;
  var AWidth: Integer);
begin
end;

procedure TPropertyEditor.ListDrawValue(const Value: string; ACanvas: TCanvas;
  const ARect: TRect; ASelected: Boolean);
begin
  ACanvas.TextRect(ARect, ARect.Left + 1, ARect.Top + 1, Value);
end;

{ these two procedures implement the default render behavior of the
  object/property inspector }
procedure TPropertyEditor.PropDrawName(ACanvas: TCanvas; const ARect: TRect;
  ASelected: Boolean);
begin
  ACanvas.TextRect(ARect, ARect.Left + 1, ARect.Top + 1, GetName);
end;

procedure TPropertyEditor.PropDrawValue(ACanvas: TCanvas; const ARect: TRect;
  ASelected: Boolean);
var
  SavedColor: TColor;
begin
  SavedColor := ACanvas.Font.Color;
  if IsReadOnly then
    ACanvas.Font.Color := clGray;//clBtnFace;
  ACanvas.TextRect(ARect, ARect.Left + 1, ARect.Top + 1, GetVisualValue);
  ACanvas.Font.Color := SavedColor
end;

function TPropertyEditor.IsReadOnly: boolean;
begin
  result := (FPropList^[0].Prop.CheckAccess(FDSOObject, AccessWrite) = 1)
end;

{ TOrdinalProperty }

function TOrdinalProperty.AllEqual: Boolean;
var
  I: Integer;
  V: Longint;
begin
  Result := False;
  if PropCount > 1 then
  begin
    V := GetOrdValue;
    for I := 1 to PropCount - 1 do
      if GetOrdValueAt(I) <> V then Exit;
  end;
  Result := True;
end;

function TOrdinalProperty.GetEditLimit: Integer;
begin
  Result := 63;
end;

{ TIntegerProperty }

function TIntegerProperty.GetValue: string;
begin
{  with GetTypeData(GetPropType)^ do
    if OrdType = otULong then // unsigned
      Result := IntToStr(Cardinal(GetOrdValue))
    else}
  try
    Result := IntToStr(GetOrdValue);
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TIntegerProperty.SetValue(const Value: string);

  procedure Error(const Args: array of const);
  begin
    raise EPropertyError.CreateResFmt(@SOutOfRange, Args);
  end;
var
  L: Int64;
begin
  L := StrToInt64(Value);
{  with GetTypeData(GetPropType)^ do
    if OrdType = otULong then
    begin   // unsigned compare and reporting needed
      if (L < Cardinal(MinValue)) or (L > Cardinal(MaxValue)) then
        // bump up to Int64 to get past the %d in the format string
        Error([Int64(Cardinal(MinValue)), Int64(Cardinal(MaxValue))]);
    end
    else if (L < MinValue) or (L > MaxValue) then
      Error([MinValue, MaxValue]);}
  SetOrdValue(L);
end;

{ TEnumProperty }

function TEnumProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paValueList, paSortList, paRevertable];
end;

function TEnumProperty.GetValue: string;
var
  L: Longint;
  indx: Longint;
  enm: TDSOEnum;
begin
  try
    L := GetOrdValue;
//!!  with GetTypeData(GetPropType)^ do
//!!    if (L < MinValue) or (L > MaxValue) then L := MaxValue;
//!!  Result := GetEnumName(GetPropType, L);
//  Result := GetEnumName(GetPropType, L);
    indx := GetPropInfo.Enum;
    enm := GetPropInfo.Parent.Enums[indx];
    indx := enm.IndexOfItemID(L, 0);
    Result := TEnumItem(enm.Items[indx]^).Name
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TEnumProperty.GetValues(Proc: TGetStrProc);
var
  I: Integer;
  Enum: TDSOEnum;
begin
  Enum := GetPropInfo.Parent.Enums[GetPropInfo.Enum];
  for I := 0 to Enum.Count - 1 do
    Proc(GetDSOEnumName(Enum, I));
end;

procedure TEnumProperty.SetValue(const Value: string);
var
  I: Integer;
begin
  I := GetDSOEnumValue(GetPropInfo.Parent.Enums[GetPropInfo.Enum], Value);
  if I < 0 then
    raise EPropertyError.CreateRes(@SInvalidPropertyValue);
  SetOrdValue(I);
end;

{ TBoolProperty  }

function TBoolProperty.GetValue: string;
begin
  try
    if GetOrdValue = 0 then
      Result := 'False'
    else
      Result := 'True';
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TBoolProperty.GetValues(Proc: TGetStrProc);
begin
  Proc('False');
  Proc('True');
end;

procedure TBoolProperty.SetValue(const Value: string);
var
  I: Integer;
begin
  if CompareText(Value, 'False') = 0 then
    I := 0
  else if CompareText(Value, 'True') = 0 then
    I := -1
  else
    I := StrToInt(Value);
  SetOrdValue(I);
end;

{ TStringProperty }

function TStringProperty.AllEqual: Boolean;
var
  I: Integer;
  V: string;
begin
  Result := False;
  if PropCount > 1 then
  begin
    V := GetStrValue;
    for I := 1 to PropCount - 1 do
      if GetStrValueAt(I) <> V then Exit;
  end;
  Result := True;
end;

function TStringProperty.GetEditLimit: Integer;
begin
  Result := 255;
  if GetPropType^.Kind = tkWideString then
//!!    Result := GetTypeData(GetPropType)^.MaxLength else
    Result := 255;
end;

function TStringProperty.GetValue: string;
begin
  try
    Result := GetStrValue;
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TStringProperty.SetValue(const Value: string);
begin
  SetStrValue(Value);
end;

type
  TStrEditForm = class(TForm)
  private
    OkBtn,
    CancelBtn: TButton;
    procedure WMSize(var Message: TMessage); message WM_SIZE;
    procedure MemoKeyPress(Sender: TObject; var Key: Char); 
  public
    Memo: TMemo;
    Panel: TPanel;
    constructor Create(AOwner: TComponent); override;
    procedure Execute(PE: TPropertyEditor);
  end;

{ TStrEditForm }

constructor TStrEditForm.Create(AOwner: TComponent);
begin
  inherited CreateNew(AOwner);
  BorderStyle := bsSizeToolWin;
  Position := poScreenCenter;
  FormStyle := fsStayOnTop;

  Panel := TPanel.Create(Self);
  with Panel do
  begin
    Parent := Self;
    Align := alBottom;
    BevelOuter := bvNone;
    Height := 30;
  end;

  OkBtn := TButton.Create(Panel);
  with OkBtn do
  begin
    Parent := Panel;
    Caption := 'Ok';
    ModalResult := mrOk;
    Default := True;
    Top := 4;
  end;

  CancelBtn := TButton.Create(Panel);
  with CancelBtn do
  begin
    Parent := Panel;
    Caption := 'Cancel';
    ModalResult := mrCancel;
    Cancel := True;
    Top := 4;
  end;

  Memo := TMemo.Create(Self);
  with Memo do
  begin
    Parent := Self;
    Align := alClient;
    ScrollBars := ssVertical;
//    WordWrap := False;
    WantTabs := True;
    OnKeyPress := MemoKeyPress;
  end;
end;

type
  THEditor = class(TPropertyEditor) end;

procedure TStrEditForm.MemoKeyPress(Sender: TObject; var Key: Char);
begin
  if Key = #27 then
  begin
    Key := #0;
    Close;
  end;
  inherited;
end;

procedure TStrEditForm.Execute(PE: TPropertyEditor);
var
  S: TStrings;
  IsObj: Boolean;
begin
  Caption := Format('Редактирование свойства "%s"', [PE.GetName]);
//!!  IsObj := PE.GetPropType.Kind = tkClass;
IsObj := False;
  S := nil;
  if IsObj then
  begin
    S := TObject(THEditor(PE).GetOrdValue) as TStrings;
    if Assigned(S) then Memo.Lines.Assign(S);
  end
  else
  begin
    Memo.MaxLength := PE.GetEditLimit;
    Memo.Text := THEditor(PE).GetStrValue;
  end;
  ActiveControl := Memo;

  if inherited ShowModal = mrOk then
    if IsObj and Assigned(S) then
      S.Assign(Memo.Lines)
    else
      THEditor(PE).SetStrValue(Memo.Text);
end;

procedure TStrEditForm.WMSize(var Message: TMessage);
var
  CliWidth: Integer;
begin
  inherited;
  CliWidth := ClientWidth;
  with CancelBtn do
    Left := CliWidth - Width - 2;
  with OkBtn do
    Left := CliWidth - (Width shl 1) - 6;
end;

var
  StrEditForm: TStrEditForm;

procedure EditStrings(PE: TPropertyEditor);
begin
  if not Assigned(StrEditForm) then begin
    StrEditForm := TStrEditForm.Create(Application);
    StrEditForm.Width := 600;
  end;
  StrEditForm.Execute(PE);
end;

{ TMultiLineStrProperty }

procedure TMultiLineStrProperty.Edit;
begin
  EditStrings(Self);
end;

function TMultiLineStrProperty.GetAttributes: TPropertyAttributes;
begin
  Result := inherited GetAttributes + [paDialog];
end;

{ TDateProperty }

function TDateProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paRevertable];
end;

function TDateProperty.GetValue: string;
var
  DT: TDateTime;
begin
  try
    DT := GetFloatValue;
    if DT = 0.0 then Result := '' else
    Result := DateToStr(DT);
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TDateProperty.SetValue(const Value: string);
var
  DT: TDateTime;
begin
  if Value = '' then DT := 0.0
  else DT := StrToDate(Value);
  SetFloatValue(DT);
end;

{ TTimeProperty }

function TTimeProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paRevertable];
end;

function TTimeProperty.GetValue: string;
var
  DT: TDateTime;
begin
  try
    DT := GetFloatValue;
    if DT = 0.0 then Result := '' else
    Result := TimeToStr(DT);
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TTimeProperty.SetValue(const Value: string);
var
  DT: TDateTime;
begin
  if Value = '' then DT := 0.0
  else DT := StrToTime(Value);
  SetFloatValue(DT);
end;

function TDateTimeProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paRevertable];
end;

function TDateTimeProperty.GetValue: string;
var
  DT: TDateTime;
begin
  try
    DT := GetFloatValue;
    if DT = 0.0 then Result := '' else
    Result := DateTimeToStr(DT);
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')';
  end
end;

procedure TDateTimeProperty.SetValue(const Value: string);
var
  DT: TDateTime;
begin
  if Value = '' then DT := 0.0
  else DT := StrToDateTime(Value);
  SetDateTimeValue(DT);
end;


{ TObjectProperty }

function TObjectProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paSubProperties, paReadOnly];
end;

procedure TObjectProperty.GetProperties(Proc: TGetPropEditProc);
begin
  try
    GetDSOObjectProperties(FDSOObject, GetPropInfo.PropObject, Proc);
  finally
  end;
end;

function TObjectProperty.GetValue: string;
var
  Disp: IDispatch;
  DSOCommon: ICommon;
begin
  try
    IUnknown(FDSOObject).QueryInterface(GetPropInfo.Owner.InterfaceGUID, Disp);
    if Disp = nil then exit;
    Disp := GetDispatchProperty(Disp, GetPropInfo.Name);
    DSOCommon := Disp as ICommon;
    FmtStr(Result, '(%s)', [DSOCommon.Name]);
  except
    on E: Exception do Result := '(ERROR: ' + E.Message + ')'; 
  end
//!!  FmtStr(Result, '(%s)', [GetPropInfo.Name]);
end;

{ TDataSourceProperty }

procedure TDataSourceProperty.Edit;
{!!var
  FontDialog: TFontDialog;}
begin
{!!  FontDialog := TFontDialog.Create(Application);
  try
    FontDialog.Font := TFont(GetOrdValue);
    FontDialog.HelpContext := hcDFontEditor;
    FontDialog.Options := FontDialog.Options + [fdShowHelp, fdForceFontExist];
    if FontDialog.Execute then SetOrdValue(Longint(FontDialog.Font));
  finally
    FontDialog.Free;
  end;}
end;

function TDataSourceProperty.GetAttributes: TPropertyAttributes;
begin
  Result := [paMultiSelect, paSubProperties, paDialog, paReadOnly];
end;

procedure TDataSourceProperty.GetProperties(Proc: TGetPropEditProc);
var
  Disp: IDispatch;
begin
  try
    IUnknown(FDSOObject).QueryInterface(GetPropInfo.Owner.InterfaceGUID, Disp);
    if Disp = nil then exit;
    Disp := GetDispatchProperty(Disp, GetPropInfo.Name);
    GetDSOObjectProperties(Disp as ICommon, GetPropInfo.PropObject, Proc);
  finally
  end;
end;

end.
