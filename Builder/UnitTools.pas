unit UnitTools;

interface

uses 
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms, Dialogs,
  StdCtrls, ExtCtrls, Buttons, ExtDlgs, ZLibEx, DIMimeStreams;

const
  XORCodes: array[0..6] of Byte = ($53, $6B, $79, $54, $65, $61, $6D); {SkyTeam}
  Option1 = '<?xml version="1.0" encoding="iso-8859-1"?><Options>';
  Option2 = '<GUI ShowBrowser="%d" ShowOption="%d" ShowUpdate="%d" ShowSplashScreen="%d" ShowServerStatus="%d"/>';
  Option3 = '<General BrowserURL="%s" BrowserLeft="%s" BrowserWidth="%s" BrowserTop="%s" BrowserHeight="%s" StatusLeft="%s" StatusWidth="%s" StatusTop="%s" StatusHeight="%s"/>';
  Option4 = '<SplashScreen Data="%s" Region="%s"/>';
  Option5 = '<Server ServerName="%s" ServerHostname="%s" ServerPort="%s" ServerPage="%s"/>';
  Option6 = '<Update UpdateData="%s"/>';
  Option7 = '<Colors MainColor="%d" MainFontColor="%d" OptionsColor="%d" OptionsFontColor="%d" />';
  Option8 = '<Skin Background="%s" MainRegion="%s" Options="%s" OptionsRegion="%s" CL_N="%s" CL_D="%s" CL_X="%s" CL_Y="%s" CN_N="%s" CN_D="%s" CN_X="%s" CN_Y="%s" UP_N="%s" UP_D="%s" UP_X="%s" UP_Y="%s" OP_N="%s" OP_D="%s" OP_X="%s" OP_Y="%s"' +
            ' CL2_N="%s" CL2_D="%s" CL2_X="%s" CL2_Y="%s" AP_N="%s" AP_D="%s" AP_X="%s" AP_Y="%s"' +
            ' ID_TEXT="%s" ID_L="%s" ID_W="%s" ID_T="%s" ID_H="%s" IDB_L="%s" IDB_W="%s" IDB_T="%s" IDB_H="%s"' +
            ' RS_TEXT="%s" RS_L="%s" RS_W="%s" RS_T="%s" RS_H="%s" RS1_L="%s" RS1_T="%s" RS2_L="%s" RS2_T="%s" RS3_L="%s" RS3_T="%s" RS4_L="%s" RS4_T="%s"' +
            ' SN_TEXT="%s" SN_L="%s" SN_W="%s" SN_T="%s" SN_H="%s" SN1_L="%s" SN1_T="%s" SN2_L="%s" SN2_T="%s"' +            
            ' />';
  Option9 = '</Options>';

type
  TOptionPosition = record
    Left: String;
    Top: String;
  end;

type
  TResolutionOptions = record
    Res1: TOptionPosition; {640x480}
    Res2: TOptionPosition; {800x600}
    Res3: TOptionPosition; {1024x768}
    Res4: TOptionPosition; {1280x1024}
  end;

type
  TSoundOptions = record
    Sound: TOptionPosition; {Sound on/off}
    Music: TOptionPosition; {Music on/off}
  end;

type
  TLabelPosition = record
    Left: String;
    Width: String;
    Top: String;
    Height: String;
  end;

type
  TIDOption = record
    ID: String;
    IDText: TLabelPosition;
    IDPos: TLabelPosition;
  end;

type
  TResOption = record
    Resolution: String;
    ResolutionText: TLabelPosition;
    ResOptions: TResolutionOptions;
  end;
  
type
  TSoundOption = record
    Sound: String;
    SoundText: TLabelPosition;
    SoundOptions: TSoundOptions;
  end;

type
  TSkinBrowser = record
    Left: String;
    Width: String;
    Top: String;
    Height: String;
  end;

type
  TSkinButton = record
    DataN: String;
    DataD: String;    
    X: String;
    Y: String;
  end;

type
  TSkinButtons = record
    Close: TSkinButton;
    Connect: TSkinButton;
    Update: TSkinButton;
    Option: TSkinButton;
    Close2: TSkinButton;
    Apply: TSkinButton;
  end;

