object frmSplitterPad: TfrmSplitterPad
  Left = 560
  Top = 223
  BorderStyle = bsNone
  Caption = 'Вставка разрыва'
  ClientHeight = 267
  ClientWidth = 214
  Color = 16053750
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  FormStyle = fsStayOnTop
  OldCreateOrder = False
  Scaled = False
  OnCreate = FormCreate
  OnHide = FormHide
  OnShow = FormShow
  PixelsPerInch = 96
  TextHeight = 13
  object TitleBar: TfmTitlebar
    Left = 3
    Top = 3
    Width = 208
    Height = 15
    ActiveTextColor = clWhite
    InactiveTextColor = clWhite
    TitlebarColor = clGray
    Font.Charset = RUSSIAN_CHARSET
    Font.Color = clWindowText
    Font.Height = -9
    Font.Name = 'MS Sans Serif'
    Font.Style = [fsBold]
    Caption = 'Вставка разрыва'
    OnMouseMove = pMainContainerMouseMove
    object btClose: TfmExtSpeedButton
      Left = 192
      Top = 1
      Width = 14
      Height = 13
      Anchors = [akTop, akRight]
      Enabled = True
      Font.Charset = DEFAULT_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      Glyph.Data = {
        C2010000424DC20100000000000036000000280000000C0000000B0000000100
        1800000000008C01000000000000000000000000000000000000808080808080
        8080808080808080808080808080808080808080808080808080808080808080
        8080808080808080808080808080808080808080808080808080808080808080
        8080808080808080FFFFFFFFFFFF808080808080808080808080FFFFFFFFFFFF
        808080808080808080808080808080FFFFFFFFFFFF808080808080FFFFFFFFFF
        FF808080808080808080808080808080808080808080FFFFFFFFFFFFFFFFFFFF
        FFFF808080808080808080808080808080808080808080808080808080FFFFFF
        FFFFFF808080808080808080808080808080808080808080808080808080FFFF
        FFFFFFFFFFFFFFFFFFFF808080808080808080808080808080808080808080FF
        FFFFFFFFFF808080808080FFFFFFFFFFFF808080808080808080808080808080
        FFFFFFFFFFFF808080808080808080808080FFFFFFFFFFFF8080808080808080
        8080808080808080808080808080808080808080808080808080808080808080
        8080808080808080808080808080808080808080808080808080808080808080
        808080808080}
      NumGlyphs = 2
      ParentFont = False
      OnClick = btCloseClick
      OnMouseDown = btCloseMouseDown
      OnMouseUp = btCloseMouseUp
      FrameColorActive = 7021576
      FrameColor = clGray
      DefaultStyle = False
      ModalResult = 0
      Style.Color = clGray
      Style.DelineateColor = clBlack
      Style.Font.Charset = DEFAULT_CHARSET
      Style.Font.Color = clWindowText
      Style.Font.Height = -11
      Style.Font.Name = 'MS Sans Serif'
      Style.Font.Style = []
      Style.Bevel.Inner = bvNone
      Style.Bevel.Outer = bvNone
      Style.Bevel.Bold = False
      Style.TextStyle = fstNone
      Style.Gradient.Active = False
      Style.Gradient.Orientation = fgdHorizontal
      Style.TextGradient.Active = False
      Style.TextGradient.Orientation = fgdHorizontal
      StyleActive.Color = 11899525
      StyleActive.DelineateColor = clBlack
      StyleActive.Font.Charset = DEFAULT_CHARSET
      StyleActive.Font.Color = clWindowText
      StyleActive.Font.Height = -11
      StyleActive.Font.Name = 'MS Sans Serif'
      StyleActive.Font.Style = []
      StyleActive.Bevel.Inner = bvNone
      StyleActive.Bevel.Outer = bvNone
      StyleActive.Bevel.Bold = False
      StyleActive.TextStyle = fstNone
      StyleActive.Gradient.Active = False
      StyleActive.Gradient.Orientation = fgdHorizontal
      StyleActive.TextGradient.Active = False
      StyleActive.TextGradient.Orientation = fgdHorizontal
      StylePushed.Color = 11899525
      StylePushed.DelineateColor = clBlack
      StylePushed.Font.Charset = DEFAULT_CHARSET
      StylePushed.Font.Color = clWhite
      StylePushed.Font.Height = -11
      StylePushed.Font.Name = 'MS Sans Serif'
      StylePushed.Font.Style = []
      StylePushed.Bevel.Inner = bvNone
      StylePushed.Bevel.Outer = bvNone
      StylePushed.Bevel.Bold = False
      StylePushed.TextStyle = fstNone
      StylePushed.Gradient.Active = False
      StylePushed.Gradient.Orientation = fgdHorizontal
      StylePushed.TextGradient.Active = False
      StylePushed.TextGradient.Orientation = fgdHorizontal
    end
  end
  object pShaperTop: TPanel
    Left = 0
    Top = 0
    Width = 214
    Height = 2
    Align = alTop
    BevelOuter = bvNone
    Color = clGray
    TabOrder = 1
    OnMouseMove = pMainContainerMouseMove
  end
  object pShaperLeft: TPanel
    Left = 0
    Top = 3
    Width = 2
    Height = 259
    Align = alLeft
    BevelOuter = bvNone
    Color = clGray
    TabOrder = 2
  end
  object pShaperRightInner: TPanel
    Left = 211
    Top = 3
    Width = 1
    Height = 259
    Align = alRight
    BevelOuter = bvNone
    Color = 16053750
    TabOrder = 3
  end
  object pShaperBottom: TPanel
    Left = 0
    Top = 265
    Width = 214
    Height = 2
    Align = alBottom
    BevelOuter = bvNone
    Color = clGray
    TabOrder = 4
  end
  object pShaperLeftInner: TPanel
    Left = 2
    Top = 3
    Width = 1
    Height = 259
    Align = alLeft
    BevelOuter = bvNone
    Color = 16053750
    TabOrder = 5
    OnMouseMove = pMainContainerMouseMove
  end
  object pShaperRight: TPanel
    Left = 212
    Top = 3
    Width = 2
    Height = 259
    Align = alRight
    BevelOuter = bvNone
    Color = clGray
    TabOrder = 6
    OnMouseMove = pMainContainerMouseMove
  end
  object pShaperTopInner: TPanel
    Left = 0
    Top = 2
    Width = 214
    Height = 1
    Align = alTop
    BevelOuter = bvNone
    Color = 16053750
    TabOrder = 7
    OnMouseMove = pMainContainerMouseMove
    object Panel2: TPanel
      Left = 0
      Top = 0
      Width = 2
      Height = 1
      Align = alLeft
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 0
    end
    object Panel3: TPanel
      Left = 212
      Top = 0
      Width = 2
      Height = 1
      Align = alRight
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 1
    end
    object PlotBlack1: TPanel
      Left = 2
      Top = 0
      Width = 1
      Height = 1
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 2
    end
    object PlotBlack2: TPanel
      Left = 211
      Top = 0
      Width = 1
      Height = 1
      Anchors = [akTop, akRight]
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 3
    end
  end
  object pShaperBottomInner: TPanel
    Left = 0
    Top = 262
    Width = 214
    Height = 3
    Align = alBottom
    BevelOuter = bvNone
    Color = 16053750
    TabOrder = 8
    OnMouseMove = pMainContainerMouseMove
    object Panel5: TPanel
      Left = 0
      Top = 0
      Width = 2
      Height = 3
      Align = alLeft
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 0
    end
    object Panel6: TPanel
      Left = 212
      Top = 0
      Width = 2
      Height = 3
      Align = alRight
      BevelOuter = bvNone
      Color = clGray
      TabOrder = 1
    end
  end
  object pMainContainer: TPanel
    Left = 3
    Top = 18
    Width = 206
    Height = 245
    AutoSize = True
    BevelOuter = bvNone
    BorderWidth = 10
    Color = 16053750
    TabOrder = 9
    OnMouseMove = pMainContainerMouseMove
    object pFilterLayer: TPanel
      Left = 10
      Top = 10
      Width = 186
      Height = 15
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 0
      OnMouseMove = pMainContainerMouseMove
      object pFilterLabel: TfmFlatPanel
        Tag = 1
        Left = 0
        Top = 0
        Width = 81
        Height = 15
        Caption = 'Фильтры'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = 16777164
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alLeft
        TabOrder = 0
        UseDockManager = True
        OnClick = pFilterLabelClick
        OnMouseMove = pFilterLabelMouseMove
      end
    end
    object pColumnTitlesLayer: TPanel
      Left = 10
      Top = 70
      Width = 186
      Height = 15
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 1
      OnMouseMove = pMainContainerMouseMove
      object pColumnTitlesLabel: TfmFlatPanel
        Tag = 1
        Left = 81
        Top = 0
        Width = 70
        Height = 15
        Caption = 'Заголовки'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = clSilver
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alLeft
        TabOrder = 0
        UseDockManager = True
        OnClick = pColumnTitlesLabelClick
        OnMouseMove = pColumnTitlesLabelMouseMove
      end
      object pColumnsTitleLeftDummy: TPanel
        Left = 0
        Top = 0
        Width = 81
        Height = 15
        Align = alLeft
        BevelOuter = bvNone
        Color = 16053750
        TabOrder = 1
        OnMouseMove = pMainContainerMouseMove
      end
    end
    object pColumnsLayer: TPanel
      Left = 10
      Top = 100
      Width = 186
      Height = 30
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 2
      object pColumnsLabel: TfmFlatPanel
        Tag = 1
        Left = 81
        Top = 0
        Width = 105
        Height = 30
        Caption = 'Ось столбцов'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = 16777164
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alClient
        TabOrder = 0
        UseDockManager = True
        OnClick = pColumnsLabelClick
        OnMouseMove = pColumnsLabelMouseMove
      end
      object pColumnsLeftDummy: TPanel
        Left = 0
        Top = 0
        Width = 81
        Height = 30
        Align = alLeft
        BevelOuter = bvNone
        Color = 16053750
        TabOrder = 1
        OnMouseMove = pMainContainerMouseMove
      end
    end
    object pTotalTitlesLayer: TPanel
      Left = 10
      Top = 145
      Width = 186
      Height = 15
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 3
      object pRowTitlesLabel: TfmFlatPanel
        Tag = 1
        Left = 0
        Top = 0
        Width = 81
        Height = 15
        PanelSides = [fsdLeft, fsdTop, fsdBottom]
        Caption = 'Заголовки'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = clSilver
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alLeft
        TabOrder = 0
        UseDockManager = True
        OnClick = pRowTitlesLabelClick
        OnMouseMove = pRowTitlesLabelMouseMove
      end
      object pTotalTitlesLabel: TfmFlatPanel
        Tag = 1
        Left = 81
        Top = 0
        Width = 105
        Height = 15
        PanelSides = [fsdTop, fsdRight, fsdBottom]
        Caption = 'Заголовки'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = 52479
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alClient
        TabOrder = 1
        UseDockManager = True
        OnClick = pTotalTitlesLabelClick
        OnMouseMove = pTotalTitlesLabelMouseMove
      end
    end
    object pDataLayer: TPanel
      Left = 10
      Top = 175
      Width = 186
      Height = 30
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 4
      object pRowLabel: TfmFlatPanel
        Tag = 1
        Left = 0
        Top = 0
        Width = 81
        Height = 30
        PanelSides = [fsdLeft, fsdTop, fsdBottom]
        Caption = 'Ось строк'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        Color = 16777164
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alLeft
        TabOrder = 0
        UseDockManager = True
        OnClick = pRowLabelClick
        OnMouseMove = pRowLabelMouseMove
      end
      object pTotalsLabel: TfmFlatPanel
        Tag = 1
        Left = 81
        Top = 0
        Width = 105
        Height = 30
        Caption = 'Данные'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clBlack
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ParentColor = True
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alClient
        TabOrder = 1
        UseDockManager = True
        OnClick = pTotalsLabelClick
        OnMouseMove = pTotalsLabelMouseMove
      end
    end
    object pTaskInfoLayer: TPanel
      Left = 10
      Top = 220
      Width = 186
      Height = 15
      Align = alTop
      BevelOuter = bvNone
      Color = 16053750
      TabOrder = 5
      OnMouseMove = pMainContainerMouseMove
      object pTaskInfoLabel: TfmFlatPanel
        Tag = 1
        Left = 0
        Top = 0
        Width = 129
        Height = 15
        Caption = 'Описание задачи'
        Font.Charset = RUSSIAN_CHARSET
        Font.Color = clGray
        Font.Height = -11
        Font.Name = 'MS Sans Serif'
        Font.Style = []
        ParentColor = True
        BorderColorTop = clGray
        BorderColorRight = clGray
        BorderColorBottom = clGray
        BorderColorLeft = clGray
        BorderWidth = 1
        Align = alLeft
        TabOrder = 0
        UseDockManager = True
        OnClick = pTaskInfoLabelClick
        OnMouseMove = pTaskInfoLabelMouseMove
      end
    end
    object pSplit1: TfmFlatPanel
      Left = 10
      Top = 25
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 6
      UseDockManager = True
      OnMouseDown = pSplit1MouseDown
      OnMouseMove = pSplit1MouseMove
    end
    object pSplit3: TfmFlatPanel
      Left = 10
      Top = 85
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 7
      UseDockManager = True
      OnMouseDown = pSplit3MouseDown
      OnMouseMove = pSplit3MouseMove
    end
    object pSplit4: TfmFlatPanel
      Left = 10
      Top = 130
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 8
      UseDockManager = True
      OnMouseDown = pSplit4MouseDown
      OnMouseMove = pSplit4MouseMove
    end
    object pSplit5: TfmFlatPanel
      Left = 10
      Top = 160
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 9
      UseDockManager = True
      OnMouseDown = pSplit5MouseDown
      OnMouseMove = pSplit5MouseMove
    end
    object pSplit6: TfmFlatPanel
      Left = 10
      Top = 205
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 10
      UseDockManager = True
      OnMouseDown = pSplit6MouseDown
      OnMouseMove = pSplit6MouseMove
    end
    object pUnitMarkerLayer: TPanel
      Left = 10
      Top = 40
      Width = 186
      Height = 15
      Align = alTop
      Alignment = taLeftJustify
      BevelOuter = bvNone
      Caption = '(тыс.руб)'
      Color = 16053750
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clWindowText
      Font.Height = -11
      Font.Name = 'Arial'
      Font.Style = [fsBold]
      ParentFont = False
      TabOrder = 11
      OnClick = pUnitMarkerLayerClick
      OnMouseMove = pMainContainerMouseMove
    end
    object pSplit2: TfmFlatPanel
      Left = 10
      Top = 55
      Width = 186
      Height = 15
      PanelSides = []
      Caption = '                           Разрыв...'
      Font.Charset = RUSSIAN_CHARSET
      Font.Color = clBlack
      Font.Height = -11
      Font.Name = 'MS Sans Serif'
      Font.Style = []
      ParentColor = True
      BorderColorTop = clBlack
      BorderColorRight = clBlack
      BorderColorBottom = clBlack
      BorderColorLeft = clBlack
      BorderWidth = 1
      Align = alTop
      Alignment = taLeftJustify
      TabOrder = 12
      UseDockManager = True
      OnMouseDown = pSplit2MouseDown
      OnMouseMove = pSplit2MouseMove
    end
  end
end
