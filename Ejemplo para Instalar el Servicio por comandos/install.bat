@echo off
SET mypath=%~dp0
cd C:\Windows\Microsoft.NET\Framework\v4.0.30319
installutil.exe %mypath%/service/ServicioM.exe

SC CONFIG Service1 start=auto
SC start Service1 

if ERRORLEVEL 1 goto error
exit
:error
echo There was a problem
pause