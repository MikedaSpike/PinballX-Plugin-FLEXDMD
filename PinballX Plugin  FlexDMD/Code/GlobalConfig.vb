Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms

Public Module GlobalConfig
    Public DebugMode As Boolean = False

    Public Sub LogDebug(msg As String)
        If DebugMode Then
            PinballX.Plugin.Logger.Log_Data("DEBUG: " & msg)
        End If
    End Sub

    Public Sub LogInfo(msg As String)
        PinballX.Plugin.Logger.Log_Data("INFO: " & msg)
    End Sub

    Public Sub LogError(msg As String)
        PinballX.Plugin.Logger.Log_Data("ERROR: " & msg)
    End Sub

    Function GetGifDuration(ByVal image As Image, ByVal Optional fps As Integer = 60) As TimeSpan?
        Dim minimumFrameDelay = (1000.0 / fps)
        If Not image.RawFormat.Equals(ImageFormat.Gif) Then Return TimeSpan.FromMilliseconds(0)
        If Not ImageAnimator.CanAnimate(image) Then Return TimeSpan.FromMilliseconds(0)
        Dim frameDimension = New FrameDimension(image.FrameDimensionsList(0))
        Dim frameCount = image.GetFrameCount(frameDimension)
        Dim totalDuration = 0

        For f = 0 To frameCount - 1
            Dim delayPropertyBytes = image.GetPropertyItem(20736).Value
            Dim frameDelay = BitConverter.ToInt32(delayPropertyBytes, f * 4) * 10
            totalDuration += (If(frameDelay < minimumFrameDelay, CInt(minimumFrameDelay), frameDelay))
        Next

        Return TimeSpan.FromMilliseconds(totalDuration)
    End Function

    Public Function GetAnimationDuration(FullPath As String) As Double

        Try
            Dim assemblyPath As String = Reflection.Assembly.GetExecutingAssembly().Location
            Dim pluginDir As String = Path.GetDirectoryName(assemblyPath)
            Dim parentDir As String = Directory.GetParent(pluginDir).FullName
            Dim ffmpegPath As String = Path.Combine(parentDir, "ffmpeg.exe")
            Dim defaultduration As Double = 10.0

            If File.Exists(ffmpegPath) Then


                Dim fi As New IO.FileInfo(FullPath)

                Dim strFFMpegParams As String = Nothing
                Dim intrslt As Integer = 0

                Dim strCommandExecOutput As String = Nothing
                Dim strCommandExecErrOutput As String = Nothing


                Dim FileExt As String = fi.Extension

                Select Case FileExt.ToLower
                    Case ".mp4", ".flv", ".avi"
                        strFFMpegParams = "-y -i """ & FullPath & ""
                    Case ".apng", ".png"
                        Dim tempMp4 As String = Path.Combine(Path.GetTempPath(), "ConvertAPNG.MP4")
                        strFFMpegParams = $"-y -i ""{FullPath}"" -preset ultrafast -r 30 -f mp4 ""{tempMp4}"""
                        Dim procapng As Process = New Process
                        With procapng.StartInfo
                            .FileName = ffmpegPath
                            .Arguments = strFFMpegParams
                            .UseShellExecute = False
                            .WindowStyle = ProcessWindowStyle.Hidden
                            .CreateNoWindow = True
                            .WorkingDirectory = parentDir
                        End With
                        procapng.Start()
                        procapng.WaitForExit()
                        intrslt = procapng.ExitCode
                        procapng.Close()
                        procapng = Nothing
                        'check if the files exists
                        If Not System.IO.File.Exists(Path.GetTempPath() & "ConvertAPNG.MP4") Or intrslt <> 0 Then
                            LogError($"Failed converting animated PNG to MP4")
                            If System.IO.File.Exists(Path.GetTempPath() & "ConvertAPNG.MP4") Then
                                System.IO.File.Delete(Path.GetTempPath() & "ConvertAPNG.MP4")
                            End If

                            Return defaultduration
                        End If
                        strFFMpegParams = "-y -i """ & Path.GetTempPath() & "ConvertAPNG.MP4"""
                    Case ".gif"
                        Using img As Image = Drawing.Image.FromFile(FullPath)
                            Dim giftime As TimeSpan? = GetGifDuration(img)
                            If giftime.HasValue Then
                                Return giftime.Value.TotalSeconds
                            Else
                                LogError("Could not determine GIF duration, using default.")
                                Return defaultduration
                            End If
                        End Using
                    Case Else
                        LogError($"Unsupported extension [{FileExt}] ")
                        Return defaultduration
                End Select


                Dim proc As Process = New Process

                With proc.StartInfo
                    .FileName = ffmpegPath
                    .CreateNoWindow = True
                    .UseShellExecute = False
                    .RedirectStandardOutput = True
                    .RedirectStandardError = True
                    .Arguments = strFFMpegParams
                End With
                proc.Start()
                'strCommandExecOutput = proc.StandardOutput.ReadToEnd() 'Commented out as some videos where hung on this 
                strCommandExecErrOutput = proc.StandardError.ReadToEnd()
                proc.WaitForExit()
                intrslt = proc.ExitCode
                proc.Close()
                proc = Nothing
                Dim index As Integer = strCommandExecErrOutput.IndexOf("Duration: ")
                If index >= 0 Then
                    ' String is found
                    index = index + "Duration: ".Length
                    Dim indexDuration As Integer = strCommandExecErrOutput.IndexOf(",", index)
                    If indexDuration >= 0 Then
                        Dim durationStr As String = strCommandExecErrOutput.Substring(index, indexDuration - index).Trim()
                        Dim ts As TimeSpan
                        If TimeSpan.TryParse(durationStr, ts) Then
                            Return ts.TotalSeconds
                        Else
                            LogError($"Could not parse duration string: [{durationStr}]")
                            Return defaultduration
                        End If
                    Else
                        LogError($"Could not determnVideolength : [{strCommandExecErrOutput}]")
                        Return defaultduration
                    End If
                Else
                    LogError($"Could not determnVideolength (2) [{strCommandExecErrOutput}]")
                    Return defaultduration
                End If
            Else
                LogInfo($"FFmpeg.exe not found at [{parentDir}]. Falback on Windows Media Player")

                Try
                    Dim wmp As Object = CreateObject("WMPlayer.OCX")
                    Dim media = wmp.newMedia(FullPath)
                    Dim duration As Double = media.duration

                    If duration > 0.1 Then Return duration
                Catch ex As Exception
                    LogError($"GetAnimationDuration Error on WMP: {ex.Message}")
                End Try
                Return defaultduration
            End If
        Catch ex As Exception
            LogError($"GetAnimationDuration Error: {ex.Message}")
            Return False
        End Try

    End Function


End Module