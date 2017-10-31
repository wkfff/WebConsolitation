unit brs_DSO_Utils;

interface

uses Windows, DSO80_TLB, ComObj, SysUtils, classes, ActiveX, oleServer, brs_GeneralFunctions;

type
    TOnReportBefore   = procedure(var obj: IDispatch; Action: Smallint; var Cancel: WordBool;
                          var Skip: WordBool)of object;
    TOnReportProgress = procedure(var obj: IDispatch; Action: Smallint; var Counter: Integer;
                          {var}const Message: WideString; var Cancel: WordBool) of object;
    TOnReportError    = procedure (var obj: IDispatch; Action: Smallint; ErrorCode: Integer;
                          const Message: WideString; var Cancel: WordBool) of object;
    TOnReportAfter    = procedure(var obj: IDispatch; Action: Smallint; success: WordBool) of object;

  TMDStoreEventsHandler = class(TObject, IUnknown, IDispatch)
  private
    fEventSource : IUnknown;
    fConnection  : integer;
    fOnReportBefore : TOnReportBefore;
    fOnReportProgress : TOnReportProgress;
    fOnReportError : TOnReportError;
    fOnReportAfter : TOnReportAfter;
  protected
    {IUnknown}
    function QueryInterface(const IID: TGUID; out Obj): HResult; stdcall;
    function _AddRef: Integer; stdcall;
    function _Release: Integer; stdcall;
    {IDispatch}
    function GetTypeInfoCount(out Count: Integer): HResult; stdcall;
    function GetTypeInfo(Index, LocaleID: Integer; out TypeInfo): HResult; stdcall;
    function GetIDsOfNames(const IID: TGUID; Names: Pointer;
      NameCount, LocaleID: Integer; DispIDs: Pointer): HResult; stdcall;
    function Invoke(DispID: Integer; const IID: TGUID; LocaleID: Integer;
      Flags: Word; var Params; VarResult, ExcepInfo, ArgErr: Pointer): HResult; stdcall;
    procedure InvokeEvent(DispID: TDispID; var Params: TVariantArray);
    //
    procedure fSetEventSource(const Val : IUnknown);
  public
    destructor Destroy; override;
    property EventSource      : IUnknown          read fEventSource      write fSetEventSource;
    property OnReportBefore   : TOnReportBefore   read fOnReportBefore   write fOnReportBefore;
    property OnReportProgress : TOnReportProgress read fOnReportProgress write fOnReportProgress;
    property OnReportError    : TOnReportError    read fOnReportError    write fOnReportError;
    property OnReportAfter    : TOnReportAfter    read fOnReportAfter    write fOnReportAfter;
  end;


function CheckDSO : boolean;
function ResolveDSOAction(const Action : integer) : string;
function ResolveDSOError(const ErrorCode : integer) : string;

implementation

// Проверка доступности DSO
function CheckDSO : boolean;
const DSOCLSID = 'DSO.Server';
begin
  result := CheckLybByProgID(DSOCLSID);
end;

{TMDStoreEventsHandler}
destructor TMDStoreEventsHandler.Destroy;
begin
  EventSource := nil;
  inherited
end;

function TMDStoreEventsHandler.QueryInterface(const IID: TGUID; out Obj): HResult;
begin
  Result := S_OK;
  // пытаемся вернуть указатель на запрошенный интерфейс
  if not GetInterface(IID, Obj)
    // всегда будем возвращать указатель на IDispatch
     then GetInterface(IDispatch, obj);
end;

function TMDStoreEventsHandler._AddRef: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;

function TMDStoreEventsHandler._Release: Integer;
begin
  // функции управления временем жизни объекта берем на себя
  result := 1
end;

function TMDStoreEventsHandler.GetTypeInfoCount(out Count: Integer): HResult;
begin
  Count := 0;
  Result:= S_OK;
end;

function TMDStoreEventsHandler.GetTypeInfo(Index, LocaleID: Integer; out TypeInfo): HResult;
begin
  Pointer(TypeInfo) := nil;
  Result := E_NOTIMPL;
end;

