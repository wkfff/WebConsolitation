(*
  ------------------------------------------------------------------------------
  MDX Эксперт, АС "Финансовый Анализ", НПО "Криста, 2004г.
  ------------------------------------------------------------------------------
  Компонент взят из фриварного пакета и постепенно будет перерабатываться под нас.
  ------------------------------------------------------------------------------
*)
unit fmWizardHeader;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs, comctrls, glCommCl;

type
  TfmWizardHeader = class(TGraphicControl)
  private
    FComments: TStrings;
    FCaptions: TStrings;
    FPageControl: TPageControl;
    FPageNo: integer;
    FCommentFont: TFont;
    FCaptionFont: TFont;
    FCaptionEnabled: boolean;
    FSymbolFont: TFont;
    FSymbol: string;
    FGradient: TGradient;
    FGlyph: TBitmap;
    FBufferedDraw: boolean;
    procedure SetCaptions(const Value: TStrings);
    procedure SetComments(const Value: TStrings);
    procedure SetPageNo(const Value: integer);
    procedure SetCaptionFont(const Value: TFont);
    procedure SetCommentFont(const Value: TFont);
    procedure SetSymbolFont(const Value: TFont);
    procedure SetSymbol(const Value: string);
    procedure SetGradient(const Value: TGradient);
    procedure SetGlyph(const Value: TBitmap);
    function GetGlyph: TBitmap;
    { Private declarations }
  protected
    procedure Notification(AComponent: TComponent; Operation: TOperation); override;
  public
    constructor Create(AOwner: TComponent); override;
    destructor Destroy; override;
    procedure Paint; override;
  published
    property Align default alTop;
    property Anchors;
    property CaptionFont: TFont read FCaptionFont write SetCaptionFont;
    property CommentFont: TFont read FCommentFont write SetCommentFont;
    property SymbolFont: TFont read FSymbolFont write SetSymbolFont;
    property PageNo: integer read FPageNo write SetPageNo;
    property Captions: TStrings read FCaptions write SetCaptions;
    property Comments: TStrings read FComments write SetComments;
    property Symbol: string read FSymbol write SetSymbol;
    property Gradient: TGradient read FGradient write SetGradient;
    property Glyph: TBitmap read GetGlyph write SetGlyph;
    property BufferedDraw: boolean read FBufferedDraw write FBufferedDraw;
    property CaptionEnabled: boolean read FCaptionEnabled write FCaptionEnabled;
//    property PageControl: TPageControl read FPageControl write SetPageControl;
  end;

procedure Register;

implementation
uses glTypes, glUtils;

procedure Register;
begin
  RegisterComponents('FM Controls', [TfmWizardHeader]);
end;

{ TfmWizardHeader }

constructor TfmWizardHeader.Create(AOwner: TComponent);
begin
  inherited;
  ControlStyle := ControlStyle + [csOpaque];
  Align := alTop;
  Height := 60;
  FCaptionEnabled := true;
  FCaptions := TStringList.Create;
  FComments := TStringList.Create;
  FGradient := TGradient.Create;
  FGradient.Active := true;
  FGradient.FromColor := clHighlight;
  FGradient.ToColor := clWindow;
  FGradient.Orientation := fgdVertical;
  FCaptionFont := Font;
  FCaptionFont.Style := [fsBold];
  FCommentFont := TFont.Create;
  FSymbolFont := TFont.Create;
  FSymbolFont.Name := 'Wingdings';
  FSymbolFont.Size := 26;
  FSymbolFont.Color := clHighlightText;
  FSymbolFont.Style := [fsBold];
//  FSymbol := '4';
end;

destructor TfmWizardHeader.Destroy;
begin
  FCaptions.Free;
  FComments.Free;
  FCommentFont.Free;
  FGradient.Free;
  FSymbolFont.Free;
  if Assigned(FGlyph) then FGlyph.Free;
  inherited;
end;

function TfmWizardHeader.GetGlyph: TBitmap;
begin
  if not Assigned(FGlyph) then FGlyph := TBitmap.Create;
  Result := FGlyph;
end;

procedure TfmWizardHeader.Notification(AComponent: TComponent; Operation: TOperation);
begin
  inherited Notification(AComponent, Operation);
  if (AComponent = FPageControl) and (Operation = opRemove) then
  begin FPageControl := nil; end;
end;

procedure TfmWizardHeader.Paint;
var
  R, SR: TRect;
  Offset: integer;
  Buffer: TBitmap;
  Caption, Comment: string;
  TargetCanvas: TCanvas;
