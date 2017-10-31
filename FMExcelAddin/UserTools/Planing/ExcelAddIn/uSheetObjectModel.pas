{
  ������ �������� ��������� ������ ����� ������������.
  � ������ ����������� ����� �������, ����������� ���������� ��� ���� ��������,
  ������� ���������� � �����.
  ��� ������ �����������. ��� ��������� ����������, ����� ���������� ��������
  � ������ ����� ������� ������� � ������� �������, ������� ����� ��������
  ������ ��������. �����������, ��� ������ ����������� ��� ����������� �
  ����� ��������� � ��������.
  ��� ����������, ������ �������� ���� �� ����� ������������ ������
  ������������� ��������.
  ������������ ���� (implementation) � ���� ������ �������.
  �������� ������ - �������� �����������. ����������� ������������� ���������
  � ����� ������� ������, ���������� �� ����� ����� �������� � �����������
  �� ����������� (�������� ��������� ������ �� ��������).

  �.� ���� ��������� �������� ����� �����/�������� � ���������� ���������
  ������� ������ (������, � �������). ������, ������ � ����� ������ ������
  ���� ������ �� ������ ��������, ������� ����� ������ �� ���� (������).
  � ���� ������ ����� �������  � ���� ������ �������� ����� ����� �
  ������������ ����� ��� �����������, � ����� ����������� ��� � ����� ��
  �������� ������������� ������ (TSheetFilterInterface -> TSheetFilter).

  ����� ����, � ���� ������ ���� ����� ������� ������, �� ������� �����������
  ���� ������������. � ��� ��, ����� ����� ��������� ����������� ����������
  �� ���� �� �����.  (� ��� ��������� ����� � �������� ����� ������...)
}


unit uSheetObjectModel;

interface

uses
  Classes, SysUtils, ExcelXP, uXMLCatalog, PlaningTools_TLB, MSXML2_TLB,
  PlaningProvider_TLB, uFMAddinExcelUtils, uXMLUtils, uCheckTV2, Windows,
  uSheetBreaks, uFMAddinGeneralUtils, uFMExcelAddInConst,
  uFMAddinXmlUtils, uSheetHistory, uExcelUtils, uOfficeUtils,
  uGlobalPlaningConst, Graphics;

const
  {�� ����� ���� ����������� ��������� ������}
  ttNone = 0;
  ttMeasure = 1;
  ttFree = 2;
  ttResult = 4;
  ttConst = 8;
  ttBase = ttMeasure + ttResult;
  ttAll = ttMeasure + ttFree + ttResult + ttConst;

  {���������� ���������� �� ���}
  tfNone = 0;
  tfPartial = 1;
  tfFull = 2;


type
  // ����� ������� �������
  TTableProcessingMode = (tpmNormal, tpmLarge, tpmHuge);
  // ��� ���������� �����
  TSheetTotalType = (wtFree, wtMeasure, wtResult, wtConst);
  // ��������� �����
  TTotalTypes = set of TSheetTotalType;

  // ��� �������� �����
  TSheetObjectType = (wsoTotal, wsoRow, wsoColumn, wsoFilter, wsoSingleCell, wsoLevel);
  TSheetObjectTypes = set of TSheetObjectType;

  {������ �������� ����������}
  TLoadModeComponent = (lmInner, lmCollections, lmFreeData, lmWithWizard, lmNoMembers, lmCatalogOnly);
  TLoadMode = set of TLoadModeComponent;

  {��� ��������� �����
    1) �� ������������
    2) ��������� - ��� ������� �������, ���������� ���������� �����
    3) ��������� - ��� ������� ������������ �������������}
  THeadingType = (htNoDefine, htTableArea, htCustomArea);
  TPoints = array of Windows.TPoint;

  TItemDeployment = (idNone, idTop, idBottom);

const
  lmNoFreeData = [lmInner, lmCollections];
  lmAll = [lmInner, lmCollections, lmFreeData];