function TMDStoreEventsHandler.GetIDsOfNames(const IID: TGUID; Names: Pointer;
  NameCount, LocaleID: Integer; DispIDs: Pointer): HResult;
begin
  Result := E_NOTIMPL;
end;

procedure TMDStoreEventsHandler.InvokeEvent(DispID: TDispID; var Params: TVariantArray);
var LowBound : integer;
begin
  LowBound := Low(Params);
  case DispID of
    // ReportBefore
    1 : if Assigned(fOnReportBefore) then fOnReportBefore(
      IDispatch(TVarData(Params[LowBound]).VDispatch), // var obj: IDispatch;
      TVarData(Params[LowBound + 1]).VSmallint,        // Action: Smallint;
      TVarData(Params[LowBound + 2]).VBoolean,         // var Cancel: WordBool;
      TVarData(Params[LowBound + 3]).VBoolean);        // var Skip: WordBool
    // ReportProgress
    2 : if Assigned(fOnReportProgress) then fOnReportProgress(
      IDispatch(TVarData(Params[LowBound]).VDispatch), // var obj: IDispatch;
      TVarData(Params[LowBound + 1]).VSmallInt,        // Action: Smallint;
      TVarData(Params[LowBound + 2]).VInteger,         // var Counter: Integer;
      widestring(TVarData(Params[LowBound + 3]).VOleStr), // var Message: WideString;
      TVarData(Params[LowBound + 4]).VBoolean);        // var Cancel: WordBool
    //  ReportError
    3 : if Assigned(fOnReportError) then fOnReportError(
      IDispatch(TVarData(Params[LowBound]).VDispatch), // var obj: IDispatch;
      TVarData(Params[LowBound + 1]).VSmallint,        // Action: Smallint;
      TVarData(Params[LowBound + 2]).VInteger,         // ErrorCode: Integer;
      widestring(TVarData(Params[LowBound + 3]).VOleStr), // const Message: WideString;
      TVarData(Params[LowBound + 4]).VBoolean);        // var Cancel: WordBool
    // ReportAfter
    4 : if Assigned(fOnReportAfter) then fOnReportAfter(
      IDispatch(TVarData(Params[LowBound]).VDispatch), // var obj: IDispatch;
      TVarData(Params[LowBound + 1]).VSmallint,        // Action: Smallint;
      TVarData(Params[LowBound + 2]).VBoolean);       // success: WordBool
   end;
end;

// приводим параметры к более удобному виду... позаимствовано из OleServer.pas
function TMDStoreEventsHandler.Invoke(DispID: Integer; const IID: TGUID;
  LocaleID: Integer; Flags: Word; var Params;
  VarResult, ExcepInfo, ArgErr: Pointer): HResult;
var
  ParamCount, I: integer;
  VarArray : TVariantArray;
begin
  // Get parameter count
  ParamCount := TDispParams(Params).cArgs;
  // Set our array to appropriate length
  SetLength(VarArray, ParamCount);
  // Copy over data
  for I := Low(VarArray) to High(VarArray) do
    VarArray[High(VarArray)-I] := OleVariant(TDispParams(Params).rgvarg^[I]);
  // передаём на обрбаботку
  InvokeEvent(DispID, VarArray);
  // Clean array
  SetLength(VarArray, 0);
  // Pascal Events return 'void' - so assume success!
  Result := S_OK;
end;

procedure TMDStoreEventsHandler.fSetEventSource(const Val : IUnknown);
begin
  // если какие-то события обрабатывались - отцеплямся
  if Assigned(EventSource) then InterfaceDisconnect(EventSource, DIID___Database, fConnection);
  if Val <> fEventSource then fEventSource := Val;
  // прицепляемся к новому объекту
  if Assigned(EventSource) then InterfaceConnect(EventSource, DIID___Database, Self as IUnknown, fConnection);
end;

