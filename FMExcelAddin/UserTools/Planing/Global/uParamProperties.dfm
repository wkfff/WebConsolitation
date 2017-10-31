object frmParamProperties: TfrmParamProperties
  Left = 428
  Top = 205
  BorderIcons = []
  BorderStyle = bsSingle
  Caption = 'Свойства параметра'
  ClientHeight = 415
  ClientWidth = 354
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  Scaled = False
  PixelsPerInch = 96
  TextHeight = 13
  object Label4: TLabel
    Left = 4
    Top = 7
    Width = 3
    Height = 13
  end
  object GroupBox2: TGroupBox
    Left = 4
    Top = 62
    Width = 345
    Height = 315
    Caption = 'Свойства параметра'
    TabOrder = 1
    object Label1: TLabel
      Left = 11
      Top = 24
      Width = 22
      Height = 13
      Caption = 'Имя'
    end
    object Label2: TLabel
      Left = 11
      Top = 72
      Width = 70
      Height = 13
      Caption = 'Комментарий'
    end
    object lDimensionName: TLabel
      Left = 8
      Top = 280
      Width = 61
      Height = 13
      Caption = 'Измерение:'
    end
    object edParamName: TEdit
      Left = 8
      Top = 40
      Width = 329
      Height = 21
      TabOrder = 0
      OnChange = edParamNameChange
    end
    object meParamComment: TMemo
      Left = 8
      Top = 88
      Width = 329
      Height = 145
      TabOrder = 1
    end
    object cbMultipleSelection: TCheckBox
      Left = 8
      Top = 248
      Width = 145
      Height = 17
      Caption = 'Множественный выбор'
      TabOrder = 2
    end
    object stDimensionName: TStaticText
      Left = 96
      Top = 280
      Width = 241
      Height = 17
      AutoSize = False
      BorderStyle = sbsSunken
      TabOrder = 4
    end
    object cbIsInherited: TCheckBox
      Left = 160
      Top = 248
      Width = 145
      Height = 17
      Caption = 'От родительской задачи'
      Enabled = False
      TabOrder = 3
    end
  end
  object cbParamsList: TComboBox
    Left = 4
    Top = 23
    Width = 345
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 0
    OnChange = cbParamsListChange
  end
  object btbCancel: TBitBtn
    Left = 273
    Top = 385
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'О&тмена'
    ModalResult = 2
    TabOrder = 4
    NumGlyphs = 2
  end
  object stParams: TStaticText
    Left = 8
    Top = 4
    Width = 125
    Height = 17
    Caption = 'Возможные параметры'
    TabOrder = 2
  end
  object bOK: TButton
    Left = 194
    Top = 385
    Width = 75
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = '&ОК'
    Enabled = False
    TabOrder = 3
    OnClick = bOKClick
  end
end