type

  TMsgType = (msgInfo, msgWarning, msgError);

  {��������������� ����������}
  TSheetAxisCollectionInterface = class;
  TSheetBasicTotal = class;
  TSheetFilterCollectionInterface = class;
  TSheetTotalCollectionInterface = class;
  TSheetCollection = class;
  TSheetFilterInterface = class;
  TSheetAxisElementInterface = class;
  TSheetLevelInterface = class;
  TSheetLevelCollectionInterface = class;
  TSheetMPElementInterface = class;
  TSheetMPCollectionInterface = class;
  TSheetSingleCellInterface = class;
  TSheetSingleCellCollectionInterface = class;
  TSheetTotalInterface = class;
  TConstInterface = class;
  TConstCollectionInterface = class;
  TParamInterface = class;
  TParamCollectionInterface = class;
  TSummaryOptions = class;
  TSheetDimension = class;
  TElementStyles = class;
  TFontOptions = class;
  TWritablesInfo = class;
  TTotalCounters = class;
  TTypeFormula = class;


  TTaskContext = class;
  TTaskParamsCollection = class;
  TTaskConstsCollection = class;
  TTaskParam = class;
  TTaskConst = class;
  TTaskEnvironment = class;


  TSheetHeading = record
    Type_: THeadingType;
    End_: TPSObject;
    Address: string;
  end;

  // �������� ����������� ������� "� ������� ������"
  TMarkerPosition = (mpLeft, mpRight, mpHidden);

  {��������� ������� �����/��������}
  TTotalMultiplier = (tmE1, tmE3, tmE6, tmE9);

  TSheetInterface = class
  protected
    {������� ��� ������ ��� �������������, ������������ �� ��������, ��������� ��� �� �������}
    FIsLock: boolean;
    {���� ������������}
    FExcelSheet: ExcelWorkSheet;
    {�������� ������}
    FTaskContext: TTaskContext;
    FTaskEnvironment: TTaskEnvironment;
    {���������� ����}
    FXMLCatalog: TXMLCatalog;
    {����� ��������� ��������}
    FProcess: ProcessForm;
    {������� �����}
    FSheetHistory: TSheetHistory;
    {��������� ������}
    FDataProvider: IPlaningProvider;
    {������� ���������� ��� ����� ��� �������� ������}
    FNeedRound: boolean;
    {��������� - ������, ��������, ���������}
    FTotalMultiplier: TTotalMultiplier;
    {�������� �� ������ �������� ��������}
    FDisplayFullFilterText: boolean;
    {�������� ��������� ������}
    FCountrySetting: integer;
    {����������� ����� � ������� ����� �����}
    FDSeparator: string;
    {����������� ��������}
    FTSeparator: string;
    {����������� ��������� ������}
    FListSeparator: string;
    {URL}
    FURL: string;
    {��� �����}
    FSchemeName: string;
    {���� ���������� ����������}
    FLastRefreshDate: string;
    {������� ���������� � ����� �� �����}
    FIsDisplaySheetInfo: boolean;
    {�������}
    FBreaks: TSheetBreakCollection;
    {�������� �� �������}
    FIsDisplayFilters: boolean;
    {�������� �� ��������� ��������}
    FIsDisplayColumnsTitles: boolean;
    {�������� �� ��������� �����}
    FIsDisplayRowsTitles: boolean;
    {�������� �� ��������� �����������}
    FIsDisplayTotalsTitles: boolean;
    {�������� �� ���� ��� ��������}
    FIsDisplayColumns: boolean;
    FMarkerPosition: TMarkerPosition;
    {���������� ������������ ����� ��� �������}
    FFilterCellsLength: integer;
    {���������� ������ �� ������� ������ �������}
    FIsMergeFilterCellsByTable: boolean;
    {����� ��������}
    FLoadMode: TLoadMode;
    {���� ������� �� �����}
    FIsLoadingFromTask: boolean;
    {����� ������� �������}
    FTableProcessingMode: TTableProcessingMode;
    {��������� ������������� �������� ����������� ��������
     � MDX ������� - NonEmptyCrossJoin}
    FAllowNECJ: boolean;
    {��������� �����}
    FSheetHeading: TSheetHeading;

    {�������� ����������� � ������� �������� �������}
    FIsDisplayCommentStructuralCell: boolean;
    {�������� ����������� � ������� � �������}
    FIsDisplayCommentDataCell: boolean;

    {�����-����� ����� ����� ��� ������}
    FPrintableStyle: boolean;
    {������ XML � ������� ���� �����������}
    FTotalsData: IXMLDOMDocument2;

    { �������� � ���������������� �������� ����� (��������� � ����������)}
    FWritablesInfo: TWritablesInfo;

    { ��������� �� ���� � ������ ��������������� (��������� ����� �������� ��
      ����������/���������/�������� ���������). ������������ - ����� ������
      � ������� - ����� �������� ������ � ���� ����������, � ������� ����
      ���������� ���������� �� ���.}
    FInConstructionMode: boolean;
    { ���������� �� �������������� � "������ ������ � �������". ��� �����
      � ����� ����� ����� "���������� �� ��������� ����������"}
    FPermitEditing: boolean;
    { ������� ������ � ������-������}
    FOnline: boolean;
    {������� ������ - �������� � ������ ���������� � ���� UName_Section,
      ������ �������� - ������������� ����� ������� ����������}
    FTotalSections: TStringList;
    {}
    FTaskId: string;

    {�������� ��� ������� �������� ���������� ��� ������� ����� ������ �� ����.}
    FKillZeroLinkedParams: boolean;

    function Get_Totals: TSheetTotalCollectionInterface; virtual; abstract;
    function Get_Rows: TSheetAxisCollectionInterface; virtual; abstract;
    function Get_Columns: TSheetAxisCollectionInterface; virtual; abstract;
    function Get_Filters: TSheetFilterCollectionInterface; virtual; abstract;
    function Get_SingleCells: TSheetSingleCellCollectionInterface; virtual; abstract;
    function Get_Consts: TConstCollectionInterface; virtual; abstract;
    function Get_Params: TParamCollectionInterface; virtual; abstract;
    function GetProcessShowing: boolean;
    function GetTaskParams: TTaskParamsCollection;
    function GetParamID: integer; virtual; abstract;
    procedure SetTotalsData(Value: IXMLDOMDocument2); virtual; abstract;
    procedure SetPermitEditing(Value: boolean);
    function GetIsSilentMode: boolean;
    procedure SetIsSilentMode(const Value: boolean);
    function GetSheetVersion: string; virtual; abstract;
  public
    {���� ��� ������ ��������������� ��������� ����� - ����� ��� ����, �����
      �� ������������ UID-�}
    InCopyMode: boolean;
    {����������� ���� ��� ������ 11008 - ��� ����������� _�����_ �� ����� �����
      ������ � ����� � ������ ������ �� ������������ (���������) ���������}
    SpecialFlagForTaskParamCopy: boolean;
    // ���� - ������������ � �������
    IsTaskConnectionLoad: boolean;
    // ���� - �������� ��� ��������� ��������� �����
    IsTaskContextLoad: boolean;

    procedure PutData(Dom: IXMLDOMDocument2; UniqueID: string); virtual;
    function GetData(UniqueID: string): IXMLDOMDocument2; virtual;

    function GetUniqueID: string; virtual; abstract;
    {����������� ����������� �����������-���.
     �.� �� ��������� ��������� ��� ��������, ����������, ����� ��
     ���������� ����������� �� ��������.
     ����������, ���������� ���������� �������� - IsIgnoredColumnAxis}
    procedure SetUpMeasuresPosition; virtual; abstract;

    {���� ����� ���������� ��������� � ����� ���������, ����� ������� �� �����}
    procedure ShowMessageEX(AMsg: string; AType: TMsgType);
    procedure ShowDetailedError(Error, Details, Caption: string);
    {���������� ������� ��������� �� ������ � ����� ���������
     (�������� �������������� ��������)}
    procedure PostMessage(AMsg: string; AType: TMsgType);
    procedure OpenOperation(Msg: string; IsCritical, IsTimed: boolean;
      OperationType: integer);
    procedure CloseOperation;
    procedure SetPBarPosition(CurPosition, MaxPosition: integer);

    procedure AddEventInSheetHistory(EventType: TSheetEventType;
      Comment: string; IsSuccess: boolean);
    {��������� ���������� �� � ������ ������ � ����.
     ���� �� ����������, �������� ������� ��������� � ������� ����� ���������.
     ��������� ������������� ������� �����, ��������������� ����� ������
     ����� ��������� ������}
    function CheckConnection: boolean;
    {���������, ������� �� ��� ��������� � �����}
    function IsDimensionUsed(DimName, HierName: string): TSheetObjectType; virtual; abstract;
    function GetDimension(DimensionName: widestring): TSheetDimension; virtual; abstract;
    // �������� ��������� ������ ��������� ���������: [[UniqueName, Name]..[UniqueName, Name]]
    function GetMembersArray(DimensionName: widestring): OleVariant;
    // ���������� ���������� ��������, �������� ������� uniquename
    procedure SetMembersByArray(DimensionName: widestring; UniqueNames: OleVariant);
    {���-�� �������� ������������ ������� ���������}
    function GetFilterCountWithScope(IsPartial: boolean): integer; virtual; abstract;

    procedure Save; overload; virtual; abstract;
    procedure Save(LoadMode: TLoadMode); overload;
    procedure ExportXml(var Dom: IXMLDOMDocument2); virtual; abstract;

    {�������� ������ ��������� ����������� � �����������}
    procedure CollectTotalsData(SingleCellsOnly: boolean); virtual; abstract;
    {���� ������ ������ ���������� ��� ������������ ������ ����������}
    function CollectOneTotalData(Total: TSheetTotalInterface): boolean; virtual;

    {���������� �� ��� ���� ��� (��������� �� ������� ���������)}
    function GetAxis(Axis: TAxisType): TSheetAxisCollectionInterface;

    procedure SetExternalLinks(Catalog: TXMLCatalog; ProcessForm: IProcessForm;
      Provider: IPlaningProvider; SheetHistory: TSheetHistory);

    function CopyCustomProperty(SourceName, DestName: string): boolean;
    function GetPID(ParamName: string): integer;

    function CheckWorkbookForResults: boolean; virtual; abstract;

    {���������� "����������������" ����������� �� ������� �������� ���.
    List1 �������� �������������� ����������, List2 - ����������������.
    ���������� ����� ������ � ����� ������� ������������� ������� �������� ���.
    ������ GrandTotalsOnly �������� ����������, ������ ������������ ��� ����� �
    ��������� ������ � ������� ����.}
    procedure GetFactorization(Axis: TSheetAxisCollectionInterface;
      FromBaseOnly: boolean;
      out List1, List2: TStringList; out GrandTotalsOnly: string); virtual; abstract;

    function MoveElement(ObjType, NewObjType, UID: string): boolean; virtual; abstract;
    function MayBeEdited: boolean;
    { ���������� ��� ���������� �� �������������� � �������� ����� ������������.
      �������� ������������� ��� �������� ������ �����, �� ����������� ������
      ������������.}
    procedure SetDefaultPermissions;
    { ���������� ����������� ���� � ���������� �����}
    procedure SwitchOffline;
    { ��� ��� ���������� ��������� ������ � ����������� � �������� �
      ����������� ��� ������� �����.}
    procedure UpdateFiltersText; virtual; abstract;
    procedure UpdateTotalsComments; virtual; abstract;
    procedure ValidateStyles;

    function GetTypeFormula(Total: TSheetTotalInterface; Row, Column: integer): TTypeFormula; virtual;
    procedure MapTypeFormula(Total: TSheetTotalInterface); virtual;
    function TypeFormulaToString(FormulaTotal: TSheetTotalInterface; CurrentRow, CurrentSectionIndex, GrandSummaryRow: integer): string; virtual; 

    {������� �� ������ ���������� � ���������, ����������� �� ������ ��������� ���������}
    procedure DeleteConstElements(Constant: TConstInterface); virtual; abstract;

    function IsSummaryRow(Row: integer): boolean; virtual; abstract;
    function IsSummaryColumn(Column: integer): boolean; virtual; abstract;
    function IsSummaryCell(Row, Column: integer): boolean; virtual; abstract;
    {���� ��������� ��������� �� ���� ���������� - �������, ��������, �������� �
      �������� ��������� �����������}
    function FindDimensionByUniqueId(UniqueId: string): TSheetDimension; virtual; abstract;

    {������� ��� ������ ��� �������������, ������������ �� ��������, ��������� ��� �� �������}
    property IsLock: boolean read FIsLock write FIsLock;
    {����� ��������� ��������}
    property Process: ProcessForm read FProcess write FProcess;
    {����� ��������}
    property LoadMode: TLoadMode read FLoadMode write FLoadMode;
    {��������� ����� �������}
    property IsProcessShowing: boolean read GetProcessShowing;
    {������ �� �������� ����}
    property ExcelSheet: ExcelWorkSheet read FExcelSheet write FExcelSheet;
    {������ �� ���������� ����}
    property XMLCatalog: TXMLCatalog read FXMLCatalog write FXMLCatalog;
    {��������� ������}
    property DataProvider: IPlaningProvider read FDataProvider write FDataProvider;
    {������� ���������� ��� ����� ��� �������� ������}
    property NeedRound: boolean read FNeedRound write FNeedRound;
    property TotalMultiplier: TTotalMultiplier read FTotalMultiplier
      write FTotalMultiplier;
    {�������� ���.���. ������}
    property MarkerPosition: TMarkerPosition read FMarkerPosition
      write FMarkerPosition;
    {���������� ������������ ����� ��� �������}
    property FilterCellsLength: integer read FFilterCellsLength
      write FFilterCellsLength;
    {���������� ������ �� ������� ������ �������}
    property IsMergeFilterCellsByTable: boolean read FIsMergeFilterCellsByTable
      write FIsMergeFilterCellsByTable;
    {������� ������ ��������� �������� ������������� ��������}
    property DisplayFullFilterText: boolean read FDisplayFullFilterText
      write FDisplayFullFilterText;
    {�������� ��������� ������}
    property CountrySetting: integer read FCountrySetting write FCountrySetting;
    {����������� ����� � ������� ����� �����}
    property DSeparator: string read FDSeparator write FDSeparator;
    {����������� ��������}
    property TSeparator: string read FTSeparator write FTSeparator;
    {����������� ��������� ������}
    property ListSeparator: string read FListSeparator write FListSeparator;
    {����� ������� �������}
    property TableProcessingMode: TTableProcessingMode read FTableProcessingMode
      write FTableProcessingMode;
    {��������� ������������� �������� ����������� ��������
     � MDX ������� - NonEmptyCrossJoin}
    property AllowNECJ: boolean read FAllowNECJ write FAllowNECJ;
    property SheetHeading: TSheetHeading read FSheetHeading write FSheetHeading;
    {URL}
    property URL: string read FURL write FURL;
    {��� �����}
    property SchemeName: string read FSchemeName write FSchemeName;
    {���� ����������, �������� ���������� �����}
    property LastRefreshDate: string read FLastRefreshDate write FLastRefreshDate;
    {������� ������ ���������� � ������}
    property IsDisplaySheetInfo: boolean read FIsDisplaySheetInfo
      write FIsDisplaySheetInfo;
    {��������� ��������}
    property Breaks: TSheetBreakCollection read FBreaks;
    {������� ������ ���������� ��������}
    property IsDisplayColumnsTitles: boolean read FIsDisplayColumnsTitles
      write FIsDisplayColumnsTitles;
    {������� ������ ���������� �����}
    property IsDisplayRowsTitles: boolean read FIsDisplayRowsTitles
      write FIsDisplayRowsTitles;
    {������� ������ ���������� �����������}
    property IsDisplayTotalsTitles: boolean read FIsDisplayTotalsTitles
      write FIsDisplayTotalsTitles;
    {������� ������ ��� ��������}
    property IsDisplayColumns: boolean read FIsDisplayColumns
      write FIsDisplayColumns;
    {������� ������ ��������}
    property IsDisplayFilters: boolean read FIsDisplayFilters
      write FIsDisplayFilters;
    property InConstructionMode: boolean read FInConstructionMode
      write FInConstructionMode;
    property PermitEditing: boolean read FPermitEditing write SetPermitEditing;
    property Online: boolean read FOnline;
    property TaskId: string read FTaskId write FTaskId;

    {������� �����-������ �������� ��� ������}
    property PrintableStyle: boolean read FPrintableStyle
      write FPrintableStyle;
    {�������� ����������� � ������� �������� �������}
    property IsDisplayCommentStructuralCell: boolean read FIsDisplayCommentStructuralCell
      write FIsDisplayCommentStructuralCell;
    {�������� ����������� � ������� � �������}
    property IsDisplayCommentDataCell: boolean read FIsDisplayCommentDataCell
      write FIsDisplayCommentDataCell;
    property TaskContext: TTaskContext read FTaskContext write FTaskContext;
    property IsLoadingFromTask: boolean read FIsLoadingFromTask write FIsLoadingFromTask;
    property IsSilentMode: boolean read GetIsSilentMode write SetIsSilentMode;
    property TotalsData: IXMLDOMDocument2 read FTotalsData write SetTotalsData;
    property WritablesInfo: TWritablesInfo read FWritablesInfo write FWritablesInfo;

    {���������}
    property Totals: TSheetTotalCollectionInterface read Get_Totals;
    property Rows: TSheetAxisCollectionInterface read Get_Rows;
    property Columns: TSheetAxisCollectionInterface read Get_Columns;
    property Filters: TSheetFilterCollectionInterface read Get_Filters;
    property SingleCells: TSheetSingleCellCollectionInterface read Get_SingleCells;
    property Consts: TConstCollectionInterface read Get_Consts;
    property Params: TParamCollectionInterface read Get_Params;
    property TaskParams: TTaskParamsCollection read GetTaskParams;
    property TotalSections: TStringList read FTotalSections;

    property Environment: TTaskEnvironment read FTaskEnvironment write FTaskEnvironment;

    property KillZeroLinkedParams: boolean read FKillZeroLinkedParams write FKillZeroLinkedParams;
    
    property SheetVersion: string read GetSheetVersion;
  end;

  {���� ������, �������� ��������� �������� �����}
  TElementStyle = (esValue, esValuePrint, esTitle, esTitlePrint);

  //����������� ����� - ������� ����� ������������
  //��������� �������� ����������, �������� �����, �������� � �������
  TSheetElement = class
  private
    {���������� �������������
    ������ ��� ��������� � ���������� Names � Custom Properties �����;
    ���������������� � 2-� �������: ��� �������� ������ ���� � ���������
    ���������(��������� ����� ��������) � ��� �������� ����������(�����
    ����������� �������� ���������� �� ��������� �� ��)}
    FUniqueID: string;
    {����� ������}
    FStyles: TElementStyles;
    { ���������� �� �������������� � "������ ������ � �������"}
    FPermitEditing: boolean;
    FProviderId: string;

  protected
    //������ �� ��������� - ��������� ��������
    FOwner: TSheetCollection;
    //���� DOM not nil ��������� ��� � ��, ����� ������� ������ �� ��
    procedure StoreData(DOM: IXMLDOMDocument2); virtual;
    function GetExcelName: string; virtual; abstract;
    function GetSheetInterface: TSheetInterface; virtual;
    function GetStyleCaption(ElementStyle: TElementStyle): string;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; virtual; abstract;
    function GetAlias: string; virtual; abstract;
    procedure SetPermitEditing(Value: boolean);
  public
    constructor Create(AOwner: TSheetCollection);
    destructor Destroy; override;
    //��������� �������� �������� �� ����
    procedure ReadFromXML(Node: IXMLDOMNode); virtual;
    //���������� �������� �������� � ����
    procedure WriteToXML(Node: IXMLDOMNode); virtual;
    //��������� ���������� �� ������������ ��������
    function Validate(out MsgText: string; out ErrorCode: integer): boolean;
       virtual; abstract;
    function GetSelfIndex: integer;
    function Refresh(Force: boolean): boolean; virtual; abstract;
    // �������� ��� �������� �����
    function GetObjectType: TSheetObjectType; virtual; abstract;
    // �������� ��������� �������� ���� �������� �����
    function GetObjectTypeStr: string; virtual; abstract;
    //��� �������� ��� ����� -
      //����� �� ��������������, � ���� ����� ������, � ���� ��������...
    function GetElementCaption: string; virtual; abstract;
    procedure SetDefaultStyles;
    // ���������� ������ � ��������� ��������
    procedure ApplyStyles; virtual; abstract;
    {������������� �������� ����� ������������ ���������}
    procedure SetOwnerStyles; virtual;
    function MayBeEdited: boolean;
    procedure ValidateStyles;
    function GetOnDeleteWarning: string; virtual;
    function MayBeDeleted: boolean;
    function OfPrimaryProvider: boolean;

    //��������� - �������� ��������
    property Owner: TSheetCollection read FOwner write FOwner;
    property UniqueID: string read FUniqueID write FUniqueID;
    property ExcelName: string read GetExcelName;
    property SheetInterface: TSheetInterface read GetSheetInterface;
    property Styles: TElementStyles read FStyles;
    property StyleCaption[ElementStyle: TElementStyle]: string read GetStyleCaption;
    property DefaultStyleName[ElementStyle: TElementStyle]: string
      read GetDefaultStyleName;
    property Alias: string read GetAlias;
    property PermitEditing: boolean read FPermitEditing write SetPermitEditing;
    property ProviderId: string read FProviderId write FProviderId;
  end;

  // ������� ���������� �� ���������
  TSheetDimension = class(TSheetElement)
  private
    //���������
    FDimension: string;
    //��������
    FHierarchy: string;
    //�������� ���������
    FMembers: IXMLDOMDocument2;
    FMemberProperties: TSheetMPCollectionInterface;

    function GetCatalogHierarchy: THierarchy;
  protected
    // ��������
    FParam: TParamInterface;
    function GetFullDimName: string;
    function GetFullDimName2: string;
    function GetFullDimNameMDX: string;
    function GetMembers: IXMLDOMDocument2;
    procedure SetMembers(DOM: IXMLDOMDocument2);
    function GetMdxText: string; virtual; abstract;
    function GetAllMember: string;
    function GetFullName: string;
    function GetHierarchyId: string;
    function GetCommentText: string; virtual; abstract;
    function GetCPName: string;
    procedure StoreData(DOM: IXMLDOMDocument2);  override;
    function GetAllMemberProperties: string;
    {���������, ����������� ����������� ��������� �������, ������������� ��
     � �������� ������� (FMembers). ��� ���� ������ ����� ��� �����������,
     ��� � ��������.
     ������� � ���, ������ ����� ������������� �� ������ � �������, �� �
     � �������, � ��������� ������� ��������� ������ � �������. ��������������,
     ��� ������� ������ �� "����������" �� ��������� ���������������.
     ���� ����� �������� � �����������, ������ ���������� �������� ��������
     �������� � ������.}
    procedure RecreateLevelsByMembers; virtual; abstract;
  public
    destructor Destroy; override;
    function IsParam: boolean;
    function IsBaseParam: boolean;
    function GetElementCaption: string; override;
    procedure EnlightMembersDom(ShouldLeaveNames: boolean);
    procedure TurnAllLevelsOn;
    {������������ ��������� � ������� (� ����������� �����������)}
    function Refresh(Force: boolean): boolean; override;

    //��� ���������
    property Dimension: string read FDimension write FDimension;
    //��� ��������
    property Hierarchy: string read FHierarchy write FHierarchy;
    // ��� �������� (Hirarchy.FullName)
    property FullName: string read GetFullName;
    //��� ���������.��� ��������
    property FullDimensionName: string read GetFullDimName;
    {���������.��� ���������.��� ��������, ���� ��������� ��� ����������
     ��� ���������.��� ��������
    }
    property FullDimensionName2: string read GetFullDimName2;
    //[��� ���������].[��� ��������]
    property FullDimensionNameMDX: string read GetFullDimNameMDX;
    property Members: IXMLDOMDocument2 read GetMembers write SetMembers;
    {���������������� MDX � �������������� Children � Descendants}
    property MdxText: string read GetMdxText;
    property AllMember: string read GetAllMember;
    property Param: TParamInterface read FParam write FParam;
    //����� �����������
    property CommentText: string read GetCommentText;
    property CPName: string read GetCPName;
    {C�������������� ������ �������� �� XMLCatalog}
    property CatalogHierarchy: THierarchy read GetCatalogHierarchy;
    property AllMemberProperties: string read GetAllMemberProperties;
    property MemberProperties: TSheetMPCollectionInterface read FMemberProperties
      write FMemberProperties;
    property HierarchyId: string read GetHierarchyId;
  end;

  //����������� ����� - ��������� ��������� �����
  TSheetCollection = class(TList)
  private
    FOwner: TSheetInterface;
    FObjectType: TSheetObjectType;
    FUseStylesForChildren: boolean;
    {����� ������}
    FStyles: TElementStyles;
    { ���������� �� �������������� � "������ ������ � �������"}
    FPermitEditing: boolean;
  protected
    FUpdating: boolean;
    function GetEmpty: boolean;
    function GetItem(Index: integer): TSheetElement;
    function GetStyleCaption(ElementStyle: TElementStyle): string; virtual; abstract;
    function GetDefaultStyleName(ElementStyle: TElementStyle): string; virtual; abstract;
    function GetLastItem: TSheetElement;
    procedure SetPermitEditing(Value: boolean);
  public
    constructor Create(AOwner: TSheetInterface);
    destructor Destroy; override;
    //������� ���������
    procedure Clear; override;
    procedure Delete(Index: Integer); virtual;
    //��������� ��������� �� DOM-a
    procedure ReadFromXML(Node: IXMLDOMNode); virtual;
    //���������� ��������� � XML
    procedure WriteToXML(Node: IXMLDOMNode); virtual;
    //���������� ��� ��������� � ������������ � ����� �� ���������
    function GetCollectionName: string; virtual; abstract;
    //��������� ���������� �� ������������ ��������
    function Validate: boolean; virtual; abstract;
    function FindById(Id: string): integer; virtual; abstract;
    function Refresh(Force: boolean): boolean; virtual; abstract;
    // �������� ��������� �������� ���� ��������� �����
    function GetObjectTypeStr: string;
    procedure SetDefaultStyles;
    procedure SetDefaultStyles2All; virtual;
    function MayBeEdited: boolean;
    function ElementsMayBeEdited: boolean;
    procedure ValidateStyles;

    property Owner: TSheetInterface read FOwner;
    property Empty: boolean read GetEmpty;
    property ObjectType: TSheetObjectType read FObjectType write FObjectType;
    property Updating: boolean read FUpdating;
    property Items[Index: integer]: TSheetElement read GetItem;
    {����������� �� ������ ����� ��� ���� ��������� ���������}
    property UseStylesForChildren: boolean read FUseStylesForChildren write FUseStylesForChildren;
    property Styles: TElementStyles read FStyles;
    property StyleCaption[ElementStyle: TElementStyle]: string read GetStyleCaption;
    property DefaultStyleName[ElementStyle: TElementStyle]: string
      read GetDefaultStyleName;
    property LastItem: TSheetElement read GetLastItem;
    property PermitEditing: boolean read FPermitEditing write SetPermitEditing;
  end;

  TFormulaParamType = (fptTotal, fptSingleCell, fptRowMP, fptColumnMP,
    fptFreeCell, fptTotalAbsolute);

  TFormulaParam = class
  private
    FName: string;
    FParamType: TFormulaParamType;
    FIsOtherSheet: boolean;
    FOffset: string;
    FParamValue: string;
    FSection: integer;
    FCoords: TStringList;
    FCoord: string;
  public
    constructor Create;
    destructor Destroy; override;

    property Name: string read FName write FName;
    property ParamType: TFormulaParamType read FParamType write FParamType;
    property ParamValue: string read FParamValue write FParamValue;
    property Offset: string read FOffset write FOffset;
    property IsOtherSheet: boolean read FIsOtherSheet write FIsOtherSheet;

    property Section: integer read FSection write FSection;
    property Coords: TStringList read FCoords write FCoords;
    property Coord: string read FCoord write FCoord;
  end;

  TFormulaParams = class(TList)
  private
    function GetItem(Index: integer): TFormulaParam;
    procedure SetItem(Index: integer; Value: TFormulaParam);
  public
    destructor Destroy; override;
    property Items[Index: integer]: TFormulaParam read GetItem write SetItem; default;
    function Add(FormulaParam: TFormulaParam): TFormulaParam;
    function CreateParam(ParamType: TFormulaParamType; Name, ParamValue, Offset: string;
      IsOtherSheet: boolean): TFormulaParam;
    procedure Delete(Index: integer);
  end;

  TTypeFormula= class
  private
    FOwner: TSheetBasicTotal;
    FEnabled: boolean;
    FTemplate: string;
    FFormulaParams: TFormulaParams;
  protected
    function GetParamCaption(ParamIndex: integer): string;
    function GetUserFormula: string;
    procedure SetUserFormula(UserFormula: string);
  public
    constructor Create(AOwner: TSheetBasicTotal);
    destructor Destroy; override;
    procedure WriteToXML(Node: IXMLDOMNode);
    procedure ReadFromXML(Node: IXMLDOMNode);
    procedure Clear;
    function IsValid(out ErrorText: string): boolean;
    function ContainAlias(Alias: string): boolean;
    function IsEqual(SampleFormula: TTypeFormula; IgnoreOffset: boolean): boolean;

    property Owner: TSheetBasicTotal read FOwner;
    property UserFormula: string read GetUserFormula write SetUserFormula;
    property Template: string read FTemplate write FTemplate;
    property Enabled: boolean read FEnabled write FEnabled;
    property FormulaParams: TFormulaParams read FFormulaParams write FFormulaParams;
  end;

  {������� ����� ��� �������� ����������}
  TSheetBasicTotal = class(TSheetElement)
  protected
    //��������� � ���� � ������������� ������������� ���������
    FCaption: string;
    //��� ����������
    FTotalType: TSheetTotalType;
    //��� ���� (��� ����� - ���� �� ����)
    FCubeName: string;
    //��� ���� (��� ����� - ���� �� ����)
    FMeasureName: string;
    //������ �����
    FNumberFormat: variant;
    //����� ���������� ������
    FDigits: integer;
    //��� ������� ����������
    FFormat: TMeasureFormat;
    //����.������ ���������� ��� ���������� �������� � ����������
    FEmptyValueSymbol: string;
    // ������ �� ���������
    FConstID: string;
    {������� �������}
    FTypeFormula: TTypeFormula;
    {������������� ���������������� ��� ��� ��������.
      ��� ��������� �����������: �������� "����������_����_7",
      ��� �������� � �������� ������ ���������� ������� ������ (Instance).
      ��� ���������: �������� "���������_���������_2"}
    FUserExcelName: string;

    procedure SetFormat(Value: TMeasureFormat); virtual;
    procedure SetDigits(Value: integer); virtual;
    function GetCube: TCube; virtual;
    function GetMeasure: TMeasure; virtual;

    function GetEmptySymbol: string;
    procedure SetTotalType(AType: TSheetTotalType); virtual; abstract;
  public
    constructor Create(AOwner: TSheetCollection);
    destructor Destroy; override;
    function BuildFormatMask: string; virtual;
    procedure ReadFromXML(Node: IXMLDOMNode); override;
    procedure WriteToXML(Node: IXMLDOMNode); override;
    function GetElementCaption: string; override;
    procedure ClearLinkedTypeFormulasValues;
    procedure MapLinkedTypeFormulasValues;
    function GetLinkedTypeFormulasWarning: string;
    procedure SwitchType(out CommentForHistory: string); virtual; abstract;
    function RepairConst: TConstInterface; virtual; abstract;
    {��������� �� �������}
    procedure Round(var AValue: extended); virtual;

    function GetMultipliedValue(AValue: extended): extended; overload; virtual; abstract;
    function GetMultipliedValue(AValue: string): string; overload; virtual; abstract;
    function GetDividedValue(AValue: extended): extended; overload; virtual; abstract;
    function GetDividedValue(AValue: string): string; overload; virtual; abstract;

    property NumberFormat: variant read FNumberFormat write FNumberFormat;
    property Format: TMeasureFormat read FFormat write SetFormat;
    property Digits: integer read FDigits write SetDigits;
    property Cube: TCube read GetCube;
    property Measure: TMeasure read GetMeasure;
    property Caption: string read FCaption write FCaption;
    property MeasureName: string read FMeasureName write FMeasureName;
    property CubeName: string read FCubeName write FCubeName;
    property TotalType: TSheetTotalType read FTotalType write SetTotalType;
    property EmptyValueSymbol: string read GetEmptySymbol write FEmptyValueSymbol;
    property ConstID: string read FConstID write FConstID;
    property TypeFormula: TTypeFormula read FTypeFormula write FTypeFormula;
  end;

  {��������� �����}
  TSheetTotalInterface = class(TSheetBasicTotal)
  protected
    //�� �������������� �� ��� ��������
    FIsIgnoredColumnAxis: boolean;
    //����� �� ������� ��� ����?
    FSummariesByVisible: boolean;
    //������ ��������� ������ � ������� ����
    FIsGrandTotalDataOnly: boolean;
    //�� �������������� �� ��� �����
    FIsIgnoredRowAxis: boolean;
    //������� ������ ������� � �����
    FColumnWidth: single;
    {����� �������� ������}
    FCountMode: TMeasureCountMode;
    {���������� �� ������� ����������}
    FIsHidden: boolean;

    function GetCommentText: string; virtual; abstract;
    function GetTitleExcelName: string; virtual; abstract;
    //���������� ���������� ����� ���������� �� ����� ������������� ���� ����������
    function GetOrdinal: integer; virtual; abstract;
    //������ ����������� �� ���������� �������� (������� ����� �������)
    function GetFiltersCommaText: string; virtual; abstract;
    function GetIsIgnoredColumnAxis: boolean; virtual; abstract;
    function GetFullName: string; virtual; abstract;
    function GetSectionCount: integer; virtual; abstract;
    function GetShift: integer; virtual; abstract;
  public
    //��������� �� �� ���������� ������ ������
    function IsFilteredBy(Filter: TSheetFilterInterface): boolean; virtual; abstract;
    function GetFullExcelName(Instance: integer): string; virtual; abstract;
    function GetUserExcelName(Instance: integer): string; virtual; abstract;
    // ��������� ��������� ����������
    function GetTotalRange(SectionIndex: integer): ExcelRange; virtual; abstract;
    function GetTotalRangeByColumn(Column: integer): ExcelRange; virtual; abstract;
    // ��������� ��������� ���������� ��� ������ �����
    function GetTotalRangeWithoutGrandSummary(SectionIndex: integer): ExcelRange; virtual; abstract;
    function GetTotalRangeWithoutGrandSummaryByColumn(Column: integer): ExcelRange; virtual; abstract;
    // ��������� ������� ���������� �� ������� ������
    function GetTotalColumn(SectionIndex: integer): integer; virtual; abstract;
    {����� �� ��������� �� ���������}
    function FactorizedBy(AxisElem: TSheetAxisElementInterface): boolean; virtual; abstract;
    {��������� ���������� �������������� �� ���}
    function FitInAxis(AxisType: TAxisType): integer; overload; virtual; abstract;
    function FitInAxis(AxisType: TAxisType; out UnfitDimensions: string): integer; overload; virtual; abstract;
    function AddTypeFormula(Row, Column: integer): TTypeFormula; virtual; abstract;
    function IsTypeFormulaException(Row, Column: integer): boolean; virtual; abstract;
    procedure ClearTypeFormulaValues; virtual; abstract;

    property IsIgnoredRowAxis: boolean read FIsIgnoredRowAxis
      write FIsIgnoredRowAxis;
    property ColumnWidth: single read FColumnWidth write FColumnWidth;
    property CommentText: string read GetCommentText;
    property ExcelName: string read GetExcelName;
    property TitleExcelName: string read GetTitleExcelName;
    property Ordinal: integer read GetOrdinal;
    property FiltersCommaText: string read GetFiltersCommaText;
    property CountMode: TMeasureCountMode read FCountMode write FCountMode;
    property SummariesByVisible: boolean read FSummariesByVisible
      write FSummariesByVisible;
    property IsIgnoredColumnAxis: boolean read GetIsIgnoredColumnAxis
      write FIsIgnoredColumnAxis;
    property IsGrandTotalDataOnly: boolean read FIsGrandTotalDataOnly
      write FIsGrandTotalDataOnly;
    // ��� �������� (FullName)
    property FullName: string read GetFullName;
    // ��� �������� (FullName)
    property SectionCount: integer read GetSectionCount;
    property Shift: integer read GetShift;
    property IsHidden: boolean read FIsHidden write FIsHidden;
  end;


  {��������� ��������� ������}
  TSheetTotalCollectionInterface = class(TSheetCollection)
  protected
    FStyleByLevels: boolean;
    FFormatByRows: boolean;
    FTotalCounters: TTotalCounters;

    function GetItem(Index: integer): TSheetTotalInterface; virtual; abstract;
    procedure SetItem(Index: integer; Value: TSheetTotalInterface); virtual; abstract;
    function GetLastItem: TSheetTotalInterface;
  public
    constructor Create(AOwner: TSheetInterface);
    //��������� ������� � ���������
    function Append: TSheetTotalInterface; virtual; abstract;
    function FindTotal(CubeName, MeasureName: string): TSheetTotalInterface; virtual; abstract;
    //���������� ���������� �� ��� ������
    function FindByAlias(aStr: string): TSheetTotalInterface; virtual; abstract;
    //���������� ���������� �� ��� ���������
    function FindByCaption(Caption: string): TSheetTotalInterface; virtual; abstract;
    // �������� �� ��������������������� MDX ��� �����������
    function MayTotalsHaveSameMDX(Index1, Index2: integer): boolean; virtual; abstract;
    {���-�� ����������� ������������ ������� ���������}
    function CountWithPlacement(IgnoredColumns: boolean): integer; virtual; abstract;
    { ������ � ����������� ������������� ���� ����������}
    function GetWithPlacement(IgnoredColumns: boolean;
      ind: integer): TSheetTotalInterface; virtual; abstract;
    function GetWithPlacementInd(IgnoredColumns: boolean;
      ind: integer): integer; virtual; abstract;
    // ���������� ���������� �� �������
    function FindByColumn(Column: integer; out SectionIndex: integer): TSheetTotalInterface; virtual; abstract;
    // ������������ �� � ��������� ���������� ��������� ����
    function CheckByType(TotalTypes: TTotalTypes): boolean; virtual; abstract;
    // �������� ������ �������� ����������� ��������� ����
    // ���������� false - �������� ���������� �����������
    function GetTotalLists(var TotalsList, IgnoredTotalsList: TStringList;
                           FTotalTypes: TTotalTypes): boolean; virtual; abstract;
    // ����������� �� ������������� �����
    function AreSummariesImperative: boolean; virtual; abstract;
    {����������� ����������� �������� NumberFormat}
    procedure GetNumberFormats; virtual; abstract;
    {���������� XPath-������� �� ������������� ������� ������}
    function GetLeafCondition(NeedRow, NeedColumn: boolean): string; virtual; abstract;
    function GetTotalCounterValue(TotalType: TSheetTotalType): string;

    property Items[Index: integer]: TSheetTotalInterface
      read GetItem write SetItem; default;
    {������������ �������������� ������� ����������� �� ������� ���}
    property StyleByLevels: boolean read FStyleByLevels write FStyleByLevels;
    {�� ������� ����� ��� ������������� ������� �����������}
    property FormatByRows: boolean read FFormatByRows write FFormatByRows;
    property LastItem: TSheetTotalInterface read GetLastItem;
  end;

  {��������� �������}
  TSheetFilterInterface = class(TSheetDimension)
  private
    //������� ���������
    FIsPartial: boolean;
  protected
    //������� �������� �������, ���� �������
    FScope: TStringList;
    function GetIsMultiple: boolean; virtual; abstract;
    function GetText: string; virtual; abstract;
    function GetScopeText: string; virtual; abstract;
    procedure SetScope(AValue: TStringList); virtual; abstract;
    function GetOwningCell: TSheetSingleCellInterface; virtual; abstract;
  public
    destructor Destroy; override;
    function IsAffectsTotal(Total: TSheetTotalInterface): boolean; virtual; abstract;
    //�� ����� _����_ ����������� ��������� ������
    function TotalTypesAffected: integer; virtual; abstract;
    {������ �������� ������ ��������� ��� ��������� �������}
    function GetMPStrings: TStringList; virtual; abstract;
    function CheckForWriteback(out Error: string): boolean; virtual; abstract;
    function WithDefaultValue(out DefaultNode: IXMLDOMNode): boolean; virtual; abstract;
    function GetFilterDescription(AdditionalDetails: boolean): string; virtual; abstract;

    property IsPartial: boolean read FIsPartial write FIsPartial;
    property Scope: TStringList read FScope write SetScope;
    property IsMultiple: boolean read GetIsMultiple;
    //�����, ��������� � ������ �����
    property Text: string read GetText;
    //��������� �������� ������� �������� ������ - ������� �����������
    property ScopeText: string read GetScopeText;
    property ExcelName: string read GetExcelName;
    property OwningCell: TSheetSingleCellInterface read GetOwningCell;
  end;

  {��������� ��������� ��������}
  TSheetFilterCollectionInterface = class(TSheetCollection)
  protected
    function GetItem(Index: integer): TSheetFilterInterface; virtual; abstract;
    procedure SetItem(Index: integer; Value: TSheetFilterInterface); virtual; abstract;
    function GetLastItem: TSheetFilterInterface;
    function GetOwningCell: TSheetSingleCellInterface; virtual; abstract;
  public
    constructor Create(Cell: TSheetSingleCellInterface); overload; virtual; abstract;
    //��������� ������� � ���������
    function Append: TSheetFilterInterface; virtual; abstract;

    function FindByDimAndHier(DimName, HierName: string): TSheetFilterInterface; virtual; abstract;
    function FindByFullDimensionName(FullDimensionName: string): TSheetFilterInterface; virtual; abstract;
    //���� �� ������ � ������ ����������� � �� � ����� ��
    function IsThereSuchFilter(DimName, HierName: string;
      Total: TSheetTotalInterface; ExceptId: string): TSheetFilterInterface; virtual; abstract;
    //���� ���������� �������, ����������� �� ���� � ��� �� ����������
    function IsThereDuplicateFilters(DimName, HierName, ExceptId: string;
      Scope: TStringList; out Msg: string): boolean; virtual; abstract;

    property Items[Index: integer]: TSheetFilterInterface
      read GetItem write SetItem; default;
    property LastItem: TSheetFilterInterface read GetLastItem;
    property OwningCell: TSheetSingleCellInterface read GetOwningCell;
  end;


  {��������� �������� ���}
  TSheetAxisElementInterface = class(TSheetDimension)
  private
    FLevels: TSheetLevelCollectionInterface;
    FIgnoreHierarchy: boolean;
    FReverseOrder: boolean;
    FHideDataMembers: boolean;
    FSummaryOptions: TSummaryOptions;
    FUseSummariesForLevels: boolean;
  protected
    function GetFieldCount: integer; virtual; abstract;
    function GetOrientation: TAxisType; virtual; abstract;
  public
    destructor Destroy; override;
    {���������� ������ ��������, ���� �� ���, ����������� � �������}
    function GetOrQueryMembers: IXMLDOMDocument2; virtual; abstract;
    {MDX-���������, ��� ����������� ��� ��������� ��������}
    function GetMDXMembersSet: string; virtual; abstract;
    {������� �������������� ���������� (GetMDXMembersSet),
    ������ ������ ������������ ������� ������ �� ������.
    ��� ������� �������� set �� ��������� �������
    (����� ���������� �� ��������� MDX-�������)}
    function GetMDXLevelsSet: string; virtual; abstract;
    procedure SetOwnerStyles; override;
    procedure LoadWithIndices(Dom: IXMLDOMDocument2); virtual; abstract;
    procedure RemoveAndRenameDataMembers(Dom: IXMLDOMDocument2); virtual; abstract;

    property Levels: TSheetLevelCollectionInterface read FLevels write FLevels;
    property IgnoreHierarchy: boolean read FIgnoreHierarchy write FIgnoreHierarchy;
    property ReverseOrder: boolean read FReverseOrder write FReverseOrder;
    property HideDataMembers: boolean read FHideDataMembers
      write FHideDataMembers;

    property FieldCount: integer read GetFieldCount;
    property Orientation: TAxisType read GetOrientation;

    property SummaryOptions: TSummaryOptions read FSummaryOptions write FSummaryOptions;
    property UseSummariesForLevels: boolean read FUseSummariesForLevels
      write FUseSummariesForLevels;
  end;

  {��������� ��������-���}
  TSheetAxisCollectionInterface = class(TSheetCollection)
  private
  protected
    //��� ��� - ����� ��� ��������
    FAxisType: TAxisType;
    FSummaryOptions: TSummaryOptions;
    FGrandSummaryOptions: TSummaryOptions;
    FUseSummariesForElements: boolean;
    FBroken: boolean;
    FReverseOrder: boolean;
    FMPBefore: boolean;
    FHideEmpty: boolean;
    {����������� ������ ������}
    FSummaryOptimization: boolean;
    {������������ �������}
    FUseIndents: boolean;
    FLevelsFormatting: boolean;
    function GetItem(Index: integer): TSheetAxisElementInterface; virtual; abstract;
    procedure SetItem(Index: integer; Value: TSheetAxisElementInterface); virtual; abstract;
    function GetMPCheckedCount: integer; virtual; abstract;
    function GetFieldCount: integer; virtual; abstract;
    function GetCommentText: string; virtual; abstract;
    function GetMarkupFieldCount: integer; virtual; abstract;
    function GetLastItem: TSheetAxisElementInterface;
  public
    constructor Create(AOwner: TSheetInterface);
    destructor Destroy; override;
    function GetFullAxisDOM: IXMLDOMDocument2; virtual; abstract;
    function MemberPropertiesAsStringList: TStringList; virtual; abstract;
    function FindByDimension(DimName: string): TSheetAxisElementInterface; virtual; abstract;
    function FindByDimAndHier(DimName, HierName: string): TSheetAxisElementInterface; virtual; abstract;
    function FindByFullDimensionName(FullDimensionName: string): TSheetAxisElementInterface; virtual; abstract;
    {��� ��������� ��� ������ � ���}
    function AllFromCube(CubeName: string): boolean; virtual; abstract;
    function GetLevelByNumber(LevelNumber: integer): TSheetLevelInterface;
    function FindByAlias(aStr: string): TSheetAxisElementInterface; virtual; abstract;


    property Items[Index: integer]: TSheetAxisElementInterface
      read GetItem write SetItem; default;

    //��������� ������� � ���������
    function Append: TSheetAxisElementInterface; virtual; abstract;
    function GetLevelNumber(AxisIndex, LevelIndex: integer): integer; virtual; abstract;
    function GetLevelIndent(AxisIndex, LevelIndex: integer): integer; overload; virtual; abstract;
    function GetLevelIndent(Level: TSheetLevelInterface): integer; overload; virtual; abstract;

    property MPCheckedCount: integer read GetMPCheckedCount;
    {���-�� ������� � ��� (c ������ ����������� ������������� �����)}
    property FieldCount: integer read GetFieldCount;
    {���������� ������� � ���, ������������ ��� �������� ���������}
    property MarkupFieldCount: integer read GetMarkupFieldCount;
    property AxisType: TAxisType read FAxisType write FAxisType;
    property Broken: boolean read FBroken write FBroken;
    property ReverseOrder: boolean read FReverseOrder write FReverseOrder;
    property CommentText: string read GetCommentText;
    property SummaryOptions: TSummaryOptions read FSummaryOptions write FSummaryOptions;
    property GrandSummaryOptions: TSummaryOptions read FGrandSummaryOptions
      write FGrandSummaryOptions;
    property UseSummariesForElements: boolean read FUseSummariesForElements
      write FUseSummariesForElements;
    property MPBefore: boolean read FMPBefore write FMPBefore;
    property HideEmpty: boolean read FHideEmpty write FHideEmpty;
    property SummaryOptimization: boolean read FSummaryOptimization
      write FSummaryOptimization;
    property UseIndents: boolean read FUseIndents write FUseIndents;
    property LevelsFormatting: boolean read FLevelsFormatting
      write FLevelsFormatting;
    property LastItem: TSheetAxisElementInterface read GetLastItem;
  end;

  TSheetLevelInterface = class(TSheetElement)
  private
    FName: string;
    //������� ������ ������� � �����
    FColumnWidth: single;
    FSummaryOptions: TSummaryOptions;
    FFontOptions: TFontOptions;
    FAllCapitals: boolean;
    FUseFormat: boolean;
    {�������� ����� ������ � ��������� (����� ���������� ����� �������
      ����� �� ��������� � �������� � ���������)}
    FInitialIndex: integer;
    FUseCustomDMTitle: boolean;
    FCustomDMTitle: string;
    FDMDeployment: TItemDeployment;
  protected
    function GetOrientation: TAxisType; virtual; abstract;
    function GetTitleExcelName: string; virtual; abstract;
    function GetParentCollection: TSheetLevelCollectionInterface;
    function GetAxisElement: TSheetAxisElementInterface;
    function GetInitialIndex: integer;
    function GetHideDataMembers: boolean;
  public
    function GetDepth: integer; virtual; abstract;

    property Name: string read FName write FName;
    property ColumnWidth: single read FColumnWidth write FColumnWidth;
    property TitleExcelName: string read GetTitleExcelName;
    property Orientation: TAxisType read GetOrientation;
    property ParentCollection: TSheetLevelCollectionInterface
      read GetParentCollection;
    property AxisElement: TSheetAxisElementInterface read GetAxisElement;
    property SummaryOptions: TSummaryOptions read FSummaryOptions
      write FSummaryOptions;
    property FontOptions: TFontOptions read FFontOptions
      write FFontOptions;
    property AllCapitals: boolean read FAllCapitals write FAllCapitals;
    property UseFormat: boolean read FUseFormat write FUseFormat;
    property InitialIndex: integer read GetInitialIndex write FInitialIndex;
    property HideDataMembers: boolean read GetHideDataMembers;
    property UseCustomDMTitle: boolean read FUseCustomDMTitle write FUseCustomDMTitle;
    property CustomDMTitle: string read FCustomDMTitle write FCustomDMTitle;
    property DMDeployment: TItemDeployment read FDMDeployment write FDMDeployment;
  end;

  TSheetLevelCollectionInterface = class(TSheetCollection)
  private
    FOwner: TSheetAxisElementInterface;
  protected
    function GetItem(Index: integer): TSheetLevelInterface; virtual; abstract;
    procedure SetItem(Index: integer; Value: TSheetLevelInterface); virtual; abstract;
    function GetLastItem: TSheetLevelInterface;
  public
    constructor Create(AOwner: TSheetAxisElementInterface);
    destructor Destroy; override;
    function Append: TSheetLevelInterface; virtual; abstract;
    //�������������� � ������ ���� ������� � ���� ������, ����������� $$$
    function GetToString: string; virtual; abstract;
    function GetNamesToString: string; virtual; abstract;
    function FindByInitialIndex(Index: integer): TSheetLevelInterface; virtual; abstract;

    property Items[Index: integer]: TSheetLevelInterface
      read GetItem write SetItem; default;
    property Owner: TSheetAxisElementInterface read FOwner;
    property LastItem: TSheetLevelInterface read GetLastItem;
    property ToString: string read GetToString;
    property NamesToString: string read GetNamesToString;
  end;

  {��������� �������� �������� ���������}
  TSheetMPElementInterface = class
  private
    FName: string;
    FOwner: TSheetMPCollectionInterface;
    FChecked: boolean;
  protected
    FMask: string;
    function GetMask: string; virtual; abstract;
    procedure SetMask(Value: string); virtual; abstract;
  public
    constructor Create(AOwner: TSheetMPCollectionInterface);
    destructor Destroy; override;
    procedure ReadFromXML(Node: IXMLDOmNode); virtual; abstract;
    procedure WriteToXML(Node: IXMLDOmNode); virtual; abstract;
    property Name: string read FName write FName;
    property Mask: string read GetMask write SetMask;
    property Owner: TSheetMPCollectionInterface read FOwner;
    property Checked: boolean read FChecked write FChecked;
  end;

  {��������� ��������� �������� ���������}
  TSheetMPCollectionInterface = class(TList)
  private
    FOwner: TSheetDimension;
  protected
    function GetItem(Index: integer): TSheetMPElementInterface; virtual; abstract;
    procedure SetItem(Index: integer; Value: TSheetMPElementInterface); virtual; abstract;
    function GetLastItem: TSheetMPElementInterface;
  public
    constructor Create(AOwner: TSheetDimension);
    destructor Destroy; override;
    function Append: TSheetMPElementInterface; virtual; abstract;
    function GetCheckedCount: integer; virtual; abstract;
    procedure ReadFromXML(Node: IXMLDOMNode); virtual; abstract;
    procedure WriteToXML(Node: IXMLDOMNode); virtual; abstract;
    procedure Reload; overload; virtual; abstract;
    function Find(AName: string): TSheetMPElementInterface; virtual; abstract;
    property Items[Index: integer]: TSheetMPElementInterface read GetItem
      write SetItem; default;
    property Owner: TSheetDimension read FOwner;
    property CheckedCount: integer read GetCheckedCount;
    property LastItem: TSheetMPElementInterface read GetLastItem;
  end;

  TSuicideMethod = (smImmediate, smProlonged);

  {��������� ��������� ������ (���� + ����� ������� ��������}
  TSheetSingleCellInterface = class(TSheetBasicTotal)
  private
    FFilters: TSheetFilterCollectionInterface;
    FTotalMultiplier: TTotalMultiplier;
  protected
    function GetMdxText: string; virtual; abstract;
    function GetCommentText: string; virtual; abstract;
    function GetName: string; virtual; abstract;
    procedure SetName(Value: string); virtual; abstract;
    function GetCommaFilters: string; virtual; abstract;
    function GetAddress: string; virtual; abstract;
    function GetPlacedinTotals: boolean; virtual; abstract;
    function GetValue: string; virtual; abstract;
  public
    destructor Destroy; override;
    function GetAddressPoint(out Column, Row: integer): boolean; virtual; abstract;
    function GetExcelRange: ExcelRange; virtual; abstract;
    procedure Suicide(Method: TSuicideMethod; out Msg: string); virtual; abstract;
    function GetUserExcelName: string; virtual; abstract;
    function GetUnderlayingTotal: TSheetTotalInterface; virtual; abstract;
    {����������� ��������� �������� ������}
    property Filters: TSheetFilterCollectionInterface read FFilters
      write FFilters;
    {����� �������}
    property MdxText: string read GetMdxText;
    {����� �����������, ���������� � ������ �����}
    property CommentText: string read GetCommentText;
    {������� ������ � ������� ������}
    property TotalMultiplier: TTotalMultiplier read FTotalMultiplier
      write FTotalMultiplier;
    {���������������� ��� ������}
    property Name: string read GetName write SetName;
    {������������ �������� ����� �������}
    property CommaFilters: string read GetCommaFilters;
    {����� ������ � ����� � ������� �1}
    property Address: string read GetAddress;
    {������� ���������� ������ ������ ��������� ���������/�����������}
    property PlacedInTotals: boolean read GetPlacedinTotals;
    {�������� �������� ���������� ����������}
    property Value: string read GetValue;
  end;

  {��������� ��������� �����}
  TSheetSingleCellCollectionInterface = class(TSheetCollection)
  protected
    FTotalCounters: TTotalCounters;
    function GetItem(Index: integer): TSheetSingleCellInterface; virtual; abstract;
    procedure SetItem(Index: integer; Item: TSheetSingleCellInterface); virtual; abstract;
    function GetLastItem: TSheetSingleCellInterface;
  public
    constructor Create(AOwner: TSheetInterface);
    //��������� ������� � ���������
    function Append: TSheetSingleCellInterface; virtual; abstract;
    {����������� ����������� �������� NumberFormat}
    procedure GetNumberFormats; virtual; abstract;
    function FindByName(AName: string): TSheetSingleCellInterface; virtual; abstract;
    property Items[Index: integer]: TSheetSingleCellInterface read GetItem write SetItem; default;
    // ������������ �� � ��������� ���������� ��������� ����
    function CheckByType(TotalTypes: TTotalTypes): boolean; virtual; abstract;
    //���������� ���������� �� ��� ������
    function FindByAlias(aStr: string): TSheetSingleCellInterface; virtual; abstract;
    function GetTotalCounterValue(TotalType: TSheetTotalType): string;

    property LastItem: TSheetSingleCellInterface read GetLastItem;
  end;

  // �������� ����� ���������� � ��������
  TParamBaseInterface = class
  private
    FUniqueId: integer;
    function GetSheetInterface: TSheetInterface; virtual; abstract;
  public
    ID: integer;
    PID: integer;
    IsInherited: boolean;
    Comment: string;
    Name: string;

    procedure WriteToXML(Node: IXMLDOMNode); virtual; abstract;
    function ReadFromXML(Node: IXMLDOMNode): boolean; virtual; abstract;

    property UniqueId: integer read FUniqueId write FUniqueId;
    property SheetInterface: TSheetInterface read GetSheetInterface;
  end;

  {����������� ��������� ���������� � ��������}
  TParamBaseCollectionInterface = class(TSheetCollection)
  protected
    FCounter: int64;
  public
    // ������� ���������� ������������� ��������
    function GetUniqueID: integer;
    property Counter: int64 read FCounter;
  end;

  {��������� ��������� ����� � ��������� � ������. ����� ��� �������������.}
  TParamPresence = (ppNoContext, ppSameId, ppMustBeKilled, ppNew,
    ppSameNameAndDimension, ppMustBeRenamed);

  TConstInterface = class(TParamBaseInterface)
  private
    FOwner: TConstCollectionInterface;
    function GetSheetInterface: TSheetInterface; override;
  public
    Value: string;
    // ����������� �� ��������� �� �����
    IsSheetConst: boolean;
    constructor Create(Owner: TConstCollectionInterface);
    function IsSheetElement: boolean; virtual; abstract;
    procedure SyncSheetConsts(OldName, NewName: string); virtual; abstract;
    function GetPresenceInTask(out TaskConst: TTaskConst): TParamPresence; virtual; abstract;

    property Owner: TConstCollectionInterface read FOwner;
  end;

  { ��������� �������� }
  TConstCollectionInterface = class(TParamBaseCollectionInterface)
  protected
    function GetItem(Index: integer): TConstInterface; virtual; abstract;
    procedure SetItem(Index: integer; Item: TConstInterface); virtual; abstract;
  public
    function Append: TConstInterface; virtual; abstract;
    function ConstByID(ID: integer): TConstInterface; virtual; abstract;
    function ConstByName(Name: string): TConstInterface; virtual; abstract;

    property Items[Index: integer]: TConstInterface read GetItem write SetItem; default;
  end;

  TParamInterface = class(TParamBaseInterface)
  private
    FOwner: TParamCollectionInterface;
    FLinks: TStringList;
    FMultiSelect: boolean;

    function GetCPName: string;
  protected
    FMembers: IXMLDOMDocument2;

    function GetMembers: IXMLDOMDocument2;
    function GetFullName: string;
    function GetSheetInterface: TSheetInterface; override;
    function GetSheetDimension: TSheetDimension; virtual; abstract;
    function GetDimension: string; virtual; abstract;
  public
    constructor Create(AOwner: TParamCollectionInterface);
    destructor Destroy; override;
    function GetPresenceInTask(out TaskParam: TTaskParam): TParamPresence; virtual; abstract;
    function GetModifiedName: string; virtual; abstract;
    procedure SetLink(SheetDimension: TSheetDimension); virtual; abstract;
    procedure RemoveLink(SheetDimension: TSheetDimension); virtual; abstract;
    procedure Delete; virtual; abstract;
    procedure SetMembers(Dom: IXMLDOMDocument2); overload;
    procedure SetMembers(Xml: string); overload;


    property Dimension: string read GetDimension;
    property Owner: TParamCollectionInterface read FOwner write FOwner;
    property Members: IXMLDOMDocument2 read GetMembers;
    property FullName: string read GetFullName;
    property Links: TStringList read FLinks write FLinks;
    property MultiSelect: boolean read FMultiSelect write FMultiSelect;
    property SheetDimension: TSheetDimension read GetSheetDimension;
    property CPName: string read GetCPName;
  end;


  TParamCollectionInterface = class(TParamBaseCollectionInterface)
  private
  protected
    function GetItem(Index: integer): TParamInterface; virtual; abstract;
    procedure SetItem(Index: integer; Item: TParamInterface); virtual; abstract;
    function Append: TParaminterface; virtual; abstract;
  public
    function AddParam(SheetDimension: TSheetDimension): TParamInterface; virtual; abstract;
    function ParamByName(Name: string): TParamInterface; virtual; abstract;
    function ParamByPid(Pid: integer): TParamInterface; virtual; abstract;
    function ParamById(Id: integer): TParamInterface; virtual; abstract;

    property Items[Index: integer]: TParamInterface read GetItem write SetItem; default;
  end;
  // �������� � UniqueId ��������� �����, � ������� ���������� ��������
  (*TParamCollectionInterface = class(TList)
  private
    FOwner: TSheetInterface;
  protected
    function GetItem(UniqueId: string): TParamInterface; virtual; abstract;
    procedure SetItem(UniqueId: string; Value: TParamInterface); virtual; abstract;
  public
    constructor Create(Owner: TSheetInterface);
    property Owner: TSheetInterface read FOwner;
    function AddParam(UniqueId: string): TParamInterface; overload; virtual; abstract;
    function AddParam(SheetDimension: TSheetDimension): TParamInterface; overload; virtual; abstract;
    procedure DeleteParam(UniqueId: string); virtual; abstract;
    function ParamByName(Name: string): TParamInterface; virtual; abstract;
    function ParamByIndex(Index: integer): TParamInterface; virtual; abstract;
    property Items[UniqueId: string]: TParamInterface read GetItem write SetItem; default;
  end;  *)

  TSummaryOptions = class
  private
    FDeployment: TItemDeployment;
    FTitle: string;
    FAllCapitals: boolean;
    FFontOptions: TFontOptions;
    function GetEnabled: boolean;
  public
    constructor Create;
    destructor Destroy; override;
    procedure WriteToXML(Node: IXMLDOMNode);
    procedure ReadFromXML(Node: IXMLDOMNode);
    procedure Copy(From: TSummaryOptions);
    function IsEqualTo(Another: TSummaryOptions): boolean;
    function GetCaption(ParentName: string): string;

    property Deployment: TItemDeployment read FDeployment write FDeployment;
    property Enabled: boolean read GetEnabled;
    property Title: string read FTitle write FTitle;
    property AllCapitals: boolean read FAllCapitals write FAllCapitals;
    property FontOptions: TFontOptions read FFontOptions;
  end;

  TElementStyles = class
  private
    {����� ������}
    FStyleNames: array[TElementStyle] of string;
    function GetStyleName(ElementStyle: TElementStyle): string;
    procedure SetStyleName(ElementStyle: TElementStyle; AName: string);
  public
    constructor Create;
    procedure WriteToXML(Node: IXMLDOMNode);
    procedure ReadFromXML(Node: IXMLDOMNode);
    procedure Copy(From: TElementStyles);

    property Name[ElementStyle: TElementStyle]: string read GetStyleName
      write SetStyleName; default;
  end;

  TFontOptions = class
  private
//    FSubscript: boolean;
    FBold: boolean;
    FItalic: boolean;
    FUnderline: boolean;
    FStrikethrough: boolean;
//    FSuperscript: boolean;
    FColorIndex: integer;
    FSize: integer;
    FName: string;
    FSubscript: boolean;
    FSuperscript: boolean;
  public
    constructor Create;
    procedure ReadFromXml(Node: IXMLDOMNode);
    procedure WriteToXml(Node: IXMLDOMNode);
    procedure CopyFromFont(AFont: ExcelXP.Font); overload;
    procedure CopyFromFont(AFont: TFont); overload;
    procedure CopyFrom(Another: TFontOptions);
    procedure CopyToFont(AFont: ExcelXP.Font); overload;
    procedure CopyToFont(AFont: TFont); overload;
    function IsEqualTo(Another: TFontOptions): boolean;

    property Bold: boolean read FBold write FBold;
    property ColorIndex: integer read FColorIndex write FColorIndex;
    property Italic: boolean read FItalic write FItalic;
    property Name: string read FName write FName;
    property Size: integer read FSize write FSize;
    property Strikethrough: boolean read FStrikethrough write FStrikethrough;
    property Subscript: boolean read FSubscript write FSubscript;
    property Superscript: boolean read FSuperscript write FSuperscript;
    property Underline: boolean read FUnderline write FUnderline;
  end;

  { ��������������� �����, �������� �������� � ���������� �����, ��������� ���
    ������.}
  TWritablesInfo = class
  private
    { �������� �������� �������� ������������ ����������� ������������
      ������ ������� �����������.}
    FWritableColumns: TByteSet;
    { ������������ ����������� ��������� - ������� ����������� �
      ����������� �� �������������� � ������ "������ � �������"}
    FEditableColumns: TByteSet;
    { �������� ����� ���� ��������� ����������� �����}
    FSingleCellsNames: TStringList;
    { ����� ��������� ����������� � ����������� �� ��������������
      � ������ "������ � �������"}
    FEditableSingleCells: TStringList;
    { �������� ���������� ��������� ����������� �����}
    FSingleCellsPoints: TPoints;
  public
    constructor Create;
    destructor Destroy; override;
    procedure ReadFromXml(Node: IXMLDOMNode);
    procedure WriteToXml(Node: IXMLDOMNode);
    function IsColumnWritable(ColumnIndex: integer): boolean;

    function CheckForWritableColumn(ESheet: ExcelWorkSheet;
      Target: ExcelRange; var Editable: boolean): boolean;
    function CheckForWritableCell(ESheet: ExcelWorkSheet; Target: ExcelRange;
      out Editable: boolean): boolean;
    function CheckForWritableRange(ESheet: ExcelWorkSheet; Target: ExcelRange;
      out Editable: boolean): boolean;
    function NoWritableColumns: boolean;

    procedure CopyFrom(Another: TWritablesInfo);
    procedure Clear;
    procedure ClearColumns;
    procedure ClearCells;
    procedure ClearCellsPoints;
    procedure Add(Total: TSheetTotalInterface; ColumnIndex: byte); overload;
    procedure Add(Cell: TSheetSingleCellInterface); overload;
    procedure Delete(CellName: string);

    {�������� ������ ����� ��� ������ ���������� ��������� ��������� ���������
    �����, ������� ���������������� ������� UpdateSingleCellsPoints ��� ��������
    ��������� ������}
    function IsSingleCellSelected(ESheet: ExcelWorkSheet;
      Row, Column: integer): boolean; overload;
    procedure UpdateSingleCellsPoints(ExcelSheet: ExcelWorksheet);
    {��������� ������������� ������ IsSingleCellSelected, ����� ������������ �
    ��� ������������� ��������� ��������� ��������� �����������}
    function IsSingleCellSelected(ESheet: ExcelWorkSheet; Row, Column: integer;
      out Writable, Editable: boolean): boolean; overload;
    function IsSingleCellSelected(ESheet: ExcelWorkSheet; Target: ExcelRange;
      out Writable, Editable: boolean): boolean; overload;
    function IsSingleCellSelected(ESheet: ExcelWorkSheet; Row, Column: integer;
      out CellId: string): boolean; overload;
    function IsSingleCellSelected(ESheet: ExcelWorkSheet; Target: ExcelRange;
      out CellId: string): boolean; overload;
  end;

  TTotalCounters = class
  private
    FCounters: array[TSheetTotalType] of int64;
  public
    constructor Create;
    procedure ReadFromXml(Node: IXMLDOMNode);
    procedure WriteToXml(Node: IXMLDOMNode);
    function GetCounterValue(TotalType: TSheetTotalType): string;
  end;

  TTaskParamBase = class
  private
    FIsInherited: boolean;
    FId: integer;
    FName: string;
    FComment: string;
    FValues: string;
  public
    procedure ReadFromXml(Node: IXMLDOMNode); virtual;

    property IsInherited: boolean read FIsInherited;
    property Id: integer read FId;
    property Name: string read FName;
    property Comment: string read FComment;
    property Values: string read FValues write FValues;
  end;

  TTaskParam = class(TTaskParamBase)
  private
    FDimension: string;
    FAllowMultiSelect: boolean;
  public
    constructor Create(AOwner: TTaskParamsCollection); virtual; abstract;

    property Dimension: string read FDimension write FDimension;
    property AllowMultiSelect: boolean read FAllowMultiSelect write FAllowMultiSelect;
  end;

  {��������� ���������� "�� ������"}
  TTaskParamsCollection = class(TList)
  private
  protected
    function GetItem(Index: integer): TTaskParam; virtual; abstract;
    procedure SetItem(Index: integer; Item: TTaskParam); virtual; abstract;
    function Append: TTaskParam; virtual; abstract;
    function  GetIsReadOnly: boolean; virtual; abstract;
  public
    procedure ReadFromXml(Node: IXMLDOMNode); virtual; abstract;

    function  ParamByID(ID: integer): TTaskParam; virtual; abstract;
    function  ParamByName(Name: string): TTaskParam; virtual; abstract;

    property Items[Index: integer]: TTaskParam read GetItem write SetItem; default;
    property IsReadOnly: boolean read GetIsReadOnly;
  end;

  TTaskConst = class(TTaskParamBase)
  private
  public
    constructor Create(AOwner: TTaskConstsCollection); virtual; abstract;
  end;


  {��������� �������� "�� ������"}
  TTaskConstsCollection = class(TList)
  private
  protected
    function GetItem(Index: integer): TTaskConst; virtual; abstract;
    procedure SetItem(Index: integer; Item: TTaskConst); virtual; abstract;
    function Append: TTaskConst; virtual; abstract;
    function  GetIsReadOnly: boolean; virtual; abstract;
  public
    procedure ReadFromXml(Node: IXMLDOMNode); virtual; abstract;

    function  ConstByID(ID: integer): TTaskConst; virtual; abstract;
    function  ConstByName(Name: string): TTaskConst; virtual; abstract;

    property Items[Index: integer]: TTaskConst read GetItem write SetItem; default;
    property IsReadOnly: boolean read GetIsReadOnly;
  end;

  TTaskContext = class
  private
  protected
    function GetParamCount: integer; virtual; abstract;
    function GetConstCount: integer; virtual; abstract;
  public
    constructor Create; virtual; abstract;
    procedure ReadFromXml(Dom: IXMLDOMDocument2); virtual; abstract;
    procedure Clear; virtual; abstract;

    function GetTaskParams: TTaskParamsCollection; virtual; abstract;
    function GetTaskConsts: TTaskConstsCollection; virtual; abstract;

    property ParamCount: integer read GetParamCount;
    property ConstCount: integer read GetConstCount;
  end;

  TTaskEnvironment = class
  private
    FSilentMode: boolean;
    FContextType: boolean;
    FIsLoadingFromTask: boolean;
    FIsTaskConnect: boolean;
    FSheetType: integer;
    FAuthType: integer;
    FLogin: string;
    FPwdHash: string;
    FConnectionStr: string;
    FAlterConnection: string;
    FSchemeName: string;
    FTaskId: string;
    FDocumentId: string;
    FTaskName: string;
    FDocumentName: string;
    FOwner: string;
    FDocPath: string;
    FDocType: string;
    FAction: integer;
    FMutexName: string;
    MutexHandle: THandle;

  public
    procedure ReadTaskProperties(Book: ExcelWorkbook);
    procedure ReadDynamicProperties(Props: IDispatch);
    procedure ClearDynamicProperties(Props: IDispatch);
    procedure ReadStaticProperties(Props: IDispatch);
    procedure ClearStaticProperties(Props: IDispatch);

    procedure CreateActionMutex;
    procedure ReleaseActionMutex;
    procedure SetActionResult(Book: ExcelWorkbook; Success: boolean; Message: string);

    property IsLoadingFromTask: boolean read FIsLoadingFromTask write FIsLoadingFromTask;
    property SilentMode: boolean read FSilentMode write FSilentMode;

    property ContextType: boolean read FContextType write FContextType;
    property SheetType: integer read FSheetType write FSheetType;
    property IsTaskConnect: boolean read FIsTaskConnect write FIsTaskConnect;
    property AuthType: integer read FAuthType write FAuthType;
    property Login: string read FLogin write FLogin;
    property PwdHash: string read FPwdHash	write FPwdHash;
    property ConnectionStr: string read FConnectionStr write FConnectionStr;
    property AlterConnection: string read FAlterConnection write FAlterConnection;
    property SchemeName: string read FSchemeName write FSchemeName;
    property Action: integer read FAction write FAction;
    property MutexName: string read FMutexName write FMutexName;

    {����������� ��������}
    property DocumentName: string read FDocumentName write FDocumentName;
    property DocumentId: string read FDocumentId write FDocumentId;
    property TaskName: string read FTaskName write FTaskName;
    property TaskId: string read FTaskId write FTaskId;
    property Owner: string read FOwner write FOwner;
    property DocPath: string read FDocPath write FDocPath;
    property DocType: string read FDocType write FDocType;
  end;

function CheckDimension(Cube: TCube; DimElem: TSheetDimension): boolean;

implementation

{************* TSheetElement implementation **************}

constructor TSheetElement.Create(AOwner: TSheetCollection);
begin
  FOwner := AOwner;
  FUniqueID := SheetInterface.GetUniqueID;
  FStyles := TElementStyles.Create;
  SetDefaultStyles;
end;

destructor TSheetElement.Destroy;
begin
  FOwner := nil;
  FreeAndNil(FStyles);
  inherited Destroy;
end;

procedure TSheetElement.StoreData(DOM: IXMLDOMDocument2);
begin
  FOwner.Owner.PutData(DOM, FUniqueID);
end;

function TSheetElement.GetSheetInterface: TSheetInterface;
begin
  result := Owner.Owner;
end;

{********* TSheetCollection implementation ************}

constructor TSheetCollection.Create(AOwner: TSheetInterface);
begin
  FOwner := AOwner;
  FUpdating := false;
  FStyles := TElementStyles.Create;
  SetDefaultStyles;
  UseStylesForChildren := false;
end;

destructor TSheetCollection.Destroy;
begin
  Clear;
  FOwner := nil;
  FreeAndNil(FStyles);
  inherited Destroy;
end;

procedure TSheetCollection.Clear;
begin
  FUpdating := true;
  try
    while Count > 0 do
      Delete(0);
  finally
    FUpdating := false;
  end;
end;

function TSheetCollection.GetEmpty: boolean;
begin
  result := Count = 0;
end;

function TSheetCollection.GetObjectTypeStr: string;
begin
  case ObjectType of
    wsoRow: result := '��������� �����';
    wsoColumn: result := '��������� ��������';
    wsoFilter: result := '��������� ��������';
    wsoTotal: result := '��������� �����������';
  end;
end;

destructor TSheetFilterInterface.Destroy;
begin
  FreeStringList(FScope);
  inherited Destroy;
end;


function TSheetLevelInterface.GetAxisElement: TSheetAxisElementInterface;
begin
  result := ParentCollection.Owner;
end;

function TSheetLevelInterface.GetHideDataMembers: boolean;
begin
  result := DMDeployment = idNone;
end;

function TSheetLevelInterface.GetInitialIndex: integer;
var
  Hierarchy: THierarchy;
  i: integer;
begin
  if FInitialIndex = -1 then
  begin
    Hierarchy := AxisElement.CatalogHierarchy;
    for i := 0 to Hierarchy.Levels.Count - 1 do
      if Hierarchy.Levels[i].Name = Name then
      begin
        FInitialIndex := i;
        break;
      end;
  end;
  result := FInitialIndex;
end;

function TSheetLevelInterface.GetParentCollection: TSheetLevelCollectionInterface;
begin
  result := (Owner as TSheetLevelCollectionInterface);
end;

constructor TSheetLevelCollectionInterface.Create(AOwner: TSheetAxisElementInterface);
begin
  inherited Create(nil);
  FOwner := AOwner;
end;

destructor TSheetLevelCollectionInterface.Destroy;
begin
  FOwner := nil;
  inherited Destroy;
end;



destructor TSheetAxisElementInterface.Destroy;
begin
  if Assigned(Levels) then
  begin
    FLevels.Clear;
    FreeAndNil(FLevels);
  end;

  inherited Destroy;
end;

constructor TSheetMPCollectionInterface.Create(AOwner: TSheetDimension);
begin
  FOwner := AOwner;
end;

destructor TSheetMPCollectionInterface.Destroy;
begin
  FOwner := nil;
  inherited Destroy;
end;


constructor TSheetMPElementInterface.Create(AOwner: TSheetMPCollectionInterface);
begin
  inherited Create;
  FOwner := AOwner;
  FMask := '';
  FChecked := false;
end;

destructor TSheetMPElementInterface.Destroy;
begin
  FOwner := nil;
  inherited;
end;

procedure TSheetInterface.PutData(Dom: IXMLDOMDocument2; UniqueID: string);
begin
  if Assigned(Dom) then
  try
    PutDataInCP(ExcelSheet, UniqueID, Dom, true);
  except
    AddEventInSheetHistory(evtUnknown, '�� ������� ���������� �� � id=' + UniqueId, false);
  end;
end;

function TSheetInterface.GetData(UniqueID: string): IXMLDOMDocument2;
begin
  result := GetDataFromCP(ExcelSheet, UniqueID);
end;

function TSheetInterface.GetAxis(Axis: TAxisType): TSheetAxisCollectionInterface;
begin
  result := nil;
  case Axis of
    axRow: result := Rows;
    axColumn: result := Columns;
  end;
end;

procedure TSheetInterface.ShowMessageEX(AMsg: string; AType: TMsgType);
begin
  if (Assigned(FProcess) and FProcess.Showing) then
    case AType of
      msgError: FProcess.PostError(AMsg);
      msgWarning: FProcess.PostWarning(AMsg);
      else FProcess.PostInfo(AMsg);
    end
  else
    if not IsSilentMode then
    case AType of
      msgError: ShowError(AMsg);
      msgWarning: ShowWarning(AMsg);
      else ShowInfo(AMsg);
    end;
end;

procedure TSheetInterface.ShowDetailedError(Error, Details,
  Caption: string);
begin
  if (Assigned(FProcess) and FProcess.Showing) then
    FProcess.PostError(Error)
  else
    if not IsSilentMode then
      ShowDetailError(Error, Details, Caption);
end;

procedure TSheetInterface.PostMessage(AMsg: string; AType: TMsgType);
begin
  AMsg := IIF(trim(AMsg) <> '', AMsg, ermUnknown);
  if Assigned(FProcess) then
    if FProcess.Showing then
      case AType of
        msgError: FProcess.PostError(AMsg);
        msgWarning: FProcess.PostWarning(AMsg);
        else FProcess.PostInfo(AMsg);
      end;
end;

procedure TSheetInterface.OpenOperation(Msg: string;
  IsCritical, IsTimed: boolean; OperationType: integer);
begin
  Msg := IIF(trim(Msg) <> '', Msg, ermUnknown);
  if Assigned(FProcess) then
    if FProcess.Showing then
      FProcess.OpenOperation(Msg, IsCritical, IsTimed, OperationType);
end;

function TSheetInterface.GetProcessShowing: boolean;
begin
  result := false;
  if Assigned(FProcess) then
    result := FProcess.Showing;
end;

procedure TSheetInterface.CloseOperation;
begin
  if Assigned(FProcess) then
    if FProcess.Showing then
      FProcess.CloseOperation;
end;

procedure TSheetInterface.SetPBarPosition(CurPosition, MaxPosition: integer);
begin
  if Assigned(FProcess) then
    if FProcess.Showing then
      FProcess.SetPBarPosition(CurPosition, MaxPosition);
end;

procedure TSheetInterface.AddEventInSheetHistory(EventType: TSheetEventType;
  Comment: string; IsSuccess: boolean);
begin
  if Assigned(FSheetHistory) then
    FSheetHistory.AddEvent(ExcelSheet, EventType, Comment, IsSuccess);
end;

function TSheetInterface.CheckConnection: boolean;
begin
  result := false;

  if Assigned(DataProvider) then
  begin
    if DataProvider.Connected then
      result := true
    else
      PostMessage(ermNoneConnection, msgError);
  end
  else
    PostMessage(ermDataProviderUnknown, msgError);
end;

{ TSheetDimension }

function TSheetDimension.GetFullDimName: string;
begin
  result := Dimension;
  if Hierarchy <> '' then
    result := result + '.' + Hierarchy;
end;

function TSheetDimension.GetFullDimName2: string;
var
  Dim, Hier: string;
begin
  Dim := Dimension;
  Hier := Hierarchy;
  if Hier = Dim then
    Hier := '';
  if (Pos(snSemanticsSeparator, Dimension) > 0) then
    Dim := StringReplace(Dimension, snSemanticsSeparator, '.', []);
  if (Hier <> '') then
    Hier := '.' + Hier;
  result := Dim + Hier;
end;

function TSheetDimension.GetFullDimNameMDX: string;
begin
  result := MemberBrackets(Dimension);
  if Hierarchy <> '' then
    result := result + '.' + MemberBrackets(Hierarchy);
end;

function TSheetDimension.GetMembers: IXMLDOMDocument2;
begin
  if IsParam then
    result := Param.Members
  else
  begin
    if not Assigned(FMembers) or not Assigned(FMembers.documentElement) then
      FMembers := FOwner.Owner.GetData(CPName);
    result := FMembers;
  end;
end;

procedure TSheetDimension.SetMembers(DOM: IXMLDOMDocument2);
begin
  if not Assigned(DOM) then
    exit;

  if Self is TSheetAxisElementInterface then
    FilterMembersDomEx(DOM);

  if not Assigned(FMembers) then
    GetDOMDocument(FMembers);
  FMembers.loadXML(DOM.XML);
  if IsParam then
    Param.SetMembers(DOM);
end;

function TSheetDimension.GetAllMember: string;
var
  Hier: THierarchy;
begin
  result := '';
  Hier := CatalogHierarchy;
  if Assigned(Hier) then
    result := Hier.AllMember;
end;

function TSheetDimension.GetFullName: string;
var
  Hier: THierarchy;
begin
  result := 'null';
  Hier := CatalogHierarchy;
  if not Assigned(Hier) then
    exit;
  result := Hier.FullName;
end;

function TSheetDimension.GetHierarchyId: string;
var
  Hier: THierarchy;
begin
  result := 'null';
  Hier := CatalogHierarchy;
  if not Assigned(Hier) then
    exit;
  result := Hier.HierarchyId;
end;

function TSheetDimension.IsParam: boolean;
begin
  try
    result := Assigned(Param);
    if result then
      result := Param.Name <> '';
  except
    result := false;
  end;
end;

destructor TSheetDimension.Destroy;
begin
  if Assigned(FMemberProperties) then
  begin
    FMemberProperties.Clear;
    FreeAndNil(FMemberProperties);
  end;
  if Assigned(FMembers) then
    KillDomDocument(FMembers);
  if IsParam then
    Param.RemoveLink(Self);
end;

procedure TSheetInterface.SetExternalLinks(Catalog: TXMLCatalog;
  ProcessForm: IProcessForm; Provider: IPlaningProvider;
  SheetHistory: TSheetHistory);
begin
  FXMLCatalog := Catalog;
  FProcess := ProcessForm;
  FDataProvider := Provider;
  FSheetHistory := SheetHistory;
end;

{ TSheetSingleCell }

destructor TSheetSingleCellInterface.Destroy;
begin
  FFilters.Free;
  inherited Destroy;
end;

function TSheetDimension.GetCPName: string;
begin
  if IsParam then
    result := Param.CPName
  else
    result := FUniqueId;
end;

procedure TSheetDimension.StoreData(DOM: IXMLDOMDocument2);
begin
  SheetInterface.PutData(DOM, CPName);
end;

function TSheetDimension.GetElementCaption: string;
begin
  result := FullDimensionName;
end;

function TSheetDimension.GetCatalogHierarchy: THierarchy;
var
  Dim: TDimension;
begin
  result := nil;
  if not Assigned(SheetInterface.XMLCatalog) then
    exit;
  if SheetInterface.XMLCatalog.Loaded then
  begin
    Dim := SheetInterface.XMLCatalog.Dimensions.Find(Dimension, ProviderId);
    if Assigned(Dim) then
      result := Dim.GetHierarchy(Hierarchy);
  end;
end;

function TSheetDimension.GetAllMemberProperties: string;
var
  Hier: THierarchy;
begin
  result := '';
  Hier := CatalogHierarchy;
  if Assigned(Hier) then
    result := Hier.MemberProperties.GetCommaList;
end;

procedure TSheetDimension.EnlightMembersDom(ShouldLeaveNames: boolean);
var
  i, j: integer;
  MembersNL: IXMLDOMNodeList;
  AName: string;
begin
  if not Assigned(Members) then
    exit;
  CutAllInvisible(Members, false);
  MembersNL := Members.selectNodes('//Member');
  for i := 0 to MembersNL.length - 1 do
    for j := MembersNL[i].attributes.length - 1 downto 0 do
    begin
      AName := MembersNL[i].attributes[j].nodeName;
      if (AName = attrLocalId) or (AName = attrPKID)
        or (ShouldLeaveNames and ((AName = attrName) or (AName = attrUniqueName))) then
        continue;
      MembersNL[i].attributes.removeNamedItem(AName);
    end;
end;

procedure TSheetDimension.TurnAllLevelsOn;
var
  NL: IXMLDOMNodeList;
  i: integer;
begin
  GetMembers;
  if not Assigned(Members) then
    exit;
  NL := Members.selectNodes('function_result/Levels/Level');
  for i := 0 to NL.length - 1 do
    if GetIntAttr(NL[i], attrLevelState, 0) = 0 then
      SetAttr(NL[i], attrLevelState, 1);
  StoreData(Members);
end;

function TSheetDimension.Refresh(Force: boolean): boolean;
var
  OldMembers, NewMembers: IXMLDOMDocument2;
begin
  result := false;
  if not SheetInterface.CheckConnection then
    exit;

  SheetInterface.OpenOperation(pfoDimensionRefresh + FullDimensionName2,
    not CriticalNode, NoteTime, otUpdate);
  try
    OldMembers := Members;
    // ���� �������� � �������� � ��������� ����� - ����� �������� �� ������
    if IsParam and Assigned(SheetInterface.TaskContext) then
    begin
      try
        OldMembers.loadXML(SheetInterface.TaskContext.GetTaskParams.ParamByName(Param.Name).Values);
      except
      end;
      {����� ��������� �� ��� ���� � �����, �� ����������� �������� ���������
       � �������. ��� �� ������ ����, ��� ������� �� ������� �� ���� ����������
       ����� ���������. �� � ������� (����������) ����� ��� ����������.
       � ����� ����� �������, ����� ����������� ������, ��� �� �� ���� ��������������}
      setMembers(OldMembers);
      RecreateLevelsByMembers;
    end;

    if not Assigned(OldMembers) then
      exit;

    try
      if Force then
      begin
        NewMembers := SheetInterface.DataProvider.GetMemberList(
          ProviderId, '', Dimension, Hierarchy,
          '', AllMemberProperties);
        if (SheetInterface.DataProvider.LastWarning <> '') then
          SheetInterface.PostMessage(SheetInterface.DataProvider.LastWarning, msgWarning);
      end
      else
      begin
        if not SheetInterface.DataProvider.UpdateMemberList(ProviderId,
          OldMembers, NewMembers, '', Dimension, Hierarchy, '', AllMemberProperties) then
        begin
          result := true;
          exit; //��������� �� ���������� (������ ������ �� ����)
        end;
        if (SheetInterface.DataProvider.LastWarning <> '') then
          SheetInterface.PostMessage(SheetInterface.DataProvider.LastWarning, msgWarning);
      end;
      if (SheetInterface.DataProvider.LastError <> '') then
      begin
        SheetInterface.PostMessage(SheetInterface.DataProvider.LastError, msgWarning);
        exit;
      end;
    except
      SheetInterface.PostMessage('�� ������� �������� ��������� ' + FullDimensionName2 + ';', msgWarning);
      exit; //� ����� ��� � ������ ��������.
    end;

    if not Assigned(NewMembers) then
      exit;

    CopyMembersState(OldMembers, NewMembers, SheetInterface.SetPBarPosition);
    SetCheckedIndication(NewMembers);

    //��������� ����������� ������
    setMembers(NewMembers);

    OldMembers := nil;
    result := true;
  finally
    SheetInterface.CloseOperation;
  end;
end;

function TSheetDimension.IsBaseParam: boolean;
begin
  result := false;
  if not IsParam then
    exit;
  result := Param.SheetDimension = Self;
end;

{ TSheetBasicTotal }

function TSheetBasicTotal.BuildFormatMask: string;
const
  Space = #32;
  Nbsp = #160;
var
  i: integer;
  ColorStr: string;
begin
  result := '';
  case SheetInterface.CountrySetting of
(*!! ��������, ��� ������ � ������ ����������� �����
    1: ColorStr := ';[Red]-'; *)
    7: ColorStr := ';[�������]-';
    else ColorStr := '';
  end;
  case Format of
    fmtCurrency, fmtNumber:
      begin
        result := '#' + SheetInterface.TSeparator + '##0';
        if Digits > 0 then
        begin
          result := result + SheetInterface.DSeparator;
          for i := 1 to Digits do
            result := result + '0';
        end;
        if ColorStr <> '' then
          result := result + ColorStr + result;
      end;
    fmtPercent:
      begin
        result := '0';
        if Digits > 0 then
        begin
          result := result + SheetInterface.DSeparator;
          for i := 1 to Digits do
            result := result + '0';
        end;
        result := result + '%';
        if ColorStr <> '' then
          result := result + ColorStr + result;
      end;
    fmtText: result := '@';
  end;
  result := StringReplace(result, Nbsp, Space, [rfReplaceAll]);
end;

function TSheetBasicTotal.GetCube: TCube;
begin
  result := nil;
  if not Assigned(SheetInterface.XmlCatalog) then
    exit;
  if not SheetInterface.XmlCatalog.Loaded then
    exit;
  result := SheetInterface.XMLCatalog.Cubes.Find(CubeName, ProviderId);
end;

function TSheetBasicTotal.GetMeasure: TMeasure;
begin
  result := nil;
  if TotalType in [wtFree, wtConst] then
    exit;
  if not Assigned(SheetInterface.XmlCatalog) then
    exit;
  if not SheetInterface.XmlCatalog.Loaded then
    exit;
  if Assigned(Cube) then
    result := Cube.Measures.Find(MeasureName) as TMeasure;
end;

procedure TSheetBasicTotal.Round(var AValue: extended);
begin
  case Format of
    fmtCurrency: AValue := SimpleRoundTo(AValue, Digits);
    fmtPercent: AValue := SimpleRoundTo(AValue, Digits + 2);
  end;
end;

procedure TSheetBasicTotal.SetDigits(Value: integer);
begin
  if Value in [0..12] then
  begin
    FDigits := Value;
    NumberFormat := BuildFormatMask;
  end;
end;

procedure TSheetBasicTotal.SetFormat(Value: TMeasureFormat);
begin
  FFormat := Value;
  NumberFormat := BuildFormatMask;
end;

function TSheetBasicTotal.GetEmptySymbol: string;
begin
try
  result := IIF((FEmptyValueSymbol = fmEmptyCell), fmEmptyCell, FEmptyValueSymbol);
except
(*  on e: exception do
    showerror(e.message);*)
end;
end;

procedure TSheetCollection.Delete(Index: Integer);
begin
  inherited Delete(Index);
end;

procedure TSheetBasicTotal.ReadFromXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  if not SheetInterface.InCopyMode then
    UniqueID := GetStrAttr(Node, attrID, '');
  FTotalType := TSheetTotalType(GetIntAttr(Node, attrTotalType, 0));
  SetDefaultStyles;
  FCaption := GetNodeStrAttr(Node, attrCaption);
  FCubeName := GetNodeStrAttr(Node, attrCube);
  FMeasureName := GetNodeStrAttr(Node, attrMeasure);
  FFormat := TMeasureFormat(GetIntAttr(Node, attrFormat, 0));
  FDigits := GetIntAttr(Node, attrDigits, 1);
  FTypeFormula.ReadFromXML(Node.selectSingleNode(attrTypeFormula));
  inherited ReadFromXML(Node);
end;

procedure TSheetBasicTotal.WriteToXML(Node: IXMLDOMNode);
var
  tmpNode: IXMLDOMNode;
begin
  inherited WriteToXML(Node);
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrTotalType, TotalType);
    setAttribute(attrFormat, Ord(Format));
    setAttribute(attrDigits, Digits);
    tmpNode := Node.ownerDocument.createNode(1, 'caption', '');
    tmpNode.text := Caption;
    Node.appendChild(tmpNode);
    if (TotalType in [wtMeasure, wtResult]) then
    begin
      SetNodeStrAttr(Node, attrCube, CubeName);
      SetNodeStrAttr(Node, attrMeasure, MeasureName);
    end;
  end;
  tmpNode := Node.ownerDocument.CreateNode(1, attrTypeFormula,'');
  FTypeFormula.WriteToXML(tmpNode);
  Node.appendChild(tmpNode);
