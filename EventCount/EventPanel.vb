Imports EventCount.PublicLib
Public Class EventPanel
	Private Shared Unit As Integer = Assets.IconSet.Height
	Private G As Graphics
	Private ET As EventT
	Public Event ItemClick(EventName As String)
	Public Event ItemChoosenChangedClick(EventName As String)
	Public Event ItemChoosenActivatedClick(EventName As String)
	Public Event ItemChoosenInactivatedClick(EventName As String)
	Public Event ItemCleared(ET As EventT)
	Public Event ItemReversed(NewChosenList As Dictionary(Of String, Boolean))
	Public Event HoverItemChanged(EventName As String)
	Private Property EventCount As New Dictionary(Of String, ULong)
	Public Property EventChoosen As New Dictionary(Of String, Boolean)
	Public ReadOnly Property HoverItem As String
		Get
			Dim P = PointToClient(MousePosition)
			If Me.ClientRectangle.Contains(P) Then
				If New Bitmap(BackgroundImage).GetPixel(P.X, P.Y).A Then
					Return PointToName(P)
				End If
			End If
			Return ""
		End Get
	End Property
	Private PreviousHoverItem As String = ""
	Public Sub New()
		InitializeComponent()
		Width = 8 * Unit
		Height = Assets.Row * Unit
		BackgroundImage = New Bitmap(Me.Width, Me.Height)
	End Sub
	Public Sub SetList(ET As EventT, List As Dictionary(Of String, EventProperty), ChosenList As List(Of String))
		EventCount = List.ToDictionary(Function(i) i.Key, Function(i) i.Value.Count)
		EventChoosen = List.ToDictionary(Function(i) i.Key, Function(i) ChosenList.Contains(i.Key))
		Me.ET = ET
		RefreshUI()
	End Sub

	Private Sub PanelLoad(sender As Object, e As EventArgs) Handles MyBase.Load
		Me.BackColor = Color.Transparent
	End Sub
	Public Sub RefreshUI() Handles Me.HoverItemChanged
		Dim BGImage As New Bitmap(Width, Height)
		G = Graphics.FromImage(BGImage)
		Dim i As UShort = 0
		For Each pairs In EventCount
			Dim Status As SelectStatus
			If pairs.Value Then
				If pairs.Key = PreviousHoverItem Or EventChoosen(pairs.Key) Then
					Status = SelectStatus.Selected
				Else
					Status = SelectStatus.None
				End If
			Else
				If pairs.Key = HoverItem Then
					Status = SelectStatus.InactiveSelected
				Else
					Status = SelectStatus.Inactive
				End If
			End If
			Dim Img = Assets.GetImg(pairs.Key, Status)
			G.DrawImage(Img,
				(i \ Assets.Row) * Unit,
				(i Mod Assets.Row) * Unit,
				Img.Width, Img.Height
			)
			Select Case ET
				Case EventT.Sounds
					If i = 3 Then
						i += 1
					End If
				Case EventT.Rows
					If i = 3 Then
						i += 2
					End If
				Case EventT.Actions
				Case EventT.Decorations
				Case EventT.Rooms
			End Select
			i += 1
		Next
		BackgroundImage = BGImage
	End Sub
	Private Sub HoverLeave() Handles MyBase.MouseLeave
		PreviousHoverItem = ""
		RaiseEvent HoverItemChanged("")
	End Sub
	Private Sub HoverMove() Handles MyBase.MouseMove
		If HoverItem <> PreviousHoverItem Then
			PreviousHoverItem = HoverItem
			RaiseEvent HoverItemChanged(PreviousHoverItem)
		End If
	End Sub
	Private Function PointToName(P As Point) As String
		Dim I = (P.X \ Unit) * Assets.Row + (P.Y \ Unit)
		Dim Dic = EventCount.ToList
		Select Case ET
			Case EventT.Sounds
				If I = 4 Then
					I = 0
				End If
				If I > 4 Then
					I -= 1
				End If
			Case EventT.Rows
				If I = 4 Or I = 5 Or I = 11 Then
					I -= 4
				End If
				If I > 5 Then
					I -= 2
				End If
			Case EventT.Actions
			Case EventT.Decorations
			Case EventT.Rooms
		End Select
		If 0 <= I And I < Dic.Count Then
			Return Dic(I).Key
		End If
		Return ""
	End Function
	Public Sub Reverse()
		'For Each item In EventChoosen
		'	EventChoosen(item.Key) = Not EventChoosen(item.Key)
		'	ReloadPanel()
		'Next
		EventChoosen = EventChoosen.ToDictionary(Function(i) i.Key, Function(i) (Not i.Value) And EventCount(i.Key) > 0)
		RaiseEvent ItemReversed(EventChoosen)
	End Sub
	Public Sub Clear()
		For Each item In EventChoosen
			EventChoosen(item.Key) = False
			RefreshUI()
			RaiseEvent ItemCleared(ET)
		Next
	End Sub
	Private Sub PanelMouseUp() Handles MyBase.MouseUp
		Dim P = PointToClient(MousePosition)
		Dim Key = PointToName(P)
		If Key <> "" AndAlso EventCount(Key) Then
			Dim value = EventChoosen(Key)
			If value Then
				RaiseEvent ItemChoosenInactivatedClick(Key)
			Else
				RaiseEvent ItemChoosenActivatedClick(Key)
			End If
			EventChoosen(Key) = Not value
			RaiseEvent ItemChoosenChangedClick(Key)
		End If
		RaiseEvent ItemClick(Key)
	End Sub
End Class