chcp 1251

rem Версия 3.13

echo off

echo Копирование файла конфигурации в MDXExpert
copy "%~dp0\Krista.FM.Update.ShedulerUpdateService.exe.config" "%~dp0\App\MDXExpert\Krista.FM.Update.ShedulerUpdateService.exe.config"

echo Создание инсталлятора

"%~dp0\App\Bin\3.5\candle" -dbuildMode=global -sw -out "%~dp0\Installer\MDXExpert\\" "%~dp0\Installer\MDXExpert\Product.wxs" "%~dp0\Installer\MDXExpert\Files.wxs" "%~dp0\Installer\MDXExpert\Shortcuts.wxs" "%~dp0\Installer\MDXExpert\Feature.wxs" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wxs" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wxs"

"%~dp0\App\Bin\3.5\light" -sice:38 -sw -spdb -cultures:ru-RU;en-US -loc "%~dp0\Installer\MDXExpert\ui_en.wxl" -loc "%~dp0\Installer\MDXExpert\ru-ru.wxl" -out "%~dp0\FMLocalAppStore\Krista.FM.MDXExpert.msi" "%~dp0\Installer\MDXExpert\Product.wixobj" "%~dp0\Installer\MDXExpert\Shortcuts.wixobj" "%~dp0\Installer\MDXExpert\Files.wixobj" "%~dp0\Installer\MDXExpert\Feature.wixobj" "%~dp0\Installer\MDXExpert\WiXUI_Wizard.wixobj" "%~dp0\Installer\MDXExpert\WixUI_Shortcuts.wixobj"

echo Создание bootstrapper

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x86 -dbuildMode=global -sw -out "%~dp0Installer\BootstrapperMDXExpert\\" -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert.exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x64 -dbuildMode=global -sw -out "%~dp0Installer\BootstrapperMDXExpert\\" -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert(64).exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"


rem --------------------------------------------------


exit