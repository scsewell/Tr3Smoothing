@echo OFF

SET BASE_DIR=%~dp0
set SOLN_DIR=%BASE_DIR%SoSmooth\

set PROJECT=%SOLN_DIR%SoSmooth.csproj

set DEPEND_DIR=%SOLN_DIR%Dependencies\
set RELEASE_DIR=%SOLN_DIR%bin\Release\
set DEBUG_DIR=%SOLN_DIR%bin\Debug\

set CONGIFURATION=Release
set OUTPUT_DIR=%RELEASE_DIR%

if exist %OUTPUT_DIR% rmdir %OUTPUT_DIR% /s /q
MSBuild.exe %PROJECT% /v:m /p:Configuration=%CONGIFURATION%;OutDir=%OUTPUT_DIR%
set LIB_DIR=%OUTPUT_DIR%lib\
mkdir %LIB_DIR%
xcopy "%DEPEND_DIR%OpenTK.dll"          "%LIB_DIR%" /y
xcopy "%DEPEND_DIR%OpenTK.dll.config"   "%LIB_DIR%" /y
xcopy "%DEPEND_DIR%GLWidget.dll"        "%LIB_DIR%" /y


set CONGIFURATION=Debug
set OUTPUT_DIR=%DEBUG_DIR%

if exist %OUTPUT_DIR% rmdir %OUTPUT_DIR% /s /q
MSBuild.exe %PROJECT% /v:m /p:Configuration=%CONGIFURATION%;OutDir=%OUTPUT_DIR%
set LIB_DIR=%OUTPUT_DIR%lib\
mkdir %LIB_DIR%
xcopy "%DEPEND_DIR%OpenTK.dll"          "%LIB_DIR%" /y
xcopy "%DEPEND_DIR%OpenTK.dll.config"   "%LIB_DIR%" /y
xcopy "%DEPEND_DIR%GLWidget.dll"        "%LIB_DIR%" /y

echo Build Complete