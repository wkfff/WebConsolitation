chcp 1251

.\App\Wix\3.5\candle -out .\Installer\Updater\ -ext WixIISExtension -ext WixUtilExtension -ext WiXNetFxExtension  .\Installer\Updater\Product.wxs .\Installer\Updater\Files.wxs .\Installer\Updater\FilesClient.wxs .\Installer\Updater\FilesMDXExpert.wxs .\Installer\Updater\FilesOfficeAddIn.wxs .\Installer\Updater\Shortcuts.wxs .\Installer\Updater\CustomActions.wxs .\Installer\Updater\MyWebUI.wxs .\Installer\Updater\UIDialogs.wxs .\Installer\Updater\IisConfiguration.wxs

.\App\Wix\3.5\light -spdb -cultures:ru-RU;en-US -ext WixUIExtension -ext WixIISExtension -ext WixUtilExtension -ext WiXNetFxExtension -out .\Release\Krista.FM.Installer.msi .\Installer\Updater\Product.wixobj .\Installer\Updater\Shortcuts.wixobj .\Installer\Updater\Files.wixobj .\Installer\Updater\FilesClient.wixobj .\Installer\Updater\FilesMDXExpert.wixobj .\Installer\Updater\FilesOfficeAddIn.wixobj .\Installer\Updater\CustomActions.wixobj .\Installer\Updater\IisConfiguration.wixobj .\Installer\Updater\MyWebUI.wixobj .\Installer\Updater\UIDialogs.wixobj

pause