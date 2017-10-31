object DetailForm: TDetailForm
  Left = 398
  Top = 370
  BorderIcons = [biSystemMenu]
  BorderStyle = bsDialog
  Caption = 'Ошибка'
  ClientHeight = 273
  ClientWidth = 387
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object Panel1: TPanel
    Left = 0
    Top = 0
    Width = 387
    Height = 113
    Align = alTop
    BevelOuter = bvNone
    TabOrder = 0
    object mError: TMemo
      Left = 0
      Top = 0
      Width = 387
      Height = 81
      Align = alTop
      Color = clMenu
      ReadOnly = True
      ScrollBars = ssVertical
      TabOrder = 0
    end
    object bDetail: TButton
      Left = 229
      Top = 86
      Width = 75
      Height = 20
      Anchors = [akRight, akBottom]
      Caption = 'Детально >>'
      TabOrder = 1
      OnClick = bDetailClick
    end
    object Button1: TButton
      Left = 309
      Top = 86
      Width = 76
      Height = 20
      Anchors = [akRight, akBottom]
      Caption = 'OK'
      TabOrder = 2
      OnClick = Button1Click
    end
  end
  object mDetailError: TMemo
    Left = 0
    Top = 113
    Width = 387
    Height = 160
    Align = alClient
    Color = clMenu
    ReadOnly = True
    ScrollBars = ssVertical
    TabOrder = 1
  end
end