function ResolveDSOError(const ErrorCode : integer) : string;
begin
  {$WARNINGS OFF}
  case ErrorCode of
    mderrAcceptError : result := 'An internal error has occurred on the specified Analysis server.';
    mderrAcquireCreditsError : result := 'An internal error has occurred on the Analysis server.';
    mderrAggregationUsageNotCustom : result := 'The EnableAggregations property cannot be set for levels in dimensions whose AggregationUsage property is not dimAggUsageCustom.';
    mderrBadParameterForServiceState : result := 'Invalid service state parameter on the computer.';
    mderrBadRequest : result := 'An internal request related error has occurred on the Analysis server.';
    mderrBindError : result := 'An internal bind related error has occurred on the Analysis server.';
    mderrCalculateError : result := 'An internal calculation related error has occurred on the Analysis server.';
    mderrCanceled : result := 'The specified transaction was canceled.';
    mderrCannotAddVirtualDimension : result := 'Cannot add a virtual dimension because its source dimension is not in the database.';
    mderrCannotChangeRemoteServer : result := 'Cannot change the RemoteServer property after it has been set.';
    mderrCannotCloneObjectIntoItself : result := 'Cannot clone an object into itself.';
    mderrCannotCommitDatabase : result := 'Unable to create a database on the Analysis server.';
    mderrCannotCreatePartition : result := 'No system partition is available for this operation. System partitions have been programmatically defined as user partitions. User-defined partitions are available only if you install Analysis Services for Microsoft® SQL Server™ 2000 Enterprise Edition.';
    mderrCannotCreateVirtualDimensionFromAnother : result := 'Cannot create a virtual dimension based on another virtual dimension.';
    mderrCannotDeleteDataSource : result := 'At least one object has a reference to the data source, so the data source cannot be deleted.';
    mderrCannotDeleteDimension : result := 'A dimension cannot be deleted because it is used in a cube.';
    mderrCannotDeleteLastPartition : result := 'Cannot delete the last partition in a cube. (A cube must have at least one partition.)';
    mderrCannotDeleteLevel : result := 'Cannot delete a level if it is used in a virtual dimension.';
    mderrCannotDeleteMemberProperty : result := 'Cannot delete a member property if it is used in a virtual dimension.';
    mderrCannotEnableRealTimeUpdatesWithoutIndexedViews : result := 'Cannot enable real time updates on the specified partition without indexed views.';
    mderrCannotExecFuncError : result := 'Cannot execute a function in a user-defined function library.';
    mderrCannotModifySharedObject : result := 'Cannot change a property of a shared dimension (or subordinate level) used in a cube.';
    mderrCannotRemoveMeasureFromDefaultAggregation : result := 'Cannot remove a measure from an aggregation created by the partition analyzer.';
    mderrCannotRenameObject : result := 'Only temporary objects can be renamed.';
    mderrCannotSaveInsideTransaction : result := 'Cannot save objects inside a DSO transaction.';
    mderrCellCalculationsNotAvailable : result := 'Calculated cells are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrChildProcessFailed : result := 'A child process failed within a transaction.';
    mderrClassError : result := 'An internal class error has occurred on the Analysis server.';
    mderrCollectionItemNotFound : result := 'Raised if you try to remove an item from a collection that does not exist in the collection.';
    mderrCollectionReadOnly : result := 'Cannot add an object to, or remove an object from, a collection that is read-only.';
    mderrCOMError : result := 'An internal COM error has occurred on the Analysis server.';
    mderrCompatibilityError : result := 'An internal compatibility related error has occurred on the Analysis server.';
    mderrConnectError : result := 'An error occurred while connecting to an Analysis server.';
    mderrCorruptedProperty : result := 'A corrupted property was found while merging partitions.';
    mderrCorruptedRegistrySettings : result := 'One or more registry settings in use by Analysis Services has been corrupted.';
    mderrCouldInitiateCubeUpdate : result := 'Could not initiate a cube update.';
    mderrCouldInitiateDimensionUpdate : result := 'Could not initiate a dimension update.';
    mderrCouldNotLockObject : result := 'Raised if you try to lock an object that is already locked (by a different application).';
    mderrCouldNotLogMissingMemberKeyErrors  : result := 'Could not write errors regarding missing member key errors to the log file.';
    mderrCouldNotOpenService : result := 'The Analysis server runs as a Microsoft Windows NT® 4.0 or Windows® 2000 service. This error is raised if the service could not be opened. For more information about the mderrCouldNotOpenService error, see the Microsoft Win32® API documentation.';
    mderrCouldNotOpenServiceControlManager : result := 'The Analysis server runs as a Windows NT 4.0 or Windows 2000 service. This error is raised if the service control manager could not be opened.';
    mderrCouldNotQueryTheService : result := 'The Analysis server runs as a Windows NT 4.0 or Windows 2000 service. This error is raised if the service could not be queried. For more information about the CouldNotQueryTheService error, see the Microsoft Win32 API documentation.';
    mderrCouldNotUnLockObject : result := 'The specified object could not be unlocked.';
    mderrCubeDimHasNoDatabaseDim : result := 'The specified dimension to be associated with a cube does not have a corresponding database dimension.';
    mderrCubeNotProcessed : result := 'The specified cube has not yet been processed.';
    mderrCustomRollupsNotAvailable : result := 'Custom rollups are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrDataError : result := 'An internal data related error has occurred on the Analysis server.';
    mderrDefinitionCannotBeEmpty : result := 'An empty definition was found while merging partitions.';
    mderrDefinitionDoesNotContainNameAndValue : result := 'A definition which does not contain a name and value was found while merging partitions.';
    mderrDeletingTablesOutsideOfTransaction : result := 'Tables cannot be deleted outside of a transaction.';
    mderrDifferentAggregationDatasources : result := 'Partitions cannot be merged because source and target partitions have different relational data sources.';
    mderrDifferentAggregationNumber : result := 'Partitions cannot be merged because source and target partitions have different numbers of aggregations.';
    mderrDifferentAggregationOLAPMode : result := 'Partitions cannot be merged because source and target partitions have different storage modes.';
    mderrDifferentAggregationStructure : result := 'Partitions cannot be merged because source and target partitions have different structures or storage modes.';
    mderrDifferentRemoteServers : result := 'Cannot merge two partitions that are on different servers.';
    mderrDimensionChangingCannotAddLevel : result := 'The specified changing dimension is being used in a cube, and either does not support adding a new lowest level, or it has an AggregationUsage property' +
      ' value other than dimAggUsageDetailsOnly and dimAggUsageStandard, and does not allow changing the top level.';
    mderrDimensionLockedByCube : result := 'Dimension is locked because it is currently being used in a cube. Remove the dimension from the cube to unlock the dimension.';
    mderrDimensionMemberNotFound : result := 'A member was found in the fact table, but not in the dimension.';
    mderrDimensionNotInUnderlyingCubes : result := 'Cannot add to a virtual cube a dimension that is not in any of the cubes on which the virtual cube is based.';
    mderrDimensionWritebackNotAvailable : result := 'Dimension writebacks are available only if you install Analysis Services for Microsoft SQL Server2000 Enterprise Edition.';
    mderrDuplicateKeyInCollection : result := 'Cannot add to a collection an item with the same name as an item already in the collection.';
    mderrExecuteSQL : result := 'An error occurred while attempting to execute a SQL statement against a data source.';
    mderrFileError : result := 'An internal file system error has occurred on the Analysis server.';
    mderrFormulaError : result := 'An internal formula related error has occurred.';
    mderrFuncNotSupportedError : result := 'An unsupported function was called by a Multidimensional Expressions (MDX) statement.';
    mderrIllegalMeasureType : result := 'Invalid measure data type found in returned SQL rowset.';
    mderrIllegalObjectName : result := 'Cannot assign an invalid name to an object.';
    mderrImpersonateError : result := 'An internal error has occurred on the Analysis server.';
    mderrInconsistentAggregations : result := 'An inconsistency has been found in the aggregations of a specified partition or partitions.';
    mderrInitializationFailed : result := 'Processing could not be initialized on the specified DSO object.';
    mderrInternal : result := 'An internal error occurred within the DSO library.';
    mderrInternetError : result := 'An error occurred with a linked cube that is available through an HTTP connection.';
    mderrinvalidAggregateFunction : result := 'An invalid aggregate function was specified.';
    mderrInvalidAggregationLevel : result := 'An invalid aggregation level was specified.';
    mderrInvalidAggUsage : result := 'The AggregationUsage property is incompatible with current settings for the dimension.';
    mderrInvalidCubeBadFactTableAlias : result := 'The SourceTableAlias property is set incorrectly.';
    mderrInvalidCubeDrillThroughNotProperlyDefined : result := 'The drillthrough options for the cube are not correctly defined.';
    mderrInvalidCubeInconsistentAggregations : result := 'Cannot create a cube with a distinct count measure and add aggregations that are not compatible with the distinct count function.';
    mderrInvalidCubeMultipleDistinctCountMeasures : result := 'Cannot create a cube with more than one measure with an AggregateFunction value of aggDistinctCount.';
    mderrInvalidCubeNoVisibleDimensions : result := 'Cannot create a cube without at least one visible dimension or visible calculated member.';
    mderrInvalidCubeNoVisibleMeasures : result := 'Cannot create a cube without at least one visible measure.';
    mderrInvalidDataType : result := 'An invalid data type was specified.';
    mderrInvalidDimensionBadAreMemberKeysUnique : result := 'The AreMemberKeysUnique property is set to True on a dimension with at least one level with AreMemberKeysUnique set to False.';
    mderrInvalidDimensionBadAreMemberNamesUnique : result := 'The AreMemberNamesUnique property is set to True on a dimension with at least one level with AreMemberNamesUnique set to False.';
    mderrInvalidDimensionBadDependsOnDimension : result := 'The DependsOnDimension property refers to a nonexistent dimension.';
    mderrInvalidDimensionLevelsAfterHiddenMustBeUnique : result := 'Must have nonunique keys in levels that are below a hidden level.';