begin
  FSymbol := copy(FSymbol, 1, 1);

  if BufferedDraw then
  begin
    Buffer := TBitmap.Create;
    Buffer.Width := Width;
    Buffer.Height := Height;
    TargetCanvas := Buffer.Canvas;
  end
  else
  begin
    Buffer := nil;
    TargetCanvas := Canvas;
  end;

  try

  if FCaptions.Count = 0 then Caption := 'Caption' else Caption := FCaptions[ Min(FCaptions.Count-1, PageNo) ];
  if FComments.Count = 0 then Comment := 'Some comment text' else Comment := FComments[ Min(FComments.Count-1, PageNo) ];

  R := ClientRect;

//Зверев: Голубые или белые мастера  
  TargetCanvas.Brush.Color := clWindow; //$00EAEDEF;
  TargetCanvas.FillRect(R);


  Inc(R.Left, 20); Dec(R.Right, 60);
  Inc(R.Top, 8); Dec(R.Bottom, 5);

  if CaptionEnabled then begin
    TargetCanvas.Font.Assign(CaptionFont);
    DrawText(TargetCanvas.Handle, PChar(Caption), length(Caption), R, DT_SINGLELINE);
    inc(R.Top, TargetCanvas.TextHeight('Hy')); inc(R.Left, 20);
  end;


//Зверев.
//Не будем вообще выводить заголовок, только пояснение

//

  TargetCanvas.Font.Assign(CommentFont);
  SR := R;
  DrawText(TargetCanvas.Handle, PChar(Comment), length(Comment), SR, DT_WORDBREAK or DT_CALCRECT);

  if CaptionEnabled then
    OffsetRect(SR, 0, (R.Bottom - SR.Bottom) div 2);

  DrawText(TargetCanvas.Handle, PChar(Comment), length(Comment), SR, DT_WORDBREAK);

  if Assigned(FGlyph)and(FGlyph.Width > 0) then
  begin
    R := ClientRect;
    Offset := (Height - FGlyph.Height) div 2;

//    BitBlt(TargetCanvas.Handle, R.Right-FGlyph.Width-Offset, R.Top+Offset, FGlyph.Width, FGlyph.Height, FGlyph.TargetCanvas.Handle, 0, 0, SRCCOPY);
    DrawBitmapExt(TargetCanvas.Handle, FGlyph, R, R.Right-FGlyph.Width-Offset, R.Top+Offset,
		  fwoNone, fdsDefault, true, GetTransparentColor(FGlyph, ftcLeftBottomPixel), 0);
  end else
  if length(FSymbol) > 0 then
  begin
    TargetCanvas.Brush.Color := clHighlight;
    R := ClientRect;
    SR := Rect(R.Right-50, R.Top+5, R.Right-5, R.Bottom-5 );
    if Assigned(Gradient) and Gradient.Active then dec(SR.Bottom, 3);
    TargetCanvas.FillRect(SR);

    TargetCanvas.Font.Assign(SymbolFont);
    SetBkMode(TargetCanvas.Handle, TRANSPARENT);
    DrawText(TargetCanvas.Handle, PChar(Symbol), length(FSymbol), SR, DT_SINGLELINE or DT_CENTER or DT_VCENTER);
  end;

  R := ClientRect;
  DrawEdge(TargetCanvas.Handle, R, EDGE_ETCHED, BF_BOTTOM);

  if Gradient.Active then
    GradientBox( TargetCanvas.Handle, Rect(R.Left, R.Bottom-5, R.Right, R.Bottom-1), Gradient, 1, 1);

  if BufferedDraw then
    BitBlt(Canvas.Handle, 0, 0, Width, Height, TargetCanvas.Handle, 0, 0, SRCCOPY);

  finally
    if BufferedDraw then Buffer.Free;
  end;
end;

procedure TfmWizardHeader.SetCaptionFont(const Value: TFont);
begin
  FCaptionFont.Assign(Value);
  Invalidate;
end;

procedure TfmWizardHeader.SetCaptions(const Value: TStrings);
begin
  FCaptions.Assign(Value);
  Invalidate;
end;

procedure TfmWizardHeader.SetCommentFont(const Value: TFont);
begin
  FCommentFont.Assign(Value);
  Invalidate;
end;

procedure TfmWizardHeader.SetComments(const Value: TStrings);
begin
  FComments.Assign(Value);
  Invalidate;
end;

procedure TfmWizardHeader.SetGlyph(const Value: TBitmap);
begin
  if not Assigned(FGlyph) then FGlyph := TBitmap.Create;
  FGlyph.Assign(Value);
end;

procedure TfmWizardHeader.SetGradient(const Value: TGradient);
begin
  FGradient.Assign(Value);
end;

procedure TfmWizardHeader.SetPageNo(const Value: integer);
begin
  FPageNo := Value;
  Paint;
end;

procedure TfmWizardHeader.SetSymbol(const Value: string);
begin
  FSymbol := Value;
  Invalidate;
end;

procedure TfmWizardHeader.SetSymbolFont(const Value: TFont);
begin
  FSymbolFont.Assign(Value);
  Invalidate;
end;

end.
