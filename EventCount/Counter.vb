Imports System.Text.RegularExpressions
Imports System.IO.File
Imports EventCount.PublicLib
Public Class Counter
	Public Property SongName$
	Public Property Length As ULong
	Public Property Progress As ULong
	Public dataSender As New Timer
	Public Event ProgressChanged(Progress As Double)
	Public Function Load(rdlevel As String)
		Length = rdlevel.Length
		For Each Item In IncludedObjects
			Item.Value.Count = 0
		Next
		Me.SongName = New Regex("[^\\]""song"": ""(?<t>.+?)""").Match(rdlevel).Groups("t").Value
		Dim M As MatchCollection = New Regex("[^\\]""type"": ""(?<t>.+?)""").Matches(rdlevel)
		For Each N As Match In M
			Dim K = N.Groups("t").Value
			If IncludedObjects.ContainsKey(K) Then
				IncludedObjects(K).Count += 1
			End If
			Progress = N.Index
			RaiseEvent ProgressChanged(Progress)
		Next
		GC.Collect()
		Return Nothing
	End Function
	Public Function Load(rdlevel As IO.StreamReader)
		Length = rdlevel.BaseStream.Length

		For Each Item In IncludedObjects
			Item.Value.Count = 0
		Next

		Dim s = ReadLine(rdlevel)

		Dim M = Regex.Match(s, """song"": ?""(?<t>[^""]+)")
		Dim N = Regex.Match(s, """type"":\s?""(?<t>.+?)""")
		While Not rdlevel.EndOfStream
			s = ReadLine(rdlevel)
			If N.Success Then
				Dim K = N.Groups("t").Value
				If IncludedObjects.ContainsKey(K) Then
					IncludedObjects(K).Count += 1
				End If
			End If
			If M.Success Then
				Me.SongName = M.Groups("t").Value
			Else
				M = Regex.Match(s, """song"":\s?""(?<t>[^""]+)")
			End If
			N = Regex.Match(s, """type"":\s?""(?<t>.+?)""")
			Progress = rdlevel.BaseStream.Position
		End While
		dataSender.Enabled = False
		RaiseEvent ProgressChanged(1)
		GC.Collect()
		Return Nothing
	End Function
	Private Function ReadLine(Stream As IO.StreamReader) As String
		Dim c(0) As Char
		Dim s As String = ""
		Stream.ReadBlock(c)
		While Not c.Contains("}"c) And Not Stream.EndOfStream
			s += c
			Stream.ReadBlock(c)
		End While
		'Debug.Print(s)
		Return s
	End Function
	Public Sub New()
		AddHandler dataSender.Tick, AddressOf SendData
		dataSender.Interval = 200
	End Sub
	Public Shared Function GetList(T As EventT) As Dictionary(Of String, EventProperty)
		Dim D As New Dictionary(Of String, EventProperty)
		For Each item In IncludedObjects
			If item.Value.Type = T Then
				D.Add(item.Key, item.Value)
			End If
		Next
		'Return IncludedObjects.GroupBy(Function(i) i.Value.Type = T)
		Return D
	End Function
	Public Shared Function GetList() As Dictionary(Of String, EventProperty)
		Return IncludedObjects
	End Function
	Public Sub SendData()
		RaiseEvent ProgressChanged(Progress / (Length + 1))
	End Sub
End Class
