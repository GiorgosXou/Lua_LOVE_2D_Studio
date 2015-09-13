Imports System.IO
Imports FastColoredTextBoxNS

Public Class CreateNewFormForm
    Private Sub CreateNewFormForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Form1.Location + New Point((Form1.Width / 2) - Me.Width / 2, (Form1.Height / 2) - Me.Height / 2)
        TextBox1.Text = ""
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not TextBox1.Text.Trim = "" Then
            If Not CreateNewProjectForm.fs Is Nothing Then CreateNewProjectForm.fs.Close()
            If Not File.Exists(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & Form1.CurrentProject & "\" & TextBox1.Text & ".lua") Then

                Form1.Label2.Text = "Creating """ & TextBox1.Text & """ File"
                Form1.Label2.Refresh()

                CreateNewProjectForm.fs = File.Create(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & Form1.CurrentProject & "\" & TextBox1.Text & ".Lua")
                CreateNewProjectForm.fs.Close()

                My.Computer.FileSystem.WriteAllText(Form1.CurrentHardDriver & "Love2DStudio\Projects\" & Form1.CurrentProject & "\" & Form1.CurrentProject & ".LuaProj", vbNewLine & TextBox1.Text & ".Lua", True)

                Form1.TabControl1.TabPages.Add(TextBox1.Text & ".Lua")
                Form1.ToolStripComboBox2.Items.Add(TextBox1.Text & ".Lua")

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

                For i = 0 To Form1.TabControl1.TabPages.Count - 1
                    If Form1.TabControl1.TabPages.Item(i).Text = TextBox1.Text & ".Lua" Then

                        Form1.TabControl1.TabPages.Item(i).Controls.Add(splitter1)
                        Form1.TabControl1.TabPages.Item(i).Controls.Add(Fstclrtxtbox1)
                        Form1.TabControl1.TabPages.Item(i).Controls.Add(documentMap1)

                        Form1.TabControl1.SelectTab(i)
                        Exit For
                    End If
                Next

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
                Form1.fstclrtxtbox.OnTextChangedDelayed(Form1.fstclrtxtbox.Range)
                Me.Close()
            Else
                Beep()
                Using New Centered_MessageBox(Me)
                    MsgBox("File Aleeady Exist Change the name.", vbInformation, "File Already Exist!")
                End Using
            End If
        Else
            Using New Centered_MessageBox(Me)
                MsgBox("Empty Name", MsgBoxStyle.Information)
            End Using
        End If

    End Sub

    Private Sub CreateNewFormForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Form1.Label2.Text = "Ready"
    End Sub
End Class