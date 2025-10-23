unit UnitLauncherUpdate;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ComCtrls, StdCtrls, IdBaseComponent, IdComponent,
  IdTCPConnection, IdTCPClient, IdHTTP, UnitLauncherRecords, ExtCtrls,
  IdCookieManager, IdIOHandler, IdIOHandlerStream, DIMimeStreams, md5;

type
  TfrmLauncherUpdate = class(TForm)
    ProgressBar: TProgressBar;
    ProgressBarTotal: TProgressBar;
    lbLog: TListBox;
    httpUpdater: TIdHTTP;
    Status: TStatusBar;
    IdCookieManager1: TIdCookieManager;
    btnCancel: TButton;
    btnUpdate: TButton;
    procedure DownloadFile(const DFrom, DTo: String);
    procedure httpUpdaterStatus(ASender: TObject; const AStatus: TIdStatus;
      const AStatusText: String);
    procedure httpUpdaterWorkBegin(Sender: TObject; AWorkMode: TWorkMode;
      const AWorkCountMax: Integer);
    procedure httpUpdaterWork(Sender: TObject; AWorkMode: TWorkMode;
      const AWorkCount: Integer);
    procedure httpUpdaterWorkEnd(Sender: TObject; AWorkMode: TWorkMode);
    procedure FormCreate(Sender: TObject);
    procedure btnCancelClick(Sender: TObject);
    procedure btnUpdateClick(Sender: TObject);
  private
    NextDownload: Boolean;
    CanCancel: Boolean;
  public
    UpdateDataUrl: String;
  end;

var
  frmLauncherUpdate: TfrmLauncherUpdate;

implementation

{$R *.dfm}

procedure TfrmLauncherUpdate.DownloadFile(const DFrom, DTo: String);
var
   FileData, FileOut: TStream;
begin

  FileData:= TMemoryStream.Create;
  try
    lbLog.Items.Add( Format('Downloading %s', [ExtractFileName(DTo)]) );
    try
      FileData.Position := 0;
      httpUpdater.Get(DFrom, FileData);

      UnpackData(FileData);     // Unpack Data
//      EncryptDecrypt(FileData); // Decrypt Data


      if not DirectoryExists(ExtractFilePath(DTo)) then
        CreateDir(ExtractFilePath(DTo));


      FileOut:= TFileStream.Create(DTo, fmCreate);
      try
        FileOut.Position := 0;
        FileData.Position:= 0;
        MimeDecodeStream(FileData, FileOut);
        //FileOut.CopyFrom(FileData, FileData.Size); // Copy to file
      finally
        FileOut.Free;
      end;
    except
      on E: Exception do lbLog.Items.Add( Format('Exception encountered: %s', [E.Message]) );
    end;
  finally
    FileData.Free;
  end;
end;

procedure TfrmLauncherUpdate.httpUpdaterStatus(ASender: TObject;
  const AStatus: TIdStatus; const AStatusText: String);
begin
  Status.Panels[0].Text:= AStatusText;
end;

procedure TfrmLauncherUpdate.httpUpdaterWorkBegin(Sender: TObject;
  AWorkMode: TWorkMode; const AWorkCountMax: Integer);
begin
  ProgressBar.Position := 0;
  ProgressBar.Max := AWorkCountMax;
  NextDownload:= False;
end;

procedure TfrmLauncherUpdate.httpUpdaterWork(Sender: TObject;
  AWorkMode: TWorkMode; const AWorkCount: Integer);
begin
  ProgressBar.Position := AWorkCount;
end;

procedure TfrmLauncherUpdate.httpUpdaterWorkEnd(Sender: TObject;
  AWorkMode: TWorkMode);
begin
  NextDownload:= True;
end;

procedure TfrmLauncherUpdate.FormCreate(Sender: TObject);
begin
  DoubleBuffered := True;
  CanCancel:= True;
end;

procedure TfrmLauncherUpdate.btnCancelClick(Sender: TObject);
begin
  if CanCancel then Close;
    
  CanCancel:= True;
  Status.Panels[0].Text:= 'Cancelled.';
end;

procedure TfrmLauncherUpdate.btnUpdateClick(Sender: TObject);
var
  i: integer;
  UpdateData: TStream;
  FileData, FileInfo: TStrings;
  CurrentLauncherPath: String;
begin
  btnUpdate.Enabled := False;
  CanCancel:= False;
  
  CurrentLauncherPath:= ExtractFilePath(Application.ExeName);

  UpdateData:= TMemoryStream.Create;
  FileData:= TStringList.Create;
  FileInfo:= TStringList.Create;
  try
    lbLog.Items.Add('Downloading update data...');
    try
      UpdateData.Position:= 0;
      httpUpdater.Get(UpdateDataUrl + 'update.cfg', UpdateData);

      EncryptDecrypt(UpdateData); // Decrypt Data
      UnpackData(UpdateData);      // Unpack Data

      UpdateData.Position:= 0;
      FileData.LoadFromStream(UpdateData);

      NextDownload:= True;
      ProgressBarTotal.Max:= FileData.Count-1;
      for I := 0 to FileData.Count - 1 do begin    // Iterate
        Application.ProcessMessages;      
        if CanCancel then Exit;

        FileInfo.Clear;
        FileInfo.CommaText := FileData[I];
        {Check if file is the same}
        if (FileInfo[1]<>MD5Print(MD5File(CurrentLauncherPath + FileInfo[0]))) or (not FileExists(CurrentLauncherPath + FileInfo[0])) then
          DownloadFile(BuildUrl(UpdateDataUrl + 'udata/', FileInfo[0] + '.pak'), CurrentLauncherPath + FileInfo[0]);

        ProgressBarTotal.Position:= I;
      end;    // for
    except
      on E: Exception do lbLog.Items.Add( Format('Exception encountered: %s', [E.Message]) );
    end;
  finally
    FileInfo.Free;
    FileData.Free;
    UpdateData.Free;
  end;

  Status.Panels[0].Text:= 'Finished';
  CanCancel := True;
end;

end.
