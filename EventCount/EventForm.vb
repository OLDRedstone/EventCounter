Imports System.ComponentModel
Imports System.Drawing.Text
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports EventCount.PublicLib

Public Class EventForm

	Public ReadOnly Property IconPath = $"{Application.StartupPath}\Assets\"

	Public Property FilePath As String = ""
	Public Property RdlevelFile As String = ""
	Public ReadOnly Property TempFile As String = "\%EventCounterRDLevelUnzip%"
	Public Property LevelName As String = ""

	Public fontCollection As New PrivateFontCollection '创建字体集合

	Public RDFont As Font

	Public Property EventTypeIndex = EventT.Sounds

	Public Property Progress As Double = 0

	Public Property Maximum As UInt64 = 0

	Public Property ChosenEvent As New List(Of String)


	ReadOnly Property RamCounter = New PerformanceCounter("Process", "Private Bytes", Process.GetCurrentProcess.ProcessName)

	Public ReadOnly Property Counter As New Counter

	Public ReadOnly Property Assets As New Assets
	Public ReadOnly Property PublicLib As New PublicLib
	Public ReadOnly Property Achievements As Achievements
	Public Property ShowData As Boolean = True
	Sub ChangeFont()
		RDFont = Assets.Font
		Label1.Font = RDFont
		Label2.Font = RDFont
		RichTextLabel1.Font = RDFont
	End Sub

	Public Sub Form_Load() Handles MyBase.Load
		AddHandler TitleBox.Click, AddressOf TitleBox_Click
		AddHandler MyBase.Click, AddressOf EventForm_Click
		AddHandler MyBase.Closing, AddressOf EventForm_Closing
		AddHandler CloseButton.MouseEnter, AddressOf MouseEnterTheCloseButton
		AddHandler CloseButton.Click, AddressOf EventForm_Closed
		AddHandler CloseButton.MouseLeave, AddressOf MouseLeaveTheCloseButton
		AddHandler HideButton.MouseUp, AddressOf MouseClickTheHiddenButton
		AddHandler TickTimer.Tick, AddressOf Ticking
		For Each Ctrl As Control In MyBase.Controls
			AddHandler Ctrl.MouseDown, AddressOf MoveFormStart
			AddHandler Ctrl.MouseMove, AddressOf MoveForm
		Next
		AddHandler MouseDown, AddressOf MoveFormStart
		AddHandler MouseMove, AddressOf MoveForm
		AddHandler OpenLevel.FileOk, AddressOf OpenLevel_FileOk
		AddHandler ProgressBar1.MouseEnter, AddressOf MouseEnterTheAdLabel
		AddHandler ProgressBar1.MouseLeave, AddressOf MouseLeaveTheAdLabel
		AddHandler EventPanel1.ItemChoosenActivatedClick, AddressOf PanelChoosenActivatedClick
		AddHandler EventPanel1.ItemChoosenInactivatedClick, AddressOf PanelChoosenInactivatedClick
		AddHandler EventPanel1.ItemCleared, AddressOf EventClear
		AddHandler EventPanel1.ItemReversed, AddressOf EventReverse
		AddHandler EventPanel1.HoverItemChanged, AddressOf HoveringItemChanged
		AddHandler RichTextLabel1.ItemClick, AddressOf RichLabelClickItem
		AddHandler Counter.ProgressChanged, AddressOf ProgressChanged
		AddHandler OutputButton.MouseEnter, AddressOf MouseEnterTheOutputButton
		AddHandler OutputButton.Click, AddressOf MouseClickTheOutputButton
		AddHandler OutputButton.MouseLeave, AddressOf MouseLeaveTheOutputButton
		FirstLoad()
		Reload()
		TickTimer.Start()
		Control.CheckForIllegalCrossThreadCalls = False
	End Sub


	Sub FirstLoad()
		Achievements.Read()
		EventPanel1.SetList(EventT.Sounds, Counter.GetList(EventT.Sounds), New List(Of String))
		ChangeFont()
		Me.Location = My.Computer.Screen.Bounds.Size / 2
		RichTextLabel1.Top = TitleBox.Height
		RichTextLabel1.Width = ProgressBar1.Left + ProgressBar1.Width
		ShowSelEvents("")
	End Sub

	Sub Reload()
		Maximum = 0
		Progress = 0
		ChosenEvent.Clear()
		RenewPanel()

	End Sub

	Sub MouseEnterTheAdLabel()
		CallTicking(Panel1.Height, Label1.Height + Label1.Top)
	End Sub


	Sub MouseLeaveTheAdLabel()
		CallTicking(Panel1.Height, 0)
	End Sub

	Sub HoveringItemChanged(s As String)
		ShowSelEvents(s)
	End Sub

	Sub ShowSelEvents(Head As String)
		RichTextLabel1.Title = Head
		RichTextLabel1.EventList = ChosenEvent.ToDictionary(Function(i) i, Function(i) GetCount(i))
		CallTicking(Me.Size, New SizeF(RichTextLabel1.Width, RichTextLabel1.Height + RichTextLabel1.Top))
		EventPanel1.RefreshUI()
	End Sub

	Public Sub KillTemp()
		If My.Computer.FileSystem.DirectoryExists($"{Environ("TEMP")}{TempFile}") Then
			My.Computer.FileSystem.DeleteDirectory($"{Environ("TEMP")}{TempFile}",
			FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
		End If
	End Sub

	Private Sub PanelChoosenActivatedClick(key As String)
		Assets.PlaySnd(SoundType.On)
		If Not ChosenEvent.Contains(key) Then
			ChosenEvent.Add(key)
			'If Not Achievements.AchiDic.ContainsKey1("Move") Then
			'	Dim CurSize As Size = Me.Size
			'	CallTicking(Me.ClientRectangle, New RectangleF(New PointF(0, 0), CurSize))
			'End If
			'Achievements.Add("Move", "窗口移动才是精髓", "The spirit of moving window!")
		End If
		ShowSelEvents(EventPanel1.HoverItem)
	End Sub

	Private Sub PanelChoosenInactivatedClick(key As String)
		Assets.PlaySnd(SoundType.Off)
		If ChosenEvent.Contains(key) Then
			ChosenEvent.Remove(key)
		End If
		ShowSelEvents(EventPanel1.HoverItem)
	End Sub
	Private Sub OpenLevel_FileOk()
	End Sub

	Sub RenewPanel()
		Dim TitleImage = My.Resources.ResourceData.Title
		TitleBox.BackgroundImage = TitleImage.Clone(
			New Rectangle(EventTypeIndex * 48, 0, 48, 152),
			TitleImage.PixelFormat)
	End Sub

	Public Sub RichLabelClickItem(name As String)
		If name = "" Or Not ShowData Then
			Exit Sub
		End If
		PanelChoosenInactivatedClick(name)
		EventPanel1.SetList(EventTypeIndex, Counter.GetList(EventTypeIndex), ChosenEvent)
	End Sub

	Private Sub TitleBox_Click(sender As Object, e As MouseEventArgs)
		Dim P = PointToClient(Cursor.Position)
		Dim Q = New Point((P.X - 4) \ 30, Math.Clamp((P.Y - 4) \ 29, 0, 4))

		Select Case e.Button
			Case MouseButtons.Left
				If Q.Y = EventTypeIndex Then
					Assets.PlaySnd(SoundType.On)
					EventPanel1.Reverse()
				Else
					EventPanel1.SetList(Q.Y, Counter.GetList(Q.Y), ChosenEvent)
					EventTypeIndex = Q.Y
					Assets.PlaySnd(EventTypeIndex)
				End If
			Case MouseButtons.Right
				Assets.PlaySnd(SoundType.Off)
				EventPanel1.Clear()
		End Select
		RenewPanel()
		ShowSelEvents("")
	End Sub

	Shared Function Ease(x As Double) As Double
		x = Math.Clamp(x, 0, 1)
		Return Math.Clamp(1 - 2 ^ (-10 * x), 0, 1)
	End Function

	Public Sub EventReverse(ReversedList As Dictionary(Of String, Boolean))
		For Each item In ReversedList
			If ChosenEvent.Contains(item.Key) Then
				ChosenEvent.Remove(item.Key)
			End If
		Next
		ChosenEvent.AddRange((From i In ReversedList Where i.Value Select i.Key).ToList)
		EventPanel1.RefreshUI()
	End Sub

	Public Sub EventClear(ET As EventT)
		ChosenEvent.RemoveAll(Function(i) Counter.GetList(ET).ContainsKey(i))
		EventPanel1.RefreshUI()
	End Sub

	Sub MouseEnterTheCloseButton()
		CloseButton.BackgroundImage = Image.FromFile(GetFile("Icons\ClosingFormSelected.png"))
	End Sub

	Sub MouseLeaveTheCloseButton()
		CloseButton.BackgroundImage = Image.FromFile(GetFile("Icons\ClosingForm.png"))
	End Sub

	Sub MouseClickTheHiddenButton()
		ShowData = Not ShowData
		RichTextLabel1.ShowText = ShowData
		If ShowData Then
			HideButton.BackgroundImage = Image.FromFile(GetFile("Icons\HideTextSelected.png"))
		Else
			HideButton.BackgroundImage = Image.FromFile(GetFile("Icons\HideText.png"))
		End If
		Assets.PlaySnd(SoundType.On)
	End Sub
	Sub MouseEnterTheOutputButton()
		OutputButton.BackgroundImage = Image.FromFile(GetFile("Icons\OutputButtonActive.png"))
	End Sub

	Sub MouseClickTheOutputButton()
		Assets.PlaySnd(SoundType.On)
		RichTextLabel1.toImage(True, 0, Short.MaxValue).Save($"{Path.GetDirectoryName(OpenLevel.FileName)}\Output.png")
		RichTextLabel1.toImage(False, 0, Short.MaxValue).Save($"{Path.GetDirectoryName(OpenLevel.FileName)}\OutputHidden.png")
		Dim b = New StreamWriter($"{Path.GetDirectoryName(OpenLevel.FileName)}\Output.txt")
		For Each I In Counter.GetList().OrderBy(Function(j) -j.Value.Count)
			b.WriteLine($"{I.Value.Type}{vbTab}{I.Value.Count}{vbTab}{GetLang(I.Key, CurrentLang)}")
		Next
		b.Close()
	End Sub

	Sub MouseLeaveTheOutputButton()
		OutputButton.BackgroundImage = Image.FromFile(GetFile("Icons\OutputButton.png"))
	End Sub

	Private Sub EventForm_Closed()
		Assets.PlaySnd(SoundType.Off)
		CloseWindow = True
		Achievements.Write()
		CallTicking(Me.Size, New SizeF(0, 0))
	End Sub
	Private Sub EventForm_Closing(sender As Object, e As CancelEventArgs)
		e.Cancel = True
		EventForm_Closed()
	End Sub

	Private MeLocation As Point
	Function MoveFormStart()
		MeLocation = Me.PointToClient(MousePosition)

		Return Nothing
	End Function
	Function MoveForm()

		If Control.MouseButtons = MouseButtons.Left Then

			Me.Location = MousePosition - MeLocation
		End If
		Return Nothing
	End Function

	Private AchiFrameOpened As Boolean = False
	Private Sub LoadRDFile()
		Assets.PlaySnd(SoundType.Close)
		If Path.GetExtension(OpenLevel.FileName) = ".rdlevel" Then
			FilePath = OpenLevel.FileName
		ElseIf Path.GetExtension(OpenLevel.FileName) = ".rdzip" Then
			KillTemp()
			Compression.ZipFile.ExtractToDirectory(OpenLevel.FileName, $"{Environ("TEMP")}{TempFile}")
			Dim s As String = Directory.GetFiles($"{Environ("TEMP")}{TempFile}", "*.rdlevel")(0)
			FilePath = $"{Environ("TEMP")}{TempFile}\{Path.GetFileName(s)}"
		Else
			Achievements.Add("NoFiles", "但是没文件啊？", "But No Files Here!")
		End If
		If File.Exists(FilePath) Then
			Dim readFile = IO.File.OpenText(FilePath)
			Dim tempTask As Task
			'Dim RDLevel = readFile.ReadToEnd
			Counter.dataSender.Enabled = True
			EventPanel1.Enabled = False
			tempTask = New Task(Sub()
									Try
										Counter.Load(readFile)
									Catch ex As OutOfMemoryException
										Achievements.Add("Boom", "爆炸啦！", "Boom!")
									End Try
									readFile.Dispose()
									GC.Collect()
									LevelName = Regex.Replace(UnicodeToString(If(Counter.SongName, "")), "</?color(=#?\w*)?>", "")
									EventPanel1.SetList(EventTypeIndex, Counter.GetList(EventTypeIndex), New List(Of String))
									EventPanel1.Enabled = True
									ChosenEvent.Clear()
									Reload()
								End Sub)
			tempTask.Start()
		End If

	End Sub
	Private lastProgress
	Public Sub ProgressChanged(Progress As Double)
		Me.Progress = Progress
		Dim millSeconds As Double = (1 - Progress) / ((Progress - lastProgress) / (Counter.dataSender.Interval))
		Dim timeRamain = TimeSpan.FromMilliseconds(
			If(
				Double.IsNaN(millSeconds),
				0,
				millSeconds))
		ProgressBar1.Value = Progress * ProgressBar1.Maximum
		lastProgress = Progress
		Label1.Text = $"预计还剩 {timeRamain.TotalSeconds:0.#} s"
	End Sub
	Private Sub EventForm_Click()
		Dim P = PointToClient(MousePosition)
		If P.X > 48 And P.X < 79 And P.Y > 4 And P.Y < 19 Then
			Assets.PlaySnd(SoundType.Open)
			If OpenLevel.ShowDialog() = DialogResult.OK Then
				LoadRDFile()
			End If
			'Reload()
			'ShowSelEvents("")
		End If
		If P.X > 80 And P.X < 111 And P.Y > 4 And P.Y < 19 Then
			If AchiFrameOpened Then
				CallTicking(New RectangleF(Panel2.Location, Panel2.Size), New Rectangle(78, 18, 34, 0))
			Else
				CallTicking(New RectangleF(Panel2.Location, Panel2.Size), New Rectangle(48, 20, 302 - 48, 152 - 20))
				Label2.Text = ShowAchievements("-Achievements-")
			End If
			AchiFrameOpened = Not AchiFrameOpened
		End If
	End Sub

	Public Shared Function UnicodeToString(strCode As String) As String
		UnicodeToString = strCode
		If InStr(UnicodeToString, "\u") <= 0 Then
			Exit Function
		End If
		strCode = LCase(strCode)
		Dim mc As MatchCollection
		mc = Regex.Matches(strCode, "\\u(\S{1,4})")
		For Each m As Match In mc
			strCode = Replace(strCode, m.ToString, ChrW("&H" & m.Groups(1).Value))
		Next
		UnicodeToString = strCode
	End Function

	Function ShowAchievements(Title As String) As String
		ShowAchievements = Title
		ShowAchievements &= vbCrLf & Achievements.ToString(CurrentLang)
		Return ShowAchievements
	End Function

	Public AnimationSpeed = 0.02

	'Private EaseType As Animation.Ease.EaseType

	Public CloseWindow As Boolean = False

	Public PanelHeightTick As Double = 1
	Public PanelHeightPreData As Double
	Public PanelHeightData As Double

	Public SizeChangeTick As Double = 1
	Public SizeChangePreData As SizeF
	Public SizeChangeData As SizeF

	Public RecPanelChangeTick As Double = 1
	Public RecPanelChangePreData As RectangleF
	Public RecPanelChangeData As RectangleF

	Public Tick As Double


	Sub CallTicking(startData As Int16, endData As Int16)
		PanelHeightPreData = startData
		PanelHeightData = endData
		PanelHeightTick = 0
	End Sub
	Sub CallTicking(startData As SizeF, endData As SizeF)
		SizeChangePreData = startData
		SizeChangeData = endData
		SizeChangeTick = 0
	End Sub
	Sub CallTicking(startData As RectangleF, endData As RectangleF)
		RecPanelChangePreData = startData
		RecPanelChangeData = endData
		RecPanelChangeTick = 0
	End Sub
	Function Ticking(sender As Object, e As EventArgs)

		'If FormSizeEaseTick < 1 Then
		'	FormSizeEaseTick += AnimationSpeed

		'End If


		If SizeChangeTick < 1 Then
			SizeChangeTick += AnimationSpeed
			ChangeData(Me.Width, SizeChangeTick, SizeChangePreData.Width, SizeChangeData.Width)
			ChangeData(Me.Height, SizeChangeTick, SizeChangePreData.Height, SizeChangeData.Height)
		ElseIf CloseWindow Then
			End
		End If

		If PanelHeightTick < 1 Then
			PanelHeightTick += AnimationSpeed
			ChangeData(Panel1.Height, PanelHeightTick, PanelHeightPreData, PanelHeightData)
		End If

		If RecPanelChangeTick < 1 Then
			RecPanelChangeTick += AnimationSpeed
			ChangeData(Panel2.Width, RecPanelChangeTick, RecPanelChangePreData.Width, RecPanelChangeData.Width)
			ChangeData(Panel2.Height, RecPanelChangeTick, RecPanelChangePreData.Height, RecPanelChangeData.Height)
			ChangeData(Panel2.Left, RecPanelChangeTick, RecPanelChangePreData.Left, RecPanelChangeData.Left)
			ChangeData(Panel2.Top, RecPanelChangeTick, RecPanelChangePreData.Top, RecPanelChangeData.Top)
		End If


		Dim P As Double = Math.Round(ramCounter.NextValue \ 104857.6) / 10
		Dim ShowP As String = IIf(P > 1024, Math.Round(P \ 102.4) / 10 & "GB", P & "MB")

		If Progress = 1 Then
			Label1.Text = $"{LevelName}{vbCrLf}{ShowP}"
		End If

		Return Nothing
	End Function

	Shared Sub ChangeData(ByRef Data As Object, ByVal x As Double, ByVal startData As Int16, ByVal endData As Int16)
		Data = startData + (endData - startData) * Ease(x)
	End Sub
End Class
