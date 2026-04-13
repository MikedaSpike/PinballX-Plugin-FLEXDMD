Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Timers
Imports System.Windows.Forms
Imports System.Xml



Namespace PinballX
    Public Structure PluginInfo
        Public Const Name As String = "FLEXDMD Plugin"
        Public Const Version As String = "2.0"
        Public Const Author As String = "Mike DA Spike"
        Public Const Description As String = "An advanced FlexDMD-based replacement for the standard PinballX XDMD." & vbCrLf & "Perfectly optimised for Real and Virtual DMD setups, supporting both 128x32 and 256x64 (HD) resolutions."
        Public Const PluginVersion As String = "2.0"
        Dim Dummy As String
    End Structure

    Public Structure PinballXMediaDirs
        Public Const WheelImages As String = "Wheel Images"
        Public Const BackglassImages As String = "Backglass Images"
        Public Const BackglassVideos As String = "Backglass Videos"
        Public Const DMDImages As String = "DMD Images"
        Public Const DMDVideos As String = "DMD Videos"
        Public Const LaunchAudio As String = "Launch Audio"
        Public Const RealDMDColorImages As String = "Real DMD Color Images"
        Public Const RealDMDColorVideos As String = "Real DMD Color Videos"
        Public Const RealDMDImages As String = "Real DMD Images"
        Public Const RealDMDVideos As String = "Real DMD Videos"
        Public Const TableAudio As String = "Table Audio"
        Public Const TableImages As String = "Table Images"
        Public Const TableImagesDesktop As String = "Table Images Desktop"
        Public Const TableVideos As String = "Table Videos"
        Public Const TableVideosDesktop As String = "Table Videos Desktop"
        Public Const TopperImages As String = "Topper Images"
        Public Const TopperVideos As String = "Topper Videos"
        Public Const FullDMDVideos As String = "FullDMD Videos"
        Public Const FullDMDImages As String = "FullDMD Images"
    End Structure

    Public Structure dtXML
        Public Const TableName As String = "XMLTABLE"
        Public Const Name As String = "name"
        Public Const Description As String = "description"
        Public Const SystemEntry As String = "systementry"
        Public Const SystemType As String = "systemtype"
        Public Const SystemName As String = "systemname"
        Public Const Rom As String = "rom"
        Public Const Theme As String = "theme"
        Public Const Manufacturer As String = "manufacturer"
        Public Const Year As String = "year"
        Public Const Type As String = "type"
        Public Const HideDMD As String = "hidedmd"
        Public Const HideTopper As String = "hidetopper"
        Public Const HideBackglass As String = "hidebackglass"
        Public Const TableEnabled As String = "enabled"
        Public Const Rating As String = "rating"
        Public Const NRofPlayers As String = "players"
        Public Const Comment As String = "comment"
        Public Const Author As String = "author"
        Public Const Version As String = "version"
        Public Const Alternateexe As String = "alternateexe"
        Public Const Exe As String = "exe"
        Public Const Gridposition As String = "gridposition"
        Public Const IPDBnr As String = "IPDBnr"
        Public Const DateAdded As String = "dateadded"
        Public Const DateModified As String = "datemodified"
        Public Const AddedNewTable As String = "addednewtable"
        Public Const FoundObsoleteTable As String = "foundobsoletetable"
        Public Const ID As String = "ID"
        Public Const StatsFileEntryFound As String = "StatsFileEntryFound"
        Public Const StatsFileDescription As String = "StatsFileDescription"
        Public Const StatsFileLastPlayed As String = "StatsFileLastPlayed"
        Public Const StatsFileSecondsPlayed As String = "StatsFileSecondsPlayed"
        Public Const StatsFileFavourite As String = "StatsFileFavourite"
        Public Const StatsFileTimesPlayed As String = "StatsFileTimesPlayed"
    End Structure


    <StructLayout(LayoutKind.Sequential)>
    Public Structure PinballXInfo
        Public Version As String
    End Structure

    Public Class Plugin
        Private disposed As Boolean = False

        Public Shared Logger As New DracLabs.Logger
        Private _iniFile As New DracLabs.IniFile

        Private _UseHD As Boolean = False

        Private _FLEXDMD As FlexDMDEngine
        Private _carousel As CarouselManager

        'Pinball X media folders

        Private blnInGame As Boolean = False 'track if game selected/active, if user presses pause we don't want to display stat screen
        Private blnAttractModeActive As Boolean = False 'tracks id Screensaver mode is active

        'pbx system tyes, 0-4 match those in use by pbx at time of writing. Others don't have a type number assigned in pbx they are named systems.
        Private Const SYSTYPE_CUSTOM0 As Integer = 0 'pbx can use 0 or 3 as system type for custom
        Private Const SYSTYPE_VISUALPINBALL As Integer = 1
        Private Const SYSTYPE_FUTUREPINBALL As Integer = 2
        Private Const SYSTYPE_CUSTOM As Integer = 3 'mame etc
        Private Const SYSTYPE_CUSTOMEXE As Integer = 4
        Private Const SYSTYPE_OTHER As Integer = 5 'pinball fx, VPE?, unused
        Private Const SYSTYPE_PINBALLARCADE As Integer = 6
        Private Const SYSTYPE_ZACCARIAPINBALL As Integer = 7
        Private Const SYSTYPE_PINBALLFX2 As Integer = 8
        Private Const SYSTYPE_PINBALLFX3 As Integer = 9

        Private WithEvents SequenceTimer As New System.Timers.Timer()
        Private CurrentMediaFile As String = ""
        Private CurrentText As String = ""
        Private MediaIsImage As Boolean = False
        Private VideoLoopCount As Integer = 0
        Private TargetLoops As Integer = 3
        Private ImageTimerCounter As Integer = 0
        Private VideoStartedTime As DateTime

        Private lastSystem As String, lastTable As String, lastMedia As String, LastHighscoreText As String
        Private ExtraSeconds As Integer = 0

        ' PinemHi categories to show in the carousel, in the following order. 
        Private ReadOnly _pinemHiCategories As String() = {
                        "TOP10_Personal", "TOP10_Personal_Specials", "TOP10_Personal_5min",
                        "TOP10_Best", "TOP10_Best_5min", "TOP10_Friends",
                        "TOP10_Friends_5min", "TOP10_Cup", "TOP10_Cup_5min"
                    }
        Private _pinemHiPath As String
        Private _showBadges As Boolean
        Private _badgeDurationMs As Integer
        Private _activeScoreCategories As New List(Of String)
        Private Shared _cachedTable As DataTable = Nothing
        Private Shared _lastXmlFile As String = String.Empty

        <StructLayout(LayoutKind.Sequential)>
        Public Structure PlugInInfo_1
            Public System As String
            Public SystemName As String
            Public INI As String
            Public GameName As String
            Public GameDescription As String
            Public GameShortDescription As String
            Public TableFile As String
            Public TablePath As String
            Public Parameters As String
            Public DisplayWidth As Integer
            Public DisplayHeight As Integer
            Public DisplayRotation As Integer
            Public DisplayWindowed As Boolean
            Public PlayFieldDisplayNbr As Integer
            Public BackGlassDisplayNbr As Integer
            Public DMDDisplayNbr As Integer
            Public Filter As String
        End Structure
        Public Sub Plugin()

        End Sub
        Public Function Initialize(ByVal InfoPtr As IntPtr) As Boolean
            Try
                Dim assemblyPath As String = Reflection.Assembly.GetExecutingAssembly().Location
                Dim dllName As String = Path.GetFileNameWithoutExtension(assemblyPath)
                Dim pluginDir As String = Path.GetDirectoryName(assemblyPath)

                Dim logPath As String = Path.Combine(pluginDir, dllName & ".log")
                Dim iniPath As String = Path.Combine(pluginDir, dllName & ".ini")

                Logger.Initialize(dllName, PluginInfo.Version, logPath)
                LogInfo("----------------------------------------------------------------")
                LogInfo("Initializing FlexDMD Plugin Version " & PluginInfo.Version)

                If Not File.Exists(iniPath) Then
                    LogError("Configuration file not found: " & iniPath)
                    Return False
                End If

                _iniFile.Load(iniPath)
                LogInfo("INI Loaded: " & iniPath)

                _UseHD = ParseBool(_iniFile.GetKeyValue("FlexDMD", "UseHD"), False)

                _FLEXDMD = New FlexDMDEngine()
                _FLEXDMD.DllPath = _iniFile.GetKeyValue("FlexDMD", "FlexDMDDllPath")

                Try
                    Dim colorVal As String = _iniFile.GetKeyValue("PinballX", "DMDColour")
                    If String.IsNullOrEmpty(colorVal) Then colorVal = "Red"

                    colorVal = colorVal.Trim()

                    If colorVal.Contains(",") Then
                        Dim rgb As String() = colorVal.Split(","c)
                        If rgb.Length = 3 Then
                            _FLEXDMD.DMDColor = Color.FromArgb(CInt(rgb(0)), CInt(rgb(1)), CInt(rgb(2)))
                        End If
                    ElseIf colorVal.StartsWith("#") Then
                        _FLEXDMD.DMDColor = ColorTranslator.FromHtml(colorVal)
                    Else
                        _FLEXDMD.DMDColor = Color.FromName(colorVal)
                    End If
                Catch ex As Exception
                    _FLEXDMD.DMDColor = Color.Red
                    LogError("Error parsing DMDColor, defaulting to Red: " & ex.Message)
                End Try

                If Not _FLEXDMD.Init(useHD:=_UseHD) Then
                    LogError("Failed to initialize FlexDMD COM object.")
                    Return False
                End If

                _carousel = New CarouselManager(_FLEXDMD)

                ' General Settings
                _carousel.UseHd = _UseHD
                _carousel.ShowMedia = ParseBool(_iniFile.GetKeyValue("PinballX", "ShowMedia"), True)
                _carousel.MaxVideoLoops = ParseInt(_iniFile.GetKeyValue("PinballX", "MaxVideoLoops"), 3)
                _carousel.ImageDuration = ParseDouble(_iniFile.GetKeyValue("PinballX", "ImageDuration"), 5.0)
                _carousel.ShowPBXHighscores = ParseBool(_iniFile.GetKeyValue("PinballX", "ShowHighscores"), True)

                ' Clock Settings
                _carousel.ShowClock = ParseBool(_iniFile.GetKeyValue("Clock", "ShowClock"), True)
                _carousel.ShowClockEverySeconds = ParseInt(_iniFile.GetKeyValue("Clock", "ShowClockEverySeconds"), 60)
                _carousel.ClockDuration = ParseDouble(_iniFile.GetKeyValue("Clock", "ClockDuration"), 10.0)
                _carousel.Show24h = ParseBool(_iniFile.GetKeyValue("Clock", "Show24h"), True)

                ' Global Debug
                GlobalConfig.DebugMode = ParseBool(_iniFile.GetKeyValue("Settings", "Debug"), False)

                ' PINemHi Configuration
                _pinemHiPath = _iniFile.GetKeyValue("PINemHi", "PinemHiPath")
                _carousel.ShowPinemHiCountdown = ParseBool(_iniFile.GetKeyValue("PINemHi", "ShowCountdown"), True)
                _carousel.ExtraSeconds = ParseInt(_iniFile.GetKeyValue("PINemHi", "ExtraSeconds"), 10)
                _carousel.ShowPinemHiScores = ParseBool(_iniFile.GetKeyValue("PINemHi", "ShowHighscores"), True)

                ' Badge Settings
                _carousel.ShowPinemHiBadges = ParseBool(_iniFile.GetKeyValue("PINemHi", "ShowBadges"), False)
                Dim txtEarned As String = _iniFile.GetKeyValue("PINemHi", "BadgesEarnedText")
                If Not String.IsNullOrEmpty(txtEarned) Then _carousel.PinemHighBadgesEarned = txtEarned

                Dim txtNone As String = _iniFile.GetKeyValue("PINemHi", "NoBadgesText")
                If Not String.IsNullOrEmpty(txtNone) Then _carousel.PinemHighNoBadgesEarned = txtNone

                ' Check for external awesome Scutter PinemHi Launcher (5 min Weekly Challenge)
                If _carousel.ShowPinemHiCountdown Then
                    Dim pinemhiPluginIni As String = Path.Combine(My.Application.Info.DirectoryPath, "Plugins", "PBXPinemhiLauncher.ini")
                    If File.Exists(pinemhiPluginIni) Then
                        Dim launcherIni As New DracLabs.IniFile
                        launcherIni.Load(pinemhiPluginIni)
                        _carousel.Weekly5MinChallengeTable = launcherIni.GetKeyValue("Tables", "WeeklyChallenge5minTableDesc")
                        LogInfo("Weekly Challenge table loaded: " & _carousel.Weekly5MinChallengeTable)
                    Else
                        LogInfo("PBXPinemhiLauncher.ini not found. Countdown interrupt disabled.")
                        _carousel.ShowPinemHiCountdown = False
                    End If
                End If

                If _carousel.ShowPinemHiScores Then
                    _activeScoreCategories.Clear()
                    For Each category In _pinemHiCategories
                        Dim showCat As Boolean = ParseBool(_iniFile.GetKeyValue("PINemHi", "Show_" & category), False)
                        LogInfo($"PINemHi Category [{category}] active: {showCat}")
                        If showCat Then _activeScoreCategories.Add(category)
                    Next
                End If

                LogInfo($"Badges Enabled: {_carousel.ShowPinemHiBadges}")
                If _carousel.ShowPinemHiBadges Then
                    LogInfo($"Badge Text (Earned): {_carousel.PinemHighBadgesEarned}")
                    LogInfo($"Badge Text (None): {_carousel.PinemHighNoBadgesEarned}")
                End If

                Dim line1 As String, line2 As String, line3 As String
                If _UseHD Then
                    line1 = $"{PluginInfo.Name.Replace("FLEXDMD", "FLEXDMD HD")} {PluginInfo.Version}"
                    line2 = "INITIALIZED SUCCESSFULLY"
                    line3 = $"BY {PluginInfo.Author}"
                Else
                    line1 = PluginInfo.Name
                    line2 = $"VERSION {PluginInfo.Version}"
                    line3 = "STATUS: OPERATIONAL"
                End If

                _carousel.StartWithSplash(line1, line2, line3)

                LogSummary()

                LogInfo("Initialize Success")
                Return True

            Catch ex As Exception
                LogError($"Initialize Critical Error: {ex.Message}")
                Return False
            Finally
                LogInfo("----------------------------------------------------------------")
            End Try
        End Function

