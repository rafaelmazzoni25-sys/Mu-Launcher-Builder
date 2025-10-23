program Builder;

uses
  Forms,
  UnitMain in 'UnitMain.pas' {frmMain},
  UnitUpdateBuilder in 'UnitUpdateBuilder.pas' {frmUpdateBuilder},
  UnitSkinBuilder in 'UnitSkinBuilder.pas' {FrameSkinBuilder: TFrame},
  UnitTools in 'UnitTools.pas',
  UnitSkinEditor in 'UnitSkinEditor.pas' {frmSkinEditor};

{$R *.res}
{$R Launcher.RES}

begin
  Application.Initialize;
  Application.CreateForm(TfrmMain, frmMain);
  Application.CreateForm(TfrmSkinEditor, frmSkinEditor);
  Application.Run;
end.
