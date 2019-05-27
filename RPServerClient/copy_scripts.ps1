New-Item -ItemType Directory -Force -Path $PSScriptRoot\..\client_packages\cs_packages *>&1 | out-null
Remove-Item -Path $PSScriptRoot\..\client_packages\cs_packages\* -Recurse
get-childitem -path $PSScriptRoot -exclude obj, bin | Get-Childitem -recurse | Where-Object {$_.Extension -eq ".cs"} | Copy-Item -Destination $PSScriptRoot\..\client_packages\cs_packages