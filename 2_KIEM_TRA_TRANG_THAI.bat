@echo off
chcp 65001 >nul
title Kiểm Tra Trạng Thái Hệ Thống
powershell -NoProfile -ExecutionPolicy Bypass -File "C:\Users\VICTUS\.tools\manage_background.ps1" -Action status
echo.
pause
