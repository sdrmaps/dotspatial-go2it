@ECHO OFF
setlocal ENABLEDELAYEDEXPANSION
for %%c in (v*.*) do (
	SET V=%%~nxc
)
SET VERSION=!V:~1!
SET COMPILE=Debug
SET PRODUCT=Go2It
CALL core\Build.cmd %COMPILE% %PRODUCT% %VERSION%