//    mderrInvalidDimensionNoMemberValues : result := 'Cannot create a dimension that is unrelated to the fact table and has levels without custom rollup expressions or custom rollup columns.';
    mderrInvalidDimensionNoVisibleLevels : result := 'Cannot create a dimension without at least one visible level.';
    mderrInvalidDimensionParentChildInvalidLevel : result := 'Cannot create a parent-child dimension that contains a non-parent-child level that is not an (All) level.';
//    mderrInvalidDimensionParentChildLevelMissing : result := 'Cannot create a parent-child dimension without a parent-child level.';
    mderrInvalidLevelBadCustomRollupColumn : result := 'The level has an invalid value for its CustomRollupColumn property.';
    mderrInvalidLevelBadOrderingMemberProperty : result := 'The OrderingMemberProperty for the level does not refer to a member property of the level.';
    mderrInvalidLevelBadParentKey : result := 'A parent-child level has an invalid value for its ParentKeyColumn property.';
    mderrInvalidLevelBadSkippedLevelsColumn : result := 'A parent-child level has an invalid value for its SkippedLevelsColumn property.';
    mderrInvalidLevelConflictingMemberProperties : result := 'A member property has a Caption that is in use by another member property with an identical language setting.';
    mderrInvalidLevelGrouping : result := 'The value of the Grouping property is invalid for the current dimension.';
    mderrInvalidLevelNamingTemplate : result := 'The LevelNamingTemplate property can lead to conflicting level names and may cause problems during processing.';
    mderrInvalidLockType : result := 'The LockType argument value specified in the LockObject method of a DSO object is invalid. For more information about valid lock types, see OlapLockTypes.';
    mderrInvalidMeasure : result := 'An invalid measure was specified.';
    mderrInvalidParent : result := 'An object that is not a member of a collection has no parent.';
    mderrInvalidPartBadFactTableAlias : result := 'The SourceTableAlias property is set incorrectly.';
    mderrInvalidPermission : result := 'An invalid member security attribute was specified in the SetPermissions method of a DSO Role object.';
    mderrInvalidProcessType : result := 'An invalid process type was specified in the Process method of a DSO object. For more information about valid process types, see ProcessTypes.';
    mderrInvalidPropertySetting : result := 'Cannot add an object to, or remove an object from, a collection that is read-only.';
    mderrInvalidRelatedColumn : result := 'An invalid column name was specified in the RelatedColumn property of a DSO clsColumn object.';
