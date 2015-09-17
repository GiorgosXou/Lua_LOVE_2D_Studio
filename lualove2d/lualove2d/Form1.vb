Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports FastColoredTextBoxNS


Public Class Form1
    Friend WithEvents fstclrtxtbox As FastColoredTextBox


    Public CurrentProject As String = String.Empty
    Dim m As New MARGINS '(0, 0, 0, 0)
    Public CurrentHardDriver As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).ToString.Remove(0, 6).Substring(0, 3)

    <DllImport("dwmapi.dll", PreserveSig:=True)>
    Private Shared Function DwmSetWindowAttribute(hwnd As IntPtr, attr As Integer, ByRef attrValue As Integer, attrSize As Integer) As Integer
    End Function

    <DllImport("dwmapi.dll")>
    Private Shared Function DwmExtendFrameIntoClientArea(hWnd As IntPtr, ByRef pMarInset As MARGINS) As Integer
    End Function

    Public Const WM_NCLBUTTONDOWN As Integer = &HA1
    Public Const HT_CAPTION As Integer = &H2

    <DllImportAttribute("user32.dll")>
    Public Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
    End Function

    <DllImportAttribute("user32.dll")>
    Public Shared Function ReleaseCapture() As Boolean
    End Function
    Private Property pageready As Boolean = False

#Region "Page Loading Functions"
    Private Sub WaitForPageLoad()
        AddHandler WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        While Not pageready
            Application.DoEvents()
        End While
        pageready = False
    End Sub

    Private Sub PageWaiter(ByVal sender As Object, ByVal e As WebBrowserDocumentCompletedEventArgs)
        If WebBrowser1.ReadyState = WebBrowserReadyState.Complete Then
            pageready = True
            RemoveHandler WebBrowser1.DocumentCompleted, New WebBrowserDocumentCompletedEventHandler(AddressOf PageWaiter)
        End If
    End Sub

