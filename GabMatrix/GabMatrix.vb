'Imports System
Imports System.Threading
'Imports System.Drawing
'Imports System.Drawing.Imaging
Imports System.ComponentModel

Namespace GabSoftware.WinControls

    Public Class GabMatrix : Inherits UserControl

#Region " Constructors "
        Public Sub New()

            MyBase.New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.Selectable, True)
            Me.SetStyle(ControlStyles.CacheText, True)


            _loaded = False
            _xOffset = 0
            _yOffset = 0

            Me.MoveType = eMoveType.RightToLeft
            If Me.MyText = "" Then
                Me.MyText = "This is a demo text"
            End If

            Me.ForeColor = System.Drawing.Color.DodgerBlue
            Me.GridColor = System.Drawing.Color.Black
            Me.BackColor = System.Drawing.Color.MidnightBlue


        End Sub
#End Region

#Region " Private variables "
        Private _gridColor As System.Drawing.Color
        Private _text As String
        Private _transformedTextWidth As Integer
        Private _transformedTextHeight As Integer
        Private _xOffset As Integer
        Private _yOffset As Integer
        Private _moveType As eMoveType

        Private _thread As Thread
        Private _loaded As Boolean

        Private _adjustingSize As Boolean = False

        Private bgmodel As Graphics
        Private bgfond As Graphics
        Private bgtext As Graphics

        Private btmodel As Bitmap
        Private bttext As Bitmap
        Private btfond As Bitmap


#End Region

#Region " Enums "
        Enum eMoveType
            None = 0
            LeftToRight = 1
            RightToLeft = 2
            TopToBottom = 4
            BottomToTop = 8
            TopLeftToBottomRight = 16
            TopRightToBottonLeft = 32
            BottomLeftToTopRight = 64
            BottomRightToTopLeft = 128
        End Enum
#End Region

#Region " Propriétés "
        ''' <summary>
        ''' Texte à afficher
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Appearance")> Public Property MyText() As String
            Get
                Return _text

            End Get
            Set(ByVal value As String)
                _text = value
                _loaded = False
                Refresh()
            End Set
        End Property

        ''' <summary>
        ''' Couleur du texte
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Appearance")> Public Overrides Property ForeColor() As System.Drawing.Color
            Get
                Return MyBase.ForeColor
            End Get
            Set(ByVal value As System.Drawing.Color)
                _loaded = False
                MyBase.ForeColor = value

            End Set
        End Property
        ''' <summary>
        ''' Couleur de la grille
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Appearance")> Public Property GridColor() As System.Drawing.Color
            Get
                Return _gridColor
            End Get
            Set(ByVal value As System.Drawing.Color)
                _gridColor = value
                _loaded = False
                Refresh()
            End Set
        End Property

        ''' <summary>
        ''' Offset X du texte à afficher
        ''' </summary>
        ''' <value>une valeur entière supérieure ou égale à zéro</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Appearance")> Public Property XOffset() As Integer
            Get
                Return _xOffset
            End Get
            Set(ByVal value As Integer)
                _xOffset = value
                _loaded = False
                Refresh()
            End Set
        End Property

        ''' <summary>
        ''' Offset Y du texte à afficher
        ''' </summary>
        ''' <value>une valeur entière supérieure ou égale zéro</value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Appearance")> Public Property YOffset() As Integer
            Get
                Return _yOffset
            End Get
            Set(ByVal value As Integer)
                _yOffset = value
                _loaded = False
                Refresh()
            End Set
        End Property


        ''' <summary>
        ''' Type du mouvement (None: Aucun mouvement)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Behavior")> Public Property MoveType() As eMoveType
            Get
                Return _moveType
            End Get
            Set(ByVal value As eMoveType)
                _moveType = value
                If value <> eMoveType.None Then
                    MainTimer.Enabled = True
                Else
                    MainTimer.Enabled = False
                End If
            End Set
        End Property

        <Browsable(True)> <Category("Appearance")> Public Overrides Property Font() As System.Drawing.Font
            Get
                Return MyBase.Font
            End Get
            Set(ByVal value As System.Drawing.Font)
                _loaded = False
                MyBase.Font = value
            End Set
        End Property

        ''' <summary>
        ''' Intervalle de temps en millisecondes entre 2 mouvements du texte. Définit la vitesse de l'affichage.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Browsable(True)> <Category("Behavior")> Public Property TimeInterval() As Integer
            Get
                Return MainTimer.Interval
            End Get
            Set(ByVal value As Integer)
                MainTimer.Interval = value
            End Set
        End Property

        <Browsable(False)> Public ReadOnly Property TextBitmap() As Bitmap
            Get
                Return btmodel
            End Get
        End Property

