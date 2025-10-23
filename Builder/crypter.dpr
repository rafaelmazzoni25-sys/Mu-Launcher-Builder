program crypter;

{$APPTYPE CONSOLE}

uses
  SysUtils, Classes, Windows, ZLibEx;

const 
  XORCodes: array[0..6] of Byte = ($53, $6B, $79, $54, $65, $61, $6D); {SkyTeam}    

procedure EncryptDecrypt( FileIN, FileOUT: TMemoryStream);
var
  CharByte, CharByte2: Byte;
  i : integer;
begin
  for i:=0 to FileIN.Size-1 do begin
    FileIN.Position := i;
    FileOUT.Position := i;
    FileIN.Read(CharByte, 1);

    CharByte2:= CharByte xor XORCodes[i mod 7]; //XOR Each byte

    FileOUT.Write(CharByte2,1);
  end;
end;

procedure EncryptDecrypt2( Data: TStream);
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

procedure PackOptionsData(Data: TStream);
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

var
   OptionFile: TStream;
begin
  try
    OptionFile:= TFileStream.Create(ParamStr(1), fmOpenReadWrite + fmShareExclusive);
    try
      PackOptionsData(OptionFile);
      EncryptDecrypt2(OptionFile);
    finally
      OptionFile.Free;
    end;
    
    WriteLn('File encrypted.'); 
  except
    WriteLn('Some error. Check files.');
  end;
end.
