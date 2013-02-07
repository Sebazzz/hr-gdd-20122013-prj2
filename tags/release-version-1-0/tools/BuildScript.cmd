@echo off

REM
REM Unity Build Script version 1.0
REM

REM Get current date
SET datetimef=%date:~-4%-%date:~6,2%-%date:~3,2%
SET buildDate=%datetimef%
echo Build date: %buildDate%

REM Set environment
SET "unityPath=%programfiles(x86)%\Unity\Editor"
SET "zevenZipPath=%programfiles%\7-zip"
SET "innoSetupPath=A:\Inno Setup 5"

SET "buildPath=%cd%"
cd ..
SET "rootPath=%cd%"
cd src
SET "projectPath=%cd%"

cd "%buildPath%"

set "buildDir=%buildDate%"
set "win32BuildDir=%buildDir%\Win32"
set "win64BuildDir=%buildDir%\Win64"
set "OSXBuildDir=%buildDir%\OSX"

REM Set Default Parameters
SET BuildUnity=True
SET BuildAllZip=True
SET BuildOSXOnlyZip=False
SET BuildWin32Installer=False
SET VerifyFiles=True
goto :Menu

REM Menu
:Menu
cls
echo+
echo Choose:
echo [A] Build without zipping
echo [B] Build with zipping
echo [C] Build with installer (for Win32) and zip (for OSX)
echo [D] Build with installer only (for Win32) 
echo [T] Disable archive verification after build
echo [U] Do not build Unity project, use build from directory '%buildDate%'
echo [Q] Quit
echo+

:choice
SET /P C=[A,B,C,U,Q]? 
for %%? in (A) do if /I "%C%"=="%%?" goto :BuildWithoutZipping
for %%? in (B) do if /I "%C%"=="%%?" goto :BuildWithZipping
for %%? in (C) do if /I "%C%"=="%%?" goto :BuildWithInstallerAndZip
for %%? in (D) do if /I "%C%"=="%%?" goto :BuildWithInstaller
for %%? in (T) do if /I "%C%"=="%%?" goto :DisableArchiveVerification
for %%? in (U) do if /I "%C%"=="%%?" goto :NoUnityBuild
for %%? in (Q) do if /I "%C%"=="%%?" exit
goto :Menu

REM Disable Archive Verification
:DisableArchiveVerification
SET VerifyFiles=False
goto :Menu

REM No Unity Build
:NoUnityBuild
SET BuildUnity=False
goto :Menu

REM Build without zipping
:BuildWithoutZipping
SET BuildAllZip=False
SET BuildOSXOnlyZip=False
SET BuildWin32Installer=False
goto :ExecuteBuild

REM Build with zipping and installer
:BuildWithInstallerAndZip
SET BuildAllZip=False
SET BuildOSXOnlyZip=True
SET BuildWin32Installer=True
goto :ExecuteBuild

REM Build with installer
:BuildWithInstaller
SET BuildAllZip=False
SET BuildOSXOnlyZip=False
SET BuildWin32Installer=True
goto :ExecuteBuild

REM Build with zipping
:BuildWithZipping
SET BuildAllZip=True
SET BuildOSXOnlyZip=False
SET BuildWin32Installer=False
goto :ExecuteBuild

REM Execute Build: Verify parameters
:ExecuteBuild
cls
echo Parameters:
echo Execute Unity build: %BuildUnity%
echo 7-zip all files: %BuildAllZip%
echo Build Win32 installer: %BuildWin32Installer%
echo Seperate OSX zip: %BuildOSXOnlyZip%
echo+
echo Project path: %projectPath%
echo Build path: %buildPath%
echo+

echo Continue? [Y/N]
SET /P C=[Y/N]? 
for %%? in (Y) do if /I "%C%"=="%%?" goto :ExecuteBuildStep1
for %%? in (N) do if /I "%C%"=="%%?" goto :Menu
goto :ExecuteBuild

REM Execute Build: Step 1 Build Via Unity
:ExecuteBuildStep1
echo+
echo Step 1: Executing build

IF %BuildUnity%==True call :ExecuteBuildUnity
IF %BuildUnity%==False echo Not building via Unity, skipping step 1

goto :ExecuteBuildStep2

REM Execute Build: Step 2 Select compression method
:ExecuteBuildStep2
echo+
echo Step 2: Packaging

IF %BuildAllZip%==True call :ExecuteZipAllFiles

IF %BuildWin32Installer%==True call :ExecuteBuildWin32Installer

IF %BuildOSXOnlyZip%==True call :ExecuteOSXZip

IF %BuildWin32Installer%==True IF %BuildOSXOnlyZip%==True call :ExecuteCreateCommonPackage

goto :ExecuteBuildStep3

REM Execute Build: Step 3 Verify Files
:ExecuteBuildStep3

echo+
echo Step 3: Verify files

IF  VerifyFiles==True call :ExecuteVerifyFiles
IF  VerifyFiles==False echo Not verifying files, skipping step 3

goto :ExecuteBuildStep4

REM Execute Build: Step 4 End
:ExecuteBuildStep4
REM echo +
REM echo Copy current to dir
REM rmdir /S /Q current
REM robocopy "%buildDir%" current /E

echo +
echo Build completed.
pause

goto :Menu

REM Verify Files / Test Archive
:ExecuteVerifyFiles

echo Testing archive...
"%zevenZipPath%\7z.exe" t %buildDate%.7z -r

goto :eof

REM Build Win32 installer
:ExecuteBuildWin32Installer

"%innoSetupPath%\Compil32.exe" /cc "%rootPath%\installer\ShiftingSheep.Installer.iss"

goto :eof

REM 7-zip all files
:ExecuteZipAllFiles

echo Zipping build directory...
"%zevenZipPath%\7z.exe" a -t7z -ssc -mx=9 -ms=on -mf=off -mmt=on -m0=LZMA:d=512m %buildDate%.7z "%buildDir%"

goto :eof

REM 7-zip OSX files
:ExecuteOSXZip

echo Zipping OSX build directory...
"%zevenZipPath%\7z.exe" a -tzip -ssc -mx=9 -mfb=257 -mpass=5 -md=64k -mmt=on %buildDate%-OSX.zip "%buildDir%\OSX"

goto :eof

REM Zip installer
:ExecuteCreateCommonPackage

echo Zipping installer and OSX in common package

"%zevenZipPath%\7z.exe" a -tzip -ssc -mx=0 "%buildDate%-win32-OSX.zip" "%buildDate%-OSX.zip" "installer\Setup.exe"

goto :eof

REM Execute Build: Build Via Unity
:ExecuteBuildUnity

REM -- Create folder structure
echo Creating directory structure...
mkdir "%buildDir%"
mkdir "%win32BuildDir%"
mkdir "%win64BuildDir%"
mkdir "%OSXBuildDir%"

REM -- Execute actual build
SET "buildLogFilePath=%buildPath%\BuildLog.log"

echo Building Win32, Win64, OSX... (this may take some time)
echo Using build log: %buildLogFilePath%
echo+

"%unityPath%\Unity.exe" -buildWindowsPlayer "%buildPath%\%win32BuildDir%\ShiftingSheep.exe" -buildOSXPlayer "%buildPath%\%OSXBuildDir%\ShiftingSheep.app" -projectPath "%projectPath%" -logFile "%buildLogFilePath%" -quit

goto :eof


echo All Done!