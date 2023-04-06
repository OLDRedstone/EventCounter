Imports System.Diagnostics.CodeAnalysis
Imports EventCount.PublicLib
Imports System.Text.RegularExpressions
Imports System.IO

Public Class Assets

	Public Shared ReadOnly Property IconSet As New Rectangle(
		New Point(48, 34),
		New Point(56, 28))
	Public Shared ReadOnly Property Row As UInt16 = 4
	Private Shared ReadOnly Property ImgLib As New Matrix(Of String, SelectStatus, Bitmap)
	Public Sub New()
		Dim Keys$ = New IO.StreamReader(GetFile($"Info\Keys")).ReadToEnd
		Dim A = Regex.Matches(Keys, "(?m)^\S+")
		For Each M As Match In A
			For i As SelectStatus = 0 To 3
				ImgLib.Add(
					M.Value,
					i,
					New Bitmap(GetFile($"Icons\{M.Value}{i}.png"))
				)
			Next
		Next
	End Sub
	Public Shared ReadOnly Property GetImg(Name As String, Status As SelectStatus) As Bitmap
		Get
			Return ImgLib(Name, Status)
		End Get
	End Property
	Public Shared Sub PlaySnd(Name As SoundType)
		Select Case Name
			Case SoundType.Open
				My.Computer.Audio.Play(GetFile($"Sounds\Open{ New Random().Next(3)}.wav"))
			Case SoundType.Close
				My.Computer.Audio.Play(GetFile($"Sounds\Close{ New Random().Next(2)}.wav"))
			Case Else
				My.Computer.Audio.Play(GetFile($"Sounds\{Name}.wav"))
		End Select
	End Sub
End Class

Public Class Achievements
	Public Shared ReadOnly Property AchiFile As String = Environ("TEMP") ' "C:\ProgramData\EventCounter"
	Public Shared AchiDic As New Matrix(Of String, LangT, String)
	Public Shared Sub Read()
		If My.Computer.FileSystem.DirectoryExists(AchiFile) And
			My.Computer.FileSystem.FileExists($"{AchiFile}\Achievements") Then
			Dim a = New StreamReader($"{AchiFile}\Achievements")
			Dim b = a.ReadToEnd
			a.Close()
			File.SetAttributes($"{AchiFile}", FileAttributes.Normal)
			File.Delete($"{AchiFile}\Achievements")
			For Each I As Match In Regex.Matches(b, "^(?<id>.+?), *(?<zh_cn>.+?), *(?<en_us>.+?)$")
				AchiDic.Add(I.Groups("id").Value, LangT.zh_cn, I.Groups(LangT.zh_cn.ToString).Value)
				AchiDic.Add(I.Groups("id").Value, LangT.en_us, I.Groups(LangT.en_us.ToString).Value)
			Next
		End If
	End Sub
	Public Shared Sub Add(ID As String, ParamArray Lang As String())
		Dim l As LangT = 0
		For Each item In Lang
			AchiDic.Add(ID, l, item)
			l += 1
		Next
	End Sub
	Public Shared Sub Remove(ID As String)
		AchiDic.Remove(ID)
	End Sub
	Public Shared Sub Write()
		Dim b = New StreamWriter($"{AchiFile}\Achievements")
		For Each I In AchiDic.ToDictionary
			b.WriteLine($"{I.Key}, {I.Value(LangT.zh_cn)}, {I.Value(LangT.en_us)}")
		Next
		b.Close()
	End Sub
	Public Overloads Shared Function ToString(lang As LangT) As String
		Dim MergeList As New List(Of String)
		For Each Dic In AchiDic.ToDictionary
			If Dic.Value.ContainsKey(lang) Then
				MergeList.Add($"{Dic.Value(lang)}")
			End If
		Next
		Return String.Join(vbCrLf, MergeList)
	End Function
End Class