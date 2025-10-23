unit UnitLauncherRecords;

interface

uses SysUtils, Windows, Classes, Graphics, ZLibEx, DIMimeStreams, JclStrings;

const
  MagicNumber = 289792; {Size of Executable}
  XORCodes: array[0..6] of Byte = ($53, $6B, $79, $54, $65, $61, $6D); {SkyTeam}
  WebzenKEY = '\Software\Webzen\Mu\Config';

type
  TOptionPosition = record
    Left: Integer;
    Top: Integer;
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
    Left: Integer;
    Width: Integer;
    Top: Integer;
    Height: Integer;
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
    Left: Integer;
    Width: Integer;
    Top: Integer;
    Height: Integer;
  end;

type
  TSkinButton = record
    DataN: String;
    DataD: String;    
    X: Integer;
    Y: Integer;
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

type
  TRGBArray = array[0..32767] of TRGBTriple;
  PRGBArray = ^TRGBArray;
  
function BuildUrl(Const UpdateUrl, FileName: String): String;
procedure EncryptDecrypt( Data: TStream);
procedure GetImageStream(ImageData: String; Bitmap: TBitmap);
procedure UnpackData(Data: TStream);
function CreateRegion(Bmp: TBitmap): THandle;

implementation

function BuildUrl(Const UpdateUrl, FileName: String): String;
begin
  Result:= UpdateUrl + LowerCase(StrReplaceChar(FileName, '\', '/'));
end;

procedure EncryptDecrypt( Data: TStream);
var
  CharByte, CharByte2: Byte;
  i : integer;
begin
  for i:=0 to Data.Size-1 do begin
    Data.Position := i;
    Data.Read(CharByte, 1);
    Data.Position := i;

    CharByte2:= CharByte xor XORCodes[i mod 7]; //XOR Each byte
    
    Data.Write(CharByte2,1);
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
    Data.Position:= 0;
  finally
    zData.Free;
  end;
end;

procedure GetImageStream(ImageData: String; Bitmap: TBitmap);
var
  DataS, DataOut: TStream;
  Data: TStream;
begin

  DataS := TStringStream.Create(ImageData);
  DataOut:= TMemoryStream.Create;
  Data:= TMemoryStream.Create;
  try
    DataS.Position:= 0;
    DataOut.Position:=0;
    DataOut.CopyFrom(DataS, DataS.Size);
    
    Data.Position:= 0;
    DataOut.Position:= 0;
    MimeDecodeStream(DataOut, Data); //Decode
    {Flip Streams}
    DataOut.Position:= 0;
    DataOut.Size := 0;
    Data.Position:= 0;
    DataOut.CopyFrom(Data, Data.Size);

    DataOut.Position:= 0;
    Data.Position:= 0;
    Data.Size := 0;    
    Data.CopyFrom(DataOut, DataOut.Size);    

    UnpackData(Data); //Uncompress
    
    Bitmap.LoadFromStream(Data);
  finally
    DataS.Free;
    DataOut.Free;
    Data.Free;
  end;
end;

function CreateRegion(Bmp: TBitmap): THandle;
var
  X, Y, StartX: Integer;
  Excl: THandle;
  Row: PRGBArray;
  TransparentColor: TRGBTriple;
begin
  Bmp.PixelFormat := pf24Bit;

  Result := CreateRectRGN(0, 0, Bmp.Width, Bmp.Height);

  for Y := 0 to Bmp.Height - 1 do
  begin
    Row := Bmp.Scanline[Y];

    StartX := -1;

    if Y = 0 then
    begin
      TransparentColor := Row[0];
    end;

    for X := 0 to Bmp.Width - 1 do
    begin
      if (Row[X].rgbtRed = TransparentColor.rgbtRed) and
         (Row[X].rgbtGreen = TransparentColor.rgbtGreen) and
         (Row[X].rgbtBlue = TransparentColor.rgbtBlue) then
      begin
        if StartX = -1 then StartX := X;
      end else
      begin
        if StartX > -1 then
        begin
          Excl := CreateRectRGN(StartX, Y, X + 1, Y + 1);
          try
            CombineRGN(Result, Result, Excl, RGN_DIFF);
            StartX := -1;
          finally
            DeleteObject(Excl);
          end;
        end;
      end;
    end;

    if StartX > -1 then
    begin
      Excl := CreateRectRGN(StartX, Y, Bmp.Width, Y + 1);
      try
        CombineRGN(Result, Result, Excl, RGN_DIFF);
      finally
        DeleteObject(Excl);
      end;
    end;
  end;
end;


end.