type
  TSkin = record
    {All}
    Buttons: TSkinButtons;
    {Main}
    MainBackground: String;
    MainRegion: String;
    Browser: TSkinBrowser;
    ServerStatus: TLabelPosition;
    {Options}
    OptionBackground: String;
    OptionRegion: String;
    ID: TIDOption;
    Res: TResOption;
    Sound: TSoundOption;
  end;

type
  TOptionsData = record
    ShowBrowse: Boolean;
    ShowOption: Boolean;
    ShowUpdate: Boolean;
    ShowSplash: Boolean;
    ShowStatus: Boolean;
    MColor    : TColor;
    MFontColor: TColor;
    OColor    : TColor;
    OFontColor: TColor;    
    BrowseURL : String;
    SplashData: String;
    SplashReg : String;    
    ServerName: String;
    ServerIP  : String;
    ServerPort: String;
    ServerPage: String;
    UpdateData: String;
  end;
   
procedure EncryptDecrypt( FileIN, FileOUT: TMemoryStream);
procedure EncryptDecrypt2( Data: TStream );
procedure PackData(Data: TStream);
procedure UnpackData(Data: TStream);
function CreateImageData(Const ImageFile: TFileName): String;  
procedure FindImageFile(EditBox: TEdit);

implementation

procedure EncryptDecrypt( FileIN, FileOUT: TMemoryStream);
var
  CharByte, CharByte2: Byte;
  i : integer;
begin
  for i:=0 to FileIN.Size-1 do begin
    FileIN.Position := i;
    FileOUT.Position:= i;
    FileIN.Read(CharByte, 1);

    CharByte2:= CharByte xor XORCodes[i mod 7]; //XOR Each byte
    FileOUT.Write(CharByte2,1);
  end;
end;

procedure EncryptDecrypt2( Data: TStream );
var
  CharByte, CharByte2: Byte;
  i : integer;
begin
  for i:=0 to Data.Size-1 do begin
    Data.Position := i;
    Data.Read(CharByte, 1);

    CharByte2:= CharByte xor XORCodes[i mod 7]; //XOR Each byte

    Data.Position:= i;
    Data.Write(CharByte2,1);
  end;
end;

procedure PackData(Data: TStream);
var
  zData: TMemoryStream;
begin
  zData:= TMemoryStream.Create;
  try
    zData.Position:= 0;
    Data.Position:= 0;
    ZCompressStream(Data, zData, zcMax);

    zData.Position:= 0;
    Data.Position:= 0;
    Data.Size:= 0;
    
    Data.CopyFrom(zData, zData.Size);
  finally
    zData.Free;
  end;
end;

procedure UnpackData(Data: TStream);
var
  zData: TMemoryStream;
begin
  zData:= TMemoryStream.Create;
  try
    zData.Position:= 0;
    Data.Position:= 0;
    try
      ZDecompressStream(Data, zData);
    except
      ;
    end;

    zData.Position:= 0;
    Data.Position:= 0;
    Data.Size:= 0;
    
    Data.CopyFrom(zData, zData.Size);
  finally
    zData.Free;
  end;
end;

function CreateImageData(Const ImageFile: TFileName): String;
var
  Image, ImageData, ImageMIME: TStream;
begin
  Image:= TFileStream.Create(ImageFile, fmOpenRead);  
  ImageData:= TMemoryStream.Create;
  ImageMIME:= TStringStream.Create('');    
  try
    Image.Position    := 0;
    ImageData.Position:= 0;

    ImageData.CopyFrom(Image, Image.Size);
    PackData(ImageData); // Compress Image
//    EncryptDecrypt2(ImageData); // Encrypt Image

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

procedure FindImageFile(EditBox: TEdit);
var OpenPictureDialog: TOpenPictureDialog;
begin
  OpenPictureDialog:= TOpenPictureDialog.Create(nil);
  try
    if OpenPictureDialog.Execute then begin
      EditBox.Text:= OpenPictureDialog.FileName;
    end;
  finally
    OpenPictureDialog.Free;
  end;
end;

end.
