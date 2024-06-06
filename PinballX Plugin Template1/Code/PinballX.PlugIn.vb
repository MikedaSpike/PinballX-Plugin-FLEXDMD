Imports System
Imports System.Collections
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices
Imports PinballX_Plugin_Template1.DracLabs



Namespace PinballX
    Public Structure PluginInfo
        Public Const Name As String = "FLEXDMD Plugin"
        Public Const Version As String = "1.2"
        Public Const Author As String = "Mike DA Spike"
        Public Const Description As String = "Replacement of PinballX XDMD for FLEXDMD"
        Public Const PluginVersion As String = "1.2"
        Dim Dummy As String
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure PinballXInfo
        Public Version As String
    End Structure



    Public Class Plugin
        Private disposed As Boolean = False
        Dim FLEXDMD As PBXFlexDMD = New PBXFlexDMD


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

        'Pinball X media folders
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

        Public Shared Logger As New DracLabs.Logger

        Private blnInGame As Boolean = False 'track if game selected/active, if user presses pause we don't want to display stat screen
        Private blnAttractModeActive As Boolean = False 'tracks id Screensaver mode is active

        'pbx system tpyes, 0-4 match those in use by pbx at time of writing. Others don't have a type number assigned in pbx they are named systems.
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
        Public Sub Plugin()

        End Sub

        Public Function Initialize(ByVal InfoPtr As IntPtr) As Boolean
            Try


                Dim Info As PinballXInfo = CType(Marshal.PtrToStructure(InfoPtr, GetType(PinballXInfo)), PinballXInfo)
                Logger.Initialize("PBXFlexDMDDisplay", PluginInfo.Version, My.Application.Info.DirectoryPath & "\Plugins\PBXFlexDMDDisplay.txt")
                Logger.Log_Data("Initialize Start : ...")
                If Not FLEXDMD.Init() Then Return False
                Logger.Log_Data("Initialize Complete")
                FLEXDMD.DisplayLine(PluginInfo.Name & " " & PluginInfo.Version & " INITIALIZED")
                Return True
            Catch ex As Exception
                Logger.Log_Data("Initialize Error : " & ex.Message)
            End Try 'Called when PinballX loads
        End Function

        Public Sub Configure()
            'Called when configuring plugins from the Plugin Manager
            Dim config As New Configuration
            config.ShowDialog()
        End Sub

        Public Sub Event_App_Exit()
            'Called when PinballX exits
            Try
                FLEXDMD.Close()
                Logger.Log_Data("Event_App_Exit")
            Catch ex As Exception
            End Try
        End Sub

        Public Sub Event_GameSelect(ByVal InfoPtr As IntPtr)
            'Called when a game is selected
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Dim intSystemType As Integer = SYSTYPE_OTHER

            If blnInGame = False Then 'this event also fires if pause button is hit, so if ingame don't do anything.
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

                    'If blnAttractModeActive = True Then
                    '    'Attractmode
                    '    If String.IsNullOrEmpty(strTableName) Then
                    '        Logger.Log_Data("Event_GameSelect : Get System Data AtrractMode : SystemName = " & strSystemName & " : System = " & strSystem & " : TableDesc = " & strTableDesc)
                    '        FLEXDMD.DisplayVideo(strSystemVideoPath & "\Real DMD Color Videos\- system -.mp4")
                    '        'properly this is System
                    '    Else
                    '        Logger.Log_Data("Event_GameSelect : Get System Data AtrractMode : SystemName = " & strSystemName & " : System = " & strSystem & " : TableDesc = " & strTableDesc & " : TableName = " & strTableName)

                    '        FLEXDMD.DisplayVideo(strSystemVideoPath & "\Real DMD Color Videos\" & strTableName & ".mp4")
                    '        'This is table 
                    '    End If
                    'Else
                    '    'none attract mode
                    '    Dim strVideoFile = GetMediaFile(My.Application.Info.DirectoryPath, strSystemName, strTableName, strTableDesc)
                    '    Logger.Log_Data("Event_GameSelect : Get System Data : SystemName = " & strSystemName & " : System = " & strSystem & " : TableDesc = " & strTableDesc & " : TableName = " & strTableName)
                    '    FLEXDMD.DisplayVideo(strVideoFile)
                    'End If

                    Dim strMediaFile = GetMediaFile(My.Application.Info.DirectoryPath, strSystemName, strTableName, strTableDesc)
                    Logger.Log_Data("Event_GameSelect : Get System Data : SystemName = " & strSystemName & " : System = " & strSystem & " : TableDesc = " & strTableDesc & " : TableName = " & strTableName & " : VideoFile = " & strMediaFile)

                    FLEXDMD.DisplayMedia(strMediaFile)


                    'FLEXDMD.DisplayVideo("C:\Pinball\Visual Pinball\Tables\VPX\Diablo.UltraDMD\extra-ball.wmv")
                    'FLEXDMD.DisplayVideo("C:\Pinball\PinballX\Media\Videos\No Real DMD Color.avi")
                    'extra-ball.wmv
                    'FLEXDMD.DisplayVideo("C:\Pinball\PinballX\Media\Visual Pinball\Real DMD Color Images\WhoDunnit_1.0.gif")
                Catch ex As Exception
                    Logger.Log_Data("Event_GameSelect Error : " & ex.Message)
                End Try
            End If


        End Sub

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
                Logger.Log_Data("GetMediaFile Error : " & ex.Message)
                Return ""
            End Try


        End Function


        Public Function Event_GameRun(ByVal InfoPtr As IntPtr) As Boolean
            'Called when a game is launched
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Try
                Static strLastsystemXMLfile As String = String.Empty
                Static dtXMLFile As DataTable
                Dim strTable As String = String.Empty
                Try
                    strTable = Info.GameName.ToString
                Catch ex As Exception
                    strTable = String.Empty
                End Try
                Dim strSystem As String = String.Empty
                Try
                    strSystem = Info.SystemName.ToString
                Catch ex As Exception
                    strSystem = String.Empty
                End Try

                blnInGame = True


                Dim strSystemXMLFile As String = My.Application.Info.DirectoryPath & "\Databases\" & strSystem & "\" & strSystem & ".xml"

                If Not String.IsNullOrEmpty(strTable) Then
                    If strSystemXMLFile <> strLastsystemXMLfile OrElse IsNothing(dtXMLFile) Then
                        Logger.Log_Data("Event_GameRun: Retrieving data from [" & strSystemXMLFile & "]")
                        dtXMLFile = GetDatasetFromXMLFile(strSystemXMLFile)
                    End If

                    Dim row As DataRow = dtXMLFile.Select(dtXML.Name & " like '" & EscapeLikeValue(strTable) & "' and " & dtXML.HideDMD & " = 'False'").FirstOrDefault() 'just check if we have a gamename where we must show the DMD during game play
                    If row Is Nothing Then
                        Logger.Log_Data("Event_GameRun: Table - '" & strTable & "', System - '" & strSystem & "'")
                        FLEXDMD.HideDMD()
                    Else
                        Logger.Log_Data("Event_GameRun: Table - '" & strTable & "', System - '" & strSystem & "', Not Hiding DMD")
                    End If
                Else
                    Logger.Log_Data("Event_GameRun: Table - '" & strTable & "', System - '" & strSystem & "'")
                    FLEXDMD.HideDMD()
                End If

                strLastsystemXMLfile = strSystemXMLFile

            Catch ex As Exception
                Logger.Log_Data("Event_GameRun Error : " & ex.Message)
            End Try
            Return True
        End Function

        ''' <summary>
        ''' replaces strange characters for SQL search to correct values
        ''' </summary>
        ''' <param name="value">value that needs to be checked and replaced</param>
        ''' <returns>new string that is replaced with correct characters</returns>
        Public Function EscapeLikeValue(ByVal value As String) As String
            Dim sb As Text.StringBuilder = New Text.StringBuilder(value.Length)

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
                Logger.Log_Error(ex.Message)
                Return Nothing
            End Try

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

        Public Function Event_Parameters(ByVal InfoPtr As IntPtr) As String
            'Called when PinballX builds the command line for the game
            Dim Info As PlugInInfo_1 = CType(Marshal.PtrToStructure(InfoPtr, GetType(PlugInInfo_1)), PlugInInfo_1)
            Dim CmdLine As String = Info.Parameters
            Return CmdLine
        End Function

        Public Sub Event_GameExit(ByVal InfoPtr As IntPtr)
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
                Logger.Log_Data("Event_GameExit: Table - '" & strTable & "' System - '" & strSystem & "'")
            Catch ex As Exception
                Logger.Log_Data("Event_GameExit Fail: " & ex.Message)
            End Try
        End Sub

        Public Function Event_ScreenSaver(ByVal Type As Integer) As Boolean
            'Called when PinballX enters and exits Screensaver Mode
            Select Case Type
                Case 1 'Enter Screensaver
                    blnAttractModeActive = True
                    Logger.Log_Data("Event_ScreenSaver : Attract mode activated")
                Case 2 'Exit Screensaver
                    blnAttractModeActive = False
                    Logger.Log_Data("Event_ScreenSaver : Attract mode stopped")
            End Select
            Return True
        End Function

        Public Sub Dispose()
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        Private Sub Dispose(ByVal disposing As Boolean)
            If Not (Me.disposed) Then
                If (disposing) Then
                    'Run disposal code here
                End If
            End If
            disposed = True
        End Sub

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

        Public ReadOnly Property PluginVersion() As String
            Get
                Return PluginInfo.PluginVersion
            End Get
        End Property
    End Class
End Namespace