#Region " Initialization Helpers "

        Private Function ParseBool(value As String, defaultValue As Boolean) As Boolean
            If String.IsNullOrEmpty(value) Then Return defaultValue
            Dim cleanValue As String = value.Trim().ToLower()
            If cleanValue = "true" OrElse cleanValue = "1" OrElse cleanValue = "yes" Then Return True
            If cleanValue = "false" OrElse cleanValue = "0" OrElse cleanValue = "no" Then Return False
            Return defaultValue
        End Function

        Private Function ParseInt(value As String, defaultValue As Integer) As Integer
            Dim result As Integer
            If Integer.TryParse(value, result) Then Return result
            Return defaultValue
        End Function

        Private Function ParseDouble(value As String, defaultValue As Double) As Double
            Dim result As Double
            If Double.TryParse(value.Replace(","c, "."c), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, result) Then
                Return result
            End If
            Return defaultValue
        End Function

        Private Sub LogSummary()
            LogInfo("Display Mode: " & If(_UseHD, "HD", "Standard"))
            LogInfo("DMD Colour (RGB): " & $"{_FLEXDMD.DMDColor.R},{_FLEXDMD.DMDColor.G},{_FLEXDMD.DMDColor.B}")
            LogInfo("Show Media: " & _carousel.ShowMedia)
            LogInfo("Show Clock: " & _carousel.ShowClock & " (Every " & _carousel.ShowClockEverySeconds & "s)")
            LogInfo("Show PBX Scores: " & _carousel.ShowPBXHighscores)
            LogInfo("Show PINemHi Scores: " & _carousel.ShowPinemHiScores)
            LogInfo("Show PINemHi Badges: " & _carousel.ShowPinemHiBadges)
        End Sub

