rem ---- Контролы ----
set mydir=%1

cd /d %mydir%\FMExcelAddin\controls
"C:\Program Files\Borland\Delphi5\Bin\dcc32.exe" -B ExcelAddinControls.dpk 

rem ---- форма индикации процессов ----
cd /d %mydir%\FMExcelAddin\UserTools\Planing\PlaningTools
"C:\Program Files\Borland\Delphi5\Bin\brcc32.exe" VersionInfo.rc
"C:\Program Files\Borland\Delphi5\Bin\dcc32.exe" -B PlaningTools.dpr 

rem ---- провайдер ----
cd /d %mydir%\FMExcelAddin\UserTools\Planing\DefattedPlaningProvider
"C:\Program Files\Borland\Delphi5\Bin\brcc32.exe" VersionInfo.rc
"C:\Program Files\Borland\Delphi5\Bin\dcc32.exe" -B PlaningProvider.dpr

rem ---- надстройка ----
cd /d %mydir%\FMExcelAddin\UserTools\Planing\ExcelAddIn
"C:\Program Files\Borland\Delphi5\Bin\brcc32.exe" VersionInfo.rc
"C:\Program Files\Borland\Delphi5\Bin\dcc32.exe" -B FMExcelAddin.dpr

