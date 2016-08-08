@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

set srcDir=src\
set slnName=%srcDir%Saturn72.Mappers.sln
set prjName=%srcDir%Saturn72.Mappers\Saturn72.Mappers.csproj
set testBin=%srcDir%Saturn72.Mappers.Tests\bin\%config%\Saturn72.Mappers.Tests.dll
set pkgDir=%cd%\%srcDir%packages


REM Restore nuget packages
echo restore nuget packages to %pkgDir% directory
call %NuGet% restore %slnName% -OutputDirectory %pkgDir% -NonInteractive


REM Build
echo Build solution %slnName%
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" %slnName% /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure


REM Unit tests
echo install nunit runners to %pkgDir%
call %nuget% install NUnit.Runners -OutputDirectory &pkgDir%
%pkgDir%\NUnit.Runners.3.4.1\tools\nunit3-console.exe /config:%config%  %testBin%
if not "%errorlevel%"=="0" goto failure


REM Package
mkdir Build
echo create nuget package from %prjName%
call %nuget% pack "%prjName%" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1

