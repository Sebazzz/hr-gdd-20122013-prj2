@echo off
copy Assets\Scene\Playground\Playground.unity Assets\Scene\%USERNAME%.me.scene
IF NOT %ERRORLEVEL%==0 PAUSE