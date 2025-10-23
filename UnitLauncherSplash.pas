unit UnitLauncherSplash;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ExtCtrls, UnitLauncherRecords;

type
  TfrmLauncherSplash = class(TForm)
    Timer1: TTimer;
    procedure Timer1Timer(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormPaint(Sender: TObject);
  private
    {shape a form}
    FRegion: THandle;
    BackgroundBitmap: TBitmap;
  public
    { Public declarations }
  end;

var
  frmLauncherSplash: TfrmLauncherSplash;

implementation

uses UnitLauncherMain;

{$R *.dfm}

procedure TfrmLauncherSplash.Timer1Timer(Sender: TObject);
begin
  Close;
end;

procedure TfrmLauncherSplash.FormCreate(Sender: TObject);
var
  Bmp: TBitmap;
begin
  DoubleBuffered := True;

  BackgroundBitmap:= TBitmap.Create;

  {shape the form}
  if not (frmLauncherMain.Options.SplashReg='EMPTY') then begin
    Bmp := TBitmap.Create;
    try
      GetImageStream(frmLauncherMain.Options.SplashReg, Bmp);

      FRegion := CreateRegion(Bmp);
      SetWindowRGN(Handle, FRegion, True);
    finally
      Bmp.Free;
    end;
  end;

  GetImageStream(frmLauncherMain.Options.SplashData, BackgroundBitmap);

  Width := BackgroundBitmap.Width;
  Height:= BackgroundBitmap.Height;

  Left:= (Screen.Width-BackgroundBitmap.Width) div 2;
  Top := (Screen.Height-BackgroundBitmap.Height) div 2;     
end;

procedure TfrmLauncherSplash.FormClose(Sender: TObject;
  var Action: TCloseAction);
begin
  if not ( frmLauncherMain.Skin.MainRegion='EMPTY') then begin
    DeleteObject(FRegion);
  end;
  BackgroundBitmap.Free;  
end;

procedure TfrmLauncherSplash.FormPaint(Sender: TObject);
begin
  Canvas.Draw(0, 0, BackgroundBitmap);
end;

end.