//    mderrInvalidRemotePartition : result := 'The RemoteServer property is empty or contains the name of a nonexistent partition.';
    mderrInvalidRemoteServerName : result := 'The RemoteServer property is empty or contains the name of a nonexistent server.';
    mderrInvalidSourceOlapObject : result := 'An invalid object was specified in the SourceOlapObject property of a DSO clsColumn object.';
    mderrInvalidStructure : result := 'The structure of the object that raised the error is invalid.';
    mderrInvalidTransactionOperation : result := 'Unable to begin, commit, or rollback a transaction on a DSO clsDatabase object. In the case of the BeginTrans method, another transaction is in process. In the case of the CommitTrans or Rollback methods, no transaction is currently in process.';
    mderrInvalidVirtualDimensionMustHaveAllLevel : result := 'Cannot create a virtual dimension that does not contain an (All) level.';
    mderrLastLevelMustBeUnique : result := 'The settings for the dimension require the AreMemberKeysUnique property of the last level in the dimension to be True.';
    mderrLinkedCubeCannotChangeProperty : result := 'Cannot change the values of the properties ColumnType and AggregationFunction for a measure in a linked cube.';
    mderrLinkedCubeInvalidConnectionString : result := 'The ConnectionString property for the linked cube object contains incorrect or incomplete information. It must refer to a server in Microsoft SQL Server 2000 Analysis Services.';
    mderrLinkedCubeInvalidServer : result := 'The publishing and subscribing servers need to be different when creating a linked cube.';
    mderrLinkedCubeInvalidSourceCube : result := 'The name of the published cube is invalid, or the user does not have adequate permissions to query the cube.';
    mderrLinkedCubeNoAggregationsAllowed : result := 'Aggregations are not allowed for linked cubes.';
    mderrLinkedCubeNotEnoughDimensions : result := 'While creating a linked cube, no dimensions were found in the specified source cube.';
    mderrLinkedCubesNotAvailable : result := 'Linked cubes are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrLinkedCubeSynchronizationFailed : result := 'Linked cube structure synchronization between subscribing server and publishing server failed.';
    mderrListenError : result := 'An internal error related to real-time updates has occurred on the Analysis server.';
    mderrLoadDLLError : result := 'An error occurred while loading a user-defined function library.';
    mderrLockAccessError : result := 'Unable to lock an object already locked.';
    mderrLockCannotBeObtained : result := 'Unable to obtain a lock from the server.';
    mderrLockDescriptionTooLong : result := 'Lock description is longer than permitted.';
    mderrLockFileCorrupted : result := 'The server reported that the lock file is corrupted.';
    mderrLockFileMissing : result := 'The server reported that the lock file is missing.';
    mderrLockNetworkDown : result := 'Network error.';
    mderrLockNetworkNameNotFound : result := 'Cannot find name on the network.';
    mderrLockNetworkPathNotFound : result := 'Cannot find this network path.';
    mderrLockNotEnoughMemory : result := 'There is not enough memory available to create a lock on a DSO object using the LockObject method.';
    mderrLockObjectNotLocked : result := 'Cannot unlock an object that is not locked.';
    mderrLockSystemError : result := 'A lock cannot be obtained because of an unknown error.';
    mderrMeasureDoesNotHaveValidSourceColumn : result := 'Cannot add a measure to a virtual cube if the name of the measures source column is not in the correct format.';
    mderrMemberPropertyNotFound : result := 'The member property was not found.';
    mderrMemoryError : result := 'An internal memory related error has occurred on the Analysis server.';
    mderrMergedPartitionsMustBothUseIndexedViewsOrTables : result := 'Partitions to be merged must both use either indexed views or aggregation tables.';
    mderrMiningModelNotProcessed : result := 'The mining model cannot be updated because it has not yet been processed.';
    mderrNameCannotBeChanged : result := 'Cannot change a DSO object name unless the object is a temporary object.';
    mderrNameCannotBeEmpty : result := 'An object cannot have an empty name.';
    mderrNetworkError : result := 'An internal network related error has occurred on the Analysis server.';
    mderrNoConnectionToServer : result := 'A connection cannot be opened on the specified Analysis server.';
    mderrNoEntryPointError : result := 'An entry point could not be found while loading a user-defined function library.';
    mderrObjectCantBeProcessedWithItsDimensions : result := 'A dimension used by the specified DSO object has already been processed in the same transaction.';
    mderrObjectChangedByAnotherApp : result := 'Cannot save object because it was not locked and was changed by another object.';
