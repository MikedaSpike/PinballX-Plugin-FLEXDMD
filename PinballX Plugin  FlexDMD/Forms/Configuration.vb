Imports System.IO
Imports System.Drawing
Imports System.ComponentModel
Imports System.Windows.Forms
Imports DracLabs

Namespace PinballX
    Public Class Configuration
        Inherits System.Windows.Forms.Form

        Dim assemblyPath As String = Reflection.Assembly.GetExecutingAssembly().Location
        Dim dllName As String = Path.GetFileNameWithoutExtension(assemblyPath)
        Dim pluginDir As String = Path.GetDirectoryName(assemblyPath)
        Private hasChanges As Boolean = False
        Private IniFile As New DracLabs.IniFile
        Private strIniPath As String = Path.Combine(pluginDir, dllName & ".ini")
        Private DebugLogging As Boolean = False
        Private DLLpath As String = String.Empty

#Region " Windows Form Designer generated code "
        Friend WithEvents numMaxLoops As NumericUpDown
        Friend WithEvents txtPinemHiPath As TextBox
        Friend WithEvents Label5 As Label
        Friend WithEvents Label6 As Label
        Friend WithEvents gpxPBX As GroupBox
        Friend WithEvents Label7 As Label
        Friend WithEvents numFlexDuration As NumericUpDown
        Friend WithEvents cbxMediaShow As CheckBox
        Friend WithEvents gpxClock As GroupBox
        Friend WithEvents cbxClockShow As CheckBox
        Friend WithEvents Label9 As Label
        Friend WithEvents numClockInterval As NumericUpDown
        Friend WithEvents gbxPINemHi As GroupBox
        Friend WithEvents cbxShowPinEMHiHighScores As CheckBox
        Friend WithEvents cmbTimeSelector As ComboBox
        Friend WithEvents chkClockDuration As Label
        Friend WithEvents numClockDuration As NumericUpDown
        Friend WithEvents Label10 As Label
        Friend WithEvents cbXShowPBXHighscores As CheckBox
        Friend WithEvents cbxShowBadges As CheckBox
        Friend WithEvents cbxShowCountdown As CheckBox
        Friend WithEvents Label11 As Label
        Friend WithEvents numExtraSeconds As NumericUpDown
        Friend WithEvents lblColorPreview As Label
        Friend WithEvents btnPickColor As Button
        Friend WithEvents Label12 As Label
        Friend WithEvents btnCancel As Button
        Friend WithEvents ToolTip1 As ToolTip
        Friend WithEvents clbPinemHiScores As CheckedListBox





        Public Sub New()
            MyBase.New()
            InitializeComponent()
            Me.Text = PluginInfo.Name & " (" & PluginInfo.Version & ")"
            lblName.Text = PluginInfo.Name
            lblVersion.Text = PluginInfo.Version
            lblAuthor.Text = PluginInfo.Author
            lblDescription.Text = PluginInfo.Description
        End Sub

        'Form overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Friend WithEvents pictureBox1 As System.Windows.Forms.PictureBox
        Friend WithEvents grpPluginInfo As System.Windows.Forms.GroupBox
        Friend WithEvents label4 As System.Windows.Forms.Label
        Friend WithEvents label3 As System.Windows.Forms.Label
        Friend WithEvents label2 As System.Windows.Forms.Label
        Friend WithEvents label1 As System.Windows.Forms.Label
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents lblName As System.Windows.Forms.Label
        Friend WithEvents lblAuthor As System.Windows.Forms.Label
        Friend WithEvents lblVersion As System.Windows.Forms.Label
        Friend WithEvents cbxUseHD As Windows.Forms.CheckBox
        Friend WithEvents butExit As System.Windows.Forms.Button
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Configuration))
            Me.pictureBox1 = New System.Windows.Forms.PictureBox()
            Me.grpPluginInfo = New System.Windows.Forms.GroupBox()
            Me.label4 = New System.Windows.Forms.Label()
            Me.label3 = New System.Windows.Forms.Label()
            Me.label2 = New System.Windows.Forms.Label()
            Me.label1 = New System.Windows.Forms.Label()
            Me.lblDescription = New System.Windows.Forms.Label()
            Me.lblName = New System.Windows.Forms.Label()
            Me.lblAuthor = New System.Windows.Forms.Label()
            Me.lblVersion = New System.Windows.Forms.Label()
            Me.gpxClock = New System.Windows.Forms.GroupBox()
            Me.Label10 = New System.Windows.Forms.Label()
            Me.cmbTimeSelector = New System.Windows.Forms.ComboBox()
            Me.chkClockDuration = New System.Windows.Forms.Label()
            Me.numClockDuration = New System.Windows.Forms.NumericUpDown()
            Me.cbxClockShow = New System.Windows.Forms.CheckBox()
            Me.Label9 = New System.Windows.Forms.Label()
            Me.numClockInterval = New System.Windows.Forms.NumericUpDown()
            Me.butExit = New System.Windows.Forms.Button()
            Me.cbxUseHD = New System.Windows.Forms.CheckBox()
            Me.numMaxLoops = New System.Windows.Forms.NumericUpDown()
            Me.txtPinemHiPath = New System.Windows.Forms.TextBox()
            Me.Label5 = New System.Windows.Forms.Label()
            Me.Label6 = New System.Windows.Forms.Label()
            Me.gpxPBX = New System.Windows.Forms.GroupBox()
            Me.Label12 = New System.Windows.Forms.Label()
            Me.lblColorPreview = New System.Windows.Forms.Label()
            Me.btnPickColor = New System.Windows.Forms.Button()
            Me.cbXShowPBXHighscores = New System.Windows.Forms.CheckBox()
            Me.cbxMediaShow = New System.Windows.Forms.CheckBox()
            Me.Label7 = New System.Windows.Forms.Label()
            Me.numFlexDuration = New System.Windows.Forms.NumericUpDown()
            Me.gbxPINemHi = New System.Windows.Forms.GroupBox()
            Me.clbPinemHiScores = New System.Windows.Forms.CheckedListBox()
            Me.Label11 = New System.Windows.Forms.Label()
            Me.numExtraSeconds = New System.Windows.Forms.NumericUpDown()
            Me.cbxShowCountdown = New System.Windows.Forms.CheckBox()
            Me.cbxShowBadges = New System.Windows.Forms.CheckBox()
            Me.cbxShowPinEMHiHighScores = New System.Windows.Forms.CheckBox()
            Me.btnCancel = New System.Windows.Forms.Button()
            Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grpPluginInfo.SuspendLayout()
            Me.gpxClock.SuspendLayout()
            CType(Me.numClockDuration, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numClockInterval, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.numMaxLoops, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gpxPBX.SuspendLayout()
            CType(Me.numFlexDuration, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gbxPINemHi.SuspendLayout()
            CType(Me.numExtraSeconds, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'pictureBox1
            '
            Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
            Me.pictureBox1.Location = New System.Drawing.Point(16, 8)
            Me.pictureBox1.Name = "pictureBox1"
            Me.pictureBox1.Size = New System.Drawing.Size(90, 90)
            Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
            Me.pictureBox1.TabIndex = 1
            Me.pictureBox1.TabStop = False
            '
            'grpPluginInfo
            '
            Me.grpPluginInfo.Controls.Add(Me.label4)
            Me.grpPluginInfo.Controls.Add(Me.label3)
            Me.grpPluginInfo.Controls.Add(Me.label2)
            Me.grpPluginInfo.Controls.Add(Me.label1)
            Me.grpPluginInfo.Controls.Add(Me.lblDescription)
            Me.grpPluginInfo.Controls.Add(Me.lblName)
            Me.grpPluginInfo.Controls.Add(Me.lblAuthor)
            Me.grpPluginInfo.Controls.Add(Me.lblVersion)
            Me.grpPluginInfo.Location = New System.Drawing.Point(120, 8)
            Me.grpPluginInfo.Name = "grpPluginInfo"
            Me.grpPluginInfo.Size = New System.Drawing.Size(578, 90)
            Me.grpPluginInfo.TabIndex = 7
            Me.grpPluginInfo.TabStop = False
            Me.grpPluginInfo.Text = "Plugin Information"
            '
            'label4
            '
            Me.label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label4.Location = New System.Drawing.Point(178, 24)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(96, 16)
            Me.label4.TabIndex = 9
            Me.label4.Text = "Description:"
            Me.label4.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label3
            '
            Me.label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label3.Location = New System.Drawing.Point(38, 40)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(60, 16)
            Me.label3.TabIndex = 8
            Me.label3.Text = "Author:"
            Me.label3.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label2
            '
            Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label2.Location = New System.Drawing.Point(38, 56)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(60, 16)
            Me.label2.TabIndex = 7
            Me.label2.Text = "Version:"
            Me.label2.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label1
            '
            Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label1.Location = New System.Drawing.Point(38, 24)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(60, 16)
            Me.label1.TabIndex = 6
            Me.label1.Text = "Name:"
            Me.label1.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'lblDescription
            '
            Me.lblDescription.Location = New System.Drawing.Point(274, 24)
            Me.lblDescription.Name = "lblDescription"
            Me.lblDescription.Size = New System.Drawing.Size(281, 63)
            Me.lblDescription.TabIndex = 5
            Me.lblDescription.Text = "Description"
            '
            'lblName
            '
            Me.lblName.Location = New System.Drawing.Point(104, 24)
            Me.lblName.Name = "lblName"
            Me.lblName.Size = New System.Drawing.Size(144, 16)
            Me.lblName.TabIndex = 3
            Me.lblName.Text = "Name"
            '
            'lblAuthor
            '
            Me.lblAuthor.Location = New System.Drawing.Point(104, 40)
            Me.lblAuthor.Name = "lblAuthor"
            Me.lblAuthor.Size = New System.Drawing.Size(144, 16)
            Me.lblAuthor.TabIndex = 4
            Me.lblAuthor.Text = "Author"
            '
            'lblVersion
            '
            Me.lblVersion.Location = New System.Drawing.Point(104, 56)
            Me.lblVersion.Name = "lblVersion"
            Me.lblVersion.Size = New System.Drawing.Size(56, 16)
            Me.lblVersion.TabIndex = 5
            Me.lblVersion.Text = "Version"
            '
            'gpxClock
            '
            Me.gpxClock.Controls.Add(Me.Label10)
            Me.gpxClock.Controls.Add(Me.cmbTimeSelector)
            Me.gpxClock.Controls.Add(Me.chkClockDuration)
            Me.gpxClock.Controls.Add(Me.numClockDuration)
            Me.gpxClock.Controls.Add(Me.cbxClockShow)
            Me.gpxClock.Controls.Add(Me.Label9)
            Me.gpxClock.Controls.Add(Me.numClockInterval)
            Me.gpxClock.Location = New System.Drawing.Point(227, 112)
            Me.gpxClock.Name = "gpxClock"
            Me.gpxClock.Size = New System.Drawing.Size(196, 227)
            Me.gpxClock.TabIndex = 30
            Me.gpxClock.TabStop = False
            Me.gpxClock.Text = "Clock"
            Me.ToolTip1.SetToolTip(Me.gpxClock, "Clock Settings")
            '
            'Label10
            '
            Me.Label10.AutoSize = True
            Me.Label10.Location = New System.Drawing.Point(40, 100)
            Me.Label10.Name = "Label10"
            Me.Label10.Size = New System.Drawing.Size(65, 13)
            Me.Label10.TabIndex = 21
            Me.Label10.Text = "Time Format"
            Me.ToolTip1.SetToolTip(Me.Label10, "Select how the time is displayed on the DMD (e.g., 12-hour AM/PM or 24-hour forma" &
        "t).")
            '
            'cmbTimeSelector
            '
            Me.cmbTimeSelector.FormattingEnabled = True
            Me.cmbTimeSelector.Items.AddRange(New Object() {"12-hour", "24-hour"})
            Me.cmbTimeSelector.Location = New System.Drawing.Point(111, 97)
            Me.cmbTimeSelector.Name = "cmbTimeSelector"
            Me.cmbTimeSelector.Size = New System.Drawing.Size(72, 21)
            Me.cmbTimeSelector.TabIndex = 34
            Me.ToolTip1.SetToolTip(Me.cmbTimeSelector, "Select how the time is displayed on the DMD (e.g., 12-hour AM/PM or 24-hour forma" &
        "t).")
            '
            'chkClockDuration
            '
            Me.chkClockDuration.AutoSize = True
            Me.chkClockDuration.Location = New System.Drawing.Point(62, 76)
            Me.chkClockDuration.Name = "chkClockDuration"
            Me.chkClockDuration.Size = New System.Drawing.Size(73, 13)
            Me.chkClockDuration.TabIndex = 19
            Me.chkClockDuration.Text = "Duration (sec)"
            Me.ToolTip1.SetToolTip(Me.chkClockDuration, "How long the clock stays visible.in seconds")
            '
            'numClockDuration
            '
            Me.numClockDuration.Location = New System.Drawing.Point(139, 74)
            Me.numClockDuration.Maximum = New Decimal(New Integer() {30, 0, 0, 0})
            Me.numClockDuration.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numClockDuration.Name = "numClockDuration"
            Me.numClockDuration.Size = New System.Drawing.Size(44, 20)
            Me.numClockDuration.TabIndex = 33
            Me.ToolTip1.SetToolTip(Me.numClockDuration, "How long the clock stays visible.in seconds")
            Me.numClockDuration.Value = New Decimal(New Integer() {5, 0, 0, 0})
            '
            'cbxClockShow
            '
            Me.cbxClockShow.AutoSize = True
            Me.cbxClockShow.Location = New System.Drawing.Point(97, 30)
            Me.cbxClockShow.Name = "cbxClockShow"
            Me.cbxClockShow.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxClockShow.Size = New System.Drawing.Size(83, 17)
            Me.cbxClockShow.TabIndex = 31
            Me.cbxClockShow.Text = "Show Clock"
            Me.cbxClockShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxClockShow, "When enabled it shows the current time on the DMD.")
            Me.cbxClockShow.UseVisualStyleBackColor = True
            '
            'Label9
            '
            Me.Label9.AutoSize = True
            Me.Label9.Location = New System.Drawing.Point(62, 53)
            Me.Label9.Name = "Label9"
            Me.Label9.Size = New System.Drawing.Size(68, 13)
            Me.Label9.TabIndex = 14
            Me.Label9.Text = "Interval (sec)"
            Me.ToolTip1.SetToolTip(Me.Label9, "Time in seconds between clock appearances..Note: it will finish the current categ" &
        "ory")
            '
            'numClockInterval
            '
            Me.numClockInterval.Increment = New Decimal(New Integer() {5, 0, 0, 0})
            Me.numClockInterval.Location = New System.Drawing.Point(139, 51)
            Me.numClockInterval.Maximum = New Decimal(New Integer() {300, 0, 0, 0})
            Me.numClockInterval.Minimum = New Decimal(New Integer() {5, 0, 0, 0})
            Me.numClockInterval.Name = "numClockInterval"
            Me.numClockInterval.Size = New System.Drawing.Size(44, 20)
            Me.numClockInterval.TabIndex = 32
            Me.ToolTip1.SetToolTip(Me.numClockInterval, "Time in seconds between clock appearances..Note: it will finish the current categ" &
        "ory")
            Me.numClockInterval.Value = New Decimal(New Integer() {39, 0, 0, 0})
            '
            'butExit
            '
            Me.butExit.Location = New System.Drawing.Point(594, 344)
            Me.butExit.Name = "butExit"
            Me.butExit.Size = New System.Drawing.Size(104, 32)
            Me.butExit.TabIndex = 250
            Me.butExit.Text = "Save && Exit"
            '
            'cbxUseHD
            '
            Me.cbxUseHD.AutoSize = True
            Me.cbxUseHD.Location = New System.Drawing.Point(-1, 152)
            Me.cbxUseHD.Name = "cbxUseHD"
            Me.cbxUseHD.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxUseHD.Size = New System.Drawing.Size(184, 17)
            Me.cbxUseHD.TabIndex = 100
            Me.cbxUseHD.Text = "Use HD rendering (256x64 pixels)"
            Me.cbxUseHD.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxUseHD, "Enable High Definition rendering (256x64 pixels). Preferred when using virtual DM" &
        "D.")
            Me.cbxUseHD.UseVisualStyleBackColor = True
            '
            'numMaxLoops
            '
            Me.numMaxLoops.Location = New System.Drawing.Point(142, 72)
            Me.numMaxLoops.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
            Me.numMaxLoops.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numMaxLoops.Name = "numMaxLoops"
            Me.numMaxLoops.Size = New System.Drawing.Size(44, 20)
            Me.numMaxLoops.TabIndex = 3
            Me.ToolTip1.SetToolTip(Me.numMaxLoops, "Number of times Video Media will be shown until it continues to another category." &
        ",")
            Me.numMaxLoops.Value = New Decimal(New Integer() {3, 0, 0, 0})
            '
            'txtPinemHiPath
            '
            Me.txtPinemHiPath.BackColor = System.Drawing.SystemColors.Window
            Me.txtPinemHiPath.Location = New System.Drawing.Point(83, 28)
            Me.txtPinemHiPath.Name = "txtPinemHiPath"
            Me.txtPinemHiPath.ReadOnly = True
            Me.txtPinemHiPath.Size = New System.Drawing.Size(154, 20)
            Me.txtPinemHiPath.TabIndex = 51
            Me.ToolTip1.SetToolTip(Me.txtPinemHiPath, "The folder path where PINemHi.exe is located. doubleclick textbox to search.")
            '
            'Label5
            '
            Me.Label5.AutoSize = True
            Me.Label5.Location = New System.Drawing.Point(6, 31)
            Me.Label5.Name = "Label5"
            Me.Label5.Size = New System.Drawing.Size(71, 13)
            Me.Label5.TabIndex = 13
            Me.Label5.Text = "PinemHi Path"
            Me.ToolTip1.SetToolTip(Me.Label5, "The folder path where PINemHi.exe is located. doubleclick textbox to search.")
            '
            'Label6
            '
            Me.Label6.AutoSize = True
            Me.Label6.Location = New System.Drawing.Point(44, 74)
            Me.Label6.Name = "Label6"
            Me.Label6.Size = New System.Drawing.Size(89, 13)
            Me.Label6.TabIndex = 14
            Me.Label6.Text = "Max Video Loops"
            Me.ToolTip1.SetToolTip(Me.Label6, "Number of times Video Media will be showed until it continues to another catagory" &
        ",")
            '
            'gpxPBX
            '
            Me.gpxPBX.Controls.Add(Me.Label12)
            Me.gpxPBX.Controls.Add(Me.lblColorPreview)
            Me.gpxPBX.Controls.Add(Me.btnPickColor)
            Me.gpxPBX.Controls.Add(Me.cbxUseHD)
            Me.gpxPBX.Controls.Add(Me.cbXShowPBXHighscores)
            Me.gpxPBX.Controls.Add(Me.cbxMediaShow)
            Me.gpxPBX.Controls.Add(Me.Label7)
            Me.gpxPBX.Controls.Add(Me.numFlexDuration)
            Me.gpxPBX.Controls.Add(Me.Label6)
            Me.gpxPBX.Controls.Add(Me.numMaxLoops)
            Me.gpxPBX.Location = New System.Drawing.Point(16, 112)
            Me.gpxPBX.Name = "gpxPBX"
            Me.gpxPBX.Size = New System.Drawing.Size(199, 227)
            Me.gpxPBX.TabIndex = 0
            Me.gpxPBX.TabStop = False
            Me.gpxPBX.Text = "PinballX"
            Me.ToolTip1.SetToolTip(Me.gpxPBX, "General Settings")
            '
            'Label12
            '
            Me.Label12.AutoSize = True
            Me.Label12.Location = New System.Drawing.Point(27, 126)
            Me.Label12.Name = "Label12"
            Me.Label12.Size = New System.Drawing.Size(65, 13)
            Me.Label12.TabIndex = 25
            Me.Label12.Text = "DMD Colour"
            Me.ToolTip1.SetToolTip(Me.Label12, "Click 'pick' button to select the DMD colour ")
            '
            'lblColorPreview
            '
            Me.lblColorPreview.AutoSize = True
            Me.lblColorPreview.Location = New System.Drawing.Point(91, 126)
            Me.lblColorPreview.Name = "lblColorPreview"
            Me.lblColorPreview.Size = New System.Drawing.Size(49, 13)
            Me.lblColorPreview.TabIndex = 24
            Me.lblColorPreview.Text = "XXXXXX"
            Me.ToolTip1.SetToolTip(Me.lblColorPreview, "Click 'pick' button to select the DMD colour ")
            '
            'btnPickColor
            '
            Me.btnPickColor.Location = New System.Drawing.Point(142, 121)
            Me.btnPickColor.Name = "btnPickColor"
            Me.btnPickColor.Size = New System.Drawing.Size(44, 23)
            Me.btnPickColor.TabIndex = 5
            Me.btnPickColor.Text = "Pick"
            Me.ToolTip1.SetToolTip(Me.btnPickColor, "Click 'pick' button to select the DMD colour ")
            Me.btnPickColor.UseVisualStyleBackColor = True
            '
            'cbXShowPBXHighscores
            '
            Me.cbXShowPBXHighscores.AutoSize = True
            Me.cbXShowPBXHighscores.Location = New System.Drawing.Point(72, 30)
            Me.cbXShowPBXHighscores.Name = "cbXShowPBXHighscores"
            Me.cbXShowPBXHighscores.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbXShowPBXHighscores.Size = New System.Drawing.Size(111, 17)
            Me.cbXShowPBXHighscores.TabIndex = 1
            Me.cbXShowPBXHighscores.Text = "Show HighScores"
            Me.cbXShowPBXHighscores.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbXShowPBXHighscores, "Display highscores from PinballX.")
            Me.cbXShowPBXHighscores.UseVisualStyleBackColor = True
            '
            'cbxMediaShow
            '
            Me.cbxMediaShow.AutoSize = True
            Me.cbxMediaShow.Location = New System.Drawing.Point(98, 49)
            Me.cbxMediaShow.Name = "cbxMediaShow"
            Me.cbxMediaShow.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxMediaShow.Size = New System.Drawing.Size(85, 17)
            Me.cbxMediaShow.TabIndex = 2
            Me.cbxMediaShow.Text = "Show Media"
            Me.cbxMediaShow.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxMediaShow, "Enable or disable DMD media (videos and/or Images)")
            Me.cbxMediaShow.UseVisualStyleBackColor = True
            '
            'Label7
            '
            Me.Label7.AutoSize = True
            Me.Label7.Location = New System.Drawing.Point(54, 100)
            Me.Label7.Name = "Label7"
            Me.Label7.Size = New System.Drawing.Size(79, 13)
            Me.Label7.TabIndex = 16
            Me.Label7.Text = "Image Duration"
            Me.ToolTip1.SetToolTip(Me.Label7, "Duration (sec)")
            '
            'numFlexDuration
            '
            Me.numFlexDuration.Location = New System.Drawing.Point(142, 98)
            Me.numFlexDuration.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
            Me.numFlexDuration.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numFlexDuration.Name = "numFlexDuration"
            Me.numFlexDuration.Size = New System.Drawing.Size(44, 20)
            Me.numFlexDuration.TabIndex = 4
            Me.ToolTip1.SetToolTip(Me.numFlexDuration, "Duration in seconds to display an image.")
            Me.numFlexDuration.Value = New Decimal(New Integer() {15, 0, 0, 0})
            '
            'gbxPINemHi
            '
            Me.gbxPINemHi.Controls.Add(Me.clbPinemHiScores)
            Me.gbxPINemHi.Controls.Add(Me.Label11)
            Me.gbxPINemHi.Controls.Add(Me.numExtraSeconds)
            Me.gbxPINemHi.Controls.Add(Me.cbxShowCountdown)
            Me.gbxPINemHi.Controls.Add(Me.cbxShowBadges)
            Me.gbxPINemHi.Controls.Add(Me.cbxShowPinEMHiHighScores)
            Me.gbxPINemHi.Controls.Add(Me.Label5)
            Me.gbxPINemHi.Controls.Add(Me.txtPinemHiPath)
            Me.gbxPINemHi.Location = New System.Drawing.Point(438, 112)
            Me.gbxPINemHi.Name = "gbxPINemHi"
            Me.gbxPINemHi.Size = New System.Drawing.Size(260, 227)
            Me.gbxPINemHi.TabIndex = 50
            Me.gbxPINemHi.TabStop = False
            Me.gbxPINemHi.Text = "PINemHi"
            Me.ToolTip1.SetToolTip(Me.gbxPINemHi, "PINemHi Highscore Settings ")
            '
            'clbPinemHiScores
            '
            Me.clbPinemHiScores.FormattingEnabled = True
            Me.clbPinemHiScores.Location = New System.Drawing.Point(46, 146)
            Me.clbPinemHiScores.Name = "clbPinemHiScores"
            Me.clbPinemHiScores.Size = New System.Drawing.Size(191, 64)
            Me.clbPinemHiScores.TabIndex = 56
            Me.ToolTip1.SetToolTip(Me.clbPinemHiScores, "Select which highscore categories to be included in the display.")
            '
            'Label11
            '
            Me.Label11.AutoSize = True
            Me.Label11.Location = New System.Drawing.Point(60, 98)
            Me.Label11.Name = "Label11"
            Me.Label11.Size = New System.Drawing.Size(124, 13)
            Me.Label11.TabIndex = 21
            Me.Label11.Text = "Extra Loading Time (sec)"
            Me.ToolTip1.SetToolTip(Me.Label11, "Adds additional seconds to the countdown to account for the time it takes to load" &
        " the VPX table.")
            '
            'numExtraSeconds
            '
            Me.numExtraSeconds.Location = New System.Drawing.Point(190, 95)
            Me.numExtraSeconds.Maximum = New Decimal(New Integer() {60, 0, 0, 0})
            Me.numExtraSeconds.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.numExtraSeconds.Name = "numExtraSeconds"
            Me.numExtraSeconds.Size = New System.Drawing.Size(44, 20)
            Me.numExtraSeconds.TabIndex = 54
            Me.ToolTip1.SetToolTip(Me.numExtraSeconds, "Adds additional seconds to the countdown to account for the time it takes to load" &
        " the VPX table.")
            Me.numExtraSeconds.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'cbxShowCountdown
            '
            Me.cbxShowCountdown.AutoSize = True
            Me.cbxShowCountdown.Location = New System.Drawing.Point(89, 75)
            Me.cbxShowCountdown.Name = "cbxShowCountdown"
            Me.cbxShowCountdown.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxShowCountdown.Size = New System.Drawing.Size(145, 17)
            Me.cbxShowCountdown.TabIndex = 53
            Me.cbxShowCountdown.Text = "Show Count Down Clock"
            Me.cbxShowCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxShowCountdown, "Enable a countdown timer for 5-minute challenge tables. (Requires Scutters' PINem" &
        "Hi plugin!)")
            Me.cbxShowCountdown.UseVisualStyleBackColor = True
            '
            'cbxShowBadges
            '
            Me.cbxShowBadges.AutoSize = True
            Me.cbxShowBadges.Location = New System.Drawing.Point(142, 52)
            Me.cbxShowBadges.Name = "cbxShowBadges"
            Me.cbxShowBadges.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxShowBadges.Size = New System.Drawing.Size(92, 17)
            Me.cbxShowBadges.TabIndex = 52
            Me.cbxShowBadges.Text = "Show Badges"
            Me.cbxShowBadges.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxShowBadges, "Displays badges on the DMD. NOTE: Icons are only visible with HD Rendering; stand" &
        "ard 128x32 displays badge count as text.")
            Me.cbxShowBadges.UseVisualStyleBackColor = True
            '
            'cbxShowPinEMHiHighScores
            '
            Me.cbxShowPinEMHiHighScores.AutoSize = True
            Me.cbxShowPinEMHiHighScores.Location = New System.Drawing.Point(125, 121)
            Me.cbxShowPinEMHiHighScores.Name = "cbxShowPinEMHiHighScores"
            Me.cbxShowPinEMHiHighScores.RightToLeft = System.Windows.Forms.RightToLeft.Yes
            Me.cbxShowPinEMHiHighScores.Size = New System.Drawing.Size(109, 17)
            Me.cbxShowPinEMHiHighScores.TabIndex = 55
            Me.cbxShowPinEMHiHighScores.Text = "Show Highscores"
            Me.cbxShowPinEMHiHighScores.TextAlign = System.Drawing.ContentAlignment.MiddleRight
            Me.ToolTip1.SetToolTip(Me.cbxShowPinEMHiHighScores, "Fetch and display highscores from PINemHi")
            Me.cbxShowPinEMHiHighScores.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            Me.btnCancel.Location = New System.Drawing.Point(484, 344)
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.Size = New System.Drawing.Size(104, 32)
            Me.btnCancel.TabIndex = 200
            Me.btnCancel.Text = "Cancel"
            '
            'ToolTip1
            '
            Me.ToolTip1.AutomaticDelay = 1000
            '
            'Configuration
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(710, 388)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.gbxPINemHi)
            Me.Controls.Add(Me.gpxPBX)
            Me.Controls.Add(Me.butExit)
            Me.Controls.Add(Me.grpPluginInfo)
            Me.Controls.Add(Me.pictureBox1)
            Me.Controls.Add(Me.gpxClock)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MinimizeBox = False
            Me.Name = "Configuration"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Configuration"
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.grpPluginInfo.ResumeLayout(False)
            Me.gpxClock.ResumeLayout(False)
            Me.gpxClock.PerformLayout()
            CType(Me.numClockDuration, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numClockInterval, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.numMaxLoops, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gpxPBX.ResumeLayout(False)
            Me.gpxPBX.PerformLayout()
            CType(Me.numFlexDuration, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gbxPINemHi.ResumeLayout(False)
            Me.gbxPINemHi.PerformLayout()
            CType(Me.numExtraSeconds, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

#Region "Events"
        Private Sub Configuration_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If File.Exists(strIniPath) Then
                    IniFile.Load(strIniPath)
                End If

                ' [FlexDMD]
                cbxUseHD.Checked = GetIniBool("FlexDMD", "UseHD", False)
                DLLpath = GetIniStr("FlexDMD", "FlexDMDDllPath", "")

                ' [PinballX]
                cbXShowPBXHighscores.Checked = GetIniBool("PinballX", "ShowHighscores", True)

                Dim cName = GetIniStr("PinballX", "DMDcolour", "DarkOrange")
                lblColorPreview.BackColor = ColorTranslator.FromHtml(cName)
                lblColorPreview.ForeColor = ColorTranslator.FromHtml(cName)

                numMaxLoops.Value = GetIniInt("PinballX", "MaxVideoLoops", 3)
                numFlexDuration.Value = GetIniDec("PinballX", "ImageDuration", 15)
                cbxMediaShow.Checked = GetIniBool("PinballX", "ShowMedia", True)


                ' [Clock]
                numClockDuration.Value = GetIniInt("Clock", "ClockDuration", 5)
                cbxClockShow.Checked = GetIniBool("Clock", "ShowClock", False)
                numClockInterval.Value = GetIniInt("Clock", "ShowClockEverySeconds", 30)
                Dim is24h = GetIniBool("Clock", "Show24h", True)
                cmbTimeSelector.SelectedIndex = If(is24h, 1, 0)

                ' [PINemHi] - General
                cbxShowPinEMHiHighScores.Checked = GetIniBool("PINemHi", "ShowHighscores", False)
                cbxShowBadges.Checked = GetIniBool("PINemHi", "ShowBadges", False)
                txtPinemHiPath.Text = GetIniStr("PINemHi", "PinemHiPath", "")
                cbxShowCountdown.Checked = GetIniBool("PINemHi", "ShowCountdown", False)
                numExtraSeconds.Value = GetIniInt("PINemHi", "ExtraSeconds", 15)

                ' [PINemHi] - Filters 
                clbPinemHiScores.Items.Clear()
                clbPinemHiScores.Items.Add("Personal Best", GetIniBool("PINemHi", "Show_TOP10_Personal", False))
                clbPinemHiScores.Items.Add("Personal Specials", GetIniBool("PINemHi", "Show_TOP10_Personal_Specials", False))
                clbPinemHiScores.Items.Add("Personal Best (5 min)", GetIniBool("PINemHi", "Show_TOP10_Personal_5min", False))
                clbPinemHiScores.Items.Add("Global Best", GetIniBool("PINemHi", "Show_TOP10_Best", False))
                clbPinemHiScores.Items.Add("Global Best (5 min)", GetIniBool("PINemHi", "Show_TOP10_Best_5min", False))
                clbPinemHiScores.Items.Add("Friends Scores", GetIniBool("PINemHi", "Show_TOP10_Friends", False))
                clbPinemHiScores.Items.Add("Friends Scores (5 min)", GetIniBool("PINemHi", "Show_TOP10_Friends_5min", False))
                clbPinemHiScores.Items.Add("Cup Standings", GetIniBool("PINemHi", "Show_TOP10_Cup", False))
                clbPinemHiScores.Items.Add("Cup Standings (5 min)", GetIniBool("PINemHi", "Show_TOP10_Cup_5min", False))
                clbPinemHiScores.Enabled = cbxShowPinEMHiHighScores.Checked

                DebugLogging = GetIniBool("Settings", "Debug", False)

                UpdateUIStates()
                hasChanges = False

            Catch ex As Exception
                MsgBox("Error while loading settings: " & ex.Message)
            End Try
        End Sub


        Private Sub butExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butExit.Click
            Dim isPinemHiActive As Boolean = cbxShowPinEMHiHighScores.Checked OrElse
                                     cbxShowBadges.Checked OrElse
                                     cbxShowCountdown.Checked


            If isPinemHiActive Then
                If Not IsValidPinemHiPath(txtPinemHiPath.Text) Then
                    MessageBox.Show("PinemHi features are enabled, but the root folder is invalid." & vbCrLf &
                            "Please select the correct folder before saving.",
                            "Invalid Configuration", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If
            Try
                IniFile.SetKeyValue("FlexDMD", "UseHD", cbxUseHD.Checked.ToString())
                IniFile.SetKeyValue("FlexDMD", "FlexDMDDllPath", DLLpath)
                IniFile.SetKeyValue("PinballX", "ShowMedia", cbxMediaShow.Checked.ToString())
                IniFile.SetKeyValue("PinballX", "MaxVideoLoops", numMaxLoops.Value.ToString())
                IniFile.SetKeyValue("PinballX", "ImageDuration", numFlexDuration.Value.ToString(Globalization.CultureInfo.InvariantCulture))
                IniFile.SetKeyValue("PinballX", "ShowHighscores", cbXShowPBXHighscores.Checked.ToString())

                Dim colorHex As String = ColorTranslator.ToHtml(lblColorPreview.BackColor)
                IniFile.SetKeyValue("PinballX", "DMDcolour", colorHex)


                Dim is24h As Boolean = (cmbTimeSelector.SelectedIndex = 1)
                IniFile.SetKeyValue("Clock", "Show24h", is24h.ToString())

                IniFile.SetKeyValue("Clock", "ClockDuration", numClockDuration.Value.ToString())
                IniFile.SetKeyValue("Clock", "ShowClock", cbxClockShow.Checked.ToString())
                IniFile.SetKeyValue("Clock", "ShowClockEverySeconds", numClockInterval.Value.ToString())

                ' PINemHi General
                IniFile.SetKeyValue("PINemHi", "PinemHiPath", txtPinemHiPath.Text)
                IniFile.SetKeyValue("PINemHi", "ShowBadges", cbxShowBadges.Checked.ToString())
                IniFile.SetKeyValue("PINemHi", "ShowCountdown", cbxShowCountdown.Checked.ToString())
                IniFile.SetKeyValue("PINemHi", "ExtraSeconds", numExtraSeconds.Value.ToString())
                IniFile.SetKeyValue("PINemHi", "ShowHighscores", cbxShowPinEMHiHighScores.Checked.ToString())
                'IniFile.SetKeyValue("PINemHi", "BadgesEarnedText", txtBadgesText.Text)
                'IniFile.SetKeyValue("PINemHi", "NoBadgesText", txtNoBadgesText.Text)

                ' PINemHi Categories 
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Personal", clbPinemHiScores.GetItemChecked(0).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Personal_Specials", clbPinemHiScores.GetItemChecked(2).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Personal_5min", clbPinemHiScores.GetItemChecked(1).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Best", clbPinemHiScores.GetItemChecked(3).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Best_5min", clbPinemHiScores.GetItemChecked(4).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Friends", clbPinemHiScores.GetItemChecked(5).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Friends_5min", clbPinemHiScores.GetItemChecked(6).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Cup", clbPinemHiScores.GetItemChecked(7).ToString())
                IniFile.SetKeyValue("PINemHi", "Show_TOP10_Cup_5min", clbPinemHiScores.GetItemChecked(8).ToString())

                ' Debug 
                IniFile.SetKeyValue("Settings", "Debug", DebugLogging.ToString)

                IniFile.Save(strIniPath)
                hasChanges = False
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Catch ex As Exception
                MsgBox("Error saving to INI: " & ex.Message)
            Finally
                Me.Close()
            End Try
        End Sub

        Private Sub btnPickColor_Click(sender As Object, e As EventArgs) Handles btnPickColor.Click
            Dim cd As New ColorDialog()
            cd.Color = lblColorPreview.BackColor
            If cd.ShowDialog() = DialogResult.OK Then
                lblColorPreview.BackColor = cd.Color
                lblColorPreview.ForeColor = cd.Color

            End If
        End Sub
        Private Sub cbxShowPinEMHiHighScores_CheckedChanged(sender As Object, e As EventArgs) Handles cbxShowPinEMHiHighScores.CheckedChanged
            If cbxShowPinEMHiHighScores.Enabled Then
                clbPinemHiScores.Enabled = cbxShowPinEMHiHighScores.Checked
            Else
                clbPinemHiScores.Enabled = False
            End If

        End Sub

        Private Sub txtPinemHiPath_Click(sender As Object, e As EventArgs) Handles txtPinemHiPath.Click
            Using fbd As New FolderBrowserDialog()
                fbd.Description = "Select the PinemHi Root Folder"
                fbd.ShowNewFolderButton = False

                If Not String.IsNullOrEmpty(txtPinemHiPath.Text) AndAlso Directory.Exists(txtPinemHiPath.Text) Then
                    fbd.SelectedPath = txtPinemHiPath.Text
                End If

                If fbd.ShowDialog() = DialogResult.OK Then
                    If IsValidPinemHiPath(fbd.SelectedPath) Then
                        txtPinemHiPath.Text = fbd.SelectedPath
                    Else
                        MessageBox.Show($"Looks like you didn't pick the PinemHi root folder{vbCrLf}Please retry",
                                "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Error)

                        txtPinemHiPath_Click(sender, e)
                    End If
                End If
            End Using
        End Sub
        Private Sub MarkAsChanged(sender As Object, e As EventArgs) Handles cbXShowPBXHighscores.CheckedChanged,
                                                                            numMaxLoops.ValueChanged,
                                                                            numFlexDuration.ValueChanged,
                                                                            cbxMediaShow.CheckedChanged,
                                                                            cbxUseHD.CheckedChanged,
                                                                            cmbTimeSelector.SelectedIndexChanged,
                                                                            numClockDuration.ValueChanged,
                                                                            cbxClockShow.CheckedChanged,
                                                                            numClockInterval.ValueChanged,
                                                                            cbxShowPinEMHiHighScores.CheckedChanged,
                                                                            txtPinemHiPath.TextChanged,
                                                                            cbxShowBadges.CheckedChanged,
                                                                            cbxShowCountdown.CheckedChanged,
                                                                            numExtraSeconds.ValueChanged,
                                                                            lblColorPreview.BackColorChanged,
                                                                            cbxShowPinEMHiHighScores.CheckedChanged

            hasChanges = True
        End Sub

        Private Sub clbPinemHiScores_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles clbPinemHiScores.ItemCheck
            hasChanges = True
        End Sub
        Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
            Me.Close()
        End Sub

        Private Sub Configuration_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
            ' Controleer of er wijzigingen zijn
            If hasChanges Then
                Dim result = MessageBox.Show("You have unsaved changes. Are you sure you want to exit without saving?",
                                             "Unsaved Changes",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Warning)

                ' Als de gebruiker op 'No' klikt, annuleren we het sluiten van het formulier
                If result = DialogResult.No Then
                    e.Cancel = True
                End If
            End If
        End Sub

        Private Sub UIstateChanges_CheckedChanged(sender As Object, e As EventArgs) Handles cbxMediaShow.CheckedChanged,
                                                                                            cbxClockShow.CheckedChanged,
                                                                                            cbxShowPinEMHiHighScores.CheckedChanged,
                                                                                            cbxShowCountdown.CheckedChanged,
                                                                                            txtPinemHiPath.TextChanged
            UpdateUIStates()
        End Sub



#End Region

#Region "Helper Methods"
        Private Function GetIniBool(section As String, key As String, defaultValue As Boolean) As Boolean
            Dim v = IniFile.GetKeyValue(section, key)
            If String.IsNullOrEmpty(v) Then Return defaultValue
            Return (v.ToLower() = "true" Or v = "1")
        End Function

        Private Function GetIniInt(section As String, key As String, defaultValue As Integer) As Integer
            Dim v = IniFile.GetKeyValue(section, key)
            Dim res As Integer
            If Integer.TryParse(v, res) Then Return res
            Return defaultValue
        End Function

        Private Function GetIniDec(section As String, key As String, defaultValue As Decimal) As Decimal
            Dim v = IniFile.GetKeyValue(section, key)
            If String.IsNullOrEmpty(v) Then Return defaultValue
            v = v.Replace(",", ".")
            Dim res As Decimal
            If Decimal.TryParse(v, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, res) Then Return res
            Return defaultValue
        End Function

        Private Function GetIniStr(section As String, key As String, defaultValue As String) As String
            Dim v = IniFile.GetKeyValue(section, key)
            If String.IsNullOrEmpty(v) Then Return defaultValue
            Return v
        End Function

        Private Function IsValidPinemHiPath(pathString As String) As Boolean
            If String.IsNullOrEmpty(pathString) OrElse Not Directory.Exists(pathString) Then
                Return False
            End If

            Dim exePath As String = Path.Combine(pathString, "PINemHi.exe")
            Dim boardPath As String = Path.Combine(pathString, "PINemHi LeaderBoard")

            Return File.Exists(exePath) AndAlso Directory.Exists(boardPath)
        End Function

        Private Sub UpdateUIStates()
            ' Media Settings Logic
            Dim mediaEnabled As Boolean = cbxMediaShow.Checked
            numFlexDuration.Enabled = mediaEnabled
            numMaxLoops.Enabled = mediaEnabled

            ' Clock Settings Logic
            Dim clockEnabled As Boolean = cbxClockShow.Checked
            numClockInterval.Enabled = clockEnabled
            numClockDuration.Enabled = clockEnabled
            cmbTimeSelector.Enabled = clockEnabled

            ' PINemHi Path & Validity Logic
            Dim isPathValid As Boolean = IsValidPinemHiPath(txtPinemHiPath.Text)

            cbxShowBadges.Enabled = isPathValid
            cbxShowCountdown.Enabled = isPathValid
            numExtraSeconds.Enabled = isPathValid AndAlso cbxShowCountdown.Checked
            cbxShowPinEMHiHighScores.Enabled = isPathValid
            clbPinemHiScores.Enabled = isPathValid AndAlso cbxShowPinEMHiHighScores.Checked


        End Sub
#End Region
    End Class

End Namespace

