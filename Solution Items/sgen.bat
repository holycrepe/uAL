@echo off
goto START
:DO_WORK
@echo ----------------------------------------------------------------------------------------
@echo          GENERATING SERIALIZATION ASSEMBLIES FOR %CD%
@echo ----------------------------------------------------------------------------------------
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\sgen.exe" /nologo /force wuAL.exe
echo.
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\sgen.exe" /nologo /force uAL.dll
echo.
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\sgen.exe" /nologo /force UTorrentRestAPI.dll
echo.
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\sgen.exe" /nologo /force /t:Torrent.Properties.Settings.BaseSettings /t:Torrent.Properties.Settings.BaseSubSettings /t:Torrent.Properties.Settings.MySettings.MySettingsBase /t:Torrent.Properties.Settings.MySettings.MySettingsBase.MyMethodsSettings Torrent.dll
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6 Tools\x64\sgen.exe" /nologo /force /o:%CD% /compiler:/delaysign- /t:System.Collections.Specialized.StringCollection "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.dll"  
echo.
echo.
goto :EOF
:START
cls
cd /d D:\Git\uAL\wUAL\bin\Debug
call :DO_WORK
cd /d D:\Git\uAL\wUAL\bin\Release
call :DO_WORK
timeout 20
:EOF