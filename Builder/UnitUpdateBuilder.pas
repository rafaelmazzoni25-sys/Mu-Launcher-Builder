unit UnitUpdateBuilder;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, Buttons, ComCtrls, md5, JclStrings, siDialog, ZLibEx,
  DIMimeStreams;

const
  XORCodes: array[0..6] of Byte = ($53, $6B, $79, $54, $65, $61, $6D); {SkyTeam}  

type
  TfrmUpdateBuilder = class(TForm)
    eFolderFilesIn: TEdit;
    Label1: TLabel;
    eFolderFilesOut: TEdit;
    Label2: TLabel;
    btnSearchDirIn: TSpeedButton;
    btnSearchDirOut: TSpeedButton;
    btnCreate: TButton;
    btnClose: TButton;
    pbPackProgress: TProgressBar;
    lblProgress: TLabel;
    BrowseForFolder: TsiBrowseForFolder;
    procedure PackFile(Const FileIN, FileOUT: TFileName);
    procedure btnCloseClick(Sender: TObject);
    procedure btnCreateClick(Sender: TObject);
    procedure btnSearchDirInClick(Sender: TObject);
    procedure btnSearchDirOutClick(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  frmUpdateBuilder: TfrmUpdateBuilder;

implementation

{$R *.dfm}

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

procedure GetFiles(APath: string; AExt: string; AList: TStrings; ARecurse: boolean; Const Strip: Integer);
var
  theExt: string;
  searchRec: SysUtils.TSearchRec;
begin

  if APath[Length(APath)] <> '\' then APath := APath + '\';

//  AList.AddObject(APath, Pointer(-1));

  if FindFirst(APath + '*.*', faAnyFile, searchRec) = 0 then repeat
    with searchRec do begin
      if (Name <> '.') and (Name <> '..') then
        if (Attr and faDirectory <= 0) then begin
          theExt := '*' + UpperCase(ExtractFileExt(searchRec.Name));

          if (AExt = '*.*') or (theExt = UpperCase(AExt)) then begin
            AList.AddObject( StrRestOf(APath + searchRec.Name, Strip), Pointer(0));
          end;  
        end else begin
          if ARecurse then begin
            GetFiles(APath + Name + '\', AExt, AList, ARecurse, Strip);
          end;
        end;
    end; {with searchRec...}

  Application.ProcessMessages;
  until FindNext(searchRec) <> 0;

  SysUtils.FindClose(searchRec);
end;

procedure TfrmUpdateBuilder.PackFile(Const FileIN, FileOUT: TFileName);
var _file_in, _file_out, temp: TStream;
begin
  _file_in := TFileStream.Create(FileIN, fmOpenRead + fmShareDenyNone); //Open file for read
  temp:= TMemoryStream.Create;
  try
    temp.Position:= 0;
    _file_in.Position:= 0;

    MimeEncodeStreamNoCRLF(_file_in, temp);

//    temp.CopyFrom(_file_in, _file_in.Size);
    
    if not DirectoryExists(ExtractFilePath(FileOUT)) then
      CreateDir(LowerCase(ExtractFilePath(FileOUT))); //Create directory

    _file_out:= TFileStream.Create(LowerCase(FileOUT) + '.pak', fmCreate);
    try
      temp.Position := 0;
      {Encrypt file}
//      EncryptDecrypt2(temp);
      
      temp.Position := 0;
      _file_out.Position:= 0;      
      {Compress File}
      ZCompressStream(temp, _file_out, zcMax);
    finally
      _file_out.Free;
    end;
  finally
    temp.Free;
    _file_in.Free;
  end;
end;

procedure TfrmUpdateBuilder.btnCloseClick(Sender: TObject);
begin
  Close;
end;

procedure TfrmUpdateBuilder.btnCreateClick(Sender: TObject);
  function CreateFileMD5Data(Const FileName, MD5Hash: String): String;
  var
    FileData: TStrings;
  begin
    FileData:= TStringList.Create;
    try
      FileData.Add(FileName);
      FileData.Add(MD5Hash);
      Result:= FileData.CommaText;
    finally
      FileData.Free;
    end;
  end;
var
  DataDir, DataOut: String;
  I: Integer;
  FileList: TStrings;
  FileData: TStrings;

  UpdateData: TStream;
  UpdateFile: TStream;
begin
  btnCreate.Enabled := False;

  DataDir:= eFolderFilesIn.Text + '\';
  DataOut:= eFolderFilesOut.Text + '\';
  
  FileList:= TStringList.Create;
  try
    GetFiles(DataDir, '*.*', FileList, True, StrLength(DataDir)+1); //Create File list
    
    FileData:= TStringList.Create;
    try
      pbPackProgress.Max:= FileList.Count-1;
      for I := 0 to FileList.Count-1 do begin    // Iterate
        Application.ProcessMessages;
        {Create Update Data}
        FileData.Add( CreateFileMD5Data(FileList[I], MD5Print(MD5File(DataDir + FileList[I]))) );

        lblProgress.Caption:= Format('Packing %s', [FileList[I]]);
        PackFile(DataDir + FileList[I], DataOut + 'udata\' + FileList[I]); //Pack file
        pbPackProgress.Position := I; //Show Progress
      end;    // for

      UpdateData:= TMemoryStream.Create;
      try
        UpdateData.Position:= 0;
        FileData.SaveToStream(UpdateData);
        
        UpdateFile:= TFileStream.Create(DataOut + 'update.cfg', fmCreate);
        try
          UpdateData.Position:= 0;
          UpdateFile.Position:= 0;
          {Compress UpdateData}
          ZCompressStream(UpdateData, UpdateFile, zcMax);
          {Encrypt Update File}
          EncryptDecrypt2(UpdateFile);
        finally
          UpdateFile.Free;
        end;
      finally
        UpdateData.Free;
      end;
    finally
      FileData.Free;
    end;

  finally
    FileList.Free;
  end;

  ShowMessage('Finished');
end;

procedure TfrmUpdateBuilder.btnSearchDirInClick(Sender: TObject);
begin
  if BrowseForFolder.Execute then begin
    eFolderFilesIn.Text := BrowseForFolder.SelectedDir;
  end;
end;

procedure TfrmUpdateBuilder.btnSearchDirOutClick(Sender: TObject);
begin
  if BrowseForFolder.Execute then begin
    eFolderFilesOut.Text := BrowseForFolder.SelectedDir;
  end;
end;

end.
