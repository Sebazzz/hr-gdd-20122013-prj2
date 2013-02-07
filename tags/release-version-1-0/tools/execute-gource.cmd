@echo off
echo Assuming current directory's bin dir contains gource...
set gourceBinDir=%cd%\Gource.me
echo Changing dir...
cd ".."
cd
svn log -r 1:HEAD --xml --verbose --quiet > subversion-log.me.xml
"%gourceBinDir%\gource.exe" -1280x720 --user-image-dir ".\users.me" --default-user-image ".\src\Assets\Textures\Icons\icon_16px.png" --title "Shifting Sheep" --camera-mode overview --file-idle-time 0 --file-filter "(.*)\.meta" subversion-log.me.xml
erase subversion-log.me.xml
pause