#End Region
    Dim NomoreDocComp As Boolean = False
    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles WebBrowser1.DocumentCompleted
        If NomoreDocComp = False Then

            Label2.Text = "Ready"
            If Not My.Computer.FileSystem.FileExists(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang") Then

                If My.Computer.Network.IsAvailable Then

                    CreateNewProjectForm.fs = File.Create(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang")
                    CreateNewProjectForm.fs.Close()

                    Label2.Text = CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang  - File Successfully Created [✓]."
                    Label2.Refresh()

                    UpdateForm.ShowDialog()
                Else
                    Label2.Text = CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang  - File Successfully Created [X]."
                    Label2.Refresh()
                    Using New Centered_MessageBox(Me)
                        MsgBox("LuaKeywords.pLang Doesnt Exist and in order to Download" & vbNewLine &
                               "this file you need Internet Connection. Try to fix the" & vbNewLine &
                               "Internet Connection and open the IDE again.", MsgBoxStyle.Critical, "LuaKeywords.pLang Doesnt Exist.")
                    End Using
                    Me.Close()
                End If

            End If

            NomoreDocComp = True
        End If
        Label2.Text = "Ready"
    End Sub
    Public Sub CreateAutoComplete()
        Dim Start As Boolean = False
        Dim TextforList As String = String.Empty

        Label2.Text = "Reading LuaKeywords..."
        For Each line As String In File.ReadAllLines(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang")


            If Not line.Trim = String.Empty Then

                If line = "}" Then Start = False : PlangFunctions.Add(TextforList)

                If Start = True Then
                    If line.Trim.StartsWith("iMgIndXNuMinT=") Then
                        TextforList &= line.Trim.Replace("iMgIndXNuMinT=", "") & "@%"
                    Else
                        TextforList &= line.Trim
                    End If
                End If

                If line = "{" Then Start = True
                If Start = False Then If Not line = "}" Then Label2.Text = "Reading LuaKeywords: " & line : Label2.Refresh() : TextforList = line.Trim & "@%" ': Console.WriteLine(line)


            End If
        Next
        Label2.Text = "Ready"
        'MsgBox(1)
    End Sub
    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Label2.Text = ""

        If Not My.Computer.FileSystem.DirectoryExists(CurrentHardDriver & "Love2DStudio") Then My.Computer.FileSystem.CreateDirectory(CurrentHardDriver & "Love2DStudio") : Label2.Text = CurrentHardDriver & "Love2DStudio  - Successfully Created [✓]."
        If Not My.Computer.FileSystem.DirectoryExists(CurrentHardDriver & "Love2DStudio\Languages") Then My.Computer.FileSystem.CreateDirectory(CurrentHardDriver & "Love2DStudio\Languages") : Label2.Text = CurrentHardDriver & "Love2DStudio\Languages  - Successfully Created [✓]."
        If Not My.Computer.FileSystem.DirectoryExists(CurrentHardDriver & "Love2DStudio\Settings") Then My.Computer.FileSystem.CreateDirectory(CurrentHardDriver & "Love2DStudio\Settings") : Label2.Text = CurrentHardDriver & "Love2DStudio\Settings  - Successfully Created [✓]."
        If Not My.Computer.FileSystem.DirectoryExists(CurrentHardDriver & "Love2DStudio\Projects") Then My.Computer.FileSystem.CreateDirectory(CurrentHardDriver & "Love2DStudio\Projects") : Label2.Text = CurrentHardDriver & "Love2DStudio\Projects  - Successfully Created [✓]."



        'AddHandler Me.fstclrtxtbox.AutoIndentNeeded, New EventHandler(Of AutoIndentEventArgs)(AddressOf Me.fctb_AutoIndentNeeded)

        Label2.Text = "Setting up FileDialog."

        OpenFileDialog1.InitialDirectory = CurrentHardDriver & "Love2DStudio\Projects"
        OpenFileDialog1.Filter = "Lua Project Files |*.LuaProj"
        OpenFileDialog1.RestoreDirectory = True
        OpenFileDialog1.Title = "Select Project File."



        If My.Computer.FileSystem.FileExists(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang") Then ToolStrip2.Refresh() : Panel1.Refresh() : ToolStrip1.Refresh() : PictureBox1.Refresh() : CreateAutoComplete()

        Label2.Text = "Main Tab is loading... ""https://love2d.org/wiki/love"" site. "

        Dim WebBrowser1 As New WebBrowser
        AddHandler WebBrowser1.DocumentCompleted, AddressOf WebBrowser1_DocumentCompleted
        With WebBrowser1
            .Size = TabControl1.TabPages.Item(0).Size - New Size(1, 1)
            .Navigate("https://love2d.org/wiki/love")
            .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right Or AnchorStyles.Left
        End With


        TabControl1.TabPages.Item(0).Controls.Add(WebBrowser1)

    End Sub
    Private Sub Form1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown, Panel1.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0)
        End If
    End Sub


    Dim LuaCommentRegex1 As Regex, LuaCommentRegex2 As Regex, LuaCommentRegex3 As Regex

    Dim LuaKeywordRegex As Regex
    Dim LuaLoveKeywordRegex As Regex
    Dim LuaNumberRegex As Regex
    Dim LuaStringRegex As Regex
    Dim LuaFunctionsRegex As Regex

    Dim platformType As Platform
    Public Shared ReadOnly Property RegexCompiledOption() As RegexOptions
        Get
            If Form1.platformType = Platform.X86 Then
                Return RegexOptions.Compiled
            Else
                Return RegexOptions.None
            End If
        End Get
    End Property
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        Me.Size = New Size(980, 548)
        Panel1.Size = New Size(978, 522)
        TabControl1.Size = New Size(980, 433)

        PictureBox1.Location = New Point(8, 7)
        ToolStrip1.Location = New Point(1, 37)
        ToolStrip2.Location = New Point(1, 62)
        TabControl1.Location = New Point(0, 90)
        Button3.Location = New Point(864, -2)
        Button2.Location = New Point(902, -2)
        Button1.Location = New Point(940, -2)
        Label1.Location = New Point(40, 15)
        Label2.Location = New Point(2, 530)

        Button3.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Button2.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Button1.Anchor = AnchorStyles.Top Or AnchorStyles.Right



        TabControl1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        ToolStripTextBox2.MaxLength = 5
        ToolStripCombobox1.MaxDropDownItems = 5

        If My.Settings.DelayedFoldInterval = "" Then My.Settings.DelayedFoldInterval = "5"

        ToolStripTextBox2.Text = My.Settings.DelayedFoldInterval

        platformType = fstclrtxtbox.SyntaxHighlighter.platformType
        'MsgBox(platformType.ToString)
        LuaStringRegex = New Regex("""""|''|"".*?[^\\]""|'.*?[^\\]'", RegexCompiledOption)
        LuaCommentRegex1 = New Regex("--.*$", RegexOptions.Multiline Or RegexCompiledOption)
        LuaCommentRegex2 = New Regex("(--\[\[.*?\]\])|(--\[\[.*)", RegexOptions.Singleline Or RegexCompiledOption)
        LuaCommentRegex3 = New Regex("(--\[\[.*?\]\])|(.*\]\])", RegexOptions.Singleline Or RegexOptions.RightToLeft Or RegexCompiledOption)
        LuaNumberRegex = New Regex("\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b", RegexCompiledOption)
        LuaLoveKeywordRegex = New Regex("\b(love)\b", RegexCompiledOption)
        LuaKeywordRegex = New Regex("\b(and|break|do|else|elseif|end|false|for|function|if|in|local|nil|not|or|repeat|return|then|true|until|while)\b", RegexCompiledOption)

        LuaFunctionsRegex = New Regex("\b(assert|collectgarbage|dofile|error|getfenv|getmetatable|ipairs|loadfile|loadstring|module|next|pairs|pcall|print|rawequal|rawget|rawset|require|select|setfenv|setmetatable|tonumber|tostring|type|unpack|xpcall)\b", RegexCompiledOption)


        Me.CenterToScreen()
        TabControl1.TabPages.Clear()
        TabControl1.TabPages.Add("Main Tab")


        If My.Computer.FileSystem.FileExists(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang") Then ' Only For Old Users Of this studio , for updating keywordlist lol :P ....
            If Not My.Computer.FileSystem.ReadAllText(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang").StartsWith("love" & vbNewLine & "{" & vbNewLine & "iMgIndXNuMinT=8" & vbNewLine & " [0.0.0.4]" & vbNewLine & "}") Then
                My.Computer.FileSystem.DeleteFile(CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang")
            End If
        End If


            ToolStripTextBox1.Text = My.Settings.lovedir

        If Not My.Computer.FileSystem.FileExists(My.Settings.lovedir & "love.exe") Then
            Using New Centered_MessageBox(Me)
                MsgBox("love.exe Not Found please go to tool>lovedir" & vbNewLine &
                       "and set the love.exe directory in order to" & vbNewLine &
                       "start projects succsefully.", MsgBoxStyle.Exclamation, My.Settings.lovedir & "love.exe Not Found")
            End Using
        End If


    End Sub
    Private Const HTCAPTION As Integer = 2
    Private Const HTLEFT As Integer = 10
    Private Const HTRIGHT As Integer = 11
    Private Const HTTOP As Integer = 12
    Private Const HTTOPLEFT As Integer = 13
    Private Const HTTOPRIGHT As Integer = 14
    Private Const HTBOTTOM As Integer = 15
    Private Const HTBOTTOMLEFT As Integer = 16
    Private Const HTBOTTOMRIGHT As Integer = 17
    Private Const WM_NCHITTEST As Integer = &H84

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_NCHITTEST Then
            Dim pt As New Point(m.LParam.ToInt32)
            pt = Me.PointToClient(pt)
            If pt.X < 5 AndAlso pt.Y < 5 Then
                m.Result = New IntPtr(HTTOPLEFT)
            ElseIf pt.X > (Me.Width - 5) AndAlso pt.Y < 5 Then
                m.Result = New IntPtr(HTTOPRIGHT)
            ElseIf pt.Y < 5 Then
                m.Result = New IntPtr(HTTOP)
            ElseIf pt.X < 5 AndAlso pt.Y > (Me.Height - 5) Then
                m.Result = New IntPtr(HTBOTTOMLEFT)
            ElseIf pt.X > (Me.Width - 5) AndAlso pt.Y > (Me.Height - 5) Then
                m.Result = New IntPtr(HTBOTTOMRIGHT)
            ElseIf pt.Y > (Me.Height - 5) Then
                m.Result = New IntPtr(HTBOTTOM)
            ElseIf pt.X < 5 Then
                m.Result = New IntPtr(HTLEFT)
            ElseIf pt.X > (Me.Width - 5) Then
                m.Result = New IntPtr(HTRIGHT)
            Else
                MyBase.WndProc(m)
            End If
        Else
            MyBase.WndProc(m)
        End If
    End Sub

    Public Structure MARGINS
        Public Left, Right, Top, Bottom As Integer
    End Structure

    Private Function CreateDropShadow() As Boolean
        Try
            Dim val As Integer = 2
            Dim ret1 As Integer = DwmSetWindowAttribute(Me.Handle, 2, val, 4)

            If ret1 = 0 Then
                Dim m As New MARGINS

                m.Left = 0
                m.Right = 0
                m.Top = 0
                m.Bottom = 1

                Dim ret2 As Integer = DwmExtendFrameIntoClientArea(Me.Handle, m)
                Return ret2 = 0
            Else
                Return False
            End If
        Catch ex As Exception
            MsgBox(" Probably dwmapi.dll not found (incompatible OS) " & vbNewLine & ex.ToString)
            ' Probably dwmapi.dll not found (incompatible OS)
            Return False
        End Try
    End Function

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        CreateDropShadow()
        MyBase.OnHandleCreated(e)
    End Sub




    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles Button1.Click ' Closes Form1
        Me.Close()

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If Me.WindowState = FormWindowState.Maximized Then
            Me.WindowState = FormWindowState.Normal
        Else
            Me.WindowState = FormWindowState.Maximized
        End If
    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click ' Minimize Form1
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Dim blackPen As New Pen(Color.FromArgb(0, 83, 156), 1)
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ' Create rectangle. 
        Dim rect As New Rectangle(0, Me.Height - 1, Me.Width, 1)
        ' Draw rectangle to screen.
        Me.CreateGraphics.DrawRectangle(blackPen, rect)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click

    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click

    End Sub

    Private Sub NewProjectToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewProjectToolStripMenuItem.Click
        Label2.Text = "Opening CreateNewProject From..."
        CreateNewProjectForm.ShowDialog()
    End Sub
    Friend WithEvents Aspliter As New Splitter
    Friend WithEvents ADocumentMap As New DocumentMap
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Label2.Text = "Opening FileDialog at """ & CurrentHardDriver & "Love2DStudio\Projects"""
        OpenFileDialog1.InitialDirectory = CurrentHardDriver & "Love2DStudio\Projects"
        OpenFileDialog1.Filter = "Lua Project Files |*.LuaProj"
        OpenFileDialog1.RestoreDirectory = True
        OpenFileDialog1.Title = "Select Project File."
        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            '   Try
            If Not OpenFileDialog1.FileName.StartsWith(CurrentHardDriver & "Love2DStudio\Projects\") Then
                Using New Centered_MessageBox(Me)
                    MsgBox("Copy or move your project at this location:" & vbNewLine & vbNewLine & CurrentHardDriver & "Love2DStudio\Projects\" & vbNewLine & "In order to be opened.", MsgBoxStyle.Information)
                End Using
                Label2.Text = "Ready"
                Exit Sub
            Else
                ToolStripCombobox1.Text = ""
                ToolStripCombobox1.Items.Clear()


                ToolStripComboBox2.Text = ""
                ToolStripComboBox2.Items.Clear()
                'MsgBox(1)
                ToolStripButton14.PerformClick()
                TabControl1.TabPages.Clear()
                Label1.Text = OpenFileDialog1.SafeFileName.Replace(".LuaProj", "") & " - Lua LÖVE 2D Studio"
                CurrentProject = OpenFileDialog1.SafeFileName.Replace(".LuaProj", "")
                'MsgBox(OpenFileDialog1.SafeFileName.Replace(".LuaProj", ""))

                For Each line As String In File.ReadAllLines(CurrentHardDriver & "Love2DStudio\Projects\" & OpenFileDialog1.SafeFileName.Replace(".LuaProj", "") & "\" & OpenFileDialog1.SafeFileName)
                    ' MsgBox(line)
                    If Not line.Trim = "" Then
                        ' MsgBox(line)
                        TabControl1.TabPages.Add(line)
                        ToolStripComboBox2.Items.Add(line)

                        Dim splitter1 As New Splitter
                        Aspliter = splitter1
                        splitter1.Dock = System.Windows.Forms.DockStyle.Right
                        splitter1.Name = "splitter1"
                        splitter1.TabIndex = 3
                        splitter1.TabStop = False
                        splitter1.BorderStyle = BorderStyle.FixedSingle
                        splitter1.Hide()

                        Dim Fstclrtxtbox1 As New FastColoredTextBox
                        With Fstclrtxtbox1
                            .Font = New Font("Consolas", CSng(11))
                            '.Language = Language.Lua
                            .Language = Language.Custom
                            .CommentPrefix = "--"
                            .Location = New Point(0, 0)
                            .ClearUndo()
                            .Dock = DockStyle.Fill
                            .Cursor = Cursors.IBeam
                            .BackColor = Color.White
                            .ForeColor = Color.Black
                            .BorderStyle = BorderStyle.FixedSingle
                            .LineNumberColor = Color.Black
                            .IndentBackColor = Color.FromArgb(230, 230, 230)
                            .SelectionColor = Color.LightBlue
                            .ServiceLinesColor = Color.Gray
                            .ShowFoldingLines = True
                            .Paddings = New System.Windows.Forms.Padding(0)
                            .BringToFront()
                            .DelayedEventsInterval = My.Settings.DelayedFoldInterval
                            .CaretColor = Color.Black
                            .CurrentLineColor = Color.Gray
                            .ImeMode = Windows.Forms.ImeMode.On 'Windows.Forms.ImeMode.On  'System.Globalization.ChineseLunisolarCalendar.ChineseEra
                            .Cursor = Cursors.IBeam
                        End With

                        Dim documentMap1 As New DocumentMap
                        ADocumentMap = documentMap1
                        documentMap1.Dock = System.Windows.Forms.DockStyle.Right
                        documentMap1.BackColor = Color.White
                        documentMap1.ForeColor = Color.FromArgb(0, 83, 156)
                        documentMap1.Name = "documentMap1"
                        documentMap1.TabIndex = 1
                        documentMap1.Target = Fstclrtxtbox1
                        documentMap1.Text = "documentMap1"
                        documentMap1.Size = New Size(184, TabControl1.Size.Height)
                        documentMap1.Hide()
                        ' documentMap1.ForeColor = Color.Black

                        For i = 0 To TabControl1.TabPages.Count - 1
                            If TabControl1.TabPages.Item(i).Text = line Then

                                TabControl1.TabPages.Item(i).Controls.Add(splitter1)
                                TabControl1.TabPages.Item(i).Controls.Add(Fstclrtxtbox1)
                                TabControl1.TabPages.Item(i).Controls.Add(documentMap1)

                                TabControl1.SelectTab(i)
                                Exit For
                            End If
                        Next
                        ' documentMap1.BringToFront()
                        'Fstclrtxtbox1.Size = TabControl1.TabPages.Item(0).Size
                        fstclrtxtbox = Fstclrtxtbox1
                        Label2.Text = "Ready"
                        ' splitter1.BringToFront()



                        popupMenu = New AutocompleteMenu(fstclrtxtbox)
                        popupMenu.SearchPattern = "[\w\.:=!<>]"
                        popupMenu.AppearInterval = 1
                        popupMenu.ToolTipDuration = 11000
                        popupMenu.MinFragmentLength = 2
                        popupMenu.ImageList = ImageList1

                        fstclrtxtbox.Select()
                        BuildAutocompleteMenu()

                        'fstclrtxtbox.OnTextChangedDelayed(fstclrtxtbox.Range)
                        fstclrtxtbox.Text = My.Computer.FileSystem.ReadAllText(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & line)

                        fstclrtxtbox.ClearUndo()


                        LuaSyntaxHighlight(fstclrtxtbox.VisibleRange)
                    End If
                Next
            End If
        End If

        Label2.Text = "Ready"
    End Sub

    Private Sub ToolStripButton11_Click(sender As Object, e As EventArgs) Handles ToolStripButton11.Click ' Navigate Editor Backward
        If Not fstclrtxtbox Is Nothing Then fstclrtxtbox.NavigateBackward()
    End Sub
    Private Sub ToolStripButton12_Click(sender As Object, e As EventArgs) Handles ToolStripButton12.Click
        If Not fstclrtxtbox Is Nothing Then fstclrtxtbox.NavigateForward()
    End Sub
    Private Sub ToolStripButton17_Click(sender As Object, e As EventArgs) Handles ToolStripButton17.Click
        If Not fstclrtxtbox Is Nothing Then
            fstclrtxtbox.Undo()
        ElseIf TabControl1.TabCount = 1 Then
            If TabControl1.TabPages.Item(0).Text = "Main Tab" Then

            End If
        End If

    End Sub
    Private Sub ToolStripButton18_Click(sender As Object, e As EventArgs) Handles ToolStripButton18.Click
        If Not fstclrtxtbox Is Nothing Then
            fstclrtxtbox.Redo() ': fstclrtxtbox.Refresh()
        ElseIf TabControl1.TabCount = 1 Then
            If TabControl1.TabPages.Item(0).Text = "Main Tab" Then
                'WebBrowser1.GoBack()
            End If
        End If
        'Parallel.[For](0, 100, Sub(i As Integer) Console.WriteLine(i & " " & TID))
    End Sub
    'Public Shared ReadOnly Property TID() As String
    '    Get
    '        Return "TID = " & System.Threading.Thread.CurrentThread.ManagedThreadId.ToString()
    '    End Get
    'End Property
    'Public Delegate Sub DelegateFor(i As Integer)
    'Public Delegate Sub DelegateProcess() ' I was trying to make it faster lol 
    'Public Class Parallel
    '    Public Shared Sub [For](from As Integer, [to] As Integer, delFor As DelegateFor)
    '        'step or chunck will be done as 4 by 4
    '        Dim [step] As Integer = 4
    '        Dim count As Integer = from - [step]
    '        Dim cntMem As Integer
    '        Dim processCount As Integer = Environment.ProcessorCount

    '        'Now let's take the next chunk 
    '        Dim process As DelegateProcess = Sub() End
    '        Dim iter As Integer = 0
    '        While True
    '            SyncLock GetType(Parallel)
    '                count += [step]
    '                cntMem = count
    '            End SyncLock
    '            For i As Integer = iter To iter + ([step] - 1)
    '                If i >= [to] Then
    '                    Return
    '                End If
    '                delFor(i)
    '            Next
    '        End While

    '        'IAsyncResult array to launch Thread(s)
    '        Dim asyncResults As IAsyncResult() = New IAsyncResult(processCount - 1) {}
    '        For i As Integer = 0 To processCount - 1
    '            asyncResults(i) = process.BeginInvoke(Nothing, Nothing)
    '        Next
    '        'EndInvoke to wait for all threads to be completed
    '        For i As Integer = 0 To 0 'threadCount - 1
    '            process.EndInvoke(asyncResults(i))
    '        Next
    '    End Sub
    'End Class
    Private Sub fstclrtxtbox_TextChanged(sender As Object, e As TextChangedEventArgs) Handles fstclrtxtbox.TextChanged
        If Regex.IsMatch(fstclrtxtbox.GetLineText(fstclrtxtbox.Selection.Start.iLine).Trim(), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/") AndAlso Regex.Replace(fstclrtxtbox.GetLineText(fstclrtxtbox.Selection.Start.iLine), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Length < fstclrtxtbox.Selection.Start.iChar Then ' _
            'Or Regex.IsMatch(fstclrtxtbox.GetLineText(fstclrtxtbox.Selection.Start.iLine).Trim(), "(?:(\""|\')(.*?[^\\\\])\1)") AndAlso Regex.Replace(fstclrtxtbox.GetLineText(fstclrtxtbox.Selection.Start.iLine), "(?:(\""|\')(.*?[^\\\\])\1)", "$1").Length < fstclrtxtbox.Selection.Start.iChar Then ' I dont Have Any Idea how it works xD  
            'Console.WriteLine(Regex.Replace(fstclrtxtbox.GetLineText(fstclrtxtbox.Selection.Start.iLine), "(""|')((?:\\\1|(?:(?!\1).))*)\1", "$1").Length)
            If popupMenu.Enabled = True Then popupMenu.Enabled = False
        Else
            If Not popupMenu Is Nothing Then popupMenu.Enabled = True
        End If


        '    If FoldingB = True Then

        '        e.ChangedRange.ClearFoldingMarkers()
        '        e.ChangedRange.SetFoldingMarkers("\b(function|for)[ \t]+[^\s']+", "\bend\b", RegexOptions.IgnoreCase)
        '        e.ChangedRange.SetFoldingMarkers("^\s*(?<range>(if|while))\b", "^\s*(?<range>end)\b", RegexOptions.IgnoreCase)
        'e.ChangedRange.SetFoldingMarkers("{", "}", RegexOptions.IgnoreCase)
        '        e.ChangedRange.BeginUpdate()
        '    Else
        '        e.ChangedRange.ClearFoldingMarkers()
        '    End If
        '    '"^\s*(?<range>For|For\s+Each)\b", @"^\s*(?<range>Next)\b"
    End Sub
    ''Private Sub fctb_TextChangedDelayed(sender As Object, e As TextChangedEventArgs) Handles fstclrtxtbox.TextChangedDelayed
    ''    fstclrtxtbox.Range.ClearFoldingMarkers()
    ''    Dim currentIndent As Integer = 0
    ''    Dim lastNonEmptyLine As Integer = 0
    ''    For i As Integer = 0 To fstclrtxtbox.LinesCount - 1
    ''        Dim line As Line = fstclrtxtbox(i)
    ''        Dim spacesCount As Integer = line.StartSpacesCount
    ''        If spacesCount <> line.Count Then
    ''            If currentIndent <= spacesCount Then
    ''                If fstclrtxtbox.GetLineText(lastNonEmptyLine).Trim.StartsWith("function") Or fstclrtxtbox.GetLineText(lastNonEmptyLine).Trim.StartsWith("sub") Then

    ''                    fstclrtxtbox(lastNonEmptyLine).FoldingStartMarker = "m" + currentIndent.ToString()

    ''                End If
    ''            Else
    ''                If currentIndent > spacesCount Then
    ''                    'If fstclrtxtbox.GetLineText(lastNonEmptyLine).Trim.StartsWith("end") Then
    ''                    fstclrtxtbox(lastNonEmptyLine).FoldingEndMarker = "m" + spacesCount.ToString()
    ''                    'Console.WriteLine(fstclrtxtbox.GetLineText(lastNonEmptyLine))
    ''                    ' End If
    ''                End If
    ''            End If
    ''            currentIndent = spacesCount
    ''            lastNonEmptyLine = i
    ''        End If
    ''    Next
    ''End Sub

    Private Sub fctb_TextChangedDelayed(sender As Object, e As TextChangedEventArgs) Handles fstclrtxtbox.TextChangedDelayed
        Try
            Dim FunctionList As New List(Of String)
            Dim iflist As New List(Of Integer)
            Dim endlist As New List(Of Integer)
            'Dim stopwatch As New Stopwatch()
            'stopwatch.Start(

            fstclrtxtbox.Range.ClearFoldingMarkers()





            Parallel.For(0, fstclrtxtbox.LinesCount - 1,
                Sub(i As Integer)
                    'Dim str As String = Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1")

                    If fstclrtxtbox.GetLineText(i).Trim.StartsWith("if") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                       fstclrtxtbox.GetLineText(i).Trim.StartsWith("function") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                       fstclrtxtbox.GetLineText(i).Trim.StartsWith("local function") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                       fstclrtxtbox.GetLineText(i).Trim.StartsWith("while") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                       fstclrtxtbox.GetLineText(i).Trim.StartsWith("for") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Then
                        iflist.Add(i)

                        If fstclrtxtbox.GetLineText(i).Trim.StartsWith("function") Or fstclrtxtbox.GetLineText(i).Trim.StartsWith("local function") Then
                            FunctionList.Add(fstclrtxtbox.GetLineText(i).Trim)
                        End If
                        'spaces1 = fstclrtxtbox(i).StartSpacesCount
                    ElseIf fstclrtxtbox.GetLineText(i).Trim.StartsWith("end") Then 'AndAlso fstclrtxtbox(i).StartSpacesCount = spaces1 Then
                        endlist.Add(i)
                        'spaces1 = 0
                    End If
                End Sub)
            'stopwatch.[Stop]()
            'Console.WriteLine("Sequential loop time in milliseconds: {0}", stopwatch.ElapsedMilliseconds)


            If Not iflist.Count = 0 AndAlso Not endlist.Count = 0 Then ''  AndAlso iflist.Count = endlist.Count this is the reason for try because i find some errors
                For i As Integer = 0 To iflist.Count - 1
                    ' MsgBox(iflist(i) & " " & endlist(iflist.Count - 1 - i))
                    Try
                        fstclrtxtbox(iflist(i)).FoldingStartMarker = "m" + fstclrtxtbox(iflist(i)).StartSpacesCount.ToString
                        fstclrtxtbox(endlist(iflist.Count - 1 - i)).FoldingEndMarker = "m" + fstclrtxtbox(endlist(iflist.Count - 1 - i)).StartSpacesCount.ToString
                    Catch ex As Exception
                        ' MsgBox(ex.ToString)
                    End Try

                Next
            End If

            Dim newFuncorChanged As Boolean = False
            For i As Integer = 0 To FunctionList.Count - 1
                For Each item As String In ToolStripCombobox1.Items.ToString
                    'Console.WriteLine(item)
                    If Not item = FunctionList.Item(i) Then newFuncorChanged = True : Exit For
                Next
                If newFuncorChanged = True Then Exit For
            Next

            If newFuncorChanged = True Then
                ToolStripCombobox1.Items.Clear()
                For i As Integer = 0 To FunctionList.Count - 1
                    ToolStripCombobox1.Items.Add(FunctionList.Item(i))
                Next
            End If
            fstclrtxtbox.Range.SetFoldingMarkers("{", "}")
            fstclrtxtbox.Range.SetFoldingMarkers("--\[\[", "\]\]")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click ' About
        AboutForm.ShowDialog()
    End Sub
    'Public Sub fctb_TextChanged(sender As Object, e As TextChangedEventArgs)
    '    'e.ChangedRange.ClearFoldingMarkers()
    '    'e.ChangedRange.SetFoldingMarkers("function", "end")
    '    ' e.ChangedRange.SetFoldingMarkers(" if", "end")
    '    ' e.ChangedRange.SetFoldingMarkers("while", "end", "\n\bend?\b", RegexOptions.IgnoreCase)
    '    '  e.ChangedRange.SetFoldingMarkers("for", "end", "\n\bend?\b", RegexOptions.IgnoreCase)
    'End Sub



    'HERE TEXTBOX3 ,... cheackstrings() i know i could use keyworks.Lowercase also but ..... i know :P !

    'Private popupMenu2 As AutocompleteMenu
    Dim keywords As String() = {"and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in", "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"}
    Dim snippets As String() = {"then" & vbLf & "^" & vbLf & "end", "if^ then" & vbLf & vbLf & "end", "while^ do" & vbLf & vbLf & "end"}
    Dim declarationSnippets As String() = {"function^()" & vbLf & "end"}
    Dim PlangFunctions As New List(Of String)

    Public popupMenu As AutocompleteMenu
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub BuildAutocompleteMenu()

        Dim items As New List(Of AutocompleteItem)()


        'For Each item As String In ApplicationList.ToArray
        '    items.Add(New MethodAutocompleteItem2(item)) 'With {.ImageIndex = CInt(Regex.Split(item, EndBracket)(1)), .ToolTipTitle = Regex.Split(item, EndBracket)(2), .ToolTipText = Regex.Split(item, EndBracket)(3)})
        'Next

        For Each item As String In PlangFunctions.ToArray
            items.Add(New MethodAutocompleteItem2(Regex.Split(item, "@%")(0)) With {.ToolTipTitle = Regex.Split(item, "@%")(0), .ImageIndex = Regex.Split(item, "@%")(1), .ToolTipText = Regex.Split(item, "@%")(2)})
        Next

        For Each item As String In snippets
            items.Add(New SnippetAutocompleteItem(item)) ' With {.ImageIndex = 1})
        Next
        For Each item As String In declarationSnippets
            items.Add(New DeclarationSnippet(item)) 'With {.ImageIndex = 0})
        Next

        For Each item As String In keywords
            items.Add(New AutocompleteItem(item))
        Next

        items.Add(New InsertSpaceSnippet())
        items.Add(New InsertSpaceSnippet("^(\w+)([=<>!: ]+)(\w+)$"))
        items.Add(New InsertEnterSnippet())


        'set as autocomplete source
        popupMenu.Items.SetAutocompleteItems(items)
        'popupMenu.ToolTip.AutoPopDelay = 10000
        'popupMenu.ToolTip.ReshowDelay = 1000
        'popupMenu.ShowItemToolTips = True
    End Sub
    Public Class MethodAutocompleteItem2
        Inherits MethodAutocompleteItem
        Private firstPart As String
        Private lastPart As String

        Public Sub New(text As String)
            MyBase.New(text)
            Dim i = text.LastIndexOf("."c)
            If i < 0 Then
                firstPart = text
            Else
                firstPart = text.Substring(0, i)
                lastPart = text.Substring(i + 1)
            End If
        End Sub

        Public Overrides Function Compare(fragmentText As String) As CompareResult


            Dim i2 As Integer = fragmentText.LastIndexOf("."c)
            If i2 < 0 Then
                If firstPart.ToLower.StartsWith(fragmentText.ToLower) AndAlso String.IsNullOrEmpty(lastPart) Then
                    Return CompareResult.VisibleAndSelected

                End If

            Else

                Dim fragmentFirstPart = fragmentText.Substring(0, i2)
                Dim fragmentLastPart = fragmentText.Substring(i2 + 1)


                If firstPart <> fragmentFirstPart Then
                    Return CompareResult.Hidden
                End If

                If lastPart IsNot Nothing AndAlso lastPart.StartsWith(fragmentLastPart) Then
                    Return CompareResult.VisibleAndSelected
                End If




                If lastPart IsNot Nothing AndAlso lastPart.ToLower().Contains(fragmentLastPart.ToLower()) Then
                    Return CompareResult.Visible

                End If
            End If

            Return CompareResult.Hidden
        End Function

        Public Overrides Function GetTextForReplace() As String
            If lastPart Is Nothing Then
                Return firstPart
            End If

            Return Convert.ToString(firstPart & Convert.ToString(".")) & lastPart
        End Function

        Public Overrides Function ToString() As String
            If lastPart Is Nothing Then
                Return firstPart
            End If

            Return lastPart
        End Function
    End Class
    Private Class DeclarationSnippet
        Inherits SnippetAutocompleteItem
        Public Sub New(ByVal snippet As String)
            MyBase.New(snippet)
        End Sub

        Public Overrides Function Compare(ByVal fragmentText As String) As CompareResult
            Dim pattern = Regex.Escape(fragmentText)
            If Regex.IsMatch(Text, "\b" & pattern, RegexOptions.IgnoreCase) Then
                Return CompareResult.Visible
            End If
            Return CompareResult.Hidden
        End Function
    End Class


    Private Class InsertSpaceSnippet
        Inherits AutocompleteItem
        Private pattern As String

        Public Sub New(ByVal pattern As String)
            MyBase.New("")
            Me.pattern = pattern
        End Sub


        Public Sub New()
            Me.New("^(\d+)([a-zA-Z_]+)(\d*)$")
        End Sub


        Public Function InsertSpaces(ByVal fragment As String) As String
            Dim m = Regex.Match(fragment, pattern)
            If m Is Nothing Then
                Return fragment
            End If
            If m.Groups(1).Value = "" AndAlso m.Groups(3).Value = "" Then
                Return fragment
            End If
            Return (m.Groups(1).Value & " " & m.Groups(2).Value & " " & m.Groups(3).Value).Trim()
        End Function

        Public Overrides Property ToolTipTitle() As String
            Get
                Return Text
            End Get
            Set(ByVal value As String)
            End Set
        End Property
    End Class


    Private Class InsertEnterSnippet
        Inherits AutocompleteItem
        Private enterPlace As Place = Place.Empty

        Public Sub New()
            MyBase.New("[Line break]")
        End Sub

        Public Overrides Function Compare(ByVal fragmentText As String) As CompareResult
            Dim r = Parent.Fragment.Clone()
            While r.Start.iChar > 0
                If r.CharBeforeStart = "}"c Then
                    enterPlace = r.Start
                    Return CompareResult.Visible
                End If

                r.GoLeftThroughFolded()
            End While

            Return CompareResult.Hidden
        End Function

        Public Overrides Function GetTextForReplace() As String
            'extend range
            Dim r As Range = Parent.Fragment
            Dim [end] As Place = r.[End]
            r.Start = enterPlace
            r.[End] = r.[End]
            'insert line break
            Return Environment.NewLine + r.Text
        End Function

        Public Overrides Sub OnSelected(ByVal popupMenu As AutocompleteMenu, ByVal e As SelectedEventArgs)
            MyBase.OnSelected(popupMenu, e)
            If Parent.Fragment.tb.AutoIndent Then
                Parent.Fragment.tb.DoAutoIndent()
            End If
        End Sub

        Public Overrides Property ToolTipTitle() As String
            Get
                Return "Insert line break after '}'"
            End Get
            Set(ByVal value As String)
            End Set
        End Property
    End Class



    Private Sub ToolStripButton10_Click(sender As Object, e As EventArgs) Handles ToolStripButton10.Click
        Using New Centered_MessageBox(Me)
            Beep()
            Select Case MsgBox("You Wanna Donate :DDD !?!?" & vbNewLine & vbNewLine & "PayPal Email:" & vbNewLine & "gxousos@gmail.com", vbYesNo, "--- Donate <3 ---")
                Case MsgBoxResult.Yes
                    Using New Centered_MessageBox(Me)
                        MsgBox("Thanks you :D So much :')!!!", MsgBoxStyle.OkOnly, "gxousos@gmail.com")
                    End Using
                'Shell("cmd /c start https://www.paypal.com", AppWinStyle.Hide)
                Case MsgBoxResult.No
                    Using New Centered_MessageBox(Me)
                        MsgBox("Thanks for your support xD Lol", MsgBoxStyle.OkOnly, "LoL")
                    End Using
            End Select
        End Using
    End Sub

    Private Sub ShowFoldingLinesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowFoldingLinesToolStripMenuItem.Click
        If Not fstclrtxtbox Is Nothing Then
            If ShowFoldingLinesToolStripMenuItem.Text.StartsWith("[✓]") Then
                ShowFoldingLinesToolStripMenuItem.Text = ShowFoldingLinesToolStripMenuItem.Text.Replace("[✓]", "[X]")
                fstclrtxtbox.ShowFoldingLines = False
            Else
                ShowFoldingLinesToolStripMenuItem.Text = ShowFoldingLinesToolStripMenuItem.Text.Replace("[X]", "[✓]")
                fstclrtxtbox.ShowFoldingLines = True
            End If
        End If
    End Sub
    Dim FoldingB As Boolean = True
    Private Sub FoldingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FoldingToolStripMenuItem.Click
        If Not fstclrtxtbox Is Nothing Then
            If FoldingToolStripMenuItem.Text.StartsWith("[✓]") Then
                FoldingToolStripMenuItem.Text = FoldingToolStripMenuItem.Text.Replace("[✓]", "[X]")
                FoldingB = False
            Else
                FoldingToolStripMenuItem.Text = FoldingToolStripMenuItem.Text.Replace("[X]", "[✓]")
                FoldingB = True
            End If
        End If
    End Sub

    Private Sub ToolStripButton15_Click(sender As Object, e As EventArgs) Handles ToolStripButton15.Click
        If Not fstclrtxtbox Is Nothing Then fstclrtxtbox.CommentSelected()
    End Sub

    Private Sub ToolStripButton16_Click(sender As Object, e As EventArgs) Handles ToolStripButton16.Click
        If Not fstclrtxtbox Is Nothing Then fstclrtxtbox.CommentSelected()
    End Sub

    Private Sub ToolStripButton13_Click(sender As Object, e As EventArgs) Handles ToolStripButton13.Click
        If Not fstclrtxtbox Is Nothing Then
            fstclrtxtbox.SaveToFile(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & TabControl1.SelectedTab.Text, Encoding.UTF8)
            Label2.Text = TabControl1.SelectedTab.Text & "  - File Successfully Saved [✓]"
        End If
    End Sub

    Private Sub fstclrtxtbox_Click(sender As Object, e As EventArgs) Handles fstclrtxtbox.Click

        If Label2.Text.EndsWith("  - File Successfully Saved [✓]") Or Label2.Text.EndsWith(") Files Successfully Saved [✓]") Then Label2.Text = "Ready"
    End Sub

    Private Sub AddNewFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddNewFileToolStripMenuItem.Click
        If Not Label1.Text = "- Lua LÖVE 2D Studio" Then CreateNewFormForm.ShowDialog()
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged

        If Not TabControl1 Is Nothing AndAlso TabControl1.TabCount > 1 Then
            Try


                fstclrtxtbox.DelayedEventsInterval = My.Settings.DelayedFoldInterval

                For Each cntrl As Object In TabControl1.SelectedTab.Controls
                    If (cntrl.GetType() Is GetType(FastColoredTextBox)) Then
                        fstclrtxtbox = cntrl
                    End If
                    If (cntrl.GetType() Is GetType(Splitter)) Then
                        Aspliter = cntrl
                    End If
                    If (cntrl.GetType() Is GetType(DocumentMap)) Then
                        ADocumentMap = cntrl
                    End If
                Next
                ToolStripCombobox1.Items.Clear()
                ToolStripCombobox1.Text = ""

                Dim FunctionList As New List(Of String)
                Dim iflist As New List(Of Integer)
                Dim endlist As New List(Of Integer)

                For i As Integer = 0 To fstclrtxtbox.LinesCount - 1


                    If fstclrtxtbox.GetLineText(i).Trim.StartsWith("if") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                    fstclrtxtbox.GetLineText(i).Trim.StartsWith("function") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                    fstclrtxtbox.GetLineText(i).Trim.StartsWith("local function") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                    fstclrtxtbox.GetLineText(i).Trim.StartsWith("while") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Or
                    fstclrtxtbox.GetLineText(i).Trim.StartsWith("for") AndAlso Not Regex.Replace(fstclrtxtbox.GetLineText(i), "(@(?:""[^""]*"")+|""(?:[^""\n\\]+|\\.)*""|'(?:[^'\n\\]+|\\.)*')|--.*|/\*(?s:.*?)\*/", "$1").Trim.EndsWith("end") Then
                        iflist.Add(i)
                        If fstclrtxtbox.GetLineText(i).Trim.StartsWith("function") Or fstclrtxtbox.GetLineText(i).Trim.StartsWith("local function") Then
                            FunctionList.Add(fstclrtxtbox.GetLineText(i).Trim)
                        End If
                        'spaces1 = fstclrtxtbox(i).StartSpacesCount
                    ElseIf fstclrtxtbox.GetLineText(i).Trim.StartsWith("end") Then 'AndAlso fstclrtxtbox(i).StartSpacesCount = spaces1 Then
                        endlist.Add(i)
                        'spaces1 = 0
                    End If


                Next


                If Not iflist.Count = 0 AndAlso Not endlist.Count = 0 Then ''  AndAlso iflist.Count = endlist.Count this is the reason for try because i find some errors
                    For i As Integer = 0 To iflist.Count - 1
                        ' MsgBox(iflist(i) & " " & endlist(iflist.Count - 1 - i))
                        Try
                            fstclrtxtbox(iflist(i)).FoldingStartMarker = "m" + fstclrtxtbox(iflist(i)).StartSpacesCount.ToString
                            fstclrtxtbox(endlist(iflist.Count - 1 - i)).FoldingEndMarker = "m" + fstclrtxtbox(endlist(iflist.Count - 1 - i)).StartSpacesCount.ToString
                        Catch ex As Exception
                        End Try

                    Next
                End If

                Dim newFuncorChanged As Boolean = False
                For i As Integer = 0 To FunctionList.Count - 1
                    For Each item As String In ToolStripCombobox1.Items.ToString
                        'Console.WriteLine(item)
                        If Not item = FunctionList.Item(i) Then newFuncorChanged = True : Exit For
                    Next
                    If newFuncorChanged = True Then Exit For
                Next

                If newFuncorChanged = True Or ToolStripCombobox1.Items.Count = 0 Then
                    For i As Integer = 0 To FunctionList.Count - 1
                        ToolStripCombobox1.Items.Add(FunctionList.Item(i))
                    Next
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub ToolStripButton14_Click(sender As Object, e As EventArgs) Handles ToolStripButton14.Click
        If Not TabControl1 Is Nothing AndAlso TabControl1.TabCount > 1 Then
            Dim a As Integer = 0
            For i = 0 To TabControl1.TabPages.Count - 1
                For Each cntrl As Object In TabControl1.TabPages.Item(i).Controls
                    If (cntrl.GetType() Is GetType(FastColoredTextBox)) Then

                        cntrl.SaveToFile(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & TabControl1.TabPages.Item(i).Text, Encoding.UTF8)
                    End If
                Next
                a += 1
            Next
            Label2.Text = "All(" & a & ") Files Successfully Saved [✓]"
        Else : ToolStripButton13.PerformClick()
        End If
    End Sub

    Private Sub ToolStripTextBox1_TextChanged(sender As Object, e As EventArgs) Handles ToolStripTextBox1.TextChanged
        My.Settings.lovedir = ToolStripTextBox1.Text
        My.Settings.Save()
    End Sub

    Private Sub ToolStrip2_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ToolStrip2.ItemClicked

    End Sub

    Private Sub ToolStripButton19_Click(sender As Object, e As EventArgs) Handles ToolStripButton19.Click
        If Not Label1.Text = "- Lua LÖVE 2D Studio" Then
            Shell("cmd /c TASKKILL /IM love.exe", AppWinStyle.Hide)
            ToolStripButton14.PerformClick()
            Label2.Text = "love.exe is being executed."
            Label2.Refresh()

            Shell("cmd /c """"" & My.Settings.lovedir & "\love.exe"" """ & CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & """", AppWinStyle.Hide)
            Label2.Text = CurrentProject & " is running"
            Label2.Refresh()

            blackPen = New Pen(Color.FromArgb(202, 81, 0), 1)
            Me.BackColor = Color.FromArgb(202, 81, 0)
            Timer2.Start()
        End If
    End Sub

    Private Sub ToolStripButton20_Click(sender As Object, e As EventArgs) Handles ToolStripButton20.Click
        If Not Label1.Text = "- Lua LÖVE 2D Studio" Then
            Shell("cmd /c TASKKILL /IM love.exe", AppWinStyle.Hide)
            blackPen = New Pen(Color.FromArgb(0, 83, 156), 1)
            Me.BackColor = Color.FromArgb(0, 83, 156)
            Timer2.Stop()
        End If
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        Dim boollove As Boolean = False
        For Each p As Process In Process.GetProcesses()
            If p.ProcessName = "love" Then
                boollove = True
            End If
        Next
        If boollove = False Then
            blackPen = New Pen(Color.FromArgb(0, 83, 156), 1)
            Me.BackColor = Color.FromArgb(0, 83, 156)
            Label2.Text = "Ready"
            Timer2.Stop()
        End If
    End Sub

    Private Sub ToolStripDropDownButton1_Click(sender As Object, e As EventArgs) Handles ToolStripCombobox1.Click

    End Sub

    Private Sub ToolStripCombobox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripCombobox1.SelectedIndexChanged

        If fstclrtxtbox Is Nothing Then Exit Sub

        For Each r In fstclrtxtbox.Range.GetRanges(Regex.Replace(ToolStripCombobox1.SelectedItem.ToString, "\([a-zA-Z0-9._, ]+\)", "")) '.Replace(ToolStripCombobox1.SelectedItem.ToString.Substring(ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") + 1, ToolStripCombobox1.SelectedItem.ToString.IndexOf(")", ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") + 1) - ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") - 1))) '.Substring(ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") + 1, ToolStripCombobox1.SelectedItem.ToString.IndexOf(")", ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") + 1) - ToolStripCombobox1.SelectedItem.ToString.IndexOf("(") - 1))
            fstclrtxtbox.Selection = r
            fstclrtxtbox.DoSelectionVisible()
        Next

    End Sub


    Private Sub ToolStripButton24_Click(sender As Object, e As EventArgs) Handles ToolStripButton24.Click

    End Sub


    Private Sub TabControl1_MouseClick(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseClick

    End Sub

    Private Sub TabControl1_MouseDown(sender As Object, e As MouseEventArgs) Handles TabControl1.MouseDown
        If TabControl1.TabPages.Item(0).Text = "Main Tab" Then Exit Sub

        If e.Button = MouseButtons.Right Then
            TabControl1.ContextMenuStrip = ContextMenuStrip1

            For i As Integer = 0 To TabControl1.TabPages.Count - 1

                If TabControl1.GetTabRect(i).Contains(e.X, e.Y) Then
                    With ContextMenuStrip1
                        .Items.Clear()
                        .Items.Add("Close """ & TabControl1.TabPages.Item(i).Text & """").Tag = i
                        .Items.Add("Delete """ & TabControl1.TabPages.Item(i).Text & """").Tag = i
                    End With
                    Exit For
                End If

            Next
        End If
    End Sub

    Private Sub ContextMenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles ContextMenuStrip1.ItemClicked
        If e.ClickedItem.Text.StartsWith("Close") Then

            For Each cntrl As Object In TabControl1.TabPages.Item(e.ClickedItem.Tag).Controls
                If (cntrl.GetType() Is GetType(FastColoredTextBox)) Then

                    cntrl.SaveToFile(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & TabControl1.TabPages.Item(e.ClickedItem.Tag).Text, Encoding.UTF8)
                    Label2.Text = TabControl1.TabPages.Item(e.ClickedItem.Tag).Text & "  - File Successfully Saved [✓]"
                    TabControl1.TabPages.RemoveAt(e.ClickedItem.Tag)

                    Exit For
                End If
            Next
        Else

        End If
    End Sub

    Private Sub ToolStripComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox2.SelectedIndexChanged

        For Each tabpg As TabPage In TabControl1.TabPages
            If tabpg.Text = ToolStripComboBox2.SelectedItem.ToString Then Exit Sub
        Next

        If My.Computer.FileSystem.FileExists(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & ToolStripComboBox2.SelectedItem.ToString) Then


            TabControl1.TabPages.Add(ToolStripComboBox2.SelectedItem.ToString)

            Dim splitter1 As New Splitter
            Aspliter = splitter1
            splitter1.Dock = System.Windows.Forms.DockStyle.Right
            splitter1.Name = "splitter1"
            splitter1.TabIndex = 3
            splitter1.TabStop = False
            splitter1.BorderStyle = BorderStyle.FixedSingle
            splitter1.Hide()

            Dim Fstclrtxtbox1 As New FastColoredTextBox
            With Fstclrtxtbox1
                .Font = New Font("Consolas", CSng(11))
                '.Language = Language.Lua
                .Language = Language.Custom
                .CommentPrefix = "--"
                .Location = New Point(0, 0)
                .ClearUndo()
                .Dock = DockStyle.Fill
                .Cursor = Cursors.IBeam
                .BackColor = Color.White
                .ForeColor = Color.Black
                .BorderStyle = BorderStyle.FixedSingle
                .LineNumberColor = Color.Black
                .IndentBackColor = Color.FromArgb(230, 230, 230)
                .SelectionColor = Color.LightBlue
                .ServiceLinesColor = Color.Gray
                .ShowFoldingLines = True
                .Paddings = New System.Windows.Forms.Padding(0)
                .BringToFront()
                .DelayedEventsInterval = My.Settings.DelayedFoldInterval
                .CaretColor = Color.Black
                .CurrentLineColor = Color.Gray
                .ImeMode = Windows.Forms.ImeMode.On 'Windows.Forms.ImeMode.On  'System.Globalization.ChineseLunisolarCalendar.ChineseEra
                .Cursor = Cursors.IBeam
            End With

            Dim documentMap1 As New DocumentMap
            ADocumentMap = documentMap1
            documentMap1.Dock = System.Windows.Forms.DockStyle.Right
            documentMap1.BackColor = Color.White
            documentMap1.ForeColor = Color.FromArgb(0, 83, 156)
            documentMap1.Name = "documentMap1"
            documentMap1.TabIndex = 1
            documentMap1.Target = Fstclrtxtbox1
            documentMap1.Text = "documentMap1"
            documentMap1.Size = New Size(184, TabControl1.Size.Height)
            documentMap1.Hide()
            ' documentMap1.ForeColor = Color.Black

            For i = 0 To TabControl1.TabPages.Count - 1
                If TabControl1.TabPages.Item(i).Text = ToolStripComboBox2.SelectedItem.ToString Then

                    TabControl1.TabPages.Item(i).Controls.Add(splitter1)
                    TabControl1.TabPages.Item(i).Controls.Add(Fstclrtxtbox1)
                    TabControl1.TabPages.Item(i).Controls.Add(documentMap1)

                    TabControl1.SelectTab(i)
                    Exit For
                End If
            Next


            fstclrtxtbox = Fstclrtxtbox1
            Label2.Text = "Ready"


            popupMenu = New AutocompleteMenu(fstclrtxtbox)
            popupMenu.SearchPattern = "[\w\.:=!<>]"
            popupMenu.AppearInterval = 1
            popupMenu.ToolTipDuration = 11000
            popupMenu.MinFragmentLength = 2
            popupMenu.ImageList = ImageList1

            fstclrtxtbox.Select()
            BuildAutocompleteMenu()

            'fstclrtxtbox.OnTextChangedDelayed(fstclrtxtbox.Range)
            fstclrtxtbox.Text = My.Computer.FileSystem.ReadAllText(CurrentHardDriver & "Love2DStudio\Projects\" & CurrentProject & "\" & ToolStripComboBox2.SelectedItem.ToString)

            fstclrtxtbox.ClearUndo()


            LuaSyntaxHighlight(fstclrtxtbox.VisibleRange)

        End If

    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click

        ToolStripButton14.PerformClick()

        Label1.Text = "- Lua LÖVE 2D Studio"

        ToolStripCombobox1.Text = ""
        ToolStripCombobox1.Items.Clear()


        ToolStripComboBox2.Text = ""
        ToolStripComboBox2.Items.Clear()

        TabControl1.TabPages.Clear()

        CurrentProject = String.Empty

        fstclrtxtbox = Nothing

        TabControl1.TabPages.Add("Main Tab")

        Label2.Text = "Main Tab is loading... ""https://love2d.org/wiki/love"" site. "

        Dim WebBrowser1 As New WebBrowser
        AddHandler WebBrowser1.DocumentCompleted, AddressOf WebBrowser1_DocumentCompleted
        With WebBrowser1
            .Size = TabControl1.TabPages.Item(0).Size - New Size(1, 1)
            .Navigate("https://love2d.org/wiki/love")
            .Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Right Or AnchorStyles.Left
        End With

        TabControl1.TabPages.Item(0).Controls.Add(WebBrowser1)

    End Sub

    Private Sub fstclrtxtbox_MouseDown(sender As Object, e As MouseEventArgs) Handles fstclrtxtbox.MouseDown
        TabControl1.ContextMenuStrip = Nothing

    End Sub

    Private Sub fstclrtxtbox_Scroll(sender As Object, e As ScrollEventArgs) Handles fstclrtxtbox.Scroll
        'fstclrtxtbox.Refresh() ' idk ....
    End Sub

    Private Sub ToolStripButton21_Click(sender As Object, e As EventArgs) Handles ToolStripButton21.Click
        OpenToolStripMenuItem.PerformClick()
    End Sub

    Private Sub ToolStripButton25_Click(sender As Object, e As EventArgs) Handles ToolStripButton25.Click
        If Not Aspliter Is Nothing Then
            If Aspliter.Visible = False Then
                Aspliter.Show()
                ADocumentMap.Show()
            Else
                Aspliter.Hide()
                ADocumentMap.Hide()
            End If
        End If
    End Sub

    Dim BlueBoldStyle As Style = New TextStyle(New SolidBrush(Color.FromArgb(0, 43, 116)), Nothing, FontStyle.Regular)
    Dim BrownStyle As Style = New TextStyle(Brushes.Brown, Nothing, FontStyle.Italic)
    Dim PinkStyle As Style = New TextStyle(New SolidBrush(Color.FromArgb(183, 6, 122)), Nothing, FontStyle.Regular)
    Dim GreenStyle As Style = New TextStyle(Brushes.Green, Nothing, FontStyle.Italic)
    Dim MagentaStyle As Style = New TextStyle(Brushes.SlateBlue, Nothing, FontStyle.Regular)
    Dim MaroonStyle As Style = New TextStyle(Brushes.Maroon, Nothing, FontStyle.Regular)

    '=====================
    Private Sub LuaSyntaxHighlight(range As Range)
        ' Try

        'Console.WriteLine(range.ToLine)
        'clear style of changed range
        range.ClearStyle(New Style() {BrownStyle, GreenStyle, MagentaStyle, BlueBoldStyle, MaroonStyle})



        'string highlighting
        range.SetStyle(BrownStyle, LuaStringRegex)
        'comment highlighting
        range.SetStyle(GreenStyle, LuaCommentRegex1)
        range.SetStyle(GreenStyle, LuaCommentRegex2)
        range.SetStyle(GreenStyle, LuaCommentRegex3)
        'number highlighting
        range.SetStyle(MagentaStyle, LuaNumberRegex)
        'keyword highlighting
        range.SetStyle(BlueBoldStyle, LuaKeywordRegex)
        range.SetStyle(PinkStyle, LuaLoveKeywordRegex)
        'functions highlighting
        range.SetStyle(MaroonStyle, LuaFunctionsRegex)

       ' Catch ex As Exception ' it happens some times lol :P

        'End Try
    End Sub

    Private Sub ToolStripTextBox2_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox2.Click

    End Sub

    Private Sub fstclrtxtbox_VisibleRangeChangedDelayed(sender As Object, e As EventArgs) Handles fstclrtxtbox.VisibleRangeChangedDelayed
        LuaSyntaxHighlight(fstclrtxtbox.VisibleRange)
    End Sub

    Private Sub fstclrtxtbox_AutoIndentNeeded(sender As Object, args As AutoIndentEventArgs) Handles fstclrtxtbox.AutoIndentNeeded

        'block {}
        If Regex.IsMatch(args.LineText, "^[^""']*\{.*\}[^""']*$") Then
                Return
            End If
            'start of block {}
            If Regex.IsMatch(args.LineText, "^[^""']*\{") Then
                args.ShiftNextLines = args.TabLength
                Return
            End If
            'end of block {}
            If Regex.IsMatch(args.LineText, "}[^""']*$") Then
                args.Shift = -args.TabLength
                args.ShiftNextLines = -args.TabLength
                Return
            End If
            'end of block
            If Regex.IsMatch(args.LineText, "^\s*(end|until)\b") Then
                args.Shift = -args.TabLength
                args.ShiftNextLines = -args.TabLength
                Return
            End If
            ' then ...
            If Regex.IsMatch(args.LineText, "\b(then)\s*\S+") Then
                Return
            End If
        'start of operator block
        If Regex.IsMatch(args.LineText, "^\s*(function|local function|do|for|while|repeat|if)\b") Then
            args.ShiftNextLines = args.TabLength
            Return
        End If

        'Statements else, elseif, case etc
        If Regex.IsMatch(args.LineText, "^\s*(else|elseif)\b", RegexOptions.IgnoreCase) Then
                args.Shift = -args.TabLength
                Return
            End If
    End Sub

    Private Sub ToolStripButton9_Click(sender As Object, e As EventArgs) Handles ToolStripButton9.Click

    End Sub

    Private Sub ToolStripTextBox2_TextChanged(sender As Object, e As EventArgs) Handles ToolStripTextBox2.TextChanged
        If ToolStripTextBox2.Text = "0" Then ToolStripTextBox2.Text = "5"
        My.Settings.DelayedFoldInterval = ToolStripTextBox2.Text
        My.Settings.Save()

        If Not fstclrtxtbox Is Nothing Then
            fstclrtxtbox.DelayedTextChangedInterval = ToolStripTextBox2.Text
        End If
    End Sub

    Private Sub ToolStripTextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ToolStripTextBox2.KeyPress
        '97 - 122 = Ascii codes for simple letters
        '65 - 90  = Ascii codes for capital letters
        '48 - 57  = Ascii codes for numbers

        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub ToolStripButton9_Disposed(sender As Object, e As EventArgs) Handles ToolStripButton9.Disposed

    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        ToolStripButton13.PerformClick()
    End Sub

    Private Sub SaveAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAllToolStripMenuItem.Click
        ToolStripButton14.PerformClick()
    End Sub

    Private Sub ToolStripButton9_DropDownClosed(sender As Object, e As EventArgs) Handles ToolStripButton9.DropDownClosed
        If ToolStripTextBox2.Text = "" Then ToolStripTextBox2.Text = "5"
    End Sub

    Private Sub ToolStripTextBox2_MouseDown(sender As Object, e As MouseEventArgs) Handles ToolStripTextBox2.MouseDown

    End Sub
    Dim bcksps As Boolean = True  ' lol 
    Private Sub fstclrtxtbox_KeyDown(sender As Object, e As KeyEventArgs) Handles fstclrtxtbox.KeyDown
        If e.KeyCode = Keys.Back AndAlso bcksps = True Then

            If fstclrtxtbox.Selection.Start.iChar > 0 AndAlso fstclrtxtbox(fstclrtxtbox.Selection.Start.iLine).Text.Substring(0, fstclrtxtbox.Selection.Start.iChar).Trim = "" Then
                bcksps = False
                SendKeys.Send("^({BACKSPACE})")
                SendKeys.Send("{BACKSPACE}")
            End If

        End If
    End Sub

    Private Sub fstclrtxtbox_KeyUp(sender As Object, e As KeyEventArgs) Handles fstclrtxtbox.KeyUp
        bcksps = True
    End Sub





    'Private Sub tabControl1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles TabControl1.DrawItem

    '    Dim g As Graphics = e.Graphics
    '    Dim tp As TabPage = TabControl1.TabPages(e.Index)
    '    Dim br As Brush = Nothing
    '    Dim fn As Font = New Font(TabControl1.Font.Name, TabControl1.Font.Size, FontStyle.Bold)
    '    Dim sf As New StringFormat
    '    Dim r As New RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width, e.Bounds.Height - 2)
    '    Dim strTitle As String = tp.Text

    '    sf.Alignment = StringAlignment.Center


    '            br = New SolidBrush(Color.Red)
    '            g.FillRectangle(br, e.Bounds)
    '            br = New SolidBrush(Color.White)
    '            g.DrawString(strTitle, fn, br, r, sf)
    '  
    'End Sub

End Class



Public Class Centered_MessageBox
    Implements IDisposable
    Private mTries As Integer = 0
    Private mOwner As Form

    Public Sub New(owner As Form)
        mOwner = owner
        owner.BeginInvoke(New MethodInvoker(AddressOf findDialog))
    End Sub

    Private Sub findDialog()
        If mTries < 0 Then
            Return
        End If
        Dim callback As New EnumThreadWndProc(AddressOf checkWindow)
        If EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero) Then
            If System.Threading.Interlocked.Increment(mTries) < 10 Then
                mOwner.BeginInvoke(New MethodInvoker(AddressOf findDialog))
            End If
        End If
    End Sub
    Private Function checkWindow(hWnd As IntPtr, lp As IntPtr) As Boolean
        Dim sb As New StringBuilder(260)
        GetClassName(hWnd, sb, sb.Capacity)
        If sb.ToString() <> "#32770" Then
            Return True
        End If
        Dim frmRect As New Rectangle(mOwner.Location, mOwner.Size)
        Dim dlgRect As RECT
        GetWindowRect(hWnd, dlgRect)
        MoveWindow(hWnd, frmRect.Left + (frmRect.Width - dlgRect.Right + dlgRect.Left) \ 2, frmRect.Top + (frmRect.Height - dlgRect.Bottom + dlgRect.Top) \ 2, dlgRect.Right - dlgRect.Left, dlgRect.Bottom - dlgRect.Top, True)
        Return False
    End Function
    Public Sub Dispose() Implements IDisposable.Dispose
        mTries = -1
    End Sub
    Private Delegate Function EnumThreadWndProc(hWnd As IntPtr, lp As IntPtr) As Boolean
    <DllImport("user32.dll")>
    Private Shared Function EnumThreadWindows(tid As Integer, callback As EnumThreadWndProc, lp As IntPtr) As Boolean
    End Function
    <DllImport("kernel32.dll")>
    Private Shared Function GetCurrentThreadId() As Integer
    End Function
    <DllImport("user32.dll")>
    Private Shared Function GetClassName(hWnd As IntPtr, buffer As StringBuilder, buflen As Integer) As Integer
    End Function
    <DllImport("user32.dll")>
    Private Shared Function GetWindowRect(hWnd As IntPtr, ByRef rc As RECT) As Boolean
    End Function
    <DllImport("user32.dll")>
    Private Shared Function MoveWindow(hWnd As IntPtr, x As Integer, y As Integer, w As Integer, h As Integer, repaint As Boolean) As Boolean
    End Function
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

End Class