//    mderrObjectIsNotWriteLocked : result := 'Cannot update an object that is not write-locked.';
    mderrObsoleteError : result := 'The reference to a DSO object has become obsolete.';
    mderrODBC : result := 'An internal error has occurred in an ODBC data source provider.';
    mderrODBCError : result := 'An internal ODBC related error has occurred on the Analysis server.';
    mderrOSError : result := 'An internal operating system related error has occurred on the Analysis server.';
    mderrPartitionMustBeProcessed : result := 'The partition associated with the specified DSO object must first be processed.';
    mderrProcessError : result := 'An internal processing error has occurred within the DSO library.';
    mderrPropertyCannotBeChanged : result := 'Property cannot be changed in this context.';
    mderrPropertyCollectionCannotBeChanged : result := 'An internal error occurred while merging partitions.';
    mderrRealTimeUpdatesNotAvailable : result := 'Real-time updates are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrRegistryConnectFailed : result := 'An error occurred while connecting to the registry.';
    mderrRegistryOpenKeyFailed : result := 'An error occurred while opening a registry key.';
    mderrRegistryQueryValueFailed : result := 'An error occurred while retrieving a value from a registry key.';
    mderrRemotePartitionCannotHaveWriteableDimension : result := 'A remote partition cannot contain a write-enabled dimension.';
    mderrRepositoryConnectionFailed : result := 'Object repository may be read-only.';
    mderrRepositoryConnectionStringChanged : result := 'Another application has changed the repository connection string for the specified Analysis Server. You need to close and reopen this server connection in order to continue.';
    mderrRepositoryIncompatible : result := 'Repository is incompatible with this version of DSO. Verify that your DSO version is compatible with your repository version.';
    mderrRepositoryUpgradeFailed : result := 'An error occurred while attempting to update the repository for the specified Analysis server.';
    mderrRevertError : result := 'An internal error has occurred on the Analysis server.';
    mderrROLAPDimensionsNotAvailable : result := 'Relational OLAP (ROLAP) dimensions are available only if you install Analysis Services for Microsoft® SQL Server™ 2000 Enterprise Edition.';
    mderrROLAPDimensionsRequireROLAPPartition : result := 'Cannot add a relational OLAP (ROLAP) dimension to a non-ROLAP partition.';
    mderrSecurityError : result := 'An internal security error has occurred on the Analysis server.';
    mderrSelectError : result := 'An internal SQL error has occurred on the Analysis server.';
    mderrServerInternal : result := 'An internal error has occurred on the specified Analysis server.';
    mderrServerObjectNotFound : result := 'The specified Analysis server could not be found.';
    mderrServerObjectNotOpened : result := 'The specified Analysis server was not opened before attempting an action with an object associated with the Analysis server.';
    mderrSkippedLevelsNotAvailable : result := 'Skipped levels and ragged hierarchies are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrSourceDoesNotExist : result := 'Cannot merge partitions because the source partition does not exist.';
    mderrStructureHasChanged : result := 'The structure of the specified object has changed.';
    mderrTargetDoesNotExist : result := 'Cannot merge partitions because the target partition does not exist.';
    mderrTimeOut : result := 'Connection to the Analysis server timed out.';
    mderrTimeoutError : result := 'A timeout error has occurred on the Analysis server.';
    mderrTooManyDimensionMembers : result := 'More than the allowed maximum of 64,000 dimension member children for a single parent member.';
    mderrTooManyLevelsInDimension : result := 'The maximum number of ungrouped levels in a dimension is 64, that is, 63 plus an (All) level.';
    mderrTooManyMissingMemberKeys : result := 'The maximum number of dimension key processing errors has been exceeded.';
    mderrUnexpectedError : result := 'An unexpected internal error has occurred.';
    mderrUnsuccesfullServiceOperation : result := 'The Analysis server service (MSSQLServerOLAPService) is not running on the specified computer.';
    mderrUserDefinedPartitionsNotAvailable : result := 'User-defined partitions are available only if you install Analysis Services for Microsoft SQL Server 2000 Enterprise Edition.';
    mderrValidateLastLevelMustBeUnique : result := 'The AreMemberKeysUnique property is set to False on the last level of a regular dimension with IsChanging set to True.';
    else result := format('Unknown DSO error : $%x', [ErrorCode]);
  end
  {$WARNINGS ON}
end;

const
  // actions
  mdactProcess                               = 1;
  mdactMerge                                 = 2;
  mdactDelete                                = 3;
  mdactDeleteOldAggregations                 = 4;
  mdactRebuild                               = 5;
  mdactCommit                                = 6;
  mdactRollback                              = 7;
  mdactCreateIndexes                         = 8;
  mdactCreateTable                           = 9;
  mdactInsertInto                            = 10;
  mdactTransaction                           = 11;
  mdactInitialize                            = 12;
  mdactCreateView                            = 13;

  mdactWriteData                             = 101;
  mdactReadData                              = 102;
  mdactAggregate                             = 103;
  mdactExecuteSQL                            = 104;
  mdactNowExecutingSQL                       = 105;
  mdactExecuteModifiedSQL                    = 106;
  mdactConnecting                            = 107;
  mdactRowsAffected                          = 108;
  mdactError                                 = 109;
  mdactWriteAggsAndIndexes                   = 110;
  mdactWriteSegment                          = 111;
  mdactDataMiningProgress                    = 112;

  // Warnings
  mdwrnSkipped                               = 901;
  mdwrnCubeNeedsToProcess                    = 902;
  mdwrnCouldNotCreateIndex                   = 903;
  mdwrnTimeoutNotSetCorrectly                = 904;
  mdwrnExecuteSQL                            = 905;
  mdwrnDeletingTablesOutsideOfTransaction    = 906;
  mdwrnCouldNotProcessWithIndexedViews       = 907;

