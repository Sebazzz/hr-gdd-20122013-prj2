#define MyAppName "Shifting Sheep"
#define MyAppVersion "1.0"
#define MyAppPublisher "Team Unicorn"
#define MyAppURL "http://bit.ly/shiftingsheep"
#define MyAppExeName "ShiftingSheep.exe"
#define TodayBuildDirectory GetDateTimeString('yyyy-mm-dd', '', '');

[Setup]
AppId={{DC533770-2721-4D37-B578-0E17A63E00C1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=..\build.me\installer
OutputBaseFilename=setup
Compression=lzma2/ultra64
SolidCompression=yes
LZMAAlgorithm=1
LZMAUseSeparateProcess=yes
LZMANumFastBytes=256
LZMANumBlockThreads=4
;WizardImageFile=
;WizardSmallImageFile=

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\build.me\{#TodayBuildDirectory}\Win32\ShiftingSheep.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\build.me\{#TodayBuildDirectory}\Win32\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