end;

function TSheetBasicTotal.GetElementCaption: string;
begin
  result := Caption;
end;

constructor TSheetBasicTotal.Create(AOwner: TSheetCollection);
begin
  Inherited Create(AOwner);
  FTypeFormula := TTypeFormula.Create(Self);
end;

destructor TSheetBasicTotal.Destroy;
begin
  FreeAndNil(FTypeFormula);
  inherited;
end;

procedure TSheetBasicTotal.ClearLinkedTypeFormulasValues;
var
  i: integer;
  CurTotal: TSheetTotalInterface;
begin
  SheetInterface.WritablesInfo.UpdateSingleCellsPoints(SheetInterface.ExcelSheet);
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    CurTotal := SheetInterface.Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(Alias) then
      {������� �������� ������� �������}
      CurTotal.ClearTypeFormulaValues;
  end;
end;

procedure TSheetBasicTotal.MapLinkedTypeFormulasValues;
var
  i: integer;
  CurTotal: TSheetTotalInterface;
begin
  SheetInterface.WritablesInfo.UpdateSingleCellsPoints(SheetInterface.ExcelSheet);
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    CurTotal := SheetInterface.Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(Alias) then
      {��������� �������� ������� �������}
      SheetInterface.MapTypeFormula(CurTotal);
  end;
