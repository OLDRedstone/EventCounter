Imports EventCount.PublicLib
Public Class RichTextLabel
	Private _Title As String
	Private _EventList As New Dictionary(Of String, ULong)
	Private _ShowText As Boolean = True
	Private Hovering As Boolean = False
	Private _numSize As SizeF
	Private _numTop As UShort
	Private itemPageCount As UShort = 12
	Private page As UShort = 0
	Private showStartIndex As UShort = 0
	Public Event ItemClick(itemName As String)
	Public ReadOnly Property Count As ULong
		Get
			Return _EventList.Sum(Function(i) i.Value)
		End Get
	End Property
	Public Property Title As String
		Get
			Return _Title
		End Get
		Set(value As String)
			_Title = value
			RefreshUI()
		End Set
	End Property
	Public Property ShowText As Boolean
		Get
			Return _ShowText
		End Get
		Set(value As Boolean)
			_ShowText = value
			RefreshUI()
		End Set
	End Property
	Public WriteOnly Property EventList As Dictionary(Of String, ULong)
		Set(value As Dictionary(Of String, ULong))
			_EventList = value.OrderBy(Function(i) PublicLib.GetType(i.Key)).ToDictionary(Function(i) i.Key, Function(i) i.Value)
			RefreshUI()
		End Set
	End Property

	Private Sub RichTextLabel_Load(sender As Object, e As EventArgs) Handles MyBase.Load
	End Sub
	Public Sub New()
		InitializeComponent()
		BackgroundImage = New Bitmap(Width, Height)
		AddHandler MouseClick, AddressOf ItemClicked
		AddHandler MouseWheel, AddressOf MouseScroll
		AddHandler MouseEnter, AddressOf MEnter
		AddHandler MouseLeave, AddressOf MLeave
	End Sub

	Public Sub ItemClicked()
		Dim index = PointToClient(MousePosition).Y \ _numTop - 1
		If index < 0 Then
			RaiseEvent ItemClick("")
		Else
			RaiseEvent ItemClick(_EventList.ToList()(showStartIndex + index).Key)
		End If
	End Sub
	Public Sub MLeave()
		Hovering = False
		RefreshUI()
	End Sub
	Public Sub MEnter()
		Hovering = True
		RefreshUI()
	End Sub
	Public Sub MouseScroll(sender As Object, arg As MouseEventArgs)
		Dim temp = showStartIndex
		If temp > 0 And arg.Delta > 0 Then
			temp -= 1
		ElseIf temp < _EventList.Count - itemPageCount And arg.Delta < 0 Then
			temp += 1
		End If
		If temp <> showStartIndex Then
			showStartIndex = temp
			RefreshUI()
		End If
	End Sub
	Public Function toImage(showFullInfo As Boolean, StartIndex As UShort, ShowCount As UShort) As Bitmap

		Dim Img As New Bitmap(1, 1)
		Dim G As Graphics = Graphics.FromImage(Img)
		Dim tempList As Dictionary(Of String, ULong) = _EventList
		Dim tempNumSize As SizeF
		Dim tempNumTop As UShort
		StartIndex = Math.Min(StartIndex, _EventList.Count)
		ShowCount = Math.Min(ShowCount, _EventList.Count - StartIndex)

		If tempList.Count = 0 Then
			tempNumSize = G.MeasureString(0, Font)
		Else
			tempNumSize = G.MeasureString(tempList.Max(Function(j) j.Value), Font)
		End If
		tempNumTop = tempNumSize.Height
		_numTop = tempNumTop


		Img = New Bitmap(Width, (ShowCount + 1) * CInt(tempNumSize.Height))
		G = Graphics.FromImage(Img)

		If _Title = "" Or _Title = Nothing Then
			G.DrawString(Count, Font, New SolidBrush(Color.White), New PointF)
		Else
			If showFullInfo Then
				G.DrawString($"{GetCount(_Title)}*{GetLang(_Title, CurrentLang)}", Font, New SolidBrush(Color.White), New PointF)
			Else
				G.DrawString($"{GetCount(_Title)}* ???", Font, New SolidBrush(Color.White), New PointF)
			End If
		End If

		Dim MaxValue As ULong
		If tempList.Count Then
			MaxValue = tempList.Max(Function(j) j.Value)
		End If

		'隐藏时排序
		If Not showFullInfo Then
			tempList = tempList.OrderBy(Function(i) -i.Value).ToDictionary(Function(i) i.Key, Function(i) i.Value)
		End If

		'Dim Range = (
		'	Math.Min(page * itemPageCount, _EventList.Count),
		'	Math.Min((page + 1) * itemPageCount - 1, _EventList.Count)
		'	)
		Dim CBack As Color
		For Each item In tempList.ToList.GetRange(startIndex, showCount).ToDictionary(Function(i) i.Key, Function(i) i.Value)
			If Not showFullInfo Then
				CBack = Color.Gray
			Else
				Select Case PublicLib.GetType(item.Key)
					Case EventT.Sounds
						CBack = Color.FromArgb(217, 36, 51)
					Case EventT.Rows
						CBack = Color.FromArgb(44, 79, 219)
					Case EventT.Actions
						CBack = Color.FromArgb(197, 68, 179)
					Case EventT.Decorations
						CBack = Color.FromArgb(0, 196, 89)
					Case EventT.Rooms
						CBack = Color.FromArgb(216, 184, 17)
				End Select
			End If
			'背景条
			G.FillRectangle(New SolidBrush(CBack),
							0, tempNumSize.Height,
							Width, tempNumSize.Height)
			'数量条
			If MaxValue = 0 Then
				G.FillRectangle(New SolidBrush(Color.FromArgb(95, Color.Black)),
								Width - (Width * item.Value) \ MaxValue, tempNumSize.Height + tempNumTop \ 2,
								Width, tempNumTop \ 2)
			Else
				G.FillRectangle(New SolidBrush(Color.FromArgb(95, Color.Black)),
								Width - (Width * item.Value) \ MaxValue, tempNumSize.Height + tempNumTop \ 2,
								Width * item.Value \ MaxValue, tempNumTop \ 2)
			End If
			'分隔条
			G.FillRectangle(New SolidBrush(Color.FromArgb(127, Color.Black)),
							0, tempNumSize.Height - tempNumTop \ 8,
							Width, tempNumTop \ 8)
			'数量
			G.DrawString($"{item.Value}", Font, New SolidBrush(Color.FromArgb(192, Color.Black)), New PointF(0, tempNumSize.Height))
			'名称
			If showFullInfo Then
				G.DrawString($"{GetLang(item.Key, LangT.zh_cn)}", Font, New SolidBrush(Color.FromArgb(192, Color.Black)), New PointF(tempNumSize.Width, tempNumSize.Height))
			Else
				G.DrawString($"???", Font, New SolidBrush(Color.FromArgb(192, Color.Black)), New PointF(tempNumSize.Width, tempNumSize.Height))
			End If
			tempNumSize.Height += tempNumTop
		Next
		'滚动条
		If Hovering Then
			G.FillRectangle(New SolidBrush(Color.FromArgb(127, Color.Black)),
								Width - tempNumTop \ 4, tempNumTop + (Height - tempNumTop) * showStartIndex \ Math.Max(_EventList.Count, 1),
								tempNumTop \ 4, (Height - tempNumTop) * showCount \ Math.Max(_EventList.Count, 1))
		End If
		_numSize = tempNumSize
		Return Img
	End Function
	Public Sub RefreshUI()

		Dim Img = toImage(ShowText, showStartIndex, itemPageCount)
		Height = Img.Height ' _numSize.Height
		BackgroundImage = Img
	End Sub
	Public Sub Add(E As String, Count As ULong)
		_EventList.Add(E, Count)
		RefreshUI()
	End Sub
	Public Sub Remove(E As String)
		_EventList.Remove(E)
		RefreshUI()
	End Sub
End Class
