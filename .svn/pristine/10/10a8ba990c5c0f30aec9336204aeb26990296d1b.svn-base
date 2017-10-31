chcp 1251

echo off

echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Client\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=local -sw -out "%~dp0Installer\Client\\" "%~dp0Installer\Client\Product.wxs" "%~dp0Installer\Client\Files.wxs" "%~dp0Installer\Client\Shortcuts.wxs" "%~dp0Installer\Client\Feature.wxs" "%~dp0Installer\Client\WiXUI_Wizard.wxs" "%~dp0Installer\Client\WixUI_Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Client\ui_en.wxl" -loc "%~dp0\Installer\Client\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.Client(local).msi" "%~dp0\Installer\Client\Product.wixobj" "%~dp0\Installer\Client\Shortcuts.wixobj" "%~dp0\Installer\Client\Files.wixobj" "%~dp0\Installer\Client\Feature.wixobj" "%~dp0\Installer\Client\WiXUI_Wizard.wixobj" "%~dp0\Installer\Client\WixUI_Shortcuts.wixobj"

rem --------------------------------------------------

echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\MDXExpert\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=local -sw -out "%~dp0\Installer\MDXExpert\\" "%~dp0\Installer\MDXExpert\Product.wxs" "%~dp0\Installer\MDXExpert\Files.wxs" "%~dp0\Installer\MDXExpert\Shortcuts.wxs" "%~dp0\Installer\MDXExpert\Feature.wxs" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wxs" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\MDXExpert\ui_en.wxl" -loc "%~dp0\Installer\MDXExpert\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.MDXExpert(local).msi" "%~dp0\Installer\MDXExpert\Product.wixobj" "%~dp0\Installer\MDXExpert\Shortcuts.wixobj" "%~dp0\Installer\MDXExpert\Files.wixobj" "%~dp0\Installer\MDXExpert\Feature.wixobj" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wixobj" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wixobj"

rem --------------------------------------------------

echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Office Add-in\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=local -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -out "%~dp0\Installer\Office Add-in\\" "%~dp0\Installer\Office Add-in\Product.wxs" "%~dp0\Installer\Office Add-in\Files.wxs" "%~dp0\Installer\Office Add-in\Feature.wxs" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wxs"

"%~dp0\App\Wix\3.5\light" -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Office Add-in\ui_en.wxl" -loc "%~dp0\Installer\Office Add-in\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.OfficeAddin(local).msi" "%~dp0\Installer\Office Add-in\Product.wixobj" "%~dp0\Installer\Office Add-in\Files.wixobj" "%~dp0\Installer\Office Add-in\Feature.wixobj" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wixobj"

pause