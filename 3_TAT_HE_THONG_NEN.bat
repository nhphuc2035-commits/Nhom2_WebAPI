@echo off
chcp 65001 >nul
title Dừng Toàn Bộ Hệ Thống Nền
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\Users\VICTUS\.tools\manage_background.ps1" -Action stop
echo.
pause