#End Region

#Region " Private methods "
        ''' <summary>
        ''' Dessine le contrôle dans un thread séparé
        ''' </summary>
        ''' <param name="data"></param>
        ''' <remarks></remarks>
        Private Sub ThreadedPaint(ByVal data As Object)
            'redessine dans un thread
            Dim e As System.Windows.Forms.PaintEventArgs = data
            Dim g As Graphics

            g = e.Graphics
            'g.SmoothingMode = Drawing2D.SmoothingMode.None

            If Not _loaded Then Initialisation()

            g.DrawImage(btfond, 0, 0)
            g.DrawImage(bttext, _xOffset, _yOffset)

        End Sub

        ''' <summary>
        ''' Transforme le texte en diodes
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Initialisation()
            Dim bt As New SolidBrush(Me.ForeColor)
            Dim c As System.Drawing.Color
            Dim p As System.Drawing.Pen
            Dim size As SizeF
            Dim i As Integer, j As Integer

            'MsgBox("salut")

            'dessine le fond
            btfond = New Bitmap(Me.Width, Me.Height)
            bgfond = Graphics.FromImage(btfond)
            For i = 0 To Width - 1 Step 3
                bgfond.DrawLine(New System.Drawing.Pen(Me.GridColor), i, 0, i, Height)
            Next
            For i = 0 To Height - 1 Step 3
                bgfond.DrawLine(New System.Drawing.Pen(Me.GridColor), 0, i, Width, i)
            Next

            'dessine le modèle
            btmodel = New Bitmap(Me.Width, Me.Height)
            bgmodel = Graphics.FromImage(btmodel)
            size = bgmodel.MeasureString(Me.MyText, Me.Font) ', 1000000)
            'btmodel = New Bitmap(Convert.ToInt32(size.Width), Me.Height)
            btmodel = New Bitmap(Convert.ToInt32(size.Width), Convert.ToInt32(size.Height))
            bgmodel = Graphics.FromImage(btmodel)
            bgmodel.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit
            bgmodel.DrawString(Me.MyText, Me.Font, bt, 0, 0)
            _transformedTextWidth = Convert.ToInt32(size.Width) * 3
            _transformedTextHeight = Convert.ToInt32(size.Height) * 3


            'dessine le texte
            'bttext = New Bitmap(_transformedTextWidth, Me.Height)
            bttext = New Bitmap(_transformedTextWidth, _transformedTextHeight)

            bgtext = Graphics.FromImage(bttext)
            For i = 0 To size.Width - 1
                For j = 0 To size.Height - 1
                    'p = New System.Drawing.Pen(btmodel.GetPixel(i, j))
                    c = btmodel.GetPixel(i, j)
                    c = IIf(c <> System.Drawing.Color.FromArgb(0, 0, 0, 0), Me.ForeColor, Me.BackColor)
                    p = New System.Drawing.Pen(c)
                    bgtext.DrawRectangle(p, i * 3 + 1, j * 3 + 1, 1, 1)
                Next
            Next

            _loaded = True

        End Sub


        ''' <summary>
        ''' Dessine le contrôle
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub GabMatrix_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

            If (Not _thread Is Nothing) Then
                If (_thread.IsAlive) Then
                    _thread.Abort()
                End If
            End If

            _thread = New Thread(AddressOf ThreadedPaint)
            _thread.IsBackground = True
            _thread.Start(e)
            _thread.Join()


        End Sub

#End Region

#Region " Public methods "
        ''' <summary>
        ''' Ordonne le raffraichissement du contrôle
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Refresh()

            MyBase.Refresh()

        End Sub

#End Region

#Region " Events "
        Private Sub GabMatrix_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Refresh()
        End Sub

        ''' <summary>
        ''' fait bouger le texte
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub MainTimer_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainTimer.Tick

            'If Me.Width < 4 Then
            '    Me.Width = 4
            'Else
            '    If Me.Width Mod 3 <> 1 Then
            '        While Me.Width Mod 3 <> 1
            '            Me.Width -= 1
            '        End While
            '    End If
            'End If

            Select Case Me.MoveType
                Case eMoveType.LeftToRight
                    _xOffset += 3
                    If _xOffset > Me.Width Then
                        _xOffset = -_transformedTextWidth
                    End If

                Case eMoveType.RightToLeft
                    _xOffset -= 3
                    If _xOffset < -_transformedTextWidth Then
                        _xOffset = Me.Width - 1
                    End If

                Case eMoveType.TopToBottom
                    _yOffset += 3
                    If _yOffset > Me.Height Then
                        _yOffset = -_transformedTextHeight
                    End If

                Case eMoveType.BottomToTop
                    _yOffset -= 3
                    If _yOffset < -_transformedTextHeight Then
                        _yOffset = Me.Height - 1
                    End If

                Case eMoveType.BottomLeftToTopRight
                    _xOffset += 3
                    If _xOffset > Me.Width Then
                        _xOffset = -_transformedTextWidth
                    End If
                    _yOffset -= 3
                    If _yOffset < -_transformedTextHeight Then
                        _yOffset = Me.Height - 1
                    End If

                Case eMoveType.BottomRightToTopLeft
                    _xOffset -= 3
                    If _xOffset < -_transformedTextWidth Then
                        _xOffset = Me.Width - 1
                    End If
                    _yOffset -= 3
                    If _yOffset < -_transformedTextHeight Then
                        _yOffset = Me.Height - 1
                    End If

                Case eMoveType.TopLeftToBottomRight
                    _xOffset += 3
                    If _xOffset > Me.Width Then
                        _xOffset = -_transformedTextWidth
                    End If
                    _yOffset += 3
                    If _yOffset > Me.Height Then
                        _yOffset = -_transformedTextHeight
                    End If

                Case eMoveType.TopRightToBottonLeft
                    _xOffset -= 3
                    If _xOffset < -_transformedTextWidth Then
                        _xOffset = Me.Width - 1
                    End If
                    _yOffset += 3
                    If _yOffset > Me.Height Then
                        _yOffset = -_transformedTextHeight
                    End If

                Case Else

            End Select

            Refresh()

        End Sub

        Private Sub GabMatrix_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged

            If _adjustingSize Then
                Exit Sub 'stack overflow protection
            End If

            'on s'assure que la taille est bien (un multiple de 3) + 1
            If Me.Width < 4 Then
                _adjustingSize = True
                Me.Width = 4
                _adjustingSize = False
            Else
                If Me.Width Mod 3 <> 1 Then
                    _adjustingSize = True
                    While Me.Width Mod 3 <> 1
                        Me.Width -= 1
                    End While
                    _adjustingSize = False
                End If
            End If
            If Me.Height < 4 Then
                _adjustingSize = True
                Me.Height = 4
                _adjustingSize = False
            Else
                If Me.Height Mod 3 <> 1 Then
                    _adjustingSize = True
                    While Me.Height Mod 3 <> 1
                        Me.Height -= 1
                    End While
                    _adjustingSize = False
                End If
            End If



            _loaded = False
            Refresh()
        End Sub

#End Region

    End Class

End Namespace