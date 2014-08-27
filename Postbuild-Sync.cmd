@echo off 
SETLOCAL ENABLEDELAYEDEXPANSION

SET PRODUCT=Go2It

for %%f in (archive/%PRODUCT%/*.msi) do (
    SET VERSION=%%~nf
)

SET /A COUNT=1
:NEXTVAR
    for /F "tokens=1* delims=." %%a in ("%VERSION%") do (
        IF !COUNT!==1 (
	    SET MAJ=%%a
	)
	IF !COUNT!==2 (
	    SET MIN=%%a
	)
	IF !COUNT!==3 (
	    SET BLD=%%a
	)
	IF !COUNT!==4 (
	    SET REV=%%a
	)
        SET /A COUNT+=1
        SET VERSION=%%b
    )
IF DEFINED VERSION GOTO NEXTVAR

SET DROPBOX="D:\Dropbox\Public\_Installs\SDR\%PRODUCT%\%JOB_NAME%-%MAJ%.%MIN%\Build-%MAJ%.%MIN%.%BLD%.%REV%"
IF NOT EXIST %DROPBOX% ( MD %DROPBOX% )

COPY /Y "archive\%PRODUCT%\*" %DROPBOX%\ 