object frmQueryRange: TfrmQueryRange
  Left = 420
  Top = 255
  BorderIcons = [biSystemMenu]
  BorderStyle = bsSingle
  Caption = 'Установка связи с листом (Excel)'
  ClientHeight = 206
  ClientWidth = 403
  Color = clBtnFace
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
  OnDestroy = FormDestroy
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 16
    Top = 8
    Width = 30
    Height = 13
    Caption = 'Книга'
  end
  object Label2: TLabel
    Left = 16
    Top = 56
    Width = 25
    Height = 13
    Caption = 'Лист'
  end
  object Label3: TLabel
    Left = 16
    Top = 104
    Width = 51
    Height = 13
    Caption = 'Диапазон'
  end
  object Bevel1: TBevel
    Left = 16
    Top = 160
    Width = 376
    Height = 17
    Shape = bsTopLine
  end
  object edBook: TEdit
    Left = 16
    Top = 24
    Width = 288
    Height = 21
    TabOrder = 0
  end
  object btnOpen: TButton
    Left = 320
    Top = 24
    Width = 72
    Height = 21
    Caption = 'Открыть...'
    TabOrder = 1
    OnClick = btnOpenClick
  end
  object cmbSheet: TComboBox
    Left = 16
    Top = 72
    Width = 288
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 2
    OnClick = cmbSheetClick
    OnDropDown = cmbSheetDropDown
  end
  object btnCancel: TButton
    Left = 321
    Top = 176
    Width = 72
    Height = 21
    Cancel = True
    Caption = 'Отмена'
    TabOrder = 3
    OnClick = btnCancelClick
  end
  object btnOK: TButton
    Left = 233
    Top = 176
    Width = 72
    Height = 21
    Caption = 'OK'
    Default = True
    TabOrder = 4
    OnClick = btnOKClick
  end
  object btnSelect: TButton
    Left = 320
    Top = 120
    Width = 72
    Height = 21
    Caption = 'Другой...'
    TabOrder = 5
    OnClick = btnSelectClick
  end
  object cmbRange: TComboBox
    Left = 16
    Top = 120
    Width = 288
    Height = 21
    Style = csDropDownList
    ItemHeight = 13
    TabOrder = 6
    OnClick = cmbRangeClick
  end
  object dlgOpen: TOpenDialog
    DefaultExt = 'xls'
    Filter = 'Книга Microsoft Excel (*.xls)|*.xls'
    Left = 344
    Top = 80
  end
end
