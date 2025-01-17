@echo off

echo Post-Build Event - Start
echo Command Line: %0 %*

@if "%~1"=="" GOTO error
@if "%~2"=="" GOTO error
@if "%~3"=="" GOTO error

SET basepath=%~dp0
SET targetdir=%1
SET arch=%2
SET config=%3

rem Remove quote
set targetdir=%targetdir:"=%

rem If arch is a RuntimeIdentifier
IF "%arch%"=="win-x64" SET arch=x64
IF "%arch%"=="win-x86" SET arch=x86

echo BasePath: %basepath%
echo TargetDir: %targetdir%
echo Arch: %arch%
echo Config: %config%

echo Compile and copy Elevated - CLI Edition
call %basepath%\..\App.CLI.Windows.Elevated\build.bat %config% %arch% || GOTO error
copy %basepath%\..\App.CLI.Windows.Elevated\bin\%arch%\%config%\App.CLI.Windows.Elevated.exe "%targetdir%"\Eddie-CLI-Elevated.exe /Y /V || GOTO error

echo Compile and copy Elevated - Service Edition
call %basepath%\..\App.Service.Windows.Elevated\build.bat %config% %arch% || GOTO error
copy %basepath%\..\App.Service.Windows.Elevated\bin\%arch%\%config%\App.Service.Windows.Elevated.exe "%targetdir%"\Eddie-Service-Elevated.exe /Y /V || GOTO error

echo Compile and copy native library
call %basepath%\..\Lib.Platform.Windows.Native\build.bat Release %arch% || GOTO error
copy %basepath%\..\Lib.Platform.Windows.Native\bin\%arch%\Release\Lib.Platform.Windows.Native.dll "%targetdir%" /Y /V || GOTO error

echo Copy WireGuard library
copy %basepath%\..\..\deploy\windows_%arch%\wgtunnel.dll "%targetdir%\wgtunnel.dll" /Y /V || GOTO error
copy %basepath%\..\..\deploy\windows_%arch%\wireguard.dll "%targetdir%\wireguard.dll" /Y /V || GOTO error

echo Copy Wintun library
copy %basepath%\..\..\deploy\windows_%arch%\wintun.dll "%targetdir%\wintun.dll" /Y /V || GOTO error

GOTO done

:error
echo Something wrong
EXIT /B 1

:done
echo Post-Build Event - End
EXIT /B 0





