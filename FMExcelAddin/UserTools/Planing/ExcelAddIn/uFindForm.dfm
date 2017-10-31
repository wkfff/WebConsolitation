object FindForm: TFindForm
  Left = 405
  Top = 308
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = 'Найти'
  ClientHeight = 89
  ClientWidth = 392
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poOwnerFormCenter
  OnClose = FormClose
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object edText: TEdit
    Left = 16
    Top = 16
    Width = 257
    Height = 21
    TabOrder = 0
    Text = 'edText'
  end
  object btnFind: TButton
    Left = 304
    Top = 16
    Width = 75
    Height = 25
    Caption = 'Найти далее'
    TabOrder = 1
    OnClick = btnFindClick
  end
  object btnCancel: TButton
    Left = 304
    Top = 56
    Width = 75
    Height = 25
    Caption = 'Отмена'
    TabOrder = 2
    OnClick = btnCancelClick
  end
  object cbCase: TCheckBox
    Left = 32
    Top = 56
    Width = 121
    Height = 17
    Caption = 'С учетом регистра'
    TabOrder = 3
  end
end
