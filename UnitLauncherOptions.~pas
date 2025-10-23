unit UnitLauncherOptions;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ExtCtrls, StdCtrls, RzButton, RzRadChk, RzBmpBtn, UnitLauncherRecords,
  JclRegistry;

type
  TfrmLauncherOptions = class(TForm)
    eAccountID: TEdit;
    rbRes0: TRzRadioButton;
    rbRes1: TRzRadioButton;
    rbRes2: TRzRadioButton;
    rbRes3: TRzRadioButton;
    cbSoundONOFF: TRzCheckBox;
    cbMusicONOFF: TRzCheckBox;
    lblIDText: TLabel;
    lblResolution: TLabel;
    lblSound: TLabel;
    procedure imgBackgroundMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure imgBackgroundMouseMove(Sender: TObject; Shift: TShiftState;
      X, Y: Integer);
    procedure imgBackgroundMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure btnCloseClick(Sender: TObject);
    procedure btnApplyClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure FormPaint(Sender: TObject);
    procedure FormMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure FormMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
  private
    {shape a form}
    FRegion: THandle;
  public
    BackgroundBitmap: TBitmap;
    btnClose: TRzBmpButton;
    btnApply: TRzBmpButton;
    Dragging: Boolean;
    DragStart: TPoint;
  end;

var
  frmLauncherOptions: TfrmLauncherOptions;

implementation

uses UnitLauncherMain;

{$R *.dfm}

procedure TfrmLauncherOptions.imgBackgroundMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
   if (Button = mbLeft) then
    begin
       Dragging := True;
       DragStart := TControl(Sender).ClientToScreen(Point(X,Y));
    end;
end;

procedure TfrmLauncherOptions.imgBackgroundMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
var
   Diff: TPoint;
begin
   if Dragging then
   begin
      Diff := TControl(Sender).ClientToScreen(Point(X,Y));
      Diff := Point(Diff.X-DragStart.X,Diff.Y-DragStart.Y);
      SetBounds(Left+Diff.X,Top+Diff.Y,Width,Height);
      DragStart.X := DragStart.X+Diff.X;
      DragStart.Y := DragStart.Y+Diff.Y;
   end;
end;

procedure TfrmLauncherOptions.imgBackgroundMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if (Button = mbLeft) then Dragging := false; 
end;

procedure TfrmLauncherOptions.btnCloseClick(Sender: TObject);
begin
  Close;
end;

procedure TfrmLauncherOptions.btnApplyClick(Sender: TObject);
var
  temp: ShortInt;
begin
  RegWriteString(HKEY_CURRENT_USER, WebzenKEY, 'ID', eAccountID.Text);
  
  if rbRes0.Checked then temp:=0;
  if rbRes1.Checked then temp:=1;
  if rbRes2.Checked then temp:=2;
  if rbRes3.Checked then temp:=3;
  RegWriteInteger(HKEY_CURRENT_USER, WebzenKEY, 'ColorDepth', temp);
  RegWriteBool(HKEY_CURRENT_USER, WebzenKEY, 'SoundOnOff', cbSoundONOFF.Checked);
  RegWriteBool(HKEY_CURRENT_USER, WebzenKEY, 'MusicOnOff', cbMusicONOFF.Checked);

  btnCloseClick(Sender); 
end;

procedure TfrmLauncherOptions.FormCreate(Sender: TObject);
var
  CloseButton: TSkinButton;
  ApplyButton: TSkinButton;
  temp: ShortInt;
  Bmp: TBitmap;
begin
  DoubleBuffered := True;
  
  try {Skip exception if no settings found}
    eAccountID.Text:= RegReadString(HKEY_CURRENT_USER, WebzenKEY, 'ID');

    temp:= RegReadInteger(HKEY_CURRENT_USER, WebzenKEY, 'ColorDepth');
    case temp of    //
      0: rbRes0.Checked := True;
      1: rbRes1.Checked := True;
      2: rbRes2.Checked := True;
      3: rbRes3.Checked := True;
    end;    // case

    cbSoundONOFF.Checked:= RegReadBool(HKEY_CURRENT_USER, WebzenKEY, 'SoundOnOff');
    cbMusicONOFF.Checked:= RegReadBool(HKEY_CURRENT_USER, WebzenKEY, 'MusicOnOff');
  except
    ;
  end;    

  CloseButton:= frmLauncherMain.Skin.Buttons.Close2;
  ApplyButton:= frmLauncherMain.Skin.Buttons.Apply;  

  BackgroundBitmap:= TBitmap.Create;
  btnClose:= TRzBmpButton.CreateParented(Self.Handle);
  btnClose.OnClick:= btnCloseClick;

  btnApply:= TRzBmpButton.CreateParented(Self.Handle);
  btnApply.OnClick:= btnApplyClick;

  {shape the form}
  if not (frmLauncherMain.Skin.OptionRegion='EMPTY') then begin
    Bmp := TBitmap.Create;
    try
      GetImageStream(frmLauncherMain.Skin.OptionRegion, Bmp);

      FRegion := CreateRegion(Bmp);
      SetWindowRGN(Handle, FRegion, True);
    finally
      Bmp.Free;
    end;
  end;   

  GetImageStream(frmLauncherMain.Skin.OptionBackground, BackgroundBitmap);

  Width := BackgroundBitmap.Width;
  Height:= BackgroundBitmap.Height;

  Left:= (Screen.Width-BackgroundBitmap.Width) div 2;
  Top := (Screen.Height-BackgroundBitmap.Height) div 2;

  btnClose.Parent := Self;
  btnClose.Left := CloseButton.X;
  btnClose.Top := CloseButton.Y;
  GetImageStream(CloseButton.DataN, btnClose.Bitmaps.Up);
  GetImageStream(CloseButton.DataD, btnClose.Bitmaps.Down);

  btnApply.Parent := Self;
  btnApply.Left := ApplyButton.X;
  btnApply.Top := ApplyButton.Y;
  GetImageStream(ApplyButton.DataN, btnApply.Bitmaps.Up);
  GetImageStream(ApplyButton.DataD, btnApply.Bitmaps.Down);

  lblIDText.Caption := frmLauncherMain.Skin.ID.ID;
  lblIDText.Left := frmLauncherMain.Skin.ID.IDText.Left;
  lblIDText.Width := frmLauncherMain.Skin.ID.IDText.Width;
  lblIDText.Top := frmLauncherMain.Skin.ID.IDText.Top;
  lblIDText.Height := frmLauncherMain.Skin.ID.IDText.Height;

  eAccountID.Left := frmLauncherMain.Skin.ID.IDPos.Left;
  eAccountID.Width := frmLauncherMain.Skin.ID.IDPos.Width;
  eAccountID.Top := frmLauncherMain.Skin.ID.IDPos.Top;
  eAccountID.Height := frmLauncherMain.Skin.ID.IDPos.Height;

  lblResolution.Caption := frmLauncherMain.Skin.Res.Resolution;
  lblResolution.Left := frmLauncherMain.Skin.Res.ResolutionText.Left;
  lblResolution.Width := frmLauncherMain.Skin.Res.ResolutionText.Width;
  lblResolution.Top := frmLauncherMain.Skin.Res.ResolutionText.Top;
  lblResolution.Height := frmLauncherMain.Skin.Res.ResolutionText.Height;

  rbRes0.Left := frmLauncherMain.Skin.Res.ResOptions.Res1.Left;
  rbRes0.Top := frmLauncherMain.Skin.Res.ResOptions.Res1.Top;
  rbRes1.Left := frmLauncherMain.Skin.Res.ResOptions.Res2.Left;
  rbRes1.Top := frmLauncherMain.Skin.Res.ResOptions.Res2.Top;
  rbRes2.Left := frmLauncherMain.Skin.Res.ResOptions.Res3.Left;
  rbRes2.Top := frmLauncherMain.Skin.Res.ResOptions.Res3.Top;
  rbRes3.Left := frmLauncherMain.Skin.Res.ResOptions.Res4.Left;
  rbRes3.Top := frmLauncherMain.Skin.Res.ResOptions.Res4.Top;

  lblSound.Caption := frmLauncherMain.Skin.Sound.Sound;
  lblSound.Left := frmLauncherMain.Skin.Sound.SoundText.Left;
  lblSound.Width := frmLauncherMain.Skin.Sound.SoundText.Width;
  lblSound.Top := frmLauncherMain.Skin.Sound.SoundText.Top;
  lblSound.Height := frmLauncherMain.Skin.Sound.SoundText.Height;

  cbSoundONOFF.Left := frmLauncherMain.Skin.Sound.SoundOptions.Sound.Left;
  cbSoundONOFF.Top := frmLauncherMain.Skin.Sound.SoundOptions.Sound.Top;
  cbMusicONOFF.Left := frmLauncherMain.Skin.Sound.SoundOptions.Music.Left;
  cbMusicONOFF.Top := frmLauncherMain.Skin.Sound.SoundOptions.Music.Top;
end;

procedure TfrmLauncherOptions.FormClose(Sender: TObject;
  var Action: TCloseAction);
begin
  btnClose.Free;
  btnApply.Free;
  {Delete shape}
  if not ( frmLauncherMain.Skin.MainRegion='EMPTY') then begin
    DeleteObject(FRegion);
  end;  
  BackgroundBitmap.Free;  
end;

procedure TfrmLauncherOptions.FormPaint(Sender: TObject);
begin
  Canvas.Draw(0, 0, BackgroundBitmap); 
end;

procedure TfrmLauncherOptions.FormMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
   if (Button = mbLeft) then
    begin
       Dragging := True;
       DragStart := TControl(Sender).ClientToScreen(Point(X,Y));
    end;
end;

procedure TfrmLauncherOptions.FormMouseMove(Sender: TObject;
  Shift: TShiftState; X, Y: Integer);
var
   Diff: TPoint;
begin
   if Dragging then
   begin
      Diff := TControl(Sender).ClientToScreen(Point(X,Y));
      Diff := Point(Diff.X-DragStart.X,Diff.Y-DragStart.Y);
      SetBounds(Left+Diff.X,Top+Diff.Y,Width,Height);
      DragStart.X := DragStart.X+Diff.X;
      DragStart.Y := DragStart.Y+Diff.Y;
   end;
end;

procedure TfrmLauncherOptions.FormMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if (Button = mbLeft) then Dragging := false;
end;

end.
