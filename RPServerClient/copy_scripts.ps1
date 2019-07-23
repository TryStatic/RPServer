New-Item -ItemType Directory -Force -Path $PSScriptRoot\..\client_packages\cs_packages *>&1 | out-null
Remove-Item -Path $PSScriptRoot\..\client_packages\cs_packages\* -Recurse
robocopy $PSScriptRoot $PSScriptRoot\..\client_packages\cs_packages\client *.cs /XD bin obj /S | Out-Null
robocopy $PSScriptRoot\..\SharedProject $PSScriptRoot\..\client_packages\cs_packages\shared *.cs /XD bin obj /S | Out-Null