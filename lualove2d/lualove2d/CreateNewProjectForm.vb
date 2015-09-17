Imports System.IO
Imports FastColoredTextBoxNS

Public Class CreateNewProjectForm
    Public fs As FileStream

    Private Sub CreateNewProjectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = New Size(392, 179)
        Me.Location = Form1.Location + New Point((Form1.Width / 2) - Me.Width / 2, (Form1.Height / 2) - Me.Height / 2)
        TextBox1.Text = ""
        TextBox2.Text = ""

        Label1.Location = New Point(12, 20)
        TextBox1.Location = New Point(12, 36)
        Label2.Location = New Point(12, 60)
        TextBox2.Location = New Point(12, 76)
        Button1.Location = New Point(267, 105)


    End Sub

    Private Sub CreateNewProjectForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Form1.Label2.Text = "Ready"
    End Sub
    Private Sub TextBox2_1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox2.KeyUp, TextBox1.KeyUp
        If e.KeyCode = Keys.Enter Then
            Me.Button1.PerformClick()
        End If
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not TextBox1.Text.Trim = "" AndAlso Not TextBox2.Text.Trim = "" Then
            If Not Form1.Label1.Text = "- Lua LÖVE 2D Studio" AndAlso Not fs Is Nothing Then fs.Close()
CrtProject: If Not Directory.Exists(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text) Then

                Form1.ToolStripCombobox1.Text = ""
                Form1.ToolStripCombobox1.Items.Clear()


                Form1.ToolStripComboBox2.Text = ""
                Form1.ToolStripComboBox2.Items.Clear()

                Form1.ToolStripButton14.PerformClick()
                Form1.Label2.Text = "Creating """ & TextBox1.Text & """ Project"

                Directory.CreateDirectory(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text)

                fs = File.Create(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text & "\" & TextBox1.Text & ".LuaProj")
                fs.Close()

                fs = File.Create(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text & "\" & TextBox2.Text & ".Lua")
                Form1.Label1.Text = TextBox1.Text & " - Lua LÖVE 2D Studio"
                fs.Close()

                My.Computer.FileSystem.WriteAllText(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text & "\" & TextBox1.Text & ".LuaProj", TextBox2.Text & ".Lua", False)

                Form1.TabControl1.TabPages.Clear()
                Form1.TabControl1.TabPages.Add(TextBox2.Text & ".Lua")

                Form1.ToolStripComboBox2.Items.Add(TextBox2.Text & ".Lua")

                Form1.CurrentProject = TextBox1.Text

                Dim splitter1 As New Splitter
                Form1.Aspliter = splitter1
                splitter1.Dock = System.Windows.Forms.DockStyle.Right
                splitter1.Name = "splitter1"
                splitter1.TabIndex = 3
                splitter1.TabStop = False
                splitter1.BorderStyle = BorderStyle.FixedSingle
                splitter1.Hide()

                Dim Fstclrtxtbox1 As New FastColoredTextBox
                With Fstclrtxtbox1
                    .Font = New Font("Consolas", CSng(11))
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
                Form1.ADocumentMap = documentMap1
                documentMap1.Dock = System.Windows.Forms.DockStyle.Right
                documentMap1.BackColor = Color.White
                documentMap1.ForeColor = Color.FromArgb(0, 83, 156)
                documentMap1.Name = "documentMap1"
                documentMap1.TabIndex = 1
                documentMap1.Target = Fstclrtxtbox1
                documentMap1.Text = "documentMap1"
                documentMap1.Size = New Size(184, Form1.TabControl1.Size.Height)
                documentMap1.Hide()
                ' documentMap1.ForeColor = Color.Black

                Form1.TabControl1.TabPages.Item(0).Controls.Add(splitter1)
                Form1.TabControl1.TabPages.Item(0).Controls.Add(Fstclrtxtbox1)
                Form1.TabControl1.TabPages.Item(0).Controls.Add(documentMap1)


                Form1.fstclrtxtbox = Fstclrtxtbox1
                Form1.Label2.Text = "Ready"


                Form1.popupMenu = New AutocompleteMenu(Form1.fstclrtxtbox)
                Form1.popupMenu.SearchPattern = "[\w\.:=!<>]"
                Form1.popupMenu.AppearInterval = 1
                Form1.popupMenu.ToolTipDuration = 11000
                Form1.popupMenu.MinFragmentLength = 2
                Form1.popupMenu.ImageList = Form1.ImageList1

                Form1.BuildAutocompleteMenu()
                Form1.fstclrtxtbox.Select()
                'Form1.fstclrtxtbox.OnTextChangedDelayed(Form1.fstclrtxtbox.Range)
                Me.Close()
            Else
                Using New Centered_MessageBox(Me)
                    Beep()
                    Select Case MsgBox("Do You Want To Overwrite Files?", vbYesNo, "File Already Exist!")
                        Case MsgBoxResult.Yes
                            ' Directory.Delete()
                            If Not Form1.Label1.Text = "- Lua LÖVE 2D Studio" AndAlso Not fs Is Nothing Then fs.Close()
                            Form1.Label2.Text = "Deleting """ & TextBox1.Text & """ Project"
                            Form1.Label2.Refresh()
                            My.Computer.FileSystem.DeleteDirectory(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & TextBox1.Text, FileIO.DeleteDirectoryOption.DeleteAllContents)

                            GoTo CrtProject

                        Case MsgBoxResult.No
                    End Select
                End Using
            End If
        Else
            Using New Centered_MessageBox(Me)
                MsgBox("Empty Name", MsgBoxStyle.Information)
            End Using
        End If
    End Sub
End Class