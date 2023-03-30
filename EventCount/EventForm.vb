Imports System.Drawing.Text
Imports System.IO
Imports System.Threading
Imports System.Runtime.InteropServices

Public Class EventForm

    ReadOnly IconPath = $"{Application.StartupPath}\Assets\"
    ReadOnly KeyString() As String = File.OpenText(GetFile("Info\Keys")).ReadToEnd.Split(vbCrLf)

    Dim FilePath As String = ""
    Dim RdlevelFile As String = ""
    ReadOnly TempFile As String = "\%EventCounterRDLevelUnzip%"
    ReadOnly AchiFile As String = "C:\ProgramData\Achievements"

    Dim RDFont As Font

    Dim IconSet As Rectangle = New Rectangle(
        New Point(48, 34),
        New Point(56, 28))
    ReadOnly IconMag As UInt16 = 1

    Dim EventTypeIndex = EType.Sound

    Dim StartControlCounts As UInt16 = 0

    ReadOnly MouseToIntroductionBox As Point = New Point(10, -10)

    ReadOnly Row As UInt16 = 4

    Dim Process As Double = 0


    Dim Cou = New Dictionary(Of String, UInt64)
    Dim EveType = New Dictionary(Of String, String)
    Dim EventsLang = New Dictionary(Of LangTable, String)
    Dim TitleLang = New Dictionary(Of LangTable, String)
    Dim AchieveMentsLang = New Dictionary(Of LangTable, String)
    Dim Assets = New Dictionary(Of String, IconTypes)
    Dim SelEvent = New List(Of String)
    Dim AchieveMents = New List(Of String)

    Dim CurrentLang As LangType = LangType.zh

    Dim Tick As Double = 0
    Dim startSize As Size
    Dim EndHeight As Int16
    Dim FrameSize As Size =
        (Me.PointToScreen(New Point()) - Me.Location) +
        New Point((Me.PointToScreen(New Point()) - Me.Location).X, -(Me.PointToScreen(New Point()) - Me.Location).X)


    ''' <summary>
    ''' 图标选择情况,但是结构体
    ''' </summary>
    Structure IconTypes
        Dim None As Image
        Dim Selected As Image
        Dim Inactive As Image
        Dim InactiveSelected As Image
    End Structure


    ''' <summary>
    ''' 语言表格
    ''' </summary>
    Structure LangTable
        Dim ID As String
        Dim Lang As LangType
    End Structure


    ''' <summary>
    ''' 语言
    ''' </summary>
    Enum LangType
        zh
        en
    End Enum


    ''' <summary>
    ''' 全局事件类型环境
    ''' </summary>
    Enum EType
        Sound
        Rail
        Motion
        Sprite
        Room
    End Enum


    ''' <summary>
    ''' 字体更换
    ''' </summary>
    Sub ChangeFont()
        Dim fontCollection As New PrivateFontCollection '创建字体集合
        fontCollection.AddFontFile(GetFile("Fonts\SourceHanSansCN-Bold.otf"))
        RDFont = New Font(fontCollection.Families(0), 12)
        InfoLabel.Font = RDFont
    End Sub


    ''' <summary>
    ''' 窗体加载
    ''' </summary>
    ''' <param name="sender">控件</param>
    ''' <param name="e">控件参数</param>
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FirstLoad()
        Reload()
    End Sub


    Sub FirstLoad()
        If My.Computer.FileSystem.FileExists(AchiFile) Then
            Dim a = New StreamReader(AchiFile)
            Dim b = a.ReadToEnd
            a.Close()
            System.IO.File.Delete(AchiFile)
            For Each I In b.Split(vbCrLf)
                If I <> "" Then
                    AchieveMents.add(I)
                End If
            Next
        End If
        'TitleBox.Size = New Point(My.Resources.ResourceData.Title.Width / 5, My.Resources.ResourceData.Title.Height)
        StartControlCounts = Me.Controls.Count
        ChangeFont()


        For Each K In KeyString
            Dim E = K.Split(vbTab)(0)
            Assets.add(E,
                    New IconTypes With {
                    .None = Image.FromFile(GetFile($"Icons\{E}None.png")),
                    .Selected = Image.FromFile(GetFile($"Icons\{E}Selected.png")),
                    .Inactive = Image.FromFile(GetFile($"Icons\{E}Inactive.png")),
                    .InactiveSelected = Image.FromFile(GetFile($"Icons\{E}InactiveSelected.png"))
                    }
            )
        Next

        For lang = 0 To 1
            For Each K In KeyString
                Dim e = K.Split(vbTab)
                EventsLang.add(New LangTable With {.ID = e(0), .Lang = lang}, e(lang + 2))
            Next
        Next

        For Each K In KeyString
            Dim E = K.Split(vbTab)
            EveType.add(E(0), E(1))
        Next

        For lang = 0 To 1
            For Each K In My.Resources.ResourceData.TitleName.Split(vbCrLf)
                Dim e = K.Split(vbTab)
                TitleLang.add(New LangTable With {.ID = e(0), .Lang = lang}, e(lang + 1))
            Next
        Next

        For lang = 0 To 1
            For Each K In My.Resources.ResourceData.Achievement.Split(vbCrLf)
                Dim e = K.Split(vbTab)
                AchieveMentsLang.add(New LangTable With {.ID = e(0), .Lang = lang}, e(lang + 1))
            Next
        Next

    End Sub


    Sub Reload()
        Cou.clear
        For Each K In KeyString
            Dim E = K.Split(vbTab)(0)
            Cou.add(E, 0)
        Next
        If RdlevelFile.Length Then
            Timer2.Start()
            Dim CE As New Thread(AddressOf CountingEvents)
            CE.Start()
            CE.Interrupt()
        End If
        SelEvent.clear
        RenewTitle(Nothing, Nothing)
        InfoLabel.Top = TitleBox.Height
        CallHeightChange(InfoLabel.Height + InfoLabel.Top + FrameSize.Height)

    End Sub


    ''' <summary>
    ''' 返回图标
    ''' </summary>
    ''' <param name="KeyWord">索引词</param>
    ''' <param name="SCType">图标状态</param>
    ''' <returns></returns>
    Function ReturnImage(KeyWord As String, SCType As String) As Image
        Dim Icon As IconTypes = Assets(KeyWord)
        Select Case SCType
            Case "None"
                Return Icon.None
            Case "Selected"
                Return Icon.Selected
            Case "Inactive"
                Return Icon.Inactive
            Case "InactiveSelected"
                Return Icon.InactiveSelected
        End Select
        Return Nothing
    End Function


    ''' <summary>
    ''' 鼠标进入图标事件
    ''' </summary>
    ''' <param name="sender">图标控件</param>
    ''' <param name="e">控件属性</param>
    Sub MouseEnterTheIcon(sender As PictureBox, e As EventArgs)
        ShowLevelName($"{IIf(Cou(sender.Name), $"{Cou(sender.Name)} * ", "")}{EventsLang(New LangTable With {.ID = sender.Name, .Lang = CurrentLang})}")
        If Cou(sender.Name) Then
            sender.BackgroundImage = ReturnImage(sender.Name, "Selected")
        Else
            sender.BackgroundImage = ReturnImage(sender.Name, "InactiveSelected")
        End If
    End Sub


    ''' <summary>
    ''' 鼠标点击图标事件
    ''' </summary>
    ''' <param name="sender">图标控件</param>
    ''' <param name="e">控件属性</param>
    Sub MouseClickTheIcon(sender As PictureBox, e As EventArgs)
        If Cou(sender.Name) Then
            If SelEvent.Contains(sender.Name) = False Then
                SelEvent.add(sender.Name)
                My.Computer.Audio.Play(GetFile("Sounds\On.wav"))
                If sender.Name = "NewWindowDance" Then
                    CallAchievements(2)
                    '''窗口移动彩蛋
                End If
            Else
                SelEvent.remove(sender.Name)
                My.Computer.Audio.Play(GetFile("Sounds\Off.wav"))
            End If
            MouseEnterTheIcon(sender, e)
            CallHeightChange(InfoLabel.Height + InfoLabel.Top + FrameSize.Height)
        End If
    End Sub


    ''' <summary>
    ''' 鼠标离开图标事件
    ''' </summary>
    ''' <param name="sender">图标控件</param>
    ''' <param name="e">控件属性</param>
    Sub MouseLeaveTheIcon(sender As PictureBox, e As EventArgs)
        ShowLevelName("")
        If SelEvent.Contains(sender.Name) = False Then
            If Cou(sender.Name) Then
                sender.BackgroundImage = ReturnImage(sender.Name, "None")
            Else
                sender.BackgroundImage = ReturnImage(sender.Name, "Inactive")
            End If
        End If
    End Sub


    Sub ShowLevelName(Head As String)
        For Each I In SelEvent
            Head += $"{vbCrLf}{TitleLang(New LangTable With {.ID = EveType(I).ToString, .Lang = CurrentLang})}     {Cou(I)} * {EventsLang(New LangTable With {.ID = I, .Lang = CurrentLang})}"
        Next
        InfoLabel.Text = Head
    End Sub


    ''' <summary>
    ''' 创建图标控件
    ''' </summary>
    ''' <param name="ET">全局事件类型</param>
    Sub CreateEventsIcon(ET As EType)
        Dim i = 0
        For Each K In KeyString
            If K.Split(vbTab)(1) = ET.ToString Then
                Dim NewIcon = New PictureBox With {
                    .Location = New Point(
                        IconSet.Left + (i \ Row) * IIf(EventTypeIndex < 2, IconSet.Width, IconSet.Width / 2) * IconMag,
                        IconSet.Top + (i Mod Row) * IconSet.Height * IconMag),
                    .Font = RDFont,
                    .BackColor = Color.Transparent,
                    .Name = K.Split(vbTab)(0)
                }

                If SelEvent.Contains(NewIcon.Name) = False Then
                    If Cou(NewIcon.Name) = 0 Then
                        NewIcon.BackgroundImage = ReturnImage(NewIcon.Name, "Inactive")
                    Else
                        NewIcon.BackgroundImage = ReturnImage(NewIcon.Name, "None")
                    End If
                Else
                    NewIcon.BackgroundImage = ReturnImage(NewIcon.Name, "Selected")
                End If
                NewIcon.Size = ReturnImage(NewIcon.Name, "None").Size

                AddHandler NewIcon.MouseEnter, AddressOf MouseEnterTheIcon
                AddHandler NewIcon.Click, AddressOf MouseClickTheIcon
                AddHandler NewIcon.MouseLeave, AddressOf MouseLeaveTheIcon
                Me.Controls.Add(NewIcon)

                i += 1
            End If
        Next
    End Sub


    ''' <summary>
    ''' 刷新图标控件
    ''' </summary>
    Sub ShowEventsCount()
        Dim i = 0
        While i < Me.Controls.Count
            If Me.Controls.Item(i).GetType.ToString = "System.Windows.Forms.PictureBox" And EveType.ContainsKey(Me.Controls.Item(i).Name) Then
                Me.Controls.Remove(Me.Controls.Item(i))
            Else i += 1
            End If
        End While
        CreateEventsIcon(EventTypeIndex)
    End Sub


    ''' <summary>
    ''' 为数组赋值
    ''' </summary>
    Sub CountingEvents()
        Dim Text = RdlevelFile.Split("{")
        For i As UInt64 = 0 To Text.Length - 1
            Dim T = Text(i)
            If T.IndexOf("type") > 0 Then
                Dim key = FindMidWord("""type"": """, """", T)
                If Cou.ContainsKey(key) Then
                    Cou(key) += 1
                End If
            End If
            Process = i / (Text.Length - 1)
        Next
        Timer2.Stop()
    End Sub


    ''' <summary>
    ''' 删除缓存
    ''' </summary>
    ''' <returns></returns>
    Public Sub KillTemp()
        If My.Computer.FileSystem.DirectoryExists($"{Environ("TEMP")}{TempFile}") Then '删除原输出
            My.Computer.FileSystem.DeleteDirectory($"{Environ("TEMP")}{TempFile}",
            FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
    End Sub


    ''' <summary>
    ''' 文件对话框
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenLevel_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenLevel.FileOk
        If Path.GetExtension(OpenLevel.FileName) = ".rdlevel" Then
        ElseIf Path.GetExtension(OpenLevel.FileName) = ".rdzip" Then
            KillTemp()
            Compression.ZipFile.ExtractToDirectory(OpenLevel.FileName, $"{Environ("TEMP")}{TempFile}")
            Dim s As String() = Directory.GetFiles($"{Environ("TEMP")}{TempFile}", "*.rdlevel")
            OpenLevel.FileName = $"{Environ("temp")}{TempFile}\{IO.Path.GetFileName(s(0))}"
        Else
            CallAchievements(1)
            RdlevelFile = ""
        End If
        FilePath = OpenLevel.FileName
        If File.Exists(FilePath) Then
            RdlevelFile = File.OpenText(FilePath).ReadToEnd
            Me.Text = FindMidWord("""song"": """, """,", RdlevelFile)
        End If

    End Sub


    ''' <summary>
    ''' 刷新侧边标题栏
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Sub RenewTitle(sender As Object, e As MouseEventArgs)
        Dim TitleImage = My.Resources.ResourceData.Title
        TitleBox.BackgroundImage = TitleImage.Clone(
            New Rectangle(EventTypeIndex * 48, 0, 48, 152),
            TitleImage.PixelFormat)
        ShowEventsCount()
    End Sub


    ''' <summary>
    ''' 左侧标题选项卡点击事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TitleBox_Click(sender As Object, e As MouseEventArgs) Handles TitleBox.Click
        Dim P = PointToClient(Cursor.Position)
        Dim Q = New Point((P.X - 4) \ 30, (P.Y - 4) \ 29)

        Select Case e.Button
            Case MouseButtons.Left
                If Q.Y = EventTypeIndex Then
                    My.Computer.Audio.Play(GetFile("Sounds\On.wav"))
                    For Each RecordedEvents In EveType
                        If RecordedEvents.value = EventTypeIndex.ToString And
                            Cou(RecordedEvents.key) Then
                            If SelEvent.Contains(RecordedEvents.key) Then
                                SelEvent.remove(RecordedEvents.key)
                            Else
                                SelEvent.add(RecordedEvents.key)
                            End If
                        End If
                    Next
                Else
                    Select Case Q
                        Case New Point(0, 0)
                            EventTypeIndex = EType.Sound
                            My.Computer.Audio.Play(GetFile("Sounds\0.wav"))
                        Case New Point(0, 1)
                            EventTypeIndex = EType.Rail
                            My.Computer.Audio.Play(GetFile("Sounds\1.wav"))
                        Case New Point(0, 2)
                            EventTypeIndex = EType.Motion
                            My.Computer.Audio.Play(GetFile("Sounds\2.wav"))
                        Case New Point(0, 3)
                            EventTypeIndex = EType.Sprite
                            My.Computer.Audio.Play(GetFile("Sounds\3.wav"))
                        Case New Point(0, 4)
                            EventTypeIndex = EType.Room
                            My.Computer.Audio.Play(GetFile("Sounds\4.wav"))
                        Case Else
                    End Select
                End If
            Case MouseButtons.Right
                My.Computer.Audio.Play(GetFile("Sounds\Off.wav"))
                Dim SE = New List(Of String)
                For Each I In SelEvent
                    SE.Add(I)
                Next
                SelEvent.Clear
                Dim SelectType As EType
                Select Case Q
                    Case New Point(0, 0)
                        SelectType = EType.Sound
                    Case New Point(0, 1)
                        SelectType = EType.Rail
                    Case New Point(0, 2)
                        SelectType = EType.Motion
                    Case New Point(0, 3)
                        SelectType = EType.Sprite
                    Case New Point(0, 4)
                        SelectType = EType.Room
                    Case Else
                End Select
                For Each I In SE
                    If EveType(I) <> SelectType.ToString Then
                        SelEvent.add(I)
                    End If
                Next
        End Select
        RenewTitle(Nothing, Nothing)
        ShowLevelName("")
        CallHeightChange(InfoLabel.Height + InfoLabel.Top + FrameSize.Height)
    End Sub


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Tick < 1 Then
            Me.Width = startSize.Width + (Math.Max(Me.BackgroundImage.Width, InfoLabel.Width) + FrameSize.Width - startSize.Width) * Ease(Tick)
            Me.Height = startSize.Height + (EndHeight - startSize.Height + FrameSize.Height) * Ease(Tick)
            Tick += 0.01
        Else Timer1.Stop()
            Tick = 0
        End If
    End Sub


    Function Ease(x As Double) As Double
        x = Math.Clamp(x, 0, 1)
        Return IIf(x = 1, 1, 1 - 2 ^ (-10 * x))
    End Function


    Sub CallHeightChange(Height As UInt16)
        startSize = Me.Size
        EndHeight = Height
        Tick = 0
        Timer1.Start()
    End Sub


    Function GetFile(Path As String) As String
        If My.Computer.FileSystem.FileExists($"{IconPath}{Path}") Then
            Return $"{IconPath}{Path}"
        Else
            CallAchievements(0)
            End
        End If
    End Function


    Function FindMidWord(startStr As String, endStr As String, text As String) As String
        Dim startIndex As Integer = text.IndexOf(startStr)
        Dim endIndex As Integer = text.IndexOf(endStr, startIndex + startStr.Length)
        If startIndex = -1 OrElse endIndex = -1 Then
            Return ""
        End If
        Dim contentStartIndex As Integer = startIndex + startStr.Length
        Dim contentLength As Integer = endIndex - contentStartIndex
        If contentLength < 0 Then
            Return ""
        End If
        Return text.Substring(contentStartIndex, contentLength)
    End Function


    Private Sub EventForm_Closed(sender As Object, e As EventArgs) Handles MyBase.Closed
        Dim b = New StreamWriter(AchiFile)
        For Each I In AchieveMents
            b.WriteLine(I)
        Next
        b.Close()
    End Sub


    Sub CallAchievements(Index As UInt16)
        Dim T = My.Resources.ResourceData.Achievement.Split(vbCrLf)(Index).Split(vbTab)(0)
        If AchieveMents.Contains(T) = False Then
            AchieveMents.add(T)
        End If
    End Sub


    Private Sub EventForm_Click(sender As Object, e As EventArgs) Handles MyBase.Click
        Dim P = PointToClient(Cursor.Position)
        If P.X > 48 And P.X < 79 And P.Y > 4 And P.Y < 19 Then
            My.Computer.Audio.Play(GetFile($"Sounds\Open{Math.Floor(Rnd() * 3)}.wav"))
            OpenLevel.ShowDialog()
            Reload()
            ShowLevelName("")
            CallHeightChange(InfoLabel.Height + InfoLabel.Top + FrameSize.Height)
        End If
        If P.X > 80 And P.X < 111 And P.Y > 4 And P.Y < 19 Then
            '''显示成就
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        ProgressBar1.Value = Process * 100
        If Process = 1 Then
            ProgressBar1.Value = 0
        End If
    End Sub
End Class
