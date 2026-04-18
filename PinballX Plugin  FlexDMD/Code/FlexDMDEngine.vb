Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Win32

Public Class FlexDMDEngine
    Private flex As Object
    Private Scene As Object
    Private Font(6) As Object
    Private renderLock As New Object()

    Public Property Width As Integer = 128
    Public Property Height As Integer = 32
    Public Property SplashDuration As Double = 3.0
    Public Property DllPath As String = String.Empty

    Private splashActors As New List(Of Object)
    Private splashStart As Double
    Private clockFontIndex As Integer
    Private _fontsLoaded As Boolean = False

    Private scrollActor As Object
    Private isScrollHorizontal As Boolean
    Private scrollTextWidth As Integer
    Private scrollTextHeight As Integer
    Private clockLabel As Object

    Private verticalScrollGroup As Object
    Private verticalScrollY As Double
    Private verticalScrollHeight As Double

    Private horizontalScrollGroup As Object
    Private horizontalScrollX As Double
    Private horizontalScrollWidth As Double

    Private badgeScrollGroup As Object
    Private badgeScrollX As Double
    Private badgeTotalWidth As Double
    Public Property BadgeScrollSpeed As Double = 1.8

    Private countdownChallengeLabel As Object
    Private countdownTimeLabel As Object
    Private countdownClockLabel As Object
    Private Enum XDMDFont
        bmp4by5 = 0
        bmp5by7 = 1
        bmp6by12 = 2
        bmp7by13 = 3
        bmp12by24 = 4
        bmp14by26 = 5
    End Enum

    Private _DMDColor As Color = Color.OrangeRed
    Public Property DMDColor As Color
        Get
            Return _DMDColor
        End Get
        Set(value As Color)
            _DMDColor = value
            For i As Integer = 0 To Font.Length - 1
                If Font(i) IsNot Nothing Then
                    Try
                        Font(i).SetColor(_DMDColor, _DMDColor)
                    Catch ex As Exception
                        LogError($"Failed to update color for font index {i}: {ex.Message}")
                    End Try
                End If
            Next
        End Set
    End Property

    Private _show24h As Boolean = True
    Public Property Show24h As Boolean
        Get
            Return _show24h
        End Get
        Set(value As Boolean)
            _show24h = value
            CalculateClockFont()
        End Set
    End Property

    Public Function Init(useHD As Boolean) As Boolean
        Try
            LogInfo("Initialising FlexDMD...")
            Return CreateFlexManager(useHD)
        Catch ex As Exception
            LogError("FlexDMDWrapper Init Failed: " & ex.Message)
            Return False
        End Try
    End Function

    Private Function CreateFlexManager(useHD As Boolean) As Boolean
        Try
            Dim blnCore As Boolean = Type.GetType("System.Runtime.Loader.AssemblyLoadContext") IsNot Nothing

            If blnCore Then
                Dim basePath As String

                If Not String.IsNullOrEmpty(DllPath) AndAlso
                   File.Exists(Path.Combine(DllPath, "FlexDMD.dll")) Then

                    basePath = DllPath
                    LogDebug($"Trying to use custom FlexDMD DLL from path: {basePath}")

                Else
                    basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PLUGINS\FLEXDMD")
                End If

                Dim dllFile As String = Path.Combine(basePath, "FlexDMD.dll")

                Dim iniFile As String = Path.Combine(basePath, "dmddevice.ini")
                If File.Exists(iniFile) Then
                    LogInfo("FlexDMD folder contains dmddevice.ini → removing DMDDEVICE_CONFIG environment variable")
                    Environment.SetEnvironmentVariable("DMDDEVICE_CONFIG", Nothing, EnvironmentVariableTarget.Process)
                End If
                Dim strFlexPath As String = If(File.Exists(dllFile),
                                               dllFile,
                                               SearchRegistry("FlexDMD.dll").ToString())

                LogInfo($"Using FlexDMD DLL from {strFlexPath}")

                Dim ass As Reflection.Assembly = Reflection.Assembly.LoadFrom(strFlexPath)
                Dim ty As Type = ass.GetType("FlexDMD.FlexDMD")
                flex = Activator.CreateInstance(ty)
            Else
                LogInfo("Using COM FlexDMD object")
                flex = CreateObject("FlexDMD.FlexDMD")
            End If

            Width = If(useHD, 256, 128)
            Height = If(useHD, 64, 32)

            flex.RenderMode = 2
            flex.Width = Width
            flex.Height = Height
            flex.Clear = True
            flex.GameName = Path.GetFileNameWithoutExtension(Reflection.Assembly.GetExecutingAssembly().Location) ' = DLL name - for multiple instances

            Scene = flex.NewGroup("Scene")
            flex.LockRenderThread()
            flex.Stage.AddActor(Scene)
            flex.UnlockRenderThread()

            LoadFonts()

            flex.Run = True
            flex.Show = True

            Return True
        Catch ex As Exception
            LogError($"CreateFlexManager Failed: {ex.Message}")
            Return False
        End Try
    End Function
    Public Sub LoadFonts()
        _fontsLoaded = False
        Try
            If flex IsNot Nothing Then
                LogInfo("Loading fonts into video memory...")

                Dim fontFiles As String() = {
                    "FlexDMD.Resources.udmd-f4by5.fnt",
                    "FlexDMD.Resources.udmd-f5by7.fnt",
                    "FlexDMD.Resources.udmd-f6by12.fnt",
                    "FlexDMD.Resources.udmd-f7by13.fnt",
                    "FlexDMD.Resources.udmd-f12by24.fnt",
                    "FlexDMD.Resources.udmd-f14by26.fnt"
                }

                SyncLock renderLock
                    For i As Integer = 0 To fontFiles.Length - 1
                        Font(i) = Nothing
                        Font(i) = flex.NewFont(fontFiles(i), DMDColor, DMDColor, 0)
                    Next
                End SyncLock

                _fontsLoaded = True
                LogInfo("Fonts loaded successfully.")
            End If
        Catch ex As Exception
            LogError("LoadFonts Error: " & ex.Message)
            _fontsLoaded = False
        End Try
    End Sub

    Public Sub HideDMD()
        If flex IsNot Nothing Then
            flex.Run = False
            flex.Show = False
        End If
    End Sub

    Public Sub ResumeDMD()
        If flex IsNot Nothing Then
            flex.Run = True
            flex.Show = True
            ClearDMD()
        End If
    End Sub

    Public Sub ClearDMD()
        Try
            SyncLock renderLock
                If flex IsNot Nothing Then
                    flex.LockRenderThread()
                    flex.Clear()
                    flex.Stage.RemoveAll()
                    If Scene IsNot Nothing Then
                        Scene.RemoveAll()
                    End If
                    flex.UnlockRenderThread()
                End If
            End SyncLock
        Catch ex As Exception
            ' don't log this one, we call Clear a lot and we don't want to flood the logs if something is wrong with the render thread
        End Try
    End Sub

    Public Sub ShowMedia(file As String)
        SyncLock renderLock
            Try
                ClearDMD()
                flex.LockRenderThread()
                flex.Stage.RemoveAll()

                Dim ext As String = Path.GetExtension(file).ToLower()
                Dim isVideo As Boolean = (ext = ".mp4" OrElse ext = ".avi" OrElse ext = ".gif" OrElse ext = ".wmv")

                If IO.File.Exists(file) Then
                    LogDebug("Loading Media: " & file)
                    Dim actor As Object = Nothing

                    If isVideo Then
                        actor = flex.NewVideo("VideoPlayer", file)
                        If actor IsNot Nothing Then actor.Loop = False
                    Else
                        actor = flex.NewImage("ImagePlayer", file)
                    End If

                    If actor IsNot Nothing Then
                        actor.SetBounds(0, 0, Width, Height)
                        flex.Stage.AddActor(actor)
                    End If
                End If

                flex.UnlockRenderThread()
            Catch ex As Exception
                LogError("ShowMedia Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
            End Try
        End SyncLock
    End Sub
    Public Sub StartSplash(line1 As String, line2 As String, line3 As String, currentTimeSec As Double)
        Try
            SyncLock renderLock
                flex.LockRenderThread()
                Scene.RemoveAll()
                splashActors.Clear()

                Dim selectedFont = If(Width > 128, Font(3), Font(1))
                If selectedFont Is Nothing Then selectedFont = Font(0) ' Fallback

                Dim l1 = flex.NewLabel("L1", selectedFont, line1)
                Dim l2 = flex.NewLabel("L2", selectedFont, line2)
                Dim l3 = flex.NewLabel("L3", selectedFont, line3)

                l1.x = -Width : l1.y = 2
                l2.x = Width : l2.y = If(Width > 128, 22, 12)
                l3.x = 0 : l3.y = Height + 10

                l1.Alignment = 1 : l1.Width = Width
                l2.Alignment = 1 : l2.Width = Width
                l3.Alignment = 1 : l3.Width = Width

                Scene.AddActor(l1)
                Scene.AddActor(l2)
                Scene.AddActor(l3)

                splashActors.Add(l1)
                splashActors.Add(l2)
                splashActors.Add(l3)

                flex.UnlockRenderThread()
            End SyncLock

            splashStart = currentTimeSec
        Catch ex As Exception
            LogError("StartSplash Error: " & ex.Message)
            If flex IsNot Nothing Then flex.UnlockRenderThread()
        End Try
    End Sub

    Public Function UpdateSplash(currentTimeSec As Double) As Boolean
        Try
            Dim t = (currentTimeSec - splashStart) / SplashDuration
            If t > 1.0 Then Return False

            SyncLock renderLock
                flex.LockRenderThread()
                Dim ease = 1 - Math.Pow(1 - t, 3)

                splashActors(0).x = -Width + (Width * ease)
                splashActors(1).x = Width - (Width * ease)

                Dim finalY3 = If(Width > 128, 44, 22)
                splashActors(2).y = (Height + 10) - ((Height + 10 - finalY3) * ease)

                flex.UnlockRenderThread()
            End SyncLock
            Return True
        Catch ex As Exception
            LogError("UpdateSplash Error: " & ex.Message)
            Return False
        End Try
    End Function

    Public Sub ShowMessage(text As String)
        SyncLock renderLock
            LogDebug("ShowMessage: " & text)
            Try
                ClearDMD()
                If flex Is Nothing Then Return


                flex.LockRenderThread()

                Dim fontIdx As Integer = GetBestFontIndex(text.Length, 4)
                Dim activeFont = Font(fontIdx)
                If activeFont Is Nothing Then activeFont = Font(0)

                Dim lbl = flex.NewLabel("FinalMsg", activeFont, text)
                lbl.Alignment = 1 ' Center

                Dim textH As Integer = GetFontHeight(fontIdx)
                Dim yPos As Integer = (Height \ 2) - (textH \ 2)

                lbl.SetBounds(0, yPos, Width, textH)

                flex.Stage.AddActor(lbl)
                flex.UnlockRenderThread()

            Catch ex As Exception
                LogError("ShowMessage Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
            End Try
        End SyncLock
    End Sub
    Public Sub StartCountdown(seconds As Integer, challengeText As String)
        SyncLock renderLock
            If flex IsNot Nothing Then
                flex.LockRenderThread()
                Scene.RemoveAll()

                Dim activeFont = If(Font(1) IsNot Nothing, Font(1), Font(0))
                Dim lbl = flex.NewLabel("Countdown", activeFont, challengeText & " " & seconds)
                lbl.Alignment = 1
                lbl.SetBounds(0, 5, Width, 25)

                Scene.AddActor(lbl)
                flex.UnlockRenderThread()
            End If
        End SyncLock
    End Sub

    Public Sub ShowText(text As String)
        ClearDMD()
        SyncLock renderLock
            flex.LockRenderThread()
            Dim lbl = flex.NewLabel("ScoreText", Font(1), text)
            lbl.Alignment = 1
            lbl.SetBounds(0, 0, Width, Height)
            Scene.AddActor(lbl)
            flex.UnlockRenderThread()
        End SyncLock
    End Sub

    Public Sub SetupClock(show24h As Boolean)
        SyncLock renderLock
            Try
                If flex Is Nothing Then Exit Sub
                ClearDMD()

                flex.LockRenderThread()

                flex.Stage.RemoveAll()
                clockLabel = Nothing

                If clockFontIndex = 0 Then CalculateClockFont()

                Dim fullTime As String = DateTime.Now.ToString(If(show24h, "HH:mm:ss", "hh:mm:ss tt"))

                clockLabel = flex.NewLabel("ClockLabel", Font(clockFontIndex), fullTime)
                clockLabel.Alignment = 1

                Dim fHeight = GetFontHeight(clockFontIndex)
                Dim yPos = Math.Max(0, (Height - fHeight) \ 2)

                clockLabel.Width = Width
                clockLabel.SetBounds(0, yPos, Width, fHeight)

                flex.Stage.AddActor(clockLabel)

                flex.UnlockRenderThread()
                LogDebug($"Clock Setup: FontIdx {clockFontIndex}, Y-Pos {yPos}, Width {Width}")
            Catch ex As Exception
                LogError("SetupClock Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
            End Try
        End SyncLock
    End Sub
    Public Sub UpdateClock(show24h As Boolean)
        Try
            SyncLock renderLock
                If flex Is Nothing Then Exit Sub
                If clockLabel Is Nothing Then
                    flex.UnlockRenderThread()
                    SetupClock(show24h)
                    Exit Sub
                End If

                flex.LockRenderThread()

                Dim now As DateTime = DateTime.Now
                Dim separator As String = If(now.Millisecond < 500, ":", ".")

                Dim hFormat As String = If(show24h, "HH", "hh")
                Dim timeStr As String = now.ToString(hFormat) & ":" &
                                   now.ToString("mm") & separator &
                                   now.ToString("ss")

                If Not show24h Then timeStr &= now.ToString(" tt")

                clockLabel.Text = timeStr
                clockLabel.Alignment = 1

                Dim fHeight = GetFontHeight(clockFontIndex)
                Dim yPos = Math.Max(0, (Height - fHeight) \ 2)

                clockLabel.Width = Width
                clockLabel.SetBounds(0, yPos, Width, fHeight)

                flex.UnlockRenderThread()
            End SyncLock
        Catch ex As Exception
            LogError("UpdateClock Error: " & ex.Message)
            If flex IsNot Nothing Then flex.UnlockRenderThread()
        End Try
    End Sub

    Public Sub CalculateClockFont()
        Try

            Dim maxChars As Integer = If(Show24h, 8, 11)
            clockFontIndex = GetBestFontIndex(maxChars, 4)
            LogInfo($"Clock font calculated at index: {clockFontIndex} (Width: {GetFontWidth(clockFontIndex)})")
        Catch ex As Exception
            LogError("CalculateClockFont Error: " & ex.Message)
        End Try

    End Sub

    Private Function GetBestFontIndex(maxChars As Integer, Optional margin As Integer = 10) As Integer
        Dim bestIndex As Integer = 0

        Try
            For i As Integer = 5 To 0 Step -1
                If (maxChars * GetFontWidth(i)) < (Width - margin) Then
                    bestIndex = i
                    Exit For
                End If
            Next
        Catch ex As Exception
            PinballX.Plugin.Logger.Log_Error("GetBestFontIndex Error: Failed to calculate font index. " & ex.Message)
            bestIndex = 0
        End Try

        Return bestIndex
    End Function

    Private Function GetFontWidth(fontIndex As Integer) As Integer
        Select Case fontIndex
            Case 0 : Return 4
            Case 1 : Return 5
            Case 2 : Return 6
            Case 3 : Return 7
            Case 4 : Return 12
            Case 5 : Return 14
            Case Else : Return 5
        End Select
    End Function

    Public Function GetFontHeight(fontIndex As Integer) As Integer
        Select Case fontIndex
            Case 0 : Return 6
            Case 1 : Return 8
            Case 2 : Return 12
            Case 3 : Return 13
            Case 4 : Return 24
            Case 5 : Return 26
            Case Else : Return 10
        End Select
    End Function
    Public Shared Function SearchRegistry(ByVal dllName As String) As String
        Dim t_clsidKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("CLSID")
        If t_clsidKey Is Nothing Then Return Nothing

        For Each subKey In t_clsidKey.GetSubKeyNames()
            Try
                Dim t_clsidSubKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("CLSID\" & subKey & "\InProcServer32")
                If t_clsidSubKey IsNot Nothing Then
                    Dim keys() As String = t_clsidSubKey.GetValueNames()
                    For Each key As String In keys
                        Dim t_value As String = CStr(t_clsidSubKey.GetValue(key, "")).ToUpper
                        If t_value.EndsWith(dllName.ToUpper) Then
                            Return t_value.Replace("FILE:///", "")
                        End If
                    Next key
                End If
            Catch
            End Try
        Next
        Return Nothing
    End Function

    Public Sub SetupVerticalScroll(text As String)
        Try
            If flex Is Nothing Then Exit Sub
            ClearDMD()
            flex.LockRenderThread()

            Dim lines() As String = text.Split({vbCrLf, vbLf, "|"}, StringSplitOptions.None)

            Dim bestFontIndex As Integer = 0
            Dim margin As Integer = 4

            For fIdx As Integer = 5 To 0 Step -1
                Dim fits As Boolean = True
                For Each l In lines
                    Dim tempLabel = flex.NewLabel("Temp", Font(fIdx), l)
                    If tempLabel.Width > (Width - margin) Then
                        fits = False
                        Exit For ' won't fit, try smaller font
                    End If
                Next
                If fits Then
                    bestFontIndex = fIdx
                    Exit For ' Houston, we have a font!
                End If
            Next

            ' --- Optioneel: Debug de langste regel voor in je logbestand ---
            Dim longestText As String = ""
            Dim maxW As Integer = 0
            For Each l In lines
                Dim tempLabel = flex.NewLabel("Temp", Font(bestFontIndex), l)
                If tempLabel.Width > maxW Then
                    maxW = tempLabel.Width
                    longestText = l
                End If
            Next
            LogDebug($"Longest line measured: '{longestText}' (Width: {maxW}px using FontIdx {bestFontIndex})")
            verticalScrollGroup = flex.NewGroup("VerticalScrollGroup")
            Dim cleanText As String = String.Join(vbCrLf, lines)
            Dim label = flex.NewLabel("HSLabel", Font(bestFontIndex), cleanText)
            label.Alignment = 1 ' Center

            verticalScrollHeight = label.Height
            LogDebug($"Measured vertical scroll block height: {verticalScrollHeight}px for text width: {Width}px")
            label.SetBounds(0, 0, Width, verticalScrollHeight)

            verticalScrollY = Height
            verticalScrollGroup.Y = verticalScrollY

            verticalScrollGroup.AddActor(label)
            flex.Stage.AddActor(verticalScrollGroup)

            flex.UnlockRenderThread()
            LogDebug($"Vertical Scroll: FontIdx {bestFontIndex}, Actual measured height {verticalScrollHeight}px")

        Catch ex As Exception
            LogError("SetupVerticalScroll Error: " & ex.Message)
            If flex IsNot Nothing Then flex.UnlockRenderThread()
        End Try
    End Sub
    Public Function UpdateVerticalScroll() As Boolean
        Dim needScroll As Boolean = False

        Try
            SyncLock renderLock
                If verticalScrollGroup IsNot Nothing Then

                    flex.LockRenderThread()

                    Dim viewportHeight As Double = flex.Height

                    verticalScrollY -= If(Width = 256, 0.8, 0.4)

                    If verticalScrollY <= -verticalScrollHeight Then
                        verticalScrollY = -verticalScrollHeight
                        needScroll = False
                    Else
                        needScroll = True
                    End If

                    verticalScrollGroup.y = verticalScrollY

                    flex.UnlockRenderThread()
                End If
            End SyncLock

        Catch ex As Exception
            LogError("UpdateVerticalScroll Error: " & ex.Message)
            If flex IsNot Nothing Then flex.UnlockRenderThread()
        End Try

        Return needScroll
    End Function

    Public Sub SetupCountdown(seconds As Integer, challengeText As String, show24h As Boolean)
        SyncLock renderLock
            Try
                ClearDMD()
                If flex Is Nothing Then Exit Sub
                flex.LockRenderThread()
                flex.Stage.RemoveAll()
                countdownChallengeLabel = flex.NewLabel("ChallengeTxt", Font(0), challengeText)
                countdownChallengeLabel.Alignment = 1
                countdownChallengeLabel.SetBounds(0, 2, Width, GetFontHeight(0))
                flex.Stage.AddActor(countdownChallengeLabel)

                Dim fontIdx As Integer = If(Height <= 32, 2, 4) ' BMP6x12 or BMP12x24
                Dim textH As Integer = GetFontHeight(fontIdx)
                Dim yPos As Integer = (Height \ 2) - (textH \ 2)

                countdownTimeLabel = flex.NewLabel("CountLabel", Font(fontIdx), "00;:00")
                countdownTimeLabel.Alignment = 1
                countdownTimeLabel.SetBounds(0, yPos, Width, textH)
                flex.Stage.AddActor(countdownTimeLabel)

                Dim timeFontIdx As Integer = 0
                Dim clockH As Integer = GetFontHeight(timeFontIdx)
                countdownClockLabel = flex.NewLabel("SmallClock", Font(timeFontIdx), "")
                countdownClockLabel.Alignment = 2 ' Right
                countdownClockLabel.SetBounds(0, Height - clockH, Width - 2, clockH)
                flex.Stage.AddActor(countdownClockLabel)
                flex.UnlockRenderThread()
            Catch ex As Exception
                LogError("SetupCountdown Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
            End Try
        End SyncLock
    End Sub
    Public Sub UpdateCountdown(remaining As TimeSpan, show24h As Boolean)
        SyncLock renderLock
            Try
                If flex Is Nothing OrElse countdownTimeLabel Is Nothing Then Exit Sub
                flex.LockRenderThread()

                Dim totalSec As Double = remaining.TotalSeconds
                Dim showText As Boolean = True

                ' Blink logic
                If totalSec > 0 AndAlso totalSec <= 15 Then
                    Dim ms As Integer = DateTime.Now.Millisecond
                    If totalSec > 10 Then
                        showText = (ms < 500)
                    ElseIf totalSec > 5 Then
                        showText = (ms < 250 OrElse (ms > 500 AndAlso ms < 750))
                    Else
                        showText = (ms Mod 250 < 125)
                    End If
                ElseIf totalSec <= 0 Then
                    showText = True
                End If

                Dim countdownStr As String = $"{(Int(remaining.TotalMinutes)):00}:{remaining.Seconds:00}"
                countdownTimeLabel.Text = If(showText, countdownStr, "")

                countdownTimeLabel.Width = Width
                countdownTimeLabel.Alignment = 1

                If countdownClockLabel IsNot Nothing Then
                    countdownClockLabel.Text = DateTime.Now.ToString(If(show24h, "HH:mm", "hh:mm tt"))
                    countdownClockLabel.Width = Width - 2
                    countdownClockLabel.Alignment = 2 ' Right
                End If

                flex.UnlockRenderThread()
            Catch ex As Exception
                LogError("UpdateCountdown Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
            End Try
        End SyncLock
    End Sub

    Public Sub SetupHorizontalScroll(text As String)
        Try
            ClearDMD()
            SyncLock renderLock
                flex.LockRenderThread()
                Scene.RemoveAll()

                Dim fontIdx As Integer = If(Height > 32, 4, 2)

                horizontalScrollGroup = flex.NewGroup("HorizScrollGroup")
                Dim label = flex.NewLabel("ScrollLabel", Font(fontIdx), text)
                label.alignment = 0

                horizontalScrollWidth = text.Length * GetFontWidth(fontIdx)
                Dim textH As Integer = GetFontHeight(fontIdx)

                label.SetBounds(0, 0, horizontalScrollWidth, textH)

                horizontalScrollX = Width
                horizontalScrollGroup.x = CInt(horizontalScrollX)

                horizontalScrollGroup.y = (Height - textH) \ 2

                horizontalScrollGroup.AddActor(label)
                Scene.AddActor(horizontalScrollGroup)

                flex.UnlockRenderThread()
            End SyncLock
        Catch ex As Exception
            LogError("SetupHorizontalScroll Error: " & ex.Message)
            If flex IsNot Nothing Then flex.UnlockRenderThread()
        End Try
    End Sub
    Public Function UpdateHorizontalScroll() As Boolean
        SyncLock renderLock
            If horizontalScrollGroup Is Nothing Then Return False

            Dim isDone As Boolean = False
            Try
                flex.LockRenderThread()

                Dim scrollSpeed As Double = If(Height > 32, 1.8, 1.0)
                horizontalScrollX -= scrollSpeed
                horizontalScrollGroup.X = CInt(horizontalScrollX)

                If (horizontalScrollX + horizontalScrollWidth) < 0 Then
                    isDone = True
                End If

                flex.UnlockRenderThread()

                If isDone Then
                    LogDebug("UpdateHorizontalScroll: Scroll finished cleanly.")
                    Return False
                End If

                Return True
            Catch ex As Exception
                If flex IsNot Nothing Then flex.UnlockRenderThread()
                LogError("UpdateHorizontalScroll Error: " & ex.Message)
                Return False
            End Try
        End SyncLock
    End Function
    Public Function SetupBadges(badges As List(Of PinemHiManager.BadgeResult), titleText As String, noBadgesText As String) As Boolean

        SyncLock renderLock
            Try
                If flex Is Nothing Then Return False
                ClearDMD()
                flex.LockRenderThread()

                badgeScrollGroup = flex.NewGroup("BadgeScrollGroup")
                BadgeScrollSpeed = If(Height > 32, 1.8, 1.0)

                Dim fontIdx As Integer = If(Height > 32, 4, 2)
                Dim textH As Integer = GetFontHeight(fontIdx)
                Dim yPos As Integer = (Height \ 2) - (textH \ 2)
                Dim charWidth As Integer = If(fontIdx = 4, 14, 7)

                Dim hasBadges As Boolean = (badges IsNot Nothing AndAlso badges.Count > 0 AndAlso Height > 32)

                If hasBadges Then
                    Dim titleLabel = flex.NewLabel("BadgeTitle", Font(fontIdx), titleText.ToUpper)
                    Dim estimatedTextWidth As Integer = titleText.Length * charWidth
                    titleLabel.SetBounds(0, yPos, estimatedTextWidth, textH)
                    badgeScrollGroup.AddActor(titleLabel)

                    Dim iconSize As Integer = Height
                    Dim padding As Integer = 6
                    Dim currentX As Integer = estimatedTextWidth + 10

                    For i As Integer = 0 To badges.Count - 1
                        Dim b = badges(i)
                        If IO.File.Exists(b.ImagePath) Then
                            Dim imgActor = flex.NewImage("BScroll_" & i, b.ImagePath)
                            imgActor.SetBounds(currentX, 0, iconSize, iconSize)
                            badgeScrollGroup.AddActor(imgActor)
                            currentX += (iconSize + padding)
                        End If
                    Next
                    badgeTotalWidth = currentX
                Else
                    Dim displayRepoText As String = If(badges IsNot Nothing AndAlso badges.Count > 0, titleText, noBadgesText)

                    Dim estimatedTextWidth As Integer = displayRepoText.Length * charWidth
                    Dim emptyLabel = flex.NewLabel("BadgeTextOnly", Font(fontIdx), displayRepoText.ToUpper)
                    emptyLabel.SetBounds(0, yPos, estimatedTextWidth, textH)

                    badgeScrollGroup.AddActor(emptyLabel)
                    badgeTotalWidth = estimatedTextWidth
                End If

                badgeScrollX = Width + 5
                badgeScrollGroup.X = CInt(badgeScrollX)

                flex.Stage.AddActor(badgeScrollGroup)
                flex.UnlockRenderThread()

                Return True
            Catch ex As Exception
                LogError("SetupBadges Error: " & ex.Message)
                If flex IsNot Nothing Then flex.UnlockRenderThread()
                Return False
            End Try
        End SyncLock
    End Function
    Public Function UpdateBadges() As Boolean
        SyncLock renderLock
            If badgeScrollGroup Is Nothing Then Return True

            Dim isDone As Boolean = False
            Try
                flex.LockRenderThread()

                badgeScrollX -= BadgeScrollSpeed
                badgeScrollGroup.X = CInt(badgeScrollX)

                If (badgeScrollX + badgeTotalWidth) < 0 Then
                    isDone = True
                End If

                flex.UnlockRenderThread()

                If isDone Then
                    LogDebug("UpdateBadges: Scroll finished cleanly.")
                    Return True
                End If

                Return False
            Catch ex As Exception
                If flex IsNot Nothing Then flex.UnlockRenderThread()
                LogError("UpdateBadges Error: " & ex.Message)
                Return True
            End Try
        End SyncLock
    End Function

End Class