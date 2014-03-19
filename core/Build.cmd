@ECHO OFF
setlocal ENABLEDELAYEDEXPANSION

REM +------------------------------------------------------------------------------+
REM | Created: 2009.10.29, Justin Penka, Spatial Data Research Inc.                |
REM +------------------------------------------------------------------------------+
REM | Batch file for compilation of all components for a SDR Desktop Product       |
REM | Options: --Help - Display example and explain inputs (overrides all options) |
REM | Options: Complete|Debug|Clean|Release - Set compile type for product         |
REM | Options: Product Name - Set the name of the Product to be built              |
REM | Options: Product Version - Set Major.Minor Product version number            |
REM +------------------------------------------------------------------------------+

IF [%1]==[] (
   goto ERROR
) ELSE (
   goto PARAMS
)

:PARAMS
IF /I "%1"=="--Help" (
    ECHO.
    ECHO    Usage: Build.cmd [BuildType] [Product] [Version] --Help
    ECHO.
    ECHO    [BuildType]
    ECHO        Complete    Complete build of all product assemblies
    ECHO        Debug       Build of active projects in the main product solution file
    ECHO        Clean       Full clean of all assemblies and configuration files
    ECHO        Release     Build asssemblies and installer - for usage by build bot
    ECHO    [Product]       Name of the Product to build - ex. AddressIt
    ECHO    [Version]       Major.Minor Version of the Product to build - ex. 2.2
    ECHO    --Help          This information screen
    EXIT /B 0
) ELSE (
    REM Change to the main script directory so we can use relative paths
    PUSHD %~dp0
    IF /I "%1"=="Complete" ( goto COMPLETE ) ELSE (
        IF /I "%1"=="Debug" ( goto DEBUG ) ELSE (
	    IF /I "%1"=="Clean" ( goto CLEAN ) ELSE (
                IF /I "%1"=="Release" ( goto RELEASE ) ELSE ( goto ERROR ))))
)

:SETVERSION
    IF NOT [%3]==[] (SET VERSION="%3") ELSE (goto ERROR )
    REM Deqoute the Version number for later usage in wix build script
    SET VERSION=%VERSION:"=%
    REM If its a debug config then clean old configuration files
    IF /I "%CONFIG%"=="Release" ( goto COMPILE ) ELSE ( goto CLEANCONFIG )

:DEQUOTE
    IF NOT [%2]==[] (SET PRODUCT="%2") ELSE ( goto ERROR )
    REM Dequote the variables as they cause issues in path and token loops
    SET PRODUCT=%PRODUCT:"=%
    SET COMPILE=%COMPILE:"=%
    SET CONFIG=%CONFIG:"=%
    SET TYPE=%TYPE:"=%
    goto SETBUILDPATH

:SETBUILDPATH
    SET "BUILDPATH=..\build\%CONFIG%"
    goto SETVERSION
	
:RELEASE
    SET TYPE="Release"
    SET COMPILE="Build"
    SET CONFIG="Release"
    goto DEQUOTE

:CLEAN
    SET TYPE="Clean"
    SET COMPILE="Clean"
    SET CONFIG="Debug"
    goto DEQUOTE

:DEBUG
    SET TYPE="Debug"
    SET COMPILE="Build"
    SET CONFIG="Debug"
    goto DEQUOTE

:COMPLETE
    SET TYPE="Complete"
    SET COMPILE="Rebuild"
    SET CONFIG="Debug"
    goto DEQUOTE

:CLEANCONFIG
    ECHO.
    ECHO Delete Configuration files from stored directory
    ECHO.
    IF EXIST "%APPDATA%\SDR\%PRODUCT%" ( RD /S/Q "%APPDATA%\SDR\%PRODUCT%" )
    REM if this is a debug type then go ahead and make new copies of the configuration files
    IF /I "%COMPILE%"=="Clean" ( goto COMPILE ) ELSE ( goto COPYFILES )

:DELETEFILES
    ECHO.
    ECHO Delete all remaining files
    ECHO.
    IF EXIST "..\Build\%CONFIG%" ( RD /S/Q "..\Build\%CONFIG%" )
    goto END

:COMPILE
    ECHO.
    ECHO Compile Application Components
    ECHO.
    REM If cleaning then do the application first to properly unregister from COM (if so it will happen in the proj file)
    IF /I "%TYPE%"=="Clean" (
        msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=%BUILDPATH%\ ..\%PRODUCT%.sln
	IF NOT %ERRORLEVEL%==0 ( goto HALT )
    )
    REM Debug mode only compiles the product extension not additional libraries
    IF /I NOT "%TYPE%"=="Debug" (
        IF /I "%TYPE%"=="Release" (
            REM In release mode we need to assign a proper version number from jenkins
	    FOR /f "eol=; tokens=* delims= " %%a in (..\config\_components.txt) do (
	        FOR %%c in (Components\%%a\v*.*) do (
		    SET SUB_VERSION=%%~nxc
		)
		msbuild.exe /p:BuildInfo=%BUILD_INFO%;Version=!SUB_VERSION:~1! AssemblyInfo\Versioning.proj
		IF NOT %ERRORLEVEL%==0 ( goto HALT )
		IF EXIST Components\%%a\%%a.csproj (
		    msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=..\..\%BUILDPATH%\ Components\%%a\%%a.sln
		    IF NOT %ERRORLEVEL%==0 ( goto HALT )
		) ELSE (
		    msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=..\..\%BUILDPATH%\Plugins Plugins\%%a\%%a.sln
		    IF NOT %ERRORLEVEL%==0 ( goto HALT )
		)
	    )
	)
	IF NOT "%TYPE%"=="Release" (
	    FOR /f "eol=; tokens=* delims= " %%a in (..\config\_components.txt) do (
		IF EXIST Components\%%a\%%a.csproj (
		    msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=..\..\%BUILDPATH%\ Components\%%a\%%a.sln
		    IF NOT %ERRORLEVEL%==0 ( goto HALT )
		) ELSE (
		    msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=..\..\%BUILDPATH%\Plugins Plugins\%%a\%%a.sln
		    IF NOT %ERRORLEVEL%==0 ( goto HALT )
		)
	    )
	)
    )
    ECHO.
    ECHO Compile Application
    ECHO.
    IF "%TYPE%"=="Release" (
        REM In release mode we need to make sure to assign a version number using jenkins
        msbuild.exe /p:BuildInfo=%BUILD_INFO%;Version=%VERSION% AssemblyInfo\Versioning.proj
        IF NOT %ERRORLEVEL%==0 ( goto HALT )
    )
    IF NOT %TYPE%=="Clean" (
        msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=%BUILDPATH%\ ..\%PRODUCT%.sln
	IF NOT %ERRORLEVEL%==0 ( goto HALT )
    )
    IF /I "%TYPE%"=="Clean" ( goto DELETEFILES ) ELSE (
        IF /I "%TYPE%"=="Release" ( goto COPYFILES ) ELSE (
            goto END ))

