unit UnitSkinBuilder;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, Buttons, UnitTools, DIMimeStreams;

type
  TFrameSkinBuilder = class(TFrame)
    cbUseSkin: TCheckBox;
    Preview: TButton;
    gbOptionsWindow: TGroupBox;
    Label25: TLabel;
    eCL2_N: TEdit;
    sbCloseN2: TSpeedButton;
    eCL2_D: TEdit;
    sbCloseD2: TSpeedButton;
    Label30: TLabel;
    eCL2_X: TEdit;
    Label31: TLabel;
    eCL2_Y: TEdit;
    eAP_N: TEdit;
    sbApplyN: TSpeedButton;
    eAP_D: TEdit;
    sbApplyD: TSpeedButton;
    Label20: TLabel;
    eAP_X: TEdit;
    Label21: TLabel;
    eAP_Y: TEdit;
    Label12: TLabel;
    Label1: TLabel;
    Label3: TLabel;
    Label4: TLabel;
    Label5: TLabel;
    Label33: TLabel;
    eIDText: TEdit;
    Label36: TLabel;
    eIDTextLeft: TEdit;
    Label37: TLabel;
    eIDTextWidth: TEdit;
    Label38: TLabel;
    eIDTextTop: TEdit;
    Label39: TLabel;
    eIDTextHeight: TEdit;
    Label40: TLabel;
    eResText: TEdit;
    Label41: TLabel;
    eResTextLeft: TEdit;
    Label42: TLabel;
    eResTextWidth: TEdit;
    Label43: TLabel;
    eResTextTop: TEdit;
    Label44: TLabel;
    eResTextHeight: TEdit;
    Label45: TLabel;
    eSoundText: TEdit;
    Label46: TLabel;
    eSoundTextLeft: TEdit;
    Label47: TLabel;
    eSoundTextWidth: TEdit;
    Label48: TLabel;
    eSoundTextTop: TEdit;
    Label49: TLabel;
    eSoundTextHeight: TEdit;
    Label50: TLabel;
    Label51: TLabel;
    eIDBoxLeft: TEdit;
    Label52: TLabel;
    eIDBoxWidth: TEdit;
    Label53: TLabel;
    eIDBoxTop: TEdit;
    Label54: TLabel;
    eIDBoxHeight: TEdit;
    Label55: TLabel;
    Label56: TLabel;
    eRes1Left: TEdit;
    Label57: TLabel;
    eRes1Top: TEdit;
    Label58: TLabel;
    Label59: TLabel;
    eRes2Left: TEdit;
    Label60: TLabel;
    eRes2Top: TEdit;
    Label61: TLabel;
    Label62: TLabel;
    eRes3Left: TEdit;
    Label63: TLabel;
    eRes3Top: TEdit;
    Label64: TLabel;
    Label65: TLabel;
    eRes4Left: TEdit;
    Label66: TLabel;
    eRes4Top: TEdit;
    Label67: TLabel;
    Label68: TLabel;
    Label69: TLabel;
    eSoundTop: TEdit;
    eSoundLeft: TEdit;
    Label70: TLabel;
    Label71: TLabel;
    Label72: TLabel;
    eMusicTop: TEdit;
    eMusicLeft: TEdit;
    gbMainWindow: TGroupBox;
    Label2: TLabel;
    eMainBackground: TEdit;
    sbMainBackground: TSpeedButton;
    sbMainRegion: TSpeedButton;
    eMainWindowRegionBitmap: TEdit;
    Label34: TLabel;
    Label6: TLabel;
    eOptionBackground: TEdit;
    sbOptionsBackground: TSpeedButton;
    sbOptionsRegion: TSpeedButton;
    eOptionsWindowRegionBitmap: TEdit;
    Label35: TLabel;
    Label7: TLabel;
    eCL_N: TEdit;
    sbCloseD: TSpeedButton;
    eCL_D: TEdit;
    sbCloseN: TSpeedButton;
    Label13: TLabel;
    eCL_X: TEdit;
    Label8: TLabel;
    eCL_Y: TEdit;
    Label9: TLabel;
    eCN_N: TEdit;
    sbConnectN: TSpeedButton;
    eCN_D: TEdit;
    sbConnectD: TSpeedButton;
    Label14: TLabel;
    eCN_X: TEdit;
    Label15: TLabel;
    eCN_Y: TEdit;
    eUP_Y: TEdit;
    eUP_X: TEdit;
    Label17: TLabel;
    Label16: TLabel;
    Label18: TLabel;
    eOP_X: TEdit;
    Label19: TLabel;
    eOP_Y: TEdit;
    eOP_D: TEdit;
    sbOptionsD: TSpeedButton;
    eUP_D: TEdit;
    sbUpdateD: TSpeedButton;
    sbUpdateN: TSpeedButton;
    eUP_N: TEdit;
    eOP_N: TEdit;
    sbOptionsN: TSpeedButton;
    Label11: TLabel;
    Label10: TLabel;
    Label22: TLabel;
    Label23: TLabel;
    Label24: TLabel;
    Label26: TLabel;
    Label27: TLabel;
    Label28: TLabel;
    Label29: TLabel;
    Label74: TLabel;
    Label75: TLabel;
    cbMainColor: TColorBox;
    Label76: TLabel;
    cbMainFontColor: TColorBox;
    Label32: TLabel;
    cbOptionsColor: TColorBox;
    Label73: TLabel;
    cbOptionsFontColor: TColorBox;
    Label77: TLabel;
    eBrowserLeft: TEdit;
    Label78: TLabel;
    eBrowserWidth: TEdit;
    Label79: TLabel;
    eBrowserTop: TEdit;
    Label80: TLabel;
    eBrowserHeight: TEdit;
    Label81: TLabel;
    Label82: TLabel;
    Label83: TLabel;
    eStatusLeft: TEdit;
    Label84: TLabel;
    eStatusWidth: TEdit;
    Label85: TLabel;
    eStatusTop: TEdit;
    Label86: TLabel;
    eStatusHeight: TEdit;
    procedure sbMainBackgroundClick(Sender: TObject);
    procedure sbMainRegionClick(Sender: TObject);
    procedure sbOptionsBackgroundClick(Sender: TObject);
    procedure sbOptionsRegionClick(Sender: TObject);
    procedure sbCloseNClick(Sender: TObject);
    procedure sbCloseDClick(Sender: TObject);
    procedure sbConnectNClick(Sender: TObject);
    procedure sbConnectDClick(Sender: TObject);
    procedure sbUpdateNClick(Sender: TObject);
    procedure sbUpdateDClick(Sender: TObject);
    procedure sbOptionsNClick(Sender: TObject);
    procedure sbOptionsDClick(Sender: TObject);
    procedure sbCloseN2Click(Sender: TObject);
    procedure sbCloseD2Click(Sender: TObject);
    procedure sbApplyNClick(Sender: TObject);
    procedure sbApplyDClick(Sender: TObject);
    procedure cbUseSkinClick(Sender: TObject);
  private
    Skin: TSkin;
  public
    function BuildSkin: String;
  end;

