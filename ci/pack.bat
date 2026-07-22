@echo off
REM Empacota a extensão com o Toolbox do Playnite (rode no Windows com Playnite instalado).
REM Uso:
REM   set PLAYNITE_DIR=C:\Users\Public\Playnite
REM   ci\pack.bat
setlocal
if "%PLAYNITE_DIR%"=="" set "PLAYNITE_DIR=%LOCALAPPDATA%\Playnite"
set "TOOLBOX=%PLAYNITE_DIR%\Toolbox.exe"
if not exist "%TOOLBOX%" (
  echo Toolbox.exe nao encontrado em %PLAYNITE_DIR%
  exit /b 1
)

set "OUT=%~dp0..\dist"
if not exist "%OUT%" mkdir "%OUT%"

"%TOOLBOX%" pack "%~dp0..\WishlistHub" "%OUT%"
echo Pacote gerado em %OUT%
echo Renomeie/copie o .pext para WishlistHub_Playnite_1_1_0.pext e anexe na Release GitHub.
endlocal
