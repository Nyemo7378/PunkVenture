@echo off
setlocal

REM 현재 bat 파일의 위치를 기준으로 Sum.exe 실행
set SCRIPT_DIR=%~dp0
start "" "%SCRIPT_DIR%Sum_Build\Sum.exe"

endlocal
