object frmLauncherMain: TfrmLauncherMain
  Left = 351
  Top = 317
  BorderStyle = bsNone
  ClientHeight = 57
  ClientWidth = 228
  Color = clBlack
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poDesktopCenter
  OnClose = FormClose
  OnCreate = FormCreate
  OnMouseDown = FormMouseDown
  OnMouseMove = FormMouseMove
  OnMouseUp = FormMouseUp
  OnPaint = FormPaint
  PixelsPerInch = 96
  TextHeight = 13
  object lblStatus: TLabel
    Left = 40
    Top = 8
    Width = 105
    Height = 25
    Alignment = taCenter
    AutoSize = False
    Color = clWhite
    Font.Charset = OEM_CHARSET
    Font.Color = clWhite
    Font.Height = -8
    Font.Name = 'Terminal'
    Font.Style = []
    ParentColor = False
    ParentFont = False
    Transparent = True
    Visible = False
  end
  object OptionsData: TJvSimpleXML
    IndentString = '  '
    Left = 184
    Top = 8
  end
  object XPManifestFile1: TXPManifestFile
    Left = 152
    Top = 8
  end
  object TimerServerStatus: TTimer
    Enabled = False
    Interval = 5000
    OnTimer = TimerServerStatusTimer
    Left = 8
    Top = 8
  end
end
