chcp 1251

rem Версия 2.3.9

echo off

echo Копирование файла конфигурации в Office Add-in
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Office Add-in\Krista.FM.Update.ShedulerUpdateService.exe.config"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=global -dPlatform=x86 -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -out "%~dp0\Installer\Office Add-in\\" "%~dp0\Installer\Office Add-in\Product.wxs" "%~dp0\Installer\Office Add-in\Files.wxs" "%~dp0\Installer\Office Add-in\Feature.wxs" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wxs" "%~dp0\Installer\Office Add-in\Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Office Add-in\ui_en.wxl" -loc "%~dp0\Installer\Office Add-in\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.OfficeAddin.msi" "%~dp0\Installer\Office Add-in\Product.wixobj" "%~dp0\Installer\Office Add-in\Files.wixobj" "%~dp0\Installer\Office Add-in\Feature.wixobj" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wixobj" "%~dp0\Installer\Office Add-in\Shortcuts.wixobj"

rem --------------------------

echo Копирование файла конфигурации в Office Add-in
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Office Add-in\Krista.FM.Update.ShedulerUpdateService.exe.config"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -arch x64 -dbuildMode=global -dPlatform=x64 -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -out "%~dp0\Installer\Office Add-in\\" "%~dp0\Installer\Office Add-in\Product.wxs" "%~dp0\Installer\Office Add-in\Files.wxs" "%~dp0\Installer\Office Add-in\Feature.wxs" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wxs" "%~dp0\Installer\Office Add-in\Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Office Add-in\ui_en.wxl" -loc "%~dp0\Installer\Office Add-in\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.OfficeAddin(x64).msi" "%~dp0\Installer\Office Add-in\Product.wixobj" "%~dp0\Installer\Office Add-in\Files.wixobj" "%~dp0\Installer\Office Add-in\Feature.wixobj" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wixobj" "%~dp0\Installer\Office Add-in\Shortcuts.wixobj"

exit