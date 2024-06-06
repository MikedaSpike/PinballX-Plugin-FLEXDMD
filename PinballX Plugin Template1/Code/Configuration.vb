Namespace PinballX
    Public Class Configuration
        Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call

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
        Friend WithEvents butExit As System.Windows.Forms.Button
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
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
            Me.butExit = New System.Windows.Forms.Button()
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.grpPluginInfo.SuspendLayout()
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
            Me.grpPluginInfo.Size = New System.Drawing.Size(368, 128)
            Me.grpPluginInfo.TabIndex = 7
            Me.grpPluginInfo.TabStop = False
            Me.grpPluginInfo.Text = "Plugin Information"
            '
            'label4
            '
            Me.label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label4.Location = New System.Drawing.Point(8, 56)
            Me.label4.Name = "label4"
            Me.label4.Size = New System.Drawing.Size(96, 16)
            Me.label4.TabIndex = 9
            Me.label4.Text = "Description:"
            Me.label4.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label3
            '
            Me.label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label3.Location = New System.Drawing.Point(8, 40)
            Me.label3.Name = "label3"
            Me.label3.Size = New System.Drawing.Size(96, 16)
            Me.label3.TabIndex = 8
            Me.label3.Text = "Author:"
            Me.label3.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label2
            '
            Me.label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label2.Location = New System.Drawing.Point(256, 24)
            Me.label2.Name = "label2"
            Me.label2.Size = New System.Drawing.Size(48, 16)
            Me.label2.TabIndex = 7
            Me.label2.Text = "Version:"
            Me.label2.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'label1
            '
            Me.label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.label1.Location = New System.Drawing.Point(8, 24)
            Me.label1.Name = "label1"
            Me.label1.Size = New System.Drawing.Size(96, 16)
            Me.label1.TabIndex = 6
            Me.label1.Text = "Name:"
            Me.label1.TextAlign = System.Drawing.ContentAlignment.TopRight
            '
            'lblDescription
            '
            Me.lblDescription.Location = New System.Drawing.Point(104, 56)
            Me.lblDescription.Name = "lblDescription"
            Me.lblDescription.Size = New System.Drawing.Size(256, 16)
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
            Me.lblAuthor.Size = New System.Drawing.Size(256, 16)
            Me.lblAuthor.TabIndex = 4
            Me.lblAuthor.Text = "Author"
            '
            'lblVersion
            '
            Me.lblVersion.Location = New System.Drawing.Point(304, 24)
            Me.lblVersion.Name = "lblVersion"
            Me.lblVersion.Size = New System.Drawing.Size(56, 16)
            Me.lblVersion.TabIndex = 5
            Me.lblVersion.Text = "Version"
            '
            'butExit
            '
            Me.butExit.Location = New System.Drawing.Point(379, 152)
            Me.butExit.Name = "butExit"
            Me.butExit.Size = New System.Drawing.Size(104, 32)
            Me.butExit.TabIndex = 9
            Me.butExit.Text = "Exit"
            '
            'Configuration
            '
            Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
            Me.ClientSize = New System.Drawing.Size(494, 198)
            Me.Controls.Add(Me.butExit)
            Me.Controls.Add(Me.grpPluginInfo)
            Me.Controls.Add(Me.pictureBox1)
            Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
            Me.MinimizeBox = False
            Me.Name = "Configuration"
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "Configuration"
            CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
            Me.grpPluginInfo.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Private Sub Configuration_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.Text = PluginInfo.Name & " (" & PluginInfo.Version & ")"
            lblName.Text = PluginInfo.Name
            lblVersion.Text = PluginInfo.Version
            lblAuthor.Text = PluginInfo.Author
            lblDescription.Text = PluginInfo.Description
        End Sub

        Private Sub butExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles butExit.Click
            Me.Close()
        End Sub
    End Class
End Namespace

