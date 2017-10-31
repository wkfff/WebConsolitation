unit uProcessFormSink;

interface

uses
  ComAddinUtils, PlaningTools_TLB, ActiveX, Windows;

type

  TOnProcessFormReturn = procedure of object;
  TOnProcessFormClose = procedure of object;
  TOnProcessFormShow = procedure of object;

  TProcessFormSink = class(TBaseSink)
  private
    FOnReturn: TOnProcessFormReturn;
    FOnClose: TOnProcessFormClose;
    FOnShow: TonProcessFormShow;
  protected
    procedure DoReturn; virtual;
    procedure DoClose; virtual;
    procedure DoShow; virtual;
    function DoInvoke (DispID: integer; const IID: TGUID; LocaleID: integer;
      Flags: word; var DPs: TDispParams; PDispIDs: PDispIdList;
      VarResult, ExcepInfo, ArgErr: pointer): HResult; override;
  public
    constructor Create; virtual;
    property OnReturn: TOnProcessFormReturn read FOnReturn write FOnReturn;
    property OnClose: TOnProcessFormClose read FOnClose write FOnClose;
    property OnShow: TonProcessFormShow read FOnShow write FOnShow;
  end;


implementation

{ TProcessFormSink }

constructor TProcessFormSink.Create;
begin
  inherited;
  FSinkIID := IProcessFormEvents;
end;

procedure TProcessFormSink.DoClose;
begin
  if Assigned(FOnClose) then
    FOnClose;
end;

function TProcessFormSink.DoInvoke(DispID: integer; const IID: TGUID;
  LocaleID: integer; Flags: word; var DPs: TDispParams;
  PDispIDs: PDispIdList; VarResult, ExcepInfo, ArgErr: pointer): HResult;
begin
  result := S_OK;
  case DispID of
    1: DoReturn;
    2: DoClose;
    3: DoShow;
  else
    result := DISP_E_MEMBERNOTFOUND;
  end;
end;

procedure TProcessFormSink.DoReturn;
begin
  if Assigned(FOnReturn) then
    FOnReturn;
end;

procedure TProcessFormSink.DoShow;
begin
  if Assigned(FOnShow) then
    FOnShow;
end;

end.
