@echo off
setlocal

:: Chemin du dossier
set "folder=%~dp0"

:: Commande � ex�cuter
set "command=dotnet run"

:: Ouvrir CMD en tant qu'administrateur et ex�cuter la commande
powershell -Command "Start-Process cmd -ArgumentList '/k cd /d %folder% && %command%' -Verb RunAs"