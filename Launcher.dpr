program Launcher;

uses
  Forms,
  UnitLauncherMain in 'UnitLauncherMain.pas' {frmLauncherMain},
  UnitLauncherOptions in 'UnitLauncherOptions.pas' {frmLauncherOptions},
  UnitLauncherUpdate in 'UnitLauncherUpdate.pas' {frmLauncherUpdate},
  UnitLauncherSplash in 'UnitLauncherSplash.pas' {frmLauncherSplash},
  UnitLauncherRecords in 'UnitLauncherRecords.pas';

{$R *.res}

begin
  Application.Initialize;
  Application.CreateForm(TfrmLauncherMain, frmLauncherMain);
  Application.Run;
end.
