; ============================================================================
; Wekeza Core Banking API — Inno Setup 6 installer script
; ============================================================================
; Builds a Windows .exe setup wizard that:
;   • Installs the self-contained .NET 10 (net10.0) API binaries
;   • Registers "WekeZaCoreAPI" as an auto-start Windows Service
;   • Adds an uninstall entry to Add/Remove Programs
;   • Removes the service cleanly on uninstall
;
; Build from the repository root (Windows):
;   "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ^
;       /DPublishDir="APIs\v1-Core\publish" ^
;       /DAppVersion="1.0.0" ^
;       installer\inno\wekeza-core.iss
;
; Or use the GitHub Actions workflow which sets these defines automatically.
; ============================================================================

; ── Compile-time defines (overridable from command line) ──────────────────
#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

#ifndef PublishDir
  #define PublishDir "..\..\APIs\v1-Core\publish"
#endif

; ── [Setup] ──────────────────────────────────────────────────────────────
[Setup]
AppId={{7F8A9B0C-1D2E-3F4A-5B6C-7D8E9F0A1B2C}
AppName=Wekeza Core Banking API
AppVersion={#AppVersion}
AppVerName=Wekeza Core Banking API {#AppVersion}
AppPublisher=Wekeza Bank
AppPublisherURL=https://wekeza.com
AppSupportURL=https://wekeza.com/support
AppUpdatesURL=https://wekeza.com

; Installation directory — 64-bit Program Files by default
DefaultDirName={commonpf64}\Wekeza Bank\Core API
DefaultGroupName=Wekeza Bank
DisableProgramGroupPage=yes

; Output
OutputDir=Output
OutputBaseFilename=wekeza-core-setup-v{#AppVersion}

; Compression
Compression=lzma2/ultra64
SolidCompression=yes
LZMAUseSeparateProcess=yes

; Require admin rights (needed for service registration)
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=

; 64-bit install only
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible

; Installer appearance
WizardStyle=modern
WizardSizePercent=120
DisableWelcomePage=no
DisableReadyPage=no

; Minimum OS: Windows 10 / Server 2016 (required for .NET 10)
MinVersion=10.0

; ── [Languages] ──────────────────────────────────────────────────────────
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

; ── [Files] ──────────────────────────────────────────────────────────────
[Files]
; Recursively copy the entire self-contained publish output
Source: "{#PublishDir}\*"; DestDir: "{app}"; \
    Flags: recursesubdirs createallsubdirs ignoreversion

; ── [Icons] ──────────────────────────────────────────────────────────────
[Icons]
; Start-menu shortcut to the uninstaller only (it's a headless service)
Name: "{group}\Uninstall Wekeza Core Banking API"; \
    Filename: "{uninstallexe}"; \
    IconFilename: "{uninstallexe}"

; ── [Messages] ────────────────────────────────────────────────────────────
[Messages]
WelcomeLabel1=Welcome to the Wekeza Core Banking API {#AppVersion} Setup
WelcomeLabel2=This will install the Wekeza Core Banking API on your computer as a Windows Service.%n%nThe service will listen on http://localhost:8080 by default.%n%nClick Next to continue.
FinishedHeadingLabel=Wekeza Core Banking API {#AppVersion} Installed
FinishedLabel=Setup has finished installing Wekeza Core Banking API on your computer.%n%nThe service "WekeZaCoreAPI" has been started and is set to start automatically with Windows.%n%nAPI base URL: http://localhost:8080%nSwagger UI:   http://localhost:8080/swagger

; ── [Code] ───────────────────────────────────────────────────────────────
[Code]

{ ── Helper: run sc.exe silently ── }
procedure RunSC(Params: String);
var
  ResultCode: Integer;
begin
  Exec(ExpandConstant('{sys}\sc.exe'), Params,
       '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

{ ── Poll until the service is stopped or the timeout (seconds) is reached ── }
procedure WaitForServiceStop(ServiceName: String; TimeoutSecs: Integer);
var
  Elapsed, ResultCode, I: Integer;
  TempFile: String;
  Lines: TArrayOfString;
  Stopped: Boolean;
begin
  TempFile := ExpandConstant('{tmp}\svcstate.txt');
  Elapsed := 0;
  repeat
    Sleep(1000);
    Elapsed := Elapsed + 1;
    { Redirect sc query output to a temp file, then scan for "STOPPED" }
    Exec(ExpandConstant('{sys}\cmd.exe'),
         '/c sc query ' + ServiceName + ' > "' + TempFile + '" 2>&1',
         '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Stopped := False;
    if LoadStringsFromFile(TempFile, Lines) then
    begin
      for I := 0 to GetArrayLength(Lines) - 1 do
      begin
        if Pos('STOPPED', UpperCase(Lines[I])) > 0 then
        begin
          Stopped := True;
          Break;
        end;
      end;
    end;
    if Stopped then
      Exit;
  until Elapsed >= TimeoutSecs;
end;

{ ── Stop the service if it is already running (e.g. upgrade scenario) ── }
procedure StopExistingService;
begin
  RunSC('stop WekeZaCoreAPI');
  WaitForServiceStop('WekeZaCoreAPI', 30);
end;

{ ── Register and start the Windows Service after files are copied ── }
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    { Register the service }
    RunSC(
      ExpandConstant(
        'create WekeZaCoreAPI'
        + ' binPath= "\"' + ExpandConstant('{app}') + '\Wekeza.Core.Api.exe\" --service"'
        + ' start= auto'
        + ' DisplayName= "Wekeza Core Banking API"'
      )
    );

    { Set the description }
    RunSC('description WekeZaCoreAPI "Wekeza Bank Core Banking REST API Service"');

    { Start the service }
    RunSC('start WekeZaCoreAPI');
  end;
end;

{ ── Stop and delete the service before files are removed (uninstall) ── }
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    RunSC('stop WekeZaCoreAPI');
    WaitForServiceStop('WekeZaCoreAPI', 30);
    RunSC('delete WekeZaCoreAPI');
  end;
end;

{ ── Warn if the service is already present (upgrade scenario) ── }
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ResultCode: Integer;
begin
  Result := '';
  { sc.exe query exits 0 if the service exists }
  if Exec(ExpandConstant('{sys}\sc.exe'), 'query WekeZaCoreAPI',
          '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
      StopExistingService;
  end;
end;
