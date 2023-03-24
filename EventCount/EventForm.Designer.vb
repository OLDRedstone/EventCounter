<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class EventForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(EventForm))
        OpenLevel = New OpenFileDialog()
        TitleBox = New PictureBox()
        InfoLabel = New Label()
        Timer1 = New Timer(components)
        CType(TitleBox, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' OpenLevel
        ' 
        OpenLevel.Filter = "节奏医生关卡文件|*.rdlevel;*.rdzip"' 
        ' TitleBox
        ' 
        TitleBox.Location = New Point(0, 0)
        TitleBox.Name = "TitleBox"
        TitleBox.Size = New Size(48, 152)
        TitleBox.TabIndex = 0
        TitleBox.TabStop = False
        ' 
        ' InfoLabel
        ' 
        InfoLabel.AutoSize = True
        InfoLabel.ForeColor = Color.White
        InfoLabel.Location = New Point(0, 155)
        InfoLabel.Name = "InfoLabel"
        InfoLabel.Size = New Size(12, 17)
        InfoLabel.TabIndex = 1
        InfoLabel.Text = " "' 
        ' Timer1
        ' 
        Timer1.Interval = 10
        ' 
        ' EventForm
        ' 
        AutoScaleDimensions = New SizeF(96F, 96F)
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = Color.FromArgb(CByte(44), CByte(44), CByte(44))
        BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        BackgroundImageLayout = ImageLayout.None
        ClientSize = New Size(0, 0)
        Controls.Add(InfoLabel)
        Controls.Add(TitleBox)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.Fixed3D
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "EventForm"
        TransparencyKey = Color.Coral
        CType(TitleBox, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents OpenLevel As OpenFileDialog
    Friend WithEvents TitleBox As PictureBox
    Friend WithEvents InfoLabel As Label
    Friend WithEvents Timer1 As Timer
End Class
