object StyleManager: TStyleManager
  Left = 341
  Top = 198
  Width = 583
  Height = 314
  BorderIcons = [biSystemMenu, biMaximize]
  Caption = 'Стили'
  Color = clBtnFace
  Constraints.MinHeight = 314
  Constraints.MinWidth = 576
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  Icon.Data = {
    0000010001002020100000000000E80200001600000028000000200000004000
    0000010004000000000080020000000000000000000000000000000000000000
    000000008000008000000080800080000000800080008080000080808000C0C0
    C0000000FF0000FF000000FFFF00FF000000FF00FF00FFFF0000FFFFFF000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    0000000000000000000000000000000000000000000000000000000000000000
    000000000000000000000000000000000000000000000000000000000000FFFF
    FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
    FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
    FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF
    FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF}
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Bevel1: TBevel
    Left = 8
    Top = 240
    Width = 560
    Height = 9
    Anchors = [akLeft, akRight, akBottom]
    Shape = bsTopLine
  end
  object btnOK: TButton
    Left = 416
    Top = 256
    Width = 72
    Height = 25
    Anchors = [akRight, akBottom]
    Caption = 'OK'
    Default = True
    ModalResult = 1
    TabOrder = 0
  end
  object btnCancel: TButton
    Left = 496
    Top = 256
    Width = 72
    Height = 25
    Anchors = [akRight, akBottom]
    Cancel = True
    Caption = 'Отмена'
    ModalResult = 2
    TabOrder = 1
  end
  object Panel3: TPanel
    Left = 8
    Top = 8
    Width = 560
    Height = 224
    Anchors = [akLeft, akTop, akRight, akBottom]
    BevelOuter = bvNone
    Caption = 'Panel3'
    TabOrder = 2
    object Splitter1: TSplitter
      Left = 292
      Top = 0
      Width = 8
      Height = 224
      Cursor = crHSplit
      MinSize = 216
      ResizeStyle = rsUpdate
    end
    object Panel4: TPanel
      Left = 0
      Top = 0
      Width = 292
      Height = 224
      Align = alLeft
      BevelOuter = bvNone
      Caption = 'Panel4'
      TabOrder = 0
      object Label1: TLabel
        Left = 0
        Top = 0
        Width = 84
        Height = 13
        Caption = 'Элементы листа'
      end
      object Panel2: TPanel
        Left = 0
        Top = 16
        Width = 292
        Height = 208
        Align = alBottom
        Anchors = [akLeft, akTop, akRight, akBottom]
        BevelOuter = bvNone
        BorderStyle = bsSingle
        Caption = 'Panel2'
        TabOrder = 0
        object tvElements: TBasicCheckTreeView
          Left = 0
          Top = 0
          Width = 288
          Height = 204
          Align = alClient
          BorderStyle = bsNone
          Constraints.MinWidth = 200
          HideSelection = False
          Indent = 19
          ParentShowHint = False
          ReadOnly = True
          ShowHint = False
          TabOrder = 0
          OnChange = tvElementsChange
          OnCustomDrawItem = tvElementsCustomDrawItem
        end
      end
    end
    object Panel5: TPanel
      Left = 300
      Top = 0
      Width = 260
      Height = 224
      Align = alClient
      BevelOuter = bvNone
      Caption = 'Panel5'
      TabOrder = 1
      object Label2: TLabel
        Left = 3
        Top = 0
        Width = 89
        Height = 13
        Caption = 'Доступные стили'
      end
      object Panel1: TPanel
        Left = 0
        Top = 16
        Width = 260
        Height = 208
        Align = alBottom
        Anchors = [akLeft, akTop, akRight, akBottom]
        BevelOuter = bvNone
        BorderStyle = bsSingle
        Caption = 'Выберите элемент для указания стиля'
        Color = clWindow
        TabOrder = 0
        object tvStyles: TBasicCheckTreeView
          Left = 0
          Top = 0
          Width = 256
          Height = 204
          Align = alClient
          BorderStyle = bsNone
          Constraints.MinWidth = 200
          HideSelection = False
          Indent = 19
          ReadOnly = True
          ShowLines = False
          ShowRoot = False
          TabOrder = 0
          OnChange = tvStylesChange
          OnCustomDrawItem = tvStylesCustomDrawItem
        end
      end
    end
  end
  object btnDefault: TButton
    Left = 8
    Top = 256
    Width = 88
    Height = 25
    Anchors = [akLeft, akBottom]
    Caption = 'По умолчанию'
    TabOrder = 3
    OnClick = btnDefaultClick
  end
end
