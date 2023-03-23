Imports System.Drawing.Imaging
Imports System.Drawing.Text
Imports System.Globalization
Imports System.Runtime.InteropServices

Public Class EventForm

    ReadOnly IconPath = $"{Application.StartupPath}\Icons\"
    ReadOnly KeyString() As String = System.IO.File.OpenText($"{IconPath}Keys.txt").ReadToEnd.Split(vbCrLf)

    Dim FilePath As String = ""
    Dim RdlevelFile As String = ""
    ReadOnly temp As String = "\%RDLevelUnzip%"

    Dim RDFont As Font

    ReadOnly IconLeft As UInt16 = 1
    Dim IconSet As Rectangle = New Rectangle(
        New Point(48, 34),
        New Point(56, 28))
    ReadOnly IconMag As UInt16 = 2

    Dim EventTypeIndex As EType = EType.Sound

    Dim StartControlCounts As UInt16 = 0

    ReadOnly MouseToIntroductionBox As Point = New Point(10, -10)

    ReadOnly Raw As UInt16 = 4

    Dim Cou = New Dictionary(Of String, UInt64)
    Dim AllLang = New Dictionary(Of LangTable, String)
    Dim Assets = New Dictionary(Of String, IconTypes)
    Dim CurrentLanguage As LangType = LangType.en


    ''' <summary>
    ''' 图标种类,但是结构体
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
    ''' 图标状态
    ''' </summary>
    Enum SCType
        None
        Selected
        Inactive
        InactiveSelected
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
        InformationLabel.Font = RDFont
    End Sub


    ''' <summary>
    ''' 读取翻译
    ''' </summary>
    Sub ReadLang()
        For lang = 0 To 1
            For Each K In KeyString
                Dim e() = K.Split(vbTab)
                AllLang.add(New LangTable With {.ID = e(0), .Lang = lang}, e(CurrentLanguage + 2))
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
        ReadImage()
        ChangeFont()
        StartControlCounts = Me.Controls.Count
        ShowEventsCount()
        Renew(Nothing, Nothing)
    End Sub


    ''' <summary>
    ''' 返回图标
    ''' </summary>
    ''' <param name="KeyWord">索引词</param>
    ''' <param name="SCType">图标状态</param>
    ''' <returns></returns>
    Function ReturnImage(KeyWord As String, SCType As SCType) As Image
        Dim Icon As IconTypes = Assets(KeyWord)
        Select Case SCType
            Case SCType.None
                Return Icon.None
            Case SCType.Selected
                Return Icon.Selected
            Case SCType.Inactive
                Return Icon.Inactive
            Case SCType.InactiveSelected
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
        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        InformationLabel.Text = $"{AllLang(New LangTable With {.ID = sender.Name, .Lang = CurrentLanguage})}  {Cou(sender.Name)}"
        If Cou(sender.Name) Then
            sender.BackgroundImage = ReturnImage(sender.Name, SCType.Selected)
        Else
            sender.BackgroundImage = ReturnImage(sender.Name, SCType.InactiveSelected)
        End If
    End Sub


    ''' <summary>
    ''' 鼠标离开图标事件
    ''' </summary>
    ''' <param name="sender">图标控件</param>
    ''' <param name="e">控件属性</param>
    Sub MouseLeaveTheIcon(sender As PictureBox, e As EventArgs)
        InformationLabel.Text = ""
        Dim Index As UInt16 = Controls.IndexOf(sender) - StartControlCounts
        If Cou(sender.Name) Then
            sender.BackgroundImage = ReturnImage(sender.Name, SCType.None)
        Else
            sender.BackgroundImage = ReturnImage(sender.Name, SCType.Inactive)
        End If
    End Sub


    ''' <summary>
    ''' 创建图标控件
    ''' </summary>
    ''' <param name="ET">全局事件类型</param>
    Sub CreateEventsIcon(ET As EType)
        Dim i = 0
        For Each K In KeyString
            If K.Split(vbTab)(1) = ET.ToString Then
                Debug.Print(StartControlCounts)
                Dim NewIcon = New PictureBox With {
                    .Location = New Point(
                        IconSet.Left + (i \ Raw) * IIf(EventTypeIndex < 2, IconSet.Width, IconSet.Width / 2) * IconMag,
                        IconSet.Top + (i Mod Raw) * IconSet.Height * IconMag),
                    .Font = RDFont,
                    .BackColor = Color.Transparent,
                    .Name = K.Split(vbTab)(0)
                }


                If Cou(NewIcon.Name) = 0 Then
                    NewIcon.BackgroundImage = ReturnImage(NewIcon.Name, SCType.Inactive)
                Else
                    NewIcon.BackgroundImage = ReturnImage(NewIcon.Name, SCType.None)
                End If
                NewIcon.Size = ReturnImage(NewIcon.Name, SCType.None).Size

                AddHandler NewIcon.MouseEnter, AddressOf MouseEnterTheIcon
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
        While Me.Controls.Count > StartControlCounts
            Me.Controls.Remove(Me.Controls.Item(StartControlCounts))
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
        If My.Computer.FileSystem.DirectoryExists(Environ("temp") & temp) Then '删除原输出
            My.Computer.FileSystem.DeleteDirectory(Environ("temp") & temp,
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
            IO.Compression.ZipFile.ExtractToDirectory(OpenLevel.FileName, Environ("temp") & temp)
            Dim s As String() = IO.Directory.GetFiles($"{Environ("temp")}{temp}", "*.rdlevel")
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
    Sub Renew(sender As Object, e As MouseEventArgs) Handles MyBase.Click
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


End Class
