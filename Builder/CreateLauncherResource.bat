@echo off
del /F /Q Launcher.exe
cd ..
copy Launcher.exe Builder\Launcher.exe
cd Builder
upxgui
dir Launcher.exe /w
crypter Launcher.exe
brcc32 Launcher.rc
del /F /Q Launcher.exe
pause