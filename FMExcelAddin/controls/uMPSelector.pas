unit uMPSelector;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, uSheetObjectModel, uStringsEditor, uFMAddinGeneralUtils, uXmlCatalog;

type

  TAbstractMPSelector = class(TStringsEditor)
  protected
    StateString: string;
    function GetStateString: string; virtual; abstract;
  public
    //constructor Create(AOwner: TComponent); override;
    procedure Load(MPCollection: TSheetMPCollectionInterface); overload; virtual; abstract;
    procedure Save(MPCollection: TSheetMPCollectionInterface); virtual; abstract;
    procedure Clear; reintroduce;
    function Changed: boolean;
    procedure UpdateState;
  end;

  TMPSelector = class(TAbstractMPSelector)
  protected
    function GetStateString: string; override;
    procedure LoadMP(Name, Mask: string; Checked: boolean);
  public
    constructor Create(AOwner: TComponent); override;

    procedure Load(MPCollection: TSheetMPCollectionInterface); overload; override;
    procedure Load(MPCollection: TMemberPropertyCollection); overload;
    procedure Save(MPCollection: TSheetMPCollectionInterface); override;
  end;

  TMPRadioSelector = class(TAbstractMPSelector)
  protected
    function GetStateString: string; override;
    function GetCheckedPropertyName: string;
  public
    constructor Create(AOWner: TComponent); override;
    procedure Load(MPCollection: TSheetMPCollectionInterface); override;
    procedure Save(MPCollection: TSheetMPCollectionInterface); override;
    property CheckedPropertyName: string read GetCheckedPropertyName;
  end;

procedure Register;

implementation

procedure Register;
begin
  RegisterComponents('FM Controls', [TMPSelector]);
  RegisterComponents('FM Controls', [TMPRadioSelector]);
end;

{ TAbstractMPSelector }

function TAbstractMPSelector.Changed: boolean;
var
  NewStateString: string;
begin
  NewStateString := GetStateString;
  result := NewStateString <> StateString;
end;

procedure TAbstractMPSelector.Clear;
begin
  inherited Clear(false);
end;

procedure TAbstractMPSelector.UpdateState;
begin
  StateString := GetStateString;
end;

{ TMPSelector }

constructor TMPSelector.Create(AOwner: TComponent);
begin
  inherited;
  Width := 320;
  AddColumn('Наименование', 200, true);
  AddColumn('Маска', 80, false);
end;

function TMPSelector.GetStateString: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to RowsCount - 1 do
  begin
    // Наименование|Отметка|Маска
    AddTail(result, '|');
    result := result + Rows[i].Value[0];
    AddTail(result, '|');
    result := result + BoolToStr(Rows[i].Checked);
    AddTail(result, '|');
    result := result + Rows[i].Value[1];
  end;
end;

procedure TMPSelector.LoadMP(Name, Mask: string; Checked: boolean);
begin
  AddRow([Name, Mask]).Checked := Checked;
  AddTail(StateString, '|');
  StateString := StateString + Name;
  AddTail(StateString, '|');
  StateString := StateString + BoolToStr(Checked);
  AddTail(StateString, '|');
  StateString := StateString + Mask;
end;

procedure TMPSelector.Load(MPCollection: TSheetMPCollectionInterface);
var
  i: integer;
  MP: TSheetMPElementInterface;
begin
  Clear;
  StateString := '';
  for i := 0 to MPCollection.Count - 1 do
  begin
    MP := MPCollection[i];
    LoadMP(MP.Name, MP.Mask, MP.Checked);
  end;
  Resize;
end;

procedure TMPSelector.Load(MPCollection: TMemberPropertyCollection);
var
  i: integer;
  MP: TMemberProperty;
begin
  Clear;
  StateString := '';
  for i := 0 to MPCollection.Count - 1 do
  begin
    MP := MPCollection[i];
    LoadMP(MP.Name, MP.Mask, false);
  end;
  Resize;
end;

procedure TMPSelector.Save(MPCollection: TSheetMPCollectionInterface);
var
  i: integer;
  MP: TSheetMPElementInterface;
begin
  MPCollection.Clear;
  for i := 0 to RowsCount - 1 do
  begin
    if i < MPCollection.Count then
      MP := MPCollection[i]
    else
      MP := MPCollection.Append;
    MP.Name := Rows[i].Value[0];
    MP.Mask := Rows[i].Value[1];
    MP.Checked := Rows[i].Checked;
  end;
end;

{ TMPRadioSelector }

constructor TMPRadioSelector.Create(AOWner: TComponent);
begin
  inherited;
  Width := 320;
  AddColumn('Наименование', 280, true);
  RadioCheckBoxes := true;
end;

function TMPRadioSelector.GetCheckedPropertyName: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to RowsCount - 1 do
    if Rows[i].Checked then
    begin
      result := Rows[i].Value[0];
      exit;
    end;
end;

function TMPRadioSelector.GetStateString: string;
var
  i: integer;
begin
  result := '';
  for i := 0 to RowsCount - 1 do
  begin
    AddTail(result, '|');
    result := result + BoolToStr(Rows[i].Checked);
  end;
end;

procedure TMPRadioSelector.Load(MPCollection: TSheetMPCollectionInterface);
var
  i: integer;
  MP: TSheetMPElementInterface;
begin
  Clear;
  StateString := '';
  for i := 0 to MPCollection.Count - 1 do
  begin
    MP := MPCollection[i];
    AddRow([MP.Name]).Checked := MP.Checked;
    AddTail(StateString, '|');
    StateString := StateString + BoolToStr(MP.Checked);
  end;
end;

procedure TMPRadioSelector.Save(MPCollection: TSheetMPCollectionInterface);
var
  i: integer;
  MP: TSheetMPElementInterface;
begin
  for i := 0 to MPCollection.Count - 1 do
  begin
    MP := MPCollection[i];
    MP.Checked := Rows[i].Checked;
  end;
end;

end.