end;

function TSheetBasicTotal.GetLinkedTypeFormulasWarning: string;
var
  TotalList: TStringList;
  i: integer;
  CurTotal: TSheetTotalInterface;
begin
  result := '';

  {������� ������ �����������, � ������� ������� ������� �������� ������ �� ������ ����������}
  TotalList := TStringList.Create;
  for i := 0 to SheetInterface.Totals.Count - 1 do
  begin
    CurTotal := SheetInterface.Totals[i];
    if CurTotal.TypeFormula.Enabled and CurTotal.TypeFormula.ContainAlias(Alias) then
      TotalList.Add(CurTotal.Caption);
  end;

  try
    if TotalList.Count = 0 then
      exit;
    result := '��������! �� ��������� ������� ��������� ������� ������� ��������� �����������:' + #10;
    for i := 0 to TotalList.Count - 1 do
      result := result + '"'+ TotalList.Strings[i] + '", ';
    result[Length(result) - 1] := '.';
    result := result + #10 + '��� �������� ������� ������� ��������� ����������� ������ �����������.';
  finally
    FreeStringList(TotalList);
  end;
end;

{ TParamInterface }

constructor TParamInterface.Create(AOwner: TParamCollectionInterface);
begin
  inherited Create;
  FOwner := AOwner;
  Links := TStringList.Create;
end;

destructor TParamInterface.Destroy;
begin
  inherited;
  FreeStringList(FLinks);
end;

function TParamInterface.GetFullName: string;
begin
  result := Name;
  if IsInherited then
    result := result + ' (�� ������������ ������)';
end;

function TParamInterface.GetMembers: IXMLDOMDocument2;
begin
  if Assigned(FMembers) then
    result := FMembers
  else
    result := SheetInterface.GetData(CPName);
  if not Assigned(result) then
  begin
    GetDomDocument(FMembers);
    result := FMembers;
  end;
end;

procedure TParamInterface.SetMembers(Dom: IXMLDOMDocument2);
var
  s: string;
begin
  s := Dom.xml;
  if Assigned(FMembers) then
    FMembers.loadXml(s) // ������� FMembers.load(Dom) ������������ ��������
  else
    SheetInterface.PutData(Dom, CPName);
end;

procedure TParamInterface.SetMembers(Xml: string);
begin
  if Xml = '' then
    exit;
  {������������ �����, ��� ������������ �������������� ������������ �������� ��������� - ��� ���
   �������� �� ���������. ��������� �������� ������ ���������, ������� ����� ����� ������ ����� � ��}
(*  if Assigned(SheetDimension) then
    SheetDimension.Members.loadXML(Xml);*)