implementation

uses UnitMain;

{$R *.dfm}

function TFrameSkinBuilder.BuildSkin: String;
function GetImageData(Const ResName: String): String;
var
  Image, ImageData, ImageMIME: TStream;
begin
  Image:= TResourceStream.Create(hInstance, ResName, Pchar('DATA'));
  ImageData:= TMemoryStream.Create;
  ImageMIME:= TStringStream.Create('');
  try
    Image.Position    := 0;
    ImageData.Position:= 0;

    ImageData.CopyFrom(Image, Image.Size);
    EncryptDecrypt2(ImageData); // Decrypt Image

    ImageData.Position := 0;
    ImageMIME.Position := 0;    
    MimeEncodeStreamNoCRLF(ImageData, ImageMIME); // Encode image data

    Result:= TStringStream(ImageMIME).DataString;
  finally
    Image.Free;
    ImageData.Free;
    ImageMIME.Free;    
  end;
end;

begin
  frmMain.Options.MColor:= cbMainColor.DefaultColorColor;
  frmMain.Options.MFontColor:= cbMainFontColor.DefaultColorColor;
  frmMain.Options.OColor:= cbOptionsColor.DefaultColorColor;
  frmMain.Options.OFontColor:= cbOptionsFontColor.DefaultColorColor;

 if cbUseSkin.Checked then begin
   Skin.MainBackground  := CreateImageData(eMainBackground.Text);
   if eMainWindowRegionBitmap.Text='' then begin
     Skin.MainRegion:= 'EMPTY';
   end else begin
     Skin.MainRegion:= CreateImageData(eMainWindowRegionBitmap.Text);
   end;

   Skin.OptionBackground:= CreateImageData(eOptionBackground.Text);
   if eOptionsWindowRegionBitmap.Text='' then begin
     Skin.OptionRegion:= 'EMPTY';
   end else begin
     Skin.OptionRegion:= CreateImageData(eOptionsWindowRegionBitmap.Text);
   end;

   frmMain.Options.MColor := cbMainColor.Selected;
   frmMain.Options.MFontColor := cbMainFontColor.Selected;
   frmMain.Options.OColor := cbOptionsColor.Selected;
   frmMain.Options.OFontColor := cbOptionsFontColor.Selected;

   frmMain.Skin.Browser.Left  := eBrowserLeft.Text;
   frmMain.Skin.Browser.Width := eBrowserWidth.Text;
   frmMain.Skin.Browser.Top   := eBrowserTop.Text;
   frmMain.Skin.Browser.Height:= eBrowserHeight.Text;

   frmMain.Skin.ServerStatus.Left  := eStatusLeft.Text;
   frmMain.Skin.ServerStatus.Width := eStatusWidth.Text;
   frmMain.Skin.ServerStatus.Top   := eStatusTop.Text;
   frmMain.Skin.ServerStatus.Height:= eStatusHeight.Text;

   Skin.Buttons.Close.DataN:= CreateImageData(eCL_N.Text);
   Skin.Buttons.Close.DataD:= CreateImageData(eCL_D.Text);
   Skin.Buttons.Close.X    := eCL_X.Text;
   Skin.Buttons.Close.Y    := eCL_Y.Text;

   Skin.Buttons.Connect.DataN:= CreateImageData(eCN_N.Text);
   Skin.Buttons.Connect.DataD:= CreateImageData(eCN_D.Text);
   Skin.Buttons.Connect.X    := eCN_X.Text;
   Skin.Buttons.Connect.Y    := eCN_Y.Text;

   Skin.Buttons.Update.DataN:= CreateImageData(eUP_N.Text);
   Skin.Buttons.Update.DataD:= CreateImageData(eUP_D.Text);
   Skin.Buttons.Update.X    := eUP_X.Text;
   Skin.Buttons.Update.Y    := eUP_Y.Text;

   Skin.Buttons.Option.DataN:= CreateImageData(eOP_N.Text);
   Skin.Buttons.Option.DataD:= CreateImageData(eOP_D.Text);
   Skin.Buttons.Option.X    := eOP_X.Text;
   Skin.Buttons.Option.Y    := eOP_Y.Text;

   Skin.Buttons.Close2.DataN:= CreateImageData(eCL2_N.Text);
   Skin.Buttons.Close2.DataD:= CreateImageData(eCL2_D.Text);
   Skin.Buttons.Close2.X    := eCL2_X.Text;
   Skin.Buttons.Close2.Y    := eCL2_Y.Text;

   Skin.Buttons.Apply.DataN:= CreateImageData(eAP_N.Text);
   Skin.Buttons.Apply.DataD:= CreateImageData(eAP_D.Text);
   Skin.Buttons.Apply.X    := eAP_X.Text;
   Skin.Buttons.Apply.Y    := eAP_Y.Text;

   Skin.ID.ID := eIDText.Text;
   Skin.ID.IDText.Left  := eIDTextLeft.Text;
   Skin.ID.IDText.Width := eIDTextWidth.Text;
   Skin.ID.IDText.Top   := eIDTextTop.Text;
   Skin.ID.IDText.Height:= eIDTextHeight.Text;

   Skin.ID.IDPos.Left  := eIDBoxLeft.Text;
   Skin.ID.IDPos.Width := eIDBoxWidth.Text;
   Skin.ID.IDPos.Top   := eIDBoxTop.Text;
   Skin.ID.IDPos.Height:= eIDBoxHeight.Text;

   Skin.Res.Resolution           := eResText.Text;
   Skin.Res.ResolutionText.Left  := eResTextLeft.Text;
   Skin.Res.ResolutionText.Width := eResTextWidth.Text;
   Skin.Res.ResolutionText.Top   := eResTextTop.Text;
   Skin.Res.ResolutionText.Height:= eResTextHeight.Text;
   Skin.Res.ResOptions.Res1.Left := eRes1Left.Text;
   Skin.Res.ResOptions.Res1.Top  := eRes1Top.Text;
   Skin.Res.ResOptions.Res2.Left := eRes2Left.Text;
   Skin.Res.ResOptions.Res2.Top  := eRes2Top.Text;
   Skin.Res.ResOptions.Res3.Left := eRes3Left.Text;
   Skin.Res.ResOptions.Res3.Top  := eRes3Top.Text;
   Skin.Res.ResOptions.Res4.Left := eRes4Left.Text;
   Skin.Res.ResOptions.Res4.Top  := eRes4Top.Text;

   Skin.Sound.Sound    := eSoundText.Text;
   Skin.Sound.SoundText.Left  := eSoundTextLeft.Text;
   Skin.Sound.SoundText.Width := eSoundTextWidth.Text;
   Skin.Sound.SoundText.Top   := eSoundTextTop.Text;
   Skin.Sound.SoundText.Height:= eSoundTextHeight.Text;
   Skin.Sound.SoundOptions.Sound.Left := eSoundLeft.Text;
   Skin.Sound.SoundOptions.Sound.Top  := eSoundTop.Text;
   Skin.Sound.SoundOptions.Music.Left := eMusicLeft.Text;
   Skin.Sound.SoundOptions.Music.Top  := eMusicTop.Text;


   Skin.ID.ID := eIDText.Text;
   Skin.ID.IDText.Left  := eIDTextLeft.Text;
   Skin.ID.IDText.Width := eIDTextWidth.Text;
   Skin.ID.IDText.Top   := eIDTextTop.Text;
   Skin.ID.IDText.Height:= eIDTextHeight.Text;
   Skin.ID.IDPos.Left  := eIDBoxLeft.Text;
   Skin.ID.IDPos.Width := eIDBoxWidth.Text;
   Skin.ID.IDPos.Top   := eIDBoxTop.Text;
   Skin.ID.IDPos.Height:= eIDBoxHeight.Text;

   Skin.Res.Resolution:= eResText.Text;
   Skin.Res.ResolutionText.Left  := eResTextLeft.Text;
   Skin.Res.ResolutionText.Width := eResTextWidth.Text;
   Skin.Res.ResolutionText.Top   := eResTextTop.Text;
   Skin.Res.ResolutionText.Height:= eResTextHeight.Text;
   Skin.Res.ResOptions.Res1.Left:= eRes1Left.Text;
   Skin.Res.ResOptions.Res1.Top := eRes1Top.Text;
   Skin.Res.ResOptions.Res2.Left:= eRes2Left.Text;
   Skin.Res.ResOptions.Res2.Top := eRes2Top.Text;
   Skin.Res.ResOptions.Res3.Left:= eRes3Left.Text;
   Skin.Res.ResOptions.Res3.Top := eRes3Top.Text;
   Skin.Res.ResOptions.Res4.Left:= eRes4Left.Text;
   Skin.Res.ResOptions.Res4.Top := eRes4Top.Text;


   Skin.Sound.Sound:= eSoundText.Text;
   Skin.Sound.SoundText.Left  := eSoundTextLeft.Text;
   Skin.Sound.SoundText.Width := eSoundTextWidth.Text;
   Skin.Sound.SoundText.Top   := eSoundTextTop.Text;
   Skin.Sound.SoundText.Height:= eSoundTextHeight.Text;
   Skin.Sound.SoundOptions.Sound.Left:= eSoundLeft.Text;
   Skin.Sound.SoundOptions.Sound.Top:= eSoundTop.Text;
   Skin.Sound.SoundOptions.Music.Left:= eMusicLeft.Text;
   Skin.Sound.SoundOptions.Music.Top:= eMusicTop.Text;

  end else begin   {Restore default skin}
    frmMain.Skin.Browser.Left  := '8';
    frmMain.Skin.Browser.Width := '432';
    frmMain.Skin.Browser.Top   := '33';
    frmMain.Skin.Browser.Height:= '324';

    frmMain.Skin.ServerStatus.Left  := '8';
    frmMain.Skin.ServerStatus.Width := '425';
    frmMain.Skin.ServerStatus.Top   := '360';
    frmMain.Skin.ServerStatus.Height:= '8';

    Skin.ID.ID := 'Enter your account ID to have it automatically entered upon login.';
    Skin.ID.IDText.Left  := '16';
    Skin.ID.IDText.Width := '417';
    Skin.ID.IDText.Top   := '40';
    Skin.ID.IDText.Height:= '41';
    Skin.ID.IDPos.Left  := '160';
    Skin.ID.IDPos.Width := '121';
    Skin.ID.IDPos.Top   := '88';
    Skin.ID.IDPos.Height:= '21';

    Skin.Res.Resolution:= 'Select the resolution rate. Higher resolutions may enable clearer graphics, but may require higher system specifications.';
    Skin.Res.ResolutionText.Left  := '16';
    Skin.Res.ResolutionText.Width := '417';
    Skin.Res.ResolutionText.Top   := '152';
    Skin.Res.ResolutionText.Height:= '41';
    Skin.Res.ResOptions.Res1.Left:= '16';
    Skin.Res.ResOptions.Res1.Top := '200';
    Skin.Res.ResOptions.Res2.Left:= '120';
    Skin.Res.ResOptions.Res2.Top := '200';
    Skin.Res.ResOptions.Res3.Left:= '224';
    Skin.Res.ResOptions.Res3.Top := '200';
    Skin.Res.ResOptions.Res4.Left:= '344';
    Skin.Res.ResOptions.Res4.Top := '200';


    Skin.Sound.Sound:= 'Select to enable or disable sound. Selecting option will enable sound.';
    Skin.Sound.SoundText.Left  := '16';
    Skin.Sound.SoundText.Width := '417';
    Skin.Sound.SoundText.Top   := '264';
    Skin.Sound.SoundText.Height:= '41';
    Skin.Sound.SoundOptions.Sound.Left:= '56';
    Skin.Sound.SoundOptions.Sound.Top:= '312';
    Skin.Sound.SoundOptions.Music.Left:= '224';
    Skin.Sound.SoundOptions.Music.Top:= '312';

    with Skin do begin
      MainBackground:= GetImageData('BMAIN');
      MainRegion:= 'EMPTY';
      OptionBackground:= GetImageData('BOPTIONS');
      OptionRegion:= 'EMPTY';
      
      Buttons.Close.DataN:= GetImageData('CANCEL1');
      Buttons.Close.DataD:= GetImageData('CANCEL2');
      Buttons.Close.X:= '306';
      Buttons.Close.Y:= '368';
      Buttons.Connect.DataN:= GetImageData('CONNECT1');
      Buttons.Connect.DataD:= GetImageData('CONNECT2');
      Buttons.Connect.X:=  '219';
      Buttons.Connect.Y:=  '368';
      Buttons.Update.DataN:= GetImageData('UPDATE1');
      Buttons.Update.DataD:= GetImageData('UPDATE2');
      Buttons.Update.X:= '132';
      Buttons.Update.Y:= '368';
      Buttons.Option.DataN:= GetImageData('OPTION1');
      Buttons.Option.DataD:= GetImageData('OPTION2');
      Buttons.Option.X:= '45';
      Buttons.Option.Y:= '368';
      Buttons.Close2.DataN:= GetImageData('CANCEL1');
      Buttons.Close2.DataD:= GetImageData('CANCEL2');
      Buttons.Close2.X:=  '219';
      Buttons.Close2.Y:=  '368';
      Buttons.Apply.DataN:= GetImageData('APPLY1');
      Buttons.Apply.DataD:= GetImageData('APPLY2');
      Buttons.Apply.X:= '132';
      Buttons.Apply.Y:= '368';
    end;    // with
  end;

  with Skin do begin
     Result:= Format(Option8, [MainBackground, MainRegion, OptionBackground, OptionRegion,
      Buttons.Close.DataN, Buttons.Close.DataD, Buttons.Close.X, Buttons.Close.Y,
      Buttons.Connect.DataN, Buttons.Connect.DataD, Buttons.Connect.X, Buttons.Connect.Y,
      Buttons.Update.DataN, Buttons.Update.DataD, Buttons.Update.X, Buttons.Update.Y,
      Buttons.Option.DataN, Buttons.Option.DataD, Buttons.Option.X, Buttons.Option.Y,
      Buttons.Close2.DataN, Buttons.Close2.DataD, Buttons.Close2.X, Buttons.Close2.Y,
      Buttons.Apply.DataN, Buttons.Apply.DataD, Buttons.Apply.X, Buttons.Apply.Y,
      ID.ID, ID.IDText.Left, ID.IDText.Width, ID.IDText.Top, ID.IDText.Height, ID.IDPos.Left, ID.IDPos.Width, ID.IDPos.Top, ID.IDPos.Height,
      Res.Resolution, Res.ResolutionText.Left,Res.ResolutionText.Width,Res.ResolutionText.Top,Res.ResolutionText.Height,Res.ResOptions.Res1.Left,Res.ResOptions.Res1.Top,Res.ResOptions.Res2.Left,Res.ResOptions.Res2.Top,Res.ResOptions.Res3.Left,Res.ResOptions.Res3.Top,Res.ResOptions.Res4.Left,Res.ResOptions.Res4.Top,
      Sound.Sound,Sound.SoundText.Left,Sound.SoundText.Width,Sound.SoundText.Top,Sound.SoundText.Height,Sound.SoundOptions.Sound.Left,Sound.SoundOptions.Sound.Top,Sound.SoundOptions.Music.Left,Sound.SoundOptions.Music.Top
     ]);
  end;    // with  
