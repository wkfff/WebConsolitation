chcp 1251

rem Версия 3.1

echo off

echo Копирование файла конфигурации в Client
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Client\Krista.FM.Update.ShedulerUpdateService.exe.config"
echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Client\Krista.FM.Updater.CustomActionExe.exe"


echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=global -sw -out "%~dp0Installer\Client\\" "%~dp0Installer\Client\Product.wxs" "%~dp0Installer\Client\Files.wxs" "%~dp0Installer\Client\Shortcuts.wxs" "%~dp0Installer\Client\Feature.wxs" "%~dp0Installer\Client\WiXUI_Wizard.wxs" "%~dp0Installer\Client\WixUI_Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -sice:38 -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Client\ui_en.wxl" -loc "%~dp0\Installer\Client\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.Client.msi" "%~dp0\Installer\Client\Product.wixobj" "%~dp0\Installer\Client\Shortcuts.wixobj" "%~dp0\Installer\Client\Files.wixobj" "%~dp0\Installer\Client\Feature.wixobj" "%~dp0\Installer\Client\WiXUI_Wizard.wixobj" "%~dp0\Installer\Client\WixUI_Shortcuts.wixobj"

exit