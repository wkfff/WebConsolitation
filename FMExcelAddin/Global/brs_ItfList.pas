unit brs_ItfList;

interface

uses windows, classes, sysutils;

type
  TInterfacesList = class(TObject)
  private
    fInterfaces : TList;
  protected
    function fGetItemsCount : integer;
    function fGetItem(Ind : integer) : IUnknown;
  public
    constructor Create; 
    destructor Destroy; override;
    procedure Add(Itf : IUnknown);
    procedure SetCapacity(Capacity : integer);
    procedure Clear;
    procedure Delete(Itf : IUnknown);
    property ItemsCount : integer read fGetItemsCount;
    property Items[Ind : integer] : IUnknown read fGetItem;
  end;

implementation

{TInterfacesList}
constructor TInterfacesList.Create;
begin
  fInterfaces := TList.Create;
end;

destructor TInterfacesList.Destroy;
begin
  Clear;
  FreeAndNil(fInterfaces);
  inherited
end;

procedure TInterfacesList.Add(Itf : IUnknown);
begin
  fInterfaces.Add(pointer(Itf));
  Itf._AddRef
end;

procedure TInterfacesList.SetCapacity(Capacity : integer);
begin
  fInterfaces.Capacity := Capacity
end;

procedure TInterfacesList.Delete(Itf : IUnknown);
var Ind : integer;
begin
  Ind := fInterfaces.IndexOf(pointer(Itf));
  if Ind <> -1 then begin
    IUnknown(fInterfaces[Ind])._Release;
    fInterfaces.Delete(Ind)
  end
end;

procedure TInterfacesList.Clear;
var i : integer;
begin
  for i := 0 to fInterfaces.Count - 1 do
    IUnknown(fInterfaces[i])._Release;
  fInterfaces.Clear;
end;

function TInterfacesList.fGetItemsCount : integer;
begin
  result := fInterfaces.Count
end;

function TInterfacesList.fGetItem(Ind : integer) : IUnknown;
begin
  result := IUnknown(fInterfaces[Ind])
end;

end.
