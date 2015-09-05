Public Class AboutForm
    Private Sub AboutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Form1.Location + New Point((Form1.Width / 2) - Me.Width / 2, (Form1.Height / 2) - Me.Height / 2)
    End Sub
End Class