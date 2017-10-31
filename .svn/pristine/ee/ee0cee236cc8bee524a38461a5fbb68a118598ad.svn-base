{
  "Разрывы" листа прланирования (объект + коллекция).
}

unit uSheetBreaks;

interface
uses
  Classes, SysUtils, ExcelXP,
  uFMExcelAddinConst, uFMAddinGeneralUtils,
  uFMAddinExcelUtils, uExcelUtils;


type
  {размещение пользовательских зон разрывов (bp = Break Placement)}
  TBreakPlacement = (bpNone, bpFilters, bpUnitMarker, bpColumnTitles, bpColumns,
    bpRowTitles, bpRows);

  {Разрыв листа}
  TSheetBreak = class
  private
    FStartRow: integer;
    FHeight: integer;
    FPlacement: TBreakPlacement;
  protected
    function GetName: string;
  public
    constructor Create(Place: TBreakPlacement);
    destructor Destroy; override;
    procedure Delete;
    procedure Load(ExcelSheet: ExcelWorksheet);
    property StartRow: integer read FStartRow write FStartRow;
    property Height: integer read FHeight write FHeight;
    property Placement: TBreakPlacement read FPlacement write FPlacement;
    property Name: string read GetName;
  end;

  {Коллекция разрывов}
  TSheetBreakCollection = class
  private
    FBreaks: array[TBreakPlacement] of TSheetBreak;
  protected
    function FindByPlacement(Place: TBreakPlacement): TSheetBreak;
  public
    constructor Create;
    destructor Destroy; override;
    procedure Clear;
    procedure Delete(Place: TBreakPlacement);
    procedure Load(ExcelSheet: ExcelWorksheet);
    property Items[Place: TBreakPlacement]: TSheetBreak
      read FindByPlacement; default;
  end;



implementation


{ TSheetBreak }

constructor TSheetBreak.Create(Place: TBreakPlacement);
begin
  FHeight := 0;
  FPlacement := Place;
end;

destructor TSheetBreak.Destroy;
begin
  inherited;
end;

procedure TSheetBreak.Delete;
begin
  FHeight := 0;
end;

function TSheetBreak.GetName: string;
begin
  case FPlacement of
    bpFilters: result := BuildExcelName(sntFiltersBreak);
    bpUnitMarker: result := BuildExcelName(sntUnitMarkerBreak);
    bpColumnTitles: result := BuildExcelName(sntColumnTitlesBreak);
    bpColumns: result := BuildExcelName(sntColumnsBreak);
    bpRowTitles: result := BuildExcelName(sntRowTitlesBreak);
    bpRows: result := BuildExcelName(sntRowsBreak);
  else
    result := '';
  end;
end;

procedure TSheetBreak.Load(ExcelSheet: ExcelWorkSheet);
var
  NameObj: ExcelXP.Name;
  Range: ExcelRange;
begin
  NameObj := GetNameObject(ExcelSheet, Name);
  if Assigned(NameObj) then
  begin
    try
      Range := NameObj.RefersToRange;
      FStartRow := Range.Row;
      FHeight := Range.Rows.Count;
    except
      Delete;
    end;
  end
  else
    Delete;
end;

{ TSheetBreakCollection }

procedure TSheetBreakCollection.Clear;
var
  Cnt: TBreakPlacement;
begin
  for Cnt := Low(TBreakPlacement) to High(TBreakPlacement) do
    FBreaks[Cnt].Delete;
end;

constructor TSheetBreakCollection.Create;
var
  i: TBreakPlacement;
begin
  for i := Low(TBreakPlacement) to High(TBreakPlacement) do
    FBreaks[i] := TSheetBreak.Create(i);
end;

procedure TSheetBreakCollection.Delete(Place: TBreakPlacement);
begin
  FBreaks[Place].Delete;
end;

destructor TSheetBreakCollection.Destroy;
var
  i: TBreakPlacement;
begin
  for i := Low(TBreakPlacement) to High(TBreakPlacement) do
    FreeAndNil(FBreaks[i]);
  inherited;
end;

function TSheetBreakCollection.FindByPlacement(Place: TBreakPlacement): TSheetBreak;
begin
  result := FBreaks[Place];
end;

procedure TSheetBreakCollection.Load(ExcelSheet: ExcelWorksheet);
var
  Place: TBreakPlacement;
begin
  for Place := Low(TBreakPlacement) to High(TBreakPlacement) do
    FBreaks[Place].Load(ExcelSheet);
end;


end.
