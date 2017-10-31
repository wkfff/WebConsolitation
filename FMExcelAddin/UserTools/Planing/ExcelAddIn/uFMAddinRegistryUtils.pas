{
  Библиотечный модуль.
  Процедуры работы с реестром (со спецификой листа планирования).
  (!) В этом модуле особенно важно написать дельный камент при объявлении
  функции.
}

unit uFMAddinRegistryUtils;

interface

uses
  Windows, Registry, uFMExcelAddinConst, classes, SysUtils, uGlobalPlaningConst;

  {Реестровые настройки плагина}
  function ReadIntegerRegSetting(ValueName: string; DefaultValue: integer): integer;
  procedure WriteIntegerRegSetting(ValueName: string; Value: integer);

  function ReadBoolRegSetting(ValueName: string; DefaultValue: Boolean): Boolean;
  procedure WriteBoolRegSetting(ValueName: string; Value: Boolean);

  function ReadStrRegSetting(ValueName: string; DefaultValue: String): String;
  procedure WriteStrRegSetting(ValueName: string; Value: String);

  {Конкретные настройки реестра}
  function AddinLogEnable: boolean;
  function AddinLogPath: string;
  procedure CopyKeyToAnotherRoot(Key: string; SourceRoot, DestRoot: HKEY);
  // копируем узел с настройками из локал машин, если нет в каррент юзер
  procedure CopyToHKCU;

implementation

function ReadIntegerRegSetting(ValueName: string; DefaultValue: integer): integer;
var
  Reg: TRegistry;
begin
  result := DefaultValue;
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(regBasePath)) then
    begin
      Reg.OpenKey(regBasePath, false);
      if Reg.ValueExists(ValueName) then
        result := Reg.ReadInteger(ValueName)
      else
      begin
        result := DefaultValue;
        Reg.WriteInteger(ValueName, DefaultValue);
      end;
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

procedure WriteIntegerRegSetting(ValueName: string; Value: integer);
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(regBasePath, true) then
    begin
      Reg.WriteInteger(ValueName, Value);
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

function ReadBoolRegSetting(ValueName: string; DefaultValue: Boolean): Boolean;
var
  Reg: TRegistry;
begin
  result := DefaultValue;
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(regBasePath)) then
    begin
      Reg.OpenKey(regBasePath, false);
      if Reg.ValueExists(ValueName) then
        result := (Reg.ReadInteger(ValueName) <> 0)
      else
      begin
        result := DefaultValue;
        if DefaultValue then
          Reg.WriteInteger(ValueName, 1)
        else
          Reg.WriteInteger(ValueName, 0);
      end;
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

procedure WriteBoolRegSetting(ValueName: string; Value: Boolean);
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(regBasePath, true) then
    begin
      if Value then
        Reg.WriteInteger(ValueName, 1)
      else
        Reg.WriteInteger(ValueName, 0);

      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

function ReadStrRegSetting(ValueName: string; DefaultValue: String): String;
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if (Reg.KeyExists(regBasePath)) then
    begin
      Reg.OpenKey(regBasePath, false);
      if Reg.ValueExists(ValueName) then
        result := Reg.ReadString(ValueName)
      else
      begin
        result := DefaultValue;
        Reg.WriteString(ValueName, DefaultValue);
      end;
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;

end;

procedure WriteStrRegSetting(ValueName: string; Value: String);
var
  Reg: TRegistry;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := HKEY_CURRENT_USER;
    if Reg.OpenKey(regBasePath, true) then
      Reg.WriteString(ValueName, Value);
  finally
    Reg.Free;
  end;
end;

function AddinLogEnable: boolean;
begin
  result := ReadBoolRegSetting(regLogEnableKey, true);
end;

function AddinLogPath: string;
begin
  result := ReadStrRegSetting(regLogPathKey, '.\');
end;

procedure CopyKeyToAnotherRoot(Key: string; SourceRoot, DestRoot: HKEY);
var
  Reg: TRegistry;
  ValueNames: TStringList;
  i: integer;
  Name: string;
  Value: variant;
  DataInfo: TRegDataInfo;
begin
  Reg := TRegistry.Create;
  try
    Reg.RootKey := SourceRoot;
    Reg.OpenKey(Key, false);
    try
      ValueNames := TStringList.Create;
      try
        Reg.GetValueNames(ValueNames);
        if (ValueNames.Count = 0) then
          exit;
        Reg.RootKey := DestRoot;
        Reg.CreateKey(Key);
        for i := 0 to ValueNames.Count - 1 do
        begin
          Reg.RootKey := SourceRoot;
          Reg.OpenKey(Key, false);

          Name := ValueNames.Strings[i];
          Reg.GetDataInfo(Name, DataInfo);
          case DataInfo.RegData of
            rdString:
                Value := Reg.ReadString(ValueNames.Strings[i]);
            rdInteger:
                Value := Reg.ReadInteger(ValueNames.Strings[i]);
          end;

          Reg.RootKey := DestRoot;
          Reg.OpenKey(Key, false);

          case DataInfo.RegData of
            rdString:
                Reg.WriteString(Name, Value);
            rdInteger:
                Reg.WriteInteger(Name, Value);
          end;

        end;
      finally
        FreeAndNil(ValueNames);
      end;
    finally
      Reg.CloseKey;
    end;
  finally
    Reg.Free;
  end;
end;

procedure CopyToHKCU;
var
  Reg: TRegistry;
begin
  try
    Reg := TRegistry.Create;
    try
      Reg.RootKey := HKEY_CURRENT_USER;
      if (Reg.KeyExists(regBasePath)) then
        abort;
      Reg.RootKey := HKEY_LOCAL_MACHINE;
      if not (Reg.KeyExists(regBasePath)) then
        abort;
      {$WARNINGS OFF}
      CopyKeyToAnotherRoot(regBasePath, HKEY(HKEY_LOCAL_MACHINE), HKEY(HKEY_CURRENT_USER));
      {$WARNINGS ON}
    finally
      Reg.Free;
    end;
  except
  end;
end;

end.