#End Region
        Private Function GetMediaFile(ByVal PinballXMediaPath As String, ByVal SystemName As String, ByVal TableName As String, ByVal TableDescription As String) As String
            Try

                'First check if we found any RealVideo media for none system
                If Not String.IsNullOrEmpty(TableName) Then


                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".mp4"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".avi"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".wmv"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableName & ".gif"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".mp4"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".avi"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".wmv"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\" & TableDescription & ".gif"

                    'Nothing found ? Check the Non Colored Directory
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".mp4"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".avi"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".wmv"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableName & ".gif"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".mp4"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".avi"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".wmv"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\" & TableDescription & ".gif"

                    'maybe we have an image ?
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableName & ".png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableName & ".png"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableName & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableName & ".gif"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableDescription & ".png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableDescription & ".png"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableDescription & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\" & TableDescription & ".gif"

                    'or a non colored Image ?  
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableName & ".png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableName & ".png"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableName & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableName & ".gif"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableDescription & ".png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableDescription & ".png"
                    If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableDescription & ".gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\" & TableDescription & ".gif"

                End If
                'Catch if we don't find a table name, but there is a color system video, we will show the color system video
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.mp4"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.avi"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.wmv"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorVideos & "\- system -.gif"

                'Catch if we don't find a table name, but there is a system video, we will show the system video
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.mp4") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.mp4"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.avi") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.avi"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.wmv") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.wmv"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDVideos & "\- system -.gif"

                'Catch if we don't find a table name, but there is a color system image, we will show the color  system image
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\- system -.png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\- system -.png"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\- system -.gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDColorImages & "\- system -.gif"

                'Catch if we don't find a table name, but there is a system image, we will show the system image
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\- system -.png") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\- system -.png"
                If IO.File.Exists(PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\- system -.gif") Then Return PinballXMediaPath & "\Media\" & SystemName & "\" & PinballXMediaDirs.RealDMDImages & "\- system -.gif"


                'if all else is failing, check if the default PinballX video is there
                If IO.File.Exists(PinballXMediaPath & "\Media\Videos\No Real DMD Color.avi") Then Return PinballXMediaPath & "\Media\Videos\No Real DMD Color.avi"

                Return ""
            Catch ex As Exception
                LogError("GetMediaFile Error : " & ex.Message)
                Return ""
            End Try


        End Function

        Private Function GetPBXHighscoreText(ByVal pbxPath As String, ByVal systemName As String, ByVal tableName As String) As String
            Try
                Dim highScoresDir As String = Path.Combine(pbxPath, "High Scores", systemName)
                Dim textFilePath As String = Path.Combine(highScoresDir, tableName & ".txt")

                If File.Exists(textFilePath) Then
                    Return File.ReadAllText(textFilePath)
                End If
            Catch ex As Exception
                LogError("GetPBXHighscoreText Error: " & ex.Message)
            End Try
            Return ""
        End Function

        Private Function GetRomName(systemName As String, gameName As String) As String
            Try
                Dim xmlFile As String = IO.Path.Combine(My.Application.Info.DirectoryPath, "Databases", systemName, systemName & ".xml")

                ' Load XML data if it's a new system or not yet loaded
                If xmlFile <> _lastXmlFile OrElse _cachedTable Is Nothing Then
                    _cachedTable = GetDatasetFromXMLFile(xmlFile)
                    _lastXmlFile = xmlFile
                End If

                If _cachedTable IsNot Nothing Then
                    ' Find the table in the XML
                    Dim query As String = $"name LIKE '{EscapeLikeValue(gameName)}'"
                    Dim row As DataRow = _cachedTable.Select(query).FirstOrDefault()

                    ' Extract the ROM name if the row and column exist
                    If row IsNot Nothing AndAlso _cachedTable.Columns.Contains("rom") AndAlso Not IsDBNull(row("rom")) Then
                        Return row("rom").ToString()
                    End If
                End If
            Catch ex As Exception
                LogError("DatabaseHelper.GetRomName Error: " & ex.Message)
            End Try

            Return String.Empty
        End Function
        Private Function GetDatasetFromXMLFile(strXMLFile As String) As DataTable
            Dim dsXMLFile As New DataSet

            Dim fs As New FileStream(strXMLFile, FileMode.Open)
            Dim objXMLReader As New StreamReader(fs, System.Text.Encoding.UTF8)

            If Not objXMLReader Is Nothing Then
                Try
                    dsXMLFile.ReadXml(objXMLReader)
                    objXMLReader.Close()
                    fs.Close()
                Catch ex As Exception
                End Try

                objXMLReader.Close()
                fs.Close()
            End If
            Try
                If dsXMLFile.Tables.Count > 0 Then
                    'remove disabled tables before return
                    Dim dr() As DataRow = dsXMLFile.Tables(0).Select("Enabled = 'False'")
                    If dr.Count >= 1 Then
                        For Each drrow As DataRow In dr
                            dsXMLFile.Tables(0).Rows.Remove(drrow)
                        Next
                    Else
                    End If
                    Try
                        'remove any tables using alternate exe flag (as cmd line won't work unless we start working out file extensions for table files and changing the exe name)
                        Dim dr2() As DataRow = dsXMLFile.Tables(0).Select("alternateexe <> ''")
                        If dr2.Count >= 1 Then
                            For Each drrow As DataRow In dr2
                                dsXMLFile.Tables(0).Rows.Remove(drrow)
                            Next
                        Else
                        End If
                    Catch ex As Exception
                    End Try
                    Return dsXMLFile.Tables(0)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                LogError(ex.Message)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' replaces strange characters for SQL search to correct values
        ''' </summary>
        ''' <param name="value">value that needs to be checked and replaced</param>
        ''' <returns>new string that is replaced with correct characters</returns>
        Public Function EscapeLikeValue(ByVal value As String) As String
            Dim sb As New System.Text.StringBuilder(value.Length)

            For i As Integer = 0 To value.Length - 1
                Dim c As Char = value(i)

                Select Case c
                    Case "]"c, "["c, "%"c, "*"c
                        sb.Append("[").Append(c).Append("]")
                    Case "'"c
                        sb.Append("''")
                    Case Else
                        sb.Append(c)
                End Select
            Next

            Return sb.ToString()
        End Function

        Private Function ReformatPinemhiScore(text As String, useHD As Boolean) As String
            Try

                If String.IsNullOrEmpty(text) Then Return ""
                If text.ToUpper().Contains("NOT SUPPORTED YET") Then Return ""

                Dim cleanedText As String = text

                cleanedText = System.Text.RegularExpressions.Regex.Replace(cleanedText, "\b\d{4}[/-]\d{1,2}[/-]\d{1,2}(?:\s+\d{2}:\d{2}:\d{2})?\b", "")
                cleanedText = System.Text.RegularExpressions.Regex.Replace(cleanedText, "\b\d{1,2}[/-]\d{1,2}[/-]\d{2,4}(?:\s+\d{2}:\d{2}:\d{2})?\b", "")

                Dim rawLines = cleanedText.Split({vbCrLf, vbLf, "|"}, StringSplitOptions.None).Select(Function(l) l.Trim()).ToList()
                Dim lines As New List(Of String)

                Dim scoreTypeIndex As Integer = -1
                Dim newHeader As String = ""
                Dim is5Min As Boolean = text.ToUpper().Contains("5 MINUTE MODE")
                Dim prefix = If(is5Min, "-- PINEMHI 5-Min --", "-- PINEMHI --")
                Dim subTitle As String = ""

                For i As Integer = 0 To Math.Min(rawLines.Count - 1, 10)
                    Dim lineUpper = rawLines(i).ToUpper()

                    If lineUpper.Contains("TOP 10 HIGHEST") Then
                        subTitle = "Global Highscores"
                    ElseIf lineUpper.Contains("TOP 10 FRIEND") Then
                        subTitle = "Friends Leaderboard"
                    ElseIf lineUpper.Contains("TOP 10 PERSONAL") Then
                        subTitle = "Personal Bests"
                    ElseIf lineUpper.Contains("CUP STANDINGS") Then
                        subTitle = "Cup Standings"
                    End If

                    If Not String.IsNullOrEmpty(subTitle) Then ' if match then we found the header for the scores, we can stop looking
                        scoreTypeIndex = i
                        newHeader = $"{prefix}{vbCrLf}{vbCrLf}{subTitle}"
                        Exit For
                    End If
                Next

                If scoreTypeIndex <> -1 Then
                    lines.Add(newHeader)
                    lines.Add("")
                    Dim hasScores As Boolean = False
                    For i As Integer = scoreTypeIndex + 1 To rawLines.Count - 1
                        If Not String.IsNullOrEmpty(rawLines(i)) AndAlso System.Text.RegularExpressions.Regex.IsMatch(rawLines(i), "\d") Then
                            hasScores = True
                            Exit For
                        End If
                    Next

                    If Not hasScores Then
                        lines.Add("No Scores Achieved")
                    Else
                        For i As Integer = scoreTypeIndex + 1 To rawLines.Count - 1
                            ProcessScoreLine(rawLines(i), lines, useHD, newHeader.Contains("Personal"))
                        Next
                    End If
                Else
                    For Each line In rawLines
                        ProcessScoreLine(line, lines, useHD, False)
                    Next
                End If

                Dim result = String.Join(vbCrLf, lines)
                Return result.Trim()
            Catch ex As Exception
                LogError("ReformatPinemhiScore Error : " & ex.Message)
                Return text
            End Try
        End Function

        Private Function ReformatPinemhiSpecial(text As String, useHD As Boolean) As String
            Try
                If String.IsNullOrEmpty(text) Then Return ""

                Dim rawLines = text.Split({vbCrLf, vbLf, "|"}, StringSplitOptions.None).ToList()
                Dim lines As New List(Of String)

                lines.Add("-- PINEMHI --")
                lines.Add("")
                lines.Add("Personal Special Achievements")
                lines.Add("")

                If rawLines.Count > 4 Then
                    For i As Integer = 4 To rawLines.Count - 1
                        Dim currentLine = rawLines(i).Trim()
                        ProcessScoreLine(currentLine, lines, useHD, False)
                    Next
                Else
                    lines.Add("No Achievements yet")
                End If

                Dim result = String.Join(vbCrLf, lines)

                result = System.Text.RegularExpressions.Regex.Replace(result, "(\r\n|\r|\n){3,}", vbCrLf & vbCrLf)

                Return result.Trim()
            Catch ex As Exception
                LogError("ReformatPinemhiSpecial Error : " & ex.Message)
                Return text
            End Try
        End Function
        Private Sub ProcessScoreLine(line As String, ByRef lines As List(Of String), useHD As Boolean, isPersonal As Boolean)
            Try
                Dim cleanedLine = line.Trim()
                If String.IsNullOrEmpty(cleanedLine) Then
                    If lines.Count > 0 AndAlso Not String.IsNullOrEmpty(lines.Last()) Then
                        lines.Add("")
                    End If
                    Exit Sub
                End If

                If isPersonal Then
                    cleanedLine = System.Text.RegularExpressions.Regex.Replace(cleanedLine, "^(\d+\s+[\d\.,]+)\s+.*", "$1")
                    lines.Add(cleanedLine)

                ElseIf Not useHD AndAlso System.Text.RegularExpressions.Regex.IsMatch(cleanedLine, "^\d+\s{2,}") Then

                    If lines.Count > 0 AndAlso Not String.IsNullOrEmpty(lines.Last()) Then
                        lines.Add("")
                    End If

                    Dim parts = System.Text.RegularExpressions.Regex.Split(cleanedLine, "\s{2,}")
                    For Each part In parts
                        If Not String.IsNullOrEmpty(part) Then
                            lines.Add(part.Trim())
                        End If
                    Next

                Else
                    lines.Add(cleanedLine)
                End If
            Catch ex As Exception
                LogError("ProcessScoreLine Error : " & ex.Message)
            End Try
        End Sub


        Public Function Process_GameRun(ByVal InfoPtr As IntPtr) As Boolean
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Try
                Static strLastsystemXMLfile As String = String.Empty
                Static dtXMLFile As DataTable
                Dim strTable As String = Info.GameName.ToString()
                Dim strSystem As String = Info.SystemName.ToString()

                blnInGame = True
                Dim strSystemXMLFile As String = My.Application.Info.DirectoryPath & "\Databases\" & strSystem & "\" & strSystem & ".xml"

                If Not String.IsNullOrEmpty(strTable) Then

                    If strSystemXMLFile <> strLastsystemXMLfile OrElse IsNothing(dtXMLFile) Then
                        dtXMLFile = GetDatasetFromXMLFile(strSystemXMLFile)
                    End If
                    Dim row As DataRow = dtXMLFile.Select(dtXML.Name & " like '" & EscapeLikeValue(strTable) & "'").FirstOrDefault()
                    Dim isHideDMD As Boolean = (row Is Nothing OrElse row(dtXML.HideDMD).ToString().ToLower() = "true")

                    Dim isWeeklyChallenge As Boolean = False
                    LogDebug("Checking if table '" & strTable & "' is the Weekly Challenge table.")
                    If _carousel.ShowPinemHiCountdown AndAlso Not String.IsNullOrEmpty(_carousel.Weekly5MinChallengeTable) Then
                        If strTable.Equals(_carousel.Weekly5MinChallengeTable, StringComparison.OrdinalIgnoreCase) Then
                            isWeeklyChallenge = True
                        End If
                    End If

                    If isWeeklyChallenge Then
                        LogInfo("MATCH: Weekly Challenge detected for '" & strTable & "'. Ignoring HideDMD. Starting countdown.")
                        Dim totalSeconds As Integer = 300
                        Dim msg As String = "WEEKLY 5 MIN CHALLENGE"
                        _carousel.StartPinemHiCountdown(totalSeconds, msg)

                    Else
                        If isHideDMD Then
                            Logger.Log_Data("Event_GameRun: HideDMD is active for '" & strTable & "'. Stopping media.")
                            If _carousel.ShowStartMessage Then
                                Logger.Log_Data("WARNING: Cannot show Start Message because HideDMD is enabled for this table.")
                            End If

                            If _carousel IsNot Nothing Then
                                _carousel.Stop()
                            End If
                        Else
                            Logger.Log_Data("Event_GameRun: Table '" & strTable & "' - DMD remains active.")
                            If _carousel.ShowStartMessage Then
                                _FLEXDMD.ShowMessage("STARTING: " & strTable)
                            End If
                        End If
                    End If
                End If

                strLastsystemXMLfile = strSystemXMLFile
                Return True
            Catch ex As Exception
                LogError("Event_GameRun Error : " & ex.Message)
                Return True
            Finally

            End Try

        End Function

        Public Sub Process_GameSelect(ByVal InfoPtr As IntPtr)

            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Dim intSystemType As Integer = SYSTYPE_OTHER

            If Not blnInGame = False Then Exit Sub 'this event also fires if pause button is hit, so if ingame don't do anything.
            Try
                Dim strSystemName As String = String.Empty
                Try
                    strSystemName = Info.SystemName.ToString
                Catch ex As Exception
                    strSystemName = String.Empty
                End Try
                Dim strSystem As String = String.Empty
                Try
                    strSystem = Info.System.ToString
                Catch ex As Exception
                    strSystem = ""
                End Try
                Dim strTableDesc As String = String.Empty
                Try
                    strTableDesc = Info.GameDescription.ToString
                Catch ex As Exception
                    strTableDesc = String.Empty
                End Try
                Dim strTableName As String = String.Empty
                Try
                    strTableName = Info.GameName.ToString
                Catch ex As Exception
                    strTableName = String.Empty
                End Try
                'get system type for non 'other' systems
                Select Case LCase(strSystemName)
                    Case "visualpinball"
                        intSystemType = SYSTYPE_VISUALPINBALL
                    Case "futurepinball"
                        intSystemType = SYSTYPE_FUTUREPINBALL
                    Case "pinballfx2"
                        intSystemType = SYSTYPE_PINBALLFX2
                    Case "pinballfx3"
                        intSystemType = SYSTYPE_PINBALLFX3
                    Case "pinballarcade"
                        intSystemType = SYSTYPE_PINBALLARCADE
                    Case "zaccaria"
                        intSystemType = SYSTYPE_ZACCARIAPINBALL
                    Case "mame"
                        intSystemType = SYSTYPE_CUSTOM
                    Case Else
                        ' intSystemType = SYSTYPE_OTHER when dimmed so leave as that
                        'can't put an else here anyway as will overwrite values from  For cnt = 0 To 19 loop
                End Select


                Dim strSystemVideoPath As String = My.Application.Info.DirectoryPath & "\Media\" & strSystemName

                If Not IO.Directory.Exists(strSystemVideoPath) Then
                    Logger.Log_Data("Event_GameSelect : Directory not exists : " & strSystemVideoPath)
                    Exit Sub
                End If

                Dim strMediaFile = GetMediaFile(My.Application.Info.DirectoryPath, strSystemName, strTableName, strTableDesc)

                Dim highscoreList As New List(Of String)

                If _carousel.ShowPBXHighscores Then
                    Dim PBXHighscoreText = GetPBXHighscoreText(My.Application.Info.DirectoryPath, strSystemName, strTableName)
                    If Not String.IsNullOrWhiteSpace(PBXHighscoreText) Then
                        LogDebug($"PBX: Highscore data found. {vbCrLf}{PBXHighscoreText}")
                        PBXHighscoreText = $"Local table Rankings{vbCrLf}{PBXHighscoreText}"
                        highscoreList.Add(PBXHighscoreText)
                    Else
                        LogDebug($"PBX: No highscore data found.")
                    End If
                End If


                Dim badgesList As New List(Of PinemHiManager.BadgeResult)

                ' Check if PinemHi features (Scores or Badges) are enabled
                If _carousel.ShowPinemHiScores OrElse _carousel.ShowPinemHiBadges Then
                    Dim romName As String = GetRomName(strSystemName, strTableName)

                    If Not String.IsNullOrEmpty(romName) Then
                        LogInfo($"ROM identified: '{romName}' for system '{strSystemName}'. Processing PinemHi data...")

                        ' Create the manager once
                        Dim phManager As New PinemHiManager(_pinemHiPath)

                        If _carousel.ShowPinemHiScores Then
                            For Each category In _activeScoreCategories
                                Dim scoreText As String = phManager.GetLeaderboardData(category, romName)

                                If Not String.IsNullOrEmpty(scoreText) Then
                                    LogDebug($"PinemHi: Highscore data found for [{category}].")
                                    Dim cleanPinemhitext As String = String.Empty
                                    If category = "TOP10_Personal_Specials" Then
                                        cleanPinemhitext = ReformatPinemhiSpecial(scoreText, _carousel.UseHd)
                                    Else
                                        cleanPinemhitext = ReformatPinemhiScore(scoreText, _carousel.UseHd)
                                    End If

                                    LogDebug($"Reformated PinemHi: scored [{vbCrLf}{cleanPinemhitext}].")
                                    highscoreList.Add(cleanPinemhitext)
                                Else
                                    LogDebug($"PinemHi: No highscore data for [{category}].")
                                End If
                            Next
                        End If

                        If _carousel.ShowPinemHiBadges Then
                            Dim totalbadgesList = PinemHiManager.GetBadges(_pinemHiPath, romName, False)
                            badgesList = PinemHiManager.GetBadges(_pinemHiPath, romName, True)

                            Dim totalCount As Integer = If(totalbadgesList IsNot Nothing, totalbadgesList.Count, 0)
                            Dim earnedCount As Integer = If(badgesList IsNot Nothing, badgesList.Count, 0)

                            LogDebug($"PinemHi Badges: {earnedCount}/{totalCount} earned for ROM '{romName}'.")

                            If earnedCount = 0 AndAlso totalCount > 0 Then
                                _carousel.PinemHighNoBadgestext = $"{_carousel.PinemHighNoBadgesEarned} (0/{totalCount} unlocked)."
                            ElseIf earnedCount > 0 Then
                                _carousel.PinemHighBadgesEarnedtext = $"{_carousel.PinemHighBadgesEarned} ({earnedCount}/{totalCount}) :"
                            Else
                                badgesList = Nothing
                            End If
                        End If

                    Else
                        LogInfo($"No ROM mapping found in databases for '{strTableName}' [{strSystemName}]. PinemHi features skipped.")
                    End If
                End If

                LogInfo("Event_GameSelect : Get System Data : SystemName = " & strSystemName & " : System = " & strSystem & " : TableDesc = " & strTableDesc & " : TableName = " & strTableName & " : VideoFile = " & strMediaFile)
                LogDebug("Event_GameSelect : Get System Data : lastSystem = " & lastSystem & " : lastTable = " & lastTable & " : lastMedia = " & lastMedia)

                If strSystemName = lastSystem AndAlso strTableName = lastTable AndAlso strMediaFile = lastMedia Then
                    LogDebug("Event_GameSelect bypassed: Table data is unchanged.")
                    Exit Sub
                ElseIf _carousel IsNot Nothing Then
                    _carousel.UpdateGameInfo(strMediaFile, highscoreList, badgesList)
                End If

                lastSystem = strSystemName
                lastTable = strTableName
                lastMedia = strMediaFile

            Catch ex As Exception
                LogError("Process_GameSelect Error : " & ex.Message)
            End Try
        End Sub

        Public Sub Process_GameExit(ByVal InfoPtr As IntPtr)
            'Called when a game is exited
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Try
                Dim strTable As String = ""
                Try
                    strTable = Info.GameName.ToString
                Catch ex As Exception
                    strTable = ""
                End Try
                Dim strSystem As String = ""
                Try
                    strSystem = Info.SystemName.ToString
                Catch ex As Exception
                    strSystem = ""
                End Try
                blnInGame = False
                If _carousel IsNot Nothing Then
                    _carousel.Resume()
                    _FLEXDMD.LoadFonts()
                End If
                LogInfo($"Event: GameExit: Table - [{strTable}] System - [{strSystem}]")
            Catch ex As Exception
                LogError($" Error: {ex.Message}")
            End Try
        End Sub
        Public Sub Process_App_Exit()
            Try
                If _carousel IsNot Nothing Then
                    _carousel.Pause()
                End If
                If _FLEXDMD IsNot Nothing Then
                    _FLEXDMD.ShowMessage("Thats all folks!")
                    System.Threading.Thread.Sleep(1500)
                End If
                If _carousel IsNot Nothing Then
                    _carousel.Stop()
                End If

                LogInfo("Event: App_Exit succesfully")
            Catch ex As Exception
                LogError($"Event: App_Exit Fail: {ex.Message}")
            End Try
        End Sub


#Region "PinballX Events"

        Public Sub Event_App_Exit()
            'called when pinballx exits
            LogInfo("Event: App_Exit")
            Process_App_Exit()
            LogInfo("Event: App_Exit Completed")
        End Sub

        Public Sub Event_GameSelect(ByVal InfoPtr As IntPtr)
            Process_GameSelect(InfoPtr)
        End Sub

        Public Function Event_GameRun(ByVal InfoPtr As IntPtr) As Boolean
            Try
                LogInfo("Event: GameRun ")
                Return Process_GameRun(InfoPtr)
            Catch ex As Exception
                LogError($"Event: GameRun Error: {ex.Message}")
                Return False
            End Try

        End Function

        Public Function Event_Parameters(ByVal InfoPtr As IntPtr) As String
            'Called when PinballX builds the command line for the game
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Dim CmdLine As String = Info.Parameters
            LogInfo($"Event: Parameters - CmdLine:[{CmdLine}]")
            Return CmdLine
        End Function

        Public Sub Event_GameExit(ByVal InfoPtr As IntPtr)
            Process_GameExit(InfoPtr)
        End Sub

        Public Function Event_ScreenSaver(ByVal Type As Integer) As Boolean
            'Called when PinballX enters and exits Screensaver Mode
            Try
                Select Case Type
                    Case 1 'Enter Screensaver
                    Case 2 'Exit Screensaver
                End Select
            Catch ex As Exception
                LogError($"Event: ScreenSaver Error: {ex.Message}")
            End Try
            Return True
        End Function
        Public Function Event_Input(Input_Keys() As Boolean, Input_Buttons() As Boolean, PinballXStatus As Integer) As Boolean
            'Called when a button has been pressed
            If Input_Keys(System.Windows.Forms.Keys.LShiftKey) Then
                ' Left Shift Key Down
            End If
            If Input_Buttons(0) Then
                ' Gamepad button 1 down
            End If
        End Function

        Public Sub Dispose()
            Try
                LogInfo("Dispose called")
                Dispose(True)
                GC.SuppressFinalize(Me)
            Catch ex As Exception
                If Logger IsNot Nothing Then LogError($"Dispose Error: {ex.Message}")
            End Try
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposed Then
                If disposing Then

                    LogInfo("Plugin Disposed Successfully")
                End If
            End If
            disposed = True
        End Sub

        ' --- PinballX Interface Properties ---
        Public ReadOnly Property Name() As String
            Get
                Return PluginInfo.Name
            End Get
        End Property

        Public ReadOnly Property Version() As String
            Get
                Return PluginInfo.Version
            End Get
        End Property

        Public ReadOnly Property Author() As String
            Get
                Return PluginInfo.Author
            End Get
        End Property

        Public ReadOnly Property Description() As String
            Get
                Return PluginInfo.Description
            End Get
        End Property

        Public ReadOnly Property PluginVersion() As Integer
            Get
                Return Val(PluginInfo.PluginVersion)
            End Get
        End Property

        Public Sub Configure()
            Dim config As New Configuration
            config.ShowDialog()
        End Sub
#End Region


    End Class
End Namespace