//  PutDataValueInCP(SheetInterface.ExcelSheet, CPName, Xml, false);
  if Assigned(FMembers) then
    FMembers.loadXml(Xml)
  else
  begin
    GetDomDocument(FMembers);
    FMembers.loadXml(Xml);
  end;
  SheetInterface.PutData(FMembers, CPName);
end;

function TSheetElement.GetSelfIndex: integer;
var
  i: integer;
begin
  result := -1;
  for i := 0 to Owner.Count - 1 do
    if Owner.Items[i] = Self then
    begin
      result := i;
      exit;
    end;
end;

function TParamInterface.GetSheetInterface: TSheetInterface;
begin
  result := Owner.Owner;
end;

function TParamInterface.GetCPName: string;
begin
  result := 'p' + IntToStr(Pid);
end;

{ TConstInterface }

constructor TConstInterface.Create(Owner: TConstCollectionInterface);
begin
  FOwner := Owner;
end;

function TSheetInterface.GetTaskParams: TTaskParamsCollection;
begin
  result := nil;
  if Assigned(FTaskContext) then
    result := FTaskContext.GetTaskParams;
end;

function TSheetInterface.CopyCustomProperty(SourceName, DestName: string): boolean;
var
  SourceCP, DestCP: CustomProperty;
begin
  result := false;
  SourceCP := GetCPByName(ExcelSheet, SourceName, false);
  if not Assigned(SourceCP) then
    exit;
  DestCP := GetCPByName(ExcelSheet, DestName, true);
  if not Assigned(DestCP) then
    exit;
  DestCP.Value := SourceCP.Value;
  result := true;
end;

function TSheetInterface.GetPID(ParamName: string): integer;
var
  Param: TParamInterface;
begin
  Param := Params.ParamByName(ParamName);
  if Assigned(Param) then
    result := Param.PID
  else
    result := GetParamID;
end;

procedure TSheetElement.ReadFromXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  FPermitEditing := GetBoolAttr(Node, attrPermitEditing, false);
  FProviderId := GetStrAttr(Node, attrProviderId, SheetInterface.XMLCatalog.PrimaryProvider);
  FStyles.ReadFromXML(Node);
end;

procedure TSheetElement.WriteToXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrID, UniqueID);
    setAttribute(attrPermitEditing, BoolToStr(FPermitEditing));
    setAttribute(attrProviderId, ProviderId);
  end;
  Styles.WriteToXML(Node);
end;

procedure TSheetElement.SetDefaultStyles;
begin
  Styles.Name[esValue] := DefaultStyleName[esValue];
  Styles.Name[esValuePrint] := DefaultStyleName[esValuePrint];
  Styles.Name[esTitle] := DefaultStyleName[esTitle];
  Styles.Name[esTitlePrint] := DefaultStyleName[esTitlePrint];
end;


function TConstInterface.GetSheetInterface: TSheetInterface;
begin
  result := Owner.Owner;
end;

{ TSummaryOptions }

procedure TSummaryOptions.Copy(From: TSummaryOptions);
begin
  FDeployment := From.Deployment;
  FTitle := From.Title;
  FAllCapitals := From.AllCapitals;
  FFontOptions.CopyFrom(From.FontOptions);
end;

constructor TSummaryOptions.Create;
begin
  FDeployment := idBottom;
  FTitle := stUsual;
  FAllCapitals := false;
  FFontOptions := TFontOptions.Create;
end;

destructor TSummaryOptions.Destroy;
begin
  inherited;
  FontOptions.Free;
end;

function TSummaryOptions.GetCaption(ParentName: string): string;
var
  Index: integer;
begin
  result := Title;
  Index := Pos('*', Title);
  if Index > 0 then
  begin
    Delete(result, Index, 1);
    Insert(ParentName, result, Index);
  end;
end;

function TSummaryOptions.GetEnabled: boolean;
begin
  result := Deployment <> idNone;
end;

function TSummaryOptions.IsEqualTo(Another: TSummaryOptions): boolean;
begin
  result :=
    (Deployment = Another.Deployment) and
    (Title = Another.Title) and
    (AllCapitals = Another.AllCapitals) and
    FontOptions.IsEqualTo(Another.FontOptions);
end;

procedure TSummaryOptions.ReadFromXML(Node: IXMLDOMNode);
var
  FontNode: IXMLDOMNode;
begin
  if not Assigned(Node) then
    exit;
  FDeployment := TItemDeployment(GetIntAttr(Node, attrDeployment, 0));
  FTitle := GetStrAttr(Node, 'title', '');
  FAllCapitals := GetBoolAttr(Node, attrAllCapitals, false);
  FontNode := Node.selectSingleNode('fontoptions');
  FontOptions.ReadFromXml(FontNode);
end;

procedure TSummaryOptions.WriteToXML(Node: IXMLDOMNode);
var
  FontNode: IXMLDOMNode;
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrDeployment, Ord(FDeployment));
    setAttribute('title', Title);
    setAttribute(attrAllCapitals, BoolToStr(AllCapitals));
  end;
  FontNode := Node.ownerDocument.createNode(1, 'fontoptions', '');
  FontOptions.WriteToXml(FontNode);
  Node.appendChild(FontNode);
end;

procedure TSheetElement.SetOwnerStyles;
begin
  Styles.Copy(Owner.Styles);
end;

function TSheetCollection.GetItem(Index: integer): TSheetElement;
begin
  result := Get(Index);
end;


{ TSheetAxisCollectionInterface }

constructor TSheetAxisCollectionInterface.Create(AOwner: TSheetInterface);
begin
  inherited;
  SummaryOptions := TSummaryOptions.Create;
  GrandSummaryOptions := TSummaryOptions.Create;
  GrandSummaryOptions.Title := stGrand;
  UseSummariesForElements := true;
end;

destructor TSheetAxisCollectionInterface.Destroy;
begin
  inherited;
  FreeAndNil(FSummaryOptions);
  FreeAndNil(FGrandSummaryOptions);
end;

function TSheetInterface.GetMembersArray(DimensionName: widestring): OleVariant;
var
  Dimension: TSheetDimension;
  MembersDom: IXmlDomDocument2;
  XPath: string;
  i: integer;
  Nodes: IXMLDOMNodeList;
  Name, UniqueName: string;
  ArrayElement: OleVariant;
begin
  result := Null;
  Dimension := GetDimension(DimensionName);
  if not Assigned(Dimension) then
    exit;
  MembersDom := Dimension.Members;
  if not Assigned(MembersDom) then
    exit;
  XPath := 'function_result/Members//' + ntMember + '[@checked="true"]';
  Nodes := MembersDom.SelectNodes(XPath);
  if (Nodes.length = 0) then
    exit;
  result := VarArrayCreate([0, Nodes.length - 1], varVariant);
  for i := 0 to Nodes.length - 1 do
  begin
    Name := GetStrAttr(Nodes[i], 'name', '');
    UniqueName := GetStrAttr(Nodes[i], 'unique_name', '');
    ArrayElement := VarArrayCreate([0, 1], varOleStr);
    ArrayElement[0] := UniqueName;
    ArrayElement[1] := Name;
    result[i] := ArrayElement;
  end;
end;

procedure TSheetInterface.SetMembersByArray(DimensionName: widestring; UniqueNames: OleVariant);
var
  Dimension: TSheetDimension;
  MembersDom: IXmlDomDocument2;
  XPath: string;
  i: integer;
  Node: IXMLDOMNode;
  UniqueName: string;
begin
  if VarIsNull(UniqueNames) then
    exit;
  Dimension := GetDimension(DimensionName);
  if not Assigned(Dimension) then
    exit;
  Dimension.Refresh(true);    
  MembersDom := Dimension.Members;
  if not Assigned(MembersDom) then
    exit;
  MakeDefaultMembersDom(MembersDom);
  for i := VarArrayLowBound(UniqueNames, 1) to VarArrayHighBound(UniqueNames, 1) do
  begin
    UniqueName := UniqueNames[i];
    EncodeXPathString(UniqueName);
    XPath := 'function_result/Members//' + ntMember + '[@unique_name="' + UniqueName + '"]';
    Node := MembersDom.selectSingleNode(XPath);
    if not Assigned(Node) then
      continue;
    (Node as IXMLDOMElement).setAttribute('checked', 'true');
  end;
end;

function TSheetElement.GetStyleCaption(
  ElementStyle: TElementStyle): string;
begin
  result := Owner.StyleCaption[ElementStyle];
end;

function TSheetAxisCollectionInterface.GetLastItem: TSheetAxisElementInterface;
begin
  result := TSheetAxisElementInterface(Last);
end;

function TSheetAxisCollectionInterface.GetLevelByNumber(
  LevelNumber: integer): TSheetLevelInterface;
var
  i: integer;
begin
  result := nil;
  for i := 0 to Count - 1 do
    if Items[i].Levels.Count > LevelNumber then
    begin
      result := Items[i].Levels[LevelNumber];
      exit;
    end
    else
      LevelNumber := LevelNumber - Items[i].Levels.Count;
end;

{ TElementStyles }

function TElementStyles.GetStyleName(ElementStyle: TElementStyle): string;
begin
  result := FStyleNames[ElementStyle];
end;

procedure TElementStyles.SetStyleName(ElementStyle: TElementStyle;
  AName: string);
begin
  if AName <> '' then
    FStyleNames[ElementStyle] := AName;
end;

procedure TElementStyles.ReadFromXML(Node: IXMLDOMNode);

  function IsStylePresent(StyleName: string): boolean;
  begin
    result := true;
    exit;
    try
//      result := Assigned(SheetInterface.ExcelSheet.Application.ActiveWorkbook.Styles[StyleName]);
    except
      result := false;
    end;
  end;

var
  SName: string;
begin
  if not Assigned(Node) then
    exit;
  SName := GetStrAttr(Node, attrValueStyle, '');
  if SName <> '' then
    if IsStylePresent(SName) then
      Name[esValue] := SName;
  SName := GetStrAttr(Node, attrValueStylePrint, '');
  if SName <> '' then
    if IsStylePresent(SName) then
      Name[esValuePrint] := SName;
  SName := GetStrAttr(Node, attrTitleStyle, '');
  if SName <> '' then
    if IsStylePresent(SName) then
      Name[esTitle] := SName;
  SName := GetStrAttr(Node, attrTitleStylePrint, '');
  if SName <> '' then
    if IsStylePresent(SName) then
      Name[esTitlePrint] := SName;
end;

procedure TElementStyles.WriteToXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  with Node as IXMLDOMElement do
  begin
    setAttribute(attrValueStyle, Name[esValue]);
    setAttribute(attrValueStylePrint, Name[esValuePrint]);
    setAttribute(attrTitleStyle, Name[esTitle]);
    setAttribute(attrTitleStylePrint, Name[esTitlePrint]);
  end;
end;

constructor TElementStyles.Create;
begin
    FillChar(FStyleNames, SizeOf(FStyleNames), 0);
end;

procedure TElementStyles.Copy(From: TElementStyles);
begin
  Name[esValue] := From.Name[esValue];
  Name[esValuePrint] := From.Name[esValuePrint];
  Name[esTitle] := From.Name[esTitle];
  Name[esTitlePrint] := From.Name[esTitlePrint];
end;

procedure TSheetCollection.ReadFromXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  Clear;
  Styles.ReadFromXml(Node);
  UseStylesForChildren := GetBoolAttr(Node, 'usestylesforchildren', false);
  FPermitEditing := GetBoolAttr(Node, attrPermitEditing, false);
end;

procedure TSheetCollection.WriteToXML(Node: IXMLDOMNode);
begin
  if not Assigned(Node) then
    exit;
  Styles.WriteToXml(Node);
  (Node as IXMLDOMElement).setAttribute('usestylesforchildren', BoolToStr(UseStylesForChildren));
  (Node as IXMLDOMElement).setAttribute(attrPermitEditing, BoolToStr(FPermitEditing));
end;

procedure TSheetCollection.SetDefaultStyles;
begin
  Styles.Name[esValue] := DefaultStyleName[esValue];
  Styles.Name[esValuePrint] := DefaultStyleName[esValuePrint];
  Styles.Name[esTitle] := DefaultStyleName[esTitle];
  Styles.Name[esTitlePrint] := DefaultStyleName[esTitlePrint];
end;

procedure TSheetAxisElementInterface.SetOwnerStyles;
var
  i: integer;
begin
  inherited SetOwnerStyles;
  Levels.UseStylesForChildren := true;
  Levels.Styles.Copy(Styles);
  for i := 0 to Levels.Count - 1 do
    Levels[i].SetOwnerStyles;
end;

procedure TSheetCollection.SetDefaultStyles2All;
var
  i: integer;
begin
  SetDefaultStyles;
  for i := 0 to Count - 1 do
    Items[i].SetDefaultStyles;
end;

{ TFontOptions }

procedure TFontOptions.CopyFromFont(AFont: Font);
begin
  with AFont do
  begin
//    FSubscript := Subscript;
    FItalic := Italic;
    FBold := Bold;
    FUnderline := Underline;
    FStrikethrough := Strikethrough;
  //  FSuperscript := Superscript;
    FColorIndex := ColorIndex;
    FSize := Size;
    FName := Name;
  end;
end;

procedure TFontOptions.CopyFrom(Another: TFontOptions);
begin
  Name := Another.Name;
  Size := Another.Size;
  Bold := Another.Bold;
  Italic := Another.Italic;
  Underline := Another.Underline;
  Strikethrough := Another.Strikethrough;
  ColorIndex := Another.ColorIndex;
end;

procedure TFontOptions.CopyFromFont(AFont: TFont);
begin
  with AFont do
  begin
    FItalic := fsItalic in Style;
    FBold := fsBold in Style;
    FUnderline := fsUnderline in Style;
    FStrikethrough := fsStrikeout in Style;
    FColorIndex := ColorIndex;
    FSize := Size;
    FName := Name;
  end;
end;

function TFontOptions.IsEqualTo(Another: TFontOptions): boolean;
begin
  result := (Name = Another.Name) and (Size = Another.Size) and
    (Bold = Another.Bold) and (Italic = Another.Italic) and
    (Underline = Another.Underline) and (Strikethrough = Another.Strikethrough);
end;

procedure TFontOptions.ReadFromXml(Node: IXMLDOMNode);
begin
//  Subscript := GetBoolAttr(Node, attrSubscript, false);
  Italic := GetBoolAttr(Node, attrItalic, false);
  Bold := GetBoolAttr(Node, attrBold, false);
  Underline := GetBoolAttr(Node, attrUnderline, false);
  Strikethrough := GetBoolAttr(Node, attrStrikethrough, false);
//  Superscript := GetBoolAttr(Node, attrSuperscript, false);
  ColorIndex := GetIntAttr(Node, attrColorIndex, 0);
  Size := GetIntAttr(Node, attrSize, 10);
  Name := GetStrAttr(Node, attrName, 'Arial');
end;

procedure TFontOptions.WriteToXml(Node: IXMLDOMNode);
begin
  with Node as IXMLDOMElement do
  begin
//    setAttribute(attrSubscript, BoolToStr(Subscript));
    setAttribute(attrItalic, BoolToStr(Italic));
    setAttribute(attrBold, BoolToStr(Bold));
    setAttribute(attrUnderline, BoolToStr(Underline));
    setAttribute(attrStrikethrough, BoolToStr(Strikethrough));
//    setAttribute(attrSuperscript, BoolToStr(Superscript));
    setAttribute(attrColorIndex, ColorIndex);
    setAttribute(attrSize, Size);
    setAttribute(attrName, Name);
  end;
end;

procedure TFontOptions.CopyToFont(AFont: TFont);
var
  FStyles: TFontStyles;