end;

procedure TFrameSkinBuilder.sbMainBackgroundClick(Sender: TObject);
begin
  FindImageFile(eMainBackground);
end;

procedure TFrameSkinBuilder.sbMainRegionClick(Sender: TObject);
begin
  FindImageFile(eMainWindowRegionBitmap);
end;

procedure TFrameSkinBuilder.sbOptionsBackgroundClick(Sender: TObject);
begin
  FindImageFile(eOptionBackground);
end;

procedure TFrameSkinBuilder.sbOptionsRegionClick(Sender: TObject);
begin
  FindImageFile(eOptionsWindowRegionBitmap);
end;

procedure TFrameSkinBuilder.sbCloseNClick(Sender: TObject);
begin
  FindImageFile(eCL_N);
end;

procedure TFrameSkinBuilder.sbCloseDClick(Sender: TObject);
begin
  FindImageFile(eCL_D);
end;

procedure TFrameSkinBuilder.sbConnectNClick(Sender: TObject);
begin
  FindImageFile(eCN_N);
end;

procedure TFrameSkinBuilder.sbConnectDClick(Sender: TObject);
begin
  FindImageFile(eCN_D);
end;

procedure TFrameSkinBuilder.sbUpdateNClick(Sender: TObject);
begin
  FindImageFile(eUP_N);
