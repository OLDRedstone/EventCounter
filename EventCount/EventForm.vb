Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Runtime.InteropServices
#Disable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型

Public Class EventForm

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
    Dim IconSet As Rectangle = New Rectangle(
        New Point(48, 34),
        New Point(56, 28))
    ReadOnly IconMag As UInt16 = 1

    Dim EventTypeIndex As EType = EType.Sound

    Dim StartControlCounts As UInt16 = 0

    ReadOnly MouseToIntroductionBox As Point = New Point(10, -10)

    ReadOnly Raw As UInt16 = 4

    Dim Cou = New Dictionary(Of String, UInt64)
    Dim Lan = New Dictionary(Of String, String)
    Dim CurrentLanguage As Language = Language.zh
    Enum Language
        zh
        en
    End Enum
    Sub ChangeFont()
        Dim fontData As Byte() = My.Resources.ResourceData.SourceHanSans_Bold
        Dim fontCollection As New PrivateFontCollection '创建字体集合
        fontCollection.AddMemoryFont(Marshal.UnsafeAddrOfPinnedArrayElement(fontData, 0), fontData.Length) '将字体字节数组添加到字体集合中
        RDFont = New Font(fontCollection.Families(0), 12)
        InformationLabel.Font = RDFont
    End Sub

    Sub ReadToKey()
        Dim SoundLan() As String = Nothing
        Dim RailLan() As String = Nothing
        Dim MotionLan() As String = Nothing
        Dim SpriteLan() As String = Nothing
        Dim roomLan() As String = Nothing
        Select Case CurrentLanguage
            Case Language.zh
                SoundLan = My.Resources.zh.Sound.Split(vbCrLf)
                RailLan = My.Resources.zh.Rail.Split(vbCrLf)
                MotionLan = My.Resources.zh.Motion.Split(vbCrLf)
                SpriteLan = My.Resources.zh.Sprite.Split(vbCrLf)
                roomLan = My.Resources.zh.Room.Split(vbCrLf)
            Case Language.en

            Case Else

        End Select
        For i = 0 To SoundEvents.Length - 1
            Lan.Add(SoundEvents(i), SoundLan(i))
        Next
        For i = 0 To RailEvents.Length - 1
            Lan.Add(RailEvents(i), RailLan(i))
        Next
        For i = 0 To MotionEvents.Length - 1
            Lan.Add(MotionEvents(i), MotionLan(i))
        Next
        For i = 0 To SpriteEvents.Length - 1
            Lan.Add(SpriteEvents(i), SpriteLan(i))
        Next
        For i = 0 To RoomEvents.Length - 1
            Lan.Add(RoomEvents(i), roomLan(i))
        Next


        For i = 0 To SoundEvents.Length - 1
            Cou.Add(SoundEvents(i), SoundEventCount(i))
        Next
        For i = 0 To RailEvents.Length - 1
            Cou.Add(RailEvents(i), RailEventCount(i))
        Next
        For i = 0 To MotionEvents.Length - 1
            Cou.Add(MotionEvents(i), MotionEventCount(i))
        Next
        For i = 0 To SpriteEvents.Length - 1
            Cou.Add(SpriteEvents(i), SpriteEventCount(i))
        Next
        For i = 0 To RoomEvents.Length - 1
            Cou.Add(RoomEvents(i), RoomEventCount(i))
        Next
    End Sub

    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If OpenLevel.ShowDialog() <> DialogResult.OK Then
            End
        End If
        CountingEvents()
        ReadToKey()
        ChangeFont()
        StartControlCounts = Me.Controls.Count
        ShowEventsCount()
        Renew(Nothing, Nothing)
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
    Sub MouseEnterTheIcon(sender As PictureBox, e As EventArgs)
        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        InformationLabel.Text = $"{Lan(sender.Name)}  {Cou(sender.Name)}"
        If ReturnData(Index, 1) Then
            sender.BackgroundImage = ReturnImage(Index, SCType.Selected)
        Else
            sender.BackgroundImage = ReturnImage(Index, SCType.InactiveSelected)
        End If
    End Sub
    Sub MouseLeaveTheIcon(sender As PictureBox, e As EventArgs)
        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        If ReturnData(Index, 1) Then
            sender.BackgroundImage = ReturnImage(Index, SCType.None)
        Else
            sender.BackgroundImage = ReturnImage(Index, SCType.Inactive)
        End If
    End Sub

    Sub CreateEventsIcon(Type As String(), TypeCount As UInt64(), Icon As Bitmap)

        Dim EIcon(Type.Length) As PictureBox
        Dim j As UInt16 = 0
        For i As UInt16 = 0 To Type.Length - 1
            EIcon(i) = New PictureBox With {
                .Location = New Point(
                    IconSet.Left + (i \ Raw) * IIf(EventTypeIndex < 2, IconSet.Width, IconSet.Width / 2) * IconMag,
                    IconSet.Top + (i Mod Raw) * IconSet.Height * IconMag),
                .Size = New Point(
                     IIf(EventTypeIndex < 2, IconSet.Width, IconSet.Width / 2) * IconMag,
                    IconSet.Height * IconMag),
                .Font = RDFont,
                .BackColor = Color.Transparent,
                .BackgroundImage = ReturnImage(i, SCType.None),
                .Name = Type(i)
            }


            If Cou(EIcon(i).Name) = 0 Then
                EIcon(i).BackgroundImage = ReturnImage(i, SCType.Inactive)
            End If

            AddHandler EIcon(i).MouseEnter, AddressOf MouseEnterTheIcon
            AddHandler EIcon(i).MouseLeave, AddressOf MouseLeaveTheIcon
            Me.Controls.Add(EIcon(i))

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
            RdlevelFile = ""
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

    Sub Renew(sender As Object, e As MouseEventArgs) Handles MyBase.Click
        EventTypeIndex = EventTypeIndex Mod 5
        Dim TitleImage = My.Resources.ResourceData.Title
        TitleBox.BackgroundImage = TitleImage.Clone(
            New Rectangle(EventTypeIndex * 48, 0, 48, 152),
            TitleImage.PixelFormat)
        ShowEventsCount()
    End Sub

    Private Sub TitleBox_Click(sender As Object, e As EventArgs) Handles TitleBox.Click
        Dim P = PointToClient(Cursor.Position)
        P = New Point((P.X - 4) \ 30, (P.Y - 4) \ 29)
        Select Case P
            Case New Point(0, 0)
                EventTypeIndex = 0
            Case New Point(0, 1)
                EventTypeIndex = 1
            Case New Point(0, 2)
                EventTypeIndex = 2
            Case New Point(0, 3)
                EventTypeIndex = 3
            Case New Point(0, 4)
                EventTypeIndex = 4
            Case Else
        End Select
        Renew(Nothing, Nothing)
    End Sub
End Class
