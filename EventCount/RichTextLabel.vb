Imports EventCount.PublicLib
Public Class RichTextLabel
	Public Property ItemHeight As UShort = 24
	Public Property NumberWidth As UShort = 24
	Private _Title As String
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
	Private _ShowText As Boolean = True
	Public Property ShowText As Boolean
		Get
			Return _ShowText
		End Get
		Set(value As Boolean)
			_ShowText = value
			RefreshUI()
		End Set
	End Property
	Private _EventList As New Dictionary(Of String, ULong)
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

	End Sub
	Public Sub RefreshUI()

		Dim Img As New Bitmap(Width, (_EventList.Count + 1) * ItemHeight)
		Dim G As Graphics = Graphics.FromImage(Img)
		If _Title = "" Or _Title = Nothing Then
			G.DrawString(Count, Font, New SolidBrush(Color.White), New PointF)
		Else
			If _ShowText Then
				G.DrawString($"{GetCount(_Title)}*{GetLang(_Title, CurrentLang)}", Font, New SolidBrush(Color.White), New PointF)
			Else
				G.DrawString($"{GetCount(_Title)}* ???", Font, New SolidBrush(Color.White), New PointF)
			End If
		End If

		Dim MaxValue As ULong
		If _EventList.Count Then
			MaxValue = _EventList.Max(Function(j) j.Value)
		End If

		NumberWidth = MaxValue.ToString.Length * 9 + 12

		Dim I = 1
		For Each item In _EventList
			Dim CBack, CFore As Color
			Select Case PublicLib.GetType(item.Key)
				Case EventT.Sounds
					CBack = Color.FromArgb(217, 36, 51)
					CFore = Color.FromArgb(61, 11, 15)
				Case EventT.Rows
					CBack = Color.FromArgb(44, 79, 219)
					CFore = Color.FromArgb(17, 30, 81)
				Case EventT.Actions
					CBack = Color.FromArgb(197, 68, 179)
					CFore = Color.FromArgb(83, 29, 76)
				Case EventT.Decorations
					CBack = Color.FromArgb(0, 196, 89)
					CFore = Color.FromArgb(29, 83, 63)
				Case EventT.Rooms
					CBack = Color.FromArgb(216, 184, 17)
					CFore = Color.FromArgb(81, 69, 6)
			End Select
			G.FillRectangle(New SolidBrush(CBack),
							0, I * ItemHeight,
							Width, ItemHeight)
			G.FillRectangle(New SolidBrush(Color.FromArgb(127, CFore)),
							NumberWidth, I * ItemHeight + ItemHeight \ 2,
							(Width - NumberWidth) * item.Value \ MaxValue, ItemHeight \ 2)
			G.DrawString($"{item.Value}", Font, New SolidBrush(CFore), New PointF(0, I * ItemHeight))
			If _ShowText Then
				G.DrawString($"{GetLang(item.Key, LangT.zh_cn)}", Font, New SolidBrush(CFore), New PointF(NumberWidth, I * ItemHeight))
			Else
				G.DrawString($"???", Font, New SolidBrush(CFore), New PointF(NumberWidth, I * ItemHeight))
			End If
			I += 1
		Next
		Height = I * ItemHeight
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
