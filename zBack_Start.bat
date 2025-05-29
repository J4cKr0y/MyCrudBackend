@echo off
setlocal

:: Chemin du dossier
set "folder=%~dp0"

:: Commande à exécuter
set "command=dotnet run"

:: Ouvrir CMD en tant qu'administrateur et exécuter la commande
powershell -Command "Start-Process cmd -ArgumentList '/k cd /d %folder% && %command%' -Verb RunAs"