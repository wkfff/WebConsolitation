object frmCopyForm: TfrmCopyForm
  Left = 334
  Top = 235
  Width = 424
  Height = 317
  BorderIcons = [biSystemMenu, biMaximize]
  Caption = 'Скопировать лист'
  Color = clBtnFace
  Constraints.MinHeight = 317
  Constraints.MinWidth = 272
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
  Scaled = False
  PixelsPerInch = 96
  TextHeight = 13
  object Panel1: TPanel
    Left = 0
    Top = 0
    Width = 416
    Height = 249
    Align = alClient
    BevelOuter = bvNone
    TabOrder = 1
    object Label2: TLabel
      Left = 8
      Top = 8
      Width = 41
      Height = 13
      Caption = 'В книгу:'
    end
    object Label3: TLabel
      Left = 8
      Top = 51
      Width = 73
      Height = 13
      Caption = 'перед листом:'
    end
    object Label1: TLabel
      Left = 8
      Top = 203
      Width = 90
      Height = 13
      Anchors = [akLeft, akBottom]
      Caption = 'Имя копии листа:'
    end
    object cbBookList: TComboBox
      Left = 8
      Top = 24
      Width = 400
      Height = 21
      Style = csDropDownList
      Anchors = [akLeft, akTop, akRight]
      ItemHeight = 13
      TabOrder = 0
      OnChange = cbBookListChange
    end
    object IsWithoutOrderingInform: TCheckBox
      Left = 8
      Top = 225
      Width = 241
      Height = 17
      Anchors = [akLeft, akBottom]
      Caption = 'Копировать без служебной информации'
      TabOrder = 3
    end
    object lbSheetList: TListBox
      Left = 8
      Top = 68
      Width = 400
      Height = 116
      Anchors = [akLeft, akTop, akRight, akBottom]
      ItemHeight = 13
      TabOrder = 1
    end
    object eNewSheetName: TEdit
      Left = 105
      Top = 197
      Width = 304
      Height = 21
      Anchors = [akLeft, akRight, akBottom]
      TabOrder = 2
    end
  end
  object Panel2: TPanel
    Left = 0
    Top = 249
    Width = 416
    Height = 41
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 0
    object Bevel1: TBevel
      Left = 0
      Top = 0
      Width = 416
      Height = 50
      Align = alTop
      Shape = bsTopLine
    end
    object btAply: TButton
      Left = 256
      Top = 11
      Width = 74
      Height = 22
      Anchors = [akRight, akBottom]
      Caption = 'Применить'
      TabOrder = 0
      OnClick = btAplyClick
    end
    object btExit: TButton
      Left = 336
      Top = 11
      Width = 75
      Height = 22
      Anchors = [akRight, akBottom]
      Caption = 'Выход'
      TabOrder = 1
      OnClick = btExitClick
    end
  end
end
