chcp 1251

echo off

echo Копирование файла конфигурации в Client
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Client\Krista.FM.Update.ShedulerUpdateService.exe.config"
echo Копирование файла адаптера схемы 
copy "%~dp0\Krista.FM.Update.SchemeAdapter.dll" "%~dp0\App\Client\Krista.FM.Update.SchemeAdapter.dll"
echo Копирование библиотеки Krista.FM.Update.Framework.dll
copy "%~dp0\Krista.FM.Update.Framework.dll" "%~dp0\App\Client\Krista.FM.Update.Framework.dll"
echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Client\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=global -sw -out "%~dp0Installer\Client\\" "%~dp0Installer\Client\Product.wxs" "%~dp0Installer\Client\Files.wxs" "%~dp0Installer\Client\Shortcuts.wxs" "%~dp0Installer\Client\Feature.wxs" "%~dp0Installer\Client\WiXUI_Wizard.wxs" "%~dp0Installer\Client\WixUI_Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -sice:38 -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Client\ui_en.wxl" -loc "%~dp0\Installer\Client\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.Client.msi" "%~dp0\Installer\Client\Product.wixobj" "%~dp0\Installer\Client\Shortcuts.wixobj" "%~dp0\Installer\Client\Files.wixobj" "%~dp0\Installer\Client\Feature.wixobj" "%~dp0\Installer\Client\WiXUI_Wizard.wixobj" "%~dp0\Installer\Client\WixUI_Shortcuts.wixobj"

rem --------------------------------------------------

echo Копирование файла конфигурации в MDXExpert
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\MDXExpert\Krista.FM.Update.ShedulerUpdateService.exe.config"
echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\MDXExpert\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=global -sw -out "%~dp0\Installer\MDXExpert\\" "%~dp0\Installer\MDXExpert\Product.wxs" "%~dp0\Installer\MDXExpert\Files.wxs" "%~dp0\Installer\MDXExpert\Shortcuts.wxs" "%~dp0\Installer\MDXExpert\Feature.wxs" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wxs" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -sice:38 -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\MDXExpert\ui_en.wxl" -loc "%~dp0\Installer\MDXExpert\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.MDXExpert.msi" "%~dp0\Installer\MDXExpert\Product.wixobj" "%~dp0\Installer\MDXExpert\Shortcuts.wixobj" "%~dp0\Installer\MDXExpert\Files.wixobj" "%~dp0\Installer\MDXExpert\Feature.wixobj" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wixobj" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wixobj"

echo Создание bootstrapper

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x86 -dbuildMode=global -sw -out "%~dp0Installer\BootstrapperMDXExpert\\" -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert.exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x64 -dbuildMode=global -sw -out "%~dp0Installer\BootstrapperMDXExpert\\" -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert(64).exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"


rem --------------------------------------------------

echo Копирование файла конфигурации в Office Add-in
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Office Add-in\Krista.FM.Update.ShedulerUpdateService.exe.config"
echo Копирование библиотеки Krista.FM.Update.Framework.dll
copy "%~dp0\Krista.FM.Update.Framework.dll" "%~dp0\App\Office Add-in\Krista.FM.Update.Framework.dll"
echo Копирование файла адаптера схемы 
copy "%~dp0\Krista.FM.Update.SchemeAdapter.dll" "%~dp0\App\Office Add-in\Krista.FM.Update.SchemeAdapter.dll"
echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Office Add-in\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -dbuildMode=global -dPlatform=x86 -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -out "%~dp0\Installer\Office Add-in\\" "%~dp0\Installer\Office Add-in\Product.wxs" "%~dp0\Installer\Office Add-in\Files.wxs" "%~dp0\Installer\Office Add-in\Feature.wxs" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wxs" "%~dp0\Installer\Office Add-in\Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Office Add-in\ui_en.wxl" -loc "%~dp0\Installer\Office Add-in\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.OfficeAddin.msi" "%~dp0\Installer\Office Add-in\Product.wixobj" "%~dp0\Installer\Office Add-in\Files.wixobj" "%~dp0\Installer\Office Add-in\Feature.wixobj" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wixobj" "%~dp0\Installer\Office Add-in\Shortcuts.wixobj"

rem --------------------------

echo Копирование файла конфигурации в Office Add-in
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\Office Add-in\Krista.FM.Update.ShedulerUpdateService.exe.config"
echo Копирование библиотеки Krista.FM.Update.Framework.dll
copy "%~dp0\Krista.FM.Update.Framework.dll" "%~dp0\App\Office Add-in\Krista.FM.Update.Framework.dll"
echo Копирование файла адаптера схемы 
copy "%~dp0\Krista.FM.Update.SchemeAdapter.dll" "%~dp0\App\Office Add-in\Krista.FM.Update.SchemeAdapter.dll"
echo Копирование CustomAction
copy "%~dp0\Krista.FM.Updater.CustomActionExe.exe" "%~dp0\App\Office Add-in\Krista.FM.Updater.CustomActionExe.exe"

echo Создание инсталлятора

"%~dp0\App\Wix\3.5\candle" -arch x64 -dbuildMode=global -dPlatform=x64 -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -out "%~dp0\Installer\Office Add-in\\" "%~dp0\Installer\Office Add-in\Product.wxs" "%~dp0\Installer\Office Add-in\Files.wxs" "%~dp0\Installer\Office Add-in\Feature.wxs" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wxs" "%~dp0\Installer\Office Add-in\Shortcuts.wxs"

"%~dp0\App\Wix\3.5\light" -ext "%~dp0\App\Wix\3.5\WixUtilExtension.dll" -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\Office Add-in\ui_en.wxl" -loc "%~dp0\Installer\Office Add-in\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.OfficeAddin(x64).msi" "%~dp0\Installer\Office Add-in\Product.wixobj" "%~dp0\Installer\Office Add-in\Files.wixobj" "%~dp0\Installer\Office Add-in\Feature.wixobj" "%~dp0\Installer\Office Add-in\WiXUI_Wizard.wixobj" "%~dp0\Installer\Office Add-in\Shortcuts.wixobj"


exit
