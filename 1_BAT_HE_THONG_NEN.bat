@echo off
chcp 65001 >nul
title Khởi Động Hệ Thống Nền (WebAPI + ngrok)
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\Users\VICTUS\.tools\manage_background.ps1" -Action start
echo.
pause