begin
  FStyles := [];
  if Bold then
    Include(FStyles, fsBold);
  if Italic then
    Include(FStyles, fsItalic);
  if Underline then
    Include(FStyles, fsUnderline);
  if Strikethrough then
    Include(FStyles, fsStrikeout);
  AFont.Style := FStyles;
  AFont.Name := Name;
  AFont.Size := Size;
end;

constructor TFontOptions.Create;
begin
  Name := 'Arial';
  Size := 10;
end;

procedure TFontOptions.CopyToFont(AFont: ExcelXP.Font);
begin
  AFont.Name := Name;
  AFont.Size := Size;
  AFont.Bold := Bold;
  AFont.Italic := Italic;
  AFont.Underline := Underline;
  AFont.Strikethrough := Strikethrough;
end;

{ TFormulaParams }

function TFormulaParams.Add(FormulaParam: TFormulaParam): TFormulaParam;
begin
  result := nil;
  if not Assigned(FormulaParam) then
    exit;
  result := GetItem(Inherited Add(FormulaParam));
end;

function TFormulaParams.CreateParam(ParamType: TFormulaParamType;
  Name, ParamValue, Offset: string; IsOtherSheet: boolean): TFormulaParam;
var
  FormulaParam: TFormulaParam;
begin
  FormulaParam := TFormulaParam.Create;
  FormulaParam.Name := Name;
  FormulaParam.ParamType := ParamType;
  FormulaParam.ParamValue := ParamValue;
  FormulaParam.Offset := Offset;
  FormulaParam.IsOtherSheet := IsOtherSheet;
  Add(FormulaParam);
  result := FormulaParam;
end;

procedure TFormulaParams.Delete(Index: integer);
var
  tParam: TFormulaParam;
begin
  tParam := GetItem(index);
  FreeAndNil(tParam);
  Inherited Delete(index);
end;

destructor TFormulaParams.Destroy;
var
  tParam: TFormulaParam;
  i: integer;
begin
  for i := 0 to Count - 1 do
  begin
    tParam := GetItem(i);
    FreeAndNil(tParam);
  end;
  inherited;
end;

function TFormulaParams.GetItem(Index: integer): TFormulaParam;
begin
  result := Get(Index);
end;

procedure TFormulaParams.SetItem(Index: integer; Value: TFormulaParam);
begin
  if not Assigned(value) then
    exit;
  Put(Index, Value);
end;

{ TTypeFormula }

function GetFormulaParam(ParamNode: IXMLDOMNode): TFormulaParam;
var
  CoordNode: IXMLDOMNode;
  i: integer;
  AName, AValue: string;
begin
  result := nil;
  if not Assigned(ParamNode) then
    exit;
  result := TFormulaParam.Create;

  with result do
  begin
    Name := ParamNode.nodeName;
    ParamType := TFormulaParamType(GetIntAttr(ParamNode, attrParamType, 0));
    ParamValue := GetStrAttr(ParamNode, attrParamValue, '');
    Offset := GetStrAttr(ParamNode, attrOffset, '');
    IsOtherSheet := GetBoolAttr(ParamNode, attrIsOtherSheet, false);
    Section := GetIntAttr(ParamNode, attrSection, 0);
    Coords.Clear;
    CoordNode := ParamNode.selectSingleNode('coord');
    if Assigned(CoordNode) then
      for i := 0 to CoordNode.attributes.length - 1 do
      begin
        AName := CoordNode.attributes[i].nodeName;
        AValue := CoordNode.attributes[i].text;
        Coords.Add(AName + '=' + AValue);
      end;
  end;
end;

constructor TTypeFormula.Create(AOwner: TSheetBasicTotal);
begin
  Inherited Create;
  FOwner := AOwner;
  FFormulaParams := TFormulaParams.Create;
end;

destructor TTypeFormula.Destroy;
begin
  FreeAndNil(FFormulaParams);
  inherited;
end;

function TTypeFormula.GetParamCaption(ParamIndex: integer): string;
var
  Total: TSheetBasicTotal;
  GrandSummaryTitle, AxisId, MPNumber, MPName, TotalColumnName: string;
  Param: TFormulaParam;
  AxisIndex, TotalColumnIndex: integer;
  AxisCollection: TSheetAxisCollectionInterface;
  AxisElement: TSheetAxisElementInterface;
  MP: TSheetMPElementInterface;
begin
  result := '';
  Param := FFormulaParams[ParamIndex];

  case Param.ParamType of
    fptTotal, fptTotalAbsolute:
      begin
        Total := FOwner.SheetInterface.Totals.FindByAlias(Param.ParamValue);
        if Assigned(Total) then
        begin
          TotalColumnIndex := TSheetTotalInterface(Total).GetTotalColumn(Param.Section);
          TotalColumnName := GetColumnName(TotalColumnIndex);
          result := Format('[%s(%s)]', [Total.GetElementCaption, TotalColumnName]);
        end
        else
        begin
          result := fmIncorrectRefRus;
          exit;
        end;
      end;
    fptSingleCell:
      begin
        Total := FOwner.SheetInterface.SingleCells.FindByAlias(Param.ParamValue);
        if Assigned(Total) then
        begin
            result := '[' + Total.GetElementCaption + ']';
            case Total.TotalType of
              wtMeasure: result := result + ('(��������� ����)');
              wtResult: result := result + ('(��������� ���������)');
              wtConst: result := result + ('(��������� ���������)');
            end;
        end
        else
        begin
          result := fmIncorrectRefRus;
          exit;
        end;
      end;
    fptRowMP, fptColumnMP:
      begin
        MPNumber := Param.ParamValue;
        CutPart(MPNumber, snNamePrefix + snSeparator);
        MPName := CutPart(MPNumber, snSeparator);
        AxisId := CutPart(MPNumber, snSeparator);
        {�� ����� ������� ������� ��������� ���������}
        if Param.ParamType = fptRowMP then
          AxisCollection := FOwner.SheetInterface.Rows
        else
          AxisCollection := FOwner.SheetInterface.Columns;
        {���������}
        AxisIndex := AxisCollection.FindById(AxisId);
        if AxisIndex <> -1 then
          AxisElement := AxisCollection[AxisIndex]
        else
        begin
          result := fmIncorrectRefRus;
          exit;
        end;
        {� ���������� ��}
        try
          MP := AxisElement.MemberProperties[StrToInt(MPNumber)];
          result := Format('[%s]."%s"', [AxisElement.GetElementCaption, MP.Name]);
        except
          result := fmIncorrectRefRus;
        end;
      end;
    fptFreeCell: result := Param.ParamValue;
  end;

  GrandSummaryTitle := FOwner.SheetInterface.Rows.GrandSummaryOptions.Title;
  if Pos(gsRow, Param.Offset) > 0 then
    result := result + '(' + GrandSummaryTitle + ')';
end;

function TTypeFormula.GetUserFormula: string;
var
  ParamCaption: string;
  i: integer;
begin
  result := '';
  if not Assigned(FOwner) then
    exit;
  result := FTemplate;

  for i := 0 to FFormulaParams.Count - 1  do
  begin
    ParamCaption := GetParamCaption(i);
    result := StringReplace(result, FFormulaParams[i].Name, ParamCaption, [rfReplaceAll])
  end;

  result := StringReplace(result, ',', ';', [rfReplaceAll]);
  result := EnToRus(result);
end;

procedure TTypeFormula.SetUserFormula(UserFormula: string);
begin

end;

procedure TTypeFormula.ReadFromXML(Node: IXMLDOMNode);
var
  i: integer;
  Param: TFormulaParam;
begin
  if not Assigned(Node) then
    exit;

  FTemplate := GetStrAttr(Node, attrTemplate, '');
  FEnabled := GetBoolAttr(Node, attrIsEnabled, false);
  for i := 0 to Node.childNodes.length - 1 do
  begin
    Param := GetFormulaParam(Node.childNodes[i]);
    FFormulaParams.Add(Param);
  end;
end;

procedure AddParam(Node: IXMLDOMNode; Param: TFormulaParam);
var
  tmpNode, CoordNode: IXMLDOMNode;
  i: integer;
  AttrName, AttrValue: string;
begin
  if not(Assigned(Node) and Assigned(Param)) then
    exit;
  tmpNode := Node.ownerDocument.CreateNode(1, Param.Name,'');
  with (tmpNode as IXMLDOMElement) do
  begin
    setAttribute(attrParamType, Param.ParamType);
    setAttribute(attrParamValue, Param.ParamValue);
    setAttribute(attrOffset, Param.Offset);
    setAttribute(attrIsOtherSheet, BoolToStr(Param.IsOtherSheet));
    setAttribute(attrSection, Param.Section);
  end;

  CoordNode := Node.ownerDocument.CreateNode(1, 'coord', '');
  for i := 0 to Param.Coords.Count - 1 do
  begin
    AttrName := Param.Coords.Names[i];
    AttrValue := Param.Coords.Values[AttrName];
    SetAttr(CoordNode, attrName, AttrValue);
  end;
  tmpNode.appendChild(CoordNode);

  Node.appendChild(tmpNode);
end;

procedure TTypeFormula.WriteToXML(Node: IXMLDOMNode);
var
  i: integer;
begin
  if not Assigned(Node) then
    exit;
  with (Node as IXMLDOMElement) do
  begin
    setAttribute(attrTemplate, FTemplate);
    setAttribute(attrIsEnabled, BoolToStr(FEnabled));
    for i := 0 to FFormulaParams.Count - 1 do
      AddParam(Node, FFormulaParams[i]);
  end;
end;

procedure TTypeFormula.Clear;
begin
  FTemplate := '';
  FEnabled := false;
  while (FFormulaParams.Count > 0) do
    FFormulaParams.Delete(0);
end;

function GetErrorText(Template: string): string;
begin
  result := '';
  if Pos('#REF', Template) > 0 then
  begin
    result := '��� �������� ������������ ������.';
    exit;
  end;

  if Pos('!', Template) > 0 then
  begin
    result := '� ��� ���������� ������ �� ������ ����.';
    exit;
  end;
end;

function TTypeFormula.IsValid(out ErrorText: string): boolean;
var
  Totals: TSheetTotalCollectionInterface;
  SingleCells: TSheetSingleCellCollectionInterface;
  tTotal: TSheetBasicTotal;
  FormulaParam: TFormulaParam;
  i: integer;
begin
  result := false;
  ErrorText := '';
  if not(Assigned(FOwner) and (FTemplate <> '')) then
    exit;

  ErrorText := GetErrorText(FTemplate);
  if ErrorText <> '' then
    exit;

  Totals := FOwner.SheetInterface.Totals;
  SingleCells := FOwner.SheetInterface.SingleCells;
  TTotal := nil;
  for i := 0 to FFormulaParams.Count - 1  do
  begin
    FormulaParam := FFormulaParams[i];
    case FormulaParam.ParamType of
      fptTotal, fptTotalAbsolute:
        tTotal := Totals.FindByAlias(FormulaParam.ParamValue);
      fptSingleCell:
        tTotal := SingleCells.FindByAlias(FormulaParam.ParamValue);
      fptRowMP, fptColumnMP, fptFreeCell:
        continue;
    end;
    if not Assigned(tTotal) then
    begin
      ErrorText := '���� �� �� ��������� ��������� �� �� ����������.';
      exit;
    end;
    if tTotal.Alias = FOwner.Alias then
    begin
      ErrorText := '� ��� ���������� ������ �� ����������, � �������� ��� �����������.';
      exit;
    end;
  end;
  result := true;
end;

function TTypeFormula.ContainAlias(Alias: string): boolean;
var
  i: integer;
  FormulaParam: TFormulaParam;
begin
  result := false;
  for i := 0 to FFormulaParams.Count - 1 do
  begin
    FormulaParam := FFormulaParams[i];
    if FormulaParam.ParamValue = Alias then
    begin
      result := true;
      exit;
    end;
  end;
end;

function TTypeFormula.IsEqual(SampleFormula: TTypeFormula;
  IgnoreOffset: boolean): boolean;
var
  i: integer;
  Param, SampleParam: TFormulaParam;
begin
  result := false;
  if not Assigned(SampleFormula) then
    exit;
  if (Template = '') or (SampleFormula.Template = '') then
    exit;
  if (Template <> SampleFormula.Template) then
    exit;
  if (FormulaParams.Count <> SampleFormula.FormulaParams.Count) then
    exit;
  for i := 0 to FormulaParams.Count - 1 do
  begin
    Param := FormulaParams[i];
    SampleParam :=  SampleFormula.FormulaParams[i];
    if (Param.Name <> SampleParam.Name) then
      exit;
    if (Param.ParamValue <> SampleParam.ParamValue) then
      exit;
    if not IgnoreOffset then
      if (Param.Offset <> SampleParam.Offset) and (SampleParam.Offset <> '') then
        exit;
  end;
  result := true;
end;

{ TWritablesInfo }

procedure TWritablesInfo.Add(Total: TSheetTotalInterface; ColumnIndex: byte);
begin
  Include(FWritableColumns , Lo(ColumnIndex));
  if Total.PermitEditing then
    Include(FEditableColumns , Lo(ColumnIndex));
end;

procedure TWritablesInfo.Add(Cell: TSheetSingleCellInterface);
begin
  if FSingleCellsNames.IndexOf(Cell.ExcelName) < 0 then
  begin
    FSingleCellsNames.Add(Cell.ExcelName);
    if Cell.PermitEditing then
      FEditableSingleCells.Add(Cell.ExcelName)
  end;
end;

function TWritablesInfo.CheckForWritableCell(ESheet: ExcelWorkSheet;
  Target: ExcelRange; out Editable: boolean): boolean;
var
  IsWritableCell: boolean;
begin
  result := false;
  if IsSingleCellSelected(ESheet, Target, IsWritableCell, Editable) then
    result := IsWritableCell;
end;

function TWritablesInfo.CheckForWritableColumn(ESheet: ExcelWorkSheet;
  Target: ExcelRange; var Editable: boolean): boolean;
var
  ERange: ExcelRange;
  StartColumn, EndColumn, ColumnIndex: integer;
begin
  result := false;
  ERange := GetRangeByName(ESheet, BuildExcelName(sntTotals));
  if not Assigned(ERange) then
    exit;
  { ����������� �������� ������ ��������� ������������ � ������� �����������}
  if not IsNestedRanges(Target, ERange) then
    exit;
  { ���� �� �������� ����� ������ �������, �� ��� ��� ��� ���������� ������
    ���� �������� ��� ������.}
  result := true;
  StartColumn := Target.Column - ERange.Column;
  EndColumn := StartColumn + Target.Columns.Count - 1;
  for ColumnIndex := StartColumn to EndColumn do
  begin
    result := result and (ColumnIndex in FWritableColumns);
    Editable := Editable and (ColumnIndex in FEditableColumns);
  end;
end;

function TWritablesInfo.CheckForWritableRange(ESheet: ExcelWorkSheet;
  Target: ExcelRange; out Editable: boolean): boolean;
var
  IsWritableCell: boolean;
begin
  Editable := true;
  if IsSingleCellSelected(ESheet, Target, IsWritableCell, Editable) then
    result := IsWritableCell
  else
    result := CheckForWritableColumn(ESheet, Target, Editable);
end;

procedure TWritablesInfo.Clear;
begin
  ClearColumns;
  ClearCells;
  ClearCellsPoints;
end;

procedure TWritablesInfo.ClearCells;
begin
  FSingleCellsNames.Clear;
  FEditableSingleCells.Clear;
end;

procedure TWritablesInfo.ClearCellsPoints;
begin
  SetLength(FSingleCellsPoints, 0);
end;

procedure TWritablesInfo.ClearColumns;
begin
  FWritableColumns := [];
  FEditableColumns := [];
end;

procedure TWritablesInfo.CopyFrom(Another: TWritablesInfo);
begin
  FWritableColumns := Another.FWritableColumns;
  FEditableColumns := Another.FEditableColumns;
  FSingleCellsNames.Assign(Another.FSingleCellsNames);
  FEditableSingleCells.Assign(Another.FEditableSingleCells);
end;

constructor TWritablesInfo.Create;
begin
  FSingleCellsNames := TStringList.Create;
  FEditableSingleCells := TStringList.Create;
end;

procedure TWritablesInfo.Delete(CellName: string);
var
  NameIndex: integer;
begin
  NameIndex := FSingleCellsNames.IndexOf(CellName);
  if NameIndex > -1 then
    FSingleCellsNames.Delete(NameIndex);

  NameIndex := FEditableSingleCells.IndexOf(CellName);
  if NameIndex > -1 then
    FEditableSingleCells.Delete(NameIndex);
end;

destructor TWritablesInfo.Destroy;
begin
  inherited;
  FreeStringList(FSingleCellsNames);
  FreeStringList(FEditableSingleCells);
  SetLength(FSingleCellsPoints, 0);
end;

function TWritablesInfo.IsColumnWritable(ColumnIndex: integer): boolean;
begin
  result := Lo(ColumnIndex) in FWritableColumns;
end;

function TWritablesInfo.IsSingleCellSelected(ESheet: ExcelWorkSheet;
  Row, Column: integer): boolean;
var
  i: integer;
begin
  result := false;
  if Length(FSingleCellsPoints) < 1 then
    exit;
  for i := 0 to Length(FSingleCellsPoints) - 1 do
    if (FSingleCellsPoints[i].x = Row) and (FSingleCellsPoints[i].y = Column) then
    begin
      result := true;
      exit;
    end;
end;

function TWritablesInfo.IsSingleCellSelected(ESheet: ExcelWorkSheet;
  Row, Column: integer; out Writable, Editable: boolean): boolean;
var
  i: integer;
  ERange: ExcelRange;
begin
  result := false;
  if (Row < 1) or (Column < 1) then
    exit;
  for i := 0 to FSingleCellsNames.Count - 1 do
  begin
    ERange := GetRangeByName(ESheet, FSingleCellsNames[i]);
    if not Assigned(ERange) then
      continue;
    if (ERange.Row = Row) and (ERange.Column = Column) then
    begin
      result := true;
      Writable := Pos(sntSingleCellResult, FSingleCellsNames[i]) > 0;
      Editable := FEditableSingleCells.IndexOf(FSingleCellsNames[i]) > -1;
      exit;
    end;
  end;
end;

function TWritablesInfo.IsSingleCellSelected(ESheet: ExcelWorkSheet;
  Target: ExcelRange; out Writable, Editable: boolean): boolean;
begin
  result := false;
  Writable := false;
  if not Assigned(Target) then
    exit;
  if (Target.Columns.Count <> 1) or (Target.Rows.Count <> 1) then
    exit;
  result := IsSingleCellSelected(ESheet, Target.Row, Target.Column,
    Writable, Editable);
end;

function TWritablesInfo.IsSingleCellSelected(ESheet: ExcelWorkSheet;
  Row, Column: integer; out CellId: string): boolean;
var
  i: integer;
  ERange: ExcelRange;
begin
  result := false;
  if (Row < 1) or (Column < 1) then
    exit;
  for i := 0 to FSingleCellsNames.Count - 1 do
  begin
    ERange := GetRangeByName(ESheet, FSingleCellsNames[i]);
    if not Assigned(ERange) then
      continue;
    if (ERange.Row = Row) and (ERange.Column = Column) then
    begin
      result := true;
      CellId := FSingleCellsNames[i];
      CutPart(CellId, snSeparator);
      CutPart(CellId, snSeparator);
      exit;
    end;
  end;
end;

function TWritablesInfo.IsSingleCellSelected(ESheet: ExcelWorkSheet;
  Target: ExcelRange; out CellId: string): boolean;
