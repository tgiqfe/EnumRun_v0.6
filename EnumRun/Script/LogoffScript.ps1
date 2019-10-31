Set-Location -Path ([System.IO.Path]::GetDirectoryName(${MyInvocation}.MyCommand.Path))
Import-Module ".\EnumRun.dll"

Enter-LogoffScript