end;

procedure TFrameSkinBuilder.sbUpdateDClick(Sender: TObject);
begin
  FindImageFile(eUP_D);
end;

procedure TFrameSkinBuilder.sbOptionsNClick(Sender: TObject);
begin
  FindImageFile(eOP_N);
end;

procedure TFrameSkinBuilder.sbOptionsDClick(Sender: TObject);
begin
  FindImageFile(eOP_D);
end;

procedure TFrameSkinBuilder.sbCloseN2Click(Sender: TObject);
begin
  FindImageFile(eCL2_N);
end;

procedure TFrameSkinBuilder.sbCloseD2Click(Sender: TObject);
begin
  FindImageFile(eCL2_D);
end;

procedure TFrameSkinBuilder.sbApplyNClick(Sender: TObject);
begin
  FindImageFile(eAP_N);
end;

procedure TFrameSkinBuilder.sbApplyDClick(Sender: TObject);
begin
  FindImageFile(eAP_D);
end;

procedure TFrameSkinBuilder.cbUseSkinClick(Sender: TObject);
begin
  if cbUseSkin.Checked then begin
    gbMainWindow.Enabled := True;
    gbOptionsWindow.Enabled:= True;
  end else begin
    gbMainWindow.Enabled := False;
    gbOptionsWindow.Enabled:= False;  
  end;
    
end;

end.