begin
  result := false;
  CellId := '';
  if not Assigned(Target) then
    exit;
  if (Target.Columns.Count <> 1) or (Target.Rows.Count <> 1) then
    exit;
  result := IsSingleCellSelected(ESheet, Target.Row, Target.Column, CellId);
end;

function TWritablesInfo.NoWritableColumns: boolean;
begin
  result := FWritableColumns = [];
end;

procedure TWritablesInfo.ReadFromXml(Node: IXMLDOMNode);
begin
  FWritableColumns := StringToByteSet(GetStrAttr(Node, 'writablecolumns', ''));
  FEditableColumns := StringToByteSet(GetStrAttr(Node, 'editablecolumns', ''));
  FSingleCellsNames.CommaText := GetStrAttr(Node, 'singlecellsnames', '');
  FEditableSingleCells.CommaText := GetStrAttr(Node, 'editablesinglecells', '');
end;

procedure TWritablesInfo.UpdateSingleCellsPoints(ExcelSheet: ExcelWorksheet);
var
  i: integer;
  tRange: ExcelRange;
begin
  if not Assigned(ExcelSheet) then
    exit;
  SetLength(FSingleCellsPoints, FSingleCellsNames.Count);
  for i := 0 to FSingleCellsNames.Count - 1 do
  begin
    tRange := GetRangeByName(ExcelSheet, FSingleCellsNames.Strings[i]);
    if not Assigned(tRange) then
      continue;
    FSingleCellsPoints[i] := Classes.Point(tRange.Row, tRange.Column);
  end;
end;

procedure TWritablesInfo.WriteToXml(Node: IXMLDOMNode);
begin
  with Node as IXMLDOMElement do
  begin
    setAttribute('writablecolumns', ByteSetToString(FWritableColumns));
    setAttribute('editablecolumns', ByteSetToString(FEditableColumns));
    setAttribute('singlecellsnames', FSingleCellsNames.CommaText);
    setAttribute('editablesinglecells', FEditableSingleCells.CommaText);
  end;
end;

{��������� � ����?}
function CheckDimension(Cube: TCube; DimElem: TSheetDimension): boolean;
begin
  result := false;
  if not Assigned(Cube) then
    exit;
  if Cube.ProviderId <> DimElem.ProviderId then
    exit;
  result := Cube.DimAndHierInCube(DimElem.Dimension, DimElem.Hierarchy);
end;

function TSheetCollection.GetLastItem: TSheetElement;
begin
  result := TSheetElement(Last);
end;

{ TSheetFilterCollectionInterface }

function TSheetFilterCollectionInterface.GetLastItem: TSheetFilterInterface;
begin
  result := TSheetFilterInterface(Last);
end;

function TSheetLevelCollectionInterface.GetLastItem: TSheetLevelInterface;
begin
  result := TSheetLevelInterface(Last);
end;

function TSheetMPCollectionInterface.GetLastItem: TSheetMPElementInterface;
begin
  result := TSheetMPElementInterface(Last);
end;

{ TSheetSingleCellCollectionInterface }

constructor TSheetSingleCellCollectionInterface.Create(
  AOwner: TSheetInterface);
begin
  inherited Create(AOwner);
  FTotalCounters := TTotalCounters.Create;
end;

function TSheetSingleCellCollectionInterface.GetLastItem: TSheetSingleCellInterface;
begin
  result := TSheetSingleCellInterface(Last);
end;

function TSheetSingleCellCollectionInterface.GetTotalCounterValue(
  TotalType: TSheetTotalType): string;
begin
  result := FTotalCounters.GetCounterValue(TotalType);
end;

{ TSheetTotalCollectionInterface }

constructor TSheetTotalCollectionInterface.Create(AOwner: TSheetInterface);
begin
  inherited Create(AOwner);
  FTotalCounters := TTotalCounters.Create;
end;

function TSheetTotalCollectionInterface.GetLastItem: TSheetTotalInterface;
begin
  result := TSheetTotalInterface(Last);
end;

procedure TSheetElement.SetPermitEditing(Value: boolean);
begin
  FPermitEditing := Value;
end;

procedure TSheetCollection.SetPermitEditing(Value: boolean);
begin
  FPermitEditing := Value;
end;

{ ��������� �� ����������� �� �������������� � "������ ������ � �������".

  ���������� ����� ���� ���������� �� ���� ������� - ��������, ���������
  � ����� � �����. ����������� ��� ������ ���������� �� �������� ���������
  ��� �����������������.

  ����������/������ ��� �������� ��������� �� ��� �������� � �����
  �������� (��� ���������).

  ����������/������ ��� ��������� ��������� �� �� �������� � �����������
  ����������/�������� ���������, �� �� ������ �� �������������� ������ ���������.

  ����������/������ ��� ����� � ����� �������� �������������� ����������� (���
  �������������) ������������� ��������, �.�. �������� ��� ������ ����
  "���������� �� ������ ����������".}

procedure TSheetInterface.SetPermitEditing(Value: boolean);
begin
  FPermitEditing := Value;
end;

function TSheetElement.MayBeEdited: boolean;
begin
  result := (SheetInterface.InConstructionMode or
    (PermitEditing and not SheetInterface.InConstructionMode))
    and SheetInterface.MayBeEdited;
end;

function TSheetCollection.MayBeEdited: boolean;
begin
  result := (Owner.InConstructionMode or
    (PermitEditing and not Owner.InConstructionMode)) and Owner.MayBeEdited;
end;

function TSheetInterface.MayBeEdited: boolean;
begin
  result := InConstructionMode or (PermitEditing and not InConstructionMode);
end;

procedure TSheetInterface.SetDefaultPermissions;

  procedure ProcessCollection(Collection: TSheetCollection);
  var
    i: integer;
  begin
    Collection.PermitEditing := false;
    for i := 0 to Collection.Count - 1 do
      Collection.Items[i].PermitEditing := false;
  end;

begin
  FPermitEditing := false;
  FInConstructionMode := true;
  ProcessCollection(Rows);
  ProcessCollection(Columns);
  ProcessCollection(Filters);
  ProcessCollection(Totals);
  ProcessCollection(SingleCells);
end;

procedure TSheetInterface.SwitchOffline;
var
  i: integer;
  DefaultNode: IXMLDOMNode;
  Filter: TSheetFilterInterface;
  XPath: string;
begin
  FOnline := false;
  FInConstructionMode := false;

  { �� �������� ��������� ���� ������� ��� ��������, ����� ����������� ���
    �������� ������ LocalId � PkId. ����� � ����������� ���������.}
  for i := 0 to Rows.Count - 1 do
  begin
    if Rows[i].IsParam then
      Rows[i].Param.RemoveLink(Rows[i]);
    Rows[i].EnlightMembersDom(Rows.HideEmpty);
  end;

  for i := 0 to Columns.Count - 1 do
  begin
    if Columns[i].IsParam then
      Columns[i].Param.RemoveLink(Columns[i]);
    Columns[i].EnlightMembersDom(false);
  end;

  { �� ����� ������� � "�������-��������" �� ����������� ���� �������� ���
    ��������� ���������, ������� ����� �������� � ���������� ������.
    � �� �� �����, �������� "�������" ������, ����������� ��������� ���
    ���������� � ������ ���� �������� ����� ��������� � ������� � ������������
    � ���, ��� ������������ � �����. ��� ����� ������ ����� � ���� ���������,
    ����� ����������. ������� ��������� ����� ������� - ������ � ��� ��� ���.
    #16161 - ��� �� ������ �������� ������� ������������ �������� (
    ���������� ����� ���� ���������� ��������)}
  for i := 0 to Filters.Count - 1 do
  begin
    Filter := Filters[i];
    if Filter.IsParam then
      Filter.Param.RemoveLink(Filter);
    if not Filter.MayBeEdited then
      continue;
    if Assigned(Filter.Members) then
    begin
      XPath := 'function_result/Members//Member[@influence=3]';
      RemoveNodes(Filter.Members, XPath);

      DefaultNode := Filter.Members.selectSingleNode(
        Format('function_result/Members//Member[@%s="true"]', [attrDefaultValue]));
      if Assigned(DefaultNode) then
      begin
        {��������� ������ �� ��������� ����� ���������, ������� �� �������� ����}
        FillNodeListAttribute(Filter.Members,
          Format('function_result/Members//Member[@%s="false"]', [attrChecked]),
          attrForbidCheck, 'true');
        FillNodeListAttribute(Filter.Members,
          Format('function_result/Members//Member[@%s="true"]', [attrChecked]),
          attrChecked, 'false');
        SetAttr(DefaultNode, attrChecked, true);
        DefaultNode.attributes.removeNamedItem(attrDefaultValue);
      end;
    end;
  end;
end;

function TSheetCollection.ElementsMayBeEdited: boolean;
var
  i: integer;
begin
  result := false;
  for i := 0 to Count - 1 do
    if Items[i].MayBeEdited then
    begin
      result := true;
      exit;
    end;
end;

procedure TSheetElement.ValidateStyles;
var
  i: TElementStyle;
  Style: ExcelXP.Style;
begin
  for i := Low(TElementStyle) to High(TElementStyle) do
  begin
    Style := nil;
    try
      Style := (SheetInterface.ExcelSheet.Parent as ExcelWorkBook).Styles.Item[Styles[i]];
    except
      Style := nil;
    end;
    if not Assigned(Style) then
      Styles[esValue] := DefaultStyleName[i];
  end;
end;

procedure TSheetCollection.ValidateStyles;
var
  i: integer;
begin
  for i := 0 to Count - 1 do
    Items[i].ValidateStyles;
end;

procedure TSheetInterface.ValidateStyles;
begin
  Rows.ValidateStyles;
  Columns.ValidateStyles;
  Filters.ValidateStyles;
  Totals.ValidateStyles;
  SingleCells.ValidateStyles;
end;

function TSheetTotalCollectionInterface.GetTotalCounterValue(
  TotalType: TSheetTotalType): string;
begin
  result := FTotalCounters.GetCounterValue(TotalType);
end;

{ TTotalCounters }

constructor TTotalCounters.Create;
var
  i: TSheetTotalType;
begin
  {��� ����� �����������}
  for i := Low(TSheetTotalType) to High(TSheetTotalType) do
    FCounters[i] := 0;
end;

function TTotalCounters.GetCounterValue(
  TotalType: TSheetTotalType): string;
begin
  inc(FCounters[TotalType]);
  result := IntToStr(FCounters[TotalType]);
end;

procedure TTotalCounters.ReadFromXml(Node: IXMLDOMNode);
begin
  Node := Node.selectSingleNode('counters');
  FCounters[wtFree] := GetIntAttr(Node, 'free', 0);
  FCounters[wtMeasure] := GetIntAttr(Node, 'measure', 0);
  FCounters[wtResult] := GetIntAttr(Node, 'result', 0);
  FCounters[wtConst] := GetIntAttr(Node, 'const', 0);
end;

procedure TTotalCounters.WriteToXml(Node: IXMLDOMNode);
var
  NewNode: IXMLDOMNode;
begin
  NewNode := Node.ownerDocument.createNode(1, 'counters', '');
  Node.appendChild(NewNode);
  SetAttr(NewNode, 'free', FCounters[wtFree]);
  SetAttr(NewNode, 'measure', FCounters[wtMeasure]);
  SetAttr(NewNode, 'result', FCounters[wtResult]);
  SetAttr(NewNode, 'const', FCounters[wtConst]);
end;

function TSheetInterface.GetTypeFormula(Total: TSheetTotalInterface; Row,
  Column: integer): TTypeFormula;
begin
//����������� �������� - ��� ��� Delphi Internal Error URW533
  result := nil;
end;

procedure TSheetInterface.MapTypeFormula(Total: TSheetTotalInterface);
begin
//����������� �������� - ��� ��� Delphi Internal Error URW533
  ;
end;

function TSheetElement.GetOnDeleteWarning: string;
begin
  result := '������� �������?';
end;

function TSheetElement.MayBeDeleted: boolean;
begin
  result := ShowQuestion(GetOnDeleteWarning);
end;

function TSheetElement.OfPrimaryProvider: boolean;
begin
  if ProviderId <> '' then
    result := ProviderId = SheetInterface.XMLCatalog.PrimaryProvider
  else
    result := true;
end;

function TParamBaseCollectionInterface.GetUniqueID: integer;
begin
  inc(FCounter);
  result := FCounter;
end;


function TSheetInterface.CollectOneTotalData(Total: TSheetTotalInterface): boolean;
begin
  //����������� �������� - ��� ��� Delphi Internal Error URW533
  result := false;
end;

{ TTaskParamBase }

procedure TTaskParamBase.ReadFromXml(Node: IXMLDOMNode);
var
  str: string;
begin
  str := ReadTaskContextAttr(Node, 'ID', 'Id');
  try
    FId := StrToInt(str);
  except
    FId := -1;
  end;

  FName := ReadTaskContextAttr(Node, 'NAME', 'Name');
  FValues := ReadTaskContextAttr(Node, 'PARAMVALUES', 'ParamValues');
  FComment := ReadTaskContextAttr(Node, 'DESCRIPTION', 'Description');

  str := ReadTaskContextAttr(Node, 'INHERITED', 'Inherited');
  try
    FIsInherited := boolean(StrToInt(str));
  except
    FIsInherited := false;
  end;
end;

{ TTaskEnvironment }

procedure TTaskEnvironment.ClearDynamicProperties(Props: IDispatch);
begin
  DeleteWBCustomProperty(Props, fmtcContextType);
  DeleteWBCustomProperty(Props, fmtcSilentMode);
  DeleteWBCustomProperty(Props, fmtcAuthType);
  DeleteWBCustomProperty(Props, fmtcLogin);
  DeleteWBCustomProperty(Props, fmtcPwdHash);
  DeleteWBCustomProperty(Props, fmtcIsTaskConnect);
  DeleteWBCustomProperty(Props, fmtcAction);
  DeleteWBCustomProperty(Props, fmtcMutexName);
  DeleteWBCustomProperty(Props, fmtcLoadingFromTask);
end;

procedure TTaskEnvironment.ClearStaticProperties(Props: IDispatch);
begin
  SetWBCustomPropertyValue(Props, fmDocumentName, '');
  SetWBCustomPropertyValue(Props, fmDocumentId, '');
  SetWBCustomPropertyValue(Props, fmTaskName, '');
  SetWBCustomPropertyValue(Props, fmTaskId, '');
  SetWBCustomPropertyValue(Props, fmOwner, '');
  SetWBCustomPropertyValue(Props, fmDocPath, '');
  SetWBCustomPropertyValue(Props, fmDocType, '');
  ReadStaticProperties(Props);
end;

procedure TTaskEnvironment.CreateActionMutex;
begin
  if (Action <> datNone) and (MutexName <> '') then
  try
    UpdateMDXLog('mutex name: ' + MutexName, '', '');
    MutexHandle := CreateMutex(nil, true, PChar(MutexName));
    if MutexHandle = 0 then
    begin
      UpdateMDXLog(GetLastErrorLoc, '', '');
      exit;
    end;
    UpdateMDXLog('mutex handle: ' + IntToStr(MutexHandle), '', '');
  except
    on e: exception do
    begin
       UpdateMDXLog(e.Message, '' , '');
    end;
  end;
end;

procedure TTaskEnvironment.ReleaseActionMutex;
begin
  if MutexHandle = 0 then
    exit;
  try
    ReleaseMutex(MutexHandle);
    CloseHandle(MutexHandle);
    UpdateMDXLog('mutex released ok', '', '');
  except
    on e: exception do
    begin
       UpdateMDXLog(e.Message, '' , '');
    end;
  end;
end;

procedure TTaskEnvironment.ReadDynamicProperties(Props: IDispatch);
var
  tmpStr: string;
begin
  tmpStr := GetWBCustomPropertyValue(Props, fmtcLoadingFromTask);
  try
    IsLoadingFromTask := AnsiUpperCase(tmpStr) = 'TRUE';
  except
    IsLoadingFromTask := false;
  end;

  tmpStr := GetWBCustomPropertyValue(Props, fmtcContextType);
  if IsLoadingFromTask then
  begin
    try
      ContextType := boolean(StrToInt(tmpStr));
    except
    end;

    tmpStr := GetWBCustomPropertyValue(Props, fmtcSilentMode);
    if IsNumber(tmpStr) then
      try
        SilentMode := boolean(StrToInt(tmpStr));
      except
        SilentMode := false;
      end
    else
      SilentMode := AnsiUpperCase(tmpStr) = 'TRUE';

    tmpStr := GetWBCustomPropertyValue(Props, fmtcAuthType);
    try
      AuthType := StrToInt(tmpStr);
    except
    end;

    Login := GetWBCustomPropertyValue(Props, fmtcLogin);
    PwdHash := GetWBCustomPropertyValue(Props, fmtcPwdHash);

    tmpStr := GetWBCustomPropertyValue(Props, fmtcIsTaskConnect);
    IsTaskConnect := AnsiUpperCase(tmpStr) = 'TRUE';

    tmpStr := GetWBCustomPropertyValue(Props, fmtcAction);
    try
      Action := StrToInt(tmpStr);
    except
    end;
    MutexName := GetWBCustomPropertyValue(Props, fmtcMutexName);
  end;
end;

procedure TTaskEnvironment.ReadStaticProperties(Props: IDispatch);
begin
  DocumentName := GetWBCustomPropertyValue(Props, fmDocumentName);
  DocumentId := GetWBCustomPropertyValue(Props, fmDocumentId);
  TaskName := GetWBCustomPropertyValue(Props, fmTaskName);
  TaskId := GetWBCustomPropertyValue(Props, fmTaskId);
  Owner := GetWBCustomPropertyValue(Props, fmOwner);
  DocPath := GetWBCustomPropertyValue(Props, fmDocPath);
  DocType := GetWBCustomPropertyValue(Props, fmDocType);

  ConnectionStr := GetWBCustomPropertyValue(Props, fmConnectionStr);
  AlterConnection := GetWBCustomPropertyValue(Props, fmAlterConnection);
  SchemeName := GetWBCustomPropertyValue(Props, fmSchemeName);
end;

procedure TTaskEnvironment.ReadTaskProperties(Book: ExcelWorkbook);
var
  Props: IDispatch;
begin
  Props := Book.CustomDocumentProperties;

  ReadDynamicProperties(Props);
  ClearDynamicProperties(Props);
  ReadStaticProperties(Props);
end;

procedure TTaskEnvironment.SetActionResult(Book: ExcelWorkbook; Success: boolean; Message: string);
var
  Props: IDispatch;
begin
  Props := Book.CustomDocumentProperties;
  SetWBCustomPropertyValue(Props, fmResultSuccess, BoolToStr(Success));
  SetWBCustomPropertyValue(Props, fmResultMessage, Message);
end;

{ TFormulaParam }

constructor TFormulaParam.Create;
begin
  FCoords := TStringList.Create;
end;

destructor TFormulaParam.Destroy;
begin
  FreeStringList(FCoords);
  inherited;
end;

function TSheetInterface.TypeFormulaToString(
  FormulaTotal: TSheetTotalInterface; CurrentRow, CurrentSectionIndex,
  GrandSummaryRow: integer): string;
begin
  //����������� �������� - ��� ��� Delphi Internal Error URW533
  result := '';
end;

procedure TSheetInterface.Save(LoadMode: TLoadMode);
begin
  FLoadMode := LoadMode;
  Save;
end;

function TSheetInterface.GetIsSilentMode: boolean;
begin
  if Assigned(Environment) then
    result := Environment.SilentMode
  else
    result := false;
end;

procedure TSheetInterface.SetIsSilentMode(const Value: boolean);
begin
  if Assigned(Environment) then
    Environment.SilentMode := Value;
end;

end.




