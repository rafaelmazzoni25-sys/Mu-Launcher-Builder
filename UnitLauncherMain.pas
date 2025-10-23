unit UnitLauncherMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, OleCtrls, SHDocVw, ExtCtrls, RzBmpBtn, RzPrgres,
  JvSimpleXml, JclSysUtils, UnitLauncherRecords, XPManifestFile,
  IdBaseComponent, IdComponent, IdRawBase, IdRawClient, IdIcmpClient,
  IdTCPConnection, IdTCPClient, IdIOHandler, IdIOHandlerSocket;

type
  TfrmLauncherMain = class(TForm)
    OptionsData: TJvSimpleXML;
    XPManifestFile1: TXPManifestFile;
    lblStatus: TLabel;
    TimerServerStatus: TTimer;
    procedure ParseOptions;
    procedure btnCloseClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure btnOptionClick(Sender: TObject);
    procedure btnUpdateClick(Sender: TObject);
    procedure btnConnectClick(Sender: TObject);
    procedure FormPaint(Sender: TObject);
    procedure FormMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure FormMouseMove(Sender: TObject; Shift: TShiftState; X,
      Y: Integer);
    procedure FormMouseUp(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
    procedure TimerServerStatusTimer(Sender: TObject);
  private
    BackGroundImage: TBitmap;
    wbBrowser: TWebBrowser;
    btnClose: TRzBmpButton;
    btnConnect: TRzBmpButton;
    btnUpdate: TRzBmpButton;
    btnOption: TRzBmpButton;
    {shape a form}
    FRegion: THandle;
  public
    Options: TOptionsData;
    Skin: TSkin;
    Dragging: Boolean;
    DragStart: TPoint; 
  end;

var
  frmLauncherMain: TfrmLauncherMain;

implementation

uses UnitLauncherOptions, UnitLauncherUpdate, UnitLauncherSplash;

{$R *.dfm}

procedure ExecuteProcess( FileName,
                          Params   : String;
                          Show     : Integer );
var
  StartUpInfo: TStartUpInfo;
  ProcessInfo: TProcessInformation;
begin
  Params := Trim(Params);
  if Params<>'' then
    FileName := FileName + ' ' + Params;

  GetStartupInfo( StartUpInfo );
  with StartUpInfo do begin
    cb := SizeOf(TStartUpInfo);
    dwFlags := STARTF_USESHOWWINDOW;
    wShowWindow := Show;
  end;
  if not (CreateProcess( nil, PChar(FileName), nil, nil, False,
    NORMAL_PRIORITY_CLASS, nil, nil, StartUpInfo, ProcessInfo)) then
      RaiseLastWin32Error;  //From SysUtils Unit!!
end;

function CheckGameServer(const Hostname: String; Const GamePort: Integer ): Boolean;
var
    Client: TIdTCPClient;
begin
 Client:= TIdTCPClient.Create(nil);
 try
  Client.Host := Hostname;
  Client.Port := GamePort;
  try
   Client.Connect;
   Result:= Client.Connected;
   Client.Disconnect;
  except
   Result:= False;
  end;
 finally
  Client.Free;         
 end;
end;

function PingServer(const Hostname: String): String;
var ICMP: TIdIcmpClient;
begin
 ICMP:= TIdIcmpClient.Create(nil);
 try
  ICMP.Host:= Hostname;
  ICMP.ReceiveTimeout:= 1000;
  try
   ICMP.Ping;
   case ICMP.ReplyStatus.ReplyStatusType of
    rsEcho: Result:= IntToStr(ICMP.ReplyStatus.MsRoundTripTime) + 'ms';
    rsError: Result:= 'Error';
    rsTimeOut: Result:= 'Time Out';
    rsErrorUnreachable: Result:= 'Host Unreachable';
    rsErrorTTLExceeded: Result:= 'TTL Exceeded';
   end;
  except
   Result:= 'No Route to host';
  end;
 finally
  ICMP.Free;
 end;
end;

procedure TfrmLauncherMain.ParseOptions;
begin
  {GUI}
  Options.ShowBrowse:= OptionsData.Root.Items.Item[0].Properties.BoolValue('ShowBrowser');
  Options.ShowOption:= OptionsData.Root.Items.Item[0].Properties.BoolValue('ShowOption');
  Options.ShowUpdate:= OptionsData.Root.Items.Item[0].Properties.BoolValue('ShowUpdate');
  Options.ShowSplash:= OptionsData.Root.Items.Item[0].Properties.BoolValue('ShowSplashScreen');
  Options.ShowStatus:= OptionsData.Root.Items.Item[0].Properties.BoolValue('ShowServerStatus');
  {Browser}
  Options.BrowseURL := OptionsData.Root.Items.Item[1].Properties.Value('BrowserURL');

  Skin.Browser.Left  := OptionsData.Root.Items.Item[1].Properties.IntValue('BrowserLeft');
  Skin.Browser.Width := OptionsData.Root.Items.Item[1].Properties.IntValue('BrowserWidth');
  Skin.Browser.Top   := OptionsData.Root.Items.Item[1].Properties.IntValue('BrowserTop');
  Skin.Browser.Height:= OptionsData.Root.Items.Item[1].Properties.IntValue('BrowserHeight');
  {ServerStatus}
  Skin.ServerStatus.Left  := OptionsData.Root.Items.Item[1].Properties.IntValue('StatusLeft');
  Skin.ServerStatus.Width := OptionsData.Root.Items.Item[1].Properties.IntValue('StatusWidth');
  Skin.ServerStatus.Top   := OptionsData.Root.Items.Item[1].Properties.IntValue('StatusTop');
  Skin.ServerStatus.Height:= OptionsData.Root.Items.Item[1].Properties.IntValue('StatusHeight');

  {SplashScreen}
  Options.SplashData:= OptionsData.Root.Items.Item[2].Properties.Value('Data');
  Options.SplashReg:= OptionsData.Root.Items.Item[2].Properties.Value('Region');
  {Server}
  Options.ServerName:= OptionsData.Root.Items.Item[3].Properties.Value('ServerName');
  Options.ServerIP  := OptionsData.Root.Items.Item[3].Properties.Value('ServerHostname');
  Options.ServerPort:= OptionsData.Root.Items.Item[3].Properties.Value('ServerPort');
  Options.ServerPage:= OptionsData.Root.Items.Item[3].Properties.Value('ServerPage');
  {Update}
  Options.UpdateData:= OptionsData.Root.Items.Item[4].Properties.Value('UpdateData');
  {Colors}
  Options.MColor := OptionsData.Root.Items.Item[5].Properties.IntValue('MainColor');
  Options.MFontColor := OptionsData.Root.Items.Item[5].Properties.IntValue('MainFontColor');
  Options.OColor := OptionsData.Root.Items.Item[5].Properties.IntValue('OptionsColor');
  Options.OFontColor := OptionsData.Root.Items.Item[5].Properties.IntValue('OptionsFontColor');
  {Skin}
  Skin.MainBackground  := OptionsData.Root.Items.Item[6].Properties.Value('Background');
  Skin.MainRegion      := OptionsData.Root.Items.Item[6].Properties.Value('MainRegion');
  Skin.OptionBackground:= OptionsData.Root.Items.Item[6].Properties.Value('Options');
  Skin.OptionRegion    := OptionsData.Root.Items.Item[6].Properties.Value('OptionsRegion');

  Skin.Buttons.Close.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('CL_N');
  Skin.Buttons.Close.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('CL_D');
  Skin.Buttons.Close.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('CL_X');
  Skin.Buttons.Close.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('CL_Y');

  Skin.Buttons.Connect.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('CN_N');
  Skin.Buttons.Connect.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('CN_D');
  Skin.Buttons.Connect.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('CN_X');
  Skin.Buttons.Connect.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('CN_Y');

  Skin.Buttons.Update.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('UP_N');
  Skin.Buttons.Update.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('UP_D');
  Skin.Buttons.Update.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('UP_X');
  Skin.Buttons.Update.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('UP_Y');

  Skin.Buttons.Option.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('OP_N');
  Skin.Buttons.Option.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('OP_D');
  Skin.Buttons.Option.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('OP_X');
  Skin.Buttons.Option.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('OP_Y');

  Skin.Buttons.Close2.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('CL2_N');
  Skin.Buttons.Close2.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('CL2_D');
  Skin.Buttons.Close2.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('CL2_X');
  Skin.Buttons.Close2.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('CL2_Y');

  Skin.Buttons.Apply.DataN:= OptionsData.Root.Items.Item[6].Properties.Value('AP_N');
  Skin.Buttons.Apply.DataD:= OptionsData.Root.Items.Item[6].Properties.Value('AP_D');
  Skin.Buttons.Apply.X    := OptionsData.Root.Items.Item[6].Properties.IntValue('AP_X');
  Skin.Buttons.Apply.Y    := OptionsData.Root.Items.Item[6].Properties.IntValue('AP_Y');

  Skin.ID.ID:= OptionsData.Root.Items.Item[6].Properties.Value('ID_TEXT');
  Skin.ID.IDText.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('ID_L');
  Skin.ID.IDText.Width:= OptionsData.Root.Items.Item[6].Properties.IntValue('ID_W');
  Skin.ID.IDText.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('ID_T');
  Skin.ID.IDText.Height:= OptionsData.Root.Items.Item[6].Properties.IntValue('ID_H');
  Skin.ID.IDPos.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('IDB_L');
  Skin.ID.IDPos.Width:= OptionsData.Root.Items.Item[6].Properties.IntValue('IDB_W');
  Skin.ID.IDPos.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('IDB_T');
  Skin.ID.IDPos.Height:= OptionsData.Root.Items.Item[6].Properties.IntValue('IDB_H');

  Skin.Res.Resolution:= OptionsData.Root.Items.Item[6].Properties.Value('RS_TEXT');
  Skin.Res.ResolutionText.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS_L');
  Skin.Res.ResolutionText.Width:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS_W');
  Skin.Res.ResolutionText.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS_T');
  Skin.Res.ResolutionText.Height:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS_H');
  Skin.Res.ResOptions.Res1.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS1_L');
  Skin.Res.ResOptions.Res1.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS1_T');
  Skin.Res.ResOptions.Res2.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS2_L');
  Skin.Res.ResOptions.Res2.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS2_T');
  Skin.Res.ResOptions.Res3.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS3_L');
  Skin.Res.ResOptions.Res3.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS3_T');
  Skin.Res.ResOptions.Res4.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS4_L');
  Skin.Res.ResOptions.Res4.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('RS4_T');

  Skin.Sound.Sound:= OptionsData.Root.Items.Item[6].Properties.Value('SN_TEXT');
  Skin.Sound.SoundText.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN_L');
  Skin.Sound.SoundText.Width:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN_W');
  Skin.Sound.SoundText.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN_T');
  Skin.Sound.SoundText.Height:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN_H');
  Skin.Sound.SoundOptions.Sound.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN1_L');
  Skin.Sound.SoundOptions.Sound.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN1_T');
  Skin.Sound.SoundOptions.Music.Left:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN2_L');
  Skin.Sound.SoundOptions.Music.Top:= OptionsData.Root.Items.Item[6].Properties.IntValue('SN2_T');
end;

procedure TfrmLauncherMain.btnCloseClick(Sender: TObject);
begin
  Close;
end;

procedure TfrmLauncherMain.FormCreate(Sender: TObject);
var
   LauncherExe: TStream;
   OptionEncDec: TMemoryStream;
   Bmp: TBitmap;

   //   temp: TFileStream;
begin
  DoubleBuffered := True;
  
  LauncherExe:= TFileStream.Create(Application.ExeName, fmOpenRead + fmShareDenyNone);
  OptionEncDec:= TMemoryStream.Create;
  BackGroundImage:= TBitmap.Create;
  
  try
{   temp:= TFileStream.Create('temp.xml', fmCreate);
    try
      LauncherExe.Position:= ;
      temp.Position:= 0;             MagicNumber

      temp.CopyFrom(LauncherExe, LauncherExe.Size-MagicNumber);
    finally
      temp.Free;
    end;}

    try
     OptionEncDec.Position:= 0;
      LauncherExe.Position:= MagicNumber; //Set position to Original File Size

      OptionEncDec.CopyFrom(LauncherExe, LauncherExe.Size-MagicNumber); // Extract Encrypted options from file

      EncryptDecrypt(OptionEncDec); // Decrypt them
      UnpackData(OptionEncDec); // Uncompress them

      OptionEncDec.Position:=0;
      OptionsData.LoadFromStream(OptionEncDec); // Load them for parse}
    except
      Application.Terminate;
    end;
  finally
    LauncherExe.Free;
    OptionEncDec.Free;
  end;

//  OptionsData.LoadFromFile('Options.xml');
  try
    ParseOptions;
  except
    Application.Terminate;
  end;

  if Options.ServerName='' then Options.ServerName := 'MU Launcher';
  if Options.ServerPage='' then Options.ServerPage := 'SkyTeam';

  Application.Title := Format('%s - %s', [Options.ServerName, Options.ServerPage]);

  Color := Options.MColor;
  Font.Color:= Options.MFontColor;

  if Options.ShowBrowse then begin
    wbBrowser:= TWebBrowser.Create(Self);
    wbBrowser.ParentWindow:= Self.Handle;
    wbBrowser.Top:= Skin.Browser.Top;
    wbBrowser.Width:= Skin.Browser.Width;
    wbBrowser.Left:= Skin.Browser.Left;
    wbBrowser.Height:= Skin.Browser.Height;
    wbBrowser.Navigate(Options.BrowseURL);
    wbBrowser.Visible:= True;
  end;

  {shape the form}
  if not (Skin.MainRegion='EMPTY') then begin
    Bmp := TBitmap.Create;
    try
      GetImageStream(Skin.MainRegion, Bmp);

      FRegion := CreateRegion(Bmp);
      SetWindowRGN(Handle, FRegion, True);
    finally
      Bmp.Free;
    end;
  end;  

  GetImageStream(Skin.MainBackground, BackGroundImage);
  Width := BackGroundImage.Width;
  Height:= BackGroundImage.Height;

  Top := (Screen.Height-BackGroundImage.Height) div 2;
  Left:= (Screen.Width-BackGroundImage.Width) div 2;

  btnClose  := TRzBmpButton.CreateParented(Self.Handle);
  btnClose.Parent := Self;
  btnClose.OnClick:= btnCloseClick;
  btnClose.Left:= Skin.Buttons.Close.X;
  btnClose.Top := Skin.Buttons.Close.Y;
  GetImageStream(Skin.Buttons.Close.DataN, btnClose.Bitmaps.Up);
  GetImageStream(Skin.Buttons.Close.DataD, btnClose.Bitmaps.Down);

  btnConnect:= TRzBmpButton.CreateParented(Self.Handle);
  btnConnect.Parent := Self;
  btnConnect.OnClick:= btnConnectClick;
  btnConnect.Left:= Skin.Buttons.Connect.X;
  btnConnect.Top := Skin.Buttons.Connect.Y;
  GetImageStream(Skin.Buttons.Connect.DataN, btnConnect.Bitmaps.Up);
  GetImageStream(Skin.Buttons.Connect.DataD, btnConnect.Bitmaps.Down);
  
  if Options.ShowUpdate then begin
    btnUpdate := TRzBmpButton.CreateParented(Self.Handle);
    btnUpdate.Parent := Self;
    btnUpdate.OnClick:= btnUpdateClick;
    btnUpdate.Left:= Skin.Buttons.Update.X;
    btnUpdate.Top := Skin.Buttons.Update.Y;
    GetImageStream(Skin.Buttons.Update.DataN, btnUpdate.Bitmaps.Up);
    GetImageStream(Skin.Buttons.Update.DataD, btnUpdate.Bitmaps.Down);
  end;  

  if Options.ShowOption then begin
    btnOption:= TRzBmpButton.CreateParented(Self.Handle);
    btnOption.Parent := Self;
    btnOption.OnClick:= btnOptionClick;
    btnOption.Left:= Skin.Buttons.Option.X;
    btnOption.Top := Skin.Buttons.Option.Y;
    GetImageStream(Skin.Buttons.Option.DataN, btnOption.Bitmaps.Up);
    GetImageStream(Skin.Buttons.Option.DataD, btnOption.Bitmaps.Down);
  end;

  if Options.ShowStatus then begin
    lblStatus.Left:= Skin.ServerStatus.Left;
    lblStatus.Width:=Skin.ServerStatus.Width;
    lblStatus.Top:=Skin.ServerStatus.Top;
    lblStatus.Height:=Skin.ServerStatus.Height;
    lblStatus.Visible := True;
    TimerServerStatus.Enabled := True;    
  end;

end;

procedure TfrmLauncherMain.FormClose(Sender: TObject;
  var Action: TCloseAction);
begin
  if Options.ShowBrowse then begin
    wbBrowser.Free;
  end;

  btnClose.Free;
  btnConnect.Free;
  if Options.ShowUpdate then btnUpdate.Free;
  if Options.ShowOption then btnOption.Free;
  {Delete shape}
  if not (Skin.MainRegion='EMPTY') then begin
    DeleteObject(FRegion);
  end;
  BackGroundImage.Free;
end;

procedure TfrmLauncherMain.btnOptionClick(Sender: TObject);
var frmLauncherOptions: TfrmLauncherOptions;
begin
  frmLauncherOptions:= TfrmLauncherOptions.Create(Self);
  frmLauncherOptions.Color := Options.OColor;
  frmLauncherOptions.Font.Color:= Options.OFontColor;
  frmLauncherOptions.ShowModal;
  frmLauncherOptions.Free;
end;

procedure TfrmLauncherMain.btnUpdateClick(Sender: TObject);
var
  frmLauncherUpdate: TfrmLauncherUpdate;
begin
  frmLauncherUpdate:= TfrmLauncherUpdate.Create(Self);
  frmLauncherUpdate.UpdateDataUrl:= Options.UpdateData;
  frmLauncherUpdate.ShowModal;
  frmLauncherUpdate.Free;
end;

procedure TfrmLauncherMain.btnConnectClick(Sender: TObject);
var
  frmLauncherSplash: TfrmLauncherSplash;
  Bitmap: TBitmap;
begin

  if Options.ShowSplash then begin {Show splash screen}
    Hide;
    Bitmap:= TBitmap.Create;
    frmLauncherSplash:= TfrmLauncherSplash.Create(Self);
    try
      frmLauncherSplash.Timer1.Enabled:= True;
      frmLauncherSplash.ShowModal;
    finally
      Bitmap.Free;
      frmLauncherSplash.Free;
    end;
  end;

  ExecuteProcess(ExtractFilePath(Application.Exename)+'main.exe', Format('connect /u%s /p%s',[Options.ServerIP, Options.ServerPort]), SW_SHOWNORMAL);

  Close;                  
end;

procedure TfrmLauncherMain.FormPaint(Sender: TObject);
begin
  Canvas.Draw(0, 0, BackGroundImage); 
end;

procedure TfrmLauncherMain.FormMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
   if (Button = mbLeft) then
    begin
       Dragging := True;
       DragStart := TControl(Sender).ClientToScreen(Point(X,Y));
    end;
end;

procedure TfrmLauncherMain.FormMouseMove(Sender: TObject;
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

procedure TfrmLauncherMain.FormMouseUp(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
begin
  if (Button = mbLeft) then Dragging := false;
end;


procedure TfrmLauncherMain.TimerServerStatusTimer(Sender: TObject);
begin
  if CheckGameServer(Options.ServerIP, StrToInt(Options.ServerPort)) then begin
    lblStatus.Caption:= Format('Online - %s', [PingServer(Options.ServerIP)]);
  end else begin
    lblStatus.Caption:= 'Server is offline';
    TimerServerStatus.Enabled := False;  
  end;
end;

end.
