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
		ProgressBar1 = New ProgressBar()
		Panel1 = New Panel()
		Label1 = New Label()
		TickTimer = New Timer(components)
		CloseButton = New PictureBox()
		Panel2 = New Panel()
		Label2 = New Label()
		EventPanel1 = New EventPanel()
		RichTextLabel1 = New RichTextLabel()
		HideButton = New PictureBox()
		OutputButton = New PictureBox()
		CType(TitleBox, ComponentModel.ISupportInitialize).BeginInit()
		Panel1.SuspendLayout()
		CType(CloseButton, ComponentModel.ISupportInitialize).BeginInit()
		Panel2.SuspendLayout()
		CType(HideButton, ComponentModel.ISupportInitialize).BeginInit()
		CType(OutputButton, ComponentModel.ISupportInitialize).BeginInit()
		SuspendLayout()
		' 
		' OpenLevel
		' 
		OpenLevel.Filter = "节奏医生关卡文件|*.rdlevel;*.rdzip"
		' 
		' TitleBox
		' 
		TitleBox.Location = New Point(0, 0)
		TitleBox.Name = "TitleBox"
		TitleBox.Size = New Size(48, 152)
		TitleBox.TabIndex = 0
		TitleBox.TabStop = False
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
		Panel1.BackColor = Color.FromArgb(CByte(59), CByte(59), CByte(59))
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
		Label1.Text = "Label1"
		' 
		' TickTimer
		' 
		TickTimer.Enabled = True
		TickTimer.Interval = 1
		' 
		' CloseButton
		' 
		CloseButton.BackgroundImage = CType(resources.GetObject("CloseButton.BackgroundImage"), Image)
		CloseButton.Location = New Point(272, 118)
		CloseButton.Name = "CloseButton"
		CloseButton.Size = New Size(28, 28)
		CloseButton.TabIndex = 5
		CloseButton.TabStop = False
		' 
		' Panel2
		' 
		Panel2.AutoScroll = True
		Panel2.Controls.Add(Label2)
		Panel2.Location = New Point(78, 18)
		Panel2.Name = "Panel2"
		Panel2.Size = New Size(34, 0)
		Panel2.TabIndex = 6
		' 
		' Label2
		' 
		Label2.AutoSize = True
		Label2.ForeColor = Color.Cyan
		Label2.Location = New Point(3, 3)
		Label2.Name = "Label2"
		Label2.Size = New Size(46, 17)
		Label2.TabIndex = 0
		Label2.Text = "Label2"
		' 
		' EventPanel1
		' 
		EventPanel1.BackColor = Color.Transparent
		EventPanel1.BackgroundImage = CType(resources.GetObject("EventPanel1.BackgroundImage"), Image)
		EventPanel1.EventChoosen = CType(resources.GetObject("EventPanel1.EventChoosen"), Dictionary(Of String, Boolean))
		EventPanel1.Location = New Point(48, 34)
		EventPanel1.Margin = New Padding(0)
		EventPanel1.Name = "EventPanel1"
		EventPanel1.Size = New Size(252, 112)
		EventPanel1.TabIndex = 7
		' 
		' RichTextLabel1
		' 
		RichTextLabel1.BackgroundImage = CType(resources.GetObject("RichTextLabel1.BackgroundImage"), Image)
		RichTextLabel1.Location = New Point(0, 152)
		RichTextLabel1.Name = "RichTextLabel1"
		RichTextLabel1.ShowText = True
		RichTextLabel1.Size = New Size(302, 17)
		RichTextLabel1.TabIndex = 8
		RichTextLabel1.Title = Nothing
		' 
		' HideButton
		' 
		HideButton.BackgroundImage = My.Resources.Resources.HideTextSelected
		HideButton.Location = New Point(272, 90)
		HideButton.Name = "HideButton"
		HideButton.Size = New Size(28, 28)
		HideButton.TabIndex = 5
		HideButton.TabStop = False
		' 
		' OutputButton
		' 
		OutputButton.BackgroundImage = CType(resources.GetObject("OutputButton.BackgroundImage"), Image)
		OutputButton.Location = New Point(272, 62)
		OutputButton.Name = "OutputButton"
		OutputButton.Size = New Size(28, 28)
		OutputButton.TabIndex = 5
		OutputButton.TabStop = False
		' 
		' EventForm
		' 
		AutoScaleMode = AutoScaleMode.None
		BackColor = Color.FromArgb(CByte(44), CByte(44), CByte(44))
		BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), Image)
		BackgroundImageLayout = ImageLayout.None
		ClientSize = New Size(10, 10)
		Controls.Add(RichTextLabel1)
		Controls.Add(Panel2)
		Controls.Add(OutputButton)
		Controls.Add(HideButton)
		Controls.Add(CloseButton)
		Controls.Add(ProgressBar1)
		Controls.Add(TitleBox)
		Controls.Add(Panel1)
		Controls.Add(EventPanel1)
		DoubleBuffered = True
		FormBorderStyle = FormBorderStyle.None
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		Name = "EventForm"
		TransparencyKey = Color.Coral
		CType(TitleBox, ComponentModel.ISupportInitialize).EndInit()
		Panel1.ResumeLayout(False)
		Panel1.PerformLayout()
		CType(CloseButton, ComponentModel.ISupportInitialize).EndInit()
		Panel2.ResumeLayout(False)
		Panel2.PerformLayout()
		CType(HideButton, ComponentModel.ISupportInitialize).EndInit()
		CType(OutputButton, ComponentModel.ISupportInitialize).EndInit()
		ResumeLayout(False)
	End Sub

	Friend WithEvents OpenLevel As OpenFileDialog
	Friend WithEvents EventPanel1 As EventPanel
	Friend WithEvents TitleBox As PictureBox
	Friend WithEvents ProgressBar1 As ProgressBar
	Friend WithEvents Panel1 As Panel
	Friend WithEvents Label1 As Label
	Friend WithEvents TickTimer As Timer
	Friend WithEvents CloseButton As PictureBox
	Friend WithEvents Panel2 As Panel
	Friend WithEvents Label2 As Label
	Friend WithEvents RichTextLabel1 As RichTextLabel
	Friend WithEvents HideButton As PictureBox
	Friend WithEvents OutputButton As PictureBox
End Class
