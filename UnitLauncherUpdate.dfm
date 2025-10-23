object frmLauncherUpdate: TfrmLauncherUpdate
  Left = 419
  Top = 258
  BorderIcons = [biMinimize]
  BorderStyle = bsSingle
  Caption = 'Client Update - Created by SkyTeam'
  ClientHeight = 225
  ClientWidth = 330
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poDesktopCenter
  OnCreate = FormCreate
  PixelsPerInch = 96
  TextHeight = 13
  object ProgressBar: TProgressBar
    Left = 8
    Top = 128
    Width = 313
    Height = 17
    Min = 0
    Max = 100
    Smooth = True
    TabOrder = 0
  end
  object ProgressBarTotal: TProgressBar
    Left = 8
    Top = 144
    Width = 313
    Height = 25
    Min = 0
    Max = 100
    Smooth = True
    TabOrder = 1
  end
  object lbLog: TListBox
    Left = 8
    Top = 8
    Width = 313
    Height = 113
    ItemHeight = 13
    TabOrder = 2
  end
  object Status: TStatusBar
    Left = 0
    Top = 206
    Width = 330
    Height = 19
    Panels = <
      item
        Width = 330
      end>
    SimplePanel = False
  end
  object btnCancel: TButton
    Left = 248
    Top = 176
    Width = 75
    Height = 25
    Caption = '&Cancel'
    TabOrder = 4
    OnClick = btnCancelClick
  end
  object btnUpdate: TButton
    Left = 168
    Top = 176
    Width = 75
    Height = 25
    Caption = '&Update'
    TabOrder = 5
    OnClick = btnUpdateClick
  end
  object httpUpdater: TIdHTTP
    OnStatus = httpUpdaterStatus
    MaxLineLength = 32768
    MaxLineAction = maException
    RecvBufferSize = 65536
    OnWork = httpUpdaterWork
    OnWorkBegin = httpUpdaterWorkBegin
    OnWorkEnd = httpUpdaterWorkEnd
    AllowCookies = True
    HandleRedirects = True
    ProxyParams.BasicAuthentication = False
    ProxyParams.ProxyPort = 0
    Request.ContentLength = -1
    Request.ContentRangeEnd = 0
    Request.ContentRangeStart = 0
    Request.Accept = 'text/html, */*'
    Request.BasicAuthentication = False
    Request.UserAgent = 'Mozilla/3.0 (compatible; SkyTeam Updater)'
    HTTPOptions = [hoForceEncodeParams]
    CookieManager = IdCookieManager1
    Left = 8
    Top = 176
  end
  object IdCookieManager1: TIdCookieManager
    Left = 40
    Top = 176
  end
end
