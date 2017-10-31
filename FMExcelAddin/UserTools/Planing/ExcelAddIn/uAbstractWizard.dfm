object fmAbstractWizard: TfmAbstractWizard
  Left = 411
  Top = 293
  BorderIcons = [biSystemMenu, biMaximize]
  BorderStyle = bsSingle
  ClientHeight = 387
  ClientWidth = 550
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
  PixelsPerInch = 96
  TextHeight = 13
  object WHeader: TfmWizardHeader
    Left = 0
    Top = 0
    Width = 550
    Height = 80
    CaptionFont.Charset = DEFAULT_CHARSET
    CaptionFont.Color = clWindowText
    CaptionFont.Height = -11
    CaptionFont.Name = 'MS Sans Serif'
    CaptionFont.Style = [fsBold]
    CommentFont.Charset = DEFAULT_CHARSET
    CommentFont.Color = clWindowText
    CommentFont.Height = -11
    CommentFont.Name = 'MS Sans Serif'
    CommentFont.Style = []
    SymbolFont.Charset = DEFAULT_CHARSET
    SymbolFont.Color = clHighlightText
    SymbolFont.Height = -35
    SymbolFont.Name = 'Wingdings'
    SymbolFont.Style = [fsBold]
    PageNo = 0
    Captions.Strings = (
      'caption')
    Comments.Strings = (
      'comment')
    Gradient.FromColor = clHighlight
    Gradient.ToColor = clWindow
    Gradient.Active = True
    Gradient.Orientation = fgdVertical
    BufferedDraw = False
    CaptionEnabled = True
  end
  object pnButtons: TPanel
    Left = 0
    Top = 347
    Width = 550
    Height = 40
    Align = alBottom
    BevelOuter = bvNone
    TabOrder = 0
    object Bevel1: TBevel
      Left = 8
      Top = 0
      Width = 537
      Height = 16
      Anchors = [akLeft, akTop, akRight]
      Shape = bsTopLine
    end
    object btnDone: TButton
      Left = 462
      Top = 8
      Width = 75
      Height = 25
      Anchors = [akTop, akRight]
      Caption = 'Готово'
      Default = True
      ModalResult = 1
      TabOrder = 3
      OnClick = btnDoneClick
    end
    object btnCancel: TButton
      Left = 390
      Top = 8
      Width = 75
      Height = 25
      Anchors = [akTop, akRight]
      Cancel = True
      Caption = 'Отмена'
      ModalResult = 2
      TabOrder = 2
    end
    object btnNext: TButton
      Left = 294
      Top = 8
      Width = 75
      Height = 25
      Anchors = [akTop, akRight]
      Caption = 'Далее >'
      TabOrder = 1
      OnClick = btnNextClick
    end
    object btnBack: TButton
      Left = 222
      Top = 8
      Width = 75
      Height = 25
      Anchors = [akTop, akRight]
      Caption = '< Назад'
      TabOrder = 0
      OnClick = btnBackClick
    end
  end
  object pcMain: TPageControl
    Left = 0
    Top = 80
    Width = 550
    Height = 267
    Align = alClient
    Style = tsButtons
    TabOrder = 1
  end
end
