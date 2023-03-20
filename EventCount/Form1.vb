Imports System.Drawing.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Threading

Public Class Form1

    ReadOnly SoundEvents() As String = My.Resources.Files.Sound.Split(vbCrLf)
    ReadOnly RailEvents() As String = My.Resources.Files.Rail.Split(vbCrLf)
    ReadOnly MotionEvents() As String = My.Resources.Files.Motion.Split(vbCrLf)
    ReadOnly SpriteEvents() As String = My.Resources.Files.Sprite.Split(vbCrLf)
    ReadOnly RoomEvents() As String = My.Resources.Files.Room.Split(vbCrLf)

    Dim SoundEventCount(SoundEvents.Length) As UInt64
    Dim RailEventCount(RailEvents.Length) As UInt64
    Dim MotionEventCount(MotionEvents.Length) As UInt64
    Dim SpriteEventCount(SpriteEvents.Length) As UInt64
    Dim RoomEventCount(RoomEvents.Length) As UInt64

    Dim FilePath As String = ""
    Dim RdlevelFile As String = ""
    Dim temp As String = "\%RDLevelUnzip%"

    ReadOnly MotionEventsImage As Image = My.Resources.Files.events
    Dim RDFont As Font

    Sub ChangeFont()
        Dim fontData As Byte() = My.Resources.Files.RDLatinFontPoint
        Dim fontCollection As New PrivateFontCollection '创建字体集合
        fontCollection.AddMemoryFont(Marshal.UnsafeAddrOfPinnedArrayElement(fontData, 0), fontData.Length) '将字体字节数组添加到字体集合中
        RDFont = New Font(fontCollection.Families(0), 24)
    End Sub

    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If OpenLevel.ShowDialog() <> DialogResult.OK Then
            End
        End If
        CountingEvents()
        ChangeFont()
        ShowEventsCount()
    End Sub
    Sub ShowEventsCount() Handles Me.Click

        Me.CreateGraphics.DrawImage(MotionEventsImage,
                                    New Rectangle(New Point(15, 15), New Point(28, 28)))

        For i = 0 To SoundEvents.Length - 1
            Me.CreateGraphics.DrawString($"{SoundEventCount(i)}", RDFont, New SolidBrush(Color.Red),
                                         New PointF(0, i * 22))
        Next
        For i = 0 To RailEvents.Length - 1
            Me.CreateGraphics.DrawString($"{RailEventCount(i)}", RDFont, New SolidBrush(Color.Cyan),
                                         New PointF(40, i * 22))
        Next
        For i = 0 To MotionEvents.Length - 1
            Me.CreateGraphics.DrawString($"{MotionEventCount(i)}", RDFont, New SolidBrush(Color.Purple),
                                         New PointF(80, i * 22))
        Next
        For i = 0 To SpriteEvents.Length - 1
            Me.CreateGraphics.DrawString($"{SpriteEventCount(i)}", RDFont, New SolidBrush(Color.Green),
                                         New PointF(120, i * 22))
        Next
        For i = 0 To RoomEvents.Length - 1
            Me.CreateGraphics.DrawString($"{RoomEventCount(i)}", RDFont, New SolidBrush(Color.Yellow),
                                         New PointF(160, i * 22))
        Next

    End Sub
    ''' <summary>
    ''' 为数组赋值
    ''' </summary>
    Sub CountingEvents()
        For i = 0 To SoundEvents.Length - 1
            SoundEventCount(i) = CountingTexts(RdlevelFile, $"""type"": ""{SoundEvents(i)}""")
        Next
        For i = 0 To RailEvents.Length - 1
            RailEventCount(i) = CountingTexts(RdlevelFile, $"""type"": ""{RailEvents(i)}""")
        Next
        For i = 0 To MotionEvents.Length - 1
            MotionEventCount(i) = CountingTexts(RdlevelFile, $"""type"": ""{MotionEvents(i)}""")
        Next
        For i = 0 To SpriteEvents.Length - 1
            SpriteEventCount(i) = CountingTexts(RdlevelFile, $"""type"": ""{SpriteEvents(i)}""")
        Next
        For i = 0 To RoomEvents.Length - 1
            RoomEventCount(i) = CountingTexts(RdlevelFile, $"""type"": ""{RoomEvents(i)}""")
        Next
    End Sub
    ''' <summary>
    ''' 删除缓存
    ''' </summary>
    ''' <returns></returns>
    Public Sub KillTemp()
        If My.Computer.FileSystem.DirectoryExists(Environ("temp") & temp) Then '删除原输出
            My.Computer.FileSystem.DeleteDirectory(Environ("temp") & temp,
            FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
    End Sub
    Private Sub OpenLevel_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenLevel.FileOk
        If IO.Path.GetExtension(OpenLevel.FileName) = ".rdlevel" Then
        ElseIf IO.Path.GetExtension(OpenLevel.FileName) = ".rdzip" Then
            KillTemp()
            IO.Compression.ZipFile.ExtractToDirectory(OpenLevel.FileName, Environ("temp") & temp)
            Dim s As String() = IO.Directory.GetFiles(Environ("temp") & temp, "*.rdlevel")
            OpenLevel.FileName = Environ("temp") & temp & "\" & IO.Path.GetFileName(s(0))
        Else MsgBox("这好像不是一个节奏医生关卡文件？")
            End
        End If
        FilePath = OpenLevel.FileName
        RdlevelFile = System.IO.File.OpenText(FilePath).ReadToEnd

    End Sub
    ''' <summary>
    ''' 统计关键词数量
    ''' </summary>
    ''' <param name="source">被查找对象</param>
    ''' <param name="key">查找关键词</param>
    ''' <returns></returns>
    Function CountingTexts(source As String, key As String) As UInt64
        Dim Count As UInt64 = 0
        Dim Text As String = source
        Dim T = InStr(Text, key)
        While T
            Text = Mid(Text, T + 1)
            T = InStr(Text, key)
            Count += 1
        End While
        Return Count
    End Function

End Class
