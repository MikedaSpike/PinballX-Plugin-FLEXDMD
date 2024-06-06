Namespace DracLabs
    Public Class Logger
#Region " Program Information "
        Private Shared lf As String = Nothing
        Private Shared debug As Boolean = False

        Public Structure Settings
            Public Shared ReadOnly Property Log_File() As String
                Get
                    Return lf
                End Get
            End Property
            Public Shared ReadOnly Property Debug_Mode() As Boolean
                Get
                    Return debug
                End Get
            End Property
        End Structure
#End Region
        ''' <summary>
        ''' Default constructor for the Log Manager.
        ''' </summary>
        Public Sub New()
            'Default constructor.
        End Sub
        ''' <summary>
        ''' Initializes your log file.
        ''' </summary>
        ''' <param name="Program_Name">The name of the program you are logging.</param>
        ''' <param name="Program_Version">The version of the program you are logging.</param>
        ''' <param name="Log_Path">The absolute path of the Log File.</param>
        Public Sub Initialize(Program_Name As String, Program_Version As String, Optional Log_Path As String = Nothing, Optional Is_Debug As Boolean = False)
            'Check and see if the log file was specified prior to writing to it:
            If Log_Path = Nothing Then
                lf = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, Program_Name & " log.txt")
            Else
                lf = Log_Path
            End If
            'Clear out the log file from the previous run.
            Try
                Using sw As System.IO.StreamWriter = System.IO.File.CreateText(lf)
                    sw.Flush()
                End Using
            Catch ex As Exception
                'MessageBox.Show(ex.Message, "INITIALIZATION ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try
            'Write the program info to the log file:
            Write_Line([String].Format("{0} (Version: {1})", Program_Name, Program_Version))
            If Is_Debug = True Then
                debug = True
                Get_System_Info()
            End If
        End Sub
        ''' <summary>
        ''' Write data to your log file.
        ''' </summary>
        ''' <param name="Message">The message you are logging.</param>
        Public Sub Log_Data(ByVal Message As String)
            Write_Line(Message)
        End Sub
        ''' <summary>
        ''' Write data to your log file.
        ''' </summary>
        ''' <param name="Message">The message you are logging.</param>
        ''' <param name="Title">The title of the log entry.</param>
        Public Sub Log_Data(Message As String, Title As String)
            Write_Line([String].Format("{0}: {1}", Title.ToUpper, Message))
        End Sub
        ''' <summary>
        ''' Write data to your log file.
        ''' </summary>
        ''' <param name="Method">The method that's throwing the error.</param>
        ''' <param name="Class">The class that contains the method.</param>
        ''' <param name="Message">The message to write.</param>
        ''' <param name="Title">The message to write.</param>
        Public Sub Log_Data(ByVal Message As String, ByVal Title As String, ByVal Method As String, ByVal [Class] As String)
            Write_Line([String].Format("{0}: {1}({2}) - {3}", Title.ToUpper, [Class], Method, Message))
        End Sub
        ''' <summary>
        ''' Write error data to your log file.
        ''' </summary>
        ''' <param name="Message">The message you are logging.</param>
        Public Sub Log_Error(Message As String)
            Write_Line([String].Format("ERROR: {0}", Message))
        End Sub
        ''' <summary>
        ''' Write error data to your log file.
        ''' </summary>
        ''' <param name="Message">The message you are logging.</param>
        ''' <param name="Title">The title of the log entry.</param>
        Public Sub Log_Error(Message As String, Title As String)
            Write_Line([String].Format("ERROR @ {0}: {1}", Title.ToUpper, Message))
        End Sub
        ''' <summary>
        ''' Write error data to your log file.
        ''' </summary>
        ''' <param name="Message">The message you are logging.</param>
        ''' <param name="Title">The title of the log entry.</param>
        ''' <param name="Error">The title of the log entry.</param>
        ''' <param name="Stack_Trace">The stack trace of the error.</param>
        Public Sub Log_Error(ByVal Message As String, ByVal Title As String, ByVal [Error] As String, ByVal Stack_Trace As String)
            Write_Line([String].Format("ERROR @ {0}: {1}", Title.ToUpper, Message))
            Write_Line([String].Format("{0}", [Error]))
            If Stack_Trace IsNot Nothing Then
                Write_Line([String].Format("{0}", Stack_Trace))
            End If
        End Sub
        ''' <summary>
        ''' Write data to your log file.
        ''' </summary>
        ''' <param name="Method">The method that's throwing the error.</param>
        ''' <param name="Class">The class that contains the method.</param>
        ''' <param name="Message">The message to write.</param>
        ''' <param name="Title">The message to write.</param>
        Public Sub Log_Error(Message As String, Title As String, Stack_Trace As String, Method As String, [Class] As String)
            Write_Line([String].Format("ERROR @ {0}: {1}({2})", Title.ToUpper, [Class], Method))
            Write_Line([String].Format("{0}", Message))
        End Sub
        ''' <summary>
        ''' Logs the system information for the current user.
        ''' </summary>
        ''' <remarks>A large chunk of this code was borrowed from Ben Baker aka Headkaze.</remarks>
        Private Sub Get_System_Info()
            Try
                Write_Line("Diagnostics: Begin system enumeraion...")
                Dim query As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
                For Each mo As System.Management.ManagementObject In query.[Get]()
                    Dim Total_RAM As [Double] = [Double].Parse(mo("TotalVisibleMemorySize").ToString()) / 1024
                    Dim Free_RAM As [Double] = [Double].Parse(mo("FreePhysicalMemory").ToString()) / 1024
                    Write_Line("OS: " & mo("Caption").ToString())
                    Write_Line("Version: " & mo("Version").ToString())
                    Write_Line("Build: " & mo("BuildNumber").ToString())
                    Write_Line("Total RAM: " & Math.Ceiling(Total_RAM) & " MB")
                    Write_Line("Available RAM: " & Math.Ceiling(Total_RAM - Free_RAM) & " MB")
                Next mo
                query = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor")
                For Each obj As System.Management.ManagementObject In query.[Get]()
                    Write_Line("CPU: " & obj("Name").ToString().TrimStart())
                Next obj
                query = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_VideoController")
                For Each obj As System.Management.ManagementObject In query.[Get]()
                    Write_Line("Video Card: " & obj("Name").ToString())
                    Write_Line("Video Driver: " & obj("DriverVersion").ToString())
                    If obj("AdapterRAM") IsNot Nothing Then
                        Dim Video_RAM As Double = Double.Parse(obj("AdapterRAM").ToString()) / 1024 / 1024
                        Write_Line("Video RAM: " & Video_RAM & " MB")
                    End If
                Next obj
                query = New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice")
                For Each obj As System.Management.ManagementObject In query.[Get]()
                    Write_Line("Sound Card: " & obj("Name").ToString())
                Next obj
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v1.0") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 1.0 Installed")
                End If
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v1.1") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 1.1 Installed")
                End If
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v2.0") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 2.0 Installed")
                End If
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v3.0") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 3.0 Installed")
                End If
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v4.0") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 4.0 Installed")
                End If
                If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\.NETFramework\policy\v4.5") IsNot Nothing Then
                    Write_Line(".NET: .NET Framework 4.0 Installed")
                End If
                query.Dispose()
                query = Nothing
                Write_Line("Diagnostics: System enumeraion completed successfully!")
            Catch ex As Exception
                'MessageBox.Show(ex.Message, "WRITE ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try
        End Sub
        ''' <summary>
        ''' Write lines to your log file.
        ''' </summary>
        ''' <param name="Log_Entry">The entry to write to the log file.</param>
        Private Sub Write_Line(ByVal Log_Entry As String)
            'Write the line to the log file:
            Try
                Using sw As System.IO.StreamWriter = System.IO.File.AppendText(lf)
                    sw.WriteLine([String].Format("{0} : {1}", DateTime.Now, Log_Entry))
                    sw.Flush()
                End Using
            Catch ex As Exception
                'MessageBox.Show(ex.Message, "WRITE ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try
        End Sub
    End Class
End Namespace
