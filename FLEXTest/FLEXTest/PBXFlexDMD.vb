Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Linq
Imports System.Reflection
Imports Microsoft.Win32

Namespace PinballX

    Public Class PBXFlexDMD

        Enum XDMDFont
            bmp4by5 = 0
            bmp5by7 = 1
            bmp6by12 = 2
            bmp7by13 = 3
        End Enum

        Public Enum PlayState
            Ready = 0
            Playing = 1
        End Enum

        Public Enum ShowWindowCommands
            FORCEMINIMIZE = 11
            HIDE = 0
            MAXIMIZE = 3
            MINIMIZE = 6
            RESTORE = 9
            SHOW = 5
            SHOWDEFAULT = 10
            SHOWMAXIMIZED = 3
            SHOWMINIMIZED = 2
            SHOWMINNOACTIVE = 7
            SHOWNA = 8
            SHOWNOACTIVATE = 4
            SHOWNORMAL = 1
        End Enum

        Dim objXDMDFont(4) As Object
        Dim objXDMDDevice As Object

        Dim objFlexDMD As Object
        Dim objFlexDMDScene As Object

        Dim objFlexDMDFont(4) As Object
        Dim objFlexDMDFontShadow(4) As Object

        Public blnUseDMDDropShadow As Boolean = False
        Public colDMDColor As Color = Color.Red
        Public colDMDShadowColor As Color = Color.Yellow
        Public blnDMDCentreText As Boolean = True

        Private FlexDMDLocker As Object = New Object()


        Public Sub Load()
            SyncLock FlexDMDLocker

                If objFlexDMD Is Nothing Then
                    InitialiseFlexDMDObjects()
                End If
            End SyncLock
        End Sub

        Public Function Init() As Boolean
            SyncLock FlexDMDLocker
                Load()

                If objFlexDMD IsNot Nothing Then
                    Return True
                End If
            End SyncLock
            Return False

        End Function


        Private Function InitialiseFlexDMDObjects() As Boolean
            Try

                Dim blnCore As Boolean = False  ' are we running from pbx .net core or framework 
                If Type.[GetType]("System.Runtime.Loader.AssemblyLoadContext") IsNot Nothing Then blnCore = True

                If blnCore Then
                    'pbx .net core
                    Dim strFlexPath As String = SearchRegistry("FlexDMD.dll").ToString
                    'MsgBox(strFlexPath)
                    Dim ass As System.Reflection.Assembly = Assembly.LoadFrom(strFlexPath)
                    Dim ty As Type = ass.GetType("FlexDMD.FlexDMD")
                    objFlexDMD = Activator.CreateInstance(ty)
                Else
                    '.pbx.net framework
                    objFlexDMD = CreateObject("FlexDMD.FlexDMD")
                End If



                objFlexDMD.RenderMode = 2
                'objFlexDMD.Color = colDMDColor
                objFlexDMD.Width = 128  'this is for dots rather than size (size is dmddevice.ini)
                objFlexDMD.Height = 32

                objFlexDMD.Clear = True
                objFlexDMD.GameName = "PBX_STATSPLUGIN"
                objFlexDMD.Show = False
                objFlexDMD.Run = False

                objFlexDMDScene = objFlexDMD.NewGroup("Scene")


                objFlexDMD.LockRenderThread

                objFlexDMD.Stage.AddActor(objFlexDMDScene)

                objFlexDMD.UnlockRenderThread

                'use same finrs as xdmd (included in flex)
                objFlexDMDFont(XDMDFont.bmp7by13) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f7by13.fnt", colDMDColor, colDMDColor, 0)
                objFlexDMDFont(XDMDFont.bmp6by12) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f6by12.fnt", colDMDColor, colDMDColor, 0)
                objFlexDMDFont(XDMDFont.bmp4by5) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f4by5.fnt", colDMDColor, colDMDColor, 0)
                objFlexDMDFont(XDMDFont.bmp5by7) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f5by7.fnt", colDMDColor, colDMDColor, 0)

                If blnUseDMDDropShadow Then
                    colDMDShadowColor = GetDropShadowColor(colDMDColor)
                    objFlexDMDFontShadow(XDMDFont.bmp7by13) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f7by13.fnt", colDMDShadowColor, colDMDColor, 0)
                    objFlexDMDFontShadow(XDMDFont.bmp6by12) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f6by12.fnt", colDMDShadowColor, colDMDColor, 0)
                    objFlexDMDFontShadow(XDMDFont.bmp4by5) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f4by5.fnt", colDMDShadowColor, colDMDColor, 0)
                    objFlexDMDFontShadow(XDMDFont.bmp5by7) = objFlexDMD.NewFont("FlexDMD.Resources.udmd-f5by7.fnt", colDMDShadowColor, colDMDColor, 0)
                End If


                Return True
            Catch ex As Exception
                Form1.Logger.Log_Data("InitialiseFlexDMDObjects Error : " & ex.Message)
                Return False
                'Throw
            End Try
        End Function

        Private Function GetDropShadowColor(ByVal DMDColour As Color) As Color
            Try
                'get a drop shadow colour. Basic colour complement code, but then fade by setting alpha to 96 (so we don't get bright shadow)
                Dim a As Byte = 96
                Dim r As Byte = DMDColour.R
                Dim g As Byte = DMDColour.G
                Dim b As Byte = DMDColour.B

                If (r = g) And (g = b) Then
                    'grey or shade of, change only alpha (assume all colors passed in are alpha =255)
                    GetDropShadowColor = Color.FromArgb(96, r, g, b)
                Else
                    GetDropShadowColor = Color.FromArgb(96, 255 - r, 255 - g, 255 - b)
                End If

            Catch ex As Exception

            End Try
        End Function

        Public Sub HideDMD()
            Try
                objFlexDMD.Run = False
                objFlexDMD.Show = False
            Catch ex As Exception

            End Try
        End Sub

        Private Sub CloseFlexDMDObjects()
            Try

                If Not objFlexDMD Is Nothing Then objFlexDMD.Run = False
                objFlexDMDFont(0) = Nothing
                objFlexDMDFont(1) = Nothing
                objFlexDMDFont(2) = Nothing
                objFlexDMDFont(3) = Nothing
                objFlexDMDScene = Nothing
                objFlexDMD = Nothing
            Catch ex As Exception
            End Try

        End Sub
        Public Shared Function SearchRegistry(ByVal dllName As String) As String

            'Open the HKEY_CLASSES_ROOT\CLSID which contains the list of all registered COM files (.ocx,.dll, .ax) 
            'on the system no matters if is 32 or 64 bits.
            Dim t_clsidKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("CLSID")

            'Get all the sub keys it contains, wich are the generated GUID of each COM.
            For Each subKey In t_clsidKey.GetSubKeyNames.ToList
                Try
                    'For each CLSID\GUID key we get the InProcServer32 sub-key .
                    Dim t_clsidSubKey As RegistryKey = Registry.ClassesRoot.OpenSubKey("CLSID\" & subKey & "\InProcServer32")

                    If Not t_clsidSubKey Is Nothing Then

                        'in the case InProcServer32 exist we get the default value wich contains the path of the COM file.
                        'Dim t_valueName As String = (From value In t_clsidSubKey.GetValueNames() Where value = "")(0).ToString

                        Dim keys() As String = t_clsidSubKey.GetValueNames()
                        For Each key As String In keys
                            'If key Is not Nothing OrElse key = "" Then
                            Dim t_value As String = CStr(t_clsidSubKey.GetValue(key, "")).ToUpper
                            'Exit For
                            'End If
                            If t_value.EndsWith(dllName.ToUpper) Then

                                Return t_value.Replace("FILE:///", "")

                            End If
                        Next key


                        ''Now gets the value.
                        'Dim t_value As String = t_clsidSubKey.GetValue(t_valueName).ToString.ToUpper

                        'If subKey = "{766E10D3-DFE3-4E1B-AC99-C4D2BE16E91F}" Then MsgBox(t_value)

                        ''And finaly if the value ends with the name of the dll (include .dll) we return it
                        'If t_value.EndsWith(dllName.ToUpper) Then

                        '    Return t_value

                        'End If

                    End If
                Catch
                End Try
            Next

            'if not exist, return nothing
            Return Nothing

        End Function

        Public Sub DisplayVideo(ByVal path As String)
            Try
                If Not objFlexDMD Is Nothing Then objFlexDMD.LockRenderThread
                If objFlexDMD.Run = False Then objFlexDMD.Run = True
                If objFlexDMD.Show = False Then objFlexDMD.Show = True
                objFlexDMDScene.RemoveAll ' clear display
                objFlexDMDScene.AddActor(objFlexDMD.NewVideo("Video", path))
                '              objFlexDMDScene.GetVideo("Video").SetSize(128, 32)
                objFlexDMDScene.GetVideo("Video").loop = True
                'objFlexDMDScene.GetVideo("Video").run = True
                If Not objFlexDMD Is Nothing Then objFlexDMD.UnlockRenderThread

            Catch ex As Exception
                Form1.Logger.Log_Data("DisplayVideo Error : " & ex.Message)
            End Try
        End Sub
        Public Sub DisplayLine(ByVal text As String)
            Try
                Dim arrLines() As String = {}
                Dim intLinePosIncrement As Integer
                Dim intLinePosStart As Integer
                Dim intCharWidth As Integer
                Dim FontToUse As XDMDFont

                If Not objFlexDMD Is Nothing Then objFlexDMD.LockRenderThread

                If objFlexDMD.Run = False Then objFlexDMD.Run = True

                If objFlexDMD.Show = False Then objFlexDMD.Show = True

                objFlexDMDScene.RemoveAll ' clear display
                '' objFlexDMDScene.AddActor(objFlexDMD.NewImage("Back", "FlexDMD.Resources.dmds.black.png"))
                'Try
                '    objFlexDMDScene.RemoveActor(objFlexDMDScene.GetLabel("Label"))
                'Catch
                'End Try

                'work out best font for the text

                arrLines = WrapLineForXDMD(text, 2)   ' should we try to keep on 1 line at all?.. 
                If arrLines(0).Length > 16 Or arrLines(1).Length > 16 Then
                    ''arrLines = WrapLineForXDMD(TextBox1.Text, 2)
                    If arrLines(0).Length > 18 Or arrLines(1).Length > 18 Then
                        arrLines = WrapLineForXDMD(text, 4)
                        If arrLines(0).Length > 21 Or arrLines(1).Length > 21 Or arrLines(2).Length > 21 Or arrLines(3).Length > 21 Then
                            arrLines = WrapLineForXDMD(text, 5)
                            intCharWidth = 4
                            intLinePosStart = 1
                            intLinePosIncrement = 6
                            FontToUse = XDMDFont.bmp4by5
                        Else
                            intCharWidth = 5
                            intLinePosStart = 0
                            intLinePosIncrement = 8
                            FontToUse = XDMDFont.bmp5by7
                        End If
                    Else
                        intCharWidth = 6
                        intLinePosStart = 3
                        intLinePosIncrement = 13
                        FontToUse = XDMDFont.bmp6by12
                    End If
                Else
                    intCharWidth = 7
                    intLinePosIncrement = 14
                    intLinePosStart = 2
                    FontToUse = XDMDFont.bmp7by13
                End If


                'If blnUseRomColour = True And objFlexDMDFont(FontToUse).FontDef.Tint <> colDMDColor Then
                '    ' .. changing flexdmd.color causes flicker so need to change the actual font if diff colour
                '    objFlexDMDFont(FontToUse) = objFlexDMD.NewFont(objFlexDMDFont(FontToUse).FontDef.Path, colDMDColor, colDMDColor, 0)
                'End If
                If blnUseDMDDropShadow Then
                    'If colDMDShadowColor <> GetDropShadowColor(colDMDColor) Then
                    'refresh drop shadow colour
                    colDMDShadowColor = GetDropShadowColor(colDMDColor)
                    If objFlexDMDFontShadow(FontToUse).FontDef.Tint <> colDMDShadowColor Then
                        objFlexDMDFontShadow(FontToUse) = objFlexDMD.NewFont(objFlexDMDFontShadow(FontToUse).FontDef.Path, colDMDShadowColor, colDMDColor, 0)
                    End If
                End If
                ''cleanup lines array to allow for better vertical centreing
                If arrLines.Length = 2 Then
                    If arrLines(1) = "" Then
                        ReDim Preserve arrLines(0)
                    End If
                ElseIf arrLines.Length = 3 Then
                    If arrLines(2) = "" Then
                        ReDim Preserve arrLines(1)
                    End If
                ElseIf arrLines.Length = 4 Then
                    If arrLines(3) = "" Then
                        ReDim Preserve arrLines(2)
                    End If
                ElseIf arrLines.Length = 5 Then
                    If arrLines(4) = "" Then
                        ReDim Preserve arrLines(3)
                    End If
                End If

                If intCharWidth <> 5 Then
                    'only font that has a # char is the 5*7, other fonts will replace it with a space, messes up alignment if that is the leading/trailing character
                    For i = 0 To arrLines.Length - 1
                        arrLines(i) = arrLines(i).Replace("#", " ")
                    Next
                End If


                ''display lines centred vertiaclly (with centre padding from CentreXDMDLineHorizontally if on)
                'Select Case arrLines.Length
                '    Case 1
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(0), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(0, intLinePosStart + CInt(intLinePosIncrement / 2), 0)
                '    Case 2
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(0), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(0, intLinePosStart, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label1", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(1), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label1").SetAlignedPosition(0, intLinePosStart + intLinePosIncrement, 0)
                '    Case 3
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(0), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(0, intLinePosStart + CInt(intLinePosIncrement / 2), 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label1", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(1), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label1").SetAlignedPosition(0, intLinePosStart + CInt(intLinePosIncrement / 2) + intLinePosIncrement, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label2", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(2), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label2").SetAlignedPosition(0, intLinePosStart + CInt(intLinePosIncrement / 2) + (2 * intLinePosIncrement), 0)

                '    Case 4
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(0), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(0, intLinePosStart, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label1", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(1), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label1").SetAlignedPosition(0, intLinePosStart + intLinePosIncrement, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label2", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(2), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label2").SetAlignedPosition(0, intLinePosStart + (2 * intLinePosIncrement), 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label3", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(3), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label3").SetAlignedPosition(0, intLinePosStart + (3 * intLinePosIncrement), 0)
                '    Case 5
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(0), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(0, intLinePosStart, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label1", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(1), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label1").SetAlignedPosition(0, intLinePosStart + intLinePosIncrement, 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label2", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(2), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label2").SetAlignedPosition(0, intLinePosStart + (2 * intLinePosIncrement), 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label3", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(3), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label3").SetAlignedPosition(0, intLinePosStart + (3 * intLinePosIncrement), 0)
                '        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label4", objFlexDMDFont(FontToUse), CentreXDMDLineHorizontally(arrLines(4), intCharWidth)))
                '        objFlexDMDScene.GetLabel("Label4").SetAlignedPosition(0, intLinePosStart + (4 * intLinePosIncrement), 0)

                '    Case Else
                'End Select

                Dim intFlexAlign As Byte
                Dim intFlexPosX As Integer
                Dim intFlexPosY As Integer
                Select Case blnDMDCentreText
                    Case True
                        intFlexAlign = 4
                        'centre align works on centre location, so middle of 128*32
                        intFlexPosY = 16
                        intFlexPosX = 64
                    Case False
                        intFlexAlign = 0 'top left
                        intFlexPosY = intLinePosStart
                        intFlexPosX = 0
                End Select


                'display lines centred vertiaclly (with centre padding from CentreXDMDLineHorizontally if on)
                Select Case arrLines.Length
                    Case 1
                        If blnUseDMDDropShadow Then
                            objFlexDMDScene.AddActor(objFlexDMD.NewLabel("LabelDrop", objFlexDMDFontShadow(FontToUse), Trim(arrLines(0))))
                            objFlexDMDScene.GetLabel("LabelDrop").SetAlignedPosition(intFlexPosX + 1, intFlexPosY + 1, intFlexAlign)
                        End If
                        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), Trim(arrLines(0))))
                        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(intFlexPosX, intFlexPosY, intFlexAlign)
                    Case 2
                        If blnUseDMDDropShadow Then
                            objFlexDMDScene.AddActor(objFlexDMD.NewLabel("LabelDrop", objFlexDMDFontShadow(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1))))
                            objFlexDMDScene.GetLabel("LabelDrop").SetAlignedPosition(intFlexPosX + 1, intFlexPosY + 1, intFlexAlign)
                        End If
                        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1))))
                        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(intFlexPosX, intFlexPosY, intFlexAlign)
                    Case 3
                        If blnUseDMDDropShadow Then
                            objFlexDMDScene.AddActor(objFlexDMD.NewLabel("LabelDrop", objFlexDMDFontShadow(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2))))
                            objFlexDMDScene.GetLabel("LabelDrop").SetAlignedPosition(intFlexPosX + 1, intFlexPosY + 1, intFlexAlign)
                        End If
                        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2))))
                        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(intFlexPosX, intFlexPosY, intFlexAlign)
                    Case 4
                        If blnUseDMDDropShadow Then
                            objFlexDMDScene.AddActor(objFlexDMD.NewLabel("LabelDrop", objFlexDMDFontShadow(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2)) & vbCrLf & Trim(arrLines(3))))
                            objFlexDMDScene.GetLabel("LabelDrop").SetAlignedPosition(intFlexPosX + 1, intFlexPosY + 1, intFlexAlign)
                        End If
                        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), Trim(arrLines(0)) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2)) & vbCrLf & Trim(arrLines(3))))
                        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(intFlexPosX, intFlexPosY, intFlexAlign)
                    Case 5
                        If blnUseDMDDropShadow Then
                            objFlexDMDScene.AddActor(objFlexDMD.NewLabel("LabelDrop", objFlexDMDFontShadow(FontToUse), arrLines(0) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2)) & vbCrLf & Trim(arrLines(3)) & vbCrLf & Trim(arrLines(4))))
                            objFlexDMDScene.GetLabel("LabelDrop").SetAlignedPosition(intFlexPosX + 1, intFlexPosY + 1, intFlexAlign)
                        End If
                        objFlexDMDScene.AddActor(objFlexDMD.NewLabel("Label", objFlexDMDFont(FontToUse), arrLines(0) & vbCrLf & Trim(arrLines(1)) & vbCrLf & Trim(arrLines(2)) & vbCrLf & Trim(arrLines(3)) & vbCrLf & Trim(arrLines(4))))
                        objFlexDMDScene.GetLabel("Label").SetAlignedPosition(intFlexPosX, intFlexPosY, intFlexAlign)
                    Case Else
                End Select


                'XDMDFont.Draw(0, 0, 15, TextBox1.Text.ToString, Drawing.Color.Red)  '5*7 font 21chars visible, so adds one pixel as gap between chars. 25chars 4by5font

                '128*32
                'so 4*5 font 25 chars , with gap beteween lines so 5 lines = 125 chars on display (2 blank pixel rows at top, 2 at bottom)
                'so 5*7 font 21 chars , with gap beteween lines so 4 lines = 84 chars on display (1 blank pixel rows at top, 0 at bottom)
                'so 6*12 font 18 chars , with gap beteween lines so 2 lines = 36 chars on display (4 blank pixel rows at top, 3 at bottom)
                'so 7*13 font 16 chars , with gap beteween lines so 2 lines = 32 chars on display (3 blank pixel rows at top, 2 at bottom)

                'so display one line at a time, split it. use 5*7 most of the time. 4*5 if > 84chars?
                ' space positions in splut will affect whar qw can do. Eg. string with space at char 23 in first line. use line splitting from wheel app to try and fit to best font, with diff lengths passed in?. and then use 4*5 as catch all?


                If Not objFlexDMD Is Nothing Then objFlexDMD.UnlockRenderThread
            Catch ex As Exception
                Form1.Logger.Log_Data("DisplayLine Error : " & ex.Message)

            End Try

        End Sub

        Private Function WrapLineForXDMD(ByVal text As String, ByVal Optional n As Integer = 16) As String()
            Try
                Dim strText As String = text
                Dim intLastStart As Integer = 0
                Dim intLastEnd As Integer = 0


                Dim words = strText.Split(Char.ConvertFromUtf32(32))
                Dim cumwordwidth = New List(Of Integer)()
                cumwordwidth.Add(0)

                For Each word In words
                    cumwordwidth.Add(cumwordwidth(cumwordwidth.Count - 1) + word.Length)
                Next

                Dim totalwidth = cumwordwidth(cumwordwidth.Count - 1) + words.Length - 1
                Dim linewidth = CDbl((totalwidth - (n - 1))) / n
                Dim cost = New Func(Of Integer, Integer, Double)(Function(i, j)
                                                                     Dim actuallinewidth = Math.Max(j - i - 1, 0) + (cumwordwidth(j) - cumwordwidth(i))
                                                                     Return (linewidth - actuallinewidth) * (linewidth - actuallinewidth)
                                                                 End Function)
                Dim best = New List(Of List(Of Tuple(Of Double, Integer)))()
                Dim tmp = New List(Of Tuple(Of Double, Integer))()
                best.Add(tmp)
                tmp.Add(New Tuple(Of Double, Integer)(0.0F, -1))

                For Each word In words
                    tmp.Add(New Tuple(Of Double, Integer)(Double.MaxValue, -1))
                Next

                For l As Integer = 1 To n + 1 - 1
                    tmp = New List(Of Tuple(Of Double, Integer))()
                    best.Add(tmp)

                    For j As Integer = 0 To words.Length + 1 - 1
                        Dim min = New Tuple(Of Double, Integer)(best(l - 1)(0).Item1 + cost(0, j), 0)

                        For k As Integer = 0 To j + 1 - 1
                            Dim loc = best(l - 1)(k).Item1 + cost(k, j)
                            If loc < min.Item1 OrElse (loc = min.Item1 AndAlso k < min.Item2) Then min = New Tuple(Of Double, Integer)(loc, k)
                        Next

                        tmp.Add(min)
                    Next
                Next

                Dim lines = New List(Of String)()
                Dim b = words.Length

                For l As Integer = n To 1 Step -1
                    Dim a = best(l)(b).Item2
                    lines.Add(String.Join(" ", words, a, b - a))
                    b = a
                Next

                lines.Reverse()

                lines.RemoveAll(Function(str) String.IsNullOrWhiteSpace(str))


                Dim arrtemp() As String = lines.ToArray
                ReDim Preserve arrtemp(n - 1)
                For x = 0 To n - 1
                    If arrtemp(x) Is Nothing Then arrtemp(x) = ""
                Next x

                WrapLineForXDMD = arrtemp

            Catch ex As Exception
                Form1.Logger.Log_Data("WrapLineForXDMD Error : " & ex.Message)

            End Try
        End Function

        Public Sub Close()
            Try
                If Not objFlexDMD Is Nothing Then objFlexDMD.Run = False
                objFlexDMDFont(0) = Nothing
                objFlexDMDFont(1) = Nothing
                objFlexDMDFont(2) = Nothing
                objFlexDMDFont(3) = Nothing
                If blnUseDMDDropShadow Then
                    objFlexDMDFontShadow(0) = Nothing
                    objFlexDMDFontShadow(1) = Nothing
                    objFlexDMDFontShadow(2) = Nothing
                    objFlexDMDFontShadow(3) = Nothing
                End If
                objFlexDMDScene = Nothing
                objFlexDMD = Nothing
            Catch ex As Exception
            End Try

        End Sub

    End Class
End Namespace
