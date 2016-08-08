@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

set srcDir = Saturn72.Mappers
set slnName = Saturn72.Mappers.sln
set projName = %srcDir%\Saturn72.Mappers.csproj
set pkgDir = packages
set testBin = Saturn72.Mappers.Tests\bin\%config%\Saturn72.Mappers.Tests.dll

REM Build
echo Build solution %slnName%
"%programfiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" %slnName% /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
if not "%errorlevel%"=="0" goto failure

REM Unit tests
echo install nunit runners to %pkdDir%
call %nuget% install NUnit.Runners -OutputDirectory &pkgDir%
%pkgDir%\NUnit.Runners.3.4.1\tools\nunit3-console.exe /config:%config%  %testBin%
if not "%errorlevel%"=="0" goto failure

REM Package
mkdir Build
echo create nuget package from %projName%
call %nuget% pack "%projName%" -symbols -o Build -p Configuration=%config% %version%
if not "%errorlevel%"=="0" goto failure

:success
exit 0

:failure
exit -1