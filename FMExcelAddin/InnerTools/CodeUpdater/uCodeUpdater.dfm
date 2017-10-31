object fmCodeUpdater: TfmCodeUpdater
  Left = 270
  Top = 153
  Width = 456
  Height = 588
  Caption = 'PK_ID Updater'
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  OnCreate = FormCreate
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 24
    Top = 24
    Width = 189
    Height = 13
    Caption = 'Папка с документами для обработки'
  end
  object Label2: TLabel
    Left = 24
    Top = 88
    Width = 81
    Height = 13
    Caption = 'Имя измерения'
  end
  object Label3: TLabel
    Left = 24
    Top = 208
    Width = 304
    Height = 13
    Caption = 'Список переименований  (Старый ПК_ИД = Новый ПК_ИД)'
  end
  object Label4: TLabel
    Left = 24
    Top = 128
    Width = 72
    Height = 13
    Caption = 'Имя иерархии'
  end
  object Label5: TLabel
    Left = 24
    Top = 168
    Width = 47
    Height = 13
    Caption = 'Full Name'
  end
  object edWorkPath: TEdit
    Left = 24
    Top = 40
    Width = 321
    Height = 21
    TabOrder = 0
    Text = 'edWorkPath'
  end
  object Button1: TButton
    Left = 352
    Top = 40
    Width = 75
    Height = 25
    Caption = 'Обзор...'
    TabOrder = 1
    OnClick = Button1Click
  end
  object edDim: TEdit
    Left = 112
    Top = 80
    Width = 233
    Height = 21
    TabOrder = 2
    Text = 'edDim'
  end
  object mList: TMemo
    Left = 24
    Top = 224
    Width = 401
    Height = 161
    Lines.Strings = (
      'mList')
    ScrollBars = ssVertical
    TabOrder = 4
  end
  object btnGo: TButton
    Left = 112
    Top = 400
    Width = 233
    Height = 25
    Caption = 'Поехали!'
    TabOrder = 5
    OnClick = btnGoClick
  end
  object edHier: TEdit
    Left = 112
    Top = 120
    Width = 233
    Height = 21
    TabOrder = 3
    Text = 'edHier'
  end
  object mLog: TMemo
    Left = 24
    Top = 440
    Width = 401
    Height = 113
    Color = clBtnFace
    Lines.Strings = (
      'mLog')
    TabOrder = 6
  end
  object edFullName: TEdit
    Left = 112
    Top = 160
    Width = 233
    Height = 21
    TabOrder = 7
  end
end
