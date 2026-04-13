Imports System.Collections.Generic
Imports System.IO
Imports System.Linq

Public Class PinemHiManager
    Private _rootPath As String

    Public Sub New(rootPath As String)
        _rootPath = rootPath
    End Sub

    Public Function GetLeaderboardData(categoryFolder As String, romName As String) As String
        If String.IsNullOrEmpty(_rootPath) Then Return ""

        Dim categoryPath As String = Path.Combine(_rootPath, "PINemHi LeaderBoard", categoryFolder)
        If Not Directory.Exists(categoryPath) Then Return ""

        ' Voor "Cup" categorieën wordt altijd cup.txt gebruikt, anders de romnaam
        Dim fileName As String = If(categoryFolder.ToLower().Contains("cup"), "cup.txt", romName & ".txt")
        Dim fullPath As String = Path.Combine(categoryPath, fileName)

        If File.Exists(fullPath) Then
            Try
                Return File.ReadAllText(fullPath)
            Catch ex As Exception
                Return ""
            End Try
        End If
        Return ""
    End Function
    Public Class BadgeResult
        Public ImagePath As String
        Public IsLocked As Boolean
    End Class

    Public Shared Function GetBadges(pinemhiPath As String, romName As String, hideLocked As Boolean) As List(Of BadgeResult)
        Dim results As New List(Of BadgeResult)()
        Dim badgeFile As String = Path.Combine(pinemhiPath, "PINemHi LeaderBoard", "Badges", romName & ".txt")

        If Not File.Exists(badgeFile) Then Return results

        Dim lines As String() = File.ReadAllLines(badgeFile) _
            .Select(Function(l) l.Trim()) _
            .Where(Function(l) Not String.IsNullOrWhiteSpace(l)) _
            .ToArray()

        ' Now we are sure that lines(0) is the name and the last 5 are the scores
        If lines.Length < 17 Then Return results

        Dim gameName As String = lines(0)
        Dim genericDir As String = Path.Combine(pinemhiPath, "PINemHi LeaderBoard", "Images", "generic")
        Dim gameSpecificDir As String = Path.Combine(pinemhiPath, "PINemHi LeaderBoard", "Images", gameName)

        Dim AddBadge = Sub(status As String, fileName As String, isSpecific As Boolean)
                           If status = "666" Then Return

                           Dim isOwned As Boolean = (status = "1")
                           Dim pathToFile As String = ""

                           If isOwned Then
                               Dim dir = If(isSpecific, gameSpecificDir, genericDir)
                               pathToFile = Path.Combine(dir, fileName.Trim())
                           Else
                               If Not hideLocked Then
                                   ' Locked badge image
                                   pathToFile = Path.Combine(genericDir, "l.png")
                               End If
                           End If

                           If Not String.IsNullOrEmpty(pathToFile) Then
                               results.Add(New BadgeResult With {.ImagePath = pathToFile, .IsLocked = Not isOwned})
                           End If
                       End Sub

        ' 1. Scores (Lines 2 to 6 -> index 1-5)
        ' We take exactly the last 5 lines for the filenames
        Dim scoreNames As String() = lines.Skip(lines.Count - 5).Take(5).ToArray()
        For i As Integer = 0 To 4
            AddBadge(lines(i + 1), scoreNames(i) & ".png", False)
        Next

        ' 2. Specials (Lines 7 to 14 -> index 6-13)
        ' Filenames 6.png to 13.png
        For i As Integer = 6 To 13
            AddBadge(lines(i), i.ToString() & ".png", True)
        Next

        ' 3. Games Played (Lines 15 to 17 -> index 14-16)
        ' Filenames 14.png to 16.png
        For i As Integer = 14 To 16
            AddBadge(lines(i), i.ToString() & ".png", False)
        Next

        Return results
    End Function
End Class