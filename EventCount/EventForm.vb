Imports System.Drawing.Imaging
Imports System.Collections.Generic
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Windows.Forms
Imports System.Runtime.InteropServices

Public Class EventForm

    ReadOnly IconPath = $"{Application.StartupPath}\Assets\"
    ReadOnly KeyString() As String = System.IO.File.OpenText($"{IconPath}Keys.txt").ReadToEnd.Split(vbCrLf)

    Dim FilePath As String = ""
    Dim RdlevelFile As String = ""
    ReadOnly temp As String = "\%RDLevelUnzip%"

    Dim RDFont As Font

    Dim IconSet As Rectangle = New Rectangle(
        New Point(48, 34),
        New Point(56, 28))
    ReadOnly IconMag As UInt16 = 1

    Dim EventTypeIndex = EType.Sound

    Dim StartControlCounts As UInt16 = 0

    ReadOnly MouseToIntroductionBox As Point = New Point(10, -10)

    ReadOnly Row As UInt16 = 4

    Dim Cou = New Dictionary(Of String, UInt64)
    Dim EveType = New Dictionary(Of String, String)
    Dim AllLang = New Dictionary(Of LangTable, String)
    Dim Assets = New Dictionary(Of String, IconTypes)
    Dim SelEvent = New List(Of String)

    Dim CurrentLang As LangType = LangType.zh

    Dim Tick As Double = 0
    Dim startSize As Size
    Dim EndHeight As Int16
    Dim FrameSize As Size = (Me.PointToScreen(New Point()) - Me.Location) + New Point((Me.PointToScreen(New Point()) - Me.Location).X, -(Me.PointToScreen(New Point()) - Me.Location).X) ' (Me.PointToScreen(New Point()) - Me.Location).X)

    Public Property EventForm1 As EventForm
        Get
            Return Nothing
        End Get
        Set(value As EventForm)
        End Set
    End Property


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
        Dim fontData As Byte() = My.Resources.ResourceData.SourceHanSans_Bold
        Dim fontCollection As New PrivateFontCollection '创建字体集合
        fontCollection.AddMemoryFont(Marshal.UnsafeAddrOfPinnedArrayElement(fontData, 0), fontData.Length) '将字体字节数组添加到字体集合中
        RDFont = New Font(fontCollection.Families(0), 12)
        InfoLabel.Font = RDFont
    End Sub


    ''' <summary>
    ''' 读取翻译
    ''' </summary>
    Sub ReadLang()
        For lang = 0 To 1
            For Each K In KeyString
                Dim e() = K.Split(vbTab)
                AllLang.add(New LangTable With {.ID = e(0), .Lang = lang}, e(CurrentLang + 2))
            Next
        Next
    End Sub


    ''' <summary>
    ''' 读取图片
    ''' </summary>
    Sub ReadImage()
        For Each K In KeyString
            Dim E = K.Split(vbTab)(0)
            Assets.add(E,
                    New IconTypes With {
                    .None = Image.FromFile($"{IconPath}{E}None.png"),
                    .Selected = Image.FromFile($"{IconPath}{E}Selected.png"),
                    .Inactive = Image.FromFile($"{IconPath}{E}Inactive.png"),
                    .InactiveSelected = Image.FromFile($"{IconPath}{E}InactiveSelected.png")
                    }
            )
        Next
    End Sub


    Sub ReadType()
        For Each K In KeyString
            Dim E = K.Split(vbTab)
            EveType.add(E(0), E(1))
        Next
    End Sub


    ''' <summary>
    ''' 窗体加载
    ''' </summary>
    ''' <param name="sender">控件</param>
    ''' <param name="e">控件参数</param>
    Private Sub Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If OpenLevel.ShowDialog() <> DialogResult.OK Then
            End
        End If
        CountingEvents()
        ReadLang()
        ReadType()
        ReadImage()
        TitleBox.Size = New Point(My.Resources.ResourceData.Title.Width / 5, My.Resources.ResourceData.Title.Height)
        StartControlCounts = Me.Controls.Count
        ChangeFont()
        ShowEventsCount()
        Renew(Nothing, Nothing)
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
        ShowInfo($"{IIf(Cou(sender.Name), $"{Cou(sender.Name)} * ", "")}{AllLang(New LangTable With {.ID = sender.Name, .Lang = CurrentLang})}")
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
            Else
                SelEvent.remove(sender.Name)
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
        ShowInfo("")
        If SelEvent.Contains(sender.Name) = False Then
            If Cou(sender.Name) Then
                sender.BackgroundImage = ReturnImage(sender.Name, "None")
            Else
                sender.BackgroundImage = ReturnImage(sender.Name, "Inactive")
            End If
        End If
    End Sub


    Sub ShowInfo(Head As String)
        For Each I In SelEvent
            Head += $"{vbCrLf}{EveType(I)}     {Cou(I)} * {AllLang(New LangTable With {.ID = I, .Lang = CurrentLang})}"
        Next
        InfoLabel.Text = Head
        'InfoLabel.CreateGraphics.DrawString("aaa", RDFont, New SolidBrush(Color.Red), New Point(0, 0))
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
        'While Me.Controls.Count > StartControlCounts
        '    Me.Controls.Remove(Me.Controls.Item(StartControlCounts))
        'End While
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
        For Each K In KeyString
            Dim E = K.Split(vbTab)(0)
            Cou.add(E, CountingTexts(RdlevelFile, $"""type"": ""{E}"""))
        Next
    End Sub


    ''' <summary>
    ''' 删除缓存
    ''' </summary>
    ''' <returns></returns>
    Public Sub KillTemp()
        If My.Computer.FileSystem.DirectoryExists($"{Environ("TEMP")}{temp}") Then '删除原输出
            My.Computer.FileSystem.DeleteDirectory($"{Environ("TEMP")}{temp}",
            FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        End If
    End Sub


    ''' <summary>
    ''' 文件对话框
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub OpenLevel_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenLevel.FileOk
        If IO.Path.GetExtension(OpenLevel.FileName) = ".rdlevel" Then
        ElseIf IO.Path.GetExtension(OpenLevel.FileName) = ".rdzip" Then
            KillTemp()
            IO.Compression.ZipFile.ExtractToDirectory(OpenLevel.FileName, Environ("TEMP") & temp)
            Dim s As String() = IO.Directory.GetFiles($"{Environ("TEMP")}{temp}", "*.rdlevel")
            OpenLevel.FileName = $"{Environ("temp")}{temp}\{IO.Path.GetFileName(s(0))}"
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


    ''' <summary>
    ''' 刷新
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Sub Renew(sender As Object, e As MouseEventArgs)
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


    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Tick < 1 Then
            Me.Width = startSize.Width + (Math.Max(Me.BackgroundImage.Width, InfoLabel.Width) + FrameSize.Width - startSize.Width) * Ease(Tick)
            Me.Height = startSize.Height + (EndHeight - startSize.Height + FrameSize.Height) * Ease(Tick)
            Tick += 0.01
            Debug.Print(Tick)
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
End Class
