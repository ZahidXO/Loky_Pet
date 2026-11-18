@echo off
REM ============================================================
REM  LOKY PET - Iniciar el sitio web (sin Visual Studio)
REM  Doble clic en este archivo para arrancar el proyecto.
REM  Se abrira automaticamente en tu navegador.
REM  Para detenerlo: cierra esta ventana negra o presiona Ctrl+C.
REM ============================================================

cd /d "%~dp0src\LokyPet.Web"

echo.
echo   Iniciando LOKY PET...
echo   Espera unos segundos, el navegador se abrira solo.
echo.

REM Abrir el navegador despues de 8 segundos (da tiempo a que compile)
start "" cmd /c "timeout /t 8 >nul & start http://localhost:5032"

REM Arrancar el servidor (perfil http, sin depuracion)
dotnet run --launch-profile http

pause
