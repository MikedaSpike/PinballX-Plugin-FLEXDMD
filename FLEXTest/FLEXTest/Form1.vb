Imports FLEXTest.PinballX
Imports System.IO

Public Class Form1

    Public Structure PluginInfo
        Public Const Name As String = "FLEXDMD Plugin"
        Public Const Version As String = "1.0"
        Public Const Author As String = "Mike DA Spike"
        Public Const Description As String = "Replacement of PinballX XDMD for FLEXDMD"
        Public Const PluginVersion As String = "1"
        Dim Dummy As String
    End Structure

    Dim FLEXDMD As PBXFlexDMD = New PBXFlexDMD
    Public Shared Logger As New DracLabs.Logger

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Logger.Initialize("PBXFlexDMDDisplay", PluginInfo.Version, Directory.GetCurrentDirectory & "\flextest.log")
        Logger.Log_Data("Initialize Start : ...")
        If FLEXDMD.Init() Then
            Logger.Log_Data("Initialize Complete")
            FLEXDMD.DisplayLine("FLEX DMD PLUGIN INITIALIZED")
            FLEXDMD.DisplayVideo("C:\Pinball\Visual Pinball\Tables\VPX\Diablo.UltraDMD\extra-ball.wmv")
            FLEXDMD.DisplayVideo("C:\Pinball\PinballX\Media\Visual Pinball\Real DMD Color Videos\24 (Stern2009)EmperorModv1.1.mp4")
        Else

            Logger.Log_Data("Initialize Failed")
        End If

    End Sub
End Class
