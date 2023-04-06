Imports System.Text.RegularExpressions
Imports EventCount.PublicLib
Public Class Counter
	Public Property SongName$
	Public Function Load(rdlevel As String)
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
		Next
		GC.Collect()
		Return Nothing
	End Function
	Public Sub New()
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
End Class
