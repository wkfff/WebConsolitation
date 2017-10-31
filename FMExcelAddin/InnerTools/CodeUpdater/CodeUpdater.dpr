program CodeUpdater;

uses
  Forms,
  uCodeUpdater in 'uCodeUpdater.pas' {fmCodeUpdater},
  uFMAddinGeneralUtils in '..\..\UserTools\Planing\ExcelAddIn\uFMAddinGeneralUtils.pas';

{$R *.RES}

begin
  Application.Initialize;
  Application.CreateForm(TfmCodeUpdater, fmCodeUpdater);
  Application.Run;
end.
