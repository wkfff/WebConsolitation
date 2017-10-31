echo off
chcp 1251

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x86 -dbuildMode=global -sw -out %~dp0Installer\BootstrapperMDXExpert\ -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert.exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"

"%~dp0App\Wix\3.6\candle.exe" -dPlatform=x64 -dbuildMode=global -sw -out %~dp0Installer\BootstrapperMDXExpert\ -ext "%~dp0App\Wix\3.6\WixUtilExtension.dll" -ext "%~dp0App\Wix\3.6\WixBalExtension.dll" "%~dp0Installer\BootstrapperMDXExpert\Bundle.wxs" "%~dp0Installer\BootstrapperMDXExpert\Package.wxs"

"%~dp0\App\Wix\3.6\Light.exe" -spdb -cultures:ru-RU;en-US -out "%~dp0\FMLocalAppStore\Krista.FM.Bootstrapper.MDXExpert(64).exe" -ext "dp0\App\Wix\3.6\WixUtilExtension.dll"	-ext "dp0\App\Wix\3.6\WixBalExtension.dll" "%~dp0\Installer\BootstrapperMDXExpert\Bundle.wixobj" "%~dp0\Installer\BootstrapperMDXExpert\Package.wixobj"

pause
