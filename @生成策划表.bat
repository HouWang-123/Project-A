set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=./AAA-Luban-Excel

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputDataDir=Assets/ArtAssets/DATAOUTPUT ^
    -x outputCodeDir=Assets/Scripts/DONOTEDIT.LubanTables

pause