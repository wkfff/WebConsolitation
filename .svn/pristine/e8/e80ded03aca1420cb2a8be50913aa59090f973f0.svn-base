unit uMessage;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  ExtCtrls, StdCtrls, fmFlatPanel, Math;

const
  UM_ReportChangedMessage=WM_USER+77;
  UM_FileOpMessage= WM_USER+79;
  UM_PanelActivatedMessage=WM_USER+81;
  UM_PanelSelectionChange=WM_USER+82;
  UM_NoteSelectionChange=WM_USER+83;
  UM_TitleSelectedMessage=WM_USER+84;
  UM_TableFieldsetAdded=WM_USER+85;
  UM_TableRefreshColorings=WM_USER+87;
  UM_PanelClose=WM_USER+88;
  UM_StructureRepChangedMessage=WM_USER+89;
  UM_ElementDataChangedMessage=WM_USER+90;
  UM_ElementTextChangedMessage=WM_USER+91;

  function MessageWindow(Msg:string; Details:string; DlgType:TMsgDlgType;
    Buttons:TMsgDlgButtons):integer;

implementation

var
  IconIDs: array[TMsgDlgType] of PChar = (IDI_EXCLAMATION, IDI_HAND,
    IDI_ASTERISK, IDI_QUESTION, nil);
  ButtonNames: array[TMsgDlgBtn] of string = (
    'Yes', 'No', 'OK', 'Cancel', 'Abort', 'Retry', 'Ignore', 'All', 'NoToAll',
    'YesToAll', 'Help');
  ButtonCaptions: array[TMsgDlgBtn] of string = (
    'Да', 'Нет', 'OK', 'Отмена', 'Abort', 'Retry', 'Ignore', 'All', 'NoToAll',
    'YesToAll', 'Help');
  ModalResults: array[TMsgDlgBtn] of Integer = (
    mrYes, mrNo, mrOk, mrCancel, mrAbort, mrRetry, mrIgnore, mrAll, mrNoToAll,
    mrYesToAll, 0);

function CreateMessageWindow(Msg:string; Details:string; DlgType:TMsgDlgType;
    Buttons:TMsgDlgButtons):TForm;
const
  HorzMargin = 8;
  VertMargin = 8;
  MsgWidth=280;
//  MsgHeight=40;
  DetailsWidth=200;
  ButtonWidth=72;
  ButtonHeight=25;
  ButtonSpacing=8;
var
  IconID: PChar;
  B, DefaultButton, CancelButton: TMsgDlgBtn;
  ButtonCount, ButtonGroupWidth, X: integer;
  DetailsHeight, MsgHeight: integer;
  p:TfmFlatPanel;

begin
  result:=TForm.Create(Application);
  with result do begin
    BorderStyle:=bsDialog;
    Caption := Application.Title;
    //Image
    IconID := IconIDs[DlgType];
    if IconID <> nil then
      with TImage.Create(Result) do begin
        Name := 'Image';
        Parent := Result;
        Picture.Icon.Handle := LoadIcon(0, IconID);
        SetBounds(HorzMargin, VertMargin, 32, 32);
      end;
    //Message Text
    with TLabel.Create(Result) do begin
      Name := 'Message';
      Height := 0;
      Parent := Result;
      WordWrap := True;
      Caption := Msg;
      BiDiMode := Result.BiDiMode;
      Left := 2*HorzMargin+32;
      Top := VertMargin;
      Width := MsgWidth;
      MsgHeight := Height div 2;
(*      SetBounds(2*HorzMargin+32, VertMargin,
        MsgWidth, MsgHeight);*)
    end;
    //Buttons
    ButtonCount := 0;
    for B := Low(TMsgDlgBtn) to High(TMsgDlgBtn) do
      if B in Buttons
        then Inc(ButtonCount);

    if ButtonCount <> 0 then
      ButtonGroupWidth := ButtonWidth * ButtonCount +
        ButtonSpacing * (ButtonCount - 1)
      else ButtonGroupWidth:=0;
    ClientWidth:= Max(ButtonGroupWidth,32+MsgWidth+ButtonSpacing)+
      HorzMargin*2;

    //Details Text
    DetailsHeight:=0;
    if Details<>''
      then begin
        p:=TfmFlatPanel.Create(Result);
        with p do begin
          DetailsHeight:=80;
          Name:= 'Panel';
          Parent:= result;
          SetBounds(HorzMargin, VertMargin+Max(32,MsgHeight)+ButtonSpacing,
            result.ClientWidth-2*HorzMargin, DetailsHeight);
        end;
        with TMemo.Create(p) do begin
          Name:='Memo';
          Parent:=p;
//          Align:=alClient;
          Color:=clBtnFace;
          ScrollBars:=ssVertical;
          BorderStyle:=bsNone;
          ReadOnly:=true;
          SetBounds(4,4,p.Width-8,p.Height-8);
          Lines.Clear;
          Lines.Add(Details);
        end;
      end;

    ClientHeight := Max(32, MsgHeight) + ButtonHeight + ButtonSpacing +
      VertMargin * 2;
    if Details<>''
      then ClientHeight:= ClientHeight+DetailsHeight+ButtonSpacing;

    if mbOk in Buttons
      then DefaultButton := mbOk
      else if mbYes in Buttons
        then DefaultButton := mbYes
          else DefaultButton := mbRetry;
    if mbCancel in Buttons
      then CancelButton := mbCancel
        else if mbNo in Buttons
          then CancelButton := mbNo
            else CancelButton := mbOk;

//    X := (ClientWidth - ButtonGroupWidth) div 2; //центральное расположение кнопок
    X:=ClientWidth-HorzMargin-ButtonGroupWidth;
    for B := Low(TMsgDlgBtn) to High(TMsgDlgBtn) do
      if B in Buttons then
        with TButton.Create(Result) do begin
          Name := ButtonNames[B];
          Parent := Result;
          Caption := ButtonCaptions[B];
          ModalResult := ModalResults[B];
          if B = DefaultButton
            then Default := True;
          if B = CancelButton
            then Cancel := True;
          if Details<>''
            then SetBounds(X, VertMargin+Max(32,MsgHeight)+2*ButtonSpacing+DetailsHeight,
              ButtonWidth, ButtonHeight)
            else SetBounds(X, VertMargin+Max(32,MsgHeight)+ButtonSpacing,
              ButtonWidth, ButtonHeight);
          Inc(X, ButtonWidth + ButtonSpacing);
        end;
  end;
end;


function MessageWindow(Msg:string; Details:string; DlgType:TMsgDlgType;
    Buttons:TMsgDlgButtons):integer;
begin
  with CreateMessageWindow(Msg, Details, DlgType, Buttons) do
    try
      Position := poScreenCenter;
      Result := ShowModal;
    finally
      Free;
    end;
end;


end.
