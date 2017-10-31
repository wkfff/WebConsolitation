unit DSO80_TLB;

// ************************************************************************ //
// WARNING                                                                    
// -------                                                                    
// The types declared in this file were generated from data read from a       
// Type Library. If this type library is explicitly or indirectly (via        
// another type library referring to this type library) re-imported, or the   
// 'Refresh' command of the Type Library Editor activated while editing the   
// Type Library, the contents of this file will be regenerated and all        
// manual modifications will be lost.                                         
// ************************************************************************ //

// PASTLWTR : 1.2
// File generated on 30.09.2004 6:58:31 PM from Type Library described below.

// ************************************************************************  //
// Type Lib: C:\Program Files\Common Files\Microsoft Shared\DSO\msmddo80.dll (1)
// LIBID: {B492C3E0-0195-11D2-89BA-00C04FB9898D}
// LCID: 0
// Helpfile: 
// HelpString: Microsoft Decision Support Objects
// DepndLst: 
//   (1) v2.0 stdole, (C:\WINNT\system32\stdole2.tlb)
//   (2) v1.2 POMInterfaces, (C:\Program Files\Common Files\Microsoft Shared\DSO\msmdint.dll)
//   (3) v6.0 VBA, (C:\WINNT\SYSTEM32\MSVBVM60.DLL)
//   (4) v2.1 ADODB, (C:\Program Files\Common Files\System\ado\msado21.tlb)
//   (5) v1.0 MSOLAPADMINLib2, (C:\Program Files\Common Files\SYSTEM\OLE DB\MSMDGD80.DLL)
// Errors:
//   Hint: TypeInfo 'Property' changed to 'Property_'
//   Hint: Symbol 'ClassName' renamed to '_className'
// ************************************************************************ //
{$TYPEDADDRESS OFF} // Unit must be compiled without type-checked pointers. 
{$WRITEABLECONST ON}
interface

uses Windows, ActiveX, ADODB_TLB, MSOLAPADMINLib80_TLB, POMInterfaces80_TLB, VBA_TLB;
  

// *********************************************************************//
// GUIDS declared in the TypeLibrary. Following prefixes are used:        
//   Type Libraries     : LIBID_xxxx                                      
//   CoClasses          : CLASS_xxxx                                      
//   DISPInterfaces     : DIID_xxxx                                       
//   Non-DISP interfaces: IID_xxxx                                        
// *********************************************************************//
const
  // TypeLibrary Major and minor versions
  DSOMajorVersion = 5;
  DSOMinorVersion = 1;

  LIBID_DSO: TGUID = '{B492C3E0-0195-11D2-89BA-00C04FB9898D}';

  IID__PartitionMeasure: TGUID = '{E8AC5839-7127-11D2-8A35-00C04FB9898D}';
  IID__ICommon: TGUID = '{E8AC5827-7127-11D2-8A35-00C04FB9898D}';
  IID__PartitionLevel: TGUID = '{E8AC5832-7127-11D2-8A35-00C04FB9898D}';
  IID__Level: TGUID = '{E8AC581C-7127-11D2-8A35-00C04FB9898D}';
  IID__PartitionDimension: TGUID = '{E8AC5833-7127-11D2-8A35-00C04FB9898D}';
  IID__Dimension: TGUID = '{E8AC5819-7127-11D2-8A35-00C04FB9898D}';
  IID__Command: TGUID = '{E8AC5811-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Command: TGUID = '{B492C3CA-0195-11D2-89BA-00C04FB9898D}';
  CLASS_PartitionLevel: TGUID = '{B492C3C6-0195-11D2-89BA-00C04FB9898D}';
  CLASS_Level: TGUID = '{B492C3BE-0195-11D2-89BA-00C04FB9898D}';
  IID__Measure: TGUID = '{E8AC5808-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Measure: TGUID = '{B492C39E-0195-11D2-89BA-00C04FB9898D}';
  IID__Role: TGUID = '{E8AC580F-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Role: TGUID = '{B492C3D1-0195-11D2-89BA-00C04FB9898D}';
  CLASS_PartitionMeasure: TGUID = '{B492C3AF-0195-11D2-89BA-00C04FB9898D}';
  CLASS_ICommon: TGUID = '{6D9E0B3D-6D07-11D2-88E2-00104BCC5A9C}';
  IID__Properties: TGUID = '{E8AC57FF-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Properties: TGUID = '{6D9E0B0F-6D07-11D2-88E2-00104BCC5A9C}';
  IID__Property: TGUID = '{E8AC57FE-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Property_: TGUID = '{6D9E0B0D-6D07-11D2-88E2-00104BCC5A9C}';
  IID__CubeAnalyzer: TGUID = '{E8AC5810-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeAnalyzer: TGUID = '{B492C39C-0195-11D2-89BA-00C04FB9898D}';
  IID__PartitionAnalyzer: TGUID = '{E8AC5814-7127-11D2-8A35-00C04FB9898D}';
  CLASS_PartitionAnalyzer: TGUID = '{B492C3A7-0195-11D2-89BA-00C04FB9898D}';
  CLASS_PartitionDimension: TGUID = '{B492C3BA-0195-11D2-89BA-00C04FB9898D}';
  CLASS_Dimension: TGUID = '{B492C3B3-0195-11D2-89BA-00C04FB9898D}';
  IID__IROLAPProvider: TGUID = '{E8AC5815-7127-11D2-8A35-00C04FB9898D}';
  DIID___IROLAPProvider: TGUID = '{E8AC5817-7127-11D2-8A35-00C04FB9898D}';
  IID__IDatabaseEvents: TGUID = '{E8AC580D-7127-11D2-8A35-00C04FB9898D}';
  CLASS_IDatabaseEvents: TGUID = '{6D9E0B1E-6D07-11D2-88E2-00104BCC5A9C}';
  IID__Server: TGUID = '{E8AC5822-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Server: TGUID = '{B492C386-0195-11D2-89BA-00C04FB9898D}';
  IID__MDStore: TGUID = '{E8AC5800-7127-11D2-8A35-00C04FB9898D}';
  CLASS_MDStore: TGUID = '{B492C38C-0195-11D2-89BA-00C04FB9898D}';
  IID__OlapCollection: TGUID = '{E8AC580A-7127-11D2-8A35-00C04FB9898D}';
  CLASS_OlapCollection: TGUID = '{B492C383-0195-11D2-89BA-00C04FB9898D}';
  IID__Partition: TGUID = '{E8AC5838-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Partition: TGUID = '{B492C3A9-0195-11D2-89BA-00C04FB9898D}';
  IID__Aggregation: TGUID = '{E8AC5837-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Aggregation: TGUID = '{B492C3AB-0195-11D2-89BA-00C04FB9898D}';
  IID__AggregationDimension: TGUID = '{E8AC5835-7127-11D2-8A35-00C04FB9898D}';
  CLASS_AggregationDimension: TGUID = '{B492C3BC-0195-11D2-89BA-00C04FB9898D}';
  IID__AggregationLevel: TGUID = '{E8AC5834-7127-11D2-8A35-00C04FB9898D}';
  CLASS_AggregationLevel: TGUID = '{B492C3C8-0195-11D2-89BA-00C04FB9898D}';
  IID__AggregationMeasure: TGUID = '{E8AC5836-7127-11D2-8A35-00C04FB9898D}';
  CLASS_AggregationMeasure: TGUID = '{B492C3AD-0195-11D2-89BA-00C04FB9898D}';
  IID__Cube: TGUID = '{E8AC5831-7127-11D2-8A35-00C04FB9898D}';
  CLASS_Cube: TGUID = '{B492C3A3-0195-11D2-89BA-00C04FB9898D}';
  IID__CubeCommand: TGUID = '{E8AC582F-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeCommand: TGUID = '{B492C3CF-0195-11D2-89BA-00C04FB9898D}';
  IID__CubeDimension: TGUID = '{E8AC582C-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeDimension: TGUID = '{B492C3B8-0195-11D2-89BA-00C04FB9898D}';
  IID__CubeGroup: TGUID = '{E8AC582E-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeGroup: TGUID = '{B492C3D3-0195-11D2-89BA-00C04FB9898D}';
  IID__CubeLevel: TGUID = '{E8AC582B-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeLevel: TGUID = '{B492C3C4-0195-11D2-89BA-00C04FB9898D}';
  IID__CubeMeasure: TGUID = '{E8AC5830-7127-11D2-8A35-00C04FB9898D}';
  CLASS_CubeMeasure: TGUID = '{B492C3A1-0195-11D2-89BA-00C04FB9898D}';
  IID__Database: TGUID = '{E8AC5820-7127-11D2-8A35-00C04FB9898D}';
  DIID___Database: TGUID = '{E8AC5821-7127-11D2-8A35-00C04FB9898D}';
  IID__DBCommand: TGUID = '{E8AC5856-7127-11D2-8A35-00C04FB9898D}';
  CLASS_DBCommand: TGUID = '{B492C3CD-0195-11D2-89BA-00C04FB9898D}';
  IID__DbDimension: TGUID = '{E8AC5828-7127-11D2-8A35-00C04FB9898D}';
  CLASS_DbDimension: TGUID = '{B492C3B6-0195-11D2-89BA-00C04FB9898D}';
  IID__DBGroup: TGUID = '{E8AC582D-7127-11D2-8A35-00C04FB9898D}';
  CLASS_DBGroup: TGUID = '{B492C3D5-0195-11D2-89BA-00C04FB9898D}';
  IID__DbLevel: TGUID = '{E8AC582A-7127-11D2-8A35-00C04FB9898D}';
  CLASS_DbLevel: TGUID = '{B492C3C2-0195-11D2-89BA-00C04FB9898D}';
  IID__MemberProperty: TGUID = '{E8AC5829-7127-11D2-8A35-00C04FB9898D}';
  CLASS_MemberProperty: TGUID = '{6D9E0B40-6D07-11D2-88E2-00104BCC5A9C}';
  IID__DataSource: TGUID = '{E8AC5818-7127-11D2-8A35-00C04FB9898D}';
  CLASS_DataSource: TGUID = '{B492C397-0195-11D2-89BA-00C04FB9898D}';
  IID__RoleCommand: TGUID = '{B7B6AAD7-6E4C-4415-A61A-F6FC01A16E68}';
  CLASS_RoleCommand: TGUID = '{19166132-FD21-40A1-89A0-38CCB25B78CB}';
  IID__Column: TGUID = '{932B8CFB-DAD2-4CEA-A963-82B057760865}';
  CLASS_Column: TGUID = '{442F2D8E-2669-4AEA-B45A-675C7DBF5C82}';
  IID__MiningModel: TGUID = '{AC8F7BBF-9B71-4A7E-B18F-1088884A9EDA}';
  CLASS_MiningModel: TGUID = '{E0AD166D-021F-4DF6-B979-3F4C3288DF1C}';
  IID__MiningModelGroup: TGUID = '{0C502ACB-20F6-40B7-B65C-CBED591C5BF1}';
  CLASS_MiningModelGroup: TGUID = '{2D6F3EBA-CCA0-4432-9BBC-A12AA981546E}';
  CLASS_IROLAPProvider: TGUID = '{6D9E0B29-6D07-11D2-88E2-00104BCC5A9C}';
  CLASS_Database: TGUID = '{B492C392-0195-11D2-89BA-00C04FB9898D}';

// *********************************************************************//
// Declaration of Enumerations defined in Type Library                    
// *********************************************************************//
// Constants for enum CommandTypes
type
  CommandTypes = TOleEnum;
const
  cmdUnknown = $00000000;
  cmdCreateMember = $00000001;
  cmdCreateSet = $00000002;
  cmdUseLibrary = $00000003;
  cmdCreateAction = $00000004;
  cmdCreateCellCalculation = $00000005;

// Constants for enum LevelTypes
type
  LevelTypes = TOleEnum;
const
  levRegular = $00000000;
  levAll = $00000001;
  levTimeYears = $00000014;
  levTimeHalfYears = $00000024;
  levTimeQuarters = $00000044;
  levTimeMonths = $00000084;
  levTimeWeeks = $00000104;
  levTimeDays = $00000204;
  levTimeHours = $00000304;
  levTimeMinutes = $00000404;
  levTimeSeconds = $00000804;
  levTimeUndefined = $00001004;
  levOrgUnit = $00001011;
  levBOMResource = $00001012;
  levQuantitative = $00001013;
  levAccount = $00001014;
  levCustomer = $00001021;
  levCustomerGroup = $00001022;
  levCustomerHousehold = $00001023;
  levProduct = $00001031;
  levProductGroup = $00001032;
  levScenario = $00001015;
  levUtility = $00001016;
  levPerson = $00001041;
  levCompany = $00001042;
  levCurrencySource = $00001051;
  levCurrencyDestination = $00001052;
  levChannel = $00001061;
  levRepresentative = $00001062;
  levPromotion = $00001071;
  levGeoContinent = $00002001;
  levGeoRegion = $00002002;
  levGeoCountry = $00002003;
  levGeoStateOrProvince = $00002004;
  levGeoCounty = $00002005;
  levGeoCity = $00002006;
  levGeoPostalCode = $00002007;
  levGeoPoint = $00002008;

// Constants for enum OrderTypes
type
  OrderTypes = TOleEnum;
const
  orderName = $00000000;
  orderKey = $00000001;
  orderMemberProperty = $00000002;

// Constants for enum HideIfValues
type
  HideIfValues = TOleEnum;
const
  hideNever = $00000000;
  hideIfOnlyChildAndBlankName = $00000001;
  hideIfOnlyChildAndParentsName = $00000002;
  hideIfBlankName = $00000003;
  hideIfParentsName = $00000004;

// Constants for enum GroupingValues
type
  GroupingValues = TOleEnum;
const
  groupingNone = $00000000;
  groupingAutomatic = $00000001;

// Constants for enum RootIfValues
type
  RootIfValues = TOleEnum;
const
  rootifParentIsBlank = $00000001;
  rootifParentIsMissing = $00000002;
  rootifParentIsSelf = $00000004;
  rootifParentIsBlankOrSelfOrMissing = $00000007;

// Constants for enum AggregatesTypes
type
  AggregatesTypes = TOleEnum;
const
  aggSum = $00000000;
  aggCount = $00000001;
  aggMin = $00000002;
  aggMax = $00000003;
  aggDistinctCount = $FFFFFFFF;

// Constants for enum DimensionTypes
type
  DimensionTypes = TOleEnum;
const
  dimRegular = $00000000;
  dimTime = $00000001;
  dimQuantitative = $00000005;
  dimAccounts = $00000006;
  dimCustomers = $00000007;
  dimProducts = $00000008;
  dimScenario = $00000009;
  dimUtility = $0000000A;
  dimCurrency = $0000000B;
  dimRates = $0000000C;
  dimChannel = $0000000D;
  dimPromotion = $0000000E;
  dimOrganization = $0000000F;
  dimBillOfMaterials = $00000010;
  dimGeography = $00000011;

// Constants for enum DimensionAggUsageTypes
type
  DimensionAggUsageTypes = TOleEnum;
const
  dimAggUsageStandard = $00000000;
  dimAggUsageTopOnly = $00000001;
  dimAggUsageDetailsOnly = $00000002;
  dimAggUsageCustom = $00000003;
  dimAggUsageTopAndDetailsOnly = $00000004;

// Constants for enum StorageModeValues
type
  StorageModeValues = TOleEnum;
const
  storeasROLAP = $00000000;
  storeasMOLAP = $00000001;

// Constants for enum MembersWithDataValues
type
  MembersWithDataValues = TOleEnum;
const
  dataforLeafMembersOnly = $00000000;
  dataforNonLeafMembersVisible = $00000001;
  dataforNonLeafMembersHidden = $00000002;

// Constants for enum SupportLevels
type
  SupportLevels = TOleEnum;
const
  supportNoROLAP = $00000000;
  supportNoSQLNoTransactions = $00000001;
  supportSQL = $00000003;
  supportTransactions = $00000005;
  supportSQLTransactions = $00000007;

// Constants for enum CloneOptions
type
  CloneOptions = TOleEnum;
const
  cloneObjectProperties = $00000000;
  cloneMinorChildren = $00000001;
  cloneMajorChildren = $00000002;

// Constants for enum ClassTypes
type
  ClassTypes = TOleEnum;
const
  clsServer = $00000001;
  clsDatabase = $00000002;
  clsDatabaseRole = $00000003;
  clsDatabaseCommand = $00000004;
  clsDatasource = $00000006;
  clsDatabaseDimension = $00000007;
  clsDatabaseLevel = $00000008;
  clsCube = $00000009;
  clsCubeMeasure = $0000000A;
  clsCubeDimension = $0000000B;
  clsCubeLevel = $0000000C;
  clsCubeCommand = $0000000D;
  clsCubeRole = $0000000E;
  clsPartition = $00000013;
  clsPartitionMeasure = $00000014;
  clsPartitionDimension = $00000015;
  clsPartitionLevel = $00000016;
  clsAggregation = $00000017;
  clsAggregationMeasure = $00000018;
  clsAggregationDimension = $00000019;
  clsAggregationLevel = $0000001A;
  clsCubeAnalyzer = $0000001C;
  clsPartitionAnalyzer = $0000001D;
  clsCollection = $0000001E;
  clsMemberProperty = $0000001F;
  clsRoleCommand = $00000020;
  clsMiningModel = $00000021;
  clsColumn = $00000022;
  clsMiningModelRole = $00000023;

// Constants for enum SubClassTypes
type
  SubClassTypes = TOleEnum;
const
  sbclsRegular = $00000000;
  sbclsRelational = $00000000;
  sbclsVirtual = $00000001;
  sbclsParentChild = $00000002;
  sbclsLinked = $00000003;
  sbclsRemote = $00000004;
  sbclsOlap = $00000005;
  sbclsNested = $00000006;
  sbclsMining = $00000007;

// Constants for enum OlapStateTypes
type
  OlapStateTypes = TOleEnum;
const
  olapStateNeverProcessed = $00000000;
  olapStateStructureChanged = $00000001;
  olapStateMemberPropertiesChanged = $00000002;
  olapStateSourceMappingChanged = $00000003;
  olapStateCurrent = $00000004;

// Constants for enum OlapLockTypes
type
  OlapLockTypes = TOleEnum;
const
  olapLockRead = $00000001;
  olaplockExtendedRead = $00000002;
  olapLockProcess = $00000004;
  olaplockWrite = $00000008;

// Constants for enum ProcessTypes
type
  ProcessTypes = TOleEnum;
const
  processDefault = $00000000;
  processFull = $00000001;
  processRefreshData = $00000002;
  processBuildStructure = $00000003;
  processResume = $00000004;
  processSuspend = $00000005;
  processRefreshDataAndIndex = $00000006;
  processReaggregate = $00000007;
  processFullReaggregate = $00000008;

// Constants for enum ProcessOptimizationModes
type
  ProcessOptimizationModes = TOleEnum;
const
  processOptimizationModeRegular = $00000000;
  processOptimizationModeLazyOptimizations = $00000001;

// Constants for enum OlapStorageModes
type
  OlapStorageModes = TOleEnum;
const
  olapmodeRolap = $0000000F;
  olapmodeMolapIndex = $00000000;
  olapmodeHybridIndex = $00000001;
  olapmodeAggsRolap = $00000003;
  olapmodeAggsMolapIndex = $00000000;

// Constants for enum OlapEditions
type
  OlapEditions = TOleEnum;
const
  olapEditionUnlimited = $00000000;
  olapEditionPivotOnly = $00000001;
  olapEditionNoPartitions = $00000002;
  olapEditionError = $FFFFFFFF;

// Constants for enum ValidateErrorCodes
type
  ValidateErrorCodes = TOleEnum;
const
  mderrNoError = $FFFFFFFF;
  mderrUnknownError = $00000000;
  mderrNameNotSet = $00000001;
  mderrParentNotSet = $00000002;
  mderrMissingDataSource = $00000003;
  mderrCannotConnectToDataSource = $00000004;
  mderrCannotAccessDataSourceTables = $00000005;
  mderrInvalidSchemaLoopDetectedInTableJoins = $00000006;
  mderrInvalidSchemaBadSourceColumn = $00000007;
  mderrInvalidSchemaDatasource = $00000008;
  mderrInvalidSchemaIslandDetectedInTableJoins = $00000009;
  mderrConnectionStringNotSet = $0000000A;
  mderrErrorBeginingDatasourceTransaction = $0000000B;
  mderrUsersNotSet = $0000000C;
  mderrStatementNotSet = $0000000D;
  mderrMeasureMissingSourceColumn = $0000000E;
  mderrMeasureInvalidColumnDataType = $0000000F;
  mderrAggColumnSizeNotSet = $00000010;
  mderrAggColumnTypeNotSet = $00000011;
  mderrInvalidLevelMemberKeyColumnNotSet = $00000012;
  mderrInvalidLevelMoreMembersThePreviousLevel = $00000013;
  mderrInvalidLevelZeroMembersCount = $00000014;
  mderrInvalidLevelBadTableInMemberKeyColumn = $00000015;
  mderrInvalidLevelType = $00000016;
  mderrInvalidLevelSourceMemberPropertyMissing = $00000017;
  mderrDimensionTableNotSet = $00000018;
  mderrInvalidDimensionZeroLevels = $00000019;
  mderrInvalidDimensionAtLeastOneNonAllLevel = $0000001A;
  mderrInvalidVirtualDimension = $0000001B;
  mderrInvalidCubeZeroMeasures = $0000001C;
  mderrInvalidCubeZeroDimensions = $0000001D;
  mderrInvalidCubeBadFactTableSize = $0000001E;
  mderrInvalidVirtualCubeMeasure = $0000001F;
  mderrInvalidVirtualCubeDimension = $00000020;
  mderrInvalidCubeBadFactTable = $00000021;
  mderrInvalidCubeNoSourceForVirtualDimension = $00000022;
  mderrInvalidCubeNoSourceLevelForVirtualDimension = $00000023;
  mderrTooManyMeasures = $00000024;
  mderrInvalidPartZeroMeasures = $00000025;
  mderrInvalidPartZeroDimensions = $00000026;
  mderrInvalidPartBadFactTableSize = $00000027;
  mderrInvalidPartBadFactTable = $00000028;
  mderrInvalidAggregationPrefix = $00000029;
  mderrOnlyOneWriteBackPartitionIsAllowed = $0000002A;
  mderrInvalidDatasetDefinition = $0000002B;
  mderrCubeToComplexForAggregationDesign = $0000002C;
  mderrAggPrefixNotSet = $0000002D;
  mderrNameAlreadySet = $0000002E;
  mderrInvalidDimensionParentChildMissing = $0000002F;
  mderrInvalidLevelBadParentKey = $00000030;
  mderrInvalidDimensionNoVisibleLevels = $00000031;
  mderrInvalidLevelBadOrderingMemberProperty = $00000032;
  mderrInvalidCubeNoVisibleDimensions = $00000033;
  mderrInvalidCubeNoVisibleMeasures = $00000034;
  mderrInvalidDimensionBadAreMemberKeysUnique = $00000035;
  mderrInvalidDimensionBadDependsOnDimension = $00000036;
  mderrInvalidDimensionParentChildTooManyLevels = $00000039;
  mderrInvalidDimensionParentChildInvalidLevel = $0000003A;
  mderrLinkedDimensionInNonLinkedCube = $0000003B;
  mderrLinkedLevelInNonLinkedDimension = $0000003C;
  mderrLinkedDimensionNotAllLevelsLinked = $0000003E;
  mderrLinkedCubeContainsNonLinkedDimension = $00000040;
  mderrLinkedDimensionPublic = $00000041;
  mderrInvalidRemoteServerName = $00000043;
  mderrInvalidCubeMultipleDistinctCountMeasures = $00000044;
  mderrInvalidCubeDrillThroughNotProperlyDefined = $00000045;
  mderrInvalidCubeInconsistentAggregations = $00000046;
  mderrInvalidParentChildLevelParentKeyMissing = $00000047;
  mderrInvalidLevelBadCustomRollupColumn = $00000048;
  mderrInvalidLevelGrouping = $00000049;
  mderrInvalidLevelBadSkippedLevelsColumn = $0000004A;
  mderrInvalidDimensionBadAreMemberNamesUnique = $0000004B;
  mderrInvalidLevelConflictingMemberProperties = $0000004C;
  mderrValidateLastLevelMustBeUnique = $0000004D;
  mderrROLAPDimensionsRequireROLAPPartition = $0000004E;
  mderrValidateLinkedCubeNoAggregationsAllowed = $0000004F;
  mderrROLAPDimensionsRequireAllLevel = $00000050;
  mderrInvalidLevelNamingTemplate = $00000051;
  mderrInvalidCubeBadFactTableAlias = $00000052;
  mderrInvalidPartBadFactTableAlias = $00000053;
  mderrInvalidVirtualDimensionDifferentColumnSizes = $00000054;
  mderrInvalidVirtualDimensionDifferentColumnTypes = $00000055;
  mderrInvalidVirtualDimensionInvalidMasterLevel = $00000056;
  mderrInvalidVirtualDimensionSourceLevelsMustBeInOrder = $00000057;
  mderrInvalidVirtualDimensionParentChildNotAllowed = $00000058;
  mderrInvalidVirtualDimensionV7sbclsVirtualLevelsNotAllowed = $00000059;
  mderrValidateLinkedCubeInvalidSourceCube = $0000005A;
  mderrInvalidDimensionLevelsAfterHiddenMustBeUnique = $0000005B;
  mderrInvalidVirtualDimensionMustHaveAllLevel = $0000005D;
  mderrInvalidMiningModelBadMiningAlgorithm = $0000005E;
  mderrInvalidMiningModelBadFromClause = $0000005F;
  mderrInvalidMiningModelBadSourceCube = $00000060;
  mderrInvalidMiningModelBadCaseDimension = $00000061;
  mderrInvalidMiningModelBadCaseLevel = $00000062;
  mderrInvalidMiningModelZeroColumns = $00000063;
  mderrInvalidMiningModelTooManyColumns = $00000064;
  mderrInvalidColumnBadSourceColumn = $00000065;
  mderrInvalidColumnBadSourceOlapObject = $00000066;
  mderrInvalidColumnBadParentKey = $00000067;
  mderrInvalidColumnCannotBePredictable = $00000068;
  mderrInvalidColumnBadContentType = $00000069;
  mderrInvalidColumnBadDistribution = $0000006A;
  mderrInvalidColumnBadModelingFlags = $0000006B;
  mderrInvalidColumnBadSpecialFlag = $0000006C;
  mderrInvalidColumnBadRelatedColumn = $0000006D;
  mderrInvalidColumnMustBeInputOrPredictable = $0000006E;
  mderrInvalidColumnBadDataType = $0000006F;
  mderrInvalidColumnBadFromClause = $00000070;
  mderrInvalidColumnZeroColumns = $00000071;
  mderrInvalidColumnTooManyColumns = $00000072;
  mderrInvalidColumnRelatedColumnsMustLinkToKey = $00000073;
  mderrInvalidColumnAtLeastOneColumnMustBeEnabled = $00000074;
  mderrInvalidColumnMemberPropertyRequiresParentLevel = $00000075;
  mderrInvalidCubeMiningDimensionInNonVirtualCube = $00000076;
  mderrInvalidMiningDimensionFromClause = $00000077;
  mderrInvalidMiningDimensionSourceTableFilterClause = $00000078;
  mderrInvalidCubeTooManyDimensions = $00000079;
  mderrInvalidCubeDistinctCountWithCustomRollups = $0000007A;
  mderrInvalidWritebackPartAggregateFunctionNotSum = $0000007B;
  mderrInvalidCubeLinkedWithTooManyPartitions = $0000007C;
  mderrInvalidDimensionBadAllLevelMemberKeyColumn = $0000007D;
  mderrInvalidCubeTooManyLevels = $0000007E;
  mderrInvalidDimLevelAfterHiddenMustHaveUniqueNames = $0000007F;
  mderrInvalidVirtualCubeNotAllUnderlyingDimsUsed = $00000080;
  mderrInvalidCubeDistinctCountWithOldVirtualDimension = $00000081;
  mderrInvalidCubeLinkedNotToAnalysisServer = $00000082;
  mderrInvalidCubeLinkedToSameServer = $00000083;
  mderrInvalidProcessOptionReservedForMOLAP = $00000084;
  mderrInvalidMiningModelAtLeastOneColumnMustBeEnabled = $00000085;
  mderrInvalidColumnCannotBeInput = $00000086;
  mderrInvalidMiningModelCannotHaveOldVirtDimension = $00000087;
  mderrInvalidMiningDimensionNoMiningModel = $00000088;
  mderrInvalidMiningModelSourceCubeUsesMiningDims = $00000089;
  mderrInvalidDimensionChangingMustHaveEnabledNonAllLevel = $0000008A;
  mderrInvalidMemberPropertySourceColumn = $0000008B;
  mderrInvalidLevelBadCustomRollupPropertiesColumn = $0000008C;
  mderrInvalidVCubeDrillThroughNotProperlyDefined = $0000008D;
  mderrInvalidCubeDistinctCountWithMiningDimension = $0000008E;
  mderrInvalidParentChildLevelHidden = $0000008F;
  mderrInvalidLevelGroupingROLAPDimension = $00000090;
  mderrInvalidCubeBadDefaultMeasure = $00000091;
  mderrInvalidMiningDimensionNoSourceColumn = $00000092;
  mderrInvalidMiningDimensionSourceColumnNotPredict = $00000093;
  mderrInvalidPartCannotHaveSliceOnROLAPDimension = $00000094;
  mderrInvalidMiningModelSourceCubeUsesCustomRollups = $00000095;

// Constants for enum ErrorCodes
type
  ErrorCodes = TOleEnum;
const
  mderrCollectionReadOnly = $80040001;
  mderrCollectionItemNotFound = $80040002;
  mderrDimensionLockedByCube = $80040003;
  mderrIllegalObjectName = $80040004;
  mderrCannotDeleteLastPartition = $80040005;
  mderrCannotRenameObject = $80040006;
  mderrInvalidPropertySetting = $80040007;
  mderrCannotDeleteDimension = $80040008;
  mderrCannotDeleteLevel = $80040009;
  mderrCannotDeleteMemberProperty = $8004000A;
  mderrCannotAddVirtualDimension = $8004000B;
  mderrObjectChangedByAnotherApp = $8004000C;
  mderrCannotSaveInsideTransaction = $8004000D;
  mderrCannotCommitDatabase = $8004000E;
  mderrCorruptedRegistrySettings = $8004000F;
  mderrDimensionNotInUnderlyingCubes = $80040010;
  mderrMeasureDoesNotHaveValidSourceColumn = $80040011;
  mderrTooManyLevelsInDimension = $80040012;
  mderrCannotRemoveMeasureFromDefaultAggregation = $80040013;
  mderrCouldNotOpenServiceControlManager = $80040014;
  mderrCouldNotOpenService = $80040015;
  mderrCouldNotQueryTheService = $80040016;
  mderrBadParameterForServiceState = $80040017;
  mderrUnsuccesfullServiceOperation = $80040018;
  mderrDuplicateKeyInCollection = $80040019;
  mderrInvalidParent = $8004001A;
  mderrCannotModifySharedObject = $8004001B;
  mderrLockDescriptionTooLong = $8004001C;
  mderrLockNetworkDown = $8004001D;
  mderrLockNetworkPathNotFound = $8004001E;
  mderrLockNetworkNameNotFound = $8004001F;
  mderrLockFileCorrupted = $80040020;
  mderrLockFileMissing = $80040021;
  mderrLockNotEnoughMemory = $80040022;
  mderrLockSystemError = $80040023;
  mderrLockCannotBeObtained = $80040024;
  mderrLockObjectNotLocked = $80040025;
  mderrLockAccessError = $80040026;
  mderrNameCannotBeEmpty = $80040027;
  mderrNameCannotBeChanged = $80040028;
  mderrPropertyCannotBeChanged = $80040029;
  mderrPropertyCollectionCannotBeChanged = $8004002A;
  mderrDefinitionCannotBeEmpty = $8004002B;
  mderrDefinitionDoesNotContainNameAndValue = $8004002C;
  mderrCorruptedProperty = $8004002D;
  mderrAggregationUsageNotCustom = $8004002E;
  mderrRegistryConnectFailed = $8004002F;
  mderrRegistryOpenKeyFailed = $80040030;
  mderrRegistryQueryValueFailed = $80040031;
  mderrRepositoryUpgradeFailed = $80040032;
  mderrRepositoryConnectionFailed = $80040033;
  mderrRepositoryIncompatible = $80040034;
  mderrInvalidLockType = $80040035;
  mderrCannotCloneObjectIntoItself = $80040036;
  mderrUnexpectedError = $80040037;
  mderrSourceDoesNotExist = $80040038;
  mderrTargetDoesNotExist = $80040039;
  mderrDifferentAggregationNumber = $8004003A;
  mderrDifferentAggregationStructure = $8004003B;
  mderrDifferentAggregationOLAPMode = $8004003C;
  mderrDifferentAggregationDatasources = $8004003D;
  mderrCannotCreatePartition = $8004003E;
  mderrLastLevelMustBeUnique = $80040040;
  mderrInvalidAggUsage = $80040042;
  mderrInvalidPermission = $80040044;
  mderrMemberPropertyNotFound = $80040045;
  mderrSkippedLevelsNotAvailable = $80040046;
  mderrCubeDimHasNoDatabaseDim = $80040047;
  mderrLinkedCubeInvalidConnectionString = $80040048;
  mderrLinkedCubeInvalidSourceCube = $80040049;
  mderrLinkedCubeNotEnoughDimensions = $8004004A;
  mderrCannotCreateVirtualDimensionFromAnother = $8004004B;
  mderrDifferentRemoteServers = $8004004D;
  mderrInvalidAggregationLevel = $8004004E;
  mderrInvalidStructure = $8004004F;
  mderrNoConnectionToServer = $80040050;
  mderrServerObjectNotOpened = $80040051;
  mderrServerInternal = $80040052;
  mderrInternal = $80040053;
  mderrCubeNotProcessed = $80040054;
  mderrLinkedCubeNoAggregationsAllowed = $80040055;
  mderrinvalidAggregateFunction = $80040056;
  mderrCannotChangeRemoteServer = $80040057;
  mderrCanceled = $80040058;
  mderrInitializationFailed = $8004005A;
  mderrServerObjectNotFound = $8004005B;
  mderrChildProcessFailed = $8004005D;
  mderrExecuteSQL = $8004005F;
  mderrCouldInitiateDimensionUpdate = $80040060;
  mderrDimensionMemberNotFound = $80040062;
  mderrTooManyDimensionMembers = $80040063;
  mderrIllegalMeasureType = $80040064;
  mderrODBC = $80040065;
  mderrStructureHasChanged = $80040067;
  mderrInvalidProcessType = $80040068;
  mderrCouldNotLockObject = $80040069;
  mderrPartitionMustBeProcessed = $8004006A;
  mderrInconsistentAggregations = $8004006B;
  mderrDeletingTablesOutsideOfTransaction = $8004006C;
  mderrObjectCantBeProcessedWithItsDimensions = $8004006D;
  mderrCouldNotUnLockObject = $8004006F;
  mderrTimeOut = $80040070;
  mderrClassError = $80040071;
  mderrOSError = $80040072;
  mderrFileError = $80040073;
  mderrMemoryError = $80040074;
  mderrNetworkError = $80040075;
  mderrODBCError = $80040076;
  mderrCOMError = $80040077;
  mderrProcessError = $80040078;
  mderrFormulaError = $80040079;
  mderrCalculateError = $8004007A;
  mderrDataError = $8004007B;
  mderrSecurityError = $8004007C;
  mderrObsoleteError = $8004007D;
  mderrCompatibilityError = $8004007E;
  mderrBadRequest = $8004007F;
  mderrBindError = $80040080;
  mderrListenError = $80040081;
  mderrSelectError = $80040082;
  mderrAcceptError = $80040083;
  mderrConnectError = $80040084;
  mderrTimeoutError = $80040085;
  mderrLoadDLLError = $80040086;
  mderrNoEntryPointError = $80040087;
  mderrCannotExecFuncError = $80040088;
  mderrAcquireCreditsError = $80040089;
  mderrFuncNotSupportedError = $8004008A;
  mderrImpersonateError = $8004008B;
  mderrRevertError = $8004008C;
  mderrCouldInitiateCubeUpdate = $8004008D;
  mderrLinkedCubeInvalidServer = $8004008E;
  mderrDimensionChangingCannotAddLevel = $8004008F;
  mderrInvalidMeasure = $80040091;
  mderrLinkedCubeCannotChangeProperty = $80040092;
  mderrRemotePartitionCannotHaveWriteableDimension = $80040093;
  mderrInternetError = $80040094;
  mderrCouldNotLogMissingMemberKeyErrors = $80040095;
  mderrTooManyMissingMemberKeys = $80040096;
  mderrInvalidRelatedColumn = $80040097;
  mderrInvalidDataType = $80040098;
  mderrInvalidSourceOlapObject = $8004009A;
  mderrMiningModelNotProcessed = $8004009B;
  mderrCannotDeleteDataSource = $8004009C;
  mderrROLAPDimensionsNotAvailable = $8004009D;
  mderrUserDefinedPartitionsNotAvailable = $8004009E;
  mderrLinkedCubesNotAvailable = $8004009F;
  mderrDimensionWritebackNotAvailable = $800400A0;
  mderrCustomRollupsNotAvailable = $800400A1;
  mderrRepositoryConnectionStringChanged = $800400A2;
  mderrLinkedCubeSynchronizationFailed = $800400A3;
  mderrMergedPartitionsMustBothUseIndexedViewsOrTables = $800400A4;
  mderrCannotEnableRealTimeUpdatesWithoutIndexedViews = $800400A5;
  mderrRealTimeUpdatesNotAvailable = $800400A6;
  mderrCellCalculationsNotAvailable = $800400A7;
  mderrInvalidTransactionOperation = $800400A8;

// Constants for enum ServerStates
type
  ServerStates = TOleEnum;
const
  stateUnknown = $00000000;
  stateConnected = $00000001;
  stateFailed = $00000002;

// Constants for enum LanguageValues
type
  LanguageValues = TOleEnum;
const
  languageAny = $00000000;
  languageArabic = $00000001;
  languageBulgarian = $00000002;
  languageCatalan = $00000003;
  languageChinese = $00000004;
  languageCzech = $00000005;
  languageDanish = $00000006;
  languageGerman = $00000007;
  languageGreek = $00000008;
  languageEnglish = $00000009;
  languageSpanish = $0000000A;
  languageFinnish = $0000000B;
  languageFrench = $0000000C;
  languageHebrew = $0000000D;
  languageHungarian = $0000000E;
  languageIcelandic = $0000000F;
  languageItalian = $00000010;
  languageJapanese = $00000011;
  languageKorean = $00000012;
  languageDutch = $00000013;
  languageNorwegian = $00000014;
  languagePolish = $00000015;
  languagePortuguese = $00000016;
  languageRhaetoRomanic = $00000017;
  languageRomanian = $00000018;
  languageRussian = $00000019;
  languageSerboCroatian = $0000001A;
  languageSlovak = $0000001B;
  languageAlbanian = $0000001C;
  languageSwedish = $0000001D;
  languageThai = $0000001E;
  languageTurkish = $0000001F;
  languageUrdu = $00000020;
  languageIndonesian = $00000021;
  languageUkrainian = $00000022;
  languageByelorussian = $00000023;
  languageSlovenian = $00000024;
  languageEstonian = $00000025;
  languageLatvian = $00000026;
  languageLithuanian = $00000027;
  languageMaori = $00000028;
  languageFarsi = $00000029;
  languageVietnamese = $0000002A;
  languageLaotian = $0000002B;
  languageKampuchean = $0000002C;
  languageBasque = $0000002D;
  languageSorbian = $0000002E;
  languageMacedonian = $0000002F;
  languageSutu = $00000030;
  languageTsonga = $00000031;
  languageTswana = $00000032;
  languageVenda = $00000033;
  languageXhosa = $00000034;
  languageZulu = $00000035;
  languageAfrikaans = $00000036;
  languageFaeroese = $00000038;
  languageMaltese = $0000003A;
  languageSami = $0000003B;
  languageScotsGaelic = $0000003C;

// Constants for enum PropertyTypeValues
type
  PropertyTypeValues = TOleEnum;
const
  propRegular = $00000000;
  propID = $00000001;
  propRelationToParent = $00000002;
  propSequence = $00000003;
  propOrgTitle = $00000011;
  propCaption = $00000021;
  propCaptionShort = $00000022;
  propCaptionDescription = $00000023;
  propCaptionAbreviation = $00000024;
  propWebURL = $00000031;
  propWebHTML = $00000032;
  propWebXMLorXSL = $00000033;
  propWebMailAlias = $00000034;
  propAddress = $00000041;
  propAddressStreet = $00000042;
  propAddressHouse = $00000043;
  propAddressCity = $00000044;
  propAddressStateorProvice = $00000045;
  propAddressZip = $00000046;
  propAddressQuarter = $00000047;
  propAddressCountry = $00000048;
  propAddressBuilding = $00000049;
  propAddressRoom = $0000004A;
  propAddressFloor = $0000004B;
  propAddressFax = $0000004C;
  propAddressPhone = $0000004D;
  propGeoCentroidX = $00000061;
  propGeoCentroidY = $00000062;
  propGeoCentroidZ = $00000063;
  propGeoBoundaryTop = $00000064;
  propGeoBoundaryLeft = $00000065;
  propGeoBoundaryBottom = $00000066;
  propGeoBoundaryRight = $00000067;
  propGeoBoundaryFront = $00000068;
  propGeoBoundaryRear = $00000069;
  propGeoBoundaryPolygon = $0000006A;
  propPhysicalSize = $00000071;
  propPhysicalColor = $00000072;
  propPhysicalWeight = $00000073;
  propPhysicalHeight = $00000074;
  propPhysicalWidth = $00000075;
  propPhysicalDepth = $00000076;
  propPhysicalVolume = $00000077;
  propPhysicalDensity = $00000078;
  propPersonFullName = $00000082;
  propPersonFirstName = $00000083;
  propPersonLastName = $00000084;
  propPersonMiddleName = $00000085;
  propPersonDemographic = $00000086;
  propPersonContact = $00000087;
  propQtyRangeLow = $00000091;
  propQtyRangeHigh = $00000092;
  propFormattingColor = $000000A1;
  propFormattingOrder = $000000A2;
  propFormattingFont = $000000A3;
  propFormattingFontEffects = $000000A4;
  propFormattingFontSize = $000000A5;
  propFormattingSubTotal = $000000A6;
  propDate = $000000B1;
  propDateStart = $000000B2;
  propDateEnded = $000000B3;
  propDateCanceled = $000000B4;
  propDateModified = $000000B5;
  propDateDuration = $000000B6;
  propVersion = $000000C1;

type

// *********************************************************************//
// Forward declaration of types defined in TypeLibrary                    
// *********************************************************************//
  _PartitionMeasure = interface;
  _PartitionMeasureDisp = dispinterface;
  _ICommon = interface;
  _ICommonDisp = dispinterface;
  _PartitionLevel = interface;
  _PartitionLevelDisp = dispinterface;
  _Level = interface;
  _LevelDisp = dispinterface;
  _PartitionDimension = interface;
  _PartitionDimensionDisp = dispinterface;
  _Dimension = interface;
  _DimensionDisp = dispinterface;
  _Command = interface;
  _CommandDisp = dispinterface;
  _Measure = interface;
  _MeasureDisp = dispinterface;
  _Role = interface;
  _RoleDisp = dispinterface;
  _Properties = interface;
  _PropertiesDisp = dispinterface;
  _Property = interface;
  _PropertyDisp = dispinterface;
  _CubeAnalyzer = interface;
  _CubeAnalyzerDisp = dispinterface;
  _PartitionAnalyzer = interface;
  _PartitionAnalyzerDisp = dispinterface;
  _IROLAPProvider = interface;
  _IROLAPProviderDisp = dispinterface;
  __IROLAPProvider = dispinterface;
  _IDatabaseEvents = interface;
  _IDatabaseEventsDisp = dispinterface;
  _Server = interface;
  _ServerDisp = dispinterface;
  _MDStore = interface;
  _MDStoreDisp = dispinterface;
  _OlapCollection = interface;
  _OlapCollectionDisp = dispinterface;
  _Partition = interface;
  _PartitionDisp = dispinterface;
  _Aggregation = interface;
  _AggregationDisp = dispinterface;
  _AggregationDimension = interface;
  _AggregationDimensionDisp = dispinterface;
  _AggregationLevel = interface;
  _AggregationLevelDisp = dispinterface;
  _AggregationMeasure = interface;
  _AggregationMeasureDisp = dispinterface;
  _Cube = interface;
  _CubeDisp = dispinterface;
  _CubeCommand = interface;
  _CubeCommandDisp = dispinterface;
  _CubeDimension = interface;
  _CubeDimensionDisp = dispinterface;
  _CubeGroup = interface;
  _CubeGroupDisp = dispinterface;
  _CubeLevel = interface;
  _CubeLevelDisp = dispinterface;
  _CubeMeasure = interface;
  _CubeMeasureDisp = dispinterface;
  _Database = interface;
  _DatabaseDisp = dispinterface;
  __Database = dispinterface;
  _DBCommand = interface;
  _DBCommandDisp = dispinterface;
  _DbDimension = interface;
  _DbDimensionDisp = dispinterface;
  _DBGroup = interface;
  _DBGroupDisp = dispinterface;
  _DbLevel = interface;
  _DbLevelDisp = dispinterface;
  _MemberProperty = interface;
  _MemberPropertyDisp = dispinterface;
  _DataSource = interface;
  _DataSourceDisp = dispinterface;
  _RoleCommand = interface;
  _RoleCommandDisp = dispinterface;
  _Column = interface;
  _ColumnDisp = dispinterface;
  _MiningModel = interface;
  _MiningModelDisp = dispinterface;
  _MiningModelGroup = interface;
  _MiningModelGroupDisp = dispinterface;

// *********************************************************************//
// Declaration of CoClasses defined in Type Library                       
// (NOTE: Here we map each CoClass to its Default Interface)              
// *********************************************************************//
  Command = _Command;
  PartitionLevel = _PartitionLevel;
  Level = _Level;
  Measure = _Measure;
  Role = _Role;
  PartitionMeasure = _PartitionMeasure;
  ICommon = _ICommon;
  Properties = _Properties;
  Property_ = _Property;
  CubeAnalyzer = _CubeAnalyzer;
  PartitionAnalyzer = _PartitionAnalyzer;
  PartitionDimension = _PartitionDimension;
  Dimension = _Dimension;
  IDatabaseEvents = _IDatabaseEvents;
  Server = _Server;
  MDStore = _MDStore;
  OlapCollection = _OlapCollection;
  Partition = _Partition;
  Aggregation = _Aggregation;
  AggregationDimension = _AggregationDimension;
  AggregationLevel = _AggregationLevel;
  AggregationMeasure = _AggregationMeasure;
  Cube = _Cube;
  CubeCommand = _CubeCommand;
  CubeDimension = _CubeDimension;
  CubeGroup = _CubeGroup;
  CubeLevel = _CubeLevel;
  CubeMeasure = _CubeMeasure;
  DBCommand = _DBCommand;
  DbDimension = _DbDimension;
  DBGroup = _DBGroup;
  DbLevel = _DbLevel;
  MemberProperty = _MemberProperty;
  DataSource = _DataSource;
  RoleCommand = _RoleCommand;
  Column = _Column;
  MiningModel = _MiningModel;
  MiningModelGroup = _MiningModelGroup;
  IROLAPProvider = _IROLAPProvider;
  Database = _Database;


// *********************************************************************//
// Interface: _PartitionMeasure
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5839-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionMeasure = interface(IDispatch)
    ['{E8AC5839-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionMeasure_28_1; safecall;
    procedure GhostMethod__PartitionMeasure_32_2; safecall;
    procedure GhostMethod__PartitionMeasure_36_3; safecall;
    procedure GhostMethod__PartitionMeasure_40_4; safecall;
    procedure GhostMethod__PartitionMeasure_44_5; safecall;
    procedure GhostMethod__PartitionMeasure_48_6; safecall;
    procedure GhostMethod__PartitionMeasure_52_7; safecall;
    procedure GhostMethod__PartitionMeasure_56_8; safecall;
    procedure GhostMethod__PartitionMeasure_60_9; safecall;
    procedure GhostMethod__PartitionMeasure_64_10; safecall;
    procedure GhostMethod__PartitionMeasure_68_11; safecall;
    procedure GhostMethod__PartitionMeasure_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_AggregationColumn: WideString; safecall;
    function Get_Valid: WordBool; safecall;
    function Get_SourceField: WideString; safecall;
    procedure Set_SourceField(var Param1: WideString); safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Parent: _Partition; safecall;
    procedure _Set_Parent(const Param1: _Partition); safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IMeasure: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__PartitionMeasure_156_13; safecall;
    procedure GhostMethod__PartitionMeasure_160_14; safecall;
    procedure GhostMethod__PartitionMeasure_164_15; safecall;
    procedure GhostMethod__PartitionMeasure_168_16; safecall;
    procedure GhostMethod__PartitionMeasure_172_17; safecall;
    procedure GhostMethod__PartitionMeasure_176_18; safecall;
    procedure GhostMethod__PartitionMeasure_180_19; safecall;
    procedure GhostMethod__PartitionMeasure_184_20; safecall;
    procedure GhostMethod__PartitionMeasure_188_21; safecall;
    procedure GhostMethod__PartitionMeasure_192_22; safecall;
    procedure GhostMethod__PartitionMeasure_196_23; safecall;
    procedure GhostMethod__PartitionMeasure_200_24; safecall;
    function Get_AggregateFunction: AggregatesTypes; safecall;
    function Get_FormatString: WideString; safecall;
    function Get_IsInternal: WordBool; safecall;
    procedure GhostMethod__PartitionMeasure_216_25; safecall;
    procedure GhostMethod__PartitionMeasure_220_26; safecall;
    procedure GhostMethod__PartitionMeasure_224_27; safecall;
    procedure GhostMethod__PartitionMeasure_228_28; safecall;
    procedure GhostMethod__PartitionMeasure_232_29; safecall;
    procedure GhostMethod__PartitionMeasure_236_30; safecall;
    procedure GhostMethod__PartitionMeasure_240_31; safecall;
    procedure GhostMethod__PartitionMeasure_244_32; safecall;
    procedure GhostMethod__PartitionMeasure_248_33; safecall;
    procedure GhostMethod__PartitionMeasure_252_34; safecall;
    procedure GhostMethod__PartitionMeasure_256_35; safecall;
    procedure GhostMethod__PartitionMeasure_260_36; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    procedure Set_AggregateFunction(Param1: AggregatesTypes); safecall;
    procedure Set_FormatString(const Param1: WideString); safecall;
    procedure Set_IsInternal(Param1: WordBool); safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_SourceColumn(const Param1: WideString); safecall;
    function Get_SourceColumnType: Smallint; safecall;
    procedure Set_SourceColumnType(Param1: Smallint); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property AggregationColumn: WideString read Get_AggregationColumn;
    property SourceField: WideString read Get_SourceField write Set_SourceField;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Partition read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property IMeasure: IDispatch read Get_IMeasure;
    property AggregateFunction: AggregatesTypes read Get_AggregateFunction write Set_AggregateFunction;
    property FormatString: WideString read Get_FormatString write Set_FormatString;
    property IsInternal: WordBool read Get_IsInternal write Set_IsInternal;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property SourceColumnType: Smallint read Get_SourceColumnType write Set_SourceColumnType;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
  end;

// *********************************************************************//
// DispIntf:  _PartitionMeasureDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5839-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionMeasureDisp = dispinterface
    ['{E8AC5839-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionMeasure_28_1; dispid 1610743808;
    procedure GhostMethod__PartitionMeasure_32_2; dispid 1610743809;
    procedure GhostMethod__PartitionMeasure_36_3; dispid 1610743810;
    procedure GhostMethod__PartitionMeasure_40_4; dispid 1610743811;
    procedure GhostMethod__PartitionMeasure_44_5; dispid 1610743812;
    procedure GhostMethod__PartitionMeasure_48_6; dispid 1610743813;
    procedure GhostMethod__PartitionMeasure_52_7; dispid 1610743814;
    procedure GhostMethod__PartitionMeasure_56_8; dispid 1610743815;
    procedure GhostMethod__PartitionMeasure_60_9; dispid 1610743816;
    procedure GhostMethod__PartitionMeasure_64_10; dispid 1610743817;
    procedure GhostMethod__PartitionMeasure_68_11; dispid 1610743818;
    procedure GhostMethod__PartitionMeasure_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027149;
    property Description: WideString dispid 1745027142;
    property Valid: WordBool dispid 1745027139;
    property AggregationColumn: WideString readonly dispid 1745027138;
    property SourceField: WideString dispid 1745027137;
    property Name: WideString dispid 1745027135;
    property Num: Smallint readonly dispid 1745027134;
    property Database: _Database readonly dispid 1745027133;
    property Server: _Server readonly dispid 1745027132;
    property Parent: _Partition dispid 1745027131;
    property Path: WideString readonly dispid 1745027129;
    property ClassType: Smallint readonly dispid 1745027128;
    property SubClassType: SubClassTypes readonly dispid 1745027127;
    property IObject: IDispatch readonly dispid 1745027126;
    property IMeasure: IDispatch readonly dispid 1745027125;
    procedure ClearCollections; dispid 1610809429;
    procedure GhostMethod__PartitionMeasure_156_13; dispid 1610743840;
    procedure GhostMethod__PartitionMeasure_160_14; dispid 1610743841;
    procedure GhostMethod__PartitionMeasure_164_15; dispid 1610743842;
    procedure GhostMethod__PartitionMeasure_168_16; dispid 1610743843;
    procedure GhostMethod__PartitionMeasure_172_17; dispid 1610743844;
    procedure GhostMethod__PartitionMeasure_176_18; dispid 1610743845;
    procedure GhostMethod__PartitionMeasure_180_19; dispid 1610743846;
    procedure GhostMethod__PartitionMeasure_184_20; dispid 1610743847;
    procedure GhostMethod__PartitionMeasure_188_21; dispid 1610743848;
    procedure GhostMethod__PartitionMeasure_192_22; dispid 1610743849;
    procedure GhostMethod__PartitionMeasure_196_23; dispid 1610743850;
    procedure GhostMethod__PartitionMeasure_200_24; dispid 1610743851;
    property AggregateFunction: AggregatesTypes dispid 1745027220;
    property FormatString: WideString dispid 1745027219;
    property IsInternal: WordBool dispid 1745027218;
    procedure GhostMethod__PartitionMeasure_216_25; dispid 1610743855;
    procedure GhostMethod__PartitionMeasure_220_26; dispid 1610743856;
    procedure GhostMethod__PartitionMeasure_224_27; dispid 1610743857;
    procedure GhostMethod__PartitionMeasure_228_28; dispid 1610743858;
    procedure GhostMethod__PartitionMeasure_232_29; dispid 1610743859;
    procedure GhostMethod__PartitionMeasure_236_30; dispid 1610743860;
    procedure GhostMethod__PartitionMeasure_240_31; dispid 1610743861;
    procedure GhostMethod__PartitionMeasure_244_32; dispid 1610743862;
    procedure GhostMethod__PartitionMeasure_248_33; dispid 1610743863;
    procedure GhostMethod__PartitionMeasure_252_34; dispid 1610743864;
    procedure GhostMethod__PartitionMeasure_256_35; dispid 1610743865;
    procedure GhostMethod__PartitionMeasure_260_36; dispid 1610743866;
    property OrdinalPosition: Smallint readonly dispid 1745027225;
    property IsValid: WordBool readonly dispid 1745027224;
    property SourceColumn: WideString dispid 1745027223;
    property SourceColumnType: Smallint dispid 1745027222;
    property IsVisible: WordBool dispid 1745027221;
  end;

// *********************************************************************//
// Interface: _ICommon
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5827-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _ICommon = interface(IDispatch)
    ['{E8AC5827-7127-11D2-8A35-00C04FB9898D}']
    function Get_Ancestor: _ICommon; safecall;
    function Get_Name: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    function Get_Parent: _ICommon; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: IDispatch); safecall;
    function Get_Path: WideString; safecall;
    function Get_Collections: _Collection; safecall;
    function Get_Properties: _Properties; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure _Set_CustomProperties(const Param1: _Properties); safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_id: WideString; safecall;
    procedure Set_id(const Param1: WideString); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Valid(Param1: WordBool); safecall;
    function Get_Version: Integer; safecall;
    procedure Set_Version(Param1: Integer); safecall;
    procedure AddChild(const col: _OlapCollection; const ChildObject: _ICommon; 
                       InsertBefore: Smallint); safecall;
    procedure ClearCollections; safecall;
    procedure GetChildren(const col: _OlapCollection); safecall;
    procedure GetObject(const PropertyCollection: _Properties; RefreshCollections: WordBool); safecall;
    procedure RemoveChild(const col: _OlapCollection; const ChildObject: _ICommon); safecall;
    procedure SaveObject; safecall;
    procedure LockObject(LockType: Smallint; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    property Ancestor: _ICommon read Get_Ancestor;
    property Name: WideString read Get_Name;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property Server: _Server read Get_Server;
    property Path: WideString read Get_Path;
    property Collections: _Collection read Get_Collections;
    property Properties: _Properties read Get_Properties;
    property CustomProperties: _Properties read Get_CustomProperties write _Set_CustomProperties;
    property IsTemporary: WordBool read Get_IsTemporary;
    property id: WideString read Get_id write Set_id;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Version: Integer read Get_Version write Set_Version;
  end;

// *********************************************************************//
// DispIntf:  _ICommonDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5827-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _ICommonDisp = dispinterface
    ['{E8AC5827-7127-11D2-8A35-00C04FB9898D}']
    property Ancestor: _ICommon readonly dispid 1745027085;
    property Name: WideString readonly dispid 1745027084;
    property ClassType: ClassTypes readonly dispid 1745027083;
    property SubClassType: SubClassTypes dispid 1745027082;
    function Parent: _ICommon; dispid 1745027081;
    property Server: _Server readonly dispid 1745027080;
    property Path: WideString readonly dispid 1745027079;
    property Collections: _Collection readonly dispid 1745027078;
    property Properties: _Properties readonly dispid 1745027077;
    property CustomProperties: _Properties dispid 1745027076;
    property IsTemporary: WordBool readonly dispid 1745027075;
    property id: WideString dispid 1745027074;
    property Valid: WordBool dispid 1745027073;
    property Version: Integer dispid 1745027072;
    procedure AddChild(const col: _OlapCollection; const ChildObject: _ICommon; 
                       InsertBefore: Smallint); dispid 1610809358;
    procedure ClearCollections; dispid 1610809359;
    procedure GetChildren(const col: _OlapCollection); dispid 1610809360;
    procedure GetObject(const PropertyCollection: _Properties; RefreshCollections: WordBool); dispid 1610809361;
    procedure RemoveChild(const col: _OlapCollection; const ChildObject: _ICommon); dispid 1610809362;
    procedure SaveObject; dispid 1610809363;
    procedure LockObject(LockType: Smallint; const LockDescription: WideString); dispid 1610809364;
    procedure UnlockObject; dispid 1610809365;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809366;
  end;

// *********************************************************************//
// Interface: _PartitionLevel
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5832-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionLevel = interface(IDispatch)
    ['{E8AC5832-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionLevel_28_1; safecall;
    procedure GhostMethod__PartitionLevel_32_2; safecall;
    procedure GhostMethod__PartitionLevel_36_3; safecall;
    procedure GhostMethod__PartitionLevel_40_4; safecall;
    procedure GhostMethod__PartitionLevel_44_5; safecall;
    procedure GhostMethod__PartitionLevel_48_6; safecall;
    procedure GhostMethod__PartitionLevel_52_7; safecall;
    procedure GhostMethod__PartitionLevel_56_8; safecall;
    procedure GhostMethod__PartitionLevel_60_9; safecall;
    procedure GhostMethod__PartitionLevel_64_10; safecall;
    procedure GhostMethod__PartitionLevel_68_11; safecall;
    procedure GhostMethod__PartitionLevel_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Validate: ValidateErrorCodes; safecall;
    function Get_AggregationColumn: WideString; safecall;
    function Get_FromClause: WideString; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_MemberNameColumn: WideString; safecall;
    function Get_SliceValue: WideString; safecall;
    function Get_UniqueItems: Smallint; safecall;
    function Get_ColumnType: Smallint; safecall;
    function Get_Ordering: OrderTypes; safecall;
    function Get_ColumnSize: Smallint; safecall;
    function Get_LevelType: LevelTypes; safecall;
    function Get_IsDisabled: WordBool; safecall;
    function Get_EnableAggregations: WordBool; safecall;
    function Get_MemberKeyColumn: WideString; safecall;
    procedure Set_MemberKeyColumn(const Param1: WideString); safecall;
    function Get_MemberKeyTable: WideString; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Path: WideString; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _PartitionDimension); safecall;
    function Get_Parent: _PartitionDimension; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    procedure Set_SliceValue(const Param1: WideString); safecall;
    function Get_Num: Smallint; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ILevel: IDispatch; safecall;
    function Get_MemberProperties: _OlapCollection; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__PartitionLevel_224_13; safecall;
    procedure GhostMethod__PartitionLevel_228_14; safecall;
    procedure GhostMethod__PartitionLevel_232_15; safecall;
    procedure GhostMethod__PartitionLevel_236_16; safecall;
    procedure GhostMethod__PartitionLevel_240_17; safecall;
    procedure GhostMethod__PartitionLevel_244_18; safecall;
    procedure GhostMethod__PartitionLevel_248_19; safecall;
    procedure GhostMethod__PartitionLevel_252_20; safecall;
    procedure GhostMethod__PartitionLevel_256_21; safecall;
    procedure GhostMethod__PartitionLevel_260_22; safecall;
    procedure GhostMethod__PartitionLevel_264_23; safecall;
    procedure GhostMethod__PartitionLevel_268_24; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_ColumnType(Param1: Smallint); safecall;
    procedure Set_ColumnSize(Param1: Smallint); safecall;
    function Get_EstimatedSize: Integer; safecall;
    procedure Set_EstimatedSize(Param1: Integer); safecall;
    procedure Set_LevelType(Param1: LevelTypes); safecall;
    procedure Set_MemberNameColumn(const Param1: WideString); safecall;
    procedure Set_IsDisabled(Param1: WordBool); safecall;
    function Get_IsUnique: WordBool; safecall;
    procedure Set_IsUnique(Param1: WordBool); safecall;
    procedure Set_Ordering(Param1: OrderTypes); safecall;
    procedure Set_EnableAggregations(Param1: WordBool); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_ParentKeyColumn: WideString; safecall;
    procedure Set_ParentKeyColumn(const Param1: WideString); safecall;
    function Get_LevelNamingTemplate: WideString; safecall;
    procedure Set_LevelNamingTemplate(const Param1: WideString); safecall;
    function Get_HideMemberIf: HideIfValues; safecall;
    procedure Set_HideMemberIf(Param1: HideIfValues); safecall;
    function Get_OrderingMemberProperty: WideString; safecall;
    procedure Set_OrderingMemberProperty(const Param1: WideString); safecall;
    function Get_CustomRollupExpression: WideString; safecall;
    procedure Set_CustomRollupExpression(const Param1: WideString); safecall;
    function Get_Grouping: GroupingValues; safecall;
    procedure Set_Grouping(Param1: GroupingValues); safecall;
    function Get_SkippedLevelsColumn: WideString; safecall;
    procedure Set_SkippedLevelsColumn(const Param1: WideString); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_CustomRollupColumn: WideString; safecall;
    procedure Set_CustomRollupColumn(const Param1: WideString); safecall;
    procedure Set_CustomRollupPropertiesColumn(const Param1: WideString); safecall;
    function Get_CustomRollupPropertiesColumn: WideString; safecall;
    function Get_RootMemberIf: RootIfValues; safecall;
    procedure Set_RootMemberIf(Param1: RootIfValues); safecall;
    function Get_UnaryOperatorColumn: WideString; safecall;
    procedure Set_UnaryOperatorColumn(const Param1: WideString); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property AggregationColumn: WideString read Get_AggregationColumn;
    property FromClause: WideString read Get_FromClause;
    property JoinClause: WideString read Get_JoinClause;
    property MemberNameColumn: WideString read Get_MemberNameColumn write Set_MemberNameColumn;
    property SliceValue: WideString read Get_SliceValue write Set_SliceValue;
    property UniqueItems: Smallint read Get_UniqueItems;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property Ordering: OrderTypes read Get_Ordering write Set_Ordering;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property LevelType: LevelTypes read Get_LevelType write Set_LevelType;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property EnableAggregations: WordBool read Get_EnableAggregations write Set_EnableAggregations;
    property MemberKeyColumn: WideString read Get_MemberKeyColumn write Set_MemberKeyColumn;
    property MemberKeyTable: WideString read Get_MemberKeyTable;
    property Cube: _Cube read Get_Cube;
    property Path: WideString read Get_Path;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _PartitionDimension read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property ILevel: IDispatch read Get_ILevel;
    property MemberProperties: _OlapCollection read Get_MemberProperties;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property EstimatedSize: Integer read Get_EstimatedSize write Set_EstimatedSize;
    property IsUnique: WordBool read Get_IsUnique write Set_IsUnique;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property ParentKeyColumn: WideString read Get_ParentKeyColumn write Set_ParentKeyColumn;
    property LevelNamingTemplate: WideString read Get_LevelNamingTemplate write Set_LevelNamingTemplate;
    property HideMemberIf: HideIfValues read Get_HideMemberIf write Set_HideMemberIf;
    property OrderingMemberProperty: WideString read Get_OrderingMemberProperty write Set_OrderingMemberProperty;
    property CustomRollupExpression: WideString read Get_CustomRollupExpression write Set_CustomRollupExpression;
    property Grouping: GroupingValues read Get_Grouping write Set_Grouping;
    property SkippedLevelsColumn: WideString read Get_SkippedLevelsColumn write Set_SkippedLevelsColumn;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property CustomRollupColumn: WideString read Get_CustomRollupColumn write Set_CustomRollupColumn;
    property CustomRollupPropertiesColumn: WideString read Get_CustomRollupPropertiesColumn write Set_CustomRollupPropertiesColumn;
    property RootMemberIf: RootIfValues read Get_RootMemberIf write Set_RootMemberIf;
    property UnaryOperatorColumn: WideString read Get_UnaryOperatorColumn write Set_UnaryOperatorColumn;
  end;

// *********************************************************************//
// DispIntf:  _PartitionLevelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5832-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionLevelDisp = dispinterface
    ['{E8AC5832-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionLevel_28_1; dispid 1610743808;
    procedure GhostMethod__PartitionLevel_32_2; dispid 1610743809;
    procedure GhostMethod__PartitionLevel_36_3; dispid 1610743810;
    procedure GhostMethod__PartitionLevel_40_4; dispid 1610743811;
    procedure GhostMethod__PartitionLevel_44_5; dispid 1610743812;
    procedure GhostMethod__PartitionLevel_48_6; dispid 1610743813;
    procedure GhostMethod__PartitionLevel_52_7; dispid 1610743814;
    procedure GhostMethod__PartitionLevel_56_8; dispid 1610743815;
    procedure GhostMethod__PartitionLevel_60_9; dispid 1610743816;
    procedure GhostMethod__PartitionLevel_64_10; dispid 1610743817;
    procedure GhostMethod__PartitionLevel_68_11; dispid 1610743818;
    procedure GhostMethod__PartitionLevel_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027165;
    property Description: WideString dispid 1745027162;
    property Valid: WordBool dispid 1745027160;
    function Validate: ValidateErrorCodes; dispid 1610809443;
    property AggregationColumn: WideString readonly dispid 1745027159;
    property FromClause: WideString readonly dispid 1745027157;
    property JoinClause: WideString readonly dispid 1745027156;
    property MemberNameColumn: WideString dispid 1745027155;
    property SliceValue: WideString dispid 1745027154;
    property UniqueItems: Smallint readonly dispid 1745027153;
    property ColumnType: Smallint dispid 1745027152;
    property Ordering: OrderTypes dispid 1745027151;
    property ColumnSize: Smallint dispid 1745027150;
    property LevelType: LevelTypes dispid 1745027149;
    property IsDisabled: WordBool dispid 1745027148;
    property EnableAggregations: WordBool dispid 1745027145;
    property MemberKeyColumn: WideString dispid 1745027144;
    property MemberKeyTable: WideString readonly dispid 1745027143;
    property Cube: _Cube readonly dispid 1745027142;
    property Path: WideString readonly dispid 1745027141;
    property Database: _Database readonly dispid 1745027139;
    property Server: _Server readonly dispid 1745027138;
    property Parent: _PartitionDimension dispid 1745027137;
    property Name: WideString dispid 1745027136;
    property Num: Smallint readonly dispid 1745027135;
    property ClassType: Smallint readonly dispid 1745027134;
    property SubClassType: SubClassTypes readonly dispid 1745027133;
    property IObject: IDispatch readonly dispid 1745027131;
    property ILevel: IDispatch readonly dispid 1745027130;
    property MemberProperties: _OlapCollection readonly dispid 1745027129;
    procedure ClearCollections; dispid 1610809445;
    procedure GhostMethod__PartitionLevel_224_13; dispid 1610743857;
    procedure GhostMethod__PartitionLevel_228_14; dispid 1610743858;
    procedure GhostMethod__PartitionLevel_232_15; dispid 1610743859;
    procedure GhostMethod__PartitionLevel_236_16; dispid 1610743860;
    procedure GhostMethod__PartitionLevel_240_17; dispid 1610743861;
    procedure GhostMethod__PartitionLevel_244_18; dispid 1610743862;
    procedure GhostMethod__PartitionLevel_248_19; dispid 1610743863;
    procedure GhostMethod__PartitionLevel_252_20; dispid 1610743864;
    procedure GhostMethod__PartitionLevel_256_21; dispid 1610743865;
    procedure GhostMethod__PartitionLevel_260_22; dispid 1610743866;
    procedure GhostMethod__PartitionLevel_264_23; dispid 1610743867;
    procedure GhostMethod__PartitionLevel_268_24; dispid 1610743868;
    property OrdinalPosition: Smallint readonly dispid 1745027191;
    property IsValid: WordBool readonly dispid 1745027190;
    property EstimatedSize: Integer dispid 1745027189;
    property IsUnique: WordBool dispid 1745027188;
    property IsVisible: WordBool dispid 1745027187;
    property ParentKeyColumn: WideString dispid 1745027186;
    property LevelNamingTemplate: WideString dispid 1745027185;
    property HideMemberIf: HideIfValues dispid 1745027184;
    property OrderingMemberProperty: WideString dispid 1745027183;
    property CustomRollupExpression: WideString dispid 1745027182;
    property Grouping: GroupingValues dispid 1745027181;
    property SkippedLevelsColumn: WideString dispid 1745027180;
    property AreMemberKeysUnique: WordBool dispid 1745027179;
    property AreMemberNamesUnique: WordBool dispid 1745027178;
    property CustomRollupColumn: WideString dispid 1745027177;
    property CustomRollupPropertiesColumn: WideString dispid 1745027176;
    property RootMemberIf: RootIfValues dispid 1745027175;
    property UnaryOperatorColumn: WideString dispid 1745027174;
  end;

// *********************************************************************//
// Interface: _Level
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC581C-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Level = interface(IDispatch)
    ['{E8AC581C-7127-11D2-8A35-00C04FB9898D}']
    function Get_CustomProperties: _Properties; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_Parent: _Dimension; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(Param1: Smallint); safecall;
    function Get_ColumnSize: Smallint; safecall;
    procedure Set_ColumnSize(Param1: Smallint); safecall;
    function Get_EstimatedSize: Integer; safecall;
    procedure Set_EstimatedSize(Param1: Integer); safecall;
    function Get_FromClause: WideString; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_LevelType: LevelTypes; safecall;
    procedure Set_LevelType(Param1: LevelTypes); safecall;
    procedure Set_MemberKeyColumn(const Param1: WideString); safecall;
    function Get_MemberKeyColumn: WideString; safecall;
    procedure Set_MemberNameColumn(const Param1: WideString); safecall;
    function Get_MemberNameColumn: WideString; safecall;
    procedure Set_IsDisabled(Param1: WordBool); safecall;
    function Get_IsDisabled: WordBool; safecall;
    procedure Set_IsUnique(Param1: WordBool); safecall;
    function Get_IsUnique: WordBool; safecall;
    procedure Set_SliceValue(const Param1: WideString); safecall;
    function Get_SliceValue: WideString; safecall;
    function Get_Ordering: OrderTypes; safecall;
    procedure Set_Ordering(Param1: OrderTypes); safecall;
    function Get_EnableAggregations: WordBool; safecall;
    procedure Set_EnableAggregations(Param1: WordBool); safecall;
    function Get_MemberProperties: _OlapCollection; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_ParentKeyColumn: WideString; safecall;
    procedure Set_ParentKeyColumn(const Param1: WideString); safecall;
    function Get_LevelNamingTemplate: WideString; safecall;
    procedure Set_LevelNamingTemplate(const Param1: WideString); safecall;
    function Get_HideMemberIf: HideIfValues; safecall;
    procedure Set_HideMemberIf(Param1: HideIfValues); safecall;
    function Get_OrderingMemberProperty: WideString; safecall;
    procedure Set_OrderingMemberProperty(const Param1: WideString); safecall;
    function Get_CustomRollupExpression: WideString; safecall;
    procedure Set_CustomRollupExpression(const Param1: WideString); safecall;
    function Get_Grouping: GroupingValues; safecall;
    procedure Set_Grouping(Param1: GroupingValues); safecall;
    procedure Set_SkippedLevelsColumn(const Param1: WideString); safecall;
    function Get_SkippedLevelsColumn: WideString; safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_CustomRollupColumn: WideString; safecall;
    procedure Set_CustomRollupColumn(const Param1: WideString); safecall;
    function Get_CustomRollupPropertiesColumn: WideString; safecall;
    procedure Set_CustomRollupPropertiesColumn(const Param1: WideString); safecall;
    function Get_RootMemberIf: RootIfValues; safecall;
    procedure Set_RootMemberIf(Param1: RootIfValues); safecall;
    function Get_UnaryOperatorColumn: WideString; safecall;
    procedure Set_UnaryOperatorColumn(const Param1: WideString); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property Parent: _Dimension read Get_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property IsValid: WordBool read Get_IsValid;
    property Description: WideString read Get_Description write Set_Description;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property EstimatedSize: Integer read Get_EstimatedSize write Set_EstimatedSize;
    property FromClause: WideString read Get_FromClause;
    property JoinClause: WideString read Get_JoinClause;
    property LevelType: LevelTypes read Get_LevelType write Set_LevelType;
    property MemberKeyColumn: WideString read Get_MemberKeyColumn write Set_MemberKeyColumn;
    property MemberNameColumn: WideString read Get_MemberNameColumn write Set_MemberNameColumn;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property IsUnique: WordBool read Get_IsUnique write Set_IsUnique;
    property SliceValue: WideString read Get_SliceValue write Set_SliceValue;
    property Ordering: OrderTypes read Get_Ordering write Set_Ordering;
    property EnableAggregations: WordBool read Get_EnableAggregations write Set_EnableAggregations;
    property MemberProperties: _OlapCollection read Get_MemberProperties;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property ParentKeyColumn: WideString read Get_ParentKeyColumn write Set_ParentKeyColumn;
    property LevelNamingTemplate: WideString read Get_LevelNamingTemplate write Set_LevelNamingTemplate;
    property HideMemberIf: HideIfValues read Get_HideMemberIf write Set_HideMemberIf;
    property OrderingMemberProperty: WideString read Get_OrderingMemberProperty write Set_OrderingMemberProperty;
    property CustomRollupExpression: WideString read Get_CustomRollupExpression write Set_CustomRollupExpression;
    property Grouping: GroupingValues read Get_Grouping write Set_Grouping;
    property SkippedLevelsColumn: WideString read Get_SkippedLevelsColumn write Set_SkippedLevelsColumn;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property CustomRollupColumn: WideString read Get_CustomRollupColumn write Set_CustomRollupColumn;
    property CustomRollupPropertiesColumn: WideString read Get_CustomRollupPropertiesColumn write Set_CustomRollupPropertiesColumn;
    property RootMemberIf: RootIfValues read Get_RootMemberIf write Set_RootMemberIf;
    property UnaryOperatorColumn: WideString read Get_UnaryOperatorColumn write Set_UnaryOperatorColumn;
  end;

// *********************************************************************//
// DispIntf:  _LevelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC581C-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _LevelDisp = dispinterface
    ['{E8AC581C-7127-11D2-8A35-00C04FB9898D}']
    property CustomProperties: _Properties readonly dispid 1745027093;
    property OrdinalPosition: Smallint readonly dispid 1745027092;
    property Parent: _Dimension readonly dispid 1745027091;
    property Name: WideString dispid 1745027090;
    property IsValid: WordBool readonly dispid 1745027089;
    property Description: WideString dispid 1745027088;
    property ClassType: ClassTypes readonly dispid 1745027087;
    property SubClassType: SubClassTypes readonly dispid 1745027086;
    property ColumnType: Smallint dispid 1745027085;
    property ColumnSize: Smallint dispid 1745027084;
    property EstimatedSize: Integer dispid 1745027083;
    property FromClause: WideString readonly dispid 1745027082;
    property JoinClause: WideString readonly dispid 1745027081;
    property LevelType: LevelTypes dispid 1745027080;
    property MemberKeyColumn: WideString dispid 1745027079;
    property MemberNameColumn: WideString dispid 1745027078;
    property IsDisabled: WordBool dispid 1745027077;
    property IsUnique: WordBool dispid 1745027076;
    property SliceValue: WideString dispid 1745027075;
    property Ordering: OrderTypes dispid 1745027074;
    property EnableAggregations: WordBool dispid 1745027073;
    property MemberProperties: _OlapCollection readonly dispid 1745027072;
    property IsVisible: WordBool dispid 1745027107;
    property ParentKeyColumn: WideString dispid 1745027106;
    property LevelNamingTemplate: WideString dispid 1745027105;
    property HideMemberIf: HideIfValues dispid 1745027104;
    property OrderingMemberProperty: WideString dispid 1745027103;
    property CustomRollupExpression: WideString dispid 1745027102;
    property Grouping: GroupingValues dispid 1745027101;
    property SkippedLevelsColumn: WideString dispid 1745027100;
    property AreMemberKeysUnique: WordBool dispid 1745027099;
    property AreMemberNamesUnique: WordBool dispid 1745027098;
    property CustomRollupColumn: WideString dispid 1745027097;
    property CustomRollupPropertiesColumn: WideString dispid 1745027096;
    property RootMemberIf: RootIfValues dispid 1745027095;
    property UnaryOperatorColumn: WideString dispid 1745027094;
  end;

// *********************************************************************//
// Interface: _PartitionDimension
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5833-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionDimension = interface(IDispatch)
    ['{E8AC5833-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionDimension_28_1; safecall;
    procedure GhostMethod__PartitionDimension_32_2; safecall;
    procedure GhostMethod__PartitionDimension_36_3; safecall;
    procedure GhostMethod__PartitionDimension_40_4; safecall;
    procedure GhostMethod__PartitionDimension_44_5; safecall;
    procedure GhostMethod__PartitionDimension_48_6; safecall;
    procedure GhostMethod__PartitionDimension_52_7; safecall;
    procedure GhostMethod__PartitionDimension_56_8; safecall;
    procedure GhostMethod__PartitionDimension_60_9; safecall;
    procedure GhostMethod__PartitionDimension_64_10; safecall;
    procedure GhostMethod__PartitionDimension_68_11; safecall;
    procedure GhostMethod__PartitionDimension_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Path: WideString; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _Partition); safecall;
    function Get_Parent: _Partition; safecall;
    function Get_Levels: _OlapCollection; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Num: Smallint; safecall;
    function Validate: ValidateErrorCodes; safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_FromClause: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_DimensionType: DimensionTypes; safecall;
    function Get_Huge: Smallint; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_AggregationUsage: DimensionAggUsageTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDimension: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__PartitionDimension_188_13; safecall;
    procedure GhostMethod__PartitionDimension_192_14; safecall;
    procedure GhostMethod__PartitionDimension_196_15; safecall;
    procedure GhostMethod__PartitionDimension_200_16; safecall;
    procedure GhostMethod__PartitionDimension_204_17; safecall;
    procedure GhostMethod__PartitionDimension_208_18; safecall;
    procedure GhostMethod__PartitionDimension_212_19; safecall;
    procedure GhostMethod__PartitionDimension_216_20; safecall;
    procedure GhostMethod__PartitionDimension_220_21; safecall;
    procedure GhostMethod__PartitionDimension_224_22; safecall;
    procedure GhostMethod__PartitionDimension_228_23; safecall;
    procedure GhostMethod__PartitionDimension_232_24; safecall;
    function Get_IsTemporary: WordBool; safecall;
    procedure GhostMethod__PartitionDimension_240_25; safecall;
    procedure GhostMethod__PartitionDimension_244_26; safecall;
    procedure GhostMethod__PartitionDimension_248_27; safecall;
    procedure GhostMethod__PartitionDimension_252_28; safecall;
    procedure GhostMethod__PartitionDimension_256_29; safecall;
    procedure GhostMethod__PartitionDimension_260_30; safecall;
    procedure GhostMethod__PartitionDimension_264_31; safecall;
    procedure GhostMethod__PartitionDimension_268_32; safecall;
    procedure GhostMethod__PartitionDimension_272_33; safecall;
    procedure GhostMethod__PartitionDimension_276_34; safecall;
    procedure GhostMethod__PartitionDimension_280_35; safecall;
    procedure GhostMethod__PartitionDimension_284_36; safecall;
    function Get_SourceTableAlias: WideString; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_IsShared: WordBool; safecall;
    function Get_State: OlapStateTypes; safecall;
    procedure Set_DimensionType(Param1: DimensionTypes); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_DataSource: _DataSource; safecall;
    procedure _Set_DataSource(const Param1: _DataSource); safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    procedure Set_AggregationUsage(Param1: DimensionAggUsageTypes); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_IsChanging: WordBool; safecall;
    procedure Set_IsChanging(Param1: WordBool); safecall;
    function Get_StorageMode: StorageModeValues; safecall;
    procedure Set_StorageMode(Param1: StorageModeValues); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_DependsOnDimension: WideString; safecall;
    procedure Set_DependsOnDimension(const Param1: WideString); safecall;
    function Get_DefaultMember: WideString; safecall;
    procedure Set_DefaultMember(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_IsVirtual: WordBool; safecall;
    procedure Set_IsVirtual(Param1: WordBool); safecall;
    function Get_MembersWithData: MembersWithDataValues; safecall;
    procedure Set_MembersWithData(Param1: MembersWithDataValues); safecall;
    function Get_DataMemberCaptionTemplate: WideString; safecall;
    procedure Set_DataMemberCaptionTemplate(const Param1: WideString); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); safecall;
    function Get_AllowSiblingsWithSameName: WordBool; safecall;
    procedure Set_AllowSiblingsWithSameName(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Cube: _Cube read Get_Cube;
    property Path: WideString read Get_Path;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Partition read Get_Parent write _Set_Parent;
    property Levels: _OlapCollection read Get_Levels;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num;
    property SourceTable: WideString read Get_SourceTable;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property DimensionType: DimensionTypes read Get_DimensionType write Set_DimensionType;
    property Huge: Smallint read Get_Huge;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property AggregationUsage: DimensionAggUsageTypes read Get_AggregationUsage write Set_AggregationUsage;
    property IObject: IDispatch read Get_IObject;
    property IDimension: IDispatch read Get_IDimension;
    property IsTemporary: WordBool read Get_IsTemporary;
    property SourceTableAlias: WideString read Get_SourceTableAlias;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property IsShared: WordBool read Get_IsShared;
    property State: OlapStateTypes read Get_State;
    property DataSource: _DataSource read Get_DataSource write _Set_DataSource;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property IsChanging: WordBool read Get_IsChanging write Set_IsChanging;
    property StorageMode: StorageModeValues read Get_StorageMode write Set_StorageMode;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property DependsOnDimension: WideString read Get_DependsOnDimension write Set_DependsOnDimension;
    property DefaultMember: WideString read Get_DefaultMember write Set_DefaultMember;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property IsVirtual: WordBool read Get_IsVirtual write Set_IsVirtual;
    property MembersWithData: MembersWithDataValues read Get_MembersWithData write Set_MembersWithData;
    property DataMemberCaptionTemplate: WideString read Get_DataMemberCaptionTemplate write Set_DataMemberCaptionTemplate;
    property AllowSiblingsWithSameName: WordBool read Get_AllowSiblingsWithSameName write Set_AllowSiblingsWithSameName;
  end;

// *********************************************************************//
// DispIntf:  _PartitionDimensionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5833-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionDimensionDisp = dispinterface
    ['{E8AC5833-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__PartitionDimension_28_1; dispid 1610743808;
    procedure GhostMethod__PartitionDimension_32_2; dispid 1610743809;
    procedure GhostMethod__PartitionDimension_36_3; dispid 1610743810;
    procedure GhostMethod__PartitionDimension_40_4; dispid 1610743811;
    procedure GhostMethod__PartitionDimension_44_5; dispid 1610743812;
    procedure GhostMethod__PartitionDimension_48_6; dispid 1610743813;
    procedure GhostMethod__PartitionDimension_52_7; dispid 1610743814;
    procedure GhostMethod__PartitionDimension_56_8; dispid 1610743815;
    procedure GhostMethod__PartitionDimension_60_9; dispid 1610743816;
    procedure GhostMethod__PartitionDimension_64_10; dispid 1610743817;
    procedure GhostMethod__PartitionDimension_68_11; dispid 1610743818;
    procedure GhostMethod__PartitionDimension_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027147;
    property Description: WideString dispid 1745027143;
    property Valid: WordBool dispid 1745027141;
    property Cube: _Cube readonly dispid 1745027140;
    property Path: WideString readonly dispid 1745027139;
    property Database: _Database readonly dispid 1745027137;
    property Server: _Server readonly dispid 1745027136;
    property Parent: _Partition dispid 1745027135;
    property Levels: _OlapCollection readonly dispid 1745027133;
    property Name: WideString dispid 1745027132;
    property Num: Smallint readonly dispid 1745027131;
    function Validate: ValidateErrorCodes; dispid 1610809428;
    property SourceTable: WideString readonly dispid 1745027130;
    property JoinClause: WideString dispid 1745027129;
    property FromClause: WideString dispid 1745027128;
    property ClassType: Smallint readonly dispid 1745027127;
    property SubClassType: SubClassTypes readonly dispid 1745027126;
    property DimensionType: DimensionTypes dispid 1745027125;
    property Huge: Smallint readonly dispid 1745027124;
    property LastUpdated: TDateTime dispid 1745027123;
    property LastProcessed: TDateTime readonly dispid 1745027122;
    property AggregationUsage: DimensionAggUsageTypes dispid 1745027121;
    property IObject: IDispatch readonly dispid 1745027120;
    property IDimension: IDispatch readonly dispid 1745027119;
    procedure ClearCollections; dispid 1610809434;
    procedure GhostMethod__PartitionDimension_188_13; dispid 1610743848;
    procedure GhostMethod__PartitionDimension_192_14; dispid 1610743849;
    procedure GhostMethod__PartitionDimension_196_15; dispid 1610743850;
    procedure GhostMethod__PartitionDimension_200_16; dispid 1610743851;
    procedure GhostMethod__PartitionDimension_204_17; dispid 1610743852;
    procedure GhostMethod__PartitionDimension_208_18; dispid 1610743853;
    procedure GhostMethod__PartitionDimension_212_19; dispid 1610743854;
    procedure GhostMethod__PartitionDimension_216_20; dispid 1610743855;
    procedure GhostMethod__PartitionDimension_220_21; dispid 1610743856;
    procedure GhostMethod__PartitionDimension_224_22; dispid 1610743857;
    procedure GhostMethod__PartitionDimension_228_23; dispid 1610743858;
    procedure GhostMethod__PartitionDimension_232_24; dispid 1610743859;
    property IsTemporary: WordBool readonly dispid 1745027211;
    procedure GhostMethod__PartitionDimension_240_25; dispid 1610743861;
    procedure GhostMethod__PartitionDimension_244_26; dispid 1610743862;
    procedure GhostMethod__PartitionDimension_248_27; dispid 1610743863;
    procedure GhostMethod__PartitionDimension_252_28; dispid 1610743864;
    procedure GhostMethod__PartitionDimension_256_29; dispid 1610743865;
    procedure GhostMethod__PartitionDimension_260_30; dispid 1610743866;
    procedure GhostMethod__PartitionDimension_264_31; dispid 1610743867;
    procedure GhostMethod__PartitionDimension_268_32; dispid 1610743868;
    procedure GhostMethod__PartitionDimension_272_33; dispid 1610743869;
    procedure GhostMethod__PartitionDimension_276_34; dispid 1610743870;
    procedure GhostMethod__PartitionDimension_280_35; dispid 1610743871;
    procedure GhostMethod__PartitionDimension_284_36; dispid 1610743872;
    property SourceTableAlias: WideString readonly dispid 1745027317;
    property EnableRealTimeUpdates: WordBool dispid 1745027316;
    property OrdinalPosition: Smallint readonly dispid 1745027229;
    property IsValid: WordBool readonly dispid 1745027228;
    property IsShared: WordBool readonly dispid 1745027227;
    property State: OlapStateTypes readonly dispid 1745027226;
    property DataSource: _DataSource dispid 1745027225;
    property IsVisible: WordBool dispid 1745027224;
    property IsChanging: WordBool dispid 1745027223;
    property StorageMode: StorageModeValues dispid 1745027222;
    property IsReadWrite: WordBool dispid 1745027221;
    property DependsOnDimension: WideString dispid 1745027220;
    property DefaultMember: WideString dispid 1745027219;
    property SourceTableFilter: WideString dispid 1745027218;
    property AreMemberNamesUnique: WordBool dispid 1745027217;
    property AreMemberKeysUnique: WordBool dispid 1745027216;
    property IsVirtual: WordBool dispid 1745027215;
    property MembersWithData: MembersWithDataValues dispid 1745027214;
    property DataMemberCaptionTemplate: WideString dispid 1745027213;
    procedure Process(Options: ProcessTypes); dispid 1610809615;
    procedure Update; dispid 1610809616;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809617;
    procedure UnlockObject; dispid 1610809618;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); dispid 1610809619;
    property AllowSiblingsWithSameName: WordBool dispid 1745027212;
  end;

// *********************************************************************//
// Interface: _Dimension
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5819-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Dimension = interface(IDispatch)
    ['{E8AC5819-7127-11D2-8A35-00C04FB9898D}']
    function Get_Levels: _OlapCollection; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_LastUpdated: TDateTime; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_IsShared: WordBool; safecall;
    function Get_State: OlapStateTypes; safecall;
    procedure Set_DimensionType(Param1: DimensionTypes); safecall;
    function Get_DimensionType: DimensionTypes; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure _Set_DataSource(const Param1: _DataSource); safecall;
    function Get_DataSource: _DataSource; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_AggregationUsage: DimensionAggUsageTypes; safecall;
    procedure Set_AggregationUsage(Param1: DimensionAggUsageTypes); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_IsChanging: WordBool; safecall;
    procedure Set_IsChanging(Param1: WordBool); safecall;
    function Get_StorageMode: StorageModeValues; safecall;
    procedure Set_StorageMode(Param1: StorageModeValues); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_DependsOnDimension: WideString; safecall;
    procedure Set_DependsOnDimension(const Param1: WideString); safecall;
    function Get_DefaultMember: WideString; safecall;
    procedure Set_DefaultMember(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_IsVirtual: WordBool; safecall;
    procedure Set_IsVirtual(Param1: WordBool); safecall;
    function Get_MembersWithData: MembersWithDataValues; safecall;
    procedure Set_MembersWithData(Param1: MembersWithDataValues); safecall;
    function Get_DataMemberCaptionTemplate: WideString; safecall;
    procedure Set_DataMemberCaptionTemplate(const Param1: WideString); safecall;
    function Get_AllowSiblingsWithSameName: WordBool; safecall;
    procedure Set_AllowSiblingsWithSameName(Param1: WordBool); safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    property Levels: _OlapCollection read Get_Levels;
    property CustomProperties: _Properties read Get_CustomProperties;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property SourceTable: WideString read Get_SourceTable;
    property Parent: _MDStore read Get_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property IsValid: WordBool read Get_IsValid;
    property IsShared: WordBool read Get_IsShared;
    property State: OlapStateTypes read Get_State;
    property DimensionType: DimensionTypes read Get_DimensionType write Set_DimensionType;
    property Description: WideString read Get_Description write Set_Description;
    property DataSource: _DataSource read Get_DataSource write _Set_DataSource;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IsTemporary: WordBool read Get_IsTemporary;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property AggregationUsage: DimensionAggUsageTypes read Get_AggregationUsage write Set_AggregationUsage;
    property SourceTableAlias: WideString read Get_SourceTableAlias;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property IsChanging: WordBool read Get_IsChanging write Set_IsChanging;
    property StorageMode: StorageModeValues read Get_StorageMode write Set_StorageMode;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property DependsOnDimension: WideString read Get_DependsOnDimension write Set_DependsOnDimension;
    property DefaultMember: WideString read Get_DefaultMember write Set_DefaultMember;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property IsVirtual: WordBool read Get_IsVirtual write Set_IsVirtual;
    property MembersWithData: MembersWithDataValues read Get_MembersWithData write Set_MembersWithData;
    property DataMemberCaptionTemplate: WideString read Get_DataMemberCaptionTemplate write Set_DataMemberCaptionTemplate;
    property AllowSiblingsWithSameName: WordBool read Get_AllowSiblingsWithSameName write Set_AllowSiblingsWithSameName;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
  end;

// *********************************************************************//
// DispIntf:  _DimensionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5819-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DimensionDisp = dispinterface
    ['{E8AC5819-7127-11D2-8A35-00C04FB9898D}']
    property Levels: _OlapCollection readonly dispid 1745027091;
    property CustomProperties: _Properties readonly dispid 1745027090;
    property OrdinalPosition: Smallint readonly dispid 1745027089;
    property SourceTable: WideString readonly dispid 1745027088;
    property Parent: _MDStore readonly dispid 1745027087;
    property Name: WideString dispid 1745027086;
    property LastUpdated: TDateTime dispid 1745027085;
    property LastProcessed: TDateTime readonly dispid 1745027084;
    property IsValid: WordBool readonly dispid 1745027083;
    property IsShared: WordBool readonly dispid 1745027082;
    property State: OlapStateTypes readonly dispid 1745027081;
    property DimensionType: DimensionTypes dispid 1745027080;
    property Description: WideString dispid 1745027079;
    property DataSource: _DataSource dispid 1745027078;
    property ClassType: ClassTypes readonly dispid 1745027077;
    property SubClassType: SubClassTypes readonly dispid 1745027076;
    property IsTemporary: WordBool readonly dispid 1745027075;
    property JoinClause: WideString dispid 1745027074;
    property FromClause: WideString dispid 1745027073;
    property AggregationUsage: DimensionAggUsageTypes dispid 1745027072;
    procedure Process(Options: ProcessTypes); dispid 1610809364;
    procedure Update; dispid 1610809365;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809366;
    procedure UnlockObject; dispid 1610809367;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); dispid 1610809368;
    property SourceTableAlias: WideString readonly dispid 1745027111;
    property IsVisible: WordBool dispid 1745027110;
    property IsChanging: WordBool dispid 1745027109;
    property StorageMode: StorageModeValues dispid 1745027108;
    property IsReadWrite: WordBool dispid 1745027107;
    property DependsOnDimension: WideString dispid 1745027106;
    property DefaultMember: WideString dispid 1745027105;
    property SourceTableFilter: WideString dispid 1745027104;
    property AreMemberNamesUnique: WordBool dispid 1745027103;
    property AreMemberKeysUnique: WordBool dispid 1745027102;
    property IsVirtual: WordBool dispid 1745027101;
    property MembersWithData: MembersWithDataValues dispid 1745027100;
    property DataMemberCaptionTemplate: WideString dispid 1745027099;
    property AllowSiblingsWithSameName: WordBool dispid 1745027098;
    property EnableRealTimeUpdates: WordBool dispid 1745027097;
  end;

// *********************************************************************//
// Interface: _Command
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5811-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Command = interface(IDispatch)
    ['{E8AC5811-7127-11D2-8A35-00C04FB9898D}']
    function Get_CustomProperties: _Properties; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_CommandType: CommandTypes; safecall;
    procedure Set_CommandType(Param1: CommandTypes); safecall;
    procedure Set_Statement(const Param1: WideString); safecall;
    function Get_Statement: WideString; safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); safecall;
    function Get_ParentObject: IDispatch; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property Parent: _MDStore read Get_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property IsValid: WordBool read Get_IsValid;
    property Description: WideString read Get_Description write Set_Description;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property CommandType: CommandTypes read Get_CommandType write Set_CommandType;
    property Statement: WideString read Get_Statement write Set_Statement;
    property ParentObject: IDispatch read Get_ParentObject;
  end;

// *********************************************************************//
// DispIntf:  _CommandDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5811-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CommandDisp = dispinterface
    ['{E8AC5811-7127-11D2-8A35-00C04FB9898D}']
    property CustomProperties: _Properties readonly dispid 1745027081;
    property OrdinalPosition: Smallint readonly dispid 1745027080;
    property Parent: _MDStore readonly dispid 1745027079;
    property Name: WideString dispid 1745027078;
    property IsValid: WordBool readonly dispid 1745027077;
    property Description: WideString dispid 1745027076;
    property ClassType: ClassTypes readonly dispid 1745027075;
    property SubClassType: SubClassTypes readonly dispid 1745027074;
    property CommandType: CommandTypes dispid 1745027073;
    property Statement: WideString dispid 1745027072;
    procedure Update; dispid 1610809354;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809355;
    procedure UnlockObject; dispid 1610809356;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); dispid 1610809357;
    property ParentObject: IDispatch readonly dispid 1745027086;
  end;

// *********************************************************************//
// Interface: _Measure
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5808-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Measure = interface(IDispatch)
    ['{E8AC5808-7127-11D2-8A35-00C04FB9898D}']
    function Get_CustomProperties: _Properties; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_AggregateFunction: AggregatesTypes; safecall;
    procedure Set_AggregateFunction(Param1: AggregatesTypes); safecall;
    function Get_FormatString: WideString; safecall;
    procedure Set_FormatString(const Param1: WideString); safecall;
    function Get_IsInternal: WordBool; safecall;
    procedure Set_IsInternal(Param1: WordBool); safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_SourceColumn(const Param1: WideString); safecall;
    function Get_SourceColumnType: Smallint; safecall;
    procedure Set_SourceColumnType(Param1: Smallint); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property Parent: _MDStore read Get_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property IsValid: WordBool read Get_IsValid;
    property Description: WideString read Get_Description write Set_Description;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property AggregateFunction: AggregatesTypes read Get_AggregateFunction write Set_AggregateFunction;
    property FormatString: WideString read Get_FormatString write Set_FormatString;
    property IsInternal: WordBool read Get_IsInternal write Set_IsInternal;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property SourceColumnType: Smallint read Get_SourceColumnType write Set_SourceColumnType;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
  end;

// *********************************************************************//
// DispIntf:  _MeasureDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5808-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _MeasureDisp = dispinterface
    ['{E8AC5808-7127-11D2-8A35-00C04FB9898D}']
    property CustomProperties: _Properties readonly dispid 1745027084;
    property OrdinalPosition: Smallint readonly dispid 1745027083;
    property Parent: _MDStore readonly dispid 1745027082;
    property Name: WideString dispid 1745027081;
    property IsValid: WordBool readonly dispid 1745027080;
    property Description: WideString dispid 1745027079;
    property ClassType: ClassTypes readonly dispid 1745027078;
    property SubClassType: SubClassTypes readonly dispid 1745027077;
    property AggregateFunction: AggregatesTypes dispid 1745027076;
    property FormatString: WideString dispid 1745027075;
    property IsInternal: WordBool dispid 1745027074;
    property SourceColumn: WideString dispid 1745027073;
    property SourceColumnType: Smallint dispid 1745027072;
    property IsVisible: WordBool dispid 1745027085;
  end;

// *********************************************************************//
// Interface: _Role
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580F-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Role = interface(IDispatch)
    ['{E8AC580F-7127-11D2-8A35-00C04FB9898D}']
    function Get_CustomProperties: _Properties; safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_Permissions(const Key: WideString): WideString; safecall;
    procedure Set_UsersList(const Param1: WideString); safecall;
    function Get_UsersList: WideString; safecall;
    procedure Update; safecall;
    function SetPermissions(const Key: WideString; const PermissionExpression: WideString): WordBool; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_ParentObject: IDispatch; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Parent: _MDStore read Get_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property IsValid: WordBool read Get_IsValid;
    property Description: WideString read Get_Description write Set_Description;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property Permissions[const Key: WideString]: WideString read Get_Permissions;
    property UsersList: WideString read Get_UsersList write Set_UsersList;
    property Commands: _OlapCollection read Get_Commands;
    property ParentObject: IDispatch read Get_ParentObject;
  end;

// *********************************************************************//
// DispIntf:  _RoleDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580F-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _RoleDisp = dispinterface
    ['{E8AC580F-7127-11D2-8A35-00C04FB9898D}']
    property CustomProperties: _Properties readonly dispid 1745027080;
    property Parent: _MDStore readonly dispid 1745027079;
    property Name: WideString dispid 1745027078;
    property IsValid: WordBool readonly dispid 1745027077;
    property Description: WideString dispid 1745027076;
    property ClassType: ClassTypes readonly dispid 1745027075;
    property SubClassType: SubClassTypes readonly dispid 1745027074;
    property Permissions[const Key: WideString]: WideString readonly dispid 1745027073;
    property UsersList: WideString dispid 1745027072;
    procedure Update; dispid 1610809353;
    function SetPermissions(const Key: WideString; const PermissionExpression: WideString): WordBool; dispid 1610809354;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809355;
    procedure UnlockObject; dispid 1610809356;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); dispid 1610809357;
    property Commands: _OlapCollection readonly dispid 1745027087;
    property ParentObject: IDispatch readonly dispid 1745027086;
  end;

// *********************************************************************//
// Interface: _Properties
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC57FF-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Properties = interface(IDispatch)
    ['{E8AC57FF-7127-11D2-8A35-00C04FB9898D}']
    function Add(Value: OleVariant; const Name: WideString; DataType: VbVarType): _Property; safecall;
    function Get_Item(Index: OleVariant): _Property; safecall;
    function Get_Count: Integer; safecall;
    procedure Remove(Index: OleVariant); safecall;
    procedure Clear; safecall;
    function Get_NewEnum: IUnknown; safecall;
    property Item[Index: OleVariant]: _Property read Get_Item; default;
    property Count: Integer read Get_Count;
    property NewEnum: IUnknown read Get_NewEnum;
  end;

// *********************************************************************//
// DispIntf:  _PropertiesDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC57FF-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PropertiesDisp = dispinterface
    ['{E8AC57FF-7127-11D2-8A35-00C04FB9898D}']
    function Add(Value: OleVariant; const Name: WideString; DataType: VbVarType): _Property; dispid 1610809345;
    property Item[Index: OleVariant]: _Property readonly dispid 0; default;
    property Count: Integer readonly dispid 1745027072;
    procedure Remove(Index: OleVariant); dispid 1610809346;
    procedure Clear; dispid 1610809347;
    property NewEnum: IUnknown readonly dispid -4;
  end;

// *********************************************************************//
// Interface: _Property
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC57FE-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Property = interface(IDispatch)
    ['{E8AC57FE-7127-11D2-8A35-00C04FB9898D}']
    function Get_Name: WideString; safecall;
    function Get_DataType: VbVarType; safecall;
    function Get_Value: OleVariant; safecall;
    procedure Set_Value(Param1: OleVariant); safecall;
    procedure _Set_Value(Param1: OleVariant); safecall;
    property Name: WideString read Get_Name;
    property DataType: VbVarType read Get_DataType;
    property Value: OleVariant read Get_Value write Set_Value;
  end;

// *********************************************************************//
// DispIntf:  _PropertyDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC57FE-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PropertyDisp = dispinterface
    ['{E8AC57FE-7127-11D2-8A35-00C04FB9898D}']
    property Name: WideString readonly dispid 1745027074;
    property DataType: VbVarType readonly dispid 1745027073;
    property Value: OleVariant dispid 1745027072;
  end;

// *********************************************************************//
// Interface: _CubeAnalyzer
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5810-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeAnalyzer = interface(IDispatch)
    ['{E8AC5810-7127-11D2-8A35-00C04FB9898D}']
    procedure CreateRandomRequests; safecall;
    function OpenQueryLogRecordset(var SQLString: WideString): _Recordset; safecall;
  end;

// *********************************************************************//
// DispIntf:  _CubeAnalyzerDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5810-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeAnalyzerDisp = dispinterface
    ['{E8AC5810-7127-11D2-8A35-00C04FB9898D}']
    procedure CreateRandomRequests; dispid 1610809346;
    function OpenQueryLogRecordset(var SQLString: WideString): _Recordset; dispid 1610809347;
  end;

// *********************************************************************//
// Interface: _PartitionAnalyzer
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5814-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionAnalyzer = interface(IDispatch)
    ['{E8AC5814-7127-11D2-8A35-00C04FB9898D}']
    function Get_Parent: _MDStore; safecall;
    procedure CloseAggregationsAnalysis; safecall;
    function Get_DesignedAggregations: _Collection; safecall;
    procedure InitializeDesign(OlapMode: OleVariant); safecall;
    function Get_AggregationAnalysisInitialized: WordBool; safecall;
    procedure AddExisitingAggregation(const agg: _MDStore; var PercentageBenefit: Double; 
                                      var AccumulatedSize: Double; var AggregationsCount: Integer); safecall;
    function NextAnalysisStep(var PercentageBenefit: Double; var AccumulatedSize: Double; 
                              var AggregationsCount: Integer): WordBool; safecall;
    procedure AddGoalQuery(const DatasetName: WideString; Frequency: Double); safecall;
    procedure PrepareGoalQueries; safecall;
    property Parent: _MDStore read Get_Parent;
    property DesignedAggregations: _Collection read Get_DesignedAggregations;
    property AggregationAnalysisInitialized: WordBool read Get_AggregationAnalysisInitialized;
  end;

// *********************************************************************//
// DispIntf:  _PartitionAnalyzerDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5814-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionAnalyzerDisp = dispinterface
    ['{E8AC5814-7127-11D2-8A35-00C04FB9898D}']
    property Parent: _MDStore readonly dispid 1745027076;
    procedure CloseAggregationsAnalysis; dispid 1610809349;
    property DesignedAggregations: _Collection readonly dispid 1745027073;
    procedure InitializeDesign(OlapMode: OleVariant); dispid 1610809361;
    property AggregationAnalysisInitialized: WordBool readonly dispid 1745027072;
    procedure AddExisitingAggregation(const agg: _MDStore; var PercentageBenefit: Double; 
                                      var AccumulatedSize: Double; var AggregationsCount: Integer); dispid 1610809377;
    function NextAnalysisStep(var PercentageBenefit: Double; var AccumulatedSize: Double; 
                              var AggregationsCount: Integer): WordBool; dispid 1610809378;
    procedure AddGoalQuery(const DatasetName: WideString; Frequency: Double); dispid 1610809381;
    procedure PrepareGoalQueries; dispid 1610809382;
  end;

// *********************************************************************//
// Interface: _IROLAPProvider
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5815-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _IROLAPProvider = interface(IDispatch)
    ['{E8AC5815-7127-11D2-8A35-00C04FB9898D}']
    function MatchingConnection(var Connection: _Connection): WordBool; safecall;
    procedure AfterOpenConnection(var Connection: _Connection); safecall;
    procedure CloseConnection(var Connection: _Connection); safecall;
    function Get_SupportLevel(var Connection: _Connection): SupportLevels; safecall;
    function Get_OpenQuoteChar(var Connection: _Connection): WideString; safecall;
    function Get_CloseQuoteChar(var Connection: _Connection): WideString; safecall;
    function ExtractTableNameFromExpression(var ds: _DataSource; var Expr: WideString; 
                                            var TableName: WideString; var errorCode: Smallint): Smallint; safecall;
    procedure AfterBeginTransaction(var Connection: _Connection); safecall;
    procedure BeforeCommitTransaction(var Connection: _Connection); safecall;
    procedure BeforeRollbackTransaction(var Connection: _Connection); safecall;
    procedure ProcessROLAPPartition(var Connection: _Connection; var Partition: _MDStore; 
                                    var Events: _IDatabaseEvents); safecall;
    procedure MergeROLAPPartitions(var Connection: _Connection; var SourcePartition: _MDStore; 
                                   var TargetPartition: _MDStore; var Events: _IDatabaseEvents); safecall;
    procedure DeleteOldAggregations(var Connection: _Connection; var Partition: _MDStore; 
                                    var TableNames: _Collection; var Events: _IDatabaseEvents); safecall;
    procedure CreateAggregationTable(var Connection: _Connection; 
                                     var SourceAggregation: _Aggregation; 
                                     var Options: ProcessTypes; var Events: _IDatabaseEvents); safecall;
    function Get_Name: WideString; safecall;
    function GetTablesRecordset(var ActiveConnection: _Connection): _Recordset; safecall;
    function GetColumnsRecordset(var ActiveConnection: _Connection; var TableName: WideString): _Recordset; safecall;
    property SupportLevel[var Connection: _Connection]: SupportLevels read Get_SupportLevel;
    property OpenQuoteChar[var Connection: _Connection]: WideString read Get_OpenQuoteChar;
    property CloseQuoteChar[var Connection: _Connection]: WideString read Get_CloseQuoteChar;
    property Name: WideString read Get_Name;
  end;

// *********************************************************************//
// DispIntf:  _IROLAPProviderDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5815-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _IROLAPProviderDisp = dispinterface
    ['{E8AC5815-7127-11D2-8A35-00C04FB9898D}']
    function MatchingConnection(var Connection: _Connection): WordBool; dispid 1610809348;
    procedure AfterOpenConnection(var Connection: _Connection); dispid 1610809349;
    procedure CloseConnection(var Connection: _Connection); dispid 1610809350;
    property SupportLevel[var Connection: _Connection]: SupportLevels readonly dispid 1745027075;
    property OpenQuoteChar[var Connection: _Connection]: WideString readonly dispid 1745027074;
    property CloseQuoteChar[var Connection: _Connection]: WideString readonly dispid 1745027073;
    function ExtractTableNameFromExpression(var ds: _DataSource; var Expr: WideString; 
                                            var TableName: WideString; var errorCode: Smallint): Smallint; dispid 1610809351;
    procedure AfterBeginTransaction(var Connection: _Connection); dispid 1610809352;
    procedure BeforeCommitTransaction(var Connection: _Connection); dispid 1610809353;
    procedure BeforeRollbackTransaction(var Connection: _Connection); dispid 1610809354;
    procedure ProcessROLAPPartition(var Connection: _Connection; var Partition: _MDStore; 
                                    var Events: _IDatabaseEvents); dispid 1610809355;
    procedure MergeROLAPPartitions(var Connection: _Connection; var SourcePartition: _MDStore; 
                                   var TargetPartition: _MDStore; var Events: _IDatabaseEvents); dispid 1610809356;
    procedure DeleteOldAggregations(var Connection: _Connection; var Partition: _MDStore; 
                                    var TableNames: _Collection; var Events: _IDatabaseEvents); dispid 1610809357;
    procedure CreateAggregationTable(var Connection: _Connection; 
                                     var SourceAggregation: _Aggregation; 
                                     var Options: ProcessTypes; var Events: _IDatabaseEvents); dispid 1610809358;
    property Name: WideString readonly dispid 1745027072;
    function GetTablesRecordset(var ActiveConnection: _Connection): _Recordset; dispid 1610809359;
    function GetColumnsRecordset(var ActiveConnection: _Connection; var TableName: WideString): _Recordset; dispid 1610809360;
  end;

// *********************************************************************//
// DispIntf:  __IROLAPProvider
// Flags:     (4240) Hidden NonExtensible Dispatchable
// GUID:      {E8AC5817-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  __IROLAPProvider = dispinterface
    ['{E8AC5817-7127-11D2-8A35-00C04FB9898D}']
    procedure ReportBefore(var obj: IDispatch; Action: Smallint; var Cancel: WordBool; 
                           var Skip: WordBool); dispid 1;
    procedure ReportProgress(var obj: IDispatch; Action: Smallint; var Counter: Integer; 
                             var Message: WideString; var Cancel: WordBool); dispid 2;
    procedure ReportError(var obj: IDispatch; Action: Smallint; errorCode: Integer; 
                          const Message: WideString; var Cancel: WordBool); dispid 3;
    procedure ReportAfter(var obj: IDispatch; Action: Smallint; success: WordBool); dispid 4;
  end;

// *********************************************************************//
// Interface: _IDatabaseEvents
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580D-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _IDatabaseEvents = interface(IDispatch)
    ['{E8AC580D-7127-11D2-8A35-00C04FB9898D}']
    procedure RaiseReportBefore(var obj: IDispatch; var Action: Smallint; var Skip: WordBool); safecall;
    procedure RaiseReportProgress(var obj: IDispatch; var Action: Smallint; var Counter: Integer; 
                                  var Message: WideString); safecall;
    procedure RaiseReportError(var obj: IDispatch; var Action: Smallint; var errorCode: Integer; 
                               var Message: WideString); safecall;
    procedure RaiseReportWarning(var obj: IDispatch; var Action: Smallint; var errorCode: Integer; 
                                 var Message: WideString); safecall;
    procedure RaiseReportAfter(var obj: IDispatch; var Action: Smallint; var success: WordBool); safecall;
  end;

// *********************************************************************//
// DispIntf:  _IDatabaseEventsDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580D-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _IDatabaseEventsDisp = dispinterface
    ['{E8AC580D-7127-11D2-8A35-00C04FB9898D}']
    procedure RaiseReportBefore(var obj: IDispatch; var Action: Smallint; var Skip: WordBool); dispid 1610809344;
    procedure RaiseReportProgress(var obj: IDispatch; var Action: Smallint; var Counter: Integer; 
                                  var Message: WideString); dispid 1610809345;
    procedure RaiseReportError(var obj: IDispatch; var Action: Smallint; var errorCode: Integer; 
                               var Message: WideString); dispid 1610809346;
    procedure RaiseReportWarning(var obj: IDispatch; var Action: Smallint; var errorCode: Integer; 
                                 var Message: WideString); dispid 1610809347;
    procedure RaiseReportAfter(var obj: IDispatch; var Action: Smallint; var success: WordBool); dispid 1610809348;
  end;

// *********************************************************************//
// Interface: _Server
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5822-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Server = interface(IDispatch)
    ['{E8AC5822-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Server_28_1; safecall;
    procedure GhostMethod__Server_32_2; safecall;
    procedure GhostMethod__Server_36_3; safecall;
    procedure GhostMethod__Server_40_4; safecall;
    procedure GhostMethod__Server_44_5; safecall;
    procedure GhostMethod__Server_48_6; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Valid(Param1: WordBool); safecall;
    function Get_RepositoryVersion: Smallint; safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Machine: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    procedure ClearCollections; safecall;
    function Get_LockTimeout: Integer; safecall;
    procedure Set_LockTimeout(Param1: Integer); safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Refresh; safecall;
    function UnlockAllObjects: WordBool; safecall;
    function Get_State: ServerStates; safecall;
    function Get_Path: WideString; safecall;
    function Get_Name: WideString; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Parent: IDispatch; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Edition: OlapEditions; safecall;
    procedure Update; safecall;
    function Get_Timeout: Integer; safecall;
    procedure Set_Timeout(Param1: Integer); safecall;
    function Get_ClassType: Smallint; safecall;
    procedure Connect(const ServerName: WideString); safecall;
    procedure CloseServer; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ServiceState: Integer; safecall;
    procedure Set_ServiceState(Param1: Integer); safecall;
    function Get_IsValid: WordBool; safecall;
    function CreateObject(ObjectType: ClassTypes; SubClassType: SubClassTypes): IDispatch; safecall;
    procedure GhostMethod__Server_180_7; safecall;
    procedure GhostMethod__Server_184_8; safecall;
    procedure GhostMethod__Server_188_9; safecall;
    procedure GhostMethod__Server_192_10; safecall;
    procedure GhostMethod__Server_196_11; safecall;
    procedure GhostMethod__Server_200_12; safecall;
    function Get_Version: WideString; safecall;
    function Get_ConnectTimeout: Integer; safecall;
    procedure Set_ConnectTimeout(Param1: Integer); safecall;
    function Get_ProcessingLogFileName: WideString; safecall;
    procedure Set_ProcessingLogFileName(var Param1: WideString); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property RepositoryVersion: Smallint read Get_RepositoryVersion;
    property Machine: WideString read Get_Machine;
    property Name: WideString read Get_Name write Set_Name;
    property LockTimeout: Integer read Get_LockTimeout write Set_LockTimeout;
    property State: ServerStates read Get_State;
    property Path: WideString read Get_Path;
    property MDStores: _OlapCollection read Get_MDStores;
    property Parent: IDispatch read Get_Parent;
    property Description: WideString read Get_Description write Set_Description;
    property Edition: OlapEditions read Get_Edition;
    property Timeout: Integer read Get_Timeout write Set_Timeout;
    property ClassType: Smallint read Get_ClassType;
    property IObject: IDispatch read Get_IObject;
    property ServiceState: Integer read Get_ServiceState write Set_ServiceState;
    property IsValid: WordBool read Get_IsValid;
    property Version: WideString read Get_Version;
    property ConnectTimeout: Integer read Get_ConnectTimeout write Set_ConnectTimeout;
    property ProcessingLogFileName: WideString read Get_ProcessingLogFileName write Set_ProcessingLogFileName;
  end;

// *********************************************************************//
// DispIntf:  _ServerDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5822-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _ServerDisp = dispinterface
    ['{E8AC5822-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Server_28_1; dispid 1610743808;
    procedure GhostMethod__Server_32_2; dispid 1610743809;
    procedure GhostMethod__Server_36_3; dispid 1610743810;
    procedure GhostMethod__Server_40_4; dispid 1610743811;
    procedure GhostMethod__Server_44_5; dispid 1610743812;
    procedure GhostMethod__Server_48_6; dispid 1610743813;
    property CustomProperties: _Properties readonly dispid 1745027118;
    property Valid: WordBool dispid 1745027114;
    property RepositoryVersion: Smallint readonly dispid 1745027113;
    property Machine: WideString readonly dispid 1745027112;
    property Name: WideString dispid 1745027108;
    procedure ClearCollections; dispid 1610809400;
    property LockTimeout: Integer dispid 1745027107;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809401;
    procedure UnlockObject; dispid 1610809403;
    procedure Refresh; dispid 1610809404;
    function UnlockAllObjects: WordBool; dispid 1610809407;
    property State: ServerStates readonly dispid 1745027105;
    property Path: WideString readonly dispid 1745027104;
    property MDStores: _OlapCollection readonly dispid 1745027102;
    property Parent: IDispatch readonly dispid 1745027101;
    property Description: WideString dispid 1745027100;
    property Edition: OlapEditions readonly dispid 1745027099;
    procedure Update; dispid 1610809408;
    property Timeout: Integer dispid 1745027098;
    property ClassType: Smallint readonly dispid 1745027096;
    procedure Connect(const ServerName: WideString); dispid 1610809418;
    procedure CloseServer; dispid 1610809419;
    property IObject: IDispatch readonly dispid 1745027095;
    property ServiceState: Integer dispid 1745027083;
    property IsValid: WordBool readonly dispid 1745027082;
    function CreateObject(ObjectType: ClassTypes; SubClassType: SubClassTypes): IDispatch; dispid 1610809426;
    procedure GhostMethod__Server_180_7; dispid 1610743846;
    procedure GhostMethod__Server_184_8; dispid 1610743847;
    procedure GhostMethod__Server_188_9; dispid 1610743848;
    procedure GhostMethod__Server_192_10; dispid 1610743849;
    procedure GhostMethod__Server_196_11; dispid 1610743850;
    procedure GhostMethod__Server_200_12; dispid 1610743851;
    property Version: WideString readonly dispid 1745027186;
    property ConnectTimeout: Integer dispid 1745027185;
    property ProcessingLogFileName: WideString dispid 1745027167;
  end;

// *********************************************************************//
// Interface: _MDStore
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5800-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _MDStore = interface(IDispatch)
    ['{E8AC5800-7127-11D2-8A35-00C04FB9898D}']
    function Get_Commands: _OlapCollection; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_Dimensions: _OlapCollection; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Measures: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_AggregationPrefix: WideString; safecall;
    procedure Set_AggregationPrefix(const Param1: WideString); safecall;
    function Get_Analyzer: IDispatch; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_EstimatedRows: Double; safecall;
    procedure Set_EstimatedRows(Param1: Double); safecall;
    function Get_EstimatedSize: Double; safecall;
    function Get_IsDefault: WordBool; safecall;
    procedure Set_IsDefault(Param1: WordBool); safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_OlapMode: OlapStorageModes; safecall;
    procedure Set_OlapMode(Param1: OlapStorageModes); safecall;
    function Get_Parent: IDispatch; safecall;
    function Get_SourceTable: WideString; safecall;
    procedure Set_SourceTable(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_Server: _Server; safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    procedure BeginTrans; safecall;
    procedure CommitTrans; safecall;
    procedure Rollback; safecall;
    procedure Merge(const SourceName: WideString); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _MDStore; Options: CloneOptions); safecall;
    function Get_MiningModels: _OlapCollection; safecall;
    function Get_SourceTableAlias: WideString; safecall;
    procedure Set_SourceTableAlias(const Param1: WideString); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_AllowDrillThrough: WordBool; safecall;
    procedure Set_AllowDrillThrough(Param1: WordBool); safecall;
    function Get_DrillThroughColumns: WideString; safecall;
    procedure Set_DrillThroughColumns(const Param1: WideString); safecall;
    function Get_DrillThroughFilter: WideString; safecall;
    procedure Set_DrillThroughFilter(const Param1: WideString); safecall;
    function Get_RemoteServer: WideString; safecall;
    procedure Set_RemoteServer(const Param1: WideString); safecall;
    function Get_DrillThroughFrom: WideString; safecall;
    procedure Set_DrillThroughFrom(const Param1: WideString); safecall;
    function Get_DrillThroughJoins: WideString; safecall;
    procedure Set_DrillThroughJoins(const Param1: WideString); safecall;
    function Get_DefaultMeasure: WideString; safecall;
    procedure Set_DefaultMeasure(const Param1: WideString); safecall;
    function Get_ProcessOptimizationMode: ProcessOptimizationModes; safecall;
    procedure Set_ProcessOptimizationMode(Param1: ProcessOptimizationModes); safecall;
    function Get_ProcessingKeyErrorLimit: Integer; safecall;
    procedure Set_ProcessingKeyErrorLimit(Param1: Integer); safecall;
    function Get_ProcessingKeyErrorLogFileName: WideString; safecall;
    procedure Set_ProcessingKeyErrorLogFileName(const Param1: WideString); safecall;
    function Get_LazyOptimizationProgress: Smallint; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    procedure CommitTransEx(Options: ProcessTypes); safecall;
    property Commands: _OlapCollection read Get_Commands;
    property DataSources: _OlapCollection read Get_DataSources;
    property Dimensions: _OlapCollection read Get_Dimensions;
    property MDStores: _OlapCollection read Get_MDStores;
    property Measures: _OlapCollection read Get_Measures;
    property Roles: _OlapCollection read Get_Roles;
    property CustomProperties: _Properties read Get_CustomProperties;
    property AggregationPrefix: WideString read Get_AggregationPrefix write Set_AggregationPrefix;
    property Analyzer: IDispatch read Get_Analyzer;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property Description: WideString read Get_Description write Set_Description;
    property EstimatedRows: Double read Get_EstimatedRows write Set_EstimatedRows;
    property EstimatedSize: Double read Get_EstimatedSize;
    property IsDefault: WordBool read Get_IsDefault write Set_IsDefault;
    property State: OlapStateTypes read Get_State;
    property IsValid: WordBool read Get_IsValid;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property Name: WideString read Get_Name write Set_Name;
    property OlapMode: OlapStorageModes read Get_OlapMode write Set_OlapMode;
    property Parent: IDispatch read Get_Parent;
    property SourceTable: WideString read Get_SourceTable write Set_SourceTable;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property Server: _Server read Get_Server;
    property IsTemporary: WordBool read Get_IsTemporary;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property MiningModels: _OlapCollection read Get_MiningModels;
    property SourceTableAlias: WideString read Get_SourceTableAlias write Set_SourceTableAlias;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property AllowDrillThrough: WordBool read Get_AllowDrillThrough write Set_AllowDrillThrough;
    property DrillThroughColumns: WideString read Get_DrillThroughColumns write Set_DrillThroughColumns;
    property DrillThroughFilter: WideString read Get_DrillThroughFilter write Set_DrillThroughFilter;
    property RemoteServer: WideString read Get_RemoteServer write Set_RemoteServer;
    property DrillThroughFrom: WideString read Get_DrillThroughFrom write Set_DrillThroughFrom;
    property DrillThroughJoins: WideString read Get_DrillThroughJoins write Set_DrillThroughJoins;
    property DefaultMeasure: WideString read Get_DefaultMeasure write Set_DefaultMeasure;
    property ProcessOptimizationMode: ProcessOptimizationModes read Get_ProcessOptimizationMode write Set_ProcessOptimizationMode;
    property ProcessingKeyErrorLimit: Integer read Get_ProcessingKeyErrorLimit write Set_ProcessingKeyErrorLimit;
    property ProcessingKeyErrorLogFileName: WideString read Get_ProcessingKeyErrorLogFileName write Set_ProcessingKeyErrorLogFileName;
    property LazyOptimizationProgress: Smallint read Get_LazyOptimizationProgress;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
  end;

// *********************************************************************//
// DispIntf:  _MDStoreDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5800-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _MDStoreDisp = dispinterface
    ['{E8AC5800-7127-11D2-8A35-00C04FB9898D}']
    property Commands: _OlapCollection readonly dispid 1745027100;
    property DataSources: _OlapCollection readonly dispid 1745027099;
    property Dimensions: _OlapCollection readonly dispid 1745027098;
    property MDStores: _OlapCollection readonly dispid 1745027097;
    property Measures: _OlapCollection readonly dispid 1745027096;
    property Roles: _OlapCollection readonly dispid 1745027095;
    property CustomProperties: _Properties readonly dispid 1745027094;
    property AggregationPrefix: WideString dispid 1745027093;
    property Analyzer: IDispatch readonly dispid 1745027092;
    property ClassType: ClassTypes readonly dispid 1745027091;
    property SubClassType: SubClassTypes readonly dispid 1745027090;
    property Description: WideString dispid 1745027089;
    property EstimatedRows: Double dispid 1745027088;
    property EstimatedSize: Double readonly dispid 1745027087;
    property IsDefault: WordBool dispid 1745027086;
    property State: OlapStateTypes readonly dispid 1745027085;
    property IsValid: WordBool readonly dispid 1745027084;
    property LastProcessed: TDateTime readonly dispid 1745027083;
    property LastUpdated: TDateTime dispid 1745027082;
    property Name: WideString dispid 1745027081;
    property OlapMode: OlapStorageModes dispid 1745027080;
    property Parent: IDispatch readonly dispid 1745027079;
    property SourceTable: WideString dispid 1745027078;
    property SourceTableFilter: WideString dispid 1745027077;
    property Server: _Server readonly dispid 1745027076;
    property IsTemporary: WordBool readonly dispid 1745027075;
    property JoinClause: WideString dispid 1745027074;
    property FromClause: WideString dispid 1745027073;
    property IsReadWrite: WordBool dispid 1745027072;
    procedure BeginTrans; dispid 1610809373;
    procedure CommitTrans; dispid 1610809374;
    procedure Rollback; dispid 1610809375;
    procedure Merge(const SourceName: WideString); dispid 1610809376;
    procedure Process(Options: ProcessTypes); dispid 1610809377;
    procedure Update; dispid 1610809378;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809379;
    procedure UnlockObject; dispid 1610809380;
    procedure Clone(const TargetObject: _MDStore; Options: CloneOptions); dispid 1610809381;
    property MiningModels: _OlapCollection readonly dispid 1745027124;
    property SourceTableAlias: WideString dispid 1745027123;
    property IsVisible: WordBool dispid 1745027122;
    property AllowDrillThrough: WordBool dispid 1745027121;
    property DrillThroughColumns: WideString dispid 1745027120;
    property DrillThroughFilter: WideString dispid 1745027119;
    property RemoteServer: WideString dispid 1745027118;
    property DrillThroughFrom: WideString dispid 1745027117;
    property DrillThroughJoins: WideString dispid 1745027116;
    property DefaultMeasure: WideString dispid 1745027115;
    property ProcessOptimizationMode: ProcessOptimizationModes dispid 1745027114;
    property ProcessingKeyErrorLimit: Integer dispid 1745027113;
    property ProcessingKeyErrorLogFileName: WideString dispid 1745027112;
    property LazyOptimizationProgress: Smallint readonly dispid 1745027111;
    property EnableRealTimeUpdates: WordBool dispid 1745027110;
    procedure CommitTransEx(Options: ProcessTypes); dispid 1610809397;
  end;

// *********************************************************************//
// Interface: _OlapCollection
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580A-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _OlapCollection = interface(IDispatch)
    ['{E8AC580A-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__OlapCollection_28_1; safecall;
    procedure GhostMethod__OlapCollection_32_2; safecall;
    procedure GhostMethod__OlapCollection_36_3; safecall;
    function Get_Parent: IDispatch; safecall;
    function Get__className: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_ContainedClassType: ClassTypes; safecall;
    function Get_ContainedSubClassType: SubClassTypes; safecall;
    procedure ReleaseChild(var vKey: OleVariant); safecall;
    procedure InitConsts(var ParentObject: IDispatch; var ClassType: Smallint; 
                         var AllowWrite: WordBool; var Ordering: Smallint; 
                         var SubClassType: Smallint); safecall;
    function Find(vKey: OleVariant): WordBool; safecall;
    function AddNew(const Name: WideString; SubClassType: SubClassTypes): IDispatch; safecall;
    procedure Add(const obj: IDispatch; const sKey: WideString; Before: OleVariant); safecall;
    function Item(vntIndexKey: OleVariant): OleVariant; safecall;
    function Get_Count: Integer; safecall;
    procedure Remove(vntIndexKey: OleVariant); safecall;
    function Get_NewEnum: IUnknown; safecall;
    procedure Move(OldIndex: Smallint; NewIndex: Smallint; const NewKey: WideString); safecall;
    property Parent: IDispatch read Get_Parent;
    property _className: WideString read Get__className;
    property ClassType: Smallint read Get_ClassType;
    property ContainedClassType: ClassTypes read Get_ContainedClassType;
    property ContainedSubClassType: SubClassTypes read Get_ContainedSubClassType;
    property Count: Integer read Get_Count;
    property NewEnum: IUnknown read Get_NewEnum;
  end;

// *********************************************************************//
// DispIntf:  _OlapCollectionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC580A-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _OlapCollectionDisp = dispinterface
    ['{E8AC580A-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__OlapCollection_28_1; dispid 1610743808;
    procedure GhostMethod__OlapCollection_32_2; dispid 1610743809;
    procedure GhostMethod__OlapCollection_36_3; dispid 1610743810;
    property Parent: IDispatch readonly dispid 1745027087;
    property _className: WideString readonly dispid 1745027085;
    property ClassType: Smallint readonly dispid 1745027084;
    property ContainedClassType: ClassTypes readonly dispid 1745027083;
    property ContainedSubClassType: SubClassTypes readonly dispid 1745027082;
    procedure ReleaseChild(var vKey: OleVariant); dispid 1610809361;
    procedure InitConsts(var ParentObject: IDispatch; var ClassType: Smallint; 
                         var AllowWrite: WordBool; var Ordering: Smallint; 
                         var SubClassType: Smallint); dispid 1610809365;
    function Find(vKey: OleVariant): WordBool; dispid 1610809366;
    function AddNew(const Name: WideString; SubClassType: SubClassTypes): IDispatch; dispid 1610809369;
    procedure Add(const obj: IDispatch; const sKey: WideString; Before: OleVariant); dispid 1610809370;
    function Item(vntIndexKey: OleVariant): OleVariant; dispid 0;
    property Count: Integer readonly dispid 1745027080;
    procedure Remove(vntIndexKey: OleVariant); dispid 1610809372;
    property NewEnum: IUnknown readonly dispid -4;
    procedure Move(OldIndex: Smallint; NewIndex: Smallint; const NewKey: WideString); dispid 1610809373;
  end;

// *********************************************************************//
// Interface: _Partition
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5838-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Partition = interface(IDispatch)
    ['{E8AC5838-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Partition_28_1; safecall;
    procedure GhostMethod__Partition_32_2; safecall;
    procedure GhostMethod__Partition_36_3; safecall;
    procedure GhostMethod__Partition_40_4; safecall;
    procedure GhostMethod__Partition_44_5; safecall;
    procedure GhostMethod__Partition_48_6; safecall;
    procedure GhostMethod__Partition_52_7; safecall;
    procedure GhostMethod__Partition_56_8; safecall;
    procedure GhostMethod__Partition_60_9; safecall;
    procedure GhostMethod__Partition_64_10; safecall;
    procedure GhostMethod__Partition_68_11; safecall;
    procedure GhostMethod__Partition_72_12; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure SaveObject(var AssumeInsert: WordBool); safecall;
    function Get_Analyzer: _PartitionAnalyzer; safecall;
    function Get_State: OlapStateTypes; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Partition: _Partition; safecall;
    function Get_Database: _Database; safecall;
    function Validate: ValidateErrorCodes; safecall;
    function Get_Server: _Server; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Aggregations: _OlapCollection; safecall;
    procedure _Set_Aggregations(var Param1: _OlapCollection); safecall;
    function Get_Dimensions: _OlapCollection; safecall;
    function Get_Measures: _OlapCollection; safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_LastProcessed: TDateTime; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure _Set_Parent(const Param1: _Cube); safecall;
    function Get_Parent: _Cube; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Path: WideString; safecall;
    function Get_DataSource: WideString; safecall;
    procedure Set_DataSource(var Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_FactTable: WideString; safecall;
    procedure Set_FactTable(var Param1: WideString); safecall;
    function Get_Where: WideString; safecall;
    procedure Set_Where(var Param1: WideString); safecall;
    function Get_FactTableSize: Integer; safecall;
    procedure Set_OlapMode(Param1: Smallint); safecall;
    function Get_OlapMode: Smallint; safecall;
    function Get_AggregationsOLAPMode: Smallint; safecall;
    procedure Set_FactTableSize(var Param1: Integer); safecall;
    function Get_EstimatedSize: Double; safecall;
    procedure DeleteAggregations; safecall;
    procedure ClearCollections; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Process(ProcessOption: ProcessTypes); safecall;
    function Get_BaseAggregation: _Aggregation; safecall;
    function Get_AggregationPrefix: WideString; safecall;
    procedure Set_AggregationPrefix(const Param1: WideString); safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ICube: IDispatch; safecall;
    procedure GhostMethod__Partition_288_13; safecall;
    procedure GhostMethod__Partition_292_14; safecall;
    procedure GhostMethod__Partition_296_15; safecall;
    procedure GhostMethod__Partition_300_16; safecall;
    procedure GhostMethod__Partition_304_17; safecall;
    procedure GhostMethod__Partition_308_18; safecall;
    procedure GhostMethod__Partition_312_19; safecall;
    procedure GhostMethod__Partition_316_20; safecall;
    procedure GhostMethod__Partition_320_21; safecall;
    procedure GhostMethod__Partition_324_22; safecall;
    procedure GhostMethod__Partition_328_23; safecall;
    procedure GhostMethod__Partition_332_24; safecall;
    function Get_RemoteServer: WideString; safecall;
    procedure Set_RemoteServer(const Param1: WideString); safecall;
    function Get_DefaultMeasure: WideString; safecall;
    procedure Set_DefaultMeasure(const Param1: WideString); safecall;
    function Get_DrillThroughColumns: WideString; safecall;
    procedure Set_DrillThroughColumns(const Param1: WideString); safecall;
    function Get_DrillThroughFilter: WideString; safecall;
    procedure Set_DrillThroughFilter(const Param1: WideString); safecall;
    function Get_DrillThroughFrom: WideString; safecall;
    procedure Set_DrillThroughFrom(const Param1: WideString); safecall;
    function Get_DrillThroughJoins: WideString; safecall;
    procedure Set_DrillThroughJoins(const Param1: WideString); safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_LazyOptimizationProgress: Smallint; safecall;
    procedure Set_ProcessingKeyErrorLogFileName(const Param1: WideString); safecall;
    function Get_ProcessingKeyErrorLogFileName: WideString; safecall;
    procedure Set_ProcessingKeyErrorLimit(Param1: Integer); safecall;
    function Get_ProcessingKeyErrorLimit: Integer; safecall;
    procedure Set_ProcessOptimizationMode(Param1: ProcessOptimizationModes); safecall;
    function Get_ProcessOptimizationMode: ProcessOptimizationModes; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_MiningModels: _OlapCollection; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_EstimatedRows: Double; safecall;
    procedure Set_EstimatedRows(Param1: Double); safecall;
    function Get_IsDefault: WordBool; safecall;
    procedure Set_IsDefault(Param1: WordBool); safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_SourceTable: WideString; safecall;
    procedure Set_SourceTable(const Param1: WideString); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    procedure Set_SourceTableAlias(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_AllowDrillThrough: WordBool; safecall;
    procedure Set_AllowDrillThrough(Param1: WordBool); safecall;
    procedure BeginTrans; safecall;
    procedure CommitTrans; safecall;
    procedure Rollback; safecall;
    procedure Merge(const SourceName: WideString); safecall;
    procedure Update; safecall;
    procedure GhostMethod__Partition_536_25; safecall;
    procedure GhostMethod__Partition_540_26; safecall;
    procedure GhostMethod__Partition_544_27; safecall;
    procedure GhostMethod__Partition_548_28; safecall;
    procedure GhostMethod__Partition_552_29; safecall;
    procedure GhostMethod__Partition_556_30; safecall;
    procedure GhostMethod__Partition_560_31; safecall;
    procedure GhostMethod__Partition_564_32; safecall;
    procedure GhostMethod__Partition_568_33; safecall;
    procedure GhostMethod__Partition_572_34; safecall;
    procedure GhostMethod__Partition_576_35; safecall;
    procedure GhostMethod__Partition_580_36; safecall;
    procedure CommitTransEx(Options: ProcessTypes); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Analyzer: _PartitionAnalyzer read Get_Analyzer;
    property State: OlapStateTypes read Get_State;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Partition: _Partition read Get_Partition;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Cube: _Cube read Get_Cube;
    property Aggregations: _OlapCollection read Get_Aggregations;
    property Dimensions: _OlapCollection read Get_Dimensions;
    property Measures: _OlapCollection read Get_Measures;
    property IsTemporary: WordBool read Get_IsTemporary;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property Parent: _Cube read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property Path: WideString read Get_Path;
    property DataSource: WideString read Get_DataSource write Set_DataSource;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FactTable: WideString read Get_FactTable write Set_FactTable;
    property Where: WideString read Get_Where write Set_Where;
    property FactTableSize: Integer read Get_FactTableSize write Set_FactTableSize;
    property OlapMode: Smallint read Get_OlapMode write Set_OlapMode;
    property AggregationsOLAPMode: Smallint read Get_AggregationsOLAPMode;
    property EstimatedSize: Double read Get_EstimatedSize;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property BaseAggregation: _Aggregation read Get_BaseAggregation;
    property AggregationPrefix: WideString read Get_AggregationPrefix write Set_AggregationPrefix;
    property IObject: IDispatch read Get_IObject;
    property ICube: IDispatch read Get_ICube;
    property RemoteServer: WideString read Get_RemoteServer write Set_RemoteServer;
    property DefaultMeasure: WideString read Get_DefaultMeasure write Set_DefaultMeasure;
    property DrillThroughColumns: WideString read Get_DrillThroughColumns write Set_DrillThroughColumns;
    property DrillThroughFilter: WideString read Get_DrillThroughFilter write Set_DrillThroughFilter;
    property DrillThroughFrom: WideString read Get_DrillThroughFrom write Set_DrillThroughFrom;
    property DrillThroughJoins: WideString read Get_DrillThroughJoins write Set_DrillThroughJoins;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property LazyOptimizationProgress: Smallint read Get_LazyOptimizationProgress;
    property ProcessingKeyErrorLogFileName: WideString read Get_ProcessingKeyErrorLogFileName write Set_ProcessingKeyErrorLogFileName;
    property ProcessingKeyErrorLimit: Integer read Get_ProcessingKeyErrorLimit write Set_ProcessingKeyErrorLimit;
    property ProcessOptimizationMode: ProcessOptimizationModes read Get_ProcessOptimizationMode write Set_ProcessOptimizationMode;
    property Commands: _OlapCollection read Get_Commands;
    property DataSources: _OlapCollection read Get_DataSources;
    property MDStores: _OlapCollection read Get_MDStores;
    property Roles: _OlapCollection read Get_Roles;
    property MiningModels: _OlapCollection read Get_MiningModels;
    property Description: WideString read Get_Description write Set_Description;
    property EstimatedRows: Double read Get_EstimatedRows write Set_EstimatedRows;
    property IsDefault: WordBool read Get_IsDefault write Set_IsDefault;
    property IsValid: WordBool read Get_IsValid;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property SourceTable: WideString read Get_SourceTable write Set_SourceTable;
    property SourceTableAlias: WideString read Get_SourceTableAlias write Set_SourceTableAlias;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property AllowDrillThrough: WordBool read Get_AllowDrillThrough write Set_AllowDrillThrough;
  end;

// *********************************************************************//
// DispIntf:  _PartitionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5838-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _PartitionDisp = dispinterface
    ['{E8AC5838-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Partition_28_1; dispid 1610743808;
    procedure GhostMethod__Partition_32_2; dispid 1610743809;
    procedure GhostMethod__Partition_36_3; dispid 1610743810;
    procedure GhostMethod__Partition_40_4; dispid 1610743811;
    procedure GhostMethod__Partition_44_5; dispid 1610743812;
    procedure GhostMethod__Partition_48_6; dispid 1610743813;
    procedure GhostMethod__Partition_52_7; dispid 1610743814;
    procedure GhostMethod__Partition_56_8; dispid 1610743815;
    procedure GhostMethod__Partition_60_9; dispid 1610743816;
    procedure GhostMethod__Partition_64_10; dispid 1610743817;
    procedure GhostMethod__Partition_68_11; dispid 1610743818;
    procedure GhostMethod__Partition_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809457;
    property CustomProperties: _Properties readonly dispid 1745027181;
    procedure SaveObject(var AssumeInsert: WordBool); dispid 1610809459;
    property Analyzer: _PartitionAnalyzer readonly dispid 1745027176;
    property State: OlapStateTypes readonly dispid 1745027175;
    property Valid: WordBool dispid 1745027174;
    property Partition: _Partition readonly dispid 1745027170;
    property Database: _Database readonly dispid 1745027169;
    function Validate: ValidateErrorCodes; dispid 1610809468;
    property Server: _Server readonly dispid 1745027168;
    property Cube: _Cube readonly dispid 1745027167;
    property Aggregations: _OlapCollection dispid 1745027166;
    property Dimensions: _OlapCollection readonly dispid 1745027165;
    property Measures: _OlapCollection readonly dispid 1745027164;
    property IsTemporary: WordBool readonly dispid 1745027163;
    property IsReadWrite: WordBool dispid 1745027162;
    property LastProcessed: TDateTime readonly dispid 1745027161;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809469;
    procedure UnlockObject; dispid 1610809470;
    property Parent: _Cube dispid 1745027160;
    property Name: WideString dispid 1745027159;
    property Path: WideString readonly dispid 1745027157;
    property DataSource: WideString dispid 1745027156;
    property FromClause: WideString dispid 1745027155;
    property JoinClause: WideString dispid 1745027154;
    property FactTable: WideString dispid 1745027153;
    property Where: WideString dispid 1745027151;
    property FactTableSize: Integer dispid 1745027149;
    property OlapMode: Smallint dispid 1745027148;
    property AggregationsOLAPMode: Smallint readonly dispid 1745027146;
    property EstimatedSize: Double readonly dispid 1745027145;
    procedure DeleteAggregations; dispid 1610809481;
    procedure ClearCollections; dispid 1610809484;
    property ClassType: Smallint readonly dispid 1745027144;
    property SubClassType: SubClassTypes readonly dispid 1745027143;
    procedure Process(ProcessOption: ProcessTypes); dispid 1610809486;
    property BaseAggregation: _Aggregation readonly dispid 1745027142;
    property AggregationPrefix: WideString dispid 1745027140;
    property IObject: IDispatch readonly dispid 1745027136;
    property ICube: IDispatch readonly dispid 1745027135;
    procedure GhostMethod__Partition_288_13; dispid 1610743873;
    procedure GhostMethod__Partition_292_14; dispid 1610743874;
    procedure GhostMethod__Partition_296_15; dispid 1610743875;
    procedure GhostMethod__Partition_300_16; dispid 1610743876;
    procedure GhostMethod__Partition_304_17; dispid 1610743877;
    procedure GhostMethod__Partition_308_18; dispid 1610743878;
    procedure GhostMethod__Partition_312_19; dispid 1610743879;
    procedure GhostMethod__Partition_316_20; dispid 1610743880;
    procedure GhostMethod__Partition_320_21; dispid 1610743881;
    procedure GhostMethod__Partition_324_22; dispid 1610743882;
    procedure GhostMethod__Partition_328_23; dispid 1610743883;
    procedure GhostMethod__Partition_332_24; dispid 1610743884;
    property RemoteServer: WideString dispid 1745027349;
    property DefaultMeasure: WideString dispid 1745027348;
    property DrillThroughColumns: WideString dispid 1745027347;
    property DrillThroughFilter: WideString dispid 1745027346;
    property DrillThroughFrom: WideString dispid 1745027345;
    property DrillThroughJoins: WideString dispid 1745027344;
    property EnableRealTimeUpdates: WordBool dispid 1745027343;
    property LazyOptimizationProgress: Smallint readonly dispid 1745027334;
    property ProcessingKeyErrorLogFileName: WideString dispid 1745027317;
    property ProcessingKeyErrorLimit: Integer dispid 1745027316;
    property ProcessOptimizationMode: ProcessOptimizationModes dispid 1745027315;
    property Commands: _OlapCollection readonly dispid 1745027229;
    property DataSources: _OlapCollection readonly dispid 1745027228;
    property MDStores: _OlapCollection readonly dispid 1745027227;
    property Roles: _OlapCollection readonly dispid 1745027226;
    property MiningModels: _OlapCollection readonly dispid 1745027225;
    property Description: WideString dispid 1745027224;
    property EstimatedRows: Double dispid 1745027223;
    property IsDefault: WordBool dispid 1745027222;
    property IsValid: WordBool readonly dispid 1745027221;
    property LastUpdated: TDateTime dispid 1745027220;
    property SourceTable: WideString dispid 1745027219;
    property SourceTableAlias: WideString dispid 1745027218;
    property SourceTableFilter: WideString dispid 1745027217;
    property IsVisible: WordBool dispid 1745027216;
    property AllowDrillThrough: WordBool dispid 1745027215;
    procedure BeginTrans; dispid 1610809680;
    procedure CommitTrans; dispid 1610809681;
    procedure Rollback; dispid 1610809682;
    procedure Merge(const SourceName: WideString); dispid 1610809683;
    procedure Update; dispid 1610809684;
    procedure GhostMethod__Partition_536_25; dispid 1610743935;
    procedure GhostMethod__Partition_540_26; dispid 1610743936;
    procedure GhostMethod__Partition_544_27; dispid 1610743937;
    procedure GhostMethod__Partition_548_28; dispid 1610743938;
    procedure GhostMethod__Partition_552_29; dispid 1610743939;
    procedure GhostMethod__Partition_556_30; dispid 1610743940;
    procedure GhostMethod__Partition_560_31; dispid 1610743941;
    procedure GhostMethod__Partition_564_32; dispid 1610743942;
    procedure GhostMethod__Partition_568_33; dispid 1610743943;
    procedure GhostMethod__Partition_572_34; dispid 1610743944;
    procedure GhostMethod__Partition_576_35; dispid 1610743945;
    procedure GhostMethod__Partition_580_36; dispid 1610743946;
    procedure CommitTransEx(Options: ProcessTypes); dispid 1610809859;
  end;

// *********************************************************************//
// Interface: _Aggregation
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5837-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Aggregation = interface(IDispatch)
    ['{E8AC5837-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Aggregation_28_1; safecall;
    procedure GhostMethod__Aggregation_32_2; safecall;
    procedure GhostMethod__Aggregation_36_3; safecall;
    procedure GhostMethod__Aggregation_40_4; safecall;
    procedure GhostMethod__Aggregation_44_5; safecall;
    procedure GhostMethod__Aggregation_48_6; safecall;
    procedure GhostMethod__Aggregation_52_7; safecall;
    procedure GhostMethod__Aggregation_56_8; safecall;
    procedure GhostMethod__Aggregation_60_9; safecall;
    procedure GhostMethod__Aggregation_64_10; safecall;
    procedure GhostMethod__Aggregation_68_11; safecall;
    procedure GhostMethod__Aggregation_72_12; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_IsDefault: WordBool; safecall;
    procedure Set_IsDefault(Param1: WordBool); safecall;
    function Get_EstimatedSize: Double; safecall;
    function Get_ActualSize: Integer; safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_TableName: WideString; safecall;
    function Get_Name: WideString; safecall;
    function Get_FactTable: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_DatasetName(var Param1: WideString); safecall;
    function Get_DatasetName: WideString; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Partition: _Partition; safecall;
    procedure _Set_Parent(const Param1: _Partition); safecall;
    function Get_Parent: _Partition; safecall;
    function Get_Path: WideString; safecall;
    function Get_OlapMode: Smallint; safecall;
    function Get_DataSource: WideString; safecall;
    procedure Set_OlapMode(Param1: Smallint); safecall;
    function Get_EstimatedRows: Double; safecall;
    procedure Set_EstimatedRows(Param1: Double); safecall;
    function Get_Dimensions: _OlapCollection; safecall;
    function Get_Measures: _OlapCollection; safecall;
    function Get_Measure: _OlapCollection; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SourceTable(const Param1: WideString); safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ICube: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure GhostMethod__Aggregation_244_13; safecall;
    procedure GhostMethod__Aggregation_248_14; safecall;
    procedure GhostMethod__Aggregation_252_15; safecall;
    procedure GhostMethod__Aggregation_256_16; safecall;
    procedure GhostMethod__Aggregation_260_17; safecall;
    procedure GhostMethod__Aggregation_264_18; safecall;
    procedure GhostMethod__Aggregation_268_19; safecall;
    procedure GhostMethod__Aggregation_272_20; safecall;
    procedure GhostMethod__Aggregation_276_21; safecall;
    procedure GhostMethod__Aggregation_280_22; safecall;
    procedure GhostMethod__Aggregation_284_23; safecall;
    procedure GhostMethod__Aggregation_288_24; safecall;
    function Get_AggregationPrefix: WideString; safecall;
    function Get_IsTemporary: WordBool; safecall;
    procedure GhostMethod__Aggregation_300_25; safecall;
    procedure GhostMethod__Aggregation_304_26; safecall;
    procedure GhostMethod__Aggregation_308_27; safecall;
    procedure GhostMethod__Aggregation_312_28; safecall;
    procedure GhostMethod__Aggregation_316_29; safecall;
    procedure GhostMethod__Aggregation_320_30; safecall;
    procedure GhostMethod__Aggregation_324_31; safecall;
    procedure GhostMethod__Aggregation_328_32; safecall;
    procedure GhostMethod__Aggregation_332_33; safecall;
    procedure GhostMethod__Aggregation_336_34; safecall;
    procedure GhostMethod__Aggregation_340_35; safecall;
    procedure GhostMethod__Aggregation_344_36; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_LazyOptimizationProgress: Smallint; safecall;
    procedure Set_ProcessingKeyErrorLogFileName(const Param1: WideString); safecall;
    function Get_ProcessingKeyErrorLogFileName: WideString; safecall;
    procedure Set_ProcessingKeyErrorLimit(Param1: Integer); safecall;
    function Get_ProcessingKeyErrorLimit: Integer; safecall;
    procedure Set_ProcessOptimizationMode(Param1: ProcessOptimizationModes); safecall;
    function Get_ProcessOptimizationMode: ProcessOptimizationModes; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_MiningModels: _OlapCollection; safecall;
    procedure Set_AggregationPrefix(const Param1: WideString); safecall;
    function Get_Analyzer: IDispatch; safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    procedure Set_SourceTableAlias(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_AllowDrillThrough: WordBool; safecall;
    procedure Set_AllowDrillThrough(Param1: WordBool); safecall;
    function Get_DrillThroughColumns: WideString; safecall;
    procedure Set_DrillThroughColumns(const Param1: WideString); safecall;
    function Get_DrillThroughFilter: WideString; safecall;
    procedure Set_DrillThroughFilter(const Param1: WideString); safecall;
    function Get_RemoteServer: WideString; safecall;
    procedure Set_RemoteServer(const Param1: WideString); safecall;
    function Get_DrillThroughFrom: WideString; safecall;
    procedure Set_DrillThroughFrom(const Param1: WideString); safecall;
    function Get_DrillThroughJoins: WideString; safecall;
    procedure Set_DrillThroughJoins(const Param1: WideString); safecall;
    function Get_DefaultMeasure: WideString; safecall;
    procedure Set_DefaultMeasure(const Param1: WideString); safecall;
    procedure BeginTrans; safecall;
    procedure CommitTrans; safecall;
    procedure Rollback; safecall;
    procedure Merge(const SourceName: WideString); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure GhostMethod__Aggregation_552_37; safecall;
    procedure GhostMethod__Aggregation_556_38; safecall;
    procedure GhostMethod__Aggregation_560_39; safecall;
    procedure GhostMethod__Aggregation_564_40; safecall;
    procedure GhostMethod__Aggregation_568_41; safecall;
    procedure GhostMethod__Aggregation_572_42; safecall;
    procedure GhostMethod__Aggregation_576_43; safecall;
    procedure GhostMethod__Aggregation_580_44; safecall;
    procedure GhostMethod__Aggregation_584_45; safecall;
    procedure GhostMethod__Aggregation_588_46; safecall;
    procedure GhostMethod__Aggregation_592_47; safecall;
    procedure GhostMethod__Aggregation_596_48; safecall;
    procedure CommitTransEx(Options: ProcessTypes); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property IsDefault: WordBool read Get_IsDefault write Set_IsDefault;
    property EstimatedSize: Double read Get_EstimatedSize;
    property ActualSize: Integer read Get_ActualSize;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property TableName: WideString read Get_TableName;
    property FactTable: WideString read Get_FactTable;
    property Description: WideString read Get_Description write Set_Description;
    property DatasetName: WideString read Get_DatasetName write Set_DatasetName;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Cube: _Cube read Get_Cube;
    property Partition: _Partition read Get_Partition;
    property Parent: _Partition read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property OlapMode: Smallint read Get_OlapMode write Set_OlapMode;
    property DataSource: WideString read Get_DataSource;
    property EstimatedRows: Double read Get_EstimatedRows write Set_EstimatedRows;
    property Dimensions: _OlapCollection read Get_Dimensions;
    property Measures: _OlapCollection read Get_Measures;
    property Measure: _OlapCollection read Get_Measure;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property SourceTable: WideString read Get_SourceTable write Set_SourceTable;
    property IObject: IDispatch read Get_IObject;
    property ICube: IDispatch read Get_ICube;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property AggregationPrefix: WideString read Get_AggregationPrefix write Set_AggregationPrefix;
    property IsTemporary: WordBool read Get_IsTemporary;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property LazyOptimizationProgress: Smallint read Get_LazyOptimizationProgress;
    property ProcessingKeyErrorLogFileName: WideString read Get_ProcessingKeyErrorLogFileName write Set_ProcessingKeyErrorLogFileName;
    property ProcessingKeyErrorLimit: Integer read Get_ProcessingKeyErrorLimit write Set_ProcessingKeyErrorLimit;
    property ProcessOptimizationMode: ProcessOptimizationModes read Get_ProcessOptimizationMode write Set_ProcessOptimizationMode;
    property Commands: _OlapCollection read Get_Commands;
    property DataSources: _OlapCollection read Get_DataSources;
    property MDStores: _OlapCollection read Get_MDStores;
    property Roles: _OlapCollection read Get_Roles;
    property MiningModels: _OlapCollection read Get_MiningModels;
    property Analyzer: IDispatch read Get_Analyzer;
    property State: OlapStateTypes read Get_State;
    property IsValid: WordBool read Get_IsValid;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property SourceTableAlias: WideString read Get_SourceTableAlias write Set_SourceTableAlias;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property AllowDrillThrough: WordBool read Get_AllowDrillThrough write Set_AllowDrillThrough;
    property DrillThroughColumns: WideString read Get_DrillThroughColumns write Set_DrillThroughColumns;
    property DrillThroughFilter: WideString read Get_DrillThroughFilter write Set_DrillThroughFilter;
    property RemoteServer: WideString read Get_RemoteServer write Set_RemoteServer;
    property DrillThroughFrom: WideString read Get_DrillThroughFrom write Set_DrillThroughFrom;
    property DrillThroughJoins: WideString read Get_DrillThroughJoins write Set_DrillThroughJoins;
    property DefaultMeasure: WideString read Get_DefaultMeasure write Set_DefaultMeasure;
  end;

// *********************************************************************//
// DispIntf:  _AggregationDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5837-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationDisp = dispinterface
    ['{E8AC5837-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Aggregation_28_1; dispid 1610743808;
    procedure GhostMethod__Aggregation_32_2; dispid 1610743809;
    procedure GhostMethod__Aggregation_36_3; dispid 1610743810;
    procedure GhostMethod__Aggregation_40_4; dispid 1610743811;
    procedure GhostMethod__Aggregation_44_5; dispid 1610743812;
    procedure GhostMethod__Aggregation_48_6; dispid 1610743813;
    procedure GhostMethod__Aggregation_52_7; dispid 1610743814;
    procedure GhostMethod__Aggregation_56_8; dispid 1610743815;
    procedure GhostMethod__Aggregation_60_9; dispid 1610743816;
    procedure GhostMethod__Aggregation_64_10; dispid 1610743817;
    procedure GhostMethod__Aggregation_68_11; dispid 1610743818;
    procedure GhostMethod__Aggregation_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809456;
    property CustomProperties: _Properties readonly dispid 1745027181;
    property IsDefault: WordBool dispid 1745027176;
    property EstimatedSize: Double readonly dispid 1745027174;
    property ActualSize: Integer readonly dispid 1745027172;
    property Valid: WordBool dispid 1745027171;
    property Name: WideString dispid 1745027170;
    property TableName: WideString readonly dispid 1745027169;
    property FactTable: WideString readonly dispid 1745027168;
    property Description: WideString dispid 1745027167;
    property DatasetName: WideString dispid 1745027165;
    property Database: _Database readonly dispid 1745027164;
    property Server: _Server readonly dispid 1745027163;
    property Cube: _Cube readonly dispid 1745027162;
    property Partition: _Partition readonly dispid 1745027161;
    property Parent: _Partition dispid 1745027160;
    property Path: WideString readonly dispid 1745027154;
    property OlapMode: Smallint dispid 1745027153;
    property DataSource: WideString readonly dispid 1745027152;
    property EstimatedRows: Double dispid 1745027151;
    property Dimensions: _OlapCollection readonly dispid 1745027148;
    property Measures: _OlapCollection readonly dispid 1745027147;
    property Measure: _OlapCollection readonly dispid 1745027145;
    property ClassType: Smallint readonly dispid 1745027144;
    property SubClassType: SubClassTypes readonly dispid 1745027143;
    property SourceTable: WideString dispid 1745027138;
    property IObject: IDispatch readonly dispid 1745027137;
    property ICube: IDispatch readonly dispid 1745027136;
    procedure ClearCollections; dispid 1610809472;
    property FromClause: WideString dispid 1745027112;
    property JoinClause: WideString dispid 1745027111;
    procedure GhostMethod__Aggregation_244_13; dispid 1610743862;
    procedure GhostMethod__Aggregation_248_14; dispid 1610743863;
    procedure GhostMethod__Aggregation_252_15; dispid 1610743864;
    procedure GhostMethod__Aggregation_256_16; dispid 1610743865;
    procedure GhostMethod__Aggregation_260_17; dispid 1610743866;
    procedure GhostMethod__Aggregation_264_18; dispid 1610743867;
    procedure GhostMethod__Aggregation_268_19; dispid 1610743868;
    procedure GhostMethod__Aggregation_272_20; dispid 1610743869;
    procedure GhostMethod__Aggregation_276_21; dispid 1610743870;
    procedure GhostMethod__Aggregation_280_22; dispid 1610743871;
    procedure GhostMethod__Aggregation_284_23; dispid 1610743872;
    procedure GhostMethod__Aggregation_288_24; dispid 1610743873;
    property AggregationPrefix: WideString dispid 1745027278;
    property IsTemporary: WordBool readonly dispid 1745027270;
    procedure GhostMethod__Aggregation_300_25; dispid 1610743876;
    procedure GhostMethod__Aggregation_304_26; dispid 1610743877;
    procedure GhostMethod__Aggregation_308_27; dispid 1610743878;
    procedure GhostMethod__Aggregation_312_28; dispid 1610743879;
    procedure GhostMethod__Aggregation_316_29; dispid 1610743880;
    procedure GhostMethod__Aggregation_320_30; dispid 1610743881;
    procedure GhostMethod__Aggregation_324_31; dispid 1610743882;
    procedure GhostMethod__Aggregation_328_32; dispid 1610743883;
    procedure GhostMethod__Aggregation_332_33; dispid 1610743884;
    procedure GhostMethod__Aggregation_336_34; dispid 1610743885;
    procedure GhostMethod__Aggregation_340_35; dispid 1610743886;
    procedure GhostMethod__Aggregation_344_36; dispid 1610743887;
    property EnableRealTimeUpdates: WordBool dispid 1745027403;
    property LazyOptimizationProgress: Smallint readonly dispid 1745027384;
    property ProcessingKeyErrorLogFileName: WideString dispid 1745027356;
    property ProcessingKeyErrorLimit: Integer dispid 1745027355;
    property ProcessOptimizationMode: ProcessOptimizationModes dispid 1745027354;
    property Commands: _OlapCollection readonly dispid 1745027299;
    property DataSources: _OlapCollection readonly dispid 1745027298;
    property MDStores: _OlapCollection readonly dispid 1745027297;
    property Roles: _OlapCollection readonly dispid 1745027296;
    property MiningModels: _OlapCollection readonly dispid 1745027295;
    property Analyzer: IDispatch readonly dispid 1745027294;
    property State: OlapStateTypes readonly dispid 1745027293;
    property IsValid: WordBool readonly dispid 1745027292;
    property LastProcessed: TDateTime readonly dispid 1745027291;
    property LastUpdated: TDateTime dispid 1745027290;
    property SourceTableAlias: WideString dispid 1745027289;
    property SourceTableFilter: WideString dispid 1745027288;
    property IsReadWrite: WordBool dispid 1745027287;
    property IsVisible: WordBool dispid 1745027286;
    property AllowDrillThrough: WordBool dispid 1745027285;
    property DrillThroughColumns: WideString dispid 1745027284;
    property DrillThroughFilter: WideString dispid 1745027283;
    property RemoteServer: WideString dispid 1745027282;
    property DrillThroughFrom: WideString dispid 1745027281;
    property DrillThroughJoins: WideString dispid 1745027280;
    property DefaultMeasure: WideString dispid 1745027279;
    procedure BeginTrans; dispid 1610809717;
    procedure CommitTrans; dispid 1610809718;
    procedure Rollback; dispid 1610809719;
    procedure Merge(const SourceName: WideString); dispid 1610809720;
    procedure Process(Options: ProcessTypes); dispid 1610809721;
    procedure Update; dispid 1610809722;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809723;
    procedure UnlockObject; dispid 1610809724;
    procedure GhostMethod__Aggregation_552_37; dispid 1610743939;
    procedure GhostMethod__Aggregation_556_38; dispid 1610743940;
    procedure GhostMethod__Aggregation_560_39; dispid 1610743941;
    procedure GhostMethod__Aggregation_564_40; dispid 1610743942;
    procedure GhostMethod__Aggregation_568_41; dispid 1610743943;
    procedure GhostMethod__Aggregation_572_42; dispid 1610743944;
    procedure GhostMethod__Aggregation_576_43; dispid 1610743945;
    procedure GhostMethod__Aggregation_580_44; dispid 1610743946;
    procedure GhostMethod__Aggregation_584_45; dispid 1610743947;
    procedure GhostMethod__Aggregation_588_46; dispid 1610743948;
    procedure GhostMethod__Aggregation_592_47; dispid 1610743949;
    procedure GhostMethod__Aggregation_596_48; dispid 1610743950;
    procedure CommitTransEx(Options: ProcessTypes); dispid 1610809866;
  end;

// *********************************************************************//
// Interface: _AggregationDimension
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5835-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationDimension = interface(IDispatch)
    ['{E8AC5835-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationDimension_28_1; safecall;
    procedure GhostMethod__AggregationDimension_32_2; safecall;
    procedure GhostMethod__AggregationDimension_36_3; safecall;
    procedure GhostMethod__AggregationDimension_40_4; safecall;
    procedure GhostMethod__AggregationDimension_44_5; safecall;
    procedure GhostMethod__AggregationDimension_48_6; safecall;
    procedure GhostMethod__AggregationDimension_52_7; safecall;
    procedure GhostMethod__AggregationDimension_56_8; safecall;
    procedure GhostMethod__AggregationDimension_60_9; safecall;
    procedure GhostMethod__AggregationDimension_64_10; safecall;
    procedure GhostMethod__AggregationDimension_68_11; safecall;
    procedure GhostMethod__AggregationDimension_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Aggregation: _Aggregation; safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Validate: ValidateErrorCodes; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Partition: _Partition; safecall;
    procedure _Set_Parent(const Param1: _Aggregation); safecall;
    function Get_Parent: _Aggregation; safecall;
    function Get_Levels: _OlapCollection; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDimension: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__AggregationDimension_184_13; safecall;
    procedure GhostMethod__AggregationDimension_188_14; safecall;
    procedure GhostMethod__AggregationDimension_192_15; safecall;
    procedure GhostMethod__AggregationDimension_196_16; safecall;
    procedure GhostMethod__AggregationDimension_200_17; safecall;
    procedure GhostMethod__AggregationDimension_204_18; safecall;
    procedure GhostMethod__AggregationDimension_208_19; safecall;
    procedure GhostMethod__AggregationDimension_212_20; safecall;
    procedure GhostMethod__AggregationDimension_216_21; safecall;
    procedure GhostMethod__AggregationDimension_220_22; safecall;
    procedure GhostMethod__AggregationDimension_224_23; safecall;
    procedure GhostMethod__AggregationDimension_228_24; safecall;
    function Get_IsTemporary: WordBool; safecall;
    procedure GhostMethod__AggregationDimension_236_25; safecall;
    procedure GhostMethod__AggregationDimension_240_26; safecall;
    procedure GhostMethod__AggregationDimension_244_27; safecall;
    procedure GhostMethod__AggregationDimension_248_28; safecall;
    procedure GhostMethod__AggregationDimension_252_29; safecall;
    procedure GhostMethod__AggregationDimension_256_30; safecall;
    procedure GhostMethod__AggregationDimension_260_31; safecall;
    procedure GhostMethod__AggregationDimension_264_32; safecall;
    procedure GhostMethod__AggregationDimension_268_33; safecall;
    procedure GhostMethod__AggregationDimension_272_34; safecall;
    procedure GhostMethod__AggregationDimension_276_35; safecall;
    procedure GhostMethod__AggregationDimension_280_36; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_SourceTableAlias: WideString; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_IsShared: WordBool; safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_DimensionType: DimensionTypes; safecall;
    procedure Set_DimensionType(Param1: DimensionTypes); safecall;
    function Get_DataSource: _DataSource; safecall;
    procedure _Set_DataSource(const Param1: _DataSource); safecall;
    function Get_AggregationUsage: DimensionAggUsageTypes; safecall;
    procedure Set_AggregationUsage(Param1: DimensionAggUsageTypes); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_IsChanging: WordBool; safecall;
    procedure Set_IsChanging(Param1: WordBool); safecall;
    function Get_StorageMode: StorageModeValues; safecall;
    procedure Set_StorageMode(Param1: StorageModeValues); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_DependsOnDimension: WideString; safecall;
    procedure Set_DependsOnDimension(const Param1: WideString); safecall;
    function Get_DefaultMember: WideString; safecall;
    procedure Set_DefaultMember(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_IsVirtual: WordBool; safecall;
    procedure Set_IsVirtual(Param1: WordBool); safecall;
    function Get_MembersWithData: MembersWithDataValues; safecall;
    procedure Set_MembersWithData(Param1: MembersWithDataValues); safecall;
    function Get_DataMemberCaptionTemplate: WideString; safecall;
    procedure Set_DataMemberCaptionTemplate(const Param1: WideString); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); safecall;
    function Get_AllowSiblingsWithSameName: WordBool; safecall;
    procedure Set_AllowSiblingsWithSameName(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Aggregation: _Aggregation read Get_Aggregation;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property Description: WideString read Get_Description write Set_Description;
    property Num: Smallint read Get_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Cube: _Cube read Get_Cube;
    property Partition: _Partition read Get_Partition;
    property Parent: _Aggregation read Get_Parent write _Set_Parent;
    property Levels: _OlapCollection read Get_Levels;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property IDimension: IDispatch read Get_IDimension;
    property IsTemporary: WordBool read Get_IsTemporary;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property SourceTable: WideString read Get_SourceTable;
    property SourceTableAlias: WideString read Get_SourceTableAlias;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property IsValid: WordBool read Get_IsValid;
    property IsShared: WordBool read Get_IsShared;
    property State: OlapStateTypes read Get_State;
    property DimensionType: DimensionTypes read Get_DimensionType write Set_DimensionType;
    property DataSource: _DataSource read Get_DataSource write _Set_DataSource;
    property AggregationUsage: DimensionAggUsageTypes read Get_AggregationUsage write Set_AggregationUsage;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property IsChanging: WordBool read Get_IsChanging write Set_IsChanging;
    property StorageMode: StorageModeValues read Get_StorageMode write Set_StorageMode;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property DependsOnDimension: WideString read Get_DependsOnDimension write Set_DependsOnDimension;
    property DefaultMember: WideString read Get_DefaultMember write Set_DefaultMember;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property IsVirtual: WordBool read Get_IsVirtual write Set_IsVirtual;
    property MembersWithData: MembersWithDataValues read Get_MembersWithData write Set_MembersWithData;
    property DataMemberCaptionTemplate: WideString read Get_DataMemberCaptionTemplate write Set_DataMemberCaptionTemplate;
    property AllowSiblingsWithSameName: WordBool read Get_AllowSiblingsWithSameName write Set_AllowSiblingsWithSameName;
  end;

// *********************************************************************//
// DispIntf:  _AggregationDimensionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5835-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationDimensionDisp = dispinterface
    ['{E8AC5835-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationDimension_28_1; dispid 1610743808;
    procedure GhostMethod__AggregationDimension_32_2; dispid 1610743809;
    procedure GhostMethod__AggregationDimension_36_3; dispid 1610743810;
    procedure GhostMethod__AggregationDimension_40_4; dispid 1610743811;
    procedure GhostMethod__AggregationDimension_44_5; dispid 1610743812;
    procedure GhostMethod__AggregationDimension_48_6; dispid 1610743813;
    procedure GhostMethod__AggregationDimension_52_7; dispid 1610743814;
    procedure GhostMethod__AggregationDimension_56_8; dispid 1610743815;
    procedure GhostMethod__AggregationDimension_60_9; dispid 1610743816;
    procedure GhostMethod__AggregationDimension_64_10; dispid 1610743817;
    procedure GhostMethod__AggregationDimension_68_11; dispid 1610743818;
    procedure GhostMethod__AggregationDimension_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027147;
    property Aggregation: _Aggregation readonly dispid 1745027142;
    property FromClause: WideString dispid 1745027140;
    property JoinClause: WideString dispid 1745027139;
    property Valid: WordBool dispid 1745027138;
    function Validate: ValidateErrorCodes; dispid 1610809428;
    property Name: WideString dispid 1745027137;
    property Description: WideString dispid 1745027132;
    property Num: Smallint readonly dispid 1745027131;
    property Database: _Database readonly dispid 1745027130;
    property Server: _Server readonly dispid 1745027129;
    property Cube: _Cube readonly dispid 1745027128;
    property Partition: _Partition readonly dispid 1745027127;
    property Parent: _Aggregation dispid 1745027126;
    property Levels: _OlapCollection readonly dispid 1745027125;
    property Path: WideString readonly dispid 1745027123;
    property ClassType: Smallint readonly dispid 1745027118;
    property SubClassType: SubClassTypes readonly dispid 1745027117;
    property IObject: IDispatch readonly dispid 1745027116;
    property IDimension: IDispatch readonly dispid 1745027115;
    procedure ClearCollections; dispid 1610809435;
    procedure GhostMethod__AggregationDimension_184_13; dispid 1610743847;
    procedure GhostMethod__AggregationDimension_188_14; dispid 1610743848;
    procedure GhostMethod__AggregationDimension_192_15; dispid 1610743849;
    procedure GhostMethod__AggregationDimension_196_16; dispid 1610743850;
    procedure GhostMethod__AggregationDimension_200_17; dispid 1610743851;
    procedure GhostMethod__AggregationDimension_204_18; dispid 1610743852;
    procedure GhostMethod__AggregationDimension_208_19; dispid 1610743853;
    procedure GhostMethod__AggregationDimension_212_20; dispid 1610743854;
    procedure GhostMethod__AggregationDimension_216_21; dispid 1610743855;
    procedure GhostMethod__AggregationDimension_220_22; dispid 1610743856;
    procedure GhostMethod__AggregationDimension_224_23; dispid 1610743857;
    procedure GhostMethod__AggregationDimension_228_24; dispid 1610743858;
    property IsTemporary: WordBool readonly dispid 1745027211;
    procedure GhostMethod__AggregationDimension_236_25; dispid 1610743860;
    procedure GhostMethod__AggregationDimension_240_26; dispid 1610743861;
    procedure GhostMethod__AggregationDimension_244_27; dispid 1610743862;
    procedure GhostMethod__AggregationDimension_248_28; dispid 1610743863;
    procedure GhostMethod__AggregationDimension_252_29; dispid 1610743864;
    procedure GhostMethod__AggregationDimension_256_30; dispid 1610743865;
    procedure GhostMethod__AggregationDimension_260_31; dispid 1610743866;
    procedure GhostMethod__AggregationDimension_264_32; dispid 1610743867;
    procedure GhostMethod__AggregationDimension_268_33; dispid 1610743868;
    procedure GhostMethod__AggregationDimension_272_34; dispid 1610743869;
    procedure GhostMethod__AggregationDimension_276_35; dispid 1610743870;
    procedure GhostMethod__AggregationDimension_280_36; dispid 1610743871;
    property EnableRealTimeUpdates: WordBool dispid 1745027322;
    property OrdinalPosition: Smallint readonly dispid 1745027235;
    property SourceTable: WideString readonly dispid 1745027234;
    property SourceTableAlias: WideString readonly dispid 1745027233;
    property LastUpdated: TDateTime dispid 1745027232;
    property LastProcessed: TDateTime readonly dispid 1745027231;
    property IsValid: WordBool readonly dispid 1745027230;
    property IsShared: WordBool readonly dispid 1745027229;
    property State: OlapStateTypes readonly dispid 1745027228;
    property DimensionType: DimensionTypes dispid 1745027227;
    property DataSource: _DataSource dispid 1745027226;
    property AggregationUsage: DimensionAggUsageTypes dispid 1745027225;
    property IsVisible: WordBool dispid 1745027224;
    property IsChanging: WordBool dispid 1745027223;
    property StorageMode: StorageModeValues dispid 1745027222;
    property IsReadWrite: WordBool dispid 1745027221;
    property DependsOnDimension: WideString dispid 1745027220;
    property DefaultMember: WideString dispid 1745027219;
    property SourceTableFilter: WideString dispid 1745027218;
    property AreMemberNamesUnique: WordBool dispid 1745027217;
    property AreMemberKeysUnique: WordBool dispid 1745027216;
    property IsVirtual: WordBool dispid 1745027215;
    property MembersWithData: MembersWithDataValues dispid 1745027214;
    property DataMemberCaptionTemplate: WideString dispid 1745027213;
    procedure Process(Options: ProcessTypes); dispid 1610809625;
    procedure Update; dispid 1610809626;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809627;
    procedure UnlockObject; dispid 1610809628;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); dispid 1610809629;
    property AllowSiblingsWithSameName: WordBool dispid 1745027212;
  end;

// *********************************************************************//
// Interface: _AggregationLevel
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5834-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationLevel = interface(IDispatch)
    ['{E8AC5834-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationLevel_28_1; safecall;
    procedure GhostMethod__AggregationLevel_32_2; safecall;
    procedure GhostMethod__AggregationLevel_36_3; safecall;
    procedure GhostMethod__AggregationLevel_40_4; safecall;
    procedure GhostMethod__AggregationLevel_44_5; safecall;
    procedure GhostMethod__AggregationLevel_48_6; safecall;
    procedure GhostMethod__AggregationLevel_52_7; safecall;
    procedure GhostMethod__AggregationLevel_56_8; safecall;
    procedure GhostMethod__AggregationLevel_60_9; safecall;
    procedure GhostMethod__AggregationLevel_64_10; safecall;
    procedure GhostMethod__AggregationLevel_68_11; safecall;
    procedure GhostMethod__AggregationLevel_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Aggregation: _Aggregation; safecall;
    function Get_FromClauseAlias: WideString; safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_MemberNameColumn: WideString; safecall;
    function Get_MemberKeyColumn: WideString; safecall;
    procedure Set_MemberKeyColumn(const Param1: WideString); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_Ordering: OrderTypes; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_AggregationColumn: WideString; safecall;
    function Get_MemberKeyTable: WideString; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Path: WideString; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _AggregationDimension); safecall;
    function Get_Parent: _AggregationDimension; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Num: Smallint; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ILevel: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__AggregationLevel_200_13; safecall;
    procedure GhostMethod__AggregationLevel_204_14; safecall;
    procedure GhostMethod__AggregationLevel_208_15; safecall;
    procedure GhostMethod__AggregationLevel_212_16; safecall;
    procedure GhostMethod__AggregationLevel_216_17; safecall;
    procedure GhostMethod__AggregationLevel_220_18; safecall;
    procedure GhostMethod__AggregationLevel_224_19; safecall;
    procedure GhostMethod__AggregationLevel_228_20; safecall;
    procedure GhostMethod__AggregationLevel_232_21; safecall;
    procedure GhostMethod__AggregationLevel_236_22; safecall;
    procedure GhostMethod__AggregationLevel_240_23; safecall;
    procedure GhostMethod__AggregationLevel_244_24; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(Param1: Smallint); safecall;
    function Get_ColumnSize: Smallint; safecall;
    procedure Set_ColumnSize(Param1: Smallint); safecall;
    function Get_EstimatedSize: Integer; safecall;
    procedure Set_EstimatedSize(Param1: Integer); safecall;
    function Get_LevelType: LevelTypes; safecall;
    procedure Set_LevelType(Param1: LevelTypes); safecall;
    procedure Set_MemberNameColumn(const Param1: WideString); safecall;
    function Get_IsDisabled: WordBool; safecall;
    procedure Set_IsDisabled(Param1: WordBool); safecall;
    function Get_IsUnique: WordBool; safecall;
    procedure Set_IsUnique(Param1: WordBool); safecall;
    function Get_SliceValue: WideString; safecall;
    procedure Set_SliceValue(const Param1: WideString); safecall;
    procedure Set_Ordering(Param1: OrderTypes); safecall;
    function Get_EnableAggregations: WordBool; safecall;
    procedure Set_EnableAggregations(Param1: WordBool); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_ParentKeyColumn: WideString; safecall;
    procedure Set_ParentKeyColumn(const Param1: WideString); safecall;
    function Get_LevelNamingTemplate: WideString; safecall;
    procedure Set_LevelNamingTemplate(const Param1: WideString); safecall;
    function Get_HideMemberIf: HideIfValues; safecall;
    procedure Set_HideMemberIf(Param1: HideIfValues); safecall;
    function Get_OrderingMemberProperty: WideString; safecall;
    procedure Set_OrderingMemberProperty(const Param1: WideString); safecall;
    function Get_CustomRollupExpression: WideString; safecall;
    procedure Set_CustomRollupExpression(const Param1: WideString); safecall;
    function Get_Grouping: GroupingValues; safecall;
    procedure Set_Grouping(Param1: GroupingValues); safecall;
    function Get_SkippedLevelsColumn: WideString; safecall;
    procedure Set_SkippedLevelsColumn(const Param1: WideString); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_CustomRollupColumn: WideString; safecall;
    procedure Set_CustomRollupColumn(const Param1: WideString); safecall;
    procedure Set_CustomRollupPropertiesColumn(const Param1: WideString); safecall;
    function Get_CustomRollupPropertiesColumn: WideString; safecall;
    function Get_RootMemberIf: RootIfValues; safecall;
    procedure Set_RootMemberIf(Param1: RootIfValues); safecall;
    function Get_MemberProperties: _OlapCollection; safecall;
    function Get_UnaryOperatorColumn: WideString; safecall;
    procedure Set_UnaryOperatorColumn(const Param1: WideString); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Aggregation: _Aggregation read Get_Aggregation;
    property FromClauseAlias: WideString read Get_FromClauseAlias;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property MemberNameColumn: WideString read Get_MemberNameColumn write Set_MemberNameColumn;
    property MemberKeyColumn: WideString read Get_MemberKeyColumn write Set_MemberKeyColumn;
    property Description: WideString read Get_Description write Set_Description;
    property Ordering: OrderTypes read Get_Ordering write Set_Ordering;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property AggregationColumn: WideString read Get_AggregationColumn;
    property MemberKeyTable: WideString read Get_MemberKeyTable;
    property Cube: _Cube read Get_Cube;
    property Path: WideString read Get_Path;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _AggregationDimension read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property ILevel: IDispatch read Get_ILevel;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property EstimatedSize: Integer read Get_EstimatedSize write Set_EstimatedSize;
    property LevelType: LevelTypes read Get_LevelType write Set_LevelType;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property IsUnique: WordBool read Get_IsUnique write Set_IsUnique;
    property SliceValue: WideString read Get_SliceValue write Set_SliceValue;
    property EnableAggregations: WordBool read Get_EnableAggregations write Set_EnableAggregations;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property ParentKeyColumn: WideString read Get_ParentKeyColumn write Set_ParentKeyColumn;
    property LevelNamingTemplate: WideString read Get_LevelNamingTemplate write Set_LevelNamingTemplate;
    property HideMemberIf: HideIfValues read Get_HideMemberIf write Set_HideMemberIf;
    property OrderingMemberProperty: WideString read Get_OrderingMemberProperty write Set_OrderingMemberProperty;
    property CustomRollupExpression: WideString read Get_CustomRollupExpression write Set_CustomRollupExpression;
    property Grouping: GroupingValues read Get_Grouping write Set_Grouping;
    property SkippedLevelsColumn: WideString read Get_SkippedLevelsColumn write Set_SkippedLevelsColumn;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property CustomRollupColumn: WideString read Get_CustomRollupColumn write Set_CustomRollupColumn;
    property CustomRollupPropertiesColumn: WideString read Get_CustomRollupPropertiesColumn write Set_CustomRollupPropertiesColumn;
    property RootMemberIf: RootIfValues read Get_RootMemberIf write Set_RootMemberIf;
    property MemberProperties: _OlapCollection read Get_MemberProperties;
    property UnaryOperatorColumn: WideString read Get_UnaryOperatorColumn write Set_UnaryOperatorColumn;
  end;

// *********************************************************************//
// DispIntf:  _AggregationLevelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5834-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationLevelDisp = dispinterface
    ['{E8AC5834-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationLevel_28_1; dispid 1610743808;
    procedure GhostMethod__AggregationLevel_32_2; dispid 1610743809;
    procedure GhostMethod__AggregationLevel_36_3; dispid 1610743810;
    procedure GhostMethod__AggregationLevel_40_4; dispid 1610743811;
    procedure GhostMethod__AggregationLevel_44_5; dispid 1610743812;
    procedure GhostMethod__AggregationLevel_48_6; dispid 1610743813;
    procedure GhostMethod__AggregationLevel_52_7; dispid 1610743814;
    procedure GhostMethod__AggregationLevel_56_8; dispid 1610743815;
    procedure GhostMethod__AggregationLevel_60_9; dispid 1610743816;
    procedure GhostMethod__AggregationLevel_64_10; dispid 1610743817;
    procedure GhostMethod__AggregationLevel_68_11; dispid 1610743818;
    procedure GhostMethod__AggregationLevel_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027158;
    property Aggregation: _Aggregation readonly dispid 1745027155;
    property FromClauseAlias: WideString readonly dispid 1745027153;
    property FromClause: WideString dispid 1745027152;
    property JoinClause: WideString dispid 1745027151;
    property MemberNameColumn: WideString dispid 1745027150;
    property MemberKeyColumn: WideString dispid 1745027149;
    property Description: WideString dispid 1745027147;
    property Ordering: OrderTypes dispid 1745027146;
    property Valid: WordBool dispid 1745027144;
    property AggregationColumn: WideString readonly dispid 1745027143;
    property MemberKeyTable: WideString readonly dispid 1745027142;
    property Cube: _Cube readonly dispid 1745027141;
    property Path: WideString readonly dispid 1745027140;
    property Database: _Database readonly dispid 1745027138;
    property Server: _Server readonly dispid 1745027137;
    property Parent: _AggregationDimension dispid 1745027136;
    property Name: WideString dispid 1745027135;
    property Num: Smallint readonly dispid 1745027134;
    property ClassType: Smallint readonly dispid 1745027133;
    property SubClassType: SubClassTypes readonly dispid 1745027132;
    property IObject: IDispatch readonly dispid 1745027130;
    property ILevel: IDispatch readonly dispid 1745027129;
    procedure ClearCollections; dispid 1610809438;
    procedure GhostMethod__AggregationLevel_200_13; dispid 1610743851;
    procedure GhostMethod__AggregationLevel_204_14; dispid 1610743852;
    procedure GhostMethod__AggregationLevel_208_15; dispid 1610743853;
    procedure GhostMethod__AggregationLevel_212_16; dispid 1610743854;
    procedure GhostMethod__AggregationLevel_216_17; dispid 1610743855;
    procedure GhostMethod__AggregationLevel_220_18; dispid 1610743856;
    procedure GhostMethod__AggregationLevel_224_19; dispid 1610743857;
    procedure GhostMethod__AggregationLevel_228_20; dispid 1610743858;
    procedure GhostMethod__AggregationLevel_232_21; dispid 1610743859;
    procedure GhostMethod__AggregationLevel_236_22; dispid 1610743860;
    procedure GhostMethod__AggregationLevel_240_23; dispid 1610743861;
    procedure GhostMethod__AggregationLevel_244_24; dispid 1610743862;
    property OrdinalPosition: Smallint readonly dispid 1745027191;
    property IsValid: WordBool readonly dispid 1745027190;
    property ColumnType: Smallint dispid 1745027189;
    property ColumnSize: Smallint dispid 1745027188;
    property EstimatedSize: Integer dispid 1745027187;
    property LevelType: LevelTypes dispid 1745027186;
    property IsDisabled: WordBool dispid 1745027185;
    property IsUnique: WordBool dispid 1745027184;
    property SliceValue: WideString dispid 1745027183;
    property EnableAggregations: WordBool dispid 1745027182;
    property IsVisible: WordBool dispid 1745027181;
    property ParentKeyColumn: WideString dispid 1745027180;
    property LevelNamingTemplate: WideString dispid 1745027179;
    property HideMemberIf: HideIfValues dispid 1745027178;
    property OrderingMemberProperty: WideString dispid 1745027177;
    property CustomRollupExpression: WideString dispid 1745027176;
    property Grouping: GroupingValues dispid 1745027175;
    property SkippedLevelsColumn: WideString dispid 1745027174;
    property AreMemberKeysUnique: WordBool dispid 1745027173;
    property AreMemberNamesUnique: WordBool dispid 1745027172;
    property CustomRollupColumn: WideString dispid 1745027171;
    property CustomRollupPropertiesColumn: WideString dispid 1745027170;
    property RootMemberIf: RootIfValues dispid 1745027169;
    property MemberProperties: _OlapCollection readonly dispid 1745027168;
    property UnaryOperatorColumn: WideString dispid 1745027167;
  end;

// *********************************************************************//
// Interface: _AggregationMeasure
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5836-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationMeasure = interface(IDispatch)
    ['{E8AC5836-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationMeasure_28_1; safecall;
    procedure GhostMethod__AggregationMeasure_32_2; safecall;
    procedure GhostMethod__AggregationMeasure_36_3; safecall;
    procedure GhostMethod__AggregationMeasure_40_4; safecall;
    procedure GhostMethod__AggregationMeasure_44_5; safecall;
    procedure GhostMethod__AggregationMeasure_48_6; safecall;
    procedure GhostMethod__AggregationMeasure_52_7; safecall;
    procedure GhostMethod__AggregationMeasure_56_8; safecall;
    procedure GhostMethod__AggregationMeasure_60_9; safecall;
    procedure GhostMethod__AggregationMeasure_64_10; safecall;
    procedure GhostMethod__AggregationMeasure_68_11; safecall;
    procedure GhostMethod__AggregationMeasure_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_AggregationColumn: WideString; safecall;
    function Get_Valid: WordBool; safecall;
    function Get_SourceField: WideString; safecall;
    procedure Set_SourceField(var Param1: WideString); safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Parent: _Aggregation; safecall;
    procedure _Set_Parent(const Param1: _Aggregation); safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IMeasure: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__AggregationMeasure_160_13; safecall;
    procedure GhostMethod__AggregationMeasure_164_14; safecall;
    procedure GhostMethod__AggregationMeasure_168_15; safecall;
    procedure GhostMethod__AggregationMeasure_172_16; safecall;
    procedure GhostMethod__AggregationMeasure_176_17; safecall;
    procedure GhostMethod__AggregationMeasure_180_18; safecall;
    procedure GhostMethod__AggregationMeasure_184_19; safecall;
    procedure GhostMethod__AggregationMeasure_188_20; safecall;
    procedure GhostMethod__AggregationMeasure_192_21; safecall;
    procedure GhostMethod__AggregationMeasure_196_22; safecall;
    procedure GhostMethod__AggregationMeasure_200_23; safecall;
    procedure GhostMethod__AggregationMeasure_204_24; safecall;
    function Get_AggregateFunction: AggregatesTypes; safecall;
    procedure GhostMethod__AggregationMeasure_212_25; safecall;
    procedure GhostMethod__AggregationMeasure_216_26; safecall;
    procedure GhostMethod__AggregationMeasure_220_27; safecall;
    procedure GhostMethod__AggregationMeasure_224_28; safecall;
    procedure GhostMethod__AggregationMeasure_228_29; safecall;
    procedure GhostMethod__AggregationMeasure_232_30; safecall;
    procedure GhostMethod__AggregationMeasure_236_31; safecall;
    procedure GhostMethod__AggregationMeasure_240_32; safecall;
    procedure GhostMethod__AggregationMeasure_244_33; safecall;
    procedure GhostMethod__AggregationMeasure_248_34; safecall;
    procedure GhostMethod__AggregationMeasure_252_35; safecall;
    procedure GhostMethod__AggregationMeasure_256_36; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    procedure Set_AggregateFunction(Param1: AggregatesTypes); safecall;
    function Get_FormatString: WideString; safecall;
    procedure Set_FormatString(const Param1: WideString); safecall;
    function Get_IsInternal: WordBool; safecall;
    procedure Set_IsInternal(Param1: WordBool); safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_SourceColumn(const Param1: WideString); safecall;
    function Get_SourceColumnType: Smallint; safecall;
    procedure Set_SourceColumnType(Param1: Smallint); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property AggregationColumn: WideString read Get_AggregationColumn;
    property SourceField: WideString read Get_SourceField write Set_SourceField;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Aggregation read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property IMeasure: IDispatch read Get_IMeasure;
    property AggregateFunction: AggregatesTypes read Get_AggregateFunction write Set_AggregateFunction;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property FormatString: WideString read Get_FormatString write Set_FormatString;
    property IsInternal: WordBool read Get_IsInternal write Set_IsInternal;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property SourceColumnType: Smallint read Get_SourceColumnType write Set_SourceColumnType;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
  end;

// *********************************************************************//
// DispIntf:  _AggregationMeasureDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5836-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _AggregationMeasureDisp = dispinterface
    ['{E8AC5836-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__AggregationMeasure_28_1; dispid 1610743808;
    procedure GhostMethod__AggregationMeasure_32_2; dispid 1610743809;
    procedure GhostMethod__AggregationMeasure_36_3; dispid 1610743810;
    procedure GhostMethod__AggregationMeasure_40_4; dispid 1610743811;
    procedure GhostMethod__AggregationMeasure_44_5; dispid 1610743812;
    procedure GhostMethod__AggregationMeasure_48_6; dispid 1610743813;
    procedure GhostMethod__AggregationMeasure_52_7; dispid 1610743814;
    procedure GhostMethod__AggregationMeasure_56_8; dispid 1610743815;
    procedure GhostMethod__AggregationMeasure_60_9; dispid 1610743816;
    procedure GhostMethod__AggregationMeasure_64_10; dispid 1610743817;
    procedure GhostMethod__AggregationMeasure_68_11; dispid 1610743818;
    procedure GhostMethod__AggregationMeasure_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027148;
    property Description: WideString dispid 1745027144;
    property Valid: WordBool dispid 1745027139;
    property AggregationColumn: WideString readonly dispid 1745027138;
    property SourceField: WideString dispid 1745027136;
    property Name: WideString dispid 1745027135;
    property Num: Smallint dispid 1745027134;
    property Database: _Database readonly dispid 1745027133;
    property Server: _Server readonly dispid 1745027132;
    property Parent: _Aggregation dispid 1745027131;
    property Path: WideString readonly dispid 1745027129;
    property ClassType: Smallint readonly dispid 1745027128;
    property SubClassType: SubClassTypes readonly dispid 1745027127;
    property IObject: IDispatch readonly dispid 1745027126;
    property IMeasure: IDispatch readonly dispid 1745027125;
    procedure ClearCollections; dispid 1610809428;
    procedure GhostMethod__AggregationMeasure_160_13; dispid 1610743841;
    procedure GhostMethod__AggregationMeasure_164_14; dispid 1610743842;
    procedure GhostMethod__AggregationMeasure_168_15; dispid 1610743843;
    procedure GhostMethod__AggregationMeasure_172_16; dispid 1610743844;
    procedure GhostMethod__AggregationMeasure_176_17; dispid 1610743845;
    procedure GhostMethod__AggregationMeasure_180_18; dispid 1610743846;
    procedure GhostMethod__AggregationMeasure_184_19; dispid 1610743847;
    procedure GhostMethod__AggregationMeasure_188_20; dispid 1610743848;
    procedure GhostMethod__AggregationMeasure_192_21; dispid 1610743849;
    procedure GhostMethod__AggregationMeasure_196_22; dispid 1610743850;
    procedure GhostMethod__AggregationMeasure_200_23; dispid 1610743851;
    procedure GhostMethod__AggregationMeasure_204_24; dispid 1610743852;
    property AggregateFunction: AggregatesTypes dispid 1745027214;
    procedure GhostMethod__AggregationMeasure_212_25; dispid 1610743854;
    procedure GhostMethod__AggregationMeasure_216_26; dispid 1610743855;
    procedure GhostMethod__AggregationMeasure_220_27; dispid 1610743856;
    procedure GhostMethod__AggregationMeasure_224_28; dispid 1610743857;
    procedure GhostMethod__AggregationMeasure_228_29; dispid 1610743858;
    procedure GhostMethod__AggregationMeasure_232_30; dispid 1610743859;
    procedure GhostMethod__AggregationMeasure_236_31; dispid 1610743860;
    procedure GhostMethod__AggregationMeasure_240_32; dispid 1610743861;
    procedure GhostMethod__AggregationMeasure_244_33; dispid 1610743862;
    procedure GhostMethod__AggregationMeasure_248_34; dispid 1610743863;
    procedure GhostMethod__AggregationMeasure_252_35; dispid 1610743864;
    procedure GhostMethod__AggregationMeasure_256_36; dispid 1610743865;
    property OrdinalPosition: Smallint readonly dispid 1745027221;
    property IsValid: WordBool readonly dispid 1745027220;
    property FormatString: WideString dispid 1745027219;
    property IsInternal: WordBool dispid 1745027218;
    property SourceColumn: WideString dispid 1745027217;
    property SourceColumnType: Smallint dispid 1745027216;
    property IsVisible: WordBool dispid 1745027215;
  end;

// *********************************************************************//
// Interface: _Cube
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5831-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Cube = interface(IDispatch)
    ['{E8AC5831-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Cube_28_1; safecall;
    procedure GhostMethod__Cube_32_2; safecall;
    procedure GhostMethod__Cube_36_3; safecall;
    procedure GhostMethod__Cube_40_4; safecall;
    procedure GhostMethod__Cube_44_5; safecall;
    procedure GhostMethod__Cube_48_6; safecall;
    procedure GhostMethod__Cube_52_7; safecall;
    procedure GhostMethod__Cube_56_8; safecall;
    procedure GhostMethod__Cube_60_9; safecall;
    procedure GhostMethod__Cube_64_10; safecall;
    procedure GhostMethod__Cube_68_11; safecall;
    procedure GhostMethod__Cube_72_12; safecall;
    procedure Clone(const TargetObject: _Cube; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    function SaveObject(var AssumeInsert: WordBool): WordBool; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_IsVirtual: WordBool; safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Dimensions: _OlapCollection; safecall;
    procedure _Set_Dimensions(const Param1: _OlapCollection); safecall;
    function Get_Measures: _OlapCollection; safecall;
    procedure _Set_Measures(const Param1: _OlapCollection); safecall;
    function Get_Database: _Database; safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_Partitions: _OlapCollection; safecall;
    function Get_Groups: _OlapCollection; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_Analyzer: _CubeAnalyzer; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure _Set_Parent(const Param1: _Database); safecall;
    function Get_Parent: _Database; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Path: WideString; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_DataSource: WideString; safecall;
    procedure Set_DataSource(var Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_FactTable: WideString; safecall;
    procedure Set_FactTable(var Param1: WideString); safecall;
    function Get_FactTableSize: Integer; safecall;
    procedure Set_FactTableSize(var Param1: Integer); safecall;
    function Get_EstimatedSize: Double; safecall;
    function Get_Where: WideString; safecall;
    procedure Set_Where(var Param1: WideString); safecall;
    function Get_IsActivated: WordBool; safecall;
    function Get_IsReadWrite: WordBool; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    procedure CreateRandomRequests; safecall;
    function Validate: ValidateErrorCodes; safecall;
    procedure ProcessCache(ProcessOption: ProcessTypes); safecall;
    procedure Process(ProcessOption: ProcessTypes); safecall;
    function Get_Committed: WordBool; safecall;
    procedure Set_Committed(var Param1: WordBool); safecall;
    function RemoveDeletedPartitionsFromServer(var cb: IMSOLAPModel): WordBool; safecall;
    function MergePartitions(var SourcePartitionName: WideString; 
                             var TargetPartitionName: WideString; var checkPartitions: WordBool): WordBool; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ICube: IDispatch; safecall;
    procedure ClearCollections; safecall;
    function Get_UnderlayingCubes: _OlapCollection; safecall;
    procedure GhostMethod__Cube_308_13; safecall;
    procedure GhostMethod__Cube_312_14; safecall;
    procedure GhostMethod__Cube_316_15; safecall;
    procedure GhostMethod__Cube_320_16; safecall;
    procedure GhostMethod__Cube_324_17; safecall;
    procedure GhostMethod__Cube_328_18; safecall;
    procedure GhostMethod__Cube_332_19; safecall;
    procedure GhostMethod__Cube_336_20; safecall;
    procedure GhostMethod__Cube_340_21; safecall;
    procedure GhostMethod__Cube_344_22; safecall;
    procedure GhostMethod__Cube_348_23; safecall;
    procedure GhostMethod__Cube_352_24; safecall;
    function Get_Server: _Server; safecall;
    procedure Set_AggregationPrefix(const Param1: WideString); safecall;
    function Get_AggregationPrefix: WideString; safecall;
    procedure GhostMethod__Cube_368_25; safecall;
    procedure GhostMethod__Cube_372_26; safecall;
    procedure GhostMethod__Cube_376_27; safecall;
    procedure GhostMethod__Cube_380_28; safecall;
    procedure GhostMethod__Cube_384_29; safecall;
    procedure GhostMethod__Cube_388_30; safecall;
    procedure GhostMethod__Cube_392_31; safecall;
    procedure GhostMethod__Cube_396_32; safecall;
    procedure GhostMethod__Cube_400_33; safecall;
    procedure GhostMethod__Cube_404_34; safecall;
    procedure GhostMethod__Cube_408_35; safecall;
    procedure GhostMethod__Cube_412_36; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_AllowDrillThrough: WordBool; safecall;
    procedure Set_AllowDrillThrough(Param1: WordBool); safecall;
    function Get_DrillThroughColumns: WideString; safecall;
    procedure Set_DrillThroughColumns(const Param1: WideString); safecall;
    function Get_DrillThroughFilter: WideString; safecall;
    procedure Set_DrillThroughFilter(const Param1: WideString); safecall;
    function Get_DrillThroughFrom: WideString; safecall;
    procedure Set_DrillThroughFrom(const Param1: WideString); safecall;
    function Get_DrillThroughJoins: WideString; safecall;
    procedure Set_DrillThroughJoins(const Param1: WideString); safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_DefaultMeasure: WideString; safecall;
    procedure Set_DefaultMeasure(const Param1: WideString); safecall;
    function Get_LazyOptimizationProgress: Smallint; safecall;
    procedure Set_ProcessingKeyErrorLogFileName(const Param1: WideString); safecall;
    function Get_ProcessingKeyErrorLogFileName: WideString; safecall;
    procedure Set_ProcessingKeyErrorLimit(Param1: Integer); safecall;
    function Get_ProcessingKeyErrorLimit: Integer; safecall;
    procedure Set_ProcessOptimizationMode(Param1: ProcessOptimizationModes); safecall;
    function Get_ProcessOptimizationMode: ProcessOptimizationModes; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_MiningModels: _OlapCollection; safecall;
    function Get_EstimatedRows: Double; safecall;
    procedure Set_EstimatedRows(Param1: Double); safecall;
    function Get_IsDefault: WordBool; safecall;
    procedure Set_IsDefault(Param1: WordBool); safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_OlapMode: OlapStorageModes; safecall;
    procedure Set_OlapMode(Param1: OlapStorageModes); safecall;
    function Get_SourceTable: WideString; safecall;
    procedure Set_SourceTable(const Param1: WideString); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    procedure Set_SourceTableAlias(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_RemoteServer: WideString; safecall;
    procedure Set_RemoteServer(const Param1: WideString); safecall;
    procedure BeginTrans; safecall;
    procedure CommitTrans; safecall;
    procedure Rollback; safecall;
    procedure Merge(const SourceName: WideString); safecall;
    procedure Update; safecall;
    procedure GhostMethod__Cube_612_37; safecall;
    procedure GhostMethod__Cube_616_38; safecall;
    procedure GhostMethod__Cube_620_39; safecall;
    procedure GhostMethod__Cube_624_40; safecall;
    procedure GhostMethod__Cube_628_41; safecall;
    procedure GhostMethod__Cube_632_42; safecall;
    procedure GhostMethod__Cube_636_43; safecall;
    procedure GhostMethod__Cube_640_44; safecall;
    procedure GhostMethod__Cube_644_45; safecall;
    procedure GhostMethod__Cube_648_46; safecall;
    procedure GhostMethod__Cube_652_47; safecall;
    procedure GhostMethod__Cube_656_48; safecall;
    procedure CommitTransEx(Options: ProcessTypes); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property IsVirtual: WordBool read Get_IsVirtual;
    property State: OlapStateTypes read Get_State;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Dimensions: _OlapCollection read Get_Dimensions write _Set_Dimensions;
    property Measures: _OlapCollection read Get_Measures write _Set_Measures;
    property Database: _Database read Get_Database;
    property IsTemporary: WordBool read Get_IsTemporary;
    property Partitions: _OlapCollection read Get_Partitions;
    property Groups: _OlapCollection read Get_Groups;
    property Commands: _OlapCollection read Get_Commands;
    property Analyzer: _CubeAnalyzer read Get_Analyzer;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property Parent: _Database read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property Path: WideString read Get_Path;
    property IsValid: WordBool read Get_IsValid;
    property DataSource: WideString read Get_DataSource write Set_DataSource;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FactTable: WideString read Get_FactTable write Set_FactTable;
    property FactTableSize: Integer read Get_FactTableSize write Set_FactTableSize;
    property EstimatedSize: Double read Get_EstimatedSize;
    property Where: WideString read Get_Where write Set_Where;
    property IsActivated: WordBool read Get_IsActivated;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property Committed: WordBool read Get_Committed write Set_Committed;
    property IObject: IDispatch read Get_IObject;
    property ICube: IDispatch read Get_ICube;
    property UnderlayingCubes: _OlapCollection read Get_UnderlayingCubes;
    property Server: _Server read Get_Server;
    property AggregationPrefix: WideString read Get_AggregationPrefix write Set_AggregationPrefix;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property AllowDrillThrough: WordBool read Get_AllowDrillThrough write Set_AllowDrillThrough;
    property DrillThroughColumns: WideString read Get_DrillThroughColumns write Set_DrillThroughColumns;
    property DrillThroughFilter: WideString read Get_DrillThroughFilter write Set_DrillThroughFilter;
    property DrillThroughFrom: WideString read Get_DrillThroughFrom write Set_DrillThroughFrom;
    property DrillThroughJoins: WideString read Get_DrillThroughJoins write Set_DrillThroughJoins;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property DefaultMeasure: WideString read Get_DefaultMeasure write Set_DefaultMeasure;
    property LazyOptimizationProgress: Smallint read Get_LazyOptimizationProgress;
    property ProcessingKeyErrorLogFileName: WideString read Get_ProcessingKeyErrorLogFileName write Set_ProcessingKeyErrorLogFileName;
    property ProcessingKeyErrorLimit: Integer read Get_ProcessingKeyErrorLimit write Set_ProcessingKeyErrorLimit;
    property ProcessOptimizationMode: ProcessOptimizationModes read Get_ProcessOptimizationMode write Set_ProcessOptimizationMode;
    property DataSources: _OlapCollection read Get_DataSources;
    property MDStores: _OlapCollection read Get_MDStores;
    property Roles: _OlapCollection read Get_Roles;
    property MiningModels: _OlapCollection read Get_MiningModels;
    property EstimatedRows: Double read Get_EstimatedRows write Set_EstimatedRows;
    property IsDefault: WordBool read Get_IsDefault write Set_IsDefault;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property OlapMode: OlapStorageModes read Get_OlapMode write Set_OlapMode;
    property SourceTable: WideString read Get_SourceTable write Set_SourceTable;
    property SourceTableAlias: WideString read Get_SourceTableAlias write Set_SourceTableAlias;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property RemoteServer: WideString read Get_RemoteServer write Set_RemoteServer;
  end;

// *********************************************************************//
// DispIntf:  _CubeDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5831-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeDisp = dispinterface
    ['{E8AC5831-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Cube_28_1; dispid 1610743808;
    procedure GhostMethod__Cube_32_2; dispid 1610743809;
    procedure GhostMethod__Cube_36_3; dispid 1610743810;
    procedure GhostMethod__Cube_40_4; dispid 1610743811;
    procedure GhostMethod__Cube_44_5; dispid 1610743812;
    procedure GhostMethod__Cube_48_6; dispid 1610743813;
    procedure GhostMethod__Cube_52_7; dispid 1610743814;
    procedure GhostMethod__Cube_56_8; dispid 1610743815;
    procedure GhostMethod__Cube_60_9; dispid 1610743816;
    procedure GhostMethod__Cube_64_10; dispid 1610743817;
    procedure GhostMethod__Cube_68_11; dispid 1610743818;
    procedure GhostMethod__Cube_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _Cube; Options: CloneOptions); dispid 1610809455;
    property CustomProperties: _Properties readonly dispid 1745027179;
    function SaveObject(var AssumeInsert: WordBool): WordBool; dispid 1610809457;
    property Description: WideString dispid 1745027175;
    property IsVirtual: WordBool readonly dispid 1745027174;
    property State: OlapStateTypes readonly dispid 1745027173;
    property Valid: WordBool dispid 1745027172;
    property Dimensions: _OlapCollection dispid 1745027171;
    property Measures: _OlapCollection dispid 1745027170;
    property Database: _Database readonly dispid 1745027169;
    property IsTemporary: WordBool readonly dispid 1745027166;
    property Partitions: _OlapCollection readonly dispid 1745027165;
    property Groups: _OlapCollection readonly dispid 1745027164;
    property Commands: _OlapCollection readonly dispid 1745027163;
    property Analyzer: _CubeAnalyzer readonly dispid 1745027162;
    property LastProcessed: TDateTime readonly dispid 1745027161;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809461;
    procedure UnlockObject; dispid 1610809462;
    property Parent: _Database dispid 1745027156;
    property Name: WideString dispid 1745027155;
    property Path: WideString readonly dispid 1745027153;
    property IsValid: WordBool readonly dispid 1745027152;
    property DataSource: WideString dispid 1745027149;
    property FromClause: WideString dispid 1745027148;
    property JoinClause: WideString dispid 1745027147;
    property FactTable: WideString dispid 1745027146;
    property FactTableSize: Integer dispid 1745027144;
    property EstimatedSize: Double readonly dispid 1745027143;
    property Where: WideString dispid 1745027142;
    property IsActivated: WordBool readonly dispid 1745027141;
    property IsReadWrite: WordBool dispid 1745027140;
    property ClassType: ClassTypes readonly dispid 1745027139;
    property SubClassType: SubClassTypes dispid 1745027138;
    procedure CreateRandomRequests; dispid 1610809491;
    function Validate: ValidateErrorCodes; dispid 1610809492;
    procedure ProcessCache(ProcessOption: ProcessTypes); dispid 1610809493;
    procedure Process(ProcessOption: ProcessTypes); dispid 1610809494;
    property Committed: WordBool dispid 1745027137;
    function RemoveDeletedPartitionsFromServer(var cb: IMSOLAPModel): WordBool; dispid 1610809508;
    function MergePartitions(var SourcePartitionName: WideString; 
                             var TargetPartitionName: WideString; var checkPartitions: WordBool): WordBool; dispid 1610809509;
    property IObject: IDispatch readonly dispid 1745027136;
    property ICube: IDispatch readonly dispid 1745027135;
    procedure ClearCollections; dispid 1610809511;
    property UnderlayingCubes: _OlapCollection readonly dispid 1745027111;
    procedure GhostMethod__Cube_308_13; dispid 1610743878;
    procedure GhostMethod__Cube_312_14; dispid 1610743879;
    procedure GhostMethod__Cube_316_15; dispid 1610743880;
    procedure GhostMethod__Cube_320_16; dispid 1610743881;
    procedure GhostMethod__Cube_324_17; dispid 1610743882;
    procedure GhostMethod__Cube_328_18; dispid 1610743883;
    procedure GhostMethod__Cube_332_19; dispid 1610743884;
    procedure GhostMethod__Cube_336_20; dispid 1610743885;
    procedure GhostMethod__Cube_340_21; dispid 1610743886;
    procedure GhostMethod__Cube_344_22; dispid 1610743887;
    procedure GhostMethod__Cube_348_23; dispid 1610743888;
    procedure GhostMethod__Cube_352_24; dispid 1610743889;
    property Server: _Server readonly dispid 1745027311;
    property AggregationPrefix: WideString dispid 1745027304;
    procedure GhostMethod__Cube_368_25; dispid 1610743893;
    procedure GhostMethod__Cube_372_26; dispid 1610743894;
    procedure GhostMethod__Cube_376_27; dispid 1610743895;
    procedure GhostMethod__Cube_380_28; dispid 1610743896;
    procedure GhostMethod__Cube_384_29; dispid 1610743897;
    procedure GhostMethod__Cube_388_30; dispid 1610743898;
    procedure GhostMethod__Cube_392_31; dispid 1610743899;
    procedure GhostMethod__Cube_396_32; dispid 1610743900;
    procedure GhostMethod__Cube_400_33; dispid 1610743901;
    procedure GhostMethod__Cube_404_34; dispid 1610743902;
    procedure GhostMethod__Cube_408_35; dispid 1610743903;
    procedure GhostMethod__Cube_412_36; dispid 1610743904;
    property IsVisible: WordBool dispid 1745027438;
    property AllowDrillThrough: WordBool dispid 1745027437;
    property DrillThroughColumns: WideString dispid 1745027436;
    property DrillThroughFilter: WideString dispid 1745027435;
    property DrillThroughFrom: WideString dispid 1745027434;
    property DrillThroughJoins: WideString dispid 1745027433;
    property EnableRealTimeUpdates: WordBool dispid 1745027432;
    property DefaultMeasure: WideString dispid 1745027430;
    property LazyOptimizationProgress: Smallint readonly dispid 1745027414;
    property ProcessingKeyErrorLogFileName: WideString dispid 1745027409;
    property ProcessingKeyErrorLimit: Integer dispid 1745027408;
    property ProcessOptimizationMode: ProcessOptimizationModes dispid 1745027407;
    property DataSources: _OlapCollection readonly dispid 1745027323;
    property MDStores: _OlapCollection readonly dispid 1745027322;
    property Roles: _OlapCollection readonly dispid 1745027321;
    property MiningModels: _OlapCollection readonly dispid 1745027320;
    property EstimatedRows: Double dispid 1745027319;
    property IsDefault: WordBool dispid 1745027318;
    property LastUpdated: TDateTime dispid 1745027317;
    property OlapMode: OlapStorageModes dispid 1745027316;
    property SourceTable: WideString dispid 1745027315;
    property SourceTableAlias: WideString dispid 1745027314;
    property SourceTableFilter: WideString dispid 1745027313;
    property RemoteServer: WideString dispid 1745027312;
    procedure BeginTrans; dispid 1610809797;
    procedure CommitTrans; dispid 1610809798;
    procedure Rollback; dispid 1610809799;
    procedure Merge(const SourceName: WideString); dispid 1610809800;
    procedure Update; dispid 1610809801;
    procedure GhostMethod__Cube_612_37; dispid 1610743954;
    procedure GhostMethod__Cube_616_38; dispid 1610743955;
    procedure GhostMethod__Cube_620_39; dispid 1610743956;
    procedure GhostMethod__Cube_624_40; dispid 1610743957;
    procedure GhostMethod__Cube_628_41; dispid 1610743958;
    procedure GhostMethod__Cube_632_42; dispid 1610743959;
    procedure GhostMethod__Cube_636_43; dispid 1610743960;
    procedure GhostMethod__Cube_640_44; dispid 1610743961;
    procedure GhostMethod__Cube_644_45; dispid 1610743962;
    procedure GhostMethod__Cube_648_46; dispid 1610743963;
    procedure GhostMethod__Cube_652_47; dispid 1610743964;
    procedure GhostMethod__Cube_656_48; dispid 1610743965;
    procedure CommitTransEx(Options: ProcessTypes); dispid 1610809993;
  end;

// *********************************************************************//
// Interface: _CubeCommand
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582F-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeCommand = interface(IDispatch)
    ['{E8AC582F-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeCommand_28_1; safecall;
    procedure GhostMethod__CubeCommand_32_2; safecall;
    procedure GhostMethod__CubeCommand_36_3; safecall;
    procedure GhostMethod__CubeCommand_40_4; safecall;
    procedure GhostMethod__CubeCommand_44_5; safecall;
    procedure GhostMethod__CubeCommand_48_6; safecall;
    procedure GhostMethod__CubeCommand_52_7; safecall;
    procedure GhostMethod__CubeCommand_56_8; safecall;
    procedure GhostMethod__CubeCommand_60_9; safecall;
    procedure GhostMethod__CubeCommand_64_10; safecall;
    procedure GhostMethod__CubeCommand_68_11; safecall;
    procedure GhostMethod__CubeCommand_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Statement: WideString; safecall;
    procedure Set_Statement(const Param1: WideString); safecall;
    procedure Set_CommandType(Param1: CommandTypes); safecall;
    function Get_CommandType: CommandTypes; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Parent: _Cube; safecall;
    procedure _Set_Parent(const Param1: _Cube); safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    function Get_IObject: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__CubeCommand_172_13; safecall;
    procedure GhostMethod__CubeCommand_176_14; safecall;
    procedure GhostMethod__CubeCommand_180_15; safecall;
    procedure GhostMethod__CubeCommand_184_16; safecall;
    procedure GhostMethod__CubeCommand_188_17; safecall;
    procedure GhostMethod__CubeCommand_192_18; safecall;
    procedure GhostMethod__CubeCommand_196_19; safecall;
    procedure GhostMethod__CubeCommand_200_20; safecall;
    procedure GhostMethod__CubeCommand_204_21; safecall;
    procedure GhostMethod__CubeCommand_208_22; safecall;
    procedure GhostMethod__CubeCommand_212_23; safecall;
    procedure GhostMethod__CubeCommand_216_24; safecall;
    function Get_ParentObject: IDispatch; safecall;
    procedure _Set_ParentObject(const Param1: IDispatch); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property Statement: WideString read Get_Statement write Set_Statement;
    property CommandType: CommandTypes read Get_CommandType write Set_CommandType;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Cube read Get_Parent write _Set_Parent;
    property Cube: _Cube read Get_Cube;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property ParentObject: IDispatch read Get_ParentObject write _Set_ParentObject;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _CubeCommandDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582F-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeCommandDisp = dispinterface
    ['{E8AC582F-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeCommand_28_1; dispid 1610743808;
    procedure GhostMethod__CubeCommand_32_2; dispid 1610743809;
    procedure GhostMethod__CubeCommand_36_3; dispid 1610743810;
    procedure GhostMethod__CubeCommand_40_4; dispid 1610743811;
    procedure GhostMethod__CubeCommand_44_5; dispid 1610743812;
    procedure GhostMethod__CubeCommand_48_6; dispid 1610743813;
    procedure GhostMethod__CubeCommand_52_7; dispid 1610743814;
    procedure GhostMethod__CubeCommand_56_8; dispid 1610743815;
    procedure GhostMethod__CubeCommand_60_9; dispid 1610743816;
    procedure GhostMethod__CubeCommand_64_10; dispid 1610743817;
    procedure GhostMethod__CubeCommand_68_11; dispid 1610743818;
    procedure GhostMethod__CubeCommand_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027123;
    property Description: WideString dispid 1745027120;
    property Valid: WordBool dispid 1745027118;
    property Name: WideString dispid 1745027117;
    property Statement: WideString dispid 1745027116;
    property CommandType: CommandTypes dispid 1745027115;
    property Num: Smallint dispid 1745027114;
    property Database: _Database readonly dispid 1745027113;
    property Server: _Server readonly dispid 1745027112;
    property Parent: _Cube dispid 1745027111;
    property Cube: _Cube readonly dispid 1745027109;
    property Path: WideString readonly dispid 1745027108;
    property ClassType: Smallint readonly dispid 1745027107;
    property SubClassType: SubClassTypes dispid 1745027106;
    property IObject: IDispatch readonly dispid 1745027105;
    procedure ClearCollections; dispid 1610809406;
    procedure GhostMethod__CubeCommand_172_13; dispid 1610743844;
    procedure GhostMethod__CubeCommand_176_14; dispid 1610743845;
    procedure GhostMethod__CubeCommand_180_15; dispid 1610743846;
    procedure GhostMethod__CubeCommand_184_16; dispid 1610743847;
    procedure GhostMethod__CubeCommand_188_17; dispid 1610743848;
    procedure GhostMethod__CubeCommand_192_18; dispid 1610743849;
    procedure GhostMethod__CubeCommand_196_19; dispid 1610743850;
    procedure GhostMethod__CubeCommand_200_20; dispid 1610743851;
    procedure GhostMethod__CubeCommand_204_21; dispid 1610743852;
    procedure GhostMethod__CubeCommand_208_22; dispid 1610743853;
    procedure GhostMethod__CubeCommand_212_23; dispid 1610743854;
    procedure GhostMethod__CubeCommand_216_24; dispid 1610743855;
    property ParentObject: IDispatch dispid 1745027174;
    property OrdinalPosition: Smallint readonly dispid 1745027136;
    property IsValid: WordBool readonly dispid 1745027135;
    procedure Update; dispid 1610809471;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809472;
    procedure UnlockObject; dispid 1610809473;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); dispid 1610809474;
  end;

// *********************************************************************//
// Interface: _CubeDimension
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582C-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeDimension = interface(IDispatch)
    ['{E8AC582C-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeDimension_28_1; safecall;
    procedure GhostMethod__CubeDimension_32_2; safecall;
    procedure GhostMethod__CubeDimension_36_3; safecall;
    procedure GhostMethod__CubeDimension_40_4; safecall;
    procedure GhostMethod__CubeDimension_44_5; safecall;
    procedure GhostMethod__CubeDimension_48_6; safecall;
    procedure GhostMethod__CubeDimension_52_7; safecall;
    procedure GhostMethod__CubeDimension_56_8; safecall;
    procedure GhostMethod__CubeDimension_60_9; safecall;
    procedure GhostMethod__CubeDimension_64_10; safecall;
    procedure GhostMethod__CubeDimension_68_11; safecall;
    procedure GhostMethod__CubeDimension_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Database: _Database; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_FromClause: WideString; safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_Server: _Server; safecall;
    function Get_Cube: _Cube; safecall;
    procedure _Set_Parent(const Param1: _Cube); safecall;
    function Get_Parent: _Cube; safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Name: WideString; safecall;
    function Get_Levels: _OlapCollection; safecall;
    procedure _Set_Levels(const Param1: _OlapCollection); safecall;
    function Get_Path: WideString; safecall;
    function Get_AggregationUsage: DimensionAggUsageTypes; safecall;
    procedure Set_AggregationUsage(Param1: DimensionAggUsageTypes); safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_DimensionType: DimensionTypes; safecall;
    function Get_IsShared: WordBool; safecall;
    function Get_Huge: Smallint; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDimension: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__CubeDimension_204_13; safecall;
    procedure GhostMethod__CubeDimension_208_14; safecall;
    procedure GhostMethod__CubeDimension_212_15; safecall;
    procedure GhostMethod__CubeDimension_216_16; safecall;
    procedure GhostMethod__CubeDimension_220_17; safecall;
    procedure GhostMethod__CubeDimension_224_18; safecall;
    procedure GhostMethod__CubeDimension_228_19; safecall;
    procedure GhostMethod__CubeDimension_232_20; safecall;
    procedure GhostMethod__CubeDimension_236_21; safecall;
    procedure GhostMethod__CubeDimension_240_22; safecall;
    procedure GhostMethod__CubeDimension_244_23; safecall;
    procedure GhostMethod__CubeDimension_248_24; safecall;
    function Get_IsTemporary: WordBool; safecall;
    procedure GhostMethod__CubeDimension_256_25; safecall;
    procedure GhostMethod__CubeDimension_260_26; safecall;
    procedure GhostMethod__CubeDimension_264_27; safecall;
    procedure GhostMethod__CubeDimension_268_28; safecall;
    procedure GhostMethod__CubeDimension_272_29; safecall;
    procedure GhostMethod__CubeDimension_276_30; safecall;
    procedure GhostMethod__CubeDimension_280_31; safecall;
    procedure GhostMethod__CubeDimension_284_32; safecall;
    procedure GhostMethod__CubeDimension_288_33; safecall;
    procedure GhostMethod__CubeDimension_292_34; safecall;
    procedure GhostMethod__CubeDimension_296_35; safecall;
    procedure GhostMethod__CubeDimension_300_36; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_IsVirtual: WordBool; safecall;
    function Get_SourceTableAlias: WideString; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_State: OlapStateTypes; safecall;
    procedure Set_DimensionType(Param1: DimensionTypes); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_DataSource: _DataSource; safecall;
    procedure _Set_DataSource(const Param1: _DataSource); safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_IsChanging: WordBool; safecall;
    procedure Set_IsChanging(Param1: WordBool); safecall;
    function Get_StorageMode: StorageModeValues; safecall;
    procedure Set_StorageMode(Param1: StorageModeValues); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_DependsOnDimension: WideString; safecall;
    procedure Set_DependsOnDimension(const Param1: WideString); safecall;
    function Get_DefaultMember: WideString; safecall;
    procedure Set_DefaultMember(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    procedure Set_IsVirtual(Param1: WordBool); safecall;
    function Get_MembersWithData: MembersWithDataValues; safecall;
    procedure Set_MembersWithData(Param1: MembersWithDataValues); safecall;
    function Get_DataMemberCaptionTemplate: WideString; safecall;
    procedure Set_DataMemberCaptionTemplate(const Param1: WideString); safecall;
    procedure Process(Options: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); safecall;
    function Get_AllowSiblingsWithSameName: WordBool; safecall;
    procedure Set_AllowSiblingsWithSameName(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Database: _Database read Get_Database;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property SourceTable: WideString read Get_SourceTable;
    property Server: _Server read Get_Server;
    property Cube: _Cube read Get_Cube;
    property Parent: _Cube read Get_Parent write _Set_Parent;
    property Num: Smallint read Get_Num write Set_Num;
    property Name: WideString read Get_Name write Set_Name;
    property Levels: _OlapCollection read Get_Levels write _Set_Levels;
    property Path: WideString read Get_Path;
    property AggregationUsage: DimensionAggUsageTypes read Get_AggregationUsage write Set_AggregationUsage;
    property IsValid: WordBool read Get_IsValid;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property DimensionType: DimensionTypes read Get_DimensionType write Set_DimensionType;
    property IsShared: WordBool read Get_IsShared;
    property Huge: Smallint read Get_Huge;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property IObject: IDispatch read Get_IObject;
    property IDimension: IDispatch read Get_IDimension;
    property IsTemporary: WordBool read Get_IsTemporary;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property IsVirtual: WordBool read Get_IsVirtual write Set_IsVirtual;
    property SourceTableAlias: WideString read Get_SourceTableAlias;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property State: OlapStateTypes read Get_State;
    property DataSource: _DataSource read Get_DataSource write _Set_DataSource;
    property IsChanging: WordBool read Get_IsChanging write Set_IsChanging;
    property StorageMode: StorageModeValues read Get_StorageMode write Set_StorageMode;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property DependsOnDimension: WideString read Get_DependsOnDimension write Set_DependsOnDimension;
    property DefaultMember: WideString read Get_DefaultMember write Set_DefaultMember;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property MembersWithData: MembersWithDataValues read Get_MembersWithData write Set_MembersWithData;
    property DataMemberCaptionTemplate: WideString read Get_DataMemberCaptionTemplate write Set_DataMemberCaptionTemplate;
    property AllowSiblingsWithSameName: WordBool read Get_AllowSiblingsWithSameName write Set_AllowSiblingsWithSameName;
  end;

// *********************************************************************//
// DispIntf:  _CubeDimensionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582C-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeDimensionDisp = dispinterface
    ['{E8AC582C-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeDimension_28_1; dispid 1610743808;
    procedure GhostMethod__CubeDimension_32_2; dispid 1610743809;
    procedure GhostMethod__CubeDimension_36_3; dispid 1610743810;
    procedure GhostMethod__CubeDimension_40_4; dispid 1610743811;
    procedure GhostMethod__CubeDimension_44_5; dispid 1610743812;
    procedure GhostMethod__CubeDimension_48_6; dispid 1610743813;
    procedure GhostMethod__CubeDimension_52_7; dispid 1610743814;
    procedure GhostMethod__CubeDimension_56_8; dispid 1610743815;
    procedure GhostMethod__CubeDimension_60_9; dispid 1610743816;
    procedure GhostMethod__CubeDimension_64_10; dispid 1610743817;
    procedure GhostMethod__CubeDimension_68_11; dispid 1610743818;
    procedure GhostMethod__CubeDimension_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027148;
    property Description: WideString dispid 1745027144;
    property Valid: WordBool dispid 1745027143;
    property Database: _Database readonly dispid 1745027142;
    property JoinClause: WideString dispid 1745027141;
    property FromClause: WideString dispid 1745027140;
    property SourceTable: WideString readonly dispid 1745027139;
    property Server: _Server readonly dispid 1745027138;
    property Cube: _Cube readonly dispid 1745027137;
    property Parent: _Cube dispid 1745027135;
    property Num: Smallint dispid 1745027133;
    property Name: WideString dispid 1745027132;
    property Levels: _OlapCollection dispid 1745027131;
    property Path: WideString readonly dispid 1745027130;
    property AggregationUsage: DimensionAggUsageTypes dispid 1745027129;
    property IsValid: WordBool readonly dispid 1745027128;
    property ClassType: Smallint readonly dispid 1745027127;
    property SubClassType: SubClassTypes readonly dispid 1745027126;
    property DimensionType: DimensionTypes dispid 1745027125;
    property IsShared: WordBool readonly dispid 1745027124;
    property Huge: Smallint readonly dispid 1745027123;
    property LastUpdated: TDateTime dispid 1745027122;
    property LastProcessed: TDateTime readonly dispid 1745027121;
    property IObject: IDispatch readonly dispid 1745027120;
    property IDimension: IDispatch readonly dispid 1745027119;
    procedure ClearCollections; dispid 1610809436;
    procedure GhostMethod__CubeDimension_204_13; dispid 1610743852;
    procedure GhostMethod__CubeDimension_208_14; dispid 1610743853;
    procedure GhostMethod__CubeDimension_212_15; dispid 1610743854;
    procedure GhostMethod__CubeDimension_216_16; dispid 1610743855;
    procedure GhostMethod__CubeDimension_220_17; dispid 1610743856;
    procedure GhostMethod__CubeDimension_224_18; dispid 1610743857;
    procedure GhostMethod__CubeDimension_228_19; dispid 1610743858;
    procedure GhostMethod__CubeDimension_232_20; dispid 1610743859;
    procedure GhostMethod__CubeDimension_236_21; dispid 1610743860;
    procedure GhostMethod__CubeDimension_240_22; dispid 1610743861;
    procedure GhostMethod__CubeDimension_244_23; dispid 1610743862;
    procedure GhostMethod__CubeDimension_248_24; dispid 1610743863;
    property IsTemporary: WordBool readonly dispid 1745027213;
    procedure GhostMethod__CubeDimension_256_25; dispid 1610743865;
    procedure GhostMethod__CubeDimension_260_26; dispid 1610743866;
    procedure GhostMethod__CubeDimension_264_27; dispid 1610743867;
    procedure GhostMethod__CubeDimension_268_28; dispid 1610743868;
    procedure GhostMethod__CubeDimension_272_29; dispid 1610743869;
    procedure GhostMethod__CubeDimension_276_30; dispid 1610743870;
    procedure GhostMethod__CubeDimension_280_31; dispid 1610743871;
    procedure GhostMethod__CubeDimension_284_32; dispid 1610743872;
    procedure GhostMethod__CubeDimension_288_33; dispid 1610743873;
    procedure GhostMethod__CubeDimension_292_34; dispid 1610743874;
    procedure GhostMethod__CubeDimension_296_35; dispid 1610743875;
    procedure GhostMethod__CubeDimension_300_36; dispid 1610743876;
    property IsVisible: WordBool dispid 1745027321;
    property IsVirtual: WordBool dispid 1745027320;
    property SourceTableAlias: WideString readonly dispid 1745027316;
    property EnableRealTimeUpdates: WordBool dispid 1745027314;
    property OrdinalPosition: Smallint readonly dispid 1745027227;
    property State: OlapStateTypes readonly dispid 1745027226;
    property DataSource: _DataSource dispid 1745027225;
    property IsChanging: WordBool dispid 1745027224;
    property StorageMode: StorageModeValues dispid 1745027223;
    property IsReadWrite: WordBool dispid 1745027222;
    property DependsOnDimension: WideString dispid 1745027221;
    property DefaultMember: WideString dispid 1745027220;
    property SourceTableFilter: WideString dispid 1745027219;
    property AreMemberNamesUnique: WordBool dispid 1745027218;
    property AreMemberKeysUnique: WordBool dispid 1745027217;
    property MembersWithData: MembersWithDataValues dispid 1745027216;
    property DataMemberCaptionTemplate: WideString dispid 1745027215;
    procedure Process(Options: ProcessTypes); dispid 1610809616;
    procedure Update; dispid 1610809617;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809618;
    procedure UnlockObject; dispid 1610809619;
    procedure Clone(const TargetObject: _Dimension; Options: CloneOptions); dispid 1610809620;
    property AllowSiblingsWithSameName: WordBool dispid 1745027214;
  end;

// *********************************************************************//
// Interface: _CubeGroup
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582E-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeGroup = interface(IDispatch)
    ['{E8AC582E-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeGroup_28_1; safecall;
    procedure GhostMethod__CubeGroup_32_2; safecall;
    procedure GhostMethod__CubeGroup_36_3; safecall;
    procedure GhostMethod__CubeGroup_40_4; safecall;
    procedure GhostMethod__CubeGroup_44_5; safecall;
    procedure GhostMethod__CubeGroup_48_6; safecall;
    procedure GhostMethod__CubeGroup_52_7; safecall;
    procedure GhostMethod__CubeGroup_56_8; safecall;
    procedure GhostMethod__CubeGroup_60_9; safecall;
    procedure GhostMethod__CubeGroup_64_10; safecall;
    procedure GhostMethod__CubeGroup_68_11; safecall;
    procedure GhostMethod__CubeGroup_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_UsersList: WideString; safecall;
    function Get_Permissions(const Key: WideString): WideString; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Parent: _Cube; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IObject: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__CubeGroup_152_13; safecall;
    procedure GhostMethod__CubeGroup_156_14; safecall;
    procedure GhostMethod__CubeGroup_160_15; safecall;
    procedure GhostMethod__CubeGroup_164_16; safecall;
    procedure GhostMethod__CubeGroup_168_17; safecall;
    procedure GhostMethod__CubeGroup_172_18; safecall;
    procedure GhostMethod__CubeGroup_176_19; safecall;
    procedure GhostMethod__CubeGroup_180_20; safecall;
    procedure GhostMethod__CubeGroup_184_21; safecall;
    procedure GhostMethod__CubeGroup_188_22; safecall;
    procedure GhostMethod__CubeGroup_192_23; safecall;
    procedure GhostMethod__CubeGroup_196_24; safecall;
    function SetPermissions(const Key: WideString; const Rights: WideString): WordBool; safecall;
    procedure GhostMethod__CubeGroup_204_25; safecall;
    procedure GhostMethod__CubeGroup_208_26; safecall;
    procedure GhostMethod__CubeGroup_212_27; safecall;
    procedure GhostMethod__CubeGroup_216_28; safecall;
    procedure GhostMethod__CubeGroup_220_29; safecall;
    procedure GhostMethod__CubeGroup_224_30; safecall;
    procedure GhostMethod__CubeGroup_228_31; safecall;
    procedure GhostMethod__CubeGroup_232_32; safecall;
    procedure GhostMethod__CubeGroup_236_33; safecall;
    procedure GhostMethod__CubeGroup_240_34; safecall;
    procedure GhostMethod__CubeGroup_244_35; safecall;
    procedure GhostMethod__CubeGroup_248_36; safecall;
    function Get_ParentObject: IDispatch; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    procedure Set_UsersList(const Param1: WideString); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property UsersList: WideString read Get_UsersList write Set_UsersList;
    property Permissions[const Key: WideString]: WideString read Get_Permissions;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Cube read Get_Parent;
    property Cube: _Cube read Get_Cube;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property ParentObject: IDispatch read Get_ParentObject;
    property Commands: _OlapCollection read Get_Commands;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _CubeGroupDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582E-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeGroupDisp = dispinterface
    ['{E8AC582E-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeGroup_28_1; dispid 1610743808;
    procedure GhostMethod__CubeGroup_32_2; dispid 1610743809;
    procedure GhostMethod__CubeGroup_36_3; dispid 1610743810;
    procedure GhostMethod__CubeGroup_40_4; dispid 1610743811;
    procedure GhostMethod__CubeGroup_44_5; dispid 1610743812;
    procedure GhostMethod__CubeGroup_48_6; dispid 1610743813;
    procedure GhostMethod__CubeGroup_52_7; dispid 1610743814;
    procedure GhostMethod__CubeGroup_56_8; dispid 1610743815;
    procedure GhostMethod__CubeGroup_60_9; dispid 1610743816;
    procedure GhostMethod__CubeGroup_64_10; dispid 1610743817;
    procedure GhostMethod__CubeGroup_68_11; dispid 1610743818;
    procedure GhostMethod__CubeGroup_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027127;
    property Description: WideString dispid 1745027123;
    property Valid: WordBool dispid 1745027121;
    property Name: WideString dispid 1745027120;
    property UsersList: WideString dispid 1745027119;
    property Permissions[const Key: WideString]: WideString readonly dispid 1745027118;
    property Num: Smallint dispid 1745027117;
    property Database: _Database readonly dispid 1745027116;
    property Server: _Server readonly dispid 1745027115;
    property Parent: _Cube readonly dispid 1745027122;
    property Cube: _Cube readonly dispid 1745027113;
    property Path: WideString readonly dispid 1745027111;
    property ClassType: Smallint readonly dispid 1745027110;
    property SubClassType: SubClassTypes readonly dispid 1745027109;
    property IObject: IDispatch readonly dispid 1745027108;
    procedure ClearCollections; dispid 1610809413;
    procedure GhostMethod__CubeGroup_152_13; dispid 1610743839;
    procedure GhostMethod__CubeGroup_156_14; dispid 1610743840;
    procedure GhostMethod__CubeGroup_160_15; dispid 1610743841;
    procedure GhostMethod__CubeGroup_164_16; dispid 1610743842;
    procedure GhostMethod__CubeGroup_168_17; dispid 1610743843;
    procedure GhostMethod__CubeGroup_172_18; dispid 1610743844;
    procedure GhostMethod__CubeGroup_176_19; dispid 1610743845;
    procedure GhostMethod__CubeGroup_180_20; dispid 1610743846;
    procedure GhostMethod__CubeGroup_184_21; dispid 1610743847;
    procedure GhostMethod__CubeGroup_188_22; dispid 1610743848;
    procedure GhostMethod__CubeGroup_192_23; dispid 1610743849;
    procedure GhostMethod__CubeGroup_196_24; dispid 1610743850;
    function SetPermissions(const Key: WideString; const Rights: WideString): WordBool; dispid 1610809461;
    procedure GhostMethod__CubeGroup_204_25; dispid 1610743852;
    procedure GhostMethod__CubeGroup_208_26; dispid 1610743853;
    procedure GhostMethod__CubeGroup_212_27; dispid 1610743854;
    procedure GhostMethod__CubeGroup_216_28; dispid 1610743855;
    procedure GhostMethod__CubeGroup_220_29; dispid 1610743856;
    procedure GhostMethod__CubeGroup_224_30; dispid 1610743857;
    procedure GhostMethod__CubeGroup_228_31; dispid 1610743858;
    procedure GhostMethod__CubeGroup_232_32; dispid 1610743859;
    procedure GhostMethod__CubeGroup_236_33; dispid 1610743860;
    procedure GhostMethod__CubeGroup_240_34; dispid 1610743861;
    procedure GhostMethod__CubeGroup_244_35; dispid 1610743862;
    procedure GhostMethod__CubeGroup_248_36; dispid 1610743863;
    property ParentObject: IDispatch readonly dispid 1745027252;
    property Commands: _OlapCollection readonly dispid 1745027248;
    property IsValid: WordBool readonly dispid 1745027190;
    procedure Update; dispid 1610809561;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809562;
    procedure UnlockObject; dispid 1610809563;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); dispid 1610809564;
  end;

// *********************************************************************//
// Interface: _CubeLevel
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582B-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeLevel = interface(IDispatch)
    ['{E8AC582B-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeLevel_28_1; safecall;
    procedure GhostMethod__CubeLevel_32_2; safecall;
    procedure GhostMethod__CubeLevel_36_3; safecall;
    procedure GhostMethod__CubeLevel_40_4; safecall;
    procedure GhostMethod__CubeLevel_44_5; safecall;
    procedure GhostMethod__CubeLevel_48_6; safecall;
    procedure GhostMethod__CubeLevel_52_7; safecall;
    procedure GhostMethod__CubeLevel_56_8; safecall;
    procedure GhostMethod__CubeLevel_60_9; safecall;
    procedure GhostMethod__CubeLevel_64_10; safecall;
    procedure GhostMethod__CubeLevel_68_11; safecall;
    procedure GhostMethod__CubeLevel_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Validate: ValidateErrorCodes; safecall;
    procedure Set_UniqueItems(var Param1: WordBool); safecall;
    function Get_FromClause: WideString; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_MemberNameColumn: WideString; safecall;
    function Get_MemberKeyTable: WideString; safecall;
    function Get_MemberKeyColumn: WideString; safecall;
    procedure Set_MemberKeyColumn(const Param1: WideString); safecall;
    procedure Set_IsDisabled(Param1: WordBool); safecall;
    function Get_IsDisabled: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_EnableAggregations: WordBool; safecall;
    procedure Set_EnableAggregations(Param1: WordBool); safecall;
    function Get_Path: WideString; safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _CubeDimension); safecall;
    function Get_Parent: _CubeDimension; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_UniqueItems: WordBool; safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(Param1: Smallint); safecall;
    function Get_EstimatedSize: Integer; safecall;
    procedure Set_EstimatedSize(Param1: Integer); safecall;
    function Get_Num: Smallint; safecall;
    function Get_IsAll: WordBool; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_ColumnSize: Smallint; safecall;
    function Get_Ordering: OrderTypes; safecall;
    procedure Set_ColumnSize(Param1: Smallint); safecall;
    function Get_LevelType: LevelTypes; safecall;
    procedure Set_LevelType(Param1: LevelTypes); safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ILevel: IDispatch; safecall;
    function Get_MemberProperties: _OlapCollection; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__CubeLevel_248_13; safecall;
    procedure GhostMethod__CubeLevel_252_14; safecall;
    procedure GhostMethod__CubeLevel_256_15; safecall;
    procedure GhostMethod__CubeLevel_260_16; safecall;
    procedure GhostMethod__CubeLevel_264_17; safecall;
    procedure GhostMethod__CubeLevel_268_18; safecall;
    procedure GhostMethod__CubeLevel_272_19; safecall;
    procedure GhostMethod__CubeLevel_276_20; safecall;
    procedure GhostMethod__CubeLevel_280_21; safecall;
    procedure GhostMethod__CubeLevel_284_22; safecall;
    procedure GhostMethod__CubeLevel_288_23; safecall;
    procedure GhostMethod__CubeLevel_292_24; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_CustomRollupExpression: WideString; safecall;
    procedure Set_CustomRollupExpression(const Param1: WideString); safecall;
    function Get_Grouping: GroupingValues; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_MemberNameColumn(const Param1: WideString); safecall;
    function Get_IsUnique: WordBool; safecall;
    procedure Set_IsUnique(Param1: WordBool); safecall;
    function Get_SliceValue: WideString; safecall;
    procedure Set_SliceValue(const Param1: WideString); safecall;
    procedure Set_Ordering(Param1: OrderTypes); safecall;
    function Get_ParentKeyColumn: WideString; safecall;
    procedure Set_ParentKeyColumn(const Param1: WideString); safecall;
    function Get_LevelNamingTemplate: WideString; safecall;
    procedure Set_LevelNamingTemplate(const Param1: WideString); safecall;
    function Get_HideMemberIf: HideIfValues; safecall;
    procedure Set_HideMemberIf(Param1: HideIfValues); safecall;
    function Get_OrderingMemberProperty: WideString; safecall;
    procedure Set_OrderingMemberProperty(const Param1: WideString); safecall;
    procedure Set_Grouping(Param1: GroupingValues); safecall;
    function Get_SkippedLevelsColumn: WideString; safecall;
    procedure Set_SkippedLevelsColumn(const Param1: WideString); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_CustomRollupColumn: WideString; safecall;
    procedure Set_CustomRollupColumn(const Param1: WideString); safecall;
    procedure Set_CustomRollupPropertiesColumn(const Param1: WideString); safecall;
    function Get_CustomRollupPropertiesColumn: WideString; safecall;
    function Get_RootMemberIf: RootIfValues; safecall;
    procedure Set_RootMemberIf(Param1: RootIfValues); safecall;
    function Get_UnaryOperatorColumn: WideString; safecall;
    procedure Set_UnaryOperatorColumn(const Param1: WideString); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property UniqueItems: WordBool read Get_UniqueItems write Set_UniqueItems;
    property FromClause: WideString read Get_FromClause;
    property JoinClause: WideString read Get_JoinClause;
    property MemberNameColumn: WideString read Get_MemberNameColumn write Set_MemberNameColumn;
    property MemberKeyTable: WideString read Get_MemberKeyTable;
    property MemberKeyColumn: WideString read Get_MemberKeyColumn write Set_MemberKeyColumn;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property EnableAggregations: WordBool read Get_EnableAggregations write Set_EnableAggregations;
    property Path: WideString read Get_Path;
    property Cube: _Cube read Get_Cube;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _CubeDimension read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property EstimatedSize: Integer read Get_EstimatedSize write Set_EstimatedSize;
    property Num: Smallint read Get_Num;
    property IsAll: WordBool read Get_IsAll;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property Ordering: OrderTypes read Get_Ordering write Set_Ordering;
    property LevelType: LevelTypes read Get_LevelType write Set_LevelType;
    property IObject: IDispatch read Get_IObject;
    property ILevel: IDispatch read Get_ILevel;
    property MemberProperties: _OlapCollection read Get_MemberProperties;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property CustomRollupExpression: WideString read Get_CustomRollupExpression write Set_CustomRollupExpression;
    property Grouping: GroupingValues read Get_Grouping write Set_Grouping;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property IsUnique: WordBool read Get_IsUnique write Set_IsUnique;
    property SliceValue: WideString read Get_SliceValue write Set_SliceValue;
    property ParentKeyColumn: WideString read Get_ParentKeyColumn write Set_ParentKeyColumn;
    property LevelNamingTemplate: WideString read Get_LevelNamingTemplate write Set_LevelNamingTemplate;
    property HideMemberIf: HideIfValues read Get_HideMemberIf write Set_HideMemberIf;
    property OrderingMemberProperty: WideString read Get_OrderingMemberProperty write Set_OrderingMemberProperty;
    property SkippedLevelsColumn: WideString read Get_SkippedLevelsColumn write Set_SkippedLevelsColumn;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property CustomRollupColumn: WideString read Get_CustomRollupColumn write Set_CustomRollupColumn;
    property CustomRollupPropertiesColumn: WideString read Get_CustomRollupPropertiesColumn write Set_CustomRollupPropertiesColumn;
    property RootMemberIf: RootIfValues read Get_RootMemberIf write Set_RootMemberIf;
    property UnaryOperatorColumn: WideString read Get_UnaryOperatorColumn write Set_UnaryOperatorColumn;
  end;

// *********************************************************************//
// DispIntf:  _CubeLevelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582B-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeLevelDisp = dispinterface
    ['{E8AC582B-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeLevel_28_1; dispid 1610743808;
    procedure GhostMethod__CubeLevel_32_2; dispid 1610743809;
    procedure GhostMethod__CubeLevel_36_3; dispid 1610743810;
    procedure GhostMethod__CubeLevel_40_4; dispid 1610743811;
    procedure GhostMethod__CubeLevel_44_5; dispid 1610743812;
    procedure GhostMethod__CubeLevel_48_6; dispid 1610743813;
    procedure GhostMethod__CubeLevel_52_7; dispid 1610743814;
    procedure GhostMethod__CubeLevel_56_8; dispid 1610743815;
    procedure GhostMethod__CubeLevel_60_9; dispid 1610743816;
    procedure GhostMethod__CubeLevel_64_10; dispid 1610743817;
    procedure GhostMethod__CubeLevel_68_11; dispid 1610743818;
    procedure GhostMethod__CubeLevel_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027165;
    function Validate: ValidateErrorCodes; dispid 1610809443;
    property UniqueItems: WordBool dispid 1745027161;
    property FromClause: WideString readonly dispid 1745027160;
    property JoinClause: WideString readonly dispid 1745027159;
    property MemberNameColumn: WideString dispid 1745027158;
    property MemberKeyTable: WideString readonly dispid 1745027157;
    property MemberKeyColumn: WideString dispid 1745027156;
    property IsDisabled: WordBool dispid 1745027155;
    property Description: WideString dispid 1745027153;
    property Valid: WordBool dispid 1745027152;
    property EnableAggregations: WordBool dispid 1745027149;
    property Path: WideString readonly dispid 1745027148;
    property Cube: _Cube readonly dispid 1745027147;
    property Database: _Database readonly dispid 1745027145;
    property Server: _Server readonly dispid 1745027144;
    property Parent: _CubeDimension dispid 1745027143;
    property Name: WideString dispid 1745027142;
    property ColumnType: Smallint dispid 1745027141;
    property EstimatedSize: Integer dispid 1745027140;
    property Num: Smallint readonly dispid 1745027139;
    property IsAll: WordBool readonly dispid 1745027138;
    property ClassType: Smallint readonly dispid 1745027137;
    property SubClassType: SubClassTypes readonly dispid 1745027136;
    property ColumnSize: Smallint dispid 1745027135;
    property Ordering: OrderTypes dispid 1745027134;
    property LevelType: LevelTypes dispid 1745027133;
    property IObject: IDispatch readonly dispid 1745027131;
    property ILevel: IDispatch readonly dispid 1745027130;
    property MemberProperties: _OlapCollection readonly dispid 1745027129;
    procedure ClearCollections; dispid 1610809445;
    procedure GhostMethod__CubeLevel_248_13; dispid 1610743863;
    procedure GhostMethod__CubeLevel_252_14; dispid 1610743864;
    procedure GhostMethod__CubeLevel_256_15; dispid 1610743865;
    procedure GhostMethod__CubeLevel_260_16; dispid 1610743866;
    procedure GhostMethod__CubeLevel_264_17; dispid 1610743867;
    procedure GhostMethod__CubeLevel_268_18; dispid 1610743868;
    procedure GhostMethod__CubeLevel_272_19; dispid 1610743869;
    procedure GhostMethod__CubeLevel_276_20; dispid 1610743870;
    procedure GhostMethod__CubeLevel_280_21; dispid 1610743871;
    procedure GhostMethod__CubeLevel_284_22; dispid 1610743872;
    procedure GhostMethod__CubeLevel_288_23; dispid 1610743873;
    procedure GhostMethod__CubeLevel_292_24; dispid 1610743874;
    property IsVisible: WordBool dispid 1745027297;
    property CustomRollupExpression: WideString dispid 1745027296;
    property Grouping: GroupingValues dispid 1745027189;
    property OrdinalPosition: Smallint readonly dispid 1745027188;
    property IsValid: WordBool readonly dispid 1745027187;
    property IsUnique: WordBool dispid 1745027186;
    property SliceValue: WideString dispid 1745027185;
    property ParentKeyColumn: WideString dispid 1745027184;
    property LevelNamingTemplate: WideString dispid 1745027183;
    property HideMemberIf: HideIfValues dispid 1745027182;
    property OrderingMemberProperty: WideString dispid 1745027181;
    property SkippedLevelsColumn: WideString dispid 1745027180;
    property AreMemberKeysUnique: WordBool dispid 1745027179;
    property AreMemberNamesUnique: WordBool dispid 1745027178;
    property CustomRollupColumn: WideString dispid 1745027177;
    property CustomRollupPropertiesColumn: WideString dispid 1745027176;
    property RootMemberIf: RootIfValues dispid 1745027175;
    property UnaryOperatorColumn: WideString dispid 1745027174;
  end;

// *********************************************************************//
// Interface: _CubeMeasure
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5830-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeMeasure = interface(IDispatch)
    ['{E8AC5830-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeMeasure_28_1; safecall;
    procedure GhostMethod__CubeMeasure_32_2; safecall;
    procedure GhostMethod__CubeMeasure_36_3; safecall;
    procedure GhostMethod__CubeMeasure_40_4; safecall;
    procedure GhostMethod__CubeMeasure_44_5; safecall;
    procedure GhostMethod__CubeMeasure_48_6; safecall;
    procedure GhostMethod__CubeMeasure_52_7; safecall;
    procedure GhostMethod__CubeMeasure_56_8; safecall;
    procedure GhostMethod__CubeMeasure_60_9; safecall;
    procedure GhostMethod__CubeMeasure_64_10; safecall;
    procedure GhostMethod__CubeMeasure_68_11; safecall;
    procedure GhostMethod__CubeMeasure_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_AggregateFunction: Smallint; safecall;
    procedure Set_AggregateFunction(Param1: Smallint); safecall;
    function Get_SourceField: WideString; safecall;
    procedure Set_SourceField(var Param1: WideString); safecall;
    function Get_AggregationColumn: WideString; safecall;
    procedure Set_AggregationColumn(var Param1: WideString); safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(var Param1: Smallint); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_FormatString: WideString; safecall;
    procedure Set_FormatString(const Param1: WideString); safecall;
    function Get_IsInternal: Smallint; safecall;
    procedure Set_IsInternal(Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Cube: _Cube; safecall;
    function Get_Parent: _Cube; safecall;
    procedure _Set_Parent(const Param1: _Cube); safecall;
    function Get_Path: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IMeasure: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__CubeMeasure_200_13; safecall;
    procedure GhostMethod__CubeMeasure_204_14; safecall;
    procedure GhostMethod__CubeMeasure_208_15; safecall;
    procedure GhostMethod__CubeMeasure_212_16; safecall;
    procedure GhostMethod__CubeMeasure_216_17; safecall;
    procedure GhostMethod__CubeMeasure_220_18; safecall;
    procedure GhostMethod__CubeMeasure_224_19; safecall;
    procedure GhostMethod__CubeMeasure_228_20; safecall;
    procedure GhostMethod__CubeMeasure_232_21; safecall;
    procedure GhostMethod__CubeMeasure_236_22; safecall;
    procedure GhostMethod__CubeMeasure_240_23; safecall;
    procedure GhostMethod__CubeMeasure_244_24; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_SourceColumn(const Param1: WideString); safecall;
    function Get_SourceColumnType: Smallint; safecall;
    procedure Set_SourceColumnType(Param1: Smallint); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property AggregateFunction: Smallint read Get_AggregateFunction write Set_AggregateFunction;
    property SourceField: WideString read Get_SourceField write Set_SourceField;
    property AggregationColumn: WideString read Get_AggregationColumn write Set_AggregationColumn;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property Description: WideString read Get_Description write Set_Description;
    property FormatString: WideString read Get_FormatString write Set_FormatString;
    property IsInternal: Smallint read Get_IsInternal write Set_IsInternal;
    property Num: Smallint read Get_Num write Set_Num;
    property Cube: _Cube read Get_Cube;
    property Parent: _Cube read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property Name: WideString read Get_Name write Set_Name;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property IMeasure: IDispatch read Get_IMeasure;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property SourceColumnType: Smallint read Get_SourceColumnType write Set_SourceColumnType;
  end;

// *********************************************************************//
// DispIntf:  _CubeMeasureDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5830-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _CubeMeasureDisp = dispinterface
    ['{E8AC5830-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__CubeMeasure_28_1; dispid 1610743808;
    procedure GhostMethod__CubeMeasure_32_2; dispid 1610743809;
    procedure GhostMethod__CubeMeasure_36_3; dispid 1610743810;
    procedure GhostMethod__CubeMeasure_40_4; dispid 1610743811;
    procedure GhostMethod__CubeMeasure_44_5; dispid 1610743812;
    procedure GhostMethod__CubeMeasure_48_6; dispid 1610743813;
    procedure GhostMethod__CubeMeasure_52_7; dispid 1610743814;
    procedure GhostMethod__CubeMeasure_56_8; dispid 1610743815;
    procedure GhostMethod__CubeMeasure_60_9; dispid 1610743816;
    procedure GhostMethod__CubeMeasure_64_10; dispid 1610743817;
    procedure GhostMethod__CubeMeasure_68_11; dispid 1610743818;
    procedure GhostMethod__CubeMeasure_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027154;
    property Valid: WordBool dispid 1745027150;
    property AggregateFunction: Smallint dispid 1745027147;
    property SourceField: WideString dispid 1745027145;
    property AggregationColumn: WideString dispid 1745027144;
    property ColumnType: Smallint dispid 1745027143;
    property Description: WideString dispid 1745027139;
    property FormatString: WideString dispid 1745027138;
    property IsInternal: Smallint dispid 1745027137;
    property Num: Smallint dispid 1745027136;
    property Cube: _Cube readonly dispid 1745027135;
    property Parent: _Cube dispid 1745027134;
    property Path: WideString readonly dispid 1745027133;
    property Name: WideString dispid 1745027132;
    property ClassType: Smallint readonly dispid 1745027130;
    property SubClassType: SubClassTypes dispid 1745027129;
    property IObject: IDispatch readonly dispid 1745027128;
    property IMeasure: IDispatch readonly dispid 1745027127;
    procedure ClearCollections; dispid 1610809433;
    procedure GhostMethod__CubeMeasure_200_13; dispid 1610743851;
    procedure GhostMethod__CubeMeasure_204_14; dispid 1610743852;
    procedure GhostMethod__CubeMeasure_208_15; dispid 1610743853;
    procedure GhostMethod__CubeMeasure_212_16; dispid 1610743854;
    procedure GhostMethod__CubeMeasure_216_17; dispid 1610743855;
    procedure GhostMethod__CubeMeasure_220_18; dispid 1610743856;
    procedure GhostMethod__CubeMeasure_224_19; dispid 1610743857;
    procedure GhostMethod__CubeMeasure_228_20; dispid 1610743858;
    procedure GhostMethod__CubeMeasure_232_21; dispid 1610743859;
    procedure GhostMethod__CubeMeasure_236_22; dispid 1610743860;
    procedure GhostMethod__CubeMeasure_240_23; dispid 1610743861;
    procedure GhostMethod__CubeMeasure_244_24; dispid 1610743862;
    property IsVisible: WordBool dispid 1745027234;
    property OrdinalPosition: Smallint readonly dispid 1745027165;
    property IsValid: WordBool readonly dispid 1745027164;
    property SourceColumn: WideString dispid 1745027163;
    property SourceColumnType: Smallint dispid 1745027162;
  end;

// *********************************************************************//
// Interface: _Database
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5820-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _Database = interface(IDispatch)
    ['{E8AC5820-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Database_28_1; safecall;
    procedure GhostMethod__Database_32_2; safecall;
    procedure GhostMethod__Database_36_3; safecall;
    procedure GhostMethod__Database_40_4; safecall;
    procedure GhostMethod__Database_44_5; safecall;
    procedure GhostMethod__Database_48_6; safecall;
    procedure GhostMethod__Database_52_7; safecall;
    procedure GhostMethod__Database_56_8; safecall;
    procedure GhostMethod__Database_60_9; safecall;
    procedure GhostMethod__Database_64_10; safecall;
    procedure GhostMethod__Database_68_11; safecall;
    procedure GhostMethod__Database_72_12; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_EstimatedSize: Double; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Dimensions: _OlapCollection; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_Groups: _OlapCollection; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Cubes: _OlapCollection; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    procedure _Set_Parent(const Param1: _Server); safecall;
    function Get_Parent: _Server; safecall;
    function Get_Name: WideString; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_Committed: WordBool; safecall;
    procedure Set_Committed(var Param1: WordBool); safecall;
    procedure Process(ProcessOption: ProcessTypes); safecall;
    procedure BeginTrans; safecall;
    procedure CommitTrans_DeleteInvalidCubes(var CommitString: WideString); safecall;
    procedure CommitTrans; safecall;
    function Rollback: WordBool; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDimension: IDispatch; safecall;
    procedure ClearCollections; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure GhostMethod__Database_216_13; safecall;
    procedure GhostMethod__Database_220_14; safecall;
    procedure GhostMethod__Database_224_15; safecall;
    procedure GhostMethod__Database_228_16; safecall;
    procedure GhostMethod__Database_232_17; safecall;
    procedure GhostMethod__Database_236_18; safecall;
    procedure GhostMethod__Database_240_19; safecall;
    procedure GhostMethod__Database_244_20; safecall;
    procedure GhostMethod__Database_248_21; safecall;
    procedure GhostMethod__Database_252_22; safecall;
    procedure GhostMethod__Database_256_23; safecall;
    procedure GhostMethod__Database_260_24; safecall;
    procedure Set_AggregationPrefix(const Param1: WideString); safecall;
    function Get_AggregationPrefix: WideString; safecall;
    procedure GhostMethod__Database_272_25; safecall;
    procedure GhostMethod__Database_276_26; safecall;
    procedure GhostMethod__Database_280_27; safecall;
    procedure GhostMethod__Database_284_28; safecall;
    procedure GhostMethod__Database_288_29; safecall;
    procedure GhostMethod__Database_292_30; safecall;
    procedure GhostMethod__Database_296_31; safecall;
    procedure GhostMethod__Database_300_32; safecall;
    procedure GhostMethod__Database_304_33; safecall;
    procedure GhostMethod__Database_308_34; safecall;
    procedure GhostMethod__Database_312_35; safecall;
    procedure GhostMethod__Database_316_36; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_IsVisible: WordBool; safecall;
    function Get_MiningModels: _OlapCollection; safecall;
    function Get_IsTemporary: WordBool; safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    procedure Set_ProcessingKeyErrorLogFileName(const Param1: WideString); safecall;
    function Get_ProcessingKeyErrorLogFileName: WideString; safecall;
    procedure Set_ProcessingKeyErrorLimit(Param1: Integer); safecall;
    function Get_ProcessingKeyErrorLimit: Integer; safecall;
    procedure Set_ProcessOptimizationMode(Param1: ProcessOptimizationModes); safecall;
    function Get_ProcessOptimizationMode: ProcessOptimizationModes; safecall;
    function Get_LazyOptimizationProgress: Smallint; safecall;
    function Get_MDStores: _OlapCollection; safecall;
    function Get_Measures: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_Analyzer: IDispatch; safecall;
    function Get_EstimatedRows: Double; safecall;
    procedure Set_EstimatedRows(Param1: Double); safecall;
    function Get_IsDefault: WordBool; safecall;
    procedure Set_IsDefault(Param1: WordBool); safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_OlapMode: OlapStorageModes; safecall;
    procedure Set_OlapMode(Param1: OlapStorageModes); safecall;
    function Get_SourceTable: WideString; safecall;
    procedure Set_SourceTable(const Param1: WideString); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    procedure Set_SourceTableAlias(const Param1: WideString); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_AllowDrillThrough: WordBool; safecall;
    procedure Set_AllowDrillThrough(Param1: WordBool); safecall;
    function Get_DrillThroughColumns: WideString; safecall;
    procedure Set_DrillThroughColumns(const Param1: WideString); safecall;
    function Get_DrillThroughFilter: WideString; safecall;
    procedure Set_DrillThroughFilter(const Param1: WideString); safecall;
    function Get_RemoteServer: WideString; safecall;
    procedure Set_RemoteServer(const Param1: WideString); safecall;
    function Get_DrillThroughFrom: WideString; safecall;
    procedure Set_DrillThroughFrom(const Param1: WideString); safecall;
    function Get_DrillThroughJoins: WideString; safecall;
    procedure Set_DrillThroughJoins(const Param1: WideString); safecall;
    function Get_DefaultMeasure: WideString; safecall;
    procedure Set_DefaultMeasure(const Param1: WideString); safecall;
    procedure Merge(const SourceName: WideString); safecall;
    procedure Update; safecall;
    procedure GhostMethod__Database_536_37; safecall;
    procedure GhostMethod__Database_540_38; safecall;
    procedure GhostMethod__Database_544_39; safecall;
    procedure GhostMethod__Database_548_40; safecall;
    procedure GhostMethod__Database_552_41; safecall;
    procedure GhostMethod__Database_556_42; safecall;
    procedure GhostMethod__Database_560_43; safecall;
    procedure GhostMethod__Database_564_44; safecall;
    procedure GhostMethod__Database_568_45; safecall;
    procedure GhostMethod__Database_572_46; safecall;
    procedure GhostMethod__Database_576_47; safecall;
    procedure GhostMethod__Database_580_48; safecall;
    procedure CommitTransEx(Options: ProcessTypes); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property State: OlapStateTypes read Get_State;
    property EstimatedSize: Double read Get_EstimatedSize;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Dimensions: _OlapCollection read Get_Dimensions;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property DataSources: _OlapCollection read Get_DataSources;
    property Groups: _OlapCollection read Get_Groups;
    property Commands: _OlapCollection read Get_Commands;
    property Description: WideString read Get_Description write Set_Description;
    property Cubes: _OlapCollection read Get_Cubes;
    property Name: WideString read Get_Name write Set_Name;
    property Parent: _Server read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property Committed: WordBool read Get_Committed write Set_Committed;
    property IObject: IDispatch read Get_IObject;
    property IDimension: IDispatch read Get_IDimension;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property AggregationPrefix: WideString read Get_AggregationPrefix write Set_AggregationPrefix;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property MiningModels: _OlapCollection read Get_MiningModels;
    property IsTemporary: WordBool read Get_IsTemporary;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property ProcessingKeyErrorLogFileName: WideString read Get_ProcessingKeyErrorLogFileName write Set_ProcessingKeyErrorLogFileName;
    property ProcessingKeyErrorLimit: Integer read Get_ProcessingKeyErrorLimit write Set_ProcessingKeyErrorLimit;
    property ProcessOptimizationMode: ProcessOptimizationModes read Get_ProcessOptimizationMode write Set_ProcessOptimizationMode;
    property LazyOptimizationProgress: Smallint read Get_LazyOptimizationProgress;
    property MDStores: _OlapCollection read Get_MDStores;
    property Measures: _OlapCollection read Get_Measures;
    property Roles: _OlapCollection read Get_Roles;
    property Analyzer: IDispatch read Get_Analyzer;
    property EstimatedRows: Double read Get_EstimatedRows write Set_EstimatedRows;
    property IsDefault: WordBool read Get_IsDefault write Set_IsDefault;
    property IsValid: WordBool read Get_IsValid;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property OlapMode: OlapStorageModes read Get_OlapMode write Set_OlapMode;
    property SourceTable: WideString read Get_SourceTable write Set_SourceTable;
    property SourceTableAlias: WideString read Get_SourceTableAlias write Set_SourceTableAlias;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property AllowDrillThrough: WordBool read Get_AllowDrillThrough write Set_AllowDrillThrough;
    property DrillThroughColumns: WideString read Get_DrillThroughColumns write Set_DrillThroughColumns;
    property DrillThroughFilter: WideString read Get_DrillThroughFilter write Set_DrillThroughFilter;
    property RemoteServer: WideString read Get_RemoteServer write Set_RemoteServer;
    property DrillThroughFrom: WideString read Get_DrillThroughFrom write Set_DrillThroughFrom;
    property DrillThroughJoins: WideString read Get_DrillThroughJoins write Set_DrillThroughJoins;
    property DefaultMeasure: WideString read Get_DefaultMeasure write Set_DefaultMeasure;
  end;

// *********************************************************************//
// DispIntf:  _DatabaseDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5820-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DatabaseDisp = dispinterface
    ['{E8AC5820-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__Database_28_1; dispid 1610743808;
    procedure GhostMethod__Database_32_2; dispid 1610743809;
    procedure GhostMethod__Database_36_3; dispid 1610743810;
    procedure GhostMethod__Database_40_4; dispid 1610743811;
    procedure GhostMethod__Database_44_5; dispid 1610743812;
    procedure GhostMethod__Database_48_6; dispid 1610743813;
    procedure GhostMethod__Database_52_7; dispid 1610743814;
    procedure GhostMethod__Database_56_8; dispid 1610743815;
    procedure GhostMethod__Database_60_9; dispid 1610743816;
    procedure GhostMethod__Database_64_10; dispid 1610743817;
    procedure GhostMethod__Database_68_11; dispid 1610743818;
    procedure GhostMethod__Database_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809428;
    property CustomProperties: _Properties readonly dispid 1745027151;
    property State: OlapStateTypes readonly dispid 1745027147;
    property EstimatedSize: Double readonly dispid 1745027146;
    property Valid: WordBool dispid 1745027145;
    property Dimensions: _OlapCollection readonly dispid 1745027144;
    property Database: _Database readonly dispid 1745027143;
    property Server: _Server readonly dispid 1745027142;
    property DataSources: _OlapCollection readonly dispid 1745027141;
    property Groups: _OlapCollection readonly dispid 1745027140;
    property Commands: _OlapCollection readonly dispid 1745027139;
    property Description: WideString dispid 1745027138;
    property Cubes: _OlapCollection readonly dispid 1745027135;
    property Name: WideString dispid 1745027134;
    property Parent: _Server dispid 1745027133;
    property Path: WideString readonly dispid 1745027131;
    property ClassType: Smallint readonly dispid 1745027130;
    property SubClassType: SubClassTypes readonly dispid 1745027129;
    property Committed: WordBool dispid 1745027128;
    procedure Process(ProcessOption: ProcessTypes); dispid 1610809461;
    procedure BeginTrans; dispid 1610809469;
    procedure CommitTrans_DeleteInvalidCubes(var CommitString: WideString); dispid 1610809472;
    procedure CommitTrans; dispid 1610809484;
    function Rollback: WordBool; dispid 1610809485;
    property IObject: IDispatch readonly dispid 1745027122;
    property IDimension: IDispatch readonly dispid 1745027121;
    procedure ClearCollections; dispid 1610809496;
    property LastProcessed: TDateTime readonly dispid 1745027111;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809497;
    procedure UnlockObject; dispid 1610809498;
    procedure GhostMethod__Database_216_13; dispid 1610743855;
    procedure GhostMethod__Database_220_14; dispid 1610743856;
    procedure GhostMethod__Database_224_15; dispid 1610743857;
    procedure GhostMethod__Database_228_16; dispid 1610743858;
    procedure GhostMethod__Database_232_17; dispid 1610743859;
    procedure GhostMethod__Database_236_18; dispid 1610743860;
    procedure GhostMethod__Database_240_19; dispid 1610743861;
    procedure GhostMethod__Database_244_20; dispid 1610743862;
    procedure GhostMethod__Database_248_21; dispid 1610743863;
    procedure GhostMethod__Database_252_22; dispid 1610743864;
    procedure GhostMethod__Database_256_23; dispid 1610743865;
    procedure GhostMethod__Database_260_24; dispid 1610743866;
    property AggregationPrefix: WideString dispid 1745027282;
    procedure GhostMethod__Database_272_25; dispid 1610743869;
    procedure GhostMethod__Database_276_26; dispid 1610743870;
    procedure GhostMethod__Database_280_27; dispid 1610743871;
    procedure GhostMethod__Database_284_28; dispid 1610743872;
    procedure GhostMethod__Database_288_29; dispid 1610743873;
    procedure GhostMethod__Database_292_30; dispid 1610743874;
    procedure GhostMethod__Database_296_31; dispid 1610743875;
    procedure GhostMethod__Database_300_32; dispid 1610743876;
    procedure GhostMethod__Database_304_33; dispid 1610743877;
    procedure GhostMethod__Database_308_34; dispid 1610743878;
    procedure GhostMethod__Database_312_35; dispid 1610743879;
    procedure GhostMethod__Database_316_36; dispid 1610743880;
    property IsVisible: WordBool dispid 1745027389;
    property MiningModels: _OlapCollection readonly dispid 1745027383;
    property IsTemporary: WordBool readonly dispid 1745027380;
    property EnableRealTimeUpdates: WordBool dispid 1745027379;
    property ProcessingKeyErrorLogFileName: WideString dispid 1745027377;
    property ProcessingKeyErrorLimit: Integer dispid 1745027376;
    property ProcessOptimizationMode: ProcessOptimizationModes dispid 1745027375;
    property LazyOptimizationProgress: Smallint readonly dispid 1745027374;
    property MDStores: _OlapCollection readonly dispid 1745027304;
    property Measures: _OlapCollection readonly dispid 1745027303;
    property Roles: _OlapCollection readonly dispid 1745027302;
    property Analyzer: IDispatch readonly dispid 1745027301;
    property EstimatedRows: Double dispid 1745027300;
    property IsDefault: WordBool dispid 1745027299;
    property IsValid: WordBool readonly dispid 1745027298;
    property LastUpdated: TDateTime dispid 1745027297;
    property OlapMode: OlapStorageModes dispid 1745027296;
    property SourceTable: WideString dispid 1745027295;
    property SourceTableAlias: WideString dispid 1745027294;
    property SourceTableFilter: WideString dispid 1745027293;
    property JoinClause: WideString dispid 1745027292;
    property FromClause: WideString dispid 1745027291;
    property IsReadWrite: WordBool dispid 1745027290;
    property AllowDrillThrough: WordBool dispid 1745027289;
    property DrillThroughColumns: WideString dispid 1745027288;
    property DrillThroughFilter: WideString dispid 1745027287;
    property RemoteServer: WideString dispid 1745027286;
    property DrillThroughFrom: WideString dispid 1745027285;
    property DrillThroughJoins: WideString dispid 1745027284;
    property DefaultMeasure: WideString dispid 1745027283;
    procedure Merge(const SourceName: WideString); dispid 1610809778;
    procedure Update; dispid 1610809779;
    procedure GhostMethod__Database_536_37; dispid 1610743935;
    procedure GhostMethod__Database_540_38; dispid 1610743936;
    procedure GhostMethod__Database_544_39; dispid 1610743937;
    procedure GhostMethod__Database_548_40; dispid 1610743938;
    procedure GhostMethod__Database_552_41; dispid 1610743939;
    procedure GhostMethod__Database_556_42; dispid 1610743940;
    procedure GhostMethod__Database_560_43; dispid 1610743941;
    procedure GhostMethod__Database_564_44; dispid 1610743942;
    procedure GhostMethod__Database_568_45; dispid 1610743943;
    procedure GhostMethod__Database_572_46; dispid 1610743944;
    procedure GhostMethod__Database_576_47; dispid 1610743945;
    procedure GhostMethod__Database_580_48; dispid 1610743946;
    procedure CommitTransEx(Options: ProcessTypes); dispid 1610809977;
  end;

// *********************************************************************//
// DispIntf:  __Database
// Flags:     (4240) Hidden NonExtensible Dispatchable
// GUID:      {E8AC5821-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  __Database = dispinterface
    ['{E8AC5821-7127-11D2-8A35-00C04FB9898D}']
    procedure ReportBefore(var obj: IDispatch; Action: Smallint; var Cancel: WordBool; 
                           var Skip: WordBool); dispid 1;
    procedure ReportProgress(var obj: IDispatch; Action: Smallint; var Counter: Integer; 
                             var Message: WideString; var Cancel: WordBool); dispid 2;
    procedure ReportError(var obj: IDispatch; Action: Smallint; errorCode: Integer; 
                          const Message: WideString; var Cancel: WordBool); dispid 3;
    procedure ReportAfter(var obj: IDispatch; Action: Smallint; success: WordBool); dispid 4;
  end;

// *********************************************************************//
// Interface: _DBCommand
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5856-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DBCommand = interface(IDispatch)
    ['{E8AC5856-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DBCommand_28_1; safecall;
    procedure GhostMethod__DBCommand_32_2; safecall;
    procedure GhostMethod__DBCommand_36_3; safecall;
    procedure GhostMethod__DBCommand_40_4; safecall;
    procedure GhostMethod__DBCommand_44_5; safecall;
    procedure GhostMethod__DBCommand_48_6; safecall;
    procedure GhostMethod__DBCommand_52_7; safecall;
    procedure GhostMethod__DBCommand_56_8; safecall;
    procedure GhostMethod__DBCommand_60_9; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure SaveObject(var AssumeInsert: WordBool); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_CommandType: CommandTypes; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Statement: WideString; safecall;
    procedure Set_Statement(const Param1: WideString); safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _Database); safecall;
    function Get_Parent: _Database; safecall;
    procedure Edit; safecall;
    procedure EditNewChild(var obj: IDispatch); safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    function Get_IObject: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__DBCommand_172_10; safecall;
    procedure GhostMethod__DBCommand_176_11; safecall;
    procedure GhostMethod__DBCommand_180_12; safecall;
    procedure GhostMethod__DBCommand_184_13; safecall;
    procedure GhostMethod__DBCommand_188_14; safecall;
    procedure GhostMethod__DBCommand_192_15; safecall;
    procedure GhostMethod__DBCommand_196_16; safecall;
    procedure GhostMethod__DBCommand_200_17; safecall;
    procedure GhostMethod__DBCommand_204_18; safecall;
    procedure Set_CommandType(Param1: CommandTypes); safecall;
    procedure GhostMethod__DBCommand_212_19; safecall;
    procedure GhostMethod__DBCommand_216_20; safecall;
    procedure GhostMethod__DBCommand_220_21; safecall;
    procedure GhostMethod__DBCommand_224_22; safecall;
    procedure GhostMethod__DBCommand_228_23; safecall;
    procedure GhostMethod__DBCommand_232_24; safecall;
    procedure GhostMethod__DBCommand_236_25; safecall;
    procedure GhostMethod__DBCommand_240_26; safecall;
    procedure GhostMethod__DBCommand_244_27; safecall;
    procedure _Set_ParentObject(const Param1: IDispatch); safecall;
    function Get_ParentObject: IDispatch; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Update; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property CommandType: CommandTypes read Get_CommandType write Set_CommandType;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property Statement: WideString read Get_Statement write Set_Statement;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Database read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property ParentObject: IDispatch read Get_ParentObject write _Set_ParentObject;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _DBCommandDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5856-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DBCommandDisp = dispinterface
    ['{E8AC5856-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DBCommand_28_1; dispid 1610743808;
    procedure GhostMethod__DBCommand_32_2; dispid 1610743809;
    procedure GhostMethod__DBCommand_36_3; dispid 1610743810;
    procedure GhostMethod__DBCommand_40_4; dispid 1610743811;
    procedure GhostMethod__DBCommand_44_5; dispid 1610743812;
    procedure GhostMethod__DBCommand_48_6; dispid 1610743813;
    procedure GhostMethod__DBCommand_52_7; dispid 1610743814;
    procedure GhostMethod__DBCommand_56_8; dispid 1610743815;
    procedure GhostMethod__DBCommand_60_9; dispid 1610743816;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809393;
    property CustomProperties: _Properties readonly dispid 1745027115;
    procedure SaveObject(var AssumeInsert: WordBool); dispid 1610809397;
    property Description: WideString dispid 1745027112;
    property CommandType: CommandTypes dispid 1745027111;
    property Valid: WordBool dispid 1745027109;
    property Name: WideString dispid 1745027108;
    property Statement: WideString dispid 1745027107;
    property Num: Smallint dispid 1745027106;
    property Database: _Database readonly dispid 1745027105;
    property Server: _Server readonly dispid 1745027104;
    property Parent: _Database dispid 1745027103;
    procedure Edit; dispid 1610809399;
    procedure EditNewChild(var obj: IDispatch); dispid 1610809400;
    property Path: WideString readonly dispid 1745027102;
    property ClassType: Smallint readonly dispid 1745027101;
    property SubClassType: SubClassTypes readonly dispid 1745027100;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809402;
    procedure UnlockObject; dispid 1610809403;
    property IObject: IDispatch readonly dispid 1745027099;
    procedure ClearCollections; dispid 1610809406;
    procedure GhostMethod__DBCommand_172_10; dispid 1610743844;
    procedure GhostMethod__DBCommand_176_11; dispid 1610743845;
    procedure GhostMethod__DBCommand_180_12; dispid 1610743846;
    procedure GhostMethod__DBCommand_184_13; dispid 1610743847;
    procedure GhostMethod__DBCommand_188_14; dispid 1610743848;
    procedure GhostMethod__DBCommand_192_15; dispid 1610743849;
    procedure GhostMethod__DBCommand_196_16; dispid 1610743850;
    procedure GhostMethod__DBCommand_200_17; dispid 1610743851;
    procedure GhostMethod__DBCommand_204_18; dispid 1610743852;
    procedure GhostMethod__DBCommand_212_19; dispid 1610743854;
    procedure GhostMethod__DBCommand_216_20; dispid 1610743855;
    procedure GhostMethod__DBCommand_220_21; dispid 1610743856;
    procedure GhostMethod__DBCommand_224_22; dispid 1610743857;
    procedure GhostMethod__DBCommand_228_23; dispid 1610743858;
    procedure GhostMethod__DBCommand_232_24; dispid 1610743859;
    procedure GhostMethod__DBCommand_236_25; dispid 1610743860;
    procedure GhostMethod__DBCommand_240_26; dispid 1610743861;
    procedure GhostMethod__DBCommand_244_27; dispid 1610743862;
    property ParentObject: IDispatch dispid 1745027167;
    property OrdinalPosition: Smallint readonly dispid 1745027136;
    property IsValid: WordBool readonly dispid 1745027135;
    procedure Update; dispid 1610809466;
  end;

// *********************************************************************//
// Interface: _DbDimension
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5828-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DbDimension = interface(IDispatch)
    ['{E8AC5828-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DbDimension_28_1; safecall;
    procedure GhostMethod__DbDimension_32_2; safecall;
    procedure GhostMethod__DbDimension_36_3; safecall;
    procedure GhostMethod__DbDimension_40_4; safecall;
    procedure GhostMethod__DbDimension_44_5; safecall;
    procedure GhostMethod__DbDimension_48_6; safecall;
    procedure GhostMethod__DbDimension_52_7; safecall;
    procedure GhostMethod__DbDimension_56_8; safecall;
    procedure GhostMethod__DbDimension_60_9; safecall;
    procedure GhostMethod__DbDimension_64_10; safecall;
    procedure GhostMethod__DbDimension_68_11; safecall;
    procedure GhostMethod__DbDimension_72_12; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure SaveObject(var AssumeInsert: WordBool); safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_IsShared: WordBool; safecall;
    function Get_OwningCube: WideString; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Used: WordBool; safecall;
    function Validate: ValidateErrorCodes; safecall;
    function Get_Levels: _OlapCollection; safecall;
    function Get_IsTime: WordBool; safecall;
    procedure Set_IsTime(var Param1: WordBool); safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    function Get_IsTemporary: WordBool; safecall;
    procedure _Set_Parent(const Param1: _Database); safecall;
    function Get_Parent: _Database; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Path: WideString; safecall;
    function Get_Description: WideString; safecall;
    function Get_DataSource: _DataSource; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_Huge: Smallint; safecall;
    procedure Set_Huge(var Param1: Smallint); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    procedure Set_DataSource(const Param1: _DataSource); safecall;
    procedure Set_DimensionType(Param1: DimensionTypes); safecall;
    function Get_DimensionType: DimensionTypes; safecall;
    procedure Set_LastUpdated(Param1: TDateTime); safecall;
    function Get_SourceTable: WideString; safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(const Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(const Param1: WideString); safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    function Get_Committed: WordBool; safecall;
    procedure Set_Committed(var Param1: WordBool); safecall;
    procedure Process(ProcessOption: ProcessTypes); safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDimension: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__DbDimension_272_13; safecall;
    procedure GhostMethod__DbDimension_276_14; safecall;
    procedure GhostMethod__DbDimension_280_15; safecall;
    procedure GhostMethod__DbDimension_284_16; safecall;
    procedure GhostMethod__DbDimension_288_17; safecall;
    procedure GhostMethod__DbDimension_292_18; safecall;
    procedure GhostMethod__DbDimension_296_19; safecall;
    procedure GhostMethod__DbDimension_300_20; safecall;
    procedure GhostMethod__DbDimension_304_21; safecall;
    procedure GhostMethod__DbDimension_308_22; safecall;
    procedure GhostMethod__DbDimension_312_23; safecall;
    procedure GhostMethod__DbDimension_316_24; safecall;
    procedure _Set_DataSource(const Param1: _DataSource); safecall;
    procedure GhostMethod__DbDimension_324_25; safecall;
    procedure GhostMethod__DbDimension_328_26; safecall;
    procedure GhostMethod__DbDimension_332_27; safecall;
    procedure GhostMethod__DbDimension_336_28; safecall;
    procedure GhostMethod__DbDimension_340_29; safecall;
    procedure GhostMethod__DbDimension_344_30; safecall;
    procedure GhostMethod__DbDimension_348_31; safecall;
    procedure GhostMethod__DbDimension_352_32; safecall;
    procedure GhostMethod__DbDimension_356_33; safecall;
    procedure GhostMethod__DbDimension_360_34; safecall;
    procedure GhostMethod__DbDimension_364_35; safecall;
    procedure GhostMethod__DbDimension_368_36; safecall;
    function Get_DefaultMember: WideString; safecall;
    procedure Set_DefaultMember(const Param1: WideString); safecall;
    function Get_IsVirtual: WordBool; safecall;
    procedure Set_IsVirtual(Param1: WordBool); safecall;
    function Get_MembersWithData: MembersWithDataValues; safecall;
    procedure Set_MembersWithData(Param1: MembersWithDataValues); safecall;
    function Get_DataMemberCaptionTemplate: WideString; safecall;
    procedure Set_DataMemberCaptionTemplate(const Param1: WideString); safecall;
    function Get_DependsOnDimension: WideString; safecall;
    procedure Set_DependsOnDimension(const Param1: WideString); safecall;
    function Get_IsChanging: WordBool; safecall;
    procedure Set_IsChanging(Param1: WordBool); safecall;
    function Get_IsReadWrite: WordBool; safecall;
    procedure Set_IsReadWrite(Param1: WordBool); safecall;
    function Get_StorageMode: StorageModeValues; safecall;
    procedure Set_StorageMode(Param1: StorageModeValues); safecall;
    function Get_SourceTableFilter: WideString; safecall;
    procedure Set_SourceTableFilter(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_AllowSiblingsWithSameName: WordBool; safecall;
    procedure Set_AllowSiblingsWithSameName(Param1: WordBool); safecall;
    function Get_EnableRealTimeUpdates: WordBool; safecall;
    procedure Set_EnableRealTimeUpdates(Param1: WordBool); safecall;
    function Get_SourceTableAlias: WideString; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_AggregationUsage: DimensionAggUsageTypes; safecall;
    procedure Set_AggregationUsage(Param1: DimensionAggUsageTypes); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    procedure Update; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property State: OlapStateTypes read Get_State;
    property IsShared: WordBool read Get_IsShared;
    property OwningCube: WideString read Get_OwningCube;
    property Num: Smallint read Get_Num write Set_Num;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Used: WordBool read Get_Used;
    property Levels: _OlapCollection read Get_Levels;
    property IsTime: WordBool read Get_IsTime write Set_IsTime;
    property IsTemporary: WordBool read Get_IsTemporary;
    property Parent: _Database read Get_Parent write _Set_Parent;
    property Name: WideString read Get_Name write Set_Name;
    property Path: WideString read Get_Path;
    property Description: WideString read Get_Description write Set_Description;
    property DataSource: _DataSource read Get_DataSource write Set_DataSource;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property Huge: Smallint read Get_Huge write Set_Huge;
    property DimensionType: DimensionTypes read Get_DimensionType write Set_DimensionType;
    property SourceTable: WideString read Get_SourceTable;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property IsValid: WordBool read Get_IsValid;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property Committed: WordBool read Get_Committed write Set_Committed;
    property IObject: IDispatch read Get_IObject;
    property IDimension: IDispatch read Get_IDimension;
    property DefaultMember: WideString read Get_DefaultMember write Set_DefaultMember;
    property IsVirtual: WordBool read Get_IsVirtual write Set_IsVirtual;
    property MembersWithData: MembersWithDataValues read Get_MembersWithData write Set_MembersWithData;
    property DataMemberCaptionTemplate: WideString read Get_DataMemberCaptionTemplate write Set_DataMemberCaptionTemplate;
    property DependsOnDimension: WideString read Get_DependsOnDimension write Set_DependsOnDimension;
    property IsChanging: WordBool read Get_IsChanging write Set_IsChanging;
    property IsReadWrite: WordBool read Get_IsReadWrite write Set_IsReadWrite;
    property StorageMode: StorageModeValues read Get_StorageMode write Set_StorageMode;
    property SourceTableFilter: WideString read Get_SourceTableFilter write Set_SourceTableFilter;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property AllowSiblingsWithSameName: WordBool read Get_AllowSiblingsWithSameName write Set_AllowSiblingsWithSameName;
    property EnableRealTimeUpdates: WordBool read Get_EnableRealTimeUpdates write Set_EnableRealTimeUpdates;
    property SourceTableAlias: WideString read Get_SourceTableAlias;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property AggregationUsage: DimensionAggUsageTypes read Get_AggregationUsage write Set_AggregationUsage;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
  end;

// *********************************************************************//
// DispIntf:  _DbDimensionDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5828-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DbDimensionDisp = dispinterface
    ['{E8AC5828-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DbDimension_28_1; dispid 1610743808;
    procedure GhostMethod__DbDimension_32_2; dispid 1610743809;
    procedure GhostMethod__DbDimension_36_3; dispid 1610743810;
    procedure GhostMethod__DbDimension_40_4; dispid 1610743811;
    procedure GhostMethod__DbDimension_44_5; dispid 1610743812;
    procedure GhostMethod__DbDimension_48_6; dispid 1610743813;
    procedure GhostMethod__DbDimension_52_7; dispid 1610743814;
    procedure GhostMethod__DbDimension_56_8; dispid 1610743815;
    procedure GhostMethod__DbDimension_60_9; dispid 1610743816;
    procedure GhostMethod__DbDimension_64_10; dispid 1610743817;
    procedure GhostMethod__DbDimension_68_11; dispid 1610743818;
    procedure GhostMethod__DbDimension_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809433;
    property CustomProperties: _Properties readonly dispid 1745027154;
    procedure SaveObject(var AssumeInsert: WordBool); dispid 1610809436;
    property State: OlapStateTypes readonly dispid 1745027149;
    property IsShared: WordBool readonly dispid 1745027148;
    property OwningCube: WideString readonly dispid 1745027147;
    property Num: Smallint dispid 1745027146;
    property Valid: WordBool dispid 1745027145;
    property Used: WordBool readonly dispid 1745027144;
    function Validate: ValidateErrorCodes; dispid 1610809441;
    property Levels: _OlapCollection readonly dispid 1745027141;
    property IsTime: WordBool dispid 1745027139;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809446;
    procedure UnlockObject; dispid 1610809447;
    property IsTemporary: WordBool readonly dispid 1745027138;
    property Parent: _Database dispid 1745027137;
    property Name: WideString dispid 1745027136;
    property Path: WideString readonly dispid 1745027135;
    property Description: WideString dispid 1745027133;
    property DataSource: _DataSource dispid 1745027132;
    property LastUpdated: TDateTime dispid 1745027131;
    property LastProcessed: TDateTime readonly dispid 1745027130;
    property Huge: Smallint dispid 1745027129;
    property DimensionType: DimensionTypes dispid 1745027128;
    property SourceTable: WideString readonly dispid 1745027127;
    property FromClause: WideString dispid 1745027126;
    property JoinClause: WideString dispid 1745027125;
    property IsValid: WordBool readonly dispid 1745027123;
    property ClassType: Smallint readonly dispid 1745027122;
    property SubClassType: SubClassTypes dispid 1745027121;
    property Committed: WordBool dispid 1745027120;
    procedure Process(ProcessOption: ProcessTypes); dispid 1610809452;
    property IObject: IDispatch readonly dispid 1745027118;
    property IDimension: IDispatch readonly dispid 1745027117;
    procedure ClearCollections; dispid 1610809458;
    procedure GhostMethod__DbDimension_272_13; dispid 1610743869;
    procedure GhostMethod__DbDimension_276_14; dispid 1610743870;
    procedure GhostMethod__DbDimension_280_15; dispid 1610743871;
    procedure GhostMethod__DbDimension_284_16; dispid 1610743872;
    procedure GhostMethod__DbDimension_288_17; dispid 1610743873;
    procedure GhostMethod__DbDimension_292_18; dispid 1610743874;
    procedure GhostMethod__DbDimension_296_19; dispid 1610743875;
    procedure GhostMethod__DbDimension_300_20; dispid 1610743876;
    procedure GhostMethod__DbDimension_304_21; dispid 1610743877;
    procedure GhostMethod__DbDimension_308_22; dispid 1610743878;
    procedure GhostMethod__DbDimension_312_23; dispid 1610743879;
    procedure GhostMethod__DbDimension_316_24; dispid 1610743880;
    procedure GhostMethod__DbDimension_324_25; dispid 1610743882;
    procedure GhostMethod__DbDimension_328_26; dispid 1610743883;
    procedure GhostMethod__DbDimension_332_27; dispid 1610743884;
    procedure GhostMethod__DbDimension_336_28; dispid 1610743885;
    procedure GhostMethod__DbDimension_340_29; dispid 1610743886;
    procedure GhostMethod__DbDimension_344_30; dispid 1610743887;
    procedure GhostMethod__DbDimension_348_31; dispid 1610743888;
    procedure GhostMethod__DbDimension_352_32; dispid 1610743889;
    procedure GhostMethod__DbDimension_356_33; dispid 1610743890;
    procedure GhostMethod__DbDimension_360_34; dispid 1610743891;
    procedure GhostMethod__DbDimension_364_35; dispid 1610743892;
    procedure GhostMethod__DbDimension_368_36; dispid 1610743893;
    property DefaultMember: WideString dispid 1745027299;
    property IsVirtual: WordBool dispid 1745027298;
    property MembersWithData: MembersWithDataValues dispid 1745027297;
    property DataMemberCaptionTemplate: WideString dispid 1745027296;
    property DependsOnDimension: WideString dispid 1745027295;
    property IsChanging: WordBool dispid 1745027294;
    property IsReadWrite: WordBool dispid 1745027293;
    property StorageMode: StorageModeValues dispid 1745027292;
    property SourceTableFilter: WideString dispid 1745027291;
    property AreMemberNamesUnique: WordBool dispid 1745027290;
    property AreMemberKeysUnique: WordBool dispid 1745027289;
    property AllowSiblingsWithSameName: WordBool dispid 1745027288;
    property EnableRealTimeUpdates: WordBool dispid 1745027287;
    property SourceTableAlias: WideString readonly dispid 1745027279;
    property OrdinalPosition: Smallint readonly dispid 1745027189;
    property AggregationUsage: DimensionAggUsageTypes dispid 1745027188;
    property IsVisible: WordBool dispid 1745027187;
    procedure Update; dispid 1610809611;
  end;

// *********************************************************************//
// Interface: _DBGroup
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582D-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DBGroup = interface(IDispatch)
    ['{E8AC582D-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DBGroup_28_1; safecall;
    procedure GhostMethod__DBGroup_32_2; safecall;
    procedure GhostMethod__DBGroup_36_3; safecall;
    procedure GhostMethod__DBGroup_40_4; safecall;
    procedure GhostMethod__DBGroup_44_5; safecall;
    procedure GhostMethod__DBGroup_48_6; safecall;
    procedure GhostMethod__DBGroup_52_7; safecall;
    procedure GhostMethod__DBGroup_56_8; safecall;
    procedure GhostMethod__DBGroup_60_9; safecall;
    procedure GhostMethod__DBGroup_64_10; safecall;
    procedure GhostMethod__DBGroup_68_11; safecall;
    procedure GhostMethod__DBGroup_72_12; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure SaveObject(var AssumeInsert: WordBool); safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_UsersList: WideString; safecall;
    procedure Set_UsersList(const Param1: WideString); safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    procedure _Set_Parent(const Param1: _Database); safecall;
    function Get_Parent: _Database; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    function Get_IObject: IDispatch; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__DBGroup_172_13; safecall;
    procedure GhostMethod__DBGroup_176_14; safecall;
    procedure GhostMethod__DBGroup_180_15; safecall;
    procedure GhostMethod__DBGroup_184_16; safecall;
    procedure GhostMethod__DBGroup_188_17; safecall;
    procedure GhostMethod__DBGroup_192_18; safecall;
    procedure GhostMethod__DBGroup_196_19; safecall;
    procedure GhostMethod__DBGroup_200_20; safecall;
    procedure GhostMethod__DBGroup_204_21; safecall;
    procedure GhostMethod__DBGroup_208_22; safecall;
    procedure GhostMethod__DBGroup_212_23; safecall;
    procedure GhostMethod__DBGroup_216_24; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function SetPermissions(const Key: WideString; const PermissionExpression: WideString): WordBool; safecall;
    function Get_Permissions(const Key: WideString): WideString; safecall;
    procedure _Set_ParentObject(const Param1: IDispatch); safecall;
    function Get_ParentObject: IDispatch; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Update; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property UsersList: WideString read Get_UsersList write Set_UsersList;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _Database read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property Commands: _OlapCollection read Get_Commands;
    property Permissions[const Key: WideString]: WideString read Get_Permissions;
    property ParentObject: IDispatch read Get_ParentObject write _Set_ParentObject;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _DBGroupDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582D-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DBGroupDisp = dispinterface
    ['{E8AC582D-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DBGroup_28_1; dispid 1610743808;
    procedure GhostMethod__DBGroup_32_2; dispid 1610743809;
    procedure GhostMethod__DBGroup_36_3; dispid 1610743810;
    procedure GhostMethod__DBGroup_40_4; dispid 1610743811;
    procedure GhostMethod__DBGroup_44_5; dispid 1610743812;
    procedure GhostMethod__DBGroup_48_6; dispid 1610743813;
    procedure GhostMethod__DBGroup_52_7; dispid 1610743814;
    procedure GhostMethod__DBGroup_56_8; dispid 1610743815;
    procedure GhostMethod__DBGroup_60_9; dispid 1610743816;
    procedure GhostMethod__DBGroup_64_10; dispid 1610743817;
    procedure GhostMethod__DBGroup_68_11; dispid 1610743818;
    procedure GhostMethod__DBGroup_72_12; dispid 1610743819;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809400;
    property CustomProperties: _Properties readonly dispid 1745027124;
    procedure SaveObject(var AssumeInsert: WordBool); dispid 1610809403;
    property Description: WideString dispid 1745027121;
    property Valid: WordBool dispid 1745027119;
    property Name: WideString dispid 1745027118;
    property UsersList: WideString dispid 1745027117;
    property Num: Smallint dispid 1745027115;
    property Database: _Database readonly dispid 1745027114;
    property Server: _Server readonly dispid 1745027113;
    property Parent: _Database dispid 1745027112;
    property Path: WideString readonly dispid 1745027111;
    property ClassType: Smallint readonly dispid 1745027110;
    property SubClassType: SubClassTypes readonly dispid 1745027109;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809406;
    procedure UnlockObject; dispid 1610809407;
    property IObject: IDispatch readonly dispid 1745027108;
    procedure ClearCollections; dispid 1610809410;
    procedure GhostMethod__DBGroup_172_13; dispid 1610743844;
    procedure GhostMethod__DBGroup_176_14; dispid 1610743845;
    procedure GhostMethod__DBGroup_180_15; dispid 1610743846;
    procedure GhostMethod__DBGroup_184_16; dispid 1610743847;
    procedure GhostMethod__DBGroup_188_17; dispid 1610743848;
    procedure GhostMethod__DBGroup_192_18; dispid 1610743849;
    procedure GhostMethod__DBGroup_196_19; dispid 1610743850;
    procedure GhostMethod__DBGroup_200_20; dispid 1610743851;
    procedure GhostMethod__DBGroup_204_21; dispid 1610743852;
    procedure GhostMethod__DBGroup_208_22; dispid 1610743853;
    procedure GhostMethod__DBGroup_212_23; dispid 1610743854;
    procedure GhostMethod__DBGroup_216_24; dispid 1610743855;
    property Commands: _OlapCollection readonly dispid 1745027202;
    function SetPermissions(const Key: WideString; const PermissionExpression: WideString): WordBool; dispid 1610809486;
    property Permissions[const Key: WideString]: WideString readonly dispid 1745027199;
    property ParentObject: IDispatch dispid 1745027197;
    property IsValid: WordBool readonly dispid 1745027139;
    procedure Update; dispid 1610809506;
  end;

// *********************************************************************//
// Interface: _DbLevel
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582A-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DbLevel = interface(IDispatch)
    ['{E8AC582A-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DbLevel_28_1; safecall;
    procedure GhostMethod__DbLevel_32_2; safecall;
    procedure GhostMethod__DbLevel_36_3; safecall;
    procedure GhostMethod__DbLevel_40_4; safecall;
    procedure GhostMethod__DbLevel_44_5; safecall;
    procedure GhostMethod__DbLevel_48_6; safecall;
    procedure GhostMethod__DbLevel_52_7; safecall;
    procedure GhostMethod__DbLevel_56_8; safecall;
    procedure GhostMethod__DbLevel_60_9; safecall;
    procedure GhostMethod__DbLevel_64_10; safecall;
    procedure GhostMethod__DbLevel_68_11; safecall;
    procedure GhostMethod__DbLevel_72_12; safecall;
    procedure GhostMethod__DbLevel_76_13; safecall;
    procedure GhostMethod__DbLevel_80_14; safecall;
    procedure GhostMethod__DbLevel_84_15; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Parent: _DbDimension; safecall;
    procedure _Set_Parent(const Param1: _DbDimension); safecall;
    function Get_Path: WideString; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Server: _Server; safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Database: _Database; safecall;
    function Get_Name: WideString; safecall;
    function Get_Ordering: OrderTypes; safecall;
    procedure Set_Ordering(Param1: OrderTypes); safecall;
    function Get_MemberNameColumn: WideString; safecall;
    procedure Set_MemberNameColumn(const Param1: WideString); safecall;
    function Get_MemberKeyTable: WideString; safecall;
    function Get_MemberKeyColumn: WideString; safecall;
    procedure Set_MemberKeyColumn(const Param1: WideString); safecall;
    function Validate: ValidateErrorCodes; safecall;
    function Get_FromClause: WideString; safecall;
    function Get_JoinClause: WideString; safecall;
    function Get_AggregationColumn: WideString; safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(Param1: Smallint); safecall;
    function Get_IsAll: WordBool; safecall;
    procedure Set_IsAll(var Param1: WordBool); safecall;
    function Get_ColumnSize: Smallint; safecall;
    procedure Set_ColumnSize(Param1: Smallint); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_UniqueItems: WordBool; safecall;
    procedure Set_UniqueItems(var Param1: WordBool); safecall;
    function Get_EstimatedSize: Integer; safecall;
    procedure Set_EstimatedSize(Param1: Integer); safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure Set_SubClassType(Param1: SubClassTypes); safecall;
    function Get_LevelType: Smallint; safecall;
    procedure Set_LevelType(Param1: Smallint); safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_ILevel: IDispatch; safecall;
    function Get_MemberProperties: _OlapCollection; safecall;
    procedure ClearCollections; safecall;
    procedure GhostMethod__DbLevel_264_16; safecall;
    procedure GhostMethod__DbLevel_268_17; safecall;
    procedure GhostMethod__DbLevel_272_18; safecall;
    procedure GhostMethod__DbLevel_276_19; safecall;
    procedure GhostMethod__DbLevel_280_20; safecall;
    procedure GhostMethod__DbLevel_284_21; safecall;
    procedure GhostMethod__DbLevel_288_22; safecall;
    procedure GhostMethod__DbLevel_292_23; safecall;
    procedure GhostMethod__DbLevel_296_24; safecall;
    procedure GhostMethod__DbLevel_300_25; safecall;
    procedure GhostMethod__DbLevel_304_26; safecall;
    procedure GhostMethod__DbLevel_308_27; safecall;
    procedure GhostMethod__DbLevel_312_28; safecall;
    procedure GhostMethod__DbLevel_316_29; safecall;
    procedure GhostMethod__DbLevel_320_30; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(Param1: WordBool); safecall;
    function Get_CustomRollupExpression: WideString; safecall;
    procedure Set_CustomRollupExpression(const Param1: WideString); safecall;
    function Get_ParentKeyColumn: WideString; safecall;
    procedure Set_ParentKeyColumn(const Param1: WideString); safecall;
    function Get_LevelNamingTemplate: WideString; safecall;
    procedure Set_LevelNamingTemplate(const Param1: WideString); safecall;
    function Get_HideMemberIf: HideIfValues; safecall;
    procedure Set_HideMemberIf(Param1: HideIfValues); safecall;
    function Get_OrderingMemberProperty: WideString; safecall;
    procedure Set_OrderingMemberProperty(const Param1: WideString); safecall;
    function Get_Grouping: GroupingValues; safecall;
    procedure Set_Grouping(Param1: GroupingValues); safecall;
    function Get_SkippedLevelsColumn: WideString; safecall;
    procedure Set_SkippedLevelsColumn(const Param1: WideString); safecall;
    function Get_AreMemberNamesUnique: WordBool; safecall;
    procedure Set_AreMemberNamesUnique(Param1: WordBool); safecall;
    function Get_AreMemberKeysUnique: WordBool; safecall;
    procedure Set_AreMemberKeysUnique(Param1: WordBool); safecall;
    function Get_CustomRollupColumn: WideString; safecall;
    procedure Set_CustomRollupColumn(const Param1: WideString); safecall;
    function Get_CustomRollupPropertiesColumn: WideString; safecall;
    procedure Set_CustomRollupPropertiesColumn(const Param1: WideString); safecall;
    function Get_RootMemberIf: RootIfValues; safecall;
    procedure Set_RootMemberIf(Param1: RootIfValues); safecall;
    function Get_UnaryOperatorColumn: WideString; safecall;
    procedure Set_UnaryOperatorColumn(const Param1: WideString); safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    function Get_IsDisabled: WordBool; safecall;
    procedure Set_IsDisabled(Param1: WordBool); safecall;
    function Get_IsUnique: WordBool; safecall;
    procedure Set_IsUnique(Param1: WordBool); safecall;
    function Get_SliceValue: WideString; safecall;
    procedure Set_SliceValue(const Param1: WideString); safecall;
    function Get_EnableAggregations: WordBool; safecall;
    procedure Set_EnableAggregations(Param1: WordBool); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Parent: _DbDimension read Get_Parent write _Set_Parent;
    property Path: WideString read Get_Path;
    property Name: WideString read Get_Name write Set_Name;
    property Server: _Server read Get_Server;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Ordering: OrderTypes read Get_Ordering write Set_Ordering;
    property MemberNameColumn: WideString read Get_MemberNameColumn write Set_MemberNameColumn;
    property MemberKeyTable: WideString read Get_MemberKeyTable;
    property MemberKeyColumn: WideString read Get_MemberKeyColumn write Set_MemberKeyColumn;
    property FromClause: WideString read Get_FromClause;
    property JoinClause: WideString read Get_JoinClause;
    property AggregationColumn: WideString read Get_AggregationColumn;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property IsAll: WordBool read Get_IsAll write Set_IsAll;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property Description: WideString read Get_Description write Set_Description;
    property UniqueItems: WordBool read Get_UniqueItems write Set_UniqueItems;
    property EstimatedSize: Integer read Get_EstimatedSize write Set_EstimatedSize;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType write Set_SubClassType;
    property LevelType: Smallint read Get_LevelType write Set_LevelType;
    property IObject: IDispatch read Get_IObject;
    property ILevel: IDispatch read Get_ILevel;
    property MemberProperties: _OlapCollection read Get_MemberProperties;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property CustomRollupExpression: WideString read Get_CustomRollupExpression write Set_CustomRollupExpression;
    property ParentKeyColumn: WideString read Get_ParentKeyColumn write Set_ParentKeyColumn;
    property LevelNamingTemplate: WideString read Get_LevelNamingTemplate write Set_LevelNamingTemplate;
    property HideMemberIf: HideIfValues read Get_HideMemberIf write Set_HideMemberIf;
    property OrderingMemberProperty: WideString read Get_OrderingMemberProperty write Set_OrderingMemberProperty;
    property Grouping: GroupingValues read Get_Grouping write Set_Grouping;
    property SkippedLevelsColumn: WideString read Get_SkippedLevelsColumn write Set_SkippedLevelsColumn;
    property AreMemberNamesUnique: WordBool read Get_AreMemberNamesUnique write Set_AreMemberNamesUnique;
    property AreMemberKeysUnique: WordBool read Get_AreMemberKeysUnique write Set_AreMemberKeysUnique;
    property CustomRollupColumn: WideString read Get_CustomRollupColumn write Set_CustomRollupColumn;
    property CustomRollupPropertiesColumn: WideString read Get_CustomRollupPropertiesColumn write Set_CustomRollupPropertiesColumn;
    property RootMemberIf: RootIfValues read Get_RootMemberIf write Set_RootMemberIf;
    property UnaryOperatorColumn: WideString read Get_UnaryOperatorColumn write Set_UnaryOperatorColumn;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property IsUnique: WordBool read Get_IsUnique write Set_IsUnique;
    property SliceValue: WideString read Get_SliceValue write Set_SliceValue;
    property EnableAggregations: WordBool read Get_EnableAggregations write Set_EnableAggregations;
  end;

// *********************************************************************//
// DispIntf:  _DbLevelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC582A-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DbLevelDisp = dispinterface
    ['{E8AC582A-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DbLevel_28_1; dispid 1610743808;
    procedure GhostMethod__DbLevel_32_2; dispid 1610743809;
    procedure GhostMethod__DbLevel_36_3; dispid 1610743810;
    procedure GhostMethod__DbLevel_40_4; dispid 1610743811;
    procedure GhostMethod__DbLevel_44_5; dispid 1610743812;
    procedure GhostMethod__DbLevel_48_6; dispid 1610743813;
    procedure GhostMethod__DbLevel_52_7; dispid 1610743814;
    procedure GhostMethod__DbLevel_56_8; dispid 1610743815;
    procedure GhostMethod__DbLevel_60_9; dispid 1610743816;
    procedure GhostMethod__DbLevel_64_10; dispid 1610743817;
    procedure GhostMethod__DbLevel_68_11; dispid 1610743818;
    procedure GhostMethod__DbLevel_72_12; dispid 1610743819;
    procedure GhostMethod__DbLevel_76_13; dispid 1610743820;
    procedure GhostMethod__DbLevel_80_14; dispid 1610743821;
    procedure GhostMethod__DbLevel_84_15; dispid 1610743822;
    property CustomProperties: _Properties readonly dispid 1745027172;
    property Valid: WordBool dispid 1745027169;
    property Parent: _DbDimension dispid 1745027167;
    property Path: WideString readonly dispid 1745027165;
    property Name: WideString dispid 1745027164;
    property Server: _Server readonly dispid 1745027163;
    property Num: Smallint dispid 1745027162;
    property Database: _Database readonly dispid 1745027161;
    property Ordering: OrderTypes dispid 1745027160;
    property MemberNameColumn: WideString dispid 1745027159;
    property MemberKeyTable: WideString readonly dispid 1745027158;
    property MemberKeyColumn: WideString dispid 1745027157;
    function Validate: ValidateErrorCodes; dispid 1610809450;
    property FromClause: WideString readonly dispid 1745027156;
    property JoinClause: WideString readonly dispid 1745027155;
    property AggregationColumn: WideString readonly dispid 1745027153;
    property ColumnType: Smallint dispid 1745027152;
    property IsAll: WordBool dispid 1745027149;
    property ColumnSize: Smallint dispid 1745027147;
    property Description: WideString dispid 1745027146;
    property UniqueItems: WordBool dispid 1745027145;
    property EstimatedSize: Integer dispid 1745027143;
    property ClassType: Smallint readonly dispid 1745027142;
    property SubClassType: SubClassTypes dispid 1745027141;
    property LevelType: Smallint dispid 1745027140;
    property IObject: IDispatch readonly dispid 1745027138;
    property ILevel: IDispatch readonly dispid 1745027136;
    property MemberProperties: _OlapCollection readonly dispid 1745027135;
    procedure ClearCollections; dispid 1610809460;
    procedure GhostMethod__DbLevel_264_16; dispid 1610743867;
    procedure GhostMethod__DbLevel_268_17; dispid 1610743868;
    procedure GhostMethod__DbLevel_272_18; dispid 1610743869;
    procedure GhostMethod__DbLevel_276_19; dispid 1610743870;
    procedure GhostMethod__DbLevel_280_20; dispid 1610743871;
    procedure GhostMethod__DbLevel_284_21; dispid 1610743872;
    procedure GhostMethod__DbLevel_288_22; dispid 1610743873;
    procedure GhostMethod__DbLevel_292_23; dispid 1610743874;
    procedure GhostMethod__DbLevel_296_24; dispid 1610743875;
    procedure GhostMethod__DbLevel_300_25; dispid 1610743876;
    procedure GhostMethod__DbLevel_304_26; dispid 1610743877;
    procedure GhostMethod__DbLevel_308_27; dispid 1610743878;
    procedure GhostMethod__DbLevel_312_28; dispid 1610743879;
    procedure GhostMethod__DbLevel_316_29; dispid 1610743880;
    procedure GhostMethod__DbLevel_320_30; dispid 1610743881;
    property IsVisible: WordBool dispid 1745027330;
    property CustomRollupExpression: WideString dispid 1745027329;
    property ParentKeyColumn: WideString dispid 1745027328;
    property LevelNamingTemplate: WideString dispid 1745027327;
    property HideMemberIf: HideIfValues dispid 1745027326;
    property OrderingMemberProperty: WideString dispid 1745027325;
    property Grouping: GroupingValues dispid 1745027324;
    property SkippedLevelsColumn: WideString dispid 1745027323;
    property AreMemberNamesUnique: WordBool dispid 1745027322;
    property AreMemberKeysUnique: WordBool dispid 1745027321;
    property CustomRollupColumn: WideString dispid 1745027320;
    property CustomRollupPropertiesColumn: WideString dispid 1745027319;
    property RootMemberIf: RootIfValues dispid 1745027318;
    property UnaryOperatorColumn: WideString dispid 1745027317;
    property OrdinalPosition: Smallint readonly dispid 1745027194;
    property IsValid: WordBool readonly dispid 1745027193;
    property IsDisabled: WordBool dispid 1745027192;
    property IsUnique: WordBool dispid 1745027191;
    property SliceValue: WideString dispid 1745027190;
    property EnableAggregations: WordBool dispid 1745027189;
  end;

// *********************************************************************//
// Interface: _MemberProperty
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5829-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _MemberProperty = interface(IDispatch)
    ['{E8AC5829-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__MemberProperty_28_1; safecall;
    procedure GhostMethod__MemberProperty_32_2; safecall;
    procedure GhostMethod__MemberProperty_36_3; safecall;
    procedure GhostMethod__MemberProperty_40_4; safecall;
    procedure GhostMethod__MemberProperty_44_5; safecall;
    procedure GhostMethod__MemberProperty_48_6; safecall;
    procedure GhostMethod__MemberProperty_52_7; safecall;
    procedure GhostMethod__MemberProperty_56_8; safecall;
    procedure GhostMethod__MemberProperty_60_9; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_ClassType: ClassTypes; safecall;
    procedure Set_SourceColumn(const Param1: WideString); safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_Caption(const Param1: WideString); safecall;
    function Get_Caption: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_Parent: _Level; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Valid(Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    function Get_Path: WideString; safecall;
    function Get_Server: _Server; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure ClearCollections; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Num(Param1: Smallint); safecall;
    procedure GhostMethod__MemberProperty_144_10; safecall;
    procedure GhostMethod__MemberProperty_148_11; safecall;
    procedure GhostMethod__MemberProperty_152_12; safecall;
    procedure GhostMethod__MemberProperty_156_13; safecall;
    procedure GhostMethod__MemberProperty_160_14; safecall;
    procedure GhostMethod__MemberProperty_164_15; safecall;
    procedure GhostMethod__MemberProperty_168_16; safecall;
    procedure GhostMethod__MemberProperty_172_17; safecall;
    procedure GhostMethod__MemberProperty_176_18; safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(var Param1: WordBool); safecall;
    function Get_ColumnSize: Smallint; safecall;
    procedure Set_ColumnSize(var Param1: Smallint); safecall;
    function Get_ColumnType: Smallint; safecall;
    procedure Set_ColumnType(var Param1: Smallint); safecall;
    function Get_Language: LanguageValues; safecall;
    procedure Set_Language(var Param1: LanguageValues); safecall;
    function Get_PropertyType: PropertyTypeValues; safecall;
    procedure Set_PropertyType(var Param1: PropertyTypeValues); safecall;
    property Name: WideString read Get_Name write Set_Name;
    property ClassType: ClassTypes read Get_ClassType;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property Caption: WideString read Get_Caption write Set_Caption;
    property Description: WideString read Get_Description write Set_Description;
    property Parent: _Level read Get_Parent;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Path: WideString read Get_Path;
    property Server: _Server read Get_Server;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property Num: Smallint read Get_Num write Set_Num;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property ColumnSize: Smallint read Get_ColumnSize write Set_ColumnSize;
    property ColumnType: Smallint read Get_ColumnType write Set_ColumnType;
    property Language: LanguageValues read Get_Language write Set_Language;
    property PropertyType: PropertyTypeValues read Get_PropertyType write Set_PropertyType;
  end;

// *********************************************************************//
// DispIntf:  _MemberPropertyDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5829-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _MemberPropertyDisp = dispinterface
    ['{E8AC5829-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__MemberProperty_28_1; dispid 1610743808;
    procedure GhostMethod__MemberProperty_32_2; dispid 1610743809;
    procedure GhostMethod__MemberProperty_36_3; dispid 1610743810;
    procedure GhostMethod__MemberProperty_40_4; dispid 1610743811;
    procedure GhostMethod__MemberProperty_44_5; dispid 1610743812;
    procedure GhostMethod__MemberProperty_48_6; dispid 1610743813;
    procedure GhostMethod__MemberProperty_52_7; dispid 1610743814;
    procedure GhostMethod__MemberProperty_56_8; dispid 1610743815;
    procedure GhostMethod__MemberProperty_60_9; dispid 1610743816;
    property Name: WideString dispid 1745027126;
    property ClassType: ClassTypes readonly dispid 1745027125;
    property SourceColumn: WideString dispid 1745027124;
    property Caption: WideString dispid 1745027123;
    property Description: WideString dispid 1745027122;
    property Parent: _Level readonly dispid 1745027121;
    property CustomProperties: _Properties readonly dispid 1745027120;
    property Valid: WordBool dispid 1745027119;
    property Path: WideString readonly dispid 1745027118;
    property Server: _Server readonly dispid 1745027117;
    property SubClassType: SubClassTypes readonly dispid 1745027116;
    procedure ClearCollections; dispid 1610809399;
    property OrdinalPosition: Smallint readonly dispid 1745027110;
    property Num: Smallint dispid 1745027109;
    procedure GhostMethod__MemberProperty_144_10; dispid 1610743837;
    procedure GhostMethod__MemberProperty_148_11; dispid 1610743838;
    procedure GhostMethod__MemberProperty_152_12; dispid 1610743839;
    procedure GhostMethod__MemberProperty_156_13; dispid 1610743840;
    procedure GhostMethod__MemberProperty_160_14; dispid 1610743841;
    procedure GhostMethod__MemberProperty_164_15; dispid 1610743842;
    procedure GhostMethod__MemberProperty_168_16; dispid 1610743843;
    procedure GhostMethod__MemberProperty_172_17; dispid 1610743844;
    procedure GhostMethod__MemberProperty_176_18; dispid 1610743845;
    property IsVisible: WordBool dispid 1745027198;
    property ColumnSize: Smallint dispid 1745027197;
    property ColumnType: Smallint dispid 1745027196;
    property Language: LanguageValues dispid 1745027195;
    property PropertyType: PropertyTypeValues dispid 1745027194;
  end;

// *********************************************************************//
// Interface: _DataSource
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5818-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DataSource = interface(IDispatch)
    ['{E8AC5818-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DataSource_28_1; safecall;
    procedure GhostMethod__DataSource_32_2; safecall;
    procedure GhostMethod__DataSource_36_3; safecall;
    procedure GhostMethod__DataSource_40_4; safecall;
    procedure GhostMethod__DataSource_44_5; safecall;
    procedure GhostMethod__DataSource_48_6; safecall;
    procedure GhostMethod__DataSource_52_7; safecall;
    procedure GhostMethod__DataSource_56_8; safecall;
    procedure GhostMethod__DataSource_60_9; safecall;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_IsReadOnly: WordBool; safecall;
    function Get_SupportedTxnDDL: Integer; safecall;
    procedure Set_Valid(Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Path: WideString; safecall;
    procedure Set_ConnectionString(const Param1: WideString); safecall;
    function Get_ConnectionString: WideString; safecall;
    function Update: WordBool; safecall;
    procedure _Set_Parent(const Param1: IDispatch); safecall;
    function Get_Parent: _MDStore; safecall;
    function IsConnected(var ErrorMsg: WideString): WordBool; safecall;
    function Get_OpenQuoteChar: OleVariant; safecall;
    function Get_CloseQuoteChar: OleVariant; safecall;
    function Connect(var ErrorMsg: WideString): WordBool; safecall;
    function Disconnect: WordBool; safecall;
    function Get_IsValid: WordBool; safecall;
    function ExecuteQuery(const query: WideString; const alternativeConnection: _Connection): WideString; safecall;
    function OpenRecordset(const query: WideString; var ErrorMsg: WideString): _Recordset; safecall;
    function Get_Connection: _Connection; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Commit(var server_srcs: IMSOLAPSources; var forceUpdate: WordBool): WordBool; safecall;
    function Get_IObject: IDispatch; safecall;
    function Get_IDatasource: IDispatch; safecall;
    function Get_ConnectTime: TDateTime; safecall;
    function QuotedName(const Name: WideString): WideString; safecall;
    procedure PrintProperties; safecall;
    procedure ClearCollections; safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property IsReadOnly: WordBool read Get_IsReadOnly;
    property SupportedTxnDDL: Integer read Get_SupportedTxnDDL;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property Path: WideString read Get_Path;
    property ConnectionString: WideString read Get_ConnectionString write Set_ConnectionString;
    property OpenQuoteChar: OleVariant read Get_OpenQuoteChar;
    property CloseQuoteChar: OleVariant read Get_CloseQuoteChar;
    property IsValid: WordBool read Get_IsValid;
    property Connection: _Connection read Get_Connection;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IObject: IDispatch read Get_IObject;
    property IDatasource: IDispatch read Get_IDatasource;
    property ConnectTime: TDateTime read Get_ConnectTime;
  end;

// *********************************************************************//
// DispIntf:  _DataSourceDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {E8AC5818-7127-11D2-8A35-00C04FB9898D}
// *********************************************************************//
  _DataSourceDisp = dispinterface
    ['{E8AC5818-7127-11D2-8A35-00C04FB9898D}']
    procedure GhostMethod__DataSource_28_1; dispid 1610743808;
    procedure GhostMethod__DataSource_32_2; dispid 1610743809;
    procedure GhostMethod__DataSource_36_3; dispid 1610743810;
    procedure GhostMethod__DataSource_40_4; dispid 1610743811;
    procedure GhostMethod__DataSource_44_5; dispid 1610743812;
    procedure GhostMethod__DataSource_48_6; dispid 1610743813;
    procedure GhostMethod__DataSource_52_7; dispid 1610743814;
    procedure GhostMethod__DataSource_56_8; dispid 1610743815;
    procedure GhostMethod__DataSource_60_9; dispid 1610743816;
    procedure Clone(const TargetObject: _ICommon; Options: CloneOptions); dispid 1610809408;
    property CustomProperties: _Properties readonly dispid 1745027132;
    property Description: WideString dispid 1745027129;
    property IsReadOnly: WordBool readonly dispid 1745027128;
    property SupportedTxnDDL: Integer readonly dispid 1745027127;
    property Valid: WordBool dispid 1745027126;
    property Name: WideString dispid 1745027125;
    property Path: WideString readonly dispid 1745027124;
    property ConnectionString: WideString dispid 1745027120;
    function Update: WordBool; dispid 1610809413;
    function Parent: IDispatch; dispid 1745027114;
    function IsConnected(var ErrorMsg: WideString): WordBool; dispid 1610809414;
    property OpenQuoteChar: OleVariant readonly dispid 1745027111;
    property CloseQuoteChar: OleVariant readonly dispid 1745027110;
    function Connect(var ErrorMsg: WideString): WordBool; dispid 1610809418;
    function Disconnect: WordBool; dispid 1610809419;
    property IsValid: WordBool readonly dispid 1745027108;
    function ExecuteQuery(const query: WideString; const alternativeConnection: _Connection): WideString; dispid 1610809424;
    function OpenRecordset(const query: WideString; var ErrorMsg: WideString): _Recordset; dispid 1610809425;
    property Connection: _Connection readonly dispid 1745027107;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809427;
    procedure UnlockObject; dispid 1610809428;
    property ClassType: Smallint readonly dispid 1745027106;
    property SubClassType: SubClassTypes readonly dispid 1745027105;
    function Commit(var server_srcs: IMSOLAPSources; var forceUpdate: WordBool): WordBool; dispid 1610809433;
    property IObject: IDispatch readonly dispid 1745027101;
    property IDatasource: IDispatch readonly dispid 1745027100;
    property ConnectTime: TDateTime readonly dispid 1745027099;
    function QuotedName(const Name: WideString): WideString; dispid 1610809437;
    procedure PrintProperties; dispid 1610809438;
    procedure ClearCollections; dispid 1610809439;
  end;

// *********************************************************************//
// Interface: _RoleCommand
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {B7B6AAD7-6E4C-4415-A61A-F6FC01A16E68}
// *********************************************************************//
  _RoleCommand = interface(IDispatch)
    ['{B7B6AAD7-6E4C-4415-A61A-F6FC01A16E68}']
    procedure GhostMethod__RoleCommand_28_1; safecall;
    procedure GhostMethod__RoleCommand_32_2; safecall;
    procedure GhostMethod__RoleCommand_36_3; safecall;
    procedure GhostMethod__RoleCommand_40_4; safecall;
    procedure GhostMethod__RoleCommand_44_5; safecall;
    procedure GhostMethod__RoleCommand_48_6; safecall;
    procedure GhostMethod__RoleCommand_52_7; safecall;
    procedure GhostMethod__RoleCommand_56_8; safecall;
    procedure GhostMethod__RoleCommand_60_9; safecall;
    procedure GhostMethod__RoleCommand_64_10; safecall;
    procedure GhostMethod__RoleCommand_68_11; safecall;
    procedure GhostMethod__RoleCommand_72_12; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_Statement: WideString; safecall;
    procedure Set_Statement(const Param1: WideString); safecall;
    procedure Set_CommandType(Param1: CommandTypes); safecall;
    function Get_CommandType: CommandTypes; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_ParentObject: IDispatch; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    procedure ClearCollections; safecall;
    function Get_OrdinalPosition: Smallint; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); safecall;
    property Description: WideString read Get_Description write Set_Description;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property Statement: WideString read Get_Statement write Set_Statement;
    property CommandType: CommandTypes read Get_CommandType write Set_CommandType;
    property Num: Smallint read Get_Num write Set_Num;
    property Parent: _MDStore read Get_Parent;
    property ParentObject: IDispatch read Get_ParentObject;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property OrdinalPosition: Smallint read Get_OrdinalPosition;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _RoleCommandDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {B7B6AAD7-6E4C-4415-A61A-F6FC01A16E68}
// *********************************************************************//
  _RoleCommandDisp = dispinterface
    ['{B7B6AAD7-6E4C-4415-A61A-F6FC01A16E68}']
    procedure GhostMethod__RoleCommand_28_1; dispid 1610743808;
    procedure GhostMethod__RoleCommand_32_2; dispid 1610743809;
    procedure GhostMethod__RoleCommand_36_3; dispid 1610743810;
    procedure GhostMethod__RoleCommand_40_4; dispid 1610743811;
    procedure GhostMethod__RoleCommand_44_5; dispid 1610743812;
    procedure GhostMethod__RoleCommand_48_6; dispid 1610743813;
    procedure GhostMethod__RoleCommand_52_7; dispid 1610743814;
    procedure GhostMethod__RoleCommand_56_8; dispid 1610743815;
    procedure GhostMethod__RoleCommand_60_9; dispid 1610743816;
    procedure GhostMethod__RoleCommand_64_10; dispid 1610743817;
    procedure GhostMethod__RoleCommand_68_11; dispid 1610743818;
    procedure GhostMethod__RoleCommand_72_12; dispid 1610743819;
    property Description: WideString dispid 1745027132;
    property CustomProperties: _Properties readonly dispid 1745027131;
    property Valid: WordBool dispid 1745027130;
    property Name: WideString dispid 1745027129;
    property Statement: WideString dispid 1745027128;
    property CommandType: CommandTypes dispid 1745027127;
    property Num: Smallint dispid 1745027126;
    property Parent: _MDStore readonly dispid 1745027125;
    property ParentObject: IDispatch readonly dispid 1745027124;
    property ClassType: ClassTypes readonly dispid 1745027123;
    property SubClassType: SubClassTypes readonly dispid 1745027122;
    procedure ClearCollections; dispid 1610809408;
    property OrdinalPosition: Smallint readonly dispid 1745027073;
    property IsValid: WordBool readonly dispid 1745027072;
    procedure Update; dispid 1610809422;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809423;
    procedure UnlockObject; dispid 1610809424;
    procedure Clone(const TargetObject: _Command; Options: CloneOptions); dispid 1610809425;
  end;

// *********************************************************************//
// Interface: _Column
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {932B8CFB-DAD2-4CEA-A963-82B057760865}
// *********************************************************************//
  _Column = interface(IDispatch)
    ['{932B8CFB-DAD2-4CEA-A963-82B057760865}']
    procedure GhostMethod__Column_28_1; safecall;
    procedure GhostMethod__Column_32_2; safecall;
    procedure GhostMethod__Column_36_3; safecall;
    procedure GhostMethod__Column_40_4; safecall;
    procedure GhostMethod__Column_44_5; safecall;
    procedure GhostMethod__Column_48_6; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(var Param1: WideString); safecall;
    function Get_Num: Smallint; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(var Param1: WideString); safecall;
    function Get_Parent: IDispatch; safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(var Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(var Param1: WideString); safecall;
    function Get_Filter: WideString; safecall;
    procedure Set_Filter(var Param1: WideString); safecall;
    function Get_AreKeysUnique: WordBool; safecall;
    procedure Set_AreKeysUnique(var Param1: WordBool); safecall;
    function Get_SourceColumn: WideString; safecall;
    procedure Set_SourceColumn(var Param1: WideString); safecall;
    function Get_SourceOlapObject: IDispatch; safecall;
    procedure _Set_SourceOlapObject(var Param1: IDispatch); safecall;
    function Get_IsDisabled: WordBool; safecall;
    procedure Set_IsDisabled(var Param1: WordBool); safecall;
    function Get_IsParentKey: WordBool; safecall;
    procedure Set_IsParentKey(var Param1: WordBool); safecall;
    function Get_IsKey: WordBool; safecall;
    procedure Set_IsKey(var Param1: WordBool); safecall;
    function Get_IsInput: WordBool; safecall;
    procedure Set_IsInput(var Param1: WordBool); safecall;
    function Get_IsPredictable: WordBool; safecall;
    procedure Set_IsPredictable(var Param1: WordBool); safecall;
    function Get_RelatedColumn: WideString; safecall;
    procedure Set_RelatedColumn(var Param1: WideString); safecall;
    function Get_DataType: DataTypeEnum; safecall;
    procedure Set_DataType(var Param1: DataTypeEnum); safecall;
    function Get_ContentType: WideString; safecall;
    procedure Set_ContentType(var Param1: WideString); safecall;
    function Get_Distribution: WideString; safecall;
    procedure Set_Distribution(var Param1: WideString); safecall;
    function Get_ModelingFlags: WideString; safecall;
    procedure Set_ModelingFlags(var Param1: WideString); safecall;
    function Get_SpecialFlag: WideString; safecall;
    procedure Set_SpecialFlag(var Param1: WideString); safecall;
    function Get_Columns: _OlapCollection; safecall;
    function Get_CustomProperties: _Properties; safecall;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property Name: WideString read Get_Name write Set_Name;
    property Num: Smallint read Get_Num write Set_Num;
    property Description: WideString read Get_Description write Set_Description;
    property Parent: IDispatch read Get_Parent;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property Filter: WideString read Get_Filter write Set_Filter;
    property AreKeysUnique: WordBool read Get_AreKeysUnique write Set_AreKeysUnique;
    property SourceColumn: WideString read Get_SourceColumn write Set_SourceColumn;
    property SourceOlapObject: IDispatch read Get_SourceOlapObject;
    property IsDisabled: WordBool read Get_IsDisabled write Set_IsDisabled;
    property IsParentKey: WordBool read Get_IsParentKey write Set_IsParentKey;
    property IsKey: WordBool read Get_IsKey write Set_IsKey;
    property IsInput: WordBool read Get_IsInput write Set_IsInput;
    property IsPredictable: WordBool read Get_IsPredictable write Set_IsPredictable;
    property RelatedColumn: WideString read Get_RelatedColumn write Set_RelatedColumn;
    property DataType: DataTypeEnum read Get_DataType write Set_DataType;
    property ContentType: WideString read Get_ContentType write Set_ContentType;
    property Distribution: WideString read Get_Distribution write Set_Distribution;
    property ModelingFlags: WideString read Get_ModelingFlags write Set_ModelingFlags;
    property SpecialFlag: WideString read Get_SpecialFlag write Set_SpecialFlag;
    property Columns: _OlapCollection read Get_Columns;
    property CustomProperties: _Properties read Get_CustomProperties;
  end;

// *********************************************************************//
// DispIntf:  _ColumnDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {932B8CFB-DAD2-4CEA-A963-82B057760865}
// *********************************************************************//
  _ColumnDisp = dispinterface
    ['{932B8CFB-DAD2-4CEA-A963-82B057760865}']
    procedure GhostMethod__Column_28_1; dispid 1610743808;
    procedure GhostMethod__Column_32_2; dispid 1610743809;
    procedure GhostMethod__Column_36_3; dispid 1610743810;
    procedure GhostMethod__Column_40_4; dispid 1610743811;
    procedure GhostMethod__Column_44_5; dispid 1610743812;
    procedure GhostMethod__Column_48_6; dispid 1610743813;
    property ClassType: ClassTypes readonly dispid 1745027136;
    property SubClassType: SubClassTypes readonly dispid 1745027135;
    property Name: WideString dispid 1745027134;
    property Num: Smallint dispid 1745027133;
    property Description: WideString dispid 1745027132;
    property Parent: IDispatch readonly dispid 1745027131;
    property FromClause: WideString dispid 1745027130;
    property JoinClause: WideString dispid 1745027129;
    property Filter: WideString dispid 1745027128;
    property AreKeysUnique: WordBool dispid 1745027127;
    property SourceColumn: WideString dispid 1745027126;
    property SourceOlapObject: IDispatch dispid 1745027125;
    property IsDisabled: WordBool dispid 1745027124;
    property IsParentKey: WordBool dispid 1745027123;
    property IsKey: WordBool dispid 1745027122;
    property IsInput: WordBool dispid 1745027121;
    property IsPredictable: WordBool dispid 1745027120;
    property RelatedColumn: WideString dispid 1745027119;
    property DataType: DataTypeEnum dispid 1745027118;
    property ContentType: WideString dispid 1745027117;
    property Distribution: WideString dispid 1745027116;
    property ModelingFlags: WideString dispid 1745027115;
    property SpecialFlag: WideString dispid 1745027114;
    property Columns: _OlapCollection readonly dispid 1745027113;
    property CustomProperties: _Properties readonly dispid 1745027112;
  end;

// *********************************************************************//
// Interface: _MiningModel
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {AC8F7BBF-9B71-4A7E-B18F-1088884A9EDA}
// *********************************************************************//
  _MiningModel = interface(IDispatch)
    ['{AC8F7BBF-9B71-4A7E-B18F-1088884A9EDA}']
    procedure GhostMethod__MiningModel_28_1; safecall;
    procedure GhostMethod__MiningModel_32_2; safecall;
    procedure GhostMethod__MiningModel_36_3; safecall;
    procedure GhostMethod__MiningModel_40_4; safecall;
    procedure GhostMethod__MiningModel_44_5; safecall;
    procedure GhostMethod__MiningModel_48_6; safecall;
    function Get_ClassType: ClassTypes; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_Name: WideString; safecall;
    procedure Set_Name(var Param1: WideString); safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Description(var Param1: WideString); safecall;
    function Get_IsVisible: WordBool; safecall;
    procedure Set_IsVisible(var Param1: WordBool); safecall;
    function Get_Parent: _MDStore; safecall;
    function Get_State: OlapStateTypes; safecall;
    function Get_LastProcessed: TDateTime; safecall;
    function Get_LastUpdated: TDateTime; safecall;
    procedure Set_LastUpdated(var Param1: TDateTime); safecall;
    function Get_MiningAlgorithm: WideString; safecall;
    procedure Set_MiningAlgorithm(var Param1: WideString); safecall;
    function Get_Parameters: WideString; safecall;
    procedure Set_Parameters(var Param1: WideString); safecall;
    function Get_FromClause: WideString; safecall;
    procedure Set_FromClause(var Param1: WideString); safecall;
    function Get_JoinClause: WideString; safecall;
    procedure Set_JoinClause(var Param1: WideString); safecall;
    function Get_Filter: WideString; safecall;
    procedure Set_Filter(var Param1: WideString); safecall;
    function Get_AreKeysUnique: WordBool; safecall;
    procedure Set_AreKeysUnique(var Param1: WordBool); safecall;
    function Get_SourceCube: WideString; safecall;
    procedure Set_SourceCube(var Param1: WideString); safecall;
    function Get_CaseDimension: WideString; safecall;
    procedure Set_CaseDimension(var Param1: WideString); safecall;
    function Get_CaseLevel: WideString; safecall;
    function Get_TrainingQuery: WideString; safecall;
    procedure Set_TrainingQuery(var Param1: WideString); safecall;
    function Get_XML: WideString; safecall;
    function Get_DataSources: _OlapCollection; safecall;
    function Get_Roles: _OlapCollection; safecall;
    function Get_Columns: _OlapCollection; safecall;
    function Get_CustomProperties: _Properties; safecall;
    procedure Clone(var TargetObject: _MiningModel; var Options: CloneOptions); safecall;
    procedure Process(ProcessOption: ProcessTypes); safecall;
    procedure Update; safecall;
    procedure ValidateStructure; safecall;
    procedure LockObject(var LockType: OlapLockTypes; var LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    property ClassType: ClassTypes read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property Name: WideString read Get_Name write Set_Name;
    property Description: WideString read Get_Description write Set_Description;
    property IsVisible: WordBool read Get_IsVisible write Set_IsVisible;
    property Parent: _MDStore read Get_Parent;
    property State: OlapStateTypes read Get_State;
    property LastProcessed: TDateTime read Get_LastProcessed;
    property LastUpdated: TDateTime read Get_LastUpdated write Set_LastUpdated;
    property MiningAlgorithm: WideString read Get_MiningAlgorithm write Set_MiningAlgorithm;
    property Parameters: WideString read Get_Parameters write Set_Parameters;
    property FromClause: WideString read Get_FromClause write Set_FromClause;
    property JoinClause: WideString read Get_JoinClause write Set_JoinClause;
    property Filter: WideString read Get_Filter write Set_Filter;
    property AreKeysUnique: WordBool read Get_AreKeysUnique write Set_AreKeysUnique;
    property SourceCube: WideString read Get_SourceCube write Set_SourceCube;
    property CaseDimension: WideString read Get_CaseDimension write Set_CaseDimension;
    property CaseLevel: WideString read Get_CaseLevel;
    property TrainingQuery: WideString read Get_TrainingQuery write Set_TrainingQuery;
    property XML: WideString read Get_XML;
    property DataSources: _OlapCollection read Get_DataSources;
    property Roles: _OlapCollection read Get_Roles;
    property Columns: _OlapCollection read Get_Columns;
    property CustomProperties: _Properties read Get_CustomProperties;
  end;

// *********************************************************************//
// DispIntf:  _MiningModelDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {AC8F7BBF-9B71-4A7E-B18F-1088884A9EDA}
// *********************************************************************//
  _MiningModelDisp = dispinterface
    ['{AC8F7BBF-9B71-4A7E-B18F-1088884A9EDA}']
    procedure GhostMethod__MiningModel_28_1; dispid 1610743808;
    procedure GhostMethod__MiningModel_32_2; dispid 1610743809;
    procedure GhostMethod__MiningModel_36_3; dispid 1610743810;
    procedure GhostMethod__MiningModel_40_4; dispid 1610743811;
    procedure GhostMethod__MiningModel_44_5; dispid 1610743812;
    procedure GhostMethod__MiningModel_48_6; dispid 1610743813;
    property ClassType: ClassTypes readonly dispid 1745027131;
    property SubClassType: SubClassTypes readonly dispid 1745027130;
    property Name: WideString dispid 1745027129;
    property Description: WideString dispid 1745027128;
    property IsVisible: WordBool dispid 1745027127;
    property Parent: _MDStore readonly dispid 1745027126;
    property State: OlapStateTypes readonly dispid 1745027125;
    property LastProcessed: TDateTime readonly dispid 1745027124;
    property LastUpdated: TDateTime dispid 1745027123;
    property MiningAlgorithm: WideString dispid 1745027122;
    property Parameters: WideString dispid 1745027121;
    property FromClause: WideString dispid 1745027120;
    property JoinClause: WideString dispid 1745027119;
    property Filter: WideString dispid 1745027118;
    property AreKeysUnique: WordBool dispid 1745027117;
    property SourceCube: WideString dispid 1745027116;
    property CaseDimension: WideString dispid 1745027115;
    property CaseLevel: WideString readonly dispid 1745027114;
    property TrainingQuery: WideString dispid 1745027113;
    property XML: WideString readonly dispid 1745027112;
    property DataSources: _OlapCollection readonly dispid 1745027111;
    property Roles: _OlapCollection readonly dispid 1745027110;
    property Columns: _OlapCollection readonly dispid 1745027109;
    property CustomProperties: _Properties readonly dispid 1745027108;
    procedure Clone(var TargetObject: _MiningModel; var Options: CloneOptions); dispid 1610809404;
    procedure Process(ProcessOption: ProcessTypes); dispid 1610809405;
    procedure Update; dispid 1610809406;
    procedure ValidateStructure; dispid 1610809407;
    procedure LockObject(var LockType: OlapLockTypes; var LockDescription: WideString); dispid 1610809408;
    procedure UnlockObject; dispid 1610809409;
  end;

// *********************************************************************//
// Interface: _MiningModelGroup
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {0C502ACB-20F6-40B7-B65C-CBED591C5BF1}
// *********************************************************************//
  _MiningModelGroup = interface(IDispatch)
    ['{0C502ACB-20F6-40B7-B65C-CBED591C5BF1}']
    procedure GhostMethod__MiningModelGroup_28_1; safecall;
    procedure GhostMethod__MiningModelGroup_32_2; safecall;
    procedure GhostMethod__MiningModelGroup_36_3; safecall;
    procedure GhostMethod__MiningModelGroup_40_4; safecall;
    procedure GhostMethod__MiningModelGroup_44_5; safecall;
    procedure GhostMethod__MiningModelGroup_48_6; safecall;
    procedure GhostMethod__MiningModelGroup_52_7; safecall;
    procedure GhostMethod__MiningModelGroup_56_8; safecall;
    procedure GhostMethod__MiningModelGroup_60_9; safecall;
    procedure GhostMethod__MiningModelGroup_64_10; safecall;
    procedure GhostMethod__MiningModelGroup_68_11; safecall;
    procedure GhostMethod__MiningModelGroup_72_12; safecall;
    function Get_CustomProperties: _Properties; safecall;
    function Get_Description: WideString; safecall;
    procedure Set_Valid(var Param1: WordBool); safecall;
    function Get_Valid: WordBool; safecall;
    procedure Set_Name(const Param1: WideString); safecall;
    function Get_Name: WideString; safecall;
    function Get_UsersList: WideString; safecall;
    function Get_Permissions(const Key: WideString): WideString; safecall;
    function SetPermissions(const Key: WideString; const Rights: WideString): WordBool; safecall;
    procedure Set_Num(var Param1: Smallint); safecall;
    function Get_Num: Smallint; safecall;
    function Get_Database: _Database; safecall;
    function Get_Server: _Server; safecall;
    function Get_Parent: _MiningModel; safecall;
    function Get_ParentObject: _MiningModel; safecall;
    function Get_MiningModel: _MiningModel; safecall;
    function Get_Commands: _OlapCollection; safecall;
    function Get_Path: WideString; safecall;
    function Get_ClassType: Smallint; safecall;
    function Get_SubClassType: SubClassTypes; safecall;
    function Get_IsValid: WordBool; safecall;
    procedure Set_Description(const Param1: WideString); safecall;
    procedure Set_UsersList(const Param1: WideString); safecall;
    procedure Update; safecall;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); safecall;
    procedure UnlockObject; safecall;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); safecall;
    property CustomProperties: _Properties read Get_CustomProperties;
    property Description: WideString read Get_Description write Set_Description;
    property Valid: WordBool read Get_Valid write Set_Valid;
    property Name: WideString read Get_Name write Set_Name;
    property UsersList: WideString read Get_UsersList write Set_UsersList;
    property Permissions[const Key: WideString]: WideString read Get_Permissions;
    property Num: Smallint read Get_Num write Set_Num;
    property Database: _Database read Get_Database;
    property Server: _Server read Get_Server;
    property Parent: _MiningModel read Get_Parent;
    property ParentObject: _MiningModel read Get_ParentObject;
    property MiningModel: _MiningModel read Get_MiningModel;
    property Commands: _OlapCollection read Get_Commands;
    property Path: WideString read Get_Path;
    property ClassType: Smallint read Get_ClassType;
    property SubClassType: SubClassTypes read Get_SubClassType;
    property IsValid: WordBool read Get_IsValid;
  end;

// *********************************************************************//
// DispIntf:  _MiningModelGroupDisp
// Flags:     (4560) Hidden Dual NonExtensible OleAutomation Dispatchable
// GUID:      {0C502ACB-20F6-40B7-B65C-CBED591C5BF1}
// *********************************************************************//
  _MiningModelGroupDisp = dispinterface
    ['{0C502ACB-20F6-40B7-B65C-CBED591C5BF1}']
    procedure GhostMethod__MiningModelGroup_28_1; dispid 1610743808;
    procedure GhostMethod__MiningModelGroup_32_2; dispid 1610743809;
    procedure GhostMethod__MiningModelGroup_36_3; dispid 1610743810;
    procedure GhostMethod__MiningModelGroup_40_4; dispid 1610743811;
    procedure GhostMethod__MiningModelGroup_44_5; dispid 1610743812;
    procedure GhostMethod__MiningModelGroup_48_6; dispid 1610743813;
    procedure GhostMethod__MiningModelGroup_52_7; dispid 1610743814;
    procedure GhostMethod__MiningModelGroup_56_8; dispid 1610743815;
    procedure GhostMethod__MiningModelGroup_60_9; dispid 1610743816;
    procedure GhostMethod__MiningModelGroup_64_10; dispid 1610743817;
    procedure GhostMethod__MiningModelGroup_68_11; dispid 1610743818;
    procedure GhostMethod__MiningModelGroup_72_12; dispid 1610743819;
    property CustomProperties: _Properties readonly dispid 1745027153;
    property Description: WideString dispid 1745027148;
    property Valid: WordBool dispid 1745027145;
    property Name: WideString dispid 1745027144;
    property UsersList: WideString dispid 1745027143;
    property Permissions[const Key: WideString]: WideString readonly dispid 1745027142;
    function SetPermissions(const Key: WideString; const Rights: WideString): WordBool; dispid 1610809431;
    property Num: Smallint dispid 1745027140;
    property Database: _Database readonly dispid 1745027139;
    property Server: _Server readonly dispid 1745027138;
    property Parent: _MiningModel readonly dispid 1745027147;
    property ParentObject: _MiningModel readonly dispid 1745027146;
    property MiningModel: _MiningModel readonly dispid 1745027136;
    property Commands: _OlapCollection readonly dispid 1745027134;
    property Path: WideString readonly dispid 1745027133;
    property ClassType: Smallint readonly dispid 1745027132;
    property SubClassType: SubClassTypes readonly dispid 1745027131;
    property IsValid: WordBool readonly dispid 1745027072;
    procedure Update; dispid 1610809451;
    procedure LockObject(LockType: OlapLockTypes; const LockDescription: WideString); dispid 1610809452;
    procedure UnlockObject; dispid 1610809453;
    procedure Clone(const TargetObject: _Role; Options: CloneOptions); dispid 1610809454;
  end;

// *********************************************************************//
// The Class CoCommand provides a Create and CreateRemote method to          
// create instances of the default interface _Command exposed by              
// the CoClass Command. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCommand = class
    class function Create: _Command;
    class function CreateRemote(const MachineName: string): _Command;
  end;

// *********************************************************************//
// The Class CoPartitionLevel provides a Create and CreateRemote method to          
// create instances of the default interface _PartitionLevel exposed by              
// the CoClass PartitionLevel. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPartitionLevel = class
    class function Create: _PartitionLevel;
    class function CreateRemote(const MachineName: string): _PartitionLevel;
  end;

// *********************************************************************//
// The Class CoLevel provides a Create and CreateRemote method to          
// create instances of the default interface _Level exposed by              
// the CoClass Level. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoLevel = class
    class function Create: _Level;
    class function CreateRemote(const MachineName: string): _Level;
  end;

// *********************************************************************//
// The Class CoMeasure provides a Create and CreateRemote method to          
// create instances of the default interface _Measure exposed by              
// the CoClass Measure. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMeasure = class
    class function Create: _Measure;
    class function CreateRemote(const MachineName: string): _Measure;
  end;

// *********************************************************************//
// The Class CoRole provides a Create and CreateRemote method to          
// create instances of the default interface _Role exposed by              
// the CoClass Role. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoRole = class
    class function Create: _Role;
    class function CreateRemote(const MachineName: string): _Role;
  end;

// *********************************************************************//
// The Class CoPartitionMeasure provides a Create and CreateRemote method to          
// create instances of the default interface _PartitionMeasure exposed by              
// the CoClass PartitionMeasure. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPartitionMeasure = class
    class function Create: _PartitionMeasure;
    class function CreateRemote(const MachineName: string): _PartitionMeasure;
  end;

// *********************************************************************//
// The Class CoICommon provides a Create and CreateRemote method to          
// create instances of the default interface _ICommon exposed by              
// the CoClass ICommon. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoICommon = class
    class function Create: _ICommon;
    class function CreateRemote(const MachineName: string): _ICommon;
  end;

// *********************************************************************//
// The Class CoProperties provides a Create and CreateRemote method to          
// create instances of the default interface _Properties exposed by              
// the CoClass Properties. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoProperties = class
    class function Create: _Properties;
    class function CreateRemote(const MachineName: string): _Properties;
  end;

// *********************************************************************//
// The Class CoProperty_ provides a Create and CreateRemote method to          
// create instances of the default interface _Property exposed by              
// the CoClass Property_. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoProperty_ = class
    class function Create: _Property;
    class function CreateRemote(const MachineName: string): _Property;
  end;

// *********************************************************************//
// The Class CoCubeAnalyzer provides a Create and CreateRemote method to          
// create instances of the default interface _CubeAnalyzer exposed by              
// the CoClass CubeAnalyzer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeAnalyzer = class
    class function Create: _CubeAnalyzer;
    class function CreateRemote(const MachineName: string): _CubeAnalyzer;
  end;

// *********************************************************************//
// The Class CoPartitionAnalyzer provides a Create and CreateRemote method to          
// create instances of the default interface _PartitionAnalyzer exposed by              
// the CoClass PartitionAnalyzer. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPartitionAnalyzer = class
    class function Create: _PartitionAnalyzer;
    class function CreateRemote(const MachineName: string): _PartitionAnalyzer;
  end;

// *********************************************************************//
// The Class CoPartitionDimension provides a Create and CreateRemote method to          
// create instances of the default interface _PartitionDimension exposed by              
// the CoClass PartitionDimension. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPartitionDimension = class
    class function Create: _PartitionDimension;
    class function CreateRemote(const MachineName: string): _PartitionDimension;
  end;

// *********************************************************************//
// The Class CoDimension provides a Create and CreateRemote method to          
// create instances of the default interface _Dimension exposed by              
// the CoClass Dimension. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDimension = class
    class function Create: _Dimension;
    class function CreateRemote(const MachineName: string): _Dimension;
  end;

// *********************************************************************//
// The Class CoIDatabaseEvents provides a Create and CreateRemote method to          
// create instances of the default interface _IDatabaseEvents exposed by              
// the CoClass IDatabaseEvents. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoIDatabaseEvents = class
    class function Create: _IDatabaseEvents;
    class function CreateRemote(const MachineName: string): _IDatabaseEvents;
  end;

// *********************************************************************//
// The Class CoServer provides a Create and CreateRemote method to          
// create instances of the default interface _Server exposed by              
// the CoClass Server. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoServer = class
    class function Create: _Server;
    class function CreateRemote(const MachineName: string): _Server;
  end;

// *********************************************************************//
// The Class CoMDStore provides a Create and CreateRemote method to          
// create instances of the default interface _MDStore exposed by              
// the CoClass MDStore. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMDStore = class
    class function Create: _MDStore;
    class function CreateRemote(const MachineName: string): _MDStore;
  end;

// *********************************************************************//
// The Class CoOlapCollection provides a Create and CreateRemote method to          
// create instances of the default interface _OlapCollection exposed by              
// the CoClass OlapCollection. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoOlapCollection = class
    class function Create: _OlapCollection;
    class function CreateRemote(const MachineName: string): _OlapCollection;
  end;

// *********************************************************************//
// The Class CoPartition provides a Create and CreateRemote method to          
// create instances of the default interface _Partition exposed by              
// the CoClass Partition. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoPartition = class
    class function Create: _Partition;
    class function CreateRemote(const MachineName: string): _Partition;
  end;

// *********************************************************************//
// The Class CoAggregation provides a Create and CreateRemote method to          
// create instances of the default interface _Aggregation exposed by              
// the CoClass Aggregation. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoAggregation = class
    class function Create: _Aggregation;
    class function CreateRemote(const MachineName: string): _Aggregation;
  end;

// *********************************************************************//
// The Class CoAggregationDimension provides a Create and CreateRemote method to          
// create instances of the default interface _AggregationDimension exposed by              
// the CoClass AggregationDimension. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoAggregationDimension = class
    class function Create: _AggregationDimension;
    class function CreateRemote(const MachineName: string): _AggregationDimension;
  end;

// *********************************************************************//
// The Class CoAggregationLevel provides a Create and CreateRemote method to          
// create instances of the default interface _AggregationLevel exposed by              
// the CoClass AggregationLevel. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoAggregationLevel = class
    class function Create: _AggregationLevel;
    class function CreateRemote(const MachineName: string): _AggregationLevel;
  end;

// *********************************************************************//
// The Class CoAggregationMeasure provides a Create and CreateRemote method to          
// create instances of the default interface _AggregationMeasure exposed by              
// the CoClass AggregationMeasure. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoAggregationMeasure = class
    class function Create: _AggregationMeasure;
    class function CreateRemote(const MachineName: string): _AggregationMeasure;
  end;

// *********************************************************************//
// The Class CoCube provides a Create and CreateRemote method to          
// create instances of the default interface _Cube exposed by              
// the CoClass Cube. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCube = class
    class function Create: _Cube;
    class function CreateRemote(const MachineName: string): _Cube;
  end;

// *********************************************************************//
// The Class CoCubeCommand provides a Create and CreateRemote method to          
// create instances of the default interface _CubeCommand exposed by              
// the CoClass CubeCommand. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeCommand = class
    class function Create: _CubeCommand;
    class function CreateRemote(const MachineName: string): _CubeCommand;
  end;

// *********************************************************************//
// The Class CoCubeDimension provides a Create and CreateRemote method to          
// create instances of the default interface _CubeDimension exposed by              
// the CoClass CubeDimension. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeDimension = class
    class function Create: _CubeDimension;
    class function CreateRemote(const MachineName: string): _CubeDimension;
  end;

// *********************************************************************//
// The Class CoCubeGroup provides a Create and CreateRemote method to          
// create instances of the default interface _CubeGroup exposed by              
// the CoClass CubeGroup. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeGroup = class
    class function Create: _CubeGroup;
    class function CreateRemote(const MachineName: string): _CubeGroup;
  end;

// *********************************************************************//
// The Class CoCubeLevel provides a Create and CreateRemote method to          
// create instances of the default interface _CubeLevel exposed by              
// the CoClass CubeLevel. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeLevel = class
    class function Create: _CubeLevel;
    class function CreateRemote(const MachineName: string): _CubeLevel;
  end;

// *********************************************************************//
// The Class CoCubeMeasure provides a Create and CreateRemote method to          
// create instances of the default interface _CubeMeasure exposed by              
// the CoClass CubeMeasure. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoCubeMeasure = class
    class function Create: _CubeMeasure;
    class function CreateRemote(const MachineName: string): _CubeMeasure;
  end;

// *********************************************************************//
// The Class CoDBCommand provides a Create and CreateRemote method to          
// create instances of the default interface _DBCommand exposed by              
// the CoClass DBCommand. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDBCommand = class
    class function Create: _DBCommand;
    class function CreateRemote(const MachineName: string): _DBCommand;
  end;

// *********************************************************************//
// The Class CoDbDimension provides a Create and CreateRemote method to          
// create instances of the default interface _DbDimension exposed by              
// the CoClass DbDimension. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDbDimension = class
    class function Create: _DbDimension;
    class function CreateRemote(const MachineName: string): _DbDimension;
  end;

// *********************************************************************//
// The Class CoDBGroup provides a Create and CreateRemote method to          
// create instances of the default interface _DBGroup exposed by              
// the CoClass DBGroup. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDBGroup = class
    class function Create: _DBGroup;
    class function CreateRemote(const MachineName: string): _DBGroup;
  end;

// *********************************************************************//
// The Class CoDbLevel provides a Create and CreateRemote method to          
// create instances of the default interface _DbLevel exposed by              
// the CoClass DbLevel. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDbLevel = class
    class function Create: _DbLevel;
    class function CreateRemote(const MachineName: string): _DbLevel;
  end;

// *********************************************************************//
// The Class CoMemberProperty provides a Create and CreateRemote method to          
// create instances of the default interface _MemberProperty exposed by              
// the CoClass MemberProperty. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMemberProperty = class
    class function Create: _MemberProperty;
    class function CreateRemote(const MachineName: string): _MemberProperty;
  end;

// *********************************************************************//
// The Class CoDataSource provides a Create and CreateRemote method to          
// create instances of the default interface _DataSource exposed by              
// the CoClass DataSource. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDataSource = class
    class function Create: _DataSource;
    class function CreateRemote(const MachineName: string): _DataSource;
  end;

// *********************************************************************//
// The Class CoRoleCommand provides a Create and CreateRemote method to          
// create instances of the default interface _RoleCommand exposed by              
// the CoClass RoleCommand. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoRoleCommand = class
    class function Create: _RoleCommand;
    class function CreateRemote(const MachineName: string): _RoleCommand;
  end;

// *********************************************************************//
// The Class CoColumn provides a Create and CreateRemote method to          
// create instances of the default interface _Column exposed by              
// the CoClass Column. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoColumn = class
    class function Create: _Column;
    class function CreateRemote(const MachineName: string): _Column;
  end;

// *********************************************************************//
// The Class CoMiningModel provides a Create and CreateRemote method to          
// create instances of the default interface _MiningModel exposed by              
// the CoClass MiningModel. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMiningModel = class
    class function Create: _MiningModel;
    class function CreateRemote(const MachineName: string): _MiningModel;
  end;

// *********************************************************************//
// The Class CoMiningModelGroup provides a Create and CreateRemote method to          
// create instances of the default interface _MiningModelGroup exposed by              
// the CoClass MiningModelGroup. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoMiningModelGroup = class
    class function Create: _MiningModelGroup;
    class function CreateRemote(const MachineName: string): _MiningModelGroup;
  end;

// *********************************************************************//
// The Class CoIROLAPProvider provides a Create and CreateRemote method to          
// create instances of the default interface _IROLAPProvider exposed by              
// the CoClass IROLAPProvider. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoIROLAPProvider = class
    class function Create: _IROLAPProvider;
    class function CreateRemote(const MachineName: string): _IROLAPProvider;
  end;

// *********************************************************************//
// The Class CoDatabase provides a Create and CreateRemote method to          
// create instances of the default interface _Database exposed by              
// the CoClass Database. The functions are intended to be used by             
// clients wishing to automate the CoClass objects exposed by the         
// server of this typelibrary.                                            
// *********************************************************************//
  CoDatabase = class
    class function Create: _Database;
    class function CreateRemote(const MachineName: string): _Database;
  end;

implementation

uses ComObj;

class function CoCommand.Create: _Command;
begin
  Result := CreateComObject(CLASS_Command) as _Command;
end;

class function CoCommand.CreateRemote(const MachineName: string): _Command;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Command) as _Command;
end;

class function CoPartitionLevel.Create: _PartitionLevel;
begin
  Result := CreateComObject(CLASS_PartitionLevel) as _PartitionLevel;
end;

class function CoPartitionLevel.CreateRemote(const MachineName: string): _PartitionLevel;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PartitionLevel) as _PartitionLevel;
end;

class function CoLevel.Create: _Level;
begin
  Result := CreateComObject(CLASS_Level) as _Level;
end;

class function CoLevel.CreateRemote(const MachineName: string): _Level;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Level) as _Level;
end;

class function CoMeasure.Create: _Measure;
begin
  Result := CreateComObject(CLASS_Measure) as _Measure;
end;

class function CoMeasure.CreateRemote(const MachineName: string): _Measure;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Measure) as _Measure;
end;

class function CoRole.Create: _Role;
begin
  Result := CreateComObject(CLASS_Role) as _Role;
end;

class function CoRole.CreateRemote(const MachineName: string): _Role;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Role) as _Role;
end;

class function CoPartitionMeasure.Create: _PartitionMeasure;
begin
  Result := CreateComObject(CLASS_PartitionMeasure) as _PartitionMeasure;
end;

class function CoPartitionMeasure.CreateRemote(const MachineName: string): _PartitionMeasure;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PartitionMeasure) as _PartitionMeasure;
end;

class function CoICommon.Create: _ICommon;
begin
  Result := CreateComObject(CLASS_ICommon) as _ICommon;
end;

class function CoICommon.CreateRemote(const MachineName: string): _ICommon;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_ICommon) as _ICommon;
end;

class function CoProperties.Create: _Properties;
begin
  Result := CreateComObject(CLASS_Properties) as _Properties;
end;

class function CoProperties.CreateRemote(const MachineName: string): _Properties;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Properties) as _Properties;
end;

class function CoProperty_.Create: _Property;
begin
  Result := CreateComObject(CLASS_Property_) as _Property;
end;

class function CoProperty_.CreateRemote(const MachineName: string): _Property;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Property_) as _Property;
end;

class function CoCubeAnalyzer.Create: _CubeAnalyzer;
begin
  Result := CreateComObject(CLASS_CubeAnalyzer) as _CubeAnalyzer;
end;

class function CoCubeAnalyzer.CreateRemote(const MachineName: string): _CubeAnalyzer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeAnalyzer) as _CubeAnalyzer;
end;

class function CoPartitionAnalyzer.Create: _PartitionAnalyzer;
begin
  Result := CreateComObject(CLASS_PartitionAnalyzer) as _PartitionAnalyzer;
end;

class function CoPartitionAnalyzer.CreateRemote(const MachineName: string): _PartitionAnalyzer;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PartitionAnalyzer) as _PartitionAnalyzer;
end;

class function CoPartitionDimension.Create: _PartitionDimension;
begin
  Result := CreateComObject(CLASS_PartitionDimension) as _PartitionDimension;
end;

class function CoPartitionDimension.CreateRemote(const MachineName: string): _PartitionDimension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_PartitionDimension) as _PartitionDimension;
end;

class function CoDimension.Create: _Dimension;
begin
  Result := CreateComObject(CLASS_Dimension) as _Dimension;
end;

class function CoDimension.CreateRemote(const MachineName: string): _Dimension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Dimension) as _Dimension;
end;

class function CoIDatabaseEvents.Create: _IDatabaseEvents;
begin
  Result := CreateComObject(CLASS_IDatabaseEvents) as _IDatabaseEvents;
end;

class function CoIDatabaseEvents.CreateRemote(const MachineName: string): _IDatabaseEvents;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_IDatabaseEvents) as _IDatabaseEvents;
end;

class function CoServer.Create: _Server;
begin
  Result := CreateComObject(CLASS_Server) as _Server;
end;

class function CoServer.CreateRemote(const MachineName: string): _Server;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Server) as _Server;
end;

class function CoMDStore.Create: _MDStore;
begin
  Result := CreateComObject(CLASS_MDStore) as _MDStore;
end;

class function CoMDStore.CreateRemote(const MachineName: string): _MDStore;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MDStore) as _MDStore;
end;

class function CoOlapCollection.Create: _OlapCollection;
begin
  Result := CreateComObject(CLASS_OlapCollection) as _OlapCollection;
end;

class function CoOlapCollection.CreateRemote(const MachineName: string): _OlapCollection;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_OlapCollection) as _OlapCollection;
end;

class function CoPartition.Create: _Partition;
begin
  Result := CreateComObject(CLASS_Partition) as _Partition;
end;

class function CoPartition.CreateRemote(const MachineName: string): _Partition;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Partition) as _Partition;
end;

class function CoAggregation.Create: _Aggregation;
begin
  Result := CreateComObject(CLASS_Aggregation) as _Aggregation;
end;

class function CoAggregation.CreateRemote(const MachineName: string): _Aggregation;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Aggregation) as _Aggregation;
end;

class function CoAggregationDimension.Create: _AggregationDimension;
begin
  Result := CreateComObject(CLASS_AggregationDimension) as _AggregationDimension;
end;

class function CoAggregationDimension.CreateRemote(const MachineName: string): _AggregationDimension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_AggregationDimension) as _AggregationDimension;
end;

class function CoAggregationLevel.Create: _AggregationLevel;
begin
  Result := CreateComObject(CLASS_AggregationLevel) as _AggregationLevel;
end;

class function CoAggregationLevel.CreateRemote(const MachineName: string): _AggregationLevel;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_AggregationLevel) as _AggregationLevel;
end;

class function CoAggregationMeasure.Create: _AggregationMeasure;
begin
  Result := CreateComObject(CLASS_AggregationMeasure) as _AggregationMeasure;
end;

class function CoAggregationMeasure.CreateRemote(const MachineName: string): _AggregationMeasure;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_AggregationMeasure) as _AggregationMeasure;
end;

class function CoCube.Create: _Cube;
begin
  Result := CreateComObject(CLASS_Cube) as _Cube;
end;

class function CoCube.CreateRemote(const MachineName: string): _Cube;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Cube) as _Cube;
end;

class function CoCubeCommand.Create: _CubeCommand;
begin
  Result := CreateComObject(CLASS_CubeCommand) as _CubeCommand;
end;

class function CoCubeCommand.CreateRemote(const MachineName: string): _CubeCommand;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeCommand) as _CubeCommand;
end;

class function CoCubeDimension.Create: _CubeDimension;
begin
  Result := CreateComObject(CLASS_CubeDimension) as _CubeDimension;
end;

class function CoCubeDimension.CreateRemote(const MachineName: string): _CubeDimension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeDimension) as _CubeDimension;
end;

class function CoCubeGroup.Create: _CubeGroup;
begin
  Result := CreateComObject(CLASS_CubeGroup) as _CubeGroup;
end;

class function CoCubeGroup.CreateRemote(const MachineName: string): _CubeGroup;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeGroup) as _CubeGroup;
end;

class function CoCubeLevel.Create: _CubeLevel;
begin
  Result := CreateComObject(CLASS_CubeLevel) as _CubeLevel;
end;

class function CoCubeLevel.CreateRemote(const MachineName: string): _CubeLevel;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeLevel) as _CubeLevel;
end;

class function CoCubeMeasure.Create: _CubeMeasure;
begin
  Result := CreateComObject(CLASS_CubeMeasure) as _CubeMeasure;
end;

class function CoCubeMeasure.CreateRemote(const MachineName: string): _CubeMeasure;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_CubeMeasure) as _CubeMeasure;
end;

class function CoDBCommand.Create: _DBCommand;
begin
  Result := CreateComObject(CLASS_DBCommand) as _DBCommand;
end;

class function CoDBCommand.CreateRemote(const MachineName: string): _DBCommand;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DBCommand) as _DBCommand;
end;

class function CoDbDimension.Create: _DbDimension;
begin
  Result := CreateComObject(CLASS_DbDimension) as _DbDimension;
end;

class function CoDbDimension.CreateRemote(const MachineName: string): _DbDimension;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DbDimension) as _DbDimension;
end;

class function CoDBGroup.Create: _DBGroup;
begin
  Result := CreateComObject(CLASS_DBGroup) as _DBGroup;
end;

class function CoDBGroup.CreateRemote(const MachineName: string): _DBGroup;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DBGroup) as _DBGroup;
end;

class function CoDbLevel.Create: _DbLevel;
begin
  Result := CreateComObject(CLASS_DbLevel) as _DbLevel;
end;

class function CoDbLevel.CreateRemote(const MachineName: string): _DbLevel;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DbLevel) as _DbLevel;
end;

class function CoMemberProperty.Create: _MemberProperty;
begin
  Result := CreateComObject(CLASS_MemberProperty) as _MemberProperty;
end;

class function CoMemberProperty.CreateRemote(const MachineName: string): _MemberProperty;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MemberProperty) as _MemberProperty;
end;

class function CoDataSource.Create: _DataSource;
begin
  Result := CreateComObject(CLASS_DataSource) as _DataSource;
end;

class function CoDataSource.CreateRemote(const MachineName: string): _DataSource;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_DataSource) as _DataSource;
end;

class function CoRoleCommand.Create: _RoleCommand;
begin
  Result := CreateComObject(CLASS_RoleCommand) as _RoleCommand;
end;

class function CoRoleCommand.CreateRemote(const MachineName: string): _RoleCommand;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_RoleCommand) as _RoleCommand;
end;

class function CoColumn.Create: _Column;
begin
  Result := CreateComObject(CLASS_Column) as _Column;
end;

class function CoColumn.CreateRemote(const MachineName: string): _Column;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Column) as _Column;
end;

class function CoMiningModel.Create: _MiningModel;
begin
  Result := CreateComObject(CLASS_MiningModel) as _MiningModel;
end;

class function CoMiningModel.CreateRemote(const MachineName: string): _MiningModel;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MiningModel) as _MiningModel;
end;

class function CoMiningModelGroup.Create: _MiningModelGroup;
begin
  Result := CreateComObject(CLASS_MiningModelGroup) as _MiningModelGroup;
end;

class function CoMiningModelGroup.CreateRemote(const MachineName: string): _MiningModelGroup;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_MiningModelGroup) as _MiningModelGroup;
end;

class function CoIROLAPProvider.Create: _IROLAPProvider;
begin
  Result := CreateComObject(CLASS_IROLAPProvider) as _IROLAPProvider;
end;

class function CoIROLAPProvider.CreateRemote(const MachineName: string): _IROLAPProvider;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_IROLAPProvider) as _IROLAPProvider;
end;

class function CoDatabase.Create: _Database;
begin
  Result := CreateComObject(CLASS_Database) as _Database;
end;

class function CoDatabase.CreateRemote(const MachineName: string): _Database;
begin
  Result := CreateRemoteComObject(MachineName, CLASS_Database) as _Database;
end;

end.