:UPDATEHARVEST
    ECHO.
    ECHO Update Component and Plugin Harvest
    ECHO.
    REM Update all the paraffin files before running wix
    IF EXIST "..\install\ProductBinariesFragment.PARAFFIN" ( Tools\paraffin\Paraffin.exe -update "..\install\ProductBinariesFragment.PARAFFIN" ) ELSE (
        Tools\paraffin\Paraffin.exe -d %BUILDPATH% -gn "PRODUCT_BINARIES" -x en-us -dr "SDR_DIRECTORY" "..\install\ProductBinariesFragment.PARAFFIN" )
    REM Check if an actual ProductBinariesFragment for WIX exists
    IF NOT EXIST "..\install\ProductBinariesFragment.wxs" ( COPY /Y "..\install\ProductBinariesFragment.PARAFFIN" "..\install\ProductBinariesFragment.wxs" )
    REM Check if the to fragments are different, if so then replace the wix fragment with the PARAFFIN fragment
    Tools\xmldiff\xmldiff.exe "..\install\ProductBinariesFragment.wxs" "..\install\ProductBinariesFragment.PARAFFIN"
    IF NOT %ERRORLEVEL%==0  ( COPY /Y "..\install\ProductBinariesFragment.PARAFFIN" "..\install\ProductBinariesFragment.wxs" )
    goto CREATEINSTALL

:CREATEINSTALL
    ECHO.
    ECHO Create Installer
    ECHO.
    REM Run wix for updating the file fragments and creating a new installer
    msbuild.exe /t:%COMPILE% /p:Configuration=%CONFIG%;OutputPath=%BUILDPATH%\ ..\install\%PRODUCT%.Install.sln
    IF NOT %ERRORLEVEL%==0 ( goto HALT )
    RENAME "%BUILDPATH%\en-us\%PRODUCT%.msi" "%VERSION%.%BUILD_INFO%.msi"
    goto ZIPARTIFACTS
	
:ZIPARTIFACTS
    ECHO.
    ECHO Zip all build artifacts for archiving
    ECHO.
    REM make sure that the archive directory is available
    RD /S/Q "..\archive\%PRODUCT%\"
    IF NOT EXIST "..\archive\%PRODUCT%\" ( MD "..\archive\%PRODUCT%" )
    Tools\7zip\7za.exe -r a ..\archive\%PRODUCT%\%VERSION%.%BUILD_INFO%-install.zip %BUILDPATH%\en-us\%VERSION%.%BUILD_INFO%.msi
    COPY /Y "%BUILDPATH%\en-us\%VERSION%.%BUILD_INFO%.msi" "..\archive\%PRODUCT%"
    DEL /S "%BUILDPATH%\en-us\%VERSION%.%BUILD_INFO%.msi"
    DEL /S "%BUILDPATH%\en-us\%PRODUCT%.wixpdb"
    Tools\7zip\7za.exe -r a ..\archive\%PRODUCT%\%VERSION%.%BUILD_INFO%-files.zip %BUILDPATH%\*
    goto DELETEFILES

:COPYFILES
    ECHO.
    ECHO Copy configuration files and other documents to product directory
    ECHO.
    REM check that the build directory exists, create it if missing
    IF NOT EXIST %BUILDPATH% ( MD %BUILDPATH% )
    REM check that the config directory exists, create if missing
    IF NOT EXIST "%BUILDPATH%\Config" ( MD "%BUILDPATH%\Config" )
    REM copy all the configuration files to product config location
    XCOPY /Y /S /E "..\config\*" "%BUILDPATH%\Config"
    REM Copy in the License file
    XCOPY /Y "..\install\License.rtf" %BUILDPATH%
    REM Copy in the Manual file
    REM TODO XCOPY /Y /S "docs" %BUILDPATH%
    REM Delete any files or directories with underscore at start of name (do not included in build)
    SET "DEL_MATCH=%BUILDPATH%\Config\_*"
    DEL /S %DEL_MATCH%
    IF "%TYPE%"=="Release" ( goto UPDATEHARVEST )
    IF "%CONFIG%"=="Debug" ( goto COMPILE )
    REM Should never ever get to this halt point
    goto HALT

:ERROR
    ECHO.
    ECHO Invalid input parameters :: Build.cmd -Help for more information
    EXIT /B 1

:HALT
    ECHO.
    ECHO Error in Build??
    pause
    EXIT /B 1

:END
    ECHO.
    ECHO Build Complete!!
    pause
    EXIT /B 0