function ResolveDSOAction(const Action : integer) : string;
begin
  case Action of
    mdactProcess : result := 'Process';
    mdactMerge : result := 'Merge';
    mdactDelete : result := 'Delete';
    mdactDeleteOldAggregations : result := 'Delete Old Aggregations';
    mdactRebuild : result := 'Rebuild';
    mdactCommit : result := 'Commit';
    mdactRollback : result := 'Rollback';
    mdactCreateIndexes : result := 'Create Indexes';
    mdactCreateTable : result := 'Create Table';
    mdactInsertInto : result := 'Insert Into';
    mdactTransaction : result := 'Transaction';
    mdactInitialize : result := 'Initialize';
    mdactCreateView : result := 'Create View';
    mdactWriteData : result := 'Write Data';
    mdactReadData : result := 'Read Data';
    mdactAggregate : result := 'Aggregate';
    mdactExecuteSQL : result := 'Execute SQL';
    mdactNowExecutingSQL : result := 'Now Executing SQL';
    mdactExecuteModifiedSQL : result := 'Executing Modified SQL';
    mdactRowsAffected : result := 'Rows Affected';
    mdactError : result := 'Error';
    mdactWriteAggsAndIndexes : result := 'Write Aggregations and Indexes';
    mdactWriteSegment : result := 'Write Segment';
    mdactDataMiningProgress : result := 'Data Mining Model Processed Percentage';
    // warnings
    mdwrnSkipped : result := 'Warning: action skipped';
    mdwrnCubeNeedsToProcess : result := 'Warning: cube needs to process';
    mdwrnCouldNotCreateIndex : result := 'Warning: could not create index';
    mdwrnTimeoutNotSetCorrectly : result := 'Warning: timeout not set correctly';
    mdwrnExecuteSQL  : result := 'Warning: error while executing SQL';
    mdwrnDeletingTablesOutsideOfTransaction : result := 'Warning: deleting tables outside of transaction';
    mdwrnCouldNotProcessWithIndexedViews : result := 'Warning: could not process with indexed views';
    else result := format('Unknown DSO Action : $%x', [Action])
  end
end;

end.
