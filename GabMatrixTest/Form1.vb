Imports GabSoftware.WinControls

Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim dr As DialogResult = CD.ShowDialog
        If dr = Windows.Forms.DialogResult.OK Then
            GabMatrix1.ForeColor = CD.Color
        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim dr As DialogResult = CD.ShowDialog
        If dr = Windows.Forms.DialogResult.OK Then
            GabMatrix1.BackColor = CD.Color
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim dr As DialogResult = CD.ShowDialog
        If dr = Windows.Forms.DialogResult.OK Then
            GabMatrix1.GridColor = CD.Color
        End If
    End Sub

    Private Sub RadioButton1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.LeftToRight
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.RightToLeft
        End If
    End Sub

    Private Sub RadioButton3_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton3.CheckedChanged
        If RadioButton3.Checked = True Then
            GabMatrix1.MoveType = gabMatrix.eMoveType.None
        End If
    End Sub

    Private Sub RadioButton4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton4.CheckedChanged
        If RadioButton4.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.TopToBottom
        End If
    End Sub

    Private Sub RadioButton5_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton5.CheckedChanged
        If RadioButton5.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.BottomToTop
        End If
    End Sub

    Private Sub RadioButton6_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton6.CheckedChanged
        If RadioButton6.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.TopLeftToBottomRight
        End If
    End Sub

    Private Sub RadioButton7_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton7.CheckedChanged
        If RadioButton7.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.TopRightToBottonLeft
        End If
    End Sub

    Private Sub RadioButton8_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton8.CheckedChanged
        If RadioButton8.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.BottomLeftToTopRight
        End If
    End Sub

    Private Sub RadioButton9_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioButton9.CheckedChanged
        If RadioButton9.Checked = True Then
            GabMatrix1.MoveType = GabMatrix.eMoveType.BottomRightToTopLeft
        End If
    End Sub

    Private Sub NumericUpDown1_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles NumericUpDown1.ValueChanged
        GabMatrix1.TimeInterval = NumericUpDown1.Value
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim dr As DialogResult = FD.ShowDialog
        If dr = Windows.Forms.DialogResult.OK Then
            GabMatrix1.Font = FD.Font
            Label3.Text = GabMatrix1.Font.Name
        End If
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        GabMatrix1.MyText = TextBox1.Text
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        TextBox1.Text = GabMatrix1.MyText
        Label3.Text = GabMatrix1.Font.Name
        GabMatrix1.MoveType = GabMatrix.eMoveType.RightToLeft



    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        PictureBox1.Image = GabMatrix1.TextBitmap
    End Sub
End Class
