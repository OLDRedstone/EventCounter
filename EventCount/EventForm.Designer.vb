﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Timer2 = New Timer(components)
        ProgressBar1 = New ProgressBar()
        Panel1 = New Panel()
        Label1 = New Label()
        Button1 = New Button()
        CType(TitleBox, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
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
        InfoLabel.Size = New Size(19, 17)
        InfoLabel.TabIndex = 1
        InfoLabel.Text = " 0"' 
        ' Timer2
        ' 
        Timer2.Enabled = True
        Timer2.Interval = 1
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(114, 4)
        ProgressBar1.MarqueeAnimationSpeed = 10
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(188, 14)
        ProgressBar1.TabIndex = 2
        ' 
        ' Panel1
        ' 
        Panel1.BackColor = Color.FromArgb(CByte(44), CByte(44), CByte(44))
        Panel1.Controls.Add(Label1)
        Panel1.Location = New Point(114, 19)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(188, 0)
        Panel1.TabIndex = 3
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.ForeColor = Color.White
        Label1.Location = New Point(0, 2)
        Label1.Name = "Label1"
        Label1.Size = New Size(46, 17)
        Label1.TabIndex = 0
        Label1.Text = "Label1"' 
        ' Button1
        ' 
        Button1.BackColor = Color.FromArgb(CByte(255), CByte(192), CByte(192))
        Button1.FlatAppearance.BorderColor = Color.Maroon
        Button1.FlatAppearance.MouseDownBackColor = Color.Red
        Button1.FlatAppearance.MouseOverBackColor = Color.FromArgb(CByte(255), CByte(128), CByte(128))
        Button1.FlatStyle = FlatStyle.Popup
        Button1.Location = New Point(272, 118)
        Button1.Name = "Button1"
        Button1.Size = New Size(28, 28)
        Button1.TabIndex = 4
        Button1.UseVisualStyleBackColor = False
        ' 
        ' EventForm
        ' 
        AutoScaleMode = AutoScaleMode.None
        BackColor = Color.FromArgb(CByte(44), CByte(44), CByte(44))
        BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
        BackgroundImageLayout = ImageLayout.None
        ClientSize = New Size(0, 0)
        Controls.Add(Button1)
        Controls.Add(ProgressBar1)
        Controls.Add(InfoLabel)
        Controls.Add(TitleBox)
        Controls.Add(Panel1)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.Fixed3D
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "EventForm"
        TransparencyKey = Color.Coral
        CType(TitleBox, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents OpenLevel As OpenFileDialog
    Friend WithEvents TitleBox As PictureBox
    Friend WithEvents InfoLabel As Label
    Friend WithEvents Timer2 As Timer
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Button1 As Button
End Class
