@echo off
pushd %~dp0

powershell -Command "Invoke-WebRequest -Uri \"https://raw.githubusercontent.com/tgiqfe/Manifest/master/Manifest/Program.cs\" -OutFile \".\Manifest\Program.cs\""
powershell -Command "Invoke-WebRequest -Uri \"https://raw.githubusercontent.com/tgiqfe/Manifest/master/Manifest/PSD1.cs\" -OutFile \".\Manifest\PSD1.cs\""
powershell -Command "Invoke-WebRequest -Uri \"https://raw.githubusercontent.com/tgiqfe/Manifest/master/Manifest/PSM1.cs\" -OutFile \".\Manifest\PSM1.cs\""
