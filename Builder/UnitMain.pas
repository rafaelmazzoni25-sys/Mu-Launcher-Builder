unit UnitMain;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Buttons, ExtDlgs, JclSysUtils, JclFileUtils,
  DIMimeStreams, ExtCtrls, ZLibEx, ComCtrls, UnitTools, RzButton, RzRadChk,
  ScrollView, PropertiesView, PropertyItems, Menus, ActnList, ToolWin,
  ImgList, XPManifestFile;

const
  v_cAnnouncementVis = 1;
  v_tAnnouncementUrl = 2;
  v_cOptionsEnable   = 4;
  v_cServerStatusVis = 6;
  v_cUpdateEnable    = 8;
  v_cUpdateUrl       = 9;
  v_cSplashEnable    =11;
  v_cSplashBack      =12;
  v_cSplashRegion    =13;
  v_tServerName      =15;
  v_tServerPage      =16;
  v_tServerHost      =17;
  v_tServerPort      =18;

type
  TfrmMain = class(TForm)
    PageControl1: TPageControl;
    TabSheet1: TTabSheet;
    tsSkin: TTabSheet;
    GeneralProperties: TPropertiesView;
    SkinProperties: TPropertiesView;
    MainMenu1: TMainMenu;
    File1: TMenuItem;
    Exit1: TMenuItem;
    Run1: TMenuItem;
    Run2: TMenuItem;
    Build1: TMenuItem;
    Help1: TMenuItem;
    About1: TMenuItem;
    ActionList1: TActionList;
    ActionBuild: TAction;
    ActionExit: TAction;
    ActionAbout: TAction;
    ActionRun: TAction;
    ToolBar1: TToolBar;
    ToolButton1: TToolButton;
    ToolButton2: TToolButton;
    ImageList1: TImageList;
    StatusBar1: TStatusBar;
    XPManifestFile1: TXPManifestFile;
    Skin1: TMenuItem;
    Edit1: TMenuItem;
    MainWindow1: TMenuItem;
    OptionsWindow1: TMenuItem;
    ActionEditMainWindowSkin: TAction;
    ActionEditOptionsWindowSkin: TAction;
    PopupMenu1: TPopupMenu;
    MainWindow2: TMenuItem;
    OptionsWindow2: TMenuItem;
    procedure btnCreateUpdatePackageClick(Sender: TObject);
    procedure FormCreate(Sender: TObject);
    procedure FormClose(Sender: TObject; var Action: TCloseAction);
    procedure ActionExitExecute(Sender: TObject);
    procedure ActionAboutExecute(Sender: TObject);
    procedure ActionBuildExecute(Sender: TObject);
    procedure ActionEditMainWindowSkinExecute(Sender: TObject);
    procedure ToolButton1Click(Sender: TObject);
  private
    SkinFrame: TFrame;
    //TGeneralOptions
  public
    Opt1, Opt2, Opt3, Opt4, Opt5, Opt6, Opt7, Opt8, Opt9: String;
    Options: TOptionsData;
    Skin: TSkin;    
  end;

var
  frmMain: TfrmMain;

implementation

uses UnitUpdateBuilder, UnitSkinBuilder, UnitSkinEditor;

{$R *.dfm}

function ValueAsBoolean( Const Value: WideString): Boolean;
begin
  Result:= False;

  if Value = 'Yes' then Result:= True;
  if Value = 'No'  then Result:= False;
end;

procedure TfrmMain.btnCreateUpdatePackageClick(Sender: TObject);
var frmUpdateBuilder: TfrmUpdateBuilder;
begin
  frmUpdateBuilder:= TfrmUpdateBuilder.Create(Self);
  frmUpdateBuilder.ShowModal;
  frmUpdateBuilder.Free;
end;

procedure TfrmMain.FormCreate(Sender: TObject);
begin
  SkinFrame:= TFrameSkinBuilder.Create(Self);
  //SkinFrame.Parent:= tsSkin;
  //SkinFrame.Align := alClient;

  Options.ShowBrowse:= False;
  Options.ShowOption:= False;
  Options.ShowUpdate:= False;
  Options.ShowSplash:= False;
  Options.ShowStatus:= False;
  Options.BrowseURL := '';
  Options.SplashData:= '';
  Options.SplashReg := 'EMPTY';
  Options.ServerName:= '';
  Options.ServerIP  := '';
  Options.ServerPort:= '44405';
  Options.ServerPage:= '';
  Options.UpdateData:= '';
end;

procedure TfrmMain.FormClose(Sender: TObject; var Action: TCloseAction);
begin
  SkinFrame.Free;
end;

procedure TfrmMain.ActionExitExecute(Sender: TObject);
begin
  Close;
end;

procedure TfrmMain.ActionAboutExecute(Sender: TObject);
var VersionInfo: TJclFileVersionInfo;
begin
  VersionInfo:= TJclFileVersionInfo.Create( Application.ExeName );
  try
    Application.MessageBox( PChar(Format( 'MU Launcher Builder %s', [VersionInfo.FileVersion] ) + #13#10#13#10 +
                         'Visit http://sky.xn.pl/ for info and support' + #13#10 +
                         'Copyright 2004-2005 BlurCode - SkyTeam'), 'About...', MB_ICONINFORMATION+MB_OK);
  finally
   VersionInfo.Free;
  end;   
end;

procedure TfrmMain.ActionBuildExecute(Sender: TObject);
var
   LauncherDataEnc: TStream;
   LauncherDataDec: TMemoryStream;
   OptionData, OptionDataS: TStream;

   FileOut, FileTemp: TStream;
   OptionString: String;

   SplashBackground, SplashRegion: TFileName;
