unit uMaskedMPSelector;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, uSheetObjectModel, Grids;

type

  TMaskedMPSelector = class(TCustomControl)
  private
    FSource: TSheetMPCollectionInterface;
    Names, Masks: TStringList;
    Checks: array of boolean;
    FBorderStyle: TBorderStyle;
    procedure SetBorderStyle(Value: TBorderStyle);
  protected
    procedure Paint; override;
    procedure CreateParams(var Params: TCreateParams); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Load(Source: TSheetMPCollectionInterface);
    procedure SaveChanges;
    procedure Clear;
  published
    property BorderStyle: TBorderStyle read FBorderStyle write SetBorderStyle default bsSingle;
    property Color default clWindow;
    property Height;
    property Width;
  end;

  (*TMaskedMPSelector = class(TStringGrid)
  private
  public
    procedure Clear;
    constructor Create(AOwner: TComponent);
    destructor Destroy; override;
    procedure Load(Source: TSheetMPCollectionInterface);
    procedure Paint;
    procedure SaveChanges;
    procedure SetBorderStyle(Value: TBorderStyle);
  end; *)

procedure Register;

implementation

procedure Register;
begin
  RegisterComponents('FM Controls', [TMaskedMPSelector]);
end;

{ TMaskedMPSelector }

procedure TMaskedMPSelector.Clear;
begin
  Names.Clear;
  Masks.Clear;
  SetLength(Checks, 0);
end;

constructor TMaskedMPSelector.Create(AOwner: TComponent);
begin
  inherited Create(AOwner);
  BorderStyle := bsSingle;
  Color := clWindow;
  Width := 240;
  Height := 160;
  TabStop := True;

  Names := TStringList.Create;
  Masks := TStringList.Create;
  SetLength(Checks, 0);
end;

procedure TMaskedMPSelector.CreateParams(var Params: TCreateParams);
begin
  inherited;
  with Params do
    Style := Style or WS_VSCROLL;
end;

destructor TMaskedMPSelector.Destroy;
begin
  if HandleAllocated then
    DestroyWindowHandle;
  Clear;
  Names.Free;
  Masks.Free;
  inherited Destroy;
end;

procedure TMaskedMPSelector.Load(Source: TSheetMPCollectionInterface);
var
  i: integer;
begin
  FSource := Source;
  Clear;
  SetLength(Checks, Source.Count);
  for i := 0 to Source.Count - 1 do
  begin
    Names.Add(Source[i].Name);
    Masks.Add(Source[i].Mask);
    Checks[i] := Source[i].Checked;
  end;
  Invalidate;
end;

procedure TMaskedMPSelector.Paint;
var
  Rect, InnerRect: TRect;
begin
  Rect := GetClientRect;
  with Canvas do
  begin
    Brush.Color := Color;
    Brush.Style := bsSolid;
    FillRect(Rect);
  end;
  if BorderStyle = bsSingle then
  begin
    Frame3D(Canvas, Rect, clBtnShadow, clBtnHighlight, 1);
    Frame3D(Canvas, Rect, clWindowFrame, clBtnFace, 1);
  end;
end;

procedure TMaskedMPSelector.SaveChanges;
var
  i: integer;
begin
  for i := 0 to FSource.Count - 1 do
  begin
    FSource[i].Mask := Masks[i];
    FSource[i].Checked := Checks[i];
  end;
end;

procedure TMaskedMPSelector.SetBorderStyle(Value: TBorderStyle);
begin
  if BorderStyle <> Value then
  begin
    FBorderStyle := Value;
    Invalidate;
  end;
end;

end.
