@echo off
pushd %~dp0

Manifest\bin\Debug\Manifest.exe
copy /y "EnumRun\Script\StartupScript.ps1" "EnumRun\bin\EnumRun\StartupScript.ps1"
copy /y "EnumRun\Script\LogonScript.ps1" "EnumRun\bin\EnumRun\LogonScript.ps1"
copy /y "EnumRun\Script\LogoffScript.ps1" "EnumRun\bin\EnumRun\LogoffScript.ps1"
copy /y "EnumRun\Script\ShutdownScript.ps1" "EnumRun\bin\EnumRun\ShutdownScript.ps1"

pause
