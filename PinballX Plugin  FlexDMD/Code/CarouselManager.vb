Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Linq
Imports System.Timers
Imports System.Windows.Forms
Imports System.Windows.Forms.LinkLabel

Public Class CarouselManager
    Private _dmd As FlexDMDEngine
    Private WithEvents _timer As System.Timers.Timer
    Private _isUpdating As Boolean = False
    Private _stopwatch As Stopwatch

    ' --- General Settings ---
    Public Property UseHd As Boolean = False
    Public Property ShowMedia As Boolean = True
    Public Property MaxVideoLoops As Integer = 3
    Public Property ImageDuration As Double = 0
    Public Property ShowPBXHighscores As Boolean = True

    ' --- Clock Settings ---
    Public Property ShowClock As Boolean = True
    Public Property ShowClockEverySeconds As Integer = 60
    Public Property ClockDuration As Double = 10.0
    Public Property Show24h As Boolean = True
    Public Property ShowScrollingInfo As Boolean = True

    ' --- PinemHi / Splash ---
    Public Property ShowPinemHiCountdown As Boolean = True
    Public Property ExtraSeconds As Integer = 10
    Public Property Weekly5MinChallengeTable As String = ""

    Public Property ShowStartMessage As Boolean = False
    Public Property ShowExitMessage As Boolean = True
    Public Property ShowPinemHiScores As Boolean = False
    Public Property ShowPinemHiBadges As Boolean = False
    Public Property PinemHighBadgesEarned As String = "Pinemhi Badges Earned"
    Public Property PinemHighNoBadgesEarned As String = "No Pinemhi badges earned for this table yet."
    Public Property PinemHighBadgesEarnedtext As String = ""
    Public Property PinemHighNoBadgestext As String = ""


    Private Enum EngineState
        Idle
        Pause
        Splash
        Media
        Badges
        Highscores
        ScrollHorizontal
        Clock
        Countdown
    End Enum

    Private _state As EngineState = EngineState.Splash
    Private _stateStartTime As Double = 0
    Private _stateTargetDuration As Double = 0.0
    Private _videoLoopsCount As Integer = 0
    Private _isVideo As Boolean = False
    Private _lastClockTime As Double = 0
    Private _isTransitioning As Boolean = False
    Private _nextStateAfterInterrupt As EngineState = EngineState.Media
    Private _countdownEnd As DateTime

    Private _pendingMediaFile As String = ""
    Private _pendingHighscoreTexts As New List(Of String)
    Private _pendingScrollText As String = ""
    Private _hasNewData As Boolean = False
    Private _pendingBadges As List(Of PinemHiManager.BadgeResult)

    Private CurrentMediaFile As String = ""
    Private CurrentHighscoreTexts As New List(Of String)
    Private _currentHighscoreIndex As Integer = 0
    Private _resumeHighscoresAfterClock As Boolean = False
    Private CurrentScrollText As String = ""

    Private CurrentBadges As List(Of PinemHiManager.BadgeResult)

    Public Sub New(wrapper As FlexDMDEngine)
        _dmd = wrapper
        _stopwatch = Stopwatch.StartNew()
        _timer = New System.Timers.Timer(30)
        _dmd.CalculateClockFont()
    End Sub

    Public Sub Start()
        LogDebug("Start of Carousel Engine Start.")
        _state = EngineState.Media
        _stateStartTime = _stopwatch.Elapsed.TotalSeconds
        _lastClockTime = _stateStartTime
        _timer.Start()
        LogDebug("Carousel Engine Started Succesfully.")
    End Sub

    Public Sub [Stop]()
        LogDebug("Stopping Carousel Engine.")
        _timer.Stop()
        _state = EngineState.Pause
        _dmd.ClearDMD()
        _dmd.HideDMD()
        LogDebug("Carousel Engine Stopped.")
    End Sub

    Public Sub Pause()
        _timer.Stop()
        _dmd.ClearDMD()
        LogDebug("Carousel Paused.")
    End Sub

    Public Sub [Resume]()
        LogDebug("Resuming Carousel Engine.")
        _hasNewData = True
        _state = EngineState.Media
        _stateStartTime = _stopwatch.Elapsed.TotalSeconds

        _dmd.ResumeDMD()

        System.Threading.Thread.Sleep(100) ' Wait briefly before starting the barrage
        _timer.Start()
        LogDebug("Carousel Resumed.")
    End Sub
    Public Sub StartWithSplash(line1 As String, line2 As String, line3 As String)
        LogDebug("Carousel start with splash")
        _state = EngineState.Splash
        _stateStartTime = _stopwatch.Elapsed.TotalSeconds
        _lastClockTime = _stateStartTime
        _dmd.StartSplash(line1, line2, line3, _stateStartTime)
        _timer.Start()
        LogDebug("Carousel Engine Started with Splash.")
    End Sub

    Public Sub UpdateGameInfo(mediaFile As String, HighscoreTexts As List(Of String), badges As List(Of PinemHiManager.BadgeResult), Optional scrollInfo As String = "")
        _pendingMediaFile = mediaFile
        _pendingHighscoreTexts = HighscoreTexts
        _pendingScrollText = scrollInfo
        _pendingBadges = badges
        _hasNewData = True
        _state = EngineState.Media
        LogDebug("New table data received: " & Path.GetFileName(mediaFile))
    End Sub

    Private Sub _timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _timer.Elapsed
        If _isUpdating OrElse _state = EngineState.Pause Then Exit Sub
        _isUpdating = True

        Try
            Dim currentSec As Double = _stopwatch.Elapsed.TotalSeconds
            Dim elapsed As Double = currentSec - _stateStartTime

            If _hasNewData Then
                _hasNewData = False
                _stateTargetDuration = 0

                _dmd.ClearDMD()

                CurrentMediaFile = _pendingMediaFile
                CurrentHighscoreTexts = _pendingHighscoreTexts
                CurrentScrollText = _pendingScrollText
                CurrentBadges = _pendingBadges

                _lastClockTime = currentSec

                _state = EngineState.Idle
                GoToNextState(currentSec)

                _isUpdating = False
                Exit Sub
            End If

            Select Case _state
                Case EngineState.Splash
                    _dmd.UpdateSplash(currentSec)
                Case EngineState.Media
                    'LogDebug($"Engine State: Media. Elapsed: {elapsed:F1}s, Target: {_stateTargetDuration:F1}s, Loops: {_videoLoopsCount}, IsVideo: {_isVideo}")
                    If elapsed >= _stateTargetDuration Then
                        If _isVideo AndAlso _videoLoopsCount < (MaxVideoLoops - 1) Then
                            _videoLoopsCount += 1
                            LogInfo($"Media Loop: {_videoLoopsCount + 1} of {MaxVideoLoops} for {Path.GetFileName(CurrentMediaFile)}")
                            _stateStartTime = currentSec
                            _dmd.ShowMedia(CurrentMediaFile)
                        Else
                            LogDebug("Media finished. Moving to Next Sequence.")
                            GoToNextState(currentSec)
                        End If
                    End If

                Case EngineState.ScrollHorizontal

                    If Not _dmd.UpdateHorizontalScroll() Then
                        LogDebug("Scroll finished. Moving to Next Sequence.")
                        GoToNextState(currentSec)
                    End If

                Case EngineState.Highscores
                    Dim isHighscoreActive As Boolean = _dmd.UpdateVerticalScroll()

                    If Not isHighscoreActive Then
                        _currentHighscoreIndex += 1

                        If _currentHighscoreIndex >= CurrentHighscoreTexts.Count Then
                            LogDebug("All HighScore segments done ")
                            _resumeHighscoresAfterClock = False
                            GoToNextState(currentSec)
                        Else
                            If ShowClock AndAlso (currentSec - _lastClockTime) >= ShowClockEverySeconds Then
                                LogDebug("Interrupt: Clock time reached during Highscore sequence. Pausing highscores.")
                                _resumeHighscoresAfterClock = True ' keep track that we want to resume highscores after clock
                                SetState(EngineState.Clock, currentSec)
                            Else
                                LogDebug($"Starting next HighScore segment index: {_currentHighscoreIndex} with text: {vbCrLf}{CurrentHighscoreTexts(_currentHighscoreIndex)}")
                                _dmd.SetupVerticalScroll(CurrentHighscoreTexts(_currentHighscoreIndex))
                                _stateStartTime = currentSec
                            End If
                        End If

                    ElseIf elapsed > 60.0 Then ' 60 seconds time out
                        _currentHighscoreIndex += 1
                        If _currentHighscoreIndex >= CurrentHighscoreTexts.Count Then
                            LogDebug("Highscore segment timeout and all segments done. Moving to next state.")
                            GoToNextState(currentSec)
                        Else
                            LogDebug($"Highscore segment timeout. Moving to next index: {_currentHighscoreIndex} with text: {CurrentHighscoreTexts(_currentHighscoreIndex)}")
                            _dmd.SetupVerticalScroll(CurrentHighscoreTexts(_currentHighscoreIndex))
                            _stateStartTime = currentSec
                        End If
                    End If
                Case EngineState.Clock
                    If elapsed >= _stateTargetDuration Then
                        If _resumeHighscoresAfterClock Then
                            _resumeHighscoresAfterClock = False
                            _state = EngineState.Highscores
                            _stateStartTime = currentSec
                            LogDebug($"Clock finished. Resuming highscores where we left off. HighScore segment index: {_currentHighscoreIndex} with text: {CurrentHighscoreTexts(_currentHighscoreIndex)}")
                            _dmd.SetupVerticalScroll(CurrentHighscoreTexts(_currentHighscoreIndex))

                        Else
                            LogDebug("Clock finished. Moving to Next Sequence.")
                            GoToNextState(currentSec)
                        End If
                    Else
                        _dmd.UpdateClock(Show24h)
                    End If

                Case EngineState.Idle
                    If Not String.IsNullOrEmpty(CurrentMediaFile) Then GoToNextState(currentSec)
                Case EngineState.Countdown
                    Dim now As DateTime = DateTime.Now
                    Dim remaining As TimeSpan = _countdownEnd - now
                    If remaining.TotalSeconds <= 0 Then
                        LogDebug("Countdown finished. ")
                    Else
                        _dmd.UpdateCountdown(remaining, Show24h)
                    End If
                Case EngineState.Badges
                    If _dmd.UpdateBadges() Then
                        LogDebug("Badges scroll finished. Moving to next state.")
                        GoToNextState(currentSec)
                    End If
            End Select

        Catch ex As Exception
            LogError("Timer Error: " & ex.Message)
        Finally
            _isUpdating = False
        End Try
    End Sub

    Private Function GetNextLogicalState(currentState As EngineState) As EngineState
        Select Case currentState
            Case EngineState.Media
                Return EngineState.Badges
            Case EngineState.Badges
                Return EngineState.ScrollHorizontal
            Case EngineState.ScrollHorizontal
                Return EngineState.Highscores
            Case Else
                Return EngineState.Media
        End Select
    End Function


    Private Sub GoToNextState(currentSec As Double)
        If _isTransitioning Then Exit Sub
        _isTransitioning = True

        Try
            Dim nextState As EngineState

            If _state = EngineState.Clock Then
                LogDebug($"Clock finished. Resuming carousel at {_nextStateAfterInterrupt}")
                nextState = _nextStateAfterInterrupt
            Else
                nextState = GetNextLogicalState(_state)
            End If

            If ShowClock AndAlso (currentSec - _lastClockTime >= ShowClockEverySeconds) Then
                LogDebug($"Interrupt: Clock time reached. Queuing {nextState} for later.")
                _nextStateAfterInterrupt = nextState
                nextState = EngineState.Clock
            End If

            Dim stateStarted As Boolean = False
            Dim loopCount As Integer = 0

            While Not stateStarted AndAlso loopCount < 5
                loopCount += 1
                LogDebug($"Setting Engine State to: {nextState.ToString}")

                _state = nextState
                _stateStartTime = currentSec

                Select Case _state
                    Case EngineState.Media
                        _videoLoopsCount = 0
                        stateStarted = TryMedia()

                    Case EngineState.Badges
                        stateStarted = TryBadges()

                    Case EngineState.ScrollHorizontal
                        stateStarted = TryScroll()

                    Case EngineState.Highscores
                        stateStarted = TryHighscores()

                    Case EngineState.Clock
                        _lastClockTime = currentSec
                        _stateTargetDuration = If(ClockDuration > 0, ClockDuration, 0)
                        _dmd.SetupClock(Show24h)
                        stateStarted = True
                End Select

                If Not stateStarted Then
                    LogDebug($"{_state} failed or skipped. Moving to next logical state.")
                    nextState = GetNextLogicalState(_state)
                End If
            End While

            If Not stateStarted Then
                If ShowClock Then
                    LogDebug("All carousel actions empty. Falling back to continuous Clock.")
                    _state = EngineState.Clock
                    _stateStartTime = currentSec
                    _stateTargetDuration = Double.MaxValue ' run indefinitely until next interrupt
                    _dmd.SetupClock(Show24h)
                Else
                    LogDebug("All carousel actions skipped or empty. Going to Idle.")
                    _state = EngineState.Idle
                End If
            End If

        Catch ex As Exception
            LogError("GoToNextState Error: " & ex.Message)
        Finally
            _isTransitioning = False
        End Try
    End Sub
    Private Sub SetState(nextState As EngineState, currentSec As Double)
        LogDebug($"Setting Engine State to: {nextState.ToString}")
        _state = nextState
        _stateStartTime = currentSec

        Select Case _state
            Case EngineState.Media
                _videoLoopsCount = 0
                If Not TryMedia() Then
                    LogDebug("TryMedia failed/skipped. Moving to next.")
                    GoToNextState(currentSec)
                End If
            Case EngineState.Badges
                If Not TryBadges() Then
                    LogDebug("TryBadges failed/skipped. Moving to next.")
                    GoToNextState(currentSec)
                End If
            Case EngineState.ScrollHorizontal
                If Not TryScroll() Then
                    LogDebug("TryScroll failed/skipped. Moving to next.")
                    GoToNextState(currentSec)
                End If

            Case EngineState.Highscores
                If Not TryHighscores() Then
                    LogDebug("TryHighscores failed/skipped. Moving to next.")
                    GoToNextState(currentSec)
                End If

            Case EngineState.Clock
                _lastClockTime = currentSec
                _stateTargetDuration = If(ClockDuration > 0, ClockDuration, 0)
                _dmd.SetupClock(Show24h)
        End Select
    End Sub

    Private Function TryMedia() As Boolean
        If ShowMedia AndAlso Not String.IsNullOrEmpty(CurrentMediaFile) AndAlso File.Exists(CurrentMediaFile) AndAlso Not MaxVideoLoops <= 0 Then
            _dmd.ShowMedia(CurrentMediaFile)

            Dim ext = Path.GetExtension(CurrentMediaFile).ToLower()
            _isVideo = (ext = ".mp4" Or ext = ".avi" Or ext = ".gif" Or ext = ".wmv")

            If _isVideo Then
                _stateTargetDuration = GetAnimationDuration(CurrentMediaFile)
                LogDebug($"Detected media duration: {_stateTargetDuration} seconds for file {CurrentMediaFile}")
            Else
                _stateTargetDuration = If(ImageDuration > 100, ImageDuration / 1000, ImageDuration)
            End If
            Return True
        End If
        Return False
    End Function

    Private Function TryPBXHighscore() As Boolean
        'LogDebug($"TryPBXHighscore: check: ShowPBXHighscores={ShowPBXHighscores}, TextLength={If(CurrentHighscoreText IsNot Nothing, CurrentHighscoreText.Length, 0)}")

        If ShowPBXHighscores AndAlso CurrentHighscoreTexts.Count > 0 Then
            _currentHighscoreIndex = 0
            _dmd.SetupVerticalScroll(CurrentHighscoreTexts(_currentHighscoreIndex))
            Return True
        End If

        LogDebug("TryPBXHighscore: No data or disabled. Skipping.")
        Return False
    End Function
    Private Function TryHighscores() As Boolean

        If CurrentHighscoreTexts IsNot Nothing AndAlso CurrentHighscoreTexts.Count > 0 Then
            _currentHighscoreIndex = 0
            LogDebug($"TryHighscores HighScore segment index: {_currentHighscoreIndex} with cleaned text: {CurrentHighscoreTexts(_currentHighscoreIndex)}")
            _dmd.SetupVerticalScroll(CurrentHighscoreTexts(_currentHighscoreIndex))
            Return True
        End If

        LogDebug("TryHighscores: No data or disabled. Skipping.")
        Return False
    End Function
    Private Function TryScroll() As Boolean
        If ShowScrollingInfo AndAlso Not String.IsNullOrEmpty(CurrentScrollText) Then
            _dmd.SetupHorizontalScroll(CurrentScrollText)
            _stateTargetDuration = 0 ' Reset duration
            Return True
        End If
        Return False
    End Function
    Public Sub StartPinemHiCountdown(seconds As Integer, challengeText As String)
        Dim totalSeconds As Integer = seconds + ExtraSeconds
        LogInfo($"PinemHi Interrupt: {totalSeconds}s - {challengeText}")
        _countdownEnd = DateTime.Now.AddSeconds(totalSeconds)

        _state = EngineState.Countdown
        _stateStartTime = _stopwatch.Elapsed.TotalSeconds
        _stateTargetDuration = totalSeconds

        _dmd.SetupCountdown(totalSeconds, challengeText, Show24h)
    End Sub
    Private Function TryBadges() As Boolean
        If Not ShowPinemHiBadges OrElse CurrentBadges Is Nothing OrElse (CurrentBadges.Count = 0 AndAlso String.IsNullOrEmpty(PinemHighNoBadgestext)) Then
            LogDebug("no badges to display")
            Return False
        End If

        If _dmd.SetupBadges(CurrentBadges, PinemHighBadgesEarnedtext, PinemHighNoBadgestext) Then
            LogDebug("Started badge/no-badge scroll sequence.")
            Return True
        End If

        Return False
    End Function
End Class