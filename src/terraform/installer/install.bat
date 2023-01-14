rem Intended for Windows

@echo off

rem Get a list of all available drives on the system
for /f "tokens=1,2 delims=: " %%a in ('wmic logicaldisk get deviceid ^| findstr /v ":"') do (
  set "local_drives=%%a"
)

rem Select the first local drive from the list
set "selected_drive=%local_drives:~0,1%"

rem Create the temp and terraform directories if they do not exist
if not exist "%selected_drive%\temp" mkdir "%selected_drive%\temp"
if not exist "%selected_drive%\terraform" mkdir "%selected_drive%\terraform"

rem Download and extract Terraform to the selected drive
powershell -Command "Invoke-WebRequest -Uri '%1' -OutFile '%selected_drive%\temp\terraform.zip'"
powershell -Command "Expand-Archive -Path '%selected_drive%\temp\terraform.zip' -force -DestinationPath '%selected_drive%\terraform'"

rem Remove temp dir with zip file
rd "%selected_drive%\temp" /s /q
