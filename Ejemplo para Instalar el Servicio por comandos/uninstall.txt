@echo off
SC STOP Service1
SC DELETE Service1

if ERRORLEVEL 1 goto error
exit
:error
echo There was a problem
pause

