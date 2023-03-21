Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Runtime.InteropServices
#Disable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型

Public Class Form1

    ReadOnly SoundEvents() As String = My.Resources.ResourceData.Sound.Split(vbCrLf)
    ReadOnly RailEvents() As String = My.Resources.ResourceData.Rail.Split(vbCrLf)
    ReadOnly MotionEvents() As String = My.Resources.ResourceData.Motion.Split(vbCrLf)
    ReadOnly SpriteEvents() As String = My.Resources.ResourceData.Sprite.Split(vbCrLf)
    ReadOnly RoomEvents() As String = My.Resources.ResourceData.Room.Split(vbCrLf)

    Dim SoundEventCount(SoundEvents.Length) As UInt64
    Dim RailEventCount(RailEvents.Length) As UInt64
    Dim MotionEventCount(MotionEvents.Length) As UInt64
    Dim SpriteEventCount(SpriteEvents.Length) As UInt64
    Dim RoomEventCount(RoomEvents.Length) As UInt64

    Dim FilePath As String = ""
    Dim RdlevelFile As String = ""
    Dim temp As String = "\%RDLevelUnzip%"

    Dim RDFont As Font

    ReadOnly IconLeft As UInt16 = 1
    ReadOnly IconSet As Rectangle = New Rectangle(
        New Point(14, 14),
        New Point(56, 28))
    ReadOnly IconMag As UInt16 = 1

    Dim EventTypeIndex As UInt16 = 0

    Dim StartControlCounts As UInt16 = 0

    ReadOnly Raw As UInt16 = 6

    Sub ChangeFont()
        Dim fontData As Byte() = My.Resources.ResourceData.RDLatinFontPoint
        Dim fontCollection As New PrivateFontCollection '创建字体集合
        fontCollection.AddMemoryFont(Marshal.UnsafeAddrOfPinnedArrayElement(fontData, 0), fontData.Length) '将字体字节数组添加到字体集合中
        RDFont = New Font(fontCollection.Families(0), 12)
    End Sub

    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If OpenLevel.ShowDialog() <> DialogResult.OK Then
            End
        End If
        CountingEvents()
        ChangeFont()
        StartControlCounts = Me.Controls.Count
        ShowEventsCount()
    End Sub
    Function ReturnData(Index As UInt16, Type As UInt16)
        Select Case EventTypeIndex
            Case 0
                Select Case Type
                    Case 0
                        Return SoundEvents(Index)
                    Case 1
                        Return SoundEventCount(Index)
                End Select
            Case 1
                Select Case Type
                    Case 0
                        Return RailEvents(Index)
                    Case 1
                        Return RailEventCount(Index)
                End Select
            Case 2
                Select Case Type
                    Case 0
                        Return MotionEvents(Index)
                    Case 1
                        Return MotionEventCount(Index)
                End Select
            Case 3
                Select Case Type
                    Case 0
                        Return SpriteEvents(Index)
                    Case 1
                        Return SpriteEventCount(Index)
                End Select
            Case 4
                Select Case Type
                    Case 0
                        Return RoomEvents(Index)
                    Case 1
                        Return RoomEventCount(Index)
                End Select
        End Select
        Return Nothing
    End Function
    Enum SCType
        None
        Selected
        Inactive
        InactiveSelected
    End Enum
    Enum EType
        Sound
        Rail
        Motion
        Sprite
        Room
    End Enum
    Function ReturnImage(i As EType, SCType As SCType) As Image
        Dim icon As Bitmap = Nothing
        Dim res As My.Resources.ResourceData
        Select Case EventTypeIndex
            Case EType.Sound
                Select Case SCType
                    Case SCType.None
                        icon = res.sounds
                    Case SCType.Inactive
                        icon = res.soundsInactive
                    Case SCType.Selected
                        icon = res.soundsSelected
                    Case SCType.InactiveSelected
                        icon = res.soundsInactiveSelected
                End Select
            Case EType.Rail
                Select Case SCType
                    Case SCType.None
                        icon = res.rails
                    Case SCType.Inactive
                        icon = res.railsInactive
                    Case SCType.Selected
                        icon = res.railsSelected
                    Case SCType.InactiveSelected
                        icon = res.railsInactiveSelected
                End Select
            Case EType.Motion
                Select Case SCType
                    Case SCType.None
                        icon = res.motions
                    Case SCType.Inactive
                        icon = res.motionsInactive
                    Case SCType.Selected
                        icon = res.motionsSelected
                    Case SCType.InactiveSelected
                        icon = res.motionsInactiveSelected
                End Select
            Case EType.Sprite
                Select Case SCType
                    Case SCType.None
                        icon = res.sprites
                    Case SCType.Inactive
                        icon = res.spritesInactive
                    Case SCType.Selected
                        icon = res.spritesSelected
                    Case SCType.InactiveSelected
                        icon = res.spritesInactiveSelected
                End Select
            Case EType.Room
                Select Case SCType
                    Case SCType.None
                        icon = res.rooms
                    Case SCType.Inactive
                        icon = res.roomsInactive
                    Case SCType.Selected
                        icon = res.roomsSelected
                    Case SCType.InactiveSelected
                        icon = res.roomsInactiveSelected
                End Select
        End Select
        Dim EventIconImage As Bitmap
        EventIconImage = icon.Clone(
                New Rectangle(
                    (i Mod (icon.Width / IconSet.Width)) * IconSet.Width,
                    (i \ (icon.Width / IconSet.Width)) * IconSet.Height,
                    IconSet.Width,
                    IconSet.Height
                ), icon.PixelFormat)
        Return EventIconImage
    End Function
    Sub a(sender As PictureBox, e As EventArgs)

        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        Me.Text = $"{Index}, {ReturnData(Index, 0)}={ReturnData(Index, 1)}"
        If ReturnData(Index, 1) > 0 Then
            sender.BackgroundImage = ReturnImage(Index, SCType.Selected)
        Else
            sender.BackgroundImage = ReturnImage(Index, SCType.InactiveSelected)
        End If
    End Sub

    Sub b(sender As PictureBox, e As EventArgs)
        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        If ReturnData(Index, 1) > 0 Then
            sender.BackgroundImage = ReturnImage(Index, SCType.None)
        Else
            sender.BackgroundImage = ReturnImage(Index, SCType.Inactive)
        End If
    End Sub

    Sub CreateEventsIcon(Type As String(), TypeCount As UInt64(), Icon As Bitmap)

        Dim EventIcon(Type.Length) As PictureBox
        Dim j As UInt16 = 0
        For i As UInt16 = 0 To Type.Length - 1
            EventIcon(i) = New PictureBox With {
                .Location = New Point(
                    IconSet.Left + (i \ Raw) * IconSet.Width * IconMag,
                    IconSet.Top + (i Mod Raw) * IconSet.Height * IconMag),
                .Size = New Point(IconSet.Width * IconMag, IconSet.Height * IconMag),
                .Text = $"{Type(i)}={TypeCount(i)}",
                .Font = RDFont,
                .BackColor = Color.Transparent,
                .BackgroundImage = ReturnImage(i, SCType.None),
                .Name = Type(i)
            }

            If TypeCount(i) = 0 Then
                EventIcon(i).BackgroundImage = ReturnImage(i, SCType.Inactive)
            End If

            AddHandler EventIcon(i).MouseEnter, AddressOf a
            AddHandler EventIcon(i).MouseLeave, AddressOf b
            Me.Controls.Add(EventIcon(i))


        Next
    End Sub
    Sub ShowEventsCount()
        While Me.Controls.Count > StartControlCounts
            Me.Controls.Remove(Me.Controls.Item(StartControlCounts))
        End While

        Select Case EventTypeIndex
            Case 0
                CreateEventsIcon(SoundEvents, SoundEventCount, My.Resources.ResourceData.sounds)
            Case 1
                CreateEventsIcon(RailEvents, RailEventCount, My.Resources.ResourceData.rails)
            Case 2
                CreateEventsIcon(MotionEvents, MotionEventCount, My.Resources.ResourceData.motions)
            Case 3
                CreateEventsIcon(SpriteEvents, SpriteEventCount, My.Resources.ResourceData.sprites)
            Case 4
                CreateEventsIcon(RoomEvents, RoomEventCount, My.Resources.ResourceData.rooms)
            Case Else

        End Select

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
