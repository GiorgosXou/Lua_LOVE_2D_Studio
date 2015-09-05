Imports System.IO
Imports System.Text.RegularExpressions

Public Class UpdateForm
    Private Sub UpdateForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Form1.Location + New Point((Form1.Width / 2) - Me.Width / 2, (Form1.Height / 2) - Me.Height / 2)
    End Sub
    Private Sub UpdateFor_Close(sender As Object, e As EventArgs) Handles Me.FormClosed
        Form1.CreateAutoComplete()
    End Sub

    Private Sub UpdateForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        UpdateLua()
    End Sub

    Public Sub UpdateLua()
        Form1.Label2.Text = "Updating..."
        Form1.Label2.Refresh()
        Me.Refresh()
        RichTextBox1.Refresh()

        Dim file As StreamWriter
        file = My.Computer.FileSystem.OpenTextFileWriter(Form1.CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang", False)
        file.WriteLine("love" & vbNewLine & "{" & vbNewLine & "iMgIndXNuMinT=8" & vbNewLine & " -" & vbNewLine & "}")
        Dim i As Integer = 1
        file.Close()
        file = My.Computer.FileSystem.OpenTextFileWriter(Form1.CurrentHardDriver & "Love2DStudio\Settings\LuaKeywords.pLang", True)


        Dim site As String = "https://love2d.org/wiki/love"

        Dim Str As String = String.Empty
        Dim SVStr As String = String.Empty
        Dim CommentOfKeyword As String = String.Empty
        Dim Source_Code As String

        Dim FindKeyWord As Boolean = False
        Dim FindKeyWord2 As Boolean = False

        Dim imgIndex As Integer = 0

bck:    Source_Code = New System.Net.WebClient().DownloadString(site)

        FindKeyWord = False
        CommentOfKeyword = String.Empty
        FindKeyWord2 = False

        For Each ln As String In Source_Code.Split(vbLf)
            If Not ln.Trim = "<h2><span class=""mw-headline"" id=""Other_Languages"">Other Languages</span></h2>" And Not ln.Trim = "<h2><span class=""mw-headline"" id=""See_Also"">See Also</span></h2>" Then

                If ln.Trim = "<h2><span class=""mw-headline"" id=""Modules"">Modules</span></h2>" Then imgIndex = 0
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Third-party_modules"">Third-party modules</span></h2>" Then imgIndex = 1
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Functions"">Functions</span></h2>" Then imgIndex = 2
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Types"">Types</span></h2>" Then imgIndex = 3
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Subtypes"">Subtypes</span></h2>" Then imgIndex = 4
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Callbacks"">Callbacks</span></h2>" Then imgIndex = 5
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Enums"">Enums</span></h2>" Then imgIndex = 6
                If ln.Trim = "<h2><span class=""mw-headline"" id=""Constructors"">Constructors</span></h2>" Then imgIndex = 7

                If FindKeyWord = True AndAlso Not CommentOfKeyword.Trim = "" Then CommentOfKeyword &= ln.Trim & vbNewLine
                If FindKeyWord = True AndAlso ln.Trim.StartsWith("<td style=""padding: 1px 5px 1px 5px; background-color: #ffffff; vertical-align: top;"">") Then CommentOfKeyword = ln.Trim.Replace("<td style=""padding: 1px 5px 1px 5px; background-color: #ffffff; vertical-align: top;"">""", "") : FindKeyWord2 = True

                If FindKeyWord = True AndAlso FindKeyWord2 = True AndAlso ln.Trim.EndsWith("</td>") Then Str &= " || " & CommentOfKeyword.Replace("<td style=""padding: 1px 5px 1px 5px; background-color: #ffffff; vertical-align: top;"">", "").Replace("</td>", "") & vbNewLine : FindKeyWord = False : FindKeyWord2 = False
                If ln.Trim.StartsWith("<td style=""padding: 1px 5px 1px 5px; background-color: #ffffff; vertical-align: top;""><a href=""/wiki/") Then Str &= ln.Trim.Replace("<td style=""padding: 1px 5px 1px 5px; background-color: #ffffff; vertical-align: top;""><a href=""/wiki/", "").Split("""")(0) : FindKeyWord = True

            Else
                For Each lnn As String In Str.Split(vbNewLine)
                    lnn = lnn.Trim
                    If Not SVStr.Contains(lnn) Then
                        i += 1
                        SVStr &= lnn & vbNewLine
                        site = "https://love2d.org/wiki/" & lnn.Split("||")(0)
                        file.WriteLine(Regex.Replace(lnn.Replace("||", vbNewLine & "{" & vbNewLine & "iMgIndXNuMinT=" & imgIndex & vbNewLine), "\<[^\>]+\>", "") & vbNewLine & "}")
                        RichTextBox1.AppendText(lnn.Split("||")(0) & vbNewLine)
                        Me.Text = "Updating " & Date.Now.ToString & " [" & i & "/1040]"
                        GoTo bck
                    End If
                Next
            End If
        Next
        file.Close()
        Form1.Label2.Text = "Ready"
    End Sub
End Class