begin
  Options.ShowBrowse:= ValueAsBoolean(GeneralProperties.FindItem(v_cAnnouncementVis).Value);
  Options.BrowseURL := GeneralProperties.FindItem(v_tAnnouncementUrl).Value;
  Options.ShowOption:= ValueAsBoolean(GeneralProperties.FindItem(v_cOptionsEnable).Value);
  Options.ShowStatus:= ValueAsBoolean(GeneralProperties.FindItem(v_cServerStatusVis).Value);
  Options.ShowUpdate:= ValueAsBoolean(  GeneralProperties.FindItem(v_cUpdateEnable).Value );
  Options.UpdateData:= GeneralProperties.FindItem(v_cUpdateUrl).Value;
  Options.ShowSplash:= ValueAsBoolean(GeneralProperties.FindItem(v_cSplashEnable).Value);

  if Options.ShowSplash then begin

    SplashBackground  := GeneralProperties.FindItem(v_cSplashBack).Value;
    if not (SplashBackground='') then begin
      Options.SplashData:= CreateImageData(SplashBackground);
    end else begin
      ShowMessage('You must specify at least the splash background.');
      Exit;
    end;

    SplashRegion := GeneralProperties.FindItem(v_cSplashRegion).Value;
    if not (SplashRegion='') then begin
      Options.SplashReg:= CreateImageData(SplashRegion);
    end else begin
      Options.SplashReg := 'EMPTY';
    end;

  end else begin
    Options.SplashData:= '';
    Options.SplashReg := 'EMPTY';
  end;

  Options.ServerName:= GeneralProperties.FindItem(v_tServerName).Value;
  Options.ServerPage:= GeneralProperties.FindItem(v_tServerPage).Value;
  Options.ServerIP  := GeneralProperties.FindItem(v_tServerHost).Value;
  Options.ServerPort:= GeneralProperties.FindItem(v_tServerPort).Value;


  Opt8:= TFrameSkinBuilder(SkinFrame).BuildSkin; {Get skin data first}

  Opt1:= Option1;
  Opt2:= Format(Option2, [BoolToInt(Options.ShowBrowse),BoolToInt(Options.ShowOption),BoolToInt(Options.ShowUpdate),BoolToInt(Options.ShowSplash), BoolToInt(Options.ShowStatus) ]);
  Opt3:= Format(Option3, [Options.BrowseURL, Skin.Browser.Left,Skin.Browser.Width,Skin.Browser.Top,Skin.Browser.Height, Skin.ServerStatus.Left,Skin.ServerStatus.Width,Skin.ServerStatus.Top,Skin.ServerStatus.Height]);
  Opt4:= Format(Option4, [Options.SplashData, Options.SplashReg]);
  Opt5:= Format(Option5, [Options.ServerName, Options.ServerIP, Options.ServerPort, Options.ServerPage]);
  Opt6:= Format(Option6, [Options.UpdateData]);
  Opt7:= Format(Option7, [Options.MColor, Options.MFontColor, Options.OColor, Options.OFontColor]);
  Opt9:= Option9;

  OptionString:= Format('%s%s%s%s%s%s%s%s%s', [Opt1, Opt2, Opt3, Opt4, Opt5, Opt6, Opt7, Opt8, Opt9]);

  OptionData:= TMemoryStream.Create;
  OptionDataS:= TStringStream.Create( OptionString );
  
  LauncherDataEnc:= TResourceStream.Create(hInstance, 'LAUNCHER', Pchar('DATA'));
  LauncherDataDec:= TMemoryStream.Create;
  FileOut:= TFileStream.Create(ExtractFilePath(Application.ExeName) + 'Launcher.exe', fmCreate);
  try
    LauncherDataEnc.Position := 0;
    LauncherDataDec.Position :=0;
    LauncherDataDec.CopyFrom(LauncherDataEnc, LauncherDataEnc.Size);
    
    {Decrypt Launcher}
    EncryptDecrypt2(LauncherDataDec);
    {Unpack Launcher}
    UnpackData(LauncherDataDec);

    OptionData.Position := 0;
    OptionDataS.Position:= 0;
    OptionData.CopyFrom(OptionDataS, OptionDataS.Size);

    OptionData.Position   := 0;

    FileTemp:= TFileStream.Create('c:\test.bin', fmCreate);
    try
      FileTemp.Position := 0;
      FileTemp.CopyFrom(OptionData, OptionData.Size);
    finally
      FileTemp.Free;
    end;    

    {Pack Options}
    PackData(OptionData);
    {Encrypt Options}
    EncryptDecrypt2(OptionData);


    FileOut.Position        := 0;
    LauncherDataDec.Position:= 0;
    OptionData.Position:= 0;
    {Copy Launcher+Options to Launcher.exe}
    FileOut.CopyFrom(LauncherDataDec, LauncherDataDec.Size);
    FileOut.CopyFrom(OptionData, OptionData.Size);
  finally
    LauncherDataEnc.Free;
    LauncherDataDec.Free;
    OptionData.Free;
    OptionDataS.Free;
    FileOut.Free;
  end;

  Application.MessageBox('Create of Launcher finished. You can now release the file Launcher.exe', 'Finished', MB_ICONINFORMATION+MB_OK);

end;

procedure TfrmMain.ActionEditMainWindowSkinExecute(Sender: TObject);
begin
  frmSkinEditor.ShowModal;
end;

procedure TfrmMain.ToolButton1Click(Sender: TObject);
begin
  frmSkinEditor.ShowModal;
end;

end.
