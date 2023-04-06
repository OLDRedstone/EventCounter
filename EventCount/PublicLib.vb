
Imports System.Text.RegularExpressions

Public Class PublicLib
	Public Shared IncludedObjects As New Dictionary(Of String, EventProperty)
	Public Shared ReadOnly Property AssetPath = $"{Application.StartupPath}Assets\"
	Public Shared ReadOnly Property CurrentLang As LangT = LangT.zh_cn

	Public Class EventProperty
		Public Property Type As EventT
		Public Property Lan As List(Of String)
		Public Property Count As ULong
		Public Sub New(T As EventT, Lan$)
			Me.Type = T
			Me.Lan = New List(Of String)
			For Each Item As Match In New Regex("[\S ]+").Matches(Lan)
				Me.Lan.Add(Item.Value)
			Next
		End Sub
		Public Overrides Function ToString() As String
			Return $"{Count}*{Lan(0)}, {Type}"
		End Function
	End Class

	Public Enum EventT
		Sounds
		Rows
		Actions
		Decorations
		Rooms
	End Enum
	Public Enum SelectStatus
		None
		Selected
		Inactive
		InactiveSelected
	End Enum
	Public Enum SoundType
		Sounds
		Rows
		Actions
		Decorations
		Rooms
		[On]
		Off
		Open
		Close
	End Enum
	Public Enum LangT
		zh_cn
		en_us
	End Enum
	Public Shared Function GetFile(FileName As String) As String
		If My.Computer.FileSystem.FileExists($"{AssetPath}{FileName}") Then
			Return $"{AssetPath}{FileName}"
		Else
			Achievements.Add("NothingHere", "你是不是偷吃！", "Did you eat them?")
			End
		End If
	End Function
	Public Sub New()
		IncludedObjects = New Dictionary(Of String, EventProperty)
		Dim KeyString$ = IO.File.OpenText(GetFile("Info\Keys")).ReadToEnd
		Dim M As MatchCollection = New Regex("(?<name>[\S ]+)\t(?<type>[\S ]+)\t(?<lan>.+)").Matches(KeyString)
		For Each N As Match In M
			Dim T As EventT
			Select Case N.Groups("type").Value
				Case "Sound"
					T = EventT.Sounds
				Case "Rail"
					T = EventT.Rows
				Case "Motion"
					T = EventT.Actions
				Case "Sprite"
					T = EventT.Decorations
				Case "Room"
					T = EventT.Rooms
			End Select
			Dim ET As New EventProperty(T, N.Groups("lan").Value)
			IncludedObjects.Add(N.Groups("name").Value, ET)
		Next
	End Sub
	Public Shared Function ContainsKey(Key$) As Boolean
		Return IncludedObjects.ContainsKey(Key)
	End Function
	Public Shared Function GetCount(Name As String) As ULong
		Return IncludedObjects(Name).Count
	End Function
	Public Shared Function GetCount(Type As EventT) As List(Of ULong)
		Return (From item In IncludedObjects Where item.Value.Type = Type Select item.Value.Count).ToList
	End Function
	Public Overloads Shared Function [GetType](Name As String) As EventT
		Return IncludedObjects(Name).Type
	End Function
	Public Shared Function GetLang(Name As String, Index As LangT) As String
		If IncludedObjects.ContainsKey(Name) Then
			Return IncludedObjects(Name).Lan(Index)
		End If
		Return ""
	End Function
End Class

Public Class Matrix(Of Key1, Key2, T)
	Private ReadOnly _Dic1 As New Dictionary(Of Key1, Dictionary(Of Key2, T))
	Default Public Overloads Property Item(I1 As Key1, I2 As Key2) As T
		Get
			If _Dic1.ContainsKey(I1) Then
				If _Dic1(I1).ContainsKey(I2) Then
					Return _Dic1(I1)(I2)
				End If
			End If
			Throw New Exception("Not exist")
		End Get
		Set(value As T)
			_Dic1(I1)(I2) = value
		End Set
	End Property
	Public Overloads ReadOnly Property Count() As UInteger
		Get
			Dim Sum As UInteger
			For Each Dic In _Dic1
				Sum += Dic.Value.Count
			Next
			Return Sum
		End Get
	End Property
	Public Function ContainsKey1(Key As Key1) As Boolean
		Return _Dic1.ContainsKey(Key)
	End Function
	Public Function ContainsKey2(Key As Key2) As Boolean
		For Each Dic In _Dic1
			If Dic.Value.ContainsKey(Key) Then
				Return True
			End If
		Next
		Return False
	End Function
	Public Overloads Function ContainsValue(Value As T) As Boolean
		For Each Dic In _Dic1
			If Dic.Value.ContainsValue(Value) Then
				Return True
			End If
		Next
		Return False
	End Function
	Public Overloads Sub Add(Key1 As Key1, Key2 As Key2, Value As T)
		_Dic1.TryAdd(Key1, New Dictionary(Of Key2, T))
		_Dic1(Key1).TryAdd(Key2, Value)
	End Sub
	Public Overloads Function Remove(Key1 As Key1, Key2 As Key2) As Boolean
		If _Dic1.ContainsKey(Key1) Then
			Return _Dic1(Key1).Remove(Key2)
		End If
		For Each Dic In _Dic1
			If Dic.Value.Count = 0 Then
				_Dic1.Remove(Dic.Key)
			End If
		Next
		Return False
	End Function
	Public Overloads Function Remove(Key1 As Key1) As Boolean
		Return _Dic1.Remove(Key1)
	End Function
	Public Overloads Function Remove(Key2 As Key2) As Boolean
		Dim Success As Boolean = False
		For Each Dic In _Dic1.Values
			If Dic.ContainsKey(Key2) Then
				Success = Success Or Dic.Remove(Key2)
			End If
		Next
		Return Success
	End Function
	Public Function ToDictionary() As Dictionary(Of Key1, Dictionary(Of Key2, T))
		Return _Dic1
	End Function
End Class