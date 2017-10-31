library PlaningTools;

uses
  ComServ,
  PlaningTools_TLB in 'PlaningTools_TLB.pas',
  brs_Process in 'brs_Process.pas' {ProcessForm: CoClass},
  brs_ProcessForm in 'brs_ProcessForm.pas' {frmProcess},
  brs_Operation in 'brs_Operation.pas' {Operation: CoClass},
  brs_OperationForm in 'brs_OperationForm.pas' {frmOperation};

exports
  DllGetClassObject,
  DllCanUnloadNow,
  DllRegisterServer,
  DllUnregisterServer;
{$R *.TLB}

{$R *.RES}

{$R PlaningToolsEx.RES}

{$R VersionInfo.RES}
begin
end.
