@echo off
echo Assuming current directory's bin dir contains gource...
set gourceBinDir=%cd%\bin
echo Changing dir...
cd /D "Z:\Developing\Visual Studio\Projects\hr-gdd-20122013-prj2"
cd
svn log -r 1:HEAD --xml --verbose --quiet > subversion-log.me.xml
%gourceBinDir%\gource.exe -1280x720 --user-image-dir ".\users.me" --default-user-image ".\src\Assets\Textures\Icons\icon_16px.png" --title "Shifting Sheep" --camera-mode overview --file-idle-time 0 --file-filter "(.*)\.meta" subversion-log.me.xml
erase subversion-log.me.xml